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
        private const string SERVICE_ID = "SpeechToTextV1";
        /// <summary>
        /// How often to send a message to the web socket to keep it alive.
        /// </summary>
        private const float WS_KEEP_ALIVE_TIME = 20.0f;
        /// <summary>
        /// If no listen state is received after start is sent within this time, we will timeout
        /// and stop listening. 
        /// </summary>
        private const float LISTEN_TIMEOUT = 10.0f;
        /// <summary>
        /// How many recording AudioClips will we queue before we enter a error state.
        /// </summary>
        private const int MAX_QUEUED_RECORDINGS = 30;
        /// <summary>
        /// Size of a clip in bytes that can be sent through the Recognize function.
        /// </summary>
        private const int MAX_RECOGNIZE_CLIP_SIZE = 4 * (1024 * 1024);
        #endregion

        #region Public Types
        /// <summary>
        /// This callback is used to return errors through the OnError property.
        /// </summary>
        /// <param name="error">A string containing the error message.</param>
        public delegate void ErrorEvent(string error);
        #endregion

        #region Private Data
        private OnRecognize m_ListenCallback = null;				// Callback is set by StartListening()                                                             
        private WSConnector m_ListenSocket = null;					// WebSocket object used when StartListening() is invoked  
        private bool m_ListenActive = false;
        private bool m_AudioSent = false;
        private bool m_IsListening = false;
        private Queue<AudioData> m_ListenRecordings = new Queue<AudioData>();
        private int m_KeepAliveRoutine = 0;							// ID of the keep alive co-routine
        private DateTime m_LastKeepAlive = DateTime.Now;
        private DateTime m_LastStartSent = DateTime.Now;
        private string m_RecognizeModel = "en-US_BroadbandModel";   // ID of the model to use.
        private int m_MaxAlternatives = 1;							// maximum number of alternatives to return.
        private bool m_Timestamps = false;
        private bool m_WordConfidence = false;
        private bool m_DetectSilence = true;						// If true, then we will try not to record silence.
        private float m_SilenceThreshold = 0.03f;					// If the audio level is below this value, then it's considered silent.
        private int m_RecordingHZ = -1;
		private static fsSerializer sm_Serializer = new fsSerializer();
		#endregion

		#region Public Properties
		/// <summary>
		/// True if StartListening() has been called.
		/// </summary>
		public bool IsListening { get { return m_IsListening; } private set { m_IsListening = value; } }
        /// <summary>
        /// True if AudioData has been sent and we are recognizing speech.
        /// </summary>
        public bool AudioSent { get { return m_AudioSent; } }
        /// <summary>
        /// This delegate is invoked when an error occurs.
        /// </summary>
        public ErrorEvent OnError { get; set; }
        /// <summary>
        /// This property controls which recognize model we use when making recognize requests of the server.
        /// </summary>
        public string RecognizeModel
        {
            get { return m_RecognizeModel; }
            set
            {
                if (m_RecognizeModel != value)
                {
                    m_RecognizeModel = value;
                    StopListening();        // close any active connection when our model is changed.
                }
            }
        }
        /// <summary>
        /// Returns the maximum number of alternatives returned by recognize.
        /// </summary>
        public int MaxAlternatives { get { return m_MaxAlternatives; } set { m_MaxAlternatives = value; } }
        /// <summary>
        /// True to return timestamps of words with results.
        /// </summary>
        public bool EnableTimestamps { get { return m_Timestamps; } set { m_Timestamps = value; } }
        /// <summary>
        /// True to return word confidence with results.
        /// </summary>
        public bool EnableWordConfidence { get { return m_WordConfidence; } set { m_WordConfidence = value; } }
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
        public bool DetectSilence { get { return m_DetectSilence; } set { m_DetectSilence = value; } }
        /// <summary>
        /// A value from 1.0 to 0.0 that determines what is considered silence. If the audio level is below this value
        /// then we consider it silence.
        /// </summary>
        public float SilenceThreshold { get { return m_SilenceThreshold; } set { m_SilenceThreshold = value; } }
        #endregion

        #region Get Models
        /// <summary>
        /// This callback object is used by the GetModels() method.
        /// </summary>
        /// <param name="models"></param>
        public delegate void OnGetModels(Model[] models);

        /// <summary>
        /// This function retrieves all the language models that the user may use by setting the RecognizeModel 
        /// public property.
        /// </summary>
        /// <param name="callback">This callback is invoked with an array of all available models. The callback will
        /// be invoked with null on error.</param>
        /// <returns>Returns true if request has been made.</returns>
        public bool GetModels(OnGetModels callback)
        {
            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/models");
            if (connector == null)
                return false;

            GetModelsRequest req = new GetModelsRequest();
            req.Callback = callback;
            req.OnResponse = OnGetModelsResponse;

            return connector.Send(req);
        }

        private class GetModelsRequest : RESTConnector.Request
        {
            public OnGetModels Callback { get; set; }
        };

        private void OnGetModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            GetModelsRequest gmr = req as GetModelsRequest;
            if (gmr == null)
                throw new WatsonException("Unexpected request type.");

            Model[] models = null;
            if (resp.Success)
            {
                models = ParseGetModelsResponse(resp.Data);
                if (models == null)
                    Log.Error("SpeechToText", "Failed to parse GetModels response.");
            }
            if (gmr.Callback != null)
                gmr.Callback(models);
        }

        private Model[] ParseGetModelsResponse(byte[] data)
        {
            string jsonString = Encoding.UTF8.GetString(data);
            if (jsonString == null)
            {
                Log.Error("SpeechToText", "Failed to get JSON string from response.");
                return null;
            }

            IDictionary json = (IDictionary)Json.Deserialize(jsonString);
            if (json == null)
            {
                Log.Error("SpechToText", "Failed to parse JSON: {0}", jsonString);
                return null;
            }

            try
            {
                List<Model> models = new List<Model>();

                IList imodels = json["models"] as IList;
                if (imodels == null)
                    throw new Exception("Expected IList");

                foreach (var m in imodels)
                {
                    IDictionary imodel = m as IDictionary;
                    if (imodel == null)
                        throw new Exception("Expected IDictionary");

                    Model model = new Model();
                    model.name = (string)imodel["name"];
                    model.rate = (long)imodel["rate"];
                    model.language = (string)imodel["language"];
                    model.description = (string)imodel["description"];
                    model.url = (string)imodel["url"];

                    models.Add(model);
                }

                return models.ToArray();
            }
            catch (Exception e)
            {
                Log.Error("SpeechToText", "Caught exception {0} when parsing GetModels() response: {1}", e.ToString(), jsonString);
            }

            return null;
        }
		#endregion

		#region Get Model
		/// <summary>
		/// This callback object is used by the GetModel() method.
		/// </summary>
		/// <param name="model">The resultant model.</param>
		public delegate void OnGetModel(Model model);

		/// <summary>
		/// This function retrieves a specified languageModel.
		/// </summary>
		/// <param name="callback">This callback is invoked with the requested model. The callback will
		/// be invoked with null on error.</param>
		/// <param name="modelID">The name of the model to get.</param>
		/// <returns>Returns true if request has been made.</returns>
		public bool GetModel(OnGetModel callback, string modelID)
		{
			if (string.IsNullOrEmpty(modelID))
				throw new ArgumentNullException("modelID");

			RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/models/" + modelID);
			if (connector == null)
				return false;

			GetModelRequest req = new GetModelRequest();
			req.Callback = callback;
			req.ModelID = modelID;
			req.OnResponse = OnGetModelResponse;

			return connector.Send(req);
		}

		private class GetModelRequest : RESTConnector.Request
		{
			public OnGetModel Callback { get; set; }
			public string ModelID { get; set; }
		};

		private void OnGetModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
		{
			Model response = new Model();
			if (resp.Success)
			{
				try
				{
					fsData data = null;
					fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
					if (!r.Succeeded)
						throw new WatsonException(r.FormattedMessages);

					object obj = response;
					r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
					if (!r.Succeeded)
						throw new WatsonException(r.FormattedMessages);
				}
				catch(Exception e)
				{
					Log.Error("SpeechToText", "Caught exception {0} when parsing GetModel() response: {1}", e.ToString(), Encoding.UTF8.GetString(resp.Data));
				}

				if (resp == null)
					Log.Error("SpeechToText", "Failed to parse GetModel response.");
			}

			if (((GetModelRequest)req).Callback != null)
				((GetModelRequest)req).Callback(resp.Success ? response :null);
		}
		#endregion

		#region Create Session
		/// <summary>
		/// This callback object is used by the CreateSession() method.
		/// </summary>
		/// <param name="session">The session data.</param>
		/// <param name="customData">Optional custom data.</param>
		public delegate void OnCreateSession(Session session, string customData);

		/// <summary>
		/// This function creates a speech to text session.
		/// </summary>
		/// <param name="callback">This callback is invoked with the sesion data. The callback will be invoked with null on error.</param>
		/// <param name="modelName">The model name to create the session using.</param>
		/// <param name="customData">Optional custom data.vc  </param>
		/// <returns></returns>
		public bool CreateSession(OnCreateSession callback, string modelName, string customData = default(string))
		{
			if (string.IsNullOrEmpty(modelName))
				throw new ArgumentNullException("modelName");

			RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/sessions");
			if (connector == null)
				return false;

			CreateSessionReq req = new CreateSessionReq();
			req.Callback = callback;
			req.ModelName = modelName;
			req.Data = customData;
			req.Parameters["model"] = modelName;
			req.Headers["Content-Type"] = "application/json";
			req.Headers["Accept"] = "application/json";
			req.Send = Encoding.UTF8.GetBytes("{}");
			req.OnResponse = CreateSessionResponse;

			return connector.Send(req);
		}

		private class CreateSessionReq : RESTConnector.Request
		{
			public OnCreateSession Callback { get; set; }
			public string ModelName { get; set; }
			public string Data { get; set; }
		}

		private void CreateSessionResponse(RESTConnector.Request req, RESTConnector.Response resp)
		{
			Session response = new Session();
			if (resp.Success)
			{
				try
				{
					fsData data = null;
					fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
					if (!r.Succeeded)
						throw new WatsonException(r.FormattedMessages);

					object obj = response;
					r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);

					string cookie;
					if (resp.ResponseHeaders.TryGetValue("Watson-DPAT", out cookie))
						response.session_cookie = cookie;
					else
						Log.Warning("SpeechToText", "Failed to get session cookie!");

					if (!r.Succeeded)
						throw new WatsonException(r.FormattedMessages);
				}
				catch (Exception e)
				{
					Log.Error("SpeechToText", "CreateSessionResponse Exception: {0}", e.ToString());
					resp.Success = false;
				}
			}

			if (((CreateSessionReq)req).Callback != null)
				((CreateSessionReq)req).Callback(resp.Success ? response : null, ((CreateSessionReq)req).Data);
		}
		#endregion

		#region Delete Session
		/// <summary>
		/// This callback is used by the DeleteSession() function.
		/// </summary>
		/// <param name="success"></param>
		/// <param name="data"></param>
		public delegate void OnDeleteSessionCallback(bool success, string data);
		/// <summary>
		/// Deletes an existing session and its engine. You cannot send requests to a session after it is deleted.
		/// </summary>
		/// <param name="callback">The callback.</param>
		/// <param name="sessionID">The ID of the session to be deleted.</param>
		/// <param name="customData">Optional custom data.</param>
		/// <returns></returns>
		public bool DeleteSession(OnDeleteSessionCallback callback, string sessionID, string session_cookie, string customData = default(string))
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			if (string.IsNullOrEmpty(sessionID))
				throw new ArgumentNullException("A sessionID to delete is required for DeleteSession");

			DeleteSessionRequest req = new DeleteSessionRequest();
			req.Callback = callback;
			req.SessionID = sessionID;
			req.SessionCookie = session_cookie;
			req.Data = customData;
			req.Delete = true;
			req.OnResponse = OnDeleteSessionResp;
			req.Headers["Cookie"] = session_cookie;
			
			string service = "/v1/sessions/{0}";
			RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(service, sessionID));
			if (connector == null)
				return false;

			return connector.Send(req);
		}

		private class DeleteSessionRequest : RESTConnector.Request
		{
			public OnDeleteSessionCallback Callback { get; set; }
			public string SessionID { get; set; }
			public string SessionCookie { get; set; }
			public string Data { get; set; }
		}

		private void OnDeleteSessionResp(RESTConnector.Request req, RESTConnector.Response resp)
		{
			if (((DeleteSessionRequest)req).Callback != null)
				((DeleteSessionRequest)req).Callback(resp.Success, ((DeleteSessionRequest)req).Data);
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
            if (m_IsListening)
                return false;
            if (!CreateListenConnector())
                return false;

            m_IsListening = true;
            m_ListenCallback = callback;
            m_KeepAliveRoutine = Runnable.Run(KeepAlive());
            m_LastKeepAlive = DateTime.Now;

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
            if (m_IsListening)
            {
                if (m_RecordingHZ < 0)
                {
                    m_RecordingHZ = clip.Clip.frequency;
                    SendStart();
                }

                if (!DetectSilence || clip.MaxLevel >= m_SilenceThreshold)
                {
                    if (m_ListenActive)
                    {
                        m_ListenSocket.Send(new WSConnector.BinaryMessage(AudioClipUtil.GetL16(clip.Clip)));
                        m_AudioSent = true;
                    }
                    else
                    {
                        // we have not received the "listening" state yet from the server, so just queue
                        // the audio clips until that happens.
                        m_ListenRecordings.Enqueue(clip);

                        // check the length of this queue and do something if it gets too full.
                        if (m_ListenRecordings.Count > MAX_QUEUED_RECORDINGS)
                        {
                            Log.Error("SpeechToText", "Recording queue is full.");

                            StopListening();
                            if (OnError != null)
                                OnError("Recording queue is full.");
                        }
                    }
                }
                else if (m_AudioSent)
                {
                    SendStop();
                    m_AudioSent = false;
                }

                // After sending start, we should get into the listening state within the amount of time specified
                // by LISTEN_TIMEOUT. If not, then stop listening and record the error.
                if (!m_ListenActive && (DateTime.Now - m_LastStartSent).TotalSeconds > LISTEN_TIMEOUT)
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
            if (!m_IsListening)
                return false;

            m_IsListening = false;
            CloseListenConnector();

            if (m_KeepAliveRoutine != 0)
            {
                Runnable.Stop(m_KeepAliveRoutine);
                m_KeepAliveRoutine = 0;
            }

            m_ListenRecordings.Clear();
            m_ListenCallback = null;
            m_RecordingHZ = -1;

            return true;
        }

        private bool CreateListenConnector()
        {
            if (m_ListenSocket == null)
            {
                m_ListenSocket = WSConnector.CreateConnector(SERVICE_ID, "/v1/recognize", "?model=" + WWW.EscapeURL(m_RecognizeModel));
                if (m_ListenSocket == null)
                    return false;

                m_ListenSocket.OnMessage = OnListenMessage;
                m_ListenSocket.OnClose = OnListenClosed;
            }

            return true;
        }

        private void CloseListenConnector()
        {
            if (m_ListenSocket != null)
            {
                m_ListenSocket.Close();
                m_ListenSocket = null;
            }
        }

        private void SendStart()
        {
            if (m_ListenSocket == null)
                throw new WatsonException("SendStart() called with null connector.");

            Dictionary<string, object> start = new Dictionary<string, object>();
            start["action"] = "start";
            start["content-type"] = "audio/l16;rate=" + m_RecordingHZ.ToString() + ";channels=1;";
            start["continuous"] = EnableContinousRecognition;
            start["max_alternatives"] = m_MaxAlternatives;
            start["interim_results"] = EnableInterimResults;
            start["word_confidence"] = m_WordConfidence;
            start["timestamps"] = m_Timestamps;

            m_ListenSocket.Send(new WSConnector.TextMessage(Json.Serialize(start)));
            m_LastStartSent = DateTime.Now;
        }

        private void SendStop()
        {
            if (m_ListenSocket == null)
                throw new WatsonException("SendStart() called with null connector.");

            if (m_ListenActive)
            {
                Dictionary<string, string> stop = new Dictionary<string, string>();
                stop["action"] = "stop";

                m_ListenSocket.Send(new WSConnector.TextMessage(Json.Serialize(stop)));
                m_LastStartSent = DateTime.Now;     // sending stop, will send the listening state again..
                m_ListenActive = false;
            }
        }

        // This keeps the WebSocket connected when we are not sending any data.
        private IEnumerator KeepAlive()
        {
            while (m_ListenSocket != null)
            {
                yield return null;

                if ((DateTime.Now - m_LastKeepAlive).TotalSeconds > WS_KEEP_ALIVE_TIME)
                {
                    Dictionary<string, string> nop = new Dictionary<string, string>();
                    nop["action"] = "no-op";

#if ENABLE_DEBUGGING
                    Log.Debug("SpeechToText", "Sending keep alive.");
#endif
                    m_ListenSocket.Send(new WSConnector.TextMessage(Json.Serialize(nop)));
                    m_LastKeepAlive = DateTime.Now;
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

                            if (m_ListenCallback != null)
                                m_ListenCallback(results);
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
                            if (m_IsListening)
                            {
                                if (!m_ListenActive)
                                {
                                    m_ListenActive = true;

                                    // send all pending audio clips ..
                                    while (m_ListenRecordings.Count > 0)
                                    {
                                        AudioData clip = m_ListenRecordings.Dequeue();
                                        m_ListenSocket.Send(new WSConnector.BinaryMessage(AudioClipUtil.GetL16(clip.Clip)));
                                        m_AudioSent = true;
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

            m_ListenActive = false;
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

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/recognize");
            if (connector == null)
                return false;

            RecognizeRequest req = new RecognizeRequest();
            req.Clip = clip;
            req.Callback = callback;

            req.Headers["Content-Type"] = "audio/wav";
            req.Send = WaveFile.CreateWAV(clip);
            if (req.Send.Length > MAX_RECOGNIZE_CLIP_SIZE)
            {
                Log.Error("SpeechToText", "AudioClip is too large for Recognize().");
                return false;
            }
            req.Parameters["model"] = m_RecognizeModel;
            req.Parameters["continuous"] = "false";
            req.Parameters["max_alternatives"] = m_MaxAlternatives.ToString();
            req.Parameters["timestamps"] = m_Timestamps ? "true" : "false";
            req.Parameters["word_confidence"] = m_WordConfidence ? "true" : "false";
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
		/// <param name="data">Optional custom data.</param>
		public delegate void GetCustomizationsCallback(Customizations customizations, string data);

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

			RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/customizations");
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
			if (resp.Success)
			{
				try
				{
					fsData data = null;
					fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
					if (!r.Succeeded)
						throw new WatsonException(r.FormattedMessages);

					object obj = customizations;
					r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
					if (!r.Succeeded)
						throw new WatsonException(r.FormattedMessages);
				}
				catch (Exception e)
				{
					Log.Error("Speech To Text", "GetCustomizations Exception: {0}", e.ToString());
					resp.Success = false;
				}
			}

			if (((GetCustomizationsReq)req).Callback != null)
				((GetCustomizationsReq)req).Callback(resp.Success ? customizations : null, ((GetCustomizationsReq)req).Data);
		}
		#endregion

		#region Create Custom Model
		/// <summary>
		/// Thid callback is used by the CreateCustomization() function.
		/// </summary>
		/// <param name="customizationID">The customizationID.</param>
		/// <param name="data">Optional custom data.</param>
		public delegate void CreateCustomizationCallback(CustomizationID customizationID, string data);

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
			customLanguage.description = description;

			fsData data;
			sm_Serializer.TrySerialize(customLanguage.GetType(), customLanguage, out data).AssertSuccessWithoutWarnings();
			string customizationJson = fsJsonPrinter.CompressedJson(data);

			CreateCustomizationRequest req = new CreateCustomizationRequest();
			req.Callback = callback;
			req.CustomLanguage = customLanguage;
			req.Data = customData;
			req.Headers["Content-Type"] = "application/json";
			req.Headers["Accept"] = "application/json";
			req.Send = Encoding.UTF8.GetBytes(customizationJson);
			req.OnResponse = OnCreateCustomizationResp;

			RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/customizations");
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
			if (resp.Success)
			{
				try
				{
					fsData data = null;
					fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
					if (!r.Succeeded)
						throw new WatsonException(r.FormattedMessages);

					object obj = customizationID;
					r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
					if (!r.Succeeded)
						throw new WatsonException(r.FormattedMessages);
				}
				catch (Exception e)
				{
					Log.Error("Speech To Text", "CreateCustomization Exception: {0}", e.ToString());
					resp.Success = false;
				}
			}

			if (((CreateCustomizationRequest)req).Callback != null)
				((CreateCustomizationRequest)req).Callback(resp.Success ? customizationID : null, ((CreateCustomizationRequest)req).Data);
		}
		#endregion

		#region Delete Custom Model
		/// <summary>
		/// This callback is used by the DeleteCustomization() function.
		/// </summary>
		/// <param name="success"></param>
		/// <param name="data"></param>
		public delegate void OnDeleteCustomizationCallback(bool success, string data);
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
			RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(service, customizationID));
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
		#endregion

		#region Train Custom Model
		#endregion

		#region Reset Custom Model
		#endregion

		#region Upgrade Custom Model
		#endregion

		#region Custom Corpora
		#endregion

		#region Custom Words
		#endregion

		#region IWatsonService interface
		/// <exclude />
		public string GetServiceID()
        {
            return SERVICE_ID;
        }

        /// <exclude />
        public void GetServiceStatus(ServiceStatus callback)
        {
            if (Config.Instance.FindCredentials(SERVICE_ID) != null)
                new CheckServiceStatus(this, callback);
            else
                callback(SERVICE_ID, false);
        }

        private class CheckServiceStatus
        {
            private SpeechToText m_Service = null;
            private ServiceStatus m_Callback = null;

            public CheckServiceStatus(SpeechToText service, ServiceStatus callback)
            {
                m_Service = service;
                m_Callback = callback;

                if (!m_Service.GetModels(OnCheckService))
                    m_Callback(SERVICE_ID, false);
            }

            private void OnCheckService(Model[] models)
            {
                if (m_Callback != null && m_Callback.Target != null)
                    m_Callback(SERVICE_ID, models != null);
            }
        };
        #endregion
    }
}
