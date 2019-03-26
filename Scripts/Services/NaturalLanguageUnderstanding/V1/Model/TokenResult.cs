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

namespace IBM.Watson.NaturalLanguageUnderstanding.V1.Model
{
    /// <summary>
    /// TokenResult.
    /// </summary>
    public class TokenResult
    {
        /// <summary>
        /// The part of speech of the token. For descriptions of the values, see [Universal Dependencies POS
        /// tags](https://universaldependencies.org/u/pos/).
        /// </summary>
        public class PartOfSpeechValue
        {
            /// <summary>
            /// Constant ADJ for ADJ
            /// </summary>
            public const string ADJ = "ADJ";
            /// <summary>
            /// Constant ADP for ADP
            /// </summary>
            public const string ADP = "ADP";
            /// <summary>
            /// Constant ADV for ADV
            /// </summary>
            public const string ADV = "ADV";
            /// <summary>
            /// Constant AUX for AUX
            /// </summary>
            public const string AUX = "AUX";
            /// <summary>
            /// Constant CCONJ for CCONJ
            /// </summary>
            public const string CCONJ = "CCONJ";
            /// <summary>
            /// Constant DET for DET
            /// </summary>
            public const string DET = "DET";
            /// <summary>
            /// Constant INTJ for INTJ
            /// </summary>
            public const string INTJ = "INTJ";
            /// <summary>
            /// Constant NOUN for NOUN
            /// </summary>
            public const string NOUN = "NOUN";
            /// <summary>
            /// Constant NUM for NUM
            /// </summary>
            public const string NUM = "NUM";
            /// <summary>
            /// Constant PART for PART
            /// </summary>
            public const string PART = "PART";
            /// <summary>
            /// Constant PRON for PRON
            /// </summary>
            public const string PRON = "PRON";
            /// <summary>
            /// Constant PROPN for PROPN
            /// </summary>
            public const string PROPN = "PROPN";
            /// <summary>
            /// Constant PUNCT for PUNCT
            /// </summary>
            public const string PUNCT = "PUNCT";
            /// <summary>
            /// Constant SCONJ for SCONJ
            /// </summary>
            public const string SCONJ = "SCONJ";
            /// <summary>
            /// Constant SYM for SYM
            /// </summary>
            public const string SYM = "SYM";
            /// <summary>
            /// Constant VERB for VERB
            /// </summary>
            public const string VERB = "VERB";
            /// <summary>
            /// Constant X for X
            /// </summary>
            public const string X = "X";
            
        }

        /// <summary>
        /// The part of speech of the token. For descriptions of the values, see [Universal Dependencies POS
        /// tags](https://universaldependencies.org/u/pos/).
        /// Constants for possible values can be found using TokenResult.PartOfSpeechValue
        /// </summary>
        [JsonProperty("part_of_speech", NullValueHandling = NullValueHandling.Ignore)]
        public string PartOfSpeech { get; set; }
        /// <summary>
        /// The token as it appears in the analyzed text.
        /// </summary>
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
        /// <summary>
        /// Character offsets indicating the beginning and end of the token in the analyzed text.
        /// </summary>
        [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
        public List<long?> Location { get; set; }
        /// <summary>
        /// The [lemma](https://wikipedia.org/wiki/Lemma_%28morphology%29) of the token.
        /// </summary>
        [JsonProperty("lemma", NullValueHandling = NullValueHandling.Ignore)]
        public string Lemma { get; set; }
    }
}
