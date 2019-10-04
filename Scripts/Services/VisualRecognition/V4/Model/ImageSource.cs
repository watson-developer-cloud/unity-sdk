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

namespace IBM.Watson.VisualRecognition.V4.Model
{
    /// <summary>
    /// The source type of the image.
    /// </summary>
    public class ImageSource
    {
        /// <summary>
        /// The source type of the image.
        /// </summary>
        public class TypeValue
        {
            /// <summary>
            /// Constant FILE for file
            /// </summary>
            public const string FILE = "file";
            /// <summary>
            /// Constant URL for url
            /// </summary>
            public const string URL = "url";
            
        }

        /// <summary>
        /// The source type of the image.
        /// Constants for possible values can be found using ImageSource.TypeValue
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
        /// <summary>
        /// Name of the image file if uploaded. Not returned when the image is passed by URL.
        /// </summary>
        [JsonProperty("filename", NullValueHandling = NullValueHandling.Ignore)]
        public string Filename { get; set; }
        /// <summary>
        /// Name of the .zip file of images if uploaded. Not returned when the image is passed directly or by URL.
        /// </summary>
        [JsonProperty("archive_filename", NullValueHandling = NullValueHandling.Ignore)]
        public string ArchiveFilename { get; set; }
        /// <summary>
        /// Source of the image before any redirects. Not returned when the image is uploaded.
        /// </summary>
        [JsonProperty("source_url", NullValueHandling = NullValueHandling.Ignore)]
        public string SourceUrl { get; set; }
        /// <summary>
        /// Fully resolved URL of the image after redirects are followed. Not returned when the image is uploaded.
        /// </summary>
        [JsonProperty("resolved_url", NullValueHandling = NullValueHandling.Ignore)]
        public string ResolvedUrl { get; set; }
    }
}
