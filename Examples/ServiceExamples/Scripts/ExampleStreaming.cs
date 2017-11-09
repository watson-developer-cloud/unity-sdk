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

//  Uncomment to test chunked
#define CHUNK_BUFFER

using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.DataTypes;
using System.Collections.Generic;
using UnityEngine.UI;

public class ExampleStreaming : MonoBehaviour
{
    private string _username = null;
    private string _password = null;
    private string _url = null;
    public Text ResultsField;

    private int _recordingRoutine = 0;
    private string _microphoneID = null;
    private AudioClip _recording = null;
    private int _recordingBufferSize = 1;
    private int _recordingHZ = 22050;
    private int _chunkCount = 50;

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
                //List<string> keywords = new List<string>();
                //keywords.Add("hello");
                //keywords.Add("testing");
                //keywords.Add("watson");
                //_speechToText.KeywordsThreshold = 0.5f;
                //_speechToText.Keywords = keywords.ToArray();
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

        Log.Debug("ExampleStreaming", "Error! {0}", error);
    }

#if CHUNK_BUFFER

    private IEnumerator RecordingHandler()
    {
        Log.Debug("ExampleStreamingChunks", "devices: {0}", Microphone.devices);
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

        //  Current chunk number
        int chunkNum = 0;

        //  Size of the chunk in samples
        int chunkSize = _recording.samples / _chunkCount;

        //  Init samples
        float[] samples = null;

        while (_recordingRoutine != 0 && _recording != null)
        {
            //  Get the mic position
            int microphonePosition = Microphone.GetPosition(_microphoneID);
            if (microphonePosition > _recording.samples || !Microphone.IsRecording(_microphoneID))
            {
                Log.Error("ExampleStreaming", "Microphone disconnected.");

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
            //If the write position is past the end of the chunk or if write position is before the start of the chunk
            while (microphonePosition > sampleEnd || microphonePosition < sampleStart)
            {
                //  Init samples
                samples = new float[chunkSize];
                //  Write data from recording into samples starting from the chunkStart
                _recording.GetData(samples, sampleStart);

                //  Create AudioData and use the samples we just created
                AudioData record = new AudioData();
                record.MaxLevel = Mathf.Abs(Mathf.Max(samples));
                record.Clip = AudioClip.Create("Recording", chunkSize, _recording.channels, _recordingHZ, false);
                record.Clip.SetData(samples, 0);

                //  Send the newly created AudioData to the service
                _speechToText.OnListen(record);

                //  Iterate or reset chunkNum
                if (chunkNum < _chunkCount - 1)
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
                sampleStart = chunkSize * chunkNum;
                sampleEnd = chunkSize * (chunkNum + 1);
            }

            yield return 0;
        }

        yield break;
    }


#else

    private IEnumerator RecordingHandler()
    {
        Log.Debug("ExampleStreaming", "devices: {0}", Microphone.devices);
        _recording = Microphone.Start(_microphoneID, true, _recordingBufferSize, _recordingHZ);
        yield return null;      // let _recordingRoutine get set..

        if (_recording == null)
        {
            StopRecording();
            yield break;
        }

        bool bFirstBlock = true;
        int midPoint = _recording.samples / 2;
        float[] samples = null;

        while (_recordingRoutine != 0 && _recording != null)
        {
            int writePos = Microphone.GetPosition(_microphoneID);
            if (writePos > _recording.samples || !Microphone.IsRecording(_microphoneID))
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
                _recording.GetData(samples, bFirstBlock ? 0 : midPoint);

                AudioData record = new AudioData();
                record.MaxLevel = Mathf.Abs(Mathf.Max(samples));
                record.Clip = AudioClip.Create("Recording", midPoint, _recording.channels, _recordingHZ, false);
                record.Clip.SetData(samples, 0);

                _speechToText.OnListen(record);

                bFirstBlock = !bFirstBlock;
            }
            else
            {
                // calculate the number of samples remaining until we ready for a block of audio, 
                // and wait that amount of time it will take to record.
                int remaining = bFirstBlock ? (midPoint - writePos) : (_recording.samples - writePos);
                float timeRemaining = (float)remaining / (float)_recordingHZ;

                yield return new WaitForSeconds(timeRemaining);
            }

        }

        yield break;
    }
#endif

    private void OnRecognize(SpeechRecognitionEvent result)
    {
        if (result != null && result.results.Length > 0)
        {
            foreach (var res in result.results)
            {
                foreach (var alt in res.alternatives)
                {
                    string text = string.Format("{0} ({1}, {2:0.00})\n", alt.transcript, res.final ? "Final" : "Interim", alt.confidence);
                    Log.Debug("ExampleStreaming", text);
                    ResultsField.text = text;
                }

                if (res.keywords_result != null && res.keywords_result.keyword != null)
                {
                    foreach (var keyword in res.keywords_result.keyword)
                    {
                        Log.Debug("ExampleSpeechToText", "keyword: {0}, confidence: {1}, start time: {2}, end time: {3}", keyword.normalized_text, keyword.confidence, keyword.start_time, keyword.end_time);
                    }
                }

                if (res.word_alternatives != null)
                {
                    foreach (var wordAlternative in res.word_alternatives)
                    {
                        Log.Debug("ExampleSpeechToText", "Word alternatives found. Start time: {0} | EndTime: {1}", wordAlternative.start_time, wordAlternative.end_time);
                        foreach (var alternative in wordAlternative.alternatives)
                            Log.Debug("ExampleSpeechToText", "\t word: {0} | confidence: {1}", alternative.word, alternative.confidence);
                    }
                }
            }
        }
    }

    private void OnRecognizeSpeaker(SpeakerRecognitionEvent result)
    {
        if (result != null)
        {
            foreach (SpeakerLabelsResult labelResult in result.speaker_labels)
            {
                Log.Debug("ExampleStreaming", string.Format("speaker result: {0} | confidence: {3} | from: {1} | to: {2}", labelResult.speaker, labelResult.from, labelResult.to, labelResult.confidence));
            }
        }
    }
}