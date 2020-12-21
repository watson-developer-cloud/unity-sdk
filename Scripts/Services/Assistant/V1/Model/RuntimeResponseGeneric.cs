/**
* (C) Copyright IBM Corp. 2019, 2020.
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
using JsonSubTypes;
using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// RuntimeResponseGeneric.
    /// Classes which extend this class:
    /// - RuntimeResponseGenericRuntimeResponseTypeText
    /// - RuntimeResponseGenericRuntimeResponseTypePause
    /// - RuntimeResponseGenericRuntimeResponseTypeImage
    /// - RuntimeResponseGenericRuntimeResponseTypeOption
    /// - RuntimeResponseGenericRuntimeResponseTypeConnectToAgent
    /// - RuntimeResponseGenericRuntimeResponseTypeSuggestion
    /// </summary>
    [JsonConverter(typeof(JsonSubtypes), "response_type")]
    [JsonSubtypes.KnownSubType(typeof(RuntimeResponseGenericRuntimeResponseTypeConnectToAgent), "connect_to_agent")]
    [JsonSubtypes.KnownSubType(typeof(RuntimeResponseGenericRuntimeResponseTypeImage), "image")]
    [JsonSubtypes.KnownSubType(typeof(RuntimeResponseGenericRuntimeResponseTypeOption), "option")]
    [JsonSubtypes.KnownSubType(typeof(RuntimeResponseGenericRuntimeResponseTypeSuggestion), "suggestion")]
    [JsonSubtypes.KnownSubType(typeof(RuntimeResponseGenericRuntimeResponseTypePause), "pause")]
    [JsonSubtypes.KnownSubType(typeof(RuntimeResponseGenericRuntimeResponseTypeText), "text")]
    public class RuntimeResponseGeneric
    {
        /// This ctor is protected to prevent instantiation of this base class.
        /// Instead, users should instantiate one of the subclasses listed below:
        /// - RuntimeResponseGenericRuntimeResponseTypeText
        /// - RuntimeResponseGenericRuntimeResponseTypePause
        /// - RuntimeResponseGenericRuntimeResponseTypeImage
        /// - RuntimeResponseGenericRuntimeResponseTypeOption
        /// - RuntimeResponseGenericRuntimeResponseTypeConnectToAgent
        /// - RuntimeResponseGenericRuntimeResponseTypeSuggestion
        protected RuntimeResponseGeneric()
        {
        }

        /// <summary>
        /// The type of response returned by the dialog node. The specified response type must be supported by the
        /// client application or channel.
        /// </summary>
        public class ResponseTypeValue
        {
            /// <summary>
            /// Constant TEXT for text
            /// </summary>
            public const string TEXT = "text";
            
        }

        /// <summary>
        /// The preferred type of control to display.
        /// </summary>
        public class PreferenceValue
        {
            /// <summary>
            /// Constant DROPDOWN for dropdown
            /// </summary>
            public const string DROPDOWN = "dropdown";
            /// <summary>
            /// Constant BUTTON for button
            /// </summary>
            public const string BUTTON = "button";
            
        }

        /// <summary>
        /// The type of response returned by the dialog node. The specified response type must be supported by the
        /// client application or channel.
        /// Constants for possible values can be found using RuntimeResponseGeneric.ResponseTypeValue
        /// </summary>
        [JsonProperty("response_type", NullValueHandling = NullValueHandling.Ignore)]
        public string ResponseType { get; set; }
        /// <summary>
        /// The preferred type of control to display.
        /// Constants for possible values can be found using RuntimeResponseGeneric.PreferenceValue
        /// </summary>
        [JsonProperty("preference", NullValueHandling = NullValueHandling.Ignore)]
        public string Preference { get; set; }
        /// <summary>
        /// The text of the response.
        /// </summary>
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; protected set; }
        /// <summary>
        /// How long to pause, in milliseconds.
        /// </summary>
        [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
        public long? Time { get; protected set; }
        /// <summary>
        /// Whether to send a "user is typing" event during the pause.
        /// </summary>
        [JsonProperty("typing", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Typing { get; protected set; }
        /// <summary>
        /// The URL of the image.
        /// </summary>
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; protected set; }
        /// <summary>
        /// The title or introductory text to show before the response.
        /// </summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; protected set; }
        /// <summary>
        /// The description to show with the the response.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; protected set; }
        /// <summary>
        /// An array of objects describing the options from which the user can choose.
        /// </summary>
        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public List<DialogNodeOutputOptionsElement> Options { get; protected set; }
        /// <summary>
        /// A message to be sent to the human agent who will be taking over the conversation.
        /// </summary>
        [JsonProperty("message_to_human_agent", NullValueHandling = NullValueHandling.Ignore)]
        public string MessageToHumanAgent { get; protected set; }
        /// <summary>
        /// An optional message to be displayed to the user to indicate that the conversation will be transferred to the
        /// next available agent.
        /// </summary>
        [JsonProperty("agent_available", NullValueHandling = NullValueHandling.Ignore)]
        public AgentAvailabilityMessage AgentAvailable { get; protected set; }
        /// <summary>
        /// An optional message to be displayed to the user to indicate that no online agent is available to take over
        /// the conversation.
        /// </summary>
        [JsonProperty("agent_unavailable", NullValueHandling = NullValueHandling.Ignore)]
        public AgentAvailabilityMessage AgentUnavailable { get; protected set; }
        /// <summary>
        /// Routing or other contextual information to be used by target service desk systems.
        /// </summary>
        [JsonProperty("transfer_info", NullValueHandling = NullValueHandling.Ignore)]
        public DialogNodeOutputConnectToAgentTransferInfo TransferInfo { get; protected set; }
        /// <summary>
        /// A label identifying the topic of the conversation, derived from the **title** property of the relevant node
        /// or the **topic** property of the dialog node response.
        /// </summary>
        [JsonProperty("topic", NullValueHandling = NullValueHandling.Ignore)]
        public string Topic { get; protected set; }
        /// <summary>
        /// The ID of the dialog node that the **topic** property is taken from. The **topic** property is populated
        /// using the value of the dialog node's **title** property.
        /// </summary>
        [JsonProperty("dialog_node", NullValueHandling = NullValueHandling.Ignore)]
        public string DialogNode { get; protected set; }
        /// <summary>
        /// An array of objects describing the possible matching dialog nodes from which the user can choose.
        /// </summary>
        [JsonProperty("suggestions", NullValueHandling = NullValueHandling.Ignore)]
        public List<DialogSuggestion> Suggestions { get; protected set; }
    }
}
