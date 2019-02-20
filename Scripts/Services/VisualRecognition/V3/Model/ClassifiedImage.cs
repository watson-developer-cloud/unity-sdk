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

namespace IBM.Watson.VisualRecognition.V3.Model
{
    /// <summary>
    /// Results for one image.
    /// </summary>
    public class ClassifiedImage
    {
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
        /// <summary>
        /// Relative path of the image file if uploaded directly. Not returned when the image is passed by URL.
        /// </summary>
        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; }
        /// <summary>
        /// Information about what might have caused a failure, such as an image that is too large. Not returned when
        /// there is no error.
        /// </summary>
        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public ErrorInfo Error { get; set; }
        /// <summary>
        /// The classifiers.
        /// </summary>
        [JsonProperty("classifiers", NullValueHandling = NullValueHandling.Ignore)]
        public List<ClassifierResult> Classifiers { get; set; }
    }
}
