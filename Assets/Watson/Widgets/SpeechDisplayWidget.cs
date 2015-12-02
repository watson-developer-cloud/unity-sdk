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


using IBM.Watson.DataModels;
using IBM.Watson.DataTypes;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 414

namespace IBM.Watson.Widgets
{
    /// <summary>
    /// Simple class for displaying the SpeechToText result data in the UI.
    /// </summary>
    public class SpeechDisplayWidget : Widget
    {
        #region Widget interface
        protected override string GetName()
        {
            return "SpeechDisplay";
        }
        #endregion

        #region Private Data

		[SerializeField]
		private bool m_ContinuousText = false;
        [SerializeField]
        private Text m_Output = null;
		[SerializeField]
		private InputField m_OutputAsInputField = null;
		[SerializeField]
		private Text m_OutputStatus = null;
        [SerializeField]
        private Input m_SpeechInput = new Input( "SpeechInput", typeof(SpeechToTextData), "OnSpeechInput" );
		[SerializeField]
		private float m_MinConfidenceToShow = 0.5f;
        #endregion

        private void OnSpeechInput( Data data )
        {
			if ( m_Output != null || m_OutputAsInputField != null)
            {
                SpeechResultList result = ((SpeechToTextData)data).Results;
                if (result != null && result.Results.Length > 0)
                {
					if(m_Output != null && !m_ContinuousText)
                    	m_Output.text = "";
					if(m_OutputAsInputField != null && !m_ContinuousText)
						m_OutputAsInputField.text = "";
					if(m_OutputStatus != null)
						m_OutputStatus.text = "";

                    foreach( var res in result.Results )
                    {
                        foreach( var alt in res.Alternatives )
                        {
                            string text = alt.Transcript;
							if(m_Output != null){
                            	m_Output.text += string.Format( "{0} ({1}, {2:0.00})\n", text, res.Final ? "Final" : "Interim", alt.Confidence );
							}
							if(m_OutputAsInputField != null){
								if(!res.Final || alt.Confidence > m_MinConfidenceToShow){
									m_OutputAsInputField.text = string.Concat( m_OutputAsInputField.text , " ", text);

									if(m_OutputStatus != null){
										m_OutputStatus.text += string.Format( "{0}, {1:0.00}", res.Final ? "Final" : "Interim", alt.Confidence );
									}
								}
							}

                        }
                    }
                }
            }
        }

    }
}
