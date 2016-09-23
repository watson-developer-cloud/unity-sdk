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
using System.Collections.Generic;
using FullSerializer;
using MiniJSON;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;

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
		#endregion

		#region Private Data
		private const string SERVICE_ID = "ConversationV1";
        private const string SERVICE_MESSAGE = "/v1/workspaces";
		private static fsSerializer sm_Serializer = new fsSerializer();
        #endregion

        #region Message
        /// <summary>
        /// The callback delegate for the Message() function.
        /// </summary>
        /// <param name="resp">The response object to a call to Message().</param>
        public delegate void OnMessage(MessageResponse resp, string customData);

        /// <summary>
        /// Message the specified workspaceId, input and callback.
        /// </summary>
        /// <param name="workspaceID">Workspace identifier.</param>
        /// <param name="input">Input.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="customData">Custom data.</param>
        public bool Message(OnMessage callback, string workspaceID, string input, string customData = default(string))
		{
			if(string.IsNullOrEmpty(workspaceID))
				throw new ArgumentNullException("workspaceId");
			if(string.IsNullOrEmpty(input))
				throw new ArgumentNullException("input");
			if(callback == null)
				throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_MESSAGE);
			if(connector == null)
				return false;

			string reqJson = "{{\"input\": {{\"text\": \"{0}\"}}}}";
			string reqString = string.Format(reqJson, input);

			MessageReq req = new MessageReq();
			req.Callback = callback;
			req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Parameters["version"] = Version.VERSION;
			req.Function = "/" + workspaceID + "/message";
            req.Data = customData;
			req.Send = Encoding.UTF8.GetBytes(reqString);
			req.OnResponse = MessageResp;

			return connector.Send(req);
		}

        /// <summary>
        /// Message the specified workspaceId, input and callback.
        /// </summary>
        /// <param name="callback">Callback.</param>
        /// <param name="workspaceID">Workspace identifier.</param>
        /// <param name="messageRequest">Message request object.</param>
        /// <param name="customData">Custom data.</param>
        /// <returns></returns>
        public bool Message(OnMessage callback, string workspaceID, MessageRequest messageRequest, string customData = default(string))
        {
            if (string.IsNullOrEmpty(workspaceID))
                throw new ArgumentNullException("workspaceId");
            if(string.IsNullOrEmpty(messageRequest.input.text))
                throw new ArgumentNullException("messageRequest.input.text");
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_MESSAGE);
            if (connector == null)
                return false;

            fsData data;
            sm_Serializer.TrySerialize(messageRequest.GetType(), messageRequest, out data).AssertSuccessWithoutWarnings();
            string reqString = fsJsonPrinter.CompressedJson(data);

            MessageReq req = new MessageReq();
            req.Callback = callback;
            req.MessageRequest = messageRequest;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Parameters["version"] = Version.VERSION;
            req.Function = "/" + workspaceID + "/message";
            req.Data = customData;
            req.Send = Encoding.UTF8.GetBytes(reqString);
            req.OnResponse = MessageResp;

            return connector.Send(req);
        }


		private class MessageReq : RESTConnector.Request
		{
			public OnMessage Callback { get; set; }
            public MessageRequest MessageRequest { get; set; }
            public string Data { get; set; }
		}

		private void MessageResp(RESTConnector.Request req, RESTConnector.Response resp)
		{
			MessageResponse response = new MessageResponse();
			if (resp.Success)
			{
				try
				{
					fsData data = null;
					fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
					if (!r.Succeeded)
						throw new WatsonException(r.FormattedMessages);

					object obj = response;
					r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
					if (!r.Succeeded)
						throw new WatsonException(r.FormattedMessages);
				}
				catch (Exception e)
				{
					Log.Error("Conversation", "MessageResp Exception: {0}", e.ToString());
					resp.Success = false;
				}
			}

			if (((MessageReq)req).Callback != null)
				((MessageReq)req).Callback(resp.Success ? response : null, ((MessageReq)req).Data);
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
			return SERVICE_ID;
		}

		/// <exclude />
		public void GetServiceStatus(ServiceStatus callback)
		{
            if (Config.Instance.FindCredentials(SERVICE_ID) != null)
                new CheckServiceStatus(this, callback);
            else
            {
    			if (callback != null && callback.Target != null)
    			{
    				callback(SERVICE_ID, false);
    			}
            }
		}

        private class CheckServiceStatus
        {
            private Conversation m_Service = null;
            private ServiceStatus m_Callback = null;
            private int m_ConversationCount = 0;

            public CheckServiceStatus(Conversation service, ServiceStatus callback)
            {
                m_Service = service;
                m_Callback = callback;

                string customServiceID = Config.Instance.GetVariableValue(SERVICE_ID + "_ID");

                //If custom classifierID is defined then we are using it to check the service health
                if (!string.IsNullOrEmpty(customServiceID))
                {

                    if (!m_Service.Message(OnMessage, customServiceID, "Ping", "Ping"))
                        OnFailure("Failed to invoke Converse().");
                    else
                        m_ConversationCount += 1;
                }
                else
                {
                    OnFailure("Please define a workspace variable in config.json (" + SERVICE_ID + "_ID)");
                }
            }
           
            private void OnMessage(MessageResponse resp, string customData)
            {
                if (m_ConversationCount > 0)
                {
                    m_ConversationCount -= 1;
                    if (resp != null)
                    {
                        if (m_ConversationCount == 0 && m_Callback != null && m_Callback.Target != null)
                            m_Callback(SERVICE_ID, true);
                    }
                    else
                        OnFailure("ConverseResponse is null.");
                }
            }

            private void OnFailure(string msg)
            {
                Log.Error("Dialog", msg);
                m_Callback(SERVICE_ID, false);
                m_ConversationCount = 0;
            }
        };
		#endregion
	}
}
