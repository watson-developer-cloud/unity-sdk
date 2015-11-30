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
        private Text m_Output = null;
        [SerializeField]
        private Input m_SpeechInput = new Input( "SpeechInput", typeof(SpeechToTextData), "OnSpeechInput" );
        #endregion

        private void OnSpeechInput( Data data )
        {
            if ( m_Output != null )
            {
                SpeechResultList result = ((SpeechToTextData)data).Results;
                if (result != null && result.Results.Length > 0)
                {
                    m_Output.text = "";

                    foreach( var res in result.Results )
                    {
                        foreach( var alt in res.Alternatives )
                        {
                            string text = alt.Transcript;
                            m_Output.text += string.Format( "{0} ({1}, {2:0.00})\n",
                                text, res.Final ? "Final" : "Interim", alt.Confidence );
                        }
                    }
                }
            }
        }

    }
}
