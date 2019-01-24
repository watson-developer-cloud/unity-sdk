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

using System.Collections.Generic;
using FullSerializer;

namespace IBM.Watson.Discovery.v1.Model
{
    /// <summary>
    /// QueryNoticesResult.
    /// </summary>
    public class QueryNoticesResult
    {
        /// <summary>
        /// The type of the original source file.
        /// </summary>
        public class FileTypeEnumValue
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
        /// </summary>
        [fsProperty("file_type")]
        public string FileType { get; set; }
        /// <summary>
        /// The unique identifier of the document.
        /// </summary>
        [fsProperty("id")]
        public string Id { get; set; }
        /// <summary>
        /// *Deprecated* This field is now part of the **result_metadata** object.
        /// </summary>
        [fsProperty("score")]
        public double? Score { get; set; }
        /// <summary>
        /// Metadata of the document.
        /// </summary>
        [fsProperty("metadata")]
        public object Metadata { get; set; }
        /// <summary>
        /// The collection ID of the collection containing the document for this result.
        /// </summary>
        [fsProperty("collection_id")]
        public string CollectionId { get; set; }
        /// <summary>
        /// Metadata of the query result.
        /// </summary>
        [fsProperty("result_metadata")]
        public QueryResultMetadata ResultMetadata { get; set; }
        /// <summary>
        /// The internal status code returned by the ingestion subsystem indicating the overall result of ingesting the
        /// source document.
        /// </summary>
        [fsProperty("code")]
        public long? Code { get; set; }
        /// <summary>
        /// Name of the original source file (if available).
        /// </summary>
        [fsProperty("filename")]
        public string Filename { get; set; }
        /// <summary>
        /// The SHA-1 hash of the original source file (formatted as a hexadecimal string).
        /// </summary>
        [fsProperty("sha1")]
        public string Sha1 { get; set; }
        /// <summary>
        /// Array of notices for the document.
        /// </summary>
        [fsProperty("notices")]
        public List<Notice> Notices { get; set; }
    }


}
