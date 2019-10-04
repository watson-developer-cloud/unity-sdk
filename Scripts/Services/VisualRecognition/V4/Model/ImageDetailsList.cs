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

namespace IBM.Watson.VisualRecognition.V4.Model
{
    /// <summary>
    /// List of information about the images.
    /// </summary>
    public class ImageDetailsList
    {
        /// <summary>
        /// The images in the collection.
        /// </summary>
        [JsonProperty("images", NullValueHandling = NullValueHandling.Ignore)]
        public List<ImageDetails> Images { get; set; }
        /// <summary>
        /// Information about what might cause less than optimal output.
        /// </summary>
        [JsonProperty("warnings", NullValueHandling = NullValueHandling.Ignore)]
        public List<Warning> Warnings { get; set; }
        /// <summary>
        /// A unique identifier of the request. Included only when an error or warning is returned.
        /// </summary>
        [JsonProperty("trace", NullValueHandling = NullValueHandling.Ignore)]
        public string Trace { get; set; }
    }
}
