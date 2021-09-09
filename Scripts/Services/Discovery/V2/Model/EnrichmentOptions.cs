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
    /// An object that contains options for the current enrichment. Starting with version `2020-08-30`, the enrichment
    /// options are not included in responses from the List Enrichments method.
    /// </summary>
    public class EnrichmentOptions
    {
        /// <summary>
        /// An array of supported languages for this enrichment. Required when `type` is `dictionary`. Optional when
        /// `type` is `rule_based`. Not valid when creating any other type of enrichment.
        /// </summary>
        [JsonProperty("languages", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Languages { get; set; }
        /// <summary>
        /// The name of the entity type. This value is used as the field name in the index. Required when `type` is
        /// `dictionary` or `regular_expression`. Not valid when creating any other type of enrichment.
        /// </summary>
        [JsonProperty("entity_type", NullValueHandling = NullValueHandling.Ignore)]
        public string EntityType { get; set; }
        /// <summary>
        /// The regular expression to apply for this enrichment. Required when `type` is `regular_expression`. Not valid
        /// when creating any other type of enrichment.
        /// </summary>
        [JsonProperty("regular_expression", NullValueHandling = NullValueHandling.Ignore)]
        public string RegularExpression { get; set; }
        /// <summary>
        /// The name of the result document field that this enrichment creates. Required when `type` is `rule_based`.
        /// Not valid when creating any other type of enrichment.
        /// </summary>
        [JsonProperty("result_field", NullValueHandling = NullValueHandling.Ignore)]
        public string ResultField { get; set; }
    }
}
