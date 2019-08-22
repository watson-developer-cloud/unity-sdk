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
    /// AlignedElement.
    /// </summary>
    public class AlignedElement
    {
        /// <summary>
        /// Identifies two elements that semantically align between the compared documents.
        /// </summary>
        [JsonProperty("element_pair", NullValueHandling = NullValueHandling.Ignore)]
        public List<ElementPair> ElementPair { get; set; }
        /// <summary>
        /// Specifies whether the aligned element is identical. Elements are considered identical despite minor
        /// differences such as leading punctuation, end-of-sentence punctuation, whitespace, the presence or absence of
        /// definite or indefinite articles, and others.
        /// </summary>
        [JsonProperty("identical_text", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IdenticalText { get; set; }
        /// <summary>
        /// Hashed values that you can send to IBM to provide feedback or receive support.
        /// </summary>
        [JsonProperty("provenance_ids", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ProvenanceIds { get; set; }
        /// <summary>
        /// Indicates that the elements aligned are contractual clauses of significance.
        /// </summary>
        [JsonProperty("significant_elements", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SignificantElements { get; set; }
    }
}
