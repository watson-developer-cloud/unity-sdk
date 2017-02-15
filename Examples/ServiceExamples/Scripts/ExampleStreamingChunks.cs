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
//#define ENABLE_TIME_LOGGING

using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.DataTypes;
using System;

public class ExampleStreamingChunks : MonoBehaviour
{
    private int m_RecordingRoutine = 0;
    private string m_MicrophoneID = null;
    private AudioClip m_Recording = null;
    private int m_RecordingBufferSize = 1;
    private int m_ChunkCount = 5;
    private int m_RecordingHZ = 22050;

    private SpeechToText m_SpeechToText = new SpeechToText();

    void Start()
    {
        LogSystem.InstallDefaultReactors();
        Log.Debug("ExampleStreamingChunks", "Start();");

        Active = true;

        StartRecording();
    }

    public bool Active
    {
        get { return m_SpeechToText.IsListening; }
        set
        {
            if (value && !m_SpeechToText.IsListening)
            {
                m_SpeechToText.DetectSilence = true;
                m_SpeechToText.EnableWordConfidence = false;
                m_SpeechToText.EnableTimestamps = false;
                m_SpeechToText.SilenceThreshold = 0.03f;
                m_SpeechToText.MaxAlternatives = 1;
                m_SpeechToText.EnableContinousRecognition = true;
                m_SpeechToText.EnableInterimResults = true;
                m_SpeechToText.OnError = OnError;
                m_SpeechToText.StartListening(OnRecognize);
            }
            else if (!value && m_SpeechToText.IsListening)
            {
                m_SpeechToText.StopListening();
            }
        }
    }

    private void StartRecording()
    {
        if (m_RecordingRoutine == 0)
        {
            UnityObjectUtil.StartDestroyQueue();
            m_RecordingRoutine = Runnable.Run(RecordingHandler());
        }
    }

    private void StopRecording()
    {
        if (m_RecordingRoutine != 0)
        {
            Microphone.End(m_MicrophoneID);
            Runnable.Stop(m_RecordingRoutine);
            m_RecordingRoutine = 0;
        }
    }

    private void OnError(string error)
    {
        Active = false;

        Log.Debug("ExampleStreamingChunks", "Error! {0}", error);
    }

    private IEnumerator RecordingHandler()
    {
        Log.Debug("ExampleStreamingChunks", "devices: {0}", Microphone.devices);
        //  Start recording
        m_Recording = Microphone.Start(m_MicrophoneID, true, m_RecordingBufferSize, m_RecordingHZ);
        yield return null;

        if (m_Recording == null)
        {
            StopRecording();
            yield break;
        }

#if ENABLE_TIME_LOGGING
        //  Set a reference to now to check timing
        DateTime now = DateTime.Now;
#endif

        //  Current chunk number
        int chunkNum = 0;

        //  Size of the chunk in samples
        int chunkSize = m_Recording.samples / m_ChunkCount;

        //  Init samples
        float[] samples = null;

        while (m_RecordingRoutine != 0 && m_Recording != null)
        {
            //  Get the mic position
            int microphonePosition = Microphone.GetPosition(m_MicrophoneID);
            if (microphonePosition > m_Recording.samples || !Microphone.IsRecording(m_MicrophoneID))
            {
                Log.Error("ExampleStreamingChunks", "Microphone disconnected.");

                StopRecording();
                yield break;
            }

            int sampleStart = chunkSize * chunkNum;
            int sampleEnd = chunkSize * (chunkNum + 1);

#if ENABLE_DEBUGGING
            Log.Debug("ExampleStreamingChunks", "microphonePosition: {0} | sampleStart: {1} | sampleEnd: {2} | chunkNum: {3}",
                microphonePosition.ToString(),
                sampleStart.ToString(),
                sampleEnd.ToString(),
                chunkNum.ToString());
#endif
            //If the write position is past the end of the chunk or if write position is before the start of the chunk and the chunk number is equal to the chunk count
            if (microphonePosition > sampleEnd || (microphonePosition < sampleStart && chunkNum == (m_ChunkCount - 1)))
            {
                //  Init samples
                samples = new float[chunkSize];
                //  Write data from recording into samples starting from the chunkStart
                m_Recording.GetData(samples, sampleStart);

                //  Create AudioData and use the samples we just created
                AudioData record = new AudioData();
                record.MaxLevel = Mathf.Max(samples);
                record.Clip = AudioClip.Create("Recording", chunkSize, m_Recording.channels, m_RecordingHZ, false);
                record.Clip.SetData(samples, 0);

                //  Send the newly created AudioData to the service
                m_SpeechToText.OnListen(record);

                //  Iterate or reset chunkNum
                if (chunkNum < m_ChunkCount - 1)
                {
                    chunkNum++;
#if ENABLE_DEBUGGING
                    Log.Debug("ExampleStreamingChunks", "Iterating chunkNum: {0}", chunkNum);
#endif
                }
                else
                {
                    chunkNum = 0;
#if ENABLE_DEBUGGING
                    Log.Debug("ExampleStreamingChunks", "Resetting chunkNum: {0}", chunkNum);
#endif
                }

#if ENABLE_TIME_LOGGING
                Log.Debug("ExampleStreamingChunks", "Sending data - time since last transmission: {0} ms", Mathf.Floor((float)(DateTime.Now - now).TotalMilliseconds));
                now = DateTime.Now;
#endif
            }
            else
            {
                // calculate the number of samples remaining until we ready for a block of audio, 
                // and wait that amount of time it will take to record.
                int remaining = sampleEnd - microphonePosition;
                float timeRemaining = (float)remaining / (float)m_RecordingHZ;

                yield return new WaitForSeconds(.1f);
            }
        }

        yield break;
    }


    private void OnRecognize(SpeechRecognitionEvent result)
    {
        if (result != null && result.results.Length > 0)
        {
            foreach (var res in result.results)
            {
                foreach (var alt in res.alternatives)
                {
                    string text = alt.transcript;
                    Log.Debug("ExampleStreamingChunks", string.Format("{0} ({1}, {2:0.00})\n", text, res.final ? "Final" : "Interim", alt.confidence));
                }
            }
        }
    }
}