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
using IBM.Cloud.SDK.Model;
using Newtonsoft.Json;

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// Query result object.
    /// </summary>
    public class QueryNoticesResult: DynamicModel<object>
    {
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
        /// The type of the original source file.
        /// Constants for possible values can be found using QueryNoticesResult.FileTypeValue
        /// </summary>
        [JsonProperty("file_type", NullValueHandling = NullValueHandling.Ignore)]
        public string FileType { get; set; }
        /// <summary>
        /// The unique identifier of the document.
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
        /// <summary>
        /// Metadata of the document.
        /// </summary>
        [JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Metadata { get; set; }
        /// <summary>
        /// The collection ID of the collection containing the document for this result.
        /// </summary>
        [JsonProperty("collection_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CollectionId { get; set; }
        /// <summary>
        /// Metadata of a query result.
        /// </summary>
        [JsonProperty("result_metadata", NullValueHandling = NullValueHandling.Ignore)]
        public QueryResultMetadata ResultMetadata { get; set; }
        /// <summary>
        /// The internal status code returned by the ingestion subsystem indicating the overall result of ingesting the
        /// source document.
        /// </summary>
        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        public long? Code { get; set; }
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
        /// Array of notices for the document.
        /// </summary>
        [JsonProperty("notices", NullValueHandling = NullValueHandling.Ignore)]
        public List<Notice> Notices { get; set; }
    }
}
