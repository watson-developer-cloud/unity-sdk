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

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// Document conversion settings.
    /// </summary>
    public class Conversions
    {
        /// <summary>
        /// A list of PDF conversion settings.
        /// </summary>
        [JsonProperty("pdf", NullValueHandling = NullValueHandling.Ignore)]
        public PdfSettings Pdf { get; set; }
        /// <summary>
        /// A list of Word conversion settings.
        /// </summary>
        [JsonProperty("word", NullValueHandling = NullValueHandling.Ignore)]
        public WordSettings Word { get; set; }
        /// <summary>
        /// A list of HTML conversion settings.
        /// </summary>
        [JsonProperty("html", NullValueHandling = NullValueHandling.Ignore)]
        public HtmlSettings Html { get; set; }
        /// <summary>
        /// A list of Document Segmentation settings.
        /// </summary>
        [JsonProperty("segment", NullValueHandling = NullValueHandling.Ignore)]
        public SegmentSettings Segment { get; set; }
        /// <summary>
        /// An array of JSON normalization operations.
        /// </summary>
        [JsonProperty("json_normalizations", NullValueHandling = NullValueHandling.Ignore)]
        public object JsonNormalizations { get; set; }
    }
}
