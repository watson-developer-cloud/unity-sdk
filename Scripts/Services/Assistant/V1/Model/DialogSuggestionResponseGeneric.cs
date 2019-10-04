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
using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// DialogSuggestionResponseGeneric.
    /// </summary>
    public class DialogSuggestionResponseGeneric
    {
        /// <summary>
        /// The type of response returned by the dialog node. The specified response type must be supported by the
        /// client application or channel.
        ///
        /// **Note:** The **suggestion** response type is part of the disambiguation feature, which is only available
        /// for Plus and Premium users. The **search_skill** response type is available only for Plus and Premium users,
        /// and is used only by the v2 runtime API.
        /// </summary>
        public class ResponseTypeValue
        {
            /// <summary>
            /// Constant TEXT for text
            /// </summary>
            public const string TEXT = "text";
            /// <summary>
            /// Constant PAUSE for pause
            /// </summary>
            public const string PAUSE = "pause";
            /// <summary>
            /// Constant IMAGE for image
            /// </summary>
            public const string IMAGE = "image";
            /// <summary>
            /// Constant OPTION for option
            /// </summary>
            public const string OPTION = "option";
            /// <summary>
            /// Constant CONNECT_TO_AGENT for connect_to_agent
            /// </summary>
            public const string CONNECT_TO_AGENT = "connect_to_agent";
            /// <summary>
            /// Constant SEARCH_SKILL for search_skill
            /// </summary>
            public const string SEARCH_SKILL = "search_skill";
            
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
        ///
        /// **Note:** The **suggestion** response type is part of the disambiguation feature, which is only available
        /// for Plus and Premium users. The **search_skill** response type is available only for Plus and Premium users,
        /// and is used only by the v2 runtime API.
        /// Constants for possible values can be found using DialogSuggestionResponseGeneric.ResponseTypeValue
        /// </summary>
        [JsonProperty("response_type", NullValueHandling = NullValueHandling.Ignore)]
        public string ResponseType { get; set; }
        /// <summary>
        /// The preferred type of control to display.
        /// Constants for possible values can be found using DialogSuggestionResponseGeneric.PreferenceValue
        /// </summary>
        [JsonProperty("preference", NullValueHandling = NullValueHandling.Ignore)]
        public string Preference { get; set; }
        /// <summary>
        /// The text of the response.
        /// </summary>
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
        /// <summary>
        /// How long to pause, in milliseconds.
        /// </summary>
        [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
        public long? Time { get; set; }
        /// <summary>
        /// Whether to send a "user is typing" event during the pause.
        /// </summary>
        [JsonProperty("typing", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Typing { get; set; }
        /// <summary>
        /// The URL of the image.
        /// </summary>
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; set; }
        /// <summary>
        /// The title or introductory text to show before the response.
        /// </summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }
        /// <summary>
        /// The description to show with the the response.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        /// <summary>
        /// An array of objects describing the options from which the user can choose.
        /// </summary>
        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public List<DialogNodeOutputOptionsElement> Options { get; set; }
        /// <summary>
        /// A message to be sent to the human agent who will be taking over the conversation.
        /// </summary>
        [JsonProperty("message_to_human_agent", NullValueHandling = NullValueHandling.Ignore)]
        public string MessageToHumanAgent { get; set; }
        /// <summary>
        /// A label identifying the topic of the conversation, derived from the **user_label** property of the relevant
        /// node.
        /// </summary>
        [JsonProperty("topic", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string Topic { get; private set; }
        /// <summary>
        /// The ID of the dialog node that the **topic** property is taken from. The **topic** property is populated
        /// using the value of the dialog node's **user_label** property.
        /// </summary>
        [JsonProperty("dialog_node", NullValueHandling = NullValueHandling.Ignore)]
        public string DialogNode { get; set; }
    }
}
