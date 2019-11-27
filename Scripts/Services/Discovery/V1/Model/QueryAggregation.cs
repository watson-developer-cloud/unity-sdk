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
using JsonSubTypes;
using Newtonsoft.Json;

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// An aggregation produced by  Discovery to analyze the input provided.
    /// </summary>
    [JsonConverter(typeof(JsonSubtypes), "Type")]
    [JsonSubtypes.KnownSubType(typeof(Histogram), "histogram")]
    [JsonSubtypes.KnownSubType(typeof(Calculation), "max")]
    [JsonSubtypes.KnownSubType(typeof(Calculation), "min")]
    [JsonSubtypes.KnownSubType(typeof(Calculation), "average")]
    [JsonSubtypes.KnownSubType(typeof(Calculation), "sum")]
    [JsonSubtypes.KnownSubType(typeof(Calculation), "unique_count")]
    [JsonSubtypes.KnownSubType(typeof(Term), "term")]
    [JsonSubtypes.KnownSubType(typeof(Filter), "filter")]
    [JsonSubtypes.KnownSubType(typeof(Nested), "nested")]
    [JsonSubtypes.KnownSubType(typeof(Timeslice), "timeslice")]
    [JsonSubtypes.KnownSubType(typeof(TopHits), "top_hits")]
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
        /// Aggregations returned by Discovery.
        /// </summary>
        [JsonProperty("aggregations", NullValueHandling = NullValueHandling.Ignore)]
        public List<QueryAggregation> Aggregations { get; set; }
    }
}
