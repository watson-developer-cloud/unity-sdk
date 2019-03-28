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

namespace IBM.Watson.NaturalLanguageClassifier.V1.Model
{
    /// <summary>
    /// Class and confidence.
    /// </summary>
    public class ClassifiedClass
    {
        /// <summary>
        /// A decimal percentage that represents the confidence that Watson has in this class. Higher values represent
        /// higher confidences.
        /// </summary>
        [JsonProperty("confidence", NullValueHandling = NullValueHandling.Ignore)]
        public double? Confidence { get; set; }
        /// <summary>
        /// Class label.
        /// </summary>
        [JsonProperty("class_name", NullValueHandling = NullValueHandling.Ignore)]
        public string ClassName { get; set; }
    }
}
