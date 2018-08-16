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
using System.Runtime.Serialization;

namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    /// <summary>
    /// QueryRelationsRelationship
    /// </summary>
    [fsObject]
    public class QueryRelationsRelationship
    {
        /// <summary>
        /// The identified relationship type.
        /// </summary>
        [fsProperty("type")]
        public string Type { get; set; }
        /// <summary>
        /// The number of times the relationship is mentioned.
        /// </summary>
        [fsProperty("frequency")]
        public long? Frequency { get; set; }
        /// <summary>
        /// Information about the relationship.
        /// </summary>
        [fsProperty("arguments")]
        public List<QueryRelationsArgument> Arguments { get; set; }
        /// <summary>
        /// List of different evidentiary items to support the result.
        /// </summary>
        [fsProperty("evidence")]
        public List<QueryEvidence> Evidence { get; set; }
    }

}
