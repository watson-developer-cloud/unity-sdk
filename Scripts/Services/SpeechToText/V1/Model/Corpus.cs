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
    /// Information about a corpus from a custom language model.
    /// </summary>
    public class Corpus
    {
        /// <summary>
        /// The status of the corpus:
        /// * `analyzed`: The service successfully analyzed the corpus. The custom model can be trained with data from
        /// the corpus.
        /// * `being_processed`: The service is still analyzing the corpus. The service cannot accept requests to add
        /// new resources or to train the custom model.
        /// * `undetermined`: The service encountered an error while processing the corpus. The `error` field describes
        /// the failure.
        /// </summary>
        public class StatusValue
        {
            /// <summary>
            /// Constant ANALYZED for analyzed
            /// </summary>
            public const string ANALYZED = "analyzed";
            /// <summary>
            /// Constant BEING_PROCESSED for being_processed
            /// </summary>
            public const string BEING_PROCESSED = "being_processed";
            /// <summary>
            /// Constant UNDETERMINED for undetermined
            /// </summary>
            public const string UNDETERMINED = "undetermined";
            
        }

        /// <summary>
        /// The status of the corpus:
        /// * `analyzed`: The service successfully analyzed the corpus. The custom model can be trained with data from
        /// the corpus.
        /// * `being_processed`: The service is still analyzing the corpus. The service cannot accept requests to add
        /// new resources or to train the custom model.
        /// * `undetermined`: The service encountered an error while processing the corpus. The `error` field describes
        /// the failure.
        /// Constants for possible values can be found using Corpus.StatusValue
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        /// <summary>
        /// The name of the corpus.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// The total number of words in the corpus. The value is `0` while the corpus is being processed.
        /// </summary>
        [JsonProperty("total_words", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalWords { get; set; }
        /// <summary>
        /// The number of OOV words in the corpus. The value is `0` while the corpus is being processed.
        /// </summary>
        [JsonProperty("out_of_vocabulary_words", NullValueHandling = NullValueHandling.Ignore)]
        public long? OutOfVocabularyWords { get; set; }
        /// <summary>
        /// If the status of the corpus is `undetermined`, the following message: `Analysis of corpus 'name' failed.
        /// Please try adding the corpus again by setting the 'allow_overwrite' flag to 'true'`.
        /// </summary>
        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public string Error { get; set; }
    }
}
