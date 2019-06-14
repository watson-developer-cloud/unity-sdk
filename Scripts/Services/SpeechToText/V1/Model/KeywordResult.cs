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

namespace IBM.Watson.SpeechToText.V1.Model
{
    /// <summary>
    /// Information about a match for a keyword from speech recognition results.
    /// </summary>
    public class KeywordResult
    {
        /// <summary>
        /// A specified keyword normalized to the spoken phrase that matched in the audio input.
        /// </summary>
        [JsonProperty("normalized_text", NullValueHandling = NullValueHandling.Ignore)]
        public string NormalizedText { get; set; }
        /// <summary>
        /// The start time in seconds of the keyword match.
        /// </summary>
        [JsonProperty("start_time", NullValueHandling = NullValueHandling.Ignore)]
        public double? StartTime { get; set; }
        /// <summary>
        /// The end time in seconds of the keyword match.
        /// </summary>
        [JsonProperty("end_time", NullValueHandling = NullValueHandling.Ignore)]
        public double? EndTime { get; set; }
        /// <summary>
        /// A confidence score for the keyword match in the range of 0.0 to 1.0.
        /// </summary>
        [JsonProperty("confidence", NullValueHandling = NullValueHandling.Ignore)]
        public double? Confidence { get; set; }
    }
}
