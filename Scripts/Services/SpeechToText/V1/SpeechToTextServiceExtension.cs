/**
* (C) Copyright IBM Corp. 2018, 2020.
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

using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Connection;
using IBM.Cloud.SDK.DataTypes;
using IBM.Cloud.SDK.Utilities;
using MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Utility = IBM.Cloud.SDK.Utilities.Utility;

namespace IBM.Watson.SpeechToText.V1
{
    public partial class SpeechToTextService : BaseService
    {
        #region Constants
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
        public bool? EnableInterimResults { get; set; }
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
        /// Specifies the Globally Unique Identifier (GUID) of a custom language model that is to be used for all requests sent over the connection. The base model of the custom language model must match the value of the model parameter. By default, no custom language model is used. For more information, see https://cloud.ibm.com/docs/speech-to-text/custom.html.
        /// </summary>
        [Obsolete("Use LanguageCustomizationId instead.")]
        public string CustomizationId { get { return _customization_id; } set { _customization_id = value; } }
        /// <summary>
        /// Specifies the Globally Unique Identifier (GUID) of a custom language model that is to be used for all requests sent over the connection. The base model of the custom language model must match the value of the model parameter. By default, no custom language model is used. For more information, see https://cloud.ibm.com/docs/speech-to-text/custom.html.
        /// </summary>
        public string LanguageCustomizationId { get { return _languageCustomizationId; } set { _languageCustomizationId = value; } }
        /// <summary>
        /// Specifies the Globally Unique Identifier (GUID) of a custom acoustic model that is to be used for all requests sent over the connection. The base model of the custom acoustic model must match the value of the model parameter. By default, no custom acoustic model is used. For more information, see https://cloud.ibm.com/docs/speech-to-text/custom.html.
        /// </summary>
        public string AcousticCustomizationId { get { return _acoustic_customization_id; } set { _acoustic_customization_id = value; } }
        /// <summary>
        /// Specifies the weight the service gives to words from a specified custom language model compared to those from the base model for all requests sent over the connection. Specify a value between 0.0 and 1.0; the default value is 0.3. For more information, see https://cloud.ibm.com/docs/speech-to-text?topic=speech-to-text-languageUse#weight.
        /// </summary>
        public float? CustomizationWeight { get { return _customization_weight; } set { _customization_weight = value; } }
        /// <summary>
        /// If true sets `Transfer-Encoding` request header to `chunked` causing the audio to be streamed to the service. By default, audio is sent all at once as a one-shot delivery. See https://cloud.ibm.com/docs/speech-to-text?topic=speech-to-text-input#transmission.
        /// </summary>
        public bool StreamMultipart { get { return _streamMultipart; } set { _streamMultipart = value; } }
        /// <summary>
        /// The name of a grammar that is to be used with the recognition request. If you specify a grammar, you must also use the `language_customization_id` parameter to specify the name of the custom language model for which the grammar is defined. The service recognizes only strings that are recognized by the specified grammar; it does not recognize other custom words from the model's words resource. See [Grammars](https://cloud.ibm.com/docs/speech-to-text/output.html).
        /// </summary>
        public string GrammarName { get; set; }
        /// <summary>
        /// If `true`, the service redacts, or masks, numeric data from final transcripts. The feature redacts any number that has three or more consecutive digits by replacing each digit with an `X` character. It is intended to redact sensitive numeric data, such as credit card numbers. By default, the service performs no redaction. \n\nWhen you enable redaction, the service automatically enables smart formatting, regardless of whether you explicitly disable that feature. To ensure maximum security, the service also disables keyword spotting (ignores the `keywords` and `keywords_threshold` parameters) and returns only a single final transcript (forces the `max_alternatives` parameter to be `1`). \n\n**Note:** Applies to US English, Japanese, and Korean transcription only. \n\nSee [Numeric redaction](https://cloud.ibm.com/docs/speech-to-text?topic=speech-to-text-output#redaction).
        /// </summary>
        public string Redaction { get; set; }
        /// <summary>
        /// If `true`, requests processing metrics about the service's transcription of
        /// the input audio. The service returns processing metrics at the interval specified by the
        /// `processing_metrics_interval` parameter. It also returns processing metrics for transcription events, for
        /// example, for final and interim results. By default, the service returns no processing metrics. (optional,
        /// default to false)
        /// </summary>
        public bool ProcessingMetrics { get; set; }
        /// <summary>
        /// Specifies the interval in real wall-clock seconds at which the
        /// service is to return processing metrics. The parameter is ignored unless the `processing_metrics` parameter
        /// is set to `true`.
        ///
        /// The parameter accepts a minimum value of 0.1 seconds. The level of precision is not restricted, so you can
        /// specify values such as 0.25 and 0.125.
        ///
        /// The service does not impose a maximum value. If you want to receive processing metrics only for
        /// transcription events instead of at periodic intervals, set the value to a large number. If the value is
        /// larger than the duration of the audio, the service returns processing metrics only for transcription events.
        /// (optional)
        /// </summary>
        public float? ProcessingMetricsInterval { get; set; }
        /// <summary>
        /// If `true`, specifies the duration of the pause service splits a transcript into multiple final results.
        /// If the service detects pauses or extended silence
        /// before it reaches the end of the audio stream, its response can include multiple final results. Silence
        /// indicates a point at which the speaker pauses between spoken words or phrases.
        ///
        /// Specify a value for the pause interval in the range of 0.0 to 120.0.
        /// * A value greater than 0 specifies the interval that the service is to use for speech recognition.
        /// * A value of 0 indicates that the service is to use the default interval. It is equivalent to omitting the
        /// parameter.
        ///
        /// The default pause interval for most languages is 0.8 seconds; the default for Chinese is 0.6 seconds.
        ///
        /// See [End of phrase silence
        /// time](https://cloud.ibm.com/docs/speech-to-text?topic=speech-to-text-output#silence_time).
        /// <summary>
        public double? EndOfPhraseSilenceTime { get; set; }
        /// <summary>
        /// If `true`, directs the service to split the transcript into
        /// multiple final results based on semantic features of the input, for example, at the conclusion of meaningful
        /// phrases such as sentences. The service bases its understanding of semantic features on the base language
        /// model that you use with a request. Custom language models and grammars can also influence how and where the
        /// service splits a transcript. By default, the service splits transcripts based solely on the pause interval.
        ///
        /// See [Split transcript at phrase
        /// end](https://cloud.ibm.com/docs/speech-to-text?topic=speech-to-text-output#split_transcript).
        /// (optional, default to false)
        /// <summary>
        public bool? SplitTranscriptAtPhraseEnd { get; set; }
        /// <summary>The sensitivity of speech activity detection that the service is to
        /// perform. Use the parameter to suppress word insertions from music, coughing, and other non-speech events.
        /// The service biases the audio it passes for speech recognition by evaluating the input audio against prior
        /// models of speech and non-speech activity.
        ///
        /// Specify a value between 0.0 and 1.0:
        /// * 0.0 suppresses all audio (no speech is transcribed).
        /// * 0.5 (the default) provides a reasonable compromise for the level of sensitivity.
        /// * 1.0 suppresses no audio (speech detection sensitivity is disabled).
        ///
        /// The values increase on a monotonic curve. See [Speech Activity
        /// Detection](https://cloud.ibm.com/docs/speech-to-text?topic=speech-to-text-input#detection).
        /// (optional)</summary>
        public float? SpeechDetectorSensitivity { get; set; }
        /// <summary>The level to which the service is to suppress background audio
        /// based on its volume to prevent it from being transcribed as speech. Use the parameter to suppress side
        /// conversations or background noise.
        ///
        /// Specify a value in the range of 0.0 to 1.0:
        /// * 0.0 (the default) provides no suppression (background audio suppression is disabled).
        /// * 0.5 provides a reasonable level of audio suppression for general usage.
        /// * 1.0 suppresses all audio (no audio is transcribed).
        ///
        /// The values increase on a monotonic curve. See [Speech Activity
        /// Detection](https://cloud.ibm.com/docs/speech-to-text?topic=speech-to-text-input#detection).
        /// (optional)</summary>
        public float? BackgroundAudioSuppression { get; set; }
        #endregion

        #region Sessionless - Streaming
        /// <summary>
        /// This callback object is used by the Recognize() and StartListening() methods.
        /// </summary>
        /// <param name="results">The ResultList object containing the results.</param>
        public delegate void OnRecognize(SpeechRecognitionEvent results);

        /// <summary>
        /// This callback object is used by the RecognizeSpeaker() method.
        /// </summary>
        /// <param name="speakerRecognitionEvent">Array of speaker label results.</param>
        public delegate void OnRecognizeSpeaker(SpeakerRecognitionEvent speakerRecognitionEvent);

        /// <summary>
        /// This starts the service listening and it will invoke the callback for any recognized speech.
        /// OnListen() must be called by the user to queue audio data to send to the service. 
        /// StopListening() should be called when you want to stop listening.
        /// </summary>
        /// <param name="callback">All recognize results are passed to this callback.</param>
        /// <param name="speakerLabelCallback">Speaker label goes through this callback if it arrives separately from recognize result.</param>
        /// <returns>Returns true on success, false on failure.</returns>
        public bool StartListening(OnRecognize callback, OnRecognizeSpeaker speakerLabelCallback = null)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (_isListening)
                return false;
            if (!CreateListenConnector())
                return false;

            Dictionary<string, string> customHeaders = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                customHeaders.Add(kvp.Key, kvp.Value);
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

                _listenSocket = WSConnector.CreateConnector(Authenticator, "/v1/recognize", "?model=" + UnityWebRequest.EscapeURL(_recognizeModel) + parsedParams, serviceUrl);
                Log.Debug("SpeechToText.CreateListenConnector()", "Created listen socket. Model: {0}, parsedParams: {1}", UnityWebRequest.EscapeURL(_recognizeModel), parsedParams);
                _listenSocket.DisableSslVerification = DisableSslVerification;
                if (_listenSocket == null)
                {
                    return false;
                }
                else
                {
#if ENABLE_DEBUGGING
                    Log.Debug("SpeechToText.CreateListenConnector()", "Created listen socket. Model: {0}, parsedParams: {1}", UnityWebRequest.EscapeURL(_recognizeModel), parsedParams);
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
                throw new IBMException("SendStart() called with null connector.");

            Dictionary<string, object> start = new Dictionary<string, object>();
            start["action"] = "start";
            start["content-type"] = "audio/l16;rate=" + _recordingHZ.ToString() + ";channels=1;";
            start["inactivity_timeout"] = InactivityTimeout;
            if (EnableInterimResults != null)
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
            if (GrammarName != null)
                start["grammar_name"] = GrammarName;
            if (Redaction != null)
                start["redaction"] = Redaction;
            if (EndOfPhraseSilenceTime != null)
                start["end_of_phrase_silence_time"] = EndOfPhraseSilenceTime;
            if (SplitTranscriptAtPhraseEnd != null)
                start["split_transcript_at_phrase_end"] = SplitTranscriptAtPhraseEnd;
            if (SpeechDetectorSensitivity != null)
                start["speech_detector_sensitivity"] = SpeechDetectorSensitivity;
            if (BackgroundAudioSuppression != null)
                start["background_audio_suppression"] = BackgroundAudioSuppression;
            if (ProcessingMetrics != null)
                start["processing_metrics"] = ProcessingMetrics;
            if (ProcessingMetricsInterval != null)
                start["processing_metrics_interval"] = ProcessingMetricsInterval;

            _listenSocket.Send(new WSConnector.TextMessage(Json.Serialize(start)));
#if ENABLE_DEBUGGING
            Log.Debug("SpeechToText.SendStart()", "SendStart() with the following params: {0}", Json.Serialize(start));
#endif
            _lastStartSent = DateTime.Now;
        }

        private void SendStop()
        {
            if (_listenSocket == null)
                throw new IBMException("SendStart() called with null connector.");

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
                    while (_keepAliveClip.loadState != AudioDataLoadState.Loaded)
                        yield return null;

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
                                _listenCallback(results);
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
                    result.confidence = Utility.StringToDouble(iresult["confidence"].ToString());
                    result.final = (bool)iresult["final"];
                    result.from = Utility.StringToDouble(iresult["from"].ToString());
                    result.to = Utility.StringToDouble(iresult["to"].ToString());
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
                                wordAlternativeResults.start_time = Utility.StringToDouble(iwordAlternative["start_time"].ToString());
                            if (iwordAlternative.Contains("end_time"))
                                wordAlternativeResults.end_time = Utility.StringToDouble(iwordAlternative["end_time"].ToString());
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
                                        wordAlternativeResult.confidence = Utility.StringToDouble(ialternative["confidence"].ToString());
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
                                alternative.confidence = Utility.StringToDouble(ialternative["confidence"].ToString());

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
                                    ts.Start = Utility.StringToDouble(itimestamp[1].ToString());
                                    ts.End = Utility.StringToDouble(itimestamp[2].ToString());
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
                                    wc.Confidence = Utility.StringToDouble(iwordconf[1].ToString());
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
                                    keywordResult.confidence = Utility.StringToDouble(iKeywordDictionary["confidence"].ToString());
                                    keywordResult.end_time = Utility.StringToDouble(iKeywordDictionary["end_time"].ToString());
                                    keywordResult.start_time = Utility.StringToDouble(iKeywordDictionary["start_time"].ToString());
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
    }

    #region Data modelss
    /// <summary>
    /// This data object contains data for the speechRecognitionEvent.
    /// </summary>
    public class SpeechRecognitionEvent
    {
        /// <summary>
        /// The results array consists of zero or more final results followed by zero or one interim result. The final results are guaranteed not to change; the interim result may be replaced by zero or more final results (followed by zero or one interim result). The service periodically sends updates to the result list, with the result_index set to the lowest index in the array that has changed.
        /// </summary>
        public SpeechRecognitionResult[] results { get; set; }
        /// <summary>
        /// An index that indicates the change point in the results array. 
        /// </summary>
        public int result_index { get; set; }
        /// <summary>
        /// An array that identifies which words were spoken by which speakers in a multi-person exchange. Returned in the response only if `speaker_labels` is `true`.
        /// </summary>
        public SpeakerLabelsResult[] speaker_labels { get; set; }
        /// <summary>
        /// An array of warning messages about invalid query parameters or JSON fields included with the request. Each warning includes a descriptive message and a list of invalid argument strings. For example, a message such as "Unknown arguments:" or "Unknown url query arguments:" followed by a list of the form "invalid_arg_1, invalid_arg_2." The request succeeds despite the warnings.
        /// </summary>
        public string[] warnings { get; set; }

        /// <exclude />
        public SpeechRecognitionEvent(SpeechRecognitionResult[] _results)
        {
            results = _results;
        }

        /// <summary>
        /// Check if our result list has atleast one valid result.
        /// </summary>
        /// <returns>Returns true if a result is found.</returns>
        public bool HasResult()
        {
            return results != null && results.Length > 0
              && results[0].alternatives != null && results[0].alternatives.Length > 0;
        }

        /// <summary>
        /// Returns true if we have a final result.
        /// </summary>
        /// <returns></returns>
        public bool HasFinalResult()
        {
            return HasResult() && results[0].final;
        }
    }

    /// <summary>
    /// This data object contains data for the speakerRecognitionEvent if speaker data comes separately from the speechRecognitionEvent.
    /// </summary>
    public class SpeakerRecognitionEvent
    {
        /// <summary>
        /// An array that identifies which words were spoken by which speakers in a multi-person exchange. Returned in the response only if `speaker_labels` is `true`.
        /// </summary>
        public SpeakerLabelsResult[] speaker_labels { get; set; }
    }

    /// <summary>
    /// The speech recognition result.
    /// </summary>
    public class SpeechRecognitionResult
    {
        /// <summary>
        /// If true, the result for this utterance is not updated further. 
        /// </summary>
        public bool final { get; set; }
        /// <summary>
        /// Array of alternative transcripts. 
        /// </summary>
        public SpeechRecognitionAlternative[] alternatives { get; set; }
        /// <summary>
        /// Dictionary (or associative array) whose keys are the strings specified for keywords if both that parameter and keywords_threshold are specified. A keyword for which no matches are found is omitted from the array.
        /// </summary>
        public KeywordResults keywords_result { get; set; }
        /// <summary>
        /// Array of word alternative hypotheses found for words of the input audio if word_alternatives_threshold is not null.
        /// </summary>
        public WordAlternativeResults[] word_alternatives { get; set; }
    }

    /// <summary>
    /// The alternatives.
    /// </summary>
    public class SpeechRecognitionAlternative
    {
        /// <summary>
        /// Transcription of the audio. 
        /// </summary>
        public string transcript { get; set; }
        /// <summary>
        /// Confidence score of the transcript in the range of 0 to 1. Available only for the best alternative and only in results marked as final.
        /// </summary>
        public double confidence { get; set; }
        /// <summary>
        /// Time alignments for each word from transcript as a list of lists. Each inner list consists of three elements: the word followed by its start and end time in seconds. Example: [["hello",0.0,1.2],["world",1.2,2.5]]. Available only for the best alternative. 
        /// </summary>
        public string[] timestamps { get; set; }
        /// <summary>
        /// Confidence score for each word of the transcript as a list of lists. Each inner list consists of two elements: the word and its confidence score in the range of 0 to 1. Example: [["hello",0.95],["world",0.866]]. Available only for the best alternative and only in results marked as final.
        /// </summary>
        public string[] word_confidence { get; set; }
        /// <summary>
        /// A optional array of timestamps objects.
        /// </summary>
        public TimeStamp[] Timestamps { get; set; }
        /// <summary>
        /// A option array of word confidence values.
        /// </summary>
        public WordConfidence[] WordConfidence { get; set; }
    }

    /// <summary>
    /// The Keword Result
    /// </summary>
    public class KeywordResults
    {
        /// <summary>
        /// List of each keyword entered via the keywords parameter and, for each keyword, an array of KeywordResult objects that provides information about its occurrences in the input audio. The keys of the list are the actual keyword strings. A keyword for which no matches are spotted in the input is omitted from the array.
        /// </summary>
        public KeywordResult[] keyword { get; set; }
    }

    /// <summary>
    /// This class holds a Word Alternative Result.
    /// </summary>
    public class WordAlternativeResults
    {
        /// <summary>
        /// Specified keyword normalized to the spoken phrase that matched in the audio input. 
        /// </summary>
        public double start_time { get; set; }
        /// <summary>
        /// Specified keyword normalized to the spoken phrase that matched in the audio input. 
        /// </summary>
        public double end_time { get; set; }
        /// <summary>
        /// Specified keyword normalized to the spoken phrase that matched in the audio input. 
        /// </summary>
        public WordAlternativeResult[] alternatives { get; set; }
    }

    /// <summary>
    /// This class holds Speaker Labels Result.
    /// </summary>
    public class SpeakerLabelsResult
    {
        /// <summary>
        /// The start time of a word from the transcript. The value matches the start time of a word from the `timestamps` array.
        /// </summary>
        public double from { get; set; }
        /// <summary>
        /// The end time of a word from the transcript. The value matches the end time of a word from the `timestamps` array.
        /// </summary>
        public double to { get; set; }
        /// <summary>
        /// The numeric identifier that the service assigns to a speaker from the audio. Speaker IDs begin at `0` initially but can evolve and change across interim results (if supported by the method) and between interim and final results as the service processes the audio. They are not guaranteed to be sequential, contiguous, or ordered.
        /// </summary>
        public Int64 speaker { get; set; }
        /// <summary>
        /// A score that indicates how confident the service is in its identification of the speaker in the range of 0 to 1.
        /// </summary>
        public double confidence { get; set; }
        /// <summary>
        /// An indication of whether the service might further change word and speaker-label results. A value of `true` means that the service guarantees not to send any further updates for the current or any preceding results; `false` means that the service might send further updates to the results.
        /// </summary>
        public bool final { get; set; }
    }

    /// <summary>
    /// This class holds a Keyword Result.
    /// </summary>
    public class KeywordResult
    {
        /// <summary>
        /// Original keyword requested by user. 
        /// </summary>
        public string keyword { get; set; }
        /// <summary>
        /// Specified keyword normalized to the spoken phrase that matched in the audio input. 
        /// </summary>
        public string normalized_text { get; set; }
        /// <summary>
        /// Start time in seconds of the keyword match.
        /// </summary>
        public double start_time { get; set; }
        /// <summary>
        /// End time in seconds of the keyword match. 
        /// </summary>
        public double end_time { get; set; }
        /// <summary>
        /// Confidence score of the keyword match in the range of 0 to 1.
        /// </summary>
        public double confidence { get; set; }
    }

    /// <summary>
    /// This data class holds a Word Alternative Result.
    /// </summary>
    public class WordAlternativeResult
    {
        /// <summary>
        /// Confidence score of the word alternative hypothesis. 
        /// </summary>
        public double confidence { get; set; }
        /// <summary>
        /// Word alternative hypothesis for a word from the input audio.
        /// </summary>
        public string word { get; set; }
    }

    /// <summary>
    /// This data class holds the sesion data.
    /// </summary>
    public class RecognizeStatus
    {
        /// <summary>
        /// Description of the state and possible actions for the current session
        /// </summary>
        public SessionStatus session { get; set; }
    }

    /// <summary>
    /// This data class holds the description of teh state and possbile actions for the current session.
    /// </summary>
    public class SessionStatus
    {
        /// <summary>
        /// State of the session. The state must be initialized to perform a new recognition request on the session. 
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// URI for information about the model that is used with the session. 
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// URI for REST recognition requests. 
        /// </summary>
        public string recognize { get; set; }
        /// <summary>
        /// URI for REST results observers. 
        /// </summary>
        public string observe_result { get; set; }
        /// <summary>
        /// URI for WebSocket recognition requests. Needed only for working with the WebSocket interface.
        /// </summary>
        public string recognizeWS { get; set; }
    }

    /// <summary>
    /// This data class holds the confidence value for a given recognized word.
    /// </summary>
    public class WordConfidence
    {
        /// <summary>
        /// The word as a string.
        /// </summary>
        public string Word { get; set; }
        /// <summary>
        /// The confidence value for this word.
        /// </summary>
        public double Confidence { get; set; }
    };
    /// <summary>
    /// This data class holds the start and stop times for a word.
    /// </summary>
    public class TimeStamp
    {
        /// <summary>
        /// The word.
        /// </summary>
        public string Word { get; set; }
        /// <summary>
        /// The start time.
        /// </summary>
        public double Start { get; set; }
        /// <summary>
        /// The stop time.
        /// </summary>
        public double End { get; set; }
    };
#endregion

}
