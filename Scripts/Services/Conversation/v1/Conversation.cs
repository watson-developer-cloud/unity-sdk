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
using UnityEngine.Networking;
using Utility = IBM.Watson.DeveloperCloud.Utilities.Utility;

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

        #region Private Data
        private const string ServiceId = "conversation";
        private const string Workspaces = "/v1/workspaces";
        private Credentials _credentials = null;
        private string _url = "https://gateway.watsonplatform.net/conversation/api";
        private string _versionDate;
        private fsSerializer _serializer = new fsSerializer();
        #endregion

        #region Constructor
        /// <summary>
        /// Conversation constructor. Use this constructor to auto load credentials via ibm-credentials.env file.
        /// </summary>
        public Conversation()
        {
            var credentialsPaths = Utility.GetCredentialsPaths();
            if (credentialsPaths.Count > 0)
            {
                foreach (string path in credentialsPaths)
                {
                    if (Utility.LoadEnvFile(path))
                    {
                        break;
                    }
                }

                string ApiKey = Environment.GetEnvironmentVariable(ServiceId.ToUpper() + "_APIKEY");
                string Username = Environment.GetEnvironmentVariable(ServiceId.ToUpper() + "_USERNAME");
                string Password = Environment.GetEnvironmentVariable(ServiceId.ToUpper() + "_PASSWORD");
                string ServiceUrl = Environment.GetEnvironmentVariable(ServiceId.ToUpper() + "_URL");

                if (string.IsNullOrEmpty(ApiKey) && (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password)))
                {
                    throw new NullReferenceException(string.Format("Either {0}_APIKEY or {0}_USERNAME and {0}_PASSWORD did not exist. Please add credentials with this key in ibm-credentials.env.", ServiceId.ToUpper()));
                }

                if (!string.IsNullOrEmpty(ApiKey))
                {
                    TokenOptions tokenOptions = new TokenOptions()
                    {
                        IamApiKey = ApiKey
                    };

                    Credentials = new Credentials(tokenOptions, ServiceUrl);

                    if (string.IsNullOrEmpty(Credentials.Url))
                    {
                        Credentials.Url = Url;
                    }
                }

                if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
                {
                    Credentials = new Credentials(Username, Password, Url);
                }
            }
        }

        /// <summary>
        /// Conversation constructor
        /// </summary>
        /// <param name="credentials">The service credentials</param>
        [Obsolete("Conversation V1 is deprecated and will be removed in the next major release of the SDK. Use Assistant V1 or Assistant V2.")]
        public Conversation(Credentials credentials)
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
            req.HttpMethod = UnityWebRequest.kHttpVerbPOST;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Parameters["version"] = VersionDate;
            req.Function = "/" + workspaceID + "/message";
            req.Send = Encoding.UTF8.GetBytes(reqString);
            req.OnResponse = MessageResp;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=v1;operation_id=Message";
            req.DisableSslVerification = DisableSslVerification;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

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
            foreach (KeyValuePair<string, string> property in requestDict)
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
            req.HttpMethod = UnityWebRequest.kHttpVerbPOST;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Parameters["version"] = VersionDate;
            req.Function = "/" + workspaceID + "/message";
            req.Send = Encoding.UTF8.GetBytes(stringToSend);
            req.OnResponse = MessageResp;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=v1;operation_id=Message";
            req.DisableSslVerification = DisableSslVerification;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

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
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

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

        #region Delete User Data
        /// <summary>
        /// Deletes all data associated with a specified customer ID. The method has no effect if no data is associated with the customer ID. 
        /// You associate a customer ID with data by passing the X-Watson-Metadata header with a request that passes data. 
        /// For more information about personal data and customer IDs, see [**Information security**](https://cloud.ibm.com/docs/services/discovery/information-security.html).
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
            req.HttpMethod = UnityWebRequest.kHttpVerbDELETE;
            req.DisableSslVerification = DisableSslVerification;
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

            req.OnResponse = OnDeleteUserDataResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=conversation;service_version=v1;operation_id=DeleteUserData";

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
                    Log.Error("Conversation.OnDeleteUserDataResponse()", "Exception: {0}", e.ToString());
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

        #region IWatsonService implementation
        /// <exclude />
        public string GetServiceID()
        {
            return ServiceId;
        }
        #endregion
    }
}
