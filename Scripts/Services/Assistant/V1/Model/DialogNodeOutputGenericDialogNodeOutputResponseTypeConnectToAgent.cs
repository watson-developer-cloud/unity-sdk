/**
* (C) Copyright IBM Corp. 2020, 2021.
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
using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// DialogNodeOutputGenericDialogNodeOutputResponseTypeConnectToAgent.
    /// </summary>
    public class DialogNodeOutputGenericDialogNodeOutputResponseTypeConnectToAgent : DialogNodeOutputGeneric
    {
        /// <summary>
        /// The type of response returned by the dialog node. The specified response type must be supported by the
        /// client application or channel.
        /// </summary>
        [JsonProperty("response_type", NullValueHandling = NullValueHandling.Ignore)]
        public new string ResponseType
        {
            get { return base.ResponseType; }
            set { base.ResponseType = value; }
        }
        /// <summary>
        /// An optional message to be sent to the human agent who will be taking over the conversation.
        /// </summary>
        [JsonProperty("message_to_human_agent", NullValueHandling = NullValueHandling.Ignore)]
        public new string MessageToHumanAgent
        {
            get { return base.MessageToHumanAgent; }
            set { base.MessageToHumanAgent = value; }
        }
        /// <summary>
        /// An optional message to be displayed to the user to indicate that the conversation will be transferred to the
        /// next available agent.
        /// </summary>
        [JsonProperty("agent_available", NullValueHandling = NullValueHandling.Ignore)]
        public new AgentAvailabilityMessage AgentAvailable
        {
            get { return base.AgentAvailable; }
            set { base.AgentAvailable = value; }
        }
        /// <summary>
        /// An optional message to be displayed to the user to indicate that no online agent is available to take over
        /// the conversation.
        /// </summary>
        [JsonProperty("agent_unavailable", NullValueHandling = NullValueHandling.Ignore)]
        public new AgentAvailabilityMessage AgentUnavailable
        {
            get { return base.AgentUnavailable; }
            set { base.AgentUnavailable = value; }
        }
        /// <summary>
        /// Routing or other contextual information to be used by target service desk systems.
        /// </summary>
        [JsonProperty("transfer_info", NullValueHandling = NullValueHandling.Ignore)]
        public new DialogNodeOutputConnectToAgentTransferInfo TransferInfo { get; protected set; }
        /// <summary>
        /// An array of objects specifying channels for which the response is intended.
        /// </summary>
        [JsonProperty("channels", NullValueHandling = NullValueHandling.Ignore)]
        public new List<ResponseGenericChannel> Channels
        {
            get { return base.Channels; }
            set { base.Channels = value; }
        }
    }
}
