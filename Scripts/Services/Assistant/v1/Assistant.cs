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

using System.Collections.Generic;
using System.Text;
using FullSerializer;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using System;
using MiniJSON;

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    public class Assistant : IWatsonService
    {
        private const string ServiceId = "Assistantv1";
        private fsSerializer _serializer = new fsSerializer();

        private Credentials _credentials = null;
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

        private string _url = "https://gateway.watsonplatform.net/assistant/api";
        /// <summary>
        /// Gets and sets the endpoint URL for the service.
        /// </summary>
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        private string _versionDate;
        /// <summary>
        /// Gets and sets the versionDate of the service.
        /// </summary>
        public string VersionDate
        {
            get { return _versionDate; }
            set { _versionDate = value; }
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
        /// Assistant constructor.
        /// </summary>
        /// <param name="credentials">The service credentials.</param>
        public Assistant(Credentials credentials)
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
                throw new WatsonException("Please provide a username and password or authorization token to use the Assistant service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }

        /// <summary>
        /// Get a response to a user's input.    There is no rate limit for this operation. 
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="request">The message to be sent. This includes the user's input, along with optional intents, entities, and context from the last response. (optional)</param>
        /// <param name="nodesVisitedDetails">Whether to include additional diagnostic information about the dialog nodes that were visited during processing of the message. (optional, default to false)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool Message(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, MessageRequest request = null, bool? nodesVisitedDetails = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            IDictionary<string, string> requestDict = new Dictionary<string, string>();
            if (request.Context != null)
                requestDict.Add("context", Json.Serialize(request.Context));
            if (request.Input != null)
                requestDict.Add("input", Json.Serialize(request.Input));
            if(request.AlternateIntents != null)
                requestDict.Add("alternate_intents", Json.Serialize(request.AlternateIntents));
            if (request.Entities != null)
                requestDict.Add("entities", Json.Serialize(request.Entities));
            if (request.Intents != null)
                requestDict.Add("intents", Json.Serialize(request.Intents));
            if (request.Output != null)
                requestDict.Add("output", Json.Serialize(request.Output));

            int iterator = 0;
            StringBuilder stringBuilder = new StringBuilder("{");
            foreach (KeyValuePair<string, string> property in requestDict)
            {
                string delimeter = iterator < requestDict.Count - 1 ? "," : "";
                stringBuilder.Append(string.Format("\"{0}\": {1}{2}", property.Key, property.Value, delimeter));
                iterator++;
            }
            stringBuilder.Append("}");

            string stringToSend = stringBuilder.ToString();

            MessageRequestObj req = new MessageRequestObj();
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
            req.Headers["Content-Type"] = "application/json";
            req.Send = Encoding.UTF8.GetBytes(stringToSend);
            req.OnResponse = OnMessageResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/message", workspaceId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class MessageRequestObj : RESTConnector.Request
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

        private void OnMessageResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            object result = null;
            string data = "";
            Dictionary<string, object> customData = ((MessageRequestObj)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

            if (resp.Success)
            {
                try
                {
                    //  For deserializing into a generic object
                    data = Encoding.UTF8.GetString(resp.Data);
                    result = Json.Deserialize(data);
                }
                catch (Exception e)
                {
                    Log.Error("Assistant.OnMessageResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((MessageRequestObj)req).SuccessCallback != null)
                    ((MessageRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((MessageRequestObj)req).FailCallback != null)
                    ((MessageRequestObj)req).FailCallback(resp.Error, customData);
            }
        }
        /// <summary>
        /// Create workspace. Create a workspace based on component objects. You must provide workspace components defining the content of the new workspace.    This operation is limited to 30 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="properties">The content of the new workspace.    The maximum size for this data is 50MB. If you need to import a larger workspace, consider importing the workspace without intents and entities and then adding them separately. (optional)</param>
        /// <returns><see cref="Workspace" />Workspace</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool CreateWorkspace(SuccessCallback<Workspace> successCallback, FailCallback failCallback, CreateWorkspace properties = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            
            CreateWorkspaceRequestObj req = new CreateWorkspaceRequestObj();
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
            req.Headers["Content-Type"] = "application/json";
            fsData data = null;
            _serializer.TrySerialize(properties, out data);
            string json = data.ToString().Replace('\"', '"');
            req.Send = Encoding.UTF8.GetBytes(json);
            req.OnResponse = OnCreateWorkspaceResponse;
            
            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/workspaces");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class CreateWorkspaceRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Workspace> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnCreateWorkspaceResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Workspace result = new Workspace();
            fsData data = null;
            Dictionary<string, object> customData = ((CreateWorkspaceRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnCreateWorkspaceResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((CreateWorkspaceRequestObj)req).SuccessCallback != null)
                    ((CreateWorkspaceRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((CreateWorkspaceRequestObj)req).FailCallback != null)
                    ((CreateWorkspaceRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Delete workspace. Delete a workspace from the service instance.    This operation is limited to 30 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool DeleteWorkspace(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            DeleteWorkspaceRequestObj req = new DeleteWorkspaceRequestObj();
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
            req.Delete = true;

            req.OnResponse = OnDeleteWorkspaceResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}", workspaceId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteWorkspaceRequestObj : RESTConnector.Request
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

        private void OnDeleteWorkspaceResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            object result = new object();
            fsData data = null;
            Dictionary<string, object> customData = ((DeleteWorkspaceRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnDeleteWorkspaceResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((DeleteWorkspaceRequestObj)req).SuccessCallback != null)
                    ((DeleteWorkspaceRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteWorkspaceRequestObj)req).FailCallback != null)
                    ((DeleteWorkspaceRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Get information about a workspace. Get information about a workspace, optionally including all workspace content.    With **export**=`false`, this operation is limited to 6000 requests per 5 minutes. With **export**=`true`, the limit is 20 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="export">Whether to include all element content in the returned data. If **export**=`false`, the returned data includes only information about the element itself. If **export**=`true`, all content, including subelements, is included. (optional, default to false)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="WorkspaceExport" />WorkspaceExport</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool GetWorkspace(SuccessCallback<WorkspaceExport> successCallback, FailCallback failCallback, string workspaceId, bool? export = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetWorkspaceRequestObj req = new GetWorkspaceRequestObj();
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
            if (includeAudit != null)
                req.Parameters["include_audit"] = includeAudit;
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetWorkspaceResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}", workspaceId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetWorkspaceRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<WorkspaceExport> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetWorkspaceResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WorkspaceExport result = new WorkspaceExport();
            fsData data = null;
            Dictionary<string, object> customData = ((GetWorkspaceRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnGetWorkspaceResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((GetWorkspaceRequestObj)req).SuccessCallback != null)
                    ((GetWorkspaceRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetWorkspaceRequestObj)req).FailCallback != null)
                    ((GetWorkspaceRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// List workspaces. List the workspaces associated with an Assistant service instance.    This operation is limited to 500 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to 100)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional, default to false)</param>
        /// <param name="sort">The attribute by which returned results will be sorted. To reverse the sort order, prefix the value with a minus sign (`-`). Supported values are `name`, `updated`, and `workspace_id`. (optional)</param>
        /// <param name="cursor">A token identifying the last object from the previous page of results. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="WorkspaceCollection" />WorkspaceCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool ListWorkspaces(SuccessCallback<WorkspaceCollection> successCallback, FailCallback failCallback, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            ListWorkspacesRequestObj req = new ListWorkspacesRequestObj();
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
            if (pageLimit != null)
                req.Parameters["page_limit"] = pageLimit;
            if (includeCount != null)
                req.Parameters["include_count"] = includeCount;
            if (!string.IsNullOrEmpty(sort))
                req.Parameters["sort"] = sort;
            if (!string.IsNullOrEmpty(cursor))
                req.Parameters["cursor"] = cursor;
            if (includeAudit != null)
                req.Parameters["include_audit"] = includeAudit;
            req.OnResponse = OnListWorkspacesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/workspaces");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ListWorkspacesRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<WorkspaceCollection> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnListWorkspacesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WorkspaceCollection result = new WorkspaceCollection();
            fsData data = null;
            Dictionary<string, object> customData = ((ListWorkspacesRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnListWorkspacesResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((ListWorkspacesRequestObj)req).SuccessCallback != null)
                    ((ListWorkspacesRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ListWorkspacesRequestObj)req).FailCallback != null)
                    ((ListWorkspacesRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Update workspace. Update an existing workspace with new or modified data. You must provide component objects defining the content of the updated workspace.    This operation is limited to 30 request per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="properties">Valid data defining the new and updated workspace content.    The maximum size for this data is 50MB. If you need to import a larger amount of workspace data, consider importing components such as intents and entities using separate operations. (optional)</param>
        /// <param name="append">Whether the new data is to be appended to the existing data in the workspace. If **append**=`false`, elements included in the new data completely replace the corresponding existing elements, including all subelements. For example, if the new data includes **entities** and **append**=`false`, all existing entities in the workspace are discarded and replaced with the new entities.    If **append**=`true`, existing elements are preserved, and the new elements are added. If any elements in the new data collide with existing elements, the update request fails. (optional, default to false)</param>
        /// <returns><see cref="Workspace" />Workspace</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool UpdateWorkspace(SuccessCallback<Workspace> successCallback, FailCallback failCallback, string workspaceId, UpdateWorkspace properties = null, bool? append = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            UpdateWorkspaceRequestObj req = new UpdateWorkspaceRequestObj();
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
            req.Headers["Content-Type"] = "application/json";
            fsData data = null;
            _serializer.TrySerialize(properties, out data);
            req.Send = Encoding.UTF8.GetBytes(data.ToString());
            req.OnResponse = OnUpdateWorkspaceResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}", workspaceId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class UpdateWorkspaceRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Workspace> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnUpdateWorkspaceResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Workspace result = new Workspace();
            fsData data = null;
            Dictionary<string, object> customData = ((UpdateWorkspaceRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnUpdateWorkspaceResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((UpdateWorkspaceRequestObj)req).SuccessCallback != null)
                    ((UpdateWorkspaceRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((UpdateWorkspaceRequestObj)req).FailCallback != null)
                    ((UpdateWorkspaceRequestObj)req).FailCallback(resp.Error, customData);
            }
        }
        /// <summary>
        /// Create intent. Create a new intent.    This operation is limited to 2000 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="body">The content of the new intent.</param>
        /// <returns><see cref="Intent" />Intent</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool CreateIntent(SuccessCallback<Intent> successCallback, FailCallback failCallback, string workspaceId, CreateIntent body, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            CreateIntentRequestObj req = new CreateIntentRequestObj();
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
            req.Headers["Content-Type"] = "application/json";
            fsData data = null;
            _serializer.TrySerialize(body, out data);
            req.Send = Encoding.UTF8.GetBytes(data.ToString());
            req.OnResponse = OnCreateIntentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents", workspaceId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class CreateIntentRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Intent> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnCreateIntentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Intent result = new Intent();
            fsData data = null;
            Dictionary<string, object> customData = ((CreateIntentRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnCreateIntentResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((CreateIntentRequestObj)req).SuccessCallback != null)
                    ((CreateIntentRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((CreateIntentRequestObj)req).FailCallback != null)
                    ((CreateIntentRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Delete intent. Delete an intent from a workspace.    This operation is limited to 2000 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The intent name.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool DeleteIntent(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string intent, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            DeleteIntentRequestObj req = new DeleteIntentRequestObj();
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
            req.Delete = true;

            req.OnResponse = OnDeleteIntentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents/{1}", workspaceId, intent));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteIntentRequestObj : RESTConnector.Request
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

        private void OnDeleteIntentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            object result = new object();
            fsData data = null;
            Dictionary<string, object> customData = ((DeleteIntentRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnDeleteIntentResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((DeleteIntentRequestObj)req).SuccessCallback != null)
                    ((DeleteIntentRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteIntentRequestObj)req).FailCallback != null)
                    ((DeleteIntentRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Get intent. Get information about an intent, optionally including all intent content.    With **export**=`false`, this operation is limited to 6000 requests per 5 minutes. With **export**=`true`, the limit is 400 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The intent name.</param>
        /// <param name="export">Whether to include all element content in the returned data. If **export**=`false`, the returned data includes only information about the element itself. If **export**=`true`, all content, including subelements, is included. (optional, default to false)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="IntentExport" />IntentExport</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool GetIntent(SuccessCallback<IntentExport> successCallback, FailCallback failCallback, string workspaceId, string intent, bool? export = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetIntentRequestObj req = new GetIntentRequestObj();
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
            if (includeAudit != null)
                req.Parameters["include_audit"] = includeAudit;
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetIntentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents/{1}", workspaceId, intent));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetIntentRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<IntentExport> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetIntentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            IntentExport result = new IntentExport();
            fsData data = null;
            Dictionary<string, object> customData = ((GetIntentRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnGetIntentResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((GetIntentRequestObj)req).SuccessCallback != null)
                    ((GetIntentRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetIntentRequestObj)req).FailCallback != null)
                    ((GetIntentRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// List intents. List the intents for a workspace.    With **export**=`false`, this operation is limited to 2000 requests per 30 minutes. With **export**=`true`, the limit is 400 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="export">Whether to include all element content in the returned data. If **export**=`false`, the returned data includes only information about the element itself. If **export**=`true`, all content, including subelements, is included. (optional, default to false)</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to 100)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional, default to false)</param>
        /// <param name="sort">The attribute by which returned results will be sorted. To reverse the sort order, prefix the value with a minus sign (`-`). Supported values are `name`, `updated`, and `workspace_id`. (optional)</param>
        /// <param name="cursor">A token identifying the last object from the previous page of results. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="IntentCollection" />IntentCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool ListIntents(SuccessCallback<IntentCollection> successCallback, FailCallback failCallback, string workspaceId, bool? export = null, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            ListIntentsRequestObj req = new ListIntentsRequestObj();
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
            if (export != null)
                req.Parameters["export"] = export;
            if (pageLimit != null)
                req.Parameters["page_limit"] = pageLimit;
            if (includeCount != null)
                req.Parameters["include_count"] = includeCount;
            if (!string.IsNullOrEmpty(sort))
                req.Parameters["sort"] = sort;
            if (!string.IsNullOrEmpty(cursor))
                req.Parameters["cursor"] = cursor;
            if (includeAudit != null)
                req.Parameters["include_audit"] = includeAudit;
            req.OnResponse = OnListIntentsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents", workspaceId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ListIntentsRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<IntentCollection> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnListIntentsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            IntentCollection result = new IntentCollection();
            fsData data = null;
            Dictionary<string, object> customData = ((ListIntentsRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnListIntentsResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((ListIntentsRequestObj)req).SuccessCallback != null)
                    ((ListIntentsRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ListIntentsRequestObj)req).FailCallback != null)
                    ((ListIntentsRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Update intent. Update an existing intent with new or modified data. You must provide component objects defining the content of the updated intent.    This operation is limited to 2000 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The intent name.</param>
        /// <param name="body">The updated content of the intent.    Any elements included in the new data will completely replace the equivalent existing elements, including all subelements. (Previously existing subelements are not retained unless they are also included in the new data.) For example, if you update the user input examples for an intent, the previously existing examples are discarded and replaced with the new examples specified in the update.</param>
        /// <returns><see cref="Intent" />Intent</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool UpdateIntent(SuccessCallback<Intent> successCallback, FailCallback failCallback, string workspaceId, string intent, UpdateIntent body, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            UpdateIntentRequestObj req = new UpdateIntentRequestObj();
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
            req.Headers["Content-Type"] = "application/json";
            fsData data = null;
            _serializer.TrySerialize(body, out data);
            req.Send = Encoding.UTF8.GetBytes(data.ToString());
            req.OnResponse = OnUpdateIntentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents/{1}", workspaceId, intent));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class UpdateIntentRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Intent> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnUpdateIntentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Intent result = new Intent();
            fsData data = null;
            Dictionary<string, object> customData = ((UpdateIntentRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnUpdateIntentResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((UpdateIntentRequestObj)req).SuccessCallback != null)
                    ((UpdateIntentRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((UpdateIntentRequestObj)req).FailCallback != null)
                    ((UpdateIntentRequestObj)req).FailCallback(resp.Error, customData);
            }
        }
        /// <summary>
        /// Create user input example. Add a new user input example to an intent.    This operation is limited to 1000 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The intent name.</param>
        /// <param name="body">The content of the new user input example.</param>
        /// <returns><see cref="Example" />Example</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool CreateExample(SuccessCallback<Example> successCallback, FailCallback failCallback, string workspaceId, string intent, CreateExample body, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            CreateExampleRequestObj req = new CreateExampleRequestObj();
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
            req.Headers["Content-Type"] = "application/json";
            fsData data = null;
            _serializer.TrySerialize(body, out data);
            req.Send = Encoding.UTF8.GetBytes(data.ToString());
            req.OnResponse = OnCreateExampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents/{1}/examples", workspaceId, intent));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class CreateExampleRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Example> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnCreateExampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Example result = new Example();
            fsData data = null;
            Dictionary<string, object> customData = ((CreateExampleRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnCreateExampleResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((CreateExampleRequestObj)req).SuccessCallback != null)
                    ((CreateExampleRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((CreateExampleRequestObj)req).FailCallback != null)
                    ((CreateExampleRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Delete user input example. Delete a user input example from an intent.    This operation is limited to 1000 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The intent name.</param>
        /// <param name="text">The text of the user input example.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool DeleteExample(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string intent, string text, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            DeleteExampleRequestObj req = new DeleteExampleRequestObj();
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
            req.Delete = true;

            req.OnResponse = OnDeleteExampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents/{1}/examples/{2}", workspaceId, intent, text));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteExampleRequestObj : RESTConnector.Request
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

        private void OnDeleteExampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            object result = new object();
            fsData data = null;
            Dictionary<string, object> customData = ((DeleteExampleRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnDeleteExampleResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((DeleteExampleRequestObj)req).SuccessCallback != null)
                    ((DeleteExampleRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteExampleRequestObj)req).FailCallback != null)
                    ((DeleteExampleRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Get user input example. Get information about a user input example.    This operation is limited to 6000 requests per 5 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The intent name.</param>
        /// <param name="text">The text of the user input example.</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="Example" />Example</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool GetExample(SuccessCallback<Example> successCallback, FailCallback failCallback, string workspaceId, string intent, string text, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetExampleRequestObj req = new GetExampleRequestObj();
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
            if (includeAudit != null)
                req.Parameters["include_audit"] = includeAudit;
            req.OnResponse = OnGetExampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents/{1}/examples/{2}", workspaceId, intent, text));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetExampleRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Example> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetExampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Example result = new Example();
            fsData data = null;
            Dictionary<string, object> customData = ((GetExampleRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnGetExampleResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((GetExampleRequestObj)req).SuccessCallback != null)
                    ((GetExampleRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetExampleRequestObj)req).FailCallback != null)
                    ((GetExampleRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// List user input examples. List the user input examples for an intent.    This operation is limited to 2500 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The intent name.</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to 100)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional, default to false)</param>
        /// <param name="sort">The attribute by which returned results will be sorted. To reverse the sort order, prefix the value with a minus sign (`-`). Supported values are `name`, `updated`, and `workspace_id`. (optional)</param>
        /// <param name="cursor">A token identifying the last object from the previous page of results. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="ExampleCollection" />ExampleCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool ListExamples(SuccessCallback<ExampleCollection> successCallback, FailCallback failCallback, string workspaceId, string intent, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            ListExamplesRequestObj req = new ListExamplesRequestObj();
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
            if (pageLimit != null)
                req.Parameters["page_limit"] = pageLimit;
            if (includeCount != null)
                req.Parameters["include_count"] = includeCount;
            if (!string.IsNullOrEmpty(sort))
                req.Parameters["sort"] = sort;
            if (!string.IsNullOrEmpty(cursor))
                req.Parameters["cursor"] = cursor;
            if (includeAudit != null)
                req.Parameters["include_audit"] = includeAudit;
            req.OnResponse = OnListExamplesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents/{1}/examples", workspaceId, intent));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ListExamplesRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<ExampleCollection> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnListExamplesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ExampleCollection result = new ExampleCollection();
            fsData data = null;
            Dictionary<string, object> customData = ((ListExamplesRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnListExamplesResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((ListExamplesRequestObj)req).SuccessCallback != null)
                    ((ListExamplesRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ListExamplesRequestObj)req).FailCallback != null)
                    ((ListExamplesRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Update user input example. Update the text of a user input example.    This operation is limited to 1000 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="intent">The intent name.</param>
        /// <param name="text">The text of the user input example.</param>
        /// <param name="body">The new text of the user input example.</param>
        /// <returns><see cref="Example" />Example</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool UpdateExample(SuccessCallback<Example> successCallback, FailCallback failCallback, string workspaceId, string intent, string text, UpdateExample body, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            UpdateExampleRequestObj req = new UpdateExampleRequestObj();
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
            req.Headers["Content-Type"] = "application/json";
            fsData data = null;
            _serializer.TrySerialize(body, out data);
            req.Send = Encoding.UTF8.GetBytes(data.ToString());
            req.OnResponse = OnUpdateExampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/intents/{1}/examples/{2}", workspaceId, intent, text));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class UpdateExampleRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Example> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnUpdateExampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Example result = new Example();
            fsData data = null;
            Dictionary<string, object> customData = ((UpdateExampleRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnUpdateExampleResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((UpdateExampleRequestObj)req).SuccessCallback != null)
                    ((UpdateExampleRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((UpdateExampleRequestObj)req).FailCallback != null)
                    ((UpdateExampleRequestObj)req).FailCallback(resp.Error, customData);
            }
        }
        /// <summary>
        /// Create counterexample. Add a new counterexample to a workspace. Counterexamples are examples that have been marked as irrelevant input.    This operation is limited to 1000 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="body">The content of the new counterexample.</param>
        /// <returns><see cref="Counterexample" />Counterexample</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool CreateCounterexample(SuccessCallback<Counterexample> successCallback, FailCallback failCallback, string workspaceId, CreateCounterexample body, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            CreateCounterexampleRequestObj req = new CreateCounterexampleRequestObj();
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
            req.Headers["Content-Type"] = "application/json";
            fsData data = null;
            _serializer.TrySerialize(body, out data);
            req.Send = Encoding.UTF8.GetBytes(data.ToString());
            req.OnResponse = OnCreateCounterexampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/counterexamples", workspaceId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class CreateCounterexampleRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Counterexample> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnCreateCounterexampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Counterexample result = new Counterexample();
            fsData data = null;
            Dictionary<string, object> customData = ((CreateCounterexampleRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnCreateCounterexampleResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((CreateCounterexampleRequestObj)req).SuccessCallback != null)
                    ((CreateCounterexampleRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((CreateCounterexampleRequestObj)req).FailCallback != null)
                    ((CreateCounterexampleRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Delete counterexample. Delete a counterexample from a workspace. Counterexamples are examples that have been marked as irrelevant input.    This operation is limited to 1000 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="text">The text of a user input counterexample (for example, `What are you wearing?`).</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool DeleteCounterexample(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string text, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            DeleteCounterexampleRequestObj req = new DeleteCounterexampleRequestObj();
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
            req.Delete = true;

            req.OnResponse = OnDeleteCounterexampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/counterexamples/{1}", workspaceId, text));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteCounterexampleRequestObj : RESTConnector.Request
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

        private void OnDeleteCounterexampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            object result = new object();
            fsData data = null;
            Dictionary<string, object> customData = ((DeleteCounterexampleRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnDeleteCounterexampleResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((DeleteCounterexampleRequestObj)req).SuccessCallback != null)
                    ((DeleteCounterexampleRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteCounterexampleRequestObj)req).FailCallback != null)
                    ((DeleteCounterexampleRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Get counterexample. Get information about a counterexample. Counterexamples are examples that have been marked as irrelevant input.    This operation is limited to 6000 requests per 5 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="text">The text of a user input counterexample (for example, `What are you wearing?`).</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="Counterexample" />Counterexample</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool GetCounterexample(SuccessCallback<Counterexample> successCallback, FailCallback failCallback, string workspaceId, string text, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetCounterexampleRequestObj req = new GetCounterexampleRequestObj();
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
            if (includeAudit != null)
                req.Parameters["include_audit"] = includeAudit;
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetCounterexampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/counterexamples/{1}", workspaceId, text));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetCounterexampleRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Counterexample> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetCounterexampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Counterexample result = new Counterexample();
            fsData data = null;
            Dictionary<string, object> customData = ((GetCounterexampleRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnGetCounterexampleResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((GetCounterexampleRequestObj)req).SuccessCallback != null)
                    ((GetCounterexampleRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetCounterexampleRequestObj)req).FailCallback != null)
                    ((GetCounterexampleRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// List counterexamples. List the counterexamples for a workspace. Counterexamples are examples that have been marked as irrelevant input.    This operation is limited to 2500 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to 100)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional, default to false)</param>
        /// <param name="sort">The attribute by which returned results will be sorted. To reverse the sort order, prefix the value with a minus sign (`-`). Supported values are `name`, `updated`, and `workspace_id`. (optional)</param>
        /// <param name="cursor">A token identifying the last object from the previous page of results. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="CounterexampleCollection" />CounterexampleCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool ListCounterexamples(SuccessCallback<CounterexampleCollection> successCallback, FailCallback failCallback, string workspaceId, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            ListCounterexamplesRequestObj req = new ListCounterexamplesRequestObj();
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
            if (pageLimit != null)
                req.Parameters["page_limit"] = pageLimit;
            if (includeCount != null)
                req.Parameters["include_count"] = includeCount;
            if (!string.IsNullOrEmpty(sort))
                req.Parameters["sort"] = sort;
            if (!string.IsNullOrEmpty(cursor))
                req.Parameters["cursor"] = cursor;
            if (includeAudit != null)
                req.Parameters["include_audit"] = includeAudit;
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListCounterexamplesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/counterexamples", workspaceId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ListCounterexamplesRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<CounterexampleCollection> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnListCounterexamplesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            CounterexampleCollection result = new CounterexampleCollection();
            fsData data = null;
            Dictionary<string, object> customData = ((ListCounterexamplesRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnListCounterexamplesResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((ListCounterexamplesRequestObj)req).SuccessCallback != null)
                    ((ListCounterexamplesRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ListCounterexamplesRequestObj)req).FailCallback != null)
                    ((ListCounterexamplesRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Update counterexample. Update the text of a counterexample. Counterexamples are examples that have been marked as irrelevant input.    This operation is limited to 1000 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="text">The text of a user input counterexample (for example, `What are you wearing?`).</param>
        /// <param name="body">The text of the counterexample.</param>
        /// <returns><see cref="Counterexample" />Counterexample</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool UpdateCounterexample(SuccessCallback<Counterexample> successCallback, FailCallback failCallback, string workspaceId, string text, UpdateCounterexample body, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            UpdateCounterexampleRequestObj req = new UpdateCounterexampleRequestObj();
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
            req.Headers["Content-Type"] = "application/json";
            fsData data = null;
            _serializer.TrySerialize(body, out data);
            req.Send = Encoding.UTF8.GetBytes(data.ToString());
            req.OnResponse = OnUpdateCounterexampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/counterexamples/{1}", workspaceId, text));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class UpdateCounterexampleRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Counterexample> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnUpdateCounterexampleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Counterexample result = new Counterexample();
            fsData data = null;
            Dictionary<string, object> customData = ((UpdateCounterexampleRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnUpdateCounterexampleResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((UpdateCounterexampleRequestObj)req).SuccessCallback != null)
                    ((UpdateCounterexampleRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((UpdateCounterexampleRequestObj)req).FailCallback != null)
                    ((UpdateCounterexampleRequestObj)req).FailCallback(resp.Error, customData);
            }
        }
        /// <summary>
        /// Create entity. Create a new entity.    This operation is limited to 1000 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="properties">The content of the new entity.</param>
        /// <returns><see cref="Entity" />Entity</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool CreateEntity(SuccessCallback<Entity> successCallback, FailCallback failCallback, string workspaceId, CreateEntity properties, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            CreateEntityRequestObj req = new CreateEntityRequestObj();
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
            req.Headers["Content-Type"] = "application/json";
            fsData data = null;
            _serializer.TrySerialize(properties, out data);
            req.Send = Encoding.UTF8.GetBytes(data.ToString());
            req.OnResponse = OnCreateEntityResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities", workspaceId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class CreateEntityRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Entity> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnCreateEntityResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Entity result = new Entity();
            fsData data = null;
            Dictionary<string, object> customData = ((CreateEntityRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnCreateEntityResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((CreateEntityRequestObj)req).SuccessCallback != null)
                    ((CreateEntityRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((CreateEntityRequestObj)req).FailCallback != null)
                    ((CreateEntityRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Delete entity. Delete an entity from a workspace.    This operation is limited to 1000 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool DeleteEntity(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string entity, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            DeleteEntityRequestObj req = new DeleteEntityRequestObj();
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
            req.Delete = true;

            req.OnResponse = OnDeleteEntityResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}", workspaceId, entity));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteEntityRequestObj : RESTConnector.Request
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

        private void OnDeleteEntityResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            object result = new object();
            fsData data = null;
            Dictionary<string, object> customData = ((DeleteEntityRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnDeleteEntityResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((DeleteEntityRequestObj)req).SuccessCallback != null)
                    ((DeleteEntityRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteEntityRequestObj)req).FailCallback != null)
                    ((DeleteEntityRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Get entity. Get information about an entity, optionally including all entity content.    With **export**=`false`, this operation is limited to 6000 requests per 5 minutes. With **export**=`true`, the limit is 200 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="export">Whether to include all element content in the returned data. If **export**=`false`, the returned data includes only information about the element itself. If **export**=`true`, all content, including subelements, is included. (optional, default to false)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="EntityExport" />EntityExport</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool GetEntity(SuccessCallback<EntityExport> successCallback, FailCallback failCallback, string workspaceId, string entity, bool? export = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetEntityRequestObj req = new GetEntityRequestObj();
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
            if (includeAudit != null)
                req.Parameters["include_audit"] = includeAudit;
            if (export != null)
                req.Parameters["export"] = export;
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetEntityResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}", workspaceId, entity));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetEntityRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<EntityExport> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetEntityResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            EntityExport result = new EntityExport();
            fsData data = null;
            Dictionary<string, object> customData = ((GetEntityRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnGetEntityResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((GetEntityRequestObj)req).SuccessCallback != null)
                    ((GetEntityRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetEntityRequestObj)req).FailCallback != null)
                    ((GetEntityRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// List entities. List the entities for a workspace.    With **export**=`false`, this operation is limited to 1000 requests per 30 minutes. With **export**=`true`, the limit is 200 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="export">Whether to include all element content in the returned data. If **export**=`false`, the returned data includes only information about the element itself. If **export**=`true`, all content, including subelements, is included. (optional, default to false)</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to 100)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional, default to false)</param>
        /// <param name="sort">The attribute by which returned results will be sorted. To reverse the sort order, prefix the value with a minus sign (`-`). Supported values are `name`, `updated`, and `workspace_id`. (optional)</param>
        /// <param name="cursor">A token identifying the last object from the previous page of results. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="EntityCollection" />EntityCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool ListEntities(SuccessCallback<EntityCollection> successCallback, FailCallback failCallback, string workspaceId, bool? export = null, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            ListEntitiesRequestObj req = new ListEntitiesRequestObj();
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
            if (export != null)
                req.Parameters["export"] = export;
            if (pageLimit != null)
                req.Parameters["page_limit"] = pageLimit;
            if (includeCount != null)
                req.Parameters["include_count"] = includeCount;
            if (!string.IsNullOrEmpty(sort))
                req.Parameters["sort"] = sort;
            if (!string.IsNullOrEmpty(cursor))
                req.Parameters["cursor"] = cursor;
            if (includeAudit != null)
                req.Parameters["include_audit"] = includeAudit;
            req.OnResponse = OnListEntitiesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities", workspaceId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ListEntitiesRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<EntityCollection> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnListEntitiesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            EntityCollection result = new EntityCollection();
            fsData data = null;
            Dictionary<string, object> customData = ((ListEntitiesRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnListEntitiesResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((ListEntitiesRequestObj)req).SuccessCallback != null)
                    ((ListEntitiesRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ListEntitiesRequestObj)req).FailCallback != null)
                    ((ListEntitiesRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Update entity. Update an existing entity with new or modified data. You must provide component objects defining the content of the updated entity.    This operation is limited to 1000 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="properties">The updated content of the entity. Any elements included in the new data will completely replace the equivalent existing elements, including all subelements. (Previously existing subelements are not retained unless they are also included in the new data.) For example, if you update the values for an entity, the previously existing values are discarded and replaced with the new values specified in the update.</param>
        /// <returns><see cref="Entity" />Entity</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool UpdateEntity(SuccessCallback<Entity> successCallback, FailCallback failCallback, string workspaceId, string entity, UpdateEntity properties, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            UpdateEntityRequestObj req = new UpdateEntityRequestObj();
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
            req.Headers["Content-Type"] = "application/json";
            fsData data = null;
            _serializer.TrySerialize(properties, out data);
            req.Send = Encoding.UTF8.GetBytes(data.ToString());
            req.OnResponse = OnUpdateEntityResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}", workspaceId, entity));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class UpdateEntityRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Entity> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnUpdateEntityResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Entity result = new Entity();
            fsData data = null;
            Dictionary<string, object> customData = ((UpdateEntityRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnUpdateEntityResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((UpdateEntityRequestObj)req).SuccessCallback != null)
                    ((UpdateEntityRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((UpdateEntityRequestObj)req).FailCallback != null)
                    ((UpdateEntityRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// List entity mentions.
        ///
        /// List mentions for a contextual entity. An entity mention is an occurrence of a contextual entity in the
        /// context of an intent user input example.
        ///
        /// This operation is limited to 200 requests per 30 minutes. For more information, see **Rate limiting**.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
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
        public bool ListMentions(SuccessCallback<EntityMentionCollection> successCallback, FailCallback failCallback, string workspaceId, string entity, bool? export = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            ListMentionsRequestObj req = new ListMentionsRequestObj();
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
            if (export != null)
                req.Parameters["export"] = export;
            if (includeAudit != null)
                req.Parameters["include_audit"] = includeAudit;
            req.OnResponse = OnListMentionsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/mentions", workspaceId, entity));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ListMentionsRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<EntityMentionCollection> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnListMentionsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            EntityMentionCollection result = new EntityMentionCollection();
            fsData data = null;
            Dictionary<string, object> customData = ((ListMentionsRequestObj)req).CustomData;

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
                    Log.Error("Assistant.OnListMentionsResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((ListMentionsRequestObj)req).SuccessCallback != null)
                    ((ListMentionsRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ListMentionsRequestObj)req).FailCallback != null)
                    ((ListMentionsRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Add entity value. Create a new value for an entity.    This operation is limited to 1000 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="properties">The new entity value.</param>
        /// <returns><see cref="Value" />Value</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool CreateValue(SuccessCallback<Value> successCallback, FailCallback failCallback, string workspaceId, string entity, CreateValue properties, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            CreateValueRequestObj req = new CreateValueRequestObj();
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
            req.Headers["Content-Type"] = "application/json";
            fsData data = null;
            _serializer.TrySerialize(properties, out data);
            req.Send = Encoding.UTF8.GetBytes(data.ToString());
            req.OnResponse = OnCreateValueResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values", workspaceId, entity));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class CreateValueRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Value> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnCreateValueResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Value result = new Value();
            fsData data = null;
            Dictionary<string, object> customData = ((CreateValueRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnCreateValueResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((CreateValueRequestObj)req).SuccessCallback != null)
                    ((CreateValueRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((CreateValueRequestObj)req).FailCallback != null)
                    ((CreateValueRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Delete entity value. Delete a value from an entity.    This operation is limited to 1000 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool DeleteValue(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            DeleteValueRequestObj req = new DeleteValueRequestObj();
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
            req.Delete = true;

            req.OnResponse = OnDeleteValueResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}", workspaceId, entity, value));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteValueRequestObj : RESTConnector.Request
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

        private void OnDeleteValueResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            object result = new object();
            fsData data = null;
            Dictionary<string, object> customData = ((DeleteValueRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnDeleteValueResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((DeleteValueRequestObj)req).SuccessCallback != null)
                    ((DeleteValueRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteValueRequestObj)req).FailCallback != null)
                    ((DeleteValueRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Get entity value. Get information about an entity value.    This operation is limited to 6000 requests per 5 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="export">Whether to include all element content in the returned data. If **export**=`false`, the returned data includes only information about the element itself. If **export**=`true`, all content, including subelements, is included. (optional, default to false)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="ValueExport" />ValueExport</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool GetValue(SuccessCallback<ValueExport> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, bool? export = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetValueRequestObj req = new GetValueRequestObj();
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
            if (export != null)
                req.Parameters["export"] = export;
            if (includeAudit != null)
                req.Parameters["include_audit"] = includeAudit;
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetValueResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}", workspaceId, entity, value));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetValueRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<ValueExport> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetValueResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ValueExport result = new ValueExport();
            fsData data = null;
            Dictionary<string, object> customData = ((GetValueRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnGetValueResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((GetValueRequestObj)req).SuccessCallback != null)
                    ((GetValueRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetValueRequestObj)req).FailCallback != null)
                    ((GetValueRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// List entity values. List the values for an entity.    This operation is limited to 2500 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="export">Whether to include all element content in the returned data. If **export**=`false`, the returned data includes only information about the element itself. If **export**=`true`, all content, including subelements, is included. (optional, default to false)</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to 100)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional, default to false)</param>
        /// <param name="sort">The attribute by which returned results will be sorted. To reverse the sort order, prefix the value with a minus sign (`-`). Supported values are `name`, `updated`, and `workspace_id`. (optional)</param>
        /// <param name="cursor">A token identifying the last object from the previous page of results. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="ValueCollection" />ValueCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool ListValues(SuccessCallback<ValueCollection> successCallback, FailCallback failCallback, string workspaceId, string entity, bool? export = null, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            ListValuesRequestObj req = new ListValuesRequestObj();
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
            if (export != null)
                req.Parameters["export"] = export;
            if (pageLimit != null)
                req.Parameters["page_limit"] = pageLimit;
            if (includeCount != null)
                req.Parameters["include_count"] = includeCount;
            if (!string.IsNullOrEmpty(sort))
                req.Parameters["sort"] = sort;
            if (!string.IsNullOrEmpty(cursor))
                req.Parameters["cursor"] = cursor;
            if (includeAudit != null)
                req.Parameters["include_audit"] = includeAudit;
            req.OnResponse = OnListValuesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values", workspaceId, entity));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ListValuesRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<ValueCollection> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnListValuesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ValueCollection result = new ValueCollection();
            fsData data = null;
            Dictionary<string, object> customData = ((ListValuesRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnListValuesResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((ListValuesRequestObj)req).SuccessCallback != null)
                    ((ListValuesRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ListValuesRequestObj)req).FailCallback != null)
                    ((ListValuesRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Update entity value. Update an existing entity value with new or modified data. You must provide component objects defining the content of the updated entity value.    This operation is limited to 1000 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="properties">The updated content of the entity value.    Any elements included in the new data will completely replace the equivalent existing elements, including all subelements. (Previously existing subelements are not retained unless they are also included in the new data.) For example, if you update the synonyms for an entity value, the previously existing synonyms are discarded and replaced with the new synonyms specified in the update.</param>
        /// <returns><see cref="Value" />Value</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool UpdateValue(SuccessCallback<Value> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, UpdateValue properties, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            UpdateValueRequestObj req = new UpdateValueRequestObj();
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
            req.Headers["Content-Type"] = "application/json";
            fsData data = null;
            _serializer.TrySerialize(properties, out data);
            req.Send = Encoding.UTF8.GetBytes(data.ToString());
            req.OnResponse = OnUpdateValueResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}", workspaceId, entity, value));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class UpdateValueRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Value> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnUpdateValueResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Value result = new Value();
            fsData data = null;
            Dictionary<string, object> customData = ((UpdateValueRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnUpdateValueResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((UpdateValueRequestObj)req).SuccessCallback != null)
                    ((UpdateValueRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((UpdateValueRequestObj)req).FailCallback != null)
                    ((UpdateValueRequestObj)req).FailCallback(resp.Error, customData);
            }
        }
        /// <summary>
        /// Add entity value synonym. Add a new synonym to an entity value.    This operation is limited to 1000 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="body">The new synonym.</param>
        /// <returns><see cref="Synonym" />Synonym</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool CreateSynonym(SuccessCallback<Synonym> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, CreateSynonym body, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            CreateSynonymRequestObj req = new CreateSynonymRequestObj();
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
            req.Headers["Content-Type"] = "application/json";
            fsData data = null;
            _serializer.TrySerialize(body, out data);
            req.Send = Encoding.UTF8.GetBytes(data.ToString());
            req.OnResponse = OnCreateSynonymResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}/synonyms", workspaceId, entity, value));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class CreateSynonymRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Synonym> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnCreateSynonymResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Synonym result = new Synonym();
            fsData data = null;
            Dictionary<string, object> customData = ((CreateSynonymRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnCreateSynonymResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((CreateSynonymRequestObj)req).SuccessCallback != null)
                    ((CreateSynonymRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((CreateSynonymRequestObj)req).FailCallback != null)
                    ((CreateSynonymRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Delete entity value synonym. Delete a synonym from an entity value.    This operation is limited to 1000 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="synonym">The text of the synonym.</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool DeleteSynonym(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, string synonym, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            DeleteSynonymRequestObj req = new DeleteSynonymRequestObj();
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
            req.Delete = true;

            req.OnResponse = OnDeleteSynonymResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}/synonyms/{3}", workspaceId, entity, value, synonym));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteSynonymRequestObj : RESTConnector.Request
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

        private void OnDeleteSynonymResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            object result = new object();
            fsData data = null;
            Dictionary<string, object> customData = ((DeleteSynonymRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnDeleteSynonymResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((DeleteSynonymRequestObj)req).SuccessCallback != null)
                    ((DeleteSynonymRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteSynonymRequestObj)req).FailCallback != null)
                    ((DeleteSynonymRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Get entity value synonym. Get information about a synonym of an entity value.    This operation is limited to 6000 requests per 5 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="synonym">The text of the synonym.</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="Synonym" />Synonym</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool GetSynonym(SuccessCallback<Synonym> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, string synonym, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetSynonymRequestObj req = new GetSynonymRequestObj();
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
            if (includeAudit != null)
                req.Parameters["include_audit"] = includeAudit;
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetSynonymResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}/synonyms/{3}", workspaceId, entity, value, synonym));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetSynonymRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Synonym> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetSynonymResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Synonym result = new Synonym();
            fsData data = null;
            Dictionary<string, object> customData = ((GetSynonymRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnGetSynonymResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((GetSynonymRequestObj)req).SuccessCallback != null)
                    ((GetSynonymRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetSynonymRequestObj)req).FailCallback != null)
                    ((GetSynonymRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// List entity value synonyms. List the synonyms for an entity value.    This operation is limited to 2500 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to 100)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional, default to false)</param>
        /// <param name="sort">The attribute by which returned results will be sorted. To reverse the sort order, prefix the value with a minus sign (`-`). Supported values are `name`, `updated`, and `workspace_id`. (optional)</param>
        /// <param name="cursor">A token identifying the last object from the previous page of results. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="SynonymCollection" />SynonymCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool ListSynonyms(SuccessCallback<SynonymCollection> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            ListSynonymsRequestObj req = new ListSynonymsRequestObj();
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
            if (pageLimit != null)
                req.Parameters["page_limit"] = pageLimit;
            if (includeCount != null)
                req.Parameters["include_count"] = includeCount;
            if (!string.IsNullOrEmpty(sort))
                req.Parameters["sort"] = sort;
            if (!string.IsNullOrEmpty(cursor))
                req.Parameters["cursor"] = cursor;
            if (includeAudit != null)
                req.Parameters["include_audit"] = includeAudit;
            req.OnResponse = OnListSynonymsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}/synonyms", workspaceId, entity, value));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ListSynonymsRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<SynonymCollection> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnListSynonymsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            SynonymCollection result = new SynonymCollection();
            fsData data = null;
            Dictionary<string, object> customData = ((ListSynonymsRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnListSynonymsResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((ListSynonymsRequestObj)req).SuccessCallback != null)
                    ((ListSynonymsRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ListSynonymsRequestObj)req).FailCallback != null)
                    ((ListSynonymsRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Update entity value synonym. Update an existing entity value synonym with new text.    This operation is limited to 1000 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="synonym">The text of the synonym.</param>
        /// <param name="body">The updated entity value synonym.</param>
        /// <returns><see cref="Synonym" />Synonym</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool UpdateSynonym(SuccessCallback<Synonym> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, string synonym, UpdateSynonym body, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            UpdateSynonymRequestObj req = new UpdateSynonymRequestObj();
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
            req.Headers["Content-Type"] = "application/json";
            fsData data = null;
            _serializer.TrySerialize(body, out data);
            req.Send = Encoding.UTF8.GetBytes(data.ToString());
            req.OnResponse = OnUpdateSynonymResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/entities/{1}/values/{2}/synonyms/{3}", workspaceId, entity, value, synonym));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class UpdateSynonymRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Synonym> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnUpdateSynonymResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Synonym result = new Synonym();
            fsData data = null;
            Dictionary<string, object> customData = ((UpdateSynonymRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnUpdateSynonymResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((UpdateSynonymRequestObj)req).SuccessCallback != null)
                    ((UpdateSynonymRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((UpdateSynonymRequestObj)req).FailCallback != null)
                    ((UpdateSynonymRequestObj)req).FailCallback(resp.Error, customData);
            }
        }
        /// <summary>
        /// Create dialog node. Create a new dialog node.    This operation is limited to 500 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="properties">A CreateDialogNode object defining the content of the new dialog node.</param>
        /// <returns><see cref="DialogNode" />DialogNode</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool CreateDialogNode(SuccessCallback<DialogNode> successCallback, FailCallback failCallback, string workspaceId, CreateDialogNode properties, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            CreateDialogNodeRequestObj req = new CreateDialogNodeRequestObj();
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
            req.Headers["Content-Type"] = "application/json";
            fsData data = null;
            _serializer.TrySerialize(properties, out data);
            req.Send = Encoding.UTF8.GetBytes(data.ToString());
            req.OnResponse = OnCreateDialogNodeResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/dialog_nodes", workspaceId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class CreateDialogNodeRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<DialogNode> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnCreateDialogNodeResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DialogNode result = new DialogNode();
            fsData data = null;
            Dictionary<string, object> customData = ((CreateDialogNodeRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnCreateDialogNodeResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((CreateDialogNodeRequestObj)req).SuccessCallback != null)
                    ((CreateDialogNodeRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((CreateDialogNodeRequestObj)req).FailCallback != null)
                    ((CreateDialogNodeRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Delete dialog node. Delete a dialog node from a workspace.    This operation is limited to 500 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="dialogNode">The dialog node ID (for example, `get_order`).</param>
        /// <returns><see cref="object" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool DeleteDialogNode(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string dialogNode, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            DeleteDialogNodeRequestObj req = new DeleteDialogNodeRequestObj();
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
            req.Delete = true;

            req.OnResponse = OnDeleteDialogNodeResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/dialog_nodes/{1}", workspaceId, dialogNode));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteDialogNodeRequestObj : RESTConnector.Request
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

        private void OnDeleteDialogNodeResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            object result = new object();
            fsData data = null;
            Dictionary<string, object> customData = ((DeleteDialogNodeRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnDeleteDialogNodeResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((DeleteDialogNodeRequestObj)req).SuccessCallback != null)
                    ((DeleteDialogNodeRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteDialogNodeRequestObj)req).FailCallback != null)
                    ((DeleteDialogNodeRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Get dialog node. Get information about a dialog node.    This operation is limited to 6000 requests per 5 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="dialogNode">The dialog node ID (for example, `get_order`).</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="DialogNode" />DialogNode</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool GetDialogNode(SuccessCallback<DialogNode> successCallback, FailCallback failCallback, string workspaceId, string dialogNode, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            GetDialogNodeRequestObj req = new GetDialogNodeRequestObj();
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
            if (includeAudit != null)
                req.Parameters["include_audit"] = includeAudit;
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetDialogNodeResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/dialog_nodes/{1}", workspaceId, dialogNode));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetDialogNodeRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<DialogNode> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetDialogNodeResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DialogNode result = new DialogNode();
            fsData data = null;
            Dictionary<string, object> customData = ((GetDialogNodeRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnGetDialogNodeResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((GetDialogNodeRequestObj)req).SuccessCallback != null)
                    ((GetDialogNodeRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetDialogNodeRequestObj)req).FailCallback != null)
                    ((GetDialogNodeRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// List dialog nodes. List the dialog nodes for a workspace.    This operation is limited to 2500 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to 100)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional, default to false)</param>
        /// <param name="sort">The attribute by which returned results will be sorted. To reverse the sort order, prefix the value with a minus sign (`-`). Supported values are `name`, `updated`, and `workspace_id`. (optional)</param>
        /// <param name="cursor">A token identifying the last object from the previous page of results. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="DialogNodeCollection" />DialogNodeCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool ListDialogNodes(SuccessCallback<DialogNodeCollection> successCallback, FailCallback failCallback, string workspaceId, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            ListDialogNodesRequestObj req = new ListDialogNodesRequestObj();
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
            if (pageLimit != null)
                req.Parameters["page_limit"] = pageLimit;
            if (includeCount != null)
                req.Parameters["include_count"] = includeCount;
            if (!string.IsNullOrEmpty(sort))
                req.Parameters["sort"] = sort;
            if (!string.IsNullOrEmpty(cursor))
                req.Parameters["cursor"] = cursor;
            if (includeAudit != null)
                req.Parameters["include_audit"] = includeAudit;
            req.OnResponse = OnListDialogNodesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/dialog_nodes", workspaceId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ListDialogNodesRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<DialogNodeCollection> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnListDialogNodesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DialogNodeCollection result = new DialogNodeCollection();
            fsData data = null;
            Dictionary<string, object> customData = ((ListDialogNodesRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnListDialogNodesResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((ListDialogNodesRequestObj)req).SuccessCallback != null)
                    ((ListDialogNodesRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ListDialogNodesRequestObj)req).FailCallback != null)
                    ((ListDialogNodesRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Update dialog node. Update an existing dialog node with new or modified data.    This operation is limited to 500 requests per 30 minutes. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="dialogNode">The dialog node ID (for example, `get_order`).</param>
        /// <param name="properties">The updated content of the dialog node.    Any elements included in the new data will completely replace the equivalent existing elements, including all subelements. (Previously existing subelements are not retained unless they are also included in the new data.) For example, if you update the actions for a dialog node, the previously existing actions are discarded and replaced with the new actions specified in the update.</param>
        /// <returns><see cref="DialogNode" />DialogNode</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool UpdateDialogNode(SuccessCallback<DialogNode> successCallback, FailCallback failCallback, string workspaceId, string dialogNode, UpdateDialogNode properties, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            UpdateDialogNodeRequestObj req = new UpdateDialogNodeRequestObj();
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
            req.Headers["Content-Type"] = "application/json";
            fsData data = null;
            _serializer.TrySerialize(properties, out data);
            req.Send = Encoding.UTF8.GetBytes(data.ToString());
            req.OnResponse = OnUpdateDialogNodeResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/dialog_nodes/{1}", workspaceId, dialogNode));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class UpdateDialogNodeRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<DialogNode> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnUpdateDialogNodeResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DialogNode result = new DialogNode();
            fsData data = null;
            Dictionary<string, object> customData = ((UpdateDialogNodeRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnUpdateDialogNodeResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((UpdateDialogNodeRequestObj)req).SuccessCallback != null)
                    ((UpdateDialogNodeRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((UpdateDialogNodeRequestObj)req).FailCallback != null)
                    ((UpdateDialogNodeRequestObj)req).FailCallback(resp.Error, customData);
            }
        }
        /// <summary>
        /// List log events in all workspaces. List the events from the logs of all workspaces in the service instance.    If **cursor** is not specified, this operation is limited to 40 requests per 30 minutes. If **cursor** is specified, the limit is 120 requests per minute. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="filter">A cacheable parameter that limits the results to those matching the specified filter. You must specify a filter query that includes a value for `language`, as well as a value for `workspace_id` or `request.context.metadata.deployment`. For more information, see the [documentation](https://console.bluemix.net/docs/services/conversation/filter-reference.html#filter-query-syntax).</param>
        /// <param name="sort">The attribute by which returned results will be sorted. To reverse the sort order, prefix the value with a minus sign (`-`). Supported values are `name`, `updated`, and `workspace_id`. (optional)</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to 100)</param>
        /// <param name="cursor">A token identifying the last object from the previous page of results. (optional)</param>
        /// <returns><see cref="LogCollection" />LogCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool ListAllLogs(SuccessCallback<LogCollection> successCallback, FailCallback failCallback, string filter, string sort = null, long? pageLimit = null, string cursor = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            ListAllLogsRequestObj req = new ListAllLogsRequestObj();
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
            if(!string.IsNullOrEmpty(filter))
                req.Parameters["filter"] = filter;
            if (!string.IsNullOrEmpty(sort))
                req.Parameters["sort"] = sort;
            if(pageLimit != null)
                req.Parameters["page_imit"] = pageLimit;
            if(!string.IsNullOrEmpty(cursor))
                req.Parameters["cursor"] = cursor;

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListAllLogsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/logs");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ListAllLogsRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<LogCollection> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnListAllLogsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            LogCollection result = new LogCollection();
            fsData data = null;
            Dictionary<string, object> customData = ((ListAllLogsRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnListAllLogsResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((ListAllLogsRequestObj)req).SuccessCallback != null)
                    ((ListAllLogsRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ListAllLogsRequestObj)req).FailCallback != null)
                    ((ListAllLogsRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// List log events in a workspace. List the events from the log of a specific workspace.    If **cursor** is not specified, this operation is limited to 40 requests per 30 minutes. If **cursor** is specified, the limit is 120 requests per minute. For more information, see [**Rate limiting**](https://www.ibm.com/watson/developercloud/assistant/api/v1/#rate-limiting).
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="sort">The attribute by which returned results will be sorted. To reverse the sort order, prefix the value with a minus sign (`-`). Supported values are `name`, `updated`, and `workspace_id`. (optional)</param>
        /// <param name="filter">A cacheable parameter that limits the results to those matching the specified filter. For more information, see the [documentation](https://console.bluemix.net/docs/services/conversation/filter-reference.html#filter-query-syntax). (optional)</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional, default to 100)</param>
        /// <param name="cursor">A token identifying the last object from the previous page of results. (optional)</param>
        /// <returns><see cref="LogCollection" />LogCollection</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw json output from the REST call will be passed in this object as the value of the 'json' key.</string></param>
        public bool ListLogs(SuccessCallback<LogCollection> successCallback, FailCallback failCallback, string workspaceId, string sort = null, string filter = null, long? pageLimit = null, string cursor = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            ListLogsRequestObj req = new ListLogsRequestObj();
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
            if (pageLimit != null)
                req.Parameters["page_limit"] = pageLimit;
            if (!string.IsNullOrEmpty(sort))
                req.Parameters["sort"] = sort;
            if (!string.IsNullOrEmpty(cursor))
                req.Parameters["cursor"] = cursor;
            if (!string.IsNullOrEmpty(filter))
                req.Parameters["filter"] = filter;
            req.OnResponse = OnListLogsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/workspaces/{0}/logs", workspaceId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class ListLogsRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<LogCollection> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnListLogsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            LogCollection result = new LogCollection();
            fsData data = null;
            Dictionary<string, object> customData = ((ListLogsRequestObj)req).CustomData;
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
                    Log.Error("Assistant.OnListLogsResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

            if (resp.Success)
            {
                if (((ListLogsRequestObj)req).SuccessCallback != null)
                    ((ListLogsRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ListLogsRequestObj)req).FailCallback != null)
                    ((ListLogsRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

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
                }
                catch (Exception e)
                {
                    Log.Error("Assistant.OnDeleteUserDataResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            customData.Add("json", data);

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
