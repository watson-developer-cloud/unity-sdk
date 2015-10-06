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
*/

using IBM.Watson.Logging;
using IBM.Watson.Connection;
using IBM.Watson.Utilities;
using MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IBM.Watson.Services.v1
{
    class SpeechToText
    {
        #region Constants
        /// <summary>
        /// How often to refresh a session to prevent it from expiring.
        /// </summary>
        const float SESSION_REFRESH_TIME = 20.0f;
        /// <summary>
        /// Size in seconds of the recording buffer. AudioClips() will be generated each time
        /// the recording hits the halfway point.
        /// </summary>
        const int RECORDING_BUFFER_SIZE = 3;
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
        public delegate void OnGetModels( Model [] models );
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
        private int m_RecordingHZ = 22050;                  // default recording HZ
        private string m_MicrophoneID = null;               // what microphone to use for recording.
        private bool m_DetectSilence = true;                // If true, then we will try not to record silence.
        private float m_SilenceThreshold = 0.1f;            // IF the audio level is below this value, then it's considered silent.
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
        public OnRecordClip RecordingCallback { get; set; }
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
            if ( m_RecordingID != 0 )
            {
                Log.Error( "SpeechToText", "Recording already active." );
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

        public bool GetModels( OnGetModels callback )
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool Recognize(AudioClip clip, OnRecognize callback)
        {

            m_RecognizeCallback = callback;
            return true;
        }
        private class RecognizeRequest : Connector.Request
        {
            AudioClip m_Clip = null;
            byte [] m_Wave = null;
            OnRecognize m_Callback = null;
        };

        private void OnRecognizeResponse(Connector.Request req, Connector.Response resp)
        {
        }
        #endregion

        #region Recording Functions
        public bool StartRecording(OnRecordClip callback)
        {
            if ( m_RecordingID != 0 )
            {
                Log.Error( "SpeechToText", "StartRecording() invoked, recording already active." );
                return false;
            }

            m_RecordingCallback = callback;
            m_RecordingID = Runnable.Run( RecordingHandler() );

            return true;
        }

        public bool StopRecording()
        {
            if ( m_RecordingID == 0 )
            {
                Log.Error( "SpeechToText", "StopRecording() called, no active recording." );
                return false;
            }

            Microphone.End( m_MicrophoneID );
            Runnable.Stop( m_RecordingID );
            m_RecordingID = 0;
            m_RecordingCallback = null;

            return true;
        }

        /// <summary>
        /// This co-routine handles recording sounds from the microphone then passing on the recorded
        /// blocks of audio to the recording callback.
        /// </summary>
        /// <returns></returns>
        private IEnumerator RecordingHandler()
        {
#if UNITY_WEBPLAYER
            yield return Application.RequestUserAuthorization( UserAuthorization.Microphone );
#endif
            AudioClip recording = Microphone.Start( m_MicrophoneID, true, RECORDING_BUFFER_SIZE, m_RecordingHZ );

            bool bFirstBlock = true;
            int midPoint = recording.samples / 2;
            while( m_RecordingCallback != null )
            {
                int writePos = Microphone.GetPosition( m_MicrophoneID );
                if ( (bFirstBlock && writePos >= midPoint)
                    || (!bFirstBlock && writePos < midPoint) )
                {
                    // front block is recorded, make a RecordClip and pass it onto our callback.
                    float [] samples = new float[ midPoint ];
                    recording.GetData( samples, bFirstBlock ? 0 : midPoint );

                    RecordClip record = new RecordClip();
                    record.MaxLevel = Mathf.Max( samples );
                    record.Clip = AudioClip.Create( "Recording", midPoint, recording.channels, m_RecordingHZ, false );
                    record.Clip.SetData( samples, 0 );

                    if ( m_RecordingCallback != null )
                        m_RecordingCallback( record );

                    bFirstBlock = !bFirstBlock;
                }
                else
                {
                    // calculate the number of samples remaining until we ready for a block of audio, 
                    // and wait that amount of time it will take to record.
                    int remaining = bFirstBlock ? (midPoint - writePos) : (recording.samples - writePos);
                    float timeRemaining = (float)remaining / (float)m_RecordingHZ;
                    yield return new WaitForSeconds( timeRemaining );
                }
            }
            
            yield break;
        }
        #endregion

        /// <summary>
        /// Parse a JSON response from the server into the ResultList object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private ResultList ParseRecognizeResponse(string json)
        {
            try
            {
                IDictionary resp = (IDictionary)Json.Deserialize(json);
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
                        alternative.Confidence = (float)ialternative["confidence"];

                        if (ialternative.Contains("timestamps"))
                        {
                            IList itimestamps = ialternative["timestamps"] as IList;

                            float[] timestamps = new float[itimestamps.Count];
                            for (int i = 0; i < itimestamps.Count; ++i)
                                timestamps[i] = (float)itimestamps[i];

                            alternative.Timestamps = timestamps;
                        }
                        if (ialternative.Contains("word_confidence"))
                        {
                            IList iconfidence = ialternative["word_confidence"] as IList;

                            float[] confidence = new float[iconfidence.Count];
                            for (int i = 0; i < iconfidence.Count; ++i)
                                confidence[i] = (float)iconfidence[i];

                            alternative.WordConfidence = confidence;
                        }
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

    }
}
