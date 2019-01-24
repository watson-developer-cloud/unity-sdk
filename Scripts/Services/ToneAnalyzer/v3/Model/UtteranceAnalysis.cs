/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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
using FullSerializer;
using IBM.Watson.Connection;

namespace IBM.Watson.ToneAnalyzer.v3.Model
{
    /// <summary>
    /// UtteranceAnalysis.
    /// </summary>
    public class UtteranceAnalysis
    {
        /// <summary>
        /// The unique identifier of the utterance. The first utterance has ID 0, and the ID of each subsequent
        /// utterance is incremented by one.
        /// </summary>
        [fsProperty("utterance_id")]
        public long? UtteranceId { get; set; }
        /// <summary>
        /// The text of the utterance.
        /// </summary>
        [fsProperty("utterance_text")]
        public string UtteranceText { get; set; }
        /// <summary>
        /// An array of `ToneChatScore` objects that provides results for the most prevalent tones of the utterance. The
        /// array includes results for any tone whose score is at least 0.5. The array is empty if no tone has a score
        /// that meets this threshold.
        /// </summary>
        [fsProperty("tones")]
        public List<ToneChatScore> Tones { get; set; }
        /// <summary>
        /// **`2017-09-21`:** An error message if the utterance contains more than 500 characters. The service does not
        /// analyze the utterance. **`2016-05-19`:** Not returned.
        /// </summary>
        [fsProperty("error")]
        public string Error { get; set; }
    }


}
