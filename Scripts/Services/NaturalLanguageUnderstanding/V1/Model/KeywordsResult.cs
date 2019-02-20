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

namespace IBM.Watson.NaturalLanguageUnderstanding.V1.Model
{
    /// <summary>
    /// The important keywords in the content, organized by relevance.
    /// </summary>
    public class KeywordsResult
    {
        /// <summary>
        /// Number of times the keyword appears in the analyzed text.
        /// </summary>
        [JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
        public long? Count { get; set; }
        /// <summary>
        /// Relevance score from 0 to 1. Higher values indicate greater relevance.
        /// </summary>
        [JsonProperty("relevance", NullValueHandling = NullValueHandling.Ignore)]
        public double? Relevance { get; set; }
        /// <summary>
        /// The keyword text.
        /// </summary>
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
        /// <summary>
        /// Emotion analysis results for the keyword, enabled with the `emotion` option.
        /// </summary>
        [JsonProperty("emotion", NullValueHandling = NullValueHandling.Ignore)]
        public EmotionScores Emotion { get; set; }
        /// <summary>
        /// Sentiment analysis results for the keyword, enabled with the `sentiment` option.
        /// </summary>
        [JsonProperty("sentiment", NullValueHandling = NullValueHandling.Ignore)]
        public FeatureSentimentResults Sentiment { get; set; }
    }
}
