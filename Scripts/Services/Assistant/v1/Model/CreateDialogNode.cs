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
    /// CreateDialogNode.
    /// </summary>
    public class CreateDialogNode
    {
        /// <summary>
        /// How the dialog node is processed.
        /// </summary>
        public class NodeTypeEnumValue
        {
            /// <summary>
            /// Constant STANDARD for standard
            /// </summary>
            public const string STANDARD = "standard";
            /// <summary>
            /// Constant EVENT_HANDLER for event_handler
            /// </summary>
            public const string EVENT_HANDLER = "event_handler";
            /// <summary>
            /// Constant FRAME for frame
            /// </summary>
            public const string FRAME = "frame";
            /// <summary>
            /// Constant SLOT for slot
            /// </summary>
            public const string SLOT = "slot";
            /// <summary>
            /// Constant RESPONSE_CONDITION for response_condition
            /// </summary>
            public const string RESPONSE_CONDITION = "response_condition";
            /// <summary>
            /// Constant FOLDER for folder
            /// </summary>
            public const string FOLDER = "folder";
            
        }

        /// <summary>
        /// How an `event_handler` node is processed.
        /// </summary>
        public class EventNameEnumValue
        {
            /// <summary>
            /// Constant FOCUS for focus
            /// </summary>
            public const string FOCUS = "focus";
            /// <summary>
            /// Constant INPUT for input
            /// </summary>
            public const string INPUT = "input";
            /// <summary>
            /// Constant FILLED for filled
            /// </summary>
            public const string FILLED = "filled";
            /// <summary>
            /// Constant VALIDATE for validate
            /// </summary>
            public const string VALIDATE = "validate";
            /// <summary>
            /// Constant FILLED_MULTIPLE for filled_multiple
            /// </summary>
            public const string FILLED_MULTIPLE = "filled_multiple";
            /// <summary>
            /// Constant GENERIC for generic
            /// </summary>
            public const string GENERIC = "generic";
            /// <summary>
            /// Constant NOMATCH for nomatch
            /// </summary>
            public const string NOMATCH = "nomatch";
            /// <summary>
            /// Constant NOMATCH_RESPONSES_DEPLETED for nomatch_responses_depleted
            /// </summary>
            public const string NOMATCH_RESPONSES_DEPLETED = "nomatch_responses_depleted";
            /// <summary>
            /// Constant DIGRESSION_RETURN_PROMPT for digression_return_prompt
            /// </summary>
            public const string DIGRESSION_RETURN_PROMPT = "digression_return_prompt";
            
        }

        /// <summary>
        /// Whether this top-level dialog node can be digressed into.
        /// </summary>
        public class DigressInEnumValue
        {
            /// <summary>
            /// Constant NOT_AVAILABLE for not_available
            /// </summary>
            public const string NOT_AVAILABLE = "not_available";
            /// <summary>
            /// Constant RETURNS for returns
            /// </summary>
            public const string RETURNS = "returns";
            /// <summary>
            /// Constant DOES_NOT_RETURN for does_not_return
            /// </summary>
            public const string DOES_NOT_RETURN = "does_not_return";
            
        }

        /// <summary>
        /// Whether this dialog node can be returned to after a digression.
        /// </summary>
        public class DigressOutEnumValue
        {
            /// <summary>
            /// Constant ALLOW_RETURNING for allow_returning
            /// </summary>
            public const string ALLOW_RETURNING = "allow_returning";
            /// <summary>
            /// Constant ALLOW_ALL for allow_all
            /// </summary>
            public const string ALLOW_ALL = "allow_all";
            /// <summary>
            /// Constant ALLOW_ALL_NEVER_RETURN for allow_all_never_return
            /// </summary>
            public const string ALLOW_ALL_NEVER_RETURN = "allow_all_never_return";
            
        }

        /// <summary>
        /// Whether the user can digress to top-level nodes while filling out slots.
        /// </summary>
        public class DigressOutSlotsEnumValue
        {
            /// <summary>
            /// Constant NOT_ALLOWED for not_allowed
            /// </summary>
            public const string NOT_ALLOWED = "not_allowed";
            /// <summary>
            /// Constant ALLOW_RETURNING for allow_returning
            /// </summary>
            public const string ALLOW_RETURNING = "allow_returning";
            /// <summary>
            /// Constant ALLOW_ALL for allow_all
            /// </summary>
            public const string ALLOW_ALL = "allow_all";
            
        }

        /// <summary>
        /// How the dialog node is processed.
        /// </summary>
        [fsProperty("type")]
        public string NodeType { get; set; }
        /// <summary>
        /// How an `event_handler` node is processed.
        /// </summary>
        [fsProperty("event_name")]
        public string EventName { get; set; }
        /// <summary>
        /// Whether this top-level dialog node can be digressed into.
        /// </summary>
        [fsProperty("digress_in")]
        public string DigressIn { get; set; }
        /// <summary>
        /// Whether this dialog node can be returned to after a digression.
        /// </summary>
        [fsProperty("digress_out")]
        public string DigressOut { get; set; }
        /// <summary>
        /// Whether the user can digress to top-level nodes while filling out slots.
        /// </summary>
        [fsProperty("digress_out_slots")]
        public string DigressOutSlots { get; set; }
        /// <summary>
        /// The dialog node ID. This string must conform to the following restrictions:
        /// - It can contain only Unicode alphanumeric, space, underscore, hyphen, and dot characters.
        /// - It must be no longer than 1024 characters.
        /// </summary>
        [fsProperty("dialog_node")]
        public string DialogNode { get; set; }
        /// <summary>
        /// The description of the dialog node. This string cannot contain carriage return, newline, or tab characters,
        /// and it must be no longer than 128 characters.
        /// </summary>
        [fsProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// The condition that will trigger the dialog node. This string cannot contain carriage return, newline, or tab
        /// characters, and it must be no longer than 2048 characters.
        /// </summary>
        [fsProperty("conditions")]
        public string Conditions { get; set; }
        /// <summary>
        /// The ID of the parent dialog node.
        /// </summary>
        [fsProperty("parent")]
        public string Parent { get; set; }
        /// <summary>
        /// The ID of the previous dialog node.
        /// </summary>
        [fsProperty("previous_sibling")]
        public string PreviousSibling { get; set; }
        /// <summary>
        /// The output of the dialog node. For more information about how to specify dialog node output, see the
        /// [documentation](https://cloud.ibm.com/docs/services/assistant/dialog-overview.html#complex).
        /// </summary>
        [fsProperty("output")]
        public DialogNodeOutput Output { get; set; }
        /// <summary>
        /// The context for the dialog node.
        /// </summary>
        [fsProperty("context")]
        public object Context { get; set; }
        /// <summary>
        /// The metadata for the dialog node.
        /// </summary>
        [fsProperty("metadata")]
        public object Metadata { get; set; }
        /// <summary>
        /// The next step to execute following this dialog node.
        /// </summary>
        [fsProperty("next_step")]
        public DialogNodeNextStep NextStep { get; set; }
        /// <summary>
        /// An array of objects describing any actions to be invoked by the dialog node.
        /// </summary>
        [fsProperty("actions")]
        public List<DialogNodeAction> Actions { get; set; }
        /// <summary>
        /// The alias used to identify the dialog node. This string must conform to the following restrictions:
        /// - It can contain only Unicode alphanumeric, space, underscore, hyphen, and dot characters.
        /// - It must be no longer than 64 characters.
        /// </summary>
        [fsProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// The location in the dialog context where output is stored.
        /// </summary>
        [fsProperty("variable")]
        public string Variable { get; set; }
        /// <summary>
        /// A label that can be displayed externally to describe the purpose of the node to users. This string must be
        /// no longer than 512 characters.
        /// </summary>
        [fsProperty("user_label")]
        public string UserLabel { get; set; }
    }


}
