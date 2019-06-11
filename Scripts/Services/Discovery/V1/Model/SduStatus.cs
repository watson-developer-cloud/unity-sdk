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

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// Object containing smart document understanding information for this collection.
    /// </summary>
    public class SduStatus
    {
        /// <summary>
        /// When `true`, smart document understanding conversion is enabled for this collection. All collections created
        /// with a version date after `2019-04-30` have smart document understanding enabled. If `false`, documents
        /// added to the collection are converted using the **conversion** settings specified in the configuration
        /// associated with the collection.
        /// </summary>
        [JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enabled { get; set; }
        /// <summary>
        /// The total number of pages annotated using smart document understanding in this collection.
        /// </summary>
        [JsonProperty("total_annotated_pages", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalAnnotatedPages { get; set; }
        /// <summary>
        /// The current number of pages that can be used for training smart document understanding. The `total_pages`
        /// number is calculated as the total number of pages identified from the documents listed in the
        /// **total_documents** field.
        /// </summary>
        [JsonProperty("total_pages", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalPages { get; set; }
        /// <summary>
        /// The total number of documents in this collection that can be used to train smart document understanding. For
        /// **lite** plan collections, the maximum is the first 20 uploaded documents (not including HTML or JSON
        /// documents). For other plans, the maximum is the first 40 uploaded documents (not including HTML or JSON
        /// documents). When the maximum is reached, additional documents uploaded to the collection are not considered
        /// for training smart document understanding.
        /// </summary>
        [JsonProperty("total_documents", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalDocuments { get; set; }
        /// <summary>
        /// Information about custom smart document understanding fields that exist in this collection.
        /// </summary>
        [JsonProperty("custom_fields", NullValueHandling = NullValueHandling.Ignore)]
        public SduStatusCustomFields CustomFields { get; set; }
    }
}
