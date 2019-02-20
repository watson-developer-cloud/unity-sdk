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
    /// A response containing the documents and aggregations for the query.
    /// </summary>
    public class QueryResponse
    {
        /// <summary>
        /// Gets or Sets MatchingResults
        /// </summary>
        [JsonProperty("matching_results", NullValueHandling = NullValueHandling.Ignore)]
        public long? MatchingResults { get; set; }
        /// <summary>
        /// Gets or Sets Results
        /// </summary>
        [JsonProperty("results", NullValueHandling = NullValueHandling.Ignore)]
        public List<QueryResult> Results { get; set; }
        /// <summary>
        /// Gets or Sets Aggregations
        /// </summary>
        [JsonProperty("aggregations", NullValueHandling = NullValueHandling.Ignore)]
        public List<QueryAggregation> Aggregations { get; set; }
        /// <summary>
        /// Gets or Sets Passages
        /// </summary>
        [JsonProperty("passages", NullValueHandling = NullValueHandling.Ignore)]
        public List<QueryPassages> Passages { get; set; }
        /// <summary>
        /// Gets or Sets DuplicatesRemoved
        /// </summary>
        [JsonProperty("duplicates_removed", NullValueHandling = NullValueHandling.Ignore)]
        public long? DuplicatesRemoved { get; set; }
        /// <summary>
        /// The session token for this query. The session token can be used to add events associated with this query to
        /// the query and event log.
        ///
        /// **Important:** Session tokens are case sensitive.
        /// </summary>
        [JsonProperty("session_token", NullValueHandling = NullValueHandling.Ignore)]
        public string SessionToken { get; set; }
        /// <summary>
        /// An object contain retrieval type information.
        /// </summary>
        [JsonProperty("retrieval_details", NullValueHandling = NullValueHandling.Ignore)]
        public RetrievalDetails RetrievalDetails { get; set; }
    }
}
