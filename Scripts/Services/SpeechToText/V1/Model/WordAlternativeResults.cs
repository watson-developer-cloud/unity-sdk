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

namespace IBM.Watson.SpeechToText.V1.Model
{
    /// <summary>
    /// Information about alternative hypotheses for words from speech recognition results.
    /// </summary>
    public class WordAlternativeResults
    {
        /// <summary>
        /// The start time in seconds of the word from the input audio that corresponds to the word alternatives.
        /// </summary>
        [JsonProperty("start_time", NullValueHandling = NullValueHandling.Ignore)]
        public double? StartTime { get; set; }
        /// <summary>
        /// The end time in seconds of the word from the input audio that corresponds to the word alternatives.
        /// </summary>
        [JsonProperty("end_time", NullValueHandling = NullValueHandling.Ignore)]
        public double? EndTime { get; set; }
        /// <summary>
        /// An array of alternative hypotheses for a word from the input audio.
        /// </summary>
        [JsonProperty("alternatives", NullValueHandling = NullValueHandling.Ignore)]
        public List<WordAlternativeResult> Alternatives { get; set; }
    }
}
