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
using System.Runtime.Serialization;

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// The next step to execute following this dialog node.
    /// </summary>
    [fsObject]
    public class DialogNodeNextStep
    {
        /// <summary>
        /// How the `next_step` reference is processed. If you specify `jump_to`, then you must also specify a value for the `dialog_node` property.
        /// </summary>
        /// <value>How the `next_step` reference is processed. If you specify `jump_to`, then you must also specify a value for the `dialog_node` property.</value>
        public enum BehaviorEnum
        {
            
            /// <summary>
            /// Enum GET_USER_INPUT for get_user_input
            /// </summary>
            [EnumMember(Value = "get_user_input")]
            GET_USER_INPUT,
            
            /// <summary>
            /// Enum SKIP_USER_INPUT for skip_user_input
            /// </summary>
            [EnumMember(Value = "skip_user_input")]
            SKIP_USER_INPUT,
            
            /// <summary>
            /// Enum JUMP_TO for jump_to
            /// </summary>
            [EnumMember(Value = "jump_to")]
            JUMP_TO,
            
            /// <summary>
            /// Enum REPROMPT for reprompt
            /// </summary>
            [EnumMember(Value = "reprompt")]
            REPROMPT,
            
            /// <summary>
            /// Enum SKIP_SLOT for skip_slot
            /// </summary>
            [EnumMember(Value = "skip_slot")]
            SKIP_SLOT,
            
            /// <summary>
            /// Enum SKIP_ALL_SLOTS for skip_all_slots
            /// </summary>
            [EnumMember(Value = "skip_all_slots")]
            SKIP_ALL_SLOTS
        }

        /// <summary>
        /// Which part of the dialog node to process next.
        /// </summary>
        /// <value>Which part of the dialog node to process next.</value>
        public enum SelectorEnum
        {
            
            /// <summary>
            /// Enum CONDITION for condition
            /// </summary>
            [EnumMember(Value = "condition")]
            CONDITION,
            
            /// <summary>
            /// Enum CLIENT for client
            /// </summary>
            [EnumMember(Value = "client")]
            CLIENT,
            
            /// <summary>
            /// Enum USER_INPUT for user_input
            /// </summary>
            [EnumMember(Value = "user_input")]
            USER_INPUT,
            
            /// <summary>
            /// Enum BODY for body
            /// </summary>
            [EnumMember(Value = "body")]
            BODY
        }

        /// <summary>
        /// How the `next_step` reference is processed. If you specify `jump_to`, then you must also specify a value for the `dialog_node` property.
        /// </summary>
        /// <value>How the `next_step` reference is processed. If you specify `jump_to`, then you must also specify a value for the `dialog_node` property.</value>
        public BehaviorEnum? Behavior { get; set; }
        /// <summary>
        /// Which part of the dialog node to process next.
        /// </summary>
        /// <value>Which part of the dialog node to process next.</value>
        public SelectorEnum? Selector { get; set; }
        /// <summary>
        /// The ID of the dialog node to process next. This parameter is required if `behavior`=`jump_to`.
        /// </summary>
        /// <value>The ID of the dialog node to process next. This parameter is required if `behavior`=`jump_to`.</value>
        public string DialogNode { get; set; }
    }

}
