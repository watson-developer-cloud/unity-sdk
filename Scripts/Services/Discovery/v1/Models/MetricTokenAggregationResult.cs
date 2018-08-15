/**
* Copyright 2018 IBM Corp. All Rights Reserved.
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
using FullSerializer;

namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    /// <summary>
    /// Aggregation result data for the requested metric.
    /// </summary>
    [fsObject]
    public class MetricTokenAggregationResult
    {
        /// <summary>
        /// The content of the **natural_language_query** parameter used in the query that this result represents.
        /// </summary>
        [fsProperty("key")]
        public string Key { get; set; }
        /// <summary>
        /// Number of matching results.
        /// </summary>
        [fsProperty("matching_results")]
        public long? MatchingResults { get; set; }
        /// <summary>
        /// The number of queries with associated events divided by the total number of queries currently stored
        /// (queries and events are stored in the log for 30 days).
        /// </summary>
        [fsProperty("event_rate")]
        public double? EventRate { get; set; }
    }

}
