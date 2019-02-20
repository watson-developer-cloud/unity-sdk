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
    /// TrainingStatus.
    /// </summary>
    public class TrainingStatus
    {
        /// <summary>
        /// Gets or Sets TotalExamples
        /// </summary>
        [JsonProperty("total_examples", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalExamples { get; set; }
        /// <summary>
        /// Gets or Sets Available
        /// </summary>
        [JsonProperty("available", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Available { get; set; }
        /// <summary>
        /// Gets or Sets Processing
        /// </summary>
        [JsonProperty("processing", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Processing { get; set; }
        /// <summary>
        /// Gets or Sets MinimumQueriesAdded
        /// </summary>
        [JsonProperty("minimum_queries_added", NullValueHandling = NullValueHandling.Ignore)]
        public bool? MinimumQueriesAdded { get; set; }
        /// <summary>
        /// Gets or Sets MinimumExamplesAdded
        /// </summary>
        [JsonProperty("minimum_examples_added", NullValueHandling = NullValueHandling.Ignore)]
        public bool? MinimumExamplesAdded { get; set; }
        /// <summary>
        /// Gets or Sets SufficientLabelDiversity
        /// </summary>
        [JsonProperty("sufficient_label_diversity", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SufficientLabelDiversity { get; set; }
        /// <summary>
        /// Gets or Sets Notices
        /// </summary>
        [JsonProperty("notices", NullValueHandling = NullValueHandling.Ignore)]
        public long? Notices { get; set; }
        /// <summary>
        /// Gets or Sets SuccessfullyTrained
        /// </summary>
        [JsonProperty("successfully_trained", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? SuccessfullyTrained { get; set; }
        /// <summary>
        /// Gets or Sets DataUpdated
        /// </summary>
        [JsonProperty("data_updated", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DataUpdated { get; set; }
    }
}
