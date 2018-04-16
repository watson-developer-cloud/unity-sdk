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

using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.Assistant.v1;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleCustomHeaders : MonoBehaviour
{
    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [SerializeField]
    private string _assistantUsername;
    [SerializeField]
    private string _assistantPassword;
    [SerializeField]
    private string _assistantUrl;
    [SerializeField]
    private string _assistantWorkspaceId;
    [SerializeField]
    private string _assistantVersionDate;
    [SerializeField]
    private string _speechToTextUsername;
    [SerializeField]
    private string _speechToTextPassword;
    [SerializeField]
    private string _speechToTextUrl;
    #endregion
    private int _recordingRoutine = 0;
    private string _microphoneID = null;
    private AudioClip _recording = null;
    private int _recordingBufferSize = 1;
    private int _recordingHZ = 22050;

    private Assistant _assistant;
    private string _inputString = "Turn on the winshield wipers";

    private SpeechToText _speechToText;
    Dictionary<string, object> speechToTextCustomData = null;

    void Start()
    {
        LogSystem.InstallDefaultReactors();


        #region http custom headers
        //  Create credential and instantiate Assistant service
        Credentials assistantCredentials = new Credentials(_assistantUsername, _assistantPassword, _assistantUrl);

        _assistant = new Assistant(assistantCredentials);
        _assistant.VersionDate = _assistantVersionDate;

        Dictionary<string, object> input = new Dictionary<string, object>();
        input.Add("text", _inputString);
        MessageRequest messageRequest = new MessageRequest()
        {
            Input = input,
            AlternateIntents = true
        };

        //  Create customData object
        Dictionary<string, object> assistantCustomData = new Dictionary<string, object>();
        //  Create a dictionary of custom headers
        Dictionary<string, string> assistantCustomHeaders = new Dictionary<string, string>();
        //  Add to the header dictionary
        assistantCustomHeaders.Add("X-Watson-Metadata", "customer_id=some-assistant-customer-id");
        //  Add the header dictionary to the custom data object
        assistantCustomData.Add(Constants.String.CUSTOM_REQUEST_HEADERS, assistantCustomHeaders);

        //  Logging what we will send
        if (assistantCustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
        {
            Log.Debug("ExampleCustomHeader.Start()", "Assistant custom request headers:");
            foreach (KeyValuePair<string, string> kvp in assistantCustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
            {
                Log.Debug("ExampleCustomHeader.Start()", "\t{0}: {1}", kvp.Key, kvp.Value);
            }
        }

        //  Call service using custom data object
        _assistant.Message(OnMessage, OnFail, _assistantWorkspaceId, messageRequest, customData: assistantCustomData);
        #endregion

        #region websocket custom headers
        //  Websocket custom headers
        //  Create credential and instantiate Speech to Text service
        Credentials speechToTextCredentials = new Credentials(_speechToTextUsername, _speechToTextPassword, _speechToTextUrl);

        _speechToText = new SpeechToText(speechToTextCredentials);

        //  Create customData object
        speechToTextCustomData = new Dictionary<string, object>();
        //  Create a dictionary of custom headers
        Dictionary<string, string> speechToTextCustomHeaders = new Dictionary<string, string>();
        //  Add header to the dictionary
        speechToTextCustomHeaders.Add("X-Watson-Metadata", "customer_id=some-speech-to-text-customer-id");
        //  Add the header dictionary to the custom data object
        speechToTextCustomData.Add(Constants.String.CUSTOM_REQUEST_HEADERS, speechToTextCustomHeaders);

        //  Logging what we will send
        if (speechToTextCustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
        {
            Log.Debug("ExampleCustomHeader.Start()", "Speech to text custom request headers:");
            foreach (KeyValuePair<string, string> kvp in speechToTextCustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
            {
                Log.Debug("ExampleCustomHeader.Start()", "\t{0}: {1}", kvp.Key, kvp.Value);
            }
        }

        //  Call service using custom data object (see Active)
        Active = true;

        StartRecording();
        #endregion
    }

    private void OnMessage(object response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleCustomHeader.OnMessage()", "Response: {0}", customData["json"].ToString());

        if (customData.ContainsKey(Constants.String.RESPONSE_HEADERS))
        {
            Log.Debug("ExampleCustomHeader.OnMessage()", "Response headers:");

            foreach (KeyValuePair<string, string> kvp in customData[Constants.String.RESPONSE_HEADERS] as Dictionary<string, string>)
            {
                Log.Debug("ExampleCustomHeader.OnMessage()", "\t{0}: {1}", kvp.Key, kvp.Value);
            }
        }
    }

    private void OnRecognize(SpeechRecognitionEvent results, Dictionary<string, object> customData)
    {
        if (customData != null)
        {
            Log.Debug("ExampleCustomHeader.OnRecognize()", "Response: {0}", customData["json"].ToString());
            if (customData.ContainsKey(Constants.String.RESPONSE_HEADERS))
            {
                Log.Debug("ExampleCustomHeader.OnRecognize()", "Response headers:");

                foreach (KeyValuePair<string, string> kvp in customData[Constants.String.RESPONSE_HEADERS] as Dictionary<string, string>)
                {
                    Log.Debug("ExampleCustomHeader.OnRecognize()", "\t{0}: {1}", kvp.Key, kvp.Value);
                }
            }
        }
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleCustomHeader.OnFail()", "Response: {0}", customData["json"].ToString());
        Log.Error("ExampleCustomHeader.OnFail()", "Error received: {0}", error.ToString());
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
                _speechToText.StartListening(OnRecognize, customData: speechToTextCustomData);
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

        Log.Debug("ExampleStreaming.OnError()", "Error! {0}", error);
    }

    private IEnumerator RecordingHandler()
    {
        Log.Debug("ExampleStreaming.RecordingHandler()", "devices: {0}", Microphone.devices);
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
                Log.Error("ExampleStreaming.RecordingHandler()", "Microphone disconnected.");

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
                record.MaxLevel = Mathf.Max(Mathf.Abs(Mathf.Min(samples)), Mathf.Max(samples));
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
}
