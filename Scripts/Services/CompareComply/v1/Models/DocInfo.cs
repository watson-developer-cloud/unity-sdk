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
    /// Information about the parsed input document.
    /// </summary>
    [fsObject]
    public class DocInfo
    {
        /// <summary>
        /// The full text of the parsed document in HTML format.
        /// </summary>
        [fsProperty("html")]
        public string Html { get; set; }
        /// <summary>
        /// The title of the parsed document. If the service did not detect a title, the value of this element is
        /// `null`.
        /// </summary>
        [fsProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// The MD5 hash of the input document.
        /// </summary>
        [fsProperty("hash")]
        public string Hash { get; set; }
    }

}
