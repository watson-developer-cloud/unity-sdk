/**
* (C) Copyright IBM Corp. 2020.
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

namespace IBM.Watson.Discovery.V2.Model
{
    /// <summary>
    /// Default settings configuration for passage search options.
    /// </summary>
    public class DefaultQueryParamsPassages
    {
        /// <summary>
        /// When `true`, a passage search is performed by default.
        /// </summary>
        [JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enabled { get; set; }
        /// <summary>
        /// The number of passages to return.
        /// </summary>
        [JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
        public long? Count { get; set; }
        /// <summary>
        /// An array of field names to perform the passage search on.
        /// </summary>
        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Fields { get; set; }
        /// <summary>
        /// The approximate number of characters that each returned passage will contain.
        /// </summary>
        [JsonProperty("characters", NullValueHandling = NullValueHandling.Ignore)]
        public long? Characters { get; set; }
        /// <summary>
        /// When `true` the number of passages that can be returned from a single document is restricted to the
        /// *max_per_document* value.
        /// </summary>
        [JsonProperty("per_document", NullValueHandling = NullValueHandling.Ignore)]
        public bool? PerDocument { get; set; }
        /// <summary>
        /// The default maximum number of passages that can be taken from a single document as the result of a passage
        /// query.
        /// </summary>
        [JsonProperty("max_per_document", NullValueHandling = NullValueHandling.Ignore)]
        public long? MaxPerDocument { get; set; }
    }
}
