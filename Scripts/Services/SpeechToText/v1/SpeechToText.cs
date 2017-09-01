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
        private const int MaxQueuedRecordings = 30;
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
        private string[] _keywords = { "" };
        private float _keywordsThreshold = 0.5f;
        private float _wordAlternativesThreshold = 0.5f;
        private bool _profanityFilter = true;
        private bool _smallFormatting = false;
        private bool _speakerLabels = false;
        private bool _timestamps = false;
        private bool _wordConfidence = false;
        private bool _detectSilence = true;            // If true, then we will try not to record silence.
        private float _silenceThreshold = 0.03f;         // If the audio level is below this value, then it's considered silent.
        private int _recordingHZ = -1;
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
        /// If true, then we will try to continuously recognize speech.
        /// </summary>
        public bool EnableContinousRecognition { get; set; }
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
        /// A value from 1.0 to 0.0 that determines what is considered silence. If the audio level is below this value
        /// then we consider it silence.
        /// </summary>
        public float SilenceThreshold { get { return _silenceThreshold; } set { _silenceThreshold = value; } }
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
        public float KeywordsThreshold { get { return _keywordsThreshold; } set { _keywordsThreshold = value; } }
        /// <summary>
        /// NON-MULTIPART ONLY: Confidence value that is the lower bound for identifying a hypothesis as a possible word alternative (also known as "Confusion Networks"). An alternative word is considered if its confidence is greater than or equal to the threshold. Specify a probability between 0 and 1 inclusive. No alternative words are computed if you omit the parameter.
        /// </summary>
        public float WordAlternativesThreshold { get { return _wordAlternativesThreshold; } set { _wordAlternativesThreshold = value; } }
        /// <summary>
        /// NON-MULTIPART ONLY: If true (the default), filters profanity from all output except for keyword results by replacing inappropriate words with a series of asterisks. Set the parameter to false to return results with no censoring. Applies to US English transcription only.
        /// </summary>
        public bool ProfanityFilter { get { return _profanityFilter; } set { _profanityFilter = value; } }
        /// <summary>
        /// NON-MULTIPART ONLY: If true, converts dates, times, series of digits and numbers, phone numbers, currency values, and Internet addresses into more readable, conventional representations in the final transcript of a recognition request. If false (the default), no formatting is performed. Applies to US English transcription only.
        /// </summary>
        public bool SmallFormatting { get { return _smallFormatting; } set { _smallFormatting = value; } }
        /// <summary>
        /// NON-MULTIPART ONLY: Indicates whether labels that identify which words were spoken by which participants in a multi-person exchange are to be included in the response. If true, speaker labels are returned; if false (the default), they are not. Speaker labels can be returned only for the following language models: en-US_NarrowbandModel, en-US_BroadbandModel, es-ES_NarrowbandModel, es-ES_BroadbandModel, ja-JP_NarrowbandModel, and ja-JP_BroadbandModel. Setting speaker_labels to true forces the timestamps parameter to be true, regardless of whether you specify false for the parameter.
        /// </summary>
        public bool SpeakerLabels { get { return _speakerLabels; } set { _speakerLabels = value; } }
        #endregion

        #region Constructor
        public SpeechToText(Credentials credentials)
        {
            if (credentials.HasCredentials() || credentials.HasAuthorizationToken())
            {
                Credentials = credentials;
            }
            else
            {
                throw new WatsonException("Please provide a username and password or authorization token to use the Speech to Text service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }
        #endregion

        #region Get Models
        /// <summary>
        /// This callback object is used by the GetModels() method.
        /// </summary>
        /// <param name="models">Array of available models.</param>
        /// <param name="customData">Custom string data sent with the call.</param>
        public delegate void OnGetModels(ModelSet models, string customData);

        /// <summary>
        /// This function retrieves all the language models that the user may use by setting the RecognizeModel 
        /// public property.
        /// </summary>
        /// <param name="callback">This callback is invoked with an array of all available models. The callback will
        /// be invoked with null on error.</param>
        /// <param name="customData">Custom string data sent with the call.</param>
        /// <returns>Returns true if request has been made.</returns>
        public bool GetModels(OnGetModels callback, string customData = default(string))
        {
            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/models");
            if (connector == null)
                return false;

            GetModelsRequest req = new GetModelsRequest();
            req.Callback = callback;
            req.OnResponse = OnGetModelsResponse;
            req.Data = customData;
            return connector.Send(req);
        }

        private class GetModelsRequest : RESTConnector.Request
        {
            public OnGetModels Callback { get; set; }
            public string Data { get; set; }
        };

        private void OnGetModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ModelSet response = new ModelSet();
            fsData data = null;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = response;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("SpeechToText", "Caught exception {0} when parsing GetModel() response: {1}", e.ToString(), Encoding.UTF8.GetString(resp.Data));
                }

                if (resp == null)
                    Log.Error("SpeechToText", "Failed to parse GetModel response.");
            }

            string customData = ((GetModelsRequest)req).Data;
            if (((GetModelsRequest)req).Callback != null)
                ((GetModelsRequest)req).Callback(resp.Success ? response : null, !string.IsNullOrEmpty(customData) ? customData : data.ToString());
        }
        #endregion

        #region Get Model
        /// <summary>
        /// This callback object is used by the GetModel() method.
        /// </summary>
        /// <param name="model">The resultant model.</param>
        public delegate void OnGetModel(Model model, string customData);

        /// <summary>
        /// This function retrieves a specified languageModel.
        /// </summary>
        /// <param name="callback">This callback is invoked with the requested model. The callback will
        /// be invoked with null on error.</param>
        /// <param name="modelID">The name of the model to get.</param>
        /// <returns>Returns true if request has been made.</returns>
        public bool GetModel(OnGetModel callback, string modelID, string customData = default(string))
        {
            if (string.IsNullOrEmpty(modelID))
                throw new ArgumentNullException("modelID");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/models/" + modelID);
            if (connector == null)
                return false;

            GetModelRequest req = new GetModelRequest();
            req.Callback = callback;
            req.ModelID = modelID;
            req.OnResponse = OnGetModelResponse;
            req.Data = customData;

            return connector.Send(req);
        }

        private class GetModelRequest : RESTConnector.Request
        {
            public OnGetModel Callback { get; set; }
            public string ModelID { get; set; }
            public string Data { get; set; }
        };

        private void OnGetModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Model response = new Model();
            fsData data = null;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = response;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("SpeechToText", "Caught exception {0} when parsing GetModel() response: {1}", e.ToString(), Encoding.UTF8.GetString(resp.Data));
                }

                if (resp == null)
                    Log.Error("SpeechToText", "Failed to parse GetModel response.");
            }

            string customData = ((GetModelRequest)req).Data;
            if (((GetModelRequest)req).Callback != null)
                ((GetModelRequest)req).Callback(resp.Success ? response : null, !string.IsNullOrEmpty(customData) ? customData : data.ToString());
        }
        #endregion

        #region Sessionless - Streaming
        /// <summary>
        /// This callback object is used by the Recognize() and StartListening() methods.
        /// </summary>
        /// <param name="results">The ResultList object containing the results.</param>
        public delegate void OnRecognize(SpeechRecognitionEvent results);

        /// <summary>
        /// This starts the service listening and it will invoke the callback for any recognized speech.
        /// OnListen() must be called by the user to queue audio data to send to the service. 
        /// StopListening() should be called when you want to stop listening.
        /// </summary>
        /// <param name="callback">All recognize results are passed to this callback.</param>
        /// <returns>Returns true on success, false on failure.</returns>
        public bool StartListening(OnRecognize callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (_isListening)
                return false;
            if (!CreateListenConnector())
                return false;

            _isListening = true;
            _listenCallback = callback;
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
        public void OnListen(AudioData clip)
        {
            if (_isListening)
            {
                if (_recordingHZ < 0)
                {
                    _recordingHZ = clip.Clip.frequency;
                    SendStart();
                }

                if (!DetectSilence || clip.MaxLevel >= _silenceThreshold)
                {
                    if (_listenActive)
                    {
                        _listenSocket.Send(new WSConnector.BinaryMessage(AudioClipUtil.GetL16(clip.Clip)));
                        _audioSent = true;
                    }
                    else
                    {
                        // we have not received the "listening" state yet from the server, so just queue
                        // the audio clips until that happens.
                        _listenRecordings.Enqueue(clip);

                        // check the length of this queue and do something if it gets too full.
                        if (_listenRecordings.Count > MaxQueuedRecordings)
                        {
                            Log.Error("SpeechToText", "Recording queue is full.");

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
                    Log.Error("SpeechToText", "Failed to enter listening state.");

                    StopListening();
                    if (OnError != null)
                        OnError("Failed to enter listening state.");
                }
            }
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
                _listenSocket = WSConnector.CreateConnector(Credentials, Url, "/v1/recognize", "?model=" + WWW.EscapeURL(_recognizeModel));
                if (_listenSocket == null)
                    return false;

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
            start["continuous"] = EnableContinousRecognition;
            start["max_alternatives"] = _maxAlternatives;
            start["interim_results"] = EnableInterimResults;
            start["word_confidence"] = _wordConfidence;
            start["timestamps"] = _timestamps;
            start["speaker_labels"] = SpeakerLabels;
            start["small_formatting"] = SmallFormatting;
            start["profanity_filter"] = ProfanityFilter;
            start["word_alternatives_threshold"] = WordAlternativesThreshold;
            start["keywords_threshold"] = KeywordsThreshold;
            start["keywords"] = Keywords;

            _listenSocket.Send(new WSConnector.TextMessage(Json.Serialize(start)));
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
                    Dictionary<string, string> nop = new Dictionary<string, string>();
                    nop["action"] = "no-op";

#if ENABLE_DEBUGGING
                    Log.Debug("SpeechToText", "Sending keep alive.");
#endif
                    _listenSocket.Send(new WSConnector.TextMessage(Json.Serialize(nop)));
                    _lastKeepAlive = DateTime.Now;
                }
            }
            Log.Debug("SpeechToText", "KeepAlive exited.");
        }

        private void OnListenMessage(WSConnector.Message msg)
        {
            if (msg is WSConnector.TextMessage)
            {
                WSConnector.TextMessage tm = (WSConnector.TextMessage)msg;

                IDictionary json = Json.Deserialize(tm.Text) as IDictionary;
                if (json != null)
                {
                    if (json.Contains("results"))
                    {
                        SpeechRecognitionEvent results = ParseRecognizeResponse(json);
                        if (results != null)
                        {
                            // when we get results, start listening for the next block ..
                            // if continuous is true, then we don't need to do this..
                            if (!EnableContinousRecognition && results.HasFinalResult())
                                SendStart();

                            if (_listenCallback != null)
                                _listenCallback(results);
                            else
                                StopListening();            // automatically stop listening if our callback is destroyed.
                        }
                        else
                            Log.Error("SpeechToText", "Failed to parse results: {0}", tm.Text);
                    }
                    else if (json.Contains("state"))
                    {
                        string state = (string)json["state"];

#if ENABLE_DEBUGGING
                        Log.Debug("SpeechToText", "Server state is {0}", state);
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
                    else if (json.Contains("error"))
                    {
                        string error = (string)json["error"];
                        Log.Error("SpeechToText", "Error: {0}", error);

                        StopListening();
                        if (OnError != null)
                            OnError(error);
                    }
                    else
                    {
                        Log.Warning("SpeechToText", "Unknown message: {0}", tm.Text);
                    }
                }
                else
                {
                    Log.Error("SpeechToText", "Failed to parse JSON from server: {0}", tm.Text);
                }
            }
        }

        private void OnListenClosed(WSConnector connector)
        {
#if ENABLE_DEBUGGING
            Log.Debug("SpeechToText", "OnListenClosed(), State = {0}", connector.State.ToString());
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
        /// <param name="clip">The AudioClip object.</param>
        /// <param name="callback">A callback to invoke with the results.</param>
        /// <returns></returns>
        public bool Recognize(AudioClip clip, OnRecognize callback)
        {
            if (clip == null)
                throw new ArgumentNullException("clip");
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/recognize");
            if (connector == null)
                return false;

            RecognizeRequest req = new RecognizeRequest();
            req.Clip = clip;
            req.Callback = callback;

            req.Headers["Content-Type"] = "audio/wav";
            req.Send = WaveFile.CreateWAV(clip);
            if (req.Send.Length > MaxRecognizeClipSize)
            {
                Log.Error("SpeechToText", "AudioClip is too large for Recognize().");
                return false;
            }
            req.Parameters["model"] = _recognizeModel;
            req.Parameters["continuous"] = "false";
            req.Parameters["max_alternatives"] = _maxAlternatives.ToString();
            req.Parameters["timestamps"] = _timestamps ? "true" : "false";
            req.Parameters["word_confidence"] = _wordConfidence ? "true" : "false";
            req.Parameters["speaker_labels"] = SpeakerLabels;
            req.Parameters["small_formatting"] = SmallFormatting;
            req.Parameters["profanity_filter"] = ProfanityFilter;
            req.Parameters["word_alternatives_threshold"] = WordAlternativesThreshold;
            req.Parameters["keywords_threshold"] = KeywordsThreshold;
            req.Parameters["keywords"] = string.Join(",", Keywords);

            req.OnResponse = OnRecognizeResponse;

            return connector.Send(req);
        }

        private class RecognizeRequest : RESTConnector.Request
        {
            public AudioClip Clip { get; set; }
            public OnRecognize Callback { get; set; }
        };

        private void OnRecognizeResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            RecognizeRequest recognizeReq = req as RecognizeRequest;
            if (recognizeReq == null)
                throw new WatsonException("Unexpected request type.");

            SpeechRecognitionEvent result = null;
            if (resp.Success)
            {
                result = ParseRecognizeResponse(resp.Data);
                if (result == null)
                {
                    Log.Error("SpeechToText", "Failed to parse json response: {0}",
                        resp.Data != null ? Encoding.UTF8.GetString(resp.Data) : "");
                }
                else
                {
                    Log.Status("SpeechToText", "Received Recognize Response, Elapsed Time: {0}, Results: {1}",
                        resp.ElapsedTime, result.results.Length);
                }
            }
            else
            {
                Log.Error("SpeechToText", "Recognize Error: {0}", resp.Error);
            }

            if (recognizeReq.Callback != null)
                recognizeReq.Callback(result);
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

                    IList ialternatives = iresult["alternatives"] as IList;
                    if (ialternatives == null)
                        continue;

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

                    result.alternatives = alternatives.ToArray();
                    results.Add(result);
                }

                return new SpeechRecognitionEvent(results.ToArray());
            }
            catch (Exception e)
            {
                Log.Error("SpeechToText", "ParseJsonResponse exception: {0}", e.ToString());
                return null;
            }
        }
        #endregion

        #region Asynchronous
        #endregion

        #region Get Custom Models
        /// <summary>
        /// This callback is used by the GetCustomizations() function.
        /// </summary>
        /// <param name="customizations">The customizations</param>
        /// <param name="customData">Optional custom data.</param>
        public delegate void GetCustomizationsCallback(Customizations customizations, string customData);

        /// <summary>
        /// Lists information about all custom language models that are owned by the calling user. Use the language query parameter to see all custom models for the specified language; omit the parameter to see all custom models for all languages.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="language">The language for which custom models are to be returned. Currently, only en-US (the default) is supported.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomizations(GetCustomizationsCallback callback, string language = "en-US", string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            GetCustomizationsReq req = new GetCustomizationsReq();
            req.Callback = callback;
            req.Data = customData;
            req.Language = language;
            req.Parameters["language"] = language;
            req.OnResponse = OnGetCustomizationsResp;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/customizations");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomizationsReq : RESTConnector.Request
        {
            public GetCustomizationsCallback Callback { get; set; }
            public string Language { get; set; }
            public string Data { get; set; }
        }

        private void OnGetCustomizationsResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Customizations customizations = new Customizations();
            fsData data = null;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = customizations;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Speech To Text", "GetCustomizations Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            string customData = ((GetCustomizationsReq)req).Data;
            if (((GetCustomizationsReq)req).Callback != null)
                ((GetCustomizationsReq)req).Callback(resp.Success ? customizations : null, !string.IsNullOrEmpty(customData) ? customData : data.ToString());
        }
        #endregion

        #region Create Custom Model
        /// <summary>
        /// Thid callback is used by the CreateCustomization() function.
        /// </summary>
        /// <param name="customizationID">The customizationID.</param>
        /// <param name="customData">Optional custom data.</param>
        public delegate void CreateCustomizationCallback(CustomizationID customizationID, string customData);

        /// <summary>
        /// Creates a new custom language model for a specified base language model. The custom language model can be used only with the base language model for which it is created. The new model is owned by the individual whose service credentials are used to create it.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="name">The custom model name.</param>
        /// <param name="base_model_name">The base model name - only en-US_BroadbandModel is currently supported.</param>
        /// <param name="description">Descripotion of the custom model.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool CreateCustomization(CreateCustomizationCallback callback, string name, string base_model_name = "en-US_BroadbandModel", string description = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
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
            req.Callback = callback;
            req.CustomLanguage = customLanguage;
            req.Data = customData;
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
            public CreateCustomizationCallback Callback { get; set; }
            public CustomLanguage CustomLanguage { get; set; }
            public string Data { get; set; }
        }

        private void OnCreateCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            CustomizationID customizationID = new CustomizationID();
            fsData data = null;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = customizationID;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Speech To Text", "CreateCustomization Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            string customData = ((CreateCustomizationRequest)req).Data;
            if (((CreateCustomizationRequest)req).Callback != null)
                ((CreateCustomizationRequest)req).Callback(resp.Success ? customizationID : null, !string.IsNullOrEmpty(customData) ? customData : data.ToString());
        }
        #endregion

        #region Delete Custom Model
        /// <summary>
        /// This callback is used by the DeleteCustomization() function.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="customData"></param>
        public delegate void OnDeleteCustomizationCallback(bool success, string customData);
        /// <summary>
        /// Deletes an existing custom language model. Only the owner of a custom model can use this method to delete the model.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="customizationID">The customization ID to be deleted.</param>
        /// <param name="customData">Optional customization data.</param>
        /// <returns></returns>
        public bool DeleteCustomization(OnDeleteCustomizationCallback callback, string customizationID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to delete is required for DeleteCustomization");

            DeleteCustomizationRequest req = new DeleteCustomizationRequest();
            req.Callback = callback;
            req.CustomizationID = customizationID;
            req.Data = customData;
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
            public OnDeleteCustomizationCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public string Data { get; set; }
        }

        private void OnDeleteCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            if (((DeleteCustomizationRequest)req).Callback != null)
                ((DeleteCustomizationRequest)req).Callback(resp.Success, ((DeleteCustomizationRequest)req).Data);
        }
        #endregion

        #region Get Custom Model
        /// <summary>
        /// This callback is used by the GetCusomization() function.
        /// </summary>
        /// <param name="customization"></param>
        /// <param name="customData"></param>
        public delegate void GetCustomizationCallback(Customization customization, string customData);
        /// <summary>
        /// Lists information about a custom language model. Only the owner of a custom model can use this method to query information about the model.
        ///	Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="customizationID">The requested custom language model's identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomization(GetCustomizationCallback callback, string customizationID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to get a custom language model.");

            GetCustomizationRequest req = new GetCustomizationRequest();
            req.Callback = callback;
            req.CustomizationID = customizationID;
            req.Data = customData;
            req.OnResponse = OnGetCustomizationResp;

            string service = "/v1/customizations/{0}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomizationRequest : RESTConnector.Request
        {
            public GetCustomizationCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public string Data { get; set; }
        }

        private void OnGetCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Customization customization = new Customization();
            fsData data = null;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = customization;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Speech To Text", "GetCustomization Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            string customData = ((GetCustomizationRequest)req).Data;
            if (((GetCustomizationRequest)req).Callback != null)
                ((GetCustomizationRequest)req).Callback(resp.Success ? customization : null, !string.IsNullOrEmpty(customData) ? customData : data.ToString());
        }
        #endregion

        #region Train Custom Model
        /// <summary>
        /// This callback is used by the TrainCustomization() function.
        /// </summary>
        /// <param name="success">The success of the call.</param>
        /// <param name="customData">Optional custom data.</param>
        public delegate void TrainCustomizationCallback(bool success, string customData);
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
        /// <param name="callback">The callback.</param>
        /// <param name="customizationID">The requested custom language model's identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool TrainCustomization(TrainCustomizationCallback callback, string customizationID, string wordTypeToAdd = WordTypeToAdd.All, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to train a custom language model.");

            TrainCustomizationRequest req = new TrainCustomizationRequest();
            req.Callback = callback;
            req.CustomizationID = customizationID;
            req.Data = customData;
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
            public TrainCustomizationCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public string Data { get; set; }
        }

        private void OnTrainCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            if (((TrainCustomizationRequest)req).Callback != null)
                ((TrainCustomizationRequest)req).Callback(resp.Success, ((TrainCustomizationRequest)req).Data);
        }
        #endregion

        #region Reset Custom Model
        /// <summary>
        /// This callback is used by the ResetCustomization() function.
        /// </summary>
        /// <param name="success">The success of the call.</param>
        /// <param name="customData">Optional custom data.</param>
        public delegate void ResetCustomizationCallback(bool success, string customData);
        /// <summary>
        /// Resets a custom language model by removing all corpora and words from the model.Resetting a custom model initializes the model to its state when it was first created. Metadata such as the name and language of the model are preserved.Only the owner of a custom model can use this method to reset the model.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="customizationID">The requested custom language model's identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool ResetCustomization(ResetCustomizationCallback callback, string customizationID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to train a reset language model.");

            ResetCustomizationRequest req = new ResetCustomizationRequest();
            req.Callback = callback;
            req.CustomizationID = customizationID;
            req.Data = customData;
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
            public ResetCustomizationCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public string Data { get; set; }
        }

        private void OnResetCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            if (((ResetCustomizationRequest)req).Callback != null)
                ((ResetCustomizationRequest)req).Callback(resp.Success, ((ResetCustomizationRequest)req).Data);
        }
        #endregion

        #region Upgrade Custom Model
        /// <summary>
        /// This callback is used by the UpgradeCustomization() function.
        /// </summary>
        /// <param name="success">The success of the call.</param>
        /// <param name="customData">Optional custom data.</param>
        public delegate void UpgradeCustomizationCallback(bool success, string customData);
        /// <summary>
        /// Upgrades a custom language model to the latest release level of the Speech to Text service. The method bases the upgrade on the latest trained data stored for the custom model. If the corpora or words for the model have changed since the model was last trained, you must use the POST /v1/customizations/{customization_id}/train method to train the model on the new data. Only the owner of a custom model can use this method to upgrade the model.
        /// Note: This method is not currently implemented.It will be added for a future release of the API.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="customizationID">The requested custom language model's identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool UpgradeCustomization(UpgradeCustomizationCallback callback, string customizationID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to upgrade a custom language model.");

            UpgradeCustomizationRequest req = new UpgradeCustomizationRequest();
            req.Callback = callback;
            req.CustomizationID = customizationID;
            req.Data = customData;
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
            public UpgradeCustomizationCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public string Data { get; set; }
        }

        private void OnUpgradeCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            if (((UpgradeCustomizationRequest)req).Callback != null)
                ((UpgradeCustomizationRequest)req).Callback(resp.Success, ((UpgradeCustomizationRequest)req).Data);
        }
        #endregion

        #region Get Custom Corpora
        /// <summary>
        /// This callback is used by the GetCustomCorpora() function.
        /// </summary>
        /// <param name="corpora">The corpora</param>
        /// <param name="customData">Optional custom data.</param>
        public delegate void GetCustomCorporaCallback(Corpora corpora, string customData);

        /// <summary>
        /// Lists information about all corpora that have been added to the specified custom language model. The information includes the total number of words and out-of-vocabulary (OOV) words, name, and status of each corpus. Only the owner of a custom model can use this method to list the model's corpora.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="language">The language for which custom models are to be returned. Currently, only en-US (the default) is supported.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomCorpora(GetCustomCorporaCallback callback, string customizationID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID is required to GetCustomCorpora");

            GetCustomCorporaReq req = new GetCustomCorporaReq();
            req.Callback = callback;
            req.Data = customData;
            req.CustomizationID = customizationID;
            req.OnResponse = OnGetCustomCorporaResp;

            string service = "/v1/customizations/{0}/corpora";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomCorporaReq : RESTConnector.Request
        {
            public GetCustomCorporaCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public string Data { get; set; }
        }

        private void OnGetCustomCorporaResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Corpora corpora = new Corpora();
            fsData data = null;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = corpora;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Speech To Text", "OnGetCustomCorporaResp Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            string customData = ((GetCustomCorporaReq)req).Data;
            if (((GetCustomCorporaReq)req).Callback != null)
                ((GetCustomCorporaReq)req).Callback(resp.Success ? corpora : null, !string.IsNullOrEmpty(customData) ? customData : data.ToString());
        }
        #endregion

        #region Get Custom Corpus
        /// <summary>
        /// This callback is used by the GetCustomCorpus() function.
        /// </summary>
        /// <param name="corpus">The corpus</param>
        /// <param name="customData">Optional custom data.</param>
        public delegate void GetCustomCorpusCallback(Corpus corpus, string customData);

        /// <summary>
        /// Lists information about all corpora that have been added to the specified custom language model. The information includes the total number of words and out-of-vocabulary (OOV) words, name, and status of each corpus. Only the owner of a custom model can use this method to list the model's corpora.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="language">The language for which custom models are to be returned. Currently, only en-US (the default) is supported.</param>
        /// <param name="corpusName">The name of the custom corpus to be returned.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomCorpus(GetCustomCorpusCallback callback, string customizationID, string corpusName, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID is required to GetCustomCorpora");
            if (string.IsNullOrEmpty(corpusName))
                throw new ArgumentNullException("A corpusName is required to GetCustomCorpora");

            GetCustomCorpusReq req = new GetCustomCorpusReq();
            req.Callback = callback;
            req.Data = customData;
            req.CustomizationID = customizationID;
            req.CustomCorpusName = corpusName;
            req.OnResponse = OnGetCustomCorpusResp;

            string service = "/v1/customizations/{0}/corpora/{1}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID, corpusName));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomCorpusReq : RESTConnector.Request
        {
            public GetCustomCorpusCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public string CustomCorpusName { get; set; }
            public string Data { get; set; }
        }

        private void OnGetCustomCorpusResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Corpus corpus = new Corpus();
            fsData data = null;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = corpus;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Speech To Text", "OnGetCustomCorpusResp Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            string customData = ((GetCustomCorpusReq)req).Data;
            if (((GetCustomCorpusReq)req).Callback != null)
                ((GetCustomCorpusReq)req).Callback(resp.Success ? corpus : null, !string.IsNullOrEmpty(customData) ? customData : data.ToString());
        }
        #endregion

        #region Delete Custom Corpus
        /// <summary>
        /// This callback is used by the DeleteCustomCorpus() function.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="customData"></param>
        public delegate void OnDeleteCustomCorpusCallback(bool success, string customData);
        /// <summary>
        /// Deletes an existing corpus from a custom language model. The service removes any out-of-vocabulary (OOV) words associated with the corpus from the custom model's words resource unless they were also added by another corpus or they have been modified in some way with the POST /v1/customizations/{customization_id}/words or PUT /v1/customizations/{customization_id}/words/{word_name} method. Removing a corpus does not affect the custom model until you train the model with the POST /v1/customizations/{customization_id}/train method. Only the owner of a custom model can use this method to delete a corpus from the model.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="customizationID">The customization ID with the corpus to be deleted.</param>
        /// <param name="corpusName">The corpus name to be deleted.</param>
        /// <param name="customData">Optional customization data.</param>
        /// <returns></returns>
        public bool DeleteCustomCorpus(OnDeleteCustomCorpusCallback callback, string customizationID, string corpusName, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID is required for DeleteCustomCorpora.");
            if (string.IsNullOrEmpty(corpusName))
                throw new ArgumentNullException("A corpusName to delete is required to DeleteCustomCorpora.");

            DeleteCustomCorpusRequest req = new DeleteCustomCorpusRequest();
            req.Callback = callback;
            req.CustomizationID = customizationID;
            req.CorpusName = corpusName;
            req.Data = customData;
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
            public OnDeleteCustomCorpusCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public string CorpusName { get; set; }
            public string Data { get; set; }
        }

        private void OnDeleteCustomCorpusResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            if (((DeleteCustomCorpusRequest)req).Callback != null)
                ((DeleteCustomCorpusRequest)req).Callback(resp.Success, ((DeleteCustomCorpusRequest)req).Data);
        }
        #endregion

        #region Add Custom Coprpus
        /// <summary>
        /// This callback is used by the AddCustomCorpus() function.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="customData"></param>
        public delegate void OnAddCustomCorpusCallback(bool success, string customData);
        /// <summary>
        /// Adds a single corpus text file of new training data to the custom language model. Use multiple requests to submit multiple corpus text files. Only the owner of a custom model can use this method to add a corpus to the model.
        /// Submit a plain text file that contains sample sentences from the domain of interest to enable the service to extract words in context.The more sentences you add that represent the context in which speakers use words from the domain, the better the service's recognition accuracy. Adding a corpus does not affect the custom model until you train the model for the new data by using the POST /v1/customizations/{customization_id}/train method.
        /// Use the following guidelines to prepare a corpus text file:
        /// - Provide a plain text file that is encoded in UTF-8 if it contains non-ASCII characters.The service assumes UTF-8 encoding if it encounters such characters.
        /// - Include each sentence of the corpus on its own line, terminating each line with a carriage return. Including multiple sentences on the same line can degrade accuracy.
        /// - Use consistent capitalization for words in the corpus. The words resource is case-sensitive; mix upper- and lowercase letters and use capitalization only when intended. 
        /// - Beware of typographical errors.The service assumes that typos are new words; unless you correct them before training the model, the service adds them to the model's vocabulary.
        /// The service automatically does the following:
        /// - Converts numbers to their equivalent words.For example:
        ///		500 becomes five hundred
        ///		and
        ///		0.15 becomes zero point fifteen
        ///	- Removes the following punctuation and special characters:
        ///		! @ # $ % ^ & * - + = ~ _ . , ; : ( ) < > [ ] { }
        ///	- Ignores phrases enclosed in ( ) (parentheses), < > (angle brackets), [] (square brackets), and { } (curly braces).
        ///	- Converts tokens that include certain symbols to meaningful strings.For example, the service converts a $ (dollar sign) followed by a number to its string representation:
        ///		$100 becomes one hundred dollars
        ///		and it converts a % (percent sign) preceded by a number to its string representation:
        ///		100% becomes one hundred percent
        ///		This list is not exhaustive; the service makes similar adjustments for other characters as needed.
        ///	
        /// The call returns an HTTP 201 response code if the corpus is valid.It then asynchronously pre-processes the contents of the corpus and automatically extracts new words that it finds.This can take on the order of a minute or two to complete depending on the total number of words and the number of new words in the corpus, as well as the current load on the service.You cannot submit requests to add additional corpora or words to the custom model, or to train the model, until the service's analysis of the corpus for the current request completes. Use the GET /v1/customizations/{customization_id}/corpora method to check the status of the analysis.
        /// 
        /// The service auto-populates the model's words resource with any word that is not found in its base vocabulary; these are referred to as out-of-vocabulary (OOV) words. You can use the GET /v1/customizations/{customization_id}/words method to examine the words resource, using other words method to eliminate typos and modify how words are pronounced as needed.
        /// 
        /// To add a corpus file that has the same name as an existing corpus, set the allow_overwrite query parameter to true; otherwise, the request fails.Overwriting an existing corpus causes the service to process the corpus text file and extract OOV words anew.Before doing so, it removes any OOV words associated with the existing corpus from the model's words resource unless they were also added by another corpus or they have been modified in some way with the POST /v1/customizations/{customization_id}/words or PUT /v1/customizations/{customization_id}/words/{word_name} method.
        /// 
        /// The service limits the overall amount of data that you can add to a custom model to a maximum of 10 million total words from all corpora combined.Also, you can add no more than 30 thousand new words to a model; this includes words that the service extracts from corpora and words that you add directly.
        /// Note: This method is currently a beta release that is available for US English only
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="customizationID">The customization ID with the corpus to be deleted.</param>
        /// <param name="corpusName">The corpus name to be deleted.</param>
        /// <param name="allowOverwrite">Allow overwriting of corpus data.</param>
        /// <param name="trainingData">A file path to plain text training data.</param>
        /// <param name="customData">Optional customization data.</param>
        /// <returns></returns>
        public bool AddCustomCorpus(OnAddCustomCorpusCallback callback, string customizationID, string corpusName, bool allowOverwrite, string trainingPath, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID is required for AddCustomCorpus.");
            if (string.IsNullOrEmpty(corpusName))
                throw new ArgumentNullException("A corpusName is required to AddCustomCorpus.");
            if (string.IsNullOrEmpty(trainingPath))
                throw new ArgumentNullException("A path to training data is required to AddCustomCorpus");

            byte[] trainingDataBytes = null;

            if (!string.IsNullOrEmpty(trainingPath))
            {
                if (LoadFile != null)
                {
                    trainingDataBytes = LoadFile(trainingPath);
                }
                else
                {
#if !UNITY_WEBPLAYER
                    trainingDataBytes = File.ReadAllBytes(trainingPath);
#endif
                }

                if (trainingDataBytes == null)
                    Log.Error("SpeechToText", "Failed to upload {0}!", trainingPath);
            }

            return AddCustomCorpus(callback, customizationID, corpusName, allowOverwrite, trainingDataBytes);
        }

        /// <summary>
        /// Overload method for AddCustomCorpus that takes byteArray training data.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="customizationID">The customization ID with the corpus to be deleted.</param>
        /// <param name="corpusName">The corpus name to be deleted.</param>
        /// <param name="allowOverwrite">Allow overwriting of corpus data.</param>
        /// <param name="trainingData">ByteArray data for training data.</param>
        /// <param name="customData">Optional customization data.</param>
        public bool AddCustomCorpus(OnAddCustomCorpusCallback callback, string customizationID, string corpusName, bool allowOverwrite, byte[] trainingData, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID is required for AddCustomCorpus.");
            if (string.IsNullOrEmpty(corpusName))
                throw new ArgumentNullException("A corpusName is requried for AddCustomCorpus.");
            if (trainingData == default(byte[]))
                throw new ArgumentNullException("Training data is required for AddCustomCorpus.");

            AddCustomCorpusRequest req = new AddCustomCorpusRequest();
            req.Callback = callback;
            req.CustomizationID = customizationID;
            req.CorpusName = corpusName;
            req.Data = customData;
            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Headers["Accept"] = "application/json";
            req.Parameters["allow_overwrite"] = allowOverwrite.ToString();
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["body"] = new RESTConnector.Form(trainingData, "trainingData.txt", "text/plain");
            req.OnResponse = OnAddCustomCorpusResp;

            string service = "/v1/customizations/{0}/corpora/{1}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID, corpusName));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class AddCustomCorpusRequest : RESTConnector.Request
        {
            public OnAddCustomCorpusCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public string CorpusName { get; set; }
            public bool AllowOverwrite { get; set; }
            public byte[] TrainingData { get; set; }
            public string Data { get; set; }
        }

        private void OnAddCustomCorpusResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            if (((AddCustomCorpusRequest)req).Callback != null)
                ((AddCustomCorpusRequest)req).Callback(resp.Success, ((AddCustomCorpusRequest)req).Data);
        }
        #endregion

        #region Get Custom Words
        /// <summary>
        /// This callback is used by the GetCustomWords() function.
        /// </summary>
        /// <param name="wordList">The custom words</param>
        /// <param name="customData">Optional custom data.</param>
        public delegate void GetCustomWordsCallback(WordsList wordList, string customData);

        /// <summary>
        /// Lists information about all custom words from a custom language model. You can list all words from the custom model's words resource, only custom words that were added or modified by the user, or only OOV words that were extracted from corpora. Only the owner of a custom model can use this method to query the words from the model.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="language">The language for which custom models are to be returned. Currently, only en-US (the default) is supported.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomWords(GetCustomWordsCallback callback, string customizationID, string wordType = WordType.All, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("customizationID");

            GetCustomWordsReq req = new GetCustomWordsReq();
            req.Callback = callback;
            req.CustomizationID = customizationID;
            req.WordType = wordType;
            req.Data = customData;
            req.Parameters["word_type"] = wordType.ToString();
            req.OnResponse = OnGetCustomWordsResp;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/words", customizationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomWordsReq : RESTConnector.Request
        {
            public GetCustomWordsCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public string WordType { get; set; }
            public string Data { get; set; }
        }

        private void OnGetCustomWordsResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WordsList wordsList = new WordsList();
            fsData data = null;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = wordsList;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Speech To Text", "OnGetCustomWordsResp Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            string customData = ((GetCustomWordsReq)req).Data;
            if (((GetCustomWordsReq)req).Callback != null)
                ((GetCustomWordsReq)req).Callback(resp.Success ? wordsList : null, !string.IsNullOrEmpty(customData) ? customData : data.ToString());
        }
        #endregion

        #region Add Custom Words
        /// <summary>
        /// This callback is used by the AddCustomWords() function.
        /// </summary>
        /// <param name="success">The success of the call.</param>
        /// <param name="customData">Optional custom data.</param>
        public delegate void AddCustomWordsCallback(bool success, string customData);
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
        /// <param name="callback">The callback.</param>
        /// <param name="customizationID">The requested custom language model's identifier.</param>
        /// <param name="useDataPath">A boolean used to differentiate overloads with identical input types..</param>
        /// <param name="wordsJsonPath">A path to a json file to train.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool AddCustomWords(AddCustomWordsCallback callback, string customizationID, bool useDataPath, string wordsJsonPath, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to add words to a custom language model.");
            if (string.IsNullOrEmpty(wordsJsonPath))
                throw new ArgumentNullException("A wordsJsonPath is required to add words to a custom language model.");

            string wordsJson = File.ReadAllText(wordsJsonPath);

            return AddCustomWords(callback, customizationID, wordsJson);
        }

        public bool AddCustomWords(AddCustomWordsCallback callback, string customizationID, Words words, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to add words to a custom language model.");
            if (words == null || words.words == null || words.words.Length == 0)
                throw new WatsonException("Custom words are required to add words to a custom language model.");

            fsData data;
            _serializer.TrySerialize(words.GetType(), words, out data).AssertSuccessWithoutWarnings();
            string wordsJson = fsJsonPrinter.CompressedJson(data);

            return AddCustomWords(callback, customizationID, wordsJson, customData);
        }

        public bool AddCustomWords(AddCustomWordsCallback callback, string customizationID, string wordsJson, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to add words to a custom language model.");
            if (string.IsNullOrEmpty(wordsJson))
                throw new ArgumentNullException("A wordsJsonPath is required to add words to a custom language model.");

            AddCustomWordsRequest req = new AddCustomWordsRequest();
            req.Callback = callback;
            req.CustomizationID = customizationID;
            req.WordsJson = wordsJson;
            req.Data = customData;
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
            public AddCustomWordsCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public string WordsJson { get; set; }
            public string Data { get; set; }
        }

        private void OnAddCustomWordsResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            if (((AddCustomWordsRequest)req).Callback != null)
                ((AddCustomWordsRequest)req).Callback(resp.Success, ((AddCustomWordsRequest)req).Data);
        }
        #endregion

        #region Delete Custom Word
        /// <summary>
        /// This callback is used by the DeleteCustomWord() function.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="customData"></param>
        public delegate void OnDeleteCustomWordCallback(bool success, string customData);
        /// <summary>
        /// Deletes a custom word from a custom language model. You can remove any word that you added to the custom model's words resource via any means. However, if the word also exists in the service's base vocabulary, the service removes only the custom pronunciation for the word; the word remains in the base vocabulary.
        /// Removing a custom word does not affect the custom model until you train the model with the POST /v1/customizations/{customization_id}/train method.Only the owner of a custom model can use this method to delete a word from the model.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="customizationID">The customization ID to be deleted.</param>
        /// <param name="customData">Optional customization data.</param>
        /// <returns></returns>
        public bool DeleteCustomWord(OnDeleteCustomWordCallback callback, string customizationID, string word, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID is required for DeleteCustomWord");
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("A word to delete is requried for DeleteCustomWord");

            DeleteCustomWordRequest req = new DeleteCustomWordRequest(); req.Callback = callback;
            req.CustomizationID = customizationID;
            req.Word = word;
            req.Data = customData;
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
            public OnDeleteCustomWordCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public string Word { get; set; }
            public string Data { get; set; }
        }

        private void OnDeleteCustomWordResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            if (((DeleteCustomWordRequest)req).Callback != null)
                ((DeleteCustomWordRequest)req).Callback(resp.Success, ((DeleteCustomWordRequest)req).Data);
        }
        #endregion

        #region Get Custom Word
        /// <summary>
        /// This callback is used by the GetCustomWord() function.
        /// </summary>
        /// <param name="word">The word</param>
        /// <param name="customData">Optional custom data.</param>
        public delegate void GetCustomWordCallback(WordData word, string customData);

        /// <summary>
        /// Lists information about a custom word from a custom language model. Only the owner of a custom model can use this method to query a word from the model.
        /// Note: This method is currently a beta release that is available for US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="language">The language for which custom models are to be returned. Currently, only en-US (the default) is supported.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomWord(GetCustomWordCallback callback, string customizationID, string word, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID is required to GetCustomWord");
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("A word is required to GetCustomWord");

            GetCustomWordReq req = new GetCustomWordReq();
            req.Callback = callback;
            req.CustomizationID = customizationID;
            req.Word = word;
            req.Data = customData;
            req.OnResponse = OnGetCustomWordResp;

            string service = "/v1/customizations/{0}/words/{1}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID, word));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomWordReq : RESTConnector.Request
        {
            public GetCustomWordCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public string Word { get; set; }
            public string Data { get; set; }
        }

        private void OnGetCustomWordResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WordData word = new WordData();
            fsData data = null;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = word;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Speech To Text", "OnGetCustomWordResp Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            string customData = ((GetCustomWordReq)req).Data;
            if (((GetCustomWordReq)req).Callback != null)
                ((GetCustomWordReq)req).Callback(resp.Success ? word : null, !string.IsNullOrEmpty(customData) ? customData : data.ToString());
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
