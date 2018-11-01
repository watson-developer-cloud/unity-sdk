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

#define ENABLE_DEBUGGING

using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Utilities;
using MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using FullSerializer;
using System.IO;

namespace IBM.Watson.DeveloperCloud.Services.SpeechToText.v1
{
    /// <summary>
    /// This class wraps the Watson Speech to Text service.
    /// <a href="http://www.ibm.com/watson/developercloud/speech-to-text.html">Speech to Text Service</a>
    /// </summary>
    public class SpeechToText : IWatsonService
    {
        #region Constants
        /// <summary>
        /// This ID is used to match up a configuration record with this service.
        /// </summary>
        private const string ServiceId = "SpeechToTextV1";
        /// <summary>
        /// How often to send a message to the web socket to keep it alive.
        /// </summary>
        private const float WsKeepAliveInterval = 20.0f;
        /// <summary>
        /// If no listen state is received after start is sent within this time, we will timeout
        /// and stop listening. 
        /// </summary>
        private const float ListenTimeout = 10.0f;
        /// <summary>
        /// How many recording AudioClips will we queue before we enter a error state.
        /// </summary>
        private const int MaxQueuedRecordings = 1000;
        /// <summary>
        /// Size of a clip in bytes that can be sent through the Recognize function.
        /// </summary>
        private const int MaxRecognizeClipSize = 4 * (1024 * 1024);
        #endregion

        #region Public Types
        /// <summary>
        /// This callback is used to return errors through the OnError property.
        /// </summary>
        /// <param name="error">A string containing the error message.</param>
        public delegate void ErrorEvent(string error);
        /// <summary>
        /// The delegate for loading a file.
        /// </summary>
        /// <param name="filename">The filename to load.</param>
        /// <returns>Should return a byte array of the file contents or null of failure.</returns>
        public delegate byte[] LoadFileDelegate(string filename);
        /// <summary>
        /// Set this property to overload the internal file loading of this class.
        /// </summary>
        public LoadFileDelegate LoadFile { get; set; }
        #endregion

        #region Private Data
        private OnRecognize _listenCallback = null;        // Callback is set by StartListening()    
        private OnRecognizeSpeaker _speakerLabelCallback = null;
        private WSConnector _listenSocket = null;          // WebSocket object used when StartListening() is invoked  
        private bool _listenActive = false;
        private bool _audioSent = false;
        private bool _isListening = false;
        private Queue<AudioData> _listenRecordings = new Queue<AudioData>();
        private int _keepAliveRoutine = 0;             // ID of the keep alive co-routine
        private DateTime _lastKeepAlive = DateTime.Now;
        private DateTime _lastStartSent = DateTime.Now;
        private string _recognizeModel = "en-US_BroadbandModel";   // ID of the model to use.
        private int _maxAlternatives = 1;              // maximum number of alternatives to return.
        private string[] _keywords = null;
        private float? _keywordsThreshold = null;
        private float? _wordAlternativesThreshold = null;
        private bool _profanityFilter = true;
        private bool _smartFormatting = false;
        private bool _speakerLabels = false;
        private bool _timestamps = false;
        private bool _wordConfidence = false;
        private bool _detectSilence = true;            // If true, then we will try not to record silence.
        private float _silenceThreshold = 0.0f;         // If the audio level is below this value, then it's considered silent.
        private int _recordingHZ = -1;
        private int _inactivityTimeout = 60;
        private string _customization_id = null;
        private string _languageCustomizationId = null;
        private string _acoustic_customization_id = null;
        private float? _customization_weight = null;
        private bool _streamMultipart = false;           //  If true sets `Transfer-Encoding` header of multipart request to `chunked`.
        private float _silenceDuration = 0.0f;
        private float _silenceCutoff = 1.0f;

        private fsSerializer _serializer = new fsSerializer();
        private Credentials _credentials = null;
        private string _url = "https://stream.watsonplatform.net/speech-to-text/api";
        #endregion

        #region Public Properties
        /// <summary>
        /// True if StartListening() has been called.
        /// </summary>
        public bool IsListening { get { return _isListening; } private set { _isListening = value; } }
        /// <summary>
        /// True if AudioData has been sent and we are recognizing speech.
        /// </summary>
        public bool AudioSent { get { return _audioSent; } }
        /// <summary>
        /// This delegate is invoked when an error occurs.
        /// </summary>
        public ErrorEvent OnError { get; set; }
        /// <summary>
        /// This property controls which recognize model we use when making recognize requests of the server.
        /// </summary>
        public string RecognizeModel
        {
            get { return _recognizeModel; }
            set
            {
                if (_recognizeModel != value)
                {
                    _recognizeModel = value;
                    StopListening();        // close any active connection when our model is changed.
                }
            }
        }
        /// <summary>
        /// Returns the maximum number of alternatives returned by recognize.
        /// </summary>
        public int MaxAlternatives { get { return _maxAlternatives; } set { _maxAlternatives = value; } }
        /// <summary>
        /// True to return timestamps of words with results.
        /// </summary>
        public bool EnableTimestamps { get { return _timestamps; } set { _timestamps = value; } }
        /// <summary>
        /// True to return word confidence with results.
        /// </summary>
        public bool EnableWordConfidence { get { return _wordConfidence; } set { _wordConfidence = value; } }
        /// <summary>
        /// If true, then we will get interim results while recognizing. The user will then need to check 
        /// the Final flag on the results.
        /// </summary>
        public bool EnableInterimResults { get; set; }
        /// <summary>
        /// If true, then we will try not to send silent audio clips to the server. This can save bandwidth
        /// when no sound is happening.
        /// </summary>
        public bool DetectSilence { get { return _detectSilence; } set { _detectSilence = value; } }
        /// <summary>
        /// A value from 1.0 to 0.0 that determines what is considered silence. If the absolute value of the 
        /// audio level is below this value then we consider it silence.
        /// </summary>
        public float SilenceThreshold
        {
            get { return _silenceThreshold; }
            set
            {
                if (value < 0f || value > 1f)
                {
                    throw new ArgumentOutOfRangeException("Silence threshold should be between 0.0f and 1.0f");
                }
                else
                {
                    _silenceThreshold = value;
                }
            }
        }
        /// <summary>
        /// Gets and sets the endpoint URL for the service.
        /// </summary>
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>
        /// Gets and sets the credentials of the service. Replace the default endpoint if endpoint is defined.
        /// </summary>
        public Credentials Credentials
        {
            get { return _credentials; }
            set
            {
                _credentials = value;
                if (!string.IsNullOrEmpty(_credentials.Url))
                {
                    _url = _credentials.Url;
                }
            }
        }

        /// <summary>
        /// NON-MULTIPART ONLY: Array of keyword strings to spot in the audio. Each keyword string can include one or more tokens. Keywords are spotted only in the final hypothesis, not in interim results. Omit the parameter or specify an empty array if you do not need to spot keywords.
        /// </summary>
        public string[] Keywords { get { return _keywords; } set { _keywords = value; } }
        /// <summary>
        /// NON-MULTIPART ONLY: Confidence value that is the lower bound for spotting a keyword. A word is considered to match a keyword if its confidence is greater than or equal to the threshold. Specify a probability between 0 and 1 inclusive. No keyword spotting is performed if you omit the parameter. If you specify a threshold, you must also specify one or more keywords.
        /// </summary>
        public float? KeywordsThreshold { get { return _keywordsThreshold; } set { _keywordsThreshold = value; } }
        /// <summary>
        /// NON-MULTIPART ONLY: Confidence value that is the lower bound for identifying a hypothesis as a possible word alternative (also known as "Confusion Networks"). An alternative word is considered if its confidence is greater than or equal to the threshold. Specify a probability between 0 and 1 inclusive. No alternative words are computed if you omit the parameter.
        /// </summary>
        public float? WordAlternativesThreshold { get { return _wordAlternativesThreshold; } set { _wordAlternativesThreshold = value; } }
        /// <summary>
        /// NON-MULTIPART ONLY: If true (the default), filters profanity from all output except for keyword results by replacing inappropriate words with a series of asterisks. Set the parameter to false to return results with no censoring. Applies to US English transcription only.
        /// </summary>
        public bool ProfanityFilter { get { return _profanityFilter; } set { _profanityFilter = value; } }
        /// <summary>
        /// NON-MULTIPART ONLY: If true, converts dates, times, series of digits and numbers, phone numbers, currency values, and Internet addresses into more readable, conventional representations in the final transcript of a recognition request. If false (the default), no formatting is performed. Applies to US English transcription only.
        /// </summary>
        public bool SmartFormatting { get { return _smartFormatting; } set { _smartFormatting = value; } }
        /// <summary>
        /// NON-MULTIPART ONLY: Indicates whether labels that identify which words were spoken by which participants in a multi-person exchange are to be included in the response. If true, speaker labels are returned; if false (the default), they are not. Speaker labels can be returned only for the following language models: en-US_NarrowbandModel, en-US_BroadbandModel, es-ES_NarrowbandModel, es-ES_BroadbandModel, ja-JP_NarrowbandModel, and ja-JP_BroadbandModel. Setting speaker_labels to true forces the timestamps parameter to be true, regardless of whether you specify false for the parameter.
        /// </summary>
        public bool SpeakerLabels { get { return _speakerLabels; } set { _speakerLabels = value; } }
        /// <summary>
        /// NON-MULTIPART ONLY: The time in seconds after which, if only silence (no speech) is detected in submitted audio, the connection is closed with a 400 error. Useful for stopping audio submission from a live microphone when a user simply walks away. Use -1 for infinity.
        /// </summary>
        public int InactivityTimeout { get { return _inactivityTimeout; } set { _inactivityTimeout = value; } }
        /// <summary>
        /// Specifies the Globally Unique Identifier (GUID) of a custom language model that is to be used for all requests sent over the connection. The base model of the custom language model must match the value of the model parameter. By default, no custom language model is used. For more information, see https://console.bluemix.net/docs/services/speech-to-text/custom.html.
        /// </summary>
        [Obsolete("Use LanguageCustomizationId instead.")]
        public string CustomizationId { get { return _customization_id; } set { _customization_id = value; } }
        /// <summary>
        /// Specifies the Globally Unique Identifier (GUID) of a custom language model that is to be used for all requests sent over the connection. The base model of the custom language model must match the value of the model parameter. By default, no custom language model is used. For more information, see https://console.bluemix.net/docs/services/speech-to-text/custom.html.
        /// </summary>
        public string LanguageCustomizationId { get { return _languageCustomizationId; } set { _languageCustomizationId = value; } }
        /// <summary>
        /// Specifies the Globally Unique Identifier (GUID) of a custom acoustic model that is to be used for all requests sent over the connection. The base model of the custom acoustic model must match the value of the model parameter. By default, no custom acoustic model is used. For more information, see https://console.bluemix.net/docs/services/speech-to-text/custom.html.
        /// </summary>
        public string AcousticCustomizationId { get { return _acoustic_customization_id; } set { _acoustic_customization_id = value; } }
        /// <summary>
        /// Specifies the weight the service gives to words from a specified custom language model compared to those from the base model for all requests sent over the connection. Specify a value between 0.0 and 1.0; the default value is 0.3. For more information, see https://console.bluemix.net/docs/services/speech-to-text/language-use.html#weight.
        /// </summary>
        public float? CustomizationWeight { get { return _customization_weight; } set { _customization_weight = value; } }
        /// <summary>
        /// If true sets `Transfer-Encoding` request header to `chunked` causing the audio to be streamed to the service. By default, audio is sent all at once as a one-shot delivery. See https://console.bluemix.net/docs/services/speech-to-text/input.html#transmission.
        /// </summary>
        public bool StreamMultipart { get { return _streamMultipart; } set { _streamMultipart = value; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Speech to Text constructor.
        /// </summary>
        /// <param name="credentials">The service credentials.</param>
        public SpeechToText(Credentials credentials)
        {
            if (credentials.HasCredentials() || credentials.HasWatsonAuthenticationToken() || credentials.HasIamTokenData())
            {
                Credentials = credentials;

                if (string.IsNullOrEmpty(credentials.Url))
                {
                    credentials.Url = Url;
                }
            }
            else
            {
                throw new WatsonException("Please provide a username and password or authorization token to use the Speech to Text service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }
        #endregion

        #region Callback delegates
        /// <summary>
        /// Success callback delegate.
        /// </summary>
        /// <typeparam name="T">Type of the returned object.</typeparam>
        /// <param name="response">The returned object.</param>
        /// <param name="customData">user defined custom data including raw json.</param>
        public delegate void SuccessCallback<T>(T response, Dictionary<string, object> customData);
        /// <summary>
        /// Fail callback delegate.
        /// </summary>
        /// <param name="error">The error object.</param>
        /// <param name="customData">User defined custom data</param>
        public delegate void FailCallback(RESTConnector.Error error, Dictionary<string, object> customData);
        #endregion

        #region Get Models
        /// <summary>
        /// This function retrieves all the language models that the user may use by setting the RecognizeModel 
        /// public property.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customData">Custom data.</param>
        /// <returns>Returns true if request has been made.</returns>
        public bool GetModels(SuccessCallback<ModelSet> successCallback, FailCallback failCallback, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/models");
            if (connector == null)
                return false;

            GetModelsRequest req = new GetModelsRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.OnResponse = OnGetModelsResponse;
            return connector.Send(req);
        }

        private class GetModelsRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<ModelSet> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        };

        private void OnGetModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ModelSet result = new ModelSet();
            fsData data = null;
            Dictionary<string, object> customData = ((GetModelsRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = result;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("SpeechToText.OnGetModelsResponse()", "OnGetAuthorsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetModelsRequest)req).SuccessCallback != null)
                    ((GetModelsRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetModelsRequest)req).FailCallback != null)
                    ((GetModelsRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Model
        /// <summary>
        /// This function retrieves a specified languageModel.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="modelID">The name of the model to get.</param>
        /// <param name="customData">Custom data.</param>
        /// <returns>Returns true if request has been made.</returns>
        public bool GetModel(SuccessCallback<Model> successCallback, FailCallback failCallback, string modelID, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(modelID))
                throw new ArgumentNullException("modelID");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/models/" + modelID);
            if (connector == null)
                return false;

            GetModelRequest req = new GetModelRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.OnResponse = OnGetModelResponse;

            return connector.Send(req);
        }

        private class GetModelRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Model> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        };

        private void OnGetModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Model result = new Model();
            fsData data = null;
            Dictionary<string, object> customData = ((GetModelRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = result;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("SpeechToText.OnGetModelResponse()", "OnGetAuthorsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetModelRequest)req).SuccessCallback != null)
                    ((GetModelRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetModelRequest)req).FailCallback != null)
                    ((GetModelRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Sessionless - Streaming
        /// <summary>
        /// This callback object is used by the Recognize() and StartListening() methods.
        /// </summary>
        /// <param name="results">The ResultList object containing the results.</param>
        public delegate void OnRecognize(SpeechRecognitionEvent results, Dictionary<string, object> customData = null);

        /// <summary>
        /// This callback object is used by the RecognizeSpeaker() method.
        /// </summary>
        /// <param name="speakerRecognitionEvent">Array of speaker label results.</param>
        public delegate void OnRecognizeSpeaker(SpeakerRecognitionEvent speakerRecognitionEvent, Dictionary<string, object> customData = null);

        /// <summary>
        /// This starts the service listening and it will invoke the callback for any recognized speech.
        /// OnListen() must be called by the user to queue audio data to send to the service. 
        /// StopListening() should be called when you want to stop listening.
        /// </summary>
        /// <param name="callback">All recognize results are passed to this callback.</param>
        /// <param name="speakerLabelCallback">Speaker label goes through this callback if it arrives separately from recognize result.</param>
        /// <returns>Returns true on success, false on failure.</returns>
        public bool StartListening(OnRecognize callback, OnRecognizeSpeaker speakerLabelCallback = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (_isListening)
                return false;
            if (!CreateListenConnector())
                return false;

            if (customData == null)
                customData = new Dictionary<string, object>();

            Dictionary<string, string> customHeaders = new Dictionary<string, string>();
            if (customData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in customData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    customHeaders.Add(kvp.Key, kvp.Value);
                }
            }

            if (customHeaders != null && _listenSocket != null)
            {
                foreach (KeyValuePair<string, string> kvp in customHeaders)
                    _listenSocket.Headers.Add(kvp.Key, kvp.Value);
            }

            _isListening = true;
            _listenCallback = callback;
            if (speakerLabelCallback != null)
                _speakerLabelCallback = speakerLabelCallback;
            _keepAliveRoutine = Runnable.Run(KeepAlive());
            _lastKeepAlive = DateTime.Now;

            return true;
        }

        /// <summary>
        /// This function should be invoked with the AudioData input after StartListening() method has been invoked.
        /// The user should continue to invoke this function until they are ready to call StopListening(), typically
        /// microphone input is sent to this function.
        /// </summary>
        /// <param name="clip">A AudioData object containing the AudioClip and max level found in the clip.</param>
        /// <returns>True if audio was sent or enqueued, false if audio was discarded.</returns>
        public bool OnListen(AudioData clip)
        {
            bool audioSentOrEnqueued = false;

            if (_isListening)
            {
                if (_recordingHZ < 0)
                {
                    _recordingHZ = clip.Clip.frequency;
                    SendStart();
                }

                // If silence persists for _silenceCutoff seconds, send stop and discard clips until audio resumes
                if (DetectSilence && clip.MaxLevel < _silenceThreshold)
                {
                    _silenceDuration += clip.Clip.length;
                }
                else
                {
                    _silenceDuration = 0.0f;
                }

                if (!DetectSilence || _silenceDuration < _silenceCutoff)
                {
                    if (_listenActive)
                    {
                        _listenSocket.Send(new WSConnector.BinaryMessage(AudioClipUtil.GetL16(clip.Clip)));
                        _audioSent = true;
                        audioSentOrEnqueued = true;
                    }
                    else
                    {
                        // we have not received the "listening" state yet from the server, so just queue
                        // the audio clips until that happens.
                        _listenRecordings.Enqueue(clip);
                        audioSentOrEnqueued = true;

                        // check the length of this queue and do something if it gets too full.
                        if (_listenRecordings.Count > MaxQueuedRecordings)
                        {
                            Log.Error("SpeechToText.OnListen()", "Recording queue is full.");

                            StopListening();
                            if (OnError != null)
                                OnError("Recording queue is full.");
                        }
                    }
                }
                else if (_audioSent)
                {
                    SendStop();
                    _audioSent = false;
                }

                // After sending start, we should get into the listening state within the amount of time specified
                // by LISTEN_TIMEOUT. If not, then stop listening and record the error.
                if (!_listenActive && (DateTime.Now - _lastStartSent).TotalSeconds > ListenTimeout)
                {
                    Log.Error("SpeechToText.OnListen()", "Failed to enter listening state.");

                    StopListening();
                    if (OnError != null)
                        OnError("Failed to enter listening state.");
                }
            }

            return audioSentOrEnqueued;
        }

        /// <summary>
        /// Invoke this function stop this service from listening.
        /// </summary>
        /// <returns>Returns true on success, false on failure.</returns>
        public bool StopListening()
        {
            if (!_isListening)
                return false;

            _isListening = false;
            CloseListenConnector();

            if (_keepAliveRoutine != 0)
            {
                Runnable.Stop(_keepAliveRoutine);
                _keepAliveRoutine = 0;
            }

            _listenRecordings.Clear();
            _listenCallback = null;
            _recordingHZ = -1;

            return true;
        }

        private bool CreateListenConnector()
        {
            if (_listenSocket == null)
            {
                Dictionary<string, string> queryParams = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(CustomizationId))
                {
                    queryParams["customization_id"] = CustomizationId;
                }
                if (!string.IsNullOrEmpty(LanguageCustomizationId))
                {
                    queryParams["language_customization_id"] = LanguageCustomizationId;
                }
                if (!string.IsNullOrEmpty(AcousticCustomizationId))
                {
                    queryParams["acoustic_customization_id"] = AcousticCustomizationId;
                }
                if (CustomizationWeight != null)
                {
                    queryParams["customization_weight"] = CustomizationWeight.ToString();
                }

                string parsedParams = "";

                foreach (KeyValuePair<string, string> kvp in queryParams)
                {
                    parsedParams += string.Format("&{0}={1}", kvp.Key, kvp.Value);
                }

                _listenSocket = WSConnector.CreateConnector(Credentials, "/v1/recognize", "?model=" + WWW.EscapeURL(_recognizeModel) + parsedParams);
                if (_listenSocket == null)
                {
                    return false;
                }
                else
                {
#if ENABLE_DEBUGGING
                    Log.Debug("SpeechToText.CreateListenConnector()", "Created listen socket. Model: {0}, parsedParams: {1}", WWW.EscapeURL(_recognizeModel), parsedParams);
#endif
                }

                _listenSocket.OnMessage = OnListenMessage;
                _listenSocket.OnClose = OnListenClosed;
            }

            return true;
        }

        private void CloseListenConnector()
        {
            if (_listenSocket != null)
            {
                _listenSocket.Close();
                _listenSocket = null;
            }
        }

        private void SendStart()
        {
            if (_listenSocket == null)
                throw new WatsonException("SendStart() called with null connector.");

            Dictionary<string, object> start = new Dictionary<string, object>();
            start["action"] = "start";
            start["content-type"] = "audio/l16;rate=" + _recordingHZ.ToString() + ";channels=1;";
            start["inactivity_timeout"] = InactivityTimeout;
            start["interim_results"] = EnableInterimResults;
            if (Keywords != null)
                start["keywords"] = Keywords;
            if (KeywordsThreshold != null)
                start["keywords_threshold"] = KeywordsThreshold;
            start["max_alternatives"] = MaxAlternatives;
            start["profanity_filter"] = ProfanityFilter;
            start["smart_formatting"] = SmartFormatting;
            start["speaker_labels"] = SpeakerLabels;
            start["timestamps"] = EnableTimestamps;
            if (WordAlternativesThreshold != null)
                start["word_alternatives_threshold"] = WordAlternativesThreshold;
            start["word_confidence"] = EnableWordConfidence;

            _listenSocket.Send(new WSConnector.TextMessage(Json.Serialize(start)));
#if ENABLE_DEBUGGING
            Log.Debug("SpeechToText.SendStart()", "SendStart() with the following params: {0}", Json.Serialize(start));
#endif
            _lastStartSent = DateTime.Now;
        }

        private void SendStop()
        {
            if (_listenSocket == null)
                throw new WatsonException("SendStart() called with null connector.");

            if (_listenActive)
            {
                Dictionary<string, string> stop = new Dictionary<string, string>();
                stop["action"] = "stop";

                _listenSocket.Send(new WSConnector.TextMessage(Json.Serialize(stop)));
                _lastStartSent = DateTime.Now;     // sending stop, will send the listening state again..
                _listenActive = false;
            }
        }

        // This keeps the WebSocket connected when we are not sending any data.
        private IEnumerator KeepAlive()
        {
            while (_listenSocket != null)
            {
                yield return null;

                if ((DateTime.Now - _lastKeepAlive).TotalSeconds > WsKeepAliveInterval)
                {
                    //  Temporary clip to use for KeepAlive
                    //  TODO: Generate small sound clip to send to the service to keep alive.
                    AudioClip _keepAliveClip = Resources.Load<AudioClip>("highHat");

#if ENABLE_DEBUGGING
                    Log.Debug("SpeechToText.KeepAlive()", "Sending keep alive.");
#endif
                    _listenSocket.Send(new WSConnector.BinaryMessage(AudioClipUtil.GetL16(_keepAliveClip)));
                    _keepAliveClip = null;

                    _lastKeepAlive = DateTime.Now;
                }
            }
            Log.Debug("SpeechToText.KeepAlive()", "KeepAlive exited.");
        }

        private void OnListenMessage(WSConnector.Message msg)
        {
            if (msg is WSConnector.TextMessage)
            {
                WSConnector.TextMessage tm = (WSConnector.TextMessage)msg;
                Dictionary<string, object> customData = new Dictionary<string, object>();
                customData.Add(Constants.String.JSON, tm.Text);
                if (tm.Headers != null && tm.Headers.Count > 0)
                    customData.Add(Constants.String.RESPONSE_HEADERS, tm.Headers);

                IDictionary json = Json.Deserialize(tm.Text) as IDictionary;
                if (json != null)
                {
                    if (json.Contains("results"))
                    {
                        SpeechRecognitionEvent results = ParseRecognizeResponse(json);
                        if (results != null)
                        {
                            //// when we get results, start listening for the next block ..
                            if (results.HasFinalResult())
                                Log.Debug("SpeechToText.OnListenMessage()", "final json response: {0}", tm.Text);
                            //    SendStart();

                            if (_listenCallback != null)
                                _listenCallback(results, customData);
                            else
                                StopListening();            // automatically stop listening if our callback is destroyed.
                        }
                        else
                            Log.Error("SpeechToText.OnListenMessage()", "Failed to parse results: {0}", tm.Text);
                    }
                    else if (json.Contains("state"))
                    {
                        string state = (string)json["state"];

#if ENABLE_DEBUGGING
                        Log.Debug("SpeechToText.OnListenMessage()", "Server state is {0}", state);
#endif
                        if (state == "listening")
                        {
                            if (_isListening)
                            {
                                if (!_listenActive)
                                {
                                    _listenActive = true;

                                    // send all pending audio clips ..
                                    while (_listenRecordings.Count > 0)
                                    {
                                        AudioData clip = _listenRecordings.Dequeue();
                                        _listenSocket.Send(new WSConnector.BinaryMessage(AudioClipUtil.GetL16(clip.Clip)));
                                        _audioSent = true;
                                    }
                                }
                            }
                        }

                    }
                    else if (json.Contains("speaker_labels"))
                    {
                        SpeakerRecognitionEvent speakerRecognitionEvent = ParseSpeakerRecognitionResponse(json);
                        if (speakerRecognitionEvent != null)
                        {
                            _speakerLabelCallback(speakerRecognitionEvent);
                        }
                    }
                    else if (json.Contains("error"))
                    {
                        string error = (string)json["error"];
                        Log.Error("SpeechToText.OnListenMessage()", "Error: {0}", error);

                        StopListening();
                        if (OnError != null)
                            OnError(error);
                    }
                    else
                    {
                        Log.Warning("SpeechToText.OnListenMessage()", "Unknown message: {0}", tm.Text);
                    }
                }
                else
                {
                    Log.Error("SpeechToText.OnListenMessage()", "Failed to parse JSON from server: {0}", tm.Text);
                }
            }
        }

        private void OnListenClosed(WSConnector connector)
        {
#if ENABLE_DEBUGGING
            Log.Debug("SpeechToText.OnListenClosed()", "OnListenClosed(), State = {0}", connector.State.ToString());
#endif

            _listenActive = false;
            StopListening();

            if (connector.State == WSConnector.ConnectionState.DISCONNECTED)
            {
                if (OnError != null)
                    OnError("Disconnected from server.");
            }
        }
        #endregion

        #region Sessionless Non-Streaming
        /// <summary>
        /// This function POSTs the given audio clip the recognize function and convert speech into text. This function should be used
        /// only on AudioClips under 4MB once they have been converted into WAV format. Use the StartListening() for continuous
        /// recognition of text.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="clip">The AudioClip object.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool Recognize(SuccessCallback<SpeechRecognitionEvent> successCallback, FailCallback failCallback, AudioClip clip, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (clip == null)
                throw new ArgumentNullException("clip");

            return Recognize(successCallback, failCallback, WaveFile.CreateWAV(clip), "audio/wav", customData);
        }

        /// <summary>
        /// This function POSTs the given audio clip the recognize function and convert speech into text. This function should be used
        /// only on AudioClips under 4MB once they have been converted into WAV format. Use the StartListening() for continuous
        /// recognition of text.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="audioData">The audio data.</param>
        /// <param name="contentType">The content type of the audio data.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool Recognize(SuccessCallback<SpeechRecognitionEvent> successCallback, FailCallback failCallback, byte[] audioData, string contentType, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (audioData == null)
                throw new ArgumentNullException("audioData");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/recognize");
            if (connector == null)
                return false;

            RecognizeRequest req = new RecognizeRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Timeout = int.MaxValue;

            req.Headers["Content-Type"] = contentType;
            if (StreamMultipart)
                req.Headers["Transfer-Encoding"] = "chunked";

            req.Send = audioData;
            if (req.Send.Length > MaxRecognizeClipSize)
            {
                Log.Error("SpeechToText.Recognize()", "AudioClip is too large for Recognize().");
                return false;
            }
            if (!string.IsNullOrEmpty(AcousticCustomizationId))
            {
                req.Parameters["acoustic_customization_id"] = AcousticCustomizationId;
            }
            if (!string.IsNullOrEmpty(CustomizationId))
            {
                req.Parameters["customization_id"] = CustomizationId;
            }
            if (CustomizationWeight != null)
            {
                req.Parameters["customization_weight"] = CustomizationWeight;
            }
            if (!string.IsNullOrEmpty(LanguageCustomizationId))
            {
                req.Parameters["language_customization_id"] = LanguageCustomizationId;
            }
            req.Parameters["inactivity_timeout"] = InactivityTimeout;
            req.Parameters["keywords"] = string.Join(",", Keywords);
            req.Parameters["keywords_threshold"] = KeywordsThreshold;
            req.Parameters["max_alternatives"] = MaxAlternatives.ToString();
            req.Parameters["model"] = RecognizeModel;
            req.Parameters["profanity_filter"] = ProfanityFilter;
            req.Parameters["smart_formatting"] = SmartFormatting;
            req.Parameters["speaker_labels"] = SpeakerLabels;
            req.Parameters["timestamps"] = EnableTimestamps ? "true" : "false";
            if (Credentials.HasWatsonAuthenticationToken())
                req.Parameters["watson-token"] = Credentials.WatsonAuthenticationToken;
            if (WordAlternativesThreshold != null)
                req.Parameters["word_alternatives_threshold"] = WordAlternativesThreshold;
            req.Parameters["word_confidence"] = EnableWordConfidence ? "true" : "false";

            req.OnResponse = OnRecognizeResponse;

            return connector.Send(req);
        }

        private class RecognizeRequest : RESTConnector.Request
        {
            //public byte[] AudioData { get; set; }
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<SpeechRecognitionEvent> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        };

        private void OnRecognizeResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            SpeechRecognitionEvent result = null;
            fsData data = null;
            Dictionary<string, object> customData = ((RecognizeRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    result = ParseRecognizeResponse(resp.Data);
                    if (result == null)
                    {
                        Log.Error("SpeechToText.OnRecognizeResponse()", "Failed to parse json response: {0}",
                            resp.Data != null ? Encoding.UTF8.GetString(resp.Data) : "");
                    }
                    else
                    {
                        Log.Status("SpeechToText.OnRecognizeResponse()", "Received Recognize Response, Elapsed Time: {0}, Results: {1}",
                            resp.ElapsedTime, result.results.Length);
                    }

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("SpeechToText.OnRecognizeResponse()", "OnGetAuthorsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }

            }
            else
            {
                Log.Error("SpeechToText.OnRecognizeResponse()", "Recognize Error: {0}", resp.Error);
                resp.Success = false;
            }

            if (resp.Success)
            {
                if (((RecognizeRequest)req).SuccessCallback != null)
                    ((RecognizeRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((RecognizeRequest)req).FailCallback != null)
                    ((RecognizeRequest)req).FailCallback(resp.Error, customData);
            }
        }

        private SpeechRecognitionEvent ParseRecognizeResponse(byte[] json)
        {
            string jsonString = Encoding.UTF8.GetString(json);
            if (jsonString == null)
                return null;

            IDictionary resp = (IDictionary)Json.Deserialize(jsonString);
            if (resp == null)
                return null;

            return ParseRecognizeResponse(resp);
        }

        private SpeakerRecognitionEvent ParseSpeakerRecognitionResponse(IDictionary resp)
        {
            if (resp == null)
                return null;

            try
            {
                List<SpeakerLabelsResult> results = new List<SpeakerLabelsResult>();
                IList iresults = resp["speaker_labels"] as IList;

                if (iresults == null)
                    return null;

                foreach (var r in iresults)
                {
                    IDictionary iresult = r as IDictionary;
                    if (iresult == null)
                        continue;

                    SpeakerLabelsResult result = new SpeakerLabelsResult();
                    result.confidence = (double)iresult["confidence"];
                    result.final = (bool)iresult["final"];
                    result.from = (double)iresult["from"];
                    result.to = (double)iresult["to"];
                    result.speaker = (Int64)iresult["speaker"];

                    results.Add(result);
                }
                SpeakerRecognitionEvent speakerRecognitionEvent = new SpeakerRecognitionEvent();
                speakerRecognitionEvent.speaker_labels = results.ToArray();
                return (speakerRecognitionEvent);
            }
            catch (Exception e)
            {
                Log.Error("SpeechToText.ParseSpeakerRecognitionResponse()", "ParseSpeakerRecognitionResponse exception: {0}", e.ToString());
                return null;
            }
        }

        private SpeechRecognitionEvent ParseRecognizeResponse(IDictionary resp)
        {
            if (resp == null)
                return null;

            try
            {
                List<SpeechRecognitionResult> results = new List<SpeechRecognitionResult>();
                IList iresults = resp["results"] as IList;
                if (iresults == null)
                    return null;

                foreach (var r in iresults)
                {
                    IDictionary iresult = r as IDictionary;
                    if (iresults == null)
                        continue;

                    SpeechRecognitionResult result = new SpeechRecognitionResult();
                    result.final = (bool)iresult["final"];

                    IList iwordAlternatives = iresult["word_alternatives"] as IList;
                    if (iwordAlternatives != null)
                    {

                        List<WordAlternativeResults> wordAlternatives = new List<WordAlternativeResults>();
                        foreach (var w in iwordAlternatives)
                        {
                            IDictionary iwordAlternative = w as IDictionary;
                            if (iwordAlternative == null)
                                continue;

                            WordAlternativeResults wordAlternativeResults = new WordAlternativeResults();
                            if (iwordAlternative.Contains("start_time"))
                                wordAlternativeResults.start_time = (double)iwordAlternative["start_time"];
                            if (iwordAlternative.Contains("end_time"))
                                wordAlternativeResults.end_time = (double)iwordAlternative["end_time"];
                            if (iwordAlternative.Contains("alternatives"))
                            {
                                List<WordAlternativeResult> wordAlternativeResultList = new List<WordAlternativeResult>();
                                IList iwordAlternativeResult = iwordAlternative["alternatives"] as IList;
                                if (iwordAlternativeResult == null)
                                    continue;

                                foreach (var a in iwordAlternativeResult)
                                {
                                    WordAlternativeResult wordAlternativeResult = new WordAlternativeResult();
                                    IDictionary ialternative = a as IDictionary;
                                    if (ialternative.Contains("word"))
                                        wordAlternativeResult.word = (string)ialternative["word"];
                                    if (ialternative.Contains("confidence"))
                                        wordAlternativeResult.confidence = (double)ialternative["confidence"];
                                    wordAlternativeResultList.Add(wordAlternativeResult);
                                }

                                wordAlternativeResults.alternatives = wordAlternativeResultList.ToArray();
                            }

                            wordAlternatives.Add(wordAlternativeResults);
                        }

                        result.word_alternatives = wordAlternatives.ToArray();
                    }

                    IList ialternatives = iresult["alternatives"] as IList;
                    if (ialternatives != null)
                    {

                        List<SpeechRecognitionAlternative> alternatives = new List<SpeechRecognitionAlternative>();
                        foreach (var a in ialternatives)
                        {
                            IDictionary ialternative = a as IDictionary;
                            if (ialternative == null)
                                continue;

                            SpeechRecognitionAlternative alternative = new SpeechRecognitionAlternative();
                            alternative.transcript = (string)ialternative["transcript"];
                            if (ialternative.Contains("confidence"))
                                alternative.confidence = (double)ialternative["confidence"];

                            if (ialternative.Contains("timestamps"))
                            {
                                IList itimestamps = ialternative["timestamps"] as IList;

                                TimeStamp[] timestamps = new TimeStamp[itimestamps.Count];
                                for (int i = 0; i < itimestamps.Count; ++i)
                                {
                                    IList itimestamp = itimestamps[i] as IList;
                                    if (itimestamp == null)
                                        continue;

                                    TimeStamp ts = new TimeStamp();
                                    ts.Word = (string)itimestamp[0];
                                    ts.Start = (double)itimestamp[1];
                                    ts.End = (double)itimestamp[2];
                                    timestamps[i] = ts;
                                }

                                alternative.Timestamps = timestamps;
                            }
                            if (ialternative.Contains("word_confidence"))
                            {
                                IList iconfidence = ialternative["word_confidence"] as IList;

                                WordConfidence[] confidence = new WordConfidence[iconfidence.Count];
                                for (int i = 0; i < iconfidence.Count; ++i)
                                {
                                    IList iwordconf = iconfidence[i] as IList;
                                    if (iwordconf == null)
                                        continue;

                                    WordConfidence wc = new WordConfidence();
                                    wc.Word = (string)iwordconf[0];
                                    wc.Confidence = (double)iwordconf[1];
                                    confidence[i] = wc;
                                }

                                alternative.WordConfidence = confidence;
                            }

                            alternatives.Add(alternative);
                        }

                        result.alternatives = alternatives.ToArray();
                    }

                    IDictionary iKeywords = iresult["keywords_result"] as IDictionary;
                    if (iKeywords != null)
                    {
                        result.keywords_result = new KeywordResults();
                        List<KeywordResult> keywordResults = new List<KeywordResult>();
                        foreach (string keyword in Keywords)
                        {
                            if (iKeywords[keyword] != null)
                            {
                                IList iKeywordList = iKeywords[keyword] as IList;
                                if (iKeywordList == null)
                                    continue;

                                foreach (var k in iKeywordList)
                                {
                                    IDictionary iKeywordDictionary = k as IDictionary;
                                    KeywordResult keywordResult = new KeywordResult();
                                    keywordResult.keyword = keyword;
                                    keywordResult.confidence = (double)iKeywordDictionary["confidence"];
                                    keywordResult.end_time = (double)iKeywordDictionary["end_time"];
                                    keywordResult.start_time = (double)iKeywordDictionary["start_time"];
                                    keywordResult.normalized_text = (string)iKeywordDictionary["normalized_text"];
                                    keywordResults.Add(keywordResult);
                                }
                            }
                        }
                        result.keywords_result.keyword = keywordResults.ToArray();
                    }

                    results.Add(result);
                }

                SpeechRecognitionEvent speechRecognitionEvent = new SpeechRecognitionEvent(results.ToArray());
                int resultIndex;
                if (int.TryParse(resp["result_index"].ToString(), out resultIndex))
                    speechRecognitionEvent.result_index = resultIndex;

                return speechRecognitionEvent;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToText.ParseRecognizeResponse()", "ParseJsonResponse exception: {0}", e.ToString());
                return null;
            }
        }
        #endregion

        #region Asynchronous
        #endregion

        #region Get Custom Models
        /// <summary>
        /// Lists information about all custom language models that are owned by the calling user. Use the language query parameter to see all custom models for the specified language; omit the parameter to see all custom models for all languages.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="language">The language for which custom models are to be returned. Currently, only en-US (the default) is supported.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomizations(SuccessCallback<Customizations> successCallback, FailCallback failCallback, string language = "en-US", Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetCustomizationsReq req = new GetCustomizationsReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Parameters["language"] = language;
            req.OnResponse = OnGetCustomizationsResp;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/customizations");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomizationsReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Customizations> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetCustomizationsResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Customizations result = new Customizations();
            fsData data = null;
            Dictionary<string, object> customData = ((GetCustomizationsReq)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = result;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("SpeechToText.OnGetCustomizationsResp()", "GetCustomizations Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetCustomizationsReq)req).SuccessCallback != null)
                    ((GetCustomizationsReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetCustomizationsReq)req).FailCallback != null)
                    ((GetCustomizationsReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Create Custom Model
        /// <summary>
        /// Creates a new custom language model for a specified base language model. The custom language model can be used only with the base language model for which it is created. The new model is owned by the individual whose service credentials are used to create it.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="name">The custom model name.</param>
        /// <param name="base_model_name">The base model name - only en-US_BroadbandModel is currently supported.</param>
        /// <param name="description">Descripotion of the custom model.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool CreateCustomization(SuccessCallback<CustomizationID> successCallback, FailCallback failCallback, string name, string base_model_name = "en-US_BroadbandModel", string description = default(string), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("A name is required to create a custom language model.");

            CustomLanguage customLanguage = new CustomLanguage();
            customLanguage.name = name;
            customLanguage.base_model_name = base_model_name;
            customLanguage.description = string.IsNullOrEmpty(description) ? name : description;

            fsData data;
            _serializer.TrySerialize(customLanguage.GetType(), customLanguage, out data).AssertSuccessWithoutWarnings();
            string customizationJson = fsJsonPrinter.CompressedJson(data);

            CreateCustomizationRequest req = new CreateCustomizationRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Send = Encoding.UTF8.GetBytes(customizationJson);
            req.OnResponse = OnCreateCustomizationResp;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/customizations");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class CreateCustomizationRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<CustomizationID> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnCreateCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            CustomizationID result = new CustomizationID();
            fsData data = null;
            Dictionary<string, object> customData = ((CreateCustomizationRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = result;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("SpeechToText.OnCreateCustomizationResp()", "CreateCustomization Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((CreateCustomizationRequest)req).SuccessCallback != null)
                    ((CreateCustomizationRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((CreateCustomizationRequest)req).FailCallback != null)
                    ((CreateCustomizationRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Delete Custom Model
        /// <summary>
        /// Deletes an existing custom language model. Only the owner of a custom model can use this method to delete the model.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The customization ID to be deleted.</param>
        /// <param name="customData">Optional customization data.</param>
        /// <returns></returns>
        public bool DeleteCustomization(SuccessCallback<bool> successCallback, FailCallback failCallback, string customizationID, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to delete is required for DeleteCustomization");

            DeleteCustomizationRequest req = new DeleteCustomizationRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Delete = true;
            req.OnResponse = OnDeleteCustomizationResp;

            string service = "/v1/customizations/{0}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteCustomizationRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<bool> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnDeleteCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((DeleteCustomizationRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((DeleteCustomizationRequest)req).SuccessCallback != null)
                    ((DeleteCustomizationRequest)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((DeleteCustomizationRequest)req).FailCallback != null)
                    ((DeleteCustomizationRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Custom Model
        /// <summary>
        /// Lists information about a custom language model. Only the owner of a custom model can use this method to query information about the model.
        ///	Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The requested custom language model's identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomization(SuccessCallback<Customization> successCallback, FailCallback failCallback, string customizationID, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to get a custom language model.");

            GetCustomizationRequest req = new GetCustomizationRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.OnResponse = OnGetCustomizationResp;

            string service = "/v1/customizations/{0}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomizationRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Customization> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Customization result = new Customization();
            fsData data = null;
            Dictionary<string, object> customData = ((GetCustomizationRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = result;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("SpeechToText.OnGetCustomizationResp()", "GetCustomization Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetCustomizationRequest)req).SuccessCallback != null)
                    ((GetCustomizationRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetCustomizationRequest)req).FailCallback != null)
                    ((GetCustomizationRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Train Custom Model
        /// <summary>
        /// Initiates the training of a custom language model with new corpora, words, or both.After adding training data to the custom model with the corpora or words methods, use this method to begin the actual training of the model on the new data.You can specify whether the custom model is to be trained with all words from its words resources or only with words that were added or modified by the user.Only the owner of a custom model can use this method to train the model.
        /// This method is asynchronous and can take on the order of minutes to complete depending on the amount of data on which the service is being trained and the current load on the service.The method returns an HTTP 200 response code to indicate that the training process has begun.
        /// You can monitor the status of the training by using the GET /v1/customizations/{customization_id} method to poll the model's status. Use a loop to check the status every 10 seconds. The method returns a Customization object that includes status and progress fields. A status of available means that the custom model is trained and ready to use. If training is in progress, the progress field indicates the progress of the training as a percentage complete.
        /// Note: For this beta release, the progress field does not reflect the current progress of the training. The field changes from 0 to 100 when training is complete.
        /// Training can fail to start for the following reasons:
        /// No training data (corpora or words) have been added to the custom model.
        /// Pre-processing of corpora to generate a list of out-of-vocabulary (OOV) words is not complete.
        /// Pre-processing of words to validate or auto-generate sounds-like pronunciations is not complete.
        /// One or more words that were added to the custom model have invalid sounds-like pronunciations that you must fix.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The customization ID with the corpus to be deleted.</param>
        /// <param name="wordTypeToAdd">The word type.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool TrainCustomization(SuccessCallback<bool> successCallback, FailCallback failCallback, string customizationID, string wordTypeToAdd = WordTypeToAdd.All, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to train a custom language model.");

            TrainCustomizationRequest req = new TrainCustomizationRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Parameters["word_type_to_add"] = wordTypeToAdd;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Send = Encoding.UTF8.GetBytes("{}");
            req.OnResponse = OnTrainCustomizationResp;

            string service = "/v1/customizations/{0}/train";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class TrainCustomizationRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<bool> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnTrainCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((TrainCustomizationRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((TrainCustomizationRequest)req).SuccessCallback != null)
                    ((TrainCustomizationRequest)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((TrainCustomizationRequest)req).FailCallback != null)
                    ((TrainCustomizationRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Reset Custom Model
        /// <summary>
        /// Resets a custom language model by removing all corpora and words from the model.Resetting a custom model initializes the model to its state when it was first created. Metadata such as the name and language of the model are preserved.Only the owner of a custom model can use this method to reset the model.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The requested custom language model's identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool ResetCustomization(SuccessCallback<bool> successCallback, FailCallback failCallback, string customizationID, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to train a reset language model.");

            ResetCustomizationRequest req = new ResetCustomizationRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Send = Encoding.UTF8.GetBytes("{}");
            req.OnResponse = OnResetCustomizationResp;

            string service = "/v1/customizations/{0}/reset";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ResetCustomizationRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<bool> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnResetCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((ResetCustomizationRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((ResetCustomizationRequest)req).SuccessCallback != null)
                    ((ResetCustomizationRequest)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((ResetCustomizationRequest)req).FailCallback != null)
                    ((ResetCustomizationRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Upgrade Custom Model
        /// <summary>
        /// Upgrades a custom language model to the latest release level of the Speech to Text service. The method bases the upgrade on the latest trained data stored for the custom model. If the corpora or words for the model have changed since the model was last trained, you must use the POST /v1/customizations/{customization_id}/train method to train the model on the new data. Only the owner of a custom model can use this method to upgrade the model.
        /// Note: This method is not currently implemented.It will be added for a future release of the API.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The requested custom language model's identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool UpgradeCustomization(SuccessCallback<bool> successCallback, FailCallback failCallback, string customizationID, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to upgrade a custom language model.");

            UpgradeCustomizationRequest req = new UpgradeCustomizationRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Send = Encoding.UTF8.GetBytes("{}");
            req.OnResponse = OnResetCustomizationResp;

            string service = "/v1/customizations/{0}/upgrade";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class UpgradeCustomizationRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<bool> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnUpgradeCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((UpgradeCustomizationRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((UpgradeCustomizationRequest)req).SuccessCallback != null)
                    ((UpgradeCustomizationRequest)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((UpgradeCustomizationRequest)req).FailCallback != null)
                    ((UpgradeCustomizationRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Custom Corpora

        /// <summary>
        /// Lists information about all corpora that have been added to the specified custom language model. The information includes the total number of words and out-of-vocabulary (OOV) words, name, and status of each corpus. Only the owner of a custom model can use this method to list the model's corpora.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The identifier of the customization you would like to add the corpora to.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomCorpora(SuccessCallback<Corpora> successCallback, FailCallback failCallback, string customizationID, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID is required to GetCustomCorpora");

            GetCustomCorporaReq req = new GetCustomCorporaReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.OnResponse = OnGetCustomCorporaResp;

            string service = "/v1/customizations/{0}/corpora";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomCorporaReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Corpora> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetCustomCorporaResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Corpora result = new Corpora();
            fsData data = null;
            Dictionary<string, object> customData = ((GetCustomCorporaReq)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = result;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("SpeechToText.OnGetCustomCorporaResp()", "OnGetCustomCorporaResp Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetCustomCorporaReq)req).SuccessCallback != null)
                    ((GetCustomCorporaReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetCustomCorporaReq)req).FailCallback != null)
                    ((GetCustomCorporaReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Custom Corpus

        /// <summary>
        /// Lists information about all corpora that have been added to the specified custom language model. The information includes the total number of words and out-of-vocabulary (OOV) words, name, and status of each corpus. Only the owner of a custom model can use this method to list the model's corpora.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The identifier of the customization you would like to get the custom corpus from.</param>
        /// <param name="corpusName">The name of the custom corpus to be returned.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomCorpus(SuccessCallback<Corpus> successCallback, FailCallback failCallback, string customizationID, string corpusName, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID is required to GetCustomCorpora");
            if (string.IsNullOrEmpty(corpusName))
                throw new ArgumentNullException("A corpusName is required to GetCustomCorpora");

            GetCustomCorpusReq req = new GetCustomCorpusReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.OnResponse = OnGetCustomCorpusResp;

            string service = "/v1/customizations/{0}/corpora/{1}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID, corpusName));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomCorpusReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Corpus> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetCustomCorpusResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Corpus result = new Corpus();
            fsData data = null;
            Dictionary<string, object> customData = ((GetCustomCorpusReq)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = result;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("SpeechToText.OnGetCustomCorpusResp()", "OnGetCustomCorpusResp Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetCustomCorpusReq)req).SuccessCallback != null)
                    ((GetCustomCorpusReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetCustomCorpusReq)req).FailCallback != null)
                    ((GetCustomCorpusReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Delete Custom Corpus
        /// <summary>
        /// Deletes an existing corpus from a custom language model. The service removes any out-of-vocabulary (OOV) words associated with the corpus from the custom model's words resource unless they were also added by another corpus or they have been modified in some way with the POST /v1/customizations/{customization_id}/words or PUT /v1/customizations/{customization_id}/words/{word_name} method. Removing a corpus does not affect the custom model until you train the model with the POST /v1/customizations/{customization_id}/train method. Only the owner of a custom model can use this method to delete a corpus from the model.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The customization ID with the corpus to be deleted.</param>
        /// <param name="corpusName">The corpus name to be deleted.</param>
        /// <param name="customData">Optional customization data.</param>
        /// <returns></returns>
        public bool DeleteCustomCorpus(SuccessCallback<bool> successCallback, FailCallback failCallback, string customizationID, string corpusName, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID is required for DeleteCustomCorpora.");
            if (string.IsNullOrEmpty(corpusName))
                throw new ArgumentNullException("A corpusName to delete is required to DeleteCustomCorpora.");

            DeleteCustomCorpusRequest req = new DeleteCustomCorpusRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Delete = true;
            req.OnResponse = OnDeleteCustomCorpusResp;

            string service = "/v1/customizations/{0}/corpora/{1}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID, corpusName));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteCustomCorpusRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<bool> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnDeleteCustomCorpusResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((DeleteCustomCorpusRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((DeleteCustomCorpusRequest)req).SuccessCallback != null)
                    ((DeleteCustomCorpusRequest)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((DeleteCustomCorpusRequest)req).FailCallback != null)
                    ((DeleteCustomCorpusRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Add Custom Coprpus
        /// <summary>
        /// Overload method for AddCustomCorpus that takes string training data.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The customization ID with the corpus to be deleted.</param>
        /// <param name="corpusName">The corpus name to be deleted.</param>
        /// <param name="allowOverwrite">Allow overwriting of corpus data.</param>
        /// <param name="trainingData">String data for training data.</param>
        /// <param name="customData">Optional customization data.</param>
        public bool AddCustomCorpus(SuccessCallback<bool> successCallback, FailCallback failCallback, string customizationID, string corpusName, bool allowOverwrite, string trainingData, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID is required for AddCustomCorpus.");
            if (string.IsNullOrEmpty(corpusName))
                throw new ArgumentNullException("A corpusName is requried for AddCustomCorpus.");
            if (string.IsNullOrEmpty(trainingData))
                throw new ArgumentNullException("Training data is required for AddCustomCorpus.");

            AddCustomCorpusRequest req = new AddCustomCorpusRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Headers["Accept"] = "application/json";
            req.Parameters["allow_overwrite"] = allowOverwrite.ToString();
            req.Send = Encoding.UTF8.GetBytes(trainingData);
            req.OnResponse = OnAddCustomCorpusResp;

            string service = "/v1/customizations/{0}/corpora/{1}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID, corpusName));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class AddCustomCorpusRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<bool> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnAddCustomCorpusResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((AddCustomCorpusRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((AddCustomCorpusRequest)req).SuccessCallback != null)
                    ((AddCustomCorpusRequest)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((AddCustomCorpusRequest)req).FailCallback != null)
                    ((AddCustomCorpusRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Custom Words

        /// <summary>
        /// Lists information about all custom words from a custom language model. You can list all words from the custom model's words resource, only custom words that were added or modified by the user, or only OOV words that were extracted from corpora. Only the owner of a custom model can use this method to query the words from the model.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The customization ID with words you would like to get.</param>
        /// <param name="wordType">The type of the word.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomWords(SuccessCallback<WordsList> successCallback, FailCallback failCallback, string customizationID, string wordType = WordType.All, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("customizationID");

            GetCustomWordsReq req = new GetCustomWordsReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Parameters["word_type"] = wordType.ToString();
            req.OnResponse = OnGetCustomWordsResp;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/words", customizationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomWordsReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<WordsList> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetCustomWordsResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WordsList result = new WordsList();
            fsData data = null;
            Dictionary<string, object> customData = ((GetCustomWordsReq)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = result;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("SpeechToText.OnGetCustomWordsResp()", "OnGetCustomWordsResp Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetCustomWordsReq)req).SuccessCallback != null)
                    ((GetCustomWordsReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetCustomWordsReq)req).FailCallback != null)
                    ((GetCustomWordsReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Add Custom Words
        /// <summary>
        /// Adds one or more custom words to a custom language model.The service populates the words resource for a custom model with out-of-vocabulary(OOV) words found in each corpus added to the model.You can use this method to add additional words or to modify existing words in the words resource.Only the owner of a custom model can use this method to add or modify custom words associated with the model.Adding or modifying custom words does not affect the custom model until you train the model for the new data by using the POST /v1/customizations/{customization_id}/train method.
        /// You add custom words by providing a Words object, which is an array of Word objects, one per word.You must use the object's word parameter to identify the word that is to be added. You can also provide one or both of the optional sounds_like and display_as fields for each word.
        /// The sounds_like field provides an array of one or more pronunciations for the word. Use the parameter to specify how the word can be pronounced by users.Use the parameter for words that are difficult to pronounce, foreign words, acronyms, and so on.For example, you might specify that the word IEEE can sound like i triple e.You can specify a maximum of five sounds-like pronunciations for a word, and each pronunciation must adhere to the following rules:
        /// Use English alphabetic characters: a-z and A-Z.
        /// To pronounce a single letter, use the letter followed by a period, for example, N.C.A.A. for the word NCAA.
        /// Use real or made-up words that are pronounceable in the native language, for example, shuchensnie for the word Sczcesny.
        /// Substitute equivalent English letters for non-English letters, for example, s for ç or ny for ñ.
        /// Substitute non-accented letters for accented letters, for example a for à or e for è.
        /// Use the spelling of numbers, for example, seventy-five for 75.
        /// You can include multiple words separated by spaces, but the service enforces a maximum of 40 total characters not including spaces.
        /// The display_as field provides a different way of spelling the word in a transcript. Use the parameter when you want the word to appear different from its usual representation or from its spelling in corpora training data.For example, you might indicate that the word IBM(trademark) is to be displayed as IBM™.
        /// If you add a custom word that already exists in the words resource for the custom model, the new definition overrides the existing data for the word.If the service encounters an error with the input data, it returns a failure code and does not add any of the words to the words resource.
        /// The call returns an HTTP 201 response code if the input data is valid.It then asynchronously pre-processes the words to add them to the model's words resource. The time that it takes for the analysis to complete depends on the number of new words that you add but is generally faster than adding a corpus or training a model.
        /// You can use the GET /v1/customizations/{ customization_id}/words or GET /v1/customizations/{customization_id}/words/{word_name} method to review the words that you add.Words with an invalid sounds_like field include an error field that describes the problem.You can use other words methods to correct errors, eliminate typos, and modify how words are pronounced as needed.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The requested custom language model's identifier.</param>
        /// <param name="useDataPath">A boolean used to differentiate overloads with identical input types..</param>
        /// <param name="wordsJsonPath">A path to a json file to train.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool AddCustomWords(SuccessCallback<bool> successCallback, FailCallback failCallback, string customizationID, bool useDataPath, string wordsJsonPath, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to add words to a custom language model.");
            if (string.IsNullOrEmpty(wordsJsonPath))
                throw new ArgumentNullException("A wordsJsonPath is required to add words to a custom language model.");

            string wordsJson = File.ReadAllText(wordsJsonPath);

            return AddCustomWords(successCallback, failCallback, customizationID, wordsJson);
        }

        /// <summary>
        /// Adds one or more custom words to a custom language model.The service populates the words resource for a custom model with out-of-vocabulary(OOV) words found in each corpus added to the model.You can use this method to add additional words or to modify existing words in the words resource.Only the owner of a custom model can use this method to add or modify custom words associated with the model.Adding or modifying custom words does not affect the custom model until you train the model for the new data by using the POST /v1/customizations/{customization_id}/train method.
        /// You add custom words by providing a Words object, which is an array of Word objects, one per word.You must use the object's word parameter to identify the word that is to be added. You can also provide one or both of the optional sounds_like and display_as fields for each word.
        /// The sounds_like field provides an array of one or more pronunciations for the word. Use the parameter to specify how the word can be pronounced by users.Use the parameter for words that are difficult to pronounce, foreign words, acronyms, and so on.For example, you might specify that the word IEEE can sound like i triple e.You can specify a maximum of five sounds-like pronunciations for a word, and each pronunciation must adhere to the following rules:
        /// Use English alphabetic characters: a-z and A-Z.
        /// To pronounce a single letter, use the letter followed by a period, for example, N.C.A.A. for the word NCAA.
        /// Use real or made-up words that are pronounceable in the native language, for example, shuchensnie for the word Sczcesny.
        /// Substitute equivalent English letters for non-English letters, for example, s for ç or ny for ñ.
        /// Substitute non-accented letters for accented letters, for example a for à or e for è.
        /// Use the spelling of numbers, for example, seventy-five for 75.
        /// You can include multiple words separated by spaces, but the service enforces a maximum of 40 total characters not including spaces.
        /// The display_as field provides a different way of spelling the word in a transcript. Use the parameter when you want the word to appear different from its usual representation or from its spelling in corpora training data.For example, you might indicate that the word IBM(trademark) is to be displayed as IBM™.
        /// If you add a custom word that already exists in the words resource for the custom model, the new definition overrides the existing data for the word.If the service encounters an error with the input data, it returns a failure code and does not add any of the words to the words resource.
        /// The call returns an HTTP 201 response code if the input data is valid.It then asynchronously pre-processes the words to add them to the model's words resource. The time that it takes for the analysis to complete depends on the number of new words that you add but is generally faster than adding a corpus or training a model.
        /// You can use the GET /v1/customizations/{ customization_id}/words or GET /v1/customizations/{customization_id}/words/{word_name} method to review the words that you add.Words with an invalid sounds_like field include an error field that describes the problem.You can use other words methods to correct errors, eliminate typos, and modify how words are pronounced as needed.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The requested custom language model's identifier.</param>
        /// <param name="words">Custom words.</param>
        /// <param name="customData">Optional custom data</param>
        /// <returns></returns>
        public bool AddCustomWords(SuccessCallback<bool> successCallback, FailCallback failCallback, string customizationID, Words words, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to add words to a custom language model.");
            if (words == null || words.words == null || words.words.Length == 0)
                throw new WatsonException("Custom words are required to add words to a custom language model.");

            fsData data;
            _serializer.TrySerialize(words.GetType(), words, out data).AssertSuccessWithoutWarnings();
            string wordsJson = fsJsonPrinter.CompressedJson(data);

            return AddCustomWords(successCallback, failCallback, customizationID, wordsJson, customData);
        }

        /// <summary>
        /// Adds one or more custom words to a custom language model.The service populates the words resource for a custom model with out-of-vocabulary(OOV) words found in each corpus added to the model.You can use this method to add additional words or to modify existing words in the words resource.Only the owner of a custom model can use this method to add or modify custom words associated with the model.Adding or modifying custom words does not affect the custom model until you train the model for the new data by using the POST /v1/customizations/{customization_id}/train method.
        /// You add custom words by providing a Words object, which is an array of Word objects, one per word.You must use the object's word parameter to identify the word that is to be added. You can also provide one or both of the optional sounds_like and display_as fields for each word.
        /// The sounds_like field provides an array of one or more pronunciations for the word. Use the parameter to specify how the word can be pronounced by users.Use the parameter for words that are difficult to pronounce, foreign words, acronyms, and so on.For example, you might specify that the word IEEE can sound like i triple e.You can specify a maximum of five sounds-like pronunciations for a word, and each pronunciation must adhere to the following rules:
        /// Use English alphabetic characters: a-z and A-Z.
        /// To pronounce a single letter, use the letter followed by a period, for example, N.C.A.A. for the word NCAA.
        /// Use real or made-up words that are pronounceable in the native language, for example, shuchensnie for the word Sczcesny.
        /// Substitute equivalent English letters for non-English letters, for example, s for ç or ny for ñ.
        /// Substitute non-accented letters for accented letters, for example a for à or e for è.
        /// Use the spelling of numbers, for example, seventy-five for 75.
        /// You can include multiple words separated by spaces, but the service enforces a maximum of 40 total characters not including spaces.
        /// The display_as field provides a different way of spelling the word in a transcript. Use the parameter when you want the word to appear different from its usual representation or from its spelling in corpora training data.For example, you might indicate that the word IBM(trademark) is to be displayed as IBM™.
        /// If you add a custom word that already exists in the words resource for the custom model, the new definition overrides the existing data for the word.If the service encounters an error with the input data, it returns a failure code and does not add any of the words to the words resource.
        /// The call returns an HTTP 201 response code if the input data is valid.It then asynchronously pre-processes the words to add them to the model's words resource. The time that it takes for the analysis to complete depends on the number of new words that you add but is generally faster than adding a corpus or training a model.
        /// You can use the GET /v1/customizations/{ customization_id}/words or GET /v1/customizations/{customization_id}/words/{word_name} method to review the words that you add.Words with an invalid sounds_like field include an error field that describes the problem.You can use other words methods to correct errors, eliminate typos, and modify how words are pronounced as needed.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The requested custom language model's identifier.</param>
        /// <param name="wordsJson">Json string of custom words object.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool AddCustomWords(SuccessCallback<bool> successCallback, FailCallback failCallback, string customizationID, string wordsJson, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to add words to a custom language model.");
            if (string.IsNullOrEmpty(wordsJson))
                throw new ArgumentNullException("A wordsJsonPath is required to add words to a custom language model.");

            AddCustomWordsRequest req = new AddCustomWordsRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Send = Encoding.UTF8.GetBytes(wordsJson);
            req.OnResponse = OnAddCustomWordsResp;

            string service = "/v1/customizations/{0}/words";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class AddCustomWordsRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<bool> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnAddCustomWordsResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((AddCustomWordsRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((AddCustomWordsRequest)req).SuccessCallback != null)
                    ((AddCustomWordsRequest)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((AddCustomWordsRequest)req).FailCallback != null)
                    ((AddCustomWordsRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Delete Custom Word
        /// <summary>
        /// Deletes a custom word from a custom language model. You can remove any word that you added to the custom model's words resource via any means. However, if the word also exists in the service's base vocabulary, the service removes only the custom pronunciation for the word; the word remains in the base vocabulary.
        /// Removing a custom word does not affect the custom model until you train the model with the POST /v1/customizations/{customization_id}/train method.Only the owner of a custom model can use this method to delete a word from the model.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The customization ID with the corpus to be deleted.</param>
        /// <param name="word">The word to be deleted.</param>
        /// <param name="customData">Optional customization data.</param>
        /// <returns></returns>
        public bool DeleteCustomWord(SuccessCallback<bool> successCallback, FailCallback failCallback, string customizationID, string word, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID is required for DeleteCustomWord");
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("A word to delete is requried for DeleteCustomWord");

            DeleteCustomWordRequest req = new DeleteCustomWordRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Delete = true;
            req.OnResponse = OnDeleteCustomWordResp;

            string service = "/v1/customizations/{0}/words/{1}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID, word));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteCustomWordRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<bool> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnDeleteCustomWordResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((DeleteCustomWordRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((DeleteCustomWordRequest)req).SuccessCallback != null)
                    ((DeleteCustomWordRequest)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((DeleteCustomWordRequest)req).FailCallback != null)
                    ((DeleteCustomWordRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Custom Word
        /// <summary>
        /// Lists information about a custom word from a custom language model. Only the owner of a custom model can use this method to query a word from the model.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The customization ID with the corpus to be deleted.</param>
        /// <param name="word">The word to get details of.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomWord(SuccessCallback<WordData> successCallback, FailCallback failCallback, string customizationID, string word, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID is required to GetCustomWord");
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("A word is required to GetCustomWord");

            GetCustomWordReq req = new GetCustomWordReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.OnResponse = OnGetCustomWordResp;

            string service = "/v1/customizations/{0}/words/{1}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID, word));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomWordReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<WordData> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetCustomWordResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WordData result = new WordData();
            fsData data = null;
            Dictionary<string, object> customData = ((GetCustomWordReq)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = result;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("SpeechToText.OnGetCustomWordResp()", "OnGetCustomWordResp Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetCustomWordReq)req).SuccessCallback != null)
                    ((GetCustomWordReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetCustomWordReq)req).FailCallback != null)
                    ((GetCustomWordReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Custom Acoustic Models
        /// <summary>
        /// Lists information about all custom acoustic models.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name = "language" >The identifier of the language for which custom acoustic models are to be returned (for example, `en-US`). Omit the parameter to see all custom acoustic models owned by the requesting service credentials.</param>
        /// <param name = "customData" >Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomAcousticModels(SuccessCallback<AcousticCustomizations> successCallback, FailCallback failCallback, string language = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetCustomAcousticModelsReq req = new GetCustomAcousticModelsReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            if (!string.IsNullOrEmpty(language))
                req.Parameters["language"] = language;
            req.OnResponse = OnGetCustomAcousticModelsResp;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/acoustic_customizations/"));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomAcousticModelsReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<AcousticCustomizations> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetCustomAcousticModelsResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            AcousticCustomizations result = new AcousticCustomizations();
            fsData data = null;
            Dictionary<string, object> customData = ((GetCustomAcousticModelsReq)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = result;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("SpeechToText.OnGetCustomAcousticModelsResp()", "OnGetCustomAcousticModelsResp Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetCustomAcousticModelsReq)req).SuccessCallback != null)
                    ((GetCustomAcousticModelsReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetCustomAcousticModelsReq)req).FailCallback != null)
                    ((GetCustomAcousticModelsReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Create Custom Acoustic Model
        /// <summary>
        /// Creates a custom acoustic model.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="name">The custom model name.</param>
        /// <param name="base_model_name">The base model name - only en-US_BroadbandModel is currently supported.</param>
        /// <param name="description">Descripotion of the custom model.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool CreateAcousticCustomization(SuccessCallback<CustomizationID> successCallback, FailCallback failCallback, string name, string base_model_name = "en-US_BroadbandModel", string description = default(string), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("A name is required to create a custom language model.");

            CustomLanguage customLanguage = new CustomLanguage();
            customLanguage.name = name;
            customLanguage.base_model_name = base_model_name;
            customLanguage.description = string.IsNullOrEmpty(description) ? name : description;

            fsData data;
            _serializer.TrySerialize(customLanguage.GetType(), customLanguage, out data).AssertSuccessWithoutWarnings();
            string customizationJson = fsJsonPrinter.CompressedJson(data);

            CreateAcousticCustomizationRequest req = new CreateAcousticCustomizationRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Send = Encoding.UTF8.GetBytes(customizationJson);
            req.OnResponse = OnCreateAcousticCustomizationResp;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/acoustic_customizations");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class CreateAcousticCustomizationRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<CustomizationID> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnCreateAcousticCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            CustomizationID result = new CustomizationID();
            fsData data = null;
            Dictionary<string, object> customData = ((CreateAcousticCustomizationRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = result;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("SpeechToText.OnCreateAcousticCustomizationResp()", "OnCreateAcousticCustomizationResp Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((CreateAcousticCustomizationRequest)req).SuccessCallback != null)
                    ((CreateAcousticCustomizationRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((CreateAcousticCustomizationRequest)req).FailCallback != null)
                    ((CreateAcousticCustomizationRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Delete Custom Acoustic Model
        /// <summary>
        /// Deletes a custom acoustic model.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The acoustic customization ID to be deleted.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool DeleteAcousticCustomization(SuccessCallback<bool> successCallback, FailCallback failCallback, string customizationID, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to delete is required for DeleteAcousticCustomization");

            DeleteAcousticCustomizationRequest req = new DeleteAcousticCustomizationRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Delete = true;
            req.OnResponse = OnDeleteAcousticCustomizationResp;

            string service = "/v1/acoustic_customizations/{0}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteAcousticCustomizationRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<bool> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnDeleteAcousticCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((DeleteAcousticCustomizationRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((DeleteAcousticCustomizationRequest)req).SuccessCallback != null)
                    ((DeleteAcousticCustomizationRequest)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((DeleteAcousticCustomizationRequest)req).FailCallback != null)
                    ((DeleteAcousticCustomizationRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Custom Acoustic Model
        /// <summary>
        /// Lists information about a custom acoustic model.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name = "customizationId" >The GUID of the custom acoustic model for which information is to be returned. You must make the request with service credentials created for the instance of the service that owns the custom model.</param>
        /// <param name = "customData" >Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomAcousticModel(SuccessCallback<AcousticCustomization> successCallback, FailCallback failCallback, string customizationId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("customizationId");

            GetCustomAcousticModelReq req = new GetCustomAcousticModelReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.OnResponse = OnGetCustomAcousticModelResp;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/acoustic_customizations/{0}", customizationId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomAcousticModelReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<AcousticCustomization> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetCustomAcousticModelResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            AcousticCustomization result = new AcousticCustomization();
            fsData data = null;
            Dictionary<string, object> customData = ((GetCustomAcousticModelReq)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = result;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("SpeechToText.OnGetCustomAcousticModelResp()", "OnGetCustomAcousticModelResp Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetCustomAcousticModelReq)req).SuccessCallback != null)
                    ((GetCustomAcousticModelReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetCustomAcousticModelReq)req).FailCallback != null)
                    ((GetCustomAcousticModelReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Train Custom Acoustic Model
        /// <summary>
        /// Trains an acoustic model
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The identifier of the customization to train.</param>
        /// <param name="customLanguageModelId">The identifierof the custom language model to train.</param>
        /// <param name="force">Force training on a training set less than 10 minutes.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool TrainAcousticCustomization(SuccessCallback<bool> successCallback, FailCallback failCallback, string customizationID, string customLanguageModelId = null, bool force = false, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to train a custom acoustic language model.");

            TrainAcousticCustomizationRequest req = new TrainAcousticCustomizationRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            if (!string.IsNullOrEmpty(customLanguageModelId))
                req.Parameters["custom_language_model_id"] = customLanguageModelId;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (force != false)
                req.Parameters["force"] = "true";
            req.Send = Encoding.UTF8.GetBytes("{}");
            req.OnResponse = OnTrainAcousticCustomizationResp;

            string service = "/v1/acoustic_customizations/{0}/train";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class TrainAcousticCustomizationRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<bool> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnTrainAcousticCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((TrainAcousticCustomizationRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((TrainAcousticCustomizationRequest)req).SuccessCallback != null)
                    ((TrainAcousticCustomizationRequest)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((TrainAcousticCustomizationRequest)req).FailCallback != null)
                    ((TrainAcousticCustomizationRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Reset Custom Acoustic Model
        /// <summary>
        /// Resets a custom acoustic model.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The identifier of the customization to reset.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool ResetAcousticCustomization(SuccessCallback<bool> successCallback, FailCallback failCallback, string customizationID, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to reset a custom acoustic language model.");

            ResetAcousticCustomizationRequest req = new ResetAcousticCustomizationRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Send = Encoding.UTF8.GetBytes("{}");
            req.OnResponse = OnResetAcousticCustomizationResp;

            string service = "/v1/acoustic_customizations/{0}/reset";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ResetAcousticCustomizationRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<bool> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnResetAcousticCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((ResetAcousticCustomizationRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((ResetAcousticCustomizationRequest)req).SuccessCallback != null)
                    ((ResetAcousticCustomizationRequest)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((ResetAcousticCustomizationRequest)req).FailCallback != null)
                    ((ResetAcousticCustomizationRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Custom Acoustic Resource
        /// <summary>
        /// Lists information about all audio resources for a custom acoustic model.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name = "customizationId" >The GUID of the custom acoustic model for which audio resources are to be listed. You must make the request with service credentials created for the instance of the service that.</param>
        /// <param name = "customData" >Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomAcousticResources(SuccessCallback<AudioResources> successCallback, FailCallback failCallback, string customizationId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("customizationId");

            GetCustomAcousticResourcesReq req = new GetCustomAcousticResourcesReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.OnResponse = OnGetCustomAcousticResourcesResp;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/acoustic_customizations/{0}/audio", customizationId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomAcousticResourcesReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<AudioResources> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetCustomAcousticResourcesResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            AudioResources result = new AudioResources();
            fsData data = null;
            Dictionary<string, object> customData = ((GetCustomAcousticResourcesReq)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = result;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("SpeechToText.OnGetCustomAcousticResourcesResp()", "OnGetCustomAcousticResourcesResp Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetCustomAcousticResourcesReq)req).SuccessCallback != null)
                    ((GetCustomAcousticResourcesReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetCustomAcousticResourcesReq)req).FailCallback != null)
                    ((GetCustomAcousticResourcesReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Delete Audio Resource
        /// <summary>
        /// Deletes an audio resource from a custom acoustic model.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The GUID of the custom acoustic model from which an audio resource is to be deleted. You must make the request with service credentials created for the instance of the service that owns the custom model.</param>
        /// <param name="audioName">The name of the audio resource that is to be deleted from the custom acoustic model.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool DeleteAcousticResource(SuccessCallback<bool> successCallback, FailCallback failCallback, string customizationID, string audioName, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to delete is required for DeleteAcousticResource");
            if (string.IsNullOrEmpty(audioName))
                throw new ArgumentNullException("An audioName to delete is required for DeleteAcousticResource");

            DeleteAcousticResourceRequest req = new DeleteAcousticResourceRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Delete = true;
            req.Timeout = 10f;
            req.OnResponse = OnDeleteAcousticResourceResp;

            string service = "/v1/acoustic_customizations/{0}/audio/{1}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID, WWW.EscapeURL(audioName)));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteAcousticResourceRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<bool> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnDeleteAcousticResourceResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((DeleteAcousticResourceRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((DeleteAcousticResourceRequest)req).SuccessCallback != null)
                    ((DeleteAcousticResourceRequest)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((DeleteAcousticResourceRequest)req).FailCallback != null)
                    ((DeleteAcousticResourceRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Custom Acoustic Resource
        /// <summary>
        /// Lists information about an audio resource for a custom acoustic model.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name = "customizationId" >The GUID of the custom acoustic model for which an audio resource is to be listed. You must make the request with service credentials created for the instance of the service that owns the custom model.</param>
        /// <param name = "audioName" >The name of the audio resource about which information is to be listed.</param>
        /// <param name = "customData" >Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomAcousticResource(SuccessCallback<AudioListing> successCallback, FailCallback failCallback, string customizationId, string audioName, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("customizationId");
            if (string.IsNullOrEmpty(audioName))
                throw new ArgumentNullException("audioName");

            GetCustomAcousticResourceReq req = new GetCustomAcousticResourceReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.OnResponse = OnGetCustomAcousticResourceResp;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/acoustic_customizations/{0}/audio/{1}", customizationId, audioName));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomAcousticResourceReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<AudioListing> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetCustomAcousticResourceResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            AudioListing result = new AudioListing();
            fsData data = null;
            Dictionary<string, object> customData = ((GetCustomAcousticResourceReq)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = result;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("SpeechToText.OnGetCustomAcousticResourceResp()", "OnGetCustomAcousticResourceResp Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetCustomAcousticResourceReq)req).SuccessCallback != null)
                    ((GetCustomAcousticResourceReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetCustomAcousticResourceReq)req).FailCallback != null)
                    ((GetCustomAcousticResourceReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Add Custom Acoustic Resource
        /// <summary>
        /// Adds an audio resource to a custom acoustic model.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationId">The cutomization identifier to add the acoustic resource to.</param>
        /// <param name="audioName">The nameof the audio resrouce.</param>
        /// <param name="contentType">The content type of the audio resource upload.</param>
        /// <param name="containedContentType">The content type of the enclosed audio resource.</param>
        /// <param name="allowOverwrite">Can this resource be overwritten?</param>
        /// <param name="audioResource">The byte[] data of the audio resource.</param>
        /// <param name="customData">optional custom data.</param>
        /// <returns>True if the acoustic resource was sent to the service.</returns>
        public bool AddAcousticResource(SuccessCallback<bool> successCallback, FailCallback failCallback, string customizationId, string audioName, string contentType, string containedContentType, bool allowOverwrite, byte[] audioResource, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("customizationId");
            if (string.IsNullOrEmpty(audioName))
                throw new ArgumentNullException("audioName");
            if (string.IsNullOrEmpty(contentType))
                throw new ArgumentNullException("contentType");
            if (string.IsNullOrEmpty(containedContentType))
                throw new ArgumentNullException("containedContentType");
            if (audioResource == null)
                throw new ArgumentNullException("audioResource");

            AddAcousticResourceRequest req = new AddAcousticResourceRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Headers["Content-Type"] = contentType;
            req.Headers["Accept"] = "application/json";
            req.Headers["Contained-Content-Type"] = containedContentType;
            req.Parameters["allow_overwrite"] = allowOverwrite ? "true" : "false";
            req.Send = audioResource;
            req.OnResponse = OnAddAcousticResourceResp;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/acoustic_customizations/{0}/audio/{1}", customizationId, audioName));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class AddAcousticResourceRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<bool> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnAddAcousticResourceResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((AddAcousticResourceRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((AddAcousticResourceRequest)req).SuccessCallback != null)
                    ((AddAcousticResourceRequest)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((AddAcousticResourceRequest)req).FailCallback != null)
                    ((AddAcousticResourceRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Delete User Data
        /// <summary>
        /// Deletes all data associated with a specified customer ID. The method has no effect if no data is associated with the customer ID. 
        /// You associate a customer ID with data by passing the X-Watson-Metadata header with a request that passes data. 
        /// For more information about personal data and customer IDs, see [**Information security**](https://console.bluemix.net/docs/services/discovery/information-security.html).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="customerId">The customer ID for which all data is to be deleted.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool DeleteUserData(SuccessCallback<object> successCallback, FailCallback failCallback, string customerId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException("customerId");

            DeleteUserDataRequestObj req = new DeleteUserDataRequestObj();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Parameters["customer_id"] = customerId;
            req.Delete = true;

            req.OnResponse = OnDeleteUserDataResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/user_data");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteUserDataRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<object> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnDeleteUserDataResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            object result = new object();
            fsData data = null;
            Dictionary<string, object> customData = ((DeleteUserDataRequestObj)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = result;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("SpeechToText.OnDeleteUserDataResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((DeleteUserDataRequestObj)req).SuccessCallback != null)
                    ((DeleteUserDataRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteUserDataRequestObj)req).FailCallback != null)
                    ((DeleteUserDataRequestObj)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region IWatsonService interface
        /// <exclude />
        public string GetServiceID()
        {
            return ServiceId;
        }
        #endregion
    }
}
