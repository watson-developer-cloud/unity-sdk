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
    /// An expansion definition. Each object respresents one set of expandable strings. For example, you could have
    /// expansions for the word `hot` in one object, and expansions for the word `cold` in another.
    /// </summary>
    [fsObject]
    public class Expansion
    {
        /// <summary>
        /// A list of terms that will be expanded for this expansion. If specified, only the items in this list are
        /// expanded.
        /// </summary>
        [fsProperty("input_terms")]
        public List<string> InputTerms { get; set; }
        /// <summary>
        /// A list of terms that this expansion will be expanded to. If specified without **input_terms**, it also
        /// functions as the input term list.
        /// </summary>
        [fsProperty("expanded_terms")]
        public List<string> ExpandedTerms { get; set; }
    }

}
