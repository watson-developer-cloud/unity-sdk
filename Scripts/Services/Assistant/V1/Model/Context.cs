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

using IBM.Cloud.SDK.Model;
using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// State information for the conversation. To maintain state, include the context from the previous response.
    /// </summary>
    public class Context: DynamicModel<object>
    {
        /// <summary>
        /// The unique identifier of the conversation.
        /// </summary>
        [JsonProperty("conversation_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ConversationId { get; set; }
        /// <summary>
        /// For internal use only.
        /// </summary>
        [JsonProperty("system", NullValueHandling = NullValueHandling.Ignore)]
        public SystemResponse System { get; set; }
        /// <summary>
        /// Metadata related to the message.
        /// </summary>
        [JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
        public MessageContextMetadata Metadata { get; set; }
    }
}
