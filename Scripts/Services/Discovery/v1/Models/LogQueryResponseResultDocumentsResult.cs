/**
* Copyright 2018 IBM Corp. All Rights Reserved.
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

namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    /// <summary>
    /// Each object in the **results** array corresponds to an individual document returned by the original query.
    /// </summary>
    [fsObject]
    public class LogQueryResponseResultDocumentsResult
    {
        /// <summary>
        /// The result rank of this document. A position of `1` indicates that it was the first returned result.
        /// </summary>
        [fsProperty("position")]
        public long? Position { get; set; }
        /// <summary>
        /// The **document_id** of the document that this result represents.
        /// </summary>
        [fsProperty("document_id")]
        public string DocumentId { get; set; }
        /// <summary>
        /// The raw score of this result. A higher score indicates a greater match to the query parameters.
        /// </summary>
        [fsProperty("score")]
        public double? Score { get; set; }
        /// <summary>
        /// The confidence score of the result's analysis. A higher score indicating greater confidence.
        /// </summary>
        [fsProperty("confidence")]
        public double? Confidence { get; set; }
        /// <summary>
        /// The **collection_id** of the document represented by this result.
        /// </summary>
        [fsProperty("collection_id")]
        public string CollectionId { get; set; }
    }

}
