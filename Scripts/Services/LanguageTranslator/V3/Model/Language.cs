/**
* (C) Copyright IBM Corp. 2020.
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

namespace IBM.Watson.LanguageTranslator.V3.Model
{
    /// <summary>
    /// Response payload for languages.
    /// </summary>
    public class Language
    {
        /// <summary>
        /// The language code for the language (for example, `af`).
        /// </summary>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string _Language { get; set; }
        /// <summary>
        /// The name of the language in English (for example, `Afrikaans`).
        /// </summary>
        [JsonProperty("language_name", NullValueHandling = NullValueHandling.Ignore)]
        public string LanguageName { get; set; }
        /// <summary>
        /// The native name of the language (for example, `Afrikaans`).
        /// </summary>
        [JsonProperty("native_language_name", NullValueHandling = NullValueHandling.Ignore)]
        public string NativeLanguageName { get; set; }
        /// <summary>
        /// The country code for the language (for example, `ZA` for South Africa).
        /// </summary>
        [JsonProperty("country_code", NullValueHandling = NullValueHandling.Ignore)]
        public string CountryCode { get; set; }
        /// <summary>
        /// Indicates whether words of the language are separated by whitespace: `true` if the words are separated;
        /// `false` otherwise.
        /// </summary>
        [JsonProperty("words_separated", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WordsSeparated { get; set; }
        /// <summary>
        /// Indicates the direction of the language: `right_to_left` or `left_to_right`.
        /// </summary>
        [JsonProperty("direction", NullValueHandling = NullValueHandling.Ignore)]
        public string Direction { get; set; }
        /// <summary>
        /// Indicates whether the language can be used as the source for translation: `true` if the language can be used
        /// as the source; `false` otherwise.
        /// </summary>
        [JsonProperty("supported_as_source", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SupportedAsSource { get; set; }
        /// <summary>
        /// Indicates whether the language can be used as the target for translation: `true` if the language can be used
        /// as the target; `false` otherwise.
        /// </summary>
        [JsonProperty("supported_as_target", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SupportedAsTarget { get; set; }
        /// <summary>
        /// Indicates whether the language supports automatic detection: `true` if the language can be detected
        /// automatically; `false` otherwise.
        /// </summary>
        [JsonProperty("identifiable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Identifiable { get; set; }
    }
}
