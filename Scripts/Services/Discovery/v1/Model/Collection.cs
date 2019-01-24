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
    /// A collection for storing documents.
    /// </summary>
    public class Collection
    {
        /// <summary>
        /// The status of the collection.
        /// </summary>
        public class StatusEnumValue
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
        /// </summary>
        [fsProperty("status")]
        public string Status { get; set; }
        /// <summary>
        /// The unique identifier of the collection.
        /// </summary>
        [fsProperty("collection_id")]
        public virtual string CollectionId { get; private set; }
        /// <summary>
        /// The name of the collection.
        /// </summary>
        [fsProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// The description of the collection.
        /// </summary>
        [fsProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// The creation date of the collection in the format yyyy-MM-dd'T'HH:mmcon:ss.SSS'Z'.
        /// </summary>
        [fsProperty("created")]
        public virtual DateTime? Created { get; private set; }
        /// <summary>
        /// The timestamp of when the collection was last updated in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        [fsProperty("updated")]
        public virtual DateTime? Updated { get; private set; }
        /// <summary>
        /// The unique identifier of the collection's configuration.
        /// </summary>
        [fsProperty("configuration_id")]
        public string ConfigurationId { get; set; }
        /// <summary>
        /// The language of the documents stored in the collection. Permitted values include `en` (English), `de`
        /// (German), and `es` (Spanish).
        /// </summary>
        [fsProperty("language")]
        public string Language { get; set; }
        /// <summary>
        /// The object providing information about the documents in the collection. Present only when retrieving details
        /// of a collection.
        /// </summary>
        [fsProperty("document_counts")]
        public DocumentCounts DocumentCounts { get; set; }
        /// <summary>
        /// The object providing information about the disk usage of the collection. Present only when retrieving
        /// details of a collection.
        /// </summary>
        [fsProperty("disk_usage")]
        public CollectionDiskUsage DiskUsage { get; set; }
        /// <summary>
        /// Provides information about the status of relevance training for collection.
        /// </summary>
        [fsProperty("training_status")]
        public TrainingStatus TrainingStatus { get; set; }
        /// <summary>
        /// Object containing source crawl status information.
        /// </summary>
        [fsProperty("source_crawl")]
        public SourceStatus SourceCrawl { get; set; }
    }


}
