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

using IBM.Watson.Services.v1;
using IBM.Watson.Logging;
using UnityEngine;
using UnityEngine.UI;
using IBM.Watson.Utilities;
using IBM.Watson.Data;

#pragma warning disable 414

namespace IBM.Watson.Widgets
{
    /// <summary>
    /// Natural Language Classifier Widget.
    /// </summary>
	public class NlcWidget : Widget
	{
	    #region Private Data
	    private NLC m_NLC = new NLC();

        [SerializeField]
        private Input m_RecognizeInput = new Input( "Recognize", typeof(SpeechToTextData), "OnRecognize" );
        [SerializeField]
        private Output m_TopClassOutput = new Output( typeof(TextData) );
        [SerializeField]
        private Output m_ClassifyOutput = new Output( typeof(ClassifyResultData) );
        [SerializeField]
        private Output m_RecognizeFailure = new Output( typeof(SpeechToTextData) );
        [SerializeField]
        private string m_ClassifierId = "5E00F7x2-nlc-540";     // default to XRAY classifier
        [SerializeField, Tooltip("What is the minimum word confidence needed to send onto the NLC?")]
        private double m_MinWordConfidence = 0.4;
        [SerializeField, Tooltip("Recognized speech below this confidence is just ignored.")]
        private double m_IgnoreWordConfidence = 0.2;
        [SerializeField, Tooltip( "If true, then the top class is sent as a event.") ]
        private bool m_SendAsEvent = false;
        [SerializeField]
        private Text m_TopClassText = null;
        #endregion

        #region Public Properties
        public NLC NLC { get { return m_NLC; } }
        #endregion

        public void ClassifyText( string text )
        {
            if (!m_NLC.Classify(m_ClassifierId, text, OnClassified))
                Log.Error("AvatarWidget", "Failed to send {0} to NLC.", text);
        }

        #region MonoBehaviour interface
        /// <exclude />
        protected override void Start()
	    {
            base.Start();
	        Logger.InstallDefaultReactors();
	    }
        #endregion

        #region Widget interface
        /// <exclude />
        protected override string GetName()
        {
            return "NLC";
        }
        #endregion

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
                    if (!m_NLC.Classify(m_ClassifierId, text, OnClassified))
                        Log.Error("AvatarWidget", "Failed to send {0} to NLC.", text);
                }
                else
                {
                    if (textConfidence > m_IgnoreWordConfidence )
                        m_RecognizeFailure.SendData( data );
                }
            }
        }

	    private void OnClassified(ClassifyResult result)
	    {
            if ( m_ClassifyOutput.IsConnected )
                m_ClassifyOutput.SendData( new ClassifyResultData( result ) );

            if ( result != null )
            {
                Log.Debug( "NlcWidget", "OnClassified: {0} ({1:0.00})", result.top_class, result.topConfidence );

                if ( m_TopClassOutput.IsConnected && !string.IsNullOrEmpty( result.top_class ) )
                    m_TopClassOutput.SendData( new TextData( result.top_class ) );
                if ( m_TopClassText != null )
                    m_TopClassText.text = result.top_class; 
                if ( m_SendAsEvent && !string.IsNullOrEmpty( result.top_class) )
                    EventManager.Instance.SendEvent( result.top_class );
            }
	    }

	}
}
