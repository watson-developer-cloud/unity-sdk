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

using System.Collections.Generic;
using FullSerializer;

namespace IBM.Watson.Assistant.v1.Model
{
    /// <summary>
    /// UpdateEntity.
    /// </summary>
    public class UpdateEntity
    {
        /// <summary>
        /// The name of the entity. This string must conform to the following restrictions:
        /// - It can contain only Unicode alphanumeric, underscore, and hyphen characters.
        /// - It cannot begin with the reserved prefix `sys-`.
        /// - It must be no longer than 64 characters.
        /// </summary>
        [fsProperty("entity")]
        public string Entity { get; set; }
        /// <summary>
        /// The description of the entity. This string cannot contain carriage return, newline, or tab characters, and
        /// it must be no longer than 128 characters.
        /// </summary>
        [fsProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// Any metadata related to the entity.
        /// </summary>
        [fsProperty("metadata")]
        public object Metadata { get; set; }
        /// <summary>
        /// Whether to use fuzzy matching for the entity.
        /// </summary>
        [fsProperty("fuzzy_match")]
        public bool? FuzzyMatch { get; set; }
        /// <summary>
        /// An array of entity values.
        /// </summary>
        [fsProperty("values")]
        public List<CreateValue> Values { get; set; }
    }


}
