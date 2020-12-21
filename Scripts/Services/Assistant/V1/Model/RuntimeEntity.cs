/**
* (C) Copyright IBM Corp. 2018, 2020.
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

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// A term from the request that was identified as an entity.
    /// </summary>
    public class RuntimeEntity
    {
        /// <summary>
        /// An entity detected in the input.
        /// </summary>
        [JsonProperty("entity", NullValueHandling = NullValueHandling.Ignore)]
        public string Entity { get; set; }
        /// <summary>
        /// An array of zero-based character offsets that indicate where the detected entity values begin and end in the
        /// input text.
        /// </summary>
        [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
        public List<long?> Location { get; set; }
        /// <summary>
        /// The entity value that was recognized in the user input.
        /// </summary>
        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }
        /// <summary>
        /// A decimal percentage that represents Watson's confidence in the recognized entity.
        /// </summary>
        [JsonProperty("confidence", NullValueHandling = NullValueHandling.Ignore)]
        public float? Confidence { get; set; }
        /// <summary>
        /// Any metadata for the entity.
        /// </summary>
        [JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Metadata { get; set; }
        /// <summary>
        /// The recognized capture groups for the entity, as defined by the entity pattern.
        /// </summary>
        [JsonProperty("groups", NullValueHandling = NullValueHandling.Ignore)]
        public List<CaptureGroup> Groups { get; set; }
        /// <summary>
        /// An object containing detailed information about the entity recognized in the user input.
        ///
        /// For more information about how system entities are interpreted, see the
        /// [documentation](https://cloud.ibm.com/docs/assistant?topic=assistant-system-entities).
        /// </summary>
        [JsonProperty("interpretation", NullValueHandling = NullValueHandling.Ignore)]
        public RuntimeEntityInterpretation Interpretation { get; set; }
        /// <summary>
        /// An array of possible alternative values that the user might have intended instead of the value returned in
        /// the **value** property. This property is returned only for `@sys-time` and `@sys-date` entities when the
        /// user's input is ambiguous.
        ///
        /// This property is included only if the new system entities are enabled for the workspace.
        /// </summary>
        [JsonProperty("alternatives", NullValueHandling = NullValueHandling.Ignore)]
        public List<RuntimeEntityAlternative> Alternatives { get; set; }
        /// <summary>
        /// An object describing the role played by a system entity that is specifies the beginning or end of a range
        /// recognized in the user input. This property is included only if the new system entities are enabled for the
        /// workspace.
        /// </summary>
        [JsonProperty("role", NullValueHandling = NullValueHandling.Ignore)]
        public RuntimeEntityRole Role { get; set; }
    }
}
