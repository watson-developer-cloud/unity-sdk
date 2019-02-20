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
using System;

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// Query event data object.
    /// </summary>
    public class EventData
    {
        /// <summary>
        /// The **environment_id** associated with the query that the event is associated with.
        /// </summary>
        [JsonProperty("environment_id", NullValueHandling = NullValueHandling.Ignore)]
        public string EnvironmentId { get; set; }
        /// <summary>
        /// The session token that was returned as part of the query results that this event is associated with.
        /// </summary>
        [JsonProperty("session_token", NullValueHandling = NullValueHandling.Ignore)]
        public string SessionToken { get; set; }
        /// <summary>
        /// The optional timestamp for the event that was created. If not provided, the time that the event was created
        /// in the log was used.
        /// </summary>
        [JsonProperty("client_timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ClientTimestamp { get; set; }
        /// <summary>
        /// The rank of the result item which the event is associated with.
        /// </summary>
        [JsonProperty("display_rank", NullValueHandling = NullValueHandling.Ignore)]
        public long? DisplayRank { get; set; }
        /// <summary>
        /// The **collection_id** of the document that this event is associated with.
        /// </summary>
        [JsonProperty("collection_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CollectionId { get; set; }
        /// <summary>
        /// The **document_id** of the document that this event is associated with.
        /// </summary>
        [JsonProperty("document_id", NullValueHandling = NullValueHandling.Ignore)]
        public string DocumentId { get; set; }
        /// <summary>
        /// The query identifier stored in the log. The query and any events associated with that query are stored with
        /// the same **query_id**.
        /// </summary>
        [JsonProperty("query_id", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string QueryId { get; private set; }
    }
}
