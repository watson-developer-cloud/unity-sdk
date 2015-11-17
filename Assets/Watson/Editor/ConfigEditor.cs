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

#if UNITY_EDITOR

using IBM.Watson.Connection;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        #region Constants
        private const string BLUEMIX_REGISTRATION = "https://console.ng.bluemix.net/registration/";

        private class ServiceSetup
        {
            public string ServiceName;
            public string ServiceAPI;
            public string URL;
            public string ServiceID;
        };

        private ServiceSetup [] SERVICE_SETUP = new ServiceSetup[]
        {
            new ServiceSetup() { ServiceName = "Speech To Text", ServiceAPI = "speech-to-text/api",
                URL ="https://console.ng.bluemix.net/catalog/speech-to-text/", ServiceID="SpeechToTextV1" },
            new ServiceSetup() { ServiceName = "Text To Speech", ServiceAPI = "text-to-speech/api",
                URL ="https://console.ng.bluemix.net/catalog/text-to-speech/", ServiceID="TextToSpeechV1" },
            new ServiceSetup() { ServiceName = "Dialog", ServiceAPI = "dialog/api",
                URL ="https://console.ng.bluemix.net/catalog/dialog/", ServiceID="DialogV1" },
            new ServiceSetup() { ServiceName = "Translation", ServiceAPI = "language-translation/api",
                URL ="https://console.ng.bluemix.net/catalog/language-translation/", ServiceID="TranslateV1" },
            new ServiceSetup() { ServiceName = "Natural Language Classifier", ServiceAPI = "natural-language-classifier/api",
                URL ="https://console.ng.bluemix.net/catalog/natural-language-classifier/", ServiceID="NlcV1" }
        };

        private const string TITLE = "Watson Unity SDK";
        private const string RUN_WIZARD_MSG =  "Thanks for installing the Watson Unity SDK, would you like to configure your credentials?";
        private const string YES = "Yes";
        private const string NO = "No";
        private const string OK = "Okay";
        #endregion

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if ( !File.Exists( Application.streamingAssetsPath + Constants.Path.CONFIG_FILE ) )
            {
                if ( EditorUtility.DisplayDialog( TITLE, RUN_WIZARD_MSG, YES, NO ) )
                {
                    PlayerPrefs.SetInt("WizardMode", 1 );
                    EditConfig();
                }
            }
        }
                                                        
        private void OnEnable()
        {
#if UNITY_5_2
            titleContent.text = "Watson Config";
#endif
            m_WatsonIcon = (Texture2D)Resources.Load(Constants.Resources.WATSON_ICON, typeof(Texture2D));
            m_WizardMode = PlayerPrefs.GetInt( "WizardMode", 1 ) != 0;
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
            ConfigEditor window = (ConfigEditor)EditorWindow.GetWindow(typeof(ConfigEditor));
            window.Show();
        }

        private delegate void WizardStepDelegate( ConfigEditor editor );

        private bool m_WizardMode = true;
        private Texture m_WatsonIcon = null;
        private Vector2 m_ScrollPos = Vector2.zero;
        private string m_PastedCredentials = "\n\n\n\n\n\n\n";

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

            GUILayout.Label(m_WatsonIcon);

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            if ( m_WizardMode )
            {
                if ( GUILayout.Button( "Advanced Mode" ) )
                {
                    m_WizardMode = false;
                    PlayerPrefs.SetInt( "WizardMode", 0 );
                }

                //GUILayout.Label( "Use this dialog to generate your configuration file for the Watson Unity SDK." );
                //GUILayout.Label( "If you have never registered for Watson BlueMix services, click on the button below to begin registration." );

                if ( GUILayout.Button( "Register for Watson Services" ) )
                    Application.OpenURL( BLUEMIX_REGISTRATION );

                foreach( var setup in SERVICE_SETUP )
                {
                    Config.CredentialInfo info = cfg.FindCredentials( setup.ServiceID );

                    bool bValid = info != null 
                        && !string.IsNullOrEmpty( info.m_URL )
                        && !string.IsNullOrEmpty( info.m_User )
                        && !string.IsNullOrEmpty( info.m_Password );

                    GUILayout.BeginHorizontal();

                    GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
                    labelStyle.normal.textColor = bValid ? Color.green : Color.red; 

                    GUILayout.Label( string.Format( "Service {0} {1}...", setup.ServiceName, bValid ? "Configured" : "NOT CONFIGURED" ), labelStyle );

                    if ( GUILayout.Button( "Configure", GUILayout.Width( 100 ) ) )
                        Application.OpenURL( setup.URL );
                    if ( bValid && GUILayout.Button( "Clear", GUILayout.Width( 100 ) ) )
                        cfg.Credentials.Remove( info );

                    GUILayout.EndHorizontal();
                }

                GUILayout.Label( "PASTE CREDENTIALS BELOW:" );
                m_PastedCredentials = EditorGUILayout.TextArea( m_PastedCredentials );
                    
                GUI.SetNextControlName("Apply");
                if ( GUILayout.Button( "Apply Credentials" ) )
                {
                    bool bParsed = false;

                    Config.CredentialInfo newInfo = new Config.CredentialInfo();
                    if ( newInfo.ParseJSON( m_PastedCredentials ) )
                    {
                        foreach (var setup in SERVICE_SETUP)
                        {
                            if (newInfo.m_URL.EndsWith(setup.ServiceAPI))
                            {
                                newInfo.m_ServiceID = setup.ServiceID;

                                bool bAdd = true;
                                // remove any previous credentials with the same service ID
                                for( int i=0;i<cfg.Credentials.Count;++i)
                                    if ( cfg.Credentials[i].m_ServiceID == newInfo.m_ServiceID )
                                    {
                                        bAdd = false;

                                        if ( EditorUtility.DisplayDialog( "Confirm", 
                                            string.Format("Replace existing service credentials for {0}?", setup.ServiceName),
                                            YES, NO ) )
                                        {
                                            cfg.Credentials.RemoveAt(i);
                                            bAdd = true;
                                            break;
                                        }
                                    }

                                if ( bAdd )
                                    cfg.Credentials.Add( newInfo );
                                bParsed = true;
                            }
                       }
                    }

                    if ( bParsed )
                    {
                        EditorUtility.DisplayDialog( "Complete", "Credentials applied.", OK );
                        m_PastedCredentials = "\n\n\n\n\n\n\n";
                        GUI.FocusControl("Apply");

                        SaveConfig();
                    }
                    else
                        EditorUtility.DisplayDialog( "Error", "Failed to parse credentials:\n" + m_PastedCredentials, OK );
                }

                if ( GUILayout.Button( "Save" ) )
                    SaveConfig();
            } 
            else
            {
                if ( GUILayout.Button( "Basic Mode" ) )
                {
                    m_WizardMode = true;
                    PlayerPrefs.SetInt( "WizardMode", 1 );
                }

                cfg.TimeOut = EditorGUILayout.FloatField("Timeout", cfg.TimeOut);
                cfg.MaxRestConnections = EditorGUILayout.IntField("Max Connections", cfg.MaxRestConnections);

                cfg.EnableGateway = EditorGUILayout.ToggleLeft("Enable Gateway", cfg.EnableGateway);
                if (cfg.EnableGateway)
                {
                    EditorGUI.indentLevel += 1;
                    cfg.GatewayURL = EditorGUILayout.TextField("Gateway URL", cfg.GatewayURL);
                    m_GatewayUser = EditorGUILayout.TextField("Gateway User", m_GatewayUser);
                    m_GatewayPassword = EditorGUILayout.PasswordField("Gateway Password", m_GatewayPassword);

                    cfg.ProductKey = EditorGUILayout.TextField("Product Key", cfg.ProductKey);
                    if (GUILayout.Button("Create Product Key")
                        && (string.IsNullOrEmpty(cfg.ProductKey) || EditorUtility.DisplayDialog("Confirm", "Please confirm you replacing your current key.", "Yes", "No")))
                    {
                        cfg.ProductKey = Guid.NewGuid().ToString();

                        Dictionary<string, object> addKeyReq = new Dictionary<string, object>();
                        addKeyReq["robotKey"] = cfg.ProductKey;
                        addKeyReq["groupName"] = Application.productName;
                        addKeyReq["deviceLimit"] = "9999";

                        Dictionary<string, string> headers = new Dictionary<string, string>();
                        headers["Authorization"] = new Credentials(m_GatewayUser, m_GatewayPassword).CreateAuthorization();
                        headers["Content-Type"] = "application/json";

                        byte[] data = Encoding.UTF8.GetBytes(MiniJSON.Json.Serialize(addKeyReq));
                        WWW www = new WWW(cfg.GatewayURL + "/v1/admin/addKey", data, headers);
                        while (!www.isDone) ;

                        if (!string.IsNullOrEmpty(www.error))
                            Log.Warning("ConfigEditor", "Register App Error: {0}", www.error);

                        bool bRegistered = false;
                        if (!string.IsNullOrEmpty(www.text))
                        {
                            IDictionary json = MiniJSON.Json.Deserialize(www.text) as IDictionary;
                            if ( json != null && json.Contains("status"))
                                bRegistered = (long)json["status"] != 0;
                            else
                                Log.Error( "ConfigEditor", "Invalid response from gateway: {0}", www.text );
                        }

                        if (bRegistered)
                        {
                            Dictionary<string, object> registerReq = new Dictionary<string, object>();
                            registerReq["robotKey"] = cfg.ProductKey;
                            registerReq["robotName"] = Application.productName;
                            registerReq["macId"] = "UnitySDK";

                            data = Encoding.UTF8.GetBytes(MiniJSON.Json.Serialize(registerReq));
                            www = new WWW(cfg.GatewayURL + "/v1/admin/addRobot", data, headers);
                            while (!www.isDone) ;

                            if (!string.IsNullOrEmpty(www.error))
                                Log.Warning("ConfigEditor", "Register Secret Error: {0}", www.error);

                            bRegistered = false;
                            if (!string.IsNullOrEmpty(www.text))
                            {
                                IDictionary json = MiniJSON.Json.Deserialize(www.text) as IDictionary;
                                if (json.Contains("status"))
                                    bRegistered = (long)json["status"] != 0;
                            }
                        }

                        if (!bRegistered)
                        {
                            Config.Instance.ProductKey = string.Empty;
                            EditorUtility.DisplayDialog("Error", "Failed to register product with gateway.", "OK");
                        }
                    }

                    EditorGUI.indentLevel -= 1;
                }

                EditorGUILayout.LabelField("BlueMix Credentials");
                EditorGUI.indentLevel += 1;
                for (int i = 0; i < cfg.Credentials.Count; ++i)
                {
                    Config.CredentialInfo info = cfg.Credentials[i];

                    info.m_ServiceID = EditorGUILayout.TextField("ServiceID", info.m_ServiceID);
                    info.m_URL = EditorGUILayout.TextField("URL", info.m_URL);
                    info.m_User = EditorGUILayout.TextField("User", info.m_User);
                    info.m_Password = EditorGUILayout.TextField("Password", info.m_Password);

                    if (GUILayout.Button("Delete"))
                        cfg.Credentials.RemoveAt(i--);
                }

                if (GUILayout.Button("Add"))
                    cfg.Credentials.Add(new Config.CredentialInfo());
                EditorGUI.indentLevel -= 1;

                if (GUILayout.Button("Save"))
                    SaveConfig();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}

#endif
