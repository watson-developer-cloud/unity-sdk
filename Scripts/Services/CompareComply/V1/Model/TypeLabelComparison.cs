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

using Newtonsoft.Json;

namespace IBM.Watson.CompareComply.V1.Model
{
    /// <summary>
    /// Identification of a specific type.
    /// </summary>
    public class TypeLabelComparison
    {
        /// <summary>
        /// A pair of `nature` and `party` objects. The `nature` object identifies the effect of the element on the
        /// identified `party`, and the `party` object identifies the affected party.
        /// </summary>
        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public Label Label { get; set; }
    }
}
