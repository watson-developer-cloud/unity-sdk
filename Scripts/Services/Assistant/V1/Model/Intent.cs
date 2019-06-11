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
    /// Intent.
    /// </summary>
    public class Intent
    {
        /// <summary>
        /// The name of the intent. This string must conform to the following restrictions:
        /// - It can contain only Unicode alphanumeric, underscore, hyphen, and dot characters.
        /// - It cannot begin with the reserved prefix `sys-`.
        /// </summary>
        [JsonProperty("intent", NullValueHandling = NullValueHandling.Ignore)]
        public string _Intent { get; set; }
        /// <summary>
        /// The description of the intent. This string cannot contain carriage return, newline, or tab characters.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
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
        /// An array of user input examples for the intent.
        /// </summary>
        [JsonProperty("examples", NullValueHandling = NullValueHandling.Ignore)]
        public List<Example> Examples { get; set; }
    }
}
