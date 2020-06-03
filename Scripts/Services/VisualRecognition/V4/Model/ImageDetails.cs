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

using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace IBM.Watson.VisualRecognition.V4.Model
{
    /// <summary>
    /// Details about an image.
    /// </summary>
    public class ImageDetails
    {
        /// <summary>
        /// The identifier of the image.
        /// </summary>
        [JsonProperty("image_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ImageId { get; set; }
        /// <summary>
        /// Date and time in Coordinated Universal Time (UTC) that the image was most recently updated.
        /// </summary>
        [JsonProperty("updated", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Updated { get; set; }
        /// <summary>
        /// Date and time in Coordinated Universal Time (UTC) that the image was created.
        /// </summary>
        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Created { get; set; }
        /// <summary>
        /// The source type of the image.
        /// </summary>
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public ImageSource Source { get; set; }
        /// <summary>
        /// Height and width of an image.
        /// </summary>
        [JsonProperty("dimensions", NullValueHandling = NullValueHandling.Ignore)]
        public ImageDimensions Dimensions { get; set; }
        /// <summary>
        /// Details about the errors.
        /// </summary>
        [JsonProperty("errors", NullValueHandling = NullValueHandling.Ignore)]
        public List<Error> Errors { get; set; }
        /// <summary>
        /// Training data for all objects.
        /// </summary>
        [JsonProperty("training_data", NullValueHandling = NullValueHandling.Ignore)]
        public TrainingDataObjects TrainingData { get; set; }
    }
}
