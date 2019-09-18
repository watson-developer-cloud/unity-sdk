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
    /// Microsoft Word styles to convert into a specified HTML head level.
    /// </summary>
    public class WordStyle
    {
        /// <summary>
        /// HTML head level that content matching this style is tagged with.
        /// </summary>
        [JsonProperty("level", NullValueHandling = NullValueHandling.Ignore)]
        public long? Level { get; set; }
        /// <summary>
        /// Array of word style names to convert.
        /// </summary>
        [JsonProperty("names", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Names { get; set; }
    }
}
