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
    /// The updated labeling from the input document, accounting for the submitted feedback.
    /// </summary>
    public class UpdatedLabelsIn
    {
        /// <summary>
        /// Description of the action specified by the element and whom it affects.
        /// </summary>
        [JsonProperty("types", NullValueHandling = NullValueHandling.Ignore)]
        public List<TypeLabel> Types { get; set; }
        /// <summary>
        /// List of functional categories into which the element falls; in other words, the subject matter of the
        /// element.
        /// </summary>
        [JsonProperty("categories", NullValueHandling = NullValueHandling.Ignore)]
        public List<Category> Categories { get; set; }
    }
}
