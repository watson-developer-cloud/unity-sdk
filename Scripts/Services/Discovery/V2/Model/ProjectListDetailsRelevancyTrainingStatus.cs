/**
* (C) Copyright IBM Corp. 2020.
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

namespace IBM.Watson.Discovery.V2.Model
{
    /// <summary>
    /// Relevancy training status information for this project.
    /// </summary>
    public class ProjectListDetailsRelevancyTrainingStatus
    {
        /// <summary>
        /// When the training data was updated.
        /// </summary>
        [JsonProperty("data_updated", NullValueHandling = NullValueHandling.Ignore)]
        public string DataUpdated { get; set; }
        /// <summary>
        /// The total number of examples.
        /// </summary>
        [JsonProperty("total_examples", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalExamples { get; set; }
        /// <summary>
        /// When `true`, sufficent label diversity is present to allow training for this project.
        /// </summary>
        [JsonProperty("sufficient_label_diversity", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SufficientLabelDiversity { get; set; }
        /// <summary>
        /// When `true`, the relevancy training is in processing.
        /// </summary>
        [JsonProperty("processing", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Processing { get; set; }
        /// <summary>
        /// When `true`, the minimum number of examples required to train has been met.
        /// </summary>
        [JsonProperty("minimum_examples_added", NullValueHandling = NullValueHandling.Ignore)]
        public bool? MinimumExamplesAdded { get; set; }
        /// <summary>
        /// The time that the most recent successful training occured.
        /// </summary>
        [JsonProperty("successfully_trained", NullValueHandling = NullValueHandling.Ignore)]
        public string SuccessfullyTrained { get; set; }
        /// <summary>
        /// When `true`, relevancy training is available when querying collections in the project.
        /// </summary>
        [JsonProperty("available", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Available { get; set; }
        /// <summary>
        /// The number of notices generated during the relevancy training.
        /// </summary>
        [JsonProperty("notices", NullValueHandling = NullValueHandling.Ignore)]
        public long? Notices { get; set; }
        /// <summary>
        /// When `true`, the minimum number of queries required to train has been met.
        /// </summary>
        [JsonProperty("minimum_queries_added", NullValueHandling = NullValueHandling.Ignore)]
        public bool? MinimumQueriesAdded { get; set; }
    }
}
