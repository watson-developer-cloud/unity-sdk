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

namespace IBM.Watson.LanguageTranslator.V3.Model
{
    /// <summary>
    /// TranslationResult.
    /// </summary>
    public class TranslationResult
    {
        /// <summary>
        /// Number of words in the input text.
        /// </summary>
        [JsonProperty("word_count", NullValueHandling = NullValueHandling.Ignore)]
        public long? WordCount { get; set; }
        /// <summary>
        /// Number of characters in the input text.
        /// </summary>
        [JsonProperty("character_count", NullValueHandling = NullValueHandling.Ignore)]
        public long? CharacterCount { get; set; }
        /// <summary>
        /// List of translation output in UTF-8, corresponding to the input text entries.
        /// </summary>
        [JsonProperty("translations", NullValueHandling = NullValueHandling.Ignore)]
        public List<Translation> Translations { get; set; }
    }
}
