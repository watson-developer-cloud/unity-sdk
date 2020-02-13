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

using Newtonsoft.Json;

namespace IBM.Watson.TextToSpeech.V1.Model
{
    /// <summary>
    /// Information about a word for the custom voice model.
    /// </summary>
    public class Word
    {
        /// <summary>
        /// **Japanese only.** The part of speech for the word. The service uses the value to produce the correct
        /// intonation for the word. You can create only a single entry, with or without a single part of speech, for
        /// any word; you cannot create multiple entries with different parts of speech for the same word. For more
        /// information, see [Working with Japanese
        /// entries](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-rules#jaNotes).
        /// </summary>
        public class PartOfSpeechValue
        {
            /// <summary>
            /// Constant DOSI for Dosi
            /// </summary>
            public const string DOSI = "Dosi";
            /// <summary>
            /// Constant FUKU for Fuku
            /// </summary>
            public const string FUKU = "Fuku";
            /// <summary>
            /// Constant GOBI for Gobi
            /// </summary>
            public const string GOBI = "Gobi";
            /// <summary>
            /// Constant HOKA for Hoka
            /// </summary>
            public const string HOKA = "Hoka";
            /// <summary>
            /// Constant JODO for Jodo
            /// </summary>
            public const string JODO = "Jodo";
            /// <summary>
            /// Constant JOSI for Josi
            /// </summary>
            public const string JOSI = "Josi";
            /// <summary>
            /// Constant KATO for Kato
            /// </summary>
            public const string KATO = "Kato";
            /// <summary>
            /// Constant KEDO for Kedo
            /// </summary>
            public const string KEDO = "Kedo";
            /// <summary>
            /// Constant KEYO for Keyo
            /// </summary>
            public const string KEYO = "Keyo";
            /// <summary>
            /// Constant KIGO for Kigo
            /// </summary>
            public const string KIGO = "Kigo";
            /// <summary>
            /// Constant KOYU for Koyu
            /// </summary>
            public const string KOYU = "Koyu";
            /// <summary>
            /// Constant MESI for Mesi
            /// </summary>
            public const string MESI = "Mesi";
            /// <summary>
            /// Constant RETA for Reta
            /// </summary>
            public const string RETA = "Reta";
            /// <summary>
            /// Constant STBI for Stbi
            /// </summary>
            public const string STBI = "Stbi";
            /// <summary>
            /// Constant STTO for Stto
            /// </summary>
            public const string STTO = "Stto";
            /// <summary>
            /// Constant STZO for Stzo
            /// </summary>
            public const string STZO = "Stzo";
            /// <summary>
            /// Constant SUJI for Suji
            /// </summary>
            public const string SUJI = "Suji";
            
        }

        /// <summary>
        /// **Japanese only.** The part of speech for the word. The service uses the value to produce the correct
        /// intonation for the word. You can create only a single entry, with or without a single part of speech, for
        /// any word; you cannot create multiple entries with different parts of speech for the same word. For more
        /// information, see [Working with Japanese
        /// entries](https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-rules#jaNotes).
        /// Constants for possible values can be found using Word.PartOfSpeechValue
        /// </summary>
        [JsonProperty("part_of_speech", NullValueHandling = NullValueHandling.Ignore)]
        public string PartOfSpeech { get; set; }
        /// <summary>
        /// The word for the custom voice model.
        /// </summary>
        [JsonProperty("word", NullValueHandling = NullValueHandling.Ignore)]
        public string _Word { get; set; }
        /// <summary>
        /// The phonetic or sounds-like translation for the word. A phonetic translation is based on the SSML format for
        /// representing the phonetic string of a word either as an IPA or IBM SPR translation. A sounds-like
        /// translation consists of one or more words that, when combined, sound like the word.
        /// </summary>
        [JsonProperty("translation", NullValueHandling = NullValueHandling.Ignore)]
        public string Translation { get; set; }
    }
}
