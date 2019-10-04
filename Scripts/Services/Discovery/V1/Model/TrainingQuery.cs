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

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// Training query details.
    /// </summary>
    public class TrainingQuery
    {
        /// <summary>
        /// The query ID associated with the training query.
        /// </summary>
        [JsonProperty("query_id", NullValueHandling = NullValueHandling.Ignore)]
        public string QueryId { get; set; }
        /// <summary>
        /// The natural text query for the training query.
        /// </summary>
        [JsonProperty("natural_language_query", NullValueHandling = NullValueHandling.Ignore)]
        public string NaturalLanguageQuery { get; set; }
        /// <summary>
        /// The filter used on the collection before the **natural_language_query** is applied.
        /// </summary>
        [JsonProperty("filter", NullValueHandling = NullValueHandling.Ignore)]
        public string Filter { get; set; }
        /// <summary>
        /// Array of training examples.
        /// </summary>
        [JsonProperty("examples", NullValueHandling = NullValueHandling.Ignore)]
        public List<TrainingExample> Examples { get; set; }
    }
}
