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

using System.Collections.Generic;
using System.Text;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Connection;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.TextToSpeech.V1.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Networking;

namespace IBM.Watson.TextToSpeech.V1
{
    public partial class TextToSpeechService : BaseService
    {
        private const string serviceId = "text_to_speech";
        private const string defaultServiceUrl = "https://stream.watsonplatform.net/text-to-speech/api";


        #region DisableSslVerification
        private bool disableSslVerification = false;
        /// <summary>
        /// Gets and sets the option to disable ssl verification
        /// </summary>
        public bool DisableSslVerification
        {
            get { return disableSslVerification; }
            set { disableSslVerification = value; }
        }
        #endregion

        /// <summary>
        /// TextToSpeechService constructor.
        /// </summary>
        public TextToSpeechService() : this(ConfigBasedAuthenticatorFactory.GetAuthenticator(serviceId)) {}

        /// <summary>
        /// TextToSpeechService constructor.
        /// </summary>
        /// <param name="authenticator">The service authenticator.</param>
        public TextToSpeechService(Authenticator authenticator) : base(authenticator, serviceId)
        {
            Authenticator = authenticator;

            if (string.IsNullOrEmpty(GetServiceUrl()))
            {
                SetServiceUrl(defaultServiceUrl);
            }
        }

        /// <summary>
        /// List voices.
        ///
        /// Lists all voices available for use with the service. The information includes the name, language, gender,
        /// and other details about the voice. To see information about a specific voice, use the **Get a voice**
        /// method.
        ///
        /// **See also:** [Listing all available
        /// voices](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-voices#listVoices).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <returns><see cref="Voices" />Voices</returns>
        public bool ListVoices(Callback<Voices> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListVoices`");

            RequestObject<Voices> req = new RequestObject<Voices>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "ListVoices"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnListVoicesResponse;

            Connector.URL = GetServiceUrl() + "/v1/voices";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListVoicesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Voices> response = new DetailedResponse<Voices>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Voices>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnListVoicesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Voices>)req).Callback != null)
                ((RequestObject<Voices>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get a voice.
        ///
        /// Gets information about the specified voice. The information includes the name, language, gender, and other
        /// details about the voice. Specify a customization ID to obtain information for a custom voice model that is
        /// defined for the language of the specified voice. To list information about all available voices, use the
        /// **List voices** method.
        ///
        /// **See also:** [Listing a specific
        /// voice](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-voices#listVoice).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="voice">The voice for which information is to be returned.</param>
        /// <param name="customizationId">The customization ID (GUID) of a custom voice model for which information is
        /// to be returned. You must make the request with credentials for the instance of the service that owns the
        /// custom model. Omit the parameter to see information about the specified voice with no customization.
        /// (optional)</param>
        /// <returns><see cref="Voice" />Voice</returns>
        public bool GetVoice(Callback<Voice> callback, string voice, string customizationId = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetVoice`");
            if (string.IsNullOrEmpty(voice))
                throw new ArgumentNullException("`voice` is required for `GetVoice`");

            RequestObject<Voice> req = new RequestObject<Voice>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "GetVoice"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(customizationId))
            {
                req.Parameters["customization_id"] = customizationId;
            }

            req.OnResponse = OnGetVoiceResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/voices/{0}", voice);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetVoiceResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Voice> response = new DetailedResponse<Voice>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Voice>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnGetVoiceResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Voice>)req).Callback != null)
                ((RequestObject<Voice>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Synthesize audio.
        ///
        /// Synthesizes text to audio that is spoken in the specified voice. The service bases its understanding of the
        /// language for the input text on the specified voice. Use a voice that matches the language of the input text.
        ///
        ///
        /// The method accepts a maximum of 5 KB of input text in the body of the request, and 8 KB for the URL and
        /// headers. The 5 KB limit includes any SSML tags that you specify. The service returns the synthesized audio
        /// stream as an array of bytes.
        ///
        /// **See also:** [The HTTP
        /// interface](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-usingHTTP#usingHTTP).
        ///
        /// ### Audio formats (accept types)
        ///
        ///  The service can return audio in the following formats (MIME types).
        /// * Where indicated, you can optionally specify the sampling rate (`rate`) of the audio. You must specify a
        /// sampling rate for the `audio/l16` and `audio/mulaw` formats. A specified sampling rate must lie in the range
        /// of 8 kHz to 192 kHz. Some formats restrict the sampling rate to certain values, as noted.
        /// * For the `audio/l16` format, you can optionally specify the endianness (`endianness`) of the audio:
        /// `endianness=big-endian` or `endianness=little-endian`.
        ///
        /// Use the `Accept` header or the `accept` parameter to specify the requested format of the response audio. If
        /// you omit an audio format altogether, the service returns the audio in Ogg format with the Opus codec
        /// (`audio/ogg;codecs=opus`). The service always returns single-channel audio.
        /// * `audio/basic` - The service returns audio with a sampling rate of 8000 Hz.
        /// * `audio/flac` - You can optionally specify the `rate` of the audio. The default sampling rate is 22,050 Hz.
        /// * `audio/l16` - You must specify the `rate` of the audio. You can optionally specify the `endianness` of the
        /// audio. The default endianness is `little-endian`.
        /// * `audio/mp3` - You can optionally specify the `rate` of the audio. The default sampling rate is 22,050 Hz.
        /// * `audio/mpeg` - You can optionally specify the `rate` of the audio. The default sampling rate is 22,050 Hz.
        /// * `audio/mulaw` - You must specify the `rate` of the audio.
        /// * `audio/ogg` - The service returns the audio in the `vorbis` codec. You can optionally specify the `rate`
        /// of the audio. The default sampling rate is 22,050 Hz.
        /// * `audio/ogg;codecs=opus` - You can optionally specify the `rate` of the audio. Only the following values
        /// are valid sampling rates: `48000`, `24000`, `16000`, `12000`, or `8000`. If you specify a value other than
        /// one of these, the service returns an error. The default sampling rate is 48,000 Hz.
        /// * `audio/ogg;codecs=vorbis` - You can optionally specify the `rate` of the audio. The default sampling rate
        /// is 22,050 Hz.
        /// * `audio/wav` - You can optionally specify the `rate` of the audio. The default sampling rate is 22,050 Hz.
        /// * `audio/webm` - The service returns the audio in the `opus` codec. The service returns audio with a
        /// sampling rate of 48,000 Hz.
        /// * `audio/webm;codecs=opus` - The service returns audio with a sampling rate of 48,000 Hz.
        /// * `audio/webm;codecs=vorbis` - You can optionally specify the `rate` of the audio. The default sampling rate
        /// is 22,050 Hz.
        ///
        /// For more information about specifying an audio format, including additional details about some of the
        /// formats, see [Audio
        /// formats](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-audioFormats#audioFormats).
        ///
        /// ### Warning messages
        ///
        ///  If a request includes invalid query parameters, the service returns a `Warnings` response header that
        /// provides messages about the invalid parameters. The warning includes a descriptive message and a list of
        /// invalid argument strings. For example, a message such as `"Unknown arguments:"` or `"Unknown url query
        /// arguments:"` followed by a list of the form `"{invalid_arg_1}, {invalid_arg_2}."` The request succeeds
        /// despite the warnings.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="text">The text to synthesize.</param>
        /// <param name="accept">The requested format (MIME type) of the audio. You can use the `Accept` header or the
        /// `accept` parameter to specify the audio format. For more information about specifying an audio format, see
        /// **Audio formats (accept types)** in the method description. (optional, default to
        /// audio/ogg;codecs=opus)</param>
        /// <param name="voice">The voice to use for synthesis. (optional, default to en-US_MichaelVoice)</param>
        /// <param name="customizationId">The customization ID (GUID) of a custom voice model to use for the synthesis.
        /// If a custom voice model is specified, it is guaranteed to work only if it matches the language of the
        /// indicated voice. You must make the request with credentials for the instance of the service that owns the
        /// custom model. Omit the parameter to use the specified voice with no customization. (optional)</param>
        /// <returns><see cref="byte[]" />byte[]</returns>
        public bool Synthesize(Callback<byte[]> callback, string text, string accept = null, string voice = null, string customizationId = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `Synthesize`");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("`text` is required for `Synthesize`");

            RequestObject<byte[]> req = new RequestObject<byte[]>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "Synthesize"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(voice))
            {
                req.Parameters["voice"] = voice;
            }
            if (!string.IsNullOrEmpty(customizationId))
            {
                req.Parameters["customization_id"] = customizationId;
            }
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "audio/basic";

            if (!string.IsNullOrEmpty(accept))
            {
                req.Headers["Accept"] = accept;
            }

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(text))
                bodyObject["text"] = text;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnSynthesizeResponse;

            Connector.URL = GetServiceUrl() + "/v1/synthesize";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnSynthesizeResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<byte[]> response = new DetailedResponse<byte[]>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            response.Result = resp.Data;

            if (((RequestObject<byte[]>)req).Callback != null)
                ((RequestObject<byte[]>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get pronunciation.
        ///
        /// Gets the phonetic pronunciation for the specified word. You can request the pronunciation for a specific
        /// format. You can also request the pronunciation for a specific voice to see the default translation for the
        /// language of that voice or for a specific custom voice model to see the translation for that voice model.
        ///
        /// **Note:** This method is currently a beta release. The method does not support the Arabic, Chinese, and
        /// Dutch languages.
        ///
        /// **See also:** [Querying a word from a
        /// language](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customWords#cuWordsQueryLanguage).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="text">The word for which the pronunciation is requested.</param>
        /// <param name="voice">A voice that specifies the language in which the pronunciation is to be returned. All
        /// voices for the same language (for example, `en-US`) return the same translation. (optional, default to
        /// en-US_MichaelVoice)</param>
        /// <param name="format">The phoneme format in which to return the pronunciation. Omit the parameter to obtain
        /// the pronunciation in the default format. (optional, default to ipa)</param>
        /// <param name="customizationId">The customization ID (GUID) of a custom voice model for which the
        /// pronunciation is to be returned. The language of a specified custom model must match the language of the
        /// specified voice. If the word is not defined in the specified custom model, the service returns the default
        /// translation for the custom model's language. You must make the request with credentials for the instance of
        /// the service that owns the custom model. Omit the parameter to see the translation for the specified voice
        /// with no customization. (optional)</param>
        /// <returns><see cref="Pronunciation" />Pronunciation</returns>
        public bool GetPronunciation(Callback<Pronunciation> callback, string text, string voice = null, string format = null, string customizationId = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetPronunciation`");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("`text` is required for `GetPronunciation`");

            RequestObject<Pronunciation> req = new RequestObject<Pronunciation>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "GetPronunciation"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(text))
            {
                req.Parameters["text"] = text;
            }
            if (!string.IsNullOrEmpty(voice))
            {
                req.Parameters["voice"] = voice;
            }
            if (!string.IsNullOrEmpty(format))
            {
                req.Parameters["format"] = format;
            }
            if (!string.IsNullOrEmpty(customizationId))
            {
                req.Parameters["customization_id"] = customizationId;
            }

            req.OnResponse = OnGetPronunciationResponse;

            Connector.URL = GetServiceUrl() + "/v1/pronunciation";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetPronunciationResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Pronunciation> response = new DetailedResponse<Pronunciation>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Pronunciation>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnGetPronunciationResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Pronunciation>)req).Callback != null)
                ((RequestObject<Pronunciation>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create a custom model.
        ///
        /// Creates a new empty custom voice model. You must specify a name for the new custom model. You can optionally
        /// specify the language and a description for the new model. The model is owned by the instance of the service
        /// whose credentials are used to create it.
        ///
        /// **Note:** This method is currently a beta release. The service does not support voice model customization
        /// for the Arabic, Chinese, and Dutch languages.
        ///
        /// **See also:** [Creating a custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customModels#cuModelsCreate).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="name">The name of the new custom voice model.</param>
        /// <param name="language">The language of the new custom voice model. Omit the parameter to use the the default
        /// language, `en-US`. (optional, default to en-US)</param>
        /// <param name="description">A description of the new custom voice model. Specifying a description is
        /// recommended. (optional)</param>
        /// <returns><see cref="VoiceModel" />VoiceModel</returns>
        public bool CreateVoiceModel(Callback<VoiceModel> callback, string name, string language = null, string description = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateVoiceModel`");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("`name` is required for `CreateVoiceModel`");

            RequestObject<VoiceModel> req = new RequestObject<VoiceModel>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "CreateVoiceModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(name))
                bodyObject["name"] = name;
            if (!string.IsNullOrEmpty(language))
                bodyObject["language"] = language;
            if (!string.IsNullOrEmpty(description))
                bodyObject["description"] = description;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateVoiceModelResponse;

            Connector.URL = GetServiceUrl() + "/v1/customizations";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateVoiceModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<VoiceModel> response = new DetailedResponse<VoiceModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<VoiceModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnCreateVoiceModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<VoiceModel>)req).Callback != null)
                ((RequestObject<VoiceModel>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List custom models.
        ///
        /// Lists metadata such as the name and description for all custom voice models that are owned by an instance of
        /// the service. Specify a language to list the voice models for that language only. To see the words in
        /// addition to the metadata for a specific voice model, use the **List a custom model** method. You must use
        /// credentials for the instance of the service that owns a model to list information about it.
        ///
        /// **Note:** This method is currently a beta release.
        ///
        /// **See also:** [Querying all custom
        /// models](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customModels#cuModelsQueryAll).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="language">The language for which custom voice models that are owned by the requesting
        /// credentials are to be returned. Omit the parameter to see all custom voice models that are owned by the
        /// requester. (optional)</param>
        /// <returns><see cref="VoiceModels" />VoiceModels</returns>
        public bool ListVoiceModels(Callback<VoiceModels> callback, string language = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListVoiceModels`");

            RequestObject<VoiceModels> req = new RequestObject<VoiceModels>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "ListVoiceModels"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(language))
            {
                req.Parameters["language"] = language;
            }

            req.OnResponse = OnListVoiceModelsResponse;

            Connector.URL = GetServiceUrl() + "/v1/customizations";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListVoiceModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<VoiceModels> response = new DetailedResponse<VoiceModels>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<VoiceModels>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnListVoiceModelsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<VoiceModels>)req).Callback != null)
                ((RequestObject<VoiceModels>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update a custom model.
        ///
        /// Updates information for the specified custom voice model. You can update metadata such as the name and
        /// description of the voice model. You can also update the words in the model and their translations. Adding a
        /// new translation for a word that already exists in a custom model overwrites the word's existing translation.
        /// A custom model can contain no more than 20,000 entries. You must use credentials for the instance of the
        /// service that owns a model to update it.
        ///
        /// You can define sounds-like or phonetic translations for words. A sounds-like translation consists of one or
        /// more words that, when combined, sound like the word. Phonetic translations are based on the SSML phoneme
        /// format for representing a word. You can specify them in standard International Phonetic Alphabet (IPA)
        /// representation
        ///
        ///   <code>&lt;phoneme alphabet="ipa" ph="t&#601;m&#712;&#593;to"&gt;&lt;/phoneme&gt;</code>
        ///
        ///   or in the proprietary IBM Symbolic Phonetic Representation (SPR)
        ///
        ///   <code>&lt;phoneme alphabet="ibm" ph="1gAstroEntxrYFXs"&gt;&lt;/phoneme&gt;</code>
        ///
        /// **Note:** This method is currently a beta release.
        ///
        /// **See also:**
        /// * [Updating a custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customModels#cuModelsUpdate)
        /// * [Adding words to a Japanese custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customWords#cuJapaneseAdd)
        /// * [Understanding
        /// customization](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customIntro#customIntro).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom voice model. You must make the
        /// request with credentials for the instance of the service that owns the custom model.</param>
        /// <param name="name">A new name for the custom voice model. (optional)</param>
        /// <param name="description">A new description for the custom voice model. (optional)</param>
        /// <param name="words">An array of `Word` objects that provides the words and their translations that are to be
        /// added or updated for the custom voice model. Pass an empty array to make no additions or updates.
        /// (optional)</param>
        /// <returns><see cref="object" />object</returns>
        public bool UpdateVoiceModel(Callback<object> callback, string customizationId, string name = null, string description = null, List<Word> words = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateVoiceModel`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `UpdateVoiceModel`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "UpdateVoiceModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(name))
                bodyObject["name"] = name;
            if (!string.IsNullOrEmpty(description))
                bodyObject["description"] = description;
            if (words != null && words.Count > 0)
                bodyObject["words"] = JToken.FromObject(words);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateVoiceModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/customizations/{0}", customizationId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateVoiceModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnUpdateVoiceModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get a custom model.
        ///
        /// Gets all information about a specified custom voice model. In addition to metadata such as the name and
        /// description of the voice model, the output includes the words and their translations as defined in the
        /// model. To see just the metadata for a voice model, use the **List custom models** method.
        ///
        /// **Note:** This method is currently a beta release.
        ///
        /// **See also:** [Querying a custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customModels#cuModelsQuery).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom voice model. You must make the
        /// request with credentials for the instance of the service that owns the custom model.</param>
        /// <returns><see cref="VoiceModel" />VoiceModel</returns>
        public bool GetVoiceModel(Callback<VoiceModel> callback, string customizationId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetVoiceModel`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `GetVoiceModel`");

            RequestObject<VoiceModel> req = new RequestObject<VoiceModel>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "GetVoiceModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnGetVoiceModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/customizations/{0}", customizationId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetVoiceModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<VoiceModel> response = new DetailedResponse<VoiceModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<VoiceModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnGetVoiceModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<VoiceModel>)req).Callback != null)
                ((RequestObject<VoiceModel>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete a custom model.
        ///
        /// Deletes the specified custom voice model. You must use credentials for the instance of the service that owns
        /// a model to delete it.
        ///
        /// **Note:** This method is currently a beta release.
        ///
        /// **See also:** [Deleting a custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customModels#cuModelsDelete).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom voice model. You must make the
        /// request with credentials for the instance of the service that owns the custom model.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteVoiceModel(Callback<object> callback, string customizationId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteVoiceModel`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `DeleteVoiceModel`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "DeleteVoiceModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnDeleteVoiceModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/customizations/{0}", customizationId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteVoiceModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnDeleteVoiceModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Add custom words.
        ///
        /// Adds one or more words and their translations to the specified custom voice model. Adding a new translation
        /// for a word that already exists in a custom model overwrites the word's existing translation. A custom model
        /// can contain no more than 20,000 entries. You must use credentials for the instance of the service that owns
        /// a model to add words to it.
        ///
        /// You can define sounds-like or phonetic translations for words. A sounds-like translation consists of one or
        /// more words that, when combined, sound like the word. Phonetic translations are based on the SSML phoneme
        /// format for representing a word. You can specify them in standard International Phonetic Alphabet (IPA)
        /// representation
        ///
        ///   <code>&lt;phoneme alphabet="ipa" ph="t&#601;m&#712;&#593;to"&gt;&lt;/phoneme&gt;</code>
        ///
        ///   or in the proprietary IBM Symbolic Phonetic Representation (SPR)
        ///
        ///   <code>&lt;phoneme alphabet="ibm" ph="1gAstroEntxrYFXs"&gt;&lt;/phoneme&gt;</code>
        ///
        /// **Note:** This method is currently a beta release.
        ///
        /// **See also:**
        /// * [Adding multiple words to a custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customWords#cuWordsAdd)
        /// * [Adding words to a Japanese custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customWords#cuJapaneseAdd)
        /// * [Understanding
        /// customization](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customIntro#customIntro).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom voice model. You must make the
        /// request with credentials for the instance of the service that owns the custom model.</param>
        /// <param name="words">The **Add custom words** method accepts an array of `Word` objects. Each object provides
        /// a word that is to be added or updated for the custom voice model and the word's translation.
        ///
        /// The **List custom words** method returns an array of `Word` objects. Each object shows a word and its
        /// translation from the custom voice model. The words are listed in alphabetical order, with uppercase letters
        /// listed before lowercase letters. The array is empty if the custom model contains no words.</param>
        /// <returns><see cref="object" />object</returns>
        public bool AddWords(Callback<object> callback, string customizationId, List<Word> words)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `AddWords`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `AddWords`");
            if (words == null)
                throw new ArgumentNullException("`words` is required for `AddWords`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "AddWords"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (words != null && words.Count > 0)
                bodyObject["words"] = JToken.FromObject(words);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnAddWordsResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/customizations/{0}/words", customizationId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnAddWordsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnAddWordsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List custom words.
        ///
        /// Lists all of the words and their translations for the specified custom voice model. The output shows the
        /// translations as they are defined in the model. You must use credentials for the instance of the service that
        /// owns a model to list its words.
        ///
        /// **Note:** This method is currently a beta release.
        ///
        /// **See also:** [Querying all words from a custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customWords#cuWordsQueryModel).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom voice model. You must make the
        /// request with credentials for the instance of the service that owns the custom model.</param>
        /// <returns><see cref="Words" />Words</returns>
        public bool ListWords(Callback<Words> callback, string customizationId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListWords`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `ListWords`");

            RequestObject<Words> req = new RequestObject<Words>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "ListWords"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnListWordsResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/customizations/{0}/words", customizationId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListWordsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Words> response = new DetailedResponse<Words>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Words>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnListWordsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Words>)req).Callback != null)
                ((RequestObject<Words>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Add a custom word.
        ///
        /// Adds a single word and its translation to the specified custom voice model. Adding a new translation for a
        /// word that already exists in a custom model overwrites the word's existing translation. A custom model can
        /// contain no more than 20,000 entries. You must use credentials for the instance of the service that owns a
        /// model to add a word to it.
        ///
        /// You can define sounds-like or phonetic translations for words. A sounds-like translation consists of one or
        /// more words that, when combined, sound like the word. Phonetic translations are based on the SSML phoneme
        /// format for representing a word. You can specify them in standard International Phonetic Alphabet (IPA)
        /// representation
        ///
        ///   <code>&lt;phoneme alphabet="ipa" ph="t&#601;m&#712;&#593;to"&gt;&lt;/phoneme&gt;</code>
        ///
        ///   or in the proprietary IBM Symbolic Phonetic Representation (SPR)
        ///
        ///   <code>&lt;phoneme alphabet="ibm" ph="1gAstroEntxrYFXs"&gt;&lt;/phoneme&gt;</code>
        ///
        /// **Note:** This method is currently a beta release.
        ///
        /// **See also:**
        /// * [Adding a single word to a custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customWords#cuWordAdd)
        /// * [Adding words to a Japanese custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customWords#cuJapaneseAdd)
        /// * [Understanding
        /// customization](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customIntro#customIntro).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom voice model. You must make the
        /// request with credentials for the instance of the service that owns the custom model.</param>
        /// <param name="word">The word that is to be added or updated for the custom voice model.</param>
        /// <param name="translation">The phonetic or sounds-like translation for the word. A phonetic translation is
        /// based on the SSML format for representing the phonetic string of a word either as an IPA translation or as
        /// an IBM SPR translation. A sounds-like is one or more words that, when combined, sound like the word.</param>
        /// <param name="partOfSpeech">**Japanese only.** The part of speech for the word. The service uses the value to
        /// produce the correct intonation for the word. You can create only a single entry, with or without a single
        /// part of speech, for any word; you cannot create multiple entries with different parts of speech for the same
        /// word. For more information, see [Working with Japanese
        /// entries](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-rules#jaNotes). (optional)</param>
        /// <returns><see cref="object" />object</returns>
        public bool AddWord(Callback<object> callback, string customizationId, string word, string translation, string partOfSpeech = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `AddWord`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `AddWord`");
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("`word` is required for `AddWord`");
            if (string.IsNullOrEmpty(translation))
                throw new ArgumentNullException("`translation` is required for `AddWord`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPUT,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "AddWord"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Headers["Content-Type"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(translation))
                bodyObject["translation"] = translation;
            if (!string.IsNullOrEmpty(partOfSpeech))
                bodyObject["part_of_speech"] = partOfSpeech;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnAddWordResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/customizations/{0}/words/{1}", customizationId, word);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnAddWordResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnAddWordResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get a custom word.
        ///
        /// Gets the translation for a single word from the specified custom model. The output shows the translation as
        /// it is defined in the model. You must use credentials for the instance of the service that owns a model to
        /// list its words.
        ///
        /// **Note:** This method is currently a beta release.
        ///
        /// **See also:** [Querying a single word from a custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customWords#cuWordQueryModel).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom voice model. You must make the
        /// request with credentials for the instance of the service that owns the custom model.</param>
        /// <param name="word">The word that is to be queried from the custom voice model.</param>
        /// <returns><see cref="Translation" />Translation</returns>
        public bool GetWord(Callback<Translation> callback, string customizationId, string word)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetWord`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `GetWord`");
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("`word` is required for `GetWord`");

            RequestObject<Translation> req = new RequestObject<Translation>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "GetWord"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnGetWordResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/customizations/{0}/words/{1}", customizationId, word);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetWordResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Translation> response = new DetailedResponse<Translation>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Translation>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnGetWordResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Translation>)req).Callback != null)
                ((RequestObject<Translation>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete a custom word.
        ///
        /// Deletes a single word from the specified custom voice model. You must use credentials for the instance of
        /// the service that owns a model to delete its words.
        ///
        /// **Note:** This method is currently a beta release.
        ///
        /// **See also:** [Deleting a word from a custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customWords#cuWordDelete).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom voice model. You must make the
        /// request with credentials for the instance of the service that owns the custom model.</param>
        /// <param name="word">The word that is to be deleted from the custom voice model.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteWord(Callback<object> callback, string customizationId, string word)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteWord`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `DeleteWord`");
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("`word` is required for `DeleteWord`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "DeleteWord"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnDeleteWordResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/customizations/{0}/words/{1}", customizationId, word);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteWordResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnDeleteWordResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete labeled data.
        ///
        /// Deletes all data that is associated with a specified customer ID. The method deletes all data for the
        /// customer ID, regardless of the method by which the information was added. The method has no effect if no
        /// data is associated with the customer ID. You must issue the request with credentials for the same instance
        /// of the service that was used to associate the customer ID with the data.
        ///
        /// You associate a customer ID with data by passing the `X-Watson-Metadata` header with a request that passes
        /// the data.
        ///
        /// **See also:** [Information
        /// security](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-information-security#information-security).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customerId">The customer ID for which all data is to be deleted.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteUserData(Callback<object> callback, string customerId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteUserData`");
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException("`customerId` is required for `DeleteUserData`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "DeleteUserData"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(customerId))
            {
                req.Parameters["customer_id"] = customerId;
            }

            req.OnResponse = OnDeleteUserDataResponse;

            Connector.URL = GetServiceUrl() + "/v1/user_data";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteUserDataResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnDeleteUserDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
    }
}
