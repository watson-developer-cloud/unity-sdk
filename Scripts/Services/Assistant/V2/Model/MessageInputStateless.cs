/**
* (C) Copyright IBM Corp. 2018, 2022.
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

namespace IBM.Watson.Assistant.V2.Model
{
    /// <summary>
    /// An input object that includes the input text.
    /// </summary>
    public class MessageInputStateless
    {
        /// <summary>
        /// The type of the message:
        ///
        /// - `text`: The user input is processed normally by the assistant.
        /// - `search`: Only search results are returned. (Any dialog or actions skill is bypassed.)
        ///
        /// **Note:** A `search` message results in an error if no search skill is configured for the assistant.
        /// </summary>
        public class MessageTypeValue
        {
            /// <summary>
            /// Constant TEXT for text
            /// </summary>
            public const string TEXT = "text";
            /// <summary>
            /// Constant SEARCH for search
            /// </summary>
            public const string SEARCH = "search";
            
        }

        /// <summary>
        /// The type of the message:
        ///
        /// - `text`: The user input is processed normally by the assistant.
        /// - `search`: Only search results are returned. (Any dialog or actions skill is bypassed.)
        ///
        /// **Note:** A `search` message results in an error if no search skill is configured for the assistant.
        /// Constants for possible values can be found using MessageInputStateless.MessageTypeValue
        /// </summary>
        [JsonProperty("message_type", NullValueHandling = NullValueHandling.Ignore)]
        public string MessageType { get; set; }
        /// <summary>
        /// The text of the user input. This string cannot contain carriage return, newline, or tab characters.
        /// </summary>
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
        /// <summary>
        /// Intents to use when evaluating the user input. Include intents from the previous response to continue using
        /// those intents rather than trying to recognize intents in the new input.
        /// </summary>
        [JsonProperty("intents", NullValueHandling = NullValueHandling.Ignore)]
        public List<RuntimeIntent> Intents { get; set; }
        /// <summary>
        /// Entities to use when evaluating the message. Include entities from the previous response to continue using
        /// those entities rather than detecting entities in the new input.
        /// </summary>
        [JsonProperty("entities", NullValueHandling = NullValueHandling.Ignore)]
        public List<RuntimeEntity> Entities { get; set; }
        /// <summary>
        /// For internal use only.
        /// </summary>
        [JsonProperty("suggestion_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SuggestionId { get; set; }
        /// <summary>
        /// An array of multimedia attachments to be sent with the message.
        ///
        /// **Note:** Attachments are not processed by the assistant itself, but can be sent to external services by
        /// webhooks.
        /// </summary>
        [JsonProperty("attachments", NullValueHandling = NullValueHandling.Ignore)]
        public List<MessageInputAttachment> Attachments { get; set; }
        /// <summary>
        /// Optional properties that control how the assistant responds.
        /// </summary>
        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public MessageInputOptionsStateless Options { get; set; }
    }
}
