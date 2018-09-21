

using FullSerializer;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
namespace IBM.WatsonDeveloperCloud.Assistant.v2
{
    /// <summary>
    /// Additional detailed information about a message response and how it was generated.
    /// </summary>
    [fsObject]
    public class MessageOutputDebug
    {
        /// <summary>
        /// When `branch_exited` is set to `true` by the Assistant, the `branch_exited_reason` specifies whether the
        /// dialog completed by itself or got interrupted.
        /// </summary>
        public enum BranchExitedReasonEnum
        {
            /// <summary>
            /// Enum completed for completed
            /// </summary>
            [EnumMember(Value = "completed")]
            completed,
            /// <summary>
            /// Enum fallback for fallback
            /// </summary>
            [EnumMember(Value = "fallback")]
            fallback
        }

        /// <summary>
        /// When `branch_exited` is set to `true` by the Assistant, the `branch_exited_reason` specifies whether the
        /// dialog completed by itself or got interrupted.
        /// </summary>
        [fsProperty("branch_exited_reason")]
        public BranchExitedReasonEnum? BranchExitedReason { get; set; }
        /// <summary>
        /// An array of objects containing detailed diagnostic information about the nodes that were triggered during
        /// processing of the input message.
        /// </summary>
        [fsProperty("nodes_visited")]
        public List<DialogNodesVisited> NodesVisited { get; set; }
        /// <summary>
        /// An array of up to 50 messages logged with the request.
        /// </summary>
        [fsProperty("log_messages")]
        public List<DialogLogMessage> LogMessages { get; set; }
        /// <summary>
        /// Assistant sets this to true when this message response concludes or interrupts a dialog.
        /// </summary>
        [fsProperty("branch_exited")]
        public bool? BranchExited { get; set; }
    }

}
