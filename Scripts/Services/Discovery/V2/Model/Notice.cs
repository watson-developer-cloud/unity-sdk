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

namespace IBM.Watson.Discovery.V2.Model
{
    /// <summary>
    /// A notice produced for the collection.
    /// </summary>
    public class Notice
    {
        /// <summary>
        /// Severity level of the notice.
        /// </summary>
        public class SeverityValue
        {
            /// <summary>
            /// Constant WARNING for warning
            /// </summary>
            public const string WARNING = "warning";
            /// <summary>
            /// Constant ERROR for error
            /// </summary>
            public const string ERROR = "error";
            
        }

        /// <summary>
        /// Severity level of the notice.
        /// Constants for possible values can be found using Notice.SeverityValue
        /// </summary>
        [JsonProperty("severity", NullValueHandling = NullValueHandling.Ignore)]
        public string Severity { get; set; }
        /// <summary>
        /// Identifies the notice. Many notices might have the same ID. This field exists so that user applications can
        /// programmatically identify a notice and take automatic corrective action. Typical notice IDs include:
        /// `index_failed`, `index_failed_too_many_requests`, `index_failed_incompatible_field`,
        /// `index_failed_cluster_unavailable`, `ingestion_timeout`, `ingestion_error`, `bad_request`, `internal_error`,
        /// `missing_model`, `unsupported_model`, `smart_document_understanding_failed_incompatible_field`,
        /// `smart_document_understanding_failed_internal_error`, `smart_document_understanding_failed_internal_error`,
        /// `smart_document_understanding_failed_warning`, `smart_document_understanding_page_error`,
        /// `smart_document_understanding_page_warning`. **Note:** This is not a complete list, other values might be
        /// returned.
        /// </summary>
        [JsonProperty("notice_id", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string NoticeId { get; private set; }
        /// <summary>
        /// The creation date of the collection in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public virtual DateTime? Created { get; private set; }
        /// <summary>
        /// Unique identifier of the document.
        /// </summary>
        [JsonProperty("document_id", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string DocumentId { get; private set; }
        /// <summary>
        /// Unique identifier of the collection.
        /// </summary>
        [JsonProperty("collection_id", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string CollectionId { get; private set; }
        /// <summary>
        /// Unique identifier of the query used for relevance training.
        /// </summary>
        [JsonProperty("query_id", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string QueryId { get; private set; }
        /// <summary>
        /// Ingestion or training step in which the notice occurred.
        /// </summary>
        [JsonProperty("step", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string Step { get; private set; }
        /// <summary>
        /// The description of the notice.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string Description { get; private set; }
    }
}
