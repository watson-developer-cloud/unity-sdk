/**
* Copyright 2018 IBM Corp. All Rights Reserved.
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

using FullSerializer;
using System.Collections.Generic;

namespace  IBM.Watson.DeveloperCloud.Services.CompareComply.v1
{
    /// <summary>
    /// The comparison of the two submitted documents.
    /// </summary>
    [fsObject]
    public class CompareReturn
    {
        /// <summary>
        /// Information about the documents being compared.
        /// </summary>
        [fsProperty("documents")]
        public List<Document> Documents { get; set; }
        /// <summary>
        /// A list of pairs of elements that semantically align between the compared documents.
        /// </summary>
        [fsProperty("aligned_elements")]
        public List<AlignedElement> AlignedElements { get; set; }
        /// <summary>
        /// A list of elements that do not semantically align between the compared documents.
        /// </summary>
        [fsProperty("unaligned_elements")]
        public List<UnalignedElement> UnalignedElements { get; set; }
        /// <summary>
        /// The analysis model used to classify the input document. For the `/v1/element_classification` method, the
        /// only valid value is `contracts`.
        /// </summary>
        [fsProperty("model_id")]
        public string ModelId { get; set; }
        /// <summary>
        /// The version of the analysis model identified by the value of the `model_id` key.
        /// </summary>
        [fsProperty("model_version")]
        public string ModelVersion { get; set; }
    }

}
