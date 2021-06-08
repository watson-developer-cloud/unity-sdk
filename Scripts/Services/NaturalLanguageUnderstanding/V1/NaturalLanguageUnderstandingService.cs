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
using IBM.Watson.NaturalLanguageUnderstanding.V1.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Networking;

namespace IBM.Watson.NaturalLanguageUnderstanding.V1
{
    public partial class NaturalLanguageUnderstandingService : BaseService
    {
        private const string defaultServiceName = "natural_language_understanding";
        private const string defaultServiceUrl = "https://api.us-south.natural-language-understanding.watson.cloud.ibm.com";

        #region Version
        private string version;
        /// <summary>
        /// Gets and sets the version of the service.
        /// Release date of the API version you want to use. Specify dates in YYYY-MM-DD format. The current version is
        /// `2021-03-25`.
        /// </summary>
        public string Version
        {
            get { return version; }
            set { version = value; }
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
        /// <param name="version">Release date of the API version you want to use. Specify dates in YYYY-MM-DD format.
        /// The current version is `2021-03-25`.</param>
        public NaturalLanguageUnderstandingService(string version) : this(version, defaultServiceName, ConfigBasedAuthenticatorFactory.GetAuthenticator(defaultServiceName)) {}

        /// <summary>
        /// NaturalLanguageUnderstandingService constructor.
        /// </summary>
        /// <param name="version">Release date of the API version you want to use. Specify dates in YYYY-MM-DD format.
        /// The current version is `2021-03-25`.</param>
        /// <param name="authenticator">The service authenticator.</param>
        public NaturalLanguageUnderstandingService(string version, Authenticator authenticator) : this(version, defaultServiceName, authenticator) {}

        /// <summary>
        /// NaturalLanguageUnderstandingService constructor.
        /// </summary>
        /// <param name="version">Release date of the API version you want to use. Specify dates in YYYY-MM-DD format.
        /// The current version is `2021-03-25`.</param>
        /// <param name="serviceName">The service name to be used when configuring the client instance</param>
        public NaturalLanguageUnderstandingService(string version, string serviceName) : this(version, serviceName, ConfigBasedAuthenticatorFactory.GetAuthenticator(serviceName)) {}

        /// <summary>
        /// NaturalLanguageUnderstandingService constructor.
        /// </summary>
        /// <param name="version">Release date of the API version you want to use. Specify dates in YYYY-MM-DD format.
        /// The current version is `2021-03-25`.</param>
        /// <param name="serviceName">The service name to be used when configuring the client instance</param>
        /// <param name="authenticator">The service authenticator.</param>
        public NaturalLanguageUnderstandingService(string version, string serviceName, Authenticator authenticator) : base(authenticator, serviceName)
        {
            Authenticator = authenticator;

            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentNullException("`version` is required");
            }
            else
            {
                Version = version;
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
        /// - Classifications
        /// - Concepts
        /// - Emotion
        /// - Entities
        /// - Keywords
        /// - Metadata
        /// - Relations
        /// - Semantic roles
        /// - Sentiment
        /// - Syntax
        /// - Summarization (Experimental)
        ///
        /// If a language for the input text is not specified with the `language` parameter, the service [automatically
        /// detects the
        /// language](https://cloud.ibm.com/docs/natural-language-understanding?topic=natural-language-understanding-detectable-languages).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="features">Specific features to analyze the document for.</param>
        /// <param name="text">The plain text to analyze. One of the `text`, `html`, or `url` parameters is required.
        /// (optional)</param>
        /// <param name="html">The HTML file to analyze. One of the `text`, `html`, or `url` parameters is required.
        /// (optional)</param>
        /// <param name="url">The webpage to analyze. One of the `text`, `html`, or `url` parameters is required.
        /// (optional)</param>
        /// <param name="clean">Set this to `false` to disable webpage cleaning. For more information about webpage
        /// cleaning, see [Analyzing
        /// webpages](https://cloud.ibm.com/docs/natural-language-understanding?topic=natural-language-understanding-analyzing-webpages).
        /// (optional, default to true)</param>
        /// <param name="xpath">An [XPath
        /// query](https://cloud.ibm.com/docs/natural-language-understanding?topic=natural-language-understanding-analyzing-webpages#xpath)
        /// to perform on `html` or `url` input. Results of the query will be appended to the cleaned webpage text
        /// before it is analyzed. To analyze only the results of the XPath query, set the `clean` parameter to `false`.
        /// (optional)</param>
        /// <param name="fallbackToRaw">Whether to use raw HTML content if text cleaning fails. (optional, default to
        /// true)</param>
        /// <param name="returnAnalyzedText">Whether or not to return the analyzed text. (optional, default to
        /// false)</param>
        /// <param name="language">ISO 639-1 code that specifies the language of your text. This overrides automatic
        /// language detection. Language support differs depending on the features you include in your analysis. For
        /// more information, see [Language
        /// support](https://cloud.ibm.com/docs/natural-language-understanding?topic=natural-language-understanding-language-support).
        /// (optional)</param>
        /// <param name="limitTextCharacters">Sets the maximum number of characters that are processed by the service.
        /// (optional)</param>
        /// <returns><see cref="AnalysisResults" />AnalysisResults</returns>
        public bool Analyze(Callback<AnalysisResults> callback, Features features, string text = null, string html = null, string url = null, bool? clean = null, string xpath = null, bool? fallbackToRaw = null, bool? returnAnalyzedText = null, string language = null, long? limitTextCharacters = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `Analyze`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
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

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }
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
        /// models](https://cloud.ibm.com/docs/natural-language-understanding?topic=natural-language-understanding-customizing)
        /// that are deployed to your Natural Language Understanding service.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <returns><see cref="ListModelsResults" />ListModelsResults</returns>
        public bool ListModels(Callback<ListModelsResults> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListModels`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");

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

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

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
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
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

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

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

        /// <summary>
        /// Create sentiment model.
        ///
        /// (Beta) Creates a custom sentiment model by uploading training data and associated metadata. The model begins
        /// the training and deploying process and is ready to use when the `status` is `available`.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="language">The 2-letter language code of this model.</param>
        /// <param name="trainingData">Training data in CSV format. For more information, see [Sentiment training data
        /// requirements](https://cloud.ibm.com/docs/natural-language-understanding?topic=natural-language-understanding-custom-sentiment#sentiment-training-data-requirements).</param>
        /// <param name="name">An optional name for the model. (optional)</param>
        /// <param name="description">An optional description of the model. (optional)</param>
        /// <param name="modelVersion">An optional version string. (optional)</param>
        /// <param name="workspaceId">ID of the Watson Knowledge Studio workspace that deployed this model to Natural
        /// Language Understanding. (optional)</param>
        /// <param name="versionDescription">The description of the version. (optional)</param>
        /// <returns><see cref="SentimentModel" />SentimentModel</returns>
        public bool CreateSentimentModel(Callback<SentimentModel> callback, string language, System.IO.MemoryStream trainingData, string name = null, string description = null, string modelVersion = null, string workspaceId = null, string versionDescription = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateSentimentModel`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (string.IsNullOrEmpty(language))
                throw new ArgumentNullException("`language` is required for `CreateSentimentModel`");
            if (trainingData == null)
                throw new ArgumentNullException("`trainingData` is required for `CreateSentimentModel`");

            RequestObject<SentimentModel> req = new RequestObject<SentimentModel>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural-language-understanding", "V1", "CreateSentimentModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (!string.IsNullOrEmpty(language))
            {
                req.Forms["language"] = new RESTConnector.Form(language);
            }
            if (trainingData != null)
            {
                req.Forms["training_data"] = new RESTConnector.Form(trainingData, "filename", "text/csv");
            }
            if (!string.IsNullOrEmpty(name))
            {
                req.Forms["name"] = new RESTConnector.Form(name);
            }
            if (!string.IsNullOrEmpty(description))
            {
                req.Forms["description"] = new RESTConnector.Form(description);
            }
            if (!string.IsNullOrEmpty(modelVersion))
            {
                req.Forms["model_version"] = new RESTConnector.Form(modelVersion);
            }
            if (!string.IsNullOrEmpty(workspaceId))
            {
                req.Forms["workspace_id"] = new RESTConnector.Form(workspaceId);
            }
            if (!string.IsNullOrEmpty(versionDescription))
            {
                req.Forms["version_description"] = new RESTConnector.Form(versionDescription);
            }
            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnCreateSentimentModelResponse;

            Connector.URL = GetServiceUrl() + "/v1/models/sentiment";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateSentimentModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<SentimentModel> response = new DetailedResponse<SentimentModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<SentimentModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageUnderstandingService.OnCreateSentimentModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<SentimentModel>)req).Callback != null)
                ((RequestObject<SentimentModel>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// List sentiment models.
        ///
        /// (Beta) Returns all custom sentiment models associated with this service instance.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <returns><see cref="ListSentimentModelsResponse" />ListSentimentModelsResponse</returns>
        public bool ListSentimentModels(Callback<ListSentimentModelsResponse> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListSentimentModels`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");

            RequestObject<ListSentimentModelsResponse> req = new RequestObject<ListSentimentModelsResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural-language-understanding", "V1", "ListSentimentModels"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnListSentimentModelsResponse;

            Connector.URL = GetServiceUrl() + "/v1/models/sentiment";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListSentimentModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ListSentimentModelsResponse> response = new DetailedResponse<ListSentimentModelsResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ListSentimentModelsResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageUnderstandingService.OnListSentimentModelsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ListSentimentModelsResponse>)req).Callback != null)
                ((RequestObject<ListSentimentModelsResponse>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Get sentiment model details.
        ///
        /// (Beta) Returns the status of the sentiment model with the given model ID.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="modelId">ID of the model.</param>
        /// <returns><see cref="SentimentModel" />SentimentModel</returns>
        public bool GetSentimentModel(Callback<SentimentModel> callback, string modelId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetSentimentModel`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (string.IsNullOrEmpty(modelId))
                throw new ArgumentNullException("`modelId` is required for `GetSentimentModel`");

            RequestObject<SentimentModel> req = new RequestObject<SentimentModel>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural-language-understanding", "V1", "GetSentimentModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnGetSentimentModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/models/sentiment/{0}", modelId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetSentimentModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<SentimentModel> response = new DetailedResponse<SentimentModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<SentimentModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageUnderstandingService.OnGetSentimentModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<SentimentModel>)req).Callback != null)
                ((RequestObject<SentimentModel>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Update sentiment model.
        ///
        /// (Beta) Overwrites the training data associated with this custom sentiment model and retrains the model. The
        /// new model replaces the current deployment.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="modelId">ID of the model.</param>
        /// <param name="language">The 2-letter language code of this model.</param>
        /// <param name="trainingData">Training data in CSV format. For more information, see [Sentiment training data
        /// requirements](https://cloud.ibm.com/docs/natural-language-understanding?topic=natural-language-understanding-custom-sentiment#sentiment-training-data-requirements).</param>
        /// <param name="name">An optional name for the model. (optional)</param>
        /// <param name="description">An optional description of the model. (optional)</param>
        /// <param name="modelVersion">An optional version string. (optional)</param>
        /// <param name="workspaceId">ID of the Watson Knowledge Studio workspace that deployed this model to Natural
        /// Language Understanding. (optional)</param>
        /// <param name="versionDescription">The description of the version. (optional)</param>
        /// <returns><see cref="SentimentModel" />SentimentModel</returns>
        public bool UpdateSentimentModel(Callback<SentimentModel> callback, string modelId, string language, System.IO.MemoryStream trainingData, string name = null, string description = null, string modelVersion = null, string workspaceId = null, string versionDescription = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateSentimentModel`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (string.IsNullOrEmpty(modelId))
                throw new ArgumentNullException("`modelId` is required for `UpdateSentimentModel`");
            if (string.IsNullOrEmpty(language))
                throw new ArgumentNullException("`language` is required for `UpdateSentimentModel`");
            if (trainingData == null)
                throw new ArgumentNullException("`trainingData` is required for `UpdateSentimentModel`");

            RequestObject<SentimentModel> req = new RequestObject<SentimentModel>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural-language-understanding", "V1", "UpdateSentimentModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (!string.IsNullOrEmpty(language))
            {
                req.Forms["language"] = new RESTConnector.Form(language);
            }
            if (trainingData != null)
            {
                req.Forms["training_data"] = new RESTConnector.Form(trainingData, "filename", "text/csv");
            }
            if (!string.IsNullOrEmpty(name))
            {
                req.Forms["name"] = new RESTConnector.Form(name);
            }
            if (!string.IsNullOrEmpty(description))
            {
                req.Forms["description"] = new RESTConnector.Form(description);
            }
            if (!string.IsNullOrEmpty(modelVersion))
            {
                req.Forms["model_version"] = new RESTConnector.Form(modelVersion);
            }
            if (!string.IsNullOrEmpty(workspaceId))
            {
                req.Forms["workspace_id"] = new RESTConnector.Form(workspaceId);
            }
            if (!string.IsNullOrEmpty(versionDescription))
            {
                req.Forms["version_description"] = new RESTConnector.Form(versionDescription);
            }
            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnUpdateSentimentModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/models/sentiment/{0}", modelId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateSentimentModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<SentimentModel> response = new DetailedResponse<SentimentModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<SentimentModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageUnderstandingService.OnUpdateSentimentModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<SentimentModel>)req).Callback != null)
                ((RequestObject<SentimentModel>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Delete sentiment model.
        ///
        /// (Beta) Un-deploys the custom sentiment model with the given model ID and deletes all associated customer
        /// data, including any training data or binary artifacts.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="modelId">ID of the model.</param>
        /// <returns><see cref="DeleteModelResults" />DeleteModelResults</returns>
        public bool DeleteSentimentModel(Callback<DeleteModelResults> callback, string modelId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteSentimentModel`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (string.IsNullOrEmpty(modelId))
                throw new ArgumentNullException("`modelId` is required for `DeleteSentimentModel`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural-language-understanding", "V1", "DeleteSentimentModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnDeleteSentimentModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/models/sentiment/{0}", modelId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteSentimentModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("NaturalLanguageUnderstandingService.OnDeleteSentimentModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DeleteModelResults>)req).Callback != null)
                ((RequestObject<DeleteModelResults>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Create categories model.
        ///
        /// (Beta) Creates a custom categories model by uploading training data and associated metadata. The model
        /// begins the training and deploying process and is ready to use when the `status` is `available`.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="language">The 2-letter language code of this model.</param>
        /// <param name="trainingData">Training data in JSON format. For more information, see [Categories training data
        /// requirements](https://cloud.ibm.com/docs/natural-language-understanding?topic=natural-language-understanding-categories##categories-training-data-requirements).</param>
        /// <param name="trainingDataContentType">The content type of trainingData. (optional)</param>
        /// <param name="name">An optional name for the model. (optional)</param>
        /// <param name="description">An optional description of the model. (optional)</param>
        /// <param name="modelVersion">An optional version string. (optional)</param>
        /// <param name="workspaceId">ID of the Watson Knowledge Studio workspace that deployed this model to Natural
        /// Language Understanding. (optional)</param>
        /// <param name="versionDescription">The description of the version. (optional)</param>
        /// <returns><see cref="CategoriesModel" />CategoriesModel</returns>
        public bool CreateCategoriesModel(Callback<CategoriesModel> callback, string language, System.IO.MemoryStream trainingData, string trainingDataContentType = null, string name = null, string description = null, string modelVersion = null, string workspaceId = null, string versionDescription = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateCategoriesModel`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (string.IsNullOrEmpty(language))
                throw new ArgumentNullException("`language` is required for `CreateCategoriesModel`");
            if (trainingData == null)
                throw new ArgumentNullException("`trainingData` is required for `CreateCategoriesModel`");

            RequestObject<CategoriesModel> req = new RequestObject<CategoriesModel>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural-language-understanding", "V1", "CreateCategoriesModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (!string.IsNullOrEmpty(language))
            {
                req.Forms["language"] = new RESTConnector.Form(language);
            }
            if (trainingData != null)
            {
                req.Forms["training_data"] = new RESTConnector.Form(trainingData, "filename", trainingDataContentType);
            }
            if (!string.IsNullOrEmpty(name))
            {
                req.Forms["name"] = new RESTConnector.Form(name);
            }
            if (!string.IsNullOrEmpty(description))
            {
                req.Forms["description"] = new RESTConnector.Form(description);
            }
            if (!string.IsNullOrEmpty(modelVersion))
            {
                req.Forms["model_version"] = new RESTConnector.Form(modelVersion);
            }
            if (!string.IsNullOrEmpty(workspaceId))
            {
                req.Forms["workspace_id"] = new RESTConnector.Form(workspaceId);
            }
            if (!string.IsNullOrEmpty(versionDescription))
            {
                req.Forms["version_description"] = new RESTConnector.Form(versionDescription);
            }
            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnCreateCategoriesModelResponse;

            Connector.URL = GetServiceUrl() + "/v1/models/categories";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateCategoriesModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<CategoriesModel> response = new DetailedResponse<CategoriesModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<CategoriesModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageUnderstandingService.OnCreateCategoriesModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<CategoriesModel>)req).Callback != null)
                ((RequestObject<CategoriesModel>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// List categories models.
        ///
        /// (Beta) Returns all custom categories models associated with this service instance.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <returns><see cref="CategoriesModelList" />CategoriesModelList</returns>
        public bool ListCategoriesModels(Callback<CategoriesModelList> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListCategoriesModels`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");

            RequestObject<CategoriesModelList> req = new RequestObject<CategoriesModelList>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural-language-understanding", "V1", "ListCategoriesModels"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnListCategoriesModelsResponse;

            Connector.URL = GetServiceUrl() + "/v1/models/categories";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListCategoriesModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<CategoriesModelList> response = new DetailedResponse<CategoriesModelList>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<CategoriesModelList>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageUnderstandingService.OnListCategoriesModelsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<CategoriesModelList>)req).Callback != null)
                ((RequestObject<CategoriesModelList>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Get categories model details.
        ///
        /// (Beta) Returns the status of the categories model with the given model ID.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="modelId">ID of the model.</param>
        /// <returns><see cref="CategoriesModel" />CategoriesModel</returns>
        public bool GetCategoriesModel(Callback<CategoriesModel> callback, string modelId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetCategoriesModel`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (string.IsNullOrEmpty(modelId))
                throw new ArgumentNullException("`modelId` is required for `GetCategoriesModel`");

            RequestObject<CategoriesModel> req = new RequestObject<CategoriesModel>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural-language-understanding", "V1", "GetCategoriesModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnGetCategoriesModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/models/categories/{0}", modelId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetCategoriesModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<CategoriesModel> response = new DetailedResponse<CategoriesModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<CategoriesModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageUnderstandingService.OnGetCategoriesModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<CategoriesModel>)req).Callback != null)
                ((RequestObject<CategoriesModel>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Update categories model.
        ///
        /// (Beta) Overwrites the training data associated with this custom categories model and retrains the model. The
        /// new model replaces the current deployment.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="modelId">ID of the model.</param>
        /// <param name="language">The 2-letter language code of this model.</param>
        /// <param name="trainingData">Training data in JSON format. For more information, see [Categories training data
        /// requirements](https://cloud.ibm.com/docs/natural-language-understanding?topic=natural-language-understanding-categories##categories-training-data-requirements).</param>
        /// <param name="trainingDataContentType">The content type of trainingData. (optional)</param>
        /// <param name="name">An optional name for the model. (optional)</param>
        /// <param name="description">An optional description of the model. (optional)</param>
        /// <param name="modelVersion">An optional version string. (optional)</param>
        /// <param name="workspaceId">ID of the Watson Knowledge Studio workspace that deployed this model to Natural
        /// Language Understanding. (optional)</param>
        /// <param name="versionDescription">The description of the version. (optional)</param>
        /// <returns><see cref="CategoriesModel" />CategoriesModel</returns>
        public bool UpdateCategoriesModel(Callback<CategoriesModel> callback, string modelId, string language, System.IO.MemoryStream trainingData, string trainingDataContentType = null, string name = null, string description = null, string modelVersion = null, string workspaceId = null, string versionDescription = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateCategoriesModel`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (string.IsNullOrEmpty(modelId))
                throw new ArgumentNullException("`modelId` is required for `UpdateCategoriesModel`");
            if (string.IsNullOrEmpty(language))
                throw new ArgumentNullException("`language` is required for `UpdateCategoriesModel`");
            if (trainingData == null)
                throw new ArgumentNullException("`trainingData` is required for `UpdateCategoriesModel`");

            RequestObject<CategoriesModel> req = new RequestObject<CategoriesModel>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural-language-understanding", "V1", "UpdateCategoriesModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (!string.IsNullOrEmpty(language))
            {
                req.Forms["language"] = new RESTConnector.Form(language);
            }
            if (trainingData != null)
            {
                req.Forms["training_data"] = new RESTConnector.Form(trainingData, "filename", trainingDataContentType);
            }
            if (!string.IsNullOrEmpty(name))
            {
                req.Forms["name"] = new RESTConnector.Form(name);
            }
            if (!string.IsNullOrEmpty(description))
            {
                req.Forms["description"] = new RESTConnector.Form(description);
            }
            if (!string.IsNullOrEmpty(modelVersion))
            {
                req.Forms["model_version"] = new RESTConnector.Form(modelVersion);
            }
            if (!string.IsNullOrEmpty(workspaceId))
            {
                req.Forms["workspace_id"] = new RESTConnector.Form(workspaceId);
            }
            if (!string.IsNullOrEmpty(versionDescription))
            {
                req.Forms["version_description"] = new RESTConnector.Form(versionDescription);
            }
            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnUpdateCategoriesModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/models/categories/{0}", modelId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateCategoriesModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<CategoriesModel> response = new DetailedResponse<CategoriesModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<CategoriesModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageUnderstandingService.OnUpdateCategoriesModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<CategoriesModel>)req).Callback != null)
                ((RequestObject<CategoriesModel>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Delete categories model.
        ///
        /// (Beta) Un-deploys the custom categories model with the given model ID and deletes all associated customer
        /// data, including any training data or binary artifacts.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="modelId">ID of the model.</param>
        /// <returns><see cref="DeleteModelResults" />DeleteModelResults</returns>
        public bool DeleteCategoriesModel(Callback<DeleteModelResults> callback, string modelId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteCategoriesModel`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (string.IsNullOrEmpty(modelId))
                throw new ArgumentNullException("`modelId` is required for `DeleteCategoriesModel`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural-language-understanding", "V1", "DeleteCategoriesModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnDeleteCategoriesModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/models/categories/{0}", modelId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteCategoriesModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("NaturalLanguageUnderstandingService.OnDeleteCategoriesModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DeleteModelResults>)req).Callback != null)
                ((RequestObject<DeleteModelResults>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Create classifications model.
        ///
        /// (Beta) Creates a custom classifications model by uploading training data and associated metadata. The model
        /// begins the training and deploying process and is ready to use when the `status` is `available`.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="language">The 2-letter language code of this model.</param>
        /// <param name="trainingData">Training data in JSON format. For more information, see [Classifications training
        /// data
        /// requirements](https://cloud.ibm.com/docs/natural-language-understanding?topic=natural-language-understanding-classifications#classification-training-data-requirements).</param>
        /// <param name="trainingDataContentType">The content type of trainingData. (optional)</param>
        /// <param name="name">An optional name for the model. (optional)</param>
        /// <param name="description">An optional description of the model. (optional)</param>
        /// <param name="modelVersion">An optional version string. (optional)</param>
        /// <param name="workspaceId">ID of the Watson Knowledge Studio workspace that deployed this model to Natural
        /// Language Understanding. (optional)</param>
        /// <param name="versionDescription">The description of the version. (optional)</param>
        /// <returns><see cref="ClassificationsModel" />ClassificationsModel</returns>
        public bool CreateClassificationsModel(Callback<ClassificationsModel> callback, string language, System.IO.MemoryStream trainingData, string trainingDataContentType = null, string name = null, string description = null, string modelVersion = null, string workspaceId = null, string versionDescription = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateClassificationsModel`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (string.IsNullOrEmpty(language))
                throw new ArgumentNullException("`language` is required for `CreateClassificationsModel`");
            if (trainingData == null)
                throw new ArgumentNullException("`trainingData` is required for `CreateClassificationsModel`");

            RequestObject<ClassificationsModel> req = new RequestObject<ClassificationsModel>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural-language-understanding", "V1", "CreateClassificationsModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (!string.IsNullOrEmpty(language))
            {
                req.Forms["language"] = new RESTConnector.Form(language);
            }
            if (trainingData != null)
            {
                req.Forms["training_data"] = new RESTConnector.Form(trainingData, "filename", trainingDataContentType);
            }
            if (!string.IsNullOrEmpty(name))
            {
                req.Forms["name"] = new RESTConnector.Form(name);
            }
            if (!string.IsNullOrEmpty(description))
            {
                req.Forms["description"] = new RESTConnector.Form(description);
            }
            if (!string.IsNullOrEmpty(modelVersion))
            {
                req.Forms["model_version"] = new RESTConnector.Form(modelVersion);
            }
            if (!string.IsNullOrEmpty(workspaceId))
            {
                req.Forms["workspace_id"] = new RESTConnector.Form(workspaceId);
            }
            if (!string.IsNullOrEmpty(versionDescription))
            {
                req.Forms["version_description"] = new RESTConnector.Form(versionDescription);
            }
            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnCreateClassificationsModelResponse;

            Connector.URL = GetServiceUrl() + "/v1/models/classifications";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateClassificationsModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ClassificationsModel> response = new DetailedResponse<ClassificationsModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ClassificationsModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageUnderstandingService.OnCreateClassificationsModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ClassificationsModel>)req).Callback != null)
                ((RequestObject<ClassificationsModel>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// List classifications models.
        ///
        /// (Beta) Returns all custom classifications models associated with this service instance.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <returns><see cref="ListClassificationsModelsResponse" />ListClassificationsModelsResponse</returns>
        public bool ListClassificationsModels(Callback<ListClassificationsModelsResponse> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListClassificationsModels`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");

            RequestObject<ListClassificationsModelsResponse> req = new RequestObject<ListClassificationsModelsResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural-language-understanding", "V1", "ListClassificationsModels"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnListClassificationsModelsResponse;

            Connector.URL = GetServiceUrl() + "/v1/models/classifications";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListClassificationsModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ListClassificationsModelsResponse> response = new DetailedResponse<ListClassificationsModelsResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ListClassificationsModelsResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageUnderstandingService.OnListClassificationsModelsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ListClassificationsModelsResponse>)req).Callback != null)
                ((RequestObject<ListClassificationsModelsResponse>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Get classifications model details.
        ///
        /// (Beta) Returns the status of the classifications model with the given model ID.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="modelId">ID of the model.</param>
        /// <returns><see cref="ClassificationsModel" />ClassificationsModel</returns>
        public bool GetClassificationsModel(Callback<ClassificationsModel> callback, string modelId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetClassificationsModel`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (string.IsNullOrEmpty(modelId))
                throw new ArgumentNullException("`modelId` is required for `GetClassificationsModel`");

            RequestObject<ClassificationsModel> req = new RequestObject<ClassificationsModel>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural-language-understanding", "V1", "GetClassificationsModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnGetClassificationsModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/models/classifications/{0}", modelId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetClassificationsModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ClassificationsModel> response = new DetailedResponse<ClassificationsModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ClassificationsModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageUnderstandingService.OnGetClassificationsModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ClassificationsModel>)req).Callback != null)
                ((RequestObject<ClassificationsModel>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Update classifications model.
        ///
        /// (Beta) Overwrites the training data associated with this custom classifications model and retrains the
        /// model. The new model replaces the current deployment.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="modelId">ID of the model.</param>
        /// <param name="language">The 2-letter language code of this model.</param>
        /// <param name="trainingData">Training data in JSON format. For more information, see [Classifications training
        /// data
        /// requirements](https://cloud.ibm.com/docs/natural-language-understanding?topic=natural-language-understanding-classifications#classification-training-data-requirements).</param>
        /// <param name="trainingDataContentType">The content type of trainingData. (optional)</param>
        /// <param name="name">An optional name for the model. (optional)</param>
        /// <param name="description">An optional description of the model. (optional)</param>
        /// <param name="modelVersion">An optional version string. (optional)</param>
        /// <param name="workspaceId">ID of the Watson Knowledge Studio workspace that deployed this model to Natural
        /// Language Understanding. (optional)</param>
        /// <param name="versionDescription">The description of the version. (optional)</param>
        /// <returns><see cref="ClassificationsModel" />ClassificationsModel</returns>
        public bool UpdateClassificationsModel(Callback<ClassificationsModel> callback, string modelId, string language, System.IO.MemoryStream trainingData, string trainingDataContentType = null, string name = null, string description = null, string modelVersion = null, string workspaceId = null, string versionDescription = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateClassificationsModel`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (string.IsNullOrEmpty(modelId))
                throw new ArgumentNullException("`modelId` is required for `UpdateClassificationsModel`");
            if (string.IsNullOrEmpty(language))
                throw new ArgumentNullException("`language` is required for `UpdateClassificationsModel`");
            if (trainingData == null)
                throw new ArgumentNullException("`trainingData` is required for `UpdateClassificationsModel`");

            RequestObject<ClassificationsModel> req = new RequestObject<ClassificationsModel>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural-language-understanding", "V1", "UpdateClassificationsModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (!string.IsNullOrEmpty(language))
            {
                req.Forms["language"] = new RESTConnector.Form(language);
            }
            if (trainingData != null)
            {
                req.Forms["training_data"] = new RESTConnector.Form(trainingData, "filename", trainingDataContentType);
            }
            if (!string.IsNullOrEmpty(name))
            {
                req.Forms["name"] = new RESTConnector.Form(name);
            }
            if (!string.IsNullOrEmpty(description))
            {
                req.Forms["description"] = new RESTConnector.Form(description);
            }
            if (!string.IsNullOrEmpty(modelVersion))
            {
                req.Forms["model_version"] = new RESTConnector.Form(modelVersion);
            }
            if (!string.IsNullOrEmpty(workspaceId))
            {
                req.Forms["workspace_id"] = new RESTConnector.Form(workspaceId);
            }
            if (!string.IsNullOrEmpty(versionDescription))
            {
                req.Forms["version_description"] = new RESTConnector.Form(versionDescription);
            }
            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnUpdateClassificationsModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/models/classifications/{0}", modelId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateClassificationsModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ClassificationsModel> response = new DetailedResponse<ClassificationsModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ClassificationsModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageUnderstandingService.OnUpdateClassificationsModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ClassificationsModel>)req).Callback != null)
                ((RequestObject<ClassificationsModel>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Delete classifications model.
        ///
        /// (Beta) Un-deploys the custom classifications model with the given model ID and deletes all associated
        /// customer data, including any training data or binary artifacts.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="modelId">ID of the model.</param>
        /// <returns><see cref="DeleteModelResults" />DeleteModelResults</returns>
        public bool DeleteClassificationsModel(Callback<DeleteModelResults> callback, string modelId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteClassificationsModel`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (string.IsNullOrEmpty(modelId))
                throw new ArgumentNullException("`modelId` is required for `DeleteClassificationsModel`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural-language-understanding", "V1", "DeleteClassificationsModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnDeleteClassificationsModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/models/classifications/{0}", modelId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteClassificationsModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("NaturalLanguageUnderstandingService.OnDeleteClassificationsModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DeleteModelResults>)req).Callback != null)
                ((RequestObject<DeleteModelResults>)req).Callback(response, resp.Error);
        }
    }
}