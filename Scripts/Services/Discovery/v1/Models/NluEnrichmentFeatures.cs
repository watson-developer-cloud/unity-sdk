/**
* Copyright 2018 IBM Corp. All Rights Reserved.
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
namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    /// <summary>
    /// NluEnrichmentFeatures.
    /// </summary>
    public class NluEnrichmentFeatures
    {
        /// <summary>
        /// An object specifying the Keyword enrichment and related parameters.
        /// </summary>
        [fsProperty("keywords")]
        public NluEnrichmentKeywords Keywords { get; set; }
        /// <summary>
        /// An object speficying the Entities enrichment and related parameters.
        /// </summary>
        [fsProperty("entities")]
        public NluEnrichmentEntities Entities { get; set; }
        /// <summary>
        /// An object specifying the sentiment extraction enrichment and related parameters.
        /// </summary>
        [fsProperty("sentiment")]
        public NluEnrichmentSentiment Sentiment { get; set; }
        /// <summary>
        /// An object specifying the emotion detection enrichment and related parameters.
        /// </summary>
        [fsProperty("emotion")]
        public NluEnrichmentEmotion Emotion { get; set; }
        /// <summary>
        /// An object that indicates the Categories enrichment will be applied to the specified field.
        /// </summary>
        [fsProperty("categories")]
        public NluEnrichmentCategories Categories { get; set; }
        /// <summary>
        /// An object specifiying the semantic roles enrichment and related parameters.
        /// </summary>
        [fsProperty("semantic_roles")]
        public NluEnrichmentSemanticRoles SemanticRoles { get; set; }
        /// <summary>
        /// An object specifying the relations enrichment and related parameters.
        /// </summary>
        [fsProperty("relations")]
        public NluEnrichmentRelations Relations { get; set; }
        /// <summary>
        /// An object specifiying the concepts enrichment and related parameters.
        /// </summary>
        [fsProperty("concepts")]
        public NluEnrichmentConcepts Concepts { get; set; }
    }

}
