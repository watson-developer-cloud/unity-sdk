/**
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

using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Utilities;
using MiniJSON;
using System;
using System.Collections.Generic;
using System.Text;
using FullSerializer;
using UnityEngine.Networking;
using Utility = IBM.Watson.DeveloperCloud.Utilities.Utility;

namespace IBM.Watson.DeveloperCloud.Services.NaturalLanguageClassifier.v1
{
    /// <summary>
    /// This class wraps the Natural Language Classifier service.
    /// <a href="http://www.ibm.com/watson/developercloud/nl-classifier.html">Natural Language Classifier Service</a>
    /// </summary>
    public class NaturalLanguageClassifier : IWatsonService
    {
        #region Public Types
        #endregion

        #region Constructor
        /// <summary>
        /// NaturalLanguageClassifier constructor. Use this constructor to auto load credentials via ibm-credentials.env file.
        /// </summary>
        public NaturalLanguageClassifier()
        {
            var credentialsPaths = Utility.GetCredentialsPaths();
            if (credentialsPaths.Count > 0)
            {
                foreach (string path in credentialsPaths)
                {
                    if (Utility.LoadEnvFile(path))
                    {
                        break;
                    }
                }

                string ApiKey = Environment.GetEnvironmentVariable(ServiceId.ToUpper() + "_APIKEY");
                string Username = Environment.GetEnvironmentVariable(ServiceId.ToUpper() + "_USERNAME");
                string Password = Environment.GetEnvironmentVariable(ServiceId.ToUpper() + "_PASSWORD");
                string ServiceUrl = Environment.GetEnvironmentVariable(ServiceId.ToUpper() + "_URL");

                if (string.IsNullOrEmpty(ApiKey) && (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password)))
                {
                    throw new NullReferenceException(string.Format("Either {0}_APIKEY or {0}_USERNAME and {0}_PASSWORD did not exist. Please add credentials with this key in ibm-credentials.env.", ServiceId.ToUpper()));
                }

                if (!string.IsNullOrEmpty(ApiKey))
                {
                    TokenOptions tokenOptions = new TokenOptions()
                    {
                        IamApiKey = ApiKey
                    };

                    Credentials = new Credentials(tokenOptions, ServiceUrl);

                    if (string.IsNullOrEmpty(Credentials.Url))
                    {
                        Credentials.Url = Url;
                    }
                }

                if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
                {
                    Credentials = new Credentials(Username, Password, Url);
                }
            }
        }

        public NaturalLanguageClassifier(Credentials credentials)
        {
            if (credentials.HasCredentials() || credentials.HasWatsonAuthenticationToken() || credentials.HasIamTokenData())
            {
                Credentials = credentials;

                if (string.IsNullOrEmpty(credentials.Url))
                {
                    credentials.Url = Url;
                }
            }
            else
            {
                throw new WatsonException("Please provide a username and password or authorization token to use the Natural Language Classifier service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }
        #endregion

        #region Public Properties
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

        #region Private Data
        private const string ServiceId = "natural_language_classifier";
        private fsSerializer _serializer = new fsSerializer();
        private Credentials _credentials = null;
        private string _url = "https://gateway.watsonplatform.net/natural-language-classifier/api";
        #endregion

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

        #region GetClassifiers
        /// <summary>
        /// Returns an array of all classifiers to the callback function.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <returns>Returns true if the request is submitted.</returns>
        public bool GetClassifiers(SuccessCallback<Classifiers> successCallback, FailCallback failCallback, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/classifiers");
            if (connector == null)
                return false;

            GetClassifiersReq req = new GetClassifiersReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.HttpMethod = UnityWebRequest.kHttpVerbGET;
            req.DisableSslVerification = DisableSslVerification;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.OnResponse = OnGetClassifiersResp;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=natural_language_classifier;service_version=v1;operation_id=GetClassifiers";

            return connector.Send(req);
        }

        private class GetClassifiersReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Classifiers> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        };

        private void OnGetClassifiersResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Classifiers result = new Classifiers();
            fsData data = null;
            Dictionary<string, object> customData = ((GetClassifiersReq)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

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
                    Log.Error("NaturalLanguageClassifier.OnGetClassifiersResp()", "GetClassifiers Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetClassifiersReq)req).SuccessCallback != null)
                    ((GetClassifiersReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetClassifiersReq)req).FailCallback != null)
                    ((GetClassifiersReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region GetClassifier
        /// <summary>
        /// Returns a specific classifer.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="classifierId">The ID of the classifier to get.</param>
        /// <returns>Returns true if the request is submitted.</returns>
        public bool GetClassifier(SuccessCallback<Classifier> successCallback, FailCallback failCallback, string classifierId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("classifierId");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/classifiers/" + classifierId);
            if (connector == null)
                return false;

            GetClassifierReq req = new GetClassifierReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.HttpMethod = UnityWebRequest.kHttpVerbGET;
            req.DisableSslVerification = DisableSslVerification;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.OnResponse = OnGetClassifierResp;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=natural_language_classifier;service_version=v1;operation_id=GetClassifier";

            return connector.Send(req);
        }

        private class GetClassifierReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Classifier> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        };

        private void OnGetClassifierResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Classifier result = new Classifier();
            fsData data = null;
            Dictionary<string, object> customData = ((GetClassifierReq)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

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
                    Log.Error("NaturalLanguageClassifier.OnGetClassifierResp()", "GetClassifiers Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetClassifierReq)req).SuccessCallback != null)
                    ((GetClassifierReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetClassifierReq)req).FailCallback != null)
                    ((GetClassifierReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region TrainClassifier
        /// <summary>
        /// Train a new classifier. 
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="classifierName">A name to give the classifier.</param>
        /// <param name="language">Language of the classifier.</param>
        /// <param name="trainingData">CSV training data.</param>
        /// <returns>Returns true if training data was submitted correctly.</returns>
        public bool TrainClassifier(SuccessCallback<Classifier> successCallback, FailCallback failCallback, string classifierName, string language, string trainingData, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(classifierName))
                throw new ArgumentNullException("classifierId");
            if (string.IsNullOrEmpty(language))
                throw new ArgumentNullException("language");
            if (string.IsNullOrEmpty(trainingData))
                throw new ArgumentNullException("trainingData");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/classifiers");
            if (connector == null)
                return false;

            Dictionary<string, object> trainingMetaData = new Dictionary<string, object>();
            trainingMetaData["language"] = language;
            trainingMetaData["name"] = classifierName;

            TrainClassifierReq req = new TrainClassifierReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.HttpMethod = UnityWebRequest.kHttpVerbPOST;
            req.DisableSslVerification = DisableSslVerification;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.OnResponse = OnTrainClassifierResp;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=natural_language_classifier;service_version=v1;operation_id=TrainClassifier";
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["training_metadata"] = new RESTConnector.Form(Encoding.UTF8.GetBytes(Json.Serialize(trainingMetaData)));
            req.Forms["training_data"] = new RESTConnector.Form(Encoding.UTF8.GetBytes(trainingData));

            return connector.Send(req);
        }

        private class TrainClassifierReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Classifier> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        };

        private void OnTrainClassifierResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Classifier result = new Classifier();
            fsData data = null;
            Dictionary<string, object> customData = ((TrainClassifierReq)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

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
                    Log.Error("NaturalLanguageClassifier.OnTrainClassifierResp()", "GetClassifiers Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((TrainClassifierReq)req).SuccessCallback != null)
                    ((TrainClassifierReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((TrainClassifierReq)req).FailCallback != null)
                    ((TrainClassifierReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region DeleteClassifier
        /// <summary>
        /// Deletes the specified classifier.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="classifierId">The ID of the classifier.</param>
        /// <returns>Returns false if we failed to submit a request.</returns>
        public bool DeleteClassifer(SuccessCallback<bool> successCallback, FailCallback failCallback, string classifierId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("classiferId");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/classifiers/" + classifierId);
            if (connector == null)
                return false;

            DeleteClassifierReq req = new DeleteClassifierReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.HttpMethod = UnityWebRequest.kHttpVerbDELETE;
            req.DisableSslVerification = DisableSslVerification;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.OnResponse = OnDeleteClassifierResp;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=natural_language_classifier;service_version=v1;operation_id=DeleteClassifier";

            return connector.Send(req);
        }

        private class DeleteClassifierReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<bool> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        };

        private void OnDeleteClassifierResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((DeleteClassifierReq)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((DeleteClassifierReq)req).SuccessCallback != null)
                    ((DeleteClassifierReq)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((DeleteClassifierReq)req).FailCallback != null)
                    ((DeleteClassifierReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Classify
        /// <summary>
        /// Classifies the given text, invokes the callback with the results.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="classifierId">The ID of the classifier to use.</param>
        /// <param name="text">The text to classify.</param>
        /// <returns>Returns false if we failed to submit the request.</returns>
        public bool Classify(SuccessCallback<ClassifyResult> successCallback, FailCallback failCallback, string classifierId, string text, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("classifierId");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/classifiers");
            if (connector == null)
                return false;

            ClassifyReq req = new ClassifyReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.HttpMethod = UnityWebRequest.kHttpVerbPOST;
            req.DisableSslVerification = DisableSslVerification;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.OnResponse = OnClassifyResp;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=natural_language_classifier;service_version=v1;operation_id=Classify";
            req.Function = "/" + classifierId + "/classify";
            req.Headers["Content-Type"] = "application/json";

            Dictionary<string, object> body = new Dictionary<string, object>();
            body["text"] = text;
            req.Send = Encoding.UTF8.GetBytes(Json.Serialize(body));

            return connector.Send(req);
        }
        private class ClassifyReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<ClassifyResult> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        };

        private void OnClassifyResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ClassifyResult result = new ClassifyResult(); ;
            fsData data = null;
            Dictionary<string, object> customData = ((ClassifyReq)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

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
                    Log.Error("NaturalLanguageClassifier.OnTrainClassifierResp()", "GetClassifiers Exception: {0}", e.ToString());
                }
            }

            if (resp.Success)
            {
                if (((ClassifyReq)req).SuccessCallback != null)
                    ((ClassifyReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ClassifyReq)req).FailCallback != null)
                    ((ClassifyReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region ClassifyCollection
        /// <summary>
        /// Returns label information for multiple phrases. The status must be Available before you can use the classifier to classify text. Note that classifying Japanese texts is a beta feature.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="classifierId">The ID of the classifier to use.</param>
        /// <param name="body">The collection of text to classify.</param>
        /// <returns>Returns false if we failed to submit the request.</returns>
        public bool ClassifyCollection(SuccessCallback<ClassificationCollection> successCallback, FailCallback failCallback, string classifierId, ClassifyCollectionInput body, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("classifierId");
            if (body == null)
                throw new ArgumentNullException("body");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/classifiers");
            if (connector == null)
                return false;

            ClassifyCollectionReq req = new ClassifyCollectionReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.HttpMethod = UnityWebRequest.kHttpVerbPOST;
            req.DisableSslVerification = DisableSslVerification;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.OnResponse = OnClassifyCollectionResp;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=natural_language_classifier;service_version=v1;operation_id=ClassifyCollection";
            req.Function = "/" + classifierId + "/classify_collection";
            req.Headers["Content-Type"] = "application/json";

            fsData data = null;
            _serializer.TrySerialize(body, out data);
            req.Send = Encoding.UTF8.GetBytes(data.ToString());

            return connector.Send(req);
        }
        private class ClassifyCollectionReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<ClassificationCollection> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        };

        private void OnClassifyCollectionResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ClassificationCollection result = new ClassificationCollection();
            fsData data = null;
            Dictionary<string, object> customData = ((ClassifyCollectionReq)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

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
                    Log.Error("NaturalLanguageClassifier.OnClassifyCollectionResp()", "OnClassifyCollectionResp Exception: {0}", e.ToString());
                }
            }

            if (resp.Success)
            {
                if (((ClassifyCollectionReq)req).SuccessCallback != null)
                    ((ClassifyCollectionReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ClassifyCollectionReq)req).FailCallback != null)
                    ((ClassifyCollectionReq)req).FailCallback(resp.Error, customData);
            }
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
