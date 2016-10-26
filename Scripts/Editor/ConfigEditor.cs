#if UNITY_EDITOR
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
*/

using IBM.Watson.DeveloperCloud.Services;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Connection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.Editor
{
    /// <summary>
    /// This class implements a window for editing the Watson configuration settings from Unity.
    /// </summary>
    class ConfigEditor : EditorWindow
    {
        #region Constants
		private const string BLUEMIX_REGISTRATION = "http://bluemix.net/registration";
        private const string API_REFERENCE = "WatsonUnitySDK.chm";
        private const string README = "https://github.com/watson-developer-cloud/unity-sdk/blob/develop/README.md";

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
            new ServiceSetup() { ServiceName = "Language Translation (to be deprecated)", ServiceAPI = "language-translation/api",
                URL ="https://console.ng.bluemix.net/catalog/services/language-translation/", ServiceID="LanguageTranslationV1" },
            //new ServiceSetup() { ServiceName = "Language Translator", ServiceAPI = "language-translator/api",
            //    URL ="https://console.ng.bluemix.net/catalog/services/language-translator/", ServiceID="LanguageTranslatorV1" },
            new ServiceSetup() { ServiceName = "Natural Language Classifier", ServiceAPI = "natural-language-classifier/api",
				URL ="https://console.ng.bluemix.net/catalog/natural-language-classifier/", ServiceID="NaturalLanguageClassifierV1" },
            new ServiceSetup() { ServiceName = "Tone Analyzer", ServiceAPI = "tone-analyzer/api",
                URL ="https://console.ng.bluemix.net/catalog/services/tone-analyzer/", ServiceID="ToneAnalyzerV3" },
            new ServiceSetup() { ServiceName = "Tradeoff Analytics", ServiceAPI = "tradeoff-analytics/api",
                URL ="https://console.ng.bluemix.net/catalog/services/tradeoff-analytics/", ServiceID="TradeoffAnalyticsV1" },
            new ServiceSetup() { ServiceName = "Personality Insights V2", ServiceAPI = "personality-insights/api",
                URL ="https://console.ng.bluemix.net/catalog/services/personality-insights/", ServiceID="PersonalityInsightsV2" },
			new ServiceSetup() { ServiceName = "Personality Insights V3", ServiceAPI = "personality-insights/api",
				URL ="https://console.ng.bluemix.net/catalog/services/personality-insights/", ServiceID="PersonalityInsightsV3" },
			new ServiceSetup() { ServiceName = "Conversation", ServiceAPI = "conversation/api",
                URL ="https://console.ng.bluemix.net/catalog/services/conversation/", ServiceID="ConversationV1" },
            new ServiceSetup() { ServiceName = "RetrieveAndRank", ServiceAPI = "retrieve-and-rank/api",
                URL ="https://console.ng.bluemix.net/catalog/services/retrieve-and-rank/", ServiceID="RetrieveAndRankV1" },
            new ServiceSetup() { ServiceName = "Alchemy API", ServiceAPI = "gateway-a.watsonplatform.net/calls",
                URL ="https://console.ng.bluemix.net/catalog/services/alchemyapi/", ServiceID="AlchemyAPIV1" },
            new ServiceSetup() { ServiceName = "Visual Recognition", ServiceAPI = "visual-recognition/api",
                URL ="https://console.ng.bluemix.net/catalog/services/visual-recognition/", ServiceID="VisualRecognitionV3" },
            new ServiceSetup() { ServiceName = "Document Conversion", ServiceAPI = "document-conversion/api",
                URL ="https://console.ng.bluemix.net/catalog/services/document-conversion/", ServiceID="DocumentConversionV1" }
        };

        private const string TITLE = "Watson Unity SDK";
        private const string RUN_WIZARD_MSG =  "Thanks for installing the Watson Unity SDK, would you like to configure your credentials?";
        private const string YES = "Yes";
        private const string NO = "No";
        private const string OK = "Okay";

        private IWatsonService [] m_Services = null;
        private Dictionary<string,bool> m_ServiceStatus = new Dictionary<string,bool>();
        private int m_CheckServiceRoutine = 0;
        private bool m_CheckServicesNow = false;
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
#if UNITY_5
            titleContent.text = "Config Editor";
#endif
            m_WatsonIcon = (Texture2D)Resources.Load(Constants.Resources.WATSON_ICON, typeof(Texture2D));
            m_StatusUnknown = (Texture2D)Resources.Load( "status_unknown", typeof(Texture2D));
            m_StatusDown = (Texture2D)Resources.Load( "status_down", typeof(Texture2D));
            m_StatusUp = (Texture2D)Resources.Load( "status_up", typeof(Texture2D));
            m_WizardMode = PlayerPrefs.GetInt( "WizardMode", 1 ) != 0;

            Runnable.EnableRunnableInEditor();
            m_CheckServiceRoutine = Runnable.Run( CheckServices() );
        }

        private void OnDisable()
        {
            if ( m_CheckServiceRoutine != 0 )
            {
                Runnable.Stop( m_CheckServiceRoutine );
                m_CheckServiceRoutine = 0;
            }
        }

        private IEnumerator CheckServices()
        {
            yield return null;
            m_Services = ServiceHelper.GetAllServices();

            DateTime lastCheck = DateTime.Now;

            while( true )
            {
                foreach( var service in m_Services )
                    service.GetServiceStatus( OnServiceStatus );

                while( (DateTime.Now - lastCheck).TotalSeconds < 60.0f && !m_CheckServicesNow )
                    yield return null;
                lastCheck = DateTime.Now;
                m_CheckServicesNow = false;
            }
        }

        private void OnServiceStatus( string serviceID, bool active )
        {
            //Log.Debug( "ConfigEditor", "Service Status for {0} is {1}.", serviceID, active ? "up" : "down" );
            m_ServiceStatus[ serviceID ] = active;
        }

        private static void SaveConfig()
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
               Directory.CreateDirectory(Application.streamingAssetsPath);
            File.WriteAllText(Application.streamingAssetsPath + "/Config.json", Config.Instance.SaveConfig());
            RESTConnector.FlushConnectors();
        }

        private static string FindFile( string directory, string name )
        {
            foreach( var f in Directory.GetFiles( directory ) )
                if ( f.EndsWith( name ) )
                    return f;

            foreach( var d in Directory.GetDirectories( directory ) )
            {
                string found = FindFile( d, name );
                if ( found != null )
                    return found;
            }

            return null;
        }

        [MenuItem("Watson/API Reference", false, 100 )]
        private static void ShowAPIReference()
        {
            Application.OpenURL( "file://" + FindFile( Application.dataPath, API_REFERENCE ) );
        }
        [MenuItem("Watson/Getting Started", false, 100 )]
        private static void ShowReadme()
        {
            Application.OpenURL( README );
        }

        [MenuItem("Watson/Configuration Editor", false, 0 )]
        private static void EditConfig()
        {
            GetWindow<ConfigEditor>().Show();
        }

        private delegate void WizardStepDelegate( ConfigEditor editor );

        private bool m_WizardMode = true;
        private Texture m_WatsonIcon = null;
        private Texture m_StatusUnknown = null;
        private Texture m_StatusUp = null;
        private Texture m_StatusDown = null;
        private Vector2 m_ScrollPos = Vector2.zero;
        private string m_PastedCredentials = "\n\n\n\n\n\n\n";

        private bool GetIsValid(ServiceSetup setup)
        {
            bool isValid = false;
            Config cfg = Config.Instance;
            Config.CredentialInfo info = cfg.FindCredentials( setup.ServiceID );
            if(info != null)
            {
                if((!string.IsNullOrEmpty(info.m_URL) && !string.IsNullOrEmpty(info.m_Password)) || !string.IsNullOrEmpty(info.m_Apikey))
                    isValid = true;
            }

            return isValid;
        }

        private void OnGUI()
        {
            Config cfg = Config.Instance;

            GUILayout.Label(m_WatsonIcon);

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            if ( m_WizardMode )
            {
                //GUILayout.Label( "Use this dialog to generate your configuration file for the Watson Unity SDK." );
                //GUILayout.Label( "If you have never registered for Watson BlueMix services, click on the button below to begin registration." );

                if(GUILayout.Button("Register for Watson Services"))
                    Application.OpenURL( BLUEMIX_REGISTRATION );

                foreach( var setup in SERVICE_SETUP )
                {
                    Config.CredentialInfo info = cfg.FindCredentials( setup.ServiceID );

                    bool bValid = GetIsValid(setup);

                    GUILayout.BeginHorizontal();

                    if ( m_ServiceStatus.ContainsKey( setup.ServiceID ) )
                    {
                        if ( m_ServiceStatus[setup.ServiceID] )
                            GUILayout.Label( m_StatusUp, GUILayout.Width( 20 ) );
                        else
                            GUILayout.Label( m_StatusDown, GUILayout.Width( 20 ) );
                    }
                    else
                        GUILayout.Label( m_StatusUnknown, GUILayout.Width( 20 ) );

                    GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
                    labelStyle.normal.textColor = bValid ? Color.green : Color.grey;

                    GUILayout.Label( string.Format( "Service {0} {1}.", setup.ServiceName, bValid ? "CONFIGURED" : "NOT CONFIGURED" ), labelStyle );

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
                        m_CheckServicesNow = true;

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

                if ( GUILayout.Button( "Advanced Mode" ) )
                {
                    m_WizardMode = false;
                    PlayerPrefs.SetInt( "WizardMode", 0 );
                }
            }
            else
            {
                cfg.ClassifierDirectory = EditorGUILayout.TextField("Classifier Directory", cfg.ClassifierDirectory );
                cfg.TimeOut = EditorGUILayout.FloatField("Timeout", cfg.TimeOut);
                cfg.MaxRestConnections = EditorGUILayout.IntField("Max Connections", cfg.MaxRestConnections);

                EditorGUILayout.LabelField("BlueMix Credentials");
                EditorGUI.indentLevel += 1;
                for (int i = 0; i < cfg.Credentials.Count; ++i)
                {
                    Config.CredentialInfo info = cfg.Credentials[i];

                    GUILayout.BeginHorizontal();
                    info.m_ServiceID = EditorGUILayout.TextField("ServiceID", info.m_ServiceID);

                    if ( !string.IsNullOrEmpty(info.m_ServiceID) && m_ServiceStatus.ContainsKey( info.m_ServiceID ) )
                    {
                        if ( m_ServiceStatus[info.m_ServiceID] )
                            GUILayout.Label( m_StatusUp, GUILayout.Width( 20 ) );
                        else
                            GUILayout.Label( m_StatusDown, GUILayout.Width( 20 ) );
                    }
                    else
                        GUILayout.Label( m_StatusUnknown, GUILayout.Width( 20 ) );
                    GUILayout.EndHorizontal();

                    info.m_URL = EditorGUILayout.TextField("URL", info.m_URL);

                    if(!string.IsNullOrEmpty(info.m_URL))
                    {
                        if(info.m_URL.StartsWith("https://gateway-a"))
                            info.m_Apikey = EditorGUILayout.TextField("API Key", info.m_Apikey);
                        else
                        {
                            info.m_User = EditorGUILayout.TextField("User", info.m_User);
                            info.m_Password = EditorGUILayout.TextField("Password", info.m_Password);
                        }
                    }

                    if (GUILayout.Button("Delete"))
                        cfg.Credentials.RemoveAt(i--);
                }

                if (GUILayout.Button("Add Credential"))
                    cfg.Credentials.Add(new Config.CredentialInfo());
                EditorGUI.indentLevel -= 1;

                EditorGUILayout.LabelField("Variables");
                EditorGUI.indentLevel += 1;
                for (int i = 0; i < cfg.Variables.Count; ++i)
                {
                    Config.Variable variable = cfg.Variables[i];

                    GUILayout.BeginHorizontal();

                    variable.Key = EditorGUILayout.TextField( variable.Key );
                    EditorGUILayout.LabelField( "=", GUILayout.Width( 30 ) );
                    variable.Value = EditorGUILayout.TextField( variable.Value );

                    if (GUILayout.Button("Delete", GUILayout.Width( 100 ) ))
                        cfg.Variables.RemoveAt(i--);

                    GUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Add Variable"))
                    cfg.Variables.Add(new Config.Variable());
                EditorGUI.indentLevel -= 1;

                if (GUILayout.Button("Save"))
                {
                    m_CheckServicesNow = true;
                    SaveConfig();
                }

                if ( GUILayout.Button( "Basic Mode" ) )
                {
                    m_WizardMode = true;
                    PlayerPrefs.SetInt( "WizardMode", 1 );
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }
}

#endif
