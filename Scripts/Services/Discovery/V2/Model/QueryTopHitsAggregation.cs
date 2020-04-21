/**
* (C) Copyright IBM Corp. 2018, 2020.
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

namespace IBM.Watson.Discovery.V2.Model
{
    /// <summary>
    /// Returns the top documents ranked by the score of the query.
    /// </summary>
    public class QueryTopHitsAggregation
    {
        /// <summary>
        /// The number of documents to return.
        /// </summary>
        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public long? Size { get; set; }
        /// <summary>
        /// Identifier specified in the query request of this aggregation.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// Gets or Sets Hits
        /// </summary>
        [JsonProperty("hits", NullValueHandling = NullValueHandling.Ignore)]
        public QueryTopHitsAggregationResult Hits { get; set; }
    }
}
