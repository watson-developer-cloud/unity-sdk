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

using UnityEngine;
using System.Collections.Generic;
using IBM.Watson.Connection;
using IBM.Watson.Utilities;
using IBM.Watson.Logging;
using System.Runtime.InteropServices;
using System.IO;
using System;
using System.Text;
using MiniJSON;

namespace IBM.Watson.Services.v1
{
    public class TextToSpeech 
    {
        #region Public Types
        public delegate void ToSpeechCallback(AudioClip clip);

        public enum AudioFormatType
        {
            OGG = 0,
            WAV,                    //Currently used
            FLAC
        };

        public enum VoiceType
        {
            en_US_Michael = 0,
            en_US_Lisa,
            en_US_Allison,
            en_GB_Kate,
            es_ES_Enrique,
            es_ES_Laura,
            es_US_Sofia,
            de_DE_Dieter,
            de_DE_Birgit,
            fr_FR_Renee,
            it_IT_Francesca,
            ja_JP_Emi,
        };
        #endregion

        #region Private Data
        private Connector m_Connector = null;
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
        #endregion

        #region Public Properties
        public AudioFormatType AudioFormat { get { return m_AudioFormat; } set { m_AudioFormat = value; } }
        public VoiceType Voice { get { return m_Voice; } set { m_Voice = value; } }
        #endregion

        #region ToSpeech Functions

        /// <summary>
        /// Private Request object that holds data specific to the ToSpeech request.
        /// </summary>
        private class ToSpeechRequest : Connector.Request
        {
            public string Text { get; set; }
            public ToSpeechCallback Callback { get; set; }
        }

        /// <summary>
        /// Converts the given text into an AudioClip that can be played.
        /// </summary>
        /// <param name="text">The text to synthesis into speech.</param>
        /// <param name="callback">The callback to invoke with the AudioClip.</param>
        /// <param name="post">If true, then we use post instead of get, this allows for text that exceeds the 5k limit.</param>
        /// <returns>Returns true if the request is sent.</returns>
        public bool ToSpeech(string text, ToSpeechCallback callback, bool post = false )
        {
            if ( !m_AudioFormats.ContainsKey(m_AudioFormat) )
            {
                Log.Error( "TextToSpeech", "Unsupported audio format: {0}", m_AudioFormat.ToString() );
                return false;
            }
            if ( !m_VoiceTypes.ContainsKey(m_Voice) )
            {
                Log.Error( "TextToSpeech", "Unsupported voice: {0}", m_Voice.ToString() );
                return false;
            }
            if (m_Connector == null)
            {
                Config.CredentialsInfo info = Config.Instance.FindCredentials(SERVICE_ID);
                if (info == null)
                {
                    Log.Error("TextToSpeech", "Unable to find credentials for service ID: {0}", SERVICE_ID);
                    return false;
                }

                m_Connector = Connector.Create(info);
                if (m_Connector == null)
                {
                    Log.Error("TextToSpeech", "Failed to create connection for URL: {0}", info.m_URL);
                    return false;
                }
            }

            ToSpeechRequest req = new ToSpeechRequest();
            req.Text = text;
            req.Callback = callback;

            req.Function = "/v1/synthesize";
            req.Parameters["accept"] = m_AudioFormats[m_AudioFormat];
            req.Parameters["voice"] = m_VoiceTypes[m_Voice];
            req.OnResponse = ToSpeechResponse;

            if (post)
            {
                Dictionary<string,string> upload = new Dictionary<string, string>();
                upload["text"] = text;

                req.Send = Encoding.UTF8.GetBytes( Json.Serialize( upload ) );               
            }
            else
            {
                req.Parameters["text"] = text;
            }

            return m_Connector.Send(req);
        }

        private void ToSpeechResponse(Connector.Request req, Connector.Response resp)
        {
            ToSpeechRequest speechReq = req as ToSpeechRequest;
            if (speechReq == null)
                throw new WatsonException("Wrong type of request object.");

            Log.Debug( "TextToSpeech", "Request completed in {0} seconds.", resp.ElapsedTime );

            AudioClip clip = null;
            if (resp.Success)
            {
                switch (m_AudioFormat)
                {
                    case AudioFormatType.WAV:
                        clip = ParseWAV(speechReq.Text, resp.Data);
                        break;
                    default:
                        Log.Error("TextToSpeech", "Unsupported audio format: {0}", m_AudioFormat.ToString());
                        break;
                }
            }
            else
            {
                Log.Error("TextToSpeech", "Request Failed: {0}", resp.Error);
            }

            if (speechReq.Callback != null)
                speechReq.Callback(clip);
        }
        #endregion

        #region WAV Parsing
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct IFF_FORM_CHUNK
        {
            public uint form_id;
            public uint form_length;
            public uint id;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct IFF_CHUNK
        {
            public uint id;
            public uint length;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct WAV_PCM
        {
            public ushort format_tag;
            public ushort channels;
            public uint sample_rate;
            public uint average_data_rate;
            public ushort alignment;
            public ushort bits_per_sample;
        };

        public static T BytesToType<T>(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }

        public static string GetID(uint id)
        {
            byte[] bytes = BitConverter.GetBytes(id);
            return new string(new char[] { (char)bytes[0], (char)bytes[1], (char)bytes[2], (char)bytes[3] });
        }

        AudioClip ParseWAV(string clipName, byte[] data)
        {
            MemoryStream stream = new MemoryStream(data, false);
            BinaryReader reader = new BinaryReader(stream);

            IFF_FORM_CHUNK form = BytesToType<IFF_FORM_CHUNK>(reader);
            if ( GetID(form.form_id) != "RIFF" || GetID(form.id) != "WAVE")
            {
                Log.Error("TextToSpeech", "Malformed WAV header: {0} != RIFF || {1} != WAVE", GetID(form.form_id), GetID(form.id) );
                return null;
            }

            WAV_PCM header = new WAV_PCM();
            bool bHeaderFound = false;

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                IFF_CHUNK chunk = BytesToType<IFF_CHUNK>(reader);

                int ChunkLength = (int)chunk.length;
                if (ChunkLength < 0)  // HACK: Deal with TextToSpeech bug where the chunk length is not set for the data chunk..
                    ChunkLength = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
                if ((ChunkLength & 0x1) != 0)
                    ChunkLength += 1;

                long ChunkEnd = reader.BaseStream.Position + ChunkLength;
                if (GetID(chunk.id) == "fmt ")
                {
                    bHeaderFound = true;
                    header = BytesToType<WAV_PCM>(reader);
                }
                else if (GetID(chunk.id) == "data")
                {
                    if (! bHeaderFound )
                    {
                        Log.Error( "TextToSpeech", "Failed to find header." );
                        return null;
                    }
                    byte[] waveform = reader.ReadBytes(ChunkLength);

                    // convert into a float based wave form..
                    int channels = (int)header.channels;
                    int bps = (int)header.bits_per_sample;
                    float divisor = 1 << (bps - 1);
                    int bytesps = bps / 8;
                    int samples = waveform.Length / bytesps;

                    Log.Debug( "TextToSpeech", "WAV INFO, channels = {0}, bps = {1}, samples = {2}, rate = {3}", 
                        channels, bps, samples, header.sample_rate );

                    float[] wf = new float[samples];
                    if (bps == 16)
                    {
                        for (int s = 0; s < samples; ++s)
                            wf[s] = ((float)BitConverter.ToInt16(waveform, s * bytesps)) / divisor;
                    }
                    else if (bps == 32)
                    {
                        for (int s = 0; s < samples; ++s)
                            wf[s] = ((float)BitConverter.ToInt32(waveform, s * bytesps)) / divisor;
                    }
                    else if (bps == 8)
                    {
                        for (int s = 0; s < samples; ++s)
                            wf[s] = ((float)BitConverter.ToChar(waveform, s * bytesps)) / divisor;
                    }
                    else
                    {
                        Log.Error("TextToSpeech", "Unspported BPS {0} in WAV data.", bps.ToString());
                        return null;
                    }

                    AudioClip clip = AudioClip.Create(clipName, samples, channels, (int)header.sample_rate, false);
                    clip.SetData(wf, 0);

                    return clip;
                }

                reader.BaseStream.Position = ChunkEnd;
            }

            return null;
        }

        #endregion

    }

}
