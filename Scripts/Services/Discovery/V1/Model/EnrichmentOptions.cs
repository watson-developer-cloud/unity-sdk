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

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// Options which are specific to a particular enrichment.
    /// </summary>
    public class EnrichmentOptions
    {
        /// <summary>
        /// ISO 639-1 code indicating the language to use for the analysis. This code overrides the automatic language
        /// detection performed by the service. Valid codes are `ar` (Arabic), `en` (English), `fr` (French), `de`
        /// (German), `it` (Italian), `pt` (Portuguese), `ru` (Russian), `es` (Spanish), and `sv` (Swedish). **Note:**
        /// Not all features support all languages, automatic detection is recommended.
        /// </summary>
        public class LanguageValue
        {
            /// <summary>
            /// Constant AR for ar
            /// </summary>
            public const string AR = "ar";
            /// <summary>
            /// Constant EN for en
            /// </summary>
            public const string EN = "en";
            /// <summary>
            /// Constant FR for fr
            /// </summary>
            public const string FR = "fr";
            /// <summary>
            /// Constant DE for de
            /// </summary>
            public const string DE = "de";
            /// <summary>
            /// Constant IT for it
            /// </summary>
            public const string IT = "it";
            /// <summary>
            /// Constant PT for pt
            /// </summary>
            public const string PT = "pt";
            /// <summary>
            /// Constant RU for ru
            /// </summary>
            public const string RU = "ru";
            /// <summary>
            /// Constant ES for es
            /// </summary>
            public const string ES = "es";
            /// <summary>
            /// Constant SV for sv
            /// </summary>
            public const string SV = "sv";
            
        }

        /// <summary>
        /// ISO 639-1 code indicating the language to use for the analysis. This code overrides the automatic language
        /// detection performed by the service. Valid codes are `ar` (Arabic), `en` (English), `fr` (French), `de`
        /// (German), `it` (Italian), `pt` (Portuguese), `ru` (Russian), `es` (Spanish), and `sv` (Swedish). **Note:**
        /// Not all features support all languages, automatic detection is recommended.
        /// Constants for possible values can be found using EnrichmentOptions.LanguageValue
        /// </summary>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }
        /// <summary>
        /// Object containing Natural Language Understanding features to be used.
        /// </summary>
        [JsonProperty("features", NullValueHandling = NullValueHandling.Ignore)]
        public NluEnrichmentFeatures Features { get; set; }
        /// <summary>
        /// *For use with `elements` enrichments only.* The element extraction model to use. Models available are:
        /// `contract`.
        /// </summary>
        [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
        public string Model { get; set; }
    }
}
