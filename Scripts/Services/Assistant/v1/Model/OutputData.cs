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
    /// An output object that includes the response to the user, the nodes that were hit, and messages from the log.
    /// </summary>
    [fsObject]
    public class OutputData
    {
        /// <summary>
        /// An array of up to 50 messages logged with the request.
        /// </summary>
        /// <value>An array of up to 50 messages logged with the request.</value>
        [fsProperty("log_messages")]
        public Dictionary<string, object> LogMessages { get; set; }
        /// <summary>
        /// An array of responses to the user.
        /// </summary>
        /// <value>An array of responses to the user.</value>
        [fsProperty("text")]
        public Dictionary<string, object> Text { get; set; }
        /// <summary>
        /// An array of the nodes that were triggered to create the response, in the order in which they were visited. This information is useful for debugging and for tracing the path taken through the node tree.
        /// </summary>
        /// <value>An array of the nodes that were triggered to create the response, in the order in which they were visited. This information is useful for debugging and for tracing the path taken through the node tree.</value>
        [fsProperty("nodes_visited")]
        public Dictionary<string, object> NodesVisited { get; set; }
        /// <summary>
        /// An array of objects containing detailed diagnostic information about the nodes that were triggered during processing of the input message. Included only if **nodes_visited_details** is set to `true` in the message request.
        /// </summary>
        /// <value>An array of objects containing detailed diagnostic information about the nodes that were triggered during processing of the input message. Included only if **nodes_visited_details** is set to `true` in the message request.</value>
        [fsProperty("nodes_visited_details")]
        public Dictionary<string, object> NodesVisitedDetails { get; set; }
    }

}
