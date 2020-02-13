/**
* (C) Copyright IBM Corp. 2018, 2020.
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
    /// Basic information about an updated object.
    /// </summary>
    public class UpdateObjectMetadata
    {
        /// <summary>
        /// The updated name of the object. The name can contain alphanumeric, underscore, hyphen, space, and dot
        /// characters. It cannot begin with the reserved prefix `sys-`.
        /// </summary>
        [JsonProperty("object", NullValueHandling = NullValueHandling.Ignore)]
        public string _Object { get; set; }
        /// <summary>
        /// Number of bounding boxes in the collection with the updated object name.
        /// </summary>
        [JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
        public virtual long? Count { get; private set; }
    }
}
