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
using IBM.Watson.NaturalLanguageUnderstanding.V1.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Networking;

namespace IBM.Watson.NaturalLanguageUnderstanding.V1
{
    public partial class NaturalLanguageUnderstandingService : BaseService
    {
        private const string serviceId = "natural_language_understanding";
        private const string defaultServiceUrl = "https://gateway.watsonplatform.net/natural-language-understanding/api";

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
        /// NaturalLanguageUnderstandingService constructor.
        /// </summary>
        /// <param name="versionDate">The service version date in `yyyy-mm-dd` format.</param>
        public NaturalLanguageUnderstandingService(string versionDate) : this(versionDate, ConfigBasedAuthenticatorFactory.GetAuthenticator(serviceId)) {}

        /// <summary>
        /// NaturalLanguageUnderstandingService constructor.
        /// </summary>
        /// <param name="versionDate">The service version date in `yyyy-mm-dd` format.</param>
        /// <param name="authenticator">The service authenticator.</param>
        public NaturalLanguageUnderstandingService(string versionDate, Authenticator authenticator) : base(versionDate, authenticator, serviceId)
        {
            Authenticator = authenticator;

            if (string.IsNullOrEmpty(versionDate))
            {
                throw new ArgumentNullException("A versionDate (format `yyyy-mm-dd`) is required to create an instance of NaturalLanguageUnderstandingService");
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
        /// Analyze text.
        ///
        /// Analyzes text, HTML, or a public webpage for the following features:
        /// - Categories
        /// - Concepts
        /// - Emotion
        /// - Entities
        /// - Keywords
        /// - Metadata
        /// - Relations
        /// - Semantic roles
        /// - Sentiment
        /// - Syntax (Experimental).
        ///
        /// If a language for the input text is not specified with the `language` parameter, the service [automatically
        /// detects the
        /// language](https://cloud.ibm.com/docs/services/natural-language-understanding?topic=natural-language-understanding-detectable-languages).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="features">Specific features to analyze the document for.</param>
        /// <param name="text">The plain text to analyze. One of the `text`, `html`, or `url` parameters is required.
        /// (optional)</param>
        /// <param name="html">The HTML file to analyze. One of the `text`, `html`, or `url` parameters is required.
        /// (optional)</param>
        /// <param name="url">The webpage to analyze. One of the `text`, `html`, or `url` parameters is required.
        /// (optional)</param>
        /// <param name="clean">Set this to `false` to disable webpage cleaning. To learn more about webpage cleaning,
        /// see the [Analyzing
        /// webpages](https://cloud.ibm.com/docs/services/natural-language-understanding?topic=natural-language-understanding-analyzing-webpages)
        /// documentation. (optional, default to true)</param>
        /// <param name="xpath">An [XPath
        /// query](https://cloud.ibm.com/docs/services/natural-language-understanding?topic=natural-language-understanding-analyzing-webpages#xpath)
        /// to perform on `html` or `url` input. Results of the query will be appended to the cleaned webpage text
        /// before it is analyzed. To analyze only the results of the XPath query, set the `clean` parameter to `false`.
        /// (optional)</param>
        /// <param name="fallbackToRaw">Whether to use raw HTML content if text cleaning fails. (optional, default to
        /// true)</param>
        /// <param name="returnAnalyzedText">Whether or not to return the analyzed text. (optional, default to
        /// false)</param>
        /// <param name="language">ISO 639-1 code that specifies the language of your text. This overrides automatic
        /// language detection. Language support differs depending on the features you include in your analysis. See
        /// [Language
        /// support](https://cloud.ibm.com/docs/services/natural-language-understanding?topic=natural-language-understanding-language-support)
        /// for more information. (optional)</param>
        /// <param name="limitTextCharacters">Sets the maximum number of characters that are processed by the service.
        /// (optional)</param>
        /// <returns><see cref="AnalysisResults" />AnalysisResults</returns>
        public bool Analyze(Callback<AnalysisResults> callback, Features features, string text = null, string html = null, string url = null, bool? clean = null, string xpath = null, bool? fallbackToRaw = null, bool? returnAnalyzedText = null, string language = null, long? limitTextCharacters = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `Analyze`");
            if (features == null)
                throw new ArgumentNullException("`features` is required for `Analyze`");

            RequestObject<AnalysisResults> req = new RequestObject<AnalysisResults>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural-language-understanding", "V1", "Analyze"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (features != null)
                bodyObject["features"] = JToken.FromObject(features);
            if (!string.IsNullOrEmpty(text))
                bodyObject["text"] = text;
            if (!string.IsNullOrEmpty(html))
                bodyObject["html"] = html;
            if (!string.IsNullOrEmpty(url))
                bodyObject["url"] = url;
            if (clean != null)
                bodyObject["clean"] = JToken.FromObject(clean);
            if (!string.IsNullOrEmpty(xpath))
                bodyObject["xpath"] = xpath;
            if (fallbackToRaw != null)
                bodyObject["fallback_to_raw"] = JToken.FromObject(fallbackToRaw);
            if (returnAnalyzedText != null)
                bodyObject["return_analyzed_text"] = JToken.FromObject(returnAnalyzedText);
            if (!string.IsNullOrEmpty(language))
                bodyObject["language"] = language;
            if (limitTextCharacters != null)
                bodyObject["limit_text_characters"] = JToken.FromObject(limitTextCharacters);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnAnalyzeResponse;

            Connector.URL = GetServiceUrl() + "/v1/analyze";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnAnalyzeResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<AnalysisResults> response = new DetailedResponse<AnalysisResults>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<AnalysisResults>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageUnderstandingService.OnAnalyzeResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<AnalysisResults>)req).Callback != null)
                ((RequestObject<AnalysisResults>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List models.
        ///
        /// Lists Watson Knowledge Studio [custom entities and relations
        /// models](https://cloud.ibm.com/docs/services/natural-language-understanding?topic=natural-language-understanding-customizing)
        /// that are deployed to your Natural Language Understanding service.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <returns><see cref="ListModelsResults" />ListModelsResults</returns>
        public bool ListModels(Callback<ListModelsResults> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListModels`");

            RequestObject<ListModelsResults> req = new RequestObject<ListModelsResults>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural-language-understanding", "V1", "ListModels"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListModelsResponse;

            Connector.URL = GetServiceUrl() + "/v1/models";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ListModelsResults> response = new DetailedResponse<ListModelsResults>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ListModelsResults>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageUnderstandingService.OnListModelsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ListModelsResults>)req).Callback != null)
                ((RequestObject<ListModelsResults>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete model.
        ///
        /// Deletes a custom model.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="modelId">Model ID of the model to delete.</param>
        /// <returns><see cref="DeleteModelResults" />DeleteModelResults</returns>
        public bool DeleteModel(Callback<DeleteModelResults> callback, string modelId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteModel`");
            if (string.IsNullOrEmpty(modelId))
                throw new ArgumentNullException("`modelId` is required for `DeleteModel`");

            RequestObject<DeleteModelResults> req = new RequestObject<DeleteModelResults>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural-language-understanding", "V1", "DeleteModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/models/{0}", modelId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DeleteModelResults> response = new DetailedResponse<DeleteModelResults>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DeleteModelResults>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageUnderstandingService.OnDeleteModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DeleteModelResults>)req).Callback != null)
                ((RequestObject<DeleteModelResults>)req).Callback(response, resp.Error);
        }
    }
}
