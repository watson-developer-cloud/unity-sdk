/**
* (C) Copyright IBM Corp. 2019, 2021.
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
    /// The response sent by the workspace, including the output text, detected intents and entities, and context.
    /// </summary>
    public class MessageResponse
    {
        /// <summary>
        /// An input object that includes the input text.
        /// </summary>
        [JsonProperty("input", NullValueHandling = NullValueHandling.Ignore)]
        public MessageInput Input { get; set; }
        /// <summary>
        /// An array of intents recognized in the user input, sorted in descending order of confidence.
        /// </summary>
        [JsonProperty("intents", NullValueHandling = NullValueHandling.Ignore)]
        public List<RuntimeIntent> Intents { get; set; }
        /// <summary>
        /// An array of entities identified in the user input.
        /// </summary>
        [JsonProperty("entities", NullValueHandling = NullValueHandling.Ignore)]
        public List<RuntimeEntity> Entities { get; set; }
        /// <summary>
        /// Whether to return more than one intent. A value of `true` indicates that all matching intents are returned.
        /// </summary>
        [JsonProperty("alternate_intents", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AlternateIntents { get; set; }
        /// <summary>
        /// State information for the conversation. To maintain state, include the context from the previous response.
        /// </summary>
        [JsonProperty("context", NullValueHandling = NullValueHandling.Ignore)]
        public Context Context { get; set; }
        /// <summary>
        /// An output object that includes the response to the user, the dialog nodes that were triggered, and messages
        /// from the log.
        /// </summary>
        [JsonProperty("output", NullValueHandling = NullValueHandling.Ignore)]
        public OutputData Output { get; set; }
        /// <summary>
        /// An array of objects describing any actions requested by the dialog node.
        /// </summary>
        [JsonProperty("actions", NullValueHandling = NullValueHandling.Ignore)]
        public virtual List<DialogNodeAction> Actions { get; private set; }
        /// <summary>
        /// A string value that identifies the user who is interacting with the workspace. The client must provide a
        /// unique identifier for each individual end user who accesses the application. For user-based plans, this user
        /// ID is used to identify unique users for billing purposes. This string cannot contain carriage return,
        /// newline, or tab characters. If no value is specified in the input, **user_id** is automatically set to the
        /// value of **context.conversation_id**.
        ///
        /// **Note:** This property is the same as the **user_id** property in the context metadata. If **user_id** is
        /// specified in both locations in a message request, the value specified at the root is used.
        /// </summary>
        [JsonProperty("user_id", NullValueHandling = NullValueHandling.Ignore)]
        public string UserId { get; set; }
    }
}
