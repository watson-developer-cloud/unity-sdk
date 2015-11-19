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

using IBM.Watson.Utilities;
using IBM.Watson.Services.v1;
using UnityEditor;
using UnityEngine;
using System.IO;
using IBM.Watson.Data;
using IBM.Watson.Logging;

namespace IBM.Watson.Editor
{

    class NLCTrainer : EditorWindow
    {
        private void OnEnable()
        {
#if UNITY_5_2
            titleContent.text = "Watson NLC";
#endif
            m_WatsonIcon = (Texture2D)Resources.Load(Constants.Resources.WATSON_ICON, typeof(Texture2D));
            EditorApplication.update += UpdateRunnable;
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateRunnable;
        }

        static void UpdateRunnable()
        {
            Runnable.Instance.UpdateRoutines();
        }


        [MenuItem("Watson/NLC Trainer")]
        private static void EditConfig()
        {
            GetWindow<NLCTrainer>().Show();
        }

        private Texture m_WatsonIcon = null;
        private Vector2 m_ScrollPos = Vector2.zero;
        private NLC m_NLC = new NLC();
        private Classifiers m_Classifiers = null;
        private string m_NewClassifierName = null;
        private string m_NewClassifierLang = "en";
        private int m_PendingRequests = 0;

        private void OnGetClassifiers(Classifiers classifiers)
        {
            m_PendingRequests -= 1;
            m_Classifiers = classifiers;
            foreach (var c in m_Classifiers.classifiers)
            {
                if ( m_NLC.GetClassifier(c.classifier_id, OnGetClassifier) )
                    m_PendingRequests += 1;
            }
        }

        private void OnGetClassifier(Classifier details)
        {
            m_PendingRequests -= 1;
            foreach (var c in m_Classifiers.classifiers)
                if (c.classifier_id == details.classifier_id)
                {
                    c.status = details.status;
                    c.status_description = details.status_description;
                }
        }

        private void OnDeleteClassifier( bool success )
        {
            if (! success )
                Log.Error( "NLCTrainer", "Failed to delete classifier." );
            else
                OnRefresh();
        }

        private void OnClassiferTrained( Classifier classifier )
        {
            if (classifier == null)
                EditorUtility.DisplayDialog( "ERROR", "Failed to train classifier.", "OK" );
            else
                OnRefresh();
        }

        private void OnRefresh()
        {
            if ( m_PendingRequests == 0 )
            {
                if (!m_NLC.GetClassifiers(OnGetClassifiers))
                    Log.Error( "NLCTrainer", "Failed to request classifiers, please make sure your NlcV1 service has credentials configured." );
                else
                    m_PendingRequests += 1;
            }
        }

        private void OnGUI()
        {
            GUILayout.Label(m_WatsonIcon);

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

            if (m_Classifiers == null || GUILayout.Button("Refresh"))
                OnRefresh();

            EditorGUILayout.LabelField("Classifiers:");
            EditorGUI.indentLevel += 1;
            if (m_Classifiers != null)
            {
                for (int i = 0; i < m_Classifiers.classifiers.Length; ++i)
                {
                    Classifier cl = m_Classifiers.classifiers[i];
                    //EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.TextField("Name", cl.name);
                    EditorGUILayout.TextField("ID", cl.classifier_id);
                    EditorGUILayout.TextField("Status", cl.status);
                    //EditorGUI.EndDisabledGroup();

                    if (GUILayout.Button("Delete"))
                    {
                        if ( EditorUtility.DisplayDialog( "Confirm", string.Format("Confirm delete of classifier {0}", cl.classifier_id), "YES", "NO" )
                            && !m_NLC.DeleteClassifer( cl.classifier_id, OnDeleteClassifier ) )
                        {
                            EditorUtility.DisplayDialog( "Error", "Failed to delete classifier.", "OK" );
                        }
                    }
                }
            }
            EditorGUI.indentLevel -= 1;

            EditorGUILayout.LabelField("Create Classifier:" );
            EditorGUI.indentLevel += 1;

            m_NewClassifierName = EditorGUILayout.TextField("Name", m_NewClassifierName );    
            m_NewClassifierLang = EditorGUILayout.TextField("Language", m_NewClassifierLang );        
            if (! string.IsNullOrEmpty( m_NewClassifierName ) && GUILayout.Button( "Train" ) )
            {
                var path = EditorUtility.OpenFilePanel( "Select Training File", "", "csv" );
                if (! string.IsNullOrEmpty( path ) )
                {
                    string trainingData = File.ReadAllText( path );
                    if (! string.IsNullOrEmpty( trainingData ) )
                    {
                        if (! m_NLC.TrainClassifier( m_NewClassifierName, m_NewClassifierLang, trainingData, OnClassiferTrained ) )
                            EditorUtility.DisplayDialog( "Error", "Failed to train classifier.", "OK" );
                    }
                    else
                        EditorUtility.DisplayDialog( "Error", "Failed to load training data: " + path, "OK" );
                }

                m_NewClassifierName = null;
            }
            EditorGUI.indentLevel -= 1;

            EditorGUILayout.EndScrollView();
        }
    }
}

#endif
