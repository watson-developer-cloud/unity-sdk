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
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Connection;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.ToneAnalyzer.V3.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Networking;

namespace IBM.Watson.ToneAnalyzer.V3
{
    public partial class ToneAnalyzerService : BaseService
    {
        private const string serviceId = "tone_analyzer";
        private const string defaultServiceUrl = "https://gateway.watsonplatform.net/tone-analyzer/api";

        #region VersionDate
        private string versionDate;
        /// <summary>
        /// Gets and sets the versionDate of the service.
        /// </summary>
        public string VersionDate
        {
            get { return versionDate; }
            set { versionDate = value; }
        }
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
        /// ToneAnalyzerService constructor.
        /// </summary>
        /// <param name="versionDate">The service version date in `yyyy-mm-dd` format.</param>
        public ToneAnalyzerService(string versionDate) : this(versionDate, ConfigBasedAuthenticatorFactory.GetAuthenticator(serviceId)) {}

        /// <summary>
        /// ToneAnalyzerService constructor.
        /// </summary>
        /// <param name="versionDate">The service version date in `yyyy-mm-dd` format.</param>
        /// <param name="authenticator">The service authenticator.</param>
        public ToneAnalyzerService(string versionDate, Authenticator authenticator) : base(versionDate, authenticator, serviceId)
        {
            Authenticator = authenticator;

            if (string.IsNullOrEmpty(versionDate))
            {
                throw new ArgumentNullException("A versionDate (format `yyyy-mm-dd`) is required to create an instance of ToneAnalyzerService");
            }
            else
            {
                VersionDate = versionDate;
            }

            if (string.IsNullOrEmpty(GetServiceUrl()))
            {
                SetServiceUrl(defaultServiceUrl);
            }
        }

        /// <summary>
        /// Analyze general tone.
        ///
        /// Use the general-purpose endpoint to analyze the tone of your input content. The service analyzes the content
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
        /// endpoint](https://cloud.ibm.com/docs/services/tone-analyzer?topic=tone-analyzer-utgpe#utgpe).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="toneInput">JSON, plain text, or HTML input that contains the content to be analyzed. For JSON
        /// input, provide an object of type `ToneInput`.</param>
        /// <param name="contentType">The type of the input. A character encoding can be specified by including a
        /// `charset` parameter. For example, 'text/plain;charset=utf-8'. (optional)</param>
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
        /// <returns><see cref="ToneAnalysis" />ToneAnalysis</returns>
        public bool Tone(Callback<ToneAnalysis> callback, ToneInput toneInput, string contentType = null, bool? sentences = null, List<string> tones = null, string contentLanguage = null, string acceptLanguage = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `Tone`");
            if (toneInput == null)
                throw new ArgumentNullException("`toneInput` is required for `Tone`");

            RequestObject<ToneAnalysis> req = new RequestObject<ToneAnalysis>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("tone_analyzer", "V3", "Tone"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (sentences != null)
            {
                req.Parameters["sentences"] = (bool)sentences ? "true" : "false";
            }
            if (tones != null && tones.Count > 0)
            {
                req.Parameters["tones"] = string.Join(",", tones.ToArray());
            }
            req.Headers["Accept"] = "application/json";

            if (!string.IsNullOrEmpty(contentType))
            {
                req.Headers["Content-Type"] = contentType;
            }

            if (!string.IsNullOrEmpty(contentLanguage))
            {
                req.Headers["Content-Language"] = contentLanguage;
            }

            if (!string.IsNullOrEmpty(acceptLanguage))
            {
                req.Headers["Accept-Language"] = acceptLanguage;
            }
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(toneInput));

            req.OnResponse = OnToneResponse;

            Connector.URL = GetServiceUrl() + "/v3/tone";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnToneResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ToneAnalysis> response = new DetailedResponse<ToneAnalysis>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ToneAnalysis>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("ToneAnalyzerService.OnToneResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ToneAnalysis>)req).Callback != null)
                ((RequestObject<ToneAnalysis>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Analyze customer-engagement tone.
        ///
        /// Use the customer-engagement endpoint to analyze the tone of customer service and customer support
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
        /// endpoint](https://cloud.ibm.com/docs/services/tone-analyzer?topic=tone-analyzer-utco#utco).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="utterances">An array of `Utterance` objects that provides the input content that the service is
        /// to analyze.</param>
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
        public bool ToneChat(Callback<UtteranceAnalyses> callback, List<Utterance> utterances, string contentLanguage = null, string acceptLanguage = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ToneChat`");
            if (utterances == null)
                throw new ArgumentNullException("`utterances` is required for `ToneChat`");

            RequestObject<UtteranceAnalyses> req = new RequestObject<UtteranceAnalyses>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("tone_analyzer", "V3", "ToneChat"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            if (!string.IsNullOrEmpty(contentLanguage))
            {
                req.Headers["Content-Language"] = contentLanguage;
            }

            if (!string.IsNullOrEmpty(acceptLanguage))
            {
                req.Headers["Accept-Language"] = acceptLanguage;
            }

            JObject bodyObject = new JObject();
            if (utterances != null && utterances.Count > 0)
                bodyObject["utterances"] = JToken.FromObject(utterances);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnToneChatResponse;

            Connector.URL = GetServiceUrl() + "/v3/tone_chat";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnToneChatResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<UtteranceAnalyses> response = new DetailedResponse<UtteranceAnalyses>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<UtteranceAnalyses>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("ToneAnalyzerService.OnToneChatResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<UtteranceAnalyses>)req).Callback != null)
                ((RequestObject<UtteranceAnalyses>)req).Callback(response, resp.Error);
        }
    }
}