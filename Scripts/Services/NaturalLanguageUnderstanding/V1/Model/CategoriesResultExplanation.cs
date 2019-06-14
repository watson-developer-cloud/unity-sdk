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

namespace IBM.Watson.NaturalLanguageUnderstanding.V1.Model
{
    /// <summary>
    /// Information that helps to explain what contributed to the categories result.
    /// </summary>
    public class CategoriesResultExplanation
    {
        /// <summary>
        /// An array of relevant text from the source that contributed to the categorization. The sorted array begins
        /// with the phrase that contributed most significantly to the result, followed by phrases that were less and
        /// less impactful.
        /// </summary>
        [JsonProperty("relevant_text", NullValueHandling = NullValueHandling.Ignore)]
        public List<CategoriesRelevantText> RelevantText { get; set; }
    }
}
