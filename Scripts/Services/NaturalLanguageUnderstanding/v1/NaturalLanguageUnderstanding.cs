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

using FullSerializer;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IBM.Watson.DeveloperCloud.Services.NaturalLanguageUnderstanding.v1
{
    public class NaturalLanguageUnderstanding : IWatsonService
    {
        #region Private Data
        private const string ServiceId = "NaturalLanguageUnderstandingV1";
        private fsSerializer _serializer = new fsSerializer();
        private Credentials _credentials = null;
        private string _url = "https://gateway.watsonplatform.net/natural-language-understanding/api";
        private string _versionDate;

        private const string AnalyzeEndpoint = "/v1/analyze";
        private const string ModelsEndpoint = "/v1/models";
        private const string ModelEndpoint = "/v1/models/{0}";
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
        /// Gets and sets the versionDate of the service.
        /// </summary>
        public string VersionDate
        {
            get
            {
                if (string.IsNullOrEmpty(_versionDate))
                    throw new ArgumentNullException("VersionDate cannot be null. Use VersionDate `2017-02-27`");

                return _versionDate;
            }
            set { _versionDate = value; }
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
        #endregion

        #region Constructor
        public NaturalLanguageUnderstanding(Credentials credentials)
        {
            if (credentials.HasCredentials() || credentials.HasAuthorizationToken())
            {
                Credentials = credentials;
            }
            else
            {
                throw new WatsonException("Please provide a username and password or authorization token to use the Natural Language Understanding service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }
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

        #region Analyze
        /// <summary>
        /// Creates a new environment. You can only create one environment per service instance.An attempt to create another environment 
        /// will result in an error. The size of the new environment can be controlled by specifying the size parameter.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="parameters">The analyze parameters.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool Analyze(SuccessCallback<AnalysisResults> successCallback, FailCallback failCallback, Parameters parameters, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            AnalyzeRequest req = new AnalyzeRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.OnResponse = OnAnalyzeResponse;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Parameters["version"] = NaturalLanguageUnderstandingVersion.Version;

            fsData data = null;
            _serializer.TrySerialize(parameters, out data);
            string sendjson = data.ToString();
            req.Send = Encoding.UTF8.GetBytes(sendjson);
            RESTConnector connector = RESTConnector.GetConnector(Credentials, AnalyzeEndpoint);
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class AnalyzeRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<AnalysisResults> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnAnalyzeResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            AnalysisResults result = new AnalysisResults();
            fsData data = null;
            Dictionary<string, object> customData = ((AnalyzeRequest)req).CustomData;

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
                    Log.Error("Discovery.OnAnalyzeResponse()", "OnAnalyzeResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((AnalyzeRequest)req).SuccessCallback != null)
                    ((AnalyzeRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((AnalyzeRequest)req).FailCallback != null)
                    ((AnalyzeRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Models
        /// <summary>
        /// Lists available models for Relations and Entities features, including Watson Knowledge Studio 
        /// custom models that you have created and linked to your Natural Language Understanding service.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool GetModels(SuccessCallback<ListModelsResults> successCallback, FailCallback failCallback, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetModelsRequest req = new GetModelsRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.Parameters["version"] = NaturalLanguageUnderstandingVersion.Version;
            req.OnResponse = OnGetModelsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, ModelsEndpoint);
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetModelsRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<ListModelsResults> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ListModelsResults result = new ListModelsResults();
            fsData data = null;
            Dictionary<string, object> customData = ((GetModelsRequest)req).CustomData;

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
                    Log.Error("Discovery.OnGetModelsResponse()", "OnGetModelssResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetModelsRequest)req).SuccessCallback != null)
                    ((GetModelsRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetModelsRequest)req).FailCallback != null)
                    ((GetModelsRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Delete Model
        /// <summary>
        /// Deletes the specified model.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="modelId">The model identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool DeleteModel(SuccessCallback<bool> successCallback, FailCallback failCallback, string modelId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            if (string.IsNullOrEmpty(modelId))
                throw new ArgumentNullException("modelId");

            DeleteModelRequest req = new DeleteModelRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.Parameters["version"] = NaturalLanguageUnderstandingVersion.Version;
            req.OnResponse = OnDeleteModelResponse;
            req.Delete = true;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(ModelEndpoint, modelId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteModelRequest : RESTConnector.Request
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
        }

        private void OnDeleteModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((DeleteModelRequest)req).CustomData;

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((DeleteModelRequest)req).SuccessCallback != null)
                    ((DeleteModelRequest)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((DeleteModelRequest)req).FailCallback != null)
                    ((DeleteModelRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region IWatsonService Interface
        /// <exclude />
        public string GetServiceID()
        {
            return ServiceId;
        }
        #endregion
    }
}
