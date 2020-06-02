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

using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V2.Model
{
    /// <summary>
    /// Spelling correction options for the message. Any options specified on an individual message override the
    /// settings configured for the skill.
    /// </summary>
    public class MessageInputOptionsSpelling
    {
        /// <summary>
        /// Whether to use spelling correction when processing the input. If spelling correction is used and
        /// **auto_correct** is `true`, any spelling corrections are automatically applied to the user input. If
        /// **auto_correct** is `false`, any suggested corrections are returned in the **output.spelling** property.
        ///
        /// This property overrides the value of the **spelling_suggestions** property in the workspace settings for the
        /// skill.
        /// </summary>
        [JsonProperty("suggestions", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Suggestions { get; set; }
        /// <summary>
        /// Whether to use autocorrection when processing the input. If this property is `true`, any corrections are
        /// automatically applied to the user input, and the original text is returned in the **output.spelling**
        /// property of the message response. This property overrides the value of the **spelling_auto_correct**
        /// property in the workspace settings for the skill.
        /// </summary>
        [JsonProperty("auto_correct", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AutoCorrect { get; set; }
    }
}
