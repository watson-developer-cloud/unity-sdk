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
    /// An object speficying the Entities enrichment and related parameters.
    /// </summary>
    public class NluEnrichmentEntities
    {
        /// <summary>
        /// When `true`, sentiment analysis of entities will be performed on the specified field.
        /// </summary>
        [JsonProperty("sentiment", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Sentiment { get; set; }
        /// <summary>
        /// When `true`, emotion detection of entities will be performed on the specified field.
        /// </summary>
        [JsonProperty("emotion", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Emotion { get; set; }
        /// <summary>
        /// The maximum number of entities to extract for each instance of the specified field.
        /// </summary>
        [JsonProperty("limit", NullValueHandling = NullValueHandling.Ignore)]
        public long? Limit { get; set; }
        /// <summary>
        /// When `true`, the number of mentions of each identified entity is recorded. The default is `false`.
        /// </summary>
        [JsonProperty("mentions", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Mentions { get; set; }
        /// <summary>
        /// When `true`, the types of mentions for each idetifieid entity is recorded. The default is `false`.
        /// </summary>
        [JsonProperty("mention_types", NullValueHandling = NullValueHandling.Ignore)]
        public bool? MentionTypes { get; set; }
        /// <summary>
        /// When `true`, a list of sentence locations for each instance of each identified entity is recorded. The
        /// default is `false`.
        /// </summary>
        [JsonProperty("sentence_locations", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SentenceLocations { get; set; }
        /// <summary>
        /// The enrichement model to use with entity extraction. May be a custom model provided by Watson Knowledge
        /// Studio, or the default public model `alchemy`.
        /// </summary>
        [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
        public string Model { get; set; }
    }
}
