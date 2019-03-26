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
    /// Status information about a submitted document.
    /// </summary>
    public class DocumentStatus
    {
        /// <summary>
        /// Status of the document in the ingestion process.
        /// </summary>
        public class StatusValue
        {
            /// <summary>
            /// Constant AVAILABLE for available
            /// </summary>
            public const string AVAILABLE = "available";
            /// <summary>
            /// Constant AVAILABLE_WITH_NOTICES for available with notices
            /// </summary>
            public const string AVAILABLE_WITH_NOTICES = "available with notices";
            /// <summary>
            /// Constant FAILED for failed
            /// </summary>
            public const string FAILED = "failed";
            /// <summary>
            /// Constant PROCESSING for processing
            /// </summary>
            public const string PROCESSING = "processing";
            /// <summary>
            /// Constant PENDING for pending
            /// </summary>
            public const string PENDING = "pending";
            
        }

        /// <summary>
        /// The type of the original source file.
        /// </summary>
        public class FileTypeValue
        {
            /// <summary>
            /// Constant PDF for pdf
            /// </summary>
            public const string PDF = "pdf";
            /// <summary>
            /// Constant HTML for html
            /// </summary>
            public const string HTML = "html";
            /// <summary>
            /// Constant WORD for word
            /// </summary>
            public const string WORD = "word";
            /// <summary>
            /// Constant JSON for json
            /// </summary>
            public const string JSON = "json";
            
        }

        /// <summary>
        /// Status of the document in the ingestion process.
        /// Constants for possible values can be found using DocumentStatus.StatusValue
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        /// <summary>
        /// The type of the original source file.
        /// Constants for possible values can be found using DocumentStatus.FileTypeValue
        /// </summary>
        [JsonProperty("file_type", NullValueHandling = NullValueHandling.Ignore)]
        public string FileType { get; set; }
        /// <summary>
        /// The unique identifier of the document.
        /// </summary>
        [JsonProperty("document_id", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string DocumentId { get; private set; }
        /// <summary>
        /// The unique identifier for the configuration.
        /// </summary>
        [JsonProperty("configuration_id", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string ConfigurationId { get; private set; }
        /// <summary>
        /// Description of the document status.
        /// </summary>
        [JsonProperty("status_description", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string StatusDescription { get; private set; }
        /// <summary>
        /// Name of the original source file (if available).
        /// </summary>
        [JsonProperty("filename", NullValueHandling = NullValueHandling.Ignore)]
        public string Filename { get; set; }
        /// <summary>
        /// The SHA-1 hash of the original source file (formatted as a hexadecimal string).
        /// </summary>
        [JsonProperty("sha1", NullValueHandling = NullValueHandling.Ignore)]
        public string Sha1 { get; set; }
        /// <summary>
        /// Array of notices produced by the document-ingestion process.
        /// </summary>
        [JsonProperty("notices", NullValueHandling = NullValueHandling.Ignore)]
        public virtual List<Notice> Notices { get; private set; }
    }
}
