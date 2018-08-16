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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    /// <summary>
    /// A respresentation of a relationship query.
    /// </summary>
    [fsObject]
    public class QueryRelations
    {
        /// <summary>
        /// The sorting method for the relationships, can be `score` or `frequency`. `frequency` is the number of unique
        /// times each entity is identified. The default is `score`.
        /// </summary>
        public enum SortEnum
        {
            /// <summary>
            /// Enum score for score
            /// </summary>
            [EnumMember(Value = "score")]
            score,
            /// <summary>
            /// Enum frequency for frequency
            /// </summary>
            [EnumMember(Value = "frequency")]
            frequency
        }

        /// <summary>
        /// The sorting method for the relationships, can be `score` or `frequency`. `frequency` is the number of unique
        /// times each entity is identified. The default is `score`.
        /// </summary>
        [fsProperty("sort")]
        public SortEnum? Sort { get; set; }
        /// <summary>
        /// An array of entities to find relationships for.
        /// </summary>
        [fsProperty("entities")]
        public List<QueryRelationsEntity> Entities { get; set; }
        /// <summary>
        /// Entity text to provide context for the queried entity and rank based on that association. For example, if
        /// you wanted to query the city of London in England your query would look for `London` with the context of
        /// `England`.
        /// </summary>
        [fsProperty("context")]
        public QueryEntitiesContext Context { get; set; }
        /// <summary>
        /// Filters to apply to the relationship query.
        /// </summary>
        [fsProperty("filter")]
        public QueryRelationsFilter Filter { get; set; }
        /// <summary>
        /// The number of results to return. The default is `10`. The maximum is `1000`.
        /// </summary>
        [fsProperty("count")]
        public long? Count { get; set; }
        /// <summary>
        /// The number of evidence items to return for each result. The default is `0`. The maximum number of evidence
        /// items per query is 10,000.
        /// </summary>
        [fsProperty("evidence_count")]
        public long? EvidenceCount { get; set; }
    }

}
