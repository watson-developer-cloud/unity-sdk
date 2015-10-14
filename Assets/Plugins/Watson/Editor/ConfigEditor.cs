/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
* @author Richard Lyle (rolyle@us.ibm.com)
*/

using IBM.Watson.Connection;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace IBM.Watson.Editor
{
    /// <summary>
    /// This class implements a window for editing the Watson configuration settings from Unity.
    /// </summary>
    class ConfigEditor : EditorWindow
    {
        private void OnEnable()
        {
            titleContent.text = "Watson Config";
            m_WatsonIcon = (Texture2D)Resources.Load( "WatsonIcon", typeof(Texture2D) );
        }

        private static void SaveConfig()
        {
            if (!System.IO.Directory.Exists(Application.streamingAssetsPath))
                System.IO.Directory.CreateDirectory(Application.streamingAssetsPath);
            System.IO.File.WriteAllText(Application.streamingAssetsPath + "/Config.json", Config.Instance.SaveConfig());
        }

        [MenuItem("Watson/Edit Config")]
        private static void EditConfig()
        {
            ConfigEditor window = (ConfigEditor)EditorWindow.GetWindow( typeof(ConfigEditor) );
            window.Show();
        }

        private Texture m_WatsonIcon = null;
        private Vector2 m_ScrollPos = Vector2.zero;
#if UNITY_EDITOR
        private string m_GatewayUser = "admin";
        private string m_GatewayPassword = "admin123";
#else
        private string m_GatewayUser = "";
        private string m_GatewayPassword = "";
#endif

        private void OnGUI()
        {
            Config cfg = Config.Instance;

            GUILayout.Label( m_WatsonIcon );

            m_ScrollPos = EditorGUILayout.BeginScrollView( m_ScrollPos );
            cfg.TimeOut = EditorGUILayout.FloatField( "Timeout", cfg.TimeOut );
            cfg.MaxRestConnections = EditorGUILayout.IntField( "Max Connections", cfg.MaxRestConnections );

            cfg.EnableGateway = EditorGUILayout.ToggleLeft( "Enable Gateway", cfg.EnableGateway );
            if ( cfg.EnableGateway )
            {
                EditorGUI.indentLevel += 1;
                cfg.GatewayURL = EditorGUILayout.TextField( "Gateway URL", cfg.GatewayURL );
                m_GatewayUser = EditorGUILayout.TextField( "Gateway User", m_GatewayUser );
                m_GatewayPassword = EditorGUILayout.TextField( "Gateway Password", m_GatewayPassword );

                cfg.CompanyKey = EditorGUILayout.TextField( "Company Key", cfg.CompanyKey );
                if ( GUILayout.Button( "Create Company Key" ) )
                {
                    cfg.CompanyKey = Guid.NewGuid().ToString();

                    Dictionary<string,object> addKeyReq = new Dictionary<string, object>();
                    addKeyReq["robotKey"] = cfg.CompanyKey;
                    addKeyReq["groupName"] = Application.companyName;
                    addKeyReq["deviceLimit"] = "9999";

                    Dictionary<string,string> headers = new Dictionary<string, string>();
                    headers["Authorization"] = new Credentials( m_GatewayUser, m_GatewayPassword ).CreateAuthorization();
                    headers["Content-Type"] = "application/json";

                    byte [] data = Encoding.UTF8.GetBytes( MiniJSON.Json.Serialize( addKeyReq ) );
                    WWW www = new WWW( cfg.GatewayURL + "/v1/admin/addKey", data, headers );
                    while(! www.isDone );

                    if (! string.IsNullOrEmpty( www.error ) )
                        Log.Warning( "ConfigEditor", "Register App Error: {0}", www.error );

                    bool bRegistered = false;
                    if (! string.IsNullOrEmpty( www.text ) )
                    {
                        IDictionary json = MiniJSON.Json.Deserialize( www.text ) as IDictionary;
                        if ( json.Contains( "status" ) )
                            bRegistered = (long)json["status"] != 0;
                    }

                    if (! bRegistered)
                    {
                        Config.Instance.CompanyKey = string.Empty;
                        EditorUtility.DisplayDialog( "Error", "Failed to register company with gateway.", "OK" );
                    }
                }

                if (! string.IsNullOrEmpty( cfg.CompanyKey ) )
                {
                    cfg.ProductKey = EditorGUILayout.TextField( "Product Key", cfg.ProductKey );

                    if ( GUILayout.Button( "Create Product Key" ) )
                    {
                        cfg.ProductKey = Guid.NewGuid().ToString();

                        Dictionary<string,object> registerReq = new Dictionary<string, object>();
                        registerReq["robotKey"] = cfg.CompanyKey;
                        registerReq["robotName"] = Application.productName;
                        registerReq["macId" ] = cfg.ProductKey;

                        Dictionary<string,string> headers = new Dictionary<string, string>();
                        headers["Authorization"] = new Credentials( m_GatewayUser, m_GatewayPassword ).CreateAuthorization();
                        headers["Content-Type"] = "application/json";

                        byte [] data = Encoding.UTF8.GetBytes( MiniJSON.Json.Serialize( registerReq ) );
                        WWW www = new WWW( cfg.GatewayURL + "/v1/admin/addRobot", data, headers );
                        while(! www.isDone );

                        if (! string.IsNullOrEmpty( www.error ) )
                            Log.Warning( "ConfigEditor", "Register Secret Error: {0}", www.error );

                        bool bRegistered = false;
                        if (! string.IsNullOrEmpty( www.text ) )
                        {
                            IDictionary json = MiniJSON.Json.Deserialize( www.text ) as IDictionary;
                            if ( json.Contains( "status" ) )
                                bRegistered = (long)json["status"] != 0;
                        }

                        if (! bRegistered)
                        {
                            Config.Instance.ProductKey = string.Empty;
                            EditorUtility.DisplayDialog( "Error", "Failed to register product with gateway.", "OK" );
                        }
                    }
                }

                EditorGUI.indentLevel -= 1;
            }

            EditorGUILayout.LabelField( "BlueMix Credentials" );
            EditorGUI.indentLevel += 1;
            for(int i=0;i<cfg.Credentials.Count;++i)
            {
                Config.BlueMixCred info = cfg.Credentials[i];

                info.m_ServiceID = EditorGUILayout.TextField( "ServiceID", info.m_ServiceID );
                info.m_URL = EditorGUILayout.TextField( "URL", info.m_URL );
                info.m_User = EditorGUILayout.TextField( "User", info.m_User );
                info.m_Password = EditorGUILayout.TextField( "Password", info.m_Password );

                if ( GUILayout.Button( "Delete" ) )
                    cfg.Credentials.RemoveAt( i-- );
            }

            if ( GUILayout.Button( "Add" ) )
                cfg.Credentials.Add( new Config.BlueMixCred() );
            EditorGUI.indentLevel -= 1;

            if ( GUILayout.Button( "Save" ) )
                SaveConfig();

            EditorGUILayout.EndScrollView();
        }

        private void OnRegisterCompany( RESTConnector.Request req, RESTConnector.Response resp )
        {
            if ( !resp.Success )
            {
            }
        }
        private void OnRegisterProduct( RESTConnector.Request req, RESTConnector.Response resp )
        {
            if ( !resp.Success )
            {
                Config.Instance.ProductKey = string.Empty;
                EditorUtility.DisplayDialog( "Error", "Failed to register Product with gateway.", "OK" );
            }
        }
    }
}
