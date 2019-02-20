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
using IBM.Watson.Assistant.V1.Model;
using Newtonsoft.Json;
using System;
using UnityEngine.Networking;

namespace IBM.Watson.Assistant.V1
{
    public class AssistantService : BaseService
    {
        private const string serviceId = "conversation";
        private const string defaultUrl = "https://gateway.watsonplatform.net/assistant/api";

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
        /// AssistantService constructor.
        /// </summary>
        /// <param name="versionDate">The service version date in `yyyy-mm-dd` format.</param>
        public AssistantService(string versionDate) : base(versionDate, serviceId)
        {
            VersionDate = versionDate;
        }

        /// <summary>
        /// AssistantService constructor.
        /// </summary>
        /// <param name="versionDate">The service version date in `yyyy-mm-dd` format.</param>
        /// <param name="credentials">The service credentials.</param>
        public AssistantService(string versionDate, Credentials credentials) : base(versionDate, credentials, serviceId)
        {
            if (string.IsNullOrEmpty(versionDate))
            {
                throw new ArgumentNullException("A versionDate (format `yyyy-mm-dd`) is required to create an instance of AssistantService");
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
                throw new WatsonException("Please provide a username and password or authorization token to use the Assistant service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }

        /// <summary>
        /// Get response to user input.
        ///
        /// Send user input to a workspace and receive a response.
        ///
        /// There is no rate limit for this operation.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="request">The message to be sent. This includes the user's input, along with optional intents,
        /// entities, and context from the last response. (optional)</param>
        /// <param name="nodesVisitedDetails">Whether to include additional diagnostic information about the dialog
        /// nodes that were visited during processing of the message. (optional, default to false)</param>
        /// <returns><see cref="MessageResponse" />MessageResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool Message(Callback<MessageResponse> callback, string workspaceId, MessageRequest request = null, bool? nodesVisitedDetails = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for Message");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for Message");

            RequestObject<MessageResponse> req = new RequestObject<MessageResponse>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=Message";
            req.Parameters["version"] = VersionDate;
            if (nodesVisitedDetails != null)
            {
                req.Parameters["nodes_visited_details"] = (bool)nodesVisitedDetails ? "true" : "false";
            }
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (request != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
            }

            req.OnResponse = OnMessageResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/message", workspaceId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnMessageResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<MessageResponse> response = new WatsonResponse<MessageResponse>();
            Dictionary<string, object> customData = ((RequestObject<MessageResponse>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<MessageResponse>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnMessageResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<MessageResponse>)req).Callback != null)
                ((RequestObject<MessageResponse>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Create workspace.
        ///
        /// Create a workspace based on component objects. You must provide workspace components defining the content of
        /// the new workspace.
        ///
        /// This operation is limited to 30 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="properties">The content of the new workspace.
        ///
        /// The maximum size for this data is 50MB. If you need to import a larger workspace, consider importing the
        /// workspace without intents and entities and then adding them separately. (optional)</param>
        /// <returns><see cref="Workspace" />Workspace</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateWorkspace(Callback<Workspace> callback, CreateWorkspace properties = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateWorkspace");

            RequestObject<Workspace> req = new RequestObject<Workspace>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=CreateWorkspace";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (properties != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(properties));
            }

            req.OnResponse = OnCreateWorkspaceResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/workspaces");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateWorkspaceResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Workspace> response = new WatsonResponse<Workspace>();
            Dictionary<string, object> customData = ((RequestObject<Workspace>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Workspace>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnCreateWorkspaceResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Workspace>)req).Callback != null)
                ((RequestObject<Workspace>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Delete workspace.
        ///
        /// Delete a workspace from the service instance.
        ///
        /// This operation is limited to 30 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteWorkspace(Callback<object> callback, string workspaceId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteWorkspace");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for DeleteWorkspace");

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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=DeleteWorkspace";
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteWorkspaceResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}", workspaceId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteWorkspaceResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("AssistantService.OnDeleteWorkspaceResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Get information about a workspace.
        ///
        /// Get information about a workspace, optionally including all workspace content.
        ///
        /// With **export**=`false`, this operation is limited to 6000 requests per 5 minutes. With **export**=`true`,
        /// the limit is 20 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="export">Whether to include all element content in the returned data. If **export**=`false`, the
        /// returned data includes only information about the element itself. If **export**=`true`, all content,
        /// including subelements, is included. (optional, default to false)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <param name="sort">Indicates how the returned workspace data will be sorted. This parameter is valid only if
        /// **export**=`true`. Specify `sort=stable` to sort all workspace objects by unique identifier, in ascending
        /// alphabetical order. (optional)</param>
        /// <returns><see cref="WorkspaceExport" />WorkspaceExport</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetWorkspace(Callback<WorkspaceExport> callback, string workspaceId, bool? export = null, bool? includeAudit = null, string sort = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetWorkspace");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for GetWorkspace");

            RequestObject<WorkspaceExport> req = new RequestObject<WorkspaceExport>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=GetWorkspace";
            req.Parameters["version"] = VersionDate;
            if (export != null)
            {
                req.Parameters["export"] = (bool)export ? "true" : "false";
            }
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }
            if (!string.IsNullOrEmpty(sort))
            {
                req.Parameters["sort"] = sort;
            }

            req.OnResponse = OnGetWorkspaceResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}", workspaceId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetWorkspaceResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<WorkspaceExport> response = new WatsonResponse<WorkspaceExport>();
            Dictionary<string, object> customData = ((RequestObject<WorkspaceExport>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<WorkspaceExport>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnGetWorkspaceResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<WorkspaceExport>)req).Callback != null)
                ((RequestObject<WorkspaceExport>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// List workspaces.
        ///
        /// List the workspaces associated with a Watson Assistant service instance.
        ///
        /// This operation is limited to 500 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to
        /// 100)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional,
        /// default to false)</param>
        /// <param name="sort">The attribute by which returned workspaces will be sorted. To reverse the sort order,
        /// prefix the value with a minus sign (`-`). (optional)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="WorkspaceCollection" />WorkspaceCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListWorkspaces(Callback<WorkspaceCollection> callback, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListWorkspaces");

            RequestObject<WorkspaceCollection> req = new RequestObject<WorkspaceCollection>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=ListWorkspaces";
            req.Parameters["version"] = VersionDate;
            if (pageLimit != null)
            {
                req.Parameters["page_limit"] = pageLimit;
            }
            if (includeCount != null)
            {
                req.Parameters["include_count"] = (bool)includeCount ? "true" : "false";
            }
            if (!string.IsNullOrEmpty(sort))
            {
                req.Parameters["sort"] = sort;
            }
            if (!string.IsNullOrEmpty(cursor))
            {
                req.Parameters["cursor"] = cursor;
            }
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnListWorkspacesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/workspaces");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListWorkspacesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<WorkspaceCollection> response = new WatsonResponse<WorkspaceCollection>();
            Dictionary<string, object> customData = ((RequestObject<WorkspaceCollection>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<WorkspaceCollection>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListWorkspacesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<WorkspaceCollection>)req).Callback != null)
                ((RequestObject<WorkspaceCollection>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Update workspace.
        ///
        /// Update an existing workspace with new or modified data. You must provide component objects defining the
        /// content of the updated workspace.
        ///
        /// This operation is limited to 30 request per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="properties">Valid data defining the new and updated workspace content.
        ///
        /// The maximum size for this data is 50MB. If you need to import a larger amount of workspace data, consider
        /// importing components such as intents and entities using separate operations. (optional)</param>
        /// <param name="append">Whether the new data is to be appended to the existing data in the workspace. If
        /// **append**=`false`, elements included in the new data completely replace the corresponding existing
        /// elements, including all subelements. For example, if the new data includes **entities** and
        /// **append**=`false`, all existing entities in the workspace are discarded and replaced with the new entities.
        ///
        ///
        /// If **append**=`true`, existing elements are preserved, and the new elements are added. If any elements in
        /// the new data collide with existing elements, the update request fails. (optional, default to false)</param>
        /// <returns><see cref="Workspace" />Workspace</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool UpdateWorkspace(Callback<Workspace> callback, string workspaceId, UpdateWorkspace properties = null, bool? append = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for UpdateWorkspace");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for UpdateWorkspace");

            RequestObject<Workspace> req = new RequestObject<Workspace>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=UpdateWorkspace";
            req.Parameters["version"] = VersionDate;
            if (append != null)
            {
                req.Parameters["append"] = (bool)append ? "true" : "false";
            }
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (properties != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(properties));
            }

            req.OnResponse = OnUpdateWorkspaceResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}", workspaceId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnUpdateWorkspaceResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Workspace> response = new WatsonResponse<Workspace>();
            Dictionary<string, object> customData = ((RequestObject<Workspace>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Workspace>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnUpdateWorkspaceResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Workspace>)req).Callback != null)
                ((RequestObject<Workspace>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Create intent.
        ///
        /// Create a new intent.
        ///
        /// This operation is limited to 2000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="body">The content of the new intent.</param>
        /// <returns><see cref="Intent" />Intent</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateIntent(Callback<Intent> callback, string workspaceId, CreateIntent body, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateIntent");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for CreateIntent");
            if (body == null)
                throw new ArgumentNullException("body is required for CreateIntent");

            RequestObject<Intent> req = new RequestObject<Intent>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=CreateIntent";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (body != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            }

            req.OnResponse = OnCreateIntentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents", workspaceId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateIntentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Intent> response = new WatsonResponse<Intent>();
            Dictionary<string, object> customData = ((RequestObject<Intent>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Intent>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnCreateIntentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Intent>)req).Callback != null)
                ((RequestObject<Intent>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Delete intent.
        ///
        /// Delete an intent from a workspace.
        ///
        /// This operation is limited to 2000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The intent name.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteIntent(Callback<object> callback, string workspaceId, string intent, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteIntent");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for DeleteIntent");
            if (string.IsNullOrEmpty(intent))
                throw new ArgumentNullException("intent is required for DeleteIntent");

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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=DeleteIntent";
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteIntentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents/{1}", workspaceId, intent));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteIntentResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("AssistantService.OnDeleteIntentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Get intent.
        ///
        /// Get information about an intent, optionally including all intent content.
        ///
        /// With **export**=`false`, this operation is limited to 6000 requests per 5 minutes. With **export**=`true`,
        /// the limit is 400 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The intent name.</param>
        /// <param name="export">Whether to include all element content in the returned data. If **export**=`false`, the
        /// returned data includes only information about the element itself. If **export**=`true`, all content,
        /// including subelements, is included. (optional, default to false)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="IntentExport" />IntentExport</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetIntent(Callback<IntentExport> callback, string workspaceId, string intent, bool? export = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetIntent");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for GetIntent");
            if (string.IsNullOrEmpty(intent))
                throw new ArgumentNullException("intent is required for GetIntent");

            RequestObject<IntentExport> req = new RequestObject<IntentExport>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=GetIntent";
            req.Parameters["version"] = VersionDate;
            if (export != null)
            {
                req.Parameters["export"] = (bool)export ? "true" : "false";
            }
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnGetIntentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents/{1}", workspaceId, intent));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetIntentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<IntentExport> response = new WatsonResponse<IntentExport>();
            Dictionary<string, object> customData = ((RequestObject<IntentExport>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<IntentExport>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnGetIntentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<IntentExport>)req).Callback != null)
                ((RequestObject<IntentExport>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// List intents.
        ///
        /// List the intents for a workspace.
        ///
        /// With **export**=`false`, this operation is limited to 2000 requests per 30 minutes. With **export**=`true`,
        /// the limit is 400 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="export">Whether to include all element content in the returned data. If **export**=`false`, the
        /// returned data includes only information about the element itself. If **export**=`true`, all content,
        /// including subelements, is included. (optional, default to false)</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to
        /// 100)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional,
        /// default to false)</param>
        /// <param name="sort">The attribute by which returned intents will be sorted. To reverse the sort order, prefix
        /// the value with a minus sign (`-`). (optional)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="IntentCollection" />IntentCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListIntents(Callback<IntentCollection> callback, string workspaceId, bool? export = null, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListIntents");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for ListIntents");

            RequestObject<IntentCollection> req = new RequestObject<IntentCollection>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=ListIntents";
            req.Parameters["version"] = VersionDate;
            if (export != null)
            {
                req.Parameters["export"] = (bool)export ? "true" : "false";
            }
            if (pageLimit != null)
            {
                req.Parameters["page_limit"] = pageLimit;
            }
            if (includeCount != null)
            {
                req.Parameters["include_count"] = (bool)includeCount ? "true" : "false";
            }
            if (!string.IsNullOrEmpty(sort))
            {
                req.Parameters["sort"] = sort;
            }
            if (!string.IsNullOrEmpty(cursor))
            {
                req.Parameters["cursor"] = cursor;
            }
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnListIntentsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents", workspaceId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListIntentsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<IntentCollection> response = new WatsonResponse<IntentCollection>();
            Dictionary<string, object> customData = ((RequestObject<IntentCollection>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<IntentCollection>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListIntentsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<IntentCollection>)req).Callback != null)
                ((RequestObject<IntentCollection>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Update intent.
        ///
        /// Update an existing intent with new or modified data. You must provide component objects defining the content
        /// of the updated intent.
        ///
        /// This operation is limited to 2000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The intent name.</param>
        /// <param name="body">The updated content of the intent.
        ///
        /// Any elements included in the new data will completely replace the equivalent existing elements, including
        /// all subelements. (Previously existing subelements are not retained unless they are also included in the new
        /// data.) For example, if you update the user input examples for an intent, the previously existing examples
        /// are discarded and replaced with the new examples specified in the update.</param>
        /// <returns><see cref="Intent" />Intent</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool UpdateIntent(Callback<Intent> callback, string workspaceId, string intent, UpdateIntent body, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for UpdateIntent");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for UpdateIntent");
            if (string.IsNullOrEmpty(intent))
                throw new ArgumentNullException("intent is required for UpdateIntent");
            if (body == null)
                throw new ArgumentNullException("body is required for UpdateIntent");

            RequestObject<Intent> req = new RequestObject<Intent>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=UpdateIntent";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (body != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            }

            req.OnResponse = OnUpdateIntentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents/{1}", workspaceId, intent));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnUpdateIntentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Intent> response = new WatsonResponse<Intent>();
            Dictionary<string, object> customData = ((RequestObject<Intent>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Intent>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnUpdateIntentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Intent>)req).Callback != null)
                ((RequestObject<Intent>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Create user input example.
        ///
        /// Add a new user input example to an intent.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The intent name.</param>
        /// <param name="body">The content of the new user input example.</param>
        /// <returns><see cref="Example" />Example</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateExample(Callback<Example> callback, string workspaceId, string intent, CreateExample body, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateExample");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for CreateExample");
            if (string.IsNullOrEmpty(intent))
                throw new ArgumentNullException("intent is required for CreateExample");
            if (body == null)
                throw new ArgumentNullException("body is required for CreateExample");

            RequestObject<Example> req = new RequestObject<Example>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=CreateExample";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (body != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            }

            req.OnResponse = OnCreateExampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents/{1}/examples", workspaceId, intent));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateExampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Example> response = new WatsonResponse<Example>();
            Dictionary<string, object> customData = ((RequestObject<Example>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Example>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnCreateExampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Example>)req).Callback != null)
                ((RequestObject<Example>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Delete user input example.
        ///
        /// Delete a user input example from an intent.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The intent name.</param>
        /// <param name="text">The text of the user input example.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteExample(Callback<object> callback, string workspaceId, string intent, string text, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteExample");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for DeleteExample");
            if (string.IsNullOrEmpty(intent))
                throw new ArgumentNullException("intent is required for DeleteExample");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text is required for DeleteExample");

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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=DeleteExample";
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteExampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents/{1}/examples/{2}", workspaceId, intent, text));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteExampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("AssistantService.OnDeleteExampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Get user input example.
        ///
        /// Get information about a user input example.
        ///
        /// This operation is limited to 6000 requests per 5 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The intent name.</param>
        /// <param name="text">The text of the user input example.</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="Example" />Example</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetExample(Callback<Example> callback, string workspaceId, string intent, string text, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetExample");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for GetExample");
            if (string.IsNullOrEmpty(intent))
                throw new ArgumentNullException("intent is required for GetExample");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text is required for GetExample");

            RequestObject<Example> req = new RequestObject<Example>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=GetExample";
            req.Parameters["version"] = VersionDate;
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnGetExampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents/{1}/examples/{2}", workspaceId, intent, text));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetExampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Example> response = new WatsonResponse<Example>();
            Dictionary<string, object> customData = ((RequestObject<Example>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Example>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnGetExampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Example>)req).Callback != null)
                ((RequestObject<Example>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// List user input examples.
        ///
        /// List the user input examples for an intent, optionally including contextual entity mentions.
        ///
        /// This operation is limited to 2500 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The intent name.</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to
        /// 100)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional,
        /// default to false)</param>
        /// <param name="sort">The attribute by which returned examples will be sorted. To reverse the sort order,
        /// prefix the value with a minus sign (`-`). (optional)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="ExampleCollection" />ExampleCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListExamples(Callback<ExampleCollection> callback, string workspaceId, string intent, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListExamples");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for ListExamples");
            if (string.IsNullOrEmpty(intent))
                throw new ArgumentNullException("intent is required for ListExamples");

            RequestObject<ExampleCollection> req = new RequestObject<ExampleCollection>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=ListExamples";
            req.Parameters["version"] = VersionDate;
            if (pageLimit != null)
            {
                req.Parameters["page_limit"] = pageLimit;
            }
            if (includeCount != null)
            {
                req.Parameters["include_count"] = (bool)includeCount ? "true" : "false";
            }
            if (!string.IsNullOrEmpty(sort))
            {
                req.Parameters["sort"] = sort;
            }
            if (!string.IsNullOrEmpty(cursor))
            {
                req.Parameters["cursor"] = cursor;
            }
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnListExamplesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents/{1}/examples", workspaceId, intent));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListExamplesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<ExampleCollection> response = new WatsonResponse<ExampleCollection>();
            Dictionary<string, object> customData = ((RequestObject<ExampleCollection>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ExampleCollection>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListExamplesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ExampleCollection>)req).Callback != null)
                ((RequestObject<ExampleCollection>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Update user input example.
        ///
        /// Update the text of a user input example.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The intent name.</param>
        /// <param name="text">The text of the user input example.</param>
        /// <param name="body">The new text of the user input example.</param>
        /// <returns><see cref="Example" />Example</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool UpdateExample(Callback<Example> callback, string workspaceId, string intent, string text, UpdateExample body, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for UpdateExample");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for UpdateExample");
            if (string.IsNullOrEmpty(intent))
                throw new ArgumentNullException("intent is required for UpdateExample");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text is required for UpdateExample");
            if (body == null)
                throw new ArgumentNullException("body is required for UpdateExample");

            RequestObject<Example> req = new RequestObject<Example>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=UpdateExample";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (body != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            }

            req.OnResponse = OnUpdateExampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents/{1}/examples/{2}", workspaceId, intent, text));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnUpdateExampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Example> response = new WatsonResponse<Example>();
            Dictionary<string, object> customData = ((RequestObject<Example>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Example>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnUpdateExampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Example>)req).Callback != null)
                ((RequestObject<Example>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Create counterexample.
        ///
        /// Add a new counterexample to a workspace. Counterexamples are examples that have been marked as irrelevant
        /// input.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="body">The content of the new counterexample.</param>
        /// <returns><see cref="Counterexample" />Counterexample</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateCounterexample(Callback<Counterexample> callback, string workspaceId, CreateCounterexample body, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateCounterexample");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for CreateCounterexample");
            if (body == null)
                throw new ArgumentNullException("body is required for CreateCounterexample");

            RequestObject<Counterexample> req = new RequestObject<Counterexample>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=CreateCounterexample";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (body != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            }

            req.OnResponse = OnCreateCounterexampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/counterexamples", workspaceId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateCounterexampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Counterexample> response = new WatsonResponse<Counterexample>();
            Dictionary<string, object> customData = ((RequestObject<Counterexample>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Counterexample>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnCreateCounterexampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Counterexample>)req).Callback != null)
                ((RequestObject<Counterexample>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Delete counterexample.
        ///
        /// Delete a counterexample from a workspace. Counterexamples are examples that have been marked as irrelevant
        /// input.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="text">The text of a user input counterexample (for example, `What are you wearing?`).</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteCounterexample(Callback<object> callback, string workspaceId, string text, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteCounterexample");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for DeleteCounterexample");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text is required for DeleteCounterexample");

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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=DeleteCounterexample";
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteCounterexampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/counterexamples/{1}", workspaceId, text));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteCounterexampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("AssistantService.OnDeleteCounterexampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Get counterexample.
        ///
        /// Get information about a counterexample. Counterexamples are examples that have been marked as irrelevant
        /// input.
        ///
        /// This operation is limited to 6000 requests per 5 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="text">The text of a user input counterexample (for example, `What are you wearing?`).</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="Counterexample" />Counterexample</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetCounterexample(Callback<Counterexample> callback, string workspaceId, string text, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetCounterexample");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for GetCounterexample");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text is required for GetCounterexample");

            RequestObject<Counterexample> req = new RequestObject<Counterexample>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=GetCounterexample";
            req.Parameters["version"] = VersionDate;
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnGetCounterexampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/counterexamples/{1}", workspaceId, text));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetCounterexampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Counterexample> response = new WatsonResponse<Counterexample>();
            Dictionary<string, object> customData = ((RequestObject<Counterexample>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Counterexample>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnGetCounterexampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Counterexample>)req).Callback != null)
                ((RequestObject<Counterexample>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// List counterexamples.
        ///
        /// List the counterexamples for a workspace. Counterexamples are examples that have been marked as irrelevant
        /// input.
        ///
        /// This operation is limited to 2500 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to
        /// 100)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional,
        /// default to false)</param>
        /// <param name="sort">The attribute by which returned counterexamples will be sorted. To reverse the sort
        /// order, prefix the value with a minus sign (`-`). (optional)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="CounterexampleCollection" />CounterexampleCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListCounterexamples(Callback<CounterexampleCollection> callback, string workspaceId, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListCounterexamples");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for ListCounterexamples");

            RequestObject<CounterexampleCollection> req = new RequestObject<CounterexampleCollection>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=ListCounterexamples";
            req.Parameters["version"] = VersionDate;
            if (pageLimit != null)
            {
                req.Parameters["page_limit"] = pageLimit;
            }
            if (includeCount != null)
            {
                req.Parameters["include_count"] = (bool)includeCount ? "true" : "false";
            }
            if (!string.IsNullOrEmpty(sort))
            {
                req.Parameters["sort"] = sort;
            }
            if (!string.IsNullOrEmpty(cursor))
            {
                req.Parameters["cursor"] = cursor;
            }
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnListCounterexamplesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/counterexamples", workspaceId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListCounterexamplesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<CounterexampleCollection> response = new WatsonResponse<CounterexampleCollection>();
            Dictionary<string, object> customData = ((RequestObject<CounterexampleCollection>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<CounterexampleCollection>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListCounterexamplesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<CounterexampleCollection>)req).Callback != null)
                ((RequestObject<CounterexampleCollection>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Update counterexample.
        ///
        /// Update the text of a counterexample. Counterexamples are examples that have been marked as irrelevant input.
        ///
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="text">The text of a user input counterexample (for example, `What are you wearing?`).</param>
        /// <param name="body">The text of the counterexample.</param>
        /// <returns><see cref="Counterexample" />Counterexample</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool UpdateCounterexample(Callback<Counterexample> callback, string workspaceId, string text, UpdateCounterexample body, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for UpdateCounterexample");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for UpdateCounterexample");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text is required for UpdateCounterexample");
            if (body == null)
                throw new ArgumentNullException("body is required for UpdateCounterexample");

            RequestObject<Counterexample> req = new RequestObject<Counterexample>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=UpdateCounterexample";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (body != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            }

            req.OnResponse = OnUpdateCounterexampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/counterexamples/{1}", workspaceId, text));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnUpdateCounterexampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Counterexample> response = new WatsonResponse<Counterexample>();
            Dictionary<string, object> customData = ((RequestObject<Counterexample>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Counterexample>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnUpdateCounterexampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Counterexample>)req).Callback != null)
                ((RequestObject<Counterexample>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Create entity.
        ///
        /// Create a new entity, or enable a system entity.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="properties">The content of the new entity.</param>
        /// <returns><see cref="Entity" />Entity</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateEntity(Callback<Entity> callback, string workspaceId, CreateEntity properties, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateEntity");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for CreateEntity");
            if (properties == null)
                throw new ArgumentNullException("properties is required for CreateEntity");

            RequestObject<Entity> req = new RequestObject<Entity>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=CreateEntity";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (properties != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(properties));
            }

            req.OnResponse = OnCreateEntityResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities", workspaceId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateEntityResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Entity> response = new WatsonResponse<Entity>();
            Dictionary<string, object> customData = ((RequestObject<Entity>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Entity>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnCreateEntityResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Entity>)req).Callback != null)
                ((RequestObject<Entity>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Delete entity.
        ///
        /// Delete an entity from a workspace, or disable a system entity.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteEntity(Callback<object> callback, string workspaceId, string entity, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteEntity");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for DeleteEntity");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("entity is required for DeleteEntity");

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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=DeleteEntity";
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteEntityResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}", workspaceId, entity));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteEntityResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("AssistantService.OnDeleteEntityResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Get entity.
        ///
        /// Get information about an entity, optionally including all entity content.
        ///
        /// With **export**=`false`, this operation is limited to 6000 requests per 5 minutes. With **export**=`true`,
        /// the limit is 200 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="export">Whether to include all element content in the returned data. If **export**=`false`, the
        /// returned data includes only information about the element itself. If **export**=`true`, all content,
        /// including subelements, is included. (optional, default to false)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="EntityExport" />EntityExport</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetEntity(Callback<EntityExport> callback, string workspaceId, string entity, bool? export = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetEntity");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for GetEntity");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("entity is required for GetEntity");

            RequestObject<EntityExport> req = new RequestObject<EntityExport>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=GetEntity";
            req.Parameters["version"] = VersionDate;
            if (export != null)
            {
                req.Parameters["export"] = (bool)export ? "true" : "false";
            }
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnGetEntityResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}", workspaceId, entity));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetEntityResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<EntityExport> response = new WatsonResponse<EntityExport>();
            Dictionary<string, object> customData = ((RequestObject<EntityExport>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<EntityExport>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnGetEntityResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<EntityExport>)req).Callback != null)
                ((RequestObject<EntityExport>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// List entities.
        ///
        /// List the entities for a workspace.
        ///
        /// With **export**=`false`, this operation is limited to 1000 requests per 30 minutes. With **export**=`true`,
        /// the limit is 200 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="export">Whether to include all element content in the returned data. If **export**=`false`, the
        /// returned data includes only information about the element itself. If **export**=`true`, all content,
        /// including subelements, is included. (optional, default to false)</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to
        /// 100)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional,
        /// default to false)</param>
        /// <param name="sort">The attribute by which returned entities will be sorted. To reverse the sort order,
        /// prefix the value with a minus sign (`-`). (optional)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="EntityCollection" />EntityCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListEntities(Callback<EntityCollection> callback, string workspaceId, bool? export = null, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListEntities");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for ListEntities");

            RequestObject<EntityCollection> req = new RequestObject<EntityCollection>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=ListEntities";
            req.Parameters["version"] = VersionDate;
            if (export != null)
            {
                req.Parameters["export"] = (bool)export ? "true" : "false";
            }
            if (pageLimit != null)
            {
                req.Parameters["page_limit"] = pageLimit;
            }
            if (includeCount != null)
            {
                req.Parameters["include_count"] = (bool)includeCount ? "true" : "false";
            }
            if (!string.IsNullOrEmpty(sort))
            {
                req.Parameters["sort"] = sort;
            }
            if (!string.IsNullOrEmpty(cursor))
            {
                req.Parameters["cursor"] = cursor;
            }
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnListEntitiesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities", workspaceId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListEntitiesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<EntityCollection> response = new WatsonResponse<EntityCollection>();
            Dictionary<string, object> customData = ((RequestObject<EntityCollection>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<EntityCollection>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListEntitiesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<EntityCollection>)req).Callback != null)
                ((RequestObject<EntityCollection>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Update entity.
        ///
        /// Update an existing entity with new or modified data. You must provide component objects defining the content
        /// of the updated entity.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="properties">The updated content of the entity. Any elements included in the new data will
        /// completely replace the equivalent existing elements, including all subelements. (Previously existing
        /// subelements are not retained unless they are also included in the new data.) For example, if you update the
        /// values for an entity, the previously existing values are discarded and replaced with the new values
        /// specified in the update.</param>
        /// <returns><see cref="Entity" />Entity</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool UpdateEntity(Callback<Entity> callback, string workspaceId, string entity, UpdateEntity properties, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for UpdateEntity");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for UpdateEntity");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("entity is required for UpdateEntity");
            if (properties == null)
                throw new ArgumentNullException("properties is required for UpdateEntity");

            RequestObject<Entity> req = new RequestObject<Entity>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=UpdateEntity";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (properties != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(properties));
            }

            req.OnResponse = OnUpdateEntityResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}", workspaceId, entity));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnUpdateEntityResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Entity> response = new WatsonResponse<Entity>();
            Dictionary<string, object> customData = ((RequestObject<Entity>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Entity>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnUpdateEntityResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Entity>)req).Callback != null)
                ((RequestObject<Entity>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// List entity mentions.
        ///
        /// List mentions for a contextual entity. An entity mention is an occurrence of a contextual entity in the
        /// context of an intent user input example.
        ///
        /// This operation is limited to 200 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="export">Whether to include all element content in the returned data. If **export**=`false`, the
        /// returned data includes only information about the element itself. If **export**=`true`, all content,
        /// including subelements, is included. (optional, default to false)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="EntityMentionCollection" />EntityMentionCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListMentions(Callback<EntityMentionCollection> callback, string workspaceId, string entity, bool? export = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListMentions");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for ListMentions");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("entity is required for ListMentions");

            RequestObject<EntityMentionCollection> req = new RequestObject<EntityMentionCollection>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=ListMentions";
            req.Parameters["version"] = VersionDate;
            if (export != null)
            {
                req.Parameters["export"] = (bool)export ? "true" : "false";
            }
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnListMentionsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/mentions", workspaceId, entity));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListMentionsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<EntityMentionCollection> response = new WatsonResponse<EntityMentionCollection>();
            Dictionary<string, object> customData = ((RequestObject<EntityMentionCollection>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<EntityMentionCollection>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListMentionsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<EntityMentionCollection>)req).Callback != null)
                ((RequestObject<EntityMentionCollection>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Add entity value.
        ///
        /// Create a new value for an entity.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="properties">The new entity value.</param>
        /// <returns><see cref="Value" />Value</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateValue(Callback<Value> callback, string workspaceId, string entity, CreateValue properties, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateValue");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for CreateValue");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("entity is required for CreateValue");
            if (properties == null)
                throw new ArgumentNullException("properties is required for CreateValue");

            RequestObject<Value> req = new RequestObject<Value>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=CreateValue";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (properties != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(properties));
            }

            req.OnResponse = OnCreateValueResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values", workspaceId, entity));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateValueResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Value> response = new WatsonResponse<Value>();
            Dictionary<string, object> customData = ((RequestObject<Value>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Value>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnCreateValueResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Value>)req).Callback != null)
                ((RequestObject<Value>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Delete entity value.
        ///
        /// Delete a value from an entity.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteValue(Callback<object> callback, string workspaceId, string entity, string value, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteValue");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for DeleteValue");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("entity is required for DeleteValue");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value is required for DeleteValue");

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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=DeleteValue";
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteValueResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}", workspaceId, entity, value));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteValueResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("AssistantService.OnDeleteValueResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Get entity value.
        ///
        /// Get information about an entity value.
        ///
        /// This operation is limited to 6000 requests per 5 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="export">Whether to include all element content in the returned data. If **export**=`false`, the
        /// returned data includes only information about the element itself. If **export**=`true`, all content,
        /// including subelements, is included. (optional, default to false)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="ValueExport" />ValueExport</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetValue(Callback<ValueExport> callback, string workspaceId, string entity, string value, bool? export = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetValue");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for GetValue");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("entity is required for GetValue");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value is required for GetValue");

            RequestObject<ValueExport> req = new RequestObject<ValueExport>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=GetValue";
            req.Parameters["version"] = VersionDate;
            if (export != null)
            {
                req.Parameters["export"] = (bool)export ? "true" : "false";
            }
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnGetValueResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}", workspaceId, entity, value));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetValueResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<ValueExport> response = new WatsonResponse<ValueExport>();
            Dictionary<string, object> customData = ((RequestObject<ValueExport>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ValueExport>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnGetValueResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ValueExport>)req).Callback != null)
                ((RequestObject<ValueExport>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// List entity values.
        ///
        /// List the values for an entity.
        ///
        /// This operation is limited to 2500 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="export">Whether to include all element content in the returned data. If **export**=`false`, the
        /// returned data includes only information about the element itself. If **export**=`true`, all content,
        /// including subelements, is included. (optional, default to false)</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to
        /// 100)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional,
        /// default to false)</param>
        /// <param name="sort">The attribute by which returned entity values will be sorted. To reverse the sort order,
        /// prefix the value with a minus sign (`-`). (optional)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="ValueCollection" />ValueCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListValues(Callback<ValueCollection> callback, string workspaceId, string entity, bool? export = null, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListValues");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for ListValues");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("entity is required for ListValues");

            RequestObject<ValueCollection> req = new RequestObject<ValueCollection>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=ListValues";
            req.Parameters["version"] = VersionDate;
            if (export != null)
            {
                req.Parameters["export"] = (bool)export ? "true" : "false";
            }
            if (pageLimit != null)
            {
                req.Parameters["page_limit"] = pageLimit;
            }
            if (includeCount != null)
            {
                req.Parameters["include_count"] = (bool)includeCount ? "true" : "false";
            }
            if (!string.IsNullOrEmpty(sort))
            {
                req.Parameters["sort"] = sort;
            }
            if (!string.IsNullOrEmpty(cursor))
            {
                req.Parameters["cursor"] = cursor;
            }
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnListValuesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values", workspaceId, entity));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListValuesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<ValueCollection> response = new WatsonResponse<ValueCollection>();
            Dictionary<string, object> customData = ((RequestObject<ValueCollection>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ValueCollection>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListValuesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ValueCollection>)req).Callback != null)
                ((RequestObject<ValueCollection>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Update entity value.
        ///
        /// Update an existing entity value with new or modified data. You must provide component objects defining the
        /// content of the updated entity value.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="properties">The updated content of the entity value.
        ///
        /// Any elements included in the new data will completely replace the equivalent existing elements, including
        /// all subelements. (Previously existing subelements are not retained unless they are also included in the new
        /// data.) For example, if you update the synonyms for an entity value, the previously existing synonyms are
        /// discarded and replaced with the new synonyms specified in the update.</param>
        /// <returns><see cref="Value" />Value</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool UpdateValue(Callback<Value> callback, string workspaceId, string entity, string value, UpdateValue properties, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for UpdateValue");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for UpdateValue");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("entity is required for UpdateValue");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value is required for UpdateValue");
            if (properties == null)
                throw new ArgumentNullException("properties is required for UpdateValue");

            RequestObject<Value> req = new RequestObject<Value>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=UpdateValue";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (properties != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(properties));
            }

            req.OnResponse = OnUpdateValueResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}", workspaceId, entity, value));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnUpdateValueResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Value> response = new WatsonResponse<Value>();
            Dictionary<string, object> customData = ((RequestObject<Value>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Value>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnUpdateValueResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Value>)req).Callback != null)
                ((RequestObject<Value>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Add entity value synonym.
        ///
        /// Add a new synonym to an entity value.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="body">The new synonym.</param>
        /// <returns><see cref="Synonym" />Synonym</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateSynonym(Callback<Synonym> callback, string workspaceId, string entity, string value, CreateSynonym body, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateSynonym");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for CreateSynonym");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("entity is required for CreateSynonym");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value is required for CreateSynonym");
            if (body == null)
                throw new ArgumentNullException("body is required for CreateSynonym");

            RequestObject<Synonym> req = new RequestObject<Synonym>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=CreateSynonym";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (body != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            }

            req.OnResponse = OnCreateSynonymResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}/synonyms", workspaceId, entity, value));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateSynonymResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Synonym> response = new WatsonResponse<Synonym>();
            Dictionary<string, object> customData = ((RequestObject<Synonym>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Synonym>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnCreateSynonymResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Synonym>)req).Callback != null)
                ((RequestObject<Synonym>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Delete entity value synonym.
        ///
        /// Delete a synonym from an entity value.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="synonym">The text of the synonym.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteSynonym(Callback<object> callback, string workspaceId, string entity, string value, string synonym, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteSynonym");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for DeleteSynonym");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("entity is required for DeleteSynonym");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value is required for DeleteSynonym");
            if (string.IsNullOrEmpty(synonym))
                throw new ArgumentNullException("synonym is required for DeleteSynonym");

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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=DeleteSynonym";
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteSynonymResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}/synonyms/{3}", workspaceId, entity, value, synonym));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteSynonymResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("AssistantService.OnDeleteSynonymResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Get entity value synonym.
        ///
        /// Get information about a synonym of an entity value.
        ///
        /// This operation is limited to 6000 requests per 5 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="synonym">The text of the synonym.</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="Synonym" />Synonym</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetSynonym(Callback<Synonym> callback, string workspaceId, string entity, string value, string synonym, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetSynonym");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for GetSynonym");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("entity is required for GetSynonym");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value is required for GetSynonym");
            if (string.IsNullOrEmpty(synonym))
                throw new ArgumentNullException("synonym is required for GetSynonym");

            RequestObject<Synonym> req = new RequestObject<Synonym>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=GetSynonym";
            req.Parameters["version"] = VersionDate;
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnGetSynonymResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}/synonyms/{3}", workspaceId, entity, value, synonym));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetSynonymResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Synonym> response = new WatsonResponse<Synonym>();
            Dictionary<string, object> customData = ((RequestObject<Synonym>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Synonym>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnGetSynonymResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Synonym>)req).Callback != null)
                ((RequestObject<Synonym>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// List entity value synonyms.
        ///
        /// List the synonyms for an entity value.
        ///
        /// This operation is limited to 2500 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to
        /// 100)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional,
        /// default to false)</param>
        /// <param name="sort">The attribute by which returned entity value synonyms will be sorted. To reverse the sort
        /// order, prefix the value with a minus sign (`-`). (optional)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="SynonymCollection" />SynonymCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListSynonyms(Callback<SynonymCollection> callback, string workspaceId, string entity, string value, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListSynonyms");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for ListSynonyms");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("entity is required for ListSynonyms");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value is required for ListSynonyms");

            RequestObject<SynonymCollection> req = new RequestObject<SynonymCollection>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=ListSynonyms";
            req.Parameters["version"] = VersionDate;
            if (pageLimit != null)
            {
                req.Parameters["page_limit"] = pageLimit;
            }
            if (includeCount != null)
            {
                req.Parameters["include_count"] = (bool)includeCount ? "true" : "false";
            }
            if (!string.IsNullOrEmpty(sort))
            {
                req.Parameters["sort"] = sort;
            }
            if (!string.IsNullOrEmpty(cursor))
            {
                req.Parameters["cursor"] = cursor;
            }
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnListSynonymsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}/synonyms", workspaceId, entity, value));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListSynonymsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<SynonymCollection> response = new WatsonResponse<SynonymCollection>();
            Dictionary<string, object> customData = ((RequestObject<SynonymCollection>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<SynonymCollection>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListSynonymsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<SynonymCollection>)req).Callback != null)
                ((RequestObject<SynonymCollection>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Update entity value synonym.
        ///
        /// Update an existing entity value synonym with new text.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="synonym">The text of the synonym.</param>
        /// <param name="body">The updated entity value synonym.</param>
        /// <returns><see cref="Synonym" />Synonym</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool UpdateSynonym(Callback<Synonym> callback, string workspaceId, string entity, string value, string synonym, UpdateSynonym body, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for UpdateSynonym");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for UpdateSynonym");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("entity is required for UpdateSynonym");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value is required for UpdateSynonym");
            if (string.IsNullOrEmpty(synonym))
                throw new ArgumentNullException("synonym is required for UpdateSynonym");
            if (body == null)
                throw new ArgumentNullException("body is required for UpdateSynonym");

            RequestObject<Synonym> req = new RequestObject<Synonym>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=UpdateSynonym";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (body != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            }

            req.OnResponse = OnUpdateSynonymResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}/synonyms/{3}", workspaceId, entity, value, synonym));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnUpdateSynonymResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<Synonym> response = new WatsonResponse<Synonym>();
            Dictionary<string, object> customData = ((RequestObject<Synonym>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Synonym>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnUpdateSynonymResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Synonym>)req).Callback != null)
                ((RequestObject<Synonym>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Create dialog node.
        ///
        /// Create a new dialog node.
        ///
        /// This operation is limited to 500 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="properties">A CreateDialogNode object defining the content of the new dialog node.</param>
        /// <returns><see cref="DialogNode" />DialogNode</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateDialogNode(Callback<DialogNode> callback, string workspaceId, CreateDialogNode properties, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for CreateDialogNode");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for CreateDialogNode");
            if (properties == null)
                throw new ArgumentNullException("properties is required for CreateDialogNode");

            RequestObject<DialogNode> req = new RequestObject<DialogNode>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=CreateDialogNode";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (properties != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(properties));
            }

            req.OnResponse = OnCreateDialogNodeResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/dialog_nodes", workspaceId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateDialogNodeResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<DialogNode> response = new WatsonResponse<DialogNode>();
            Dictionary<string, object> customData = ((RequestObject<DialogNode>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DialogNode>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnCreateDialogNodeResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DialogNode>)req).Callback != null)
                ((RequestObject<DialogNode>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Delete dialog node.
        ///
        /// Delete a dialog node from a workspace.
        ///
        /// This operation is limited to 500 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="dialogNode">The dialog node ID (for example, `get_order`).</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteDialogNode(Callback<object> callback, string workspaceId, string dialogNode, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for DeleteDialogNode");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for DeleteDialogNode");
            if (string.IsNullOrEmpty(dialogNode))
                throw new ArgumentNullException("dialogNode is required for DeleteDialogNode");

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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=DeleteDialogNode";
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteDialogNodeResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/dialog_nodes/{1}", workspaceId, dialogNode));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteDialogNodeResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("AssistantService.OnDeleteDialogNodeResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Get dialog node.
        ///
        /// Get information about a dialog node.
        ///
        /// This operation is limited to 6000 requests per 5 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="dialogNode">The dialog node ID (for example, `get_order`).</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="DialogNode" />DialogNode</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool GetDialogNode(Callback<DialogNode> callback, string workspaceId, string dialogNode, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for GetDialogNode");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for GetDialogNode");
            if (string.IsNullOrEmpty(dialogNode))
                throw new ArgumentNullException("dialogNode is required for GetDialogNode");

            RequestObject<DialogNode> req = new RequestObject<DialogNode>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=GetDialogNode";
            req.Parameters["version"] = VersionDate;
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnGetDialogNodeResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/dialog_nodes/{1}", workspaceId, dialogNode));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetDialogNodeResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<DialogNode> response = new WatsonResponse<DialogNode>();
            Dictionary<string, object> customData = ((RequestObject<DialogNode>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DialogNode>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnGetDialogNodeResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DialogNode>)req).Callback != null)
                ((RequestObject<DialogNode>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// List dialog nodes.
        ///
        /// List the dialog nodes for a workspace.
        ///
        /// This operation is limited to 2500 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to
        /// 100)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional,
        /// default to false)</param>
        /// <param name="sort">The attribute by which returned dialog nodes will be sorted. To reverse the sort order,
        /// prefix the value with a minus sign (`-`). (optional)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="DialogNodeCollection" />DialogNodeCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListDialogNodes(Callback<DialogNodeCollection> callback, string workspaceId, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListDialogNodes");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for ListDialogNodes");

            RequestObject<DialogNodeCollection> req = new RequestObject<DialogNodeCollection>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=ListDialogNodes";
            req.Parameters["version"] = VersionDate;
            if (pageLimit != null)
            {
                req.Parameters["page_limit"] = pageLimit;
            }
            if (includeCount != null)
            {
                req.Parameters["include_count"] = (bool)includeCount ? "true" : "false";
            }
            if (!string.IsNullOrEmpty(sort))
            {
                req.Parameters["sort"] = sort;
            }
            if (!string.IsNullOrEmpty(cursor))
            {
                req.Parameters["cursor"] = cursor;
            }
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnListDialogNodesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/dialog_nodes", workspaceId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListDialogNodesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<DialogNodeCollection> response = new WatsonResponse<DialogNodeCollection>();
            Dictionary<string, object> customData = ((RequestObject<DialogNodeCollection>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DialogNodeCollection>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListDialogNodesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DialogNodeCollection>)req).Callback != null)
                ((RequestObject<DialogNodeCollection>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Update dialog node.
        ///
        /// Update an existing dialog node with new or modified data.
        ///
        /// This operation is limited to 500 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="dialogNode">The dialog node ID (for example, `get_order`).</param>
        /// <param name="properties">The updated content of the dialog node.
        ///
        /// Any elements included in the new data will completely replace the equivalent existing elements, including
        /// all subelements. (Previously existing subelements are not retained unless they are also included in the new
        /// data.) For example, if you update the actions for a dialog node, the previously existing actions are
        /// discarded and replaced with the new actions specified in the update.</param>
        /// <returns><see cref="DialogNode" />DialogNode</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool UpdateDialogNode(Callback<DialogNode> callback, string workspaceId, string dialogNode, UpdateDialogNode properties, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for UpdateDialogNode");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for UpdateDialogNode");
            if (string.IsNullOrEmpty(dialogNode))
                throw new ArgumentNullException("dialogNode is required for UpdateDialogNode");
            if (properties == null)
                throw new ArgumentNullException("properties is required for UpdateDialogNode");

            RequestObject<DialogNode> req = new RequestObject<DialogNode>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=UpdateDialogNode";
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            if (properties != null)
            {
                req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(properties));
            }

            req.OnResponse = OnUpdateDialogNodeResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/dialog_nodes/{1}", workspaceId, dialogNode));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnUpdateDialogNodeResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<DialogNode> response = new WatsonResponse<DialogNode>();
            Dictionary<string, object> customData = ((RequestObject<DialogNode>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DialogNode>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnUpdateDialogNodeResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DialogNode>)req).Callback != null)
                ((RequestObject<DialogNode>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// List log events in all workspaces.
        ///
        /// List the events from the logs of all workspaces in the service instance.
        ///
        /// If **cursor** is not specified, this operation is limited to 40 requests per 30 minutes. If **cursor** is
        /// specified, the limit is 120 requests per minute. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="filter">A cacheable parameter that limits the results to those matching the specified filter.
        /// You must specify a filter query that includes a value for `language`, as well as a value for `workspace_id`
        /// or `request.context.metadata.deployment`. For more information, see the
        /// [documentation](https://cloud.ibm.com/docs/services/assistant/filter-reference.html#filter-query-syntax).</param>
        /// <param name="sort">How to sort the returned log events. You can sort by **request_timestamp**. To reverse
        /// the sort order, prefix the parameter value with a minus sign (`-`). (optional, default to
        /// request_timestamp)</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to
        /// 100)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <returns><see cref="LogCollection" />LogCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListAllLogs(Callback<LogCollection> callback, string filter, string sort = null, long? pageLimit = null, string cursor = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListAllLogs");
            if (string.IsNullOrEmpty(filter))
                throw new ArgumentNullException("filter is required for ListAllLogs");

            RequestObject<LogCollection> req = new RequestObject<LogCollection>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=ListAllLogs";
            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(filter))
            {
                req.Parameters["filter"] = filter;
            }
            if (!string.IsNullOrEmpty(sort))
            {
                req.Parameters["sort"] = sort;
            }
            if (pageLimit != null)
            {
                req.Parameters["page_limit"] = pageLimit;
            }
            if (!string.IsNullOrEmpty(cursor))
            {
                req.Parameters["cursor"] = cursor;
            }

            req.OnResponse = OnListAllLogsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/logs");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListAllLogsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<LogCollection> response = new WatsonResponse<LogCollection>();
            Dictionary<string, object> customData = ((RequestObject<LogCollection>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<LogCollection>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListAllLogsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<LogCollection>)req).Callback != null)
                ((RequestObject<LogCollection>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// List log events in a workspace.
        ///
        /// List the events from the log of a specific workspace.
        ///
        /// If **cursor** is not specified, this operation is limited to 40 requests per 30 minutes. If **cursor** is
        /// specified, the limit is 120 requests per minute. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="sort">How to sort the returned log events. You can sort by **request_timestamp**. To reverse
        /// the sort order, prefix the parameter value with a minus sign (`-`). (optional, default to
        /// request_timestamp)</param>
        /// <param name="filter">A cacheable parameter that limits the results to those matching the specified filter.
        /// For more information, see the
        /// [documentation](https://cloud.ibm.com/docs/services/assistant/filter-reference.html#filter-query-syntax).
        /// (optional)</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to
        /// 100)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <returns><see cref="LogCollection" />LogCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool ListLogs(Callback<LogCollection> callback, string workspaceId, string sort = null, string filter = null, long? pageLimit = null, string cursor = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("A callback is required for ListLogs");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("workspaceId is required for ListLogs");

            RequestObject<LogCollection> req = new RequestObject<LogCollection>
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=ListLogs";
            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(sort))
            {
                req.Parameters["sort"] = sort;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                req.Parameters["filter"] = filter;
            }
            if (pageLimit != null)
            {
                req.Parameters["page_limit"] = pageLimit;
            }
            if (!string.IsNullOrEmpty(cursor))
            {
                req.Parameters["cursor"] = cursor;
            }

            req.OnResponse = OnListLogsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/logs", workspaceId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListLogsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<LogCollection> response = new WatsonResponse<LogCollection>();
            Dictionary<string, object> customData = ((RequestObject<LogCollection>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<LogCollection>(json);
                customData.Add("json", json);
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListLogsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<LogCollection>)req).Callback != null)
                ((RequestObject<LogCollection>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Delete labeled data.
        ///
        /// Deletes all data associated with a specified customer ID. The method has no effect if no data is associated
        /// with the customer ID.
        ///
        /// You associate a customer ID with data by passing the `X-Watson-Metadata` header with a request that passes
        /// data. For more information about personal data and customer IDs, see [Information
        /// security](https://cloud.ibm.com/docs/services/assistant/information-security.html).
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

            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=V1;operation_id=DeleteUserData";
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
                Log.Error("AssistantService.OnDeleteUserDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
        }
    }
}