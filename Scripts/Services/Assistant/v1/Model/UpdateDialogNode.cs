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
using System.Runtime.Serialization;

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// UpdateDialogNode.
    /// </summary>
    [fsObject]
    public class UpdateDialogNode
    {
        /// <summary>
        /// How the dialog node is processed.
        /// </summary>
        /// <value>How the dialog node is processed.</value>
        public enum NodeTypeEnum
        {
            
            /// <summary>
            /// Enum STANDARD for standard
            /// </summary>
            [EnumMember(Value = "standard")]
            STANDARD,
            
            /// <summary>
            /// Enum EVENT_HANDLER for event_handler
            /// </summary>
            [EnumMember(Value = "event_handler")]
            EVENT_HANDLER,
            
            /// <summary>
            /// Enum FRAME for frame
            /// </summary>
            [EnumMember(Value = "frame")]
            FRAME,
            
            /// <summary>
            /// Enum SLOT for slot
            /// </summary>
            [EnumMember(Value = "slot")]
            SLOT,
            
            /// <summary>
            /// Enum RESPONSE_CONDITION for response_condition
            /// </summary>
            [EnumMember(Value = "response_condition")]
            RESPONSE_CONDITION
        }

        /// <summary>
        /// How an `event_handler` node is processed.
        /// </summary>
        /// <value>How an `event_handler` node is processed.</value>
        public enum EventNameEnum
        {
            
            /// <summary>
            /// Enum FOCUS for focus
            /// </summary>
            [EnumMember(Value = "focus")]
            FOCUS,
            
            /// <summary>
            /// Enum INPUT for input
            /// </summary>
            [EnumMember(Value = "input")]
            INPUT,
            
            /// <summary>
            /// Enum FILLED for filled
            /// </summary>
            [EnumMember(Value = "filled")]
            FILLED,
            
            /// <summary>
            /// Enum VALIDATE for validate
            /// </summary>
            [EnumMember(Value = "validate")]
            VALIDATE,
            
            /// <summary>
            /// Enum FILLED_MULTIPLE for filled_multiple
            /// </summary>
            [EnumMember(Value = "filled_multiple")]
            FILLED_MULTIPLE,
            
            /// <summary>
            /// Enum GENERIC for generic
            /// </summary>
            [EnumMember(Value = "generic")]
            GENERIC,
            
            /// <summary>
            /// Enum NOMATCH for nomatch
            /// </summary>
            [EnumMember(Value = "nomatch")]
            NOMATCH,
            
            /// <summary>
            /// Enum NOMATCH_RESPONSES_DEPLETED for nomatch_responses_depleted
            /// </summary>
            [EnumMember(Value = "nomatch_responses_depleted")]
            NOMATCH_RESPONSES_DEPLETED
        }

        /// <summary>
        /// How the dialog node is processed.
        /// </summary>
        /// <value>How the dialog node is processed.</value>
        public NodeTypeEnum? NodeType { get; set; }
        /// <summary>
        /// How an `event_handler` node is processed.
        /// </summary>
        /// <value>How an `event_handler` node is processed.</value>
        public EventNameEnum? EventName { get; set; }
        /// <summary>
        /// The dialog node ID.
        /// </summary>
        /// <value>The dialog node ID.</value>
        public string DialogNode { get; set; }
        /// <summary>
        /// The description of the dialog node.
        /// </summary>
        /// <value>The description of the dialog node.</value>
        public string Description { get; set; }
        /// <summary>
        /// The condition that will trigger the dialog node.
        /// </summary>
        /// <value>The condition that will trigger the dialog node.</value>
        public string Conditions { get; set; }
        /// <summary>
        /// The ID of the parent dialog node (if any).
        /// </summary>
        /// <value>The ID of the parent dialog node (if any).</value>
        public string Parent { get; set; }
        /// <summary>
        /// The previous dialog node.
        /// </summary>
        /// <value>The previous dialog node.</value>
        public string PreviousSibling { get; set; }
        /// <summary>
        /// The output of the dialog node.
        /// </summary>
        /// <value>The output of the dialog node.</value>
        public object Output { get; set; }
        /// <summary>
        /// The context for the dialog node.
        /// </summary>
        /// <value>The context for the dialog node.</value>
        public object Context { get; set; }
        /// <summary>
        /// The metadata for the dialog node.
        /// </summary>
        /// <value>The metadata for the dialog node.</value>
        public object Metadata { get; set; }
        /// <summary>
        /// The next step to execute following this dialog node.
        /// </summary>
        /// <value>The next step to execute following this dialog node.</value>
        public DialogNodeNextStep NextStep { get; set; }
        /// <summary>
        /// The alias used to identify the dialog node.
        /// </summary>
        /// <value>The alias used to identify the dialog node.</value>
        public string Title { get; set; }
        /// <summary>
        /// The location in the dialog context where output is stored.
        /// </summary>
        /// <value>The location in the dialog context where output is stored.</value>
        public string Variable { get; set; }
        /// <summary>
        /// The actions for the dialog node.
        /// </summary>
        /// <value>The actions for the dialog node.</value>
        public List<DialogNodeAction> Actions { get; set; }
    }

}
