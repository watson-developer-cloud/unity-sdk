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

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// A response from the Assistant service.
    /// </summary>
    [fsObject]
    public class MessageResponse
    {
        /// <summary>
        /// The user input from the request.
        /// </summary>
        /// <value>The user input from the request.</value>
        [fsProperty("input")]
        public object Input { get; set; }
        /// <summary>
        /// An array of intents recognized in the user input, sorted in descending order of confidence.
        /// </summary>
        /// <value>An array of intents recognized in the user input, sorted in descending order of confidence.</value>
        [fsProperty("intents")]
        public object Intents { get; set; }
        /// <summary>
        /// An array of entities identified in the user input.
        /// </summary>
        /// <value>An array of entities identified in the user input.</value>
        [fsProperty("entities")]
        public object Entities { get; set; }
        /// <summary>
        /// Whether to return more than one intent. A value of `true` indicates that all matching intents are returned.
        /// </summary>
        /// <value>Whether to return more than one intent. A value of `true` indicates that all matching intents are returned.</value>
        [fsProperty("alternate_intents")]
        public object AlternateIntents { get; set; }
        /// <summary>
        /// State information for the conversation.
        /// </summary>
        /// <value>State information for the conversation.</value>
        [fsProperty("context")]
        public object Context { get; set; }
        /// <summary>
        /// Output from the dialog, including the response to the user, the nodes that were triggered, and log messages.
        /// </summary>
        /// <value>Output from the dialog, including the response to the user, the nodes that were triggered, and log messages.</value>
        [fsProperty("output")]
        public object Output { get; set; }
        /// <summary>
        /// An array of objects describing any actions requested by the dialog node.
        /// </summary>
        [fsProperty("actions")]
        public object Actions { get; set; }
    }

}
