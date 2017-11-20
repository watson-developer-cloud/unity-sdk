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

using System;
using System.Text;
using FullSerializer;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;
using MiniJSON;
using System.Collections.Generic;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.Services.Conversation.v1
{
    /// <summary>
    /// This class wraps the Watson Conversation service. 
    /// <a href="http://www.ibm.com/watson/developercloud/conversation.html">Conversation Service</a>
    /// </summary>
    public class Conversation : IWatsonService
    {
        #region Public Types
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
                    throw new ArgumentNullException("VersionDate cannot be null. Use VersionDate `2017-05-26`");

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

        #region Private Data
        private const string ServiceId = "ConversationV1";
        private const string Workspaces = "/v1/workspaces";
        private Credentials _credentials = null;
        private string _url = "https://gateway.watsonplatform.net/conversation/api";
        private string _versionDate;
        #endregion

        #region Constructor
        /// <summary>
        /// Conversation constructor
        /// </summary>
        /// <param name="credentials">The service credentials</param>
        public Conversation(Credentials credentials)
        {
            if (credentials.HasCredentials() || credentials.HasAuthorizationToken())
            {
                Credentials = credentials;
            }
            else
            {
                throw new WatsonException("Please provide a username and password or authorization token to use the Conversation service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
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

        #region Message
        /// <summary>
        /// Message the specified workspaceId, input and callback.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="workspaceID">Workspace identifier.</param>
        /// <param name="input">Input.</param>
        /// <param name="customData">Custom data.</param>
        public bool Message(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceID, string input, Dictionary<string, object> customData = null)
        {
            //if (string.IsNullOrEmpty(workspaceID))
            //    throw new ArgumentNullException("workspaceId");
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, Workspaces);
            if (connector == null)
                return false;

            string reqJson = "{{\"input\": {{\"text\": \"{0}\"}}}}";
            string reqString = string.Format(reqJson, input);

            MessageReq req = new MessageReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Parameters["version"] = VersionDate;
            req.Function = "/" + workspaceID + "/message";
            req.Send = Encoding.UTF8.GetBytes(reqString);
            req.OnResponse = MessageResp;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            return connector.Send(req);
        }

        /// <summary>
        /// Message the specified workspaceId, input and callback.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="workspaceID">Workspace identifier.</param>
        /// <param name="messageRequest">Message request object.</param>
        /// <param name="customData">Custom data.</param>
        /// <returns></returns>
        public bool Message(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceID, MessageRequest messageRequest, Dictionary<string, object> customData = null)
        {
            if (string.IsNullOrEmpty(workspaceID))
                throw new ArgumentNullException("workspaceId");
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, Workspaces);
            if (connector == null)
                return false;
            
            IDictionary<string, string> requestDict = new Dictionary<string, string>();
            if (messageRequest.context != null)
                requestDict.Add("context", Json.Serialize(messageRequest.context));
            if (messageRequest.input != null)
                requestDict.Add("input", Json.Serialize(messageRequest.input));
            requestDict.Add("alternate_intents", Json.Serialize(messageRequest.alternate_intents));
            if (messageRequest.entities != null)
                requestDict.Add("entities", Json.Serialize(messageRequest.entities));
            if (messageRequest.intents != null)
                requestDict.Add("intents", Json.Serialize(messageRequest.intents));
            if (messageRequest.output != null)
                requestDict.Add("output", Json.Serialize(messageRequest.output));

            int iterator = 0;
            StringBuilder stringBuilder = new StringBuilder("{");
            foreach(KeyValuePair<string, string> property in requestDict)
            {
                string delimeter = iterator < requestDict.Count - 1 ? "," : "";
                stringBuilder.Append(string.Format("\"{0}\": {1}{2}", property.Key, property.Value, delimeter));
                iterator++;
            }
            stringBuilder.Append("}");

            string stringToSend = stringBuilder.ToString();

            MessageReq req = new MessageReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Parameters["version"] = VersionDate;
            req.Function = "/" + workspaceID + "/message";
            req.Send = Encoding.UTF8.GetBytes(stringToSend);
            req.OnResponse = MessageResp;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            return connector.Send(req);
        }


        private class MessageReq : RESTConnector.Request
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

        private void MessageResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            object result = null;
            string data = "";
            Dictionary<string, object> customData = ((MessageReq)req).CustomData;

            if (resp.Success)
            {
                try
                {
                    //  For deserializing into a generic object
                    data = Encoding.UTF8.GetString(resp.Data);
                    result = Json.Deserialize(data);
                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("Conversation.MessageResp()", "MessageResp Exception: {0}", e.ToString());
                    data = e.Message;
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((MessageReq)req).SuccessCallback != null)
                    ((MessageReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((MessageReq)req).FailCallback != null)
                    ((MessageReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Intents
        #endregion

        #region Entities
        #endregion

        #region Dialog Nodes
        #endregion

        #region IWatsonService implementation
        /// <exclude />
        public string GetServiceID()
        {
            return ServiceId;
        }
        #endregion
    }
}
