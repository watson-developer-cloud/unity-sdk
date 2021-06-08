/**
* (C) Copyright IBM Corp. 2020, 2021.
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
    /// A object that contains options for the current enrichment.
    /// </summary>
    public class EnrichmentOptions
    {
        /// <summary>
        /// An array of supported languages for this enrichment.
        /// </summary>
        [JsonProperty("languages", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Languages { get; set; }
        /// <summary>
        /// The type of entity. Required when creating `dictionary` and `regular_expression` **type** enrichment. Not
        /// valid when creating any other type of enrichment.
        /// </summary>
        [JsonProperty("entity_type", NullValueHandling = NullValueHandling.Ignore)]
        public string EntityType { get; set; }
        /// <summary>
        /// The regular expression to apply for this enrichment. Required only when the **type** of enrichment being
        /// created is a `regular_expression`. Not valid when creating any other type of enrichment.
        /// </summary>
        [JsonProperty("regular_expression", NullValueHandling = NullValueHandling.Ignore)]
        public string RegularExpression { get; set; }
        /// <summary>
        /// The name of the result document field that this enrichment creates. Required only when the enrichment
        /// **type** is `rule_based`. Not valid when creating any other type of enrichment.
        /// </summary>
        [JsonProperty("result_field", NullValueHandling = NullValueHandling.Ignore)]
        public string ResultField { get; set; }
    }
}
