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

namespace IBM.Watson.Services.v1            // Add DeveloperCloud
{
    class SpeechToText
    {
        #region Constants
        const string SERVICE_ID = "SpeechToTextV1";
        /// <summary>
        /// How often to refresh a session to prevent it from expiring.
        /// </summary>
        const float SESSION_REFRESH_TIME = 20.0f;
        /// <summary>
        /// Size in seconds of the recording buffer. AudioClips() will be generated each time
        /// the recording hits the halfway point.
        /// </summary>
        const int RECORDING_BUFFER_SIZE = 2;
        #endregion

        #region Public Types
        public class Model
        {
            public string Name { get; set; }
            public float Rate { get; set; }
            public string Language { get; set; }
            public string Description { get; set; }
            public string URL { get; set; }
        };
        public class Alternative
        {
            public string Transcript { get; set; }
            public float Confidence { get; set; }
            public float[] Timestamps { get; set; }
            public float[] WordConfidence { get; set; }
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
        #endregion

        #region Private Data
        private Connector m_Connector = null;
        private OnRecognize m_RecognizeCallback = null;
        private string m_RecognizeModel = "en-US_BroadbandModel";    // ID of the model to use.
        private int m_MaxAlternatives = 1;                  // maximum number of alternatives to return.
        private bool m_Timestamps = false;
        private bool m_WordConfidence = false;
        private bool m_UseSession = false;
        private string m_SessionID = null;
        private float m_LastSessionRefresh = 0.0f;          // last time when we refreshed our session ID
        private OnRecordClip m_RecordingCallback = null;
        private int m_RecordingID = 0;                      // ID of our co-routine when recording, 0 if not recording currently.
        private AudioClip m_Recording = null;              
        private int m_RecordingHZ = 22050;                  // default recording HZ
        private string m_MicrophoneID = null;               // what microphone to use for recording.
        private bool m_DetectSilence = true;                // If true, then we will try not to record silence.
        private float m_SilenceThreshold = 0.03f;           // If the audio level is below this value, then it's considered silent.
        #endregion

        #region Public Properties
        /// <summary>
        /// What is our currently active reconize model.
        /// </summary>
        public string RecognizeModel { get { return m_RecognizeModel; } set { m_RecognizeModel = value; } }
        /// <summary>
        /// Returns the maximum number of alternatives returned by ToText().
        /// </summary>
        public int MaxAlternatives { get { return m_MaxAlternatives; } set { m_MaxAlternatives = value; } }
        /// <summary>
        /// True to return timestamps of words with results.
        /// </summary>
        public bool Timestamps { get { return m_Timestamps; } set { m_Timestamps = value; } }
        /// <summary>
        /// True to return word confidence with results.
        /// </summary>
        public bool WordConfidence { get { return m_WordConfidence; } set { m_WordConfidence = value; } }
        /// <summary>
        /// If true, Create a session to lock an engine to the session.
        /// </summary>
        public bool UseSession { get { return m_UseSession; } set { m_UseSession = value; } }
        /// <summary>
        /// The ID of our session.
        /// </summary>
        public string SessionID { get { return m_SessionID; } set { m_SessionID = value; } }
        /// <summary>
        /// Returns a list of available microphone devices.
        /// </summary>
        public string[] Microphones { get { return Microphone.devices; } }
        /// <summary>
        /// Microphone recording HZ.
        /// </summary>
        public int RecordingHZ { get { return m_RecordingHZ; } set { m_RecordingHZ = value; } }
        /// <summary>
        /// Returns the name of the selected microphone.
        /// </summary>
        public string MicrophoneID { get { return m_MicrophoneID; } set { m_MicrophoneID = value; } }
        /// <summary>
        /// If true, then we will try not to send silent audio clips to the server.
        /// </summary>
        public bool DetectSilence { get { return m_DetectSilence; } set { m_DetectSilence = value; } }
        /// <summary>
        /// A value from 1.0 to 0.0 that determines what is considered silence. If the audio level is below this value
        /// then we consider it silence.
        /// </summary>
        public float SilenceThreshold { get { return m_SilenceThreshold; } set { m_SilenceThreshold = value; } }
        #endregion

        #region Listening Functions
        public bool StartListening(OnRecognize callback)
        {
            if (m_RecordingID != 0)
            {
                Log.Error("SpeechToText", "Recording already active.");
                return false;
            }

            m_RecognizeCallback = callback;
            return true;
        }

        public bool StopListening()
        {
            return true;
        }
        #endregion


        #region Recognize Functions

        public bool GetModels(OnGetModels callback)
        {
            return true;
        }

        /// <summary>
        /// This function POSTs the given audio clip the reconize function and convert speech into text. This function should be used
        /// only on AudioClips under 4MB once they have been converted into WAV format.
        /// </summary>
        /// <param name="clip">The AudioClip object.</param>
        /// <param name="callback">A callback to invoke with the results.</param>
        /// <returns></returns>
        public bool Recognize(AudioClip clip, OnRecognize callback)
        {
            if (clip == null )
                throw new ArgumentNullException("clip");
            if (callback == null )
                throw new ArgumentNullException("callback");

            if (m_Connector == null)
            {
                Config.CredentialsInfo info = Config.Instance.FindCredentials(SERVICE_ID);
                if (info == null)
                {
                    Log.Error("SpeechToText", "Unable to find credentials for Service ID: {0}", SERVICE_ID);
                    return false;
                }

                m_Connector = Connector.Create(info);
                if (m_Connector == null)
                {
                    Log.Error("SpeechToText", "Failed to create connection for URL: {0}", info.m_URL);
                    return false;
                }
            }

            RecognizeRequest req = new RecognizeRequest();
            req.Clip = clip;
            req.Callback = callback;

            req.Function = "/v1/recognize";
            req.ContentType = "audio/wav";
            req.Send = WaveFile.CreateWAV( clip );     
            if ( req.Send.Length > (4 * (1024 * 1024)) )
            {
                Log.Error( "SpeechToText", "AudioClip is too large for Recognize()." );
                return false;
            }
            req.Parameters["model"] = m_RecognizeModel;
            req.Parameters["continuous"] = "false";     // TODO: Support this query parameter
            req.Parameters["max_alternatives"] = m_MaxAlternatives.ToString();
            req.Parameters["timestamps"] = m_Timestamps ? "true" : "false";
            req.Parameters["word_confidence"] = m_WordConfidence ? "true" : "false";
            req.OnResponse = OnRecognizeResponse;

            return m_Connector.Send( req );
        }

        private class RecognizeRequest : Connector.Request
        {
            public AudioClip Clip { get; set; }
            public OnRecognize Callback { get; set; }
        };

        private void OnRecognizeResponse(Connector.Request req, Connector.Response resp)
        {
            RecognizeRequest recognizeReq = req as RecognizeRequest;
            if ( recognizeReq == null )
                throw new WatsonException( "Unexpected request type." );

            ResultList result = null;
            if ( resp.Success )
            {
                result = ParseRecognizeResponse( resp.Data );
                if ( result == null )
                {
                    Log.Error( "SpeechToText", "Failed to parse json response: {0}", 
                        resp.Data != null ? Encoding.UTF8.GetString( resp.Data ) : "" );
                }
                else
                {
                    Log.Status( "SpeechToText", "Received Recognize Response, Elapsed Time: {0}, Results: {1}",
                        resp.ElapsedTime, result.Results.Length );
                }
            }
            else
            {
                Log.Error( "SpeechToText", "Recognize Error: {0}", resp.Error );
            }

            if ( recognizeReq.Callback != null )
                recognizeReq.Callback( result );
        }

        private ResultList ParseRecognizeResponse( byte[] json)
        {
            if ( json == null )
                return null;

            try
            {
                string jsonString = Encoding.UTF8.GetString( json );
                if ( jsonString == null )
                    return null;
                IDictionary resp = (IDictionary)Json.Deserialize( jsonString );
                if (resp == null)
                    return null;

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
                        alternative.Confidence = (float)(double)ialternative["confidence"];

                        if (ialternative.Contains("timestamps"))
                        {
                            IList itimestamps = ialternative["timestamps"] as IList;

                            float[] timestamps = new float[itimestamps.Count];
                            for (int i = 0; i < itimestamps.Count; ++i)
                                timestamps[i] = (float)(double)itimestamps[i];

                            alternative.Timestamps = timestamps;
                        }
                        if (ialternative.Contains("word_confidence"))
                        {
                            IList iconfidence = ialternative["word_confidence"] as IList;

                            float[] confidence = new float[iconfidence.Count];
                            for (int i = 0; i < iconfidence.Count; ++i)
                                confidence[i] = (float)(double)iconfidence[i];

                            alternative.WordConfidence = confidence;
                        }

                        alternatives.Add( alternative );
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
        /// <summary>
        /// This function begins recording from the microphone and passes all recorded audio to the provided callback function. 
        /// </summary>
        /// <param name="callback"></param>
        /// <returns>Returns true on success.</returns>
        public bool StartRecording(OnRecordClip callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (m_RecordingID != 0)
            {
                Log.Error("SpeechToText", "StartRecording() invoked, recording already active.");
                return false;
            }

            m_RecordingCallback = callback;
            m_RecordingID = Runnable.Run(RecordingHandler());

            return true;
        }

        /// <summary>
        /// Stop recording from the configured microphone.
        /// </summary>
        /// <returns>Returns true on success.</returns>
        public bool StopRecording()
        {
            if (m_RecordingID == 0)
            {
                Log.Error("SpeechToText", "StopRecording() called, no active recording.");
                return false;
            }
            
            // TODO: We may need to figure out a method to get the last bit of audio out of our buffer.
            Microphone.End(m_MicrophoneID);
            Runnable.Stop(m_RecordingID);
            m_RecordingID = 0;
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
