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

namespace IBM.Watson.Discovery.V2.Model
{
    /// <summary>
    /// Configuration for passage retrieval.
    /// </summary>
    public class QueryLargePassages: QueryAggregation
    {
        /// <summary>
        /// A passages query that returns the most relevant passages from the results.
        /// </summary>
        [JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enabled { get; set; }
        /// <summary>
        /// When `true`, passages will be returned whithin their respective result.
        /// </summary>
        [JsonProperty("per_document", NullValueHandling = NullValueHandling.Ignore)]
        public bool? PerDocument { get; set; }
        /// <summary>
        /// Maximum number of passages to return per result.
        /// </summary>
        [JsonProperty("max_per_document", NullValueHandling = NullValueHandling.Ignore)]
        public long? MaxPerDocument { get; set; }
        /// <summary>
        /// A list of fields that passages are drawn from. If this parameter not specified, then all top-level fields
        /// are included.
        /// </summary>
        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Fields { get; set; }
        /// <summary>
        /// The maximum number of passages to return. The search returns fewer passages if the requested total is not
        /// found. The default is `10`. The maximum is `100`.
        /// </summary>
        [JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
        public long? Count { get; set; }
        /// <summary>
        /// The approximate number of characters that any one passage will have.
        /// </summary>
        [JsonProperty("characters", NullValueHandling = NullValueHandling.Ignore)]
        public long? Characters { get; set; }
    }
}
