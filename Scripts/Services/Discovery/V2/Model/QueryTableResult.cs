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

namespace IBM.Watson.Discovery.V2.Model
{
    /// <summary>
    /// A tables whose content or context match a search query.
    /// </summary>
    public class QueryTableResult
    {
        /// <summary>
        /// The identifier for the retrieved table.
        /// </summary>
        [JsonProperty("table_id", NullValueHandling = NullValueHandling.Ignore)]
        public string TableId { get; set; }
        /// <summary>
        /// The identifier of the document the table was retrieved from.
        /// </summary>
        [JsonProperty("source_document_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SourceDocumentId { get; set; }
        /// <summary>
        /// The identifier of the collection the table was retrieved from.
        /// </summary>
        [JsonProperty("collection_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CollectionId { get; set; }
        /// <summary>
        /// HTML snippet of the table info.
        /// </summary>
        [JsonProperty("table_html", NullValueHandling = NullValueHandling.Ignore)]
        public string TableHtml { get; set; }
        /// <summary>
        /// The offset of the table html snippet in the original document html.
        /// </summary>
        [JsonProperty("table_html_offset", NullValueHandling = NullValueHandling.Ignore)]
        public long? TableHtmlOffset { get; set; }
        /// <summary>
        /// Full table object retrieved from Table Understanding Enrichment.
        /// </summary>
        [JsonProperty("table", NullValueHandling = NullValueHandling.Ignore)]
        public TableResultTable Table { get; set; }
    }
}
