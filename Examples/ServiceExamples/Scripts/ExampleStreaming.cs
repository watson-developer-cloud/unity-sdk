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

using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.DataTypes;

public class ExampleStreaming : MonoBehaviour
{
  private int m_RecordingRoutine = 0;
  private string m_MicrophoneID = null;
  private AudioClip m_Recording = null;
  private int m_RecordingBufferSize = 2;
  private int m_RecordingHZ = 22050;

  private SpeechToText m_SpeechToText = new SpeechToText();

  void Start()
  {
    LogSystem.InstallDefaultReactors();
    Log.Debug("ExampleStreaming", "Start();");

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

    Log.Debug("ExampleStreaming", "Error! {0}", error);
  }

  private IEnumerator RecordingHandler()
  {
    Log.Debug("ExampleStreaming", "devices: {0}", Microphone.devices);
    m_Recording = Microphone.Start(m_MicrophoneID, true, m_RecordingBufferSize, m_RecordingHZ);
    yield return null;      // let m_RecordingRoutine get set..

    if (m_Recording == null)
    {
      StopRecording();
      yield break;
    }

    bool bFirstBlock = true;
    int midPoint = m_Recording.samples / 2;
    float[] samples = null;

    while (m_RecordingRoutine != 0 && m_Recording != null)
    {
      int writePos = Microphone.GetPosition(m_MicrophoneID);
      if (writePos > m_Recording.samples || !Microphone.IsRecording(m_MicrophoneID))
      {
        Log.Error("MicrophoneWidget", "Microphone disconnected.");

        StopRecording();
        yield break;
      }

      if ((bFirstBlock && writePos >= midPoint)
        || (!bFirstBlock && writePos < midPoint))
      {
        // front block is recorded, make a RecordClip and pass it onto our callback.
        samples = new float[midPoint];
        m_Recording.GetData(samples, bFirstBlock ? 0 : midPoint);

        AudioData record = new AudioData();
        record.MaxLevel = Mathf.Max(samples);
        record.Clip = AudioClip.Create("Recording", midPoint, m_Recording.channels, m_RecordingHZ, false);
        record.Clip.SetData(samples, 0);

        m_SpeechToText.OnListen(record);

        bFirstBlock = !bFirstBlock;
      }
      else
      {
        // calculate the number of samples remaining until we ready for a block of audio, 
        // and wait that amount of time it will take to record.
        int remaining = bFirstBlock ? (midPoint - writePos) : (m_Recording.samples - writePos);
        float timeRemaining = (float)remaining / (float)m_RecordingHZ;

        yield return new WaitForSeconds(timeRemaining);
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
          Log.Debug("ExampleStreaming", string.Format("{0} ({1}, {2:0.00})\n", text, res.final ? "Final" : "Interim", alt.confidence));
        }
      }
    }
  }
}