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
using IBM.Watson.Connection;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace IBM.Watson.Assistant.V2
{
    public class AssistantService
    {
        private fsSerializer serializer = new fsSerializer();
        private Credentials credentials = null;
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
                    url = credentials.Url;
                }
            }
        }

        private string url = "https://gateway.watsonplatform.net/assistant/api";
        /// <summary>
        /// Gets and sets the endpoint URL for the service.
        /// </summary>
        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        private string versionDate;
        /// <summary>
        /// Gets and sets the versionDate of the service.
        /// </summary>
        public string VersionDate
        {
            get { return versionDate; }
            set { versionDate = value; }
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

        /// <summary>
        /// AssistantService constructor.
        /// </summary>
        /// <param name="credentials">The service credentials.</param>
        public AssistantService(Credentials credentials)
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
        /// <param name="response">The returned WatsonResponse.</param>
        /// <param name="customData">user defined custom data including raw json.</param>
        public delegate void Callback<T>(WatsonResponse<T> response, WatsonError error, Dictionary<string, object> customData);
        #endregion

        /// <summary>
        /// Create a session.
        ///
        /// Create a new session. A session is used to send user input to a skill and receive responses. It also
        /// maintains the state of the conversation.
        /// </summary>
        /// <param name="callback">The function that is called when the operation is successful.</param>
        /// <param name="assistantId">Unique identifier of the assistant. You can find the assistant ID of an assistant
        /// on the **Assistants** tab of the Watson Assistant tool. For information about creating assistants, see the
        /// [documentation](https://console.bluemix.net/docs/services/assistant/create-assistant.html#creating-assistants).
        ///
        /// **Note:** Currently, the v2 API does not support creating assistants.</param>
        /// <returns><see cref="SessionResponse" />SessionResponse</returns>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        public bool CreateSession(Callback<SessionResponse> callback, String assistantId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback is required for CreateSession");
            }
            if (string.IsNullOrEmpty(assistantId))
            {
                throw new ArgumentException("assistantId is required for CreateSession");
            }

            CreateSessionRequestObj req = new CreateSessionRequestObj
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
            req.Headers["Content-Type"] = "application/json";
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnCreateSessionResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v2/assistants/{0}/sessions", assistantId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private class CreateSessionRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public Callback<SessionResponse> Callback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnCreateSessionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<SessionResponse> response = new WatsonResponse<SessionResponse>
            {
                Result = new SessionResponse()
            };

            fsData data = null;
            Dictionary<string, object> customData = ((CreateSessionRequestObj)req).CustomData;

            try
            {
                fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                if (!r.Succeeded)
                {
                    throw new WatsonException(r.FormattedMessages);
                }

                object obj = response.Result;
                r = serializer.TryDeserialize(data, obj.GetType(), ref obj);
                if (!r.Succeeded)
                {
                    throw new WatsonException(r.FormattedMessages);
                }

                customData.Add("json", data);
            }
            catch (Exception e)
            {
                Log.Error("Assistant.OnCreateSessionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((CreateSessionRequestObj)req).Callback != null)
            {
                ((CreateSessionRequestObj)req).Callback(response, resp.Error, customData);
            }
        }

        /// <summary>
        /// Delete session.
        ///
        /// Deletes a session explicitly before it times out.
        /// </summary>
        /// <param name="callback">The function that is called when the operation is successful.</param>
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
        public bool DeleteSession(Callback<object> callback, String assistantId, String sessionId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback is required for DeleteSession");
            }
            if (string.IsNullOrEmpty(assistantId))
            {
                throw new ArgumentException("assistantId is required for DeleteSession");
            }
            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentException("sessionId is required for DeleteSession");
            }

            DeleteSessionRequestObj req = new DeleteSessionRequestObj
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
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnDeleteSessionResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v2/assistants/{0}/sessions/{1}", assistantId, sessionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private class DeleteSessionRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public Callback<object> Callback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnDeleteSessionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<object> response = new WatsonResponse<object>
            {
                Result = new object()
            };

            fsData data = null;
            Dictionary<string, object> customData = ((DeleteSessionRequestObj)req).CustomData;

            try
            {
                fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                if (!r.Succeeded)
                {
                    throw new WatsonException(r.FormattedMessages);
                }

                object obj = response.Result;
                r = serializer.TryDeserialize(data, obj.GetType(), ref obj);
                if (!r.Succeeded)
                {
                    throw new WatsonException(r.FormattedMessages);
                }

                customData.Add("json", data);
            }
            catch (Exception e)
            {
                Log.Error("Assistant.OnDeleteSessionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((DeleteSessionRequestObj)req).Callback != null)
            {
                ((DeleteSessionRequestObj)req).Callback(response, resp.Error, customData);
            }
        }
        /// <summary>
        /// Send user input to assistant.
        ///
        /// Send user input to an assistant and receive a response.
        ///
        /// There is no rate limit for this operation.
        /// </summary>
        /// <param name="callback">The function that is called when the operation is successful.</param>
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
        public bool Message(Callback<MessageResponse> callback, String assistantId, String sessionId, MessageRequest request = null, Dictionary<string, object> customData = null)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback is required for Message");
            }
            if (string.IsNullOrEmpty(assistantId))
            {
                throw new ArgumentException("assistantId is required for Message");
            }
            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentException("sessionId is required for Message");
            }

            MessageRequestObj req = new MessageRequestObj
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

            if (request != null)
            {
                fsData data = null;
                serializer.TrySerialize(request, out data);
                fsSerializer.StripDeserializationMetadata(ref data);
                string json = data.ToString().Replace('\"', '"');
                req.Send = Encoding.UTF8.GetBytes(json);
            }

            req.Headers["Content-Type"] = "application/json";
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnMessageResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v2/assistants/{0}/sessions/{1}/message", assistantId, sessionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private class MessageRequestObj : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public Callback<MessageResponse> Callback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnMessageResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            WatsonResponse<MessageResponse> response = new WatsonResponse<MessageResponse>();
            response.Result = new MessageResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((MessageRequestObj)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                if (!r.Succeeded)
                {
                    throw new WatsonException(r.FormattedMessages);
                }

                object obj = response.Result;
                r = serializer.TryDeserialize(data, obj.GetType(), ref obj);
                if (!r.Succeeded)
                {
                    throw new WatsonException(r.FormattedMessages);
                }

                customData.Add("json", data);
            }
            catch (Exception e)
            {
                Log.Error("Assistant.OnMessageResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((MessageRequestObj)req).Callback != null)
            {
                ((MessageRequestObj)req).Callback(response, resp.Error, customData);
            }
        }
    }
}
