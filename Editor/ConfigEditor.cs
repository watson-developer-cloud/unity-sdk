using IBM.Watson.Utilities;
using System;
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

            EditorGUILayout.LabelField( "Credentials" );
            EditorGUI.indentLevel += 1;
            for(int i=0;i<cfg.Credentials.Count;++i)
            {
                Config.CredentialsInfo info = cfg.Credentials[i];

                info.m_ServiceID = EditorGUILayout.TextField( "ServiceID", info.m_ServiceID );
                info.m_User = EditorGUILayout.TextField( "User", info.m_User );
                info.m_Password = EditorGUILayout.TextField( "Password", info.m_Password );

                if ( GUILayout.Button( "Delete" ) )
                    cfg.Credentials.RemoveAt( i-- );
            }

            if ( GUILayout.Button( "Add" ) )
                cfg.Credentials.Add( new Config.CredentialsInfo() );
            EditorGUI.indentLevel -= 1;

            if ( GUILayout.Button( "Save" ) )
                SaveConfig();

            EditorGUILayout.EndScrollView();
        }
    }
}
