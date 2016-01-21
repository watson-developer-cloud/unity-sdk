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

using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Services.v1;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.DataModels;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

#pragma warning disable 414

namespace IBM.Watson.DeveloperCloud.Widgets
{
    /// <summary>
    /// Natural Language Classifier Widget.
    /// </summary>
	public class NaturalLanguageClassifierWidget : Widget
    {
        #region Private Data
        private NaturalLanguageClassifier m_NLC = new NaturalLanguageClassifier();
        private Classifier m_Selected = null;

        [SerializeField]
        private Input m_RecognizeInput = new Input("Recognize", typeof(SpeechToTextData), "OnRecognize");
        [SerializeField]
        private Output m_ClassifyOutput = new Output(typeof(ClassifyResultData));
        [SerializeField]
        private string m_ClassifierName = string.Empty;
        [SerializeField]
        private string m_ClassifierId = string.Empty;
        [SerializeField, Tooltip("What is the minimum word confidence needed to send onto the NLC?")]
        private double m_MinWordConfidence = 0.4;
        [SerializeField, Tooltip("Recognized speech below this confidence is just ignored.")]
        private double m_IgnoreWordConfidence = 0.2;
        [SerializeField, Tooltip("What is the minimum confidence for a classification event to be fired.")]
        private double m_MinClassEventConfidence = 0.5;
        [SerializeField]
        private string m_Language = "en";

        [Serializable]
        private class ClassEventMapping
        {
            public string m_Class = null;
            public Constants.Event m_Event = Constants.Event.NONE;
        };
        [SerializeField]
        private List<ClassEventMapping> m_ClassEventList = new List<ClassEventMapping>();
        private Dictionary<string, Constants.Event> m_ClassEventMap = new Dictionary<string, Constants.Event>();

        [SerializeField]
        private Text m_TopClassText = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Returns the NLC service object.
        /// </summary>
        public NaturalLanguageClassifier NLC { get { return m_NLC; } }
        #endregion

        #region MonoBehaviour interface
        /// <exclude />
        protected override void Start()
        {
            base.Start();

            // resolve configuration variables
            m_ClassifierName = Config.Instance.ResolveVariables(m_ClassifierName);
            m_ClassifierId = Config.Instance.ResolveVariables(m_ClassifierId);

            if (string.IsNullOrEmpty(m_ClassifierId))
            {
                Log.Status("NlcWidget", "Auto selecting a classifier.");
                if (!m_NLC.GetClassifiers(OnGetClassifiers))
                    Log.Error("NlcWidget", "Failed to request all classifiers.");
            }
            else
            {
                if (!m_NLC.GetClassifier(m_ClassifierId, OnGetClassifier))
                    Log.Equals("NlcWidget", "Failed to request classifier.");
            }
        }

        private void OnEnable()
        {
            EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_DEBUG_COMMAND, OnDebugCommand);
        }
        private void OnDisable()
        {
            EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_DEBUG_COMMAND, OnDebugCommand);
        }
        #endregion

        #region Widget interface
        /// <exclude />
        protected override string GetName()
        {
            return "NLC";
        }
        #endregion

        #region Private Functions
        private void OnGetClassifiers(Classifiers classifiers)
        {
            if (classifiers != null)
            {
                foreach (var classifier in classifiers.classifiers)
                {
                    if (!string.IsNullOrEmpty(m_ClassifierName) && !classifier.name.StartsWith(m_ClassifierName))
                        continue;
                    if (classifier.language != m_Language)
                        continue;

                    m_NLC.GetClassifier(classifier.classifier_id, OnGetClassifier);
                }
            }
        }

        private void OnGetClassifier(Classifier classifier)
        {
            if (classifier != null && classifier.status == "Available")
            {
                if (m_Selected == null || m_Selected.created.CompareTo(classifier.created) < 0)
                {
                    m_Selected = classifier;
                    m_ClassifierId = m_Selected.classifier_id;

                    Log.Status("NlcWidget", "Selected classifier {0}, Created: {1}, Name: {2}",
                        m_Selected.classifier_id, m_Selected.created, m_Selected.name);
                }
            }
        }

        private void OnRecognize(Data data)
        {
            SpeechResultList result = ((SpeechToTextData)data).Results;
            if (result.HasFinalResult())
            {
                string text = result.Results[0].Alternatives[0].Transcript;
                double textConfidence = result.Results[0].Alternatives[0].Confidence;

                Log.Debug("NlcWidget", "OnRecognize: {0} ({1:0.00})", text, textConfidence);
                EventManager.Instance.SendEvent(Constants.Event.ON_DEBUG_MESSAGE, string.Format("{0} ({1:0.00})", text, textConfidence));

                if (textConfidence > m_MinWordConfidence)
                {
                    if (!string.IsNullOrEmpty(m_ClassifierId))
                    {
                        if (!m_NLC.Classify(m_ClassifierId, text, OnClassified))
                            Log.Error("NlcWidget", "Failed to send {0} to NLC.", text);
                    }
                    else
                        Log.Equals("NlcWidget", "No valid classifier set.");
                }
                else
                {
                    if (textConfidence > m_IgnoreWordConfidence)
                        EventManager.Instance.SendEvent(Constants.Event.ON_CLASSIFY_FAILURE, result);
                }
            }
        }

        private void OnClassified(ClassifyResult result)
        {
            EventManager.Instance.SendEvent(Constants.Event.ON_CLASSIFY_RESULT, result);

            if (m_ClassifyOutput.IsConnected)
                m_ClassifyOutput.SendData(new ClassifyResultData(result));

            if (result != null)
            {
                Log.Debug("NlcWidget", "OnClassified: {0} ({1:0.00})", result.top_class, result.topConfidence);

                if (m_TopClassText != null)
                    m_TopClassText.text = result.top_class;

                if (!string.IsNullOrEmpty(result.top_class))
                {
                    if (result.topConfidence >= m_MinClassEventConfidence)
                    {
                        if (m_ClassEventList.Count > 0 && m_ClassEventMap.Count == 0)
                        {
                            // initialize the map
                            foreach (var ev in m_ClassEventList)
                                m_ClassEventMap[ev.m_Class] = ev.m_Event;
                        }

                        Constants.Event sendEvent;
                        if (!m_ClassEventMap.TryGetValue(result.top_class, out sendEvent))
                        {
                            Log.Warning("NlcWidget", "No class mapping found for {0}", result.top_class);
                            EventManager.Instance.SendEvent(result.top_class, result);
                        }
                        else
                            EventManager.Instance.SendEvent(sendEvent, result);
                    }
                    else
                    {
                        if (result.topConfidence > m_IgnoreWordConfidence)
                            EventManager.Instance.SendEvent(Constants.Event.ON_CLASSIFY_FAILURE, result);
                    }
                }
            }
        }
        #endregion

        #region Event Handlers
        private void OnDebugCommand(object[] args)
        {
            string text = args != null && args.Length > 0 ? args[0] as string : string.Empty;
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(m_ClassifierId))
            {
                if (!m_NLC.Classify(m_ClassifierId, text, OnClassified))
                    Log.Error("NlcWidget", "Failed to send {0} to NLC.", (string)args[0]);
            }
        }
        #endregion
    }
}
