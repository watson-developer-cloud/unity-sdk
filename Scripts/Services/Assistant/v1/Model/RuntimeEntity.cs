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
        /// An entity detected in the input.
        /// </summary>
        /// <value>An entity detected in the input.</value>
        [fsProperty("entity")]
        public Dictionary<string, object> Entity { get; set; }
        /// <summary>
        /// An array of zero-based character offsets that indicate where the detected entity values begin and end in the input text.
        /// </summary>
        /// <value>An array of zero-based character offsets that indicate where the detected entity values begin and end in the input text.</value>
        [fsProperty("location")]
        public Dictionary<string, object> Location { get; set; }
        /// <summary>
        /// The term in the input text that was recognized as an entity value.
        /// </summary>
        /// <value>The term in the input text that was recognized as an entity value.</value>
        [fsProperty("value")]
        public Dictionary<string, object> Value { get; set; }
        /// <summary>
        /// A decimal percentage that represents Watson's confidence in the entity.
        /// </summary>
        /// <value>A decimal percentage that represents Watson's confidence in the entity.</value>
        [fsProperty("confidence")]
        public Dictionary<string, object> Confidence { get; set; }
        /// <summary>
        /// Any metadata for the entity.
        /// </summary>
        /// <value>Any metadata for the entity.</value>
        [fsProperty("metadata")]
        public Dictionary<string, object> Metadata { get; set; }
        /// <summary>
        /// The recognized capture groups for the entity, as defined by the entity pattern.
        /// </summary>
        /// <value>The recognized capture groups for the entity, as defined by the entity pattern.</value>
        [fsProperty("groups")]
        public Dictionary<string, object> Groups { get; set; }
    }

}
