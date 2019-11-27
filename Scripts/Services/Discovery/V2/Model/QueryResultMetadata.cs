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
    /// Metadata of a query result.
    /// </summary>
    public class QueryResultMetadata
    {
        /// <summary>
        /// The document retrieval source that produced this search result.
        /// </summary>
        public class DocumentRetrievalSourceValue
        {
            /// <summary>
            /// Constant SEARCH for search
            /// </summary>
            public const string SEARCH = "search";
            /// <summary>
            /// Constant CURATION for curation
            /// </summary>
            public const string CURATION = "curation";
            
        }

        /// <summary>
        /// The document retrieval source that produced this search result.
        /// Constants for possible values can be found using QueryResultMetadata.DocumentRetrievalSourceValue
        /// </summary>
        [JsonProperty("document_retrieval_source", NullValueHandling = NullValueHandling.Ignore)]
        public string DocumentRetrievalSource { get; set; }
        /// <summary>
        /// The collection id associated with this training data set.
        /// </summary>
        [JsonProperty("collection_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CollectionId { get; set; }
        /// <summary>
        /// The confidence score for the given result. Calculated based on how relevant the result is estimated to be.
        /// confidence can range from `0.0` to `1.0`. The higher the number, the more relevant the document. The
        /// `confidence` value for a result was calculated using the model specified in the
        /// `document_retrieval_strategy` field of the result set. This field is only returned if the
        /// **natural_language_query** parameter is specified in the query.
        /// </summary>
        [JsonProperty("confidence", NullValueHandling = NullValueHandling.Ignore)]
        public double? Confidence { get; set; }
    }
}
