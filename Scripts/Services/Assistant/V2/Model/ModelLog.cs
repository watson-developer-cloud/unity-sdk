/**
* (C) Copyright IBM Corp. 2020.
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
    /// ModelLog.
    /// </summary>
    public class ModelLog
    {
        /// <summary>
        /// A unique identifier for the logged event.
        /// </summary>
        [JsonProperty("log_id", NullValueHandling = NullValueHandling.Ignore)]
        public string LogId { get; set; }
        /// <summary>
        /// A stateful message request formatted for the Watson Assistant service.
        /// </summary>
        [JsonProperty("request", NullValueHandling = NullValueHandling.Ignore)]
        public MessageRequest Request { get; set; }
        /// <summary>
        /// A response from the Watson Assistant service.
        /// </summary>
        [JsonProperty("response", NullValueHandling = NullValueHandling.Ignore)]
        public MessageResponse Response { get; set; }
        /// <summary>
        /// Unique identifier of the assistant.
        /// </summary>
        [JsonProperty("assistant_id", NullValueHandling = NullValueHandling.Ignore)]
        public string AssistantId { get; set; }
        /// <summary>
        /// The ID of the session the message was part of.
        /// </summary>
        [JsonProperty("session_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SessionId { get; set; }
        /// <summary>
        /// The unique identifier of the skill that responded to the message.
        /// </summary>
        [JsonProperty("skill_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SkillId { get; set; }
        /// <summary>
        /// The name of the snapshot (dialog skill version) that responded to the message (for example, `draft`).
        /// </summary>
        [JsonProperty("snapshot", NullValueHandling = NullValueHandling.Ignore)]
        public string Snapshot { get; set; }
        /// <summary>
        /// The timestamp for receipt of the message.
        /// </summary>
        [JsonProperty("request_timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public string RequestTimestamp { get; set; }
        /// <summary>
        /// The timestamp for the system response to the message.
        /// </summary>
        [JsonProperty("response_timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public string ResponseTimestamp { get; set; }
        /// <summary>
        /// The language of the assistant to which the message request was made.
        /// </summary>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }
        /// <summary>
        /// The customer ID specified for the message, if any.
        /// </summary>
        [JsonProperty("customer_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CustomerId { get; set; }
    }
}
