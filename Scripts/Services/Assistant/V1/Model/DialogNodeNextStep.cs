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
    /// The next step to execute following this dialog node.
    /// </summary>
    public class DialogNodeNextStep
    {
        /// <summary>
        /// What happens after the dialog node completes. The valid values depend on the node type:
        /// - The following values are valid for any node:
        ///   - `get_user_input`
        ///   - `skip_user_input`
        ///   - `jump_to`
        /// - If the node is of type `event_handler` and its parent node is of type `slot` or `frame`, additional values
        /// are also valid:
        ///   - if **event_name**=`filled` and the type of the parent node is `slot`:
        ///     - `reprompt`
        ///     - `skip_all_slots`
        /// - if **event_name**=`nomatch` and the type of the parent node is `slot`:
        ///     - `reprompt`
        ///     - `skip_slot`
        ///     - `skip_all_slots`
        /// - if **event_name**=`generic` and the type of the parent node is `frame`:
        ///     - `reprompt`
        ///     - `skip_slot`
        ///     - `skip_all_slots`
        ///      If you specify `jump_to`, then you must also specify a value for the `dialog_node` property.
        /// </summary>
        public class BehaviorValue
        {
            /// <summary>
            /// Constant GET_USER_INPUT for get_user_input
            /// </summary>
            public const string GET_USER_INPUT = "get_user_input";
            /// <summary>
            /// Constant SKIP_USER_INPUT for skip_user_input
            /// </summary>
            public const string SKIP_USER_INPUT = "skip_user_input";
            /// <summary>
            /// Constant JUMP_TO for jump_to
            /// </summary>
            public const string JUMP_TO = "jump_to";
            /// <summary>
            /// Constant REPROMPT for reprompt
            /// </summary>
            public const string REPROMPT = "reprompt";
            /// <summary>
            /// Constant SKIP_SLOT for skip_slot
            /// </summary>
            public const string SKIP_SLOT = "skip_slot";
            /// <summary>
            /// Constant SKIP_ALL_SLOTS for skip_all_slots
            /// </summary>
            public const string SKIP_ALL_SLOTS = "skip_all_slots";
            
        }

        /// <summary>
        /// Which part of the dialog node to process next.
        /// </summary>
        public class SelectorValue
        {
            /// <summary>
            /// Constant CONDITION for condition
            /// </summary>
            public const string CONDITION = "condition";
            /// <summary>
            /// Constant CLIENT for client
            /// </summary>
            public const string CLIENT = "client";
            /// <summary>
            /// Constant USER_INPUT for user_input
            /// </summary>
            public const string USER_INPUT = "user_input";
            /// <summary>
            /// Constant BODY for body
            /// </summary>
            public const string BODY = "body";
            
        }

        /// <summary>
        /// What happens after the dialog node completes. The valid values depend on the node type:
        /// - The following values are valid for any node:
        ///   - `get_user_input`
        ///   - `skip_user_input`
        ///   - `jump_to`
        /// - If the node is of type `event_handler` and its parent node is of type `slot` or `frame`, additional values
        /// are also valid:
        ///   - if **event_name**=`filled` and the type of the parent node is `slot`:
        ///     - `reprompt`
        ///     - `skip_all_slots`
        /// - if **event_name**=`nomatch` and the type of the parent node is `slot`:
        ///     - `reprompt`
        ///     - `skip_slot`
        ///     - `skip_all_slots`
        /// - if **event_name**=`generic` and the type of the parent node is `frame`:
        ///     - `reprompt`
        ///     - `skip_slot`
        ///     - `skip_all_slots`
        ///      If you specify `jump_to`, then you must also specify a value for the `dialog_node` property.
        /// Constants for possible values can be found using DialogNodeNextStep.BehaviorValue
        /// </summary>
        [JsonProperty("behavior", NullValueHandling = NullValueHandling.Ignore)]
        public string Behavior { get; set; }
        /// <summary>
        /// Which part of the dialog node to process next.
        /// Constants for possible values can be found using DialogNodeNextStep.SelectorValue
        /// </summary>
        [JsonProperty("selector", NullValueHandling = NullValueHandling.Ignore)]
        public string Selector { get; set; }
        /// <summary>
        /// The ID of the dialog node to process next. This parameter is required if **behavior**=`jump_to`.
        /// </summary>
        [JsonProperty("dialog_node", NullValueHandling = NullValueHandling.Ignore)]
        public string DialogNode { get; set; }
    }
}
