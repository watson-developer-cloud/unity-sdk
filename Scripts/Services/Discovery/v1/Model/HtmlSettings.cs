/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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
using FullSerializer;

namespace IBM.Watson.Discovery.v1.Model
{
    /// <summary>
    /// A list of HTML conversion settings.
    /// </summary>
    public class HtmlSettings
    {
        /// <summary>
        /// Gets or Sets ExcludeTagsCompletely
        /// </summary>
        [fsProperty("exclude_tags_completely")]
        public List<string> ExcludeTagsCompletely { get; set; }
        /// <summary>
        /// Gets or Sets ExcludeTagsKeepContent
        /// </summary>
        [fsProperty("exclude_tags_keep_content")]
        public List<string> ExcludeTagsKeepContent { get; set; }
        /// <summary>
        /// Gets or Sets KeepContent
        /// </summary>
        [fsProperty("keep_content")]
        public XPathPatterns KeepContent { get; set; }
        /// <summary>
        /// Gets or Sets ExcludeContent
        /// </summary>
        [fsProperty("exclude_content")]
        public XPathPatterns ExcludeContent { get; set; }
        /// <summary>
        /// Gets or Sets KeepTagAttributes
        /// </summary>
        [fsProperty("keep_tag_attributes")]
        public List<string> KeepTagAttributes { get; set; }
        /// <summary>
        /// Gets or Sets ExcludeTagAttributes
        /// </summary>
        [fsProperty("exclude_tag_attributes")]
        public List<string> ExcludeTagAttributes { get; set; }
    }


}
