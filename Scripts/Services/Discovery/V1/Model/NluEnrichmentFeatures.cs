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
    /// Object containing Natural Language Understanding features to be used.
    /// </summary>
    public class NluEnrichmentFeatures
    {
        /// <summary>
        /// An object specifying the Keyword enrichment and related parameters.
        /// </summary>
        [JsonProperty("keywords", NullValueHandling = NullValueHandling.Ignore)]
        public NluEnrichmentKeywords Keywords { get; set; }
        /// <summary>
        /// An object speficying the Entities enrichment and related parameters.
        /// </summary>
        [JsonProperty("entities", NullValueHandling = NullValueHandling.Ignore)]
        public NluEnrichmentEntities Entities { get; set; }
        /// <summary>
        /// An object specifying the sentiment extraction enrichment and related parameters.
        /// </summary>
        [JsonProperty("sentiment", NullValueHandling = NullValueHandling.Ignore)]
        public NluEnrichmentSentiment Sentiment { get; set; }
        /// <summary>
        /// An object specifying the emotion detection enrichment and related parameters.
        /// </summary>
        [JsonProperty("emotion", NullValueHandling = NullValueHandling.Ignore)]
        public NluEnrichmentEmotion Emotion { get; set; }
        /// <summary>
        /// An object that indicates the Categories enrichment will be applied to the specified field.
        /// </summary>
        [JsonProperty("categories", NullValueHandling = NullValueHandling.Ignore)]
        public NluEnrichmentCategories Categories { get; set; }
        /// <summary>
        /// An object specifiying the semantic roles enrichment and related parameters.
        /// </summary>
        [JsonProperty("semantic_roles", NullValueHandling = NullValueHandling.Ignore)]
        public NluEnrichmentSemanticRoles SemanticRoles { get; set; }
        /// <summary>
        /// An object specifying the relations enrichment and related parameters.
        /// </summary>
        [JsonProperty("relations", NullValueHandling = NullValueHandling.Ignore)]
        public NluEnrichmentRelations Relations { get; set; }
        /// <summary>
        /// An object specifiying the concepts enrichment and related parameters.
        /// </summary>
        [JsonProperty("concepts", NullValueHandling = NullValueHandling.Ignore)]
        public NluEnrichmentConcepts Concepts { get; set; }
    }
}
