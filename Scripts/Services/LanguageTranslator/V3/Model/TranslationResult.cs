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

namespace IBM.Watson.LanguageTranslator.V3.Model
{
    /// <summary>
    /// TranslationResult.
    /// </summary>
    public class TranslationResult
    {
        /// <summary>
        /// An estimate of the number of words in the input text.
        /// </summary>
        [JsonProperty("word_count", NullValueHandling = NullValueHandling.Ignore)]
        public long? WordCount { get; set; }
        /// <summary>
        /// Number of characters in the input text.
        /// </summary>
        [JsonProperty("character_count", NullValueHandling = NullValueHandling.Ignore)]
        public long? CharacterCount { get; set; }
        /// <summary>
        /// The language code of the source text if the source language was automatically detected.
        /// </summary>
        [JsonProperty("detected_language", NullValueHandling = NullValueHandling.Ignore)]
        public string DetectedLanguage { get; set; }
        /// <summary>
        /// A score between 0 and 1 indicating the confidence of source language detection. A higher value indicates
        /// greater confidence. This is returned only when the service automatically detects the source language.
        /// </summary>
        [JsonProperty("detected_language_confidence", NullValueHandling = NullValueHandling.Ignore)]
        public double? DetectedLanguageConfidence { get; set; }
        /// <summary>
        /// List of translation output in UTF-8, corresponding to the input text entries.
        /// </summary>
        [JsonProperty("translations", NullValueHandling = NullValueHandling.Ignore)]
        public List<Translation> Translations { get; set; }
    }
}
