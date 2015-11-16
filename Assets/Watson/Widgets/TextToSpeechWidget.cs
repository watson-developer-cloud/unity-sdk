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

using UnityEngine;
using UnityEngine.UI;
using IBM.Watson.Services.v1;
using IBM.Watson.Logging;
using IBM.Watson.Data;
using System.Collections.Generic;

#pragma warning disable 414

namespace IBM.Watson.Widgets
{
    /// <summary>
    /// TextToSpeech widget class wraps the TextToSpeech serivce.
    /// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class TextToSpeechWidget : Widget
	{
	    #region Private Data
	    TextToSpeech m_TTS = new TextToSpeech();

        [SerializeField]
        private Input m_TextInput = new Input( "Text", typeof(TextData), "OnTextInput" ); 
        [SerializeField]
        private Output m_Speaking = new Output( typeof(BooleanData) );
        [SerializeField]
        private Output m_LevelOut = new Output( typeof(FloatData) );
        [SerializeField, Tooltip( "How often to send level out data in seconds.") ] 
        private float m_LevelOutInterval = 0.05f;
		[SerializeField]
		private float m_LevelOutputModifier = 1.0f;
	    [SerializeField]
	    private Button m_TextToSpeechButton = null;
	    [SerializeField]
	    private InputField m_Input = null;
	    [SerializeField]
	    private Text m_StatusText = null;
	    [SerializeField]
	    private TextToSpeech.VoiceType m_Voice = TextToSpeech.VoiceType.en_US_Michael;
	    [SerializeField]
	    private bool m_UsePost = false;

        private AudioSource m_Source = null;
        private int m_LastPlayPos = 0;
        private Queue<AudioClip> m_SpeechQueue = new Queue<AudioClip>();
	    #endregion

	    public void OnTextToSpeech()
	    {
	        if ( m_TTS.Voice != m_Voice )
	            m_TTS.Voice = m_Voice;
            if ( m_Input != null )
	            m_TTS.ToSpeech( m_Input.text, OnSpeech, m_UsePost );
	        if ( m_StatusText != null )
	            m_StatusText.text = "THINKING";
	        if ( m_TextToSpeechButton != null )
	            m_TextToSpeechButton.interactable = false;
	    }

        #region Private Functions
        private void OnTextInput( Data data )
        {
	        if ( m_TTS.Voice != m_Voice )
	            m_TTS.Voice = m_Voice;
            m_TTS.ToSpeech( ((TextData)data).Text, OnSpeech, m_UsePost );
        }

	    private void OnEnable()
	    {
	        Logger.InstallDefaultReactors();

	        if ( m_StatusText != null )
	            m_StatusText.text = "READY";
	        if ( m_Input != null )
	            m_Input.text = "No problem with opening the pod bay doors.";
	    }

        protected override void Start()
        {
            base.Start();
            m_Source = GetComponent<AudioSource>();
        }

	    private void OnSpeech( AudioClip clip )
	    {
	        if ( clip != null )
                m_SpeechQueue.Enqueue( clip );
        }

        private void Update()
        {
            if ( m_SpeechQueue.Count > 0 && m_Source != null && !m_Source.isPlaying )
	        {
                AudioClip clip = m_SpeechQueue.Dequeue();
                if ( m_Speaking.IsConnected )
                    m_Speaking.SendData( new BooleanData( true ) );

	            m_Source.spatialBlend = 0.0f;     // 2D sound
	            m_Source.loop = false;            // do not loop
	            m_Source.clip = clip;             // clip
	            m_Source.Play();

                Invoke( "OnEndSpeech", ((float)clip.samples / (float)clip.frequency) + 0.1f );
                if ( m_LevelOut.IsConnected )
                {
                    m_LastPlayPos = 0;
                    InvokeRepeating( "OnLevelOut", m_LevelOutInterval, m_LevelOutInterval );
                }
	        }

	        if ( m_TextToSpeechButton != null )
	            m_TextToSpeechButton.interactable = true;
	        if ( m_StatusText != null )
	            m_StatusText.text = "READY";
	    }

        private void OnLevelOut()
        {
            if ( m_Source != null && m_Source.isPlaying )
            {
                int currentPos = m_Source.timeSamples;
                if ( currentPos > m_LastPlayPos )
                {
                    float [] samples = new float[ currentPos - m_LastPlayPos ];
                    m_Source.clip.GetData( samples, m_LastPlayPos );
					m_LevelOut.SendData( new TextToSpeechData( Mathf.Max( samples ) * m_LevelOutputModifier) );
                    m_LastPlayPos = currentPos;
                }
            }
            else
                CancelInvoke( "OnLevelOut" );
        }
        private void OnEndSpeech()
        {
            if ( m_Speaking.IsConnected )
                m_Speaking.SendData( new BooleanData( false ) );
            if (m_Source.isPlaying)
                m_Source.Stop();
        }

        protected override string GetName()
        {
            return "TextToSpeech";
        }
        #endregion
    }

}
