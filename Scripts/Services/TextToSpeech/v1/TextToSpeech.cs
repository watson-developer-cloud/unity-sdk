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
using System.Text.RegularExpressions;

namespace IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1
{
    /// <summary>
    /// This class wraps the Text to Speech service.
    /// <a href="http://www.ibm.com/watson/developercloud/text-to-speech.html">Text to Speech Service</a>
    /// </summary>
    public class TextToSpeech : IWatsonService
    {
        #region Private Data
        private VoiceType _voice = VoiceType.en_US_Michael;
        private AudioFormatType _audioFormat = AudioFormatType.WAV;
        private Dictionary<VoiceType, string> _voiceTypes = new Dictionary<VoiceType, string>()
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
      { VoiceType.pt_BR_Isabela, "pt-BR_IsabelaVoice"},
        };
        private Dictionary<AudioFormatType, string> _audioFormats = new Dictionary<AudioFormatType, string>()
        {
            { AudioFormatType.OGG, "audio/ogg;codecs=opus" },
            { AudioFormatType.WAV, "audio/wav" },
            { AudioFormatType.FLAC, "audio/flac" },
        };
        private const string ServiceId = "TextToSpeechV1";
        private fsSerializer _serializer = new fsSerializer();
        private const float RequestTimeout = 10.0f * 60.0f;
        private Credentials _credentials = null;
        private string _url = "https://stream.watsonplatform.net/text-to-speech/api";
        #endregion

        #region Public Properties
        /// <summary>
        /// This property allows the user to set the AudioFormat to use. Currently, only WAV is supported.
        /// </summary>
        public AudioFormatType AudioFormat { get { return _audioFormat; } set { _audioFormat = value; } }
        /// <summary>
        /// This property allows the user to specify the voice to use.
        /// </summary>
        public VoiceType Voice
        {
            get { return _voice; }
            set
            {
                if (_voice != value)
                {
                    _voice = value;
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
        #endregion

        #region GetVoiceType
        private string GetVoiceType(VoiceType voiceType)
        {
            if (_voiceTypes.ContainsKey(voiceType))
            {
                string voiceName = "";
                _voiceTypes.TryGetValue(voiceType, out voiceName);
                return voiceName;
            }
            else
            {
                Log.Warning("TextToSpeech.GetVoiceType()", "There is no voicetype for {0}!", voiceType);
                return null;
            }
        }
        #endregion

        #region Constructor
        public TextToSpeech(Credentials credentials)
        {
            if (credentials.HasCredentials() || credentials.HasAuthorizationToken())
            {
                Credentials = credentials;
            }
            else
            {
                throw new WatsonException("Please provide a username and password or authorization token to use the Text to Speech service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
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

        #region GetVoices 
        /// <summary>
        /// Returns all available voices that can be used.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <returns>Returns ture if the request was submitted.</returns>
        public bool GetVoices(SuccessCallback<Voices> successCallback, FailCallback failCallback, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/voices");
            if (connector == null)
                return false;

            GetVoicesReq req = new GetVoicesReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.OnResponse = OnGetVoicesResp;

            return connector.Send(req);
        }

        private class GetVoicesReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Voices> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        };

        private void OnGetVoicesResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Voices result = new Voices();
            fsData data = null;
            Dictionary<string, object> customData = ((GetVoicesReq)req).CustomData;

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
                    Log.Error("TextToSpeech.OnGetVoicesResp()", "GetVoices Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetVoicesReq)req).SuccessCallback != null)
                    ((GetVoicesReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetVoicesReq)req).FailCallback != null)
                    ((GetVoicesReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region GetVoice 
        /// <summary>
        /// Return specific voice.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="voice">The name of the voice you would like to get. If this is null, TextToSpeech will default to the set voice.</param>
        /// <returns>Returns ture if the request was submitted.</returns>
        public bool GetVoice(SuccessCallback<Voice> successCallback, FailCallback failCallback, VoiceType? voice = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (voice == null)
                voice = _voice;

            string service = "/v1/voices/{0}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, GetVoiceType((VoiceType)voice)));
            if (connector == null)
                return false;

            GetVoiceReq req = new GetVoiceReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.OnResponse = OnGetVoiceResp;

            return connector.Send(req);
        }
        private class GetVoiceReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Voice> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetVoiceResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Voice result = new Voice();
            fsData data = null;
            Dictionary<string, object> customData = ((GetVoiceReq)req).CustomData;

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
                    Log.Error("TextToSpeech.OnGetVoiceResp()", "GetVoice Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetVoiceReq)req).SuccessCallback != null)
                    ((GetVoiceReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetVoiceReq)req).FailCallback != null)
                    ((GetVoiceReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Synthesize Functions
        /// <summary>
        /// Private Request object that holds data specific to the ToSpeech request.
        /// </summary>
        private class ToSpeechRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<AudioClip> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
            /// <summary>
            /// Text identifier.
            /// </summary>
            public string TextId { get; set; }
            /// <summary>
            /// The text.
            /// </summary>
            public string Text { get; set; }
        }

        /// <summary>
        /// Converts the given text into an AudioClip that can be played.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="text">The text to synthesis into speech.</param>
        /// <param name="usePost">If true, then we use post instead of get, this allows for text that exceeds the 5k limit.</param>
        /// <returns>Returns true if the request is sent.</returns>
        public bool ToSpeech(SuccessCallback<AudioClip> successCallback, FailCallback failCallback, string text, bool usePost = false, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            if (!_audioFormats.ContainsKey(_audioFormat))
            {
                Log.Error("TextToSpeech.ToSpeech()", "Unsupported audio format: {0}", _audioFormat.ToString());
                return false;
            }
            if (!_voiceTypes.ContainsKey(_voice))
            {
                Log.Error("TextToSpeech.ToSpeech()", "Unsupported voice: {0}", _voice.ToString());
                return false;
            }

            //for responses from Watson Conversaton
            string escapedText = text.Replace("\\\"", "\"");
            string decodedText = DecodeUnicodeCharacters(escapedText);

            string textId = Utility.GetMD5(decodedText);

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/synthesize");
            if (connector == null)
            {
                Log.Error("TextToSpeech.ToSpeech()", "Failed to get connector.");
                return false;
            }

            ToSpeechRequest req = new ToSpeechRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.TextId = textId;
            req.Text = decodedText;
            req.Parameters["accept"] = _audioFormats[_audioFormat];
            req.Parameters["voice"] = _voiceTypes[_voice];
            req.OnResponse = ToSpeechResponse;

            if (usePost)
            {
                Dictionary<string, string> upload = new Dictionary<string, string>();
                upload["text"] = decodedText;

                req.Send = Encoding.UTF8.GetBytes(Json.Serialize(upload));
                req.Headers["Content-Type"] = "application/json";
            }
            else
            {
                req.Parameters["text"] = decodedText;
            }

            return connector.Send(req);
        }

        private void ToSpeechResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ToSpeechRequest speechReq = req as ToSpeechRequest;
            if (speechReq == null)
                throw new WatsonException("Wrong type of request object.");

            Dictionary<string, object> customData = ((ToSpeechRequest)req).CustomData;

            if (resp.Success)
            {
                AudioClip clip = ProcessResponse(speechReq.TextId, resp.Data);
                if (clip == null)
                    Log.Error("TextToSpeech.ToSpeechResponse()", "Request Failed: {0}", resp.Error);

                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);
                customData.Add("data", resp.Data);

                if (((ToSpeechRequest)req).SuccessCallback != null)
                    ((ToSpeechRequest)req).SuccessCallback(clip, customData);
            }
            else
            {
                if (((ToSpeechRequest)req).FailCallback != null)
                    ((ToSpeechRequest)req).FailCallback(resp.Error, customData);
            }
        }

        private AudioClip ProcessResponse(string textId, byte[] data)
        {
            switch (_audioFormat)
            {
                case AudioFormatType.WAV:
                    return WaveFile.ParseWAV(textId, data);
                default:
                    break;
            }

            Log.Error("TextToSpeech.ProcessResponse()", "Unsupported audio format: {0}", _audioFormat.ToString());
            return null;
        }

        private string DecodeUnicodeCharacters(string text)
        {
            string decodedString = text;

            MatchCollection matches = Regex.Matches(text, @"\\u.{4}");

            foreach(Match match in matches) {
                string pureCode = match.ToString().Replace("\\u", "");
                int codeNumber = int.Parse(pureCode, System.Globalization.NumberStyles.HexNumber);
                string unicodeString = char.ConvertFromUtf32(codeNumber);

                decodedString = decodedString.Replace("\\u" + pureCode, unicodeString);
            }

            return decodedString;
        }
        #endregion

        #region GetPronunciation
        /// <summary>
        /// Returns the phonetic pronunciation for the word specified by the text parameter. You can request 
        /// the pronunciation for a specific format. You can also request the pronunciation for a specific
        /// voice to see the default translation for the language of that voice or for a specific custom voice
        /// model to see the translation for that voice model.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="text">The text string to pronounce.</param>
        /// <param name="voice">Specify a voice to obtain the pronunciation for the specified word in the language of that voice. All voices for the same language (for example, en-US) return the same translation. Do not specify both a voice and a customization_id. Retrieve available voices with the GET /v1/voices method. If this is null, TextToSpeech will default to the set voice.</param>
        /// <param name="format">Specify the phoneme set in which to return the pronunciation. Omit the parameter to obtain the pronunciation in the default format. Either ipa or spr.</param>
        /// <param name="customization_id">GUID of a custom voice model for which the pronunciation is to be returned. You must make the request with the service credentials of the model's owner. If the word is not defined in the specified voice model, the service returns the default translation for the model's language. Omit the parameter to see the translation for the specified voice with no customization. Do not specify both a voice and a customization_id.</param>
        /// <returns></returns>
        public bool GetPronunciation(SuccessCallback<Pronunciation> successCallback, FailCallback failCallback, string text, VoiceType? voice = null, string format = "ipa", string customization_id = default(string), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (voice == null)
                voice = _voice;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/pronunciation");
            if (connector == null)
                return false;

            GetPronunciationReq req = new GetPronunciationReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.Parameters["text"] = text;
            req.Parameters["voice"] = GetVoiceType((VoiceType)voice);
            req.Parameters["format"] = format;
            if (!string.IsNullOrEmpty(customization_id))
                req.Parameters["customization_id"] = customization_id;
            req.OnResponse = OnGetPronunciationResp;

            return connector.Send(req);
        }

        private class GetPronunciationReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Pronunciation> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetPronunciationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Pronunciation result = new Pronunciation();
            fsData data = null;
            Dictionary<string, object> customData = ((GetPronunciationReq)req).CustomData;

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
                    Log.Error("TextToSpeech.OnGetPronunciationResp()", "GetPronunciation Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetPronunciationReq)req).SuccessCallback != null)
                    ((GetPronunciationReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetPronunciationReq)req).FailCallback != null)
                    ((GetPronunciationReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Customizations
        /// <summary>
        /// Lists metadata such as the name and description for the custom voice models that you own. Use the language query parameter to list the voice models that you own for the specified language only. Omit the parameter to see all voice models that you own for all languages. To see the words in addition to the metadata for a specific voice model, use the GET /v1/customizations/{customization_id} method. Only the owner of a custom voice model can use this method to list information about the model.
        /// Note: This method is currently a beta release that supports US English only
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomizations(SuccessCallback<Customizations> successCallback, FailCallback failCallback, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetCustomizationsReq req = new GetCustomizationsReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
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
                    Log.Error("TextToSpeech.OnGetCustomizationsResp()", "GetCustomizations Exception: {0}", e.ToString());
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

        #region Create Customization
        /// <summary>
        /// Creates a new empty custom voice model that is owned by the requesting user.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="name">Name of the new custom voice model.</param>
        /// <param name="language">Language of the new custom voice model. Omit the parameter to use the default language, en-US. = ['de-DE', 'en-US', 'en-GB', 'es-ES', 'es-US', 'fr-FR', 'it-IT', 'ja-JP', 'pt-BR'].</param>
        /// <param name="description">Description of the new custom voice model.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool CreateCustomization(SuccessCallback<CustomizationID> successCallback, FailCallback failCallback, string name, string language = default(string), string description = default(string), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("A name is required to create a custom voice model.");

            CustomVoice customVoice = new CustomVoice();
            customVoice.name = name;
            customVoice.language = language;
            customVoice.description = description;

            fsData data;
            _serializer.TrySerialize(customVoice.GetType(), customVoice, out data).AssertSuccessWithoutWarnings();
            string customizationJson = fsJsonPrinter.CompressedJson(data);

            CreateCustomizationRequest req = new CreateCustomizationRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
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
                    Log.Error("TextToSpeech.OnCreateCustomizationResp()", "CreateCustomization Exception: {0}", e.ToString());
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

        #region Delete Customization
        /// <summary>
        /// Deletes the custom voice model with the specified `customization_id`. Only the owner of a custom voice model can use this method to delete the model.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The voice model to be deleted's identifier.</param>
        /// <param name="customData">Optional custom data.</param>
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
            req.Timeout = RequestTimeout;
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

        #region Get Customization
        /// <summary>
        /// Lists all information about the custom voice model with the specified `customization_id`. In addition to metadata such as the name and description of the voice model, the output includes the words in the model and their translations as defined in the model. To see just the metadata for a voice model, use the GET `/v1/customizations` method. Only the owner of a custom voice model can use this method to query information about the model.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The requested custom voice model's identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomization(SuccessCallback<Customization> successCallback, FailCallback failCallback, string customizationID, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to get a custom voice model.");

            GetCustomizationRequest req = new GetCustomizationRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
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
                    Log.Error("TextToSpeech.OnGetCustomizationResp()", "GetCustomization Exception: {0}", e.ToString());
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

        #region Update Customization
        /// <summary>
        /// Updates information for the custom voice model with the specified `customization_id`. You can update the metadata such as the name and description of the voice model. You can also update the words in the model and their translations. A custom model can contain no more than 20,000 entries. Only the owner of a custom voice model can use this method to update the model.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The identifier of the custom voice model to be updated.</param>
        /// <param name="customVoiceUpdate">Custom voice model update data.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool UpdateCustomization(SuccessCallback<bool> successCallback, FailCallback failCallback, string customizationID, CustomVoiceUpdate customVoiceUpdate, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("customizationID");
            if (!customVoiceUpdate.HasData())
                throw new ArgumentNullException("Custom voice update data is required to update a custom voice model.");

            fsData data;
            _serializer.TrySerialize(customVoiceUpdate.GetType(), customVoiceUpdate, out data).AssertSuccessWithoutWarnings();
            string customizationJson = fsJsonPrinter.CompressedJson(data);

            UpdateCustomizationRequest req = new UpdateCustomizationRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Send = Encoding.UTF8.GetBytes(customizationJson);
            req.OnResponse = OnUpdateCustomizationResp;

            string service = "/v1/customizations/{0}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        //	TODO add UpdateCustomization overload using path to json file.

        private class UpdateCustomizationRequest : RESTConnector.Request
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

        private void OnUpdateCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((UpdateCustomizationRequest)req).CustomData;

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((UpdateCustomizationRequest)req).SuccessCallback != null)
                    ((UpdateCustomizationRequest)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((UpdateCustomizationRequest)req).FailCallback != null)
                    ((UpdateCustomizationRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Customization Words
        /// <summary>
        /// Lists all of the words and their translations for the custom voice model with the specified `customization_id`. The output shows the translations as they are defined in the model. Only the owner of a custom voice model can use this method to query information about the model's words.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The requested custom voice model's identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomizationWords(SuccessCallback<Words> successCallback, FailCallback failCallback, string customizationID, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID is required to get a custom voice model's words.");

            GetCustomizationWordsRequest req = new GetCustomizationWordsRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.OnResponse = OnGetCustomizationWordsResp;

            string service = "/v1/customizations/{0}/words";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomizationWordsRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Words> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetCustomizationWordsResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Words result = new Words();
            fsData data = null;
            Dictionary<string, object> customData = ((GetCustomizationWordsRequest)req).CustomData;

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
                    Log.Error("TextToSpeech.OnGetCustomizationWordsResp()", "GetCustomizationWords Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetCustomizationWordsRequest)req).SuccessCallback != null)
                    ((GetCustomizationWordsRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetCustomizationWordsRequest)req).FailCallback != null)
                    ((GetCustomizationWordsRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Add Customization Words
        /// <summary>
        /// Adds one or more words and their translations to the custom voice model with the specified `customization_id`. A custom model can contain no more than 20,000 entries. Only the owner of a custom voice model can use this method to add words to the model.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The identifier of the custom voice model to be updated.</param>
        /// <param name="words">Words object to add to custom voice model.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool AddCustomizationWords(SuccessCallback<bool> successCallback, FailCallback failCallback, string customizationID, Words words, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("customizationID");
            if (!words.HasData())
                throw new ArgumentNullException("Words data is required to add words to a custom voice model.");

            fsData data;
            _serializer.TrySerialize(words.GetType(), words, out data).AssertSuccessWithoutWarnings();
            string customizationJson = fsJsonPrinter.CompressedJson(data);

            AddCustomizationWordsRequest req = new AddCustomizationWordsRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Send = Encoding.UTF8.GetBytes(customizationJson);
            req.OnResponse = OnAddCustomizationWordsResp;

            string service = "/v1/customizations/{0}/words";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class AddCustomizationWordsRequest : RESTConnector.Request
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

        private void OnAddCustomizationWordsResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((AddCustomizationWordsRequest)req).CustomData;

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((AddCustomizationWordsRequest)req).SuccessCallback != null)
                    ((AddCustomizationWordsRequest)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((AddCustomizationWordsRequest)req).FailCallback != null)
                    ((AddCustomizationWordsRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Delete Customization Word
        /// <summary>
        /// Deletes a single word from the custom voice model with the specified customization_id. Only the owner of a custom voice model can use this method to delete a word from the model.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The voice model's identifier.</param>
        /// <param name="word">The word to be deleted.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool DeleteCustomizationWord(SuccessCallback<bool> successCallback, FailCallback failCallback, string customizationID, string word, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID is required for DeleteCustomizationWord");
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("A word to delete is required for DeleteCustomizationWord");

            DeleteCustomizationWordRequest req = new DeleteCustomizationWordRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.Timeout = RequestTimeout;
            req.Delete = true;
            req.OnResponse = OnDeleteCustomizationWordResp;

            string service = "/v1/customizations/{0}/words/{1}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID, word));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteCustomizationWordRequest : RESTConnector.Request
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

        private void OnDeleteCustomizationWordResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((DeleteCustomizationWordRequest)req).CustomData;

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((DeleteCustomizationWordRequest)req).SuccessCallback != null)
                    ((DeleteCustomizationWordRequest)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((DeleteCustomizationWordRequest)req).FailCallback != null)
                    ((DeleteCustomizationWordRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Customization Word
        /// <summary>
        /// Returns the translation for a single word from the custom model with the specified `customization_id`. The output shows the translation as it is defined in the model. Only the owner of a custom voice model can use this method to query information about a word from the model.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The requested custom voice model's identifier.</param>
        /// <param name="word">The requested word.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomizationWord(SuccessCallback<Translation> successCallback, FailCallback failCallback, string customizationID, string word, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID is required to get a custom voice model's words.");
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("A word is requrred to get a translation.");

            GetCustomizationWordRequest req = new GetCustomizationWordRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.OnResponse = OnGetCustomizationWordResp;

            string service = "/v1/customizations/{0}/words/{1}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID, word));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomizationWordRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Translation> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetCustomizationWordResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Translation result = new Translation();
            fsData data = null;
            Dictionary<string, object> customData = ((GetCustomizationWordRequest)req).CustomData;

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
                    Log.Error("TextToSpeech.OnGetCustomizationWordResp()", "GetCustomizationWord Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetCustomizationWordRequest)req).SuccessCallback != null)
                    ((GetCustomizationWordRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetCustomizationWordRequest)req).FailCallback != null)
                    ((GetCustomizationWordRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Add Customization Word
        /// <summary>
        /// Adds a single word and its translation to the custom voice model with the specified `customization_id`. A custom model can contain no more than 20,000 entries. Only the owner of a custom voice model can use this method to add a word to the model.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customizationID">The identifier of the custom voice model to be updated.</param>
        /// <param name="words">Words object to add to custom voice model.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool AddCustomizationWord(SuccessCallback<bool> successCallback, FailCallback failCallback, string customizationID, string word, string translation, Dictionary<string, object> customData = null)
        {
            Log.Error("TextToSpeech.AddCustomizationWord()", "AddCustomizationWord is not supported. Unity WWW does not support PUT method! Use AddCustomizationWords() instead!");
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("customizationID");
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("word");
            if (string.IsNullOrEmpty(translation))
                throw new ArgumentNullException("translation");

            string json = "{\n\t\"translation\":\"" + translation + "\"\n}";

            AddCustomizationWordRequest req = new AddCustomizationWordRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Headers["X-HTTP-Method-Override"] = "PUT";
            req.Send = Encoding.UTF8.GetBytes(json);
            req.OnResponse = OnAddCustomizationWordResp;

            string service = "/v1/customizations/{0}/words/{1}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID, word));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class AddCustomizationWordRequest : RESTConnector.Request
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

        private void OnAddCustomizationWordResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((AddCustomizationWordRequest)req).CustomData;

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((AddCustomizationWordRequest)req).SuccessCallback != null)
                    ((AddCustomizationWordRequest)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((AddCustomizationWordRequest)req).FailCallback != null)
                    ((AddCustomizationWordRequest)req).FailCallback(resp.Error, customData);
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
