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
    /// Object containing Entity query response information.
    /// </summary>
    [fsObject]
    public class QueryEntitiesResponseItem
    {
        /// <summary>
        /// Entity text content.
        /// </summary>
        [fsProperty("text")]
        public string Text { get; set; }
        /// <summary>
        /// The type of the result entity.
        /// </summary>
        [fsProperty("type")]
        public string Type { get; set; }
        /// <summary>
        /// List of different evidentiary items to support the result.
        /// </summary>
        [fsProperty("evidence")]
        public List<QueryEvidence> Evidence { get; set; }
    }

}
