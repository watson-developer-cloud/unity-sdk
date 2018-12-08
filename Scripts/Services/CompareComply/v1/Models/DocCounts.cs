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

namespace  IBM.Watson.DeveloperCloud.Services.CompareComply.v1
{
    /// <summary>
    /// Document counts.
    /// </summary>
    [fsObject]
    public class DocCounts
    {
        /// <summary>
        /// Total number of documents.
        /// </summary>
        [fsProperty("total")]
        public long? Total { get; set; }
        /// <summary>
        /// Number of pending documents.
        /// </summary>
        [fsProperty("pending")]
        public long? Pending { get; set; }
        /// <summary>
        /// Number of documents successfully processed.
        /// </summary>
        [fsProperty("successful")]
        public long? Successful { get; set; }
        /// <summary>
        /// Number of documents not successfully processed.
        /// </summary>
        [fsProperty("failed")]
        public long? Failed { get; set; }
    }

}
