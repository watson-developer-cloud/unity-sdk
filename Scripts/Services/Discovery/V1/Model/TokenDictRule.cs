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

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// An object defining a single tokenizaion rule.
    /// </summary>
    public class TokenDictRule
    {
        /// <summary>
        /// The string to tokenize.
        /// </summary>
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
        /// <summary>
        /// Array of tokens that the `text` field is split into when found.
        /// </summary>
        [JsonProperty("tokens", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Tokens { get; set; }
        /// <summary>
        /// Array of tokens that represent the content of the `text` field in an alternate character set.
        /// </summary>
        [JsonProperty("readings", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Readings { get; set; }
        /// <summary>
        /// The part of speech that the `text` string belongs to. For example `noun`. Custom parts of speech can be
        /// specified.
        /// </summary>
        [JsonProperty("part_of_speech", NullValueHandling = NullValueHandling.Ignore)]
        public string PartOfSpeech { get; set; }
    }
}
