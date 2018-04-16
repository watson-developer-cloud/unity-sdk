﻿/**
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
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// This class is an example of how to stream audio from the device microphone to the Speech to Text service in Unity.
/// </summary>
public class ExampleStreamingSplitSamples : MonoBehaviour
{
    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [SerializeField]
    private string _username;
    [SerializeField]
    private string _password;
    [SerializeField]
    private string _url;
    #endregion

    /// <summary>
    /// Text field to display the results of streaming.
    /// </summary>
    public Text ResultsField;

    private int _recordingRoutine = 0;
    private string _microphoneID = null;
    private AudioClip _recording = null;
    private int _recordingBufferSize = 1;
    private int _recordingHZ = 22050;
    private int _sampleSegments = 50;

    private SpeechToText _speechToText;

    void Start()
    {
        LogSystem.InstallDefaultReactors();

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_username, _password, _url);

        _speechToText = new SpeechToText(credentials);
        Active = true;

        StartRecording();
    }

    /// <summary>
    /// Gets or sets the Active state.
    /// </summary>
    public bool Active
    {
        get { return _speechToText.IsListening; }
        set
        {
            if (value && !_speechToText.IsListening)
            {
                _speechToText.DetectSilence = true;
                _speechToText.EnableWordConfidence = true;
                _speechToText.EnableTimestamps = true;
                _speechToText.SilenceThreshold = 0.01f;
                _speechToText.MaxAlternatives = 0;
                _speechToText.EnableInterimResults = true;
                _speechToText.OnError = OnError;
                _speechToText.InactivityTimeout = -1;
                _speechToText.ProfanityFilter = false;
                _speechToText.SmartFormatting = true;
                _speechToText.SpeakerLabels = false;
                _speechToText.WordAlternativesThreshold = null;
                _speechToText.StartListening(OnRecognize, OnRecognizeSpeaker);
            }
            else if (!value && _speechToText.IsListening)
            {
                _speechToText.StopListening();
            }
        }
    }

    private void StartRecording()
    {
        if (_recordingRoutine == 0)
        {
            UnityObjectUtil.StartDestroyQueue();
            _recordingRoutine = Runnable.Run(RecordingHandler());
        }
    }

    private void StopRecording()
    {
        if (_recordingRoutine != 0)
        {
            Microphone.End(_microphoneID);
            Runnable.Stop(_recordingRoutine);
            _recordingRoutine = 0;
        }
    }

    private void OnError(string error)
    {
        Active = false;

        Log.Debug("ExampleStreamingSplitSamples.OnError()", "Error! {0}", error);
    }

    private IEnumerator RecordingHandler()
    {
        Log.Debug("ExampleStreamingSplitSamples.RecordingHandler()", "devices: {0}", Microphone.devices);
        //  Start recording
        _recording = Microphone.Start(_microphoneID, true, _recordingBufferSize, _recordingHZ);
        yield return null;

        if (_recording == null)
        {
            StopRecording();
            yield break;
        }

#if ENABLE_TIME_LOGGING
        //  Set a reference to now to check timing
        DateTime now = DateTime.Now;
#endif

        //  Current sample segment number
        int sampleSegmentNum = 0;

        //  Size of the sample segment in samples
        int sampleSegmentSize = _recording.samples / _sampleSegments;

        //  Init samples
        float[] samples = null;

        while (_recordingRoutine != 0 && _recording != null)
        {
            //  Get the mic position
            int microphonePosition = Microphone.GetPosition(_microphoneID);
            if (microphonePosition > _recording.samples || !Microphone.IsRecording(_microphoneID))
            {
                Log.Error("ExampleStreamingSplitSamples.RecordingHandler()", "Microphone disconnected.");

                StopRecording();
                yield break;
            }

            int sampleStart = sampleSegmentSize * sampleSegmentNum;
            int sampleEnd = sampleSegmentSize * (sampleSegmentNum + 1);

#if ENABLE_DEBUGGING
            Log.Debug("ExampleStreamingSplitSamples.RecordinHandler", "microphonePosition: {0} | sampleStart: {1} | sampleEnd: {2} | sampleSegmentNum: {3}",
            microphonePosition.ToString(),
            sampleStart.ToString(),
            sampleEnd.ToString(),
            sampleSegmentNum.ToString());
#endif
            //If the write position is past the end of the sample segment or if write position is before the start of the sample segment
            while (microphonePosition > sampleEnd || microphonePosition < sampleStart)
            {
                //  Init samples
                samples = new float[sampleSegmentSize];
                //  Write data from recording into samples starting from the sampleSegmentStart
                _recording.GetData(samples, sampleStart);

                //  Create AudioData and use the samples we just created
                AudioData record = new AudioData();
                record.MaxLevel = Mathf.Max(Mathf.Abs(Mathf.Min(samples)), Mathf.Max(samples));
                record.Clip = AudioClip.Create("Recording", sampleSegmentSize, _recording.channels, _recordingHZ, false);
                record.Clip.SetData(samples, 0);

                //  Send the newly created AudioData to the service
                _speechToText.OnListen(record);

                //  Iterate or reset sampleSegmentNum
                if (sampleSegmentNum < _sampleSegments - 1)
                {
                    sampleSegmentNum++;
#if ENABLE_DEBUGGING
                    Log.Debug("ExampleStreamingSplitSamples.RecordingHandler()", "Iterating sampleSegmentNum: {0}", sampleSegmentNum);
#endif
                }
                else
                {
                    sampleSegmentNum = 0;
#if ENABLE_DEBUGGING
                    Log.Debug("ExampleStreamingSplitSamples.RecordingHandler()", "Resetting sampleSegmentNum: {0}", sampleSegmentNum);
#endif
                }

#if ENABLE_TIME_LOGGING
                Log.Debug("ExampleStreamingSplitSamples.RecordingHandler", "Sending data - time since last transmission: {0} ms", Mathf.Floor((float)(DateTime.Now - now).TotalMilliseconds));
                now = DateTime.Now;
#endif
                sampleStart = sampleSegmentSize * sampleSegmentNum;
                sampleEnd = sampleSegmentSize * (sampleSegmentNum + 1);
            }

            yield return 0;
        }

        yield break;
    }

    private void OnRecognize(SpeechRecognitionEvent result, Dictionary<string, object> customData)
    {
        if (result != null && result.results.Length > 0)
        {
            foreach (var res in result.results)
            {
                foreach (var alt in res.alternatives)
                {
                    string text = string.Format("{0} ({1}, {2:0.00})\n", alt.transcript, res.final ? "Final" : "Interim", alt.confidence);
                    Log.Debug("ExampleStreamingSplitSamples.OnRecognize()", text);
                    ResultsField.text = text;
                }

                if (res.keywords_result != null && res.keywords_result.keyword != null)
                {
                    foreach (var keyword in res.keywords_result.keyword)
                    {
                        Log.Debug("ExampleStreamingSplitSamples.OnRecognize", "keyword: {0}, confidence: {1}, start time: {2}, end time: {3}", keyword.normalized_text, keyword.confidence, keyword.start_time, keyword.end_time);
                    }
                }

                if (res.word_alternatives != null)
                {
                    foreach (var wordAlternative in res.word_alternatives)
                    {
                        Log.Debug("ExampleStreamingSplitSamples.OnRecognize()", "Word alternatives found. Start time: {0} | EndTime: {1}", wordAlternative.start_time, wordAlternative.end_time);
                        foreach (var alternative in wordAlternative.alternatives)
                            Log.Debug("ExampleStreamingSplitSamples.OnRecognie()", "\t word: {0} | confidence: {1}", alternative.word, alternative.confidence);
                    }
                }
            }
        }
    }

    private void OnRecognizeSpeaker(SpeakerRecognitionEvent result, Dictionary<string, object> customData)
    {
        if (result != null)
        {
            foreach (SpeakerLabelsResult labelResult in result.speaker_labels)
            {
                Log.Debug("ExampleStreamingSplitSamples.OnRecongizeSpeaker()", string.Format("speaker result: {0} | confidence: {3} | from: {1} | to: {2}", labelResult.speaker, labelResult.from, labelResult.to, labelResult.confidence));
            }
        }
    }
}