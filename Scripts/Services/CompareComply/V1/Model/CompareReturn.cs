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

namespace IBM.Watson.CompareComply.V1.Model
{
    /// <summary>
    /// The comparison of the two submitted documents.
    /// </summary>
    public class CompareReturn
    {
        /// <summary>
        /// The analysis model used to compare the input documents. For the **Compare two documents** method, the only
        /// valid value is `contracts`.
        /// </summary>
        [JsonProperty("model_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ModelId { get; set; }
        /// <summary>
        /// The version of the analysis model identified by the value of the `model_id` key.
        /// </summary>
        [JsonProperty("model_version", NullValueHandling = NullValueHandling.Ignore)]
        public string ModelVersion { get; set; }
        /// <summary>
        /// Information about the documents being compared.
        /// </summary>
        [JsonProperty("documents", NullValueHandling = NullValueHandling.Ignore)]
        public List<Document> Documents { get; set; }
        /// <summary>
        /// A list of pairs of elements that semantically align between the compared documents.
        /// </summary>
        [JsonProperty("aligned_elements", NullValueHandling = NullValueHandling.Ignore)]
        public List<AlignedElement> AlignedElements { get; set; }
        /// <summary>
        /// A list of elements that do not semantically align between the compared documents.
        /// </summary>
        [JsonProperty("unaligned_elements", NullValueHandling = NullValueHandling.Ignore)]
        public List<UnalignedElement> UnalignedElements { get; set; }
    }
}
