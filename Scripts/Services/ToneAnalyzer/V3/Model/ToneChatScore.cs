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

namespace IBM.Watson.ToneAnalyzer.V3.Model
{
    /// <summary>
    /// The score for an utterance from the input content.
    /// </summary>
    public class ToneChatScore
    {
        /// <summary>
        /// The unique, non-localized identifier of the tone for the results. The service returns results only for tones
        /// whose scores meet a minimum threshold of 0.5.
        /// </summary>
        public class ToneIdValue
        {
            /// <summary>
            /// Constant EXCITED for excited
            /// </summary>
            public const string EXCITED = "excited";
            /// <summary>
            /// Constant FRUSTRATED for frustrated
            /// </summary>
            public const string FRUSTRATED = "frustrated";
            /// <summary>
            /// Constant IMPOLITE for impolite
            /// </summary>
            public const string IMPOLITE = "impolite";
            /// <summary>
            /// Constant POLITE for polite
            /// </summary>
            public const string POLITE = "polite";
            /// <summary>
            /// Constant SAD for sad
            /// </summary>
            public const string SAD = "sad";
            /// <summary>
            /// Constant SATISFIED for satisfied
            /// </summary>
            public const string SATISFIED = "satisfied";
            /// <summary>
            /// Constant SYMPATHETIC for sympathetic
            /// </summary>
            public const string SYMPATHETIC = "sympathetic";
            
        }

        /// <summary>
        /// The unique, non-localized identifier of the tone for the results. The service returns results only for tones
        /// whose scores meet a minimum threshold of 0.5.
        /// Constants for possible values can be found using ToneChatScore.ToneIdValue
        /// </summary>
        [JsonProperty("tone_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ToneId { get; set; }
        /// <summary>
        /// The score for the tone in the range of 0.5 to 1. A score greater than 0.75 indicates a high likelihood that
        /// the tone is perceived in the utterance.
        /// </summary>
        [JsonProperty("score", NullValueHandling = NullValueHandling.Ignore)]
        public double? Score { get; set; }
        /// <summary>
        /// The user-visible, localized name of the tone.
        /// </summary>
        [JsonProperty("tone_name", NullValueHandling = NullValueHandling.Ignore)]
        public string ToneName { get; set; }
    }
}
