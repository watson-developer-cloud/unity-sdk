/**
* (C) Copyright IBM Corp. 2019, 2021.
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

/**
* IBM OpenAPI SDK Code Generator Version: 99-SNAPSHOT-902c9336-20210513-140138
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
        private const string defaultServiceName = "text_to_speech";
        private const string defaultServiceUrl = "https://api.us-south.text-to-speech.watson.cloud.ibm.com";

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
        public TextToSpeechService() : this(defaultServiceName, ConfigBasedAuthenticatorFactory.GetAuthenticator(defaultServiceName)) {}

        /// <summary>
        /// TextToSpeechService constructor.
        /// </summary>
        /// <param name="authenticator">The service authenticator.</param>
        public TextToSpeechService(Authenticator authenticator) : this(defaultServiceName, authenticator) {}

        /// <summary>
        /// TextToSpeechService constructor.
        /// </summary>
        /// <param name="serviceName">The service name to be used when configuring the client instance</param>
        public TextToSpeechService(string serviceName) : this(serviceName, ConfigBasedAuthenticatorFactory.GetAuthenticator(serviceName)) {}

        /// <summary>
        /// TextToSpeechService constructor.
        /// </summary>
        /// <param name="serviceName">The service name to be used when configuring the client instance</param>
        /// <param name="authenticator">The service authenticator.</param>
        public TextToSpeechService(string serviceName, Authenticator authenticator) : base(authenticator, serviceName)
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
        /// and other details about the voice. The ordering of the list of voices can change from call to call; do not
        /// rely on an alphabetized or static list of voices. To see information about a specific voice, use the **Get a
        /// voice** method.
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
        /// details about the voice. Specify a customization ID to obtain information for a custom model that is defined
        /// for the language of the specified voice. To list information about all available voices, use the **List
        /// voices** method.
        ///
        /// **See also:** [Listing a specific
        /// voice](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-voices#listVoice).
        ///
        /// ### Important voice updates
        ///
        ///  The service's voices underwent significant change on 2 December 2020.
        /// * The Arabic, Chinese, Dutch, Australian English, and Korean voices are now neural instead of concatenative.
        /// * The `ar-AR_OmarVoice` voice is deprecated. Use `ar-MS_OmarVoice` voice instead.
        /// * The `ar-AR` language identifier cannot be used to create a custom model. Use the `ar-MS` identifier
        /// instead.
        /// * The standard concatenative voices for the following languages are now deprecated: Brazilian Portuguese,
        /// United Kingdom and United States English, French, German, Italian, Japanese, and Spanish (all dialects).
        /// * The features expressive SSML, voice transformation SSML, and use of the `volume` attribute of the
        /// `<prosody>` element are deprecated and are not supported with any of the service's neural voices.
        /// * All of the service's voices are now customizable and generally available (GA) for production use.
        ///
        /// The deprecated voices and features will continue to function for at least one year but might be removed at a
        /// future date. You are encouraged to migrate to the equivalent neural voices at your earliest convenience. For
        /// more information about all voice updates, see the [2 December 2020 service
        /// update](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-release-notes#December2020) in the
        /// release notes.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="voice">The voice for which information is to be returned. For more information about specifying
        /// a voice, see **Important voice updates** in the method description.</param>
        /// <param name="customizationId">The customization ID (GUID) of a custom model for which information is to be
        /// returned. You must make the request with credentials for the instance of the service that owns the custom
        /// model. Omit the parameter to see information about the specified voice with no customization.
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
        /// ### Important voice updates
        ///
        ///  The service's voices underwent significant change on 2 December 2020.
        /// * The Arabic, Chinese, Dutch, Australian English, and Korean voices are now neural instead of concatenative.
        /// * The `ar-AR_OmarVoice` voice is deprecated. Use `ar-MS_OmarVoice` voice instead.
        /// * The `ar-AR` language identifier cannot be used to create a custom model. Use the `ar-MS` identifier
        /// instead.
        /// * The standard concatenative voices for the following languages are now deprecated: Brazilian Portuguese,
        /// United Kingdom and United States English, French, German, Italian, Japanese, and Spanish (all dialects).
        /// * The features expressive SSML, voice transformation SSML, and use of the `volume` attribute of the
        /// `<prosody>` element are deprecated and are not supported with any of the service's neural voices.
        /// * All of the service's voices are now customizable and generally available (GA) for production use.
        ///
        /// The deprecated voices and features will continue to function for at least one year but might be removed at a
        /// future date. You are encouraged to migrate to the equivalent neural voices at your earliest convenience. For
        /// more information about all voice updates, see the [2 December 2020 service
        /// update](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-release-notes#December2020) in the
        /// release notes.
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
        /// <param name="voice">The voice to use for synthesis. For more information about specifying a voice, see
        /// **Important voice updates** in the method description. (optional, default to en-US_MichaelV3Voice)</param>
        /// <param name="customizationId">The customization ID (GUID) of a custom model to use for the synthesis. If a
        /// custom model is specified, it works only if it matches the language of the indicated voice. You must make
        /// the request with credentials for the instance of the service that owns the custom model. Omit the parameter
        /// to use the specified voice with no customization. (optional)</param>
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
        /// language of that voice or for a specific custom model to see the translation for that model.
        ///
        /// **See also:** [Querying a word from a
        /// language](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customWords#cuWordsQueryLanguage).
        ///
        /// ### Important voice updates
        ///
        ///  The service's voices underwent significant change on 2 December 2020.
        /// * The Arabic, Chinese, Dutch, Australian English, and Korean voices are now neural instead of concatenative.
        /// * The `ar-AR_OmarVoice` voice is deprecated. Use `ar-MS_OmarVoice` voice instead.
        /// * The `ar-AR` language identifier cannot be used to create a custom model. Use the `ar-MS` identifier
        /// instead.
        /// * The standard concatenative voices for the following languages are now deprecated: Brazilian Portuguese,
        /// United Kingdom and United States English, French, German, Italian, Japanese, and Spanish (all dialects).
        /// * The features expressive SSML, voice transformation SSML, and use of the `volume` attribute of the
        /// `<prosody>` element are deprecated and are not supported with any of the service's neural voices.
        /// * All of the service's voices are now customizable and generally available (GA) for production use.
        ///
        /// The deprecated voices and features will continue to function for at least one year but might be removed at a
        /// future date. You are encouraged to migrate to the equivalent neural voices at your earliest convenience. For
        /// more information about all voice updates, see the [2 December 2020 service
        /// update](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-release-notes#December2020) in the
        /// release notes.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="text">The word for which the pronunciation is requested.</param>
        /// <param name="voice">A voice that specifies the language in which the pronunciation is to be returned. All
        /// voices for the same language (for example, `en-US`) return the same translation. For more information about
        /// specifying a voice, see **Important voice updates** in the method description. (optional, default to
        /// en-US_MichaelV3Voice)</param>
        /// <param name="format">The phoneme format in which to return the pronunciation. The Arabic, Chinese, Dutch,
        /// Australian English, and Korean languages support only IPA. Omit the parameter to obtain the pronunciation in
        /// the default format. (optional, default to ipa)</param>
        /// <param name="customizationId">The customization ID (GUID) of a custom model for which the pronunciation is
        /// to be returned. The language of a specified custom model must match the language of the specified voice. If
        /// the word is not defined in the specified custom model, the service returns the default translation for the
        /// custom model's language. You must make the request with credentials for the instance of the service that
        /// owns the custom model. Omit the parameter to see the translation for the specified voice with no
        /// customization. (optional)</param>
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
        /// Creates a new empty custom model. You must specify a name for the new custom model. You can optionally
        /// specify the language and a description for the new model. The model is owned by the instance of the service
        /// whose credentials are used to create it.
        ///
        /// **See also:** [Creating a custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customModels#cuModelsCreate).
        ///
        /// ### Important voice updates
        ///
        ///  The service's voices underwent significant change on 2 December 2020.
        /// * The Arabic, Chinese, Dutch, Australian English, and Korean voices are now neural instead of concatenative.
        /// * The `ar-AR_OmarVoice` voice is deprecated. Use `ar-MS_OmarVoice` voice instead.
        /// * The `ar-AR` language identifier cannot be used to create a custom model. Use the `ar-MS` identifier
        /// instead.
        /// * The standard concatenative voices for the following languages are now deprecated: Brazilian Portuguese,
        /// United Kingdom and United States English, French, German, Italian, Japanese, and Spanish (all dialects).
        /// * The features expressive SSML, voice transformation SSML, and use of the `volume` attribute of the
        /// `<prosody>` element are deprecated and are not supported with any of the service's neural voices.
        /// * All of the service's voices are now customizable and generally available (GA) for production use.
        ///
        /// The deprecated voices and features will continue to function for at least one year but might be removed at a
        /// future date. You are encouraged to migrate to the equivalent neural voices at your earliest convenience. For
        /// more information about all voice updates, see the [2 December 2020 service
        /// update](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-release-notes#December2020) in the
        /// release notes.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="name">The name of the new custom model.</param>
        /// <param name="language">The language of the new custom model. You create a custom model for a specific
        /// language, not for a specific voice. A custom model can be used with any voice for its specified language.
        /// Omit the parameter to use the the default language, `en-US`. **Note:** The `ar-AR` language identifier
        /// cannot be used to create a custom model. Use the `ar-MS` identifier instead. (optional, default to
        /// en-US)</param>
        /// <param name="description">A description of the new custom model. Specifying a description is recommended.
        /// (optional)</param>
        /// <returns><see cref="CustomModel" />CustomModel</returns>
        public bool CreateCustomModel(Callback<CustomModel> callback, string name, string language = null, string description = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateCustomModel`");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("`name` is required for `CreateCustomModel`");

            RequestObject<CustomModel> req = new RequestObject<CustomModel>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "CreateCustomModel"))
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

            req.OnResponse = OnCreateCustomModelResponse;

            Connector.URL = GetServiceUrl() + "/v1/customizations";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateCustomModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<CustomModel> response = new DetailedResponse<CustomModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<CustomModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnCreateCustomModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<CustomModel>)req).Callback != null)
                ((RequestObject<CustomModel>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// List custom models.
        ///
        /// Lists metadata such as the name and description for all custom models that are owned by an instance of the
        /// service. Specify a language to list the custom models for that language only. To see the words and prompts
        /// in addition to the metadata for a specific custom model, use the **Get a custom model** method. You must use
        /// credentials for the instance of the service that owns a model to list information about it.
        ///
        /// **See also:** [Querying all custom
        /// models](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customModels#cuModelsQueryAll).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="language">The language for which custom models that are owned by the requesting credentials are
        /// to be returned. Omit the parameter to see all custom models that are owned by the requester.
        /// (optional)</param>
        /// <returns><see cref="CustomModels" />CustomModels</returns>
        public bool ListCustomModels(Callback<CustomModels> callback, string language = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListCustomModels`");

            RequestObject<CustomModels> req = new RequestObject<CustomModels>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "ListCustomModels"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(language))
            {
                req.Parameters["language"] = language;
            }

            req.OnResponse = OnListCustomModelsResponse;

            Connector.URL = GetServiceUrl() + "/v1/customizations";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListCustomModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<CustomModels> response = new DetailedResponse<CustomModels>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<CustomModels>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnListCustomModelsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<CustomModels>)req).Callback != null)
                ((RequestObject<CustomModels>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Update a custom model.
        ///
        /// Updates information for the specified custom model. You can update metadata such as the name and description
        /// of the model. You can also update the words in the model and their translations. Adding a new translation
        /// for a word that already exists in a custom model overwrites the word's existing translation. A custom model
        /// can contain no more than 20,000 entries. You must use credentials for the instance of the service that owns
        /// a model to update it.
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
        /// **See also:**
        /// * [Updating a custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customModels#cuModelsUpdate)
        /// * [Adding words to a Japanese custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customWords#cuJapaneseAdd)
        /// * [Understanding
        /// customization](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customIntro#customIntro).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom model. You must make the request
        /// with credentials for the instance of the service that owns the custom model.</param>
        /// <param name="name">A new name for the custom model. (optional)</param>
        /// <param name="description">A new description for the custom model. (optional)</param>
        /// <param name="words">An array of `Word` objects that provides the words and their translations that are to be
        /// added or updated for the custom model. Pass an empty array to make no additions or updates.
        /// (optional)</param>
        /// <returns><see cref="object" />object</returns>
        public bool UpdateCustomModel(Callback<object> callback, string customizationId, string name = null, string description = null, List<Word> words = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateCustomModel`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `UpdateCustomModel`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "UpdateCustomModel"))
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

            req.OnResponse = OnUpdateCustomModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/customizations/{0}", customizationId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateCustomModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("TextToSpeechService.OnUpdateCustomModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Get a custom model.
        ///
        /// Gets all information about a specified custom model. In addition to metadata such as the name and
        /// description of the custom model, the output includes the words and their translations that are defined for
        /// the model, as well as any prompts that are defined for the model. To see just the metadata for a model, use
        /// the **List custom models** method.
        ///
        /// **See also:** [Querying a custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customModels#cuModelsQuery).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom model. You must make the request
        /// with credentials for the instance of the service that owns the custom model.</param>
        /// <returns><see cref="CustomModel" />CustomModel</returns>
        public bool GetCustomModel(Callback<CustomModel> callback, string customizationId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetCustomModel`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `GetCustomModel`");

            RequestObject<CustomModel> req = new RequestObject<CustomModel>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "GetCustomModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnGetCustomModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/customizations/{0}", customizationId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetCustomModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<CustomModel> response = new DetailedResponse<CustomModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<CustomModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnGetCustomModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<CustomModel>)req).Callback != null)
                ((RequestObject<CustomModel>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Delete a custom model.
        ///
        /// Deletes the specified custom model. You must use credentials for the instance of the service that owns a
        /// model to delete it.
        ///
        /// **See also:** [Deleting a custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customModels#cuModelsDelete).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom model. You must make the request
        /// with credentials for the instance of the service that owns the custom model.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteCustomModel(Callback<object> callback, string customizationId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteCustomModel`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `DeleteCustomModel`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "DeleteCustomModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnDeleteCustomModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/customizations/{0}", customizationId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteCustomModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("TextToSpeechService.OnDeleteCustomModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Add custom words.
        ///
        /// Adds one or more words and their translations to the specified custom model. Adding a new translation for a
        /// word that already exists in a custom model overwrites the word's existing translation. A custom model can
        /// contain no more than 20,000 entries. You must use credentials for the instance of the service that owns a
        /// model to add words to it.
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
        /// **See also:**
        /// * [Adding multiple words to a custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customWords#cuWordsAdd)
        /// * [Adding words to a Japanese custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customWords#cuJapaneseAdd)
        /// * [Understanding
        /// customization](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customIntro#customIntro).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom model. You must make the request
        /// with credentials for the instance of the service that owns the custom model.</param>
        /// <param name="words">The **Add custom words** method accepts an array of `Word` objects. Each object provides
        /// a word that is to be added or updated for the custom model and the word's translation.
        ///
        /// The **List custom words** method returns an array of `Word` objects. Each object shows a word and its
        /// translation from the custom model. The words are listed in alphabetical order, with uppercase letters listed
        /// before lowercase letters. The array is empty if the custom model contains no words.</param>
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
        /// Lists all of the words and their translations for the specified custom model. The output shows the
        /// translations as they are defined in the model. You must use credentials for the instance of the service that
        /// owns a model to list its words.
        ///
        /// **See also:** [Querying all words from a custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customWords#cuWordsQueryModel).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom model. You must make the request
        /// with credentials for the instance of the service that owns the custom model.</param>
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
        /// Adds a single word and its translation to the specified custom model. Adding a new translation for a word
        /// that already exists in a custom model overwrites the word's existing translation. A custom model can contain
        /// no more than 20,000 entries. You must use credentials for the instance of the service that owns a model to
        /// add a word to it.
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
        /// **See also:**
        /// * [Adding a single word to a custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customWords#cuWordAdd)
        /// * [Adding words to a Japanese custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customWords#cuJapaneseAdd)
        /// * [Understanding
        /// customization](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customIntro#customIntro).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom model. You must make the request
        /// with credentials for the instance of the service that owns the custom model.</param>
        /// <param name="word">The word that is to be added or updated for the custom model.</param>
        /// <param name="translation">The phonetic or sounds-like translation for the word. A phonetic translation is
        /// based on the SSML format for representing the phonetic string of a word either as an IPA translation or as
        /// an IBM SPR translation. The Arabic, Chinese, Dutch, Australian English, and Korean languages support only
        /// IPA. A sounds-like is one or more words that, when combined, sound like the word.</param>
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
        /// **See also:** [Querying a single word from a custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customWords#cuWordQueryModel).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom model. You must make the request
        /// with credentials for the instance of the service that owns the custom model.</param>
        /// <param name="word">The word that is to be queried from the custom model.</param>
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
        /// Deletes a single word from the specified custom model. You must use credentials for the instance of the
        /// service that owns a model to delete its words.
        ///
        /// **See also:** [Deleting a word from a custom
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-customWords#cuWordDelete).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom model. You must make the request
        /// with credentials for the instance of the service that owns the custom model.</param>
        /// <param name="word">The word that is to be deleted from the custom model.</param>
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
        /// List custom prompts.
        ///
        /// Lists information about all custom prompts that are defined for a custom model. The information includes the
        /// prompt ID, prompt text, status, and optional speaker ID for each prompt of the custom model. You must use
        /// credentials for the instance of the service that owns the custom model. The same information about all of
        /// the prompts for a custom model is also provided by the **Get a custom model** method. That method provides
        /// complete details about a specified custom model, including its language, owner, custom words, and more.
        ///
        /// **Beta:** Custom prompts are beta functionality that is supported only for use with US English custom models
        /// and voices.
        ///
        /// **See also:** [Listing custom
        /// prompts](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-tbe-custom-prompts#tbe-custom-prompts-list).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom model. You must make the request
        /// with credentials for the instance of the service that owns the custom model.</param>
        /// <returns><see cref="Prompts" />Prompts</returns>
        public bool ListCustomPrompts(Callback<Prompts> callback, string customizationId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListCustomPrompts`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `ListCustomPrompts`");

            RequestObject<Prompts> req = new RequestObject<Prompts>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "ListCustomPrompts"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnListCustomPromptsResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/customizations/{0}/prompts", customizationId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListCustomPromptsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Prompts> response = new DetailedResponse<Prompts>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Prompts>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnListCustomPromptsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Prompts>)req).Callback != null)
                ((RequestObject<Prompts>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Add a custom prompt.
        ///
        /// Adds a custom prompt to a custom model. A prompt is defined by the text that is to be spoken, the audio for
        /// that text, a unique user-specified ID for the prompt, and an optional speaker ID. The information is used to
        /// generate prosodic data that is not visible to the user. This data is used by the service to produce the
        /// synthesized audio upon request. You must use credentials for the instance of the service that owns a custom
        /// model to add a prompt to it. You can add a maximum of 1000 custom prompts to a single custom model.
        ///
        /// You are recommended to assign meaningful values for prompt IDs. For example, use `goodbye` to identify a
        /// prompt that speaks a farewell message. Prompt IDs must be unique within a given custom model. You cannot
        /// define two prompts with the same name for the same custom model. If you provide the ID of an existing
        /// prompt, the previously uploaded prompt is replaced by the new information. The existing prompt is
        /// reprocessed by using the new text and audio and, if provided, new speaker model, and the prosody data
        /// associated with the prompt is updated.
        ///
        /// The quality of a prompt is undefined if the language of a prompt does not match the language of its custom
        /// model. This is consistent with any text or SSML that is specified for a speech synthesis request. The
        /// service makes a best-effort attempt to render the specified text for the prompt; it does not validate that
        /// the language of the text matches the language of the model.
        ///
        /// Adding a prompt is an asynchronous operation. Although it accepts less audio than speaker enrollment, the
        /// service must align the audio with the provided text. The time that it takes to process a prompt depends on
        /// the prompt itself. The processing time for a reasonably sized prompt generally matches the length of the
        /// audio (for example, it takes 20 seconds to process a 20-second prompt).
        ///
        /// For shorter prompts, you can wait for a reasonable amount of time and then check the status of the prompt
        /// with the **Get a custom prompt** method. For longer prompts, consider using that method to poll the service
        /// every few seconds to determine when the prompt becomes available. No prompt can be used for speech synthesis
        /// if it is in the `processing` or `failed` state. Only prompts that are in the `available` state can be used
        /// for speech synthesis.
        ///
        /// When it processes a request, the service attempts to align the text and the audio that are provided for the
        /// prompt. The text that is passed with a prompt must match the spoken audio as closely as possible. Optimally,
        /// the text and audio match exactly. The service does its best to align the specified text with the audio, and
        /// it can often compensate for mismatches between the two. But if the service cannot effectively align the text
        /// and the audio, possibly because the magnitude of mismatches between the two is too great, processing of the
        /// prompt fails.
        ///
        /// ### Evaluating a prompt
        ///
        ///  Always listen to and evaluate a prompt to determine its quality before using it in production. To evaluate
        /// a prompt, include only the single prompt in a speech synthesis request by using the following SSML
        /// extension, in this case for a prompt whose ID is `goodbye`:
        ///
        /// `<ibm:prompt id="goodbye"/>`
        ///
        /// In some cases, you might need to rerecord and resubmit a prompt as many as five times to address the
        /// following possible problems:
        /// * The service might fail to detect a mismatch between the prompt’s text and audio. The longer the prompt,
        /// the greater the chance for misalignment between its text and audio. Therefore, multiple shorter prompts are
        /// preferable to a single long prompt.
        /// * The text of a prompt might include a word that the service does not recognize. In this case, you can
        /// create a custom word and pronunciation pair to tell the service how to pronounce the word. You must then
        /// re-create the prompt.
        /// * The quality of the input audio might be insufficient or the service’s processing of the audio might fail
        /// to detect the intended prosody. Submitting new audio for the prompt can correct these issues.
        ///
        /// If a prompt that is created without a speaker ID does not adequately reflect the intended prosody, enrolling
        /// the speaker and providing a speaker ID for the prompt is one recommended means of potentially improving the
        /// quality of the prompt. This is especially important for shorter prompts such as "good-bye" or "thank you,"
        /// where less audio data makes it more difficult to match the prosody of the speaker.
        ///
        /// **Beta:** Custom prompts are beta functionality that is supported only for use with US English custom models
        /// and voices.
        ///
        /// **See also:**
        /// * [Add a custom
        /// prompt](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-tbe-create#tbe-create-add-prompt)
        /// * [Evaluate a custom
        /// prompt](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-tbe-create#tbe-create-evaluate-prompt)
        /// * [Rules for creating custom
        /// prompts](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-tbe-rules#tbe-rules-prompts).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom model. You must make the request
        /// with credentials for the instance of the service that owns the custom model.</param>
        /// <param name="promptId">The identifier of the prompt that is to be added to the custom model:
        /// * Include a maximum of 49 characters in the ID.
        /// * Include only alphanumeric characters and `_` (underscores) in the ID.
        /// * Do not include XML sensitive characters (double quotes, single quotes, ampersands, angle brackets, and
        /// slashes) in the ID.
        /// * To add a new prompt, the ID must be unique for the specified custom model. Otherwise, the new information
        /// for the prompt overwrites the existing prompt that has that ID.</param>
        /// <param name="metadata">Information about the prompt that is to be added to a custom model. The following
        /// example of a `PromptMetadata` object includes both the required prompt text and an optional speaker model
        /// ID:
        ///
        /// `{ "prompt_text": "Thank you and good-bye!", "speaker_id": "823068b2-ed4e-11ea-b6e0-7b6456aa95cc"
        /// }`.</param>
        /// <param name="file">An audio file that speaks the text of the prompt with intonation and prosody that matches
        /// how you would like the prompt to be spoken.
        /// * The prompt audio must be in WAV format and must have a minimum sampling rate of 16 kHz. The service
        /// accepts audio with higher sampling rates. The service transcodes all audio to 16 kHz before processing it.
        /// * The length of the prompt audio is limited to 30 seconds.</param>
        /// <returns><see cref="Prompt" />Prompt</returns>
        public bool AddCustomPrompt(Callback<Prompt> callback, string customizationId, string promptId, PromptMetadata metadata, System.IO.MemoryStream file)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `AddCustomPrompt`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `AddCustomPrompt`");
            if (string.IsNullOrEmpty(promptId))
                throw new ArgumentNullException("`promptId` is required for `AddCustomPrompt`");
            if (metadata == null)
                throw new ArgumentNullException("`metadata` is required for `AddCustomPrompt`");
            if (file == null)
                throw new ArgumentNullException("`file` is required for `AddCustomPrompt`");

            RequestObject<Prompt> req = new RequestObject<Prompt>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "AddCustomPrompt"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Forms = new Dictionary<string, RESTConnector.Form>();

            if (metadata != null)
            {
                byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(metadata));
                req.Forms["metadata"] = new RESTConnector.Form(new System.IO.MemoryStream(byteArray), "" ,"application/json");
            }
            if (file != null)
            {
                req.Forms["file"] = new RESTConnector.Form(file, "filename", "audio/wav");
            }

            req.OnResponse = OnAddCustomPromptResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/customizations/{0}/prompts/{1}", customizationId, promptId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnAddCustomPromptResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Prompt> response = new DetailedResponse<Prompt>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Prompt>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnAddCustomPromptResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Prompt>)req).Callback != null)
                ((RequestObject<Prompt>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Get a custom prompt.
        ///
        /// Gets information about a specified custom prompt for a specified custom model. The information includes the
        /// prompt ID, prompt text, status, and optional speaker ID for each prompt of the custom model. You must use
        /// credentials for the instance of the service that owns the custom model.
        ///
        /// **Beta:** Custom prompts are beta functionality that is supported only for use with US English custom models
        /// and voices.
        ///
        /// **See also:** [Listing custom
        /// prompts](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-tbe-custom-prompts#tbe-custom-prompts-list).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom model. You must make the request
        /// with credentials for the instance of the service that owns the custom model.</param>
        /// <param name="promptId">The identifier (name) of the prompt.</param>
        /// <returns><see cref="Prompt" />Prompt</returns>
        public bool GetCustomPrompt(Callback<Prompt> callback, string customizationId, string promptId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetCustomPrompt`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `GetCustomPrompt`");
            if (string.IsNullOrEmpty(promptId))
                throw new ArgumentNullException("`promptId` is required for `GetCustomPrompt`");

            RequestObject<Prompt> req = new RequestObject<Prompt>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "GetCustomPrompt"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnGetCustomPromptResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/customizations/{0}/prompts/{1}", customizationId, promptId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetCustomPromptResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Prompt> response = new DetailedResponse<Prompt>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Prompt>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnGetCustomPromptResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Prompt>)req).Callback != null)
                ((RequestObject<Prompt>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Delete a custom prompt.
        ///
        /// Deletes an existing custom prompt from a custom model. The service deletes the prompt with the specified ID.
        /// You must use credentials for the instance of the service that owns the custom model from which the prompt is
        /// to be deleted.
        ///
        /// **Caution:** Deleting a custom prompt elicits a 400 response code from synthesis requests that attempt to
        /// use the prompt. Make sure that you do not attempt to use a deleted prompt in a production application.
        ///
        /// **Beta:** Custom prompts are beta functionality that is supported only for use with US English custom models
        /// and voices.
        ///
        /// **See also:** [Deleting a custom
        /// prompt](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-tbe-custom-prompts#tbe-custom-prompts-delete).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom model. You must make the request
        /// with credentials for the instance of the service that owns the custom model.</param>
        /// <param name="promptId">The identifier (name) of the prompt that is to be deleted.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteCustomPrompt(Callback<object> callback, string customizationId, string promptId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteCustomPrompt`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `DeleteCustomPrompt`");
            if (string.IsNullOrEmpty(promptId))
                throw new ArgumentNullException("`promptId` is required for `DeleteCustomPrompt`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "DeleteCustomPrompt"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnDeleteCustomPromptResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/customizations/{0}/prompts/{1}", customizationId, promptId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteCustomPromptResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("TextToSpeechService.OnDeleteCustomPromptResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// List speaker models.
        ///
        /// Lists information about all speaker models that are defined for a service instance. The information includes
        /// the speaker ID and speaker name of each defined speaker. You must use credentials for the instance of a
        /// service to list its speakers.
        ///
        /// **Beta:** Speaker models and the custom prompts with which they are used are beta functionality that is
        /// supported only for use with US English custom models and voices.
        ///
        /// **See also:** [Listing speaker
        /// models](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-tbe-speaker-models#tbe-speaker-models-list).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <returns><see cref="Speakers" />Speakers</returns>
        public bool ListSpeakerModels(Callback<Speakers> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListSpeakerModels`");

            RequestObject<Speakers> req = new RequestObject<Speakers>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "ListSpeakerModels"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnListSpeakerModelsResponse;

            Connector.URL = GetServiceUrl() + "/v1/speakers";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListSpeakerModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Speakers> response = new DetailedResponse<Speakers>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Speakers>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnListSpeakerModelsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Speakers>)req).Callback != null)
                ((RequestObject<Speakers>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Create a speaker model.
        ///
        /// Creates a new speaker model, which is an optional enrollment token for users who are to add prompts to
        /// custom models. A speaker model contains information about a user's voice. The service extracts this
        /// information from a WAV audio sample that you pass as the body of the request. Associating a speaker model
        /// with a prompt is optional, but the information that is extracted from the speaker model helps the service
        /// learn about the speaker's voice.
        ///
        /// A speaker model can make an appreciable difference in the quality of prompts, especially short prompts with
        /// relatively little audio, that are associated with that speaker. A speaker model can help the service produce
        /// a prompt with more confidence; the lack of a speaker model can potentially compromise the quality of a
        /// prompt.
        ///
        /// The gender of the speaker who creates a speaker model does not need to match the gender of a voice that is
        /// used with prompts that are associated with that speaker model. For example, a speaker model that is created
        /// by a male speaker can be associated with prompts that are spoken by female voices.
        ///
        /// You create a speaker model for a given instance of the service. The new speaker model is owned by the
        /// service instance whose credentials are used to create it. That same speaker can then be used to create
        /// prompts for all custom models within that service instance. No language is associated with a speaker model,
        /// but each custom model has a single specified language. You can add prompts only to US English models.
        ///
        /// You specify a name for the speaker when you create it. The name must be unique among all speaker names for
        /// the owning service instance. To re-create a speaker model for an existing speaker name, you must first
        /// delete the existing speaker model that has that name.
        ///
        /// Speaker enrollment is a synchronous operation. Although it accepts more audio data than a prompt, the
        /// process of adding a speaker is very fast. The service simply extracts information about the speaker’s voice
        /// from the audio. Unlike prompts, speaker models neither need nor accept a transcription of the audio. When
        /// the call returns, the audio is fully processed and the speaker enrollment is complete.
        ///
        /// The service returns a speaker ID with the request. A speaker ID is globally unique identifier (GUID) that
        /// you use to identify the speaker in subsequent requests to the service.
        ///
        /// **Beta:** Speaker models and the custom prompts with which they are used are beta functionality that is
        /// supported only for use with US English custom models and voices.
        ///
        /// **See also:**
        /// * [Create a speaker
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-tbe-create#tbe-create-speaker-model)
        /// * [Rules for creating speaker
        /// models](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-tbe-rules#tbe-rules-speakers).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="speakerName">The name of the speaker that is to be added to the service instance.
        /// * Include a maximum of 49 characters in the name.
        /// * Include only alphanumeric characters and `_` (underscores) in the name.
        /// * Do not include XML sensitive characters (double quotes, single quotes, ampersands, angle brackets, and
        /// slashes) in the name.
        /// * Do not use the name of an existing speaker that is already defined for the service instance.</param>
        /// <param name="audio">An enrollment audio file that contains a sample of the speaker’s voice.
        /// * The enrollment audio must be in WAV format and must have a minimum sampling rate of 16 kHz. The service
        /// accepts audio with higher sampling rates. It transcodes all audio to 16 kHz before processing it.
        /// * The length of the enrollment audio is limited to 1 minute. Speaking one or two paragraphs of text that
        /// include five to ten sentences is recommended.</param>
        /// <returns><see cref="SpeakerModel" />SpeakerModel</returns>
        public bool CreateSpeakerModel(Callback<SpeakerModel> callback, string speakerName, System.IO.MemoryStream audio)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateSpeakerModel`");
            if (string.IsNullOrEmpty(speakerName))
                throw new ArgumentNullException("`speakerName` is required for `CreateSpeakerModel`");
            if (audio == null)
                throw new ArgumentNullException("`audio` is required for `CreateSpeakerModel`");

            RequestObject<SpeakerModel> req = new RequestObject<SpeakerModel>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "CreateSpeakerModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(speakerName))
            {
                req.Parameters["speaker_name"] = speakerName;
            }
            req.Headers["Content-Type"] = "audio/wav";
            req.Headers["Accept"] = "application/json";
            req.Send = audio.ToArray();

            req.OnResponse = OnCreateSpeakerModelResponse;

            Connector.URL = GetServiceUrl() + "/v1/speakers";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateSpeakerModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<SpeakerModel> response = new DetailedResponse<SpeakerModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<SpeakerModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnCreateSpeakerModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<SpeakerModel>)req).Callback != null)
                ((RequestObject<SpeakerModel>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Get a speaker model.
        ///
        /// Gets information about all prompts that are defined by a specified speaker for all custom models that are
        /// owned by a service instance. The information is grouped by the customization IDs of the custom models. For
        /// each custom model, the information lists information about each prompt that is defined for that custom model
        /// by the speaker. You must use credentials for the instance of the service that owns a speaker model to list
        /// its prompts.
        ///
        /// **Beta:** Speaker models and the custom prompts with which they are used are beta functionality that is
        /// supported only for use with US English custom models and voices.
        ///
        /// **See also:** [Listing the custom prompts for a speaker
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-tbe-speaker-models#tbe-speaker-models-list-prompts).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="speakerId">The speaker ID (GUID) of the speaker model. You must make the request with service
        /// credentials for the instance of the service that owns the speaker model.</param>
        /// <returns><see cref="SpeakerCustomModels" />SpeakerCustomModels</returns>
        public bool GetSpeakerModel(Callback<SpeakerCustomModels> callback, string speakerId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetSpeakerModel`");
            if (string.IsNullOrEmpty(speakerId))
                throw new ArgumentNullException("`speakerId` is required for `GetSpeakerModel`");

            RequestObject<SpeakerCustomModels> req = new RequestObject<SpeakerCustomModels>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "GetSpeakerModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnGetSpeakerModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/speakers/{0}", speakerId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetSpeakerModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<SpeakerCustomModels> response = new DetailedResponse<SpeakerCustomModels>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<SpeakerCustomModels>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnGetSpeakerModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<SpeakerCustomModels>)req).Callback != null)
                ((RequestObject<SpeakerCustomModels>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Delete a speaker model.
        ///
        /// Deletes an existing speaker model from the service instance. The service deletes the enrolled speaker with
        /// the specified speaker ID. You must use credentials for the instance of the service that owns a speaker model
        /// to delete the speaker.
        ///
        /// Any prompts that are associated with the deleted speaker are not affected by the speaker's deletion. The
        /// prosodic data that defines the quality of a prompt is established when the prompt is created. A prompt is
        /// static and remains unaffected by deletion of its associated speaker. However, the prompt cannot be
        /// resubmitted or updated with its original speaker once that speaker is deleted.
        ///
        /// **Beta:** Speaker models and the custom prompts with which they are used are beta functionality that is
        /// supported only for use with US English custom models and voices.
        ///
        /// **See also:** [Deleting a speaker
        /// model](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-tbe-speaker-models#tbe-speaker-models-delete).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="speakerId">The speaker ID (GUID) of the speaker model. You must make the request with service
        /// credentials for the instance of the service that owns the speaker model.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteSpeakerModel(Callback<object> callback, string speakerId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteSpeakerModel`");
            if (string.IsNullOrEmpty(speakerId))
                throw new ArgumentNullException("`speakerId` is required for `DeleteSpeakerModel`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("text_to_speech", "V1", "DeleteSpeakerModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnDeleteSpeakerModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/speakers/{0}", speakerId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteSpeakerModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("TextToSpeechService.OnDeleteSpeakerModelResponse()", "Exception: {0}", e.ToString());
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
        /// of the service that was used to associate the customer ID with the data. You associate a customer ID with
        /// data by passing the `X-Watson-Metadata` header with a request that passes the data.
        ///
        /// **Note:** If you delete an instance of the service from the service console, all data associated with that
        /// service instance is automatically deleted. This includes all custom models and word/translation pairs, and
        /// all data related to speech synthesis requests.
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