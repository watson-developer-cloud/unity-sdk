/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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
using FullSerializer;
using IBM.Watson.Connection;
using IBM.Watson.ToneAnalyzer.v3.Model;
using IBM.WatsonDeveloperCloud.Http;
using IBM.WatsonDeveloperCloud.Service;
using IBM.WatsonDeveloperCloud.Util;
using System;

namespace IBM.Watson.ToneAnalyzer.v3
{
    public class ToneAnalyzerService : IWatsonService
    {
        private const string ServiceId = "ToneAnalyzerServicev3";
        private fsSerializer _serializer = new fsSerializer();

        private Credentials _credentials = null;
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

        private string _url  = "https://gateway.watsonplatform.net/tone-analyzer/api";
        /// <summary>
        /// Gets and sets the endpoint URL for the service.
        /// </summary>
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        private string _versionDate;
        /// <summary>
        /// Gets and sets the versionDate of the service.
        /// </summary>
        public string VersionDate
        {
            get { return _versionDate; }
            set { _versionDate = value; }
        }

        /// <summary>
        /// ToneAnalyzerService constructor.
        /// </summary>
        /// <param name="credentials">The service credentials.</param>
        public ToneAnalyzerService(Credentials credentials)
        {
            if (credentials.HasCredentials() || credentials.HasIamTokenData())
            {
                Credentials = credentials;

                if (string.IsNullOrEmpty(credentials.Url))
                {
                    credentials.Url = Url;
                }
            }
            else
            {
                throw new WatsonException("Please provide a username and password or authorization token to use the ToneAnalyzerService service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }

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

        /// <summary>
        /// Analyze general tone.
        ///
        /// Use the general purpose endpoint to analyze the tone of your input content. The service analyzes the content
        /// for emotional and language tones. The method always analyzes the tone of the full document; by default, it
        /// also analyzes the tone of each individual sentence of the content.
        ///
        /// You can submit no more than 128 KB of total input content and no more than 1000 individual sentences in
        /// JSON, plain text, or HTML format. The service analyzes the first 1000 sentences for document-level analysis
        /// and only the first 100 sentences for sentence-level analysis.
        ///
        /// Per the JSON specification, the default character encoding for JSON content is effectively always UTF-8; per
        /// the HTTP specification, the default encoding for plain text and HTML is ISO-8859-1 (effectively, the ASCII
        /// character set). When specifying a content type of plain text or HTML, include the `charset` parameter to
        /// indicate the character encoding of the input text; for example: `Content-Type: text/plain;charset=utf-8`.
        /// For `text/html`, the service removes HTML tags and analyzes only the textual content.
        ///
        /// **See also:** [Using the general-purpose
        /// endpoint](https://cloud.ibm.com/docs/services/tone-analyzer/using-tone.html#using-the-general-purpose-endpoint).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="toneInput">JSON, plain text, or HTML input that contains the content to be analyzed. For JSON
        /// input, provide an object of type `ToneInput`.</param>
        /// <param name="sentences">Indicates whether the service is to return an analysis of each individual sentence
        /// in addition to its analysis of the full document. If `true` (the default), the service returns results for
        /// each sentence. (optional, default to true)</param>
        /// <param name="tones">**`2017-09-21`:** Deprecated. The service continues to accept the parameter for
        /// backward-compatibility, but the parameter no longer affects the response.
        ///
        /// **`2016-05-19`:** A comma-separated list of tones for which the service is to return its analysis of the
        /// input; the indicated tones apply both to the full document and to individual sentences of the document. You
        /// can specify one or more of the valid values. Omit the parameter to request results for all three tones.
        /// (optional)</param>
        /// <param name="contentLanguage">The language of the input text for the request: English or French. Regional
        /// variants are treated as their parent language; for example, `en-US` is interpreted as `en`. The input
        /// content must match the specified language. Do not submit content that contains both languages. You can use
        /// different languages for **Content-Language** and **Accept-Language**.
        /// * **`2017-09-21`:** Accepts `en` or `fr`.
        /// * **`2016-05-19`:** Accepts only `en`. (optional, default to en)</param>
        /// <param name="acceptLanguage">The desired language of the response. For two-character arguments, regional
        /// variants are treated as their parent language; for example, `en-US` is interpreted as `en`. You can use
        /// different languages for **Content-Language** and **Accept-Language**. (optional, default to en)</param>
        /// <param name="contentType">The type of the input. A character encoding can be specified by including a
        /// `charset` parameter. For example, 'text/plain;charset=utf-8'. (optional)</param>
        /// <returns><see cref="ToneAnalysis" />ToneAnalysis</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool Tone(SuccessCallback<ToneAnalysis> successCallback, FailCallback failCallback, ToneInput toneInput, bool? sentences = null, List<string> tones = null, string contentLanguage = null, string acceptLanguage = null, string contentType = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            ToneRequestObj req = new ToneRequestObj();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if(req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach(KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Parameters["version"] = VersionDate;
            if(!string.IsNullOrEmpty(contentLanguage))
            request.Headers["Content-Language"] = contentLanguage;
            if(!string.IsNullOrEmpty(acceptLanguage))
            request.Headers["Accept-Language"] = acceptLanguage;
            if(!string.IsNullOrEmpty(contentType))
            request.Headers["Content-Type"] = contentType;
            if (sentences != null)
            req.Parameters["sentences"] = sentences;
            req.Parameters["tones"] = tones != null && tones.Count > 0 ? string.Join(",", tones.ToArray()) : null;
            req.OnResponse = OnToneResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, $"{this.Url}/v3/tone");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ToneRequestObj : RESTConnector.Request
            {
                /// <summary>
                /// The success callback.
                /// </summary>
                public SuccessCallback<ToneAnalysis> SuccessCallback { get; set; }
                /// <summary>
                /// The fail callback.
                /// </summary>
                public FailCallback FailCallback { get; set; }
                /// <summary>
                /// Custom data.
                /// </summary>
                public Dictionary<string, object> CustomData { get; set; }
            }

        private void OnToneResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ToneAnalysis result = new ToneAnalysis();
            fsData data = null;
            Dictionary<string, object> customData = ((ToneRequestObj)req).CustomData;

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
                    Log.Error("ToneAnalyzerService.OnToneResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((ToneRequestObj)req).SuccessCallback != null)
                    ((ToneRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ToneRequestObj)req).FailCallback != null)
                    ((ToneRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Analyze customer engagement tone.
        ///
        /// Use the customer engagement endpoint to analyze the tone of customer service and customer support
        /// conversations. For each utterance of a conversation, the method reports the most prevalent subset of the
        /// following seven tones: sad, frustrated, satisfied, excited, polite, impolite, and sympathetic.
        ///
        /// If you submit more than 50 utterances, the service returns a warning for the overall content and analyzes
        /// only the first 50 utterances. If you submit a single utterance that contains more than 500 characters, the
        /// service returns an error for that utterance and does not analyze the utterance. The request fails if all
        /// utterances have more than 500 characters. Per the JSON specification, the default character encoding for
        /// JSON content is effectively always UTF-8.
        ///
        /// **See also:** [Using the customer-engagement
        /// endpoint](https://cloud.ibm.com/docs/services/tone-analyzer/using-tone-chat.html#using-the-customer-engagement-endpoint).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="utterances">An object that contains the content to be analyzed.</param>
        /// <param name="contentLanguage">The language of the input text for the request: English or French. Regional
        /// variants are treated as their parent language; for example, `en-US` is interpreted as `en`. The input
        /// content must match the specified language. Do not submit content that contains both languages. You can use
        /// different languages for **Content-Language** and **Accept-Language**.
        /// * **`2017-09-21`:** Accepts `en` or `fr`.
        /// * **`2016-05-19`:** Accepts only `en`. (optional, default to en)</param>
        /// <param name="acceptLanguage">The desired language of the response. For two-character arguments, regional
        /// variants are treated as their parent language; for example, `en-US` is interpreted as `en`. You can use
        /// different languages for **Content-Language** and **Accept-Language**. (optional, default to en)</param>
        /// <returns><see cref="UtteranceAnalyses" />UtteranceAnalyses</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ToneChat(SuccessCallback<UtteranceAnalyses> successCallback, FailCallback failCallback, ToneChatInput utterances, string contentLanguage = null, string acceptLanguage = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            ToneChatRequestObj req = new ToneChatRequestObj();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if(req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach(KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Parameters["version"] = VersionDate;
            if(!string.IsNullOrEmpty(contentLanguage))
            request.Headers["Content-Language"] = contentLanguage;
            if(!string.IsNullOrEmpty(acceptLanguage))
            request.Headers["Accept-Language"] = acceptLanguage;
            req.OnResponse = OnToneChatResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, $"{this.Url}/v3/tone_chat");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ToneChatRequestObj : RESTConnector.Request
            {
                /// <summary>
                /// The success callback.
                /// </summary>
                public SuccessCallback<UtteranceAnalyses> SuccessCallback { get; set; }
                /// <summary>
                /// The fail callback.
                /// </summary>
                public FailCallback FailCallback { get; set; }
                /// <summary>
                /// Custom data.
                /// </summary>
                public Dictionary<string, object> CustomData { get; set; }
            }

        private void OnToneChatResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            UtteranceAnalyses result = new UtteranceAnalyses();
            fsData data = null;
            Dictionary<string, object> customData = ((ToneChatRequestObj)req).CustomData;

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
                    Log.Error("ToneAnalyzerService.OnToneChatResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((ToneChatRequestObj)req).SuccessCallback != null)
                    ((ToneChatRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ToneChatRequestObj)req).FailCallback != null)
                    ((ToneChatRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        #region IWatsonService Interface
        /// <exclude />
        public string GetServiceID()
        {
            return ServiceId;
        }
        #endregion
    }
}