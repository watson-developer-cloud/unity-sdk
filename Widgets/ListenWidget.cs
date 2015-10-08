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
using IBM.Watson.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListenWidget : MonoBehaviour
{
    #region Private Data
    private SpeechToText m_STT = new SpeechToText();
    [SerializeField]
    private Text m_StatusText = null;
    [SerializeField]
    private Button m_ListenButton = null;
    [SerializeField]
    private float m_SilenceThreshold = 0.03f;
    [SerializeField]
    private Text m_Transcript = null;
    #endregion

    public void OnListenButton()
    {
        if (! m_STT.IsListening() )
        {
            m_STT.StartListening( OnRecognize );
            if ( m_StatusText != null )
                m_StatusText.text = "LISTENING";
        }
        else
        {
            m_STT.StopListening();
            if ( m_StatusText != null )
                m_StatusText.text = "READY";
        }
    }

    private void Start()
    {
        Logger.InstallDefaultReactors();

        if ( m_StatusText != null )
            m_StatusText.text = "READY";
    }

    private void OnRecognize(SpeechToText.ResultList result)
    {
        if (result != null)
        {
            //Log.Status("SpeechToText", "{0} result received.", result.Results.Length);
            for (int i = 0; i < result.Results.Length; ++i)
            {
                //Log.Status("SpeechToText", "Result {0}: Alternatives {1}", i, result.Results[i].Alternatives.Length);
                for (int j = 0; j < result.Results[i].Alternatives.Length; ++j)
                {
                    Log.Status("SpeechToText", "Result {0}, Alternative {1}, Transcript: {2}",
                        i, j, result.Results[i].Alternatives[j].Transcript);

                    if ( m_Transcript != null )
                        m_Transcript.text = result.Results[i].Alternatives[j].Transcript + "\n";
                }
            }
        }
    }
}
