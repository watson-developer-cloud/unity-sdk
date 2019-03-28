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
    /// An aggregation produced by the Discovery service to analyze the input provided.
    /// </summary>
    public class QueryAggregation
    {
        /// <summary>
        /// The type of aggregation command used. For example: term, filter, max, min, etc.
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
        /// <summary>
        /// Array of aggregation results.
        /// </summary>
        [JsonProperty("results", NullValueHandling = NullValueHandling.Ignore)]
        public List<AggregationResult> Results { get; set; }
        /// <summary>
        /// Number of matching results.
        /// </summary>
        [JsonProperty("matching_results", NullValueHandling = NullValueHandling.Ignore)]
        public long? MatchingResults { get; set; }
        /// <summary>
        /// Aggregations returned by the Discovery service.
        /// </summary>
        [JsonProperty("aggregations", NullValueHandling = NullValueHandling.Ignore)]
        public List<QueryAggregation> Aggregations { get; set; }
    }
}
