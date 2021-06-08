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
    /// Default query parameters for this project.
    /// </summary>
    public class DefaultQueryParams
    {
        /// <summary>
        /// An array of collection identifiers to query. If empty or omitted all collections in the project are queried.
        /// </summary>
        [JsonProperty("collection_ids", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> CollectionIds { get; set; }
        /// <summary>
        /// Default settings configuration for passage search options.
        /// </summary>
        [JsonProperty("passages", NullValueHandling = NullValueHandling.Ignore)]
        public DefaultQueryParamsPassages Passages { get; set; }
        /// <summary>
        /// Default project query settings for table results.
        /// </summary>
        [JsonProperty("table_results", NullValueHandling = NullValueHandling.Ignore)]
        public DefaultQueryParamsTableResults TableResults { get; set; }
        /// <summary>
        /// A string representing the default aggregation query for the project.
        /// </summary>
        [JsonProperty("aggregation", NullValueHandling = NullValueHandling.Ignore)]
        public string Aggregation { get; set; }
        /// <summary>
        /// Object that contains suggested refinement settings.
        /// </summary>
        [JsonProperty("suggested_refinements", NullValueHandling = NullValueHandling.Ignore)]
        public DefaultQueryParamsSuggestedRefinements SuggestedRefinements { get; set; }
        /// <summary>
        /// When `true`, a spelling suggestions for the query are returned by default.
        /// </summary>
        [JsonProperty("spelling_suggestions", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SpellingSuggestions { get; set; }
        /// <summary>
        /// When `true`, a highlights for the query are returned by default.
        /// </summary>
        [JsonProperty("highlight", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Highlight { get; set; }
        /// <summary>
        /// The number of document results returned by default.
        /// </summary>
        [JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
        public long? Count { get; set; }
        /// <summary>
        /// A comma separated list of document fields to sort results by default.
        /// </summary>
        [JsonProperty("sort", NullValueHandling = NullValueHandling.Ignore)]
        public string Sort { get; set; }
        /// <summary>
        /// An array of field names to return in document results if present by default.
        /// </summary>
        [JsonProperty("return", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> _Return { get; set; }
    }
}
