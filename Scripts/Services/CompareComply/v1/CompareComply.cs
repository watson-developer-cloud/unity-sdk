/**
* Copyright 2018 IBM Corp. All Rights Reserved.
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

using FullSerializer;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using Utility = IBM.Watson.DeveloperCloud.Utilities.Utility;

namespace IBM.Watson.DeveloperCloud.Services.CompareComply.v1
{
    public class CompareComply : IWatsonService
    {
        private const string ServiceId = "compare_comply";
        private fsSerializer _serializer = new fsSerializer();

        private Credentials credentials = null;
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
                    _url = credentials.Url;
                }
            }
        }

        private string _url = "https://gateway.watsonplatform.net/compare-comply/api";
        /// <summary>
        /// Gets and sets the endpoint URL for the service.
        /// </summary>
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        private string versionDate;
        /// <summary>
        /// Gets and sets the versionDate of the service.
        /// </summary>
        public string VersionDate
        {
            get { return versionDate; }
            set { versionDate = value; }
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

        /// <summary>
        /// CompareComply constructor. Use this constructor to auto load credentials via ibm-credentials.env file.
        /// </summary>
        public CompareComply()
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

        /// <summary>
        /// CompareComply constructor.
        /// </summary>
        /// <param name="credentials">The service credentials.</param>
        public CompareComply(Credentials credentials)
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
                throw new WatsonException("Please provide a username and password or authorization token to use the Compare Comply service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
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
        /// Convert file to HTML.
        ///
        /// Uploads an input file to the service instance, which returns an HTML version of the document.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="file">The file to convert.</param>
        /// <param name="modelId">The analysis model to be used by the service. For the `/v1/element_classification` and
        /// `/v1/comparison` methods, the default is `contracts`. For the `/v1/tables` method, the default is `tables`.
        /// These defaults apply to the standalone methods as well as to the methods' use in batch-processing requests.
        /// (optional)</param>
        /// <param name="fileContentType">The content type of file. (optional)</param>
        /// <returns><see cref="HTMLReturn" />HTMLReturn</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ConvertToHtml(SuccessCallback<HTMLReturn> successCallback, FailCallback failCallback, byte[] file, string modelId = null, string fileContentType = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (file == null)
                throw new ArgumentNullException("file");

            ConvertToHtmlRequestObj req = new ConvertToHtmlRequestObj();
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
            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["file"] = new RESTConnector.Form(file, "file", fileContentType);
            if (!string.IsNullOrEmpty(modelId))
                req.Parameters["model_id"] = modelId;
            req.OnResponse = OnConvertToHtmlResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=compare-comply;service_version=v1;operation_id=ConvertToHtml";

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/html_conversion");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ConvertToHtmlRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<HTMLReturn> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnConvertToHtmlResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            HTMLReturn result = new HTMLReturn();
            fsData data = null;
            Dictionary<string, object> customData = ((ConvertToHtmlRequestObj)req).CustomData;

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
                    Log.Error("CompareComply.OnConvertToHtmlResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((ConvertToHtmlRequestObj)req).SuccessCallback != null)
                    ((ConvertToHtmlRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ConvertToHtmlRequestObj)req).FailCallback != null)
                    ((ConvertToHtmlRequestObj)req).FailCallback(resp.Error, customData);
            }
        }
        /// <summary>
        /// Classify the elements of a document.
        ///
        /// Uploads a file to the service instance, which returns an analysis of the document's structural and semantic
        /// elements.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="file">The PDF file to convert.</param>
        /// <param name="modelId">The analysis model to be used by the service. For the `/v1/element_classification` and
        /// `/v1/comparison` methods, the default is `contracts`. For the `/v1/tables` method, the default is `tables`.
        /// These defaults apply to the standalone methods as well as to the methods' use in batch-processing requests.
        /// (optional)</param>
        /// <returns><see cref="ClassifyReturn" />ClassifyReturn</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ClassifyElements(SuccessCallback<ClassifyReturn> successCallback, FailCallback failCallback, byte[] file, string modelId = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (file == null)
                throw new ArgumentNullException("file");

            ClassifyElementsRequestObj req = new ClassifyElementsRequestObj();
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
            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["file"] = new RESTConnector.Form(file, "file", "application/pdf");
            if (!string.IsNullOrEmpty(modelId))
                req.Parameters["model_id"] = modelId;
            req.OnResponse = OnClassifyElementsResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=compare-comply;service_version=v1;operation_id=ClassifyElements";

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/element_classification");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ClassifyElementsRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<ClassifyReturn> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnClassifyElementsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ClassifyReturn result = new ClassifyReturn();
            fsData data = null;
            Dictionary<string, object> customData = ((ClassifyElementsRequestObj)req).CustomData;

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
                    Log.Error("CompareComply.OnClassifyElementsResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((ClassifyElementsRequestObj)req).SuccessCallback != null)
                    ((ClassifyElementsRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ClassifyElementsRequestObj)req).FailCallback != null)
                    ((ClassifyElementsRequestObj)req).FailCallback(resp.Error, customData);
            }
        }
        /// <summary>
        /// Extract a document's tables.
        ///
        /// Uploads a document file to the service instance, which extracts the contents of the document's tables.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="file">The PDF file on which to run table extraction.</param>
        /// <param name="modelId">The analysis model to be used by the service. For the `/v1/element_classification` and
        /// `/v1/comparison` methods, the default is `contracts`. For the `/v1/tables` method, the default is `tables`.
        /// These defaults apply to the standalone methods as well as to the methods' use in batch-processing requests.
        /// (optional)</param>
        /// <returns><see cref="TableReturn" />TableReturn</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ExtractTables(SuccessCallback<TableReturn> successCallback, FailCallback failCallback, byte[] file, string modelId = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (file == null)
                throw new ArgumentNullException("file");

            ExtractTablesRequestObj req = new ExtractTablesRequestObj();
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
            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["file"] = new RESTConnector.Form(file, "file", "application/pdf");
            if (!string.IsNullOrEmpty(modelId))
                req.Parameters["model_id"] = modelId;
            req.OnResponse = OnExtractTablesResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=compare-comply;service_version=v1;operation_id=ExtractTables";

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/tables");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ExtractTablesRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<TableReturn> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnExtractTablesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            TableReturn result = new TableReturn();
            fsData data = null;
            Dictionary<string, object> customData = ((ExtractTablesRequestObj)req).CustomData;

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
                    Log.Error("CompareComply.OnExtractTablesResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((ExtractTablesRequestObj)req).SuccessCallback != null)
                    ((ExtractTablesRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ExtractTablesRequestObj)req).FailCallback != null)
                    ((ExtractTablesRequestObj)req).FailCallback(resp.Error, customData);
            }
        }
        /// <summary>
        /// Compare two documents.
        ///
        /// Uploads two input files to the service instance, which analyzes the content and returns parsed JSON
        /// comparing the two documents. Uploaded files must be in the same file format.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="file1">The first file to compare.</param>
        /// <param name="file2">The second file to compare.</param>
        /// <param name="file1Label">A text label for the first file. The label cannot exceed 64 characters in length.
        /// The default is `file_1`. (optional)</param>
        /// <param name="file2Label">A text label for the second file. The label cannot exceed 64 characters in length.
        /// The default is `file_2`. (optional)</param>
        /// <param name="modelId">The analysis model to be used by the service. For the `/v1/element_classification` and
        /// `/v1/comparison` methods, the default is `contracts`. For the `/v1/tables` method, the default is `tables`.
        /// These defaults apply to the standalone methods as well as to the methods' use in batch-processing requests.
        /// (optional)</param>
        /// <param name="file1ContentType">The content type of file1. (optional)</param>
        /// <param name="file2ContentType">The content type of file2. (optional)</param>
        /// <returns><see cref="CompareReturn" />CompareReturn</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CompareDocuments(SuccessCallback<CompareReturn> successCallback, FailCallback failCallback, byte[] file1, byte[] file2, string file1Label = null, string file2Label = null, string modelId = null, string file1ContentType = null, string file2ContentType = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (file1 == null)
                throw new ArgumentNullException("file1");
            if (file2 == null)
                throw new ArgumentNullException("file2");

            CompareDocumentsRequestObj req = new CompareDocumentsRequestObj();
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
            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["file_1"] = new RESTConnector.Form(file1, "file1", file1ContentType);
            req.Forms["file_2"] = new RESTConnector.Form(file2, "file2", file2ContentType);
            if (!string.IsNullOrEmpty(file1Label))
                req.Parameters["file_1_label"] = file1Label;
            if (!string.IsNullOrEmpty(file2Label))
                req.Parameters["file_2_label"] = file2Label;
            if (!string.IsNullOrEmpty(modelId))
                req.Parameters["model_id"] = modelId;
            req.OnResponse = OnCompareDocumentsResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=compare-comply;service_version=v1;operation_id=CompareDocuments";

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/comparison");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class CompareDocumentsRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<CompareReturn> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnCompareDocumentsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            CompareReturn result = new CompareReturn();
            fsData data = null;
            Dictionary<string, object> customData = ((CompareDocumentsRequestObj)req).CustomData;

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
                    Log.Error("CompareComply.OnCompareDocumentsResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((CompareDocumentsRequestObj)req).SuccessCallback != null)
                    ((CompareDocumentsRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((CompareDocumentsRequestObj)req).FailCallback != null)
                    ((CompareDocumentsRequestObj)req).FailCallback(resp.Error, customData);
            }
        }
        /// <summary>
        /// Add feedback.
        ///
        /// Adds feedback in the form of _labels_ from a subject-matter expert (SME) to a governing document.
        /// **Important:** Feedback is not immediately incorporated into the training model, nor is it guaranteed to be
        /// incorporated at a later date. Instead, submitted feedback is used to suggest future updates to the training
        /// model.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="feedbackData">An object that defines the feedback to be submitted.</param>
        /// <returns><see cref="FeedbackReturn" />FeedbackReturn</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool AddFeedback(SuccessCallback<FeedbackReturn> successCallback, FailCallback failCallback, FeedbackInput feedbackData, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (feedbackData == null)
                throw new ArgumentNullException("feedbackData");

            AddFeedbackRequestObj req = new AddFeedbackRequestObj();
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

            fsData data = null;
            _serializer.TrySerialize(feedbackData, out data);
            string json = data.ToString().Replace('\"', '"');
            req.Send = Encoding.UTF8.GetBytes(json);

            req.Headers["Content-Type"] = "application/json";
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnAddFeedbackResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=compare-comply;service_version=v1;operation_id=AddFeedback";

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/feedback");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class AddFeedbackRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<FeedbackReturn> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnAddFeedbackResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            FeedbackReturn result = new FeedbackReturn();
            fsData data = null;
            Dictionary<string, object> customData = ((AddFeedbackRequestObj)req).CustomData;

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
                    Log.Error("CompareComply.OnAddFeedbackResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((AddFeedbackRequestObj)req).SuccessCallback != null)
                    ((AddFeedbackRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((AddFeedbackRequestObj)req).FailCallback != null)
                    ((AddFeedbackRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Deletes a specified feedback entry.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="feedbackId">An string that specifies the feedback entry to be deleted from the
        /// document.</param>
        /// <param name="modelId">The analysis model to be used by the service. For the `/v1/element_classification` and
        /// `/v1/comparison` methods, the default is `contracts`. For the `/v1/tables` method, the default is `tables`.
        /// These defaults apply to the standalone methods as well as to the methods' use in batch-processing requests.
        /// (optional)</param>
        /// <returns><see cref="FeedbackDeleted" />FeedbackDeleted</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteFeedback(SuccessCallback<FeedbackDeleted> successCallback, FailCallback failCallback, string feedbackId, string modelId = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            DeleteFeedbackRequestObj req = new DeleteFeedbackRequestObj();
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
            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(modelId))
                req.Parameters["model_id"] = modelId;
            req.OnResponse = OnDeleteFeedbackResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=compare-comply;service_version=v1;operation_id=DeleteFeedback";

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/feedback/{0}", feedbackId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteFeedbackRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<FeedbackDeleted> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnDeleteFeedbackResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            FeedbackDeleted result = new FeedbackDeleted();
            fsData data = null;
            Dictionary<string, object> customData = ((DeleteFeedbackRequestObj)req).CustomData;

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
                    Log.Error("CompareComply.OnDeleteFeedbackResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((DeleteFeedbackRequestObj)req).SuccessCallback != null)
                    ((DeleteFeedbackRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteFeedbackRequestObj)req).FailCallback != null)
                    ((DeleteFeedbackRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// List a specified feedback entry.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="feedbackId">An string that specifies the feedback entry to be included in the output.</param>
        /// <param name="modelId">The analysis model to be used by the service. For the `/v1/element_classification` and
        /// `/v1/comparison` methods, the default is `contracts`. For the `/v1/tables` method, the default is `tables`.
        /// These defaults apply to the standalone methods as well as to the methods' use in batch-processing requests.
        /// (optional)</param>
        /// <returns><see cref="GetFeedback" />GetFeedback</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetFeedback(SuccessCallback<GetFeedback> successCallback, FailCallback failCallback, string feedbackId, string modelId = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(feedbackId))
                throw new ArgumentNullException("feedbackId");

            GetFeedbackRequestObj req = new GetFeedbackRequestObj();
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
            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(modelId))
                req.Parameters["model_id"] = modelId;
            req.OnResponse = OnGetFeedbackResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=compare-comply;service_version=v1;operation_id=GetFeedback";

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/feedback/{0}", feedbackId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetFeedbackRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<GetFeedback> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetFeedbackResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            GetFeedback result = new GetFeedback();
            fsData data = null;
            Dictionary<string, object> customData = ((GetFeedbackRequestObj)req).CustomData;

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
                    Log.Error("CompareComply.OnGetFeedbackResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetFeedbackRequestObj)req).SuccessCallback != null)
                    ((GetFeedbackRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetFeedbackRequestObj)req).FailCallback != null)
                    ((GetFeedbackRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// List the feedback in documents.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="feedbackType">An optional string that filters the output to include only feedback with the
        /// specified feedback type. The only permitted value is `element_classification`. (optional)</param>
        /// <param name="before">An optional string in the format `YYYY-MM-DD` that filters the output to include only
        /// feedback that was added before the specified date. (optional)</param>
        /// <param name="after">An optional string in the format `YYYY-MM-DD` that filters the output to include only
        /// feedback that was added after the specified date. (optional)</param>
        /// <param name="documentTitle">An optional string that filters the output to include only feedback from the
        /// document with the specified `document_title`. (optional)</param>
        /// <param name="modelId">An optional string that filters the output to include only feedback with the specified
        /// `model_id`. The only permitted value is `contracts`. (optional)</param>
        /// <param name="modelVersion">An optional string that filters the output to include only feedback with the
        /// specified `model_version`. (optional)</param>
        /// <param name="categoryRemoved">An optional string in the form of a comma-separated list of categories. If
        /// this is specified, the service filters the output to include only feedback that has at least one category
        /// from the list removed. (optional)</param>
        /// <param name="categoryAdded">An optional string in the form of a comma-separated list of categories. If this
        /// is specified, the service filters the output to include only feedback that has at least one category from
        /// the list added. (optional)</param>
        /// <param name="categoryNotChanged">An optional string in the form of a comma-separated list of categories. If
        /// this is specified, the service filters the output to include only feedback that has at least one category
        /// from the list unchanged. (optional)</param>
        /// <param name="typeRemoved">An optional string of comma-separated `nature`:`party` pairs. If this is
        /// specified, the service filters the output to include only feedback that has at least one `nature`:`party`
        /// pair from the list removed. (optional)</param>
        /// <param name="typeAdded">An optional string of comma-separated `nature`:`party` pairs. If this is specified,
        /// the service filters the output to include only feedback that has at least one `nature`:`party` pair from the
        /// list removed. (optional)</param>
        /// <param name="typeNotChanged">An optional string of comma-separated `nature`:`party` pairs. If this is
        /// specified, the service filters the output to include only feedback that has at least one `nature`:`party`
        /// pair from the list unchanged. (optional)</param>
        /// <param name="pageLimit">An optional integer specifying the number of documents that you want the service to
        /// return. The default value is `10` and the maximum value is `100`. (optional)</param>
        /// <param name="cursor">An optional string that returns the set of documents after the previous set. Use this
        /// parameter with the `page_limit` parameter. (optional)</param>
        /// <param name="sort">An optional comma-separated list of fields in the document to sort on. You can optionally
        /// specify the sort direction by prefixing the value of the field with `-` for descending order or `+` for
        /// ascending order (the default). Currently permitted sorting fields are `created`, `user_id`, and
        /// `document_title`. (optional)</param>
        /// <param name="includeTotal">An optional boolean value. If specified as `true`, the `pagination` object in the
        /// output includes a value called `total` that gives the total count of feedback created. (optional)</param>
        /// <returns><see cref="FeedbackList" />FeedbackList</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListFeedback(SuccessCallback<FeedbackList> successCallback, FailCallback failCallback, string feedbackType = null, DateTime? before = null, DateTime? after = null, string documentTitle = null, string modelId = null, string modelVersion = null, string categoryRemoved = null, string categoryAdded = null, string categoryNotChanged = null, string typeRemoved = null, string typeAdded = null, string typeNotChanged = null, long? pageLimit = null, string cursor = null, string sort = null, bool? includeTotal = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            ListFeedbackRequestObj req = new ListFeedbackRequestObj();
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
            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(feedbackType))
                req.Parameters["feedback_type"] = feedbackType;
            if (before != null)
                req.Parameters["before"] = before;
            if (after != null)
                req.Parameters["after"] = after;
            if (!string.IsNullOrEmpty(documentTitle))
                req.Parameters["document_title"] = documentTitle;
            if (!string.IsNullOrEmpty(modelId))
                req.Parameters["model_id"] = modelId;
            if (!string.IsNullOrEmpty(modelVersion))
                req.Parameters["model_version"] = modelVersion;
            if (!string.IsNullOrEmpty(categoryRemoved))
                req.Parameters["category_removed"] = categoryRemoved;
            if (!string.IsNullOrEmpty(categoryAdded))
                req.Parameters["category_added"] = categoryAdded;
            if (!string.IsNullOrEmpty(categoryNotChanged))
                req.Parameters["category_not_changed"] = categoryNotChanged;
            if (!string.IsNullOrEmpty(typeRemoved))
                req.Parameters["type_removed"] = typeRemoved;
            if (!string.IsNullOrEmpty(typeAdded))
                req.Parameters["type_added"] = typeAdded;
            if (!string.IsNullOrEmpty(typeNotChanged))
                req.Parameters["type_not_changed"] = typeNotChanged;
            if (pageLimit != null)
                req.Parameters["page_limit"] = pageLimit;
            if (!string.IsNullOrEmpty(cursor))
                req.Parameters["cursor"] = cursor;
            if (!string.IsNullOrEmpty(sort))
                req.Parameters["sort"] = sort;
            if (includeTotal != null)
                req.Parameters["include_total"] = includeTotal;
            req.OnResponse = OnListFeedbackResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=compare-comply;service_version=v1;operation_id=ListFeedback";

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/feedback");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ListFeedbackRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<FeedbackList> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnListFeedbackResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            FeedbackList result = new FeedbackList();
            fsData data = null;
            Dictionary<string, object> customData = ((ListFeedbackRequestObj)req).CustomData;

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
                    Log.Error("CompareComply.OnListFeedbackResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((ListFeedbackRequestObj)req).SuccessCallback != null)
                    ((ListFeedbackRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ListFeedbackRequestObj)req).FailCallback != null)
                    ((ListFeedbackRequestObj)req).FailCallback(resp.Error, customData);
            }
        }
        /// <summary>
        /// Submit a batch-processing request.
        ///
        /// Run Compare and Comply methods over a collection of input documents.
        /// **Important:** Batch processing requires the use of the [IBM Cloud Object Storage
        /// service](https://cloud.ibm.com/docs/services/cloud-object-storage/about-cos.html#about-ibm-cloud-object-storage).
        /// The use of IBM Cloud Object Storage with Compare and Comply is discussed at [Using batch
        /// processing](https://cloud.ibm.com/docs/services/compare-comply/batching.html#before-you-batch).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="function">The Compare and Comply method to run across the submitted input documents. Possible
        /// values are `html_conversion`, `element_classification`, and `tables`.</param>
        /// <param name="inputCredentialsFile">A JSON file containing the input Cloud Object Storage credentials. At a
        /// minimum, the credentials must enable `READ` permissions on the bucket defined by the `input_bucket_name`
        /// parameter.</param>
        /// <param name="inputBucketLocation">The geographical location of the Cloud Object Storage input bucket as
        /// listed on the **Endpoint** tab of your Cloud Object Storage instance; for example, `us-geo`, `eu-geo`, or
        /// `ap-geo`.</param>
        /// <param name="inputBucketName">The name of the Cloud Object Storage input bucket.</param>
        /// <param name="outputCredentialsFile">A JSON file that lists the Cloud Object Storage output credentials. At a
        /// minimum, the credentials must enable `READ` and `WRITE` permissions on the bucket defined by the
        /// `output_bucket_name` parameter.</param>
        /// <param name="outputBucketLocation">The geographical location of the Cloud Object Storage output bucket as
        /// listed on the **Endpoint** tab of your Cloud Object Storage instance; for example, `us-geo`, `eu-geo`, or
        /// `ap-geo`.</param>
        /// <param name="outputBucketName">The name of the Cloud Object Storage output bucket.</param>
        /// <param name="modelId">The analysis model to be used by the service. For the `/v1/element_classification` and
        /// `/v1/comparison` methods, the default is `contracts`. For the `/v1/tables` method, the default is `tables`.
        /// These defaults apply to the standalone methods as well as to the methods' use in batch-processing requests.
        /// (optional)</param>
        /// <returns><see cref="BatchStatus" />BatchStatus</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateBatch(SuccessCallback<BatchStatus> successCallback, FailCallback failCallback, string function, byte[] inputCredentialsFile, string inputBucketLocation, string inputBucketName, byte[] outputCredentialsFile, string outputBucketLocation, string outputBucketName, string modelId = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(function))
                throw new ArgumentNullException("function");
            if (inputCredentialsFile == null)
                throw new ArgumentNullException("inputCredentialsFile");
            if (string.IsNullOrEmpty(inputBucketLocation))
                throw new ArgumentNullException("inputBucketLocation");
            if (string.IsNullOrEmpty(inputBucketName))
                throw new ArgumentNullException("inputBucketName");
            if (outputCredentialsFile == null)
                throw new ArgumentNullException("outputCredentialsFile");
            if (string.IsNullOrEmpty(outputBucketLocation))
                throw new ArgumentNullException("outputBucketLocation");
            if (string.IsNullOrEmpty(outputBucketName))
                throw new ArgumentNullException("outputBucketName");

            CreateBatchRequestObj req = new CreateBatchRequestObj();
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

            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["input_credentials_file"] = new RESTConnector.Form(inputCredentialsFile, "file", "application/json");
            req.Forms["input_bucket_location"] = new RESTConnector.Form(inputBucketLocation);
            req.Forms["input_bucket_name"] = new RESTConnector.Form(inputBucketName);
            req.Forms["output_credentials_file"] = new RESTConnector.Form(outputCredentialsFile, "file", "application/json");
            req.Forms["output_bucket_location"] = new RESTConnector.Form(outputBucketLocation);
            req.Forms["output_bucket_name"] = new RESTConnector.Form(outputBucketName);
            req.Parameters["function"] = function;
            if (!string.IsNullOrEmpty(modelId))
                req.Parameters["model_id"] = modelId;
            req.OnResponse = OnCreateBatchResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=compare-comply;service_version=v1;operation_id=CreateBatch";

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/batches");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class CreateBatchRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<BatchStatus> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnCreateBatchResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            BatchStatus result = new BatchStatus();
            fsData data = null;
            Dictionary<string, object> customData = ((CreateBatchRequestObj)req).CustomData;

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
                    Log.Error("CompareComply.OnCreateBatchResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((CreateBatchRequestObj)req).SuccessCallback != null)
                    ((CreateBatchRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((CreateBatchRequestObj)req).FailCallback != null)
                    ((CreateBatchRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Gets information about a specific batch-processing request.
        ///
        /// Gets information about a batch-processing request with a specified ID.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="batchId">The ID of the batch-processing request whose information you want to retrieve.</param>
        /// <returns><see cref="BatchStatus" />BatchStatus</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetBatch(SuccessCallback<BatchStatus> successCallback, FailCallback failCallback, string batchId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(batchId))
                throw new ArgumentNullException("batchId");

            GetBatchRequestObj req = new GetBatchRequestObj();
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
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnGetBatchResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=compare-comply;service_version=v1;operation_id=GetBatch";

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/batches/{0}", batchId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetBatchRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<BatchStatus> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetBatchResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            BatchStatus result = new BatchStatus();
            fsData data = null;
            Dictionary<string, object> customData = ((GetBatchRequestObj)req).CustomData;

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
                    Log.Error("CompareComply.OnGetBatchResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetBatchRequestObj)req).SuccessCallback != null)
                    ((GetBatchRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetBatchRequestObj)req).FailCallback != null)
                    ((GetBatchRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Gets the list of submitted batch-processing jobs.
        ///
        /// Gets the list of batch-processing jobs submitted by users.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <returns><see cref="Batches" />Batches</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListBatches(SuccessCallback<Batches> successCallback, FailCallback failCallback, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetBatchesRequestObj req = new GetBatchesRequestObj();
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
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnGetBatchesResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=compare-comply;service_version=v1;operation_id=ListBatches";

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/batches");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetBatchesRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Batches> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetBatchesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Batches result = new Batches();
            fsData data = null;
            Dictionary<string, object> customData = ((GetBatchesRequestObj)req).CustomData;

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
                    Log.Error("CompareComply.OnGetBatchesResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetBatchesRequestObj)req).SuccessCallback != null)
                    ((GetBatchesRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetBatchesRequestObj)req).FailCallback != null)
                    ((GetBatchesRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Updates a pending or active batch-processing request.
        ///
        /// Updates a pending or active batch-processing request. You can rescan the input bucket to check for new
        /// documents or cancel a request.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="batchId">The ID of the batch-processing request you want to update.</param>
        /// <param name="action">The action you want to perform on the specified batch-processing request. Possible
        /// values are `rescan` and `cancel`.</param>
        /// <param name="modelId">The analysis model to be used by the service. For the `/v1/element_classification` and
        /// `/v1/comparison` methods, the default is `contracts`. For the `/v1/tables` method, the default is `tables`.
        /// These defaults apply to the standalone methods as well as to the methods' use in batch-processing requests.
        /// (optional)</param>
        /// <returns><see cref="BatchStatus" />BatchStatus</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool UpdateBatch(SuccessCallback<BatchStatus> successCallback, FailCallback failCallback, string batchId, string action, string modelId = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(batchId))
                throw new ArgumentNullException("batchId");
            if (string.IsNullOrEmpty(action))
                throw new ArgumentNullException("action");

            UpdateBatchRequestObj req = new UpdateBatchRequestObj();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.HttpMethod = UnityWebRequest.kHttpVerbPUT;
            req.DisableSslVerification = DisableSslVerification;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(action))
                req.Parameters["action"] = action;
            if (!string.IsNullOrEmpty(modelId))
                req.Parameters["model_id"] = modelId;
            req.OnResponse = OnUpdateBatchResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=compare-comply;service_version=v1;operation_id=UpdateBatch";

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/batches/{0}", batchId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class UpdateBatchRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<BatchStatus> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnUpdateBatchResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            BatchStatus result = new BatchStatus();
            fsData data = null;
            Dictionary<string, object> customData = ((UpdateBatchRequestObj)req).CustomData;

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
                    Log.Error("CompareComply.OnUpdateBatchResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((UpdateBatchRequestObj)req).SuccessCallback != null)
                    ((UpdateBatchRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((UpdateBatchRequestObj)req).FailCallback != null)
                    ((UpdateBatchRequestObj)req).FailCallback(resp.Error, customData);
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
