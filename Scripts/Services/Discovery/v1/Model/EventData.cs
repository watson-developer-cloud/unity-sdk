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

using FullSerializer;
using System;

namespace IBM.Watson.Discovery.v1.Model
{
    /// <summary>
    /// Query event data object.
    /// </summary>
    public class EventData
    {
        /// <summary>
        /// The **environment_id** associated with the query that the event is associated with.
        /// </summary>
        [fsProperty("environment_id")]
        public string EnvironmentId { get; set; }
        /// <summary>
        /// The session token that was returned as part of the query results that this event is associated with.
        /// </summary>
        [fsProperty("session_token")]
        public string SessionToken { get; set; }
        /// <summary>
        /// The optional timestamp for the event that was created. If not provided, the time that the event was created
        /// in the log was used.
        /// </summary>
        [fsProperty("client_timestamp")]
        public DateTime? ClientTimestamp { get; set; }
        /// <summary>
        /// The rank of the result item which the event is associated with.
        /// </summary>
        [fsProperty("display_rank")]
        public long? DisplayRank { get; set; }
        /// <summary>
        /// The **collection_id** of the document that this event is associated with.
        /// </summary>
        [fsProperty("collection_id")]
        public string CollectionId { get; set; }
        /// <summary>
        /// The **document_id** of the document that this event is associated with.
        /// </summary>
        [fsProperty("document_id")]
        public string DocumentId { get; set; }
        /// <summary>
        /// The query identifier stored in the log. The query and any events associated with that query are stored with
        /// the same **query_id**.
        /// </summary>
        [fsProperty("query_id")]
        public virtual string QueryId { get; private set; }
    }


}
