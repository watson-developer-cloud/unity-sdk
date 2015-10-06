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

[RequireComponent(typeof(AudioSource))]
public class SpeechToTextWidget : MonoBehaviour
{
    #region Private Data
    private SpeechToText m_STT = new SpeechToText();
    private List<AudioClip> m_Recordings = new List<AudioClip>();
    [SerializeField]
    private Text m_Transcript = null;
    #endregion

    private void OnEnable()
    {
        Logger.InstallDefaultReactors();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Start Recording"))
            m_STT.StartRecording(OnRecordClip);
        if (GUILayout.Button("Stop Recording"))
        {
            m_STT.StopRecording();

            AudioClip recording = AudioClipUtil.Combine(m_Recordings.ToArray());
            m_Recordings.Clear();

            m_STT.Recognize(recording, OnRecognize);

            //byte[] waveFile = WaveFile.CreateWAV(recording);
            //System.IO.File.WriteAllBytes(Application.persistentDataPath + "/Recording.wav", waveFile);
        }

        if ( GUILayout.Button( "Start Listening" ) )
            m_STT.StartListening( OnRecognize );
        if ( GUILayout.Button( "Stop Listening" ) )
            m_STT.StopListening();
    }

    private void OnRecordClip(SpeechToText.RecordClip record)
    {
        if (record != null)
        {
            Log.Status("SpeechToTextWidget", "MaxLevel = {0}", record.MaxLevel);
            m_Recordings.Add(record.Clip);
        }
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
                        m_Transcript.text += result.Results[i].Alternatives[j].Transcript + "\n";

                    // keep the length of the text under a reasonable number for display..
                    while ( m_Transcript.text.Length > 5000 )
                    {
                        int nextNewline = m_Transcript.text.IndexOf( '\n' );
                        if ( nextNewline < 0 )
                            break;
                        m_Transcript.text = m_Transcript.text.Substring( nextNewline + 1 );
                    }
                }
            }
        }
    }
}
