/**
* (C) Copyright IBM Corp. 2018, 2020.
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
    /// Component results for a speech recognition request.
    /// </summary>
    public class SpeechRecognitionResult
    {
        /// <summary>
        /// If the `split_transcript_at_phrase_end` parameter is `true`, describes the reason for the split:
        /// * `end_of_data` - The end of the input audio stream.
        /// * `full_stop` - A full semantic stop, such as for the conclusion of a grammatical sentence. The insertion of
        /// splits is influenced by the base language model and biased by custom language models and grammars.
        /// * `reset` - The amount of audio that is currently being processed exceeds the two-minute maximum. The
        /// service splits the transcript to avoid excessive memory use.
        /// * `silence` - A pause or silence that is at least as long as the pause interval.
        /// </summary>
        public class EndOfUtteranceValue
        {
            /// <summary>
            /// Constant END_OF_DATA for end_of_data
            /// </summary>
            public const string END_OF_DATA = "end_of_data";
            /// <summary>
            /// Constant FULL_STOP for full_stop
            /// </summary>
            public const string FULL_STOP = "full_stop";
            /// <summary>
            /// Constant RESET for reset
            /// </summary>
            public const string RESET = "reset";
            /// <summary>
            /// Constant SILENCE for silence
            /// </summary>
            public const string SILENCE = "silence";
            
        }

        /// <summary>
        /// If the `split_transcript_at_phrase_end` parameter is `true`, describes the reason for the split:
        /// * `end_of_data` - The end of the input audio stream.
        /// * `full_stop` - A full semantic stop, such as for the conclusion of a grammatical sentence. The insertion of
        /// splits is influenced by the base language model and biased by custom language models and grammars.
        /// * `reset` - The amount of audio that is currently being processed exceeds the two-minute maximum. The
        /// service splits the transcript to avoid excessive memory use.
        /// * `silence` - A pause or silence that is at least as long as the pause interval.
        /// Constants for possible values can be found using SpeechRecognitionResult.EndOfUtteranceValue
        /// </summary>
        [JsonProperty("end_of_utterance", NullValueHandling = NullValueHandling.Ignore)]
        public string EndOfUtterance { get; set; }
        /// <summary>
        /// An indication of whether the transcription results are final. If `true`, the results for this utterance are
        /// not updated further; no additional results are sent for a `result_index` once its results are indicated as
        /// final.
        /// </summary>
        [JsonProperty("final", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Final { get; set; }
        /// <summary>
        /// An array of alternative transcripts. The `alternatives` array can include additional requested output such
        /// as word confidence or timestamps.
        /// </summary>
        [JsonProperty("alternatives", NullValueHandling = NullValueHandling.Ignore)]
        public List<SpeechRecognitionAlternative> Alternatives { get; set; }
        /// <summary>
        /// A dictionary (or associative array) whose keys are the strings specified for `keywords` if both that
        /// parameter and `keywords_threshold` are specified. The value for each key is an array of matches spotted in
        /// the audio for that keyword. Each match is described by a `KeywordResult` object. A keyword for which no
        /// matches are found is omitted from the dictionary. The dictionary is omitted entirely if no matches are found
        /// for any keywords.
        /// </summary>
        [JsonProperty("keywords_result", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, List<KeywordResult>> KeywordsResult { get; set; }
        /// <summary>
        /// An array of alternative hypotheses found for words of the input audio if a `word_alternatives_threshold` is
        /// specified.
        /// </summary>
        [JsonProperty("word_alternatives", NullValueHandling = NullValueHandling.Ignore)]
        public List<WordAlternativeResults> WordAlternatives { get; set; }
    }
}
