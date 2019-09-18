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

using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V2.Model
{
    /// <summary>
    /// SearchResult.
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// The unique identifier of the document in the Discovery service collection.
        ///
        /// This property is included in responses from search skills, which are a beta feature available only to Plus
        /// or Premium plan users.
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
        /// <summary>
        /// An object containing search result metadata from the Discovery service.
        /// </summary>
        [JsonProperty("result_metadata", NullValueHandling = NullValueHandling.Ignore)]
        public SearchResultMetadata ResultMetadata { get; set; }
        /// <summary>
        /// A description of the search result. This is taken from an abstract, summary, or highlight field in the
        /// Discovery service response, as specified in the search skill configuration.
        /// </summary>
        [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
        public string Body { get; set; }
        /// <summary>
        /// The title of the search result. This is taken from a title or name field in the Discovery service response,
        /// as specified in the search skill configuration.
        /// </summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }
        /// <summary>
        /// The URL of the original data object in its native data source.
        /// </summary>
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }
        /// <summary>
        /// An object containing segments of text from search results with query-matching text highlighted using HTML
        /// <em> tags.
        /// </summary>
        [JsonProperty("highlight", NullValueHandling = NullValueHandling.Ignore)]
        public SearchResultHighlight Highlight { get; set; }
    }
}
