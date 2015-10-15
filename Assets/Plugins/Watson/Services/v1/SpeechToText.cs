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
* @author Richard Lyle (rolyle@us.ibm.com)
*/

using IBM.Watson.Logging;
using IBM.Watson.Connection;
using IBM.Watson.Utilities;
using MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

namespace IBM.Watson.Services.v1            // Add DeveloperCloud
{
    class SpeechToText
    {
        #region Constants
        /// <summary>
        /// This ID is used to match up a configuration record with this service.
        /// </summary>
        const string SERVICE_ID = "SpeechToTextV1";
        /// <summary>
        /// How often to send a message to the web socket to keep it alive.
        /// </summary>
        const float WS_KEEP_ALIVE_TIME = 20.0f;
        /// <summary>
        /// Size in seconds of the recording buffer. AudioClips() will be generated each time
        /// the recording hits the halfway point.
        /// </summary>
        const int RECORDING_BUFFER_SIZE = 2;
        /// <summary>
        /// How many recording AudioClips will we queue before we enter a error state.
        /// </summary>
        const int MAX_QUEUED_RECORDINGS = 30;
        /// <summary>
        /// Size of a clip in bytes that can be sent through the Recognize function.
        /// </summary>
        const int MAX_RECOGNIZE_CLIP_SIZE = 4 * (1024 * 1024);
        #endregion

        #region Public Types
        public class Model
        {
            public string Name { get; set; }
            public long Rate { get; set; }
            public string Language { get; set; }
            public string Description { get; set; }
            public string URL { get; set; }
        };
        public class WordConfidence
        {
            public string Word { get; set; }
            public double Confidence { get; set; }
        };
        public class TimeStamp
        {
            public string Word { get; set; }
            public double Start { get; set; }
            public double End { get; set; }
        };
        public class Alternative
        {
            public string Transcript { get; set; }
            public double Confidence { get; set; }
            public TimeStamp[] Timestamps { get; set; }
            public WordConfidence[] WordConfidence { get; set; }
        };
        public class Result
        {
            public bool Final { get; set; }
            public Alternative[] Alternatives { get; set; }
        };
        public class ResultList
        {
            public Result[] Results { get; set; }

            public ResultList(Result[] results)
            {
                Results = results;
            }
        };
        public class RecordClip
        {
            public AudioClip Clip { get; set; }
            public float MaxLevel { get; set; }
        };
        public delegate void OnRecordClip(RecordClip clip);
        public delegate void OnRecognize(ResultList results);
        public delegate void OnGetModels(Model[] models);
        public delegate void ErrorEvent(string error);
        #endregion

        #region Private Data
        private OnRecognize m_ListenCallback = null;        // Callback is set by StartListening()                                                             
        private WSConnector m_ListenSocket = null;          // WebSocket object used when StartListening() is invoked  
        private bool m_ListenActive = false;
        private bool m_AudioSent = false;
        private bool m_IsListening = false;
        private Queue<AudioClip> m_ListenRecordings = new Queue<AudioClip>();
        private int m_KeepAliveRoutine = 0;                      // ID of the keep alive co-routine
        private float m_LastWSMessage = 0.0f;               // last time we sent a message on the WS
        private string m_RecognizeModel = "en-US_BroadbandModel";    // ID of the model to use.
        private int m_MaxAlternatives = 1;                  // maximum number of alternatives to return.
        private bool m_Timestamps = false;
        private bool m_WordConfidence = false;
        private OnRecordClip m_RecordingCallback = null;
        private int m_RecordingRoutine = 0;                      // ID of our co-routine when recording, 0 if not recording currently.
        private AudioClip m_Recording = null;
        private int m_RecordingHZ = 22050;                  // default recording HZ
        private string m_MicrophoneID = null;               // what microphone to use for recording.
        private bool m_DetectSilence = true;                // If true, then we will try not to record silence.
        private float m_SilenceThreshold = 0.03f;           // If the audio level is below this value, then it's considered silent.
        #endregion

        #region Public Properties
        /// <summary>
        /// This delegate is invoked when an error occurs.
        /// </summary>
        public ErrorEvent OnError { get; set; }
        /// <summary>
        /// This property controls which recognize model we use when making recognize requests of the server.
        /// </summary>
        public string RecognizeModel { get { return m_RecognizeModel; } set { m_RecognizeModel = value; } }
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
        /// Returns a list of available microphone devices.
        /// </summary>
        public string[] Microphones { get { return Microphone.devices; } }
        /// <summary>
        /// Microphone recording HZ.
        /// </summary>
        public int RecordingHZ { get { return m_RecordingHZ; } set { m_RecordingHZ = value; } }
        /// <summary>
        /// Returns the name of the selected microphone. If this is null or empty, then the default microphone will be used.
        /// </summary>
        public string MicrophoneID { get { return m_MicrophoneID; } set { m_MicrophoneID = value; } }
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

        #region Listening Functions

        /// <summary>
        /// Checks if StartListening() has already been called.
        /// </summary>
        /// <returns>true is returned if we are already listening.</returns>
        public bool IsListening()
        {
            return m_IsListening;
        }

        /// <summary>
        /// This starts the service listening to the microphone and invoking the callback for any recognized 
        /// speech. StopListening() should be called to stop this service from sending audio data to the
        /// server. This function can send a continuous stream of audio to the server for processing.
        /// </summary>
        /// <param name="callback">All recognize results are passed to this callback.</param>
        /// <returns>Returns true on success, false on failure.</returns>
        public bool StartListening(OnRecognize callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (IsListening())
                return false;
            if (!CreateListenConnector())
                return false;

            m_IsListening = true;
            m_ListenCallback = callback;
            StartRecording(OnListenRecord);
            m_KeepAliveRoutine = Runnable.Run(KeepAlive());

            SendStart();
            return true;
        }

        /// <summary>
        /// Invoke this function stop this service from listening.
        /// </summary>
        /// <returns>Returns true on success, false on failure.</returns>
        public bool StopListening()
        {
            if (!IsListening())
                return false;

            m_IsListening = false;
            CloseListenConnector();

            if (m_KeepAliveRoutine != 0)
            {
                Runnable.Stop(m_KeepAliveRoutine);
                m_KeepAliveRoutine = 0;
            }

            if (IsRecording())
                StopRecording();

            m_ListenRecordings.Clear();
            m_ListenCallback = null;

            return true;
        }

        private bool CreateListenConnector()
        {
            if (m_ListenSocket == null)
            {
                m_ListenSocket = WSConnector.CreateConnector( SERVICE_ID, "/v1/recognize", "?model=" + WWW.EscapeURL(m_RecognizeModel) );
                if ( m_ListenSocket == null )
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
            start["continuous"] = true;
            start["max_alternatives"] = m_MaxAlternatives;
            start["interim_results"] = true;
            start["word_confidence"] = m_WordConfidence;
            start["timestamps"] = m_Timestamps;

            m_ListenSocket.Send(new WSConnector.TextMessage(Json.Serialize(start)));
            m_LastWSMessage = Time.time;
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
                m_LastWSMessage = Time.time;
                m_ListenActive = false;
            }
        }

        // This keeps the WebSocket connected when we are not sending any data.
        private IEnumerator KeepAlive()
        {
            while (m_ListenSocket != null)
            {
                yield return null;

                if (Time.time > (m_LastWSMessage + WS_KEEP_ALIVE_TIME))
                {
                    Dictionary<string, string> nop = new Dictionary<string, string>();
                    nop["action"] = "no-op";

                    m_ListenSocket.Send(new WSConnector.TextMessage(Json.Serialize(nop)));
                    m_LastWSMessage = Time.time;
                }
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
                    if (json.Contains("results"))
                    {
                        ResultList results = ParseRecognizeResponse(json);
                        if (results != null)
                        {
                            if ( m_ListenCallback != null )
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

                        Log.Status("SpeechToText", "Server state is {0}", state);
                        if (state == "listening")
                        {
                            if (m_ListenActive)
                                Log.Warning("SpeechToText", "Already in listen active state.");

                            if ( m_IsListening )
                            {
                                m_ListenActive = true;

                                // send all pending audio clips ..
                                while (m_ListenRecordings.Count > 0)
                                {
                                    AudioClip clip = m_ListenRecordings.Dequeue();
                                    m_ListenSocket.Send(new WSConnector.BinaryMessage(AudioClipUtil.GetL16(clip)));
                                    m_LastWSMessage = Time.time;
                                    m_AudioSent = true;
                                }
                            }
                        }

                    }
                    else if (json.Contains("error"))
                    {
                        Log.Error("SpeechToText", "WebSocket error: {0}", (string)json["error"]);
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
            if (connector.State == WSConnector.ConnectionState.DISCONNECTED)
            {
                Log.Error("SpeechToText", "Disconnected from server.");

                m_ListenActive = false;
                StopListening();

                if (OnError != null)
                    OnError("Disconnected from server.");
            }
        }

        private void OnListenRecord(RecordClip clip)
        {
            if ( m_IsListening )
            {
                if (!DetectSilence || clip.MaxLevel >= m_SilenceThreshold)
                {
                    if (m_ListenActive)
                    {
                        m_ListenSocket.Send(new WSConnector.BinaryMessage(AudioClipUtil.GetL16(clip.Clip)));
                        m_LastWSMessage = Time.time;
                        m_AudioSent = true;
                    }
                    else
                    {
                        // we have not received the "listening" state yet from the server, so just queue
                        // the audio clips until that happens.
                        m_ListenRecordings.Enqueue(clip.Clip);

                        // We need to check the length of this queue and do something if it gets too full.
                        if ( m_ListenRecordings.Count > MAX_QUEUED_RECORDINGS )
                        {
                            Log.Error( "SpeechToText", "Recording queue has hit the maximum size." );

                            StopListening();
                            if ( OnError != null )
                                OnError( "Recording queue is full." );
                        }
                    }
                }
                else if (m_AudioSent)
                {
                    SendStop();
                    m_AudioSent = false;
                }
            }
        }
        #endregion


        #region GetModels Functions
        /// <summary>
        /// This function retrieves all the language models that the user may use by setting the RecognizeModel 
        /// public property.
        /// </summary>
        /// <param name="callback">This callback is invoked with an array of all available models. The callback will
        /// be invoked with null on error.</param>
        /// <returns>Returns true if request has been made.</returns>
        public bool GetModels(OnGetModels callback)
        {
            RESTConnector connector = RESTConnector.GetConnector( SERVICE_ID, "/v1/models" );
            if ( connector == null )
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

            try {
                List<Model> models = new List<Model>();

                IList imodels = json["models"] as IList;
                if (imodels == null)
                    throw new Exception( "Expected IList" );

                foreach( var m in imodels )
                {
                    IDictionary imodel = m as IDictionary;
                    if (imodel == null)
                        throw new Exception( "Expected IDictionary" );

                    Model model = new Model();
                    model.Name = (string)imodel["name"];
                    model.Rate = (long)imodel["rate"];
                    model.Language = (string)imodel["language"];
                    model.Description = (string)imodel["description"];
                    model.URL = (string)imodel["url"];
                    
                    models.Add( model );
                }

                return models.ToArray();
            }
            catch( Exception e )
            {
                Log.Error( "SpeechToText", "Caught exception {0} when parsing GetModels() response: {1}", e.ToString(), jsonString );
            }

            return null;
        }
        #endregion

        #region Recognize Functions
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

            RESTConnector connector = RESTConnector.GetConnector( SERVICE_ID, "/v1/recognize" );
            if (connector == null )
                return false;

            RecognizeRequest req = new RecognizeRequest();
            req.Clip = clip;
            req.Callback = callback;

            req.Headers["Content-Type"] = "audio/wav";
            req.Send = WaveFile.CreateWAV(clip);
            if (req.Send.Length > MAX_RECOGNIZE_CLIP_SIZE )
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

            ResultList result = null;
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
                        resp.ElapsedTime, result.Results.Length);
                }
            }
            else
            {
                Log.Error("SpeechToText", "Recognize Error: {0}", resp.Error);
            }

            if (recognizeReq.Callback != null)
                recognizeReq.Callback(result);
        }

        private ResultList ParseRecognizeResponse(byte[] json)
        {
            string jsonString = Encoding.UTF8.GetString(json);
            if (jsonString == null)
                return null;

            IDictionary resp = (IDictionary)Json.Deserialize(jsonString);
            if (resp == null)
                return null;

            return ParseRecognizeResponse(resp);
        }

        private ResultList ParseRecognizeResponse(IDictionary resp)
        {
            if (resp == null)
                return null;

            try
            {
                List<Result> results = new List<Result>();
                IList iresults = resp["results"] as IList;
                if (iresults == null)
                    return null;

                foreach (var r in iresults)
                {
                    IDictionary iresult = r as IDictionary;
                    if (iresults == null)
                        continue;

                    Result result = new Result();
                    result.Final = (bool)iresult["final"];

                    IList ialternatives = iresult["alternatives"] as IList;
                    if (ialternatives == null)
                        continue;

                    List<Alternative> alternatives = new List<Alternative>();
                    foreach (var a in ialternatives)
                    {
                        IDictionary ialternative = a as IDictionary;
                        if (ialternative == null)
                            continue;

                        Alternative alternative = new Alternative();
                        alternative.Transcript = (string)ialternative["transcript"];
                        if (ialternative.Contains("confidence"))
                            alternative.Confidence = (double)ialternative["confidence"];

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
                    result.Alternatives = alternatives.ToArray();
                    results.Add(result);
                }

                return new ResultList(results.ToArray());
            }
            catch (Exception e)
            {
                Log.Error("SpeechToText", "ParseJsonResponse exception: {0}", e.ToString());
                return null;
            }
        }
        #endregion

        #region Recording Functions
        public bool IsRecording()
        {
            return m_RecordingRoutine != 0;
        }

        /// <summary>
        /// This function begins recording from the microphone and passes all recorded audio to the provided callback function. 
        /// </summary>
        /// <param name="callback"></param>
        /// <returns>Returns true on success.</returns>
        public bool StartRecording(OnRecordClip callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (m_RecordingRoutine != 0)
            {
                Log.Error("SpeechToText", "StartRecording() invoked, recording already active.");
                return false;
            }

            m_RecordingCallback = callback;
            m_RecordingRoutine = Runnable.Run(RecordingHandler());

            return true;
        }

        /// <summary>
        /// Stop recording from the configured microphone.
        /// </summary>
        /// <returns>Returns true on success.</returns>
        public bool StopRecording()
        {
            if (m_RecordingRoutine == 0)
            {
                Log.Error("SpeechToText", "StopRecording() called, no active recording.");
                return false;
            }

            // TODO: We may need to figure out a method to get the last bit of audio out of our buffer.
            Microphone.End(m_MicrophoneID);
            Runnable.Stop(m_RecordingRoutine);
            m_RecordingRoutine = 0;
            m_RecordingCallback = null;
            m_Recording = null;

            return true;
        }

        private IEnumerator RecordingHandler()
        {
#if UNITY_WEBPLAYER
            yield return Application.RequestUserAuthorization( UserAuthorization.Microphone );
#endif
            m_Recording = Microphone.Start(m_MicrophoneID, true, RECORDING_BUFFER_SIZE, m_RecordingHZ);

            bool bFirstBlock = true;
            int midPoint = m_Recording.samples / 2;
            while (m_RecordingCallback != null)
            {
                int writePos = Microphone.GetPosition(m_MicrophoneID);
                if ((bFirstBlock && writePos >= midPoint)
                    || (!bFirstBlock && writePos < midPoint))
                {
                    // front block is recorded, make a RecordClip and pass it onto our callback.
                    float[] samples = new float[midPoint];
                    m_Recording.GetData(samples, bFirstBlock ? 0 : midPoint);

                    RecordClip record = new RecordClip();
                    record.MaxLevel = Mathf.Max(samples);
                    record.Clip = AudioClip.Create("Recording", midPoint, m_Recording.channels, m_RecordingHZ, false);
                    record.Clip.SetData(samples, 0);

                    if (m_RecordingCallback != null)
                        m_RecordingCallback(record);
                    else
                        StopRecording();        // automatically stop recording if the callback goes away.

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
        #endregion


    }
}
