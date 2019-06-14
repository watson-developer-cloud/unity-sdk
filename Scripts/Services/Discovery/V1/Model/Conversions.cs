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
        /// Defines operations that can be used to transform the final output JSON into a normalized form. Operations
        /// are executed in the order that they appear in the array.
        /// </summary>
        [JsonProperty("json_normalizations", NullValueHandling = NullValueHandling.Ignore)]
        public List<NormalizationOperation> JsonNormalizations { get; set; }
        /// <summary>
        /// When `true`, automatic text extraction from images (this includes images embedded in supported document
        /// formats, for example PDF, and suppported image formats, for example TIFF) is performed on documents uploaded
        /// to the collection. This field is supported on **Advanced** and higher plans only. **Lite** plans do not
        /// support image text recognition.
        /// </summary>
        [JsonProperty("image_text_recognition", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ImageTextRecognition { get; set; }
    }
}
