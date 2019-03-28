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

namespace IBM.Watson.VisualRecognition.V3.Model
{
    /// <summary>
    /// The location of the bounding box around the face.
    /// </summary>
    public class FaceLocation
    {
        /// <summary>
        /// Width in pixels of face region.
        /// </summary>
        [JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
        public float? Width { get; set; }
        /// <summary>
        /// Height in pixels of face region.
        /// </summary>
        [JsonProperty("height", NullValueHandling = NullValueHandling.Ignore)]
        public float? Height { get; set; }
        /// <summary>
        /// X-position of top-left pixel of face region.
        /// </summary>
        [JsonProperty("left", NullValueHandling = NullValueHandling.Ignore)]
        public float? Left { get; set; }
        /// <summary>
        /// Y-position of top-left pixel of face region.
        /// </summary>
        [JsonProperty("top", NullValueHandling = NullValueHandling.Ignore)]
        public float? Top { get; set; }
    }
}
