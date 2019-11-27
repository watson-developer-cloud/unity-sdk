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
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Connection;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.Assistant.V1.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Networking;

namespace IBM.Watson.Assistant.V1
{
    public partial class AssistantService : BaseService
    {
        private const string serviceId = "assistant";
        private const string defaultServiceUrl = "https://gateway.watsonplatform.net/assistant/api";

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
        public AssistantService(string versionDate) : this(versionDate, ConfigBasedAuthenticatorFactory.GetAuthenticator(serviceId)) {}

        /// <summary>
        /// AssistantService constructor.
        /// </summary>
        /// <param name="versionDate">The service version date in `yyyy-mm-dd` format.</param>
        /// <param name="authenticator">The service authenticator.</param>
        public AssistantService(string versionDate, Authenticator authenticator) : base(versionDate, authenticator, serviceId)
        {
            Authenticator = authenticator;

            if (string.IsNullOrEmpty(versionDate))
            {
                throw new ArgumentNullException("A versionDate (format `yyyy-mm-dd`) is required to create an instance of AssistantService");
            }
            else
            {
                VersionDate = versionDate;
            }

            if (string.IsNullOrEmpty(GetServiceUrl()))
            {
                SetServiceUrl(defaultServiceUrl);
            }
        }

        /// <summary>
        /// Get response to user input.
        ///
        /// Send user input to a workspace and receive a response.
        ///
        /// **Important:** This method has been superseded by the new v2 runtime API. The v2 API offers significant
        /// advantages, including ease of deployment, automatic state management, versioning, and search capabilities.
        /// For more information, see the
        /// [documentation](https://cloud.ibm.com/docs/services/assistant?topic=assistant-api-overview).
        ///
        /// There is no rate limit for this operation.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="input">An input object that includes the input text. (optional)</param>
        /// <param name="intents">Intents to use when evaluating the user input. Include intents from the previous
        /// response to continue using those intents rather than trying to recognize intents in the new input.
        /// (optional)</param>
        /// <param name="entities">Entities to use when evaluating the message. Include entities from the previous
        /// response to continue using those entities rather than detecting entities in the new input.
        /// (optional)</param>
        /// <param name="alternateIntents">Whether to return more than one intent. A value of `true` indicates that all
        /// matching intents are returned. (optional, default to false)</param>
        /// <param name="context">State information for the conversation. To maintain state, include the context from
        /// the previous response. (optional)</param>
        /// <param name="output">An output object that includes the response to the user, the dialog nodes that were
        /// triggered, and messages from the log. (optional)</param>
        /// <param name="nodesVisitedDetails">Whether to include additional diagnostic information about the dialog
        /// nodes that were visited during processing of the message. (optional, default to false)</param>
        /// <returns><see cref="MessageResponse" />MessageResponse</returns>
        public bool Message(Callback<MessageResponse> callback, string workspaceId, MessageInput input = null, List<RuntimeIntent> intents = null, List<RuntimeEntity> entities = null, bool? alternateIntents = null, Context context = null, OutputData output = null, bool? nodesVisitedDetails = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `Message`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `Message`");

            RequestObject<MessageResponse> req = new RequestObject<MessageResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "Message"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (nodesVisitedDetails != null)
            {
                req.Parameters["nodes_visited_details"] = (bool)nodesVisitedDetails ? "true" : "false";
            }
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (input != null)
                bodyObject["input"] = JToken.FromObject(input);
            if (intents != null && intents.Count > 0)
                bodyObject["intents"] = JToken.FromObject(intents);
            if (entities != null && entities.Count > 0)
                bodyObject["entities"] = JToken.FromObject(entities);
            if (alternateIntents != null)
                bodyObject["alternate_intents"] = JToken.FromObject(alternateIntents);
            if (context != null)
                bodyObject["context"] = JToken.FromObject(context);
            if (output != null)
                bodyObject["output"] = JToken.FromObject(output);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnMessageResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/message", workspaceId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnMessageResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<MessageResponse> response = new DetailedResponse<MessageResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<MessageResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnMessageResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<MessageResponse>)req).Callback != null)
                ((RequestObject<MessageResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List workspaces.
        ///
        /// List the workspaces associated with a Watson Assistant service instance.
        ///
        /// This operation is limited to 500 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional)</param>
        /// <param name="sort">The attribute by which returned workspaces will be sorted. To reverse the sort order,
        /// prefix the value with a minus sign (`-`). (optional)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="WorkspaceCollection" />WorkspaceCollection</returns>
        public bool ListWorkspaces(Callback<WorkspaceCollection> callback, long? pageLimit = null, string sort = null, string cursor = null, bool? includeAudit = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListWorkspaces`");

            RequestObject<WorkspaceCollection> req = new RequestObject<WorkspaceCollection>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "ListWorkspaces"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (pageLimit != null)
            {
                req.Parameters["page_limit"] = pageLimit;
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

            Connector.URL = GetServiceUrl() + "/v1/workspaces";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListWorkspacesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<WorkspaceCollection> response = new DetailedResponse<WorkspaceCollection>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<WorkspaceCollection>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListWorkspacesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<WorkspaceCollection>)req).Callback != null)
                ((RequestObject<WorkspaceCollection>)req).Callback(response, resp.Error);
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
        /// <param name="name">The name of the workspace. This string cannot contain carriage return, newline, or tab
        /// characters. (optional)</param>
        /// <param name="description">The description of the workspace. This string cannot contain carriage return,
        /// newline, or tab characters. (optional)</param>
        /// <param name="language">The language of the workspace. (optional)</param>
        /// <param name="metadata">Any metadata related to the workspace. (optional)</param>
        /// <param name="learningOptOut">Whether training data from the workspace (including artifacts such as intents
        /// and entities) can be used by IBM for general service improvements. `true` indicates that workspace training
        /// data is not to be used. (optional, default to false)</param>
        /// <param name="systemSettings">Global settings for the workspace. (optional)</param>
        /// <param name="intents">An array of objects defining the intents for the workspace. (optional)</param>
        /// <param name="entities">An array of objects describing the entities for the workspace. (optional)</param>
        /// <param name="dialogNodes">An array of objects describing the dialog nodes in the workspace.
        /// (optional)</param>
        /// <param name="counterexamples">An array of objects defining input examples that have been marked as
        /// irrelevant input. (optional)</param>
        /// <param name="webhooks"> (optional)</param>
        /// <returns><see cref="Workspace" />Workspace</returns>
        public bool CreateWorkspace(Callback<Workspace> callback, string name = null, string description = null, string language = null, Dictionary<string, object> metadata = null, bool? learningOptOut = null, WorkspaceSystemSettings systemSettings = null, List<CreateIntent> intents = null, List<CreateEntity> entities = null, List<DialogNode> dialogNodes = null, List<Counterexample> counterexamples = null, List<Webhook> webhooks = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateWorkspace`");

            RequestObject<Workspace> req = new RequestObject<Workspace>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "CreateWorkspace"))
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
            if (!string.IsNullOrEmpty(language))
                bodyObject["language"] = language;
            if (metadata != null)
                bodyObject["metadata"] = JToken.FromObject(metadata);
            if (learningOptOut != null)
                bodyObject["learning_opt_out"] = JToken.FromObject(learningOptOut);
            if (systemSettings != null)
                bodyObject["system_settings"] = JToken.FromObject(systemSettings);
            if (intents != null && intents.Count > 0)
                bodyObject["intents"] = JToken.FromObject(intents);
            if (entities != null && entities.Count > 0)
                bodyObject["entities"] = JToken.FromObject(entities);
            if (dialogNodes != null && dialogNodes.Count > 0)
                bodyObject["dialog_nodes"] = JToken.FromObject(dialogNodes);
            if (counterexamples != null && counterexamples.Count > 0)
                bodyObject["counterexamples"] = JToken.FromObject(counterexamples);
            if (webhooks != null && webhooks.Count > 0)
                bodyObject["webhooks"] = JToken.FromObject(webhooks);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateWorkspaceResponse;

            Connector.URL = GetServiceUrl() + "/v1/workspaces";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateWorkspaceResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Workspace> response = new DetailedResponse<Workspace>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Workspace>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnCreateWorkspaceResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Workspace>)req).Callback != null)
                ((RequestObject<Workspace>)req).Callback(response, resp.Error);
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
        /// <returns><see cref="Workspace" />Workspace</returns>
        public bool GetWorkspace(Callback<Workspace> callback, string workspaceId, bool? export = null, bool? includeAudit = null, string sort = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetWorkspace`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `GetWorkspace`");

            RequestObject<Workspace> req = new RequestObject<Workspace>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "GetWorkspace"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

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

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}", workspaceId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetWorkspaceResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Workspace> response = new DetailedResponse<Workspace>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Workspace>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnGetWorkspaceResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Workspace>)req).Callback != null)
                ((RequestObject<Workspace>)req).Callback(response, resp.Error);
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
        /// <param name="name">The name of the workspace. This string cannot contain carriage return, newline, or tab
        /// characters. (optional)</param>
        /// <param name="description">The description of the workspace. This string cannot contain carriage return,
        /// newline, or tab characters. (optional)</param>
        /// <param name="language">The language of the workspace. (optional)</param>
        /// <param name="metadata">Any metadata related to the workspace. (optional)</param>
        /// <param name="learningOptOut">Whether training data from the workspace (including artifacts such as intents
        /// and entities) can be used by IBM for general service improvements. `true` indicates that workspace training
        /// data is not to be used. (optional, default to false)</param>
        /// <param name="systemSettings">Global settings for the workspace. (optional)</param>
        /// <param name="intents">An array of objects defining the intents for the workspace. (optional)</param>
        /// <param name="entities">An array of objects describing the entities for the workspace. (optional)</param>
        /// <param name="dialogNodes">An array of objects describing the dialog nodes in the workspace.
        /// (optional)</param>
        /// <param name="counterexamples">An array of objects defining input examples that have been marked as
        /// irrelevant input. (optional)</param>
        /// <param name="append">Whether the new data is to be appended to the existing data in the workspace. If
        /// **append**=`false`, elements included in the new data completely replace the corresponding existing
        /// elements, including all subelements. For example, if the new data includes **entities** and
        /// **append**=`false`, all existing entities in the workspace are discarded and replaced with the new entities.
        ///
        /// If **append**=`true`, existing elements are preserved, and the new elements are added. If any elements in
        /// the new data collide with existing elements, the update request fails. (optional, default to false)</param>
        /// <param name="webhooks"> (optional)</param>
        /// <returns><see cref="Workspace" />Workspace</returns>
        public bool UpdateWorkspace(Callback<Workspace> callback, string workspaceId, string name = null, string description = null, string language = null, Dictionary<string, object> metadata = null, bool? learningOptOut = null, WorkspaceSystemSettings systemSettings = null, List<CreateIntent> intents = null, List<CreateEntity> entities = null, List<DialogNode> dialogNodes = null, List<Counterexample> counterexamples = null, bool? append = null, List<Webhook> webhooks = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateWorkspace`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `UpdateWorkspace`");

            RequestObject<Workspace> req = new RequestObject<Workspace>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "UpdateWorkspace"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (append != null)
            {
                req.Parameters["append"] = (bool)append ? "true" : "false";
            }
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(name))
                bodyObject["name"] = name;
            if (!string.IsNullOrEmpty(description))
                bodyObject["description"] = description;
            if (!string.IsNullOrEmpty(language))
                bodyObject["language"] = language;
            if (metadata != null)
                bodyObject["metadata"] = JToken.FromObject(metadata);
            if (learningOptOut != null)
                bodyObject["learning_opt_out"] = JToken.FromObject(learningOptOut);
            if (systemSettings != null)
                bodyObject["system_settings"] = JToken.FromObject(systemSettings);
            if (intents != null && intents.Count > 0)
                bodyObject["intents"] = JToken.FromObject(intents);
            if (entities != null && entities.Count > 0)
                bodyObject["entities"] = JToken.FromObject(entities);
            if (dialogNodes != null && dialogNodes.Count > 0)
                bodyObject["dialog_nodes"] = JToken.FromObject(dialogNodes);
            if (counterexamples != null && counterexamples.Count > 0)
                bodyObject["counterexamples"] = JToken.FromObject(counterexamples);
            if (webhooks != null && webhooks.Count > 0)
                bodyObject["webhooks"] = JToken.FromObject(webhooks);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateWorkspaceResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}", workspaceId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateWorkspaceResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Workspace> response = new DetailedResponse<Workspace>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Workspace>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnUpdateWorkspaceResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Workspace>)req).Callback != null)
                ((RequestObject<Workspace>)req).Callback(response, resp.Error);
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
        public bool DeleteWorkspace(Callback<object> callback, string workspaceId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteWorkspace`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `DeleteWorkspace`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "DeleteWorkspace"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteWorkspaceResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}", workspaceId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteWorkspaceResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("AssistantService.OnDeleteWorkspaceResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
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
        /// <param name="pageLimit">The number of records to return in each page of results. (optional)</param>
        /// <param name="sort">The attribute by which returned intents will be sorted. To reverse the sort order, prefix
        /// the value with a minus sign (`-`). (optional)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="IntentCollection" />IntentCollection</returns>
        public bool ListIntents(Callback<IntentCollection> callback, string workspaceId, bool? export = null, long? pageLimit = null, string sort = null, string cursor = null, bool? includeAudit = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListIntents`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `ListIntents`");

            RequestObject<IntentCollection> req = new RequestObject<IntentCollection>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "ListIntents"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (export != null)
            {
                req.Parameters["export"] = (bool)export ? "true" : "false";
            }
            if (pageLimit != null)
            {
                req.Parameters["page_limit"] = pageLimit;
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

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/intents", workspaceId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListIntentsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<IntentCollection> response = new DetailedResponse<IntentCollection>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<IntentCollection>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListIntentsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<IntentCollection>)req).Callback != null)
                ((RequestObject<IntentCollection>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create intent.
        ///
        /// Create a new intent.
        ///
        /// If you want to create multiple intents with a single API call, consider using the **[Update
        /// workspace](#update-workspace)** method instead.
        ///
        /// This operation is limited to 2000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The name of the intent. This string must conform to the following restrictions:
        /// - It can contain only Unicode alphanumeric, underscore, hyphen, and dot characters.
        /// - It cannot begin with the reserved prefix `sys-`.</param>
        /// <param name="description">The description of the intent. This string cannot contain carriage return,
        /// newline, or tab characters. (optional)</param>
        /// <param name="examples">An array of user input examples for the intent. (optional)</param>
        /// <returns><see cref="Intent" />Intent</returns>
        public bool CreateIntent(Callback<Intent> callback, string workspaceId, string intent, string description = null, List<Example> examples = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateIntent`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `CreateIntent`");
            if (string.IsNullOrEmpty(intent))
                throw new ArgumentNullException("`intent` is required for `CreateIntent`");

            RequestObject<Intent> req = new RequestObject<Intent>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "CreateIntent"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(intent))
                bodyObject["intent"] = intent;
            if (!string.IsNullOrEmpty(description))
                bodyObject["description"] = description;
            if (examples != null && examples.Count > 0)
                bodyObject["examples"] = JToken.FromObject(examples);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateIntentResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/intents", workspaceId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateIntentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Intent> response = new DetailedResponse<Intent>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Intent>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnCreateIntentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Intent>)req).Callback != null)
                ((RequestObject<Intent>)req).Callback(response, resp.Error);
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
        /// <returns><see cref="Intent" />Intent</returns>
        public bool GetIntent(Callback<Intent> callback, string workspaceId, string intent, bool? export = null, bool? includeAudit = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetIntent`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `GetIntent`");
            if (string.IsNullOrEmpty(intent))
                throw new ArgumentNullException("`intent` is required for `GetIntent`");

            RequestObject<Intent> req = new RequestObject<Intent>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "GetIntent"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

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

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/intents/{1}", workspaceId, intent);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetIntentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Intent> response = new DetailedResponse<Intent>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Intent>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnGetIntentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Intent>)req).Callback != null)
                ((RequestObject<Intent>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update intent.
        ///
        /// Update an existing intent with new or modified data. You must provide component objects defining the content
        /// of the updated intent.
        ///
        /// If you want to update multiple intents with a single API call, consider using the **[Update
        /// workspace](#update-workspace)** method instead.
        ///
        /// This operation is limited to 2000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The intent name.</param>
        /// <param name="newIntent">The name of the intent. This string must conform to the following restrictions:
        /// - It can contain only Unicode alphanumeric, underscore, hyphen, and dot characters.
        /// - It cannot begin with the reserved prefix `sys-`. (optional)</param>
        /// <param name="newDescription">The description of the intent. This string cannot contain carriage return,
        /// newline, or tab characters. (optional)</param>
        /// <param name="newExamples">An array of user input examples for the intent. (optional)</param>
        /// <returns><see cref="Intent" />Intent</returns>
        public bool UpdateIntent(Callback<Intent> callback, string workspaceId, string intent, string newIntent = null, string newDescription = null, List<Example> newExamples = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateIntent`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `UpdateIntent`");
            if (string.IsNullOrEmpty(intent))
                throw new ArgumentNullException("`intent` is required for `UpdateIntent`");

            RequestObject<Intent> req = new RequestObject<Intent>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "UpdateIntent"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(newIntent))
                bodyObject["intent"] = newIntent;
            if (!string.IsNullOrEmpty(newDescription))
                bodyObject["description"] = newDescription;
            if (newExamples != null && newExamples.Count > 0)
                bodyObject["examples"] = JToken.FromObject(newExamples);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateIntentResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/intents/{1}", workspaceId, intent);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateIntentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Intent> response = new DetailedResponse<Intent>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Intent>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnUpdateIntentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Intent>)req).Callback != null)
                ((RequestObject<Intent>)req).Callback(response, resp.Error);
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
        public bool DeleteIntent(Callback<object> callback, string workspaceId, string intent)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteIntent`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `DeleteIntent`");
            if (string.IsNullOrEmpty(intent))
                throw new ArgumentNullException("`intent` is required for `DeleteIntent`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "DeleteIntent"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteIntentResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/intents/{1}", workspaceId, intent);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteIntentResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("AssistantService.OnDeleteIntentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
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
        /// <param name="pageLimit">The number of records to return in each page of results. (optional)</param>
        /// <param name="sort">The attribute by which returned examples will be sorted. To reverse the sort order,
        /// prefix the value with a minus sign (`-`). (optional)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="ExampleCollection" />ExampleCollection</returns>
        public bool ListExamples(Callback<ExampleCollection> callback, string workspaceId, string intent, long? pageLimit = null, string sort = null, string cursor = null, bool? includeAudit = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListExamples`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `ListExamples`");
            if (string.IsNullOrEmpty(intent))
                throw new ArgumentNullException("`intent` is required for `ListExamples`");

            RequestObject<ExampleCollection> req = new RequestObject<ExampleCollection>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "ListExamples"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (pageLimit != null)
            {
                req.Parameters["page_limit"] = pageLimit;
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

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/intents/{1}/examples", workspaceId, intent);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListExamplesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ExampleCollection> response = new DetailedResponse<ExampleCollection>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ExampleCollection>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListExamplesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ExampleCollection>)req).Callback != null)
                ((RequestObject<ExampleCollection>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create user input example.
        ///
        /// Add a new user input example to an intent.
        ///
        /// If you want to add multiple exaples with a single API call, consider using the **[Update
        /// intent](#update-intent)** method instead.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The intent name.</param>
        /// <param name="text">The text of a user input example. This string must conform to the following restrictions:
        /// - It cannot contain carriage return, newline, or tab characters.
        /// - It cannot consist of only whitespace characters.</param>
        /// <param name="mentions">An array of contextual entity mentions. (optional)</param>
        /// <returns><see cref="Example" />Example</returns>
        public bool CreateExample(Callback<Example> callback, string workspaceId, string intent, string text, List<Mention> mentions = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateExample`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `CreateExample`");
            if (string.IsNullOrEmpty(intent))
                throw new ArgumentNullException("`intent` is required for `CreateExample`");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("`text` is required for `CreateExample`");

            RequestObject<Example> req = new RequestObject<Example>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "CreateExample"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(text))
                bodyObject["text"] = text;
            if (mentions != null && mentions.Count > 0)
                bodyObject["mentions"] = JToken.FromObject(mentions);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateExampleResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/intents/{1}/examples", workspaceId, intent);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateExampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Example> response = new DetailedResponse<Example>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Example>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnCreateExampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Example>)req).Callback != null)
                ((RequestObject<Example>)req).Callback(response, resp.Error);
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
        public bool GetExample(Callback<Example> callback, string workspaceId, string intent, string text, bool? includeAudit = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetExample`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `GetExample`");
            if (string.IsNullOrEmpty(intent))
                throw new ArgumentNullException("`intent` is required for `GetExample`");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("`text` is required for `GetExample`");

            RequestObject<Example> req = new RequestObject<Example>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "GetExample"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnGetExampleResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/intents/{1}/examples/{2}", workspaceId, intent, text);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetExampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Example> response = new DetailedResponse<Example>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Example>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnGetExampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Example>)req).Callback != null)
                ((RequestObject<Example>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update user input example.
        ///
        /// Update the text of a user input example.
        ///
        /// If you want to update multiple examples with a single API call, consider using the **[Update
        /// intent](#update-intent)** method instead.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The intent name.</param>
        /// <param name="text">The text of the user input example.</param>
        /// <param name="newText">The text of the user input example. This string must conform to the following
        /// restrictions:
        /// - It cannot contain carriage return, newline, or tab characters.
        /// - It cannot consist of only whitespace characters. (optional)</param>
        /// <param name="newMentions">An array of contextual entity mentions. (optional)</param>
        /// <returns><see cref="Example" />Example</returns>
        public bool UpdateExample(Callback<Example> callback, string workspaceId, string intent, string text, string newText = null, List<Mention> newMentions = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateExample`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `UpdateExample`");
            if (string.IsNullOrEmpty(intent))
                throw new ArgumentNullException("`intent` is required for `UpdateExample`");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("`text` is required for `UpdateExample`");

            RequestObject<Example> req = new RequestObject<Example>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "UpdateExample"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(newText))
                bodyObject["text"] = newText;
            if (newMentions != null && newMentions.Count > 0)
                bodyObject["mentions"] = JToken.FromObject(newMentions);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateExampleResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/intents/{1}/examples/{2}", workspaceId, intent, text);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateExampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Example> response = new DetailedResponse<Example>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Example>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnUpdateExampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Example>)req).Callback != null)
                ((RequestObject<Example>)req).Callback(response, resp.Error);
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
        public bool DeleteExample(Callback<object> callback, string workspaceId, string intent, string text)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteExample`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `DeleteExample`");
            if (string.IsNullOrEmpty(intent))
                throw new ArgumentNullException("`intent` is required for `DeleteExample`");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("`text` is required for `DeleteExample`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "DeleteExample"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteExampleResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/intents/{1}/examples/{2}", workspaceId, intent, text);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteExampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("AssistantService.OnDeleteExampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
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
        /// <param name="pageLimit">The number of records to return in each page of results. (optional)</param>
        /// <param name="sort">The attribute by which returned counterexamples will be sorted. To reverse the sort
        /// order, prefix the value with a minus sign (`-`). (optional)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="CounterexampleCollection" />CounterexampleCollection</returns>
        public bool ListCounterexamples(Callback<CounterexampleCollection> callback, string workspaceId, long? pageLimit = null, string sort = null, string cursor = null, bool? includeAudit = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListCounterexamples`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `ListCounterexamples`");

            RequestObject<CounterexampleCollection> req = new RequestObject<CounterexampleCollection>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "ListCounterexamples"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (pageLimit != null)
            {
                req.Parameters["page_limit"] = pageLimit;
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

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/counterexamples", workspaceId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListCounterexamplesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<CounterexampleCollection> response = new DetailedResponse<CounterexampleCollection>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<CounterexampleCollection>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListCounterexamplesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<CounterexampleCollection>)req).Callback != null)
                ((RequestObject<CounterexampleCollection>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create counterexample.
        ///
        /// Add a new counterexample to a workspace. Counterexamples are examples that have been marked as irrelevant
        /// input.
        ///
        /// If you want to add multiple counterexamples with a single API call, consider using the **[Update
        /// workspace](#update-workspace)** method instead.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="text">The text of a user input marked as irrelevant input. This string must conform to the
        /// following restrictions:
        /// - It cannot contain carriage return, newline, or tab characters.
        /// - It cannot consist of only whitespace characters.</param>
        /// <returns><see cref="Counterexample" />Counterexample</returns>
        public bool CreateCounterexample(Callback<Counterexample> callback, string workspaceId, string text)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateCounterexample`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `CreateCounterexample`");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("`text` is required for `CreateCounterexample`");

            RequestObject<Counterexample> req = new RequestObject<Counterexample>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "CreateCounterexample"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(text))
                bodyObject["text"] = text;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateCounterexampleResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/counterexamples", workspaceId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateCounterexampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Counterexample> response = new DetailedResponse<Counterexample>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Counterexample>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnCreateCounterexampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Counterexample>)req).Callback != null)
                ((RequestObject<Counterexample>)req).Callback(response, resp.Error);
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
        public bool GetCounterexample(Callback<Counterexample> callback, string workspaceId, string text, bool? includeAudit = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetCounterexample`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `GetCounterexample`");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("`text` is required for `GetCounterexample`");

            RequestObject<Counterexample> req = new RequestObject<Counterexample>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "GetCounterexample"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnGetCounterexampleResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/counterexamples/{1}", workspaceId, text);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetCounterexampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Counterexample> response = new DetailedResponse<Counterexample>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Counterexample>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnGetCounterexampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Counterexample>)req).Callback != null)
                ((RequestObject<Counterexample>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update counterexample.
        ///
        /// Update the text of a counterexample. Counterexamples are examples that have been marked as irrelevant input.
        ///
        /// If you want to update multiple counterexamples with a single API call, consider using the **[Update
        /// workspace](#update-workspace)** method instead.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="text">The text of a user input counterexample (for example, `What are you wearing?`).</param>
        /// <param name="newText">The text of a user input marked as irrelevant input. This string must conform to the
        /// following restrictions:
        /// - It cannot contain carriage return, newline, or tab characters.
        /// - It cannot consist of only whitespace characters. (optional)</param>
        /// <returns><see cref="Counterexample" />Counterexample</returns>
        public bool UpdateCounterexample(Callback<Counterexample> callback, string workspaceId, string text, string newText = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateCounterexample`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `UpdateCounterexample`");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("`text` is required for `UpdateCounterexample`");

            RequestObject<Counterexample> req = new RequestObject<Counterexample>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "UpdateCounterexample"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(newText))
                bodyObject["text"] = newText;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateCounterexampleResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/counterexamples/{1}", workspaceId, text);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateCounterexampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Counterexample> response = new DetailedResponse<Counterexample>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Counterexample>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnUpdateCounterexampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Counterexample>)req).Callback != null)
                ((RequestObject<Counterexample>)req).Callback(response, resp.Error);
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
        public bool DeleteCounterexample(Callback<object> callback, string workspaceId, string text)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteCounterexample`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `DeleteCounterexample`");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("`text` is required for `DeleteCounterexample`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "DeleteCounterexample"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteCounterexampleResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/counterexamples/{1}", workspaceId, text);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteCounterexampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("AssistantService.OnDeleteCounterexampleResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
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
        /// <param name="pageLimit">The number of records to return in each page of results. (optional)</param>
        /// <param name="sort">The attribute by which returned entities will be sorted. To reverse the sort order,
        /// prefix the value with a minus sign (`-`). (optional)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="EntityCollection" />EntityCollection</returns>
        public bool ListEntities(Callback<EntityCollection> callback, string workspaceId, bool? export = null, long? pageLimit = null, string sort = null, string cursor = null, bool? includeAudit = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListEntities`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `ListEntities`");

            RequestObject<EntityCollection> req = new RequestObject<EntityCollection>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "ListEntities"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (export != null)
            {
                req.Parameters["export"] = (bool)export ? "true" : "false";
            }
            if (pageLimit != null)
            {
                req.Parameters["page_limit"] = pageLimit;
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

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/entities", workspaceId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListEntitiesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<EntityCollection> response = new DetailedResponse<EntityCollection>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<EntityCollection>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListEntitiesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<EntityCollection>)req).Callback != null)
                ((RequestObject<EntityCollection>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create entity.
        ///
        /// Create a new entity, or enable a system entity.
        ///
        /// If you want to create multiple entities with a single API call, consider using the **[Update
        /// workspace](#update-workspace)** method instead.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity. This string must conform to the following restrictions:
        /// - It can contain only Unicode alphanumeric, underscore, and hyphen characters.
        /// - If you specify an entity name beginning with the reserved prefix `sys-`, it must be the name of a system
        /// entity that you want to enable. (Any entity content specified with the request is ignored.).</param>
        /// <param name="description">The description of the entity. This string cannot contain carriage return,
        /// newline, or tab characters. (optional)</param>
        /// <param name="metadata">Any metadata related to the entity. (optional)</param>
        /// <param name="fuzzyMatch">Whether to use fuzzy matching for the entity. (optional)</param>
        /// <param name="values">An array of objects describing the entity values. (optional)</param>
        /// <returns><see cref="Entity" />Entity</returns>
        public bool CreateEntity(Callback<Entity> callback, string workspaceId, string entity, string description = null, Dictionary<string, object> metadata = null, bool? fuzzyMatch = null, List<CreateValue> values = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateEntity`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `CreateEntity`");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("`entity` is required for `CreateEntity`");

            RequestObject<Entity> req = new RequestObject<Entity>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "CreateEntity"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(entity))
                bodyObject["entity"] = entity;
            if (!string.IsNullOrEmpty(description))
                bodyObject["description"] = description;
            if (metadata != null)
                bodyObject["metadata"] = JToken.FromObject(metadata);
            if (fuzzyMatch != null)
                bodyObject["fuzzy_match"] = JToken.FromObject(fuzzyMatch);
            if (values != null && values.Count > 0)
                bodyObject["values"] = JToken.FromObject(values);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateEntityResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/entities", workspaceId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateEntityResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Entity> response = new DetailedResponse<Entity>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Entity>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnCreateEntityResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Entity>)req).Callback != null)
                ((RequestObject<Entity>)req).Callback(response, resp.Error);
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
        /// <returns><see cref="Entity" />Entity</returns>
        public bool GetEntity(Callback<Entity> callback, string workspaceId, string entity, bool? export = null, bool? includeAudit = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetEntity`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `GetEntity`");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("`entity` is required for `GetEntity`");

            RequestObject<Entity> req = new RequestObject<Entity>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "GetEntity"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

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

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/entities/{1}", workspaceId, entity);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetEntityResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Entity> response = new DetailedResponse<Entity>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Entity>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnGetEntityResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Entity>)req).Callback != null)
                ((RequestObject<Entity>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update entity.
        ///
        /// Update an existing entity with new or modified data. You must provide component objects defining the content
        /// of the updated entity.
        ///
        /// If you want to update multiple entities with a single API call, consider using the **[Update
        /// workspace](#update-workspace)** method instead.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="newEntity">The name of the entity. This string must conform to the following restrictions:
        /// - It can contain only Unicode alphanumeric, underscore, and hyphen characters.
        /// - It cannot begin with the reserved prefix `sys-`. (optional)</param>
        /// <param name="newDescription">The description of the entity. This string cannot contain carriage return,
        /// newline, or tab characters. (optional)</param>
        /// <param name="newMetadata">Any metadata related to the entity. (optional)</param>
        /// <param name="newFuzzyMatch">Whether to use fuzzy matching for the entity. (optional)</param>
        /// <param name="newValues">An array of objects describing the entity values. (optional)</param>
        /// <returns><see cref="Entity" />Entity</returns>
        public bool UpdateEntity(Callback<Entity> callback, string workspaceId, string entity, string newEntity = null, string newDescription = null, Dictionary<string, object> newMetadata = null, bool? newFuzzyMatch = null, List<CreateValue> newValues = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateEntity`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `UpdateEntity`");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("`entity` is required for `UpdateEntity`");

            RequestObject<Entity> req = new RequestObject<Entity>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "UpdateEntity"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(newEntity))
                bodyObject["entity"] = newEntity;
            if (!string.IsNullOrEmpty(newDescription))
                bodyObject["description"] = newDescription;
            if (newMetadata != null)
                bodyObject["metadata"] = JToken.FromObject(newMetadata);
            if (newFuzzyMatch != null)
                bodyObject["fuzzy_match"] = JToken.FromObject(newFuzzyMatch);
            if (newValues != null && newValues.Count > 0)
                bodyObject["values"] = JToken.FromObject(newValues);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateEntityResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/entities/{1}", workspaceId, entity);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateEntityResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Entity> response = new DetailedResponse<Entity>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Entity>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnUpdateEntityResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Entity>)req).Callback != null)
                ((RequestObject<Entity>)req).Callback(response, resp.Error);
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
        public bool DeleteEntity(Callback<object> callback, string workspaceId, string entity)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteEntity`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `DeleteEntity`");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("`entity` is required for `DeleteEntity`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "DeleteEntity"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteEntityResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/entities/{1}", workspaceId, entity);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteEntityResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("AssistantService.OnDeleteEntityResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
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
        public bool ListMentions(Callback<EntityMentionCollection> callback, string workspaceId, string entity, bool? export = null, bool? includeAudit = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListMentions`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `ListMentions`");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("`entity` is required for `ListMentions`");

            RequestObject<EntityMentionCollection> req = new RequestObject<EntityMentionCollection>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "ListMentions"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

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

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/entities/{1}/mentions", workspaceId, entity);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListMentionsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<EntityMentionCollection> response = new DetailedResponse<EntityMentionCollection>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<EntityMentionCollection>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListMentionsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<EntityMentionCollection>)req).Callback != null)
                ((RequestObject<EntityMentionCollection>)req).Callback(response, resp.Error);
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
        /// <param name="pageLimit">The number of records to return in each page of results. (optional)</param>
        /// <param name="sort">The attribute by which returned entity values will be sorted. To reverse the sort order,
        /// prefix the value with a minus sign (`-`). (optional)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="ValueCollection" />ValueCollection</returns>
        public bool ListValues(Callback<ValueCollection> callback, string workspaceId, string entity, bool? export = null, long? pageLimit = null, string sort = null, string cursor = null, bool? includeAudit = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListValues`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `ListValues`");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("`entity` is required for `ListValues`");

            RequestObject<ValueCollection> req = new RequestObject<ValueCollection>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "ListValues"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (export != null)
            {
                req.Parameters["export"] = (bool)export ? "true" : "false";
            }
            if (pageLimit != null)
            {
                req.Parameters["page_limit"] = pageLimit;
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

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/entities/{1}/values", workspaceId, entity);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListValuesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ValueCollection> response = new DetailedResponse<ValueCollection>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ValueCollection>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListValuesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ValueCollection>)req).Callback != null)
                ((RequestObject<ValueCollection>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create entity value.
        ///
        /// Create a new value for an entity.
        ///
        /// If you want to create multiple entity values with a single API call, consider using the **[Update
        /// entity](#update-entity)** method instead.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value. This string must conform to the following restrictions:
        /// - It cannot contain carriage return, newline, or tab characters.
        /// - It cannot consist of only whitespace characters.</param>
        /// <param name="metadata">Any metadata related to the entity value. (optional)</param>
        /// <param name="type">Specifies the type of entity value. (optional, default to synonyms)</param>
        /// <param name="synonyms">An array of synonyms for the entity value. A value can specify either synonyms or
        /// patterns (depending on the value type), but not both. A synonym must conform to the following resrictions:
        /// - It cannot contain carriage return, newline, or tab characters.
        /// - It cannot consist of only whitespace characters. (optional)</param>
        /// <param name="patterns">An array of patterns for the entity value. A value can specify either synonyms or
        /// patterns (depending on the value type), but not both. A pattern is a regular expression; for more
        /// information about how to specify a pattern, see the
        /// [documentation](https://cloud.ibm.com/docs/services/assistant?topic=assistant-entities#entities-create-dictionary-based).
        /// (optional)</param>
        /// <returns><see cref="Value" />Value</returns>
        public bool CreateValue(Callback<Value> callback, string workspaceId, string entity, string value, Dictionary<string, object> metadata = null, string type = null, List<string> synonyms = null, List<string> patterns = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateValue`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `CreateValue`");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("`entity` is required for `CreateValue`");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("`value` is required for `CreateValue`");

            RequestObject<Value> req = new RequestObject<Value>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "CreateValue"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(value))
                bodyObject["value"] = value;
            if (metadata != null)
                bodyObject["metadata"] = JToken.FromObject(metadata);
            if (!string.IsNullOrEmpty(type))
                bodyObject["type"] = type;
            if (synonyms != null && synonyms.Count > 0)
                bodyObject["synonyms"] = JToken.FromObject(synonyms);
            if (patterns != null && patterns.Count > 0)
                bodyObject["patterns"] = JToken.FromObject(patterns);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateValueResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/entities/{1}/values", workspaceId, entity);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateValueResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Value> response = new DetailedResponse<Value>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Value>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnCreateValueResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Value>)req).Callback != null)
                ((RequestObject<Value>)req).Callback(response, resp.Error);
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
        /// <returns><see cref="Value" />Value</returns>
        public bool GetValue(Callback<Value> callback, string workspaceId, string entity, string value, bool? export = null, bool? includeAudit = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetValue`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `GetValue`");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("`entity` is required for `GetValue`");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("`value` is required for `GetValue`");

            RequestObject<Value> req = new RequestObject<Value>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "GetValue"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

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

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}", workspaceId, entity, value);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetValueResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Value> response = new DetailedResponse<Value>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Value>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnGetValueResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Value>)req).Callback != null)
                ((RequestObject<Value>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update entity value.
        ///
        /// Update an existing entity value with new or modified data. You must provide component objects defining the
        /// content of the updated entity value.
        ///
        /// If you want to update multiple entity values with a single API call, consider using the **[Update
        /// entity](#update-entity)** method instead.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="newValue">The text of the entity value. This string must conform to the following restrictions:
        /// - It cannot contain carriage return, newline, or tab characters.
        /// - It cannot consist of only whitespace characters. (optional)</param>
        /// <param name="newMetadata">Any metadata related to the entity value. (optional)</param>
        /// <param name="newType">Specifies the type of entity value. (optional, default to synonyms)</param>
        /// <param name="newSynonyms">An array of synonyms for the entity value. A value can specify either synonyms or
        /// patterns (depending on the value type), but not both. A synonym must conform to the following resrictions:
        /// - It cannot contain carriage return, newline, or tab characters.
        /// - It cannot consist of only whitespace characters. (optional)</param>
        /// <param name="newPatterns">An array of patterns for the entity value. A value can specify either synonyms or
        /// patterns (depending on the value type), but not both. A pattern is a regular expression; for more
        /// information about how to specify a pattern, see the
        /// [documentation](https://cloud.ibm.com/docs/services/assistant?topic=assistant-entities#entities-create-dictionary-based).
        /// (optional)</param>
        /// <returns><see cref="Value" />Value</returns>
        public bool UpdateValue(Callback<Value> callback, string workspaceId, string entity, string value, string newValue = null, Dictionary<string, object> newMetadata = null, string newType = null, List<string> newSynonyms = null, List<string> newPatterns = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateValue`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `UpdateValue`");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("`entity` is required for `UpdateValue`");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("`value` is required for `UpdateValue`");

            RequestObject<Value> req = new RequestObject<Value>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "UpdateValue"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(newValue))
                bodyObject["value"] = newValue;
            if (newMetadata != null)
                bodyObject["metadata"] = JToken.FromObject(newMetadata);
            if (!string.IsNullOrEmpty(newType))
                bodyObject["type"] = newType;
            if (newSynonyms != null && newSynonyms.Count > 0)
                bodyObject["synonyms"] = JToken.FromObject(newSynonyms);
            if (newPatterns != null && newPatterns.Count > 0)
                bodyObject["patterns"] = JToken.FromObject(newPatterns);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateValueResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}", workspaceId, entity, value);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateValueResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Value> response = new DetailedResponse<Value>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Value>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnUpdateValueResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Value>)req).Callback != null)
                ((RequestObject<Value>)req).Callback(response, resp.Error);
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
        public bool DeleteValue(Callback<object> callback, string workspaceId, string entity, string value)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteValue`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `DeleteValue`");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("`entity` is required for `DeleteValue`");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("`value` is required for `DeleteValue`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "DeleteValue"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteValueResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}", workspaceId, entity, value);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteValueResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("AssistantService.OnDeleteValueResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
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
        /// <param name="pageLimit">The number of records to return in each page of results. (optional)</param>
        /// <param name="sort">The attribute by which returned entity value synonyms will be sorted. To reverse the sort
        /// order, prefix the value with a minus sign (`-`). (optional)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="SynonymCollection" />SynonymCollection</returns>
        public bool ListSynonyms(Callback<SynonymCollection> callback, string workspaceId, string entity, string value, long? pageLimit = null, string sort = null, string cursor = null, bool? includeAudit = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListSynonyms`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `ListSynonyms`");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("`entity` is required for `ListSynonyms`");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("`value` is required for `ListSynonyms`");

            RequestObject<SynonymCollection> req = new RequestObject<SynonymCollection>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "ListSynonyms"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (pageLimit != null)
            {
                req.Parameters["page_limit"] = pageLimit;
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

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}/synonyms", workspaceId, entity, value);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListSynonymsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<SynonymCollection> response = new DetailedResponse<SynonymCollection>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<SynonymCollection>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListSynonymsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<SynonymCollection>)req).Callback != null)
                ((RequestObject<SynonymCollection>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create entity value synonym.
        ///
        /// Add a new synonym to an entity value.
        ///
        /// If you want to create multiple synonyms with a single API call, consider using the **[Update
        /// entity](#update-entity)** or **[Update entity value](#update-entity-value)** method instead.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="synonym">The text of the synonym. This string must conform to the following restrictions:
        /// - It cannot contain carriage return, newline, or tab characters.
        /// - It cannot consist of only whitespace characters.</param>
        /// <returns><see cref="Synonym" />Synonym</returns>
        public bool CreateSynonym(Callback<Synonym> callback, string workspaceId, string entity, string value, string synonym)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateSynonym`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `CreateSynonym`");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("`entity` is required for `CreateSynonym`");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("`value` is required for `CreateSynonym`");
            if (string.IsNullOrEmpty(synonym))
                throw new ArgumentNullException("`synonym` is required for `CreateSynonym`");

            RequestObject<Synonym> req = new RequestObject<Synonym>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "CreateSynonym"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(synonym))
                bodyObject["synonym"] = synonym;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateSynonymResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}/synonyms", workspaceId, entity, value);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateSynonymResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Synonym> response = new DetailedResponse<Synonym>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Synonym>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnCreateSynonymResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Synonym>)req).Callback != null)
                ((RequestObject<Synonym>)req).Callback(response, resp.Error);
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
        public bool GetSynonym(Callback<Synonym> callback, string workspaceId, string entity, string value, string synonym, bool? includeAudit = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetSynonym`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `GetSynonym`");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("`entity` is required for `GetSynonym`");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("`value` is required for `GetSynonym`");
            if (string.IsNullOrEmpty(synonym))
                throw new ArgumentNullException("`synonym` is required for `GetSynonym`");

            RequestObject<Synonym> req = new RequestObject<Synonym>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "GetSynonym"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnGetSynonymResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}/synonyms/{3}", workspaceId, entity, value, synonym);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetSynonymResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Synonym> response = new DetailedResponse<Synonym>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Synonym>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnGetSynonymResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Synonym>)req).Callback != null)
                ((RequestObject<Synonym>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update entity value synonym.
        ///
        /// Update an existing entity value synonym with new text.
        ///
        /// If you want to update multiple synonyms with a single API call, consider using the **[Update
        /// entity](#update-entity)** or **[Update entity value](#update-entity-value)** method instead.
        ///
        /// This operation is limited to 1000 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="synonym">The text of the synonym.</param>
        /// <param name="newSynonym">The text of the synonym. This string must conform to the following restrictions:
        /// - It cannot contain carriage return, newline, or tab characters.
        /// - It cannot consist of only whitespace characters. (optional)</param>
        /// <returns><see cref="Synonym" />Synonym</returns>
        public bool UpdateSynonym(Callback<Synonym> callback, string workspaceId, string entity, string value, string synonym, string newSynonym = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateSynonym`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `UpdateSynonym`");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("`entity` is required for `UpdateSynonym`");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("`value` is required for `UpdateSynonym`");
            if (string.IsNullOrEmpty(synonym))
                throw new ArgumentNullException("`synonym` is required for `UpdateSynonym`");

            RequestObject<Synonym> req = new RequestObject<Synonym>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "UpdateSynonym"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(newSynonym))
                bodyObject["synonym"] = newSynonym;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateSynonymResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}/synonyms/{3}", workspaceId, entity, value, synonym);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateSynonymResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Synonym> response = new DetailedResponse<Synonym>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Synonym>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnUpdateSynonymResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Synonym>)req).Callback != null)
                ((RequestObject<Synonym>)req).Callback(response, resp.Error);
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
        public bool DeleteSynonym(Callback<object> callback, string workspaceId, string entity, string value, string synonym)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteSynonym`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `DeleteSynonym`");
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("`entity` is required for `DeleteSynonym`");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("`value` is required for `DeleteSynonym`");
            if (string.IsNullOrEmpty(synonym))
                throw new ArgumentNullException("`synonym` is required for `DeleteSynonym`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "DeleteSynonym"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteSynonymResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}/synonyms/{3}", workspaceId, entity, value, synonym);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteSynonymResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("AssistantService.OnDeleteSynonymResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
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
        /// <param name="pageLimit">The number of records to return in each page of results. (optional)</param>
        /// <param name="sort">The attribute by which returned dialog nodes will be sorted. To reverse the sort order,
        /// prefix the value with a minus sign (`-`). (optional)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in
        /// the response. (optional, default to false)</param>
        /// <returns><see cref="DialogNodeCollection" />DialogNodeCollection</returns>
        public bool ListDialogNodes(Callback<DialogNodeCollection> callback, string workspaceId, long? pageLimit = null, string sort = null, string cursor = null, bool? includeAudit = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListDialogNodes`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `ListDialogNodes`");

            RequestObject<DialogNodeCollection> req = new RequestObject<DialogNodeCollection>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "ListDialogNodes"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (pageLimit != null)
            {
                req.Parameters["page_limit"] = pageLimit;
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

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/dialog_nodes", workspaceId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListDialogNodesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DialogNodeCollection> response = new DetailedResponse<DialogNodeCollection>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DialogNodeCollection>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListDialogNodesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DialogNodeCollection>)req).Callback != null)
                ((RequestObject<DialogNodeCollection>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create dialog node.
        ///
        /// Create a new dialog node.
        ///
        /// If you want to create multiple dialog nodes with a single API call, consider using the **[Update
        /// workspace](#update-workspace)** method instead.
        ///
        /// This operation is limited to 500 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="dialogNode">The dialog node ID. This string must conform to the following restrictions:
        /// - It can contain only Unicode alphanumeric, space, underscore, hyphen, and dot characters.</param>
        /// <param name="description">The description of the dialog node. This string cannot contain carriage return,
        /// newline, or tab characters. (optional)</param>
        /// <param name="conditions">The condition that will trigger the dialog node. This string cannot contain
        /// carriage return, newline, or tab characters. (optional)</param>
        /// <param name="parent">The ID of the parent dialog node. This property is omitted if the dialog node has no
        /// parent. (optional)</param>
        /// <param name="previousSibling">The ID of the previous sibling dialog node. This property is omitted if the
        /// dialog node has no previous sibling. (optional)</param>
        /// <param name="output">The output of the dialog node. For more information about how to specify dialog node
        /// output, see the
        /// [documentation](https://cloud.ibm.com/docs/services/assistant?topic=assistant-dialog-overview#dialog-overview-responses).
        /// (optional)</param>
        /// <param name="context">The context for the dialog node. (optional)</param>
        /// <param name="metadata">The metadata for the dialog node. (optional)</param>
        /// <param name="nextStep">The next step to execute following this dialog node. (optional)</param>
        /// <param name="title">The alias used to identify the dialog node. This string must conform to the following
        /// restrictions:
        /// - It can contain only Unicode alphanumeric, space, underscore, hyphen, and dot characters.
        /// (optional)</param>
        /// <param name="type">How the dialog node is processed. (optional)</param>
        /// <param name="eventName">How an `event_handler` node is processed. (optional)</param>
        /// <param name="variable">The location in the dialog context where output is stored. (optional)</param>
        /// <param name="actions">An array of objects describing any actions to be invoked by the dialog node.
        /// (optional)</param>
        /// <param name="digressIn">Whether this top-level dialog node can be digressed into. (optional)</param>
        /// <param name="digressOut">Whether this dialog node can be returned to after a digression. (optional)</param>
        /// <param name="digressOutSlots">Whether the user can digress to top-level nodes while filling out slots.
        /// (optional)</param>
        /// <param name="userLabel">A label that can be displayed externally to describe the purpose of the node to
        /// users. (optional)</param>
        /// <param name="disambiguationOptOut">Whether the dialog node should be excluded from disambiguation
        /// suggestions. (optional, default to false)</param>
        /// <returns><see cref="DialogNode" />DialogNode</returns>
        public bool CreateDialogNode(Callback<DialogNode> callback, string workspaceId, string dialogNode, string description = null, string conditions = null, string parent = null, string previousSibling = null, DialogNodeOutput output = null, Dictionary<string, object> context = null, Dictionary<string, object> metadata = null, DialogNodeNextStep nextStep = null, string title = null, string type = null, string eventName = null, string variable = null, List<DialogNodeAction> actions = null, string digressIn = null, string digressOut = null, string digressOutSlots = null, string userLabel = null, bool? disambiguationOptOut = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateDialogNode`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `CreateDialogNode`");
            if (string.IsNullOrEmpty(dialogNode))
                throw new ArgumentNullException("`dialogNode` is required for `CreateDialogNode`");

            RequestObject<DialogNode> req = new RequestObject<DialogNode>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "CreateDialogNode"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(dialogNode))
                bodyObject["dialog_node"] = dialogNode;
            if (!string.IsNullOrEmpty(description))
                bodyObject["description"] = description;
            if (!string.IsNullOrEmpty(conditions))
                bodyObject["conditions"] = conditions;
            if (!string.IsNullOrEmpty(parent))
                bodyObject["parent"] = parent;
            if (!string.IsNullOrEmpty(previousSibling))
                bodyObject["previous_sibling"] = previousSibling;
            if (output != null)
                bodyObject["output"] = JToken.FromObject(output);
            if (context != null)
                bodyObject["context"] = JToken.FromObject(context);
            if (metadata != null)
                bodyObject["metadata"] = JToken.FromObject(metadata);
            if (nextStep != null)
                bodyObject["next_step"] = JToken.FromObject(nextStep);
            if (!string.IsNullOrEmpty(title))
                bodyObject["title"] = title;
            if (!string.IsNullOrEmpty(type))
                bodyObject["type"] = type;
            if (!string.IsNullOrEmpty(eventName))
                bodyObject["event_name"] = eventName;
            if (!string.IsNullOrEmpty(variable))
                bodyObject["variable"] = variable;
            if (actions != null && actions.Count > 0)
                bodyObject["actions"] = JToken.FromObject(actions);
            if (!string.IsNullOrEmpty(digressIn))
                bodyObject["digress_in"] = digressIn;
            if (!string.IsNullOrEmpty(digressOut))
                bodyObject["digress_out"] = digressOut;
            if (!string.IsNullOrEmpty(digressOutSlots))
                bodyObject["digress_out_slots"] = digressOutSlots;
            if (!string.IsNullOrEmpty(userLabel))
                bodyObject["user_label"] = userLabel;
            if (disambiguationOptOut != null)
                bodyObject["disambiguation_opt_out"] = JToken.FromObject(disambiguationOptOut);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateDialogNodeResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/dialog_nodes", workspaceId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateDialogNodeResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DialogNode> response = new DetailedResponse<DialogNode>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DialogNode>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnCreateDialogNodeResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DialogNode>)req).Callback != null)
                ((RequestObject<DialogNode>)req).Callback(response, resp.Error);
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
        public bool GetDialogNode(Callback<DialogNode> callback, string workspaceId, string dialogNode, bool? includeAudit = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetDialogNode`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `GetDialogNode`");
            if (string.IsNullOrEmpty(dialogNode))
                throw new ArgumentNullException("`dialogNode` is required for `GetDialogNode`");

            RequestObject<DialogNode> req = new RequestObject<DialogNode>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "GetDialogNode"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (includeAudit != null)
            {
                req.Parameters["include_audit"] = (bool)includeAudit ? "true" : "false";
            }

            req.OnResponse = OnGetDialogNodeResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/dialog_nodes/{1}", workspaceId, dialogNode);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetDialogNodeResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DialogNode> response = new DetailedResponse<DialogNode>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DialogNode>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnGetDialogNodeResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DialogNode>)req).Callback != null)
                ((RequestObject<DialogNode>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update dialog node.
        ///
        /// Update an existing dialog node with new or modified data.
        ///
        /// If you want to update multiple dialog nodes with a single API call, consider using the **[Update
        /// workspace](#update-workspace)** method instead.
        ///
        /// This operation is limited to 500 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="dialogNode">The dialog node ID (for example, `get_order`).</param>
        /// <param name="newDialogNode">The dialog node ID. This string must conform to the following restrictions:
        /// - It can contain only Unicode alphanumeric, space, underscore, hyphen, and dot characters.
        /// (optional)</param>
        /// <param name="newDescription">The description of the dialog node. This string cannot contain carriage return,
        /// newline, or tab characters. (optional)</param>
        /// <param name="newConditions">The condition that will trigger the dialog node. This string cannot contain
        /// carriage return, newline, or tab characters. (optional)</param>
        /// <param name="newParent">The ID of the parent dialog node. This property is omitted if the dialog node has no
        /// parent. (optional)</param>
        /// <param name="newPreviousSibling">The ID of the previous sibling dialog node. This property is omitted if the
        /// dialog node has no previous sibling. (optional)</param>
        /// <param name="newOutput">The output of the dialog node. For more information about how to specify dialog node
        /// output, see the
        /// [documentation](https://cloud.ibm.com/docs/services/assistant?topic=assistant-dialog-overview#dialog-overview-responses).
        /// (optional)</param>
        /// <param name="newContext">The context for the dialog node. (optional)</param>
        /// <param name="newMetadata">The metadata for the dialog node. (optional)</param>
        /// <param name="newNextStep">The next step to execute following this dialog node. (optional)</param>
        /// <param name="newTitle">The alias used to identify the dialog node. This string must conform to the following
        /// restrictions:
        /// - It can contain only Unicode alphanumeric, space, underscore, hyphen, and dot characters.
        /// (optional)</param>
        /// <param name="newType">How the dialog node is processed. (optional)</param>
        /// <param name="newEventName">How an `event_handler` node is processed. (optional)</param>
        /// <param name="newVariable">The location in the dialog context where output is stored. (optional)</param>
        /// <param name="newActions">An array of objects describing any actions to be invoked by the dialog node.
        /// (optional)</param>
        /// <param name="newDigressIn">Whether this top-level dialog node can be digressed into. (optional)</param>
        /// <param name="newDigressOut">Whether this dialog node can be returned to after a digression.
        /// (optional)</param>
        /// <param name="newDigressOutSlots">Whether the user can digress to top-level nodes while filling out slots.
        /// (optional)</param>
        /// <param name="newUserLabel">A label that can be displayed externally to describe the purpose of the node to
        /// users. (optional)</param>
        /// <param name="newDisambiguationOptOut">Whether the dialog node should be excluded from disambiguation
        /// suggestions. (optional, default to false)</param>
        /// <returns><see cref="DialogNode" />DialogNode</returns>
        public bool UpdateDialogNode(Callback<DialogNode> callback, string workspaceId, string dialogNode, string newDialogNode = null, string newDescription = null, string newConditions = null, string newParent = null, string newPreviousSibling = null, DialogNodeOutput newOutput = null, Dictionary<string, object> newContext = null, Dictionary<string, object> newMetadata = null, DialogNodeNextStep newNextStep = null, string newTitle = null, string newType = null, string newEventName = null, string newVariable = null, List<DialogNodeAction> newActions = null, string newDigressIn = null, string newDigressOut = null, string newDigressOutSlots = null, string newUserLabel = null, bool? newDisambiguationOptOut = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateDialogNode`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `UpdateDialogNode`");
            if (string.IsNullOrEmpty(dialogNode))
                throw new ArgumentNullException("`dialogNode` is required for `UpdateDialogNode`");

            RequestObject<DialogNode> req = new RequestObject<DialogNode>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "UpdateDialogNode"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(newDialogNode))
                bodyObject["dialog_node"] = newDialogNode;
            if (!string.IsNullOrEmpty(newDescription))
                bodyObject["description"] = newDescription;
            if (!string.IsNullOrEmpty(newConditions))
                bodyObject["conditions"] = newConditions;
            if (!string.IsNullOrEmpty(newParent))
                bodyObject["parent"] = newParent;
            if (!string.IsNullOrEmpty(newPreviousSibling))
                bodyObject["previous_sibling"] = newPreviousSibling;
            if (newOutput != null)
                bodyObject["output"] = JToken.FromObject(newOutput);
            if (newContext != null)
                bodyObject["context"] = JToken.FromObject(newContext);
            if (newMetadata != null)
                bodyObject["metadata"] = JToken.FromObject(newMetadata);
            if (newNextStep != null)
                bodyObject["next_step"] = JToken.FromObject(newNextStep);
            if (!string.IsNullOrEmpty(newTitle))
                bodyObject["title"] = newTitle;
            if (!string.IsNullOrEmpty(newType))
                bodyObject["type"] = newType;
            if (!string.IsNullOrEmpty(newEventName))
                bodyObject["event_name"] = newEventName;
            if (!string.IsNullOrEmpty(newVariable))
                bodyObject["variable"] = newVariable;
            if (newActions != null && newActions.Count > 0)
                bodyObject["actions"] = JToken.FromObject(newActions);
            if (!string.IsNullOrEmpty(newDigressIn))
                bodyObject["digress_in"] = newDigressIn;
            if (!string.IsNullOrEmpty(newDigressOut))
                bodyObject["digress_out"] = newDigressOut;
            if (!string.IsNullOrEmpty(newDigressOutSlots))
                bodyObject["digress_out_slots"] = newDigressOutSlots;
            if (!string.IsNullOrEmpty(newUserLabel))
                bodyObject["user_label"] = newUserLabel;
            if (newDisambiguationOptOut != null)
                bodyObject["disambiguation_opt_out"] = JToken.FromObject(newDisambiguationOptOut);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateDialogNodeResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/dialog_nodes/{1}", workspaceId, dialogNode);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateDialogNodeResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DialogNode> response = new DetailedResponse<DialogNode>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DialogNode>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnUpdateDialogNodeResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DialogNode>)req).Callback != null)
                ((RequestObject<DialogNode>)req).Callback(response, resp.Error);
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
        public bool DeleteDialogNode(Callback<object> callback, string workspaceId, string dialogNode)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteDialogNode`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `DeleteDialogNode`");
            if (string.IsNullOrEmpty(dialogNode))
                throw new ArgumentNullException("`dialogNode` is required for `DeleteDialogNode`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "DeleteDialogNode"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteDialogNodeResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/dialog_nodes/{1}", workspaceId, dialogNode);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteDialogNodeResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("AssistantService.OnDeleteDialogNodeResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
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
        /// the sort order, prefix the parameter value with a minus sign (`-`). (optional)</param>
        /// <param name="filter">A cacheable parameter that limits the results to those matching the specified filter.
        /// For more information, see the
        /// [documentation](https://cloud.ibm.com/docs/services/assistant?topic=assistant-filter-reference#filter-reference).
        /// (optional)</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <returns><see cref="LogCollection" />LogCollection</returns>
        public bool ListLogs(Callback<LogCollection> callback, string workspaceId, string sort = null, string filter = null, long? pageLimit = null, string cursor = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListLogs`");
            if (string.IsNullOrEmpty(workspaceId))
                throw new ArgumentNullException("`workspaceId` is required for `ListLogs`");

            RequestObject<LogCollection> req = new RequestObject<LogCollection>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "ListLogs"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

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

            Connector.URL = GetServiceUrl() + string.Format("/v1/workspaces/{0}/logs", workspaceId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListLogsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<LogCollection> response = new DetailedResponse<LogCollection>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<LogCollection>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListLogsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<LogCollection>)req).Callback != null)
                ((RequestObject<LogCollection>)req).Callback(response, resp.Error);
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
        /// You must specify a filter query that includes a value for `language`, as well as a value for
        /// `request.context.system.assistant_id`, `workspace_id`, or `request.context.metadata.deployment`. For more
        /// information, see the
        /// [documentation](https://cloud.ibm.com/docs/services/assistant?topic=assistant-filter-reference#filter-reference).</param>
        /// <param name="sort">How to sort the returned log events. You can sort by **request_timestamp**. To reverse
        /// the sort order, prefix the parameter value with a minus sign (`-`). (optional)</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <returns><see cref="LogCollection" />LogCollection</returns>
        public bool ListAllLogs(Callback<LogCollection> callback, string filter, string sort = null, long? pageLimit = null, string cursor = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListAllLogs`");
            if (string.IsNullOrEmpty(filter))
                throw new ArgumentNullException("`filter` is required for `ListAllLogs`");

            RequestObject<LogCollection> req = new RequestObject<LogCollection>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "ListAllLogs"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

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

            Connector.URL = GetServiceUrl() + "/v1/logs";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListAllLogsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<LogCollection> response = new DetailedResponse<LogCollection>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<LogCollection>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnListAllLogsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<LogCollection>)req).Callback != null)
                ((RequestObject<LogCollection>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete labeled data.
        ///
        /// Deletes all data associated with a specified customer ID. The method has no effect if no data is associated
        /// with the customer ID.
        ///
        /// You associate a customer ID with data by passing the `X-Watson-Metadata` header with a request that passes
        /// data. For more information about personal data and customer IDs, see [Information
        /// security](https://cloud.ibm.com/docs/services/assistant?topic=assistant-information-security#information-security).
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V1", "DeleteUserData"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(customerId))
            {
                req.Parameters["customer_id"] = customerId;
            }

            req.OnResponse = OnDeleteUserDataResponse;

            Connector.URL = GetServiceUrl() + "/v1/user_data";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
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
                Log.Error("AssistantService.OnDeleteUserDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
    }
}