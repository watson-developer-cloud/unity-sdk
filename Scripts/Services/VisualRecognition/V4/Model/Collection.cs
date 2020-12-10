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
using System;

namespace IBM.Watson.VisualRecognition.V4.Model
{
    /// <summary>
    /// Details about a collection.
    /// </summary>
    public class Collection
    {
        /// <summary>
        /// The identifier of the collection.
        /// </summary>
        [JsonProperty("collection_id", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string CollectionId { get; private set; }
        /// <summary>
        /// The name of the collection.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// The description of the collection.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        /// <summary>
        /// Date and time in Coordinated Universal Time (UTC) that the collection was created.
        /// </summary>
        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public virtual DateTime? Created { get; private set; }
        /// <summary>
        /// Date and time in Coordinated Universal Time (UTC) that the collection was most recently updated.
        /// </summary>
        [JsonProperty("updated", NullValueHandling = NullValueHandling.Ignore)]
        public virtual DateTime? Updated { get; private set; }
        /// <summary>
        /// Number of images in the collection.
        /// </summary>
        [JsonProperty("image_count", NullValueHandling = NullValueHandling.Ignore)]
        public virtual long? ImageCount { get; private set; }
        /// <summary>
        /// Training status information for the collection.
        /// </summary>
        [JsonProperty("training_status", NullValueHandling = NullValueHandling.Ignore)]
        public CollectionTrainingStatus TrainingStatus { get; set; }
    }
}
