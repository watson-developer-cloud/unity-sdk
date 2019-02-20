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
    /// An expansion definition. Each object respresents one set of expandable strings. For example, you could have
    /// expansions for the word `hot` in one object, and expansions for the word `cold` in another.
    /// </summary>
    public class Expansion
    {
        /// <summary>
        /// A list of terms that will be expanded for this expansion. If specified, only the items in this list are
        /// expanded.
        /// </summary>
        [JsonProperty("input_terms", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> InputTerms { get; set; }
        /// <summary>
        /// A list of terms that this expansion will be expanded to. If specified without **input_terms**, it also
        /// functions as the input term list.
        /// </summary>
        [JsonProperty("expanded_terms", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ExpandedTerms { get; set; }
    }
}
