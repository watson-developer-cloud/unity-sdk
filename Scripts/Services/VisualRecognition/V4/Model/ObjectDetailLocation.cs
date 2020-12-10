/**
* (C) Copyright IBM Corp. 2019, 2020.
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
    /// Defines the location of the bounding box around the object.
    /// </summary>
    public class ObjectDetailLocation
    {
        /// <summary>
        /// Y-position of top-left pixel of the bounding box.
        /// </summary>
        [JsonProperty("top", NullValueHandling = NullValueHandling.Ignore)]
        public long? Top { get; set; }
        /// <summary>
        /// X-position of top-left pixel of the bounding box.
        /// </summary>
        [JsonProperty("left", NullValueHandling = NullValueHandling.Ignore)]
        public long? Left { get; set; }
        /// <summary>
        /// Width in pixels of of the bounding box.
        /// </summary>
        [JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
        public long? Width { get; set; }
        /// <summary>
        /// Height in pixels of the bounding box.
        /// </summary>
        [JsonProperty("height", NullValueHandling = NullValueHandling.Ignore)]
        public long? Height { get; set; }
    }
}
