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

namespace IBM.Watson.NaturalLanguageUnderstanding.V1.Model
{
    /// <summary>
    /// Recognizes when two entities are related and identifies the type of relation. For example, an `awardedTo`
    /// relation might connect the entities "Nobel Prize" and "Albert Einstein". See [Relation
    /// types](https://cloud.ibm.com/docs/services/natural-language-understanding?topic=natural-language-understanding-relations).
    ///
    /// Supported languages: Arabic, English, German, Japanese, Korean, Spanish. Chinese, Dutch, French, Italian, and
    /// Portuguese custom models are also supported.
    /// </summary>
    public class RelationsOptions
    {
        /// <summary>
        /// Enter a [custom
        /// model](https://cloud.ibm.com/docs/services/natural-language-understanding?topic=natural-language-understanding-customizing)
        /// ID to override the default model.
        /// </summary>
        [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
        public string Model { get; set; }
    }
}
