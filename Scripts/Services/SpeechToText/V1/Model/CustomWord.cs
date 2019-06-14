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
    /// Information about a word that is to be added to a custom language model.
    /// </summary>
    public class CustomWord
    {
        /// <summary>
        /// For the **Add custom words** method, you must specify the custom word that is to be added to or updated in
        /// the custom model. Do not include spaces in the word. Use a `-` (dash) or `_` (underscore) to connect the
        /// tokens of compound words.
        ///
        /// Omit this parameter for the **Add a custom word** method.
        /// </summary>
        [JsonProperty("word", NullValueHandling = NullValueHandling.Ignore)]
        public string Word { get; set; }
        /// <summary>
        /// An array of sounds-like pronunciations for the custom word. Specify how words that are difficult to
        /// pronounce, foreign words, acronyms, and so on can be pronounced by users.
        /// * For a word that is not in the service's base vocabulary, omit the parameter to have the service
        /// automatically generate a sounds-like pronunciation for the word.
        /// * For a word that is in the service's base vocabulary, use the parameter to specify additional
        /// pronunciations for the word. You cannot override the default pronunciation of a word; pronunciations you add
        /// augment the pronunciation from the base vocabulary.
        ///
        /// A word can have at most five sounds-like pronunciations. A pronunciation can include at most 40 characters
        /// not including spaces.
        /// </summary>
        [JsonProperty("sounds_like", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> SoundsLike { get; set; }
        /// <summary>
        /// An alternative spelling for the custom word when it appears in a transcript. Use the parameter when you want
        /// the word to have a spelling that is different from its usual representation or from its spelling in corpora
        /// training data.
        /// </summary>
        [JsonProperty("display_as", NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayAs { get; set; }
    }
}
