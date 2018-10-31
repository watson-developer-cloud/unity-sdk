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
using System;
using System.Runtime.Serialization;

namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    /// <summary>
    /// Individual result object for a **logs** query. Each object represents either a query to a Discovery collection
    /// or an event that is associated with a query.
    /// </summary>
    [fsObject]
    public class LogQueryResponseResult
    {
        /// <summary>
        /// The type of event that this object respresents. Possible values are
        ///
        ///  -  `query` the log of a query to a collection
        ///
        ///  -  `click` the result of a call to the **events** endpoint.
        /// </summary>
        public enum EventTypeEnum
        {
            /// <summary>
            /// Enum click for click
            /// </summary>
            [EnumMember(Value = "click")]
            click,
            /// <summary>
            /// Enum query for query
            /// </summary>
            [EnumMember(Value = "query")]
            query
        }

        /// <summary>
        /// The type of result that this **event** is associated with. Only returned with logs of type `event`.
        /// </summary>
        public enum ResultTypeEnum
        {
            /// <summary>
            /// Enum document for document
            /// </summary>
            [EnumMember(Value = "document")]
            document
        }

        /// <summary>
        /// The type of log entry returned.
        ///
        ///  **query** indicates that the log represents the results of a call to the single collection **query**
        /// method.
        ///
        ///  **event** indicates that the log represents  a call to the **events** API.
        /// </summary>
        [fsProperty("document_type")]
        public string DocumentType { get; set; }
        /// <summary>
        /// The type of event that this object respresents. Possible values are
        ///
        ///  -  `query` the log of a query to a collection
        ///
        ///  -  `click` the result of a call to the **events** endpoint.
        /// </summary>
        [fsProperty("event_type")]
        public EventTypeEnum? EventType { get; set; }
        /// <summary>
        /// The type of result that this **event** is associated with. Only returned with logs of type `event`.
        /// </summary>
        [fsProperty("result_type")]
        public ResultTypeEnum? ResultType { get; set; }
        /// <summary>
        /// The environment ID that is associated with this log entry.
        /// </summary>
        [fsProperty("environment_id")]
        public string EnvironmentId { get; set; }
        /// <summary>
        /// The **customer_id** label that was specified in the header of the query or event API call that corresponds
        /// to this log entry.
        /// </summary>
        [fsProperty("customer_id")]
        public string CustomerId { get; set; }
        /// <summary>
        /// The value of the **natural_language_query** query parameter that was used to create these results. Only
        /// returned with logs of type **query**.
        ///
        /// **Note:** Other query parameters (such as **filter** or **deduplicate**) might  have been used with this
        /// query, but are not recorded.
        /// </summary>
        [fsProperty("natural_language_query")]
        public string NaturalLanguageQuery { get; set; }
        /// <summary>
        /// Object containing result information that was returned by the query used to create this log entry. Only
        /// returned with logs of type `query`.
        /// </summary>
        [fsProperty("document_results")]
        public LogQueryResponseResultDocuments DocumentResults { get; set; }
        /// <summary>
        /// Date that the log result was created. Returned in `YYYY-MM-DDThh:mm:ssZ` format.
        /// </summary>
        [fsProperty("created_timestamp")]
        public DateTime CreatedTimestamp { get; set; }
        /// <summary>
        /// Date specified by the user when recording an event. Returned in `YYYY-MM-DDThh:mm:ssZ` format. Only returned
        /// with logs of type **event**.
        /// </summary>
        [fsProperty("client_timestamp")]
        public DateTime ClientTimestamp { get; set; }
        /// <summary>
        /// Identifier that corresponds to the **natural_language_query** string used in the original or associated
        /// query. All **event** and **query** log entries that have the same original **natural_language_query** string
        /// also have them same **query_id**. This field can be used to recall all **event** and **query** log results
        /// that have the same original query (**event** logs do not contain the original **natural_language_query**
        /// field).
        /// </summary>
        [fsProperty("query_id")]
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
        [fsProperty("session_token")]
        public string SessionToken { get; set; }
        /// <summary>
        /// The collection ID of the document associated with this event. Only returned with logs of type `event`.
        /// </summary>
        [fsProperty("collection_id")]
        public string CollectionId { get; set; }
        /// <summary>
        /// The original display rank of the document associated with this event. Only returned with logs of type
        /// `event`.
        /// </summary>
        [fsProperty("display_rank")]
        public long? DisplayRank { get; set; }
        /// <summary>
        /// The document ID of the document associated with this event. Only returned with logs of type `event`.
        /// </summary>
        [fsProperty("document_id")]
        public string DocumentId { get; set; }
    }

}
