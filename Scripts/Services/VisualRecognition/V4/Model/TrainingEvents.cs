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
using System;

namespace IBM.Watson.VisualRecognition.V4.Model
{
    /// <summary>
    /// Details about the training events.
    /// </summary>
    public class TrainingEvents
    {
        /// <summary>
        /// The starting day for the returned training events in Coordinated Universal Time (UTC). If not specified in
        /// the request, it identifies the earliest training event.
        /// </summary>
        [JsonProperty("start_time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// The ending day for the returned training events in Coordinated Universal Time (UTC). If not specified in the
        /// request, it lists the current time.
        /// </summary>
        [JsonProperty("end_time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// The total number of training events in the response for the start and end times.
        /// </summary>
        [JsonProperty("completed_events", NullValueHandling = NullValueHandling.Ignore)]
        public long? CompletedEvents { get; set; }
        /// <summary>
        /// The total number of images that were used in training for the start and end times.
        /// </summary>
        [JsonProperty("trained_images", NullValueHandling = NullValueHandling.Ignore)]
        public long? TrainedImages { get; set; }
        /// <summary>
        /// The completed training events for the start and end time.
        /// </summary>
        [JsonProperty("events", NullValueHandling = NullValueHandling.Ignore)]
        public List<TrainingEvent> Events { get; set; }
    }
}
