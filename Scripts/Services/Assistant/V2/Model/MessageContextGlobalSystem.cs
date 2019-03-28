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

using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V2.Model
{
    /// <summary>
    /// Built-in system properties that apply to all skills used by the assistant.
    /// </summary>
    public class MessageContextGlobalSystem
    {
        /// <summary>
        /// The user time zone. The assistant uses the time zone to correctly resolve relative time references.
        /// </summary>
        [JsonProperty("timezone", NullValueHandling = NullValueHandling.Ignore)]
        public string Timezone { get; set; }
        /// <summary>
        /// A string value that identifies the user who is interacting with the assistant. The client must provide a
        /// unique identifier for each individual end user who accesses the application. For Plus and Premium plans,
        /// this user ID is used to identify unique users for billing purposes. This string cannot contain carriage
        /// return, newline, or tab characters.
        /// </summary>
        [JsonProperty("user_id", NullValueHandling = NullValueHandling.Ignore)]
        public string UserId { get; set; }
        /// <summary>
        /// A counter that is automatically incremented with each turn of the conversation. A value of 1 indicates that
        /// this is the the first turn of a new conversation, which can affect the behavior of some skills (for example,
        /// triggering the start node of a dialog).
        /// </summary>
        [JsonProperty("turn_count", NullValueHandling = NullValueHandling.Ignore)]
        public long? TurnCount { get; set; }
    }
}
