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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V2.Model
{
    /// <summary>
    /// Additional detailed information about a message response and how it was generated.
    /// </summary>
    public class MessageOutputDebug
    {
        /// <summary>
        /// When `branch_exited` is set to `true` by the Assistant, the `branch_exited_reason` specifies whether the
        /// dialog completed by itself or got interrupted.
        /// </summary>
        public class BranchExitedReasonValue
        {
            /// <summary>
            /// Constant COMPLETED for completed
            /// </summary>
            public const string COMPLETED = "completed";
            /// <summary>
            /// Constant FALLBACK for fallback
            /// </summary>
            public const string FALLBACK = "fallback";
            
        }

        /// <summary>
        /// When `branch_exited` is set to `true` by the Assistant, the `branch_exited_reason` specifies whether the
        /// dialog completed by itself or got interrupted.
        /// Constants for possible values can be found using MessageOutputDebug.BranchExitedReasonValue
        /// </summary>
        [JsonProperty("branch_exited_reason", NullValueHandling = NullValueHandling.Ignore)]
        public string BranchExitedReason { get; set; }
        /// <summary>
        /// An array of objects containing detailed diagnostic information about the nodes that were triggered during
        /// processing of the input message.
        /// </summary>
        [JsonProperty("nodes_visited", NullValueHandling = NullValueHandling.Ignore)]
        public List<DialogNodesVisited> NodesVisited { get; set; }
        /// <summary>
        /// An array of up to 50 messages logged with the request.
        /// </summary>
        [JsonProperty("log_messages", NullValueHandling = NullValueHandling.Ignore)]
        public List<DialogLogMessage> LogMessages { get; set; }
        /// <summary>
        /// Assistant sets this to true when this message response concludes or interrupts a dialog.
        /// </summary>
        [JsonProperty("branch_exited", NullValueHandling = NullValueHandling.Ignore)]
        public bool? BranchExited { get; set; }
    }
}
