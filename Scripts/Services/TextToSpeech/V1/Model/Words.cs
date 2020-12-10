/**
* (C) Copyright IBM Corp. 2019, 2020.
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

namespace IBM.Watson.TextToSpeech.V1.Model
{
    /// <summary>
    /// For the **Add custom words** method, one or more words that are to be added or updated for the custom model and
    /// the translation for each specified word.
    ///
    /// For the **List custom words** method, the words and their translations from the custom model.
    /// </summary>
    public class Words
    {
        /// <summary>
        /// The **Add custom words** method accepts an array of `Word` objects. Each object provides a word that is to
        /// be added or updated for the custom model and the word's translation.
        ///
        /// The **List custom words** method returns an array of `Word` objects. Each object shows a word and its
        /// translation from the custom model. The words are listed in alphabetical order, with uppercase letters listed
        /// before lowercase letters. The array is empty if the custom model contains no words.
        /// </summary>
        [JsonProperty("words", NullValueHandling = NullValueHandling.Ignore)]
        public List<Word> _Words { get; set; }
    }
}
