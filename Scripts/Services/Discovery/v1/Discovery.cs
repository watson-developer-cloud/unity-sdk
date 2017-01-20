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
    public class Discovery : IWatsonService
    {
        #region Private Data
        private const string SERVICE_ID = "DiscoveryV1";
        private static fsSerializer sm_Serializer = new fsSerializer();

        private const string SERVICE_ENVIRONMENTS = "/v1/environments";
        private const string SERVICE_ENVIRONMENT = "/v1/environments/{0}";
        private const string SERVICE_ENVIRONMENT_PREVIEW = "/v1/environments/{0}/preview";
        private const string SERVICE_ENVIRONMENT_CONFIGURATIONS = "/v1/environments/{0}/configurations";
        private const string SERVICE_ENVIRONMENT_CONFIGURATION = "/v1/environments/{0}/configurations/{1}";
        private const string SERVICE_ENVIRONMENT_COLLECTIONS = "/v1/environments/{0}/collections";
        private const string SERVICE_ENVIRONMENT_COLLECTION = "/v1/environments/{0}/collections/{1}";
        private const string SERVICE_ENVIRONMENT_COLLECTION_FIELDS = "/v1/environments/{0}/collections/{1}/fields";
        private const string SERVICE_ENVIRONMENT_COLLECTION_DOCUMENTS = "/v1/environments/{0}/collections/{1}/documents";
        private const string SERVICE_ENVIRONMENT_COLLECTION_DOCUMENT = "/v1/environments/{0}/collections/{1}/documents/{2}";
        private const string SERVICE_ENVIRONMENT_COLLECTION_QUERY = "/v1/environments/{0}/collections/{1}/query";
        #endregion

        #region Public Types
        #endregion

        #region Environments
        #region GetEnvironments
        public delegate void OnGetEnvironments(GetEnvironmentsResponse resp, string customData);

        public bool GetEnvironments(OnGetEnvironments callback, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            GetEnvironmentsRequest req = new GetEnvironmentsRequest();
            req.Callback = callback;
            req.Data = customData;
            req.Parameters["version"] = DiscoveryVersion.Version;
            req.OnResponse = OnGetEnvironmentsResponse;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_ENVIRONMENTS);
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
            GetEnvironmentsResponse environmentsData = new GetEnvironmentsResponse();

            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);

                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = environmentsData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Discovery", "OnGetEnvironmentsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetEnvironmentsRequest)req).Callback != null)
                ((GetEnvironmentsRequest)req).Callback(resp.Success ? environmentsData : null, ((GetEnvironmentsRequest)req).Data);

        }
        #endregion

        #region Add Environment
        public delegate void OnAddEnvironment(Environment resp, string customData);

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
            req.Parameters["version"] = DiscoveryVersion.Version;
            req.OnResponse = OnAddEnvironmentResponse;

            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            string sendjson = Json.Serialize(addEnvironmentData);
            req.Send = Encoding.UTF8.GetBytes(sendjson);

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_ENVIRONMENTS);
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
            Environment environmentData = new Environment();

            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);

                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = environmentData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Discovery", "OnAddEnvironmentResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((AddEnvironmentRequest)req).Callback != null)
                ((AddEnvironmentRequest)req).Callback(resp.Success ? environmentData : null, ((AddEnvironmentRequest)req).Data);
        }
        #endregion

        #region GetEnvironment
        public delegate void OnGetEnvironment(Environment resp, string customData);

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
            req.Parameters["version"] = DiscoveryVersion.Version;
            req.OnResponse = OnGetEnvironmentResponse;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_ENVIRONMENT, environmentID));
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
            Environment environmentData = new Environment();

            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);

                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = environmentData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Discovery", "OnGetEnvironmentResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetEnvironmentRequest)req).Callback != null)
                ((GetEnvironmentRequest)req).Callback(resp.Success ? environmentData : null, ((GetEnvironmentRequest)req).Data);

        }
        #endregion

        #region Delete Environment
        public delegate void OnDeleteEnvironment(bool success, string customData);

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
            req.Parameters["version"] = DiscoveryVersion.Version;
            req.OnResponse = OnDeleteEnvironmentResponse;
            req.Delete = true;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_ENVIRONMENT, environmentID));
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
            if (((DeleteEnvironmentRequest)req).Callback != null)
                ((DeleteEnvironmentRequest)req).Callback(resp.Success, ((DeleteEnvironmentRequest)req).Data);
        }
        #endregion
        #endregion

        #region Configurations
        #region Get Configurations
        public delegate void OnGetConfigurations(GetConfigurationsResponse resp, string customData);

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
            req.Parameters["version"] = DiscoveryVersion.Version;
            if (!string.IsNullOrEmpty(name))
            {
                req.Name = name;
                req.Parameters["name"] = name;
            }
            req.OnResponse = OnGetConfigurationsResponse;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_ENVIRONMENT_CONFIGURATIONS, environmentID));
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
            GetConfigurationsResponse configurations = new GetConfigurationsResponse();

            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);

                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = configurations;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Discovery", "OnGetConfigurationsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetConfigurationsRequest)req).Callback != null)
                ((GetConfigurationsRequest)req).Callback(resp.Success ? configurations : null, ((GetConfigurationsRequest)req).Data);
        }
        #endregion

        #region Add Configuration
        public delegate void OnAddConfiguration(Configuration resp, string customData);

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
            req.Parameters["version"] = DiscoveryVersion.Version;
            req.OnResponse = OnAddConfigurationResponse;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Send = configurationJsonData;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_ENVIRONMENT_CONFIGURATIONS, environmentID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        public class AddConfigurationRequest : RESTConnector.Request
        {
            public OnAddConfiguration Callback { get; set; }
            public string EnvironmentID { get; set; }
            public byte[] ConfigurationJsonData { get; set; }
            public string Data { get; set; }
        }

        private void OnAddConfigurationResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Configuration configuration = new Configuration();

            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);

                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = configuration;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Discovery", "OnGetConfigurationResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((AddConfigurationRequest)req).Callback != null)
                ((AddConfigurationRequest)req).Callback(resp.Success ? configuration : null, ((AddConfigurationRequest)req).Data);
        }
        #endregion

        #region Get Configuration
        public delegate void OnGetConfiguration(Configuration resp, string customData);

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
            req.Parameters["version"] = DiscoveryVersion.Version;
            req.OnResponse = OnGetConfigurationResponse;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_ENVIRONMENT_CONFIGURATION, environmentID, configurationID));
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
            Configuration configuration = new Configuration();

            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);

                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = configuration;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Discovery", "OnGetConfigurationResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetConfigurationRequest)req).Callback != null)
                ((GetConfigurationRequest)req).Callback(resp.Success ? configuration : null, ((GetConfigurationRequest)req).Data);
        }
        #endregion

        #region Delete Configuration
        public delegate void OnDeleteConfiguration(bool success, string customData);

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
            req.Parameters["version"] = DiscoveryVersion.Version;
            req.OnResponse = OnDeleteConfigurationResponse;
            req.Delete = true;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_ENVIRONMENT_CONFIGURATION, environmentID, configurationID));
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
            if (((DeleteConfigurationRequest)req).Callback != null)
                ((DeleteConfigurationRequest)req).Callback(resp.Success, ((DeleteConfigurationRequest)req).Data);
        }
        #endregion
        #endregion

        #region Preview Configuration
        public delegate void OnPreviewConfiguration(TestDocument resp, string customData);

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
            req.Parameters["version"] = DiscoveryVersion.Version;
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

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_ENVIRONMENT_PREVIEW, environmentID));
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
            TestDocument testDocument = new TestDocument();

            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);

                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = testDocument;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Discovery", "OnPreviewConfigurationResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((PreviewConfigurationRequest)req).Callback != null)
                ((PreviewConfigurationRequest)req).Callback(resp.Success ? testDocument : null, ((PreviewConfigurationRequest)req).Data);

        }
        #endregion

        #region Collections
        #region Get Collections
        public delegate void OnGetCollections(GetCollectionsResponse resp, string customData);

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
            req.Parameters["version"] = DiscoveryVersion.Version;
            if (!string.IsNullOrEmpty(name))
            {
                req.Name = name;
                req.Parameters["name"] = name;
            }
            req.OnResponse = OnGetCollectionsResponse;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_ENVIRONMENT_COLLECTIONS, environmentID));
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
            GetCollectionsResponse collections = new GetCollectionsResponse();

            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);

                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = collections;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Discovery", "OnGetCollectionsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetCollectionsRequest)req).Callback != null)
                ((GetCollectionsRequest)req).Callback(resp.Success ? collections : null, ((GetCollectionsRequest)req).Data);
        }
        #endregion

        #region Add Collection
        public delegate void OnAddCollection(CollectionRef resp, string customData);

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
            req.Parameters["version"] = DiscoveryVersion.Version;
            req.OnResponse = OnAddCollectionResponse;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Send = collectionData;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_ENVIRONMENT_COLLECTIONS, environmentID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        public class AddCollectionRequest : RESTConnector.Request
        {
            public OnAddCollection Callback { get; set; }
            public string EnvironmentID { get; set; }
            public byte[] CollectionJsonData { get; set; }
            public string Data { get; set; }
        }

        private void OnAddCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            CollectionRef collection = new CollectionRef();

            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);

                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = collection;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Discovery", "OnGetConfigurationResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((AddCollectionRequest)req).Callback != null)
                ((AddCollectionRequest)req).Callback(resp.Success ? collection : null, ((AddCollectionRequest)req).Data);
        }
        #endregion

        #region Get Collection
        public delegate void OnGetCollection(Collection resp, string customData);

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
            req.Parameters["version"] = DiscoveryVersion.Version;
            req.OnResponse = OnGetCollectionResponse;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_ENVIRONMENT_COLLECTION, environmentID, collectionID));
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
            Collection collection = new Collection();

            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);

                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = collection;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Discovery", "OnGetCollectionResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetCollectionRequest)req).Callback != null)
                ((GetCollectionRequest)req).Callback(resp.Success ? collection : null, ((GetCollectionRequest)req).Data);
        }
        #endregion

        #region Delete Collection
        public delegate void OnDeleteCollection(bool success, string customData);

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
            req.Parameters["version"] = DiscoveryVersion.Version;
            req.OnResponse = OnDeleteCollectionResponse;
            req.Delete = true;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_ENVIRONMENT_COLLECTION, environmentID, collectionID));
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
            if (((DeleteCollectionRequest)req).Callback != null)
                ((DeleteCollectionRequest)req).Callback(resp.Success, ((DeleteCollectionRequest)req).Data);
        }
        #endregion

        #region Get Fields
        public delegate void OnGetFields(GetFieldsResponse resp, string customData);

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
            req.Parameters["version"] = DiscoveryVersion.Version;
            req.OnResponse = OnGetFieldsResponse;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_ENVIRONMENT_COLLECTION_FIELDS, environmentID, collectionID));
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
            GetFieldsResponse fields = new GetFieldsResponse();

            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);

                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = fields;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Discovery", "OnGetFieldsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetFieldsRequest)req).Callback != null)
                ((GetFieldsRequest)req).Callback(resp.Success ? fields : null, ((GetFieldsRequest)req).Data);
        }
        #endregion
        #endregion

        #region Documents
        #region Add Document
        public delegate void OnAddDocument(DocumentAccepted resp, string customData);

        public bool AddDocument(OnAddDocument callback, string environmentID, string contentFilePath, string configurationID = default(string), string configurationFilePath = default(string), string metadata = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");

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
                return AddDocumentUsingConfigID(callback, environmentID, contentData, contentMimeType, configurationID, metadata, customData);
            else if (!string.IsNullOrEmpty(configurationFilePath))
                return AddDocumentUsingConfigFile(callback, environmentID, contentData, contentMimeType, configurationFilePath, metadata, customData);
            else
                throw new WatsonException("A configurationID or configuration file path is required");
        }

        public bool AddDocumentUsingConfigID(OnAddDocument callback, string environmentID, byte[] contentData, string contentMimeType, string configurationID, string metadata = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");

            if (contentData == null)
                throw new ArgumentNullException("contentData");

            if (string.IsNullOrEmpty(contentMimeType))
                throw new ArgumentNullException("contentMimeType");

            if (string.IsNullOrEmpty(configurationID))
                throw new ArgumentNullException("configurationID");

            return AddDocument(callback, environmentID, contentData, contentMimeType, configurationID, null, metadata, customData);
        }

        public bool AddDocumentUsingConfigFile(OnAddDocument callback, string environmentID, byte[] contentData, string contentMimeType, string configurationFilePath, string metadata = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");

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

            return AddDocument(callback, environmentID, contentData, contentMimeType, null, configuration, metadata, customData);
        }

        public bool AddDocument(OnAddDocument callback, string environmentID, byte[] contentData, string contentMimeType, string configurationID = default(string), string configuration = default(string), string metadata = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (string.IsNullOrEmpty(environmentID))
                throw new ArgumentNullException("environmentID");

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
            req.ConfigurationID = configurationID;
            req.Configuration = configuration;
            req.ContentData = contentData;
            req.ContentMimeType = contentMimeType;
            req.Metadata = metadata;
            req.Data = customData;
            req.Parameters["version"] = DiscoveryVersion.Version;
            req.OnResponse = OnAddDocumentResponse;

            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["file"] = new RESTConnector.Form(contentData, "contentData", contentMimeType);

            if (!string.IsNullOrEmpty(metadata))
                req.Forms["metadata"] = new RESTConnector.Form(metadata);

            if (!string.IsNullOrEmpty(configurationID))
                req.Parameters["configuration_id"] = configurationID;

            if (!string.IsNullOrEmpty(configuration))
                req.Forms["configuration"] = new RESTConnector.Form(configuration);

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_ENVIRONMENT_COLLECTION_DOCUMENTS, environmentID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class AddDocumentRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public string EnvironmentID { get; set; }
            public string ConfigurationID { get; set; }
            public string Configuration { get; set; }
            public byte[] ContentData { get; set; }
            public string ContentMimeType { get; set; }
            public string Metadata { get; set; }
            public OnAddDocument Callback { get; set; }
        }

        private void OnAddDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DocumentAccepted doucmentAccepted = new DocumentAccepted();

            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);

                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = doucmentAccepted;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Discovery", "OnPreviewConfigurationResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((AddDocumentRequest)req).Callback != null)
                ((AddDocumentRequest)req).Callback(resp.Success ? doucmentAccepted : null, ((AddDocumentRequest)req).Data);

        }
        #endregion

        #region Delete Doucment
        #endregion

        #region Get Document
        #endregion

        #region Update Document
        #endregion
        #endregion

        #region Queries
        #endregion

        #region IWatsonService Interface
        public string GetServiceID()
        {
            return SERVICE_ID;
        }

        public void GetServiceStatus(ServiceStatus callback)
        {
            if (Config.Instance.FindCredentials(SERVICE_ID) != null)
                new CheckServiceStatus(this, callback);
            else
                callback(SERVICE_ID, false);
        }

        private class CheckServiceStatus
        {
            private Discovery m_Service = null;
            private ServiceStatus m_Callback = null;

            public CheckServiceStatus(Discovery service, ServiceStatus callback)
            {
                m_Service = service;
                m_Callback = callback;

                if (!m_Service.GetEnvironments(OnGetEnvironments, "CheckServiceStatus"))
                    m_Callback(SERVICE_ID, false);
            }

            private void OnGetEnvironments(GetEnvironmentsResponse environmentsData, string customData)
            {
                if (m_Callback != null)
                    m_Callback(SERVICE_ID, environmentsData != null);
            }
        }
        #endregion
    }
}
