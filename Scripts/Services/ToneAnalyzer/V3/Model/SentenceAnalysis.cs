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

namespace IBM.Watson.ToneAnalyzer.V3.Model
{
    /// <summary>
    /// The results of the analysis for the individual sentences of the input content.
    /// </summary>
    public class SentenceAnalysis
    {
        /// <summary>
        /// The unique identifier of a sentence of the input content. The first sentence has ID 0, and the ID of each
        /// subsequent sentence is incremented by one.
        /// </summary>
        [JsonProperty("sentence_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? SentenceId { get; set; }
        /// <summary>
        /// The text of the input sentence.
        /// </summary>
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
        /// <summary>
        /// **`2017-09-21`:** An array of `ToneScore` objects that provides the results of the analysis for each
        /// qualifying tone of the sentence. The array includes results for any tone whose score is at least 0.5. The
        /// array is empty if no tone has a score that meets this threshold. **`2016-05-19`:** Not returned.
        /// </summary>
        [JsonProperty("tones", NullValueHandling = NullValueHandling.Ignore)]
        public List<ToneScore> Tones { get; set; }
        /// <summary>
        /// **`2017-09-21`:** Not returned. **`2016-05-19`:** An array of `ToneCategory` objects that provides the
        /// results of the tone analysis for the sentence. The service returns results only for the tones specified with
        /// the `tones` parameter of the request.
        /// </summary>
        [JsonProperty("tone_categories", NullValueHandling = NullValueHandling.Ignore)]
        public List<ToneCategory> ToneCategories { get; set; }
        /// <summary>
        /// **`2017-09-21`:** Not returned. **`2016-05-19`:** The offset of the first character of the sentence in the
        /// overall input content.
        /// </summary>
        [JsonProperty("input_from", NullValueHandling = NullValueHandling.Ignore)]
        public long? InputFrom { get; set; }
        /// <summary>
        /// **`2017-09-21`:** Not returned. **`2016-05-19`:** The offset of the last character of the sentence in the
        /// overall input content.
        /// </summary>
        [JsonProperty("input_to", NullValueHandling = NullValueHandling.Ignore)]
        public long? InputTo { get; set; }
    }
}
