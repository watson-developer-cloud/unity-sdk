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
    /// DialogNodeOutputGeneric.
    /// Classes which extend this class:
    /// - DialogNodeOutputGenericDialogNodeOutputResponseTypeText
    /// - DialogNodeOutputGenericDialogNodeOutputResponseTypePause
    /// - DialogNodeOutputGenericDialogNodeOutputResponseTypeImage
    /// - DialogNodeOutputGenericDialogNodeOutputResponseTypeOption
    /// - DialogNodeOutputGenericDialogNodeOutputResponseTypeConnectToAgent
    /// - DialogNodeOutputGenericDialogNodeOutputResponseTypeSearchSkill
    /// </summary>
    [JsonConverter(typeof(JsonSubtypes), "response_type")]
    [JsonSubtypes.KnownSubType(typeof(DialogNodeOutputGenericDialogNodeOutputResponseTypeConnectToAgent), "connect_to_agent")]
    [JsonSubtypes.KnownSubType(typeof(DialogNodeOutputGenericDialogNodeOutputResponseTypeImage), "image")]
    [JsonSubtypes.KnownSubType(typeof(DialogNodeOutputGenericDialogNodeOutputResponseTypeOption), "option")]
    [JsonSubtypes.KnownSubType(typeof(DialogNodeOutputGenericDialogNodeOutputResponseTypePause), "pause")]
    [JsonSubtypes.KnownSubType(typeof(DialogNodeOutputGenericDialogNodeOutputResponseTypeSearchSkill), "search_skill")]
    [JsonSubtypes.KnownSubType(typeof(DialogNodeOutputGenericDialogNodeOutputResponseTypeText), "text")]
    public class DialogNodeOutputGeneric
    {
        /// This ctor is protected to prevent instantiation of this base class.
        /// Instead, users should instantiate one of the subclasses listed below:
        /// - DialogNodeOutputGenericDialogNodeOutputResponseTypeText
        /// - DialogNodeOutputGenericDialogNodeOutputResponseTypePause
        /// - DialogNodeOutputGenericDialogNodeOutputResponseTypeImage
        /// - DialogNodeOutputGenericDialogNodeOutputResponseTypeOption
        /// - DialogNodeOutputGenericDialogNodeOutputResponseTypeConnectToAgent
        /// - DialogNodeOutputGenericDialogNodeOutputResponseTypeSearchSkill
        protected DialogNodeOutputGeneric()
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
        /// How a response is selected from the list, if more than one response is specified.
        /// </summary>
        public class SelectionPolicyValue
        {
            /// <summary>
            /// Constant SEQUENTIAL for sequential
            /// </summary>
            public const string SEQUENTIAL = "sequential";
            /// <summary>
            /// Constant RANDOM for random
            /// </summary>
            public const string RANDOM = "random";
            /// <summary>
            /// Constant MULTILINE for multiline
            /// </summary>
            public const string MULTILINE = "multiline";
            
        }

        /// <summary>
        /// The preferred type of control to display, if supported by the channel.
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
        /// The type of the search query.
        /// </summary>
        public class QueryTypeValue
        {
            /// <summary>
            /// Constant NATURAL_LANGUAGE for natural_language
            /// </summary>
            public const string NATURAL_LANGUAGE = "natural_language";
            /// <summary>
            /// Constant DISCOVERY_QUERY_LANGUAGE for discovery_query_language
            /// </summary>
            public const string DISCOVERY_QUERY_LANGUAGE = "discovery_query_language";
            
        }

        /// <summary>
        /// The type of response returned by the dialog node. The specified response type must be supported by the
        /// client application or channel.
        /// Constants for possible values can be found using DialogNodeOutputGeneric.ResponseTypeValue
        /// </summary>
        [JsonProperty("response_type", NullValueHandling = NullValueHandling.Ignore)]
        public string ResponseType { get; set; }
        /// <summary>
        /// How a response is selected from the list, if more than one response is specified.
        /// Constants for possible values can be found using DialogNodeOutputGeneric.SelectionPolicyValue
        /// </summary>
        [JsonProperty("selection_policy", NullValueHandling = NullValueHandling.Ignore)]
        public string SelectionPolicy { get; set; }
        /// <summary>
        /// The preferred type of control to display, if supported by the channel.
        /// Constants for possible values can be found using DialogNodeOutputGeneric.PreferenceValue
        /// </summary>
        [JsonProperty("preference", NullValueHandling = NullValueHandling.Ignore)]
        public string Preference { get; set; }
        /// <summary>
        /// The type of the search query.
        /// Constants for possible values can be found using DialogNodeOutputGeneric.QueryTypeValue
        /// </summary>
        [JsonProperty("query_type", NullValueHandling = NullValueHandling.Ignore)]
        public string QueryType { get; set; }
        /// <summary>
        /// A list of one or more objects defining text responses.
        /// </summary>
        [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
        public List<DialogNodeOutputTextValuesElement> Values { get; protected set; }
        /// <summary>
        /// The delimiter to use as a separator between responses when `selection_policy`=`multiline`.
        /// </summary>
        [JsonProperty("delimiter", NullValueHandling = NullValueHandling.Ignore)]
        public string Delimiter { get; protected set; }
        /// <summary>
        /// How long to pause, in milliseconds. The valid values are from 0 to 10000.
        /// </summary>
        [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
        public long? Time { get; protected set; }
        /// <summary>
        /// Whether to send a "user is typing" event during the pause. Ignored if the channel does not support this
        /// event.
        /// </summary>
        [JsonProperty("typing", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Typing { get; protected set; }
        /// <summary>
        /// The URL of the image.
        /// </summary>
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; protected set; }
        /// <summary>
        /// An optional title to show before the response.
        /// </summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; protected set; }
        /// <summary>
        /// An optional description to show with the response.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; protected set; }
        /// <summary>
        /// An array of objects describing the options from which the user can choose. You can include up to 20 options.
        /// </summary>
        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public List<DialogNodeOutputOptionsElement> Options { get; protected set; }
        /// <summary>
        /// An optional message to be sent to the human agent who will be taking over the conversation.
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
        /// The text of the search query. This can be either a natural-language query or a query that uses the Discovery
        /// query language syntax, depending on the value of the **query_type** property. For more information, see the
        /// [Discovery service
        /// documentation](https://cloud.ibm.com/docs/discovery?topic=discovery-query-operators#query-operators).
        /// </summary>
        [JsonProperty("query", NullValueHandling = NullValueHandling.Ignore)]
        public string Query { get; protected set; }
        /// <summary>
        /// An optional filter that narrows the set of documents to be searched. For more information, see the
        /// [Discovery service documentation]([Discovery service
        /// documentation](https://cloud.ibm.com/docs/discovery?topic=discovery-query-parameters#filter).
        /// </summary>
        [JsonProperty("filter", NullValueHandling = NullValueHandling.Ignore)]
        public string Filter { get; protected set; }
        /// <summary>
        /// The version of the Discovery service API to use for the query.
        /// </summary>
        [JsonProperty("discovery_version", NullValueHandling = NullValueHandling.Ignore)]
        public string DiscoveryVersion { get; protected set; }
    }
}
