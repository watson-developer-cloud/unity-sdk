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
    /// Training status for the objects in the collection.
    /// </summary>
    public class ObjectTrainingStatus
    {
        /// <summary>
        /// Whether you can analyze images in the collection with the **objects** feature.
        /// </summary>
        [JsonProperty("ready", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Ready { get; set; }
        /// <summary>
        /// Whether training is in progress.
        /// </summary>
        [JsonProperty("in_progress", NullValueHandling = NullValueHandling.Ignore)]
        public bool? InProgress { get; set; }
        /// <summary>
        /// Whether there are changes to the training data since the most recent training.
        /// </summary>
        [JsonProperty("data_changed", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DataChanged { get; set; }
        /// <summary>
        /// Whether the most recent training failed.
        /// </summary>
        [JsonProperty("latest_failed", NullValueHandling = NullValueHandling.Ignore)]
        public bool? LatestFailed { get; set; }
        /// <summary>
        /// Whether the model can be downloaded after the training status is `ready`.
        /// </summary>
        [JsonProperty("rscnn_ready", NullValueHandling = NullValueHandling.Ignore)]
        public bool? RscnnReady { get; set; }
        /// <summary>
        /// Details about the training. If training is in progress, includes information about the status. If training
        /// is not in progress, includes a success message or information about why training failed.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
    }
}
