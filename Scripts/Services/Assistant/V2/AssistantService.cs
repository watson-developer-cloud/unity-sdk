/**
* (C) Copyright IBM Corp. 2018, 2020.
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
        private const string defaultServiceUrl = "https://api.us-south.assistant.watson.cloud.ibm.com";

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
        /// Create a session.
        ///
        /// Create a new session. A session is used to send user input to a skill and receive responses. It also
        /// maintains the state of the conversation. A session persists until it is deleted, or until it times out
        /// because of inactivity. (For more information, see the
        /// [documentation](https://cloud.ibm.com/docs/assistant?topic=assistant-assistant-settings).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="assistantId">Unique identifier of the assistant. To find the assistant ID in the Watson
        /// Assistant user interface, open the assistant settings and click **API Details**. For information about
        /// creating assistants, see the
        /// [documentation](https://cloud.ibm.com/docs/assistant?topic=assistant-assistant-add#assistant-add-task).
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

            Connector.URL = GetServiceUrl() + string.Format("/v2/assistants/{0}/sessions", assistantId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
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
        /// Deletes a session explicitly before it times out. (For more information about the session inactivity
        /// timeout, see the [documentation](https://cloud.ibm.com/docs/assistant?topic=assistant-assistant-settings)).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="assistantId">Unique identifier of the assistant. To find the assistant ID in the Watson
        /// Assistant user interface, open the assistant settings and click **API Details**. For information about
        /// creating assistants, see the
        /// [documentation](https://cloud.ibm.com/docs/assistant?topic=assistant-assistant-add#assistant-add-task).
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

            Connector.URL = GetServiceUrl() + string.Format("/v2/assistants/{0}/sessions/{1}", assistantId, sessionId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
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
        /// Send user input to assistant (stateful).
        ///
        /// Send user input to an assistant and receive a response, with conversation state (including context data)
        /// stored by Watson Assistant for the duration of the session.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="assistantId">Unique identifier of the assistant. To find the assistant ID in the Watson
        /// Assistant user interface, open the assistant settings and click **API Details**. For information about
        /// creating assistants, see the
        /// [documentation](https://cloud.ibm.com/docs/assistant?topic=assistant-assistant-add#assistant-add-task).
        ///
        /// **Note:** Currently, the v2 API does not support creating assistants.</param>
        /// <param name="sessionId">Unique identifier of the session.</param>
        /// <param name="input">An input object that includes the input text. (optional)</param>
        /// <param name="context">Context data for the conversation. You can use this property to set or modify context
        /// variables, which can also be accessed by dialog nodes. The context is stored by the assistant on a
        /// per-session basis.
        ///
        /// **Note:** The total size of the context data stored for a stateful session cannot exceed 100KB.
        /// (optional)</param>
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

            Connector.URL = GetServiceUrl() + string.Format("/v2/assistants/{0}/sessions/{1}/message", assistantId, sessionId);
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
        /// Send user input to assistant (stateless).
        ///
        /// Send user input to an assistant and receive a response, with conversation state (including context data)
        /// managed by your application.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="assistantId">Unique identifier of the assistant. To find the assistant ID in the Watson
        /// Assistant user interface, open the assistant settings and click **API Details**. For information about
        /// creating assistants, see the
        /// [documentation](https://cloud.ibm.com/docs/assistant?topic=assistant-assistant-add#assistant-add-task).
        ///
        /// **Note:** Currently, the v2 API does not support creating assistants.</param>
        /// <param name="input">An input object that includes the input text. (optional)</param>
        /// <param name="context">Context data for the conversation. You can use this property to set or modify context
        /// variables, which can also be accessed by dialog nodes. The context is not stored by the assistant. To
        /// maintain session state, include the context from the previous response.
        ///
        /// **Note:** The total size of the context data for a stateless session cannot exceed 250KB. (optional)</param>
        /// <returns><see cref="MessageResponseStateless" />MessageResponseStateless</returns>
        public bool MessageStateless(Callback<MessageResponseStateless> callback, string assistantId, MessageInputStateless input = null, MessageContextStateless context = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `MessageStateless`");
            if (string.IsNullOrEmpty(assistantId))
                throw new ArgumentNullException("`assistantId` is required for `MessageStateless`");

            RequestObject<MessageResponseStateless> req = new RequestObject<MessageResponseStateless>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V2", "MessageStateless"))
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

            req.OnResponse = OnMessageStatelessResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/assistants/{0}/message", assistantId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnMessageStatelessResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<MessageResponseStateless> response = new DetailedResponse<MessageResponseStateless>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<MessageResponseStateless>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("AssistantService.OnMessageStatelessResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<MessageResponseStateless>)req).Callback != null)
                ((RequestObject<MessageResponseStateless>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List log events for an assistant.
        ///
        /// List the events from the log of an assistant.
        ///
        /// This method is available only with Premium plans.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="assistantId">Unique identifier of the assistant. To find the assistant ID in the Watson
        /// Assistant user interface, open the assistant settings and click **API Details**. For information about
        /// creating assistants, see the
        /// [documentation](https://cloud.ibm.com/docs/assistant?topic=assistant-assistant-add#assistant-add-task).
        ///
        /// **Note:** Currently, the v2 API does not support creating assistants.</param>
        /// <param name="sort">How to sort the returned log events. You can sort by **request_timestamp**. To reverse
        /// the sort order, prefix the parameter value with a minus sign (`-`). (optional)</param>
        /// <param name="filter">A cacheable parameter that limits the results to those matching the specified filter.
        /// For more information, see the
        /// [documentation](https://cloud.ibm.com/docs/assistant?topic=assistant-filter-reference#filter-reference).
        /// (optional)</param>
        /// <param name="pageLimit">The number of records to return in each page of results. (optional)</param>
        /// <param name="cursor">A token identifying the page of results to retrieve. (optional)</param>
        /// <returns><see cref="LogCollection" />LogCollection</returns>
        public bool ListLogs(Callback<LogCollection> callback, string assistantId, string sort = null, string filter = null, long? pageLimit = null, string cursor = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListLogs`");
            if (string.IsNullOrEmpty(assistantId))
                throw new ArgumentNullException("`assistantId` is required for `ListLogs`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V2", "ListLogs"))
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

            Connector.URL = GetServiceUrl() + string.Format("/v2/assistants/{0}/logs", assistantId);
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
        /// Delete labeled data.
        ///
        /// Deletes all data associated with a specified customer ID. The method has no effect if no data is associated
        /// with the customer ID.
        ///
        /// You associate a customer ID with data by passing the `X-Watson-Metadata` header with a request that passes
        /// data. For more information about personal data and customer IDs, see [Information
        /// security](https://cloud.ibm.com/docs/assistant?topic=assistant-information-security#information-security).
        ///
        /// This operation is limited to 4 requests per minute. For more information, see **Rate limiting**.
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("conversation", "V2", "DeleteUserData"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(customerId))
            {
                req.Parameters["customer_id"] = customerId;
            }

            req.OnResponse = OnDeleteUserDataResponse;

            Connector.URL = GetServiceUrl() + "/v2/user_data";
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