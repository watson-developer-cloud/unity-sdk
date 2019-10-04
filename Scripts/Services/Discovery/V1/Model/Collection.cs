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
    /// A collection for storing documents.
    /// </summary>
    public class Collection
    {
        /// <summary>
        /// The status of the collection.
        /// </summary>
        public class StatusValue
        {
            /// <summary>
            /// Constant ACTIVE for active
            /// </summary>
            public const string ACTIVE = "active";
            /// <summary>
            /// Constant PENDING for pending
            /// </summary>
            public const string PENDING = "pending";
            /// <summary>
            /// Constant MAINTENANCE for maintenance
            /// </summary>
            public const string MAINTENANCE = "maintenance";
            
        }

        /// <summary>
        /// The status of the collection.
        /// Constants for possible values can be found using Collection.StatusValue
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        /// <summary>
        /// The unique identifier of the collection.
        /// </summary>
        [JsonProperty("collection_id", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string CollectionId { get; private set; }
        /// <summary>
        /// The name of the collection.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// The description of the collection.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        /// <summary>
        /// The creation date of the collection in the format yyyy-MM-dd'T'HH:mmcon:ss.SSS'Z'.
        /// </summary>
        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public virtual DateTime? Created { get; private set; }
        /// <summary>
        /// The timestamp of when the collection was last updated in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        [JsonProperty("updated", NullValueHandling = NullValueHandling.Ignore)]
        public virtual DateTime? Updated { get; private set; }
        /// <summary>
        /// The unique identifier of the collection's configuration.
        /// </summary>
        [JsonProperty("configuration_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ConfigurationId { get; set; }
        /// <summary>
        /// The language of the documents stored in the collection. Permitted values include `en` (English), `de`
        /// (German), and `es` (Spanish).
        /// </summary>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }
        /// <summary>
        /// Object containing collection document count information.
        /// </summary>
        [JsonProperty("document_counts", NullValueHandling = NullValueHandling.Ignore)]
        public DocumentCounts DocumentCounts { get; set; }
        /// <summary>
        /// Summary of the disk usage statistics for this collection.
        /// </summary>
        [JsonProperty("disk_usage", NullValueHandling = NullValueHandling.Ignore)]
        public CollectionDiskUsage DiskUsage { get; set; }
        /// <summary>
        /// Training status details.
        /// </summary>
        [JsonProperty("training_status", NullValueHandling = NullValueHandling.Ignore)]
        public TrainingStatus TrainingStatus { get; set; }
        /// <summary>
        /// Object containing information about the crawl status of this collection.
        /// </summary>
        [JsonProperty("crawl_status", NullValueHandling = NullValueHandling.Ignore)]
        public CollectionCrawlStatus CrawlStatus { get; set; }
        /// <summary>
        /// Object containing smart document understanding information for this collection.
        /// </summary>
        [JsonProperty("smart_document_understanding", NullValueHandling = NullValueHandling.Ignore)]
        public SduStatus SmartDocumentUnderstanding { get; set; }
    }
}
