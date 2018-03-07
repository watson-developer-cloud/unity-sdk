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

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    public class Assistant : IWatsonService, IAssistant
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

        private string _url  = "https://gateway.watsonplatform.net/assistant/api";
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

        /// <summary>
        /// Assistant constructor.
        /// </summary>
        /// <param name="credentials">The service credentials.</param>
        public Assistant(Credentials credentials)
        {
            if (credentials.HasCredentials() || credentials.HasAuthorizationToken())
            {
                Credentials = credentials;
            }
            else
            {
                throw new WatsonException("Please provide a username and password or authorization token to use the Assistant service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnCreateWorkspaceResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces", Url));
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
                    Log.Error("Assistant.OnCreateWorkspaceResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteWorkspaceResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}", Url, workspaceId));
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
                    Log.Error("Assistant.OnDeleteWorkspaceResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetWorkspaceResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}", Url, workspaceId));
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
                    Log.Error("Assistant.OnGetWorkspaceResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListWorkspacesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces"));
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
                    Log.Error("Assistant.OnListWorkspacesResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnUpdateWorkspaceResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}", Url, workspaceId));
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
                    Log.Error("Assistant.OnUpdateWorkspaceResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
        public bool Message(SuccessCallback<MessageResponse> successCallback, FailCallback failCallback, string workspaceId, MessageRequest request = null, bool? nodesVisitedDetails = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            MessageRequestObj req = new MessageRequestObj();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnMessageResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/message", Url, workspaceId));
            if (connector == null)
            return false;

            return connector.Send(req);
        }

        private class MessageRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<MessageResponse> SuccessCallback { get; set; }
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
            MessageResponse result = new MessageResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((MessageRequestObj)req).CustomData;

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
                    Log.Error("Assistant.OnMessageResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnCreateIntentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{0}/intents", Url, workspaceId));
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
                    Log.Error("Assistant.OnCreateIntentResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteIntentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/intents/{2}", Url, workspaceId, intent));
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
                    Log.Error("Assistant.OnDeleteIntentResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetIntentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/intents/{2}", Url, workspaceId, intent));
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
                    Log.Error("Assistant.OnGetIntentResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListIntentsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/intents", Url, workspaceId));
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
                    Log.Error("Assistant.OnListIntentsResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnUpdateIntentResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/intents/{2}", Url, workspaceId, intent));
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
                    Log.Error("Assistant.OnUpdateIntentResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnCreateExampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/intents/{2}/examples", Url, workspaceId, intent));
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
                    Log.Error("Assistant.OnCreateExampleResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteExampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/intents/{2}/examples/{3}", Url, workspaceId, intent, text));
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
                    Log.Error("Assistant.OnDeleteExampleResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetExampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/intents/{2}/examples/{3}", Url, workspaceId, intent, text));
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
                    Log.Error("Assistant.OnGetExampleResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListExamplesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/intents/{2}/examples", Url, workspaceId, intent));
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
                    Log.Error("Assistant.OnListExamplesResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnUpdateExampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/intents/{2}/examples/{3}", Url, workspaceId, intent, text));
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
                    Log.Error("Assistant.OnUpdateExampleResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnCreateEntityResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/entities", Url, workspaceId));
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
                    Log.Error("Assistant.OnCreateEntityResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteEntityResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/entities/{2}", Url, workspaceId, entity));
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
                    Log.Error("Assistant.OnDeleteEntityResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetEntityResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/entities/{2}", Url, workspaceId, entity));
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
                    Log.Error("Assistant.OnGetEntityResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListEntitiesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/entities", Url, workspaceId));
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
                    Log.Error("Assistant.OnListEntitiesResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnUpdateEntityResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/entities/{2}", Url, workspaceId, entity));
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
                    Log.Error("Assistant.OnUpdateEntityResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnCreateValueResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/entities/{2}/values", Url, workspaceId, entity));
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
                    Log.Error("Assistant.OnCreateValueResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteValueResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/entities/{2}/values/{3}", Url, workspaceId, entity, value));
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
                    Log.Error("Assistant.OnDeleteValueResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetValueResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/entities/{2}/values/{3}", Url, workspaceId, entity, value));
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
                    Log.Error("Assistant.OnGetValueResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListValuesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/entities/{2}/values", Url, workspaceId, entity));
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
                    Log.Error("Assistant.OnListValuesResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnUpdateValueResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/entities/{2}/values/{3}", Url, workspaceId, entity, value));
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
                    Log.Error("Assistant.OnUpdateValueResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnCreateSynonymResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/entities/{2}/values/{3}/synonyms", Url, workspaceId, entity, value));
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
                    Log.Error("Assistant.OnCreateSynonymResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteSynonymResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/entities/{2}/values/{3}/synonyms/{4}", Url, workspaceId, entity, value, synonym));
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
                    Log.Error("Assistant.OnDeleteSynonymResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetSynonymResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/entities/{2}/values/{3}/synonyms/{4}", Url, workspaceId, entity, value, synonym));
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
                    Log.Error("Assistant.OnGetSynonymResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListSynonymsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/entities/{2}/values/{3}/synonyms", Url, workspaceId, entity, value));
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
                    Log.Error("Assistant.OnListSynonymsResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnUpdateSynonymResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/entities/{2}/values/{3}/synonyms/{4}", Url, workspaceId, entity, value, synonym));
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
                    Log.Error("Assistant.OnUpdateSynonymResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnCreateDialogNodeResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/dialog_nodes", Url, workspaceId));
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
                    Log.Error("Assistant.OnCreateDialogNodeResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteDialogNodeResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/dialogNodes/{2}", Url, workspaceId, dialogNode));
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
                    Log.Error("Assistant.OnDeleteDialogNodeResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetDialogNodeResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/dialogNodes/{2}", Url, workspaceId, dialogNode));
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
                    Log.Error("Assistant.OnGetDialogNodeResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListDialogNodesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/dialog_nodes", Url, workspaceId));
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
                    Log.Error("Assistant.OnListDialogNodesResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnUpdateDialogNodeResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/dialogNodes/{2}", Url, workspaceId, dialogNode));
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
                    Log.Error("Assistant.OnUpdateDialogNodeResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListAllLogsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/logs", Url));
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
                    Log.Error("Assistant.OnListAllLogsResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListLogsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/logs", Url, workspaceId));
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
                    Log.Error("Assistant.OnListLogsResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnCreateCounterexampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/counterexamples", Url, workspaceId));
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
                    Log.Error("Assistant.OnCreateCounterexampleResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteCounterexampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/counterexamples/{2}", Url, workspaceId, text));
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
                    Log.Error("Assistant.OnDeleteCounterexampleResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetCounterexampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/counterexamples/{2}", Url, workspaceId, text));
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
                    Log.Error("Assistant.OnGetCounterexampleResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListCounterexamplesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/counterexamples", Url, workspaceId));
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
                    Log.Error("Assistant.OnListCounterexamplesResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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
            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnUpdateCounterexampleResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("{0}/v1/workspaces/{1}/counterexamples/{2}", Url, workspaceId, text));
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
                    Log.Error("Assistant.OnUpdateCounterexampleResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

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

        #region IWatsonService Interface
        /// <exclude />
        public string GetServiceID()
        {
        return ServiceId;
        }
        #endregion
    }
}
