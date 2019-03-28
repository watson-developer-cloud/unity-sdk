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
    /// Tokenization options.
    /// </summary>
    public class SyntaxOptionsTokens
    {
        /// <summary>
        /// Set this to `true` to return the lemma for each token.
        /// </summary>
        [JsonProperty("lemma", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Lemma { get; set; }
        /// <summary>
        /// Set this to `true` to return the part of speech for each token.
        /// </summary>
        [JsonProperty("part_of_speech", NullValueHandling = NullValueHandling.Ignore)]
        public bool? PartOfSpeech { get; set; }
    }
}
