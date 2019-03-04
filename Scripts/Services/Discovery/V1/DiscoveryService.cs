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
using System;
using UnityEngine.Networking;

namespace IBM.Watson.Discovery.V1
{
    public class DiscoveryService : BaseService
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
        /// <param name="body">An object that defines an environment name and optional description. The fields in this
        /// object are not approved for personal information and cannot be deleted based on customer ID.</param>
        /// <returns><see cref="ModelEnvironment" />ModelEnvironment</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateEnvironment(Callback<ModelEnvironment> callback, CreateEnvironmentRequest body, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateEnvironment");
            if (body == null)
                throw new ArgumentNullException("body is required for CreateEnvironment");

            RequestObject<ModelEnvironment> req = new RequestObject<ModelEnvironment>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=CreateEnvironment";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (body != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            }

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
            Dictionary<string, object> customData = ((RequestObject<ModelEnvironment>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ModelEnvironment>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateEnvironmentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ModelEnvironment>)req).Callback != null)
                ((RequestObject<ModelEnvironment>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Delete environment.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <returns><see cref="DeleteEnvironmentResponse" />DeleteEnvironmentResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteEnvironment(Callback<DeleteEnvironmentResponse> callback, string environmentId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteEnvironment");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for DeleteEnvironment");

            RequestObject<DeleteEnvironmentResponse> req = new RequestObject<DeleteEnvironmentResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=DeleteEnvironment";
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
            Dictionary<string, object> customData = ((RequestObject<DeleteEnvironmentResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DeleteEnvironmentResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnDeleteEnvironmentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DeleteEnvironmentResponse>)req).Callback != null)
                ((RequestObject<DeleteEnvironmentResponse>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Get environment info.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <returns><see cref="ModelEnvironment" />ModelEnvironment</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetEnvironment(Callback<ModelEnvironment> callback, string environmentId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetEnvironment");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for GetEnvironment");

            RequestObject<ModelEnvironment> req = new RequestObject<ModelEnvironment>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=GetEnvironment";
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
            Dictionary<string, object> customData = ((RequestObject<ModelEnvironment>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ModelEnvironment>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetEnvironmentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ModelEnvironment>)req).Callback != null)
                ((RequestObject<ModelEnvironment>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// List environments.
        ///
        /// List existing environments for the service instance.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="name">Show only the environment with the given name. (optional)</param>
        /// <returns><see cref="ListEnvironmentsResponse" />ListEnvironmentsResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListEnvironments(Callback<ListEnvironmentsResponse> callback, string name = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListEnvironments");

            RequestObject<ListEnvironmentsResponse> req = new RequestObject<ListEnvironmentsResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=ListEnvironments";
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
            Dictionary<string, object> customData = ((RequestObject<ListEnvironmentsResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ListEnvironmentsResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListEnvironmentsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ListEnvironmentsResponse>)req).Callback != null)
                ((RequestObject<ListEnvironmentsResponse>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListFields(Callback<ListCollectionFieldsResponse> callback, string environmentId, List<string> collectionIds, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListFields");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for ListFields");
            if (collectionIds == null)
                throw new ArgumentNullException("collectionIds is required for ListFields");

            RequestObject<ListCollectionFieldsResponse> req = new RequestObject<ListCollectionFieldsResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=ListFields";
            req.Parameters["version"] = VersionDate;
            req.Parameters["collection_ids"] = collectionIds != null && collectionIds.Count > 0 ? string.Join(",", collectionIds.ToArray()) : null;

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
            Dictionary<string, object> customData = ((RequestObject<ListCollectionFieldsResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ListCollectionFieldsResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListFieldsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ListCollectionFieldsResponse>)req).Callback != null)
                ((RequestObject<ListCollectionFieldsResponse>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Update an environment.
        ///
        /// Updates an environment. The environment's **name** and  **description** parameters can be changed. You must
        /// specify a **name** for the environment.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="body">An object that defines the environment's name and, optionally, description.</param>
        /// <returns><see cref="ModelEnvironment" />ModelEnvironment</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool UpdateEnvironment(Callback<ModelEnvironment> callback, string environmentId, UpdateEnvironmentRequest body, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for UpdateEnvironment");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for UpdateEnvironment");
            if (body == null)
                throw new ArgumentNullException("body is required for UpdateEnvironment");

            RequestObject<ModelEnvironment> req = new RequestObject<ModelEnvironment>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPUT,
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=UpdateEnvironment";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (body != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            }

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
            Dictionary<string, object> customData = ((RequestObject<ModelEnvironment>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ModelEnvironment>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnUpdateEnvironmentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ModelEnvironment>)req).Callback != null)
                ((RequestObject<ModelEnvironment>)req).Callback(response, resp.Error, customData);
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
        /// <param name="configuration">Input an object that enables you to customize how your content is ingested and
        /// what enrichments are added to your data.
        ///
        /// **name** is required and must be unique within the current **environment**. All other properties are
        /// optional.
        ///
        /// If the input configuration contains the **configuration_id**, **created**, or **updated** properties, then
        /// they will be ignored and overridden by the system (an error is not returned so that the overridden fields do
        /// not need to be removed when copying a configuration).
        ///
        /// The configuration can contain unrecognized JSON fields. Any such fields will be ignored and will not
        /// generate an error. This makes it easier to use newer configuration files with older versions of the API and
        /// the service. It also makes it possible for the tooling to add additional metadata and information to the
        /// configuration.</param>
        /// <returns><see cref="Configuration" />Configuration</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateConfiguration(Callback<Configuration> callback, string environmentId, Configuration configuration, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateConfiguration");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for CreateConfiguration");
            if (configuration == null)
                throw new ArgumentNullException("configuration is required for CreateConfiguration");

            RequestObject<Configuration> req = new RequestObject<Configuration>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=CreateConfiguration";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (configuration != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(configuration));
            }

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
            Dictionary<string, object> customData = ((RequestObject<Configuration>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Configuration>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateConfigurationResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Configuration>)req).Callback != null)
                ((RequestObject<Configuration>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteConfiguration(Callback<DeleteConfigurationResponse> callback, string environmentId, string configurationId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteConfiguration");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for DeleteConfiguration");
            if (string.IsNullOrEmpty(configurationId))
                throw new ArgumentNullException("configurationId is required for DeleteConfiguration");

            RequestObject<DeleteConfigurationResponse> req = new RequestObject<DeleteConfigurationResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=DeleteConfiguration";
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
            Dictionary<string, object> customData = ((RequestObject<DeleteConfigurationResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DeleteConfigurationResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnDeleteConfigurationResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DeleteConfigurationResponse>)req).Callback != null)
                ((RequestObject<DeleteConfigurationResponse>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Get configuration details.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="configurationId">The ID of the configuration.</param>
        /// <returns><see cref="Configuration" />Configuration</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetConfiguration(Callback<Configuration> callback, string environmentId, string configurationId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetConfiguration");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for GetConfiguration");
            if (string.IsNullOrEmpty(configurationId))
                throw new ArgumentNullException("configurationId is required for GetConfiguration");

            RequestObject<Configuration> req = new RequestObject<Configuration>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=GetConfiguration";
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
            Dictionary<string, object> customData = ((RequestObject<Configuration>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Configuration>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetConfigurationResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Configuration>)req).Callback != null)
                ((RequestObject<Configuration>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListConfigurations(Callback<ListConfigurationsResponse> callback, string environmentId, string name = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListConfigurations");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for ListConfigurations");

            RequestObject<ListConfigurationsResponse> req = new RequestObject<ListConfigurationsResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=ListConfigurations";
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
            Dictionary<string, object> customData = ((RequestObject<ListConfigurationsResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ListConfigurationsResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListConfigurationsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ListConfigurationsResponse>)req).Callback != null)
                ((RequestObject<ListConfigurationsResponse>)req).Callback(response, resp.Error, customData);
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
        /// <param name="configuration">Input an object that enables you to update and customize how your data is
        /// ingested and what enrichments are added to your data.
        /// The **name** parameter is required and must be unique within the current **environment**. All other
        /// properties are optional, but if they are omitted  the default values replace the current value of each
        /// omitted property.
        ///
        /// If the input configuration contains the **configuration_id**, **created**, or **updated** properties, they
        /// are ignored and overridden by the system, and an error is not returned so that the overridden fields do not
        /// need to be removed when updating a configuration.
        ///
        /// The configuration can contain unrecognized JSON fields. Any such fields are ignored and do not generate an
        /// error. This makes it easier to use newer configuration files with older versions of the API and the service.
        /// It also makes it possible for the tooling to add additional metadata and information to the
        /// configuration.</param>
        /// <returns><see cref="Configuration" />Configuration</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool UpdateConfiguration(Callback<Configuration> callback, string environmentId, string configurationId, Configuration configuration, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for UpdateConfiguration");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for UpdateConfiguration");
            if (string.IsNullOrEmpty(configurationId))
                throw new ArgumentNullException("configurationId is required for UpdateConfiguration");
            if (configuration == null)
                throw new ArgumentNullException("configuration is required for UpdateConfiguration");

            RequestObject<Configuration> req = new RequestObject<Configuration>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPUT,
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=UpdateConfiguration";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (configuration != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(configuration));
            }

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
            Dictionary<string, object> customData = ((RequestObject<Configuration>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Configuration>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnUpdateConfigurationResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Configuration>)req).Callback != null)
                ((RequestObject<Configuration>)req).Callback(response, resp.Error, customData);
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
        /// <param name="file">The content of the document to ingest. The maximum supported file size is 50 megabytes.
        /// Files larger than 50 megabytes is rejected. (optional)</param>
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
        /// <param name="fileContentType">The content type of file. (optional)</param>
        /// <returns><see cref="TestDocument" />TestDocument</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool TestConfigurationInEnvironment(Callback<TestDocument> callback, string environmentId, string configuration = null, System.IO.FileStream file = null, string metadata = null, string step = null, string configurationId = null, string fileContentType = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for TestConfigurationInEnvironment");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for TestConfigurationInEnvironment");

            RequestObject<TestDocument> req = new RequestObject<TestDocument>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=TestConfigurationInEnvironment";
            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (!string.IsNullOrEmpty(configuration))
            {
                req.Forms["configuration"] = new RESTConnector.Form("configuration");
            }
            req.Forms["file"] = new RESTConnector.Form(file, file.Name, fileContentType);
            if (file != null)
            {
                req.Forms["file"] = new RESTConnector.Form(file.ToString());
            }
            if (!string.IsNullOrEmpty(metadata))
            {
                req.Forms["metadata"] = new RESTConnector.Form("metadata");
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
            Dictionary<string, object> customData = ((RequestObject<TestDocument>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TestDocument>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnTestConfigurationInEnvironmentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TestDocument>)req).Callback != null)
                ((RequestObject<TestDocument>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Create a collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="body">Input an object that allows you to add a collection.</param>
        /// <returns><see cref="Collection" />Collection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateCollection(Callback<Collection> callback, string environmentId, CreateCollectionRequest body, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateCollection");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for CreateCollection");
            if (body == null)
                throw new ArgumentNullException("body is required for CreateCollection");

            RequestObject<Collection> req = new RequestObject<Collection>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=CreateCollection";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (body != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            }

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
            Dictionary<string, object> customData = ((RequestObject<Collection>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Collection>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateCollectionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Collection>)req).Callback != null)
                ((RequestObject<Collection>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Delete a collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <returns><see cref="DeleteCollectionResponse" />DeleteCollectionResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteCollection(Callback<DeleteCollectionResponse> callback, string environmentId, string collectionId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteCollection");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for DeleteCollection");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for DeleteCollection");

            RequestObject<DeleteCollectionResponse> req = new RequestObject<DeleteCollectionResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=DeleteCollection";
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
            Dictionary<string, object> customData = ((RequestObject<DeleteCollectionResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DeleteCollectionResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnDeleteCollectionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DeleteCollectionResponse>)req).Callback != null)
                ((RequestObject<DeleteCollectionResponse>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Get collection details.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <returns><see cref="Collection" />Collection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetCollection(Callback<Collection> callback, string environmentId, string collectionId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetCollection");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for GetCollection");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for GetCollection");

            RequestObject<Collection> req = new RequestObject<Collection>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=GetCollection";
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
            Dictionary<string, object> customData = ((RequestObject<Collection>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Collection>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetCollectionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Collection>)req).Callback != null)
                ((RequestObject<Collection>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListCollectionFields(Callback<ListCollectionFieldsResponse> callback, string environmentId, string collectionId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListCollectionFields");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for ListCollectionFields");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for ListCollectionFields");

            RequestObject<ListCollectionFieldsResponse> req = new RequestObject<ListCollectionFieldsResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=ListCollectionFields";
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
            Dictionary<string, object> customData = ((RequestObject<ListCollectionFieldsResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ListCollectionFieldsResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListCollectionFieldsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ListCollectionFieldsResponse>)req).Callback != null)
                ((RequestObject<ListCollectionFieldsResponse>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListCollections(Callback<ListCollectionsResponse> callback, string environmentId, string name = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListCollections");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for ListCollections");

            RequestObject<ListCollectionsResponse> req = new RequestObject<ListCollectionsResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=ListCollections";
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
            Dictionary<string, object> customData = ((RequestObject<ListCollectionsResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ListCollectionsResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListCollectionsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ListCollectionsResponse>)req).Callback != null)
                ((RequestObject<ListCollectionsResponse>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Update a collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="body">Input an object that allows you to update a collection. (optional)</param>
        /// <returns><see cref="Collection" />Collection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool UpdateCollection(Callback<Collection> callback, string environmentId, string collectionId, UpdateCollectionRequest body = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for UpdateCollection");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for UpdateCollection");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for UpdateCollection");

            RequestObject<Collection> req = new RequestObject<Collection>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPUT,
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=UpdateCollection";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (body != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            }

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
            Dictionary<string, object> customData = ((RequestObject<Collection>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Collection>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnUpdateCollectionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Collection>)req).Callback != null)
                ((RequestObject<Collection>)req).Callback(response, resp.Error, customData);
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
        /// <param name="body">An object that defines the expansion list.</param>
        /// <returns><see cref="Expansions" />Expansions</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateExpansions(Callback<Expansions> callback, string environmentId, string collectionId, Expansions body, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateExpansions");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for CreateExpansions");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for CreateExpansions");
            if (body == null)
                throw new ArgumentNullException("body is required for CreateExpansions");

            RequestObject<Expansions> req = new RequestObject<Expansions>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=CreateExpansions";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (body != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            }

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
            Dictionary<string, object> customData = ((RequestObject<Expansions>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Expansions>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateExpansionsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Expansions>)req).Callback != null)
                ((RequestObject<Expansions>)req).Callback(response, resp.Error, customData);
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
        /// <returns><see cref="TokenDictStatusResponse" />TokenDictStatusResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateStopwordList(Callback<TokenDictStatusResponse> callback, string environmentId, string collectionId, System.IO.FileStream stopwordFile, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateStopwordList");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for CreateStopwordList");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for CreateStopwordList");
            if (stopwordFile == null)
                throw new ArgumentNullException("stopwordFile is required for CreateStopwordList");

            RequestObject<TokenDictStatusResponse> req = new RequestObject<TokenDictStatusResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=CreateStopwordList";
            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["stopwordFile"] = new RESTConnector.Form(stopwordFile, stopwordFile.Name, "application/octet-stream");
            if (stopwordFile != null)
            {
                req.Forms["stopwordFile"] = new RESTConnector.Form(stopwordFile.ToString());
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
            Dictionary<string, object> customData = ((RequestObject<TokenDictStatusResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TokenDictStatusResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateStopwordListResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TokenDictStatusResponse>)req).Callback != null)
                ((RequestObject<TokenDictStatusResponse>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Create tokenization dictionary.
        ///
        /// Upload a custom tokenization dictionary to use with the specified collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="tokenizationDictionary">An object that represents the tokenization dictionary to be uploaded.
        /// (optional)</param>
        /// <returns><see cref="TokenDictStatusResponse" />TokenDictStatusResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateTokenizationDictionary(Callback<TokenDictStatusResponse> callback, string environmentId, string collectionId, TokenDict tokenizationDictionary = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateTokenizationDictionary");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for CreateTokenizationDictionary");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for CreateTokenizationDictionary");

            RequestObject<TokenDictStatusResponse> req = new RequestObject<TokenDictStatusResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=CreateTokenizationDictionary";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (tokenizationDictionary != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(tokenizationDictionary));
            }

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
            Dictionary<string, object> customData = ((RequestObject<TokenDictStatusResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TokenDictStatusResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateTokenizationDictionaryResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TokenDictStatusResponse>)req).Callback != null)
                ((RequestObject<TokenDictStatusResponse>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteExpansions(Callback<object> callback, string environmentId, string collectionId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteExpansions");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for DeleteExpansions");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for DeleteExpansions");

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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=DeleteExpansions";
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
                Log.Error("DiscoveryService.OnDeleteExpansionsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteStopwordList(Callback<object> callback, string environmentId, string collectionId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteStopwordList");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for DeleteStopwordList");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for DeleteStopwordList");

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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=DeleteStopwordList";
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
                Log.Error("DiscoveryService.OnDeleteStopwordListResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteTokenizationDictionary(Callback<object> callback, string environmentId, string collectionId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteTokenizationDictionary");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for DeleteTokenizationDictionary");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for DeleteTokenizationDictionary");

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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=DeleteTokenizationDictionary";
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
                Log.Error("DiscoveryService.OnDeleteTokenizationDictionaryResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetTokenizationDictionaryStatus(Callback<TokenDictStatusResponse> callback, string environmentId, string collectionId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetTokenizationDictionaryStatus");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for GetTokenizationDictionaryStatus");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for GetTokenizationDictionaryStatus");

            RequestObject<TokenDictStatusResponse> req = new RequestObject<TokenDictStatusResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=GetTokenizationDictionaryStatus";
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
            Dictionary<string, object> customData = ((RequestObject<TokenDictStatusResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TokenDictStatusResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetTokenizationDictionaryStatusResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TokenDictStatusResponse>)req).Callback != null)
                ((RequestObject<TokenDictStatusResponse>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListExpansions(Callback<Expansions> callback, string environmentId, string collectionId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListExpansions");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for ListExpansions");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for ListExpansions");

            RequestObject<Expansions> req = new RequestObject<Expansions>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=ListExpansions";
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
            Dictionary<string, object> customData = ((RequestObject<Expansions>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Expansions>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListExpansionsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Expansions>)req).Callback != null)
                ((RequestObject<Expansions>)req).Callback(response, resp.Error, customData);
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
        /// `,`.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="file">The content of the document to ingest. The maximum supported file size is 50 megabytes.
        /// Files larger than 50 megabytes is rejected. (optional)</param>
        /// <param name="metadata">If you're using the Data Crawler to upload your documents, you can test a document
        /// against the type of metadata that the Data Crawler might send. The maximum supported metadata file size is 1
        /// MB. Metadata parts larger than 1 MB are rejected.
        /// Example:  ``` {
        ///   "Creator": "Johnny Appleseed",
        ///   "Subject": "Apples"
        /// } ```. (optional)</param>
        /// <param name="fileContentType">The content type of file. (optional)</param>
        /// <returns><see cref="DocumentAccepted" />DocumentAccepted</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool AddDocument(Callback<DocumentAccepted> callback, string environmentId, string collectionId, System.IO.FileStream file = null, string metadata = null, string fileContentType = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for AddDocument");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for AddDocument");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for AddDocument");

            RequestObject<DocumentAccepted> req = new RequestObject<DocumentAccepted>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=AddDocument";
            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["file"] = new RESTConnector.Form(file, file.Name, fileContentType);
            if (file != null)
            {
                req.Forms["file"] = new RESTConnector.Form(file.ToString());
            }
            if (!string.IsNullOrEmpty(metadata))
            {
                req.Forms["metadata"] = new RESTConnector.Form("metadata");
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
            Dictionary<string, object> customData = ((RequestObject<DocumentAccepted>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DocumentAccepted>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnAddDocumentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DocumentAccepted>)req).Callback != null)
                ((RequestObject<DocumentAccepted>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteDocument(Callback<DeleteDocumentResponse> callback, string environmentId, string collectionId, string documentId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteDocument");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for DeleteDocument");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for DeleteDocument");
            if (string.IsNullOrEmpty(documentId))
                throw new ArgumentNullException("documentId is required for DeleteDocument");

            RequestObject<DeleteDocumentResponse> req = new RequestObject<DeleteDocumentResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=DeleteDocument";
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
            Dictionary<string, object> customData = ((RequestObject<DeleteDocumentResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DeleteDocumentResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnDeleteDocumentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DeleteDocumentResponse>)req).Callback != null)
                ((RequestObject<DeleteDocumentResponse>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetDocumentStatus(Callback<DocumentStatus> callback, string environmentId, string collectionId, string documentId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetDocumentStatus");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for GetDocumentStatus");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for GetDocumentStatus");
            if (string.IsNullOrEmpty(documentId))
                throw new ArgumentNullException("documentId is required for GetDocumentStatus");

            RequestObject<DocumentStatus> req = new RequestObject<DocumentStatus>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=GetDocumentStatus";
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
            Dictionary<string, object> customData = ((RequestObject<DocumentStatus>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DocumentStatus>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetDocumentStatusResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DocumentStatus>)req).Callback != null)
                ((RequestObject<DocumentStatus>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Update a document.
        ///
        /// Replace an existing document. Starts ingesting a document with optional metadata.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="documentId">The ID of the document.</param>
        /// <param name="file">The content of the document to ingest. The maximum supported file size is 50 megabytes.
        /// Files larger than 50 megabytes is rejected. (optional)</param>
        /// <param name="metadata">If you're using the Data Crawler to upload your documents, you can test a document
        /// against the type of metadata that the Data Crawler might send. The maximum supported metadata file size is 1
        /// MB. Metadata parts larger than 1 MB are rejected.
        /// Example:  ``` {
        ///   "Creator": "Johnny Appleseed",
        ///   "Subject": "Apples"
        /// } ```. (optional)</param>
        /// <param name="fileContentType">The content type of file. (optional)</param>
        /// <returns><see cref="DocumentAccepted" />DocumentAccepted</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool UpdateDocument(Callback<DocumentAccepted> callback, string environmentId, string collectionId, string documentId, System.IO.FileStream file = null, string metadata = null, string fileContentType = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for UpdateDocument");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for UpdateDocument");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for UpdateDocument");
            if (string.IsNullOrEmpty(documentId))
                throw new ArgumentNullException("documentId is required for UpdateDocument");

            RequestObject<DocumentAccepted> req = new RequestObject<DocumentAccepted>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=UpdateDocument";
            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["file"] = new RESTConnector.Form(file, file.Name, fileContentType);
            if (file != null)
            {
                req.Forms["file"] = new RESTConnector.Form(file.ToString());
            }
            if (!string.IsNullOrEmpty(metadata))
            {
                req.Forms["metadata"] = new RESTConnector.Form("metadata");
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
            Dictionary<string, object> customData = ((RequestObject<DocumentAccepted>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DocumentAccepted>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnUpdateDocumentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DocumentAccepted>)req).Callback != null)
                ((RequestObject<DocumentAccepted>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Long environment queries.
        ///
        /// Complex queries might be too long for a standard method query. By using this method, you can construct
        /// longer queries. However, these queries may take longer to complete than the standard method. For details,
        /// see the [Discovery service documentation](https://console.bluemix.net/docs/services/discovery/using.html).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="queryLong">An object that represents the query to be submitted. (optional)</param>
        /// <param name="loggingOptOut">If `true`, queries are not stored in the Discovery **Logs** endpoint. (optional,
        /// default to false)</param>
        /// <returns><see cref="QueryResponse" />QueryResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool FederatedQuery(Callback<QueryResponse> callback, string environmentId, QueryLarge queryLong = null, bool? loggingOptOut = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for FederatedQuery");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for FederatedQuery");

            RequestObject<QueryResponse> req = new RequestObject<QueryResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=FederatedQuery";
            req.Parameters["version"] = VersionDate;
            if (loggingOptOut != null)
            {
                req.Headers["X-Watson-Logging-Opt-Out"] = (bool)loggingOptOut ? "true" : "false";
            }
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (queryLong != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(queryLong));
            }

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
            Dictionary<string, object> customData = ((RequestObject<QueryResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<QueryResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnFederatedQueryResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<QueryResponse>)req).Callback != null)
                ((RequestObject<QueryResponse>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Query multiple collection system notices.
        ///
        /// Queries for notices (errors or warnings) that might have been generated by the system. Notices are generated
        /// when ingesting documents and performing relevance training. See the [Discovery service
        /// documentation](https://console.bluemix.net/docs/services/discovery/using.html) for more details on the query
        /// language.
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
        /// <param name="count">Number of results to return. (optional, default to 10)</param>
        /// <param name="returnFields">A comma-separated list of the portion of the document hierarchy to return.
        /// (optional)</param>
        /// <param name="offset">The number of query results to skip at the beginning. For example, if the total number
        /// of results that are returned is 10 and the offset is 8, it returns the last two results. (optional)</param>
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool FederatedQueryNotices(Callback<QueryNoticesResponse> callback, string environmentId, List<string> collectionIds, string filter = null, string query = null, string naturalLanguageQuery = null, string aggregation = null, long? count = null, List<string> returnFields = null, long? offset = null, List<string> sort = null, bool? highlight = null, string deduplicateField = null, bool? similar = null, List<string> similarDocumentIds = null, List<string> similarFields = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for FederatedQueryNotices");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for FederatedQueryNotices");
            if (collectionIds == null)
                throw new ArgumentNullException("collectionIds is required for FederatedQueryNotices");

            RequestObject<QueryNoticesResponse> req = new RequestObject<QueryNoticesResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=FederatedQueryNotices";
            req.Parameters["version"] = VersionDate;
            req.Parameters["collection_ids"] = collectionIds != null && collectionIds.Count > 0 ? string.Join(",", collectionIds.ToArray()) : null;
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
            req.Parameters["return"] = returnFields != null && returnFields.Count > 0 ? string.Join(",", returnFields.ToArray()) : null;
            if (offset != null)
            {
                req.Parameters["offset"] = offset;
            }
            req.Parameters["sort"] = sort != null && sort.Count > 0 ? string.Join(",", sort.ToArray()) : null;
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
            req.Parameters["similar.document_ids"] = similarDocumentIds != null && similarDocumentIds.Count > 0 ? string.Join(",", similarDocumentIds.ToArray()) : null;
            req.Parameters["similar.fields"] = similarFields != null && similarFields.Count > 0 ? string.Join(",", similarFields.ToArray()) : null;

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
            Dictionary<string, object> customData = ((RequestObject<QueryNoticesResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<QueryNoticesResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnFederatedQueryNoticesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<QueryNoticesResponse>)req).Callback != null)
                ((RequestObject<QueryNoticesResponse>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Long collection queries.
        ///
        /// Complex queries might be too long for a standard method query. By using this method, you can construct
        /// longer queries. However, these queries may take longer to complete than the standard method. For details,
        /// see the [Discovery service documentation](https://console.bluemix.net/docs/services/discovery/using.html).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="queryLong">An object that represents the query to be submitted. (optional)</param>
        /// <param name="loggingOptOut">If `true`, queries are not stored in the Discovery **Logs** endpoint. (optional,
        /// default to false)</param>
        /// <returns><see cref="QueryResponse" />QueryResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool Query(Callback<QueryResponse> callback, string environmentId, string collectionId, QueryLarge queryLong = null, bool? loggingOptOut = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for Query");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for Query");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for Query");

            RequestObject<QueryResponse> req = new RequestObject<QueryResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=Query";
            req.Parameters["version"] = VersionDate;
            if (loggingOptOut != null)
            {
                req.Headers["X-Watson-Logging-Opt-Out"] = (bool)loggingOptOut ? "true" : "false";
            }
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (queryLong != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(queryLong));
            }

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
            Dictionary<string, object> customData = ((RequestObject<QueryResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<QueryResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnQueryResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<QueryResponse>)req).Callback != null)
                ((RequestObject<QueryResponse>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Knowledge Graph entity query.
        ///
        /// See the [Knowledge Graph
        /// documentation](https://console.bluemix.net/docs/services/discovery/building-kg.html) for more details.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="entityQuery">An object specifying the entities to query, which functions to perform, and any
        /// additional constraints.</param>
        /// <returns><see cref="QueryEntitiesResponse" />QueryEntitiesResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool QueryEntities(Callback<QueryEntitiesResponse> callback, string environmentId, string collectionId, QueryEntities entityQuery, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for QueryEntities");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for QueryEntities");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for QueryEntities");
            if (entityQuery == null)
                throw new ArgumentNullException("entityQuery is required for QueryEntities");

            RequestObject<QueryEntitiesResponse> req = new RequestObject<QueryEntitiesResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=QueryEntities";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (entityQuery != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entityQuery));
            }

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
            Dictionary<string, object> customData = ((RequestObject<QueryEntitiesResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<QueryEntitiesResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnQueryEntitiesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<QueryEntitiesResponse>)req).Callback != null)
                ((RequestObject<QueryEntitiesResponse>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Query system notices.
        ///
        /// Queries for notices (errors or warnings) that might have been generated by the system. Notices are generated
        /// when ingesting documents and performing relevance training. See the [Discovery service
        /// documentation](https://console.bluemix.net/docs/services/discovery/using.html) for more details on the query
        /// language.
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
        /// <param name="count">Number of results to return. (optional, default to 10)</param>
        /// <param name="returnFields">A comma-separated list of the portion of the document hierarchy to return.
        /// (optional)</param>
        /// <param name="offset">The number of query results to skip at the beginning. For example, if the total number
        /// of results that are returned is 10 and the offset is 8, it returns the last two results. (optional)</param>
        /// <param name="sort">A comma-separated list of fields in the document to sort on. You can optionally specify a
        /// sort direction by prefixing the field with `-` for descending or `+` for ascending. Ascending is the default
        /// sort direction if no prefix is specified. (optional)</param>
        /// <param name="highlight">When true, a highlight field is returned for each result which contains the fields
        /// which match the query with `<em></em>` tags around the matching query terms. (optional, default to
        /// false)</param>
        /// <param name="passagesFields">A comma-separated list of fields that passages are drawn from. If this
        /// parameter not specified, then all top-level fields are included. (optional)</param>
        /// <param name="passagesCount">The maximum number of passages to return. The search returns fewer passages if
        /// the requested total is not found. (optional, default to 10)</param>
        /// <param name="passagesCharacters">The approximate number of characters that any one passage will have.
        /// (optional, default to 400)</param>
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool QueryNotices(Callback<QueryNoticesResponse> callback, string environmentId, string collectionId, string filter = null, string query = null, string naturalLanguageQuery = null, bool? passages = null, string aggregation = null, long? count = null, List<string> returnFields = null, long? offset = null, List<string> sort = null, bool? highlight = null, List<string> passagesFields = null, long? passagesCount = null, long? passagesCharacters = null, string deduplicateField = null, bool? similar = null, List<string> similarDocumentIds = null, List<string> similarFields = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for QueryNotices");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for QueryNotices");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for QueryNotices");

            RequestObject<QueryNoticesResponse> req = new RequestObject<QueryNoticesResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=QueryNotices";
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
            req.Parameters["return"] = returnFields != null && returnFields.Count > 0 ? string.Join(",", returnFields.ToArray()) : null;
            if (offset != null)
            {
                req.Parameters["offset"] = offset;
            }
            req.Parameters["sort"] = sort != null && sort.Count > 0 ? string.Join(",", sort.ToArray()) : null;
            if (highlight != null)
            {
                req.Parameters["highlight"] = (bool)highlight ? "true" : "false";
            }
            req.Parameters["passages.fields"] = passagesFields != null && passagesFields.Count > 0 ? string.Join(",", passagesFields.ToArray()) : null;
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
            req.Parameters["similar.document_ids"] = similarDocumentIds != null && similarDocumentIds.Count > 0 ? string.Join(",", similarDocumentIds.ToArray()) : null;
            req.Parameters["similar.fields"] = similarFields != null && similarFields.Count > 0 ? string.Join(",", similarFields.ToArray()) : null;

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
            Dictionary<string, object> customData = ((RequestObject<QueryNoticesResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<QueryNoticesResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnQueryNoticesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<QueryNoticesResponse>)req).Callback != null)
                ((RequestObject<QueryNoticesResponse>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Knowledge Graph relationship query.
        ///
        /// See the [Knowledge Graph
        /// documentation](https://console.bluemix.net/docs/services/discovery/building-kg.html) for more details.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="relationshipQuery">An object that describes the relationships to be queried and any query
        /// constraints (such as filters).</param>
        /// <returns><see cref="QueryRelationsResponse" />QueryRelationsResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool QueryRelations(Callback<QueryRelationsResponse> callback, string environmentId, string collectionId, QueryRelations relationshipQuery, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for QueryRelations");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for QueryRelations");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for QueryRelations");
            if (relationshipQuery == null)
                throw new ArgumentNullException("relationshipQuery is required for QueryRelations");

            RequestObject<QueryRelationsResponse> req = new RequestObject<QueryRelationsResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=QueryRelations";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (relationshipQuery != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(relationshipQuery));
            }

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
            Dictionary<string, object> customData = ((RequestObject<QueryRelationsResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<QueryRelationsResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnQueryRelationsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<QueryRelationsResponse>)req).Callback != null)
                ((RequestObject<QueryRelationsResponse>)req).Callback(response, resp.Error, customData);
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
        /// <param name="body">The body of the training data query that is to be added to the collection's training
        /// data.</param>
        /// <returns><see cref="TrainingQuery" />TrainingQuery</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool AddTrainingData(Callback<TrainingQuery> callback, string environmentId, string collectionId, NewTrainingQuery body, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for AddTrainingData");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for AddTrainingData");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for AddTrainingData");
            if (body == null)
                throw new ArgumentNullException("body is required for AddTrainingData");

            RequestObject<TrainingQuery> req = new RequestObject<TrainingQuery>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=AddTrainingData";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (body != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            }

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
            Dictionary<string, object> customData = ((RequestObject<TrainingQuery>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingQuery>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnAddTrainingDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingQuery>)req).Callback != null)
                ((RequestObject<TrainingQuery>)req).Callback(response, resp.Error, customData);
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
        /// <param name="body">The body of the example that is to be added to the specified query.</param>
        /// <returns><see cref="TrainingExample" />TrainingExample</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateTrainingExample(Callback<TrainingExample> callback, string environmentId, string collectionId, string queryId, TrainingExample body, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateTrainingExample");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for CreateTrainingExample");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for CreateTrainingExample");
            if (string.IsNullOrEmpty(queryId))
                throw new ArgumentNullException("queryId is required for CreateTrainingExample");
            if (body == null)
                throw new ArgumentNullException("body is required for CreateTrainingExample");

            RequestObject<TrainingExample> req = new RequestObject<TrainingExample>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=CreateTrainingExample";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (body != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            }

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
            Dictionary<string, object> customData = ((RequestObject<TrainingExample>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingExample>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateTrainingExampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingExample>)req).Callback != null)
                ((RequestObject<TrainingExample>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteAllTrainingData(Callback<object> callback, string environmentId, string collectionId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteAllTrainingData");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for DeleteAllTrainingData");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for DeleteAllTrainingData");

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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=DeleteAllTrainingData";
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
                Log.Error("DiscoveryService.OnDeleteAllTrainingDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteTrainingData(Callback<object> callback, string environmentId, string collectionId, string queryId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteTrainingData");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for DeleteTrainingData");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for DeleteTrainingData");
            if (string.IsNullOrEmpty(queryId))
                throw new ArgumentNullException("queryId is required for DeleteTrainingData");

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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=DeleteTrainingData";
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
                Log.Error("DiscoveryService.OnDeleteTrainingDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteTrainingExample(Callback<object> callback, string environmentId, string collectionId, string queryId, string exampleId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteTrainingExample");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for DeleteTrainingExample");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for DeleteTrainingExample");
            if (string.IsNullOrEmpty(queryId))
                throw new ArgumentNullException("queryId is required for DeleteTrainingExample");
            if (string.IsNullOrEmpty(exampleId))
                throw new ArgumentNullException("exampleId is required for DeleteTrainingExample");

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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=DeleteTrainingExample";
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
                Log.Error("DiscoveryService.OnDeleteTrainingExampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetTrainingData(Callback<TrainingQuery> callback, string environmentId, string collectionId, string queryId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetTrainingData");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for GetTrainingData");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for GetTrainingData");
            if (string.IsNullOrEmpty(queryId))
                throw new ArgumentNullException("queryId is required for GetTrainingData");

            RequestObject<TrainingQuery> req = new RequestObject<TrainingQuery>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=GetTrainingData";
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
            Dictionary<string, object> customData = ((RequestObject<TrainingQuery>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingQuery>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetTrainingDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingQuery>)req).Callback != null)
                ((RequestObject<TrainingQuery>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetTrainingExample(Callback<TrainingExample> callback, string environmentId, string collectionId, string queryId, string exampleId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetTrainingExample");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for GetTrainingExample");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for GetTrainingExample");
            if (string.IsNullOrEmpty(queryId))
                throw new ArgumentNullException("queryId is required for GetTrainingExample");
            if (string.IsNullOrEmpty(exampleId))
                throw new ArgumentNullException("exampleId is required for GetTrainingExample");

            RequestObject<TrainingExample> req = new RequestObject<TrainingExample>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=GetTrainingExample";
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
            Dictionary<string, object> customData = ((RequestObject<TrainingExample>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingExample>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetTrainingExampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingExample>)req).Callback != null)
                ((RequestObject<TrainingExample>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListTrainingData(Callback<TrainingDataSet> callback, string environmentId, string collectionId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListTrainingData");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for ListTrainingData");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for ListTrainingData");

            RequestObject<TrainingDataSet> req = new RequestObject<TrainingDataSet>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=ListTrainingData";
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
            Dictionary<string, object> customData = ((RequestObject<TrainingDataSet>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingDataSet>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListTrainingDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingDataSet>)req).Callback != null)
                ((RequestObject<TrainingDataSet>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListTrainingExamples(Callback<TrainingExampleList> callback, string environmentId, string collectionId, string queryId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListTrainingExamples");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for ListTrainingExamples");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for ListTrainingExamples");
            if (string.IsNullOrEmpty(queryId))
                throw new ArgumentNullException("queryId is required for ListTrainingExamples");

            RequestObject<TrainingExampleList> req = new RequestObject<TrainingExampleList>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=ListTrainingExamples";
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
            Dictionary<string, object> customData = ((RequestObject<TrainingExampleList>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingExampleList>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListTrainingExamplesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingExampleList>)req).Callback != null)
                ((RequestObject<TrainingExampleList>)req).Callback(response, resp.Error, customData);
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
        /// <param name="body">The body of the example that is to be added to the specified query.</param>
        /// <returns><see cref="TrainingExample" />TrainingExample</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool UpdateTrainingExample(Callback<TrainingExample> callback, string environmentId, string collectionId, string queryId, string exampleId, TrainingExamplePatch body, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for UpdateTrainingExample");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for UpdateTrainingExample");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("collectionId is required for UpdateTrainingExample");
            if (string.IsNullOrEmpty(queryId))
                throw new ArgumentNullException("queryId is required for UpdateTrainingExample");
            if (string.IsNullOrEmpty(exampleId))
                throw new ArgumentNullException("exampleId is required for UpdateTrainingExample");
            if (body == null)
                throw new ArgumentNullException("body is required for UpdateTrainingExample");

            RequestObject<TrainingExample> req = new RequestObject<TrainingExample>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPUT,
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=UpdateTrainingExample";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (body != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            }

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
            Dictionary<string, object> customData = ((RequestObject<TrainingExample>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingExample>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnUpdateTrainingExampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingExample>)req).Callback != null)
                ((RequestObject<TrainingExample>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Delete labeled data.
        ///
        /// Deletes all data associated with a specified customer ID. The method has no effect if no data is associated
        /// with the customer ID.
        ///
        /// You associate a customer ID with data by passing the **X-Watson-Metadata** header with a request that passes
        /// data. For more information about personal data and customer IDs, see [Information
        /// security](https://console.bluemix.net/docs/services/discovery/information-security.html).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customerId">The customer ID for which all data is to be deleted.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteUserData(Callback<object> callback, string customerId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteUserData");
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException("customerId is required for DeleteUserData");

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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=DeleteUserData";
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
                Log.Error("DiscoveryService.OnDeleteUserDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Create event.
        ///
        /// The **Events** API can be used to create log entries that are associated with specific queries. For example,
        /// you can record which documents in the results set were "clicked" by a user and when that click occured.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="queryEvent">An object that defines a query event to be added to the log.</param>
        /// <returns><see cref="CreateEventResponse" />CreateEventResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateEvent(Callback<CreateEventResponse> callback, CreateEventObject queryEvent, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateEvent");
            if (queryEvent == null)
                throw new ArgumentNullException("queryEvent is required for CreateEvent");

            RequestObject<CreateEventResponse> req = new RequestObject<CreateEventResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=CreateEvent";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (queryEvent != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(queryEvent));
            }

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
            Dictionary<string, object> customData = ((RequestObject<CreateEventResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<CreateEventResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateEventResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<CreateEventResponse>)req).Callback != null)
                ((RequestObject<CreateEventResponse>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetMetricsEventRate(Callback<MetricResponse> callback, DateTime? startTime = null, DateTime? endTime = null, string resultType = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetMetricsEventRate");

            RequestObject<MetricResponse> req = new RequestObject<MetricResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=GetMetricsEventRate";
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
            Dictionary<string, object> customData = ((RequestObject<MetricResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<MetricResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetMetricsEventRateResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<MetricResponse>)req).Callback != null)
                ((RequestObject<MetricResponse>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetMetricsQuery(Callback<MetricResponse> callback, DateTime? startTime = null, DateTime? endTime = null, string resultType = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetMetricsQuery");

            RequestObject<MetricResponse> req = new RequestObject<MetricResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=GetMetricsQuery";
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
            Dictionary<string, object> customData = ((RequestObject<MetricResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<MetricResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetMetricsQueryResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<MetricResponse>)req).Callback != null)
                ((RequestObject<MetricResponse>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetMetricsQueryEvent(Callback<MetricResponse> callback, DateTime? startTime = null, DateTime? endTime = null, string resultType = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetMetricsQueryEvent");

            RequestObject<MetricResponse> req = new RequestObject<MetricResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=GetMetricsQueryEvent";
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
            Dictionary<string, object> customData = ((RequestObject<MetricResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<MetricResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetMetricsQueryEventResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<MetricResponse>)req).Callback != null)
                ((RequestObject<MetricResponse>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetMetricsQueryNoResults(Callback<MetricResponse> callback, DateTime? startTime = null, DateTime? endTime = null, string resultType = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetMetricsQueryNoResults");

            RequestObject<MetricResponse> req = new RequestObject<MetricResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=GetMetricsQueryNoResults";
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
            Dictionary<string, object> customData = ((RequestObject<MetricResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<MetricResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetMetricsQueryNoResultsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<MetricResponse>)req).Callback != null)
                ((RequestObject<MetricResponse>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Most frequent query tokens with an event.
        ///
        /// The most frequent query tokens parsed from the **natural_language_query** parameter and their corresponding
        /// "click" event rate within the recording period (queries and events are stored for 30 days). A query token is
        /// an individual word or unigram within the query string.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="count">Number of results to return. (optional, default to 10)</param>
        /// <returns><see cref="MetricTokenResponse" />MetricTokenResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetMetricsQueryTokenEvent(Callback<MetricTokenResponse> callback, long? count = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetMetricsQueryTokenEvent");

            RequestObject<MetricTokenResponse> req = new RequestObject<MetricTokenResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=GetMetricsQueryTokenEvent";
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
            Dictionary<string, object> customData = ((RequestObject<MetricTokenResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<MetricTokenResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetMetricsQueryTokenEventResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<MetricTokenResponse>)req).Callback != null)
                ((RequestObject<MetricTokenResponse>)req).Callback(response, resp.Error, customData);
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
        /// <param name="count">Number of results to return. (optional, default to 10)</param>
        /// <param name="offset">The number of query results to skip at the beginning. For example, if the total number
        /// of results that are returned is 10 and the offset is 8, it returns the last two results. (optional)</param>
        /// <param name="sort">A comma-separated list of fields in the document to sort on. You can optionally specify a
        /// sort direction by prefixing the field with `-` for descending or `+` for ascending. Ascending is the default
        /// sort direction if no prefix is specified. (optional)</param>
        /// <returns><see cref="LogQueryResponse" />LogQueryResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool QueryLog(Callback<LogQueryResponse> callback, string filter = null, string query = null, long? count = null, long? offset = null, List<string> sort = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for QueryLog");

            RequestObject<LogQueryResponse> req = new RequestObject<LogQueryResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=QueryLog";
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
            req.Parameters["sort"] = sort != null && sort.Count > 0 ? string.Join(",", sort.ToArray()) : null;

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
            Dictionary<string, object> customData = ((RequestObject<LogQueryResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<LogQueryResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnQueryLogResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<LogQueryResponse>)req).Callback != null)
                ((RequestObject<LogQueryResponse>)req).Callback(response, resp.Error, customData);
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
        /// <param name="credentialsParameter">An object that defines an individual set of source credentials.</param>
        /// <returns><see cref="ModelCredentials" />ModelCredentials</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateCredentials(Callback<ModelCredentials> callback, string environmentId, ModelCredentials credentialsParameter, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateCredentials");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for CreateCredentials");
            if (credentialsParameter == null)
                throw new ArgumentNullException("credentialsParameter is required for CreateCredentials");

            RequestObject<ModelCredentials> req = new RequestObject<ModelCredentials>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=CreateCredentials";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (credentialsParameter != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(credentialsParameter));
            }

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
            Dictionary<string, object> customData = ((RequestObject<ModelCredentials>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ModelCredentials>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateCredentialsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ModelCredentials>)req).Callback != null)
                ((RequestObject<ModelCredentials>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteCredentials(Callback<DeleteCredentials> callback, string environmentId, string credentialId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteCredentials");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for DeleteCredentials");
            if (string.IsNullOrEmpty(credentialId))
                throw new ArgumentNullException("credentialId is required for DeleteCredentials");

            RequestObject<DeleteCredentials> req = new RequestObject<DeleteCredentials>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=DeleteCredentials";
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
            Dictionary<string, object> customData = ((RequestObject<DeleteCredentials>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DeleteCredentials>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnDeleteCredentialsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DeleteCredentials>)req).Callback != null)
                ((RequestObject<DeleteCredentials>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetCredentials(Callback<ModelCredentials> callback, string environmentId, string credentialId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetCredentials");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for GetCredentials");
            if (string.IsNullOrEmpty(credentialId))
                throw new ArgumentNullException("credentialId is required for GetCredentials");

            RequestObject<ModelCredentials> req = new RequestObject<ModelCredentials>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=GetCredentials";
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
            Dictionary<string, object> customData = ((RequestObject<ModelCredentials>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ModelCredentials>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetCredentialsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ModelCredentials>)req).Callback != null)
                ((RequestObject<ModelCredentials>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListCredentials(Callback<CredentialsList> callback, string environmentId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListCredentials");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for ListCredentials");

            RequestObject<CredentialsList> req = new RequestObject<CredentialsList>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=ListCredentials";
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
            Dictionary<string, object> customData = ((RequestObject<CredentialsList>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<CredentialsList>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListCredentialsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<CredentialsList>)req).Callback != null)
                ((RequestObject<CredentialsList>)req).Callback(response, resp.Error, customData);
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
        /// <param name="credentialsParameter">An object that defines an individual set of source credentials.</param>
        /// <returns><see cref="ModelCredentials" />ModelCredentials</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool UpdateCredentials(Callback<ModelCredentials> callback, string environmentId, string credentialId, ModelCredentials credentialsParameter, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for UpdateCredentials");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for UpdateCredentials");
            if (string.IsNullOrEmpty(credentialId))
                throw new ArgumentNullException("credentialId is required for UpdateCredentials");
            if (credentialsParameter == null)
                throw new ArgumentNullException("credentialsParameter is required for UpdateCredentials");

            RequestObject<ModelCredentials> req = new RequestObject<ModelCredentials>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPUT,
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=UpdateCredentials";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (credentialsParameter != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(credentialsParameter));
            }

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
            Dictionary<string, object> customData = ((RequestObject<ModelCredentials>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ModelCredentials>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnUpdateCredentialsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ModelCredentials>)req).Callback != null)
                ((RequestObject<ModelCredentials>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Create Gateway.
        ///
        /// Create a gateway configuration to use with a remotely installed gateway.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <param name="gatewayName">The name of the gateway to created. (optional)</param>
        /// <returns><see cref="Gateway" />Gateway</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateGateway(Callback<Gateway> callback, string environmentId, GatewayName gatewayName = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateGateway");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for CreateGateway");

            RequestObject<Gateway> req = new RequestObject<Gateway>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=CreateGateway";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (gatewayName != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(gatewayName));
            }

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
            Dictionary<string, object> customData = ((RequestObject<Gateway>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Gateway>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateGatewayResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Gateway>)req).Callback != null)
                ((RequestObject<Gateway>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteGateway(Callback<GatewayDelete> callback, string environmentId, string gatewayId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteGateway");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for DeleteGateway");
            if (string.IsNullOrEmpty(gatewayId))
                throw new ArgumentNullException("gatewayId is required for DeleteGateway");

            RequestObject<GatewayDelete> req = new RequestObject<GatewayDelete>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=DeleteGateway";
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
            Dictionary<string, object> customData = ((RequestObject<GatewayDelete>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<GatewayDelete>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnDeleteGatewayResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<GatewayDelete>)req).Callback != null)
                ((RequestObject<GatewayDelete>)req).Callback(response, resp.Error, customData);
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
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetGatewayDetails(Callback<Gateway> callback, string environmentId, string gatewayId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetGatewayDetails");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for GetGatewayDetails");
            if (string.IsNullOrEmpty(gatewayId))
                throw new ArgumentNullException("gatewayId is required for GetGatewayDetails");

            RequestObject<Gateway> req = new RequestObject<Gateway>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=GetGatewayDetails";
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetGatewayDetailsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/gateways/{1}", environmentId, gatewayId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetGatewayDetailsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Gateway> response = new DetailedResponse<Gateway>();
            Dictionary<string, object> customData = ((RequestObject<Gateway>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Gateway>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetGatewayDetailsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Gateway>)req).Callback != null)
                ((RequestObject<Gateway>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// List Gateways.
        ///
        /// List the currently configured gateways.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <returns><see cref="GatewayList" />GatewayList</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetGatewayList(Callback<GatewayList> callback, string environmentId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetGatewayList");
            if (string.IsNullOrEmpty(environmentId))
                throw new ArgumentNullException("environmentId is required for GetGatewayList");

            RequestObject<GatewayList> req = new RequestObject<GatewayList>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=discovery;service_version=V1;operation_id=GetGatewayList";
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetGatewayListResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/environments/{0}/gateways", environmentId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetGatewayListResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<GatewayList> response = new DetailedResponse<GatewayList>();
            Dictionary<string, object> customData = ((RequestObject<GatewayList>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<GatewayList>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetGatewayListResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<GatewayList>)req).Callback != null)
                ((RequestObject<GatewayList>)req).Callback(response, resp.Error, customData);
        }
    }
}