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

using System.Collections.Generic;
using FullSerializer;

namespace IBM.Watson.Discovery.v1.Model
{
    /// <summary>
    /// TrainingDataSet.
    /// </summary>
    public class TrainingDataSet
    {
        /// <summary>
        /// Gets or Sets EnvironmentId
        /// </summary>
        [fsProperty("environment_id")]
        public string EnvironmentId { get; set; }
        /// <summary>
        /// Gets or Sets CollectionId
        /// </summary>
        [fsProperty("collection_id")]
        public string CollectionId { get; set; }
        /// <summary>
        /// Gets or Sets Queries
        /// </summary>
        [fsProperty("queries")]
        public List<TrainingQuery> Queries { get; set; }
    }


}
