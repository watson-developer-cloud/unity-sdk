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
using System;

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// Aggregation result data for the requested metric.
    /// </summary>
    public class MetricAggregationResult
    {
        /// <summary>
        /// Date in string form representing the start of this interval.
        /// </summary>
        [JsonProperty("key_as_string", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? KeyAsString { get; set; }
        /// <summary>
        /// Unix epoch time equivalent of the **key_as_string**, that represents the start of this interval.
        /// </summary>
        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public long? Key { get; set; }
        /// <summary>
        /// Number of matching results.
        /// </summary>
        [JsonProperty("matching_results", NullValueHandling = NullValueHandling.Ignore)]
        public long? MatchingResults { get; set; }
        /// <summary>
        /// The number of queries with associated events divided by the total number of queries for the interval. Only
        /// returned with **event_rate** metrics.
        /// </summary>
        [JsonProperty("event_rate", NullValueHandling = NullValueHandling.Ignore)]
        public double? EventRate { get; set; }
    }
}
