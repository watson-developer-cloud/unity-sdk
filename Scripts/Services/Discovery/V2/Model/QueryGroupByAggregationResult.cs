/**
* (C) Copyright IBM Corp. 2020.
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
    /// Top value result for the term aggregation.
    /// </summary>
    public class QueryGroupByAggregationResult
    {
        /// <summary>
        /// Value of the field with a non-zero frequency in the document set.
        /// </summary>
        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }
        /// <summary>
        /// Number of documents containing the 'key'.
        /// </summary>
        [JsonProperty("matching_results", NullValueHandling = NullValueHandling.Ignore)]
        public long? MatchingResults { get; set; }
        /// <summary>
        /// The relevancy for this group.
        /// </summary>
        [JsonProperty("relevancy", NullValueHandling = NullValueHandling.Ignore)]
        public double? Relevancy { get; set; }
        /// <summary>
        /// The number of documents which have the group as the value of specified field in the whole set of documents
        /// in this collection. Returned only when the `relevancy` parameter is set to `true`.
        /// </summary>
        [JsonProperty("total_matching_documents", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalMatchingDocuments { get; set; }
        /// <summary>
        /// The estimated number of documents which would match the query and also meet the condition. Returned only
        /// when the `relevancy` parameter is set to `true`.
        /// </summary>
        [JsonProperty("estimated_matching_documents", NullValueHandling = NullValueHandling.Ignore)]
        public long? EstimatedMatchingDocuments { get; set; }
        /// <summary>
        /// An array of sub aggregations.
        /// </summary>
        [JsonProperty("aggregations", NullValueHandling = NullValueHandling.Ignore)]
        public List<QueryAggregation> Aggregations { get; set; }
    }
}
