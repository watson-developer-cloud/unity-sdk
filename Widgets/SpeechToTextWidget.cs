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
*/

using IBM.Watson.Services.v1;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SpeechToTextWidget : MonoBehaviour
{
    #region Private Data
    SpeechToText m_STT = new SpeechToText();
    List<AudioClip> m_Recordings = new List<AudioClip>();
    #endregion

    private void OnEnable()
    {
        Logger.InstallDefaultReactors();
    }

    private void OnGUI()
    {
        if ( GUILayout.Button( "Start Recording" ) )
            m_STT.StartRecording( OnRecordClip );
        if ( GUILayout.Button( "Stop Recording" ) )
        {
            m_STT.StopRecording();

            AudioClip recording = AudioClipUtil.Combine( m_Recordings.ToArray() );
            m_Recordings.Clear();

            byte [] waveFile = WaveFile.CreateWAV( recording );
            System.IO.File.WriteAllBytes( Application.persistentDataPath + "/Recording.wav", waveFile );
        }
    }

    private void OnRecordClip( SpeechToText.RecordClip record )
    {
        if ( record != null )
        {
            Log.Status( "SpeechToTextWidget", "MaxLevel = {0}", record.MaxLevel );
            m_Recordings.Add( record.Clip );
        }

    }
}
