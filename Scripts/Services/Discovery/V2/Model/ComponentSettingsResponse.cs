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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IBM.Watson.Discovery.V2.Model
{
    /// <summary>
    /// A response containing the default component settings.
    /// </summary>
    public class ComponentSettingsResponse
    {
        /// <summary>
        /// Fields shown in the results section of the UI.
        /// </summary>
        [JsonProperty("fields_shown", NullValueHandling = NullValueHandling.Ignore)]
        public ComponentSettingsFieldsShown FieldsShown { get; set; }
        /// <summary>
        /// Whether or not autocomplete is enabled.
        /// </summary>
        [JsonProperty("autocomplete", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Autocomplete { get; set; }
        /// <summary>
        /// Whether or not structured search is enabled.
        /// </summary>
        [JsonProperty("structured_search", NullValueHandling = NullValueHandling.Ignore)]
        public bool? StructuredSearch { get; set; }
        /// <summary>
        /// Number or results shown per page.
        /// </summary>
        [JsonProperty("results_per_page", NullValueHandling = NullValueHandling.Ignore)]
        public long? ResultsPerPage { get; set; }
        /// <summary>
        /// a list of component setting aggregations.
        /// </summary>
        [JsonProperty("aggregations", NullValueHandling = NullValueHandling.Ignore)]
        public List<ComponentSettingsAggregation> Aggregations { get; set; }
    }
}
