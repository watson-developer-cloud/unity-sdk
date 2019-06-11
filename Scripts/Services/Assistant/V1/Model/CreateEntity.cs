/**
* Copyright 2018, 2019 IBM Corp. All Rights Reserved.
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
using Newtonsoft.Json;
using System;

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// CreateEntity.
    /// </summary>
    public class CreateEntity
    {
        /// <summary>
        /// The name of the entity. This string must conform to the following restrictions:
        /// - It can contain only Unicode alphanumeric, underscore, and hyphen characters.
        /// - If you specify an entity name beginning with the reserved prefix `sys-`, it must be the name of a system
        /// entity that you want to enable. (Any entity content specified with the request is ignored.).
        /// </summary>
        [JsonProperty("entity", NullValueHandling = NullValueHandling.Ignore)]
        public string Entity { get; set; }
        /// <summary>
        /// The description of the entity. This string cannot contain carriage return, newline, or tab characters.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        /// <summary>
        /// Any metadata related to the entity.
        /// </summary>
        [JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Metadata { get; set; }
        /// <summary>
        /// Whether to use fuzzy matching for the entity.
        /// </summary>
        [JsonProperty("fuzzy_match", NullValueHandling = NullValueHandling.Ignore)]
        public bool? FuzzyMatch { get; set; }
        /// <summary>
        /// The timestamp for creation of the object.
        /// </summary>
        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public virtual DateTime? Created { get; private set; }
        /// <summary>
        /// The timestamp for the most recent update to the object.
        /// </summary>
        [JsonProperty("updated", NullValueHandling = NullValueHandling.Ignore)]
        public virtual DateTime? Updated { get; private set; }
        /// <summary>
        /// An array of objects describing the entity values.
        /// </summary>
        [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
        public List<CreateValue> Values { get; set; }
    }
}
