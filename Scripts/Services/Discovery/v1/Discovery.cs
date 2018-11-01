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
        private const string CredentialsEndpoint = "/v1/environments/{0}/credentials";
        private const string CredentialEndpoint = "/v1/environments/{0}/credentials/{1}";
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

                if (string.IsNullOrEmpty(credentials.Url))
                {
                    credentials.Url = Url;
                }
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
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
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
        public bool AddEnvironment(SuccessCallback<Environment> successCallback, FailCallback failCallback, string name = default(string), string description = default(string), SizeEnum? size = null, Dictionary<string, object> customData = null)
        {
            CreateEnvironmentRequest createEnvironmentRequest = new CreateEnvironmentRequest();
            if(!string.IsNullOrEmpty(name))
            {
                createEnvironmentRequest.Name = name;
            }

            if(!string.IsNullOrEmpty(description))
            {
                createEnvironmentRequest.Description = description;
            }

            if(size != null)
            {
                createEnvironmentRequest.Size = size;
            }
            

            return AddEnvironment(successCallback, failCallback, createEnvironmentRequest, customData);
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
        [Obsolete("Use AddEnvironment with CreateEnvironmentRequest instead.")]
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
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
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

        /// <summary>
        /// Creates a new environment. You can only create one environment per service instance.An attempt to create another environment 
        /// will result in an error. The size of the new environment can be controlled by specifying the size parameter.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="createEnvironmentRequest">An object that defines an environment name and optional description. The fields in this object are not approved for personal information and cannot be deleted based on customer ID.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool AddEnvironment(SuccessCallback<Environment> successCallback, FailCallback failCallback, CreateEnvironmentRequest createEnvironmentRequest, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            AddEnvironmentRequest req = new AddEnvironmentRequest();
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
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnAddEnvironmentResponse;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            fsData data = null;
            _serializer.TrySerialize(createEnvironmentRequest, out data);
            string json = data.ToString().Replace('\"', '"');
            req.Send = Encoding.UTF8.GetBytes(json);

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
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
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
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
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
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
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
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
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
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
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
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
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
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
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
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
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
        public bool AddCollection(SuccessCallback<CollectionRef> successCallback, FailCallback failCallback, string environmentID, string name, string description = default(string), string configurationID = default(string), string language = default(string), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(language))
                throw new ArgumentNullException("language");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["name"] = name;
            parameters["description"] = description;
            parameters["configuration_id"] = configurationID;
            parameters["language"] = language;

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
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
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
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
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
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
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
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
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
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
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
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
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
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
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
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
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
        /// Query documents in multiple collections.
        ///
        /// See the [Discovery service documentation](https://console.bluemix.net/docs/services/discovery/using.html)
        /// for more details.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionIds">A comma-separated list of collection IDs to be queried against.</param>
        /// <param name="filter">A cacheable query that limits the documents returned to exclude any documents that
        /// don't mention the query content. Filter searches are better for metadata type searches and when you are
        /// trying to get a sense of concepts in the data set. (optional)</param>
        /// <param name="query">A query search returns all documents in your data set with full enrichments and full
        /// text, but with the most relevant documents listed first. Use a query search when you want to find the most
        /// relevant search results. You cannot use **natural_language_query** and **query** at the same time.
        /// (optional)</param>
        /// <param name="naturalLanguageQuery">A natural language query that returns relevant documents by utilizing
        /// training data and natural language understanding. You cannot use **natural_language_query** and **query** at
        /// the same time. (optional)</param>
        /// <param name="aggregation">An aggregation search uses combinations of filters and query search to return an
        /// exact answer. Aggregations are useful for building applications, because you can use them to build lists,
        /// tables, and time series. For a full list of possible aggregrations, see the Query reference.
        /// (optional)</param>
        /// <param name="count">Number of results to return. (optional, default to 10)</param>
        /// <param name="returnFields">A comma separated list of the portion of the document hierarchy to return.
        /// (optional)</param>
        /// <param name="offset">The number of query results to skip at the beginning. For example, if the total number
        /// of results that are returned is 10, and the offset is 8, it returns the last two results. (optional)</param>
        /// <param name="sort">A comma separated list of fields in the document to sort on. You can optionally specify a
        /// sort direction by prefixing the field with `-` for descending or `+` for ascending. Ascending is the default
        /// sort direction if no prefix is specified. (optional)</param>
        /// <param name="highlight">When true a highlight field is returned for each result which contains the fields
        /// that match the query with `<em></em>` tags around the matching query terms. Defaults to false.
        /// (optional)</param>
        /// <param name="deduplicate">When `true` and used with a Watson Discovery News collection, duplicate results
        /// (based on the contents of the **title** field) are removed. Duplicate comparison is limited to the current
        /// query only; **offset** is not considered. This parameter is currently Beta functionality. (optional, default
        /// to false)</param>
        /// <param name="deduplicateField">When specified, duplicate results based on the field specified are removed
        /// from the returned results. Duplicate comparison is limited to the current query only, **offset** is not
        /// considered. This parameter is currently Beta functionality. (optional)</param>
        /// <param name="similar">When `true`, results are returned based on their similarity to the document IDs
        /// specified in the **similar.document_ids** parameter. (optional, default to false)</param>
        /// <param name="similarDocumentIds">A comma-separated list of document IDs that will be used to find similar
        /// documents.
        ///
        /// **Note:** If the **natural_language_query** parameter is also specified, it will be used to expand the scope
        /// of the document similarity search to include the natural language query. Other query parameters, such as
        /// **filter** and **query** are subsequently applied and reduce the query scope. (optional)</param>
        /// <param name="similarFields">A comma-separated list of field names that will be used as a basis for
        /// comparison to identify similar documents. If not specified, the entire document is used for comparison.
        /// (optional)</param>
        /// <param name="passages">A passages query that returns the most relevant passages from the results.
        /// (optional)</param>
        /// <param name="passagesFields">A comma-separated list of fields that passages are drawn from. If this
        /// parameter not specified, then all top-level fields are included. (optional)</param>
        /// <param name="passagesCount">The maximum number of passages to return. The search returns fewer passages if
        /// the requested total is not found. The default is `10`. The maximum is `100`. (optional)</param>
        /// <param name="passagesCharacters">The approximate number of characters that any one passage will have. The
        /// default is `400`. The minimum is `50`. The maximum is `2000`. (optional)</param>
        /// <returns><see cref="QueryResponse" />QueryResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool FederatedQuery(SuccessCallback<QueryResponse> successCallback, FailCallback failCallback, string environmentId, List<string> collectionIds, string filter = null, string query = null, string naturalLanguageQuery = null, string aggregation = null, long? count = null, List<string> returnFields = null, long? offset = null, List<string> sort = null, bool? highlight = null, bool? deduplicate = null, string deduplicateField = null, bool? similar = null, List<string> similarDocumentIds = null, List<string> similarFields = null, bool? passages = null, List<string> passagesFields = null, long? passagesCount = null, long? passagesCharacters = null, string bias = null, string loggingOptOut = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentId))
            {
                throw new ArgumentNullException("environmentId");
            }
            if(collectionIds == null || collectionIds.Count < 1)
            {
                throw new ArgumentNullException("collectionId");
            }

            FederatedQueryRequestObj req = new FederatedQueryRequestObj();
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
            req.Parameters["version"] = VersionDate;
            if (loggingOptOut != null)
            {
                req.Headers.Add("X-Watson-Logging-Opt-Out", loggingOptOut.ToString());
            }
            req.Headers["Content-Type"] = "application/json";

            QueryLarge queryLarge = new QueryLarge()
            {
                Filter = filter,
                Query = query,
                NaturalLanguageQuery = naturalLanguageQuery,
                Passages = passages,
                Aggregation = aggregation,
                Count = count,
                ReturnFields = (returnFields == null || returnFields.Count < 1) ? null : string.Join(", ", returnFields.ToArray()),
                Offset = offset,
                Sort = (sort == null || sort.Count < 1) ? null : string.Join(", ", sort.ToArray()),
                Highlight = highlight,
                PassagesFields = (passagesFields == null || passagesFields.Count < 1) ? null : string.Join(", ", passagesFields.ToArray()),
                PassagesCount = passagesCount,
                PassagesCharacters = passagesCharacters,
                Deduplicate = deduplicate,
                DeduplicateField = deduplicateField,
                CollectionIds = (collectionIds == null || collectionIds.Count < 1) ? null : string.Join(", ", collectionIds.ToArray()),
                Similar = similar,
                SimilarDocumentIds = (similarDocumentIds == null || similarDocumentIds.Count < 1) ? null : string.Join(", ", similarDocumentIds.ToArray()),
                SimilarFields = (similarFields == null || similarFields.Count < 1) ? null : string.Join(", ", similarFields.ToArray()),
                Bias = bias
            };

            fsData data = null;
            _serializer.TrySerialize(queryLarge, out data);
            string json = data.ToString().Replace('\"', '"');
            req.Send = Encoding.UTF8.GetBytes(json);

            req.OnResponse = OnFederatedQueryResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/query", environmentId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class FederatedQueryRequestObj : RESTConnector.Request
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

        private void OnFederatedQueryResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            QueryResponse result = new QueryResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((FederatedQueryRequestObj)req).CustomData;

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
                    Log.Error("Discovery.OnFederatedQueryResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((FederatedQueryRequestObj)req).SuccessCallback != null)
                    ((FederatedQueryRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((FederatedQueryRequestObj)req).FailCallback != null)
                    ((FederatedQueryRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Query multiple collection system notices.
        ///
        /// Queries for notices (errors or warnings) that might have been generated by the system. Notices are generated
        /// when ingesting documents and performing relevance training. See the [Discovery service
        /// documentation](https://console.bluemix.net/docs/services/discovery/using.html) for more details on the query
        /// language.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionIds">A comma-separated list of collection IDs to be queried against.</param>
        /// <param name="filter">A cacheable query that limits the documents returned to exclude any documents that
        /// don't mention the query content. Filter searches are better for metadata type searches and when you are
        /// trying to get a sense of concepts in the data set. (optional)</param>
        /// <param name="query">A query search returns all documents in your data set with full enrichments and full
        /// text, but with the most relevant documents listed first. Use a query search when you want to find the most
        /// relevant search results. You cannot use **natural_language_query** and **query** at the same time.
        /// (optional)</param>
        /// <param name="naturalLanguageQuery">A natural language query that returns relevant documents by utilizing
        /// training data and natural language understanding. You cannot use **natural_language_query** and **query** at
        /// the same time. (optional)</param>
        /// <param name="aggregation">An aggregation search uses combinations of filters and query search to return an
        /// exact answer. Aggregations are useful for building applications, because you can use them to build lists,
        /// tables, and time series. For a full list of possible aggregrations, see the Query reference.
        /// (optional)</param>
        /// <param name="count">Number of results to return. (optional, default to 10)</param>
        /// <param name="returnFields">A comma separated list of the portion of the document hierarchy to return.
        /// (optional)</param>
        /// <param name="offset">The number of query results to skip at the beginning. For example, if the total number
        /// of results that are returned is 10, and the offset is 8, it returns the last two results. (optional)</param>
        /// <param name="sort">A comma separated list of fields in the document to sort on. You can optionally specify a
        /// sort direction by prefixing the field with `-` for descending or `+` for ascending. Ascending is the default
        /// sort direction if no prefix is specified. (optional)</param>
        /// <param name="highlight">When true a highlight field is returned for each result which contains the fields
        /// that match the query with `<em></em>` tags around the matching query terms. Defaults to false.
        /// (optional)</param>
        /// <param name="deduplicateField">When specified, duplicate results based on the field specified are removed
        /// from the returned results. Duplicate comparison is limited to the current query only, **offset** is not
        /// considered. This parameter is currently Beta functionality. (optional)</param>
        /// <param name="similar">When `true`, results are returned based on their similarity to the document IDs
        /// specified in the **similar.document_ids** parameter. (optional, default to false)</param>
        /// <param name="similarDocumentIds">A comma-separated list of document IDs that will be used to find similar
        /// documents.
        ///
        /// **Note:** If the **natural_language_query** parameter is also specified, it will be used to expand the scope
        /// of the document similarity search to include the natural language query. Other query parameters, such as
        /// **filter** and **query** are subsequently applied and reduce the query scope. (optional)</param>
        /// <param name="similarFields">A comma-separated list of field names that will be used as a basis for
        /// comparison to identify similar documents. If not specified, the entire document is used for comparison.
        /// (optional)</param>
        /// <returns><see cref="QueryNoticesResponse" />QueryNoticesResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool FederatedQueryNotices(SuccessCallback<QueryNoticesResponse> successCallback, FailCallback failCallback, string environmentId, List<string> collectionIds, string filter = null, string query = null, string naturalLanguageQuery = null, string aggregation = null, long? count = null, List<string> returnFields = null, long? offset = null, List<string> sort = null, bool? highlight = null, string deduplicateField = null, bool? similar = null, List<string> similarDocumentIds = null, List<string> similarFields = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            FederatedQueryNoticesRequestObj req = new FederatedQueryNoticesRequestObj();
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
            req.Parameters["version"] = VersionDate;
            if (collectionIds != null)
                req.Parameters["collection_ids"] = collectionIds != null && collectionIds.Count > 0 ? string.Join(",", collectionIds.ToArray()) : null;
            if (!string.IsNullOrEmpty(filter))
                req.Parameters["filter"] = filter;
            if (!string.IsNullOrEmpty(query))
                req.Parameters["query"] = query;
            if (!string.IsNullOrEmpty(naturalLanguageQuery))
                req.Parameters["natural_language_query"] = naturalLanguageQuery;
            if (!string.IsNullOrEmpty(aggregation))
                req.Parameters["aggregation"] = aggregation;
            if (count != null)
                req.Parameters["count"] = count;
            if (returnFields != null)
                req.Parameters["return"] = returnFields != null && returnFields.Count > 0 ? string.Join(",", returnFields.ToArray()) : null;
            if (offset != null)
                req.Parameters["offset"] = offset;
            if (sort != null)
                req.Parameters["sort"] = sort != null && sort.Count > 0 ? string.Join(",", sort.ToArray()) : null;
            if (highlight != null)
                req.Parameters["highlight"] = highlight;
            if (!string.IsNullOrEmpty(deduplicateField))
                req.Parameters["deduplicate.field"] = deduplicateField;
            if (similar != null)
                req.Parameters["similar"] = similar;
            if (similarDocumentIds != null)
                req.Parameters["similar.document_ids"] = similarDocumentIds != null && similarDocumentIds.Count > 0 ? string.Join(",", similarDocumentIds.ToArray()) : null;
            if (similarFields != null)
                req.Parameters["similar.fields"] = similarFields != null && similarFields.Count > 0 ? string.Join(",", similarFields.ToArray()) : null;
            req.OnResponse = OnFederatedQueryNoticesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/notices", environmentId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class FederatedQueryNoticesRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<QueryNoticesResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnFederatedQueryNoticesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            QueryNoticesResponse result = new QueryNoticesResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((FederatedQueryNoticesRequestObj)req).CustomData;

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
                    Log.Error("Discovery.OnFederatedQueryNoticesResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((FederatedQueryNoticesRequestObj)req).SuccessCallback != null)
                    ((FederatedQueryNoticesRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((FederatedQueryNoticesRequestObj)req).FailCallback != null)
                    ((FederatedQueryNoticesRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Query your collection.
        ///
        /// After your content is uploaded and enriched by the Discovery service, you can build queries to search your
        /// content. For details, see the [Discovery service
        /// documentation](https://console.bluemix.net/docs/services/discovery/using.html).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="filter">A cacheable query that limits the documents returned to exclude any documents that
        /// don't mention the query content. Filter searches are better for metadata type searches and when you are
        /// trying to get a sense of concepts in the data set. (optional)</param>
        /// <param name="query">A query search returns all documents in your data set with full enrichments and full
        /// text, but with the most relevant documents listed first. Use a query search when you want to find the most
        /// relevant search results. You cannot use **natural_language_query** and **query** at the same time.
        /// (optional)</param>
        /// <param name="naturalLanguageQuery">A natural language query that returns relevant documents by utilizing
        /// training data and natural language understanding. You cannot use **natural_language_query** and **query** at
        /// the same time. (optional)</param>
        /// <param name="passages">A passages query that returns the most relevant passages from the results.
        /// (optional)</param>
        /// <param name="aggregation">An aggregation search uses combinations of filters and query search to return an
        /// exact answer. Aggregations are useful for building applications, because you can use them to build lists,
        /// tables, and time series. For a full list of possible aggregrations, see the Query reference.
        /// (optional)</param>
        /// <param name="count">Number of results to return. (optional, default to 10)</param>
        /// <param name="returnFields">A comma separated list of the portion of the document hierarchy to return.
        /// (optional)</param>
        /// <param name="offset">The number of query results to skip at the beginning. For example, if the total number
        /// of results that are returned is 10, and the offset is 8, it returns the last two results. (optional)</param>
        /// <param name="sort">A comma separated list of fields in the document to sort on. You can optionally specify a
        /// sort direction by prefixing the field with `-` for descending or `+` for ascending. Ascending is the default
        /// sort direction if no prefix is specified. (optional)</param>
        /// <param name="highlight">When true a highlight field is returned for each result which contains the fields
        /// that match the query with `<em></em>` tags around the matching query terms. Defaults to false.
        /// (optional)</param>
        /// <param name="passagesFields">A comma-separated list of fields that passages are drawn from. If this
        /// parameter not specified, then all top-level fields are included. (optional)</param>
        /// <param name="passagesCount">The maximum number of passages to return. The search returns fewer passages if
        /// the requested total is not found. The default is `10`. The maximum is `100`. (optional)</param>
        /// <param name="passagesCharacters">The approximate number of characters that any one passage will have. The
        /// default is `400`. The minimum is `50`. The maximum is `2000`. (optional)</param>
        /// <param name="deduplicate">When `true` and used with a Watson Discovery News collection, duplicate results
        /// (based on the contents of the **title** field) are removed. Duplicate comparison is limited to the current
        /// query only; **offset** is not considered. This parameter is currently Beta functionality. (optional, default
        /// to false)</param>
        /// <param name="deduplicateField">When specified, duplicate results based on the field specified are removed
        /// from the returned results. Duplicate comparison is limited to the current query only, **offset** is not
        /// considered. This parameter is currently Beta functionality. (optional)</param>
        /// <param name="similar">When `true`, results are returned based on their similarity to the document IDs
        /// specified in the **similar.document_ids** parameter. (optional, default to false)</param>
        /// <param name="similarDocumentIds">A comma-separated list of document IDs that will be used to find similar
        /// documents.
        ///
        /// **Note:** If the **natural_language_query** parameter is also specified, it will be used to expand the scope
        /// of the document similarity search to include the natural language query. Other query parameters, such as
        /// **filter** and **query** are subsequently applied and reduce the query scope. (optional)</param>
        /// <param name="similarFields">A comma-separated list of field names that will be used as a basis for
        /// comparison to identify similar documents. If not specified, the entire document is used for comparison.
        /// (optional)</param>
        /// <param name="loggingOptOut">If `true`, queries are not stored in the Discovery **Logs** endpoint. (optional,
        /// default to false)</param>
        /// <returns><see cref="QueryResponse" />QueryResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool Query(SuccessCallback<QueryResponse> successCallback, FailCallback failCallback, string environmentId, string collectionId, string filter = null, string query = null, string naturalLanguageQuery = null, bool? passages = null, string aggregation = null, long? count = null, List<string> returnFields = null, long? offset = null, List<string> sort = null, bool? highlight = null, List<string> passagesFields = null, long? passagesCount = null, long? passagesCharacters = null, bool? deduplicate = null, string deduplicateField = null, bool? similar = null, List<string> similarDocumentIds = null, List<string> similarFields = null, string bias = null, bool? loggingOptOut = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            QueryRequestObj req = new QueryRequestObj();
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
            req.Parameters["version"] = VersionDate;
            if (loggingOptOut != null)
            {
                req.Headers.Add("X-Watson-Logging-Opt-Out", loggingOptOut.ToString());
            }
            req.Headers["Content-Type"] = "application/json";

            QueryLarge queryLarge = new QueryLarge()
            {
                Filter = filter,
                Query = query,
                NaturalLanguageQuery = naturalLanguageQuery,
                Passages = passages,
                Aggregation = aggregation,
                Count = count,
                ReturnFields = (returnFields == null || returnFields.Count < 1) ? null : string.Join(", ", returnFields.ToArray()),
                Offset = offset,
                Sort = (sort == null || sort.Count < 1) ? null : string.Join(", ", sort.ToArray()),
                Highlight = highlight,
                PassagesFields = (passagesFields == null || passagesFields.Count < 1) ? null : string.Join(", ", passagesFields.ToArray()),
                PassagesCount = passagesCount,
                PassagesCharacters = passagesCharacters,
                Deduplicate = deduplicate,
                DeduplicateField = deduplicateField,
                Similar = similar,
                SimilarDocumentIds = (similarDocumentIds == null || similarDocumentIds.Count < 1) ? null : string.Join(", ", similarDocumentIds.ToArray()),
                SimilarFields = (similarFields == null || similarFields.Count < 1) ? null : string.Join(", ", similarFields.ToArray()),
                Bias = bias
            };

            fsData data = null;
            _serializer.TrySerialize(queryLarge, out data);
            string json = data.ToString().Replace('\"', '"');
            req.Send = Encoding.UTF8.GetBytes(json);

            req.OnResponse = OnQueryResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/query", environmentId, collectionId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class QueryRequestObj : RESTConnector.Request
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
            Dictionary<string, object> customData = ((QueryRequestObj)req).CustomData;

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
                    Log.Error("Discovery.OnQueryResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((QueryRequestObj)req).SuccessCallback != null)
                    ((QueryRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((QueryRequestObj)req).FailCallback != null)
                    ((QueryRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Knowledge Graph entity query.
        ///
        /// See the [Knowledge Graph
        /// documentation](https://console.bluemix.net/docs/services/discovery/building-kg.html) for more details.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="entityQuery">An object specifying the entities to query, which functions to perform, and any
        /// additional constraints.</param>
        /// <returns><see cref="QueryEntitiesResponse" />QueryEntitiesResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool QueryEntities(SuccessCallback<QueryEntitiesResponse> successCallback, FailCallback failCallback, string environmentId, string collectionId, QueryEntities entityQuery, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            QueryEntitiesRequestObj req = new QueryEntitiesRequestObj();
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
            req.Parameters["version"] = VersionDate;

            fsData data = null;
            _serializer.TrySerialize(entityQuery, out data);
            string json = data.ToString().Replace('\"', '"');
            req.Send = Encoding.UTF8.GetBytes(json);

            req.OnResponse = OnQueryEntitiesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/query_entities", environmentId, collectionId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class QueryEntitiesRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<QueryEntitiesResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnQueryEntitiesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            QueryEntitiesResponse result = new QueryEntitiesResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((QueryEntitiesRequestObj)req).CustomData;

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
                    Log.Error("Discovery.OnQueryEntitiesResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((QueryEntitiesRequestObj)req).SuccessCallback != null)
                    ((QueryEntitiesRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((QueryEntitiesRequestObj)req).FailCallback != null)
                    ((QueryEntitiesRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Query system notices.
        ///
        /// Queries for notices (errors or warnings) that might have been generated by the system. Notices are generated
        /// when ingesting documents and performing relevance training. See the [Discovery service
        /// documentation](https://console.bluemix.net/docs/services/discovery/using.html) for more details on the query
        /// language.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="filter">A cacheable query that limits the documents returned to exclude any documents that
        /// don't mention the query content. Filter searches are better for metadata type searches and when you are
        /// trying to get a sense of concepts in the data set. (optional)</param>
        /// <param name="query">A query search returns all documents in your data set with full enrichments and full
        /// text, but with the most relevant documents listed first. Use a query search when you want to find the most
        /// relevant search results. You cannot use **natural_language_query** and **query** at the same time.
        /// (optional)</param>
        /// <param name="naturalLanguageQuery">A natural language query that returns relevant documents by utilizing
        /// training data and natural language understanding. You cannot use **natural_language_query** and **query** at
        /// the same time. (optional)</param>
        /// <param name="passages">A passages query that returns the most relevant passages from the results.
        /// (optional)</param>
        /// <param name="aggregation">An aggregation search uses combinations of filters and query search to return an
        /// exact answer. Aggregations are useful for building applications, because you can use them to build lists,
        /// tables, and time series. For a full list of possible aggregrations, see the Query reference.
        /// (optional)</param>
        /// <param name="count">Number of results to return. (optional, default to 10)</param>
        /// <param name="returnFields">A comma separated list of the portion of the document hierarchy to return.
        /// (optional)</param>
        /// <param name="offset">The number of query results to skip at the beginning. For example, if the total number
        /// of results that are returned is 10, and the offset is 8, it returns the last two results. (optional)</param>
        /// <param name="sort">A comma separated list of fields in the document to sort on. You can optionally specify a
        /// sort direction by prefixing the field with `-` for descending or `+` for ascending. Ascending is the default
        /// sort direction if no prefix is specified. (optional)</param>
        /// <param name="highlight">When true a highlight field is returned for each result which contains the fields
        /// that match the query with `<em></em>` tags around the matching query terms. Defaults to false.
        /// (optional)</param>
        /// <param name="passagesFields">A comma-separated list of fields that passages are drawn from. If this
        /// parameter not specified, then all top-level fields are included. (optional)</param>
        /// <param name="passagesCount">The maximum number of passages to return. The search returns fewer passages if
        /// the requested total is not found. The default is `10`. The maximum is `100`. (optional)</param>
        /// <param name="passagesCharacters">The approximate number of characters that any one passage will have. The
        /// default is `400`. The minimum is `50`. The maximum is `2000`. (optional)</param>
        /// <param name="deduplicateField">When specified, duplicate results based on the field specified are removed
        /// from the returned results. Duplicate comparison is limited to the current query only, **offset** is not
        /// considered. This parameter is currently Beta functionality. (optional)</param>
        /// <param name="similar">When `true`, results are returned based on their similarity to the document IDs
        /// specified in the **similar.document_ids** parameter. (optional, default to false)</param>
        /// <param name="similarDocumentIds">A comma-separated list of document IDs that will be used to find similar
        /// documents.
        ///
        /// **Note:** If the **natural_language_query** parameter is also specified, it will be used to expand the scope
        /// of the document similarity search to include the natural language query. Other query parameters, such as
        /// **filter** and **query** are subsequently applied and reduce the query scope. (optional)</param>
        /// <param name="similarFields">A comma-separated list of field names that will be used as a basis for
        /// comparison to identify similar documents. If not specified, the entire document is used for comparison.
        /// (optional)</param>
        /// <returns><see cref="QueryNoticesResponse" />QueryNoticesResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool QueryNotices(SuccessCallback<QueryNoticesResponse> successCallback, FailCallback failCallback, string environmentId, string collectionId, string filter = null, string query = null, string naturalLanguageQuery = null, bool? passages = null, string aggregation = null, long? count = null, List<string> returnFields = null, long? offset = null, List<string> sort = null, bool? highlight = null, List<string> passagesFields = null, long? passagesCount = null, long? passagesCharacters = null, string deduplicateField = null, bool? similar = null, List<string> similarDocumentIds = null, List<string> similarFields = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            QueryNoticesRequestObj req = new QueryNoticesRequestObj();
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
            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(filter))
                req.Parameters["filter"] = filter;
            if (!string.IsNullOrEmpty(query))
                req.Parameters["query"] = query;
            if (!string.IsNullOrEmpty(naturalLanguageQuery))
                req.Parameters["natural_language_query"] = naturalLanguageQuery;
            if (passages != null)
                req.Parameters["passages"] = passages;
            if (!string.IsNullOrEmpty(aggregation))
                req.Parameters["aggregation"] = aggregation;
            if (count != null)
                req.Parameters["count"] = count;
            if (returnFields != null)
                req.Parameters["return"] = returnFields != null && returnFields.Count > 0 ? string.Join(",", returnFields.ToArray()) : null;
            if (offset != null)
                req.Parameters["offset"] = offset;
            if (sort != null)
                req.Parameters["sort"] = sort != null && sort.Count > 0 ? string.Join(",", sort.ToArray()) : null;
            if (highlight != null)
                req.Parameters["highlight"] = highlight;
            if(passagesFields != null)
                req.Parameters["passages.fields"] = passagesFields != null && passagesFields.Count > 0 ? string.Join(",", passagesFields.ToArray()) : null;
            if (passagesCount != null)
                req.Parameters["passages.count"] = passagesCount;
            if (passagesCharacters != null)
                req.Parameters["passages.characters"] = passagesCharacters;
            if (!string.IsNullOrEmpty(deduplicateField))
                req.Parameters["deduplicate.field"] = deduplicateField;
            if (similar != null)
                req.Parameters["similar"] = similar;
            if (similarDocumentIds != null)
                req.Parameters["similar.document_ids"] = similarDocumentIds != null && similarDocumentIds.Count > 0 ? string.Join(",", similarDocumentIds.ToArray()) : null;
            if (similarFields != null)
                req.Parameters["similar.fields"] = similarFields != null && similarFields.Count > 0 ? string.Join(",", similarFields.ToArray()) : null;
            req.OnResponse = OnQueryNoticesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/notices", environmentId, collectionId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class QueryNoticesRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<QueryNoticesResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnQueryNoticesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            QueryNoticesResponse result = new QueryNoticesResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((QueryNoticesRequestObj)req).CustomData;

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
                    Log.Error("Discovery.OnQueryNoticesResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((QueryNoticesRequestObj)req).SuccessCallback != null)
                    ((QueryNoticesRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((QueryNoticesRequestObj)req).FailCallback != null)
                    ((QueryNoticesRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Knowledge Graph relationship query.
        ///
        /// See the [Knowledge Graph
        /// documentation](https://console.bluemix.net/docs/services/discovery/building-kg.html) for more details.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="relationshipQuery">An object that describes the relationships to be queried and any query
        /// constraints (such as filters).</param>
        /// <returns><see cref="QueryRelationsResponse" />QueryRelationsResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool QueryRelations(SuccessCallback<QueryRelationsResponse> successCallback, FailCallback failCallback, string environmentId, string collectionId, QueryRelations relationshipQuery, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            QueryRelationsRequestObj req = new QueryRelationsRequestObj();
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
            req.Parameters["version"] = VersionDate;

            fsData data = null;
            _serializer.TrySerialize(relationshipQuery, out data);
            string json = data.ToString().Replace('\"', '"');
            req.Send = Encoding.UTF8.GetBytes(json);

            req.OnResponse = OnQueryRelationsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/query_relations", environmentId, collectionId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class QueryRelationsRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<QueryRelationsResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnQueryRelationsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            QueryRelationsResponse result = new QueryRelationsResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((QueryRelationsRequestObj)req).CustomData;

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
                    Log.Error("Discovery.OnQueryRelationsResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((QueryRelationsRequestObj)req).SuccessCallback != null)
                    ((QueryRelationsRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((QueryRelationsRequestObj)req).FailCallback != null)
                    ((QueryRelationsRequestObj)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Expansions
        /// <summary>
        /// Create or update expansion list.
        ///
        /// Create or replace the Expansion list for this collection. The maximum number of expanded terms per
        /// collection is `500`.
        /// The current expansion list is replaced with the uploaded content.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="body">An object that defines the expansion list.</param>
        /// <returns><see cref="Expansions" />Expansions</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateExpansions(SuccessCallback<Expansions> successCallback, FailCallback failCallback, string environmentId, string collectionId, Expansions body, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            CreateExpansionsRequestObj req = new CreateExpansionsRequestObj();
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
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnCreateExpansionsResponse;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            fsData data = null;
            _serializer.TrySerialize(body, out data);
            string json = data.ToString().Replace('\"', '"');
            req.Send = Encoding.UTF8.GetBytes(json);

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/expansions", environmentId, collectionId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class CreateExpansionsRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Expansions> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnCreateExpansionsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Expansions result = new Expansions();
            fsData data = null;
            Dictionary<string, object> customData = ((CreateExpansionsRequestObj)req).CustomData;

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
                    Log.Error("Discovery.OnCreateExpansionsResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((CreateExpansionsRequestObj)req).SuccessCallback != null)
                    ((CreateExpansionsRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((CreateExpansionsRequestObj)req).FailCallback != null)
                    ((CreateExpansionsRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Delete the expansion list.
        ///
        /// Remove the expansion information for this collection. The expansion list must be deleted to disable query
        /// expansion for a collection.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <returns><see cref="" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteExpansions(SuccessCallback<object> successCallback, FailCallback failCallback, string environmentId, string collectionId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            DeleteExpansionsRequestObj req = new DeleteExpansionsRequestObj();
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
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnDeleteExpansionsResponse;
            req.Delete = true;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/expansions", environmentId, collectionId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteExpansionsRequestObj : RESTConnector.Request
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

        private void OnDeleteExpansionsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            object result = new object();
            Dictionary<string, object> customData = ((DeleteExpansionsRequestObj)req).CustomData;

            if (resp.Success)
            {
                if (((DeleteExpansionsRequestObj)req).SuccessCallback != null)
                    ((DeleteExpansionsRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteExpansionsRequestObj)req).FailCallback != null)
                    ((DeleteExpansionsRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Get the expansion list.
        ///
        /// Returns the current expansion list for the specified collection. If an expansion list is not specified, an
        /// object with empty expansion arrays is returned.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <returns><see cref="Expansions" />Expansions</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListExpansions(SuccessCallback<Expansions> successCallback, FailCallback failCallback, string environmentId, string collectionId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            ListExpansionsRequestObj req = new ListExpansionsRequestObj();
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
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnListExpansionsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/expansions", environmentId, collectionId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ListExpansionsRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Expansions> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnListExpansionsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Expansions result = new Expansions();
            fsData data = null;
            Dictionary<string, object> customData = ((ListExpansionsRequestObj)req).CustomData;

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
                    Log.Error("Discovery.OnListExpansionsResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((ListExpansionsRequestObj)req).SuccessCallback != null)
                    ((ListExpansionsRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ListExpansionsRequestObj)req).FailCallback != null)
                    ((ListExpansionsRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Create tokenization dictionary.
        ///
        /// Upload a custom tokenization dictionary to use with the specified collection.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="tokenizationDictionary">An object that represents the tokenization dictionary to be uploaded.
        /// (optional)</param>
        /// <returns><see cref="TokenDictStatusResponse" />TokenDictStatusResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateTokenizationDictionary(SuccessCallback<TokenDictStatusResponse> successCallback, FailCallback failCallback, string environmentId, string collectionId, TokenDict tokenizationDictionary = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            CreateTokenizationDictionaryRequestObj req = new CreateTokenizationDictionaryRequestObj();
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
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnCreateTokenizationDictionaryResponse;

            fsData data = null;
            _serializer.TrySerialize(tokenizationDictionary, out data);
            string json = data.ToString().Replace('\"', '"');
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Send = Encoding.UTF8.GetBytes(json);

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/word_lists/tokenization_dictionary", environmentId, collectionId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class CreateTokenizationDictionaryRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<TokenDictStatusResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnCreateTokenizationDictionaryResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            TokenDictStatusResponse result = new TokenDictStatusResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((CreateTokenizationDictionaryRequestObj)req).CustomData;

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
                    Log.Error("Discovery.OnCreateTokenizationDictionaryResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((CreateTokenizationDictionaryRequestObj)req).SuccessCallback != null)
                    ((CreateTokenizationDictionaryRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((CreateTokenizationDictionaryRequestObj)req).FailCallback != null)
                    ((CreateTokenizationDictionaryRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Delete tokenization dictionary.
        ///
        /// Delete the tokenization dictionary from the collection.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <returns><see cref="" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteTokenizationDictionary(SuccessCallback<object> successCallback, FailCallback failCallback, string environmentId, string collectionId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            DeleteTokenizationDictionaryRequestObj req = new DeleteTokenizationDictionaryRequestObj();
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
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnDeleteTokenizationDictionaryResponse;
            req.Delete = true;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/word_lists/tokenization_dictionary", environmentId, collectionId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteTokenizationDictionaryRequestObj : RESTConnector.Request
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

        private void OnDeleteTokenizationDictionaryResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            object result = new object();
            Dictionary<string, object> customData = ((DeleteTokenizationDictionaryRequestObj)req).CustomData;
            
            if (resp.Success)
            {
                if (((DeleteTokenizationDictionaryRequestObj)req).SuccessCallback != null)
                    ((DeleteTokenizationDictionaryRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteTokenizationDictionaryRequestObj)req).FailCallback != null)
                    ((DeleteTokenizationDictionaryRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Get tokenization dictionary status.
        ///
        /// Returns the current status of the tokenization dictionary for the specified collection.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <returns><see cref="TokenDictStatusResponse" />TokenDictStatusResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetTokenizationDictionaryStatus(SuccessCallback<TokenDictStatusResponse> successCallback, FailCallback failCallback, string environmentId, string collectionId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetTokenizationDictionaryStatusRequestObj req = new GetTokenizationDictionaryStatusRequestObj();
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
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnGetTokenizationDictionaryStatusResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/word_lists/tokenization_dictionary", environmentId, collectionId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetTokenizationDictionaryStatusRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<TokenDictStatusResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetTokenizationDictionaryStatusResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            TokenDictStatusResponse result = new TokenDictStatusResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((GetTokenizationDictionaryStatusRequestObj)req).CustomData;

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
                    Log.Error("Discovery.OnGetTokenizationDictionaryStatusResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetTokenizationDictionaryStatusRequestObj)req).SuccessCallback != null)
                    ((GetTokenizationDictionaryStatusRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetTokenizationDictionaryStatusRequestObj)req).FailCallback != null)
                    ((GetTokenizationDictionaryStatusRequestObj)req).FailCallback(resp.Error, customData);
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

        #region Credentials
        #region List Credentials
        /// <summary>
        /// List all the source credentials that have been created for this service instance.
        /// Note: All credentials are sent over an encrypted connection and encrypted at rest.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool ListCredentials(SuccessCallback<CredentialsList> successCallback, FailCallback failCallback, string environmentId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentException("environmentId");

            ListCredentialsRequest req = new ListCredentialsRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.Headers["Content-Type"] = "application/json";
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnListCredentialsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(CredentialsEndpoint, environmentId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ListCredentialsRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<CredentialsList> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnListCredentialsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            CredentialsList result = new CredentialsList();
            fsData data = null;
            Dictionary<string, object> customData = ((ListCredentialsRequest)req).CustomData;
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
                    Log.Error("Discovery.OnListCredentialsResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((ListCredentialsRequest)req).SuccessCallback != null)
                    ((ListCredentialsRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ListCredentialsRequest)req).FailCallback != null)
                    ((ListCredentialsRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region CreateCredentials
        /// <summary>
        /// Create credentials.
        ///
        /// Creates a set of credentials to connect to a remote source. Created credentials are used in a configuration
        /// to associate a collection with the remote source.
        ///
        /// **Note:** All credentials are sent over an encrypted connection and encrypted at rest.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="credentialsParameter">An object that defines an individual set of source credentials.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        /// <returns><see cref="Credentials" />Credentials</returns>
        public bool CreateCredentials(SuccessCallback<SourceCredentials> successCallback, FailCallback failCallback, string environmentId, SourceCredentials credentialsParameter, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId");
            if (credentialsParameter == null)
                throw new ArgumentNullException("credentialsParameter");

            CreateCredentialsRequest req = new CreateCredentialsRequest();
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
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            fsData data = null;
            _serializer.TrySerialize(credentialsParameter, out data);
            string json = data.ToString().Replace('\"', '"');
            req.Send = Encoding.UTF8.GetBytes(json);

            req.OnResponse = OnCreateCredentialsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(CredentialsEndpoint, environmentId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class CreateCredentialsRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<SourceCredentials> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnCreateCredentialsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            SourceCredentials result = new SourceCredentials();
            fsData data = null;
            Dictionary<string, object> customData = ((CreateCredentialsRequest)req).CustomData;
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
                }
                catch (Exception e)
                {
                    Log.Error("Assistant.OnCreateCredentialsResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((CreateCredentialsRequest)req).SuccessCallback != null)
                    ((CreateCredentialsRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((CreateCredentialsRequest)req).FailCallback != null)
                    ((CreateCredentialsRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Delete Credentials
        /// <summary>
        /// Delete credentials.
        ///
        /// Deletes a set of stored credentials from your Discovery instance.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="credentialId">The unique identifier for a set of source credentials.</param>
        /// <param name="customData">Custom data object to pass data including custom request headers.</param>
        /// <returns><see cref="DeleteCredentials" />DeleteCredentials</returns>
        public bool DeleteCredentials(SuccessCallback<DeleteCredentials> successCallback, FailCallback failCallback, string environmentID, string credentialId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(credentialId))
                throw new ArgumentNullException("credentialId");

            DeleteCredentialsRequest req = new DeleteCredentialsRequest();
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
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnDeleteCredentialsResponse;
            req.Delete = true;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(CredentialEndpoint, environmentID, credentialId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteCredentialsRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<DeleteCredentials> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnDeleteCredentialsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DeleteCredentials result = new DeleteCredentials();
            fsData data = null;
            Dictionary<string, object> customData = ((DeleteCredentialsRequest)req).CustomData;
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
                    Log.Error("Discovery.OnDeleteCredentialsResponse()", "OnDeleteCredentialsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((DeleteCredentialsRequest)req).SuccessCallback != null)
                    ((DeleteCredentialsRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteCredentialsRequest)req).FailCallback != null)
                    ((DeleteCredentialsRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region GetCredential
        /// <summary>
        /// View Credentials.
        ///
        /// Returns details about the specified credentials.
        ///
        ///  **Note:** Secure credential information such as a password or SSH key is never returned and must be
        /// obtained from the source system.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="credentialId">The unique identifier for a set of source credentials.</param>
        /// <param name="customData">Custom data object to pass data including custom request headers.</param>
        /// <returns><see cref="Credentials" />Credentials</returns>
        public bool GetCredential(SuccessCallback<SourceCredentials> successCallback, FailCallback failCallback, string environmentID, string credentialId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(credentialId))
                throw new ArgumentNullException("credentialId");

            GetCredentialRequest req = new GetCredentialRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.Headers["Content-Type"] = "application/json";
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnGetCredentialResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(CredentialEndpoint, environmentID, credentialId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCredentialRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<SourceCredentials> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetCredentialResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            SourceCredentials result = new SourceCredentials();
            fsData data = null;
            Dictionary<string, object> customData = ((GetCredentialRequest)req).CustomData;
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
                    Log.Error("Discovery.OnGetCredentialResponse()", "OnGetCredentialResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetCredentialRequest)req).SuccessCallback != null)
                    ((GetCredentialRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetCredentialRequest)req).FailCallback != null)
                    ((GetCredentialRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion
        #endregion

        #region Events and feedback
        /// <summary>
        /// Create event.
        ///
        /// The **Events** API can be used to create log entries that are associated with specific queries. For example,
        /// you can record which documents in the results set were "clicked" by a user and when that click occured.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="queryEvent">An object that defines a query event to be added to the log.</param>
        /// <returns><see cref="CreateEventResponse" />CreateEventResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateEvent(SuccessCallback<CreateEventResponse> successCallback, FailCallback failCallback, CreateEventObject queryEvent, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            CreateEventRequestObj req = new CreateEventRequestObj();
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
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnCreateEventResponse;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            fsData data = null;
            _serializer.TrySerialize(queryEvent, out data);
            string json = data.ToString().Replace('\"', '"');
            req.Send = Encoding.UTF8.GetBytes(json);

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/events");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class CreateEventRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<CreateEventResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnCreateEventResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            CreateEventResponse result = new CreateEventResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((CreateEventRequestObj)req).CustomData;

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
                    Log.Error("Discovery.OnCreateEventResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((CreateEventRequestObj)req).SuccessCallback != null)
                    ((CreateEventRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((CreateEventRequestObj)req).FailCallback != null)
                    ((CreateEventRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Percentage of queries with an associated event.
        ///
        /// The percentage of queries using the **natural_language_query** parameter that have a corresponding "click"
        /// event over a specified time window.  This metric requires having integrated event tracking in your
        /// application using the **Events** API.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="startTime">Metric is computed from data recorded after this timestamp; must be in
        /// `YYYY-MM-DDThh:mm:ssZ` format. (optional)</param>
        /// <param name="endTime">Metric is computed from data recorded before this timestamp; must be in
        /// `YYYY-MM-DDThh:mm:ssZ` format. (optional)</param>
        /// <param name="resultType">The type of result to consider when calculating the metric. (optional)</param>
        /// <returns><see cref="MetricResponse" />MetricResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetMetricsEventRate(SuccessCallback<MetricResponse> successCallback, FailCallback failCallback, DateTime? startTime = null, DateTime? endTime = null, string resultType = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetMetricsEventRateRequestObj req = new GetMetricsEventRateRequestObj();
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
            req.Parameters["version"] = VersionDate;
            if (startTime != null)
                req.Parameters["start_time"] = startTime;
            if (endTime != null)
                req.Parameters["end_time"] = endTime;
            if (!string.IsNullOrEmpty(resultType))
                req.Parameters["result_type"] = resultType;
            req.OnResponse = OnGetMetricsEventRateResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/metrics/event_rate");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetMetricsEventRateRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<MetricResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetMetricsEventRateResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            MetricResponse result = new MetricResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((GetMetricsEventRateRequestObj)req).CustomData;

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
                    Log.Error("Discovery.OnGetMetricsEventRateResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetMetricsEventRateRequestObj)req).SuccessCallback != null)
                    ((GetMetricsEventRateRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetMetricsEventRateRequestObj)req).FailCallback != null)
                    ((GetMetricsEventRateRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Number of queries over time.
        ///
        /// Total number of queries using the **natural_language_query** parameter over a specific time window.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="startTime">Metric is computed from data recorded after this timestamp; must be in
        /// `YYYY-MM-DDThh:mm:ssZ` format. (optional)</param>
        /// <param name="endTime">Metric is computed from data recorded before this timestamp; must be in
        /// `YYYY-MM-DDThh:mm:ssZ` format. (optional)</param>
        /// <param name="resultType">The type of result to consider when calculating the metric. (optional)</param>
        /// <returns><see cref="MetricResponse" />MetricResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetMetricsQuery(SuccessCallback<MetricResponse> successCallback, FailCallback failCallback, DateTime? startTime = null, DateTime? endTime = null, string resultType = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetMetricsQueryRequestObj req = new GetMetricsQueryRequestObj();
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
            req.Parameters["version"] = VersionDate;
            if (startTime != null)
                req.Parameters["start_time"] = startTime;
            if (endTime != null)
                req.Parameters["end_time"] = endTime;
            if (!string.IsNullOrEmpty(resultType))
                req.Parameters["result_type"] = resultType;
            req.OnResponse = OnGetMetricsQueryResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/metrics/number_of_queries");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetMetricsQueryRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<MetricResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetMetricsQueryResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            MetricResponse result = new MetricResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((GetMetricsQueryRequestObj)req).CustomData;

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
                    Log.Error("Discovery.OnGetMetricsQueryResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetMetricsQueryRequestObj)req).SuccessCallback != null)
                    ((GetMetricsQueryRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetMetricsQueryRequestObj)req).FailCallback != null)
                    ((GetMetricsQueryRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Number of queries with an event over time.
        ///
        /// Total number of queries using the **natural_language_query** parameter that have a corresponding "click"
        /// event over a specified time window. This metric requires having integrated event tracking in your
        /// application using the **Events** API.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="startTime">Metric is computed from data recorded after this timestamp; must be in
        /// `YYYY-MM-DDThh:mm:ssZ` format. (optional)</param>
        /// <param name="endTime">Metric is computed from data recorded before this timestamp; must be in
        /// `YYYY-MM-DDThh:mm:ssZ` format. (optional)</param>
        /// <param name="resultType">The type of result to consider when calculating the metric. (optional)</param>
        /// <returns><see cref="MetricResponse" />MetricResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetMetricsQueryEvent(SuccessCallback<MetricResponse> successCallback, FailCallback failCallback, DateTime? startTime = null, DateTime? endTime = null, string resultType = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetMetricsQueryEventRequestObj req = new GetMetricsQueryEventRequestObj();
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
            req.Parameters["version"] = VersionDate;
            if (startTime != null)
                req.Parameters["start_time"] = startTime;
            if (endTime != null)
                req.Parameters["end_time"] = endTime;
            if (!string.IsNullOrEmpty(resultType))
                req.Parameters["result_type"] = resultType;
            req.OnResponse = OnGetMetricsQueryEventResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/metrics/number_of_queries_with_event");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetMetricsQueryEventRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<MetricResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetMetricsQueryEventResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            MetricResponse result = new MetricResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((GetMetricsQueryEventRequestObj)req).CustomData;

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
                    Log.Error("Discovery.OnGetMetricsQueryEventResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetMetricsQueryEventRequestObj)req).SuccessCallback != null)
                    ((GetMetricsQueryEventRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetMetricsQueryEventRequestObj)req).FailCallback != null)
                    ((GetMetricsQueryEventRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Number of queries with no search results over time.
        ///
        /// Total number of queries using the **natural_language_query** parameter that have no results returned over a
        /// specified time window.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="startTime">Metric is computed from data recorded after this timestamp; must be in
        /// `YYYY-MM-DDThh:mm:ssZ` format. (optional)</param>
        /// <param name="endTime">Metric is computed from data recorded before this timestamp; must be in
        /// `YYYY-MM-DDThh:mm:ssZ` format. (optional)</param>
        /// <param name="resultType">The type of result to consider when calculating the metric. (optional)</param>
        /// <returns><see cref="MetricResponse" />MetricResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetMetricsQueryNoResults(SuccessCallback<MetricResponse> successCallback, FailCallback failCallback, DateTime? startTime = null, DateTime? endTime = null, string resultType = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetMetricsQueryNoResultsRequestObj req = new GetMetricsQueryNoResultsRequestObj();
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
            req.Parameters["version"] = VersionDate;
            if (startTime != null)
                req.Parameters["start_time"] = startTime;
            if (endTime != null)
                req.Parameters["end_time"] = endTime;
            if (!string.IsNullOrEmpty(resultType))
                req.Parameters["result_type"] = resultType;
            req.OnResponse = OnGetMetricsQueryNoResultsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/metrics/number_of_queries_with_no_search_results");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetMetricsQueryNoResultsRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<MetricResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetMetricsQueryNoResultsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            MetricResponse result = new MetricResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((GetMetricsQueryNoResultsRequestObj)req).CustomData;

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
                    Log.Error("Discovery.OnGetMetricsQueryNoResultsResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetMetricsQueryNoResultsRequestObj)req).SuccessCallback != null)
                    ((GetMetricsQueryNoResultsRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetMetricsQueryNoResultsRequestObj)req).FailCallback != null)
                    ((GetMetricsQueryNoResultsRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Most frequent query tokens with an event.
        ///
        /// The most frequent query tokens parsed from the **natural_language_query** parameter and their corresponding
        /// "click" event rate within the recording period (queries and events are stored for 30 days). A query token is
        /// an individual word or unigram within the query string.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="count">Number of results to return. (optional, default to 10)</param>
        /// <returns><see cref="MetricTokenResponse" />MetricTokenResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetMetricsQueryTokenEvent(SuccessCallback<MetricTokenResponse> successCallback, FailCallback failCallback, long? count = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetMetricsQueryTokenEventRequestObj req = new GetMetricsQueryTokenEventRequestObj();
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
            req.Parameters["version"] = VersionDate;
            if (count != null)
                req.Parameters["count"] = count;
            req.OnResponse = OnGetMetricsQueryTokenEventResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/metrics/top_query_tokens_with_event_rate");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetMetricsQueryTokenEventRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<MetricTokenResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetMetricsQueryTokenEventResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            MetricTokenResponse result = new MetricTokenResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((GetMetricsQueryTokenEventRequestObj)req).CustomData;

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
                    Log.Error("Discovery.OnGetMetricsQueryTokenEventResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetMetricsQueryTokenEventRequestObj)req).SuccessCallback != null)
                    ((GetMetricsQueryTokenEventRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetMetricsQueryTokenEventRequestObj)req).FailCallback != null)
                    ((GetMetricsQueryTokenEventRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Search the query and event log.
        ///
        /// Searches the query and event log to find query sessions that match the specified criteria. Searching the
        /// **logs** endpoint uses the standard Discovery query syntax for the parameters that are supported.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="filter">A cacheable query that limits the documents returned to exclude any documents that
        /// don't mention the query content. Filter searches are better for metadata type searches and when you are
        /// trying to get a sense of concepts in the data set. (optional)</param>
        /// <param name="query">A query search returns all documents in your data set with full enrichments and full
        /// text, but with the most relevant documents listed first. Use a query search when you want to find the most
        /// relevant search results. You cannot use **natural_language_query** and **query** at the same time.
        /// (optional)</param>
        /// <param name="count">Number of results to return. (optional, default to 10)</param>
        /// <param name="offset">The number of query results to skip at the beginning. For example, if the total number
        /// of results that are returned is 10, and the offset is 8, it returns the last two results. (optional)</param>
        /// <param name="sort">A comma separated list of fields in the document to sort on. You can optionally specify a
        /// sort direction by prefixing the field with `-` for descending or `+` for ascending. Ascending is the default
        /// sort direction if no prefix is specified. (optional)</param>
        /// <returns><see cref="LogQueryResponse" />LogQueryResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool QueryLog(SuccessCallback<LogQueryResponse> successCallback, FailCallback failCallback, string filter = null, string query = null, long? count = null, long? offset = null, List<string> sort = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            QueryLogRequestObj req = new QueryLogRequestObj();
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
            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(filter))
                req.Parameters["filter"] = filter;
            if (!string.IsNullOrEmpty(query))
                req.Parameters["query"] = query;
            if (count != null)
                req.Parameters["count"] = count;
            if (offset != null)
                req.Parameters["offset"] = offset;
            if(sort != null)
                req.Parameters["sort"] = sort != null && sort.Count > 0 ? string.Join(",", sort.ToArray()) : null;
            req.OnResponse = OnQueryLogResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/logs");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class QueryLogRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<LogQueryResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnQueryLogResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            LogQueryResponse result = new LogQueryResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((QueryLogRequestObj)req).CustomData;

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
                    Log.Error("Discovery.OnQueryLogResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((QueryLogRequestObj)req).SuccessCallback != null)
                    ((QueryLogRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((QueryLogRequestObj)req).FailCallback != null)
                    ((QueryLogRequestObj)req).FailCallback(resp.Error, customData);
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
