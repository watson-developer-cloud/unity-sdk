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

namespace IBM.Watson.Discovery.V2.Model
{
    /// <summary>
    /// A response that contains the documents and aggregations for the query.
    /// </summary>
    public class QueryResponse
    {
        /// <summary>
        /// The number of matching results for the query.
        /// </summary>
        [JsonProperty("matching_results", NullValueHandling = NullValueHandling.Ignore)]
        public long? MatchingResults { get; set; }
        /// <summary>
        /// Array of document results for the query.
        /// </summary>
        [JsonProperty("results", NullValueHandling = NullValueHandling.Ignore)]
        public List<QueryResult> Results { get; set; }
        /// <summary>
        /// Array of aggregations for the query.
        /// </summary>
        [JsonProperty("aggregations", NullValueHandling = NullValueHandling.Ignore)]
        public List<QueryAggregation> Aggregations { get; set; }
        /// <summary>
        /// An object contain retrieval type information.
        /// </summary>
        [JsonProperty("retrieval_details", NullValueHandling = NullValueHandling.Ignore)]
        public RetrievalDetails RetrievalDetails { get; set; }
        /// <summary>
        /// Suggested correction to the submitted **natural_language_query** value.
        /// </summary>
        [JsonProperty("suggested_query", NullValueHandling = NullValueHandling.Ignore)]
        public string SuggestedQuery { get; set; }
        /// <summary>
        /// Array of suggested refinements.
        /// </summary>
        [JsonProperty("suggested_refinements", NullValueHandling = NullValueHandling.Ignore)]
        public List<QuerySuggestedRefinement> SuggestedRefinements { get; set; }
        /// <summary>
        /// Array of table results.
        /// </summary>
        [JsonProperty("table_results", NullValueHandling = NullValueHandling.Ignore)]
        public List<QueryTableResult> TableResults { get; set; }
        /// <summary>
        /// Passages returned by Discovery.
        /// </summary>
        [JsonProperty("passages", NullValueHandling = NullValueHandling.Ignore)]
        public List<QueryResponsePassage> Passages { get; set; }
    }
}
