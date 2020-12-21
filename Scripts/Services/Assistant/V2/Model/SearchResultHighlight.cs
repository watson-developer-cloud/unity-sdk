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
using IBM.Cloud.SDK.Model;
using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V2.Model
{
    /// <summary>
    /// An object containing segments of text from search results with query-matching text highlighted using HTML `<em>`
    /// tags.
    /// </summary>
    public class SearchResultHighlight : DynamicModel<List<string>>
    {
        /// <summary>
        /// An array of strings containing segments taken from body text in the search results, with query-matching
        /// substrings highlighted.
        /// </summary>
        [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Body { get; set; }
        /// <summary>
        /// An array of strings containing segments taken from title text in the search results, with query-matching
        /// substrings highlighted.
        /// </summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Title { get; set; }
        /// <summary>
        /// An array of strings containing segments taken from URLs in the search results, with query-matching
        /// substrings highlighted.
        /// </summary>
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Url { get; set; }
    }
}
