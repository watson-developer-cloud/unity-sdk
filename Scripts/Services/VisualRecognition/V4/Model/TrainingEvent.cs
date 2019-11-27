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
using System;

namespace IBM.Watson.VisualRecognition.V4.Model
{
    /// <summary>
    /// Details about the training event.
    /// </summary>
    public class TrainingEvent
    {
        /// <summary>
        /// Trained object type. Only `objects` is currently supported.
        /// </summary>
        public class TypeValue
        {
            /// <summary>
            /// Constant OBJECTS for objects
            /// </summary>
            public const string OBJECTS = "objects";
            
        }

        /// <summary>
        /// Training status of the training event.
        /// </summary>
        public class StatusValue
        {
            /// <summary>
            /// Constant FAILED for failed
            /// </summary>
            public const string FAILED = "failed";
            /// <summary>
            /// Constant SUCCEEDED for succeeded
            /// </summary>
            public const string SUCCEEDED = "succeeded";
            
        }

        /// <summary>
        /// Trained object type. Only `objects` is currently supported.
        /// Constants for possible values can be found using TrainingEvent.TypeValue
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
        /// <summary>
        /// Training status of the training event.
        /// Constants for possible values can be found using TrainingEvent.StatusValue
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        /// <summary>
        /// Identifier of the trained collection.
        /// </summary>
        [JsonProperty("collection_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CollectionId { get; set; }
        /// <summary>
        /// Date and time in Coordinated Universal Time (UTC) that training on the collection finished.
        /// </summary>
        [JsonProperty("completion_time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CompletionTime { get; set; }
        /// <summary>
        /// The total number of images that were used in training for this training event.
        /// </summary>
        [JsonProperty("image_count", NullValueHandling = NullValueHandling.Ignore)]
        public long? ImageCount { get; set; }
    }
}
