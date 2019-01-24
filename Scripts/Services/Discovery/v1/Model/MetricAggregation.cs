/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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
using FullSerializer;

namespace IBM.Watson.Discovery.v1.Model
{
    /// <summary>
    /// An aggregation analyzing log information for queries and events.
    /// </summary>
    public class MetricAggregation
    {
        /// <summary>
        /// The measurement interval for this metric. Metric intervals are always 1 day (`1d`).
        /// </summary>
        [fsProperty("interval")]
        public string Interval { get; set; }
        /// <summary>
        /// The event type associated with this metric result. This field, when present, will always be `click`.
        /// </summary>
        [fsProperty("event_type")]
        public string EventType { get; set; }
        /// <summary>
        /// Gets or Sets Results
        /// </summary>
        [fsProperty("results")]
        public List<MetricAggregationResult> Results { get; set; }
    }


}
