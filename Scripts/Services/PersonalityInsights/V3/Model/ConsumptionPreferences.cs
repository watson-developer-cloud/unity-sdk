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

namespace IBM.Watson.PersonalityInsights.V3.Model
{
    /// <summary>
    /// A consumption preference that the service inferred from the input content.
    /// </summary>
    public class ConsumptionPreferences
    {
        /// <summary>
        /// The unique, non-localized identifier of the consumption preference to which the results pertain. IDs have
        /// the form `consumption_preferences_{preference}`.
        /// </summary>
        [JsonProperty("consumption_preference_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ConsumptionPreferenceId { get; set; }
        /// <summary>
        /// The user-visible, localized name of the consumption preference.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// The score for the consumption preference:
        /// * `0.0`: Unlikely
        /// * `0.5`: Neutral
        /// * `1.0`: Likely
        ///
        /// The scores for some preferences are binary and do not allow a neutral value. The score is an indication of
        /// preference based on the results inferred from the input text, not a normalized percentile.
        /// </summary>
        [JsonProperty("score", NullValueHandling = NullValueHandling.Ignore)]
        public double? Score { get; set; }
    }
}
