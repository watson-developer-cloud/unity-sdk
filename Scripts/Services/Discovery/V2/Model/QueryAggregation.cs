/**
* (C) Copyright IBM Corp. 2019, 2020.
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

using JsonSubTypes;
using Newtonsoft.Json;

namespace IBM.Watson.Discovery.V2.Model
{
    /// <summary>
    /// An abstract aggregation type produced by Discovery to analyze the input provided.
    /// </summary>
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(QueryTermAggregation), "term")]
    [JsonSubtypes.KnownSubType(typeof(QueryHistogramAggregation), "histogram")]
    [JsonSubtypes.KnownSubType(typeof(QueryTimesliceAggregation), "timeslice")]
    [JsonSubtypes.KnownSubType(typeof(QueryNestedAggregation), "nested")]
    [JsonSubtypes.KnownSubType(typeof(QueryFilterAggregation), "filter")]
    [JsonSubtypes.KnownSubType(typeof(QueryCalculationAggregation), "min")]
    [JsonSubtypes.KnownSubType(typeof(QueryCalculationAggregation), "max")]
    [JsonSubtypes.KnownSubType(typeof(QueryCalculationAggregation), "sum")]
    [JsonSubtypes.KnownSubType(typeof(QueryCalculationAggregation), "average")]
    [JsonSubtypes.KnownSubType(typeof(QueryCalculationAggregation), "unique_count")]
    [JsonSubtypes.KnownSubType(typeof(QueryTopHitsAggregation), "top_hits")]
    [JsonSubtypes.KnownSubType(typeof(QueryGroupByAggregation), "group_by")]
    public class QueryAggregation
    {
        /// <summary>
        /// The type of aggregation command used. Options include: term, histogram, timeslice, nested, filter, min, max,
        /// sum, average, unique_count, and top_hits.
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
    }
}
