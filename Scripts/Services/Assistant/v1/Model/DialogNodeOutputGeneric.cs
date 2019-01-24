/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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
using FullSerializer;

namespace IBM.Watson.Assistant.v1.Model
{
    /// <summary>
    /// DialogNodeOutputGeneric.
    /// </summary>
    public class DialogNodeOutputGeneric
    {
        /// <summary>
        /// The type of response returned by the dialog node. The specified response type must be supported by the
        /// client application or channel.
        /// </summary>
        public class ResponseTypeEnumValue
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
            
        }

        /// <summary>
        /// How a response is selected from the list, if more than one response is specified. Valid only when
        /// **response_type**=`text`.
        /// </summary>
        public class SelectionPolicyEnumValue
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
        /// The preferred type of control to display, if supported by the channel. Valid only when
        /// **response_type**=`option`.
        /// </summary>
        public class PreferenceEnumValue
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
        /// </summary>
        [fsProperty("response_type")]
        public string ResponseType { get; set; }
        /// <summary>
        /// How a response is selected from the list, if more than one response is specified. Valid only when
        /// **response_type**=`text`.
        /// </summary>
        [fsProperty("selection_policy")]
        public string SelectionPolicy { get; set; }
        /// <summary>
        /// The preferred type of control to display, if supported by the channel. Valid only when
        /// **response_type**=`option`.
        /// </summary>
        [fsProperty("preference")]
        public string Preference { get; set; }
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
        /// An optional title to show before the response. Valid only when **response_type**=`image` or `option`. This
        /// string must be no longer than 512 characters.
        /// </summary>
        [fsProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// An optional description to show with the response. Valid only when **response_type**=`image` or `option`.
        /// This string must be no longer than 256 characters.
        /// </summary>
        [fsProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// An array of objects describing the options from which the user can choose. You can include up to 20 options.
        /// Required when **response_type**=`option`.
        /// </summary>
        [fsProperty("options")]
        public List<DialogNodeOutputOptionsElement> Options { get; set; }
        /// <summary>
        /// An optional message to be sent to the human agent who will be taking over the conversation. Valid only when
        /// **reponse_type**=`connect_to_agent`. This string must be no longer than 256 characters.
        /// </summary>
        [fsProperty("message_to_human_agent")]
        public string MessageToHumanAgent { get; set; }
    }


}
