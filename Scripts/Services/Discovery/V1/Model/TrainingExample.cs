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

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// Training example details.
    /// </summary>
    public class TrainingExample
    {
        /// <summary>
        /// The document ID associated with this training example.
        /// </summary>
        [JsonProperty("document_id", NullValueHandling = NullValueHandling.Ignore)]
        public string DocumentId { get; set; }
        /// <summary>
        /// The cross reference associated with this training example.
        /// </summary>
        [JsonProperty("cross_reference", NullValueHandling = NullValueHandling.Ignore)]
        public string CrossReference { get; set; }
        /// <summary>
        /// The relevance of the training example.
        /// </summary>
        [JsonProperty("relevance", NullValueHandling = NullValueHandling.Ignore)]
        public long? Relevance { get; set; }
    }
}
