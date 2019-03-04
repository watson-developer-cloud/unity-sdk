/**
* Copyright 2018, 2019 IBM Corp. All Rights Reserved.
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
using IBM.Cloud.SDK.Connection;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.TextToSpeech.V1.Model;
using Newtonsoft.Json;
using System;
using UnityEngine.Networking;

namespace IBM.Watson.TextToSpeech.V1
{
    public class TextToSpeechService : BaseService
    {
        private const string serviceId = "text_to_speech";
        private const string defaultUrl = "https://stream.watsonplatform.net/text-to-speech/api";

        #region Credentials
        /// <summary>
        /// Gets and sets the credentials of the service. Replace the default endpoint if endpoint is defined.
        /// </summary>
        public Credentials Credentials
        {
            get { return credentials; }
            set
            {
                credentials = value;
                if (!string.IsNullOrEmpty(credentials.Url))
                {
                    Url = credentials.Url;
                }
            }
        }
        #endregion

        #region Url
        /// <summary>
        /// Gets and sets the endpoint URL for the service.
        /// </summary>
        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        #endregion

        #region VersionDate
        #endregion

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
        
        public TextToSpeechService() : base(serviceId)
        {
            
        }

        /// <summary>
        /// TextToSpeechService constructor.
        /// </summary>
        
        /// <param name="credentials">The service credentials.</param>
        public TextToSpeechService(Credentials credentials) : base(credentials, serviceId)
        {
            if (credentials.HasCredentials() || credentials.HasIamTokenData())
            {
                Credentials = credentials;

                if (string.IsNullOrEmpty(credentials.Url))
                {
                    credentials.Url = defaultUrl;
                }
            }
            else
            {
                throw new IBMException("Please provide a username and password or authorization token to use the TextToSpeech service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }

        /// <summary>
        /// Get a voice.
        ///
        /// Gets information about the specified voice. The information includes the name, language, gender, and other
        /// details about the voice. Specify a customization ID to obtain information for that custom voice model of the
        /// specified voice. To list information about all available voices, use the **List voices** method.
        ///
        /// **See also:** [Specifying a voice](https://cloud.ibm.com/docs/services/text-to-speech/http.html#voices).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="voice">The voice for which information is to be returned.</param>
        /// <param name="customizationId">The customization ID (GUID) of a custom voice model for which information is
        /// to be returned. You must make the request with service credentials created for the instance of the service
        /// that owns the custom model. Omit the parameter to see information about the specified voice with no
        /// customization. (optional)</param>
        /// <returns><see cref="Voice" />Voice</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetVoice(Callback<Voice> callback, string voice, string customizationId = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetVoice");
            if (string.IsNullOrEmpty(voice))
                throw new ArgumentNullException("voice is required for GetVoice");

            RequestObject<Voice> req = new RequestObject<Voice>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=text_to_speech;service_version=V1;operation_id=GetVoice";
            if (!string.IsNullOrEmpty(customizationId))
            {
                req.Parameters["customization_id"] = customizationId;
            }

            req.OnResponse = OnGetVoiceResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/voices/{0}", voice));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetVoiceResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Voice> response = new DetailedResponse<Voice>();
            Dictionary<string, object> customData = ((RequestObject<Voice>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Voice>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnGetVoiceResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Voice>)req).Callback != null)
                ((RequestObject<Voice>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// List voices.
        ///
        /// Lists all voices available for use with the service. The information includes the name, language, gender,
        /// and other details about the voice. To see information about a specific voice, use the **Get a voice**
        /// method.
        ///
        /// **See also:** [Specifying a voice](https://cloud.ibm.com/docs/services/text-to-speech/http.html#voices).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <returns><see cref="Voices" />Voices</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListVoices(Callback<Voices> callback, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListVoices");

            RequestObject<Voices> req = new RequestObject<Voices>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=text_to_speech;service_version=V1;operation_id=ListVoices";

            req.OnResponse = OnListVoicesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/voices");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListVoicesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Voices> response = new DetailedResponse<Voices>();
            Dictionary<string, object> customData = ((RequestObject<Voices>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Voices>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnListVoicesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Voices>)req).Callback != null)
                ((RequestObject<Voices>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Synthesize audio.
        ///
        /// Synthesizes text to audio that is spoken in the specified voice. The service bases its understanding of the
        /// language for the input text on the specified voice. Use a voice that matches the language of the input text.
        ///
        ///
        /// The service returns the synthesized audio stream as an array of bytes. You can pass a maximum of 5 KB of
        /// text to the service.
        ///
        /// **See also:** [Synthesizing text to
        /// audio](https://cloud.ibm.com/docs/services/text-to-speech/http.html#synthesize).
        ///
        /// ### Audio formats (accept types)
        ///
        ///  The service can return audio in the following formats (MIME types).
        /// * Where indicated, you can optionally specify the sampling rate (`rate`) of the audio. You must specify a
        /// sampling rate for the `audio/l16` and `audio/mulaw` formats. A specified sampling rate must lie in the range
        /// of 8 kHz to 192 kHz.
        /// * For the `audio/l16` format, you can optionally specify the endianness (`endianness`) of the audio:
        /// `endianness=big-endian` or `endianness=little-endian`.
        ///
        /// Use the `Accept` header or the `accept` parameter to specify the requested format of the response audio. If
        /// you omit an audio format altogether, the service returns the audio in Ogg format with the Opus codec
        /// (`audio/ogg;codecs=opus`). The service always returns single-channel audio.
        /// * `audio/basic`
        ///
        ///   The service returns audio with a sampling rate of 8000 Hz.
        /// * `audio/flac`
        ///
        ///   You can optionally specify the `rate` of the audio. The default sampling rate is 22,050 Hz.
        /// * `audio/l16`
        ///
        ///   You must specify the `rate` of the audio. You can optionally specify the `endianness` of the audio. The
        /// default endianness is `little-endian`.
        /// * `audio/mp3`
        ///
        ///   You can optionally specify the `rate` of the audio. The default sampling rate is 22,050 Hz.
        /// * `audio/mpeg`
        ///
        ///   You can optionally specify the `rate` of the audio. The default sampling rate is 22,050 Hz.
        /// * `audio/mulaw`
        ///
        ///   You must specify the `rate` of the audio.
        /// * `audio/ogg`
        ///
        ///   The service returns the audio in the `vorbis` codec. You can optionally specify the `rate` of the audio.
        /// The default sampling rate is 22,050 Hz.
        /// * `audio/ogg;codecs=opus`
        ///
        ///   You can optionally specify the `rate` of the audio. The default sampling rate is 22,050 Hz.
        /// * `audio/ogg;codecs=vorbis`
        ///
        ///   You can optionally specify the `rate` of the audio. The default sampling rate is 22,050 Hz.
        /// * `audio/wav`
        ///
        ///   You can optionally specify the `rate` of the audio. The default sampling rate is 22,050 Hz.
        /// * `audio/webm`
        ///
        ///   The service returns the audio in the `opus` codec. The service returns audio with a sampling rate of
        /// 48,000 Hz.
        /// * `audio/webm;codecs=opus`
        ///
        ///   The service returns audio with a sampling rate of 48,000 Hz.
        /// * `audio/webm;codecs=vorbis`
        ///
        ///   You can optionally specify the `rate` of the audio. The default sampling rate is 22,050 Hz.
        ///
        /// For more information about specifying an audio format, including additional details about some of the
        /// formats, see [Specifying an audio
        /// format](https://cloud.ibm.com/docs/services/text-to-speech/http.html#format).
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
        /// <param name="text">A `Text` object that provides the text to synthesize. Specify either plain text or a
        /// subset of SSML. SSML is an XML-based markup language that provides text annotation for speech-synthesis
        /// applications. Pass a maximum of 5 KB of text.</param>
        /// <param name="voice">The voice to use for synthesis. (optional, default to en-US_MichaelVoice)</param>
        /// <param name="customizationId">The customization ID (GUID) of a custom voice model to use for the synthesis.
        /// If a custom voice model is specified, it is guaranteed to work only if it matches the language of the
        /// indicated voice. You must make the request with service credentials created for the instance of the service
        /// that owns the custom model. Omit the parameter to use the specified voice with no customization.
        /// (optional)</param>
        /// <param name="accept">The requested format (MIME type) of the audio. You can use the `Accept` header or the
        /// `accept` parameter to specify the audio format. For more information about specifying an audio format, see
        /// **Audio formats (accept types)** in the method description.
        ///
        /// Default: `audio/ogg;codecs=opus`. (optional)</param>
        /// <returns><see cref="byte[]" />byte[]</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool Synthesize(Callback<byte[]> callback, Text text, string voice = null, string customizationId = null, string accept = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for Synthesize");
            if (text == null)
                throw new ArgumentNullException("text is required for Synthesize");

            RequestObject<byte[]> req = new RequestObject<byte[]>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=text_to_speech;service_version=V1;operation_id=Synthesize";
            if (!string.IsNullOrEmpty(accept))
            {
                req.Headers["Accept"] = accept;
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
            if (text != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(text));
            }

            req.OnResponse = OnSynthesizeResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/synthesize");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnSynthesizeResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<byte[]> response = new DetailedResponse<byte[]>();
            Dictionary<string, object> customData = ((RequestObject<byte[]>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<byte[]>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnSynthesizeResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<byte[]>)req).Callback != null)
                ((RequestObject<byte[]>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Get pronunciation.
        ///
        /// Gets the phonetic pronunciation for the specified word. You can request the pronunciation for a specific
        /// format. You can also request the pronunciation for a specific voice to see the default translation for the
        /// language of that voice or for a specific custom voice model to see the translation for that voice model.
        ///
        /// **Note:** This method is currently a beta release.
        ///
        /// **See also:** [Querying a word from a
        /// language](https://cloud.ibm.com/docs/services/text-to-speech/custom-entries.html#cuWordsQueryLanguage).
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
        /// translation for the custom model's language. You must make the request with service credentials created for
        /// the instance of the service that owns the custom model. Omit the parameter to see the translation for the
        /// specified voice with no customization. (optional)</param>
        /// <returns><see cref="Pronunciation" />Pronunciation</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetPronunciation(Callback<Pronunciation> callback, string text, string voice = null, string format = null, string customizationId = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetPronunciation");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text is required for GetPronunciation");

            RequestObject<Pronunciation> req = new RequestObject<Pronunciation>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=text_to_speech;service_version=V1;operation_id=GetPronunciation";
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

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/pronunciation");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetPronunciationResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Pronunciation> response = new DetailedResponse<Pronunciation>();
            Dictionary<string, object> customData = ((RequestObject<Pronunciation>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Pronunciation>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnGetPronunciationResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Pronunciation>)req).Callback != null)
                ((RequestObject<Pronunciation>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Create a custom model.
        ///
        /// Creates a new empty custom voice model. You must specify a name for the new custom model. You can optionally
        /// specify the language and a description for the new model. The model is owned by the instance of the service
        /// whose credentials are used to create it.
        ///
        /// **Note:** This method is currently a beta release.
        ///
        /// **See also:** [Creating a custom
        /// model](https://cloud.ibm.com/docs/services/text-to-speech/custom-models.html#cuModelsCreate).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="createVoiceModel">A `CreateVoiceModel` object that contains information about the new custom
        /// voice model.</param>
        /// <returns><see cref="VoiceModel" />VoiceModel</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateVoiceModel(Callback<VoiceModel> callback, CreateVoiceModel createVoiceModel, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateVoiceModel");
            if (createVoiceModel == null)
                throw new ArgumentNullException("createVoiceModel is required for CreateVoiceModel");

            RequestObject<VoiceModel> req = new RequestObject<VoiceModel>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=text_to_speech;service_version=V1;operation_id=CreateVoiceModel";
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (createVoiceModel != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(createVoiceModel));
            }

            req.OnResponse = OnCreateVoiceModelResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/customizations");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateVoiceModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<VoiceModel> response = new DetailedResponse<VoiceModel>();
            Dictionary<string, object> customData = ((RequestObject<VoiceModel>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<VoiceModel>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnCreateVoiceModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<VoiceModel>)req).Callback != null)
                ((RequestObject<VoiceModel>)req).Callback(response, resp.Error, customData);
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
        /// model](https://cloud.ibm.com/docs/services/text-to-speech/custom-models.html#cuModelsDelete).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom voice model. You must make the
        /// request with service credentials created for the instance of the service that owns the custom model.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteVoiceModel(Callback<object> callback, string customizationId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteVoiceModel");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("customizationId is required for DeleteVoiceModel");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=text_to_speech;service_version=V1;operation_id=DeleteVoiceModel";

            req.OnResponse = OnDeleteVoiceModelResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteVoiceModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            Dictionary<string, object> customData = ((RequestObject<object>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnDeleteVoiceModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
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
        /// model](https://cloud.ibm.com/docs/services/text-to-speech/custom-models.html#cuModelsQuery).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom voice model. You must make the
        /// request with service credentials created for the instance of the service that owns the custom model.</param>
        /// <returns><see cref="VoiceModel" />VoiceModel</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetVoiceModel(Callback<VoiceModel> callback, string customizationId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetVoiceModel");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("customizationId is required for GetVoiceModel");

            RequestObject<VoiceModel> req = new RequestObject<VoiceModel>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=text_to_speech;service_version=V1;operation_id=GetVoiceModel";

            req.OnResponse = OnGetVoiceModelResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetVoiceModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<VoiceModel> response = new DetailedResponse<VoiceModel>();
            Dictionary<string, object> customData = ((RequestObject<VoiceModel>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<VoiceModel>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnGetVoiceModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<VoiceModel>)req).Callback != null)
                ((RequestObject<VoiceModel>)req).Callback(response, resp.Error, customData);
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
        /// models](https://cloud.ibm.com/docs/services/text-to-speech/custom-models.html#cuModelsQueryAll).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="language">The language for which custom voice models that are owned by the requesting service
        /// credentials are to be returned. Omit the parameter to see all custom voice models that are owned by the
        /// requester. (optional)</param>
        /// <returns><see cref="VoiceModels" />VoiceModels</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListVoiceModels(Callback<VoiceModels> callback, string language = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListVoiceModels");

            RequestObject<VoiceModels> req = new RequestObject<VoiceModels>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=text_to_speech;service_version=V1;operation_id=ListVoiceModels";
            if (!string.IsNullOrEmpty(language))
            {
                req.Parameters["language"] = language;
            }

            req.OnResponse = OnListVoiceModelsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/customizations");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListVoiceModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<VoiceModels> response = new DetailedResponse<VoiceModels>();
            Dictionary<string, object> customData = ((RequestObject<VoiceModels>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<VoiceModels>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnListVoiceModelsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<VoiceModels>)req).Callback != null)
                ((RequestObject<VoiceModels>)req).Callback(response, resp.Error, customData);
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
        /// model](https://cloud.ibm.com/docs/services/text-to-speech/custom-models.html#cuModelsUpdate)
        /// * [Adding words to a Japanese custom
        /// model](https://cloud.ibm.com/docs/services/text-to-speech/custom-entries.html#cuJapaneseAdd)
        /// * [Understanding customization](https://cloud.ibm.com/docs/services/text-to-speech/custom-intro.html).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom voice model. You must make the
        /// request with service credentials created for the instance of the service that owns the custom model.</param>
        /// <param name="updateVoiceModel">An `UpdateVoiceModel` object that contains information that is to be updated
        /// for the custom voice model.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool UpdateVoiceModel(Callback<object> callback, string customizationId, UpdateVoiceModel updateVoiceModel, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for UpdateVoiceModel");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("customizationId is required for UpdateVoiceModel");
            if (updateVoiceModel == null)
                throw new ArgumentNullException("updateVoiceModel is required for UpdateVoiceModel");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=text_to_speech;service_version=V1;operation_id=UpdateVoiceModel";
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (updateVoiceModel != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(updateVoiceModel));
            }

            req.OnResponse = OnUpdateVoiceModelResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnUpdateVoiceModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            Dictionary<string, object> customData = ((RequestObject<object>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnUpdateVoiceModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
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
        /// model](https://cloud.ibm.com/docs/services/text-to-speech/custom-entries.html#cuWordAdd)
        /// * [Adding words to a Japanese custom
        /// model](https://cloud.ibm.com/docs/services/text-to-speech/custom-entries.html#cuJapaneseAdd)
        /// * [Understanding customization](https://cloud.ibm.com/docs/services/text-to-speech/custom-intro.html).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom voice model. You must make the
        /// request with service credentials created for the instance of the service that owns the custom model.</param>
        /// <param name="word">The word that is to be added or updated for the custom voice model.</param>
        /// <param name="translation">The translation for the word that is to be added or updated.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool AddWord(Callback<object> callback, string customizationId, string word, Translation translation, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for AddWord");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("customizationId is required for AddWord");
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("word is required for AddWord");
            if (translation == null)
                throw new ArgumentNullException("translation is required for AddWord");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPUT,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=text_to_speech;service_version=V1;operation_id=AddWord";
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "*/*";
            if (translation != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(translation));
            }

            req.OnResponse = OnAddWordResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/words/{1}", customizationId, word));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnAddWordResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            Dictionary<string, object> customData = ((RequestObject<object>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnAddWordResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
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
        /// model](https://cloud.ibm.com/docs/services/text-to-speech/custom-entries.html#cuWordsAdd)
        /// * [Adding words to a Japanese custom
        /// model](https://cloud.ibm.com/docs/services/text-to-speech/custom-entries.html#cuJapaneseAdd)
        /// * [Understanding customization](https://cloud.ibm.com/docs/services/text-to-speech/custom-intro.html).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom voice model. You must make the
        /// request with service credentials created for the instance of the service that owns the custom model.</param>
        /// <param name="customWords">A `Words` object that provides one or more words that are to be added or updated
        /// for the custom voice model and the translation for each specified word.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool AddWords(Callback<object> callback, string customizationId, Words customWords, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for AddWords");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("customizationId is required for AddWords");
            if (customWords == null)
                throw new ArgumentNullException("customWords is required for AddWords");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=text_to_speech;service_version=V1;operation_id=AddWords";
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (customWords != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(customWords));
            }

            req.OnResponse = OnAddWordsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/words", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnAddWordsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            Dictionary<string, object> customData = ((RequestObject<object>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnAddWordsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
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
        /// model](https://cloud.ibm.com/docs/services/text-to-speech/custom-entries.html#cuWordDelete).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom voice model. You must make the
        /// request with service credentials created for the instance of the service that owns the custom model.</param>
        /// <param name="word">The word that is to be deleted from the custom voice model.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteWord(Callback<object> callback, string customizationId, string word, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteWord");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("customizationId is required for DeleteWord");
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("word is required for DeleteWord");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=text_to_speech;service_version=V1;operation_id=DeleteWord";

            req.OnResponse = OnDeleteWordResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/words/{1}", customizationId, word));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteWordResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            Dictionary<string, object> customData = ((RequestObject<object>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnDeleteWordResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
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
        /// model](https://cloud.ibm.com/docs/services/text-to-speech/custom-entries.html#cuWordQueryModel).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom voice model. You must make the
        /// request with service credentials created for the instance of the service that owns the custom model.</param>
        /// <param name="word">The word that is to be queried from the custom voice model.</param>
        /// <returns><see cref="Translation" />Translation</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetWord(Callback<Translation> callback, string customizationId, string word, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetWord");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("customizationId is required for GetWord");
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("word is required for GetWord");

            RequestObject<Translation> req = new RequestObject<Translation>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=text_to_speech;service_version=V1;operation_id=GetWord";

            req.OnResponse = OnGetWordResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/words/{1}", customizationId, word));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetWordResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Translation> response = new DetailedResponse<Translation>();
            Dictionary<string, object> customData = ((RequestObject<Translation>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Translation>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnGetWordResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Translation>)req).Callback != null)
                ((RequestObject<Translation>)req).Callback(response, resp.Error, customData);
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
        /// model](https://cloud.ibm.com/docs/services/text-to-speech/custom-entries.html#cuWordsQueryModel).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom voice model. You must make the
        /// request with service credentials created for the instance of the service that owns the custom model.</param>
        /// <returns><see cref="Words" />Words</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListWords(Callback<Words> callback, string customizationId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListWords");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("customizationId is required for ListWords");

            RequestObject<Words> req = new RequestObject<Words>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=text_to_speech;service_version=V1;operation_id=ListWords";

            req.OnResponse = OnListWordsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/words", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListWordsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Words> response = new DetailedResponse<Words>();
            Dictionary<string, object> customData = ((RequestObject<Words>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Words>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnListWordsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Words>)req).Callback != null)
                ((RequestObject<Words>)req).Callback(response, resp.Error, customData);
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
        /// security](https://cloud.ibm.com/docs/services/text-to-speech/information-security.html).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customerId">The customer ID for which all data is to be deleted.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteUserData(Callback<object> callback, string customerId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteUserData");
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException("customerId is required for DeleteUserData");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=text_to_speech;service_version=V1;operation_id=DeleteUserData";
            if (!string.IsNullOrEmpty(customerId))
            {
                req.Parameters["customer_id"] = customerId;
            }

            req.OnResponse = OnDeleteUserDataResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/user_data");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteUserDataResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            Dictionary<string, object> customData = ((RequestObject<object>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("TextToSpeechService.OnDeleteUserDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
        }
    }
}