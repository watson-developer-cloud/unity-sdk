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
    /// An output object that includes the response to the user, the dialog nodes that were triggered, and messages from
    /// the log.
    /// </summary>
    public class OutputData
    {
        /// <summary>
        /// An array of up to 50 messages logged with the request.
        /// </summary>
        [fsProperty("log_messages")]
        public List<LogMessage> LogMessages { get; set; }
        /// <summary>
        /// An array of responses to the user.
        /// </summary>
        [fsProperty("text")]
        public List<string> Text { get; set; }
        /// <summary>
        /// Output intended for any channel. It is the responsibility of the client application to implement the
        /// supported response types.
        /// </summary>
        [fsProperty("generic")]
        public List<DialogRuntimeResponseGeneric> Generic { get; set; }
        /// <summary>
        /// An array of the nodes that were triggered to create the response, in the order in which they were visited.
        /// This information is useful for debugging and for tracing the path taken through the node tree.
        /// </summary>
        [fsProperty("nodes_visited")]
        public List<string> NodesVisited { get; set; }
        /// <summary>
        /// An array of objects containing detailed diagnostic information about the nodes that were triggered during
        /// processing of the input message. Included only if **nodes_visited_details** is set to `true` in the message
        /// request.
        /// </summary>
        [fsProperty("nodes_visited_details")]
        public List<DialogNodeVisitedDetails> NodesVisitedDetails { get; set; }
    }


}
