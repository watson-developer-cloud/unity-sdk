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

namespace IBM.Watson.Editor
{
	
	[RequireComponent(typeof(AudioSource))]
	public class TextToSpeechWidget : Widget
	{
	    #region Private Data
	    TextToSpeech m_TTS = new TextToSpeech();

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
	    #endregion

	    public void OnTextToSpeech()
	    {
	        if ( m_TTS.Voice != m_Voice )
	            m_TTS.Voice = m_Voice;
	        
	        m_TTS.ToSpeech( m_Input.text, OnSpeech, m_UsePost );
	        if ( m_StatusText != null )
	            m_StatusText.text = "THINKING";
	        if ( m_TextToSpeechButton != null )
	            m_TextToSpeechButton.interactable = false;
	    }

	    private void OnEnable()
	    {
	        Logger.InstallDefaultReactors();

	        if ( m_StatusText != null )
	            m_StatusText.text = "READY";
	        if ( m_Input != null )
	            m_Input.text = "No problem with opening the pod bay doors.";
	    }

	    private void OnSpeech( AudioClip clip )
	    {
	        if ( clip != null )
	        {
	 		    AudioSource source = GetComponent<AudioSource>();
	            if ( source != null )
	            {
	                source.spatialBlend = 0.0f;     // 2D sound
	                source.loop = false;            // do not loop
	                source.clip = clip;             // clip
	                source.Play();
	            }
	        }

	        if ( m_TextToSpeechButton != null )
	            m_TextToSpeechButton.interactable = true;
	        if ( m_StatusText != null )
	            m_StatusText.text = "READY";
	    }
	}

}
