/**
* Copyright 2018 IBM Corp. All Rights Reserved.
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

using FullSerializer;
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    /// <summary>
    /// Tokenization dictionary describing how words are tokenized during ingestion and at query time.
    /// </summary>
    [fsObject]
    public class TokenDict
    {
        /// <summary>
        /// An array of tokenization rules. Each rule contains, the original `text` string, component `tokens`, any
        /// alternate character set `readings`, and which `part_of_speech` the text is from.
        /// </summary>
        [fsProperty("tokenization_rules")]
        public List<TokenDictRule> TokenizationRules { get; set; }
    }

}
