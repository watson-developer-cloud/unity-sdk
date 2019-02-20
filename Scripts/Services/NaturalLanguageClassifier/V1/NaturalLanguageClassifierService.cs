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
using IBM.Watson.NaturalLanguageClassifier.V1.Model;
using Newtonsoft.Json;
using System;
using UnityEngine.Networking;

namespace IBM.Watson.NaturalLanguageClassifier.V1
{
    public class NaturalLanguageClassifierService : BaseService
    {
        private const string serviceId = "natural_language_classifier";
        private const string defaultUrl = "https://gateway.watsonplatform.net/natural-language-classifier/api";

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
        /// NaturalLanguageClassifierService constructor.
        /// </summary>
        
        public NaturalLanguageClassifierService() : base(serviceId)
        {
            
        }

        /// <summary>
        /// NaturalLanguageClassifierService constructor.
        /// </summary>
        
        /// <param name="credentials">The service credentials.</param>
        public NaturalLanguageClassifierService(Credentials credentials) : base(credentials, serviceId)
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
                throw new WatsonException("Please provide a username and password or authorization token to use the NaturalLanguageClassifier service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
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
        /// <param name="body">Phrase to classify.</param>
        /// <returns><see cref="Classification" />Classification</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool Classify(Callback<Classification> callback, string classifierId, ClassifyInput body, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for Classify");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("classifierId is required for Classify");
            if (body == null)
                throw new ArgumentNullException("body is required for Classify");

            RequestObject<Classification> req = new RequestObject<Classification>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=natural_language_classifier;service_version=V1;operation_id=Classify";
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (body != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            }

            req.OnResponse = OnClassifyResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/classifiers/{0}/classify", classifierId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnClassifyResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Classification> response = new WatsonResponse<Classification>();
            Dictionary<string, object> customData = ((RequestObject<Classification>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Classification>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageClassifierService.OnClassifyResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Classification>)req).Callback != null)
                ((RequestObject<Classification>)req).Callback(response, resp.Error, customData);
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
        /// <param name="body">Phrase to classify. You can submit up to 30 text phrases in a request.</param>
        /// <returns><see cref="ClassificationCollection" />ClassificationCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ClassifyCollection(Callback<ClassificationCollection> callback, string classifierId, ClassifyCollectionInput body, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ClassifyCollection");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("classifierId is required for ClassifyCollection");
            if (body == null)
                throw new ArgumentNullException("body is required for ClassifyCollection");

            RequestObject<ClassificationCollection> req = new RequestObject<ClassificationCollection>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=natural_language_classifier;service_version=V1;operation_id=ClassifyCollection";
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (body != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            }

            req.OnResponse = OnClassifyCollectionResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/classifiers/{0}/classify_collection", classifierId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnClassifyCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<ClassificationCollection> response = new WatsonResponse<ClassificationCollection>();
            Dictionary<string, object> customData = ((RequestObject<ClassificationCollection>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ClassificationCollection>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageClassifierService.OnClassifyCollectionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ClassificationCollection>)req).Callback != null)
                ((RequestObject<ClassificationCollection>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Create classifier.
        ///
        /// Sends data to create and train a classifier and returns information about the new classifier.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="metadata">Metadata in JSON format. The metadata identifies the language of the data, and an
        /// optional name to identify the classifier. Specify the language with the 2-letter primary language code as
        /// assigned in ISO standard 639.
        ///
        /// Supported languages are English (`en`), Arabic (`ar`), French (`fr`), German, (`de`), Italian (`it`),
        /// Japanese (`ja`), Korean (`ko`), Brazilian Portuguese (`pt`), and Spanish (`es`).</param>
        /// <param name="trainingData">Training data in CSV format. Each text value must have at least one class. The
        /// data can include up to 3,000 classes and 20,000 records. For details, see [Data
        /// preparation](https://cloud.ibm.com/docs/services/natural-language-classifier/using-your-data.html).</param>
        /// <returns><see cref="Classifier" />Classifier</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateClassifier(Callback<Classifier> callback, System.IO.FileStream metadata, System.IO.FileStream trainingData, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateClassifier");
            if (metadata == null)
                throw new ArgumentNullException("metadata is required for CreateClassifier");
            if (trainingData == null)
                throw new ArgumentNullException("trainingData is required for CreateClassifier");

            RequestObject<Classifier> req = new RequestObject<Classifier>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=natural_language_classifier;service_version=V1;operation_id=CreateClassifier";
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["metadata"] = new RESTConnector.Form(metadata, metadata.Name, "application/json");
            if (metadata != null)
            {
                req.Forms["metadata"] = new RESTConnector.Form(metadata.ToString());
            }
            req.Forms["trainingData"] = new RESTConnector.Form(trainingData, trainingData.Name, "text/csv");
            if (trainingData != null)
            {
                req.Forms["trainingData"] = new RESTConnector.Form(trainingData.ToString());
            }

            req.OnResponse = OnCreateClassifierResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/classifiers");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateClassifierResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Classifier> response = new WatsonResponse<Classifier>();
            Dictionary<string, object> customData = ((RequestObject<Classifier>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Classifier>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageClassifierService.OnCreateClassifierResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Classifier>)req).Callback != null)
                ((RequestObject<Classifier>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Delete classifier.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="classifierId">Classifier ID to delete.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteClassifier(Callback<object> callback, string classifierId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteClassifier");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("classifierId is required for DeleteClassifier");

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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=natural_language_classifier;service_version=V1;operation_id=DeleteClassifier";

            req.OnResponse = OnDeleteClassifierResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/classifiers/{0}", classifierId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteClassifierResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<object> response = new WatsonResponse<object>();
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
                Log.Error("NaturalLanguageClassifierService.OnDeleteClassifierResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Get information about a classifier.
        ///
        /// Returns status and other information about a classifier.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="classifierId">Classifier ID to query.</param>
        /// <returns><see cref="Classifier" />Classifier</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetClassifier(Callback<Classifier> callback, string classifierId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetClassifier");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("classifierId is required for GetClassifier");

            RequestObject<Classifier> req = new RequestObject<Classifier>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=natural_language_classifier;service_version=V1;operation_id=GetClassifier";

            req.OnResponse = OnGetClassifierResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/classifiers/{0}", classifierId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetClassifierResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Classifier> response = new WatsonResponse<Classifier>();
            Dictionary<string, object> customData = ((RequestObject<Classifier>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Classifier>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageClassifierService.OnGetClassifierResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Classifier>)req).Callback != null)
                ((RequestObject<Classifier>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// List classifiers.
        ///
        /// Returns an empty array if no classifiers are available.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <returns><see cref="ClassifierList" />ClassifierList</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListClassifiers(Callback<ClassifierList> callback, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListClassifiers");

            RequestObject<ClassifierList> req = new RequestObject<ClassifierList>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=natural_language_classifier;service_version=V1;operation_id=ListClassifiers";

            req.OnResponse = OnListClassifiersResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/classifiers");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListClassifiersResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<ClassifierList> response = new WatsonResponse<ClassifierList>();
            Dictionary<string, object> customData = ((RequestObject<ClassifierList>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ClassifierList>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("NaturalLanguageClassifierService.OnListClassifiersResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ClassifierList>)req).Callback != null)
                ((RequestObject<ClassifierList>)req).Callback(response, resp.Error, customData);
        }
    }
}