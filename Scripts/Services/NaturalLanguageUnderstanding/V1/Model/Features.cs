/**
* (C) Copyright IBM Corp. 2018, 2020.
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

namespace IBM.Watson.NaturalLanguageUnderstanding.V1.Model
{
    /// <summary>
    /// Analysis features and options.
    /// </summary>
    public class Features
    {
        /// <summary>
        /// Returns high-level concepts in the content. For example, a research paper about deep learning might return
        /// the concept, "Artificial Intelligence" although the term is not mentioned.
        ///
        /// Supported languages: English, French, German, Italian, Japanese, Korean, Portuguese, Spanish.
        /// </summary>
        [JsonProperty("concepts", NullValueHandling = NullValueHandling.Ignore)]
        public ConceptsOptions Concepts { get; set; }
        /// <summary>
        /// Detects anger, disgust, fear, joy, or sadness that is conveyed in the content or by the context around
        /// target phrases specified in the targets parameter. You can analyze emotion for detected entities with
        /// `entities.emotion` and for keywords with `keywords.emotion`.
        ///
        /// Supported languages: English.
        /// </summary>
        [JsonProperty("emotion", NullValueHandling = NullValueHandling.Ignore)]
        public EmotionOptions Emotion { get; set; }
        /// <summary>
        /// Identifies people, cities, organizations, and other entities in the content. For more information, see
        /// [Entity types and
        /// subtypes](https://cloud.ibm.com/docs/natural-language-understanding?topic=natural-language-understanding-entity-types).
        ///
        /// Supported languages: English, French, German, Italian, Japanese, Korean, Portuguese, Russian, Spanish,
        /// Swedish. Arabic, Chinese, and Dutch are supported only through custom models.
        /// </summary>
        [JsonProperty("entities", NullValueHandling = NullValueHandling.Ignore)]
        public EntitiesOptions Entities { get; set; }
        /// <summary>
        /// Returns important keywords in the content.
        ///
        /// Supported languages: English, French, German, Italian, Japanese, Korean, Portuguese, Russian, Spanish,
        /// Swedish.
        /// </summary>
        [JsonProperty("keywords", NullValueHandling = NullValueHandling.Ignore)]
        public KeywordsOptions Keywords { get; set; }
        /// <summary>
        /// Returns information from the document, including author name, title, RSS/ATOM feeds, prominent page image,
        /// and publication date. Supports URL and HTML input types only.
        /// </summary>
        [JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
        public MetadataOptions Metadata { get; set; }
        /// <summary>
        /// Recognizes when two entities are related and identifies the type of relation. For example, an `awardedTo`
        /// relation might connect the entities "Nobel Prize" and "Albert Einstein". For more information, see [Relation
        /// types](https://cloud.ibm.com/docs/natural-language-understanding?topic=natural-language-understanding-relations).
        ///
        /// Supported languages: Arabic, English, German, Japanese, Korean, Spanish. Chinese, Dutch, French, Italian,
        /// and Portuguese custom models are also supported.
        /// </summary>
        [JsonProperty("relations", NullValueHandling = NullValueHandling.Ignore)]
        public RelationsOptions Relations { get; set; }
        /// <summary>
        /// Parses sentences into subject, action, and object form.
        ///
        /// Supported languages: English, German, Japanese, Korean, Spanish.
        /// </summary>
        [JsonProperty("semantic_roles", NullValueHandling = NullValueHandling.Ignore)]
        public SemanticRolesOptions SemanticRoles { get; set; }
        /// <summary>
        /// Analyzes the general sentiment of your content or the sentiment toward specific target phrases. You can
        /// analyze sentiment for detected entities with `entities.sentiment` and for keywords with
        /// `keywords.sentiment`.
        ///
        ///  Supported languages: Arabic, English, French, German, Italian, Japanese, Korean, Portuguese, Russian,
        /// Spanish.
        /// </summary>
        [JsonProperty("sentiment", NullValueHandling = NullValueHandling.Ignore)]
        public SentimentOptions Sentiment { get; set; }
        /// <summary>
        /// Returns a five-level taxonomy of the content. The top three categories are returned.
        ///
        /// Supported languages: Arabic, English, French, German, Italian, Japanese, Korean, Portuguese, Spanish.
        /// </summary>
        [JsonProperty("categories", NullValueHandling = NullValueHandling.Ignore)]
        public CategoriesOptions Categories { get; set; }
        /// <summary>
        /// Returns tokens and sentences from the input text.
        /// </summary>
        [JsonProperty("syntax", NullValueHandling = NullValueHandling.Ignore)]
        public SyntaxOptions Syntax { get; set; }
    }
}
