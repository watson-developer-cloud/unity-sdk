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

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// Object containing notice query results.
    /// </summary>
    public class QueryNoticesResponse
    {
        /// <summary>
        /// The number of matching results.
        /// </summary>
        [JsonProperty("matching_results", NullValueHandling = NullValueHandling.Ignore)]
        public long? MatchingResults { get; set; }
        /// <summary>
        /// Array of document results that match the query.
        /// </summary>
        [JsonProperty("results", NullValueHandling = NullValueHandling.Ignore)]
        public List<QueryNoticesResult> Results { get; set; }
        /// <summary>
        /// Array of aggregation results that match the query.
        /// </summary>
        [JsonProperty("aggregations", NullValueHandling = NullValueHandling.Ignore)]
        public List<QueryAggregation> Aggregations { get; set; }
        /// <summary>
        /// Array of passage results that match the query.
        /// </summary>
        [JsonProperty("passages", NullValueHandling = NullValueHandling.Ignore)]
        public List<QueryPassages> Passages { get; set; }
        /// <summary>
        /// The number of duplicates removed from this notices query.
        /// </summary>
        [JsonProperty("duplicates_removed", NullValueHandling = NullValueHandling.Ignore)]
        public long? DuplicatesRemoved { get; set; }
    }
}
