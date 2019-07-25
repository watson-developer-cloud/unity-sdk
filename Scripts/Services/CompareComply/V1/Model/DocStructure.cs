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

namespace IBM.Watson.CompareComply.V1.Model
{
    /// <summary>
    /// The structure of the input document.
    /// </summary>
    public class DocStructure
    {
        /// <summary>
        /// An array containing one object per section or subsection identified in the input document.
        /// </summary>
        [JsonProperty("section_titles", NullValueHandling = NullValueHandling.Ignore)]
        public List<SectionTitles> SectionTitles { get; set; }
        /// <summary>
        /// An array containing one object per section or subsection, in parallel with the `section_titles` array, that
        /// details the leading sentences in the corresponding section or subsection.
        /// </summary>
        [JsonProperty("leading_sentences", NullValueHandling = NullValueHandling.Ignore)]
        public List<LeadingSentence> LeadingSentences { get; set; }
        /// <summary>
        /// An array containing one object per paragraph, in parallel with the `section_titles` and `leading_sentences`
        /// arrays.
        /// </summary>
        [JsonProperty("paragraphs", NullValueHandling = NullValueHandling.Ignore)]
        public List<Paragraphs> Paragraphs { get; set; }
    }
}
