﻿/**
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

        #region GetVoices 
        /// <summary>
        /// This callback is used by the GetVoices() function.
        /// </summary>
        public delegate void GetVoicesCallback(RESTConnector.ParsedResponse<Voices> resp);
        /// <summary>
        /// Returns all available voices that can be used.
        /// </summary>
        /// <param name="callback">The callback to invoke with the list of available voices.</param>
        /// <returns>Returns ture if the request was submitted.</returns>
        public bool GetVoices(GetVoicesCallback callback, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/voices");
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
            public string Data { get; set; }
        };

        private void OnGetVoicesResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetVoicesReq)req).Data;

            RESTConnector.ParsedResponse<Voices> parsedResp = new RESTConnector.ParsedResponse<Voices>(resp, customData, _serializer);

            if (((GetVoicesReq)req).Callback != null)
                ((GetVoicesReq)req).Callback(parsedResp);
        }
        #endregion

        #region GetVoice 
        /// <summary>
        /// This callback is used by the GetVoice() function.
        /// </summary>
        public delegate void GetVoiceCallback(RESTConnector.ParsedResponse<Voice> resp);
        /// <summary>
        /// Return specific voice.
        /// </summary>
        /// <param name="callback">The callback to invoke with the voice.</param>
        /// <param name="voice">The name of the voice you would like to get. If this is null, TextToSpeech will default to the set voice.</param>
        /// <returns>Returns ture if the request was submitted.</returns>
        public bool GetVoice(GetVoiceCallback callback, VoiceType? voice = null, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (voice == null)
                voice = _voice;

            string service = "/v1/voices/{0}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, GetVoiceType((VoiceType)voice)));
            if (connector == null)
                return false;

            GetVoiceReq req = new GetVoiceReq();
            req.Callback = callback;
            req.OnResponse = OnGetVoiceResp;

            return connector.Send(req);
        }
        private class GetVoiceReq : RESTConnector.Request
        {
            public GetVoiceCallback Callback { get; set; }
            public string Data { get; set; }
        }

        private void OnGetVoiceResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetVoiceReq)req).Data;

            RESTConnector.ParsedResponse<Voice> parsedResp = new RESTConnector.ParsedResponse<Voice>(resp, customData, _serializer);

            if (((GetVoiceReq)req).Callback != null)
                ((GetVoiceReq)req).Callback(parsedResp);
        }
        #endregion

        #region Synthesize Functions
        /// <summary>
        /// This callback is passed into the ToSpeech() method.
        /// </summary>
        public delegate void ToSpeechCallback(RESTConnector.ParsedResponse<AudioClip> resp);
        /// <summary>
        /// Private Request object that holds data specific to the ToSpeech request.
        /// </summary>
        private class ToSpeechRequest : RESTConnector.Request
        {
            public string TextId { get; set; }
            public string Text { get; set; }
            public ToSpeechCallback Callback { get; set; }
            public string Data { get; set; }
        }

        /// <summary>
        /// Converts the given text into an AudioClip that can be played.
        /// </summary>
        /// <param name="text">The text to synthesis into speech.</param>
        /// <param name="callback">The callback to invoke with the AudioClip.</param>
        /// <param name="usePost">If true, then we use post instead of get, this allows for text that exceeds the 5k limit.</param>
        /// <returns>Returns true if the request is sent.</returns>
        public bool ToSpeech(string text, ToSpeechCallback callback, bool usePost = false, string customData = default(string))
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (callback == null)
                throw new ArgumentNullException("callback");

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

            string textId = Utility.GetMD5(text);

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/synthesize");
            if (connector == null)
            {
                Log.Error("TextToSpeech.ToSpeech()", "Failed to get connector.");
                return false;
            }

            ToSpeechRequest req = new ToSpeechRequest();
            req.TextId = textId;
            req.Text = text;
            req.Callback = callback;
            req.Parameters["accept"] = _audioFormats[_audioFormat];
            req.Parameters["voice"] = _voiceTypes[_voice];
            req.OnResponse = ToSpeechResponse;

            if (usePost)
            {
                Dictionary<string, string> upload = new Dictionary<string, string>();
                upload["text"] = text;

                req.Send = Encoding.UTF8.GetBytes(Json.Serialize(upload));
                req.Headers["Content-Type"] = "application/json";
            }
            else
            {
                req.Parameters["text"] = text;
            }

            return connector.Send(req);
        }

        private void ToSpeechResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ToSpeechRequest speechReq = req as ToSpeechRequest;
            if (speechReq == null)
                throw new WatsonException("Wrong type of request object.");

            //Log.Debug( "TextToSpeech.ToSpeechResponse()", "Request completed in {0} seconds.", resp.ElapsedTime );

            string customData = ((ToSpeechRequest)req).Data;

            RESTConnector.ParsedResponse<AudioClip> parsedResp = new RESTConnector.ParsedResponse<AudioClip>(resp, customData, null, false);

            AudioClip clip = resp.Success ? ProcessResponse(speechReq.TextId, resp.Data) : null;
            if (clip == null)
                Log.Error("TextToSpeech.ToSpeechResponse()", "Request Failed: {0}", resp.Error);
            else
                parsedResp.DataObject = clip;

            if (((ToSpeechRequest)req).Callback != null)
                ((ToSpeechRequest)req).Callback(parsedResp);
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
        #endregion

        #region GetPronunciation
        /// <summary>
        /// This callback is used by the GetPronunciation() function.
        /// </summary>
        public delegate void GetPronunciationCallback(RESTConnector.ParsedResponse<Pronunciation> resp);
        /// <summary>
        /// Returns the phonetic pronunciation for the word specified by the text parameter. You can request 
        /// the pronunciation for a specific format. You can also request the pronunciation for a specific
        /// voice to see the default translation for the language of that voice or for a specific custom voice
        /// model to see the translation for that voice model.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="callback">The GetPronunciationCallback</param>
        /// <param name="text">The text string to pronounce.</param>
        /// <param name="voice">Specify a voice to obtain the pronunciation for the specified word in the language of that voice. All voices for the same language (for example, en-US) return the same translation. Do not specify both a voice and a customization_id. Retrieve available voices with the GET /v1/voices method. If this is null, TextToSpeech will default to the set voice.</param>
        /// <param name="format">Specify the phoneme set in which to return the pronunciation. Omit the parameter to obtain the pronunciation in the default format. Either ipa or spr.</param>
        /// <param name="customization_id">GUID of a custom voice model for which the pronunciation is to be returned. You must make the request with the service credentials of the model's owner. If the word is not defined in the specified voice model, the service returns the default translation for the model's language. Omit the parameter to see the translation for the specified voice with no customization. Do not specify both a voice and a customization_id.</param>
        /// <returns></returns>
        public bool GetPronunciation(GetPronunciationCallback callback, string text, VoiceType? voice = null, string format = "ipa", string customization_id = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (voice == null)
                voice = _voice;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/pronunciation");
            if (connector == null)
                return false;

            GetPronunciationReq req = new GetPronunciationReq();
            req.Callback = callback;
            req.Text = text;
            req.Voice = (VoiceType)voice;
            req.Format = format;
            req.Customization_ID = customization_id;
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
            public GetPronunciationCallback Callback { get; set; }
            public string Text { get; set; }
            public VoiceType Voice { get; set; }
            public string Format { get; set; }
            public string Customization_ID { get; set; }
            public string Data { get; set; }
        }

        private void OnGetPronunciationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetPronunciationReq)req).Data;

            RESTConnector.ParsedResponse<Pronunciation> parsedResp = new RESTConnector.ParsedResponse<Pronunciation>(resp, customData, _serializer);

            if (((GetPronunciationReq)req).Callback != null)
                ((GetPronunciationReq)req).Callback(parsedResp);
        }
        #endregion

        #region Get Customizations
        /// <summary>
        /// This callback is used by the GetCustomizations() function.
        /// </summary>
        public delegate void GetCustomizationsCallback(RESTConnector.ParsedResponse<Customizations> resp);

        /// <summary>
        /// Lists metadata such as the name and description for the custom voice models that you own. Use the language query parameter to list the voice models that you own for the specified language only. Omit the parameter to see all voice models that you own for all languages. To see the words in addition to the metadata for a specific voice model, use the GET /v1/customizations/{customization_id} method. Only the owner of a custom voice model can use this method to list information about the model.
        /// Note: This method is currently a beta release that supports US English only
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomizations(GetCustomizationsCallback callback, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            GetCustomizationsReq req = new GetCustomizationsReq();
            req.Callback = callback;
            req.Data = customData;
            req.OnResponse = OnGetCustomizationsResp;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/customizations");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomizationsReq : RESTConnector.Request
        {
            public GetCustomizationsCallback Callback { get; set; }
            public string Data { get; set; }
        }

        private void OnGetCustomizationsResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetCustomizationsReq)req).Data;

            RESTConnector.ParsedResponse<Customizations> parsedResp = new RESTConnector.ParsedResponse<Customizations>(resp, customData, _serializer);

            if (((GetCustomizationsReq)req).Callback != null)
                ((GetCustomizationsReq)req).Callback(parsedResp);
        }
        #endregion

        #region Create Customization
        /// <summary>
        /// Thid callback is used by the CreateCustomization() function.
        /// </summary>
        public delegate void CreateCustomizationCallback(RESTConnector.ParsedResponse<CustomizationID> resp);

        /// <summary>
        /// Creates a new empty custom voice model that is owned by the requesting user.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="name">Name of the new custom voice model.</param>
        /// <param name="language">Language of the new custom voice model. Omit the parameter to use the default language, en-US. = ['de-DE', 'en-US', 'en-GB', 'es-ES', 'es-US', 'fr-FR', 'it-IT', 'ja-JP', 'pt-BR'].</param>
        /// <param name="description">Description of the new custom voice model.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool CreateCustomization(CreateCustomizationCallback callback, string name, string language = default(string), string description = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
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
            req.Callback = callback;
            req.CustomVoice = customVoice;
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
            public CustomVoice CustomVoice { get; set; }
            public string Data { get; set; }
        }

        private void OnCreateCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((CreateCustomizationRequest)req).Data;

            RESTConnector.ParsedResponse<CustomizationID> parsedResp = new RESTConnector.ParsedResponse<CustomizationID>(resp, customData, _serializer);

            if (((CreateCustomizationRequest)req).Callback != null)
                ((CreateCustomizationRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Delete Customization
        /// <summary>
        /// This callback is used by the DeleteCustomization() function.
        /// </summary>
        public delegate void OnDeleteCustomizationCallback(RESTConnector.ParsedResponse<object> resp);
        /// <summary>
        /// Deletes the custom voice model with the specified `customization_id`. Only the owner of a custom voice model can use this method to delete the model.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="customizationID">The voice model to be deleted's identifier.</param>
        /// <param name="customData">Optional custom data.</param>
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
            public OnDeleteCustomizationCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public string Data { get; set; }
        }

        private void OnDeleteCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((DeleteCustomizationRequest)req).Data;

            RESTConnector.ParsedResponse<object> parsedResp = new RESTConnector.ParsedResponse<object>(resp, customData, null);

            if (((DeleteCustomizationRequest)req).Callback != null)
                ((DeleteCustomizationRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Get Customization
        /// <summary>
        /// This callback is used by the GetCusomization() function.
        /// </summary>
        public delegate void GetCustomizationCallback(RESTConnector.ParsedResponse<Customization> resp);
        /// <summary>
        /// Lists all information about the custom voice model with the specified `customization_id`. In addition to metadata such as the name and description of the voice model, the output includes the words in the model and their translations as defined in the model. To see just the metadata for a voice model, use the GET `/v1/customizations` method. Only the owner of a custom voice model can use this method to query information about the model.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="customizationID">The requested custom voice model's identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomization(GetCustomizationCallback callback, string customizationID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID to get a custom voice model.");

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
            string customData = ((GetCustomizationRequest)req).Data;

            RESTConnector.ParsedResponse<Customization> parsedResp = new RESTConnector.ParsedResponse<Customization>(resp, customData, _serializer);

            if (((GetCustomizationRequest)req).Callback != null)
                ((GetCustomizationRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Update Customization
        /// <summary>
        /// This callback is used by the UpdateCustomization() function.
        /// </summary>
        public delegate void UpdateCustomizationCallback(RESTConnector.ParsedResponse<object> resp);

        /// <summary>
        /// Updates information for the custom voice model with the specified `customization_id`. You can update the metadata such as the name and description of the voice model. You can also update the words in the model and their translations. A custom model can contain no more than 20,000 entries. Only the owner of a custom voice model can use this method to update the model.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="customizationID">The identifier of the custom voice model to be updated.</param>
        /// <param name="customVoiceUpdate">Custom voice model update data.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool UpdateCustomization(UpdateCustomizationCallback callback, string customizationID, CustomVoiceUpdate customVoiceUpdate, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("customizationID");
            if (!customVoiceUpdate.HasData())
                throw new ArgumentNullException("Custom voice update data is required to update a custom voice model.");

            fsData data;
            _serializer.TrySerialize(customVoiceUpdate.GetType(), customVoiceUpdate, out data).AssertSuccessWithoutWarnings();
            string customizationJson = fsJsonPrinter.CompressedJson(data);

            UpdateCustomizationRequest req = new UpdateCustomizationRequest();
            req.Callback = callback;
            req.CustomVoiceUpdate = customVoiceUpdate;
            req.CustomizationID = customizationID;
            req.Data = customData;
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
            public UpdateCustomizationCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public CustomVoiceUpdate CustomVoiceUpdate { get; set; }
            public string Data { get; set; }
        }

        private void OnUpdateCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((UpdateCustomizationRequest)req).Data;

            RESTConnector.ParsedResponse<object> parsedResp = new RESTConnector.ParsedResponse<object>(resp, customData, null);

            if (((UpdateCustomizationRequest)req).Callback != null)
                ((UpdateCustomizationRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Get Customization Words
        /// <summary>
        /// This callback is used by the GetCusomizationWords() function.
        /// </summary>
        public delegate void GetCustomizationWordsCallback(RESTConnector.ParsedResponse<Words> resp);
        /// <summary>
        /// Lists all of the words and their translations for the custom voice model with the specified `customization_id`. The output shows the translations as they are defined in the model. Only the owner of a custom voice model can use this method to query information about the model's words.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="customizationID">The requested custom voice model's identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomizationWords(GetCustomizationWordsCallback callback, string customizationID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID is required to get a custom voice model's words.");

            GetCustomizationWordsRequest req = new GetCustomizationWordsRequest();
            req.Callback = callback;
            req.CustomizationID = customizationID;
            req.Data = customData;
            req.OnResponse = OnGetCustomizationWordsResp;

            string service = "/v1/customizations/{0}/words";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomizationWordsRequest : RESTConnector.Request
        {
            public GetCustomizationWordsCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public string Data { get; set; }
        }

        private void OnGetCustomizationWordsResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetCustomizationWordsRequest)req).Data;

            RESTConnector.ParsedResponse<Words> parsedResp = new RESTConnector.ParsedResponse<Words>(resp, customData, _serializer);

            if (((GetCustomizationWordsRequest)req).Callback != null)
                ((GetCustomizationWordsRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Add Customization Words
        /// <summary>
        /// This callback is used by the AddCustomizationWords() function.
        /// </summary>
        public delegate void AddCustomizationWordsCallback(RESTConnector.ParsedResponse<object> resp);

        /// <summary>
        /// Adds one or more words and their translations to the custom voice model with the specified `customization_id`. A custom model can contain no more than 20,000 entries. Only the owner of a custom voice model can use this method to add words to the model.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="customizationID">The identifier of the custom voice model to be updated.</param>
        /// <param name="words">Words object to add to custom voice model.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool AddCustomizationWords(AddCustomizationWordsCallback callback, string customizationID, Words words, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("customizationID");
            if (!words.HasData())
                throw new ArgumentNullException("Words data is required to add words to a custom voice model.");

            fsData data;
            _serializer.TrySerialize(words.GetType(), words, out data).AssertSuccessWithoutWarnings();
            string customizationJson = fsJsonPrinter.CompressedJson(data);

            AddCustomizationWordsRequest req = new AddCustomizationWordsRequest();
            req.Callback = callback;
            req.Words = words;
            req.CustomizationID = customizationID;
            req.Data = customData;
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
            public AddCustomizationWordsCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public Words Words { get; set; }
            public string Data { get; set; }
        }

        private void OnAddCustomizationWordsResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((AddCustomizationWordsRequest)req).Data;

            RESTConnector.ParsedResponse<object> parsedResp = new RESTConnector.ParsedResponse<object>(resp, customData, null);

            if (((AddCustomizationWordsRequest)req).Callback != null)
                ((AddCustomizationWordsRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Delete Customization Word
        /// <summary>
        /// This callback is used by the DeleteCustomizationWord() function.
        /// </summary>
        public delegate void OnDeleteCustomizationWordCallback(RESTConnector.ParsedResponse<object> resp);
        /// <summary>
        /// Deletes a single word from the custom voice model with the specified customization_id. Only the owner of a custom voice model can use this method to delete a word from the model.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="customizationID">The voice model's identifier.</param>
        /// <param name="word">The word to be deleted.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool DeleteCustomizationWord(OnDeleteCustomizationWordCallback callback, string customizationID, string word, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID is required for DeleteCustomizationWord");
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("A word to delete is required for DeleteCustomizationWord");

            DeleteCustomizationWordRequest req = new DeleteCustomizationWordRequest();
            req.Callback = callback;
            req.CustomizationID = customizationID;
            req.Word = word;
            req.Data = customData;
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
            public OnDeleteCustomizationWordCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public string Word { get; set; }
            public string Data { get; set; }
        }

        private void OnDeleteCustomizationWordResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((DeleteCustomizationWordRequest)req).Data;

            RESTConnector.ParsedResponse<object> parsedResp = new RESTConnector.ParsedResponse<object>(resp, customData, null);

            if (((DeleteCustomizationWordRequest)req).Callback != null)
                ((DeleteCustomizationWordRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Get Customization Word
        /// <summary>
        /// This callback is used by the GetCusomizationWord() function.
        /// </summary>
        public delegate void GetCustomizationWordCallback(RESTConnector.ParsedResponse<Translation> resp);
        /// <summary>
        /// Returns the translation for a single word from the custom model with the specified `customization_id`. The output shows the translation as it is defined in the model. Only the owner of a custom voice model can use this method to query information about a word from the model.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="customizationID">The requested custom voice model's identifier.</param>
        /// <param name="word">The requested word.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetCustomizationWord(GetCustomizationWordCallback callback, string customizationID, string word, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("A customizationID is required to get a custom voice model's words.");
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("A word is requrred to get a translation.");

            GetCustomizationWordRequest req = new GetCustomizationWordRequest();
            req.Callback = callback;
            req.CustomizationID = customizationID;
            req.Word = word;
            req.Data = customData;
            req.OnResponse = OnGetCustomizationWordResp;

            string service = "/v1/customizations/{0}/words/{1}";
            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(service, customizationID, word));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCustomizationWordRequest : RESTConnector.Request
        {
            public GetCustomizationWordCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public string Word { get; set; }
            public string Data { get; set; }
        }

        private void OnGetCustomizationWordResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetCustomizationWordRequest)req).Data;

            RESTConnector.ParsedResponse<Translation> parsedResp = new RESTConnector.ParsedResponse<Translation>(resp, customData, _serializer);

            if (((GetCustomizationWordRequest)req).Callback != null)
                ((GetCustomizationWordRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Add Customization Word
        /// <summary>
        /// This callback is used by the AddCustomizationWord() function.
        /// </summary>
        public delegate void AddCustomizationWordCallback(RESTConnector.ParsedResponse<object> resp);

        /// <summary>
        /// Adds a single word and its translation to the custom voice model with the specified `customization_id`. A custom model can contain no more than 20,000 entries. Only the owner of a custom voice model can use this method to add a word to the model.
        /// Note: This method is currently a beta release that supports US English only.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="customizationID">The identifier of the custom voice model to be updated.</param>
        /// <param name="words">Words object to add to custom voice model.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool AddCustomizationWord(AddCustomizationWordCallback callback, string customizationID, string word, string translation, string customData = default(string))
        {
            Log.Error("TextToSpeech.AddCustomizationWord()", "AddCustomizationWord is not supported. Unity WWW does not support PUT method! Use AddCustomizationWords() instead!");
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(customizationID))
                throw new ArgumentNullException("customizationID");
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("word");
            if (string.IsNullOrEmpty(translation))
                throw new ArgumentNullException("translation");

            string json = "{\n\t\"translation\":\"" + translation + "\"\n}";

            AddCustomizationWordRequest req = new AddCustomizationWordRequest();
            req.Callback = callback;
            req.CustomizationID = customizationID;
            req.Word = word;
            req.Translation = translation;
            req.Data = customData;
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
            public AddCustomizationWordCallback Callback { get; set; }
            public string CustomizationID { get; set; }
            public string Word { get; set; }
            public string Translation { get; set; }
            public string Data { get; set; }
        }

        private void OnAddCustomizationWordResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((AddCustomizationWordRequest)req).Data;

            RESTConnector.ParsedResponse<object> parsedResp = new RESTConnector.ParsedResponse<object>(resp, customData, null);

            if (((AddCustomizationWordRequest)req).Callback != null)
                ((AddCustomizationWordRequest)req).Callback(parsedResp);
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
