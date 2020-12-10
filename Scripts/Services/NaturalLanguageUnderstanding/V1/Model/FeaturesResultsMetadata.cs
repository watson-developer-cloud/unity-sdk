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

namespace IBM.Watson.NaturalLanguageUnderstanding.V1.Model
{
    /// <summary>
    /// Webpage metadata, such as the author and the title of the page.
    /// </summary>
    public class FeaturesResultsMetadata
    {
        /// <summary>
        /// The authors of the document.
        /// </summary>
        [JsonProperty("authors", NullValueHandling = NullValueHandling.Ignore)]
        public List<Author> Authors { get; set; }
        /// <summary>
        /// The publication date in the format ISO 8601.
        /// </summary>
        [JsonProperty("publication_date", NullValueHandling = NullValueHandling.Ignore)]
        public string PublicationDate { get; set; }
        /// <summary>
        /// The title of the document.
        /// </summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }
        /// <summary>
        /// URL of a prominent image on the webpage.
        /// </summary>
        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; }
        /// <summary>
        /// RSS/ATOM feeds found on the webpage.
        /// </summary>
        [JsonProperty("feeds", NullValueHandling = NullValueHandling.Ignore)]
        public List<Feed> Feeds { get; set; }
    }
}
