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
    /// A list of HTML conversion settings.
    /// </summary>
    public class HtmlSettings
    {
        /// <summary>
        /// Array of HTML tags that are excluded completely.
        /// </summary>
        [JsonProperty("exclude_tags_completely", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ExcludeTagsCompletely { get; set; }
        /// <summary>
        /// Array of HTML tags which are excluded but still retain content.
        /// </summary>
        [JsonProperty("exclude_tags_keep_content", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ExcludeTagsKeepContent { get; set; }
        /// <summary>
        /// Object containing an array of XPaths.
        /// </summary>
        [JsonProperty("keep_content", NullValueHandling = NullValueHandling.Ignore)]
        public XPathPatterns KeepContent { get; set; }
        /// <summary>
        /// Object containing an array of XPaths.
        /// </summary>
        [JsonProperty("exclude_content", NullValueHandling = NullValueHandling.Ignore)]
        public XPathPatterns ExcludeContent { get; set; }
        /// <summary>
        /// An array of HTML tag attributes to keep in the converted document.
        /// </summary>
        [JsonProperty("keep_tag_attributes", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> KeepTagAttributes { get; set; }
        /// <summary>
        /// Array of HTML tag attributes to exclude.
        /// </summary>
        [JsonProperty("exclude_tag_attributes", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ExcludeTagAttributes { get; set; }
    }
}
