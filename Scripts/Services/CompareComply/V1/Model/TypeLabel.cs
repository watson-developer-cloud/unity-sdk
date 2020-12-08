/**
* (C) Copyright IBM Corp. 2019, 2020.
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

namespace IBM.Watson.CompareComply.V1.Model
{
    /// <summary>
    /// Identification of a specific type.
    /// </summary>
    public class TypeLabel
    {
        /// <summary>
        /// The type of modification of the feedback entry in the updated labels response.
        /// </summary>
        public class ModificationValue
        {
            /// <summary>
            /// Constant ADDED for added
            /// </summary>
            public const string ADDED = "added";
            /// <summary>
            /// Constant UNCHANGED for unchanged
            /// </summary>
            public const string UNCHANGED = "unchanged";
            /// <summary>
            /// Constant REMOVED for removed
            /// </summary>
            public const string REMOVED = "removed";
            
        }

        /// <summary>
        /// The type of modification of the feedback entry in the updated labels response.
        /// Constants for possible values can be found using TypeLabel.ModificationValue
        /// </summary>
        [JsonProperty("modification", NullValueHandling = NullValueHandling.Ignore)]
        public string Modification { get; set; }
        /// <summary>
        /// A pair of `nature` and `party` objects. The `nature` object identifies the effect of the element on the
        /// identified `party`, and the `party` object identifies the affected party.
        /// </summary>
        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public Label Label { get; set; }
        /// <summary>
        /// Hashed values that you can send to IBM to provide feedback or receive support.
        /// </summary>
        [JsonProperty("provenance_ids", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ProvenanceIds { get; set; }
    }
}
