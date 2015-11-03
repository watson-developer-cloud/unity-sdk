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

#define ENABLE_DEBUGGING

using IBM.Watson.Services.v1;
using IBM.Watson.Logging;
using UnityEngine;
using UnityEngine.UI;
using IBM.Watson.Data;

#pragma warning disable 414

namespace IBM.Watson.Widgets
{
    /// <summary>
    /// SpeechToText Widget that wraps the SpeechToText service.
    /// </summary>
	public class SpeechToTextWidget : Widget
	{
	    #region Private Data
	    private SpeechToText m_STT = new SpeechToText();
	    [SerializeField]
	    private Text m_StatusText = null;
	    [SerializeField]
	    private bool m_DetectSilence = true;
	    [SerializeField]
	    private float m_SilenceThreshold = 0.03f;
	    [SerializeField]
	    private bool m_WordConfidence = false;
	    [SerializeField]
	    private bool m_TimeStamps = false;
	    [SerializeField]
	    private int m_MaxAlternatives = 1;
        [SerializeField]
        private bool m_EnableContinous = false;
        [SerializeField]
        private bool m_EnableInterimResults = false;
	    [SerializeField]
	    private Text m_Transcript = null;
        [SerializeField]
        private Input m_AudioInput = new Input( "Audio", typeof(AudioData), "OnAudio" );
        [SerializeField]
        private Output m_TextOutput = new Output( typeof(TextData) );
        [SerializeField]
        private Output m_ResultOutput = new Output( typeof(SpeechToTextData) );
        #endregion

        #region Public Properties
        /// <summary>
        /// This property starts or stop's this widget listening for speech.
        /// </summary>
        public bool Active
        {
            get { return m_STT.IsListening; }
            set {
                if ( value && !m_STT.IsListening )
                {
 	                m_STT.DetectSilence = m_DetectSilence;
	                m_STT.EnableWordConfidence = m_WordConfidence;
	                m_STT.EnableTimestamps = m_TimeStamps;
	                m_STT.SilenceThreshold = m_SilenceThreshold;
	                m_STT.MaxAlternatives = m_MaxAlternatives;
                    m_STT.EnableContinousRecognition = m_EnableContinous;
                    m_STT.EnableInterimResults = m_EnableInterimResults;
	                m_STT.OnError = OnError;
	                m_STT.StartListening( OnRecognize );
	                if ( m_StatusText != null )
	                    m_StatusText.text = "LISTENING";
                }
                else if ( !value && m_STT.IsListening )
                {
 	                m_STT.StopListening();
	                if ( m_StatusText != null )
	                    m_StatusText.text = "READY";
                   }
            }
        }
        #endregion

        /// <summary>
        /// Button handler to toggle the active state of this widget.
        /// </summary>
        public void OnListenButton()
	    {
            Active = !Active;
	    }

        /// <exclude />
        protected override string GetName()
        {
            return "SpeechToText";
        }

        /// <exclude />
        protected override void Start()
	    {
            base.Start();
	        Logger.InstallDefaultReactors();

	        if ( m_StatusText != null )
	            m_StatusText.text = "READY";
	    }

	    private void OnError( string error )
	    {
            Active = false;
	        if ( m_StatusText != null )
	            m_StatusText.text = "ERROR: " + error;
	    }
        
        private void OnAudio( Data data )
        {
            if (! Active )
                Active = true;

            m_STT.OnListen( (AudioData)data );
        }

	    private void OnRecognize(SpeechResultList result)
	    {
            m_ResultOutput.SendData( new SpeechToTextData( result ) );

            if (result != null && result.Results.Length > 0)
            {
                if ( m_Transcript != null )
                    m_Transcript.text = "";

                foreach( var res in result.Results )
                {
                    foreach( var alt in res.Alternatives )
                    {
                        string text = alt.Transcript;
                        if ( res.Final )
                            m_TextOutput.SendData( new TextData( text ) );

                        if ( m_Transcript != null )
                            m_Transcript.text += string.Format( "{0} ({1}, {2:0.00})\n",
                                text, res.Final ? "Final" : "Interim", alt.Confidence );
                    }
                }
            }
	    }

	}
}
