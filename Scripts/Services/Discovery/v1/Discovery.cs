﻿/**
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
    /// <a href="http://www.ibm.com/watson/developercloud/discovery.html">Discovery Service</a>
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

        private const float DeleteTimeout = 100f;
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
        public Discovery(Credentials credentials)
        {
            if (credentials.HasCredentials() || credentials.HasAuthorizationToken())
            {
                Credentials = credentials;
            }
            else
            {
                throw new WatsonException("Please provide a username and password or authorization token to use the Discovery service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }
        #endregion

        #region Environments
        #region GetEnvironments
        /// <summary>
        /// The callback used by GetEnvironments().
        /// </summary>
        /// <param name="resp">The GetEnvironments response.</param>
        public delegate void OnGetEnvironments(RESTConnector.ParsedResponse<GetEnvironmentsResponse> resp);

        /// <summary>
        /// This class lists environments in a discovery instance. There are two environments returned: A read-only environment with the News
        /// collection (IBM Managed) and a user-created environment that the user can utilize to analyze and query their own data.
        /// </summary>
        /// <param name="callback">The OnGetEnvironments callback.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool GetEnvironments(OnGetEnvironments callback, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            GetEnvironmentsRequest req = new GetEnvironmentsRequest();
            req.Callback = callback;
            req.Data = customData;
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnGetEnvironmentsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, Environments);
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetEnvironmentsRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetEnvironments Callback { get; set; }
        }

        private void OnGetEnvironmentsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetEnvironmentsRequest)req).Data;

            RESTConnector.ParsedResponse<GetEnvironmentsResponse> parsedResp = new RESTConnector.ParsedResponse<GetEnvironmentsResponse>(resp, customData, _serializer);

            if (((GetEnvironmentsRequest)req).Callback != null)
                ((GetEnvironmentsRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Add Environment
        /// <summary>
        /// The callback used by AddEnvironment().
        /// </summary>
        /// <param name="resp">The Environment response.</param>
        public delegate void OnAddEnvironment(RESTConnector.ParsedResponse<Environment> resp);

        /// <summary>
        /// Creates a new environment. You can only create one environment per service instance.An attempt to create another environment 
        /// will result in an error. The size of the new environment can be controlled by specifying the size parameter.
        /// </summary>
        /// <param name="callback">The OnAddEnvironment callback.</param>
        /// <param name="name">The name of the environment to be created.</param>
        /// <param name="description">The description of the environment to be created.</param>
        /// <param name="size">The size of the environment to be created. See <a href="http://www.ibm.com/watson/developercloud/discovery.html#pricing-block">pricing.</a></param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool AddEnvironment(OnAddEnvironment callback, string name = default(string), string description = default(string), int size = 0, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            Dictionary<string, object> addEnvironmentData = new Dictionary<string, object>();
            addEnvironmentData["name"] = name;
            addEnvironmentData["description"] = description;
            addEnvironmentData["size"] = size;

            return AddEnvironment(callback, addEnvironmentData, customData);
        }

        /// <summary>
        /// Creates a new environment. You can only create one environment per service instance.An attempt to create another environment 
        /// will result in an error. The size of the new environment can be controlled by specifying the size parameter.
        /// </summary>
        /// <param name="callback">The OnAddEnvironment callback.</param>
        /// <param name="addEnvironmentData">The AddEnvironmentData.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool AddEnvironment(OnAddEnvironment callback, Dictionary<string, object> addEnvironmentData, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (addEnvironmentData == null)
                throw new ArgumentNullException("addEnvironmentData");

            AddEnvironmentRequest req = new AddEnvironmentRequest();
            req.Callback = callback;
            req.AddEnvironmentData = addEnvironmentData;
            req.Data = customData;
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
            public string Data { get; set; }
            public Dictionary<string, object> AddEnvironmentData { get; set; }
            public OnAddEnvironment Callback { get; set; }
        }

        private void OnAddEnvironmentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((AddEnvironmentRequest)req).Data;

            RESTConnector.ParsedResponse<Environment> parsedResp = new RESTConnector.ParsedResponse<Environment>(resp, customData, _serializer);

            if (((AddEnvironmentRequest)req).Callback != null)
                ((AddEnvironmentRequest)req).Callback(parsedResp);
        }
        #endregion

        #region GetEnvironment
        /// <summary>
        /// The callback used by GetEnvironment().
        /// </summary>
        /// <param name="resp">The Environment response.</param>
        public delegate void OnGetEnvironment(RESTConnector.ParsedResponse<Environment> resp);

        /// <summary>
        /// Returns specified environment data.
        /// </summary>
        /// <param name="callback">The OnGetEnvironment callback.</param>
        /// <param name="environmentID">The environment identifier requested.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool GetEnvironment(OnGetEnvironment callback, string environmentID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");

            GetEnvironmentRequest req = new GetEnvironmentRequest();
            req.Callback = callback;
            req.EnvironmentID = environmentID;
            req.Data = customData;
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnGetEnvironmentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Environment, environmentID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetEnvironmentRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public string EnvironmentID { get; set; }
            public OnGetEnvironment Callback { get; set; }
        }

        private void OnGetEnvironmentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetEnvironmentRequest)req).Data;

            RESTConnector.ParsedResponse<Environment> parsedResp = new RESTConnector.ParsedResponse<Environment>(resp, customData, _serializer);

            if (((GetEnvironmentRequest)req).Callback != null)
                ((GetEnvironmentRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Delete Environment
        /// <summary>
        /// The callback used by DeleteEnvironment().
        /// </summary>
        public delegate void OnDeleteEnvironment(RESTConnector.ParsedResponse<object> resp);

        /// <summary>
        /// Deletes the specified environment.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool DeleteEnvironment(OnDeleteEnvironment callback, string environmentID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");

            DeleteEnvironmentRequest req = new DeleteEnvironmentRequest();
            req.Callback = callback;
            req.EnvironmentID = environmentID;
            req.Data = customData;
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnDeleteEnvironmentResponse;
            req.Delete = true;
            req.Timeout = DeleteTimeout;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Environment, environmentID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteEnvironmentRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public string EnvironmentID { get; set; }
            public OnDeleteEnvironment Callback { get; set; }
        }

        private void OnDeleteEnvironmentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((DeleteEnvironmentRequest)req).Data;

            RESTConnector.ParsedResponse<object> parsedResp = new RESTConnector.ParsedResponse<object>(resp, customData, null);

            if (((DeleteEnvironmentRequest)req).Callback != null)
                ((DeleteEnvironmentRequest)req).Callback(parsedResp);
        }
        #endregion
        #endregion

        #region Configurations
        #region Get Configurations
        /// <summary>
        /// The callback used by GetConfigurations().
        /// </summary>
        /// <param name="resp">The GetConfigurationsResponse.</param>
        public delegate void OnGetConfigurations(RESTConnector.ParsedResponse<GetConfigurationsResponse> resp);

        /// <summary>
        /// Lists an environment's configurations.
        /// </summary>
        /// <param name="callback">The OnGetConfigurations callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="name">An optional configuration name to search.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool GetConfigurations(OnGetConfigurations callback, string environmentID, string name = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");

            GetConfigurationsRequest req = new GetConfigurationsRequest();
            req.Callback = callback;
            req.Data = customData;
            req.EnvironmentID = environmentID;
            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(name))
            {
                req.Name = name;
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
            public string Data { get; set; }
            public string EnvironmentID { get; set; }
            public string Name { get; set; }
            public OnGetConfigurations Callback { get; set; }
        }

        private void OnGetConfigurationsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetConfigurationsRequest)req).Data;

            RESTConnector.ParsedResponse<GetConfigurationsResponse> parsedResp = new RESTConnector.ParsedResponse<GetConfigurationsResponse>(resp, customData, _serializer);

            if (((GetConfigurationsRequest)req).Callback != null)
                ((GetConfigurationsRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Add Configuration
        /// <summary>
        /// The callback used by AddConfiguration().
        /// </summary>
        /// <param name="resp">The Configuration response.</param>
        public delegate void OnAddConfiguration(RESTConnector.ParsedResponse<Configuration> resp);

        /// <summary>
        /// Adds a configuration via external json file.
        /// </summary>
        /// <param name="callback">The OnAddConfiguration callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="configurationJsonPath">The path to the configuration json file.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool AddConfiguration(OnAddConfiguration callback, string environmentID, string configurationJsonPath, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(configurationJsonPath))
                throw new ArgumentNullException("configurationJsonPath");

            byte[] configJsonData;

            try
            {
                configJsonData = Encoding.UTF8.GetBytes(File.ReadAllText(configurationJsonPath));
            }
            catch (Exception e)
            {
                throw new WatsonException(string.Format("Failed to load configuration json: {0}", e.Message));
            }

            return AddConfiguration(callback, environmentID, configJsonData, customData);
        }

        /// <summary>
        /// Adds a configuration via json byte data.
        /// </summary>
        /// <param name="callback">The OnAddConfiguration callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="configurationJsonData">A byte array of configuration json data.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool AddConfiguration(OnAddConfiguration callback, string environmentID, byte[] configurationJsonData, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (configurationJsonData == null)
                throw new ArgumentNullException("configurationJsonData");

            AddConfigurationRequest req = new AddConfigurationRequest();
            req.Callback = callback;
            req.Data = customData;
            req.EnvironmentID = environmentID;
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
            public OnAddConfiguration Callback { get; set; }
            public string EnvironmentID { get; set; }
            public byte[] ConfigurationJsonData { get; set; }
            public string Data { get; set; }
        }

        private void OnAddConfigurationResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((AddConfigurationRequest)req).Data;

            RESTConnector.ParsedResponse<Configuration> parsedResp = new RESTConnector.ParsedResponse<Configuration>(resp, customData, _serializer);

            if (((AddConfigurationRequest)req).Callback != null)
                ((AddConfigurationRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Get Configuration
        /// <summary>
        /// The callback uesd by GetConfiguration().
        /// </summary>
        /// <param name="resp">The Configuration response.</param>
        public delegate void OnGetConfiguration(RESTConnector.ParsedResponse<Configuration> resp);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback">The OnGetConfiguration callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="configurationID">The configuration identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool GetConfiguration(OnGetConfiguration callback, string environmentID, string configurationID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(configurationID))
                throw new ArgumentNullException("configurationID");

            GetConfigurationRequest req = new GetConfigurationRequest();
            req.Callback = callback;
            req.Data = customData;
            req.EnvironmentID = environmentID;
            req.ConfigurationID = configurationID;
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnGetConfigurationResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Configuration, environmentID, configurationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetConfigurationRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public string EnvironmentID { get; set; }
            public string ConfigurationID { get; set; }
            public OnGetConfiguration Callback { get; set; }
        }

        private void OnGetConfigurationResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetConfigurationRequest)req).Data;

            RESTConnector.ParsedResponse<Configuration> parsedResp = new RESTConnector.ParsedResponse<Configuration>(resp, customData, _serializer);

            if (((GetConfigurationRequest)req).Callback != null)
                ((GetConfigurationRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Delete Configuration
        /// <summary>
        /// The callback used by DeleteConfiguration().
        /// </summary>
        public delegate void OnDeleteConfiguration(RESTConnector.ParsedResponse<object> resp);

        /// <summary>
        /// Deletes an environments specified configuration.
        /// </summary>
        /// <param name="callback">The OnDeleteConfiguration callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="configurationID">The configuration identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool DeleteConfiguration(OnDeleteConfiguration callback, string environmentID, string configurationID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");

            if (string.IsNullOrEmpty(configurationID))
                throw new ArgumentNullException("configurationID");

            DeleteConfigurationRequest req = new DeleteConfigurationRequest();
            req.Callback = callback;
            req.EnvironmentID = environmentID;
            req.ConfigurationID = configurationID;
            req.Data = customData;
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnDeleteConfigurationResponse;
            req.Delete = true;
            req.Timeout = DeleteTimeout;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Configuration, environmentID, configurationID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteConfigurationRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public string EnvironmentID { get; set; }
            public string ConfigurationID { get; set; }
            public OnDeleteConfiguration Callback { get; set; }
        }

        private void OnDeleteConfigurationResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((DeleteConfigurationRequest)req).Data;

            RESTConnector.ParsedResponse<object> parsedResp = new RESTConnector.ParsedResponse<object>(resp, customData, null);

            if (((DeleteConfigurationRequest)req).Callback != null)
                ((DeleteConfigurationRequest)req).Callback(parsedResp);
        }
        #endregion
        #endregion

        #region Preview Configuration
        /// <summary>
        /// The callback used by PreviewConfiguration().
        /// </summary>
        /// <param name="resp">The response.</param>
        public delegate void OnPreviewConfiguration(RESTConnector.ParsedResponse<TestDocument> resp);

        /// <summary>
        /// Runs a sample document through the default or your configuration and returns diagnostic information designed to 
        /// help you understand how the document was processed. The document is not added to the index.
        /// </summary>
        /// <param name="callback">The OnPreviewConfiguration callback.</param>
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
        public bool PreviewConfiguration(OnPreviewConfiguration callback, string environmentID, string configurationID, string configurationFilePath, string contentFilePath, string metadata = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

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

            return PreviewConfiguration(callback, environmentID, configurationID, configurationFilePath, contentData, contentMimeType, customData);
        }

        /// <summary>
        /// Runs a sample document through the default or your configuration and returns diagnostic information designed to 
        /// help you understand how the document was processed. The document is not added to the index.
        /// </summary>
        /// <param name="callback">The OnPreviewConfiguration callback.</param>
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
        public bool PreviewConfiguration(OnPreviewConfiguration callback, string environmentID, string configurationID, string configurationFilePath, byte[] contentData, string contentMimeType, string metadata = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");

            if (string.IsNullOrEmpty(configurationID) && string.IsNullOrEmpty(configurationFilePath))
                throw new ArgumentNullException("configurationID or configurationFilePath");

            if (!string.IsNullOrEmpty(configurationID) && !string.IsNullOrEmpty(configurationFilePath))
                throw new WatsonException("Use either a configurationID OR designate a test configuration file path - not both");

            if (contentData == null)
                throw new ArgumentNullException("contentData");

            PreviewConfigurationRequest req = new PreviewConfigurationRequest();
            req.Callback = callback;
            req.EnvironmentID = environmentID;
            req.ConfigurationID = configurationID;
            req.ConfigurationFilePath = configurationFilePath;
            req.ContentData = contentData;
            req.Metadata = metadata;
            req.Data = customData;
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
            public string Data { get; set; }
            public string EnvironmentID { get; set; }
            public string ConfigurationID { get; set; }
            public string ConfigurationFilePath { get; set; }
            public byte[] ContentData { get; set; }
            public string Metadata { get; set; }
            public OnPreviewConfiguration Callback { get; set; }
        }

        private void OnPreviewConfigurationResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((PreviewConfigurationRequest)req).Data;

            RESTConnector.ParsedResponse<TestDocument> parsedResp = new RESTConnector.ParsedResponse<TestDocument>(resp, customData, _serializer);

            if (((PreviewConfigurationRequest)req).Callback != null)
                ((PreviewConfigurationRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Collections
        #region Get Collections
        /// <summary>
        /// The callback used by GetCollections().
        /// </summary>
        /// <param name="resp">The GetCollectionsResponse.</param>
        public delegate void OnGetCollections(RESTConnector.ParsedResponse<GetCollectionsResponse> resp);

        /// <summary>
        /// Lists a specified environment's collections.
        /// </summary>
        /// <param name="callback">The OnGetCollections callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="name">Find collections with the given name.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool GetCollections(OnGetCollections callback, string environmentID, string name = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");

            GetCollectionsRequest req = new GetCollectionsRequest();
            req.Callback = callback;
            req.Data = customData;
            req.EnvironmentID = environmentID;
            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(name))
            {
                req.Name = name;
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
            public string Data { get; set; }
            public string EnvironmentID { get; set; }
            public string Name { get; set; }
            public OnGetCollections Callback { get; set; }
        }

        private void OnGetCollectionsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetCollectionsRequest)req).Data;

            RESTConnector.ParsedResponse<GetCollectionsResponse> parsedResp = new RESTConnector.ParsedResponse<GetCollectionsResponse>(resp, customData, _serializer);

            if (((GetCollectionsRequest)req).Callback != null)
                ((GetCollectionsRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Add Collection
        /// <summary>
        /// The callback used by OnAddCollection
        /// </summary>
        /// <param name="resp">The CollectionRef response.</param>
        public delegate void OnAddCollection(RESTConnector.ParsedResponse<CollectionRef> resp);

        /// <summary>
        /// Adds a collection to a specified environment.
        /// </summary>
        /// <param name="callback">The OnAddCollection callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="name">The name of the collection to be created.</param>
        /// <param name="description">The description of the collection to be created.</param>
        /// <param name="configurationID">The configuration identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool AddCollection(OnAddCollection callback, string environmentID, string name, string description = default(string), string configurationID = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["name"] = name;
            parameters["description"] = description;
            parameters["configuration_id"] = configurationID;

            return AddCollection(callback, environmentID, Encoding.UTF8.GetBytes(Json.Serialize(parameters)), customData);
        }

        /// <summary>
        /// Adds a collection to a specified environment.
        /// </summary>
        /// <param name="callback">The OnAddCollection callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionData">A byte array of json collection data.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool AddCollection(OnAddCollection callback, string environmentID, byte[] collectionData, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (collectionData == null)
                throw new ArgumentNullException("collectionData");

            AddCollectionRequest req = new AddCollectionRequest();
            req.Callback = callback;
            req.Data = customData;
            req.EnvironmentID = environmentID;
            req.CollectionJsonData = collectionData;
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
            public OnAddCollection Callback { get; set; }
            public string EnvironmentID { get; set; }
            public byte[] CollectionJsonData { get; set; }
            public string Data { get; set; }
        }

        private void OnAddCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((AddCollectionRequest)req).Data;

            RESTConnector.ParsedResponse<CollectionRef> parsedResp = new RESTConnector.ParsedResponse<CollectionRef>(resp, customData, _serializer);

            if (((AddCollectionRequest)req).Callback != null)
                ((AddCollectionRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Get Collection
        /// <summary>
        /// The callback used by GetCollection().
        /// </summary>
        /// <param name="resp">The Collection response.</param>
        public delegate void OnGetCollection(RESTConnector.ParsedResponse<Collection> resp);

        /// <summary>
        /// Lists a specified collecton's details.
        /// </summary>
        /// <param name="callback">The OnGetCollection callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool GetCollection(OnGetCollection callback, string environmentID, string collectionID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");

            GetCollectionRequest req = new GetCollectionRequest();
            req.Callback = callback;
            req.Data = customData;
            req.EnvironmentID = environmentID;
            req.CollectionID = collectionID;
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnGetCollectionResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Collection, environmentID, collectionID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCollectionRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public string EnvironmentID { get; set; }
            public string CollectionID { get; set; }
            public OnGetCollection Callback { get; set; }
        }

        private void OnGetCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetCollectionRequest)req).Data;

            RESTConnector.ParsedResponse<Collection> parsedResp = new RESTConnector.ParsedResponse<Collection>(resp, customData, _serializer);

            if (((GetCollectionRequest)req).Callback != null)
                ((GetCollectionRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Delete Collection
        /// <summary>
        /// The callback used by DeleteCollection().
        /// </summary>
        public delegate void OnDeleteCollection(RESTConnector.ParsedResponse<object> resp);

        /// <summary>
        /// Deletes a specified collection.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool DeleteCollection(OnDeleteCollection callback, string environmentID, string collectionID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");

            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");

            DeleteCollectionRequest req = new DeleteCollectionRequest();
            req.Callback = callback;
            req.EnvironmentID = environmentID;
            req.CollectionID = collectionID;
            req.Data = customData;
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnDeleteCollectionResponse;
            req.Delete = true;
            req.Timeout = DeleteTimeout;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Collection, environmentID, collectionID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteCollectionRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public string EnvironmentID { get; set; }
            public string CollectionID { get; set; }
            public OnDeleteCollection Callback { get; set; }
        }

        private void OnDeleteCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((DeleteCollectionRequest)req).Data;

            RESTConnector.ParsedResponse<object> parsedResp = new RESTConnector.ParsedResponse<object>(resp, customData, null);

            if (((DeleteCollectionRequest)req).Callback != null)
                ((DeleteCollectionRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Get Fields
        /// <summary>
        /// The callback used by GetFields().
        /// </summary>
        /// <param name="resp">The GetFieldsResponse.</param>
        public delegate void OnGetFields(RESTConnector.ParsedResponse<GetFieldsResponse> resp);

        /// <summary>
        /// Gets a list of the the unique fields (and their types) stored in the index.
        /// </summary>
        /// <param name="callback">The OnGetFields callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool GetFields(OnGetFields callback, string environmentID, string collectionID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");

            GetFieldsRequest req = new GetFieldsRequest();
            req.Callback = callback;
            req.Data = customData;
            req.EnvironmentID = environmentID;
            req.CollectionID = collectionID;
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnGetFieldsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Fields, environmentID, collectionID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetFieldsRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public string EnvironmentID { get; set; }
            public string CollectionID { get; set; }
            public string Name { get; set; }
            public OnGetFields Callback { get; set; }
        }

        private void OnGetFieldsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetFieldsRequest)req).Data;

            RESTConnector.ParsedResponse<GetFieldsResponse> parsedResp = new RESTConnector.ParsedResponse<GetFieldsResponse>(resp, customData, _serializer);

            if (((GetFieldsRequest)req).Callback != null)
                ((GetFieldsRequest)req).Callback(parsedResp);
        }
        #endregion
        #endregion

        #region Documents
        #region Add Document
        /// <summary>
        /// The callbackused by AddDocument().
        /// </summary>
        /// <param name="resp">The DocumentAccepted response.</param>
        public delegate void OnAddDocument(RESTConnector.ParsedResponse<DocumentAccepted> resp);

        /// <summary>
        /// Add a document to a collection with optional metadata and optional configuration. The configuration to use to process 
        /// the document can be provided using the configuration_id argument. Returns immediately after the system has accepted the 
        /// document for processing. The user must provide document content, metadata, or both. If the request is missing both document 
        /// content and metadata, then it will be rejected.
        /// </summary>
        /// <param name="callback">The callback.</param>
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
        public bool AddDocument(OnAddDocument callback, string environmentID, string collectionID, string contentFilePath, string configurationID = default(string), string configurationFilePath = default(string), string metadata = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

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
                return AddDocumentUsingConfigID(callback, environmentID, collectionID, contentData, contentMimeType, configurationID, metadata, customData);
            else if (!string.IsNullOrEmpty(configurationFilePath))
                return AddDocumentUsingConfigFile(callback, environmentID, collectionID, contentData, contentMimeType, configurationFilePath, metadata, customData);
            else
                throw new WatsonException("A configurationID or configuration file path is required");
        }

        /// <summary>
        /// Add a document to a collection with optional metadata and optional configuration. The configuration to use to process 
        /// the document can be provided using the configuration_id argument. Returns immediately after the system has accepted the 
        /// document for processing. The user must provide document content, metadata, or both. If the request is missing both document 
        /// content and metadata, then it will be rejected.
        /// </summary>
        /// <param name="callback">The OnAddDocument callback.</param>
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
        public bool AddDocumentUsingConfigID(OnAddDocument callback, string environmentID, string collectionID, byte[] contentData, string contentMimeType, string configurationID, string metadata = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

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

            return AddDocument(callback, environmentID, collectionID, contentData, contentMimeType, configurationID, null, metadata, customData);
        }

        /// <summary>
        /// Add a document to a collection with optional metadata and optional configuration. The configuration to use to process 
        /// the document can be provided using the configuration_id argument. Returns immediately after the system has accepted the 
        /// document for processing. The user must provide document content, metadata, or both. If the request is missing both document 
        /// content and metadata, then it will be rejected.
        /// </summary>
        /// <param name="callback">The OnAddDocument callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="contentData">A byte array of content to be ingested.</param>
        /// <param name="contentMimeType">The mimeType of the content data to be ingested./param>
        /// <param name="configurationFilePath">The file path to the configuration to use to process the document.</param>
        /// <param name="metadata">If you're using the Data Crawler to upload your documents, you can test a document against the type 
        /// of metadata that the Data Crawler might send. The maximum supported metadata file size is 1 MB. Metadata parts larger than 
        /// 1 MB are rejected. Example: { "Creator": "Johnny Appleseed", "Subject": "Apples" }</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool AddDocumentUsingConfigFile(OnAddDocument callback, string environmentID, string collectionID, byte[] contentData, string contentMimeType, string configurationFilePath, string metadata = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

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

            return AddDocument(callback, environmentID, collectionID, contentData, contentMimeType, null, configuration, metadata, customData);
        }

        /// <summary>
        /// Add a document to a collection with optional metadata and optional configuration. The configuration to use to process 
        /// the document can be provided using the configuration_id argument. Returns immediately after the system has accepted the 
        /// document for processing. The user must provide document content, metadata, or both. If the request is missing both document 
        /// content and metadata, then it will be rejected.
        /// </summary>
        /// <param name="callback">The OnAddDocument callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="contentData">A byte array of content to be ingested.</param>
        /// <param name="contentMimeType">The mimeType of the content data to be ingested./param>
        /// <param name="configurationID">The configuration identifier. If this is specified, do not specify a configuration.</param>
        /// <param name="configuration">A json string of the configuration to test. If this is specified, do not specify a configurationID.</param>
        /// <param name="metadata">If you're using the Data Crawler to upload your documents, you can test a document against the type 
        /// of metadata that the Data Crawler might send. The maximum supported metadata file size is 1 MB. Metadata parts larger than 
        /// 1 MB are rejected. Example: { "Creator": "Johnny Appleseed", "Subject": "Apples" }</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool AddDocument(OnAddDocument callback, string environmentID, string collectionID, byte[] contentData, string contentMimeType, string configurationID = default(string), string configuration = default(string), string metadata = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

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
            req.Callback = callback;
            req.EnvironmentID = environmentID;
            req.CollectionID = collectionID;
            req.ConfigurationID = configurationID;
            req.Configuration = configuration;
            req.ContentData = contentData;
            req.ContentMimeType = contentMimeType;
            req.Metadata = metadata;
            req.Data = customData;
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
            public string Data { get; set; }
            public string EnvironmentID { get; set; }
            public string CollectionID { get; set; }
            public string ConfigurationID { get; set; }
            public string Configuration { get; set; }
            public byte[] ContentData { get; set; }
            public string ContentMimeType { get; set; }
            public string Metadata { get; set; }
            public OnAddDocument Callback { get; set; }
        }

        private void OnAddDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((AddDocumentRequest)req).Data;

            RESTConnector.ParsedResponse<DocumentAccepted> parsedResp = new RESTConnector.ParsedResponse<DocumentAccepted>(resp, customData, _serializer);

            if (((AddDocumentRequest)req).Callback != null)
                ((AddDocumentRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Delete Doucment
        /// <summary>
        /// The callback used by DeleteDocument().
        /// </summary>
        public delegate void OnDeleteDocument(RESTConnector.ParsedResponse<object> resp);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback">The OnDeleteDocument callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="documentID">The document identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool DeleteDocument(OnDeleteDocument callback, string environmentID, string collectionID, string documentID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");

            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");

            if (string.IsNullOrEmpty(documentID))
                throw new ArgumentNullException("documentID");

            DeleteDocumentRequest req = new DeleteDocumentRequest();
            req.Callback = callback;
            req.EnvironmentID = environmentID;
            req.CollectionID = collectionID;
            req.DocumentID = documentID;
            req.Data = customData;
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnDeleteDocumentResponse;
            req.Delete = true;
            req.Timeout = DeleteTimeout;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Document, environmentID, collectionID, documentID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteDocumentRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public string EnvironmentID { get; set; }
            public string CollectionID { get; set; }
            public string DocumentID { get; set; }
            public OnDeleteDocument Callback { get; set; }
        }

        private void OnDeleteDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((DeleteDocumentRequest)req).Data;

            RESTConnector.ParsedResponse<object> parsedResp = new RESTConnector.ParsedResponse<object>(resp, customData, null);

            if (((DeleteDocumentRequest)req).Callback != null)
                ((DeleteDocumentRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Get Document
        /// <summary>
        /// The callback used by GetDocument().
        /// </summary>
        /// <param name="resp">The DocumentStatus response.</param>
        public delegate void OnGetDocument(RESTConnector.ParsedResponse<DocumentStatus> resp);

        /// <summary>
        /// Lists a specified document's details.
        /// </summary>
        /// <param name="callback">The OnGetDocument callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="documentID">The document identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool GetDocument(OnGetDocument callback, string environmentID, string collectionID, string documentID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (string.IsNullOrEmpty(documentID))
                throw new ArgumentNullException("documentID");

            GetDocumentRequest req = new GetDocumentRequest();
            req.Callback = callback;
            req.Data = customData;
            req.EnvironmentID = environmentID;
            req.CollectionID = collectionID;
            req.DocumentID = documentID;
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnGetDocumentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(Document, environmentID, collectionID, documentID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetDocumentRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public string EnvironmentID { get; set; }
            public string CollectionID { get; set; }
            public string DocumentID { get; set; }
            public OnGetDocument Callback { get; set; }
        }

        private void OnGetDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetDocumentRequest)req).Data;

            RESTConnector.ParsedResponse<DocumentStatus> parsedResp = new RESTConnector.ParsedResponse<DocumentStatus>(resp, customData, _serializer);

            if (((GetDocumentRequest)req).Callback != null)
                ((GetDocumentRequest)req).Callback(parsedResp);
        }
        #endregion

        #region Update Document
        /// <summary>
        /// The callback used by UpdateDocument().
        /// </summary>
        /// <param name="resp">The DocumentAccepted response.</param>
        public delegate void OnUpdateDocument(RESTConnector.ParsedResponse<DocumentAccepted> resp);

        /// <summary>
        /// Updates a specified document.
        /// </summary>
        /// <param name="callback">The callback.</param>
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
        public bool UpdateDocument(OnUpdateDocument callback, string environmentID, string collectionID, string documentID, string contentFilePath, string configurationID = default(string), string configurationFilePath = default(string), string metadata = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

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
                return UpdateDocumentUsingConfigID(callback, environmentID, collectionID, documentID, contentData, contentMimeType, configurationID, metadata, customData);
            else if (!string.IsNullOrEmpty(configurationFilePath))
                return UpdateDocumentUsingConfigFile(callback, environmentID, collectionID, documentID, contentData, contentMimeType, configurationFilePath, metadata, customData);
            else
                throw new WatsonException("A configurationID or configuration file path is required");
        }

        /// <summary>
        /// Updates a specified document using ConfigID.
        /// </summary>
        /// <param name="callback">The OnUpdateDocument callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="documentID">The document identifier.</param>
        /// <param name="contentData">A byte array of content to be ingested.</param>
        /// <param name="contentMimeType">The mimeType of the content data to be ingested./param>
        /// <param name="configurationID">The identifier of the configuration to use to process the document.</param>
        /// <param name="metadata">If you're using the Data Crawler to upload your documents, you can test a document against the type 
        /// of metadata that the Data Crawler might send. The maximum supported metadata file size is 1 MB. Metadata parts larger than 
        /// 1 MB are rejected. Example: { "Creator": "Johnny Appleseed", "Subject": "Apples" }</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool UpdateDocumentUsingConfigID(OnUpdateDocument callback, string environmentID, string collectionID, string documentID, byte[] contentData, string contentMimeType, string configurationID, string metadata = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

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

            return UpdateDocument(callback, environmentID, collectionID, documentID, contentData, contentMimeType, configurationID, null, metadata, customData);
        }

        /// <summary>
        /// Updates a specified document using a configuration file path.
        /// </summary>
        /// <param name="callback">The OnUpdateDocument callback.</param>
        /// <param name="environmentID">The environment identifier.</param>
        /// <param name="collectionID">The collection identifier.</param>
        /// <param name="documentID">The document identifier.</param>
        /// <param name="contentData">A byte array of content to be ingested.</param>
        /// <param name="contentMimeType">The mimeType of the content data to be ingested./param>
        /// <param name="configurationFilePath">The file path to the configuration to use to process</param>
        /// <param name="metadata">If you're using the Data Crawler to upload your documents, you can test a document against the type 
        /// of metadata that the Data Crawler might send. The maximum supported metadata file size is 1 MB. Metadata parts larger than 
        /// 1 MB are rejected. Example: { "Creator": "Johnny Appleseed", "Subject": "Apples" }</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool UpdateDocumentUsingConfigFile(OnUpdateDocument callback, string environmentID, string collectionID, string documentID, byte[] contentData, string contentMimeType, string configurationFilePath, string metadata = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

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

            return UpdateDocument(callback, environmentID, collectionID, documentID, contentData, contentMimeType, null, configuration, metadata, customData);
        }

        /// <summary>
        /// Updates a specified document.
        /// </summary>
        /// <param name="callback">The OnUpdateDocument callback.</param>
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
        public bool UpdateDocument(OnUpdateDocument callback, string environmentID, string collectionID, string documentID, byte[] contentData, string contentMimeType, string configurationID = default(string), string configuration = default(string), string metadata = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

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
            req.Callback = callback;
            req.EnvironmentID = environmentID;
            req.CollectionID = collectionID;
            req.DocumentID = documentID;
            req.ConfigurationID = configurationID;
            req.Configuration = configuration;
            req.ContentData = contentData;
            req.ContentMimeType = contentMimeType;
            req.Metadata = metadata;
            req.Data = customData;
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
            public string Data { get; set; }
            public string EnvironmentID { get; set; }
            public string CollectionID { get; set; }
            public string DocumentID { get; set; }
            public string ConfigurationID { get; set; }
            public string Configuration { get; set; }
            public byte[] ContentData { get; set; }
            public string ContentMimeType { get; set; }
            public string Metadata { get; set; }
            public OnUpdateDocument Callback { get; set; }
        }

        private void OnUpdateDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((UpdateDocumentRequest)req).Data;

            RESTConnector.ParsedResponse<DocumentAccepted> parsedResp = new RESTConnector.ParsedResponse<DocumentAccepted>(resp, customData, _serializer);

            if (((UpdateDocumentRequest)req).Callback != null)
                ((UpdateDocumentRequest)req).Callback(parsedResp);
        }
        #endregion
        #endregion

        #region Queries
        /// <summary>
        /// The callback used by Query().
        /// </summary>
        /// <param name="resp">The QueryResponse.</param>
        public delegate void OnQuery(RESTConnector.ParsedResponse<QueryResponse> resp);

        /// <summary>
        /// Query the discovery instance.
        /// </summary>
        /// <param name="callback">The OnQuery callback.</param>
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
        public bool Query(OnQuery callback, string environmentID, string collectionID, string filter, string query, string aggregation, int count, string _return, int offset, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (string.IsNullOrEmpty(query))
                throw new ArgumentNullException("query");

            QueryRequest req = new QueryRequest();
            req.Callback = callback;
            req.Data = customData;
            req.EnvironmentID = environmentID;
            req.CollectionID = collectionID;
            req.Filter = filter;
            req.Query = query;
            req.Aggregation = aggregation;
            req.Count = count;
            req.Return = _return;
            req.Offset = offset;

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
            public string Data { get; set; }
            public string EnvironmentID { get; set; }
            public string CollectionID { get; set; }
            public string Filter { get; set; }
            public string Query { get; set; }
            public string Aggregation { get; set; }
            public int Count { get; set; }
            public string Return { get; set; }
            public int Offset { get; set; }
            public OnQuery Callback { get; set; }
        }

        private void OnQueryResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((QueryRequest)req).Data;

            RESTConnector.ParsedResponse<QueryResponse> parsedResp = new RESTConnector.ParsedResponse<QueryResponse>(resp, customData, _serializer);

            if (((QueryRequest)req).Callback != null)
                ((QueryRequest)req).Callback(parsedResp);
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
