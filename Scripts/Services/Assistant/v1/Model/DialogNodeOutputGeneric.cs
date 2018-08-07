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

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// DialogNodeOutputGeneric
    /// </summary>
    [fsObject]
    public class DialogNodeOutputGeneric
    {
        /// <summary>
        /// The type of response returned by the dialog node. The specified response type must be supported by the
        /// client application or channel.
        /// </summary>
        public enum ResponseTypeEnum
        {
            /// <summary>
            /// Enum text for text
            /// </summary>
            [EnumMember(Value = "text")]
            TEXT,
            /// <summary>
            /// Enum pause for pause
            /// </summary>
            [EnumMember(Value = "pause")]
            PAUSE,
            /// <summary>
            /// Enum image for image
            /// </summary>
            [EnumMember(Value = "image")]
            IMAGE,
            /// <summary>
            /// Enum option for option
            /// </summary>
            [EnumMember(Value = "option")]
            OPTION,
            /// <summary>
            /// Enum connectToAgent for connect_to_agent
            /// </summary>
            [EnumMember(Value = "connect_to_agent")]
            CONNECT_TO_AGENT
        }

        /// <summary>
        /// How a response is selected from the list, if more than one response is specified. Valid only when
        /// **response_type**=`text`.
        /// </summary>
        public enum SelectionPolicyEnum
        {
            /// <summary>
            /// Enum sequential for sequential
            /// </summary>
            [EnumMember(Value = "sequential")]
            SEQUENTIAL,
            /// <summary>
            /// Enum random for random
            /// </summary>
            [EnumMember(Value = "random")]
            RANDOM,
            /// <summary>
            /// Enum multiline for multiline
            /// </summary>
            [EnumMember(Value = "multiline")]
            MULTILINE
        }

        /// <summary>
        /// The preferred type of control to display, if supported by the channel. Valid only when
        /// **response_type**=`option`.
        /// </summary>
        public enum PreferenceEnum
        {
            /// <summary>
            /// Enum dropdown for dropdown
            /// </summary>
            [EnumMember(Value = "dropdown")]
            DROPDOWN,
            /// <summary>
            /// Enum button for button
            /// </summary>
            [EnumMember(Value = "button")]
            BUTTON
        }

        /// <summary>
        /// The type of response returned by the dialog node. The specified response type must be supported by the
        /// client application or channel.
        /// </summary>
        [fsProperty("response_type")]
        public ResponseTypeEnum? ResponseType { get; set; }
        /// <summary>
        /// How a response is selected from the list, if more than one response is specified. Valid only when
        /// **response_type**=`text`.
        /// </summary>
        [fsProperty("selection_policy")]
        public SelectionPolicyEnum? SelectionPolicy { get; set; }
        /// <summary>
        /// The preferred type of control to display, if supported by the channel. Valid only when
        /// **response_type**=`option`.
        /// </summary>
        [fsProperty("preference")]
        public PreferenceEnum? Preference { get; set; }
        /// <summary>
        /// A list of one or more objects defining text responses. Required when **response_type**=`text`.
        /// </summary>
        [fsProperty("values")]
        public List<DialogNodeOutputTextValuesElement> Values { get; set; }
        /// <summary>
        /// The delimiter to use as a separator between responses when `selection_policy`=`multiline`.
        /// </summary>
        [fsProperty("delimiter")]
        public string Delimiter { get; set; }
        /// <summary>
        /// How long to pause, in milliseconds. The valid values are from 0 to 10000. Valid only when
        /// **response_type**=`pause`.
        /// </summary>
        [fsProperty("time")]
        public long? Time { get; set; }
        /// <summary>
        /// Whether to send a \"user is typing\" event during the pause. Ignored if the channel does not support this
        /// event. Valid only when **response_type**=`pause`.
        /// </summary>
        [fsProperty("typing")]
        public bool? Typing { get; set; }
        /// <summary>
        /// The URL of the image. Required when **response_type**=`image`.
        /// </summary>
        [fsProperty("source")]
        public string Source { get; set; }
        /// <summary>
        /// An optional title to show before the response. Valid only when **response_type**=`image` or `option`.
        /// </summary>
        [fsProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// An optional description to show with the response. Valid only when **response_type**=`image` or `option`.
        /// </summary>
        [fsProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// An array of objects describing the options from which the user can choose. Required when
        /// **response_type**=`option`.
        /// </summary>
        [fsProperty("options")]
        public List<DialogNodeOutputOptionsElement> Options { get; set; }
        /// <summary>
        /// An optional message to be sent to the human agent who will be taking over the conversation. Valid only when
        /// **reponse_type**=`connect_to_agent`.
        /// </summary>
        [fsProperty("message_to_human_agent")]
        public string MessageToHumanAgent { get; set; }
    }

}
