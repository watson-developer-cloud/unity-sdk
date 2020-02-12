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

namespace IBM.Watson.TextToSpeech.V1
{
    public partial class TextToSpeechService : BaseService
    {
        #region Constants
        /// <summary>
        /// How many recording Texts will we queue before we enter a error state.
        /// </summary>
        private const int MaxQueuedRecordings = 1000;
        #endregion

        #region Public Types
        /// <summary>
        /// This callback is used to return errors through the OnError property.
        /// </summary>
        /// <param name="error">A string containing the error message.</param>
        public delegate void ErrorEvent(string error);
        #endregion

        #region Private Data
        private OnSynthesize _listenCallback = null;        // Callback is set by StartListening()
        private WSConnector _listenSocket = null;          // WebSocket object used when StartAnalyzing() is invoked
        private bool _listenActive = false;
        private bool _isListening = false;
        private Queue<string> _listenTexts = new Queue<string>();
        private string _text = null;
        private string _accept = "audio/basic";
        private string _voice = "en-US_AllisonV3Voice";   // Voice to use.
        private string _customization_id = null;

        private string _url = "https://stream.watsonplatform.net/text-to-speech/api";
        #endregion

        #region Public Properties
        /// <summary>
        /// True if StartListening() has been called.
        /// </summary>
        public bool IsListening { get { return _isListening; } private set { _isListening = value; } }
        /// <summary>
        /// This delegate is invoked when an error occurs.
        /// </summary>
        public ErrorEvent OnError { get; set; }
        /// <summary>
        /// The requested format (MIME type) of the audio. You can use the `Accept` header or the
        /// `accept` parameter to specify the audio format. For more information about specifying an audio format, see
        /// **Audio formats (accept types)** in the method description. (optional, default to
        /// audio/ogg;codecs=opus).
        /// </summary>
        public string Accept { get { return _accept; } set { _accept = value; } }
        /// <summary>
        /// The voice to use for synthesis. (optional, default to en-US_MichaelVoice).
        /// </summary>
        public string Voice { get { return _voice; } set { _voice = value; } }
        /// <summary>
        /// The customization ID (GUID) of a custom voice model to use for the synthesis.
        /// If a custom voice model is specified, it is guaranteed to work only if it matches the language of the
        /// indicated voice. You must make the request with credentials for the instance of the service that owns the
        /// custom model. Omit the parameter to use the specified voice with no customization. (optional).
        /// </summary>
        public string CustomizationId { get { return _customization_id; } set { _customization_id = value; } }
        #endregion

        #region Sessionless - Streaming
        /// <summary>
        /// This callback object is used by the Synthesize().
        /// </summary>
        /// <param name="results">The ResultList object containing the results.</param>
        public delegate void OnSynthesize(byte[] result);


        /// <summary>
        /// This starts the service listening and it will invoke the callback for any recognized speech.
        /// OnListen() must be called by the user to queue audio data to send to the service. 
        /// StopListening() should be called when you want to stop listening.
        /// </summary>
        /// <param name="callback">All recognize results are passed to this callback.</param>
        /// <returns>Returns true on success, false on failure.</returns>
        public bool StartListening(OnSynthesize callback)
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

            // _listenSocket.Send(new WSConnector.TextMessage("start"));
            _isListening = true;
            _listenCallback = callback;

            return true;
        }

        /// <summary>
        /// This function should be invoked with the text input after StartListening() method has been invoked.
        /// The user should continue to invoke this function until they are ready to call StopListening().
        /// </summary>
        /// <param name="text">.</param>
        /// <returns>True if text was sent or enqueued, false if text was discarded.</returns>
        public bool OnListen(string text)
        {
            bool textSentOrEnqueued = false;

            if (_isListening)
            {
                Log.Debug("ExampleTextToSpeechV1", "on listen: {0}", text);
                Dictionary<string, string> data = new Dictionary<string, string>();
                data["text"] = text;
                _listenSocket.Send(new WSConnector.TextMessage(Json.Serialize(data)));
                textSentOrEnqueued = true;
            }
            return textSentOrEnqueued;
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

            _listenTexts.Clear();
            _listenCallback = null;

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
                if (!string.IsNullOrEmpty(Voice))
                {
                    queryParams["voice"] = Voice;
                }

                string parsedParams = "";

                foreach (KeyValuePair<string, string> kvp in queryParams)
                {
                    parsedParams += string.Format("{0}={1}", kvp.Key, kvp.Value);
                }

                _listenSocket = WSConnector.CreateConnector(Authenticator, "/v1/synthesize?",  parsedParams, serviceUrl);
                _listenSocket.Headers.Add("Content-Type", "application/json");
                _listenSocket.Headers.Add("Accept", Accept);

                Log.Debug("TextToSpeech.CreateListenConnector()", "Created listen socket. parsedParams: {0}", parsedParams);
                _listenSocket.DisableSslVerification = DisableSslVerification;
                if (_listenSocket == null)
                {
                    return false;
                }
                else
                {
#if ENABLE_DEBUGGING
                    Log.Debug("TextToSpeech.CreateListenConnector()", "Created listen socket. parsedParams: {0}", parsedParams);
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

        private void OnListenMessage(WSConnector.Message msg)
        {
            if (msg is WSConnector.TextMessage)
            {
                WSConnector.TextMessage tm = (WSConnector.TextMessage)msg;
                IDictionary json = Json.Deserialize(tm.Text) as IDictionary;
                if (json != null)
                {
                    if (json.Contains("error"))
                    {
                        string error = (string)json["error"];
                        Log.Error("TextToSpeech.OnListenMessage()", "Error: {0}", error);

                        StopListening();
                        if (OnError != null)
                            OnError(error);
                    }
                }
                else
                {
                    Log.Error("TextToSpeech.OnListenMessage()", "Failed to parse JSON from server: {0}", tm.Text);
                }
            }
            else if (msg is WSConnector.BinaryMessage)
            {
                Log.Debug("TextToSpeech.OnListenMessage()", "got message");
                WSConnector.BinaryMessage message = (WSConnector.BinaryMessage)msg;
                // OnSynthesize(message);
                // handle audio clip
            }

        }

        private void OnListenClosed(WSConnector connector)
        {
#if ENABLE_DEBUGGING
            Log.Debug("TextToSpeech.OnListenClosed()", "OnListenClosed(), State = {0}", connector.State.ToString());
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
    }
}
