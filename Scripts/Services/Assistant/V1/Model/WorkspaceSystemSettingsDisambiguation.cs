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

using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// Workspace settings related to the disambiguation feature.
    /// </summary>
    public class WorkspaceSystemSettingsDisambiguation
    {
        /// <summary>
        /// The sensitivity of the disambiguation feature to intent detection uncertainty. Higher sensitivity means that
        /// the disambiguation feature is triggered more often and includes more choices.
        /// </summary>
        public class SensitivityValue
        {
            /// <summary>
            /// Constant AUTO for auto
            /// </summary>
            public const string AUTO = "auto";
            /// <summary>
            /// Constant HIGH for high
            /// </summary>
            public const string HIGH = "high";
            /// <summary>
            /// Constant MEDIUM_HIGH for medium_high
            /// </summary>
            public const string MEDIUM_HIGH = "medium_high";
            /// <summary>
            /// Constant MEDIUM for medium
            /// </summary>
            public const string MEDIUM = "medium";
            /// <summary>
            /// Constant MEDIUM_LOW for medium_low
            /// </summary>
            public const string MEDIUM_LOW = "medium_low";
            /// <summary>
            /// Constant LOW for low
            /// </summary>
            public const string LOW = "low";
            
        }

        /// <summary>
        /// The sensitivity of the disambiguation feature to intent detection uncertainty. Higher sensitivity means that
        /// the disambiguation feature is triggered more often and includes more choices.
        /// Constants for possible values can be found using WorkspaceSystemSettingsDisambiguation.SensitivityValue
        /// </summary>
        [JsonProperty("sensitivity", NullValueHandling = NullValueHandling.Ignore)]
        public string Sensitivity { get; set; }
        /// <summary>
        /// The text of the introductory prompt that accompanies disambiguation options presented to the user.
        /// </summary>
        [JsonProperty("prompt", NullValueHandling = NullValueHandling.Ignore)]
        public string Prompt { get; set; }
        /// <summary>
        /// The user-facing label for the option users can select if none of the suggested options is correct. If no
        /// value is specified for this property, this option does not appear.
        /// </summary>
        [JsonProperty("none_of_the_above_prompt", NullValueHandling = NullValueHandling.Ignore)]
        public string NoneOfTheAbovePrompt { get; set; }
        /// <summary>
        /// Whether the disambiguation feature is enabled for the workspace.
        /// </summary>
        [JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enabled { get; set; }
        /// <summary>
        /// Whether the order in which disambiguation suggestions are presented should be randomized (but still
        /// influenced by relative confidence).
        /// </summary>
        [JsonProperty("randomize", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Randomize { get; set; }
        /// <summary>
        /// The maximum number of disambigation suggestions that can be included in a `suggestion` response.
        /// </summary>
        [JsonProperty("max_suggestions", NullValueHandling = NullValueHandling.Ignore)]
        public long? MaxSuggestions { get; set; }
        /// <summary>
        /// For internal use only.
        /// </summary>
        [JsonProperty("suggestion_text_policy", NullValueHandling = NullValueHandling.Ignore)]
        public string SuggestionTextPolicy { get; set; }
    }
}
