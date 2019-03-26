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

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// ModelLog.
    /// </summary>
    public class ModelLog
    {
        /// <summary>
        /// A request sent to the workspace, including the user input and context.
        /// </summary>
        [JsonProperty("request", NullValueHandling = NullValueHandling.Ignore)]
        public MessageRequest Request { get; set; }
        /// <summary>
        /// The response sent by the workspace, including the output text, detected intents and entities, and context.
        /// </summary>
        [JsonProperty("response", NullValueHandling = NullValueHandling.Ignore)]
        public MessageResponse Response { get; set; }
        /// <summary>
        /// A unique identifier for the logged event.
        /// </summary>
        [JsonProperty("log_id", NullValueHandling = NullValueHandling.Ignore)]
        public string LogId { get; set; }
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
        /// The unique identifier of the workspace where the request was made.
        /// </summary>
        [JsonProperty("workspace_id", NullValueHandling = NullValueHandling.Ignore)]
        public string WorkspaceId { get; set; }
        /// <summary>
        /// The language of the workspace where the message request was made.
        /// </summary>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }
    }
}
