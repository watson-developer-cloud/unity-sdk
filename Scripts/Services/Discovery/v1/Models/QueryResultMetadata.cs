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
    /// Metadata of a query result.
    /// </summary>
    [fsObject]
    public class QueryResultMetadata
    {
        /// <summary>
        /// An unbounded measure of the relevance of a particular result, dependent on the query and matching document.
        /// A higher score indicates a greater match to the query parameters.
        /// </summary>
        [fsProperty("score")]
        public double? Score { get; set; }
        /// <summary>
        /// The confidence score for the given result. Calculated based on how relevant the result is estimated to be,
        /// compared to a trained relevancy model. confidence can range from `0.0` to `1.0`. The higher the number, the
        /// more relevant the document.
        /// </summary>
        [fsProperty("confidence")]
        public double? Confidence { get; set; }
    }

}
