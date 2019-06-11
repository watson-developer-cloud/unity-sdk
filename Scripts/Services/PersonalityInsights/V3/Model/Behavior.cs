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
    /// The temporal behavior for the input content.
    /// </summary>
    public class Behavior
    {
        /// <summary>
        /// The unique, non-localized identifier of the characteristic to which the results pertain. IDs have the form
        /// `behavior_{value}`.
        /// </summary>
        [JsonProperty("trait_id", NullValueHandling = NullValueHandling.Ignore)]
        public string TraitId { get; set; }
        /// <summary>
        /// The user-visible, localized name of the characteristic.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// The category of the characteristic: `behavior` for temporal data.
        /// </summary>
        [JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; }
        /// <summary>
        /// For JSON content that is timestamped, the percentage of timestamped input data that occurred during that day
        /// of the week or hour of the day. The range is 0 to 1.
        /// </summary>
        [JsonProperty("percentage", NullValueHandling = NullValueHandling.Ignore)]
        public double? Percentage { get; set; }
    }
}
