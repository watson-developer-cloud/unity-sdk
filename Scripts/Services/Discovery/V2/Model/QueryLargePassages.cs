/**
* (C) Copyright IBM Corp. 2019, 2021.
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

namespace IBM.Watson.Discovery.V2.Model
{
    /// <summary>
    /// Configuration for passage retrieval.
    /// </summary>
    public class QueryLargePassages
    {
        /// <summary>
        /// A passages query that returns the most relevant passages from the results.
        /// </summary>
        [JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enabled { get; set; }
        /// <summary>
        /// If `true`, ranks the documents by document quality, and then returns the highest-ranked passages per
        /// document in a `document_passages` field for each document entry in the results list of the response.
        ///
        /// If `false`, ranks the passages from all of the documents by passage quality regardless of the document
        /// quality and returns them in a separate `passages` field in the response.
        /// </summary>
        [JsonProperty("per_document", NullValueHandling = NullValueHandling.Ignore)]
        public bool? PerDocument { get; set; }
        /// <summary>
        /// Maximum number of passages to return per document in the result. Ignored if `passages.per_document` is
        /// `false`.
        /// </summary>
        [JsonProperty("max_per_document", NullValueHandling = NullValueHandling.Ignore)]
        public long? MaxPerDocument { get; set; }
        /// <summary>
        /// A list of fields to extract passages from. If this parameter is an empty list, then all root-level fields
        /// are included.
        /// </summary>
        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Fields { get; set; }
        /// <summary>
        /// The maximum number of passages to return. Ignored if `passages.per_document` is `true`.
        /// </summary>
        [JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
        public long? Count { get; set; }
        /// <summary>
        /// The approximate number of characters that any one passage will have.
        /// </summary>
        [JsonProperty("characters", NullValueHandling = NullValueHandling.Ignore)]
        public long? Characters { get; set; }
        /// <summary>
        /// When true, `answer` objects are returned as part of each passage in the query results. The primary
        /// difference between an `answer` and a `passage` is that the length of a passage is defined by the query,
        /// where the length of an `answer` is calculated by Discovery based on how much text is needed to answer the
        /// question.
        ///
        /// This parameter is ignored if passages are not enabled for the query, or no **natural_language_query** is
        /// specified.
        ///
        /// If the **find_answers** parameter is set to `true` and **per_document** parameter is also set to `true`,
        /// then the document search results and the passage search results within each document are reordered using the
        /// answer confidences. The goal of this reordering is to place the best answer as the first answer of the first
        /// passage of the first document. Similarly, if the **find_answers** parameter is set to `true` and
        /// **per_document** parameter is set to `false`, then the passage search results are reordered in decreasing
        /// order of the highest confidence answer for each document and passage.
        ///
        /// The **find_answers** parameter is available only on managed instances of Discovery.
        /// </summary>
        [JsonProperty("find_answers", NullValueHandling = NullValueHandling.Ignore)]
        public bool? FindAnswers { get; set; }
        /// <summary>
        /// The number of `answer` objects to return per passage if the **find_answers** parmeter is specified as
        /// `true`.
        /// </summary>
        [JsonProperty("max_answers_per_passage", NullValueHandling = NullValueHandling.Ignore)]
        public long? MaxAnswersPerPassage { get; set; }
    }
}
