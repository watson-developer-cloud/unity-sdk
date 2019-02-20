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
    /// A respresentation of a relationship query.
    /// </summary>
    public class QueryRelations
    {
        /// <summary>
        /// The sorting method for the relationships, can be `score` or `frequency`. `frequency` is the number of unique
        /// times each entity is identified. The default is `score`. This parameter cannot be used in the same query as
        /// the **bias** parameter.
        /// </summary>
        public class SortValue
        {
            /// <summary>
            /// Constant SCORE for score
            /// </summary>
            public const string SCORE = "score";
            /// <summary>
            /// Constant FREQUENCY for frequency
            /// </summary>
            public const string FREQUENCY = "frequency";
            
        }

        /// <summary>
        /// The sorting method for the relationships, can be `score` or `frequency`. `frequency` is the number of unique
        /// times each entity is identified. The default is `score`. This parameter cannot be used in the same query as
        /// the **bias** parameter.
        /// Constants for possible values can be found using QueryRelations.SortValue
        /// </summary>
        [JsonProperty("sort", NullValueHandling = NullValueHandling.Ignore)]
        public string Sort { get; set; }
        /// <summary>
        /// An array of entities to find relationships for.
        /// </summary>
        [JsonProperty("entities", NullValueHandling = NullValueHandling.Ignore)]
        public List<QueryRelationsEntity> Entities { get; set; }
        /// <summary>
        /// Entity text to provide context for the queried entity and rank based on that association. For example, if
        /// you wanted to query the city of London in England your query would look for `London` with the context of
        /// `England`.
        /// </summary>
        [JsonProperty("context", NullValueHandling = NullValueHandling.Ignore)]
        public QueryEntitiesContext Context { get; set; }
        /// <summary>
        /// Filters to apply to the relationship query.
        /// </summary>
        [JsonProperty("filter", NullValueHandling = NullValueHandling.Ignore)]
        public QueryRelationsFilter Filter { get; set; }
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
