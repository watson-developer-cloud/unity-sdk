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

namespace IBM.Watson.CompareComply.V1.Model
{
    /// <summary>
    /// Basic information about the input document.
    /// </summary>
    public class Document
    {
        /// <summary>
        /// Document title, if detected.
        /// </summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }
        /// <summary>
        /// The input document converted into HTML format.
        /// </summary>
        [JsonProperty("html", NullValueHandling = NullValueHandling.Ignore)]
        public string Html { get; set; }
        /// <summary>
        /// The MD5 hash value of the input document.
        /// </summary>
        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)]
        public string Hash { get; set; }
        /// <summary>
        /// The label applied to the input document with the calling method's `file_1_label` or `file_2_label` value.
        /// This field is specified only in the output of the **Comparing two documents** method.
        /// </summary>
        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }
    }
}
