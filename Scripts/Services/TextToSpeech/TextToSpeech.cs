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

using UnityEngine;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using System.Text;
using MiniJSON;
using System;
using FullSerializer;

namespace IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1
{
	/// <summary>
	/// This class wraps the Text to Speech service.
	/// <a href="http://www.ibm.com/watson/developercloud/text-to-speech.html">Text to Speech Service</a>
	/// </summary>
	public class TextToSpeech : IWatsonService
    {
        #region Public Types

        /// <summary>
        /// This callback is passed into the ToSpeech() method.
        /// </summary>
        /// <param name="clip">The AudioClip containing the audio to play.</param>
        public delegate void ToSpeechCallback(AudioClip clip);
        /// <summary>
        /// This callback is used by the GetVoices() function.
        /// </summary>
        /// <param name="voices">The Voices object.</param>
        public delegate void GetVoicesCallback(Voices voices);

        #endregion

        #region Private Data
        private DataCache m_SpeechCache = null;
        private VoiceType m_Voice = VoiceType.en_US_Michael;
        private AudioFormatType m_AudioFormat = AudioFormatType.WAV;
        private Dictionary<VoiceType, string> m_VoiceTypes = new Dictionary<VoiceType, string>()
        {
            { VoiceType.en_US_Michael, "en-US_MichaelVoice" },
            { VoiceType.en_US_Lisa, "en-US_LisaVoice" },
            { VoiceType.en_US_Allison, "en-US_AllisonVoice" },
            { VoiceType.en_GB_Kate, "en-GB_KateVoice" },
            { VoiceType.es_ES_Enrique, "es-ES_EnriqueVoice" },
            { VoiceType.es_ES_Laura, "es-ES_LauraVoice" },
            { VoiceType.es_US_Sofia, "es-US_SofiaVoice" },
            { VoiceType.de_DE_Dieter, "de-DE_DieterVoice" },
            { VoiceType.de_DE_Birgit, "de-DE_BirgitVoice" },
            { VoiceType.fr_FR_Renee, "fr-FR_ReneeVoice" },
            { VoiceType.it_IT_Francesca, "it-IT_FrancescaVoice" },
            { VoiceType.ja_JP_Emi, "ja-JP_EmiVoice" },
        };
        private Dictionary<AudioFormatType, string> m_AudioFormats = new Dictionary<AudioFormatType, string>()
        {
            { AudioFormatType.OGG, "audio/ogg;codecs=opus" },
            { AudioFormatType.WAV, "audio/wav" },
            { AudioFormatType.FLAC, "audio/flac" },
        };
        private const string SERVICE_ID = "TextToSpeechV1";
        private static fsSerializer sm_Serializer = new fsSerializer();
        #endregion

        #region Public Properties
        /// <summary>
        /// Disable the local cache.
        /// </summary>
        public bool DisableCache { get; set; }
        /// <summary>
        /// This property allows the user to set the AudioFormat to use. Currently, only WAV is supported.
        /// </summary>
        public AudioFormatType AudioFormat { get { return m_AudioFormat; } set { m_AudioFormat = value; } }
        /// <summary>
        /// This property allows the user to specify the voice to use.
        /// </summary>
        public VoiceType Voice
        {
            get { return m_Voice; }
            set
            {
                if (m_Voice != value)
                {
                    m_Voice = value;
                    m_SpeechCache = null;
                }
            }
        }
        #endregion

        #region GetVoices 
        /// <summary>
        /// Returns all available voices that can be used.
        /// </summary>
        /// <param name="callback">The callback to invoke with the list of available voices.</param>
        /// <returns>Returns ture if the request was submitted.</returns>
        public bool GetVoices(GetVoicesCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/voices");
            if (connector == null)
                return false;

            GetVoicesReq req = new GetVoicesReq();
            req.Callback = callback;
            req.OnResponse = OnGetVoicesResp;

            return connector.Send(req);
        }
        private class GetVoicesReq : RESTConnector.Request
        {
            public GetVoicesCallback Callback { get; set; }
        };
        private void OnGetVoicesResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Voices voices = new Voices();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = voices;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Natural Language Classifier", "GetVoices Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetVoicesReq)req).Callback != null)
                ((GetVoicesReq)req).Callback(resp.Success ? voices : null);
        }
        #endregion

        #region ToSpeech Functions

        /// <summary>
        /// Private Request object that holds data specific to the ToSpeech request.
        /// </summary>
        private class ToSpeechRequest : RESTConnector.Request
        {
            public string TextId { get; set; }
            public string Text { get; set; }
            public ToSpeechCallback Callback { get; set; }
        }

        /// <summary>
        /// Converts the given text into an AudioClip that can be played.
        /// </summary>
        /// <param name="text">The text to synthesis into speech.</param>
        /// <param name="callback">The callback to invoke with the AudioClip.</param>
        /// <param name="usePost">If true, then we use post instead of get, this allows for text that exceeds the 5k limit.</param>
        /// <returns>Returns true if the request is sent.</returns>
        public bool ToSpeech(string text, ToSpeechCallback callback, bool usePost = false)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (!m_AudioFormats.ContainsKey(m_AudioFormat))
            {
                Log.Error("TextToSpeech", "Unsupported audio format: {0}", m_AudioFormat.ToString());
                return false;
            }
            if (!m_VoiceTypes.ContainsKey(m_Voice))
            {
                Log.Error("TextToSpeech", "Unsupported voice: {0}", m_Voice.ToString());
                return false;
            }

            string textId = Utility.GetMD5(text);
            if (!DisableCache)
            {
                if (m_SpeechCache == null)
                    m_SpeechCache = new DataCache("TextToSpeech_" + m_VoiceTypes[m_Voice]);

                byte[] data = m_SpeechCache.Find(textId);
                if (data != null)
                {
                    AudioClip clip = ProcessResponse(textId, data);
                    callback(clip);
                    return true;
                }
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/synthesize");
            if (connector == null)
            {
                Log.Error("TextToSpeech", "Failed to get connector.");
                return false;
            }

            ToSpeechRequest req = new ToSpeechRequest();
            req.TextId = textId;
            req.Text = text;
            req.Callback = callback;
            req.Parameters["accept"] = m_AudioFormats[m_AudioFormat];
            req.Parameters["voice"] = m_VoiceTypes[m_Voice];
            req.OnResponse = ToSpeechResponse;

            if (usePost)
            {
                Dictionary<string, string> upload = new Dictionary<string, string>();
                upload["text"] = "\"" + text + "\"";

                req.Send = Encoding.UTF8.GetBytes(Json.Serialize(upload));
                req.Headers["Content-Type"] = "application/json";
            }
            else
            {
                req.Parameters["text"] = "\"" + text + "\"";
            }

            return connector.Send(req);
        }

        private void ToSpeechResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ToSpeechRequest speechReq = req as ToSpeechRequest;
            if (speechReq == null)
                throw new WatsonException("Wrong type of request object.");

            //Log.Debug( "TextToSpeech", "Request completed in {0} seconds.", resp.ElapsedTime );

            AudioClip clip = resp.Success ? ProcessResponse(speechReq.TextId, resp.Data) : null;
            if (clip == null)
                Log.Error("TextToSpeech", "Request Failed: {0}", resp.Error);
            if (m_SpeechCache != null && clip != null)
                m_SpeechCache.Save(speechReq.TextId, resp.Data);

            if (speechReq.Callback != null)
                speechReq.Callback(clip);
        }

        private AudioClip ProcessResponse(string textId, byte[] data)
        {
            switch (m_AudioFormat)
            {
                case AudioFormatType.WAV:
                    return WaveFile.ParseWAV(textId, data);
                default:
                    break;
            }

            Log.Error("TextToSpeech", "Unsupported audio format: {0}", m_AudioFormat.ToString());
            return null;
        }
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
            private TextToSpeech m_Service = null;
            private ServiceStatus m_Callback = null;

            public CheckServiceStatus(TextToSpeech service, ServiceStatus callback)
            {
                m_Service = service;
                m_Callback = callback;

                if (!m_Service.GetVoices(OnCheckService))
                    m_Callback(SERVICE_ID, false);
            }

            private void OnCheckService(Voices voices)
            {
                if (m_Callback != null && m_Callback.Target != null)
                    m_Callback(SERVICE_ID, voices != null);
            }
        };
        #endregion
    }

}
