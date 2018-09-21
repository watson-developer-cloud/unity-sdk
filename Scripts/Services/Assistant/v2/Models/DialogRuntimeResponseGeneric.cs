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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IBM.WatsonDeveloperCloud.Assistant.v2
{
    /// <summary>
    /// DialogRuntimeResponseGeneric
    /// </summary>
    [fsObject]
    public class DialogRuntimeResponseGeneric
    {
        /// <summary>
        /// The type of response returned by the dialog node. The specified response type must be supported by the
        /// client application or channel.
        ///
        /// **Note:** The **suggestion** response type is part of the disambiguation feature, which is only available
        /// for Premium users.
        /// </summary>
        public enum ResponseTypeEnum
        {
            /// <summary>
            /// Enum text for text
            /// </summary>
            [EnumMember(Value = "text")]
            text,
            /// <summary>
            /// Enum pause for pause
            /// </summary>
            [EnumMember(Value = "pause")]
            pause,
            /// <summary>
            /// Enum image for image
            /// </summary>
            [EnumMember(Value = "image")]
            image,
            /// <summary>
            /// Enum option for option
            /// </summary>
            [EnumMember(Value = "option")]
            option,
            /// <summary>
            /// Enum connectToAgent for connect_to_agent
            /// </summary>
            [EnumMember(Value = "connect_to_agent")]
            connectToAgent,
            /// <summary>
            /// Enum suggestion for suggestion
            /// </summary>
            [EnumMember(Value = "suggestion")]
            suggestion
        }

        /// <summary>
        /// The preferred type of control to display.
        /// </summary>
        public enum PreferenceEnum
        {
            /// <summary>
            /// Enum dropdown for dropdown
            /// </summary>
            [EnumMember(Value = "dropdown")]
            dropdown,
            /// <summary>
            /// Enum button for button
            /// </summary>
            [EnumMember(Value = "button")]
            button
        }

        /// <summary>
        /// The type of response returned by the dialog node. The specified response type must be supported by the
        /// client application or channel.
        ///
        /// **Note:** The **suggestion** response type is part of the disambiguation feature, which is only available
        /// for Premium users.
        /// </summary>
        [fsProperty("response_type")]
        public ResponseTypeEnum? ResponseType { get; set; }
        /// <summary>
        /// The preferred type of control to display.
        /// </summary>
        [fsProperty("preference")]
        public PreferenceEnum? Preference { get; set; }
        /// <summary>
        /// The text of the response.
        /// </summary>
        [fsProperty("text")]
        public string Text { get; set; }
        /// <summary>
        /// How long to pause, in milliseconds.
        /// </summary>
        [fsProperty("time")]
        public long? Time { get; set; }
        /// <summary>
        /// Whether to send a \"user is typing\" event during the pause.
        /// </summary>
        [fsProperty("typing")]
        public bool? Typing { get; set; }
        /// <summary>
        /// The URL of the image.
        /// </summary>
        [fsProperty("source")]
        public string Source { get; set; }
        /// <summary>
        /// The title to show before the response.
        /// </summary>
        [fsProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// The description to show with the the response.
        /// </summary>
        [fsProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// An array of objects describing the options from which the user can choose.
        /// </summary>
        [fsProperty("options")]
        public List<DialogNodeOutputOptionsElement> Options { get; set; }
        /// <summary>
        /// A message to be sent to the human agent who will be taking over the conversation.
        /// </summary>
        [fsProperty("message_to_human_agent")]
        public string MessageToHumanAgent { get; set; }
        /// <summary>
        /// A label identifying the topic of the conversation, derived from the **user_label** property of the relevant
        /// node.
        /// </summary>
        [fsProperty("topic")]
        public virtual string Topic { get; private set; }
        /// <summary>
        /// An array of objects describing the possible matching dialog nodes from which the user can choose.
        ///
        /// **Note:** The **suggestions** property is part of the disambiguation feature, which is only available for
        /// Premium users.
        /// </summary>
        [fsProperty("suggestions")]
        public List<DialogSuggestion> Suggestions { get; set; }
    }

}
