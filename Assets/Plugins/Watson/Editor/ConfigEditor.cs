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

using IBM.Watson.Utilities;
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
                cfg.AppKey = EditorGUILayout.TextField( "Application Key", cfg.AppKey );
                cfg.SecretKey = EditorGUILayout.TextField( "Secret Key", cfg.SecretKey );
                EditorGUI.indentLevel -= 1;
            }

            EditorGUILayout.LabelField( "Credentials" );
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
    }
}
