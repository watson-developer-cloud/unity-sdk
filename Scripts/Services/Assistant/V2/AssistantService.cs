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
using IBM.Watson.Assistant.V2.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Networking;

namespace IBM.Watson.Assistant.V2
{
    public partial class AssistantService : BaseService
    {
        private const string serviceId = "assistant";
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
                throw new IBMException("Please provide a username and password or authorization token to use the Assistant service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }

        /// <summary>
        /// Create a session.
        ///
        /// Create a new session. A session is used to send user input to a skill and receive responses. It also
        /// maintains the state of the conversation.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="assistantId">Unique identifier of the assistant. You can find the assistant ID of an assistant
        /// on the **Assistants** tab of the Watson Assistant tool. For information about creating assistants, see the
        /// [documentation](https://cloud.ibm.com/docs/services/assistant?topic=assistant-assistant-add#assistant-add-task).
        ///
        /// **Note:** Currently, the v2 API does not support creating assistants.</param>
        /// <returns><see cref="SessionResponse" />SessionResponse</returns>
        public bool CreateSession(Callback<SessionResponse> callback, string assistantId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateSession`");
            if (string.IsNullOrEmpty(assistantId))
                throw new ArgumentNullException("`assistantId` is required for `CreateSession`");

            RequestObject<SessionResponse> req = new RequestObject<SessionResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V2", "CreateSession"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnCreateSessionResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v2/assistants/{0}/sessions", assistantId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateSessionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<SessionResponse> response = new DetailedResponse<SessionResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<SessionResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnCreateSessionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<SessionResponse>)req).Callback != null)
                ((RequestObject<SessionResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete session.
        ///
        /// Deletes a session explicitly before it times out.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="assistantId">Unique identifier of the assistant. You can find the assistant ID of an assistant
        /// on the **Assistants** tab of the Watson Assistant tool. For information about creating assistants, see the
        /// [documentation](https://cloud.ibm.com/docs/services/assistant?topic=assistant-assistant-add#assistant-add-task).
        ///
        /// **Note:** Currently, the v2 API does not support creating assistants.</param>
        /// <param name="sessionId">Unique identifier of the session.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteSession(Callback<object> callback, string assistantId, string sessionId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteSession`");
            if (string.IsNullOrEmpty(assistantId))
                throw new ArgumentNullException("`assistantId` is required for `DeleteSession`");
            if (string.IsNullOrEmpty(sessionId))
                throw new ArgumentNullException("`sessionId` is required for `DeleteSession`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V2", "DeleteSession"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
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

        private void OnDeleteSessionResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("AssistantService.OnDeleteSessionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Send user input to assistant.
        ///
        /// Send user input to an assistant and receive a response.
        ///
        /// There is no rate limit for this operation.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="assistantId">Unique identifier of the assistant. You can find the assistant ID of an assistant
        /// on the **Assistants** tab of the Watson Assistant tool. For information about creating assistants, see the
        /// [documentation](https://cloud.ibm.com/docs/services/assistant?topic=assistant-assistant-add#assistant-add-task).
        ///
        /// **Note:** Currently, the v2 API does not support creating assistants.</param>
        /// <param name="sessionId">Unique identifier of the session.</param>
        /// <param name="input">An input object that includes the input text. (optional)</param>
        /// <param name="context">State information for the conversation. The context is stored by the assistant on a
        /// per-session basis. You can use this property to set or modify context variables, which can also be accessed
        /// by dialog nodes. (optional)</param>
        /// <returns><see cref="MessageResponse" />MessageResponse</returns>
        public bool Message(Callback<MessageResponse> callback, string assistantId, string sessionId, MessageInput input = null, MessageContext context = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `Message`");
            if (string.IsNullOrEmpty(assistantId))
                throw new ArgumentNullException("`assistantId` is required for `Message`");
            if (string.IsNullOrEmpty(sessionId))
                throw new ArgumentNullException("`sessionId` is required for `Message`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V2", "Message"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (input != null)
                bodyObject["input"] = JToken.FromObject(input);
            if (context != null)
                bodyObject["context"] = JToken.FromObject(context);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnMessageResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v2/assistants/{0}/sessions/{1}/message", assistantId, sessionId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
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
    }
}