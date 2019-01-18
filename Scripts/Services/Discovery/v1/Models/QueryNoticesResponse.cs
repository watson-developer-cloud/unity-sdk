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
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    /// <summary>
    /// QueryNoticesResponse
    /// </summary>
    [fsObject]
    public class QueryNoticesResponse
    {
        /// <summary>
        /// Gets or Sets matchingResults
        /// </summary>
        [fsProperty("matching_results")]
        public long? MatchingResults { get; set; }
        /// <summary>
        /// Gets or Sets results
        /// </summary>
        [fsProperty("results")]
        public List<object> Results { get; set; }
        /// <summary>
        /// Gets or Sets aggregations
        /// </summary>
        [fsProperty("aggregations")]
        public List<QueryAggregation> Aggregations { get; set; }
        /// <summary>
        /// Gets or Sets passages
        /// </summary>
        [fsProperty("passages")]
        public List<QueryPassages> Passages { get; set; }
        /// <summary>
        /// Gets or Sets duplicatesRemoved
        /// </summary>
        [fsProperty("duplicates_removed")]
        public long? DuplicatesRemoved { get; set; }
    }

}
