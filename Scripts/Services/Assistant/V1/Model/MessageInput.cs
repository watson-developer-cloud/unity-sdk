/**
* (C) Copyright IBM Corp. 2018, 2020.
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

using IBM.Cloud.SDK.Model;
using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// An input object that includes the input text.
    /// </summary>
    public class MessageInput: DynamicModel<object>
    {
        /// <summary>
        /// The text of the user input. This string cannot contain carriage return, newline, or tab characters.
        /// </summary>
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
        /// <summary>
        /// Whether to use spelling correction when processing the input. This property overrides the value of the
        /// **spelling_suggestions** property in the workspace settings.
        /// </summary>
        [JsonProperty("spelling_suggestions", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SpellingSuggestions { get; set; }
        /// <summary>
        /// Whether to use autocorrection when processing the input. If spelling correction is used and this property is
        /// `false`, any suggested corrections are returned in the **suggested_text** property of the message response.
        /// If this property is `true`, any corrections are automatically applied to the user input, and the original
        /// text is returned in the **original_text** property of the message response. This property overrides the
        /// value of the **spelling_auto_correct** property in the workspace settings.
        /// </summary>
        [JsonProperty("spelling_auto_correct", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SpellingAutoCorrect { get; set; }
        /// <summary>
        /// Any suggested corrections of the input text. This property is returned only if spelling correction is
        /// enabled and autocorrection is disabled.
        /// </summary>
        [JsonProperty("suggested_text", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string SuggestedText { get; private set; }
        /// <summary>
        /// The original user input text. This property is returned only if autocorrection is enabled and the user input
        /// was corrected.
        /// </summary>
        [JsonProperty("original_text", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string OriginalText { get; private set; }
    }
}
