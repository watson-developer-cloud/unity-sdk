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

using FullSerializer;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.WatsonDeveloperCloud.Assistant.v2;
using System;
using System.Collections.Generic;
using System.Text;

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v2
{
    public class Assistant : IWatsonService
    {
        private const string ServiceId = "AssistantV2";
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
            if (credentials.HasCredentials() || credentials.HasIamTokenData())
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
        /// Create a session.
        ///
        /// Create a new session. A session is used to send user input to a skill and receive responses. It also
        /// maintains the state of the conversation.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="assistantId">Unique identifier of the assistant. You can find the assistant ID of an assistant
        /// on the **Assistants** tab of the Watson Assistant tool. For information about creating assistants, see the
        /// [documentation](https://console.bluemix.net/docs/services/assistant/create-assistant.html#creating-assistants).
        ///
        /// **Note:** Currently, the v2 API does not support creating assistants.</param>
        /// <returns><see cref="SessionResponse" />SessionResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateSession(SuccessCallback<SessionResponse> successCallback, FailCallback failCallback, String assistantId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            CreateSessionRequestObj req = new CreateSessionRequestObj();
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
            req.Headers["Content-Type"] = "application/json";
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnCreateSessionResponse;
            req.Post = true;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v2/assistants/{0}/sessions", assistantId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class CreateSessionRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<SessionResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnCreateSessionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            SessionResponse result = new SessionResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((CreateSessionRequestObj)req).CustomData;

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
                    Log.Error("Assistant.OnCreateSessionResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((CreateSessionRequestObj)req).SuccessCallback != null)
                    ((CreateSessionRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((CreateSessionRequestObj)req).FailCallback != null)
                    ((CreateSessionRequestObj)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Delete session.
        ///
        /// Deletes a session explicitly before it times out.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="assistantId">Unique identifier of the assistant. You can find the assistant ID of an assistant
        /// on the **Assistants** tab of the Watson Assistant tool. For information about creating assistants, see the
        /// [documentation](https://console.bluemix.net/docs/services/assistant/create-assistant.html#creating-assistants).
        ///
        /// **Note:** Currently, the v2 API does not support creating assistants.</param>
        /// <param name="sessionId">Unique identifier of the session.</param>
        /// <returns><see cref="" />object</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool DeleteSession(SuccessCallback<object> successCallback, FailCallback failCallback, String assistantId, String sessionId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            DeleteSessionRequestObj req = new DeleteSessionRequestObj();
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
            req.OnResponse = OnDeleteSessionResponse;
            req.Delete = true;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v2/assistants/{0}/sessions/{1}", assistantId, sessionId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteSessionRequestObj : RESTConnector.Request
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

        private void OnDeleteSessionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            object result = new object();
            fsData data = null;
            Dictionary<string, object> customData = ((DeleteSessionRequestObj)req).CustomData;

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
                    Log.Error("Assistant.OnDeleteSessionResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((DeleteSessionRequestObj)req).SuccessCallback != null)
                    ((DeleteSessionRequestObj)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteSessionRequestObj)req).FailCallback != null)
                    ((DeleteSessionRequestObj)req).FailCallback(resp.Error, customData);
            }
        }
        /// <summary>
        /// Send user input to assistant.
        ///
        /// Send user input to an assistant and receive a response.
        ///
        /// There is no rate limit for this operation.
        /// </summary>
        /// <param name="successCallback">The function that is called when the operation is successful.</param>
        /// <param name="failCallback">The function that is called when the operation fails.</param>
        /// <param name="assistantId">Unique identifier of the assistant. You can find the assistant ID of an assistant
        /// on the **Assistants** tab of the Watson Assistant tool. For information about creating assistants, see the
        /// [documentation](https://console.bluemix.net/docs/services/assistant/create-assistant.html#creating-assistants).
        ///
        /// **Note:** Currently, the v2 API does not support creating assistants.</param>
        /// <param name="sessionId">Unique identifier of the session.</param>
        /// <param name="request">The message to be sent. This includes the user's input, along with optional intents,
        /// entities, and context from the last response. (optional)</param>
        /// <returns><see cref="MessageResponse" />MessageResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool Message(SuccessCallback<MessageResponse> successCallback, FailCallback failCallback, String assistantId, String sessionId, MessageRequest request = null, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

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

            IDictionary<string, string> requestDict = new Dictionary<string, string>();
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

            req.Send = Encoding.UTF8.GetBytes(stringToSend);
            req.Headers["Content-Type"] = "application/json";
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnMessageResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v2/assistants/{0}/sessions/{1}/message", assistantId, sessionId));
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

        #region IWatsonService Interface
        /// <exclude />
        public string GetServiceID()
        {
            return ServiceId;
        }
        #endregion
    }
}
