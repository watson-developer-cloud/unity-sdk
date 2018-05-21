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
using MiniJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    /// <summary>
    /// This class wraps the Discovery service
    /// <a href="https://www.ibm.com/watson/services/discovery/">Discovery Service</a>
    /// </summary>
    public class Discovery : IWatsonService
    {
        #region Private Data
        private const string ServiceId = "DiscoveryV1";
        private fsSerializer _serializer = new fsSerializer();
        private Credentials _credentials = null;
        private string _url = "https://gateway.watsonplatform.net/discovery/api";
        private string _versionDate;

        private const string Environments = "/v1/environments";
        private const string Environment = "/v1/environments/{0}";
        private const string PreviewEnvironment = "/v1/environments/{0}/preview";
        private const string Configurations = "/v1/environments/{0}/configurations";
        private const string Configuration = "/v1/environments/{0}/configurations/{1}";
        private const string Collections = "/v1/environments/{0}/collections";
        private const string Collection = "/v1/environments/{0}/collections/{1}";
        private const string Fields = "/v1/environments/{0}/collections/{1}/fields";
        private const string Documents = "/v1/environments/{0}/collections/{1}/documents";
        private const string Document = "/v1/environments/{0}/collections/{1}/documents/{2}";
        private const string QueryCollection = "/v1/environments/{0}/collections/{1}/query";
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
                    throw new ArgumentNullException("VersionDate cannot be null. Use `2016-12-01`");

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
        /// <summary>
        /// Discovery constructor.
        /// </summary>
        /// <param name="credentials">The service credentials.</param>
        public Discovery(Credentials credentials)
        {
            if (credentials.HasCredentials() || credentials.HasWatsonAuthenticationToken() || credentials.HasIamTokenData())
            {
                Credentials = credentials;
            }
            else
            {
                throw new WatsonException("Please provide a username and password or authorization token to use the Discovery service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
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

        #region Environments
        #region GetEnvironments
        /// <summary>
        /// This class lists environments in a discovery instance. There are two environments returned: A read-only environment with the News
        /// collection (IBM Managed) and a user-created environment that the user can utilize to analyze and query their own data.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool GetEnvironments(SuccessCallback<GetEnvironmentsResponse> successCallback, FailCallback failCallback, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetEnvironmentsRequest req = new GetEnvironmentsRequest();
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
            req.OnResponse = OnGetEnvironmentsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, Environments);
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetEnvironmentsRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<GetEnvironmentsResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetEnvironmentsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            GetEnvironmentsResponse result = new GetEnvironmentsResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((GetEnvironmentsRequest)req).CustomData;
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
                    Log.Error("Discovery.OnGetEnvironmentsResponse()", "OnGetEnvironmentsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetEnvironmentsRequest)req).SuccessCallback != null)
                    ((GetEnvironmentsRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetEnvironmentsRequest)req).FailCallback != null)
                    ((GetEnvironmentsRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Add Environment
        /// <summary>
        /// Creates a new environment. You can only create one environment per service instance.An attempt to create another environment 
        /// will result in an error. The size of the new environment can be controlled by specifying the size parameter.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="name">The name of the environment to be created.</param>
        /// <param name="description">The description of the environment to be created.</param>
        /// <param name="size">The size of the environment to be created. See <a href="https://www.ibm.com/watson/services/discovery/#pricing-block">pricing.</a></param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool AddEnvironment(SuccessCallback<Environment> successCallback, FailCallback failCallback, string name = default(string), string description = default(string), int size = 0, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            Dictionary<string, object> addEnvironmentData = new Dictionary<string, object>();
            addEnvironmentData["name"] = name;
            addEnvironmentData["description"] = description;
            addEnvironmentData["size"] = size;

            return AddEnvironment(successCallback, failCallback, addEnvironmentData, customData);
        }

        /// <summary>
        /// Creates a new environment. You can only create one environment per service instance.An attempt to create another environment 
        /// will result in an error. The size of the new environment can be controlled by specifying the size parameter.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="addEnvironmentData">The AddEnvironmentData.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool AddEnvironment(SuccessCallback<Environment> successCallback, FailCallback failCallback, Dictionary<string, object> addEnvironmentData, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            AddEnvironmentRequest req = new AddEnvironmentRequest();
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
            req.OnResponse = OnAddEnvironmentResponse;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            string sendjson = Json.Serialize(addEnvironmentData);
            req.Send = Encoding.UTF8.GetBytes(sendjson);

            RESTConnector connector = RESTConnector.GetConnector(Credentials, Environments);
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class AddEnvironmentRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Environment> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnAddEnvironmentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Environment result = new Environment();
            fsData data = null;
            Dictionary<string, object> customData = ((AddEnvironmentRequest)req).CustomData;
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
                    Log.Error("Discovery.OnAddEnvironmentResponse()", "OnAddEnvironmentResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((AddEnvironmentRequest)req).SuccessCallback != null)
                    ((AddEnvironmentRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((AddEnvironmentRequest)req).FailCallback != null)
                    ((AddEnvironmentRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region GetEnvironment
        /// <summary>
        /// Returns specified environment data.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier requested.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool GetEnvironment(SuccessCallback<Environment> successCallback, FailCallback failCallback, string environmentID, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");

            GetEnvironmentRequest req = new GetEnvironmentRequest();
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
            req.OnResponse = OnGetEnvironmentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Environment, environmentID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetEnvironmentRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Environment> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetEnvironmentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Environment result = new Environment();
            fsData data = null;
            Dictionary<string, object> customData = ((GetEnvironmentRequest)req).CustomData;
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
                    Log.Error("Discovery.OnGetEnvironmentResponse()", "OnGetEnvironmentResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetEnvironmentRequest)req).SuccessCallback != null)
                    ((GetEnvironmentRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetEnvironmentRequest)req).FailCallback != null)
                    ((GetEnvironmentRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Delete Environment
        /// <summary>
        /// Deletes the specified environment.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool DeleteEnvironment(SuccessCallback<DeleteEnvironmentResponse> successCallback, FailCallback failCallback, string environmentID, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");

            DeleteEnvironmentRequest req = new DeleteEnvironmentRequest();
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
            req.OnResponse = OnDeleteEnvironmentResponse;
            req.Delete = true;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Environment, environmentID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteEnvironmentRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<DeleteEnvironmentResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnDeleteEnvironmentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DeleteEnvironmentResponse result = new DeleteEnvironmentResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((DeleteEnvironmentRequest)req).CustomData;
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
                    Log.Error("Discovery.OnDeleteEnvironmentResponse()", "OnDeleteEnvironmentResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((DeleteEnvironmentRequest)req).SuccessCallback != null)
                    ((DeleteEnvironmentRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteEnvironmentRequest)req).FailCallback != null)
                    ((DeleteEnvironmentRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion
        #endregion

        #region Configurations
        #region Get Configurations
        /// <summary>
        /// Lists an environment's configurations.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="name">An optional configuration name to search.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool GetConfigurations(SuccessCallback<GetConfigurationsResponse> successCallback, FailCallback failCallback, string environmentID, string name = default(string), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetConfigurationsRequest req = new GetConfigurationsRequest();
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
            if (!string.IsNullOrEmpty(name))
            {
                req.Parameters["name"] = name;
            }
            req.OnResponse = OnGetConfigurationsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Configurations, environmentID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetConfigurationsRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<GetConfigurationsResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetConfigurationsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            GetConfigurationsResponse result = new GetConfigurationsResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((GetConfigurationsRequest)req).CustomData;
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
                    Log.Error("Discovery.OnGetConfigurationsResponse()", "OnGetConfigurationsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetConfigurationsRequest)req).SuccessCallback != null)
                    ((GetConfigurationsRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetConfigurationsRequest)req).FailCallback != null)
                    ((GetConfigurationsRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Add Configuration
        /// <summary>
        /// Adds a configuration via external json file.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="configJson">The configuration json.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool AddConfiguration(SuccessCallback<Configuration> successCallback, FailCallback failCallback, string environmentID, string configJson, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(configJson))
                throw new ArgumentNullException("configurationJsonPath");

            byte[] configJsonData;

            try
            {
                configJsonData = Encoding.UTF8.GetBytes(configJson);
            }
            catch (Exception e)
            {
                throw new WatsonException(string.Format("Failed to load configuration json: {0}", e.Message));
            }

            return AddConfiguration(successCallback, failCallback, environmentID, configJsonData, customData);
        }

        /// <summary>
        /// Adds a configuration via json byte data.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="configurationJsonData">A byte array of configuration json data.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool AddConfiguration(SuccessCallback<Configuration> successCallback, FailCallback failCallback, string environmentID, byte[] configurationJsonData, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (configurationJsonData == null)
                throw new ArgumentNullException("configurationJsonData");

            AddConfigurationRequest req = new AddConfigurationRequest();
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
            req.OnResponse = OnAddConfigurationResponse;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Send = configurationJsonData;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Configurations, environmentID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class AddConfigurationRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Configuration> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnAddConfigurationResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Configuration result = new Configuration();
            fsData data = null;
            Dictionary<string, object> customData = ((AddConfigurationRequest)req).CustomData;
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
                    Log.Error("Discovery.OnAddConfigurationResponse()", "OnGetConfigurationResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((AddConfigurationRequest)req).SuccessCallback != null)
                    ((AddConfigurationRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((AddConfigurationRequest)req).FailCallback != null)
                    ((AddConfigurationRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Configuration
        /// <summary>
        /// Gets details of an environment's configuration.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="configurationID">The configuration identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool GetConfiguration(SuccessCallback<Configuration> successCallback, FailCallback failCallback, string environmentID, string configurationID, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(configurationID))
                throw new ArgumentNullException("configurationID");

            GetConfigurationRequest req = new GetConfigurationRequest();
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
            req.OnResponse = OnGetConfigurationResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Configuration, environmentID, configurationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetConfigurationRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Configuration> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetConfigurationResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Configuration result = new Configuration();
            fsData data = null;
            Dictionary<string, object> customData = ((GetConfigurationRequest)req).CustomData;
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
                    Log.Error("Discovery.OnGetConfigurationResponse()", "OnGetConfigurationResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetConfigurationRequest)req).SuccessCallback != null)
                    ((GetConfigurationRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetConfigurationRequest)req).FailCallback != null)
                    ((GetConfigurationRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Delete Configuration
        /// <summary>
        /// Deletes an environments specified configuration.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="configurationID">The configuration identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool DeleteConfiguration(SuccessCallback<DeleteConfigurationResponse> successCallback, FailCallback failCallback, string environmentID, string configurationID, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(configurationID))
                throw new ArgumentNullException("configurationID");

            DeleteConfigurationRequest req = new DeleteConfigurationRequest();
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
            req.OnResponse = OnDeleteConfigurationResponse;
            req.Delete = true;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Configuration, environmentID, configurationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteConfigurationRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<DeleteConfigurationResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnDeleteConfigurationResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DeleteConfigurationResponse result = new DeleteConfigurationResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((DeleteConfigurationRequest)req).CustomData;
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
                    Log.Error("Discovery.OnGetAuthorsResponse()", "OnGetAuthorsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((DeleteConfigurationRequest)req).SuccessCallback != null)
                    ((DeleteConfigurationRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteConfigurationRequest)req).FailCallback != null)
                    ((DeleteConfigurationRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion
        #endregion

        #region Preview Configuration
        /// <summary>
        /// Runs a sample document through the default or your configuration and returns diagnostic information designed to 
        /// help you understand how the document was processed. The document is not added to the index.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="configurationID">The ID of the configuration to use to process the document. If the configurationFilePath is also 
        /// provided (both are present at the same time), then request will be rejected.</param>
        /// <param name="configurationFilePath">The configuration to use to process the document. If this part is provided, then the 
        /// provided configuration is used to process the document. If the configuration_id is also provided (both are present at the 
        /// same time), then request is rejected. The maximum supported configuration size is 1 MB. Configuration parts larger than 1 MB 
        /// are rejected. See the GET /configurations/{configuration_id} operation for an example configuration.</param>
        /// <param name="contentFilePath">The file path to document to ingest.The maximum supported file size is 50 megabytes. Files 
        /// larger than 50 megabytes is rejected.</param>
        /// <param name="metadata">If you're using the Data Crawler to upload your documents, you can test a document against the type of 
        /// metadata that the Data Crawler might send. The maximum supported metadata file size is 1 MB. Metadata parts larger than 1 MB 
        /// are rejected. Example: { "Creator": "Johnny Appleseed", "Subject": "Apples" }</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool PreviewConfiguration(SuccessCallback<TestDocument> successCallback, FailCallback failCallback, string environmentID, string configurationID, string configurationFilePath, string contentFilePath, string metadata = default(string), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(configurationID) && string.IsNullOrEmpty(configurationFilePath))
                throw new ArgumentNullException("configurationID or configurationFilePath");
            if (!string.IsNullOrEmpty(configurationID) && !string.IsNullOrEmpty(configurationFilePath))
                throw new WatsonException("Use either a configurationID OR designate a test configuration file path - not both");
            if (string.IsNullOrEmpty(contentFilePath))
                throw new ArgumentNullException("contentFilePath");

            byte[] contentData;
            try
            {
                contentData = File.ReadAllBytes(contentFilePath);
            }
            catch (Exception e)
            {
                throw new WatsonException(string.Format("Failed to load content: {0}", e.Message));
            }

            string contentMimeType = Utility.GetMimeType(Path.GetExtension(contentFilePath));

            return PreviewConfiguration(successCallback, failCallback, environmentID, configurationID, configurationFilePath, contentData, contentMimeType, metadata, customData);
        }

        /// <summary>
        /// Runs a sample document through the default or your configuration and returns diagnostic information designed to 
        /// help you understand how the document was processed. The document is not added to the index.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="configurationID">The ID of the configuration to use to process the document. If the configurationFilePath is also 
        /// provided (both are present at the same time), then request will be rejected.</param>
        /// <param name="configurationFilePath">The configuration to use to process the document. If this part is provided, then the 
        /// provided configuration is used to process the document. If the configuration_id is also provided (both are present at the 
        /// same time), then request is rejected. The maximum supported configuration size is 1 MB. Configuration parts larger than 1 MB 
        /// are rejected. See the GET /configurations/{configuration_id} operation for an example configuration.</param>
        /// <param name="contentData">The byte array data of the document to ingest.The maximum supported file size is 50 megabytes. Files 
        /// larger than 50 megabytes is rejected.</param>
        /// <param name="contentMimeType">The mimeType of the document to ingest.</param>
        /// <param name="metadata">If you're using the Data Crawler to upload your documents, you can test a document against the type of 
        /// metadata that the Data Crawler might send. The maximum supported metadata file size is 1 MB. Metadata parts larger than 1 MB 
        /// are rejected. Example: { "Creator": "Johnny Appleseed", "Subject": "Apples" }</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool PreviewConfiguration(SuccessCallback<TestDocument> successCallback, FailCallback failCallback, string environmentID, string configurationID, string configurationFilePath, byte[] contentData, string contentMimeType, string metadata = default(string), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(configurationID) && string.IsNullOrEmpty(configurationFilePath))
                throw new ArgumentNullException("configurationID or configurationFilePath");
            if (!string.IsNullOrEmpty(configurationID) && !string.IsNullOrEmpty(configurationFilePath))
                throw new WatsonException("Use either a configurationID OR designate a test configuration file path - not both");

            if (contentData == null)
                throw new ArgumentNullException("contentData");

            PreviewConfigurationRequest req = new PreviewConfigurationRequest();
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
            req.OnResponse = OnPreviewConfigurationResponse;

            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["file"] = new RESTConnector.Form(contentData, "contentData", contentMimeType);
            req.Forms["metadata"] = new RESTConnector.Form(metadata);

            if (!string.IsNullOrEmpty(configurationFilePath))
            {
                string configJson;

                try
                {
                    configJson = File.ReadAllText(configurationFilePath);
                }
                catch (Exception e)
                {
                    throw new WatsonException(string.Format("Failed to load configuration json: {0}", e.Message));
                }

                req.Forms["configuration"] = new RESTConnector.Form(configJson);
            }
            else if (!string.IsNullOrEmpty(configurationID))
            {
                req.Parameters["configuration_id"] = configurationID;
            }

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(PreviewEnvironment, environmentID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class PreviewConfigurationRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<TestDocument> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnPreviewConfigurationResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            TestDocument result = new TestDocument();
            fsData data = null;
            Dictionary<string, object> customData = ((PreviewConfigurationRequest)req).CustomData;
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
                    Log.Error("Discovery.OnPreviewConfigurationResponse()", "OnPreviewConfigurationResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((PreviewConfigurationRequest)req).SuccessCallback != null)
                    ((PreviewConfigurationRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((PreviewConfigurationRequest)req).FailCallback != null)
                    ((PreviewConfigurationRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Collections
        #region Get Collections
        /// <summary>
        /// Lists a specified environment's collections.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="name">Find collections with the given name.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool GetCollections(SuccessCallback<GetCollectionsResponse> successCallback, FailCallback failCallback, string environmentID, string name = default(string), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");

            GetCollectionsRequest req = new GetCollectionsRequest();
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
            if (!string.IsNullOrEmpty(name))
            {
                req.Parameters["name"] = name;
            }
            req.OnResponse = OnGetCollectionsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Collections, environmentID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCollectionsRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<GetCollectionsResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetCollectionsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            GetCollectionsResponse result = new GetCollectionsResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((GetCollectionsRequest)req).CustomData;
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
                    Log.Error("Discovery.OnGetCollectionsResponse()", "OnGetCollectionsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetCollectionsRequest)req).SuccessCallback != null)
                    ((GetCollectionsRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetCollectionsRequest)req).FailCallback != null)
                    ((GetCollectionsRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Add Collection
        /// <summary>
        /// Adds a collection to a specified environment.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="name">The name of the collection to be created.</param>
        /// <param name="description">The description of the collection to be created.</param>
        /// <param name="configurationID">The configuration identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool AddCollection(SuccessCallback<CollectionRef> successCallback, FailCallback failCallback, string environmentID, string name, string description = default(string), string configurationID = default(string), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["name"] = name;
            parameters["description"] = description;
            parameters["configuration_id"] = configurationID;

            return AddCollection(successCallback, failCallback, environmentID, Encoding.UTF8.GetBytes(Json.Serialize(parameters)), customData);
        }

        /// <summary>
        /// Adds a collection to a specified environment.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionData">A byte array of json collection data.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool AddCollection(SuccessCallback<CollectionRef> successCallback, FailCallback failCallback, string environmentID, byte[] collectionData, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (collectionData == null)
                throw new ArgumentNullException("collectionData");

            AddCollectionRequest req = new AddCollectionRequest();
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
            req.OnResponse = OnAddCollectionResponse;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Send = collectionData;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Collections, environmentID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class AddCollectionRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<CollectionRef> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnAddCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            CollectionRef result = new CollectionRef();
            fsData data = null;
            Dictionary<string, object> customData = ((AddCollectionRequest)req).CustomData;
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
                    Log.Error("Discovery.OnAddCollectionResponse()", "OnGetConfigurationResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((AddCollectionRequest)req).SuccessCallback != null)
                    ((AddCollectionRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((AddCollectionRequest)req).FailCallback != null)
                    ((AddCollectionRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Collection
        /// <summary>
        /// Lists a specified collecton's details.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool GetCollection(SuccessCallback<Collection> successCallback, FailCallback failCallback, string environmentID, string collectionID, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");

            GetCollectionRequest req = new GetCollectionRequest();
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
            req.OnResponse = OnGetCollectionResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Collection, environmentID, collectionID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCollectionRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Collection> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Collection result = new Collection();
            fsData data = null;
            Dictionary<string, object> customData = ((GetCollectionRequest)req).CustomData;
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
                    Log.Error("Discovery.OnGetCollectionResponse()", "OnGetCollectionResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetCollectionRequest)req).SuccessCallback != null)
                    ((GetCollectionRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetCollectionRequest)req).FailCallback != null)
                    ((GetCollectionRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Delete Collection
        /// <summary>
        /// Deletes a specified collection.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool DeleteCollection(SuccessCallback<DeleteCollectionResponse> successCallback, FailCallback failCallback, string environmentID, string collectionID, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");

            DeleteCollectionRequest req = new DeleteCollectionRequest();
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
            req.OnResponse = OnDeleteCollectionResponse;
            req.Delete = true;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Collection, environmentID, collectionID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteCollectionRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<DeleteCollectionResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnDeleteCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DeleteCollectionResponse result = new DeleteCollectionResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((DeleteCollectionRequest)req).CustomData;
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
                    Log.Error("Discovery.OnDeleteCollectionResponse()", "OnDeleteCollectionResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((DeleteCollectionRequest)req).SuccessCallback != null)
                    ((DeleteCollectionRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteCollectionRequest)req).FailCallback != null)
                    ((DeleteCollectionRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Fields
        /// <summary>
        /// Gets a list of the the unique fields (and their types) stored in the index.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool GetFields(SuccessCallback<GetFieldsResponse> successCallback, FailCallback failCallback, string environmentID, string collectionID, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");

            GetFieldsRequest req = new GetFieldsRequest();
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
            req.OnResponse = OnGetFieldsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Fields, environmentID, collectionID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetFieldsRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<GetFieldsResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetFieldsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            GetFieldsResponse result = new GetFieldsResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((GetFieldsRequest)req).CustomData;
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
                    Log.Error("Discovery.OnGetFieldsResponse()", "OnGetFieldsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetFieldsRequest)req).SuccessCallback != null)
                    ((GetFieldsRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetFieldsRequest)req).FailCallback != null)
                    ((GetFieldsRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion
        #endregion

        #region Documents
        #region Add Document
        /// <summary>
        /// Add a document to a collection with optional metadata and optional configuration. The configuration to use to process 
        /// the document can be provided using the configuration_id argument. Returns immediately after the system has accepted the 
        /// document for processing. The user must provide document content, metadata, or both. If the request is missing both document 
        /// content and metadata, then it will be rejected.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="contentFilePath">The path to content file to be added.</param>
        /// <param name="configurationID">The ID of the configuration to use to process the document. If the configuration form part
        /// is also provided (both are present at the same time), then request will be rejected.</param>
        /// <param name="configurationFilePath">The content of the document to ingest.The maximum supported file size is 50 megabytes. 
        /// Files larger than 50 megabytes is rejected.</param>
        /// <param name="metadata">If you're using the Data Crawler to upload your documents, you can test a document against the type 
        /// of metadata that the Data Crawler might send. The maximum supported metadata file size is 1 MB. Metadata parts larger than 
        /// 1 MB are rejected. Example: { "Creator": "Johnny Appleseed", "Subject": "Apples" }</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool AddDocument(SuccessCallback<DocumentAccepted> successCallback, FailCallback failCallback, string environmentID, string collectionID, string contentFilePath, string configurationID = default(string), string configurationFilePath = default(string), string metadata = default(string), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (string.IsNullOrEmpty(contentFilePath))
                throw new ArgumentNullException("contentFilePath");
            if (string.IsNullOrEmpty(configurationID) && string.IsNullOrEmpty(configurationFilePath))
                throw new ArgumentNullException("Set either a configurationID or configurationFilePath");
            if (!string.IsNullOrEmpty(configurationID) && !string.IsNullOrEmpty(configurationFilePath))
                throw new WatsonException("Use either a configurationID OR designate a configurationFilePath - not both");
            if (string.IsNullOrEmpty(contentFilePath) && string.IsNullOrEmpty(metadata))
                throw new WatsonException("The user must provide document content, metadata, or both");

            byte[] contentData = null;
            string contentMimeType = default(string);

            try
            {
                contentData = File.ReadAllBytes(contentFilePath);
                contentMimeType = Utility.GetMimeType(Path.GetExtension(contentFilePath));
            }
            catch (Exception e)
            {
                throw new WatsonException(string.Format("Failed to load content: {0}", e.Message));
            }

            if (contentData == null || string.IsNullOrEmpty(contentMimeType))
                throw new WatsonException("Failed to load content");

            if (!string.IsNullOrEmpty(configurationID))
                return AddDocumentUsingConfigID(successCallback, failCallback, environmentID, collectionID, contentData, contentMimeType, configurationID, metadata, customData);
            else if (!string.IsNullOrEmpty(configurationFilePath))
                return AddDocumentUsingConfigFile(successCallback, failCallback, environmentID, collectionID, contentData, contentMimeType, configurationFilePath, metadata, customData);
            else
                throw new WatsonException("A configurationID or configuration file path is required");
        }

        /// <summary>
        /// Add a document to a collection with optional metadata and optional configuration. The configuration to use to process 
        /// the document can be provided using the configuration_id argument. Returns immediately after the system has accepted the 
        /// document for processing. The user must provide document content, metadata, or both. If the request is missing both document 
        /// content and metadata, then it will be rejected.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="contentData">A byte array of content to be ingested.</param>
        /// <param name="contentMimeType">The mimeType of the content data to be ingested.</param>
        /// <param name="configurationID">The identifier of the configuration to use to process the document.</param>
        /// <param name="metadata">If you're using the Data Crawler to upload your documents, you can test a document against the type 
        /// of metadata that the Data Crawler might send. The maximum supported metadata file size is 1 MB. Metadata parts larger than 
        /// 1 MB are rejected. Example: { "Creator": "Johnny Appleseed", "Subject": "Apples" }</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool AddDocumentUsingConfigID(SuccessCallback<DocumentAccepted> successCallback, FailCallback failCallback, string environmentID, string collectionID, byte[] contentData, string contentMimeType, string configurationID, string metadata = default(string), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (contentData == null)
                throw new ArgumentNullException("contentData");
            if (string.IsNullOrEmpty(contentMimeType))
                throw new ArgumentNullException("contentMimeType");
            if (string.IsNullOrEmpty(configurationID))
                throw new ArgumentNullException("configurationID");

            return AddDocument(successCallback, failCallback, environmentID, collectionID, contentData, contentMimeType, configurationID, null, metadata, customData);
        }

        /// <summary>
        /// Add a document to a collection with optional metadata and optional configuration. The configuration to use to process 
        /// the document can be provided using the configuration_id argument. Returns immediately after the system has accepted the 
        /// document for processing. The user must provide document content, metadata, or both. If the request is missing both document 
        /// content and metadata, then it will be rejected.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="contentData">A byte array of content to be ingested.</param>
        /// <param name="contentMimeType">The mimeType of the content data to be ingested.</param>
        /// <param name="configurationFilePath">The file path to the configuration to use to process the document.</param>
        /// <param name="metadata">If you're using the Data Crawler to upload your documents, you can test a document against the type 
        /// of metadata that the Data Crawler might send. The maximum supported metadata file size is 1 MB. Metadata parts larger than 
        /// 1 MB are rejected. Example: { "Creator": "Johnny Appleseed", "Subject": "Apples" }</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool AddDocumentUsingConfigFile(SuccessCallback<DocumentAccepted> successCallback, FailCallback failCallback, string environmentID, string collectionID, byte[] contentData, string contentMimeType, string configurationFilePath, string metadata = default(string), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (contentData == null)
                throw new ArgumentNullException("contentData");
            if (string.IsNullOrEmpty(contentMimeType))
                throw new ArgumentNullException("contentMimeType");
            if (string.IsNullOrEmpty(configurationFilePath))
                throw new ArgumentNullException("configurationFilePath");

            string configuration = default(string);

            if (!string.IsNullOrEmpty(configurationFilePath))
            {
                try
                {
                    configuration = File.ReadAllText(configurationFilePath);
                }
                catch (Exception e)
                {
                    throw new WatsonException(string.Format("Failed to load configuration json: {0}", e.Message));
                }
            }

            return AddDocument(successCallback, failCallback, environmentID, collectionID, contentData, contentMimeType, null, configuration, metadata, customData);
        }

        /// <summary>
        /// Add a document to a collection with optional metadata and optional configuration. The configuration to use to process 
        /// the document can be provided using the configuration_id argument. Returns immediately after the system has accepted the 
        /// document for processing. The user must provide document content, metadata, or both. If the request is missing both document 
        /// content and metadata, then it will be rejected.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="contentData">A byte array of content to be ingested.</param>
        /// <param name="contentMimeType">The mimeType of the content data to be ingested.</param>
        /// <param name="configurationID">The configuration identifier. If this is specified, do not specify a configuration.</param>
        /// <param name="configuration">A json string of the configuration to test. If this is specified, do not specify a configurationID.</param>
        /// <param name="metadata">If you're using the Data Crawler to upload your documents, you can test a document against the type of metadata 
        /// that the Data Crawler might send. The maximum supported metadata file size is 1 MB. Metadata parts larger than 1 MB are rejected. 
        /// Example: { "Creator": "Johnny Appleseed", "Subject": "Apples" }</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool AddDocument(SuccessCallback<DocumentAccepted> successCallback, FailCallback failCallback, string environmentID, string collectionID, byte[] contentData, string contentMimeType, string configurationID = default(string), string configuration = default(string), string metadata = default(string), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (contentData == null)
                throw new ArgumentNullException("contentData");
            if (string.IsNullOrEmpty(contentMimeType))
                throw new ArgumentNullException("contentMimeType");
            if (string.IsNullOrEmpty(configurationID) && string.IsNullOrEmpty(configuration))
                throw new ArgumentNullException("Set either a configurationID or test configuration string");
            if (!string.IsNullOrEmpty(configurationID) && !string.IsNullOrEmpty(configuration))
                throw new WatsonException("Use either a configurationID OR designate a test configuration string - not both");

            AddDocumentRequest req = new AddDocumentRequest();
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
            req.OnResponse = OnAddDocumentResponse;

            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["file"] = new RESTConnector.Form(contentData, "contentData", contentMimeType);

            if (!string.IsNullOrEmpty(metadata))
                req.Forms["metadata"] = new RESTConnector.Form(metadata);

            if (!string.IsNullOrEmpty(configurationID))
                req.Parameters["configuration_id"] = configurationID;

            if (!string.IsNullOrEmpty(configuration))
                req.Forms["configuration"] = new RESTConnector.Form(configuration);

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Documents, environmentID, collectionID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class AddDocumentRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<DocumentAccepted> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnAddDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DocumentAccepted result = new DocumentAccepted();
            fsData data = null;
            Dictionary<string, object> customData = ((AddDocumentRequest)req).CustomData;
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
                    Log.Error("Discovery.OnAddDocumentResponse()", "OnAddDocumentResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((AddDocumentRequest)req).SuccessCallback != null)
                    ((AddDocumentRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((AddDocumentRequest)req).FailCallback != null)
                    ((AddDocumentRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Delete Doucment
        /// <summary>
        /// 
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="documentID">The document identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool DeleteDocument(SuccessCallback<DeleteDocumentResponse> successCallback, FailCallback failCallback, string environmentID, string collectionID, string documentID, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (string.IsNullOrEmpty(documentID))
                throw new ArgumentNullException("documentID");

            DeleteDocumentRequest req = new DeleteDocumentRequest();
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
            req.OnResponse = OnDeleteDocumentResponse;
            req.Delete = true;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Document, environmentID, collectionID, documentID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteDocumentRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<DeleteDocumentResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnDeleteDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DeleteDocumentResponse result = new DeleteDocumentResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((DeleteDocumentRequest)req).CustomData;
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
                    Log.Error("Discovery.OnDeleteDocumentResponse()", "OnDeleteDocumentResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((DeleteDocumentRequest)req).SuccessCallback != null)
                    ((DeleteDocumentRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteDocumentRequest)req).FailCallback != null)
                    ((DeleteDocumentRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Document
        /// <summary>
        /// Lists a specified document's details.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="documentID">The document identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool GetDocument(SuccessCallback<DocumentStatus> successCallback, FailCallback failCallback, string environmentID, string collectionID, string documentID, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (string.IsNullOrEmpty(documentID))
                throw new ArgumentNullException("documentID");

            GetDocumentRequest req = new GetDocumentRequest();
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
            req.OnResponse = OnGetDocumentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Document, environmentID, collectionID, documentID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetDocumentRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<DocumentStatus> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DocumentStatus result = new DocumentStatus();
            fsData data = null;
            Dictionary<string, object> customData = ((GetDocumentRequest)req).CustomData;
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
                    Log.Error("Discovery.OnGetDocumentResponse()", "OnGetDocumentResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetDocumentRequest)req).SuccessCallback != null)
                    ((GetDocumentRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetDocumentRequest)req).FailCallback != null)
                    ((GetDocumentRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Update Document
        /// <summary>
        /// Updates a specified document.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="documentID">The document identifier.</param>
        /// <param name="contentFilePath">The file path to the updated document to be ingested.</param>
        /// <param name="configurationID">The configuration identifier to use for ingestion. If this is specified, do not specify configurationFilePath.</param>
        /// <param name="configurationFilePath">The path to a configuration file to use for ingestion. If this is specified, do not specify configurationID.</param>
        /// <param name="metadata">If you're using the Data Crawler to upload your documents, you can test a document against the type 
        /// of metadata that the Data Crawler might send. The maximum supported metadata file size is 1 MB. Metadata parts larger than 
        /// 1 MB are rejected. Example: { "Creator": "Johnny Appleseed", "Subject": "Apples" }</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool UpdateDocument(SuccessCallback<DocumentAccepted> successCallback, FailCallback failCallback, string environmentID, string collectionID, string documentID, string contentFilePath, string configurationID = default(string), string configurationFilePath = default(string), string metadata = default(string), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (string.IsNullOrEmpty(documentID))
                throw new ArgumentNullException("documentID");
            if (string.IsNullOrEmpty(contentFilePath))
                throw new ArgumentNullException("contentFilePath");
            if (string.IsNullOrEmpty(configurationID) && string.IsNullOrEmpty(configurationFilePath))
                throw new ArgumentNullException("Set either a configurationID or configurationFilePath");
            if (!string.IsNullOrEmpty(configurationID) && !string.IsNullOrEmpty(configurationFilePath))
                throw new WatsonException("Use either a configurationID OR designate a configurationFilePath - not both");

            byte[] contentData = null;
            string contentMimeType = default(string);

            try
            {
                contentData = File.ReadAllBytes(contentFilePath);
                contentMimeType = Utility.GetMimeType(Path.GetExtension(contentFilePath));
            }
            catch (Exception e)
            {
                throw new WatsonException(string.Format("Failed to load content: {0}", e.Message));
            }

            if (contentData == null || string.IsNullOrEmpty(contentMimeType))
                throw new WatsonException("Failed to load content");

            if (!string.IsNullOrEmpty(configurationID))
                return UpdateDocumentUsingConfigID(successCallback, failCallback, environmentID, collectionID, documentID, contentData, contentMimeType, configurationID, metadata, customData);
            else if (!string.IsNullOrEmpty(configurationFilePath))
                return UpdateDocumentUsingConfigFile(successCallback, failCallback, environmentID, collectionID, documentID, contentData, contentMimeType, configurationFilePath, metadata, customData);
            else
                throw new WatsonException("A configurationID or configuration file path is required");
        }

        /// <summary>
        /// Updates a specified document using ConfigID.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="documentID">The document identifier.</param>
        /// <param name="contentData">A byte array of content to be ingested.</param>
        /// <param name="contentMimeType">The mimeType of the content data to be ingested.</param>
        /// <param name="configurationID">The identifier of the configuration to use to process the document.</param>
        /// <param name="metadata">If you're using the Data Crawler to upload your documents, you can test a document against the type 
        /// of metadata that the Data Crawler might send. The maximum supported metadata file size is 1 MB. Metadata parts larger than 
        /// 1 MB are rejected. Example: { "Creator": "Johnny Appleseed", "Subject": "Apples" }</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool UpdateDocumentUsingConfigID(SuccessCallback<DocumentAccepted> successCallback, FailCallback failCallback, string environmentID, string collectionID, string documentID, byte[] contentData, string contentMimeType, string configurationID, string metadata = default(string), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (string.IsNullOrEmpty(documentID))
                throw new ArgumentNullException("documentID");
            if (contentData == null)
                throw new ArgumentNullException("contentData");
            if (string.IsNullOrEmpty(contentMimeType))
                throw new ArgumentNullException("contentMimeType");
            if (string.IsNullOrEmpty(configurationID))
                throw new ArgumentNullException("configurationID");

            return UpdateDocument(successCallback, failCallback, environmentID, collectionID, documentID, contentData, contentMimeType, configurationID, null, metadata, customData);
        }

        /// <summary>
        /// Updates a specified document using a configuration file path.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="documentID">The document identifier.</param>
        /// <param name="contentData">A byte array of content to be ingested.</param>
        /// <param name="contentMimeType">The mimeType of the content data to be ingested.</param>
        /// <param name="configurationFilePath">The file path to the configuration to use to process</param>
        /// <param name="metadata">If you're using the Data Crawler to upload your documents, you can test a document against the type 
        /// of metadata that the Data Crawler might send. The maximum supported metadata file size is 1 MB. Metadata parts larger than 
        /// 1 MB are rejected. Example: { "Creator": "Johnny Appleseed", "Subject": "Apples" }</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool UpdateDocumentUsingConfigFile(SuccessCallback<DocumentAccepted> successCallback, FailCallback failCallback, string environmentID, string collectionID, string documentID, byte[] contentData, string contentMimeType, string configurationFilePath, string metadata = default(string), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (string.IsNullOrEmpty(documentID))
                throw new ArgumentNullException("documentID");
            if (contentData == null)
                throw new ArgumentNullException("contentData");
            if (string.IsNullOrEmpty(contentMimeType))
                throw new ArgumentNullException("contentMimeType");
            if (string.IsNullOrEmpty(configurationFilePath))
                throw new ArgumentNullException("configurationFilePath");

            string configuration = default(string);

            if (!string.IsNullOrEmpty(configurationFilePath))
            {
                try
                {
                    configuration = File.ReadAllText(configurationFilePath);
                }
                catch (Exception e)
                {
                    throw new WatsonException(string.Format("Failed to load configuration json: {0}", e.Message));
                }
            }

            return UpdateDocument(successCallback, failCallback, environmentID, collectionID, documentID, contentData, contentMimeType, null, configuration, metadata, customData);
        }

        /// <summary>
        /// Updates a specified document.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="documentID">The document identifier.</param>
        /// <param name="contentData">A byte array of content to be updated.</param>
        /// <param name="contentMimeType">The mimeType of the content data to be updated.</param>
        /// <param name="configurationID">The configuration identifier. If this is specified, do not specify a configuration.</param>
        /// <param name="configuration">A json string of the configuration to test. If this is specified, do not specify a configurationID.</param>
        /// <param name="metadata">If you're using the Data Crawler to upload your documents, you can test a document against the type 
        /// of metadata that the Data Crawler might send. The maximum supported metadata file size is 1 MB. Metadata parts larger than 
        /// 1 MB are rejected. Example: { "Creator": "Johnny Appleseed", "Subject": "Apples" }</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool UpdateDocument(SuccessCallback<DocumentAccepted> successCallback, FailCallback failCallback, string environmentID, string collectionID, string documentID, byte[] contentData, string contentMimeType, string configurationID = default(string), string configuration = default(string), string metadata = default(string), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (string.IsNullOrEmpty(documentID))
                throw new ArgumentNullException("documentID");
            if (contentData == null)
                throw new ArgumentNullException("contentData");
            if (string.IsNullOrEmpty(contentMimeType))
                throw new ArgumentNullException("contentMimeType");
            if (string.IsNullOrEmpty(configurationID) && string.IsNullOrEmpty(configuration))
                throw new ArgumentNullException("Set either a configurationID or test configuration string");
            if (!string.IsNullOrEmpty(configurationID) && !string.IsNullOrEmpty(configuration))
                throw new WatsonException("Use either a configurationID OR designate a test configuration string - not both");

            UpdateDocumentRequest req = new UpdateDocumentRequest();
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
            req.OnResponse = OnUpdateDocumentResponse;

            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["file"] = new RESTConnector.Form(contentData, "contentData", contentMimeType);

            if (!string.IsNullOrEmpty(metadata))
                req.Forms["metadata"] = new RESTConnector.Form(metadata);

            if (!string.IsNullOrEmpty(configurationID))
                req.Parameters["configuration_id"] = configurationID;

            if (!string.IsNullOrEmpty(configuration))
                req.Forms["configuration"] = new RESTConnector.Form(configuration);

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Document, environmentID, collectionID, documentID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class UpdateDocumentRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<DocumentAccepted> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnUpdateDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DocumentAccepted result = new DocumentAccepted();
            fsData data = null;
            Dictionary<string, object> customData = ((UpdateDocumentRequest)req).CustomData;
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
                    Log.Error("Discovery.OnUpdateDocumentResponse()", "OnUpdateDocumentResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((UpdateDocumentRequest)req).SuccessCallback != null)
                    ((UpdateDocumentRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((UpdateDocumentRequest)req).FailCallback != null)
                    ((UpdateDocumentRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion
        #endregion

        #region Queries
        /// <summary>
        /// Query the discovery instance.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="filter">A cacheable query that limits the documents returned to exclude any documents that don't mention 
        /// the query content. Filter searches are better for metadata type searches and when you are trying to get a sense of concepts 
        /// in the data set.</param>
        /// <param name="query">A query search returns all documents in your data set with full enrichments and full text, but with the
        /// most relevant documents listed first. Use a query search when you want to find the most relevant search results.</param>
        /// <param name="aggregation">An aggregation search uses combinations of filters and query search to return an exact answer. 
        /// Aggregations are useful for building applications, because you can use them to build lists, tables, and time series. For a 
        /// full list of possible aggregrations, see the Query reference.</param>
        /// <param name="count">Number of documents to return.</param>
        /// <param name="_return">A comma separated list of the portion of the document hierarchy to return.</param>
        /// <param name="offset">The number of query results to skip at the beginning. For example, if the total number of results that
        /// are returned is 10, and the offset is 8, it returns the last two results.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool Query(SuccessCallback<QueryResponse> successCallback, FailCallback failCallback, string environmentID, string collectionID, string filter, string query, string aggregation, int count, string _return, int offset, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (string.IsNullOrEmpty(query))
                throw new ArgumentNullException("query");

            QueryRequest req = new QueryRequest();
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

            if (!string.IsNullOrEmpty(filter))
                req.Parameters["filter"] = filter;

            if (!string.IsNullOrEmpty(query))
                req.Parameters["query"] = query;

            if (!string.IsNullOrEmpty(aggregation))
                req.Parameters["aggregation"] = aggregation;

            if (!string.IsNullOrEmpty(_return))
                req.Parameters["_return"] = _return;

            req.Parameters["offset"] = offset;
            req.Parameters["count"] = count;
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnQueryResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(QueryCollection, environmentID, collectionID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class QueryRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<QueryResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnQueryResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            QueryResponse result = new QueryResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((QueryRequest)req).CustomData;
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
                    Log.Error("Discovery.OnQueryResponse()", "OnQueryResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((QueryRequest)req).SuccessCallback != null)
                    ((QueryRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((QueryRequest)req).FailCallback != null)
                    ((QueryRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Delete User Data
        /// <summary>
        /// Deletes all data associated with a specified customer ID. The method has no effect if no data is associated with the customer ID. 
        /// You associate a customer ID with data by passing the X-Watson-Metadata header with a request that passes data. 
        /// For more information about personal data and customer IDs, see [**Information security**](https://console.bluemix.net/docs/services/discovery/information-security.html).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="customerId">The customer ID for which all data is to be deleted.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool DeleteUserData(SuccessCallback<object> successCallback, FailCallback failCallback, string customerId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException("customerId");

            DeleteUserDataRequestObj req = new DeleteUserDataRequestObj();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Parameters["customer_id"] = customerId;
            req.Parameters["version"] = VersionDate;
            req.Delete = true;

            req.OnResponse = OnDeleteUserDataResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/user_data");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteUserDataRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<object> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnDeleteUserDataResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            object result = new object();
            fsData data = null;
            Dictionary<string, object> customData = ((DeleteUserDataRequestObj)req).CustomData;
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
                    Log.Error("Discovery.OnDeleteUserDataResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((DeleteUserDataRequestObj)req).SuccessCallback != null)
                    ((DeleteUserDataRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteUserDataRequestObj)req).FailCallback != null)
                    ((DeleteUserDataRequestObj)req).FailCallback(resp.Error, customData);
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
