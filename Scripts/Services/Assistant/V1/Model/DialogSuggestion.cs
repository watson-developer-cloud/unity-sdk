/**
* (C) Copyright IBM Corp. 2019, 2021.
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

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// DialogSuggestion.
    /// </summary>
    public class DialogSuggestion
    {
        /// <summary>
        /// The user-facing label for the disambiguation option. This label is taken from the **title** or
        /// **user_label** property of the corresponding dialog node.
        /// </summary>
        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }
        /// <summary>
        /// An object defining the message input, intents, and entities to be sent to the Watson Assistant service if
        /// the user selects the corresponding disambiguation option.
        /// </summary>
        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public DialogSuggestionValue Value { get; set; }
        /// <summary>
        /// The dialog output that will be returned from the Watson Assistant service if the user selects the
        /// corresponding option.
        /// </summary>
        [JsonProperty("output", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Output { get; set; }
        /// <summary>
        /// The unique ID of the dialog node that the **label** property is taken from. The **label** property is
        /// populated using the value of the dialog node's **title** or **user_label** property.
        /// </summary>
        [JsonProperty("dialog_node", NullValueHandling = NullValueHandling.Ignore)]
        public string DialogNode { get; set; }
    }
}
