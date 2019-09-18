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

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// Training status details.
    /// </summary>
    public class TrainingStatus
    {
        /// <summary>
        /// The total number of training examples uploaded to this collection.
        /// </summary>
        [JsonProperty("total_examples", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalExamples { get; set; }
        /// <summary>
        /// When `true`, the collection has been successfully trained.
        /// </summary>
        [JsonProperty("available", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Available { get; set; }
        /// <summary>
        /// When `true`, the collection is currently processing training.
        /// </summary>
        [JsonProperty("processing", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Processing { get; set; }
        /// <summary>
        /// When `true`, the collection has a sufficent amount of queries added for training to occur.
        /// </summary>
        [JsonProperty("minimum_queries_added", NullValueHandling = NullValueHandling.Ignore)]
        public bool? MinimumQueriesAdded { get; set; }
        /// <summary>
        /// When `true`, the collection has a sufficent amount of examples added for training to occur.
        /// </summary>
        [JsonProperty("minimum_examples_added", NullValueHandling = NullValueHandling.Ignore)]
        public bool? MinimumExamplesAdded { get; set; }
        /// <summary>
        /// When `true`, the collection has a sufficent amount of diversity in labeled results for training to occur.
        /// </summary>
        [JsonProperty("sufficient_label_diversity", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SufficientLabelDiversity { get; set; }
        /// <summary>
        /// The number of notices associated with this data set.
        /// </summary>
        [JsonProperty("notices", NullValueHandling = NullValueHandling.Ignore)]
        public long? Notices { get; set; }
        /// <summary>
        /// The timestamp of when the collection was successfully trained.
        /// </summary>
        [JsonProperty("successfully_trained", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? SuccessfullyTrained { get; set; }
        /// <summary>
        /// The timestamp of when the data was uploaded.
        /// </summary>
        [JsonProperty("data_updated", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DataUpdated { get; set; }
    }
}
