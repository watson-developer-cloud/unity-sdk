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

namespace IBM.Watson.PersonalityInsights.V3.Model
{
    /// <summary>
    /// The personality profile that the service generated for the input content.
    /// </summary>
    public class Profile
    {
        /// <summary>
        /// The language model that was used to process the input.
        /// </summary>
        public class ProcessedLanguageValue
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
            /// Constant ES for es
            /// </summary>
            public const string ES = "es";
            /// <summary>
            /// Constant JA for ja
            /// </summary>
            public const string JA = "ja";
            /// <summary>
            /// Constant KO for ko
            /// </summary>
            public const string KO = "ko";
            
        }

        /// <summary>
        /// The language model that was used to process the input.
        /// Constants for possible values can be found using Profile.ProcessedLanguageValue
        /// </summary>
        [JsonProperty("processed_language", NullValueHandling = NullValueHandling.Ignore)]
        public string ProcessedLanguage { get; set; }
        /// <summary>
        /// The number of words from the input that were used to produce the profile.
        /// </summary>
        [JsonProperty("word_count", NullValueHandling = NullValueHandling.Ignore)]
        public long? WordCount { get; set; }
        /// <summary>
        /// When guidance is appropriate, a string that provides a message that indicates the number of words found and
        /// where that value falls in the range of required or suggested number of words.
        /// </summary>
        [JsonProperty("word_count_message", NullValueHandling = NullValueHandling.Ignore)]
        public string WordCountMessage { get; set; }
        /// <summary>
        /// A recursive array of `Trait` objects that provides detailed results for the Big Five personality
        /// characteristics (dimensions and facets) inferred from the input text.
        /// </summary>
        [JsonProperty("personality", NullValueHandling = NullValueHandling.Ignore)]
        public List<Trait> Personality { get; set; }
        /// <summary>
        /// Detailed results for the Needs characteristics inferred from the input text.
        /// </summary>
        [JsonProperty("needs", NullValueHandling = NullValueHandling.Ignore)]
        public List<Trait> Needs { get; set; }
        /// <summary>
        /// Detailed results for the Values characteristics inferred from the input text.
        /// </summary>
        [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
        public List<Trait> Values { get; set; }
        /// <summary>
        /// For JSON content that is timestamped, detailed results about the social behavior disclosed by the input in
        /// terms of temporal characteristics. The results include information about the distribution of the content
        /// over the days of the week and the hours of the day.
        /// </summary>
        [JsonProperty("behavior", NullValueHandling = NullValueHandling.Ignore)]
        public List<Behavior> Behavior { get; set; }
        /// <summary>
        /// If the **consumption_preferences** parameter is `true`, detailed results for each category of consumption
        /// preferences. Each element of the array provides information inferred from the input text for the individual
        /// preferences of that category.
        /// </summary>
        [JsonProperty("consumption_preferences", NullValueHandling = NullValueHandling.Ignore)]
        public List<ConsumptionPreferencesCategory> ConsumptionPreferences { get; set; }
        /// <summary>
        /// An array of warning messages that are associated with the input text for the request. The array is empty if
        /// the input generated no warnings.
        /// </summary>
        [JsonProperty("warnings", NullValueHandling = NullValueHandling.Ignore)]
        public List<Warning> Warnings { get; set; }
    }
}
