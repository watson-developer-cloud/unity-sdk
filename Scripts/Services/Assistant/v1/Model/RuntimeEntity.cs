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

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// A term from the request that was identified as an entity.
    /// </summary>
    [fsObject]
    public class RuntimeEntity
    {
        /// <summary>
        /// The recognized entity from a term in the input.
        /// </summary>
        /// <value>The recognized entity from a term in the input.</value>
        public Dictionary<string, object> Entity { get; set; }
        /// <summary>
        /// Zero-based character offsets that indicate where the entity value begins and ends in the input text.
        /// </summary>
        /// <value>Zero-based character offsets that indicate where the entity value begins and ends in the input text.</value>
        public Dictionary<string, object> Location { get; set; }
        /// <summary>
        /// The term in the input text that was recognized.
        /// </summary>
        /// <value>The term in the input text that was recognized.</value>
        public Dictionary<string, object> Value { get; set; }
        /// <summary>
        /// A decimal percentage that represents Watson's confidence in the entity.
        /// </summary>
        /// <value>A decimal percentage that represents Watson's confidence in the entity.</value>
        public Dictionary<string, object> Confidence { get; set; }
        /// <summary>
        /// The metadata for the entity.
        /// </summary>
        /// <value>The metadata for the entity.</value>
        public Dictionary<string, object> Metadata { get; set; }
        /// <summary>
        /// The recognized capture groups for the entity, as defined by the entity pattern.
        /// </summary>
        /// <value>The recognized capture groups for the entity, as defined by the entity pattern.</value>
        public Dictionary<string, object> Groups { get; set; }
    }

}
