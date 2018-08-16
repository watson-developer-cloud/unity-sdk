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
    /// QueryEntities
    /// </summary>
    [fsObject]
    public class QueryEntities
    {
        /// <summary>
        /// The entity query feature to perform. Supported features are `disambiguate` and `similar_entities`.
        /// </summary>
        [fsProperty("feature")]
        public string Feature { get; set; }
        /// <summary>
        /// A text string that appears within the entity text field.
        /// </summary>
        [fsProperty("entity")]
        public QueryEntitiesEntity Entity { get; set; }
        /// <summary>
        /// Entity text to provide context for the queried entity and rank based on that association. For example, if
        /// you wanted to query the city of London in England your query would look for `London` with the context of
        /// `England`.
        /// </summary>
        [fsProperty("context")]
        public QueryEntitiesContext Context { get; set; }
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
