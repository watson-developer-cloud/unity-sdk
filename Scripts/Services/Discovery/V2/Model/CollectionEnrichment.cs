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
    /// An object describing an Enrichment for a collection.
    /// </summary>
    public class CollectionEnrichment
    {
        /// <summary>
        /// The unique identifier of this enrichment.
        /// </summary>
        [JsonProperty("enrichment_id", NullValueHandling = NullValueHandling.Ignore)]
        public string EnrichmentId { get; set; }
        /// <summary>
        /// An array of field names that the enrichment is applied to.
        /// </summary>
        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Fields { get; set; }
    }
}
