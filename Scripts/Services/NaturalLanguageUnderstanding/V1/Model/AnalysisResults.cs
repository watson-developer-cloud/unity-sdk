/**
* (C) Copyright IBM Corp. 2019, 2021.
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

namespace IBM.Watson.NaturalLanguageUnderstanding.V1.Model
{
    /// <summary>
    /// Results of the analysis, organized by feature.
    /// </summary>
    public class AnalysisResults
    {
        /// <summary>
        /// Language used to analyze the text.
        /// </summary>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }
        /// <summary>
        /// Text that was used in the analysis.
        /// </summary>
        [JsonProperty("analyzed_text", NullValueHandling = NullValueHandling.Ignore)]
        public string AnalyzedText { get; set; }
        /// <summary>
        /// URL of the webpage that was analyzed.
        /// </summary>
        [JsonProperty("retrieved_url", NullValueHandling = NullValueHandling.Ignore)]
        public string RetrievedUrl { get; set; }
        /// <summary>
        /// API usage information for the request.
        /// </summary>
        [JsonProperty("usage", NullValueHandling = NullValueHandling.Ignore)]
        public AnalysisResultsUsage Usage { get; set; }
        /// <summary>
        /// The general concepts referenced or alluded to in the analyzed text.
        /// </summary>
        [JsonProperty("concepts", NullValueHandling = NullValueHandling.Ignore)]
        public List<ConceptsResult> Concepts { get; set; }
        /// <summary>
        /// The entities detected in the analyzed text.
        /// </summary>
        [JsonProperty("entities", NullValueHandling = NullValueHandling.Ignore)]
        public List<EntitiesResult> Entities { get; set; }
        /// <summary>
        /// The keywords from the analyzed text.
        /// </summary>
        [JsonProperty("keywords", NullValueHandling = NullValueHandling.Ignore)]
        public List<KeywordsResult> Keywords { get; set; }
        /// <summary>
        /// The categories that the service assigned to the analyzed text.
        /// </summary>
        [JsonProperty("categories", NullValueHandling = NullValueHandling.Ignore)]
        public List<CategoriesResult> Categories { get; set; }
        /// <summary>
        /// The classifications assigned to the analyzed text.
        /// </summary>
        [JsonProperty("classifications", NullValueHandling = NullValueHandling.Ignore)]
        public List<ClassificationsResult> Classifications { get; set; }
        /// <summary>
        /// The anger, disgust, fear, joy, or sadness conveyed by the content.
        /// </summary>
        [JsonProperty("emotion", NullValueHandling = NullValueHandling.Ignore)]
        public EmotionResult Emotion { get; set; }
        /// <summary>
        /// Webpage metadata, such as the author and the title of the page.
        /// </summary>
        [JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
        public FeaturesResultsMetadata Metadata { get; set; }
        /// <summary>
        /// The relationships between entities in the content.
        /// </summary>
        [JsonProperty("relations", NullValueHandling = NullValueHandling.Ignore)]
        public List<RelationsResult> Relations { get; set; }
        /// <summary>
        /// Sentences parsed into `subject`, `action`, and `object` form.
        /// </summary>
        [JsonProperty("semantic_roles", NullValueHandling = NullValueHandling.Ignore)]
        public List<SemanticRolesResult> SemanticRoles { get; set; }
        /// <summary>
        /// The sentiment of the content.
        /// </summary>
        [JsonProperty("sentiment", NullValueHandling = NullValueHandling.Ignore)]
        public SentimentResult Sentiment { get; set; }
        /// <summary>
        /// Tokens and sentences returned from syntax analysis.
        /// </summary>
        [JsonProperty("syntax", NullValueHandling = NullValueHandling.Ignore)]
        public SyntaxResult Syntax { get; set; }
    }
}
