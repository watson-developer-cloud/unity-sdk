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

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// QueryEntities.
    /// </summary>
    public class QueryEntities
    {
        /// <summary>
        /// The entity query feature to perform. Supported features are `disambiguate` and `similar_entities`.
        /// </summary>
        [JsonProperty("feature", NullValueHandling = NullValueHandling.Ignore)]
        public string Feature { get; set; }
        /// <summary>
        /// A text string that appears within the entity text field.
        /// </summary>
        [JsonProperty("entity", NullValueHandling = NullValueHandling.Ignore)]
        public QueryEntitiesEntity Entity { get; set; }
        /// <summary>
        /// Entity text to provide context for the queried entity and rank based on that association. For example, if
        /// you wanted to query the city of London in England your query would look for `London` with the context of
        /// `England`.
        /// </summary>
        [JsonProperty("context", NullValueHandling = NullValueHandling.Ignore)]
        public QueryEntitiesContext Context { get; set; }
        /// <summary>
        /// The number of results to return. The default is `10`. The maximum is `1000`.
        /// </summary>
        [JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
        public long? Count { get; set; }
        /// <summary>
        /// The number of evidence items to return for each result. The default is `0`. The maximum number of evidence
        /// items per query is 10,000.
        /// </summary>
        [JsonProperty("evidence_count", NullValueHandling = NullValueHandling.Ignore)]
        public long? EvidenceCount { get; set; }
    }
}
