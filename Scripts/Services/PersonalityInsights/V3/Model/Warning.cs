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

namespace IBM.Watson.PersonalityInsights.V3.Model
{
    /// <summary>
    /// A warning message that is associated with the input content.
    /// </summary>
    public class Warning
    {
        /// <summary>
        /// The identifier of the warning message.
        /// </summary>
        public class WarningIdValue
        {
            /// <summary>
            /// Constant WORD_COUNT_MESSAGE for WORD_COUNT_MESSAGE
            /// </summary>
            public const string WORD_COUNT_MESSAGE = "WORD_COUNT_MESSAGE";
            /// <summary>
            /// Constant JSON_AS_TEXT for JSON_AS_TEXT
            /// </summary>
            public const string JSON_AS_TEXT = "JSON_AS_TEXT";
            /// <summary>
            /// Constant CONTENT_TRUNCATED for CONTENT_TRUNCATED
            /// </summary>
            public const string CONTENT_TRUNCATED = "CONTENT_TRUNCATED";
            /// <summary>
            /// Constant PARTIAL_TEXT_USED for PARTIAL_TEXT_USED
            /// </summary>
            public const string PARTIAL_TEXT_USED = "PARTIAL_TEXT_USED";
            
        }

        /// <summary>
        /// The identifier of the warning message.
        /// Constants for possible values can be found using Warning.WarningIdValue
        /// </summary>
        [JsonProperty("warning_id", NullValueHandling = NullValueHandling.Ignore)]
        public string WarningId { get; set; }
        /// <summary>
        /// The message associated with the `warning_id`:
        /// * `WORD_COUNT_MESSAGE`: "There were {number} words in the input. We need a minimum of 600, preferably 1,200
        /// or more, to compute statistically significant estimates."
        /// * `JSON_AS_TEXT`: "Request input was processed as text/plain as indicated, however detected a JSON input.
        /// Did you mean application/json?"
        /// * `CONTENT_TRUNCATED`: "For maximum accuracy while also optimizing processing time, only the first 250KB of
        /// input text (excluding markup) was analyzed. Accuracy levels off at approximately 3,000 words so this did not
        /// affect the accuracy of the profile."
        /// * `PARTIAL_TEXT_USED`, "The text provided to compute the profile was trimmed for performance reasons. This
        /// action does not affect the accuracy of the output, as not all of the input text was required." Applies only
        /// when Arabic input text exceeds a threshold at which additional words do not contribute to the accuracy of
        /// the profile.
        /// </summary>
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
    }
}
