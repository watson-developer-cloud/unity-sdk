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

namespace IBM.Watson.Discovery.v1.Model
{
    /// <summary>
    /// QueryResult.
    /// </summary>
    public class QueryResult
    {
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
    }


}
