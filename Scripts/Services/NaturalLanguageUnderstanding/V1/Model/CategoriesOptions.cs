/**
* (C) Copyright IBM Corp. 2019, 2021.
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

namespace IBM.Watson.NaturalLanguageUnderstanding.V1.Model
{
    /// <summary>
    /// Returns a five-level taxonomy of the content. The top three categories are returned.
    ///
    /// Supported languages: Arabic, English, French, German, Italian, Japanese, Korean, Portuguese, Spanish.
    /// </summary>
    public class CategoriesOptions
    {
        /// <summary>
        /// Set this to `true` to return explanations for each categorization. **This is available only for English
        /// categories.**.
        /// </summary>
        [JsonProperty("explanation", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Explanation { get; set; }
        /// <summary>
        /// Maximum number of categories to return.
        /// </summary>
        [JsonProperty("limit", NullValueHandling = NullValueHandling.Ignore)]
        public long? Limit { get; set; }
        /// <summary>
        /// (Beta) Enter a [custom
        /// model](https://cloud.ibm.com/docs/natural-language-understanding?topic=natural-language-understanding-customizing)
        /// ID to override the standard categories model. **This is available only for English categories.**.
        /// </summary>
        [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
        public string Model { get; set; }
    }
}
