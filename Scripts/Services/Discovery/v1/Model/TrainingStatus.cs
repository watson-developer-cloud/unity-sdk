/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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

using FullSerializer;
using System;

namespace IBM.Watson.Discovery.v1.Model
{
    /// <summary>
    /// TrainingStatus.
    /// </summary>
    public class TrainingStatus
    {
        /// <summary>
        /// Gets or Sets TotalExamples
        /// </summary>
        [fsProperty("total_examples")]
        public long? TotalExamples { get; set; }
        /// <summary>
        /// Gets or Sets Available
        /// </summary>
        [fsProperty("available")]
        public bool? Available { get; set; }
        /// <summary>
        /// Gets or Sets Processing
        /// </summary>
        [fsProperty("processing")]
        public bool? Processing { get; set; }
        /// <summary>
        /// Gets or Sets MinimumQueriesAdded
        /// </summary>
        [fsProperty("minimum_queries_added")]
        public bool? MinimumQueriesAdded { get; set; }
        /// <summary>
        /// Gets or Sets MinimumExamplesAdded
        /// </summary>
        [fsProperty("minimum_examples_added")]
        public bool? MinimumExamplesAdded { get; set; }
        /// <summary>
        /// Gets or Sets SufficientLabelDiversity
        /// </summary>
        [fsProperty("sufficient_label_diversity")]
        public bool? SufficientLabelDiversity { get; set; }
        /// <summary>
        /// Gets or Sets Notices
        /// </summary>
        [fsProperty("notices")]
        public long? Notices { get; set; }
        /// <summary>
        /// Gets or Sets SuccessfullyTrained
        /// </summary>
        [fsProperty("successfully_trained")]
        public DateTime? SuccessfullyTrained { get; set; }
        /// <summary>
        /// Gets or Sets DataUpdated
        /// </summary>
        [fsProperty("data_updated")]
        public DateTime? DataUpdated { get; set; }
    }


}
