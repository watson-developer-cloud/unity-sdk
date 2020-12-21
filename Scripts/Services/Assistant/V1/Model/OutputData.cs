/**
* (C) Copyright IBM Corp. 2019, 2020.
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
using IBM.Cloud.SDK.Model;
using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// An output object that includes the response to the user, the dialog nodes that were triggered, and messages from
    /// the log.
    /// </summary>
    public class OutputData : DynamicModel<object>
    {
        /// <summary>
        /// An array of the nodes that were triggered to create the response, in the order in which they were visited.
        /// This information is useful for debugging and for tracing the path taken through the node tree.
        /// </summary>
        [JsonProperty("nodes_visited", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> NodesVisited { get; set; }
        /// <summary>
        /// An array of objects containing detailed diagnostic information about the nodes that were triggered during
        /// processing of the input message. Included only if **nodes_visited_details** is set to `true` in the message
        /// request.
        /// </summary>
        [JsonProperty("nodes_visited_details", NullValueHandling = NullValueHandling.Ignore)]
        public List<DialogNodeVisitedDetails> NodesVisitedDetails { get; set; }
        /// <summary>
        /// An array of up to 50 messages logged with the request.
        /// </summary>
        [JsonProperty("log_messages", NullValueHandling = NullValueHandling.Ignore)]
        public List<LogMessage> LogMessages { get; set; }
        /// <summary>
        /// An array of responses to the user.
        /// </summary>
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Text { get; set; }
        /// <summary>
        /// Output intended for any channel. It is the responsibility of the client application to implement the
        /// supported response types.
        /// </summary>
        [JsonProperty("generic", NullValueHandling = NullValueHandling.Ignore)]
        public List<RuntimeResponseGeneric> Generic { get; set; }
    }
}
