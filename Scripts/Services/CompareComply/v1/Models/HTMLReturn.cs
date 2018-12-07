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

namespace  IBM.Watson.DeveloperCloud.Services.CompareComply.v1
{
    /// <summary>
    /// The HTML converted from an input document.
    /// </summary>
    [fsObject]
    public class HTMLReturn
    {
        /// <summary>
        /// The number of pages in the input document.
        /// </summary>
        [fsProperty("num_pages")]
        public string NumPages { get; set; }
        /// <summary>
        /// The author of the input document, if identified.
        /// </summary>
        [fsProperty("author")]
        public string Author { get; set; }
        /// <summary>
        /// The publication date of the input document, if identified.
        /// </summary>
        [fsProperty("publication_date")]
        public string PublicationDate { get; set; }
        /// <summary>
        /// The title of the input document, if identified.
        /// </summary>
        [fsProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// The HTML version of the input document.
        /// </summary>
        [fsProperty("html")]
        public string Html { get; set; }
    }

}
