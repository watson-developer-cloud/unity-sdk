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
using IBM.Watson.NaturalLanguageClassifier.V1.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Networking;

namespace IBM.Watson.NaturalLanguageClassifier.V1
{
    public partial class NaturalLanguageClassifierService : BaseService
    {
        private const string serviceId = "natural_language_classifier";
        private const string defaultServiceUrl = "https://gateway.watsonplatform.net/natural-language-classifier/api";


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
        /// NaturalLanguageClassifierService constructor.
        /// </summary>
        public NaturalLanguageClassifierService() : this(ConfigBasedAuthenticatorFactory.GetAuthenticator(serviceId)) {}

        /// <summary>
        /// NaturalLanguageClassifierService constructor.
        /// </summary>
        /// <param name="authenticator">The service authenticator.</param>
        public NaturalLanguageClassifierService(Authenticator authenticator) : base(authenticator, serviceId)
        {
            Authenticator = authenticator;

            if (string.IsNullOrEmpty(GetServiceUrl()))
            {
                SetServiceUrl(defaultServiceUrl);
            }
        }

        /// <summary>
        /// Classify a phrase.
        ///
        /// Returns label information for the input. The status must be `Available` before you can use the classifier to
        /// classify text.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="classifierId">Classifier ID to use.</param>
        /// <param name="text">The submitted phrase. The maximum length is 2048 characters.</param>
        /// <returns><see cref="Classification" />Classification</returns>
        public bool Classify(Callback<Classification> callback, string classifierId, string text)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `Classify`");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("`classifierId` is required for `Classify`");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("`text` is required for `Classify`");

            RequestObject<Classification> req = new RequestObject<Classification>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural_language_classifier", "V1", "Classify"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(text))
                bodyObject["text"] = text;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnClassifyResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/classifiers/{0}/classify", classifierId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnClassifyResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Classification> response = new DetailedResponse<Classification>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Classification>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageClassifierService.OnClassifyResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Classification>)req).Callback != null)
                ((RequestObject<Classification>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Classify multiple phrases.
        ///
        /// Returns label information for multiple phrases. The status must be `Available` before you can use the
        /// classifier to classify text.
        ///
        /// Note that classifying Japanese texts is a beta feature.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="classifierId">Classifier ID to use.</param>
        /// <param name="collection">The submitted phrases.</param>
        /// <returns><see cref="ClassificationCollection" />ClassificationCollection</returns>
        public bool ClassifyCollection(Callback<ClassificationCollection> callback, string classifierId, List<ClassifyInput> collection)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ClassifyCollection`");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("`classifierId` is required for `ClassifyCollection`");
            if (collection == null)
                throw new ArgumentNullException("`collection` is required for `ClassifyCollection`");

            RequestObject<ClassificationCollection> req = new RequestObject<ClassificationCollection>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural_language_classifier", "V1", "ClassifyCollection"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (collection != null && collection.Count > 0)
                bodyObject["collection"] = JToken.FromObject(collection);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnClassifyCollectionResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/classifiers/{0}/classify_collection", classifierId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnClassifyCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ClassificationCollection> response = new DetailedResponse<ClassificationCollection>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ClassificationCollection>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageClassifierService.OnClassifyCollectionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ClassificationCollection>)req).Callback != null)
                ((RequestObject<ClassificationCollection>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create classifier.
        ///
        /// Sends data to create and train a classifier and returns information about the new classifier.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="trainingMetadata">Metadata in JSON format. The metadata identifies the language of the data,
        /// and an optional name to identify the classifier. Specify the language with the 2-letter primary language
        /// code as assigned in ISO standard 639.
        ///
        /// Supported languages are English (`en`), Arabic (`ar`), French (`fr`), German, (`de`), Italian (`it`),
        /// Japanese (`ja`), Korean (`ko`), Brazilian Portuguese (`pt`), and Spanish (`es`).</param>
        /// <param name="trainingData">Training data in CSV format. Each text value must have at least one class. The
        /// data can include up to 3,000 classes and 20,000 records. For details, see [Data
        /// preparation](https://cloud.ibm.com/docs/natural-language-classifier?topic=natural-language-classifier-using-your-data).</param>
        /// <returns><see cref="Classifier" />Classifier</returns>
        public bool CreateClassifier(Callback<Classifier> callback, System.IO.MemoryStream trainingMetadata, System.IO.MemoryStream trainingData)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateClassifier`");
            if (trainingMetadata == null)
                throw new ArgumentNullException("`trainingMetadata` is required for `CreateClassifier`");
            if (trainingData == null)
                throw new ArgumentNullException("`trainingData` is required for `CreateClassifier`");

            RequestObject<Classifier> req = new RequestObject<Classifier>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural_language_classifier", "V1", "CreateClassifier"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (trainingMetadata != null)
            {
                req.Forms["training_metadata"] = new RESTConnector.Form(trainingMetadata, "filename", "application/json");
            }
            if (trainingData != null)
            {
                req.Forms["training_data"] = new RESTConnector.Form(trainingData, "filename", "text/csv");
            }

            req.OnResponse = OnCreateClassifierResponse;

            Connector.URL = GetServiceUrl() + "/v1/classifiers";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateClassifierResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Classifier> response = new DetailedResponse<Classifier>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Classifier>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageClassifierService.OnCreateClassifierResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Classifier>)req).Callback != null)
                ((RequestObject<Classifier>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List classifiers.
        ///
        /// Returns an empty array if no classifiers are available.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <returns><see cref="ClassifierList" />ClassifierList</returns>
        public bool ListClassifiers(Callback<ClassifierList> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListClassifiers`");

            RequestObject<ClassifierList> req = new RequestObject<ClassifierList>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural_language_classifier", "V1", "ListClassifiers"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnListClassifiersResponse;

            Connector.URL = GetServiceUrl() + "/v1/classifiers";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListClassifiersResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ClassifierList> response = new DetailedResponse<ClassifierList>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ClassifierList>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageClassifierService.OnListClassifiersResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ClassifierList>)req).Callback != null)
                ((RequestObject<ClassifierList>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get information about a classifier.
        ///
        /// Returns status and other information about a classifier.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="classifierId">Classifier ID to query.</param>
        /// <returns><see cref="Classifier" />Classifier</returns>
        public bool GetClassifier(Callback<Classifier> callback, string classifierId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetClassifier`");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("`classifierId` is required for `GetClassifier`");

            RequestObject<Classifier> req = new RequestObject<Classifier>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural_language_classifier", "V1", "GetClassifier"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnGetClassifierResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/classifiers/{0}", classifierId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetClassifierResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Classifier> response = new DetailedResponse<Classifier>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Classifier>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageClassifierService.OnGetClassifierResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Classifier>)req).Callback != null)
                ((RequestObject<Classifier>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete classifier.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="classifierId">Classifier ID to delete.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteClassifier(Callback<object> callback, string classifierId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteClassifier`");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("`classifierId` is required for `DeleteClassifier`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("natural_language_classifier", "V1", "DeleteClassifier"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnDeleteClassifierResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/classifiers/{0}", classifierId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteClassifierResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("NaturalLanguageClassifierService.OnDeleteClassifierResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
    }
}
