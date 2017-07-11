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

//#define ENABLE_DEBUGGING

using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Logging;
using UnityEngine;
using UnityEngine.UI;
using IBM.Watson.DeveloperCloud.Utilities;

#pragma warning disable 414

namespace IBM.Watson.DeveloperCloud.Widgets
{
    /// <summary>
    /// SpeechToText Widget that wraps the SpeechToText service.
    /// </summary>
    public class SpeechToTextWidget : Widget
    {
        #region Inputs
        [SerializeField]
        private Input m_AudioInput = new Input("Audio", typeof(AudioData), "OnAudio");
        [SerializeField]
        private Input m_LanguageInput = new Input("Language", typeof(LanguageData), "OnLanguage");
        #endregion

        #region Outputs
        [SerializeField]
        private Output m_ResultOutput = new Output(typeof(SpeechToTextData), true);
        #endregion

        #region Private Data
        private SpeechToText _speechToText;
        [SerializeField]
        private Text _statusText = null;
        [SerializeField]
        private bool _detectSilence = true;
        [SerializeField]
        private float _silenceThreshold = 0.03f;
        [SerializeField]
        private bool _wordConfidence = false;
        [SerializeField]
        private bool _timeStamps = false;
        [SerializeField]
        private int _maxAlternatives = 1;
        [SerializeField]
        private bool _enableContinous = false;
        [SerializeField]
        private bool _enableInterimResults = false;
        [SerializeField]
        private Text _transcript = null;
        [SerializeField, Tooltip("Language ID to use in the speech recognition model.")]
        private string _language = "en-US";
        #endregion

        #region Public Properties
        /// <summary>
        /// This property starts or stop's this widget listening for speech.
        /// </summary>
        public bool Active
        {
            get { return _speechToText.IsListening; }
            set
            {
                if (value && !_speechToText.IsListening)
                {
                    _speechToText.DetectSilence = _detectSilence;
                    _speechToText.EnableWordConfidence = _wordConfidence;
                    _speechToText.EnableTimestamps = _timeStamps;
                    _speechToText.SilenceThreshold = _silenceThreshold;
                    _speechToText.MaxAlternatives = _maxAlternatives;
                    _speechToText.EnableContinousRecognition = _enableContinous;
                    _speechToText.EnableInterimResults = _enableInterimResults;
                    _speechToText.OnError = OnError;
                    _speechToText.StartListening(OnRecognize);
                    if (_statusText != null)
                        _statusText.text = "LISTENING";
                }
                else if (!value && _speechToText.IsListening)
                {
                    _speechToText.StopListening();
                    if (_statusText != null)
                        _statusText.text = "READY";
                }
            }
        }
        #endregion

        #region Widget Interface
        /// <exclude />
        protected override string GetName()
        {
            return "SpeechToText";
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Button handler to toggle the active state of this widget.
        /// </summary>
        public void OnListenButton()
        {
            Active = !Active;
        }

        /// <exclude />
        protected override void Start()
        {
            base.Start();

            if (_statusText != null)
                _statusText.text = "READY";
            if (!_speechToText.GetModels(OnGetModels))
                Log.Error("SpeechToTextWidget", "Failed to request models.");
        }

        private void OnDisable()
        {
            if (Active)
                Active = false;
        }

        private void OnError(string error)
        {
            Active = false;
            if (_statusText != null)
                _statusText.text = "ERROR: " + error;
        }

        private void OnAudio(Data data)
        {
            if (!Active)
                Active = true;

            _speechToText.OnListen((AudioData)data);
        }

        private void OnLanguage(Data data)
        {
            LanguageData language = data as LanguageData;
            if (language == null)
                throw new WatsonException("Unexpected data type");

            if (!string.IsNullOrEmpty(language.Language))
            {
                _language = language.Language;

                if (!_speechToText.GetModels(OnGetModels))
                    Log.Error("SpeechToTextWidget", "Failed to rquest models.");
            }
        }

        private void OnGetModels(ModelSet models, string customData)
        {
            if (models != null)
            {
                Model bestModel = null;
                foreach (var model in models.models)
                {
                    if (model.language.StartsWith(_language)
                        && (bestModel == null || model.rate > bestModel.rate))
                    {
                        bestModel = model;
                    }
                }

                if (bestModel != null)
                {
                    Log.Status("SpeechToTextWidget", "Selecting Recognize Model: {0} ", bestModel.name);
                    _speechToText.RecognizeModel = bestModel.name;
                }
            }
        }

        private void OnRecognize(SpeechRecognitionEvent result)
        {
            m_ResultOutput.SendData(new SpeechToTextData(result));

            if (result != null && result.results.Length > 0)
            {
                if (_transcript != null)
                    _transcript.text = "";

                foreach (var res in result.results)
                {
                    foreach (var alt in res.alternatives)
                    {
                        string text = alt.transcript;

                        if (_transcript != null)
                            _transcript.text += string.Format("{0} ({1}, {2:0.00})\n",
                                text, res.final ? "Final" : "Interim", alt.confidence);
                    }
                }
            }
        }
        #endregion
    }
}
