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
using IBM.Watson.Discovery.V1.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Networking;

namespace IBM.Watson.Discovery.V1
{
    public partial class DiscoveryService : BaseService
    {
        private const string serviceId = "discovery";
        private const string defaultUrl = "https://gateway.watsonplatform.net/discovery/api";

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
        private string versionDate;
        /// <summary>
        /// Gets and sets the versionDate of the service.
        /// </summary>
        public string VersionDate
        {
            get { return versionDate; }
            set { versionDate = value; }
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
        /// DiscoveryService constructor.
        /// </summary>
        /// <param name="versionDate">The service version date in `yyyy-mm-dd` format.</param>
        public DiscoveryService(string versionDate) : base(versionDate, serviceId)
        {
            VersionDate = versionDate;
        }

        /// <summary>
        /// DiscoveryService constructor.
        /// </summary>
        /// <param name="versionDate">The service version date in `yyyy-mm-dd` format.</param>
        /// <param name="credentials">The service credentials.</param>
        public DiscoveryService(string versionDate, Credentials credentials) : base(versionDate, credentials, serviceId)
        {
            if (string.IsNullOrEmpty(versionDate))
            {
                throw new ArgumentNullException("A versionDate (format `yyyy-mm-dd`) is required to create an instance of DiscoveryService");
            }
            else
            {
                VersionDate = versionDate;
            }

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
                throw new IBMException("Please provide a username and password or authorization token to use the Discovery service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }

        /// <summary>
        /// Create an environment.
        ///
        /// Creates a new environment for private data. An environment must be created before collections can be
        /// created.
        ///
        /// **Note**: You can create only one environment for private data per service instance. An attempt to create
        /// another environment results in an error.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="name">Name that identifies the environment.</param>
        /// <param name="description">Description of the environment. (optional, default to )</param>
        /// <param name="size">Size of the environment. In the Lite plan the default and only accepted value is `LT`, in
        /// all other plans the default is `S`. (optional)</param>
        /// <returns><see cref="ModelEnvironment" />ModelEnvironment</returns>
        public bool CreateEnvironment(Callback<ModelEnvironment> callback, string name, string description = null, string size = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateEnvironment`");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("`name` is required for `CreateEnvironment`");

            RequestObject<ModelEnvironment> req = new RequestObject<ModelEnvironment>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "CreateEnvironment"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(name))
                bodyObject["name"] = name;
            if (!string.IsNullOrEmpty(description))
                bodyObject["description"] = description;
            if (!string.IsNullOrEmpty(size))
                bodyObject["size"] = size;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateEnvironmentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/environments");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateEnvironmentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ModelEnvironment> response = new DetailedResponse<ModelEnvironment>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ModelEnvironment>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateEnvironmentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ModelEnvironment>)req).Callback != null)
                ((RequestObject<ModelEnvironment>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete environment.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <returns><see cref="DeleteEnvironmentResponse" />DeleteEnvironmentResponse</returns>
        public bool DeleteEnvironment(Callback<DeleteEnvironmentResponse> callback, string environmentId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteEnvironment`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `DeleteEnvironment`");

            RequestObject<DeleteEnvironmentResponse> req = new RequestObject<DeleteEnvironmentResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "DeleteEnvironment"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteEnvironmentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}", environmentId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteEnvironmentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DeleteEnvironmentResponse> response = new DetailedResponse<DeleteEnvironmentResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DeleteEnvironmentResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnDeleteEnvironmentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DeleteEnvironmentResponse>)req).Callback != null)
                ((RequestObject<DeleteEnvironmentResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get environment info.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <returns><see cref="ModelEnvironment" />ModelEnvironment</returns>
        public bool GetEnvironment(Callback<ModelEnvironment> callback, string environmentId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetEnvironment`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `GetEnvironment`");

            RequestObject<ModelEnvironment> req = new RequestObject<ModelEnvironment>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "GetEnvironment"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetEnvironmentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}", environmentId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetEnvironmentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ModelEnvironment> response = new DetailedResponse<ModelEnvironment>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ModelEnvironment>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetEnvironmentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ModelEnvironment>)req).Callback != null)
                ((RequestObject<ModelEnvironment>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List environments.
        ///
        /// List existing environments for the service instance.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="name">Show only the environment with the given name. (optional)</param>
        /// <returns><see cref="ListEnvironmentsResponse" />ListEnvironmentsResponse</returns>
        public bool ListEnvironments(Callback<ListEnvironmentsResponse> callback, string name = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListEnvironments`");

            RequestObject<ListEnvironmentsResponse> req = new RequestObject<ListEnvironmentsResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "ListEnvironments"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(name))
            {
                req.Parameters["name"] = name;
            }

            req.OnResponse = OnListEnvironmentsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/environments");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListEnvironmentsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ListEnvironmentsResponse> response = new DetailedResponse<ListEnvironmentsResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ListEnvironmentsResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListEnvironmentsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ListEnvironmentsResponse>)req).Callback != null)
                ((RequestObject<ListEnvironmentsResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List fields across collections.
        ///
        /// Gets a list of the unique fields (and their types) stored in the indexes of the specified collections.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionIds">A comma-separated list of collection IDs to be queried against.</param>
        /// <returns><see cref="ListCollectionFieldsResponse" />ListCollectionFieldsResponse</returns>
        public bool ListFields(Callback<ListCollectionFieldsResponse> callback, string environmentId, List<string> collectionIds)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListFields`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `ListFields`");
            if (collectionIds == null)
                throw new ArgumentNullException("`collectionIds` is required for `ListFields`");

            RequestObject<ListCollectionFieldsResponse> req = new RequestObject<ListCollectionFieldsResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "ListFields"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (collectionIds != null && collectionIds.Count > 0)
            {
                req.Parameters["collection_ids"] = string.Join(",", collectionIds.ToArray());
            }

            req.OnResponse = OnListFieldsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/fields", environmentId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListFieldsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ListCollectionFieldsResponse> response = new DetailedResponse<ListCollectionFieldsResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ListCollectionFieldsResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListFieldsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ListCollectionFieldsResponse>)req).Callback != null)
                ((RequestObject<ListCollectionFieldsResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update an environment.
        ///
        /// Updates an environment. The environment's **name** and  **description** parameters can be changed. You must
        /// specify a **name** for the environment.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="name">Name that identifies the environment. (optional, default to )</param>
        /// <param name="description">Description of the environment. (optional, default to )</param>
        /// <param name="size">Size that the environment should be increased to. Environment size cannot be modified
        /// when using a Lite plan. Environment size can only increased and not decreased. (optional)</param>
        /// <returns><see cref="ModelEnvironment" />ModelEnvironment</returns>
        public bool UpdateEnvironment(Callback<ModelEnvironment> callback, string environmentId, string name = null, string description = null, string size = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateEnvironment`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `UpdateEnvironment`");

            RequestObject<ModelEnvironment> req = new RequestObject<ModelEnvironment>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "UpdateEnvironment"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(name))
                bodyObject["name"] = name;
            if (!string.IsNullOrEmpty(description))
                bodyObject["description"] = description;
            if (!string.IsNullOrEmpty(size))
                bodyObject["size"] = size;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateEnvironmentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}", environmentId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnUpdateEnvironmentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ModelEnvironment> response = new DetailedResponse<ModelEnvironment>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ModelEnvironment>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnUpdateEnvironmentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ModelEnvironment>)req).Callback != null)
                ((RequestObject<ModelEnvironment>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Add configuration.
        ///
        /// Creates a new configuration.
        ///
        /// If the input configuration contains the **configuration_id**, **created**, or **updated** properties, then
        /// they are ignored and overridden by the system, and an error is not returned so that the overridden fields do
        /// not need to be removed when copying a configuration.
        ///
        /// The configuration can contain unrecognized JSON fields. Any such fields are ignored and do not generate an
        /// error. This makes it easier to use newer configuration files with older versions of the API and the service.
        /// It also makes it possible for the tooling to add additional metadata and information to the configuration.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="name">The name of the configuration.</param>
        /// <param name="description">The description of the configuration, if available. (optional)</param>
        /// <param name="conversions">Document conversion settings. (optional)</param>
        /// <param name="enrichments">An array of document enrichment settings for the configuration. (optional)</param>
        /// <param name="normalizations">Defines operations that can be used to transform the final output JSON into a
        /// normalized form. Operations are executed in the order that they appear in the array. (optional)</param>
        /// <param name="source">Object containing source parameters for the configuration. (optional)</param>
        /// <returns><see cref="Configuration" />Configuration</returns>
        public bool CreateConfiguration(Callback<Configuration> callback, string environmentId, string name, string description = null, Conversions conversions = null, List<Enrichment> enrichments = null, List<NormalizationOperation> normalizations = null, Source source = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateConfiguration`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `CreateConfiguration`");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("`name` is required for `CreateConfiguration`");

            RequestObject<Configuration> req = new RequestObject<Configuration>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "CreateConfiguration"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(name))
                bodyObject["name"] = name;
            if (!string.IsNullOrEmpty(description))
                bodyObject["description"] = description;
            if (conversions != null)
                bodyObject["conversions"] = JToken.FromObject(conversions);
            if (enrichments != null && enrichments.Count > 0)
                bodyObject["enrichments"] = JToken.FromObject(enrichments);
            if (normalizations != null && normalizations.Count > 0)
                bodyObject["normalizations"] = JToken.FromObject(normalizations);
            if (source != null)
                bodyObject["source"] = JToken.FromObject(source);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateConfigurationResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/configurations", environmentId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateConfigurationResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Configuration> response = new DetailedResponse<Configuration>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Configuration>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateConfigurationResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Configuration>)req).Callback != null)
                ((RequestObject<Configuration>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete a configuration.
        ///
        /// The deletion is performed unconditionally. A configuration deletion request succeeds even if the
        /// configuration is referenced by a collection or document ingestion. However, documents that have already been
        /// submitted for processing continue to use the deleted configuration. Documents are always processed with a
        /// snapshot of the configuration as it existed at the time the document was submitted.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="configurationId">The ID of the configuration.</param>
        /// <returns><see cref="DeleteConfigurationResponse" />DeleteConfigurationResponse</returns>
        public bool DeleteConfiguration(Callback<DeleteConfigurationResponse> callback, string environmentId, string configurationId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteConfiguration`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `DeleteConfiguration`");
            if (string.IsNullOrEmpty(configurationId))
                throw new ArgumentNullException("`configurationId` is required for `DeleteConfiguration`");

            RequestObject<DeleteConfigurationResponse> req = new RequestObject<DeleteConfigurationResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "DeleteConfiguration"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteConfigurationResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/configurations/{1}", environmentId, configurationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteConfigurationResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DeleteConfigurationResponse> response = new DetailedResponse<DeleteConfigurationResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DeleteConfigurationResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnDeleteConfigurationResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DeleteConfigurationResponse>)req).Callback != null)
                ((RequestObject<DeleteConfigurationResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get configuration details.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="configurationId">The ID of the configuration.</param>
        /// <returns><see cref="Configuration" />Configuration</returns>
        public bool GetConfiguration(Callback<Configuration> callback, string environmentId, string configurationId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetConfiguration`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `GetConfiguration`");
            if (string.IsNullOrEmpty(configurationId))
                throw new ArgumentNullException("`configurationId` is required for `GetConfiguration`");

            RequestObject<Configuration> req = new RequestObject<Configuration>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "GetConfiguration"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetConfigurationResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/configurations/{1}", environmentId, configurationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetConfigurationResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Configuration> response = new DetailedResponse<Configuration>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Configuration>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetConfigurationResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Configuration>)req).Callback != null)
                ((RequestObject<Configuration>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List configurations.
        ///
        /// Lists existing configurations for the service instance.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="name">Find configurations with the given name. (optional)</param>
        /// <returns><see cref="ListConfigurationsResponse" />ListConfigurationsResponse</returns>
        public bool ListConfigurations(Callback<ListConfigurationsResponse> callback, string environmentId, string name = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListConfigurations`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `ListConfigurations`");

            RequestObject<ListConfigurationsResponse> req = new RequestObject<ListConfigurationsResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "ListConfigurations"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(name))
            {
                req.Parameters["name"] = name;
            }

            req.OnResponse = OnListConfigurationsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/configurations", environmentId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListConfigurationsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ListConfigurationsResponse> response = new DetailedResponse<ListConfigurationsResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ListConfigurationsResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListConfigurationsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ListConfigurationsResponse>)req).Callback != null)
                ((RequestObject<ListConfigurationsResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update a configuration.
        ///
        /// Replaces an existing configuration.
        ///   * Completely replaces the original configuration.
        ///   * The **configuration_id**, **updated**, and **created** fields are accepted in the request, but they are
        /// ignored, and an error is not generated. It is also acceptable for users to submit an updated configuration
        /// with none of the three properties.
        ///   * Documents are processed with a snapshot of the configuration as it was at the time the document was
        /// submitted to be ingested. This means that already submitted documents will not see any updates made to the
        /// configuration.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="configurationId">The ID of the configuration.</param>
        /// <param name="name">The name of the configuration.</param>
        /// <param name="description">The description of the configuration, if available. (optional)</param>
        /// <param name="conversions">Document conversion settings. (optional)</param>
        /// <param name="enrichments">An array of document enrichment settings for the configuration. (optional)</param>
        /// <param name="normalizations">Defines operations that can be used to transform the final output JSON into a
        /// normalized form. Operations are executed in the order that they appear in the array. (optional)</param>
        /// <param name="source">Object containing source parameters for the configuration. (optional)</param>
        /// <returns><see cref="Configuration" />Configuration</returns>
        public bool UpdateConfiguration(Callback<Configuration> callback, string environmentId, string configurationId, string name, string description = null, Conversions conversions = null, List<Enrichment> enrichments = null, List<NormalizationOperation> normalizations = null, Source source = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateConfiguration`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `UpdateConfiguration`");
            if (string.IsNullOrEmpty(configurationId))
                throw new ArgumentNullException("`configurationId` is required for `UpdateConfiguration`");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("`name` is required for `UpdateConfiguration`");

            RequestObject<Configuration> req = new RequestObject<Configuration>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "UpdateConfiguration"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(name))
                bodyObject["name"] = name;
            if (!string.IsNullOrEmpty(description))
                bodyObject["description"] = description;
            if (conversions != null)
                bodyObject["conversions"] = JToken.FromObject(conversions);
            if (enrichments != null && enrichments.Count > 0)
                bodyObject["enrichments"] = JToken.FromObject(enrichments);
            if (normalizations != null && normalizations.Count > 0)
                bodyObject["normalizations"] = JToken.FromObject(normalizations);
            if (source != null)
                bodyObject["source"] = JToken.FromObject(source);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateConfigurationResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/configurations/{1}", environmentId, configurationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnUpdateConfigurationResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Configuration> response = new DetailedResponse<Configuration>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Configuration>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnUpdateConfigurationResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Configuration>)req).Callback != null)
                ((RequestObject<Configuration>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Test configuration.
        ///
        /// Runs a sample document through the default or your configuration and returns diagnostic information designed
        /// to help you understand how the document was processed. The document is not added to the index.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="configuration">The configuration to use to process the document. If this part is provided, then
        /// the provided configuration is used to process the document. If the **configuration_id** is also provided
        /// (both are present at the same time), then request is rejected. The maximum supported configuration size is 1
        /// MB. Configuration parts larger than 1 MB are rejected.
        /// See the `GET /configurations/{configuration_id}` operation for an example configuration. (optional)</param>
        /// <param name="file">The content of the document to ingest. The maximum supported file size when adding a file
        /// to a collection is 50 megabytes, the maximum supported file size when testing a confiruration is 1 megabyte.
        /// Files larger than the supported size are rejected. (optional)</param>
        /// <param name="filename">The filename for file. (optional)</param>
        /// <param name="fileContentType">The content type of file. (optional)</param>
        /// <param name="metadata">If you're using the Data Crawler to upload your documents, you can test a document
        /// against the type of metadata that the Data Crawler might send. The maximum supported metadata file size is 1
        /// MB. Metadata parts larger than 1 MB are rejected.
        /// Example:  ``` {
        ///   "Creator": "Johnny Appleseed",
        ///   "Subject": "Apples"
        /// } ```. (optional)</param>
        /// <param name="step">Specify to only run the input document through the given step instead of running the
        /// input document through the entire ingestion workflow. Valid values are `convert`, `enrich`, and `normalize`.
        /// (optional)</param>
        /// <param name="configurationId">The ID of the configuration to use to process the document. If the
        /// **configuration** form part is also provided (both are present at the same time), then the request will be
        /// rejected. (optional)</param>
        /// <returns><see cref="TestDocument" />TestDocument</returns>
        public bool TestConfigurationInEnvironment(Callback<TestDocument> callback, string environmentId, string configuration = null, System.IO.MemoryStream file = null, string filename = null, string fileContentType = null, string metadata = null, string step = null, string configurationId = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `TestConfigurationInEnvironment`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `TestConfigurationInEnvironment`");

            RequestObject<TestDocument> req = new RequestObject<TestDocument>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "TestConfigurationInEnvironment"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (!string.IsNullOrEmpty(configuration))
            {
                req.Forms["configuration"] = new RESTConnector.Form(configuration);
            }
            if (file != null)
            {
                req.Forms["file"] = new RESTConnector.Form(file, filename, fileContentType);
            }
            if (!string.IsNullOrEmpty(metadata))
            {
                req.Forms["metadata"] = new RESTConnector.Form(metadata);
            }
            if (!string.IsNullOrEmpty(step))
            {
                req.Parameters["step"] = step;
            }
            if (!string.IsNullOrEmpty(configurationId))
            {
                req.Parameters["configuration_id"] = configurationId;
            }

            req.OnResponse = OnTestConfigurationInEnvironmentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/preview", environmentId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnTestConfigurationInEnvironmentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TestDocument> response = new DetailedResponse<TestDocument>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TestDocument>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnTestConfigurationInEnvironmentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TestDocument>)req).Callback != null)
                ((RequestObject<TestDocument>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create a collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="name">The name of the collection to be created.</param>
        /// <param name="description">A description of the collection. (optional, default to )</param>
        /// <param name="configurationId">The ID of the configuration in which the collection is to be created.
        /// (optional, default to )</param>
        /// <param name="language">The language of the documents stored in the collection, in the form of an ISO 639-1
        /// language code. (optional, default to en)</param>
        /// <returns><see cref="Collection" />Collection</returns>
        public bool CreateCollection(Callback<Collection> callback, string environmentId, string name, string description = null, string configurationId = null, string language = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateCollection`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `CreateCollection`");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("`name` is required for `CreateCollection`");

            RequestObject<Collection> req = new RequestObject<Collection>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "CreateCollection"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(name))
                bodyObject["name"] = name;
            if (!string.IsNullOrEmpty(description))
                bodyObject["description"] = description;
            if (!string.IsNullOrEmpty(configurationId))
                bodyObject["configuration_id"] = configurationId;
            if (!string.IsNullOrEmpty(language))
                bodyObject["language"] = language;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateCollectionResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections", environmentId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Collection> response = new DetailedResponse<Collection>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Collection>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateCollectionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Collection>)req).Callback != null)
                ((RequestObject<Collection>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete a collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <returns><see cref="DeleteCollectionResponse" />DeleteCollectionResponse</returns>
        public bool DeleteCollection(Callback<DeleteCollectionResponse> callback, string environmentId, string collectionId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteCollection`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `DeleteCollection`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `DeleteCollection`");

            RequestObject<DeleteCollectionResponse> req = new RequestObject<DeleteCollectionResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "DeleteCollection"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteCollectionResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DeleteCollectionResponse> response = new DetailedResponse<DeleteCollectionResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DeleteCollectionResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnDeleteCollectionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DeleteCollectionResponse>)req).Callback != null)
                ((RequestObject<DeleteCollectionResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get collection details.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <returns><see cref="Collection" />Collection</returns>
        public bool GetCollection(Callback<Collection> callback, string environmentId, string collectionId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetCollection`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `GetCollection`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `GetCollection`");

            RequestObject<Collection> req = new RequestObject<Collection>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "GetCollection"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetCollectionResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Collection> response = new DetailedResponse<Collection>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Collection>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetCollectionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Collection>)req).Callback != null)
                ((RequestObject<Collection>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List collection fields.
        ///
        /// Gets a list of the unique fields (and their types) stored in the index.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <returns><see cref="ListCollectionFieldsResponse" />ListCollectionFieldsResponse</returns>
        public bool ListCollectionFields(Callback<ListCollectionFieldsResponse> callback, string environmentId, string collectionId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListCollectionFields`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `ListCollectionFields`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `ListCollectionFields`");

            RequestObject<ListCollectionFieldsResponse> req = new RequestObject<ListCollectionFieldsResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "ListCollectionFields"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListCollectionFieldsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/fields", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListCollectionFieldsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ListCollectionFieldsResponse> response = new DetailedResponse<ListCollectionFieldsResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ListCollectionFieldsResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListCollectionFieldsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ListCollectionFieldsResponse>)req).Callback != null)
                ((RequestObject<ListCollectionFieldsResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List collections.
        ///
        /// Lists existing collections for the service instance.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="name">Find collections with the given name. (optional)</param>
        /// <returns><see cref="ListCollectionsResponse" />ListCollectionsResponse</returns>
        public bool ListCollections(Callback<ListCollectionsResponse> callback, string environmentId, string name = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListCollections`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `ListCollections`");

            RequestObject<ListCollectionsResponse> req = new RequestObject<ListCollectionsResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "ListCollections"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(name))
            {
                req.Parameters["name"] = name;
            }

            req.OnResponse = OnListCollectionsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections", environmentId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListCollectionsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ListCollectionsResponse> response = new DetailedResponse<ListCollectionsResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ListCollectionsResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListCollectionsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ListCollectionsResponse>)req).Callback != null)
                ((RequestObject<ListCollectionsResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update a collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="name">The name of the collection.</param>
        /// <param name="description">A description of the collection. (optional, default to )</param>
        /// <param name="configurationId">The ID of the configuration in which the collection is to be updated.
        /// (optional, default to )</param>
        /// <returns><see cref="Collection" />Collection</returns>
        public bool UpdateCollection(Callback<Collection> callback, string environmentId, string collectionId, string name, string description = null, string configurationId = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateCollection`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `UpdateCollection`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `UpdateCollection`");

            RequestObject<Collection> req = new RequestObject<Collection>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "UpdateCollection"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(name))
                bodyObject["name"] = name;
            if (!string.IsNullOrEmpty(description))
                bodyObject["description"] = description;
            if (!string.IsNullOrEmpty(configurationId))
                bodyObject["configuration_id"] = configurationId;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateCollectionResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnUpdateCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Collection> response = new DetailedResponse<Collection>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Collection>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnUpdateCollectionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Collection>)req).Callback != null)
                ((RequestObject<Collection>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create or update expansion list.
        ///
        /// Create or replace the Expansion list for this collection. The maximum number of expanded terms per
        /// collection is `500`.
        /// The current expansion list is replaced with the uploaded content.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="expansions">An array of query expansion definitions.
        ///
        ///  Each object in the **expansions** array represents a term or set of terms that will be expanded into other
        /// terms. Each expansion object can be configured as bidirectional or unidirectional. Bidirectional means that
        /// all terms are expanded to all other terms in the object. Unidirectional means that a set list of terms can
        /// be expanded into a second list of terms.
        ///
        ///  To create a bi-directional expansion specify an **expanded_terms** array. When found in a query, all items
        /// in the **expanded_terms** array are then expanded to the other items in the same array.
        ///
        ///  To create a uni-directional expansion, specify both an array of **input_terms** and an array of
        /// **expanded_terms**. When items in the **input_terms** array are present in a query, they are expanded using
        /// the items listed in the **expanded_terms** array.</param>
        /// <returns><see cref="Expansions" />Expansions</returns>
        public bool CreateExpansions(Callback<Expansions> callback, string environmentId, string collectionId, List<Expansion> expansions)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateExpansions`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `CreateExpansions`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `CreateExpansions`");
            if (expansions == null)
                throw new ArgumentNullException("`expansions` is required for `CreateExpansions`");

            RequestObject<Expansions> req = new RequestObject<Expansions>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "CreateExpansions"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (expansions != null && expansions.Count > 0)
                bodyObject["expansions"] = JToken.FromObject(expansions);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateExpansionsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/expansions", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateExpansionsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Expansions> response = new DetailedResponse<Expansions>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Expansions>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateExpansionsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Expansions>)req).Callback != null)
                ((RequestObject<Expansions>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create stopword list.
        ///
        /// Upload a custom stopword list to use with the specified collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="stopwordFile">The content of the stopword list to ingest.</param>
        /// <param name="stopwordFilename">The filename for stopwordFile.</param>
        /// <returns><see cref="TokenDictStatusResponse" />TokenDictStatusResponse</returns>
        public bool CreateStopwordList(Callback<TokenDictStatusResponse> callback, string environmentId, string collectionId, System.IO.MemoryStream stopwordFile, string stopwordFilename)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateStopwordList`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `CreateStopwordList`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `CreateStopwordList`");
            if (stopwordFile == null)
                throw new ArgumentNullException("`stopwordFile` is required for `CreateStopwordList`");
            if (string.IsNullOrEmpty(stopwordFilename))
                throw new ArgumentNullException("`stopwordFilename` is required for `CreateStopwordList`");

            RequestObject<TokenDictStatusResponse> req = new RequestObject<TokenDictStatusResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "CreateStopwordList"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (stopwordFile != null)
            {
                req.Forms["stopword_file"] = new RESTConnector.Form(stopwordFile, stopwordFilename, "application/octet-stream");
            }

            req.OnResponse = OnCreateStopwordListResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/word_lists/stopwords", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateStopwordListResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TokenDictStatusResponse> response = new DetailedResponse<TokenDictStatusResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TokenDictStatusResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateStopwordListResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TokenDictStatusResponse>)req).Callback != null)
                ((RequestObject<TokenDictStatusResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create tokenization dictionary.
        ///
        /// Upload a custom tokenization dictionary to use with the specified collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="tokenizationRules">An array of tokenization rules. Each rule contains, the original `text`
        /// string, component `tokens`, any alternate character set `readings`, and which `part_of_speech` the text is
        /// from. (optional)</param>
        /// <returns><see cref="TokenDictStatusResponse" />TokenDictStatusResponse</returns>
        public bool CreateTokenizationDictionary(Callback<TokenDictStatusResponse> callback, string environmentId, string collectionId, List<TokenDictRule> tokenizationRules = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateTokenizationDictionary`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `CreateTokenizationDictionary`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `CreateTokenizationDictionary`");

            RequestObject<TokenDictStatusResponse> req = new RequestObject<TokenDictStatusResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "CreateTokenizationDictionary"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (tokenizationRules != null && tokenizationRules.Count > 0)
                bodyObject["tokenization_rules"] = JToken.FromObject(tokenizationRules);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateTokenizationDictionaryResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/word_lists/tokenization_dictionary", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateTokenizationDictionaryResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TokenDictStatusResponse> response = new DetailedResponse<TokenDictStatusResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TokenDictStatusResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateTokenizationDictionaryResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TokenDictStatusResponse>)req).Callback != null)
                ((RequestObject<TokenDictStatusResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete the expansion list.
        ///
        /// Remove the expansion information for this collection. The expansion list must be deleted to disable query
        /// expansion for a collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteExpansions(Callback<object> callback, string environmentId, string collectionId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteExpansions`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `DeleteExpansions`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `DeleteExpansions`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "DeleteExpansions"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteExpansionsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/expansions", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteExpansionsResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("DiscoveryService.OnDeleteExpansionsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete a custom stopword list.
        ///
        /// Delete a custom stopword list from the collection. After a custom stopword list is deleted, the default list
        /// is used for the collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteStopwordList(Callback<object> callback, string environmentId, string collectionId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteStopwordList`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `DeleteStopwordList`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `DeleteStopwordList`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "DeleteStopwordList"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteStopwordListResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/word_lists/stopwords", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteStopwordListResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("DiscoveryService.OnDeleteStopwordListResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete tokenization dictionary.
        ///
        /// Delete the tokenization dictionary from the collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteTokenizationDictionary(Callback<object> callback, string environmentId, string collectionId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteTokenizationDictionary`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `DeleteTokenizationDictionary`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `DeleteTokenizationDictionary`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "DeleteTokenizationDictionary"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteTokenizationDictionaryResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/word_lists/tokenization_dictionary", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteTokenizationDictionaryResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("DiscoveryService.OnDeleteTokenizationDictionaryResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get stopword list status.
        ///
        /// Returns the current status of the stopword list for the specified collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <returns><see cref="TokenDictStatusResponse" />TokenDictStatusResponse</returns>
        public bool GetStopwordListStatus(Callback<TokenDictStatusResponse> callback, string environmentId, string collectionId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetStopwordListStatus`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `GetStopwordListStatus`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `GetStopwordListStatus`");

            RequestObject<TokenDictStatusResponse> req = new RequestObject<TokenDictStatusResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "GetStopwordListStatus"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetStopwordListStatusResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/word_lists/stopwords", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetStopwordListStatusResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TokenDictStatusResponse> response = new DetailedResponse<TokenDictStatusResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TokenDictStatusResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetStopwordListStatusResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TokenDictStatusResponse>)req).Callback != null)
                ((RequestObject<TokenDictStatusResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get tokenization dictionary status.
        ///
        /// Returns the current status of the tokenization dictionary for the specified collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <returns><see cref="TokenDictStatusResponse" />TokenDictStatusResponse</returns>
        public bool GetTokenizationDictionaryStatus(Callback<TokenDictStatusResponse> callback, string environmentId, string collectionId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetTokenizationDictionaryStatus`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `GetTokenizationDictionaryStatus`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `GetTokenizationDictionaryStatus`");

            RequestObject<TokenDictStatusResponse> req = new RequestObject<TokenDictStatusResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "GetTokenizationDictionaryStatus"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetTokenizationDictionaryStatusResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/word_lists/tokenization_dictionary", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetTokenizationDictionaryStatusResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TokenDictStatusResponse> response = new DetailedResponse<TokenDictStatusResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TokenDictStatusResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetTokenizationDictionaryStatusResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TokenDictStatusResponse>)req).Callback != null)
                ((RequestObject<TokenDictStatusResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get the expansion list.
        ///
        /// Returns the current expansion list for the specified collection. If an expansion list is not specified, an
        /// object with empty expansion arrays is returned.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <returns><see cref="Expansions" />Expansions</returns>
        public bool ListExpansions(Callback<Expansions> callback, string environmentId, string collectionId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListExpansions`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `ListExpansions`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `ListExpansions`");

            RequestObject<Expansions> req = new RequestObject<Expansions>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "ListExpansions"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListExpansionsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/expansions", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListExpansionsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Expansions> response = new DetailedResponse<Expansions>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Expansions>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListExpansionsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Expansions>)req).Callback != null)
                ((RequestObject<Expansions>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Add a document.
        ///
        /// Add a document to a collection with optional metadata.
        ///
        ///   * The **version** query parameter is still required.
        ///
        ///   * Returns immediately after the system has accepted the document for processing.
        ///
        ///   * The user must provide document content, metadata, or both. If the request is missing both document
        /// content and metadata, it is rejected.
        ///
        ///   * The user can set the **Content-Type** parameter on the **file** part to indicate the media type of the
        /// document. If the **Content-Type** parameter is missing or is one of the generic media types (for example,
        /// `application/octet-stream`), then the service attempts to automatically detect the document's media type.
        ///
        ///   * The following field names are reserved and will be filtered out if present after normalization: `id`,
        /// `score`, `highlight`, and any field with the prefix of: `_`, `+`, or `-`
        ///
        ///   * Fields with empty name values after normalization are filtered out before indexing.
        ///
        ///   * Fields containing the following characters after normalization are filtered out before indexing: `#` and
        /// `,`
        ///
        ///  **Note:** Documents can be added with a specific **document_id** by using the
        /// **_/v1/environments/{environment_id}/collections/{collection_id}/documents** method.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="file">The content of the document to ingest. The maximum supported file size when adding a file
        /// to a collection is 50 megabytes, the maximum supported file size when testing a confiruration is 1 megabyte.
        /// Files larger than the supported size are rejected. (optional)</param>
        /// <param name="filename">The filename for file. (optional)</param>
        /// <param name="fileContentType">The content type of file. (optional)</param>
        /// <param name="metadata">If you're using the Data Crawler to upload your documents, you can test a document
        /// against the type of metadata that the Data Crawler might send. The maximum supported metadata file size is 1
        /// MB. Metadata parts larger than 1 MB are rejected.
        /// Example:  ``` {
        ///   "Creator": "Johnny Appleseed",
        ///   "Subject": "Apples"
        /// } ```. (optional)</param>
        /// <returns><see cref="DocumentAccepted" />DocumentAccepted</returns>
        public bool AddDocument(Callback<DocumentAccepted> callback, string environmentId, string collectionId, System.IO.MemoryStream file = null, string filename = null, string fileContentType = null, string metadata = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `AddDocument`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `AddDocument`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `AddDocument`");

            RequestObject<DocumentAccepted> req = new RequestObject<DocumentAccepted>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "AddDocument"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (file != null)
            {
                req.Forms["file"] = new RESTConnector.Form(file, filename, fileContentType);
            }
            if (!string.IsNullOrEmpty(metadata))
            {
                req.Forms["metadata"] = new RESTConnector.Form(metadata);
            }

            req.OnResponse = OnAddDocumentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/documents", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnAddDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DocumentAccepted> response = new DetailedResponse<DocumentAccepted>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DocumentAccepted>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnAddDocumentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DocumentAccepted>)req).Callback != null)
                ((RequestObject<DocumentAccepted>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete a document.
        ///
        /// If the given document ID is invalid, or if the document is not found, then the a success response is
        /// returned (HTTP status code `200`) with the status set to 'deleted'.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="documentId">The ID of the document.</param>
        /// <returns><see cref="DeleteDocumentResponse" />DeleteDocumentResponse</returns>
        public bool DeleteDocument(Callback<DeleteDocumentResponse> callback, string environmentId, string collectionId, string documentId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteDocument`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `DeleteDocument`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `DeleteDocument`");
            if (string.IsNullOrEmpty(documentId))
                throw new ArgumentNullException("`documentId` is required for `DeleteDocument`");

            RequestObject<DeleteDocumentResponse> req = new RequestObject<DeleteDocumentResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "DeleteDocument"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteDocumentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/documents/{2}", environmentId, collectionId, documentId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DeleteDocumentResponse> response = new DetailedResponse<DeleteDocumentResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DeleteDocumentResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnDeleteDocumentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DeleteDocumentResponse>)req).Callback != null)
                ((RequestObject<DeleteDocumentResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get document details.
        ///
        /// Fetch status details about a submitted document. **Note:** this operation does not return the document
        /// itself. Instead, it returns only the document's processing status and any notices (warnings or errors) that
        /// were generated when the document was ingested. Use the query API to retrieve the actual document content.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="documentId">The ID of the document.</param>
        /// <returns><see cref="DocumentStatus" />DocumentStatus</returns>
        public bool GetDocumentStatus(Callback<DocumentStatus> callback, string environmentId, string collectionId, string documentId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetDocumentStatus`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `GetDocumentStatus`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `GetDocumentStatus`");
            if (string.IsNullOrEmpty(documentId))
                throw new ArgumentNullException("`documentId` is required for `GetDocumentStatus`");

            RequestObject<DocumentStatus> req = new RequestObject<DocumentStatus>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "GetDocumentStatus"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetDocumentStatusResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/documents/{2}", environmentId, collectionId, documentId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetDocumentStatusResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DocumentStatus> response = new DetailedResponse<DocumentStatus>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DocumentStatus>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetDocumentStatusResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DocumentStatus>)req).Callback != null)
                ((RequestObject<DocumentStatus>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update a document.
        ///
        /// Replace an existing document or add a document with a specified **document_id**. Starts ingesting a document
        /// with optional metadata.
        ///
        /// **Note:** When uploading a new document with this method it automatically replaces any document stored with
        /// the same **document_id** if it exists.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="documentId">The ID of the document.</param>
        /// <param name="file">The content of the document to ingest. The maximum supported file size when adding a file
        /// to a collection is 50 megabytes, the maximum supported file size when testing a confiruration is 1 megabyte.
        /// Files larger than the supported size are rejected. (optional)</param>
        /// <param name="filename">The filename for file. (optional)</param>
        /// <param name="fileContentType">The content type of file. (optional)</param>
        /// <param name="metadata">If you're using the Data Crawler to upload your documents, you can test a document
        /// against the type of metadata that the Data Crawler might send. The maximum supported metadata file size is 1
        /// MB. Metadata parts larger than 1 MB are rejected.
        /// Example:  ``` {
        ///   "Creator": "Johnny Appleseed",
        ///   "Subject": "Apples"
        /// } ```. (optional)</param>
        /// <returns><see cref="DocumentAccepted" />DocumentAccepted</returns>
        public bool UpdateDocument(Callback<DocumentAccepted> callback, string environmentId, string collectionId, string documentId, System.IO.MemoryStream file = null, string filename = null, string fileContentType = null, string metadata = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateDocument`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `UpdateDocument`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `UpdateDocument`");
            if (string.IsNullOrEmpty(documentId))
                throw new ArgumentNullException("`documentId` is required for `UpdateDocument`");

            RequestObject<DocumentAccepted> req = new RequestObject<DocumentAccepted>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "UpdateDocument"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (file != null)
            {
                req.Forms["file"] = new RESTConnector.Form(file, filename, fileContentType);
            }
            if (!string.IsNullOrEmpty(metadata))
            {
                req.Forms["metadata"] = new RESTConnector.Form(metadata);
            }

            req.OnResponse = OnUpdateDocumentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/documents/{2}", environmentId, collectionId, documentId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnUpdateDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DocumentAccepted> response = new DetailedResponse<DocumentAccepted>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DocumentAccepted>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnUpdateDocumentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DocumentAccepted>)req).Callback != null)
                ((RequestObject<DocumentAccepted>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Long environment queries.
        ///
        /// Complex queries might be too long for a standard method query. By using this method, you can construct
        /// longer queries. However, these queries may take longer to complete than the standard method. For details,
        /// see the [Discovery service
        /// documentation](https://cloud.ibm.com/docs/services/discovery?topic=discovery-query-concepts#query-concepts).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="filter">A cacheable query that excludes documents that don't mention the query content. Filter
        /// searches are better for metadata-type searches and for assessing the concepts in the data set.
        /// (optional)</param>
        /// <param name="query">A query search returns all documents in your data set with full enrichments and full
        /// text, but with the most relevant documents listed first. Use a query search when you want to find the most
        /// relevant search results. You cannot use **natural_language_query** and **query** at the same time.
        /// (optional)</param>
        /// <param name="naturalLanguageQuery">A natural language query that returns relevant documents by utilizing
        /// training data and natural language understanding. You cannot use **natural_language_query** and **query** at
        /// the same time. (optional)</param>
        /// <param name="passages">A passages query that returns the most relevant passages from the results.
        /// (optional)</param>
        /// <param name="aggregation">An aggregation search that returns an exact answer by combining query search with
        /// filters. Useful for applications to build lists, tables, and time series. For a full list of possible
        /// aggregations, see the Query reference. (optional)</param>
        /// <param name="count">Number of results to return. (optional)</param>
        /// <param name="returnFields">A comma-separated list of the portion of the document hierarchy to return.
        /// (optional)</param>
        /// <param name="offset">The number of query results to skip at the beginning. For example, if the total number
        /// of results that are returned is 10 and the offset is 8, it returns the last two results. (optional)</param>
        /// <param name="sort">A comma-separated list of fields in the document to sort on. You can optionally specify a
        /// sort direction by prefixing the field with `-` for descending or `+` for ascending. Ascending is the default
        /// sort direction if no prefix is specified. This parameter cannot be used in the same query as the **bias**
        /// parameter. (optional)</param>
        /// <param name="highlight">When true, a highlight field is returned for each result which contains the fields
        /// which match the query with `<em></em>` tags around the matching query terms. (optional, default to
        /// false)</param>
        /// <param name="passagesFields">A comma-separated list of fields that passages are drawn from. If this
        /// parameter not specified, then all top-level fields are included. (optional)</param>
        /// <param name="passagesCount">The maximum number of passages to return. The search returns fewer passages if
        /// the requested total is not found. The default is `10`. The maximum is `100`. (optional)</param>
        /// <param name="passagesCharacters">The approximate number of characters that any one passage will have.
        /// (optional)</param>
        /// <param name="deduplicate">When `true`, and used with a Watson Discovery News collection, duplicate results
        /// (based on the contents of the **title** field) are removed. Duplicate comparison is limited to the current
        /// query only; **offset** is not considered. This parameter is currently Beta functionality. (optional, default
        /// to false)</param>
        /// <param name="deduplicateField">When specified, duplicate results based on the field specified are removed
        /// from the returned results. Duplicate comparison is limited to the current query only, **offset** is not
        /// considered. This parameter is currently Beta functionality. (optional)</param>
        /// <param name="collectionIds">A comma-separated list of collection IDs to be queried against. Required when
        /// querying multiple collections, invalid when performing a single collection query. (optional)</param>
        /// <param name="similar">When `true`, results are returned based on their similarity to the document IDs
        /// specified in the **similar.document_ids** parameter. (optional, default to false)</param>
        /// <param name="similarDocumentIds">A comma-separated list of document IDs to find similar documents.
        ///
        /// **Tip:** Include the **natural_language_query** parameter to expand the scope of the document similarity
        /// search with the natural language query. Other query parameters, such as **filter** and **query**, are
        /// subsequently applied and reduce the scope. (optional)</param>
        /// <param name="similarFields">A comma-separated list of field names that are used as a basis for comparison to
        /// identify similar documents. If not specified, the entire document is used for comparison. (optional)</param>
        /// <param name="bias">Field which the returned results will be biased against. The specified field must be
        /// either a **date** or **number** format. When a **date** type field is specified returned results are biased
        /// towards field values closer to the current date. When a **number** type field is specified, returned results
        /// are biased towards higher field values. This parameter cannot be used in the same query as the **sort**
        /// parameter. (optional)</param>
        /// <param name="loggingOptOut">If `true`, queries are not stored in the Discovery **Logs** endpoint. (optional,
        /// default to false)</param>
        /// <returns><see cref="QueryResponse" />QueryResponse</returns>
        public bool FederatedQuery(Callback<QueryResponse> callback, string environmentId, string filter = null, string query = null, string naturalLanguageQuery = null, bool? passages = null, string aggregation = null, long? count = null, string returnFields = null, long? offset = null, string sort = null, bool? highlight = null, string passagesFields = null, long? passagesCount = null, long? passagesCharacters = null, bool? deduplicate = null, string deduplicateField = null, string collectionIds = null, bool? similar = null, string similarDocumentIds = null, string similarFields = null, string bias = null, bool? loggingOptOut = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `FederatedQuery`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `FederatedQuery`");

            RequestObject<QueryResponse> req = new RequestObject<QueryResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "FederatedQuery"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            if (loggingOptOut != null)
            {
                req.Headers["X-Watson-Logging-Opt-Out"] = (bool)loggingOptOut ? "true" : "false";
            }

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(filter))
                bodyObject["filter"] = filter;
            if (!string.IsNullOrEmpty(query))
                bodyObject["query"] = query;
            if (!string.IsNullOrEmpty(naturalLanguageQuery))
                bodyObject["natural_language_query"] = naturalLanguageQuery;
            if (passages != null)
                bodyObject["passages"] = JToken.FromObject(passages);
            if (!string.IsNullOrEmpty(aggregation))
                bodyObject["aggregation"] = aggregation;
            if (count != null)
                bodyObject["count"] = JToken.FromObject(count);
            if (!string.IsNullOrEmpty(returnFields))
                bodyObject["return"] = returnFields;
            if (offset != null)
                bodyObject["offset"] = JToken.FromObject(offset);
            if (!string.IsNullOrEmpty(sort))
                bodyObject["sort"] = sort;
            if (highlight != null)
                bodyObject["highlight"] = JToken.FromObject(highlight);
            if (!string.IsNullOrEmpty(passagesFields))
                bodyObject["passages.fields"] = passagesFields;
            if (passagesCount != null)
                bodyObject["passages.count"] = JToken.FromObject(passagesCount);
            if (passagesCharacters != null)
                bodyObject["passages.characters"] = JToken.FromObject(passagesCharacters);
            if (deduplicate != null)
                bodyObject["deduplicate"] = JToken.FromObject(deduplicate);
            if (!string.IsNullOrEmpty(deduplicateField))
                bodyObject["deduplicate.field"] = deduplicateField;
            if (!string.IsNullOrEmpty(collectionIds))
                bodyObject["collection_ids"] = collectionIds;
            if (similar != null)
                bodyObject["similar"] = JToken.FromObject(similar);
            if (!string.IsNullOrEmpty(similarDocumentIds))
                bodyObject["similar.document_ids"] = similarDocumentIds;
            if (!string.IsNullOrEmpty(similarFields))
                bodyObject["similar.fields"] = similarFields;
            if (!string.IsNullOrEmpty(bias))
                bodyObject["bias"] = bias;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnFederatedQueryResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/query", environmentId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnFederatedQueryResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<QueryResponse> response = new DetailedResponse<QueryResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<QueryResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnFederatedQueryResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<QueryResponse>)req).Callback != null)
                ((RequestObject<QueryResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Query multiple collection system notices.
        ///
        /// Queries for notices (errors or warnings) that might have been generated by the system. Notices are generated
        /// when ingesting documents and performing relevance training. See the [Discovery service
        /// documentation](https://cloud.ibm.com/docs/services/discovery?topic=discovery-query-concepts#query-concepts)
        /// for more details on the query language.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionIds">A comma-separated list of collection IDs to be queried against.</param>
        /// <param name="filter">A cacheable query that excludes documents that don't mention the query content. Filter
        /// searches are better for metadata-type searches and for assessing the concepts in the data set.
        /// (optional)</param>
        /// <param name="query">A query search returns all documents in your data set with full enrichments and full
        /// text, but with the most relevant documents listed first. Use a query search when you want to find the most
        /// relevant search results. You cannot use **natural_language_query** and **query** at the same time.
        /// (optional)</param>
        /// <param name="naturalLanguageQuery">A natural language query that returns relevant documents by utilizing
        /// training data and natural language understanding. You cannot use **natural_language_query** and **query** at
        /// the same time. (optional)</param>
        /// <param name="aggregation">An aggregation search that returns an exact answer by combining query search with
        /// filters. Useful for applications to build lists, tables, and time series. For a full list of possible
        /// aggregations, see the Query reference. (optional)</param>
        /// <param name="count">Number of results to return. The maximum for the **count** and **offset** values
        /// together in any one query is **10000**. (optional)</param>
        /// <param name="returnFields">A comma-separated list of the portion of the document hierarchy to return.
        /// (optional)</param>
        /// <param name="offset">The number of query results to skip at the beginning. For example, if the total number
        /// of results that are returned is 10 and the offset is 8, it returns the last two results. The maximum for the
        /// **count** and **offset** values together in any one query is **10000**. (optional)</param>
        /// <param name="sort">A comma-separated list of fields in the document to sort on. You can optionally specify a
        /// sort direction by prefixing the field with `-` for descending or `+` for ascending. Ascending is the default
        /// sort direction if no prefix is specified. (optional)</param>
        /// <param name="highlight">When true, a highlight field is returned for each result which contains the fields
        /// which match the query with `<em></em>` tags around the matching query terms. (optional, default to
        /// false)</param>
        /// <param name="deduplicateField">When specified, duplicate results based on the field specified are removed
        /// from the returned results. Duplicate comparison is limited to the current query only, **offset** is not
        /// considered. This parameter is currently Beta functionality. (optional)</param>
        /// <param name="similar">When `true`, results are returned based on their similarity to the document IDs
        /// specified in the **similar.document_ids** parameter. (optional, default to false)</param>
        /// <param name="similarDocumentIds">A comma-separated list of document IDs to find similar documents.
        ///
        /// **Tip:** Include the **natural_language_query** parameter to expand the scope of the document similarity
        /// search with the natural language query. Other query parameters, such as **filter** and **query**, are
        /// subsequently applied and reduce the scope. (optional)</param>
        /// <param name="similarFields">A comma-separated list of field names that are used as a basis for comparison to
        /// identify similar documents. If not specified, the entire document is used for comparison. (optional)</param>
        /// <returns><see cref="QueryNoticesResponse" />QueryNoticesResponse</returns>
        public bool FederatedQueryNotices(Callback<QueryNoticesResponse> callback, string environmentId, List<string> collectionIds, string filter = null, string query = null, string naturalLanguageQuery = null, string aggregation = null, long? count = null, List<string> returnFields = null, long? offset = null, List<string> sort = null, bool? highlight = null, string deduplicateField = null, bool? similar = null, List<string> similarDocumentIds = null, List<string> similarFields = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `FederatedQueryNotices`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `FederatedQueryNotices`");
            if (collectionIds == null)
                throw new ArgumentNullException("`collectionIds` is required for `FederatedQueryNotices`");

            RequestObject<QueryNoticesResponse> req = new RequestObject<QueryNoticesResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "FederatedQueryNotices"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (collectionIds != null && collectionIds.Count > 0)
            {
                req.Parameters["collection_ids"] = string.Join(",", collectionIds.ToArray());
            }
            if (!string.IsNullOrEmpty(filter))
            {
                req.Parameters["filter"] = filter;
            }
            if (!string.IsNullOrEmpty(query))
            {
                req.Parameters["query"] = query;
            }
            if (!string.IsNullOrEmpty(naturalLanguageQuery))
            {
                req.Parameters["natural_language_query"] = naturalLanguageQuery;
            }
            if (!string.IsNullOrEmpty(aggregation))
            {
                req.Parameters["aggregation"] = aggregation;
            }
            if (count != null)
            {
                req.Parameters["count"] = count;
            }
            if (returnFields != null && returnFields.Count > 0)
            {
                req.Parameters["return"] = string.Join(",", returnFields.ToArray());
            }
            if (offset != null)
            {
                req.Parameters["offset"] = offset;
            }
            if (sort != null && sort.Count > 0)
            {
                req.Parameters["sort"] = string.Join(",", sort.ToArray());
            }
            if (highlight != null)
            {
                req.Parameters["highlight"] = (bool)highlight ? "true" : "false";
            }
            if (!string.IsNullOrEmpty(deduplicateField))
            {
                req.Parameters["deduplicate.field"] = deduplicateField;
            }
            if (similar != null)
            {
                req.Parameters["similar"] = (bool)similar ? "true" : "false";
            }
            if (similarDocumentIds != null && similarDocumentIds.Count > 0)
            {
                req.Parameters["similar.document_ids"] = string.Join(",", similarDocumentIds.ToArray());
            }
            if (similarFields != null && similarFields.Count > 0)
            {
                req.Parameters["similar.fields"] = string.Join(",", similarFields.ToArray());
            }

            req.OnResponse = OnFederatedQueryNoticesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/notices", environmentId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnFederatedQueryNoticesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<QueryNoticesResponse> response = new DetailedResponse<QueryNoticesResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<QueryNoticesResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnFederatedQueryNoticesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<QueryNoticesResponse>)req).Callback != null)
                ((RequestObject<QueryNoticesResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Long collection queries.
        ///
        /// Complex queries might be too long for a standard method query. By using this method, you can construct
        /// longer queries. However, these queries may take longer to complete than the standard method. For details,
        /// see the [Discovery service
        /// documentation](https://cloud.ibm.com/docs/services/discovery?topic=discovery-query-concepts#query-concepts).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="filter">A cacheable query that excludes documents that don't mention the query content. Filter
        /// searches are better for metadata-type searches and for assessing the concepts in the data set.
        /// (optional)</param>
        /// <param name="query">A query search returns all documents in your data set with full enrichments and full
        /// text, but with the most relevant documents listed first. Use a query search when you want to find the most
        /// relevant search results. You cannot use **natural_language_query** and **query** at the same time.
        /// (optional)</param>
        /// <param name="naturalLanguageQuery">A natural language query that returns relevant documents by utilizing
        /// training data and natural language understanding. You cannot use **natural_language_query** and **query** at
        /// the same time. (optional)</param>
        /// <param name="passages">A passages query that returns the most relevant passages from the results.
        /// (optional)</param>
        /// <param name="aggregation">An aggregation search that returns an exact answer by combining query search with
        /// filters. Useful for applications to build lists, tables, and time series. For a full list of possible
        /// aggregations, see the Query reference. (optional)</param>
        /// <param name="count">Number of results to return. (optional)</param>
        /// <param name="returnFields">A comma-separated list of the portion of the document hierarchy to return.
        /// (optional)</param>
        /// <param name="offset">The number of query results to skip at the beginning. For example, if the total number
        /// of results that are returned is 10 and the offset is 8, it returns the last two results. (optional)</param>
        /// <param name="sort">A comma-separated list of fields in the document to sort on. You can optionally specify a
        /// sort direction by prefixing the field with `-` for descending or `+` for ascending. Ascending is the default
        /// sort direction if no prefix is specified. This parameter cannot be used in the same query as the **bias**
        /// parameter. (optional)</param>
        /// <param name="highlight">When true, a highlight field is returned for each result which contains the fields
        /// which match the query with `<em></em>` tags around the matching query terms. (optional, default to
        /// false)</param>
        /// <param name="passagesFields">A comma-separated list of fields that passages are drawn from. If this
        /// parameter not specified, then all top-level fields are included. (optional)</param>
        /// <param name="passagesCount">The maximum number of passages to return. The search returns fewer passages if
        /// the requested total is not found. The default is `10`. The maximum is `100`. (optional)</param>
        /// <param name="passagesCharacters">The approximate number of characters that any one passage will have.
        /// (optional)</param>
        /// <param name="deduplicate">When `true`, and used with a Watson Discovery News collection, duplicate results
        /// (based on the contents of the **title** field) are removed. Duplicate comparison is limited to the current
        /// query only; **offset** is not considered. This parameter is currently Beta functionality. (optional, default
        /// to false)</param>
        /// <param name="deduplicateField">When specified, duplicate results based on the field specified are removed
        /// from the returned results. Duplicate comparison is limited to the current query only, **offset** is not
        /// considered. This parameter is currently Beta functionality. (optional)</param>
        /// <param name="collectionIds">A comma-separated list of collection IDs to be queried against. Required when
        /// querying multiple collections, invalid when performing a single collection query. (optional)</param>
        /// <param name="similar">When `true`, results are returned based on their similarity to the document IDs
        /// specified in the **similar.document_ids** parameter. (optional, default to false)</param>
        /// <param name="similarDocumentIds">A comma-separated list of document IDs to find similar documents.
        ///
        /// **Tip:** Include the **natural_language_query** parameter to expand the scope of the document similarity
        /// search with the natural language query. Other query parameters, such as **filter** and **query**, are
        /// subsequently applied and reduce the scope. (optional)</param>
        /// <param name="similarFields">A comma-separated list of field names that are used as a basis for comparison to
        /// identify similar documents. If not specified, the entire document is used for comparison. (optional)</param>
        /// <param name="bias">Field which the returned results will be biased against. The specified field must be
        /// either a **date** or **number** format. When a **date** type field is specified returned results are biased
        /// towards field values closer to the current date. When a **number** type field is specified, returned results
        /// are biased towards higher field values. This parameter cannot be used in the same query as the **sort**
        /// parameter. (optional)</param>
        /// <param name="loggingOptOut">If `true`, queries are not stored in the Discovery **Logs** endpoint. (optional,
        /// default to false)</param>
        /// <returns><see cref="QueryResponse" />QueryResponse</returns>
        public bool Query(Callback<QueryResponse> callback, string environmentId, string collectionId, string filter = null, string query = null, string naturalLanguageQuery = null, bool? passages = null, string aggregation = null, long? count = null, string returnFields = null, long? offset = null, string sort = null, bool? highlight = null, string passagesFields = null, long? passagesCount = null, long? passagesCharacters = null, bool? deduplicate = null, string deduplicateField = null, string collectionIds = null, bool? similar = null, string similarDocumentIds = null, string similarFields = null, string bias = null, bool? loggingOptOut = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `Query`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `Query`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `Query`");

            RequestObject<QueryResponse> req = new RequestObject<QueryResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "Query"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            if (loggingOptOut != null)
            {
                req.Headers["X-Watson-Logging-Opt-Out"] = (bool)loggingOptOut ? "true" : "false";
            }

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(filter))
                bodyObject["filter"] = filter;
            if (!string.IsNullOrEmpty(query))
                bodyObject["query"] = query;
            if (!string.IsNullOrEmpty(naturalLanguageQuery))
                bodyObject["natural_language_query"] = naturalLanguageQuery;
            if (passages != null)
                bodyObject["passages"] = JToken.FromObject(passages);
            if (!string.IsNullOrEmpty(aggregation))
                bodyObject["aggregation"] = aggregation;
            if (count != null)
                bodyObject["count"] = JToken.FromObject(count);
            if (!string.IsNullOrEmpty(returnFields))
                bodyObject["return"] = returnFields;
            if (offset != null)
                bodyObject["offset"] = JToken.FromObject(offset);
            if (!string.IsNullOrEmpty(sort))
                bodyObject["sort"] = sort;
            if (highlight != null)
                bodyObject["highlight"] = JToken.FromObject(highlight);
            if (!string.IsNullOrEmpty(passagesFields))
                bodyObject["passages.fields"] = passagesFields;
            if (passagesCount != null)
                bodyObject["passages.count"] = JToken.FromObject(passagesCount);
            if (passagesCharacters != null)
                bodyObject["passages.characters"] = JToken.FromObject(passagesCharacters);
            if (deduplicate != null)
                bodyObject["deduplicate"] = JToken.FromObject(deduplicate);
            if (!string.IsNullOrEmpty(deduplicateField))
                bodyObject["deduplicate.field"] = deduplicateField;
            if (!string.IsNullOrEmpty(collectionIds))
                bodyObject["collection_ids"] = collectionIds;
            if (similar != null)
                bodyObject["similar"] = JToken.FromObject(similar);
            if (!string.IsNullOrEmpty(similarDocumentIds))
                bodyObject["similar.document_ids"] = similarDocumentIds;
            if (!string.IsNullOrEmpty(similarFields))
                bodyObject["similar.fields"] = similarFields;
            if (!string.IsNullOrEmpty(bias))
                bodyObject["bias"] = bias;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnQueryResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/query", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnQueryResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<QueryResponse> response = new DetailedResponse<QueryResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<QueryResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnQueryResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<QueryResponse>)req).Callback != null)
                ((RequestObject<QueryResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Knowledge Graph entity query.
        ///
        /// See the [Knowledge Graph documentation](https://cloud.ibm.com/docs/services/discovery?topic=discovery-kg#kg)
        /// for more details.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="feature">The entity query feature to perform. Supported features are `disambiguate` and
        /// `similar_entities`. (optional)</param>
        /// <param name="entity">A text string that appears within the entity text field. (optional)</param>
        /// <param name="context">Entity text to provide context for the queried entity and rank based on that
        /// association. For example, if you wanted to query the city of London in England your query would look for
        /// `London` with the context of `England`. (optional)</param>
        /// <param name="count">The number of results to return. The default is `10`. The maximum is `1000`.
        /// (optional)</param>
        /// <param name="evidenceCount">The number of evidence items to return for each result. The default is `0`. The
        /// maximum number of evidence items per query is 10,000. (optional)</param>
        /// <returns><see cref="QueryEntitiesResponse" />QueryEntitiesResponse</returns>
        public bool QueryEntities(Callback<QueryEntitiesResponse> callback, string environmentId, string collectionId, string feature = null, QueryEntitiesEntity entity = null, QueryEntitiesContext context = null, long? count = null, long? evidenceCount = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `QueryEntities`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `QueryEntities`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `QueryEntities`");

            RequestObject<QueryEntitiesResponse> req = new RequestObject<QueryEntitiesResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "QueryEntities"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(feature))
                bodyObject["feature"] = feature;
            if (entity != null)
                bodyObject["entity"] = JToken.FromObject(entity);
            if (context != null)
                bodyObject["context"] = JToken.FromObject(context);
            if (count != null)
                bodyObject["count"] = JToken.FromObject(count);
            if (evidenceCount != null)
                bodyObject["evidence_count"] = JToken.FromObject(evidenceCount);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnQueryEntitiesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/query_entities", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnQueryEntitiesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<QueryEntitiesResponse> response = new DetailedResponse<QueryEntitiesResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<QueryEntitiesResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnQueryEntitiesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<QueryEntitiesResponse>)req).Callback != null)
                ((RequestObject<QueryEntitiesResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Query system notices.
        ///
        /// Queries for notices (errors or warnings) that might have been generated by the system. Notices are generated
        /// when ingesting documents and performing relevance training. See the [Discovery service
        /// documentation](https://cloud.ibm.com/docs/services/discovery?topic=discovery-query-concepts#query-concepts)
        /// for more details on the query language.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="filter">A cacheable query that excludes documents that don't mention the query content. Filter
        /// searches are better for metadata-type searches and for assessing the concepts in the data set.
        /// (optional)</param>
        /// <param name="query">A query search returns all documents in your data set with full enrichments and full
        /// text, but with the most relevant documents listed first. Use a query search when you want to find the most
        /// relevant search results. You cannot use **natural_language_query** and **query** at the same time.
        /// (optional)</param>
        /// <param name="naturalLanguageQuery">A natural language query that returns relevant documents by utilizing
        /// training data and natural language understanding. You cannot use **natural_language_query** and **query** at
        /// the same time. (optional)</param>
        /// <param name="passages">A passages query that returns the most relevant passages from the results.
        /// (optional)</param>
        /// <param name="aggregation">An aggregation search that returns an exact answer by combining query search with
        /// filters. Useful for applications to build lists, tables, and time series. For a full list of possible
        /// aggregations, see the Query reference. (optional)</param>
        /// <param name="count">Number of results to return. The maximum for the **count** and **offset** values
        /// together in any one query is **10000**. (optional)</param>
        /// <param name="returnFields">A comma-separated list of the portion of the document hierarchy to return.
        /// (optional)</param>
        /// <param name="offset">The number of query results to skip at the beginning. For example, if the total number
        /// of results that are returned is 10 and the offset is 8, it returns the last two results. The maximum for the
        /// **count** and **offset** values together in any one query is **10000**. (optional)</param>
        /// <param name="sort">A comma-separated list of fields in the document to sort on. You can optionally specify a
        /// sort direction by prefixing the field with `-` for descending or `+` for ascending. Ascending is the default
        /// sort direction if no prefix is specified. (optional)</param>
        /// <param name="highlight">When true, a highlight field is returned for each result which contains the fields
        /// which match the query with `<em></em>` tags around the matching query terms. (optional, default to
        /// false)</param>
        /// <param name="passagesFields">A comma-separated list of fields that passages are drawn from. If this
        /// parameter not specified, then all top-level fields are included. (optional)</param>
        /// <param name="passagesCount">The maximum number of passages to return. The search returns fewer passages if
        /// the requested total is not found. (optional)</param>
        /// <param name="passagesCharacters">The approximate number of characters that any one passage will have.
        /// (optional)</param>
        /// <param name="deduplicateField">When specified, duplicate results based on the field specified are removed
        /// from the returned results. Duplicate comparison is limited to the current query only, **offset** is not
        /// considered. This parameter is currently Beta functionality. (optional)</param>
        /// <param name="similar">When `true`, results are returned based on their similarity to the document IDs
        /// specified in the **similar.document_ids** parameter. (optional, default to false)</param>
        /// <param name="similarDocumentIds">A comma-separated list of document IDs to find similar documents.
        ///
        /// **Tip:** Include the **natural_language_query** parameter to expand the scope of the document similarity
        /// search with the natural language query. Other query parameters, such as **filter** and **query**, are
        /// subsequently applied and reduce the scope. (optional)</param>
        /// <param name="similarFields">A comma-separated list of field names that are used as a basis for comparison to
        /// identify similar documents. If not specified, the entire document is used for comparison. (optional)</param>
        /// <returns><see cref="QueryNoticesResponse" />QueryNoticesResponse</returns>
        public bool QueryNotices(Callback<QueryNoticesResponse> callback, string environmentId, string collectionId, string filter = null, string query = null, string naturalLanguageQuery = null, bool? passages = null, string aggregation = null, long? count = null, List<string> returnFields = null, long? offset = null, List<string> sort = null, bool? highlight = null, List<string> passagesFields = null, long? passagesCount = null, long? passagesCharacters = null, string deduplicateField = null, bool? similar = null, List<string> similarDocumentIds = null, List<string> similarFields = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `QueryNotices`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `QueryNotices`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `QueryNotices`");

            RequestObject<QueryNoticesResponse> req = new RequestObject<QueryNoticesResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "QueryNotices"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(filter))
            {
                req.Parameters["filter"] = filter;
            }
            if (!string.IsNullOrEmpty(query))
            {
                req.Parameters["query"] = query;
            }
            if (!string.IsNullOrEmpty(naturalLanguageQuery))
            {
                req.Parameters["natural_language_query"] = naturalLanguageQuery;
            }
            if (passages != null)
            {
                req.Parameters["passages"] = (bool)passages ? "true" : "false";
            }
            if (!string.IsNullOrEmpty(aggregation))
            {
                req.Parameters["aggregation"] = aggregation;
            }
            if (count != null)
            {
                req.Parameters["count"] = count;
            }
            if (returnFields != null && returnFields.Count > 0)
            {
                req.Parameters["return"] = string.Join(",", returnFields.ToArray());
            }
            if (offset != null)
            {
                req.Parameters["offset"] = offset;
            }
            if (sort != null && sort.Count > 0)
            {
                req.Parameters["sort"] = string.Join(",", sort.ToArray());
            }
            if (highlight != null)
            {
                req.Parameters["highlight"] = (bool)highlight ? "true" : "false";
            }
            if (passagesFields != null && passagesFields.Count > 0)
            {
                req.Parameters["passages.fields"] = string.Join(",", passagesFields.ToArray());
            }
            if (passagesCount != null)
            {
                req.Parameters["passages.count"] = passagesCount;
            }
            if (passagesCharacters != null)
            {
                req.Parameters["passages.characters"] = passagesCharacters;
            }
            if (!string.IsNullOrEmpty(deduplicateField))
            {
                req.Parameters["deduplicate.field"] = deduplicateField;
            }
            if (similar != null)
            {
                req.Parameters["similar"] = (bool)similar ? "true" : "false";
            }
            if (similarDocumentIds != null && similarDocumentIds.Count > 0)
            {
                req.Parameters["similar.document_ids"] = string.Join(",", similarDocumentIds.ToArray());
            }
            if (similarFields != null && similarFields.Count > 0)
            {
                req.Parameters["similar.fields"] = string.Join(",", similarFields.ToArray());
            }

            req.OnResponse = OnQueryNoticesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/notices", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnQueryNoticesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<QueryNoticesResponse> response = new DetailedResponse<QueryNoticesResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<QueryNoticesResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnQueryNoticesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<QueryNoticesResponse>)req).Callback != null)
                ((RequestObject<QueryNoticesResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Knowledge Graph relationship query.
        ///
        /// See the [Knowledge Graph documentation](https://cloud.ibm.com/docs/services/discovery?topic=discovery-kg#kg)
        /// for more details.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="entities">An array of entities to find relationships for. (optional)</param>
        /// <param name="context">Entity text to provide context for the queried entity and rank based on that
        /// association. For example, if you wanted to query the city of London in England your query would look for
        /// `London` with the context of `England`. (optional)</param>
        /// <param name="sort">The sorting method for the relationships, can be `score` or `frequency`. `frequency` is
        /// the number of unique times each entity is identified. The default is `score`. This parameter cannot be used
        /// in the same query as the **bias** parameter. (optional)</param>
        /// <param name="filter"> (optional)</param>
        /// <param name="count">The number of results to return. The default is `10`. The maximum is `1000`.
        /// (optional)</param>
        /// <param name="evidenceCount">The number of evidence items to return for each result. The default is `0`. The
        /// maximum number of evidence items per query is 10,000. (optional)</param>
        /// <returns><see cref="QueryRelationsResponse" />QueryRelationsResponse</returns>
        public bool QueryRelations(Callback<QueryRelationsResponse> callback, string environmentId, string collectionId, List<QueryRelationsEntity> entities = null, QueryEntitiesContext context = null, string sort = null, QueryRelationsFilter filter = null, long? count = null, long? evidenceCount = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `QueryRelations`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `QueryRelations`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `QueryRelations`");

            RequestObject<QueryRelationsResponse> req = new RequestObject<QueryRelationsResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "QueryRelations"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (entities != null && entities.Count > 0)
                bodyObject["entities"] = JToken.FromObject(entities);
            if (context != null)
                bodyObject["context"] = JToken.FromObject(context);
            if (!string.IsNullOrEmpty(sort))
                bodyObject["sort"] = sort;
            if (filter != null)
                bodyObject["filter"] = JToken.FromObject(filter);
            if (count != null)
                bodyObject["count"] = JToken.FromObject(count);
            if (evidenceCount != null)
                bodyObject["evidence_count"] = JToken.FromObject(evidenceCount);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnQueryRelationsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/query_relations", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnQueryRelationsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<QueryRelationsResponse> response = new DetailedResponse<QueryRelationsResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<QueryRelationsResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnQueryRelationsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<QueryRelationsResponse>)req).Callback != null)
                ((RequestObject<QueryRelationsResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Add query to training data.
        ///
        /// Adds a query to the training data for this collection. The query can contain a filter and natural language
        /// query.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="naturalLanguageQuery">The natural text query for the new training query. (optional)</param>
        /// <param name="filter">The filter used on the collection before the **natural_language_query** is applied.
        /// (optional)</param>
        /// <param name="examples">Array of training examples. (optional)</param>
        /// <returns><see cref="TrainingQuery" />TrainingQuery</returns>
        public bool AddTrainingData(Callback<TrainingQuery> callback, string environmentId, string collectionId, string naturalLanguageQuery = null, string filter = null, List<TrainingExample> examples = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `AddTrainingData`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `AddTrainingData`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `AddTrainingData`");

            RequestObject<TrainingQuery> req = new RequestObject<TrainingQuery>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "AddTrainingData"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(naturalLanguageQuery))
                bodyObject["natural_language_query"] = naturalLanguageQuery;
            if (!string.IsNullOrEmpty(filter))
                bodyObject["filter"] = filter;
            if (examples != null && examples.Count > 0)
                bodyObject["examples"] = JToken.FromObject(examples);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnAddTrainingDataResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/training_data", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnAddTrainingDataResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TrainingQuery> response = new DetailedResponse<TrainingQuery>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingQuery>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnAddTrainingDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingQuery>)req).Callback != null)
                ((RequestObject<TrainingQuery>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Add example to training data query.
        ///
        /// Adds a example to this training data query.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="queryId">The ID of the query used for training.</param>
        /// <param name="documentId">The document ID associated with this training example. (optional)</param>
        /// <param name="crossReference">The cross reference associated with this training example. (optional)</param>
        /// <param name="relevance">The relevance of the training example. (optional)</param>
        /// <returns><see cref="TrainingExample" />TrainingExample</returns>
        public bool CreateTrainingExample(Callback<TrainingExample> callback, string environmentId, string collectionId, string queryId, string documentId = null, string crossReference = null, long? relevance = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateTrainingExample`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `CreateTrainingExample`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `CreateTrainingExample`");
            if (string.IsNullOrEmpty(queryId))
                throw new ArgumentNullException("`queryId` is required for `CreateTrainingExample`");

            RequestObject<TrainingExample> req = new RequestObject<TrainingExample>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "CreateTrainingExample"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(documentId))
                bodyObject["document_id"] = documentId;
            if (!string.IsNullOrEmpty(crossReference))
                bodyObject["cross_reference"] = crossReference;
            if (relevance != null)
                bodyObject["relevance"] = JToken.FromObject(relevance);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateTrainingExampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/training_data/{2}/examples", environmentId, collectionId, queryId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateTrainingExampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TrainingExample> response = new DetailedResponse<TrainingExample>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingExample>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateTrainingExampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingExample>)req).Callback != null)
                ((RequestObject<TrainingExample>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete all training data.
        ///
        /// Deletes all training data from a collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteAllTrainingData(Callback<object> callback, string environmentId, string collectionId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteAllTrainingData`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `DeleteAllTrainingData`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `DeleteAllTrainingData`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "DeleteAllTrainingData"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteAllTrainingDataResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/training_data", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteAllTrainingDataResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("DiscoveryService.OnDeleteAllTrainingDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete a training data query.
        ///
        /// Removes the training data query and all associated examples from the training data set.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="queryId">The ID of the query used for training.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteTrainingData(Callback<object> callback, string environmentId, string collectionId, string queryId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteTrainingData`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `DeleteTrainingData`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `DeleteTrainingData`");
            if (string.IsNullOrEmpty(queryId))
                throw new ArgumentNullException("`queryId` is required for `DeleteTrainingData`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "DeleteTrainingData"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteTrainingDataResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/training_data/{2}", environmentId, collectionId, queryId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteTrainingDataResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("DiscoveryService.OnDeleteTrainingDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete example for training data query.
        ///
        /// Deletes the example document with the given ID from the training data query.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="queryId">The ID of the query used for training.</param>
        /// <param name="exampleId">The ID of the document as it is indexed.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteTrainingExample(Callback<object> callback, string environmentId, string collectionId, string queryId, string exampleId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteTrainingExample`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `DeleteTrainingExample`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `DeleteTrainingExample`");
            if (string.IsNullOrEmpty(queryId))
                throw new ArgumentNullException("`queryId` is required for `DeleteTrainingExample`");
            if (string.IsNullOrEmpty(exampleId))
                throw new ArgumentNullException("`exampleId` is required for `DeleteTrainingExample`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "DeleteTrainingExample"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteTrainingExampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/training_data/{2}/examples/{3}", environmentId, collectionId, queryId, exampleId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteTrainingExampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("DiscoveryService.OnDeleteTrainingExampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get details about a query.
        ///
        /// Gets details for a specific training data query, including the query string and all examples.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="queryId">The ID of the query used for training.</param>
        /// <returns><see cref="TrainingQuery" />TrainingQuery</returns>
        public bool GetTrainingData(Callback<TrainingQuery> callback, string environmentId, string collectionId, string queryId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetTrainingData`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `GetTrainingData`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `GetTrainingData`");
            if (string.IsNullOrEmpty(queryId))
                throw new ArgumentNullException("`queryId` is required for `GetTrainingData`");

            RequestObject<TrainingQuery> req = new RequestObject<TrainingQuery>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "GetTrainingData"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetTrainingDataResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/training_data/{2}", environmentId, collectionId, queryId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetTrainingDataResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TrainingQuery> response = new DetailedResponse<TrainingQuery>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingQuery>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetTrainingDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingQuery>)req).Callback != null)
                ((RequestObject<TrainingQuery>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get details for training data example.
        ///
        /// Gets the details for this training example.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="queryId">The ID of the query used for training.</param>
        /// <param name="exampleId">The ID of the document as it is indexed.</param>
        /// <returns><see cref="TrainingExample" />TrainingExample</returns>
        public bool GetTrainingExample(Callback<TrainingExample> callback, string environmentId, string collectionId, string queryId, string exampleId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetTrainingExample`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `GetTrainingExample`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `GetTrainingExample`");
            if (string.IsNullOrEmpty(queryId))
                throw new ArgumentNullException("`queryId` is required for `GetTrainingExample`");
            if (string.IsNullOrEmpty(exampleId))
                throw new ArgumentNullException("`exampleId` is required for `GetTrainingExample`");

            RequestObject<TrainingExample> req = new RequestObject<TrainingExample>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "GetTrainingExample"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetTrainingExampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/training_data/{2}/examples/{3}", environmentId, collectionId, queryId, exampleId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetTrainingExampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TrainingExample> response = new DetailedResponse<TrainingExample>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingExample>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetTrainingExampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingExample>)req).Callback != null)
                ((RequestObject<TrainingExample>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List training data.
        ///
        /// Lists the training data for the specified collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <returns><see cref="TrainingDataSet" />TrainingDataSet</returns>
        public bool ListTrainingData(Callback<TrainingDataSet> callback, string environmentId, string collectionId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListTrainingData`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `ListTrainingData`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `ListTrainingData`");

            RequestObject<TrainingDataSet> req = new RequestObject<TrainingDataSet>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "ListTrainingData"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListTrainingDataResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/training_data", environmentId, collectionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListTrainingDataResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TrainingDataSet> response = new DetailedResponse<TrainingDataSet>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingDataSet>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListTrainingDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingDataSet>)req).Callback != null)
                ((RequestObject<TrainingDataSet>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List examples for a training data query.
        ///
        /// List all examples for this training data query.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="queryId">The ID of the query used for training.</param>
        /// <returns><see cref="TrainingExampleList" />TrainingExampleList</returns>
        public bool ListTrainingExamples(Callback<TrainingExampleList> callback, string environmentId, string collectionId, string queryId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListTrainingExamples`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `ListTrainingExamples`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `ListTrainingExamples`");
            if (string.IsNullOrEmpty(queryId))
                throw new ArgumentNullException("`queryId` is required for `ListTrainingExamples`");

            RequestObject<TrainingExampleList> req = new RequestObject<TrainingExampleList>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "ListTrainingExamples"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListTrainingExamplesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/training_data/{2}/examples", environmentId, collectionId, queryId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListTrainingExamplesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TrainingExampleList> response = new DetailedResponse<TrainingExampleList>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingExampleList>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListTrainingExamplesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingExampleList>)req).Callback != null)
                ((RequestObject<TrainingExampleList>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Change label or cross reference for example.
        ///
        /// Changes the label or cross reference query for this training data example.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="queryId">The ID of the query used for training.</param>
        /// <param name="exampleId">The ID of the document as it is indexed.</param>
        /// <param name="crossReference">The example to add. (optional)</param>
        /// <param name="relevance">The relevance value for this example. (optional)</param>
        /// <returns><see cref="TrainingExample" />TrainingExample</returns>
        public bool UpdateTrainingExample(Callback<TrainingExample> callback, string environmentId, string collectionId, string queryId, string exampleId, string crossReference = null, long? relevance = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateTrainingExample`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `UpdateTrainingExample`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `UpdateTrainingExample`");
            if (string.IsNullOrEmpty(queryId))
                throw new ArgumentNullException("`queryId` is required for `UpdateTrainingExample`");
            if (string.IsNullOrEmpty(exampleId))
                throw new ArgumentNullException("`exampleId` is required for `UpdateTrainingExample`");

            RequestObject<TrainingExample> req = new RequestObject<TrainingExample>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "UpdateTrainingExample"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(crossReference))
                bodyObject["cross_reference"] = crossReference;
            if (relevance != null)
                bodyObject["relevance"] = JToken.FromObject(relevance);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateTrainingExampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/collections/{1}/training_data/{2}/examples/{3}", environmentId, collectionId, queryId, exampleId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnUpdateTrainingExampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TrainingExample> response = new DetailedResponse<TrainingExample>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingExample>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnUpdateTrainingExampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingExample>)req).Callback != null)
                ((RequestObject<TrainingExample>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete labeled data.
        ///
        /// Deletes all data associated with a specified customer ID. The method has no effect if no data is associated
        /// with the customer ID.
        ///
        /// You associate a customer ID with data by passing the **X-Watson-Metadata** header with a request that passes
        /// data. For more information about personal data and customer IDs, see [Information
        /// security](https://cloud.ibm.com/docs/services/discovery?topic=discovery-information-security#information-security).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customerId">The customer ID for which all data is to be deleted.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteUserData(Callback<object> callback, string customerId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteUserData`");
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException("`customerId` is required for `DeleteUserData`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "DeleteUserData"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(customerId))
            {
                req.Parameters["customer_id"] = customerId;
            }

            req.OnResponse = OnDeleteUserDataResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/user_data");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteUserDataResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("DiscoveryService.OnDeleteUserDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create event.
        ///
        /// The **Events** API can be used to create log entries that are associated with specific queries. For example,
        /// you can record which documents in the results set were "clicked" by a user and when that click occured.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="type">The event type to be created.</param>
        /// <param name="data">Query event data object.</param>
        /// <returns><see cref="CreateEventResponse" />CreateEventResponse</returns>
        public bool CreateEvent(Callback<CreateEventResponse> callback, string type, EventData data)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateEvent`");
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException("`type` is required for `CreateEvent`");
            if (data == null)
                throw new ArgumentNullException("`data` is required for `CreateEvent`");

            RequestObject<CreateEventResponse> req = new RequestObject<CreateEventResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "CreateEvent"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(type))
                bodyObject["type"] = type;
            if (data != null)
                bodyObject["data"] = JToken.FromObject(data);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateEventResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/events");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateEventResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<CreateEventResponse> response = new DetailedResponse<CreateEventResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<CreateEventResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateEventResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<CreateEventResponse>)req).Callback != null)
                ((RequestObject<CreateEventResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Percentage of queries with an associated event.
        ///
        /// The percentage of queries using the **natural_language_query** parameter that have a corresponding "click"
        /// event over a specified time window.  This metric requires having integrated event tracking in your
        /// application using the **Events** API.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="startTime">Metric is computed from data recorded after this timestamp; must be in
        /// `YYYY-MM-DDThh:mm:ssZ` format. (optional)</param>
        /// <param name="endTime">Metric is computed from data recorded before this timestamp; must be in
        /// `YYYY-MM-DDThh:mm:ssZ` format. (optional)</param>
        /// <param name="resultType">The type of result to consider when calculating the metric. (optional)</param>
        /// <returns><see cref="MetricResponse" />MetricResponse</returns>
        public bool GetMetricsEventRate(Callback<MetricResponse> callback, DateTime? startTime = null, DateTime? endTime = null, string resultType = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetMetricsEventRate`");

            RequestObject<MetricResponse> req = new RequestObject<MetricResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "GetMetricsEventRate"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (startTime != null)
            {
                req.Parameters["start_time"] = startTime;
            }
            if (endTime != null)
            {
                req.Parameters["end_time"] = endTime;
            }
            if (!string.IsNullOrEmpty(resultType))
            {
                req.Parameters["result_type"] = resultType;
            }

            req.OnResponse = OnGetMetricsEventRateResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/metrics/event_rate");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetMetricsEventRateResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<MetricResponse> response = new DetailedResponse<MetricResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<MetricResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetMetricsEventRateResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<MetricResponse>)req).Callback != null)
                ((RequestObject<MetricResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Number of queries over time.
        ///
        /// Total number of queries using the **natural_language_query** parameter over a specific time window.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="startTime">Metric is computed from data recorded after this timestamp; must be in
        /// `YYYY-MM-DDThh:mm:ssZ` format. (optional)</param>
        /// <param name="endTime">Metric is computed from data recorded before this timestamp; must be in
        /// `YYYY-MM-DDThh:mm:ssZ` format. (optional)</param>
        /// <param name="resultType">The type of result to consider when calculating the metric. (optional)</param>
        /// <returns><see cref="MetricResponse" />MetricResponse</returns>
        public bool GetMetricsQuery(Callback<MetricResponse> callback, DateTime? startTime = null, DateTime? endTime = null, string resultType = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetMetricsQuery`");

            RequestObject<MetricResponse> req = new RequestObject<MetricResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "GetMetricsQuery"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (startTime != null)
            {
                req.Parameters["start_time"] = startTime;
            }
            if (endTime != null)
            {
                req.Parameters["end_time"] = endTime;
            }
            if (!string.IsNullOrEmpty(resultType))
            {
                req.Parameters["result_type"] = resultType;
            }

            req.OnResponse = OnGetMetricsQueryResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/metrics/number_of_queries");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetMetricsQueryResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<MetricResponse> response = new DetailedResponse<MetricResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<MetricResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetMetricsQueryResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<MetricResponse>)req).Callback != null)
                ((RequestObject<MetricResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Number of queries with an event over time.
        ///
        /// Total number of queries using the **natural_language_query** parameter that have a corresponding "click"
        /// event over a specified time window. This metric requires having integrated event tracking in your
        /// application using the **Events** API.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="startTime">Metric is computed from data recorded after this timestamp; must be in
        /// `YYYY-MM-DDThh:mm:ssZ` format. (optional)</param>
        /// <param name="endTime">Metric is computed from data recorded before this timestamp; must be in
        /// `YYYY-MM-DDThh:mm:ssZ` format. (optional)</param>
        /// <param name="resultType">The type of result to consider when calculating the metric. (optional)</param>
        /// <returns><see cref="MetricResponse" />MetricResponse</returns>
        public bool GetMetricsQueryEvent(Callback<MetricResponse> callback, DateTime? startTime = null, DateTime? endTime = null, string resultType = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetMetricsQueryEvent`");

            RequestObject<MetricResponse> req = new RequestObject<MetricResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "GetMetricsQueryEvent"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (startTime != null)
            {
                req.Parameters["start_time"] = startTime;
            }
            if (endTime != null)
            {
                req.Parameters["end_time"] = endTime;
            }
            if (!string.IsNullOrEmpty(resultType))
            {
                req.Parameters["result_type"] = resultType;
            }

            req.OnResponse = OnGetMetricsQueryEventResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/metrics/number_of_queries_with_event");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetMetricsQueryEventResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<MetricResponse> response = new DetailedResponse<MetricResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<MetricResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetMetricsQueryEventResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<MetricResponse>)req).Callback != null)
                ((RequestObject<MetricResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Number of queries with no search results over time.
        ///
        /// Total number of queries using the **natural_language_query** parameter that have no results returned over a
        /// specified time window.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="startTime">Metric is computed from data recorded after this timestamp; must be in
        /// `YYYY-MM-DDThh:mm:ssZ` format. (optional)</param>
        /// <param name="endTime">Metric is computed from data recorded before this timestamp; must be in
        /// `YYYY-MM-DDThh:mm:ssZ` format. (optional)</param>
        /// <param name="resultType">The type of result to consider when calculating the metric. (optional)</param>
        /// <returns><see cref="MetricResponse" />MetricResponse</returns>
        public bool GetMetricsQueryNoResults(Callback<MetricResponse> callback, DateTime? startTime = null, DateTime? endTime = null, string resultType = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetMetricsQueryNoResults`");

            RequestObject<MetricResponse> req = new RequestObject<MetricResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "GetMetricsQueryNoResults"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (startTime != null)
            {
                req.Parameters["start_time"] = startTime;
            }
            if (endTime != null)
            {
                req.Parameters["end_time"] = endTime;
            }
            if (!string.IsNullOrEmpty(resultType))
            {
                req.Parameters["result_type"] = resultType;
            }

            req.OnResponse = OnGetMetricsQueryNoResultsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/metrics/number_of_queries_with_no_search_results");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetMetricsQueryNoResultsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<MetricResponse> response = new DetailedResponse<MetricResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<MetricResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetMetricsQueryNoResultsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<MetricResponse>)req).Callback != null)
                ((RequestObject<MetricResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Most frequent query tokens with an event.
        ///
        /// The most frequent query tokens parsed from the **natural_language_query** parameter and their corresponding
        /// "click" event rate within the recording period (queries and events are stored for 30 days). A query token is
        /// an individual word or unigram within the query string.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="count">Number of results to return. The maximum for the **count** and **offset** values
        /// together in any one query is **10000**. (optional)</param>
        /// <returns><see cref="MetricTokenResponse" />MetricTokenResponse</returns>
        public bool GetMetricsQueryTokenEvent(Callback<MetricTokenResponse> callback, long? count = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetMetricsQueryTokenEvent`");

            RequestObject<MetricTokenResponse> req = new RequestObject<MetricTokenResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "GetMetricsQueryTokenEvent"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (count != null)
            {
                req.Parameters["count"] = count;
            }

            req.OnResponse = OnGetMetricsQueryTokenEventResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/metrics/top_query_tokens_with_event_rate");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetMetricsQueryTokenEventResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<MetricTokenResponse> response = new DetailedResponse<MetricTokenResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<MetricTokenResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetMetricsQueryTokenEventResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<MetricTokenResponse>)req).Callback != null)
                ((RequestObject<MetricTokenResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Search the query and event log.
        ///
        /// Searches the query and event log to find query sessions that match the specified criteria. Searching the
        /// **logs** endpoint uses the standard Discovery query syntax for the parameters that are supported.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="filter">A cacheable query that excludes documents that don't mention the query content. Filter
        /// searches are better for metadata-type searches and for assessing the concepts in the data set.
        /// (optional)</param>
        /// <param name="query">A query search returns all documents in your data set with full enrichments and full
        /// text, but with the most relevant documents listed first. Use a query search when you want to find the most
        /// relevant search results. You cannot use **natural_language_query** and **query** at the same time.
        /// (optional)</param>
        /// <param name="count">Number of results to return. The maximum for the **count** and **offset** values
        /// together in any one query is **10000**. (optional)</param>
        /// <param name="offset">The number of query results to skip at the beginning. For example, if the total number
        /// of results that are returned is 10 and the offset is 8, it returns the last two results. The maximum for the
        /// **count** and **offset** values together in any one query is **10000**. (optional)</param>
        /// <param name="sort">A comma-separated list of fields in the document to sort on. You can optionally specify a
        /// sort direction by prefixing the field with `-` for descending or `+` for ascending. Ascending is the default
        /// sort direction if no prefix is specified. (optional)</param>
        /// <returns><see cref="LogQueryResponse" />LogQueryResponse</returns>
        public bool QueryLog(Callback<LogQueryResponse> callback, string filter = null, string query = null, long? count = null, long? offset = null, List<string> sort = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `QueryLog`");

            RequestObject<LogQueryResponse> req = new RequestObject<LogQueryResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "QueryLog"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(filter))
            {
                req.Parameters["filter"] = filter;
            }
            if (!string.IsNullOrEmpty(query))
            {
                req.Parameters["query"] = query;
            }
            if (count != null)
            {
                req.Parameters["count"] = count;
            }
            if (offset != null)
            {
                req.Parameters["offset"] = offset;
            }
            if (sort != null && sort.Count > 0)
            {
                req.Parameters["sort"] = string.Join(",", sort.ToArray());
            }

            req.OnResponse = OnQueryLogResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/logs");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnQueryLogResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<LogQueryResponse> response = new DetailedResponse<LogQueryResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<LogQueryResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnQueryLogResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<LogQueryResponse>)req).Callback != null)
                ((RequestObject<LogQueryResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create credentials.
        ///
        /// Creates a set of credentials to connect to a remote source. Created credentials are used in a configuration
        /// to associate a collection with the remote source.
        ///
        /// **Note:** All credentials are sent over an encrypted connection and encrypted at rest.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="sourceType">The source that this credentials object connects to.
        /// -  `box` indicates the credentials are used to connect an instance of Enterprise Box.
        /// -  `salesforce` indicates the credentials are used to connect to Salesforce.
        /// -  `sharepoint` indicates the credentials are used to connect to Microsoft SharePoint Online.
        /// -  `web_crawl` indicates the credentials are used to perform a web crawl.
        /// =  `cloud_object_storage` indicates the credentials are used to connect to an IBM Cloud Object Store.
        /// (optional)</param>
        /// <param name="credentialDetails">Object containing details of the stored credentials.
        ///
        /// Obtain credentials for your source from the administrator of the source. (optional)</param>
        /// <returns><see cref="ModelCredentials" />ModelCredentials</returns>
        public bool CreateCredentials(Callback<ModelCredentials> callback, string environmentId, string sourceType = null, CredentialDetails credentialDetails = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateCredentials`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `CreateCredentials`");

            RequestObject<ModelCredentials> req = new RequestObject<ModelCredentials>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "CreateCredentials"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(sourceType))
                bodyObject["source_type"] = sourceType;
            if (credentialDetails != null)
                bodyObject["credential_details"] = JToken.FromObject(credentialDetails);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateCredentialsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/credentials", environmentId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateCredentialsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ModelCredentials> response = new DetailedResponse<ModelCredentials>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ModelCredentials>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateCredentialsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ModelCredentials>)req).Callback != null)
                ((RequestObject<ModelCredentials>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete credentials.
        ///
        /// Deletes a set of stored credentials from your Discovery instance.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="credentialId">The unique identifier for a set of source credentials.</param>
        /// <returns><see cref="DeleteCredentials" />DeleteCredentials</returns>
        public bool DeleteCredentials(Callback<DeleteCredentials> callback, string environmentId, string credentialId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteCredentials`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `DeleteCredentials`");
            if (string.IsNullOrEmpty(credentialId))
                throw new ArgumentNullException("`credentialId` is required for `DeleteCredentials`");

            RequestObject<DeleteCredentials> req = new RequestObject<DeleteCredentials>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "DeleteCredentials"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteCredentialsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/credentials/{1}", environmentId, credentialId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteCredentialsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DeleteCredentials> response = new DetailedResponse<DeleteCredentials>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DeleteCredentials>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnDeleteCredentialsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DeleteCredentials>)req).Callback != null)
                ((RequestObject<DeleteCredentials>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// View Credentials.
        ///
        /// Returns details about the specified credentials.
        ///
        ///  **Note:** Secure credential information such as a password or SSH key is never returned and must be
        /// obtained from the source system.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="credentialId">The unique identifier for a set of source credentials.</param>
        /// <returns><see cref="ModelCredentials" />ModelCredentials</returns>
        public bool GetCredentials(Callback<ModelCredentials> callback, string environmentId, string credentialId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetCredentials`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `GetCredentials`");
            if (string.IsNullOrEmpty(credentialId))
                throw new ArgumentNullException("`credentialId` is required for `GetCredentials`");

            RequestObject<ModelCredentials> req = new RequestObject<ModelCredentials>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "GetCredentials"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetCredentialsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/credentials/{1}", environmentId, credentialId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetCredentialsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ModelCredentials> response = new DetailedResponse<ModelCredentials>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ModelCredentials>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetCredentialsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ModelCredentials>)req).Callback != null)
                ((RequestObject<ModelCredentials>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List credentials.
        ///
        /// List all the source credentials that have been created for this service instance.
        ///
        ///  **Note:**  All credentials are sent over an encrypted connection and encrypted at rest.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <returns><see cref="CredentialsList" />CredentialsList</returns>
        public bool ListCredentials(Callback<CredentialsList> callback, string environmentId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListCredentials`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `ListCredentials`");

            RequestObject<CredentialsList> req = new RequestObject<CredentialsList>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "ListCredentials"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListCredentialsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/credentials", environmentId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListCredentialsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<CredentialsList> response = new DetailedResponse<CredentialsList>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<CredentialsList>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListCredentialsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<CredentialsList>)req).Callback != null)
                ((RequestObject<CredentialsList>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update credentials.
        ///
        /// Updates an existing set of source credentials.
        ///
        /// **Note:** All credentials are sent over an encrypted connection and encrypted at rest.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="credentialId">The unique identifier for a set of source credentials.</param>
        /// <param name="sourceType">The source that this credentials object connects to.
        /// -  `box` indicates the credentials are used to connect an instance of Enterprise Box.
        /// -  `salesforce` indicates the credentials are used to connect to Salesforce.
        /// -  `sharepoint` indicates the credentials are used to connect to Microsoft SharePoint Online.
        /// -  `web_crawl` indicates the credentials are used to perform a web crawl.
        /// =  `cloud_object_storage` indicates the credentials are used to connect to an IBM Cloud Object Store.
        /// (optional)</param>
        /// <param name="credentialDetails">Object containing details of the stored credentials.
        ///
        /// Obtain credentials for your source from the administrator of the source. (optional)</param>
        /// <returns><see cref="ModelCredentials" />ModelCredentials</returns>
        public bool UpdateCredentials(Callback<ModelCredentials> callback, string environmentId, string credentialId, string sourceType = null, CredentialDetails credentialDetails = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateCredentials`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `UpdateCredentials`");
            if (string.IsNullOrEmpty(credentialId))
                throw new ArgumentNullException("`credentialId` is required for `UpdateCredentials`");

            RequestObject<ModelCredentials> req = new RequestObject<ModelCredentials>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "UpdateCredentials"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(sourceType))
                bodyObject["source_type"] = sourceType;
            if (credentialDetails != null)
                bodyObject["credential_details"] = JToken.FromObject(credentialDetails);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateCredentialsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/credentials/{1}", environmentId, credentialId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnUpdateCredentialsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ModelCredentials> response = new DetailedResponse<ModelCredentials>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ModelCredentials>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnUpdateCredentialsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ModelCredentials>)req).Callback != null)
                ((RequestObject<ModelCredentials>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create Gateway.
        ///
        /// Create a gateway configuration to use with a remotely installed gateway.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="name">User-defined name. (optional)</param>
        /// <returns><see cref="Gateway" />Gateway</returns>
        public bool CreateGateway(Callback<Gateway> callback, string environmentId, string name = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateGateway`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `CreateGateway`");

            RequestObject<Gateway> req = new RequestObject<Gateway>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "CreateGateway"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(name))
                bodyObject["name"] = name;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateGatewayResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/gateways", environmentId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateGatewayResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Gateway> response = new DetailedResponse<Gateway>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Gateway>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateGatewayResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Gateway>)req).Callback != null)
                ((RequestObject<Gateway>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete Gateway.
        ///
        /// Delete the specified gateway configuration.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="gatewayId">The requested gateway ID.</param>
        /// <returns><see cref="GatewayDelete" />GatewayDelete</returns>
        public bool DeleteGateway(Callback<GatewayDelete> callback, string environmentId, string gatewayId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteGateway`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `DeleteGateway`");
            if (string.IsNullOrEmpty(gatewayId))
                throw new ArgumentNullException("`gatewayId` is required for `DeleteGateway`");

            RequestObject<GatewayDelete> req = new RequestObject<GatewayDelete>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "DeleteGateway"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteGatewayResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/gateways/{1}", environmentId, gatewayId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteGatewayResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<GatewayDelete> response = new DetailedResponse<GatewayDelete>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<GatewayDelete>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnDeleteGatewayResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<GatewayDelete>)req).Callback != null)
                ((RequestObject<GatewayDelete>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List Gateway Details.
        ///
        /// List information about the specified gateway.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="gatewayId">The requested gateway ID.</param>
        /// <returns><see cref="Gateway" />Gateway</returns>
        public bool GetGateway(Callback<Gateway> callback, string environmentId, string gatewayId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetGateway`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `GetGateway`");
            if (string.IsNullOrEmpty(gatewayId))
                throw new ArgumentNullException("`gatewayId` is required for `GetGateway`");

            RequestObject<Gateway> req = new RequestObject<Gateway>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "GetGateway"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetGatewayResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/gateways/{1}", environmentId, gatewayId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetGatewayResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Gateway> response = new DetailedResponse<Gateway>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Gateway>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetGatewayResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Gateway>)req).Callback != null)
                ((RequestObject<Gateway>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List Gateways.
        ///
        /// List the currently configured gateways.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <returns><see cref="GatewayList" />GatewayList</returns>
        public bool ListGateways(Callback<GatewayList> callback, string environmentId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListGateways`");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("`environmentId` is required for `ListGateways`");

            RequestObject<GatewayList> req = new RequestObject<GatewayList>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V1", "ListGateways"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListGatewaysResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/gateways", environmentId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListGatewaysResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<GatewayList> response = new DetailedResponse<GatewayList>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<GatewayList>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListGatewaysResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<GatewayList>)req).Callback != null)
                ((RequestObject<GatewayList>)req).Callback(response, resp.Error);
        }
    }
}