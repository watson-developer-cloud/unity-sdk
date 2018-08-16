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
    /// Entity description and location within evidence field.
    /// </summary>
    [fsObject]
    public class QueryEvidenceEntity
    {
        /// <summary>
        /// The entity type for this entity. Possible types vary based on model used.
        /// </summary>
        [fsProperty("type")]
        public string Type { get; set; }
        /// <summary>
        /// The original text of this entity as found in the evidence field.
        /// </summary>
        [fsProperty("text")]
        public string Text { get; set; }
        /// <summary>
        /// The start location of the entity text in the identified field. This value is inclusive.
        /// </summary>
        [fsProperty("start_offset")]
        public long? StartOffset { get; set; }
        /// <summary>
        /// The end location of the entity text in the identified field. This value is exclusive.
        /// </summary>
        [fsProperty("end_offset")]
        public long? EndOffset { get; set; }
    }

}
