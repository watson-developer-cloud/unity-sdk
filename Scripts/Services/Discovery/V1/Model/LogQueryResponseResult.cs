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
    /// Individual result object for a **logs** query. Each object represents either a query to a Discovery collection
    /// or an event that is associated with a query.
    /// </summary>
    public class LogQueryResponseResult
    {
        /// <summary>
        /// The type of log entry returned.
        ///
        ///  **query** indicates that the log represents the results of a call to the single collection **query**
        /// method.
        ///
        ///  **event** indicates that the log represents  a call to the **events** API.
        /// </summary>
        public class DocumentTypeValue
        {
            /// <summary>
            /// Constant QUERY for query
            /// </summary>
            public const string QUERY = "query";
            /// <summary>
            /// Constant EVENT for event
            /// </summary>
            public const string EVENT = "event";
            
        }

        /// <summary>
        /// The type of event that this object respresents. Possible values are
        ///
        ///  -  `query` the log of a query to a collection
        ///
        ///  -  `click` the result of a call to the **events** endpoint.
        /// </summary>
        public class EventTypeValue
        {
            /// <summary>
            /// Constant CLICK for click
            /// </summary>
            public const string CLICK = "click";
            /// <summary>
            /// Constant QUERY for query
            /// </summary>
            public const string QUERY = "query";
            
        }

        /// <summary>
        /// The type of result that this **event** is associated with. Only returned with logs of type `event`.
        /// </summary>
        public class ResultTypeValue
        {
            /// <summary>
            /// Constant DOCUMENT for document
            /// </summary>
            public const string DOCUMENT = "document";
            
        }

        /// <summary>
        /// The type of log entry returned.
        ///
        ///  **query** indicates that the log represents the results of a call to the single collection **query**
        /// method.
        ///
        ///  **event** indicates that the log represents  a call to the **events** API.
        /// Constants for possible values can be found using LogQueryResponseResult.DocumentTypeValue
        /// </summary>
        [JsonProperty("document_type", NullValueHandling = NullValueHandling.Ignore)]
        public string DocumentType { get; set; }
        /// <summary>
        /// The type of event that this object respresents. Possible values are
        ///
        ///  -  `query` the log of a query to a collection
        ///
        ///  -  `click` the result of a call to the **events** endpoint.
        /// Constants for possible values can be found using LogQueryResponseResult.EventTypeValue
        /// </summary>
        [JsonProperty("event_type", NullValueHandling = NullValueHandling.Ignore)]
        public string EventType { get; set; }
        /// <summary>
        /// The type of result that this **event** is associated with. Only returned with logs of type `event`.
        /// Constants for possible values can be found using LogQueryResponseResult.ResultTypeValue
        /// </summary>
        [JsonProperty("result_type", NullValueHandling = NullValueHandling.Ignore)]
        public string ResultType { get; set; }
        /// <summary>
        /// The environment ID that is associated with this log entry.
        /// </summary>
        [JsonProperty("environment_id", NullValueHandling = NullValueHandling.Ignore)]
        public string EnvironmentId { get; set; }
        /// <summary>
        /// The **customer_id** label that was specified in the header of the query or event API call that corresponds
        /// to this log entry.
        /// </summary>
        [JsonProperty("customer_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CustomerId { get; set; }
        /// <summary>
        /// The value of the **natural_language_query** query parameter that was used to create these results. Only
        /// returned with logs of type **query**.
        ///
        /// **Note:** Other query parameters (such as **filter** or **deduplicate**) might  have been used with this
        /// query, but are not recorded.
        /// </summary>
        [JsonProperty("natural_language_query", NullValueHandling = NullValueHandling.Ignore)]
        public string NaturalLanguageQuery { get; set; }
        /// <summary>
        /// Object containing result information that was returned by the query used to create this log entry. Only
        /// returned with logs of type `query`.
        /// </summary>
        [JsonProperty("document_results", NullValueHandling = NullValueHandling.Ignore)]
        public LogQueryResponseResultDocuments DocumentResults { get; set; }
        /// <summary>
        /// Date that the log result was created. Returned in `YYYY-MM-DDThh:mm:ssZ` format.
        /// </summary>
        [JsonProperty("created_timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CreatedTimestamp { get; set; }
        /// <summary>
        /// Date specified by the user when recording an event. Returned in `YYYY-MM-DDThh:mm:ssZ` format. Only returned
        /// with logs of type **event**.
        /// </summary>
        [JsonProperty("client_timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ClientTimestamp { get; set; }
        /// <summary>
        /// Identifier that corresponds to the **natural_language_query** string used in the original or associated
        /// query. All **event** and **query** log entries that have the same original **natural_language_query** string
        /// also have them same **query_id**. This field can be used to recall all **event** and **query** log results
        /// that have the same original query (**event** logs do not contain the original **natural_language_query**
        /// field).
        /// </summary>
        [JsonProperty("query_id", NullValueHandling = NullValueHandling.Ignore)]
        public string QueryId { get; set; }
        /// <summary>
        /// Unique identifier (within a 24-hour period) that identifies a single `query` log and any `event` logs that
        /// were created for it.
        ///
        /// **Note:** If the exact same query is run at the exact same time on different days, the **session_token** for
        /// those queries might be identical. However, the **created_timestamp** differs.
        ///
        /// **Note:** Session tokens are case sensitive. To avoid matching on session tokens that are identical except
        /// for case, use the exact match operator (`::`) when you query for a specific session token.
        /// </summary>
        [JsonProperty("session_token", NullValueHandling = NullValueHandling.Ignore)]
        public string SessionToken { get; set; }
        /// <summary>
        /// The collection ID of the document associated with this event. Only returned with logs of type `event`.
        /// </summary>
        [JsonProperty("collection_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CollectionId { get; set; }
        /// <summary>
        /// The original display rank of the document associated with this event. Only returned with logs of type
        /// `event`.
        /// </summary>
        [JsonProperty("display_rank", NullValueHandling = NullValueHandling.Ignore)]
        public long? DisplayRank { get; set; }
        /// <summary>
        /// The document ID of the document associated with this event. Only returned with logs of type `event`.
        /// </summary>
        [JsonProperty("document_id", NullValueHandling = NullValueHandling.Ignore)]
        public string DocumentId { get; set; }
    }
}
