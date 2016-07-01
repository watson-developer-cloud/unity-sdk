/**
* Copyright 2015 IBM Corp. All Rights Reserved.
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

using UnityEngine;
using System.Collections;
using FullSerializer;
using IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1;

namespace IBM.Watson.DeveloperCloud.Services.AlchemyDataNews.v1
{
    #region GetNews
    /// <summary>
    /// The NewsResponse.
    /// </summary>
    [fsObject]
    public class NewsResponse
    {
        /// <summary>
        ///  THe status of the request.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// The results.
        /// </summary>
        public Result result { get; set; }
    }

    /// <summary>
    /// The Result data.
    /// </summary>
    [fsObject]
    public class Result
    {
        /// <summary>
        /// The number of results.
        /// </summary>
        public string count { get; set; }
        /// <summary>
        /// The request status.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// The docs data.
        /// </summary>
        public Docs[] docs { get; set; }
    }
    #endregion

    #region Docs
    /// <summary>
    /// The Docs data.
    /// </summary>
    [fsObject]
    public class Docs
    {
        /// <summary>
        /// The request ID.
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// The source data.
        /// </summary>
        public Source source { get; set; }
        /// <summary>
        /// Timestamp of the request.
        /// </summary>
        public string timestamp { get; set; }
    }
    #endregion

    #region Source
    /// <summary>
    /// The Source data.
    /// </summary>
    [fsObject]
    public class Source
    {
        /// <summary>
        /// Original data.
        /// </summary>
        public Original original { get; set; }
    }
    #endregion

    #region Originial
    /// <summary>
    /// The Original data.
    /// </summary>
    [fsObject]
    public class Original
    {
        /// <summary>
        /// The URL.
        /// </summary>
        public string url { get; set; }
    }
    #endregion

    #region Enriched
    /// <summary>
    /// The Enriched data.
    /// </summary>
    [fsObject]
    public class Enriched
    {
        /// <summary>
        /// The URL.
        /// </summary>
        public URL url { get; set; }
    }
    #endregion

    #region URL
    /// <summary>
    /// The URL data.
    /// </summary>
    [fsObject]
    public class URL
    {
        /// <summary>
        /// The image data.
        /// </summary>
        public string image { get; set; }
        /// <summary>
        /// The image keywords.
        /// </summary>
        public ImageKeyword[] imageKeywords { get; set; }
        /// <summary>
        /// The RSS Feed data.
        /// </summary>
        public Feed[] feeds { get; set; }
        /// <summary>
        /// The URL.
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// The raw title.
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// The cleaned title.
        /// </summary>
        public string cleanedTitle { get; set; }
        /// <summary>
        /// Detected language.
        /// </summary>
        public string language { get; set; }
        /// <summary>
        /// The PublicationDate data.
        /// </summary>
        public PublicationDate publicationDate { get; set; }
        /// <summary>
        /// Text data.
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// /The Author data.
        /// </summary>
        public string author { get; set; }
        /// <summary>
        /// Keyword Extraction data.
        /// </summary>
        public Keyword[] keywords { get; set; }
        /// <summary>
        /// Entity extraction data.
        /// </summary>
        public Entity[] entities { get; set; }
        /// <summary>
        /// The concept data.
        /// </summary>
        public Concept[] concepts { get; set; }
        /// <summary>
        /// The relations data.
        /// </summary>
        public Relation relations { get; set; }
        /// <summary>
        /// The DocSentiment.
        /// </summary>
        public DocSentiment docSentiment { get; set; }
        /// <summary>
        /// The taxonomy data.
        /// </summary>
        public Taxonomy taxonomy { get; set; }
        /// <summary>
        /// The enriched title data.
        /// </summary>
        public EnrichedTitle[] enrichedTitle { get; set; }
    }
    #endregion

    #region EnrichedTitle
    /// <summary>
    /// The EnrichedTitle data.
    /// </summary>
    [fsObject]
    public class EnrichedTitle
    {
        /// <summary>
        /// Keyword extraction data.
        /// </summary>
        public Keyword[] keywords { get; set; }
        /// <summary>
        /// Entity extraction data.
        /// </summary>
        public Entity[] entities { get; set; }
        /// <summary>
        /// The Concepts data.
        /// </summary>
        public Concept[] concepts { get; set; }
        /// <summary>
        /// Relations data.
        /// </summary>
        public Relation[] relations { get; set; }
        /// <summary>
        /// The DocSentiment.
        /// </summary>
        public DocSentiment docSentiment { get; set; }
        /// <summary>
        /// Taxonomy data.
        /// </summary>
        public Taxonomy[] taxonomy { get; set; }
    }
    #endregion
    
    #region AlchemyData News Fields
    /// <summary>
    /// All fields for querying and receiving data.
    /// </summary>
    public class Fields
    {
        public const string ORIGINAL_URL = "original.url";
        public const string ENRICHED_URL_IMAGE = "enriched.url.image";
        public const string ENRICHED_URL_IMAGEKEYWORDS = "enriched.url.imageKeywords";
        public const string ENRICHED_URL_IMAGEKEYWORDS_IMAGEKEYWORD_TEXT = "enriched.url.imageKeywords.imageKeyword.text";
        public const string ENRICHED_URL_IMAGEKEYWORDS_IMAGEKEYWORD_SCORE = "enriched.url.imageKeywords.imageKeyword.score";
        public const string ENRICHED_URL_FEEDS = "enriched.url.feeds";
        public const string ENRICHED_URL_FEEDS_FEED_FEED = "enriched.url.feeds.feed.feed";
        public const string ENRICHED_URL_URL = "enriched.url.url";
        public const string ENRICHED_URL_TITLE = "enriched.url.title";
        public const string ENRICHED_URL_CLEANEDTITLE = "enriched.url.cleanedTitle";
        public const string ENRICHED_URL_LANGUAGE = "enriched.url.language";
        public const string ENRICHED_URL_PUBLICATIONDATE_DATE = "enriched.url.publicationDate.date";
        public const string ENRICHED_URL_PUBLICATIONDATE_CONFIDENT = "enriched.url.publicationDate.confident";
        public const string ENRICHED_URL_TEXT = "enriched.url.text";
        public const string ENRICHED_URL_AUTHOR = "enriched.url.author";
        public const string ENRICHED_URL_KEYWORDS = "enriched.url.keywords";
        public const string ENRICHED_URL_KEYWORDS_KEYWORD_TEXT = "enriched.url.keywords.keyword.text";
        public const string ENRICHED_URL_KEYWORDS_KEYWORD_RELEVANCE = "enriched.url.keywords.keyword.relevance";
        public const string ENRICHED_URL_KEYWORDS_KEYWORD_SENTIMENT_TYPE = "enriched.url.keywords.keyword.sentiment.type";
        public const string ENRICHED_URL_KEYWORDS_KEYWORD_SENTIMENT_SCORE = "enriched.url.keywords.keyword.sentiment.score";
        public const string ENRICHED_URL_KEYWORDS_KEYWORD_SENTIMENT_MIXED = "enriched.url.keywords.keyword.sentiment.mixed";
        public const string ENRICHED_URL_KEYWORDS_KEYWORD_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.keywords.keyword.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENTITIES = "enriched.url.entities";
        public const string ENRICHED_URL_ENTITIES_ENTITY_TEXT = "enriched.url.entities.entity.text";
        public const string ENRICHED_URL_ENTITIES_ENTITY_TYPE = "enriched.url.entities.entity.type";
        public const string ENRICHED_URL_ENTITIES_ENTITY_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.entities.entity.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENTITIES_ENTITY_COUNT = "enriched.url.entities.entity.count";
        public const string ENRICHED_URL_ENTITIES_ENTITY_RELEVANCE = "enriched.url.entities.entity.relevance";
        public const string ENRICHED_URL_ENTITIES_ENTITY_SENTIMENT_TYPE = "enriched.url.entities.entity.sentiment.type";
        public const string ENRICHED_URL_ENTITIES_ENTITY_SENTIMENT_SCORE = "enriched.url.entities.entity.sentiment.score";
        public const string ENRICHED_URL_ENTITIES_ENTITY_SENTIMENT_MIXED = "enriched.url.entities.entity.sentiment.mixed";
        public const string ENRICHED_URL_ENTITIES_ENTITY_DISAMBIGUATED_NAME = "enriched.url.entities.entity.disambiguated.name";
        public const string ENRICHED_URL_ENTITIES_ENTITY_DISAMBIGUATED_GEO = "enriched.url.entities.entity.disambiguated.geo";
        public const string ENRICHED_URL_ENTITIES_ENTITY_DISAMBIGUATED_DBPEDIA = "enriched.url.entities.entity.disambiguated.dbpedia";
        public const string ENRICHED_URL_ENTITIES_ENTITY_DISAMBIGUATED_WEBSITE = "enriched.url.entities.entity.disambiguated.website";
        public const string ENRICHED_URL_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE = "enriched.url.entities.entity.disambiguated.subType";
        public const string ENRICHED_URL_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE_SUBTYPE_ = "enriched.url.entities.entity.disambiguated.subType.subType_";
        public const string ENRICHED_URL_ENTITIES_ENTITY_QUOTATIONS = "enriched.url.entities.entity.quotations";
        public const string ENRICHED_URL_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__QUOTATION = "enriched.url.entities.entity.quotations.quotation_.quotation";
        public const string ENRICHED_URL_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_TYPE = "enriched.url.entities.entity.quotations.quotation_.sentiment.type";
        public const string ENRICHED_URL_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_SCORE = "enriched.url.entities.entity.quotations.quotation_.sentiment.score";
        public const string ENRICHED_URL_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_MIXED = "enriched.url.entities.entity.quotations.quotation_.sentiment.mixed";
        public const string ENRICHED_URL_CONCEPTS = "enriched.url.concepts";
        public const string ENRICHED_URL_CONCEPTS_CONCEPT_TEXT = "enriched.url.concepts.concept.text";
        public const string ENRICHED_URL_CONCEPTS_CONCEPT_RELEVANCE = "enriched.url.concepts.concept.relevance";
        public const string ENRICHED_URL_CONCEPTS_CONCEPT_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.concepts.concept.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_RELATIONS = "enriched.url.relations";
        public const string ENRICHED_URL_RELATIONS_RELATION_SENTENCE = "enriched.url.relations.relation.sentence";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_TEXT = "enriched.url.relations.relation.subject.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES = "enriched.url.relations.relation.subject.entities";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_TEXT = "enriched.url.relations.relation.subject.entities.entity.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_TYPE = "enriched.url.relations.relation.subject.entities.entity.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.relations.relation.subject.entities.entity.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_COUNT = "enriched.url.relations.relation.subject.entities.entity.count";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_RELEVANCE = "enriched.url.relations.relation.subject.entities.entity.relevance";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_SENTIMENT_TYPE = "enriched.url.relations.relation.subject.entities.entity.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_SENTIMENT_SCORE = "enriched.url.relations.relation.subject.entities.entity.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_SENTIMENT_MIXED = "enriched.url.relations.relation.subject.entities.entity.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_NAME = "enriched.url.relations.relation.subject.entities.entity.disambiguated.name";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_GEO = "enriched.url.relations.relation.subject.entities.entity.disambiguated.geo";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_DBPEDIA = "enriched.url.relations.relation.subject.entities.entity.disambiguated.dbpedia";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_WEBSITE = "enriched.url.relations.relation.subject.entities.entity.disambiguated.website";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE = "enriched.url.relations.relation.subject.entities.entity.disambiguated.subType";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE_SUBTYPE_ = "enriched.url.relations.relation.subject.entities.entity.disambiguated.subType.subType_";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS = "enriched.url.relations.relation.subject.entities.entity.quotations";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__QUOTATION = "enriched.url.relations.relation.subject.entities.entity.quotations.quotation_.quotation";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_TYPE = "enriched.url.relations.relation.subject.entities.entity.quotations.quotation_.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_SCORE = "enriched.url.relations.relation.subject.entities.entity.quotations.quotation_.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_MIXED = "enriched.url.relations.relation.subject.entities.entity.quotations.quotation_.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_KEYWORDS = "enriched.url.relations.relation.subject.keywords";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_TEXT = "enriched.url.relations.relation.subject.keywords.keyword.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_RELEVANCE = "enriched.url.relations.relation.subject.keywords.keyword.relevance";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_SENTIMENT_TYPE = "enriched.url.relations.relation.subject.keywords.keyword.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_SENTIMENT_SCORE = "enriched.url.relations.relation.subject.keywords.keyword.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_SENTIMENT_MIXED = "enriched.url.relations.relation.subject.keywords.keyword.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.relations.relation.subject.keywords.keyword.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_SENTIMENT_TYPE = "enriched.url.relations.relation.subject.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_SENTIMENT_SCORE = "enriched.url.relations.relation.subject.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_SENTIMENT_MIXED = "enriched.url.relations.relation.subject.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_ACTION_TEXT = "enriched.url.relations.relation.action.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_ACTION_LEMMATIZED = "enriched.url.relations.relation.action.lemmatized";
        public const string ENRICHED_URL_RELATIONS_RELATION_ACTION_VERB_TEXT = "enriched.url.relations.relation.action.verb.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_ACTION_VERB_TENSE = "enriched.url.relations.relation.action.verb.tense";
        public const string ENRICHED_URL_RELATIONS_RELATION_ACTION_VERB_NEGATED = "enriched.url.relations.relation.action.verb.negated";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_TEXT = "enriched.url.relations.relation.object.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES = "enriched.url.relations.relation.object.entities";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_TEXT = "enriched.url.relations.relation.object.entities.entity.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_TYPE = "enriched.url.relations.relation.object.entities.entity.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.relations.relation.object.entities.entity.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_COUNT = "enriched.url.relations.relation.object.entities.entity.count";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_RELEVANCE = "enriched.url.relations.relation.object.entities.entity.relevance";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_SENTIMENT_TYPE = "enriched.url.relations.relation.object.entities.entity.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_SENTIMENT_SCORE = "enriched.url.relations.relation.object.entities.entity.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_SENTIMENT_MIXED = "enriched.url.relations.relation.object.entities.entity.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_NAME = "enriched.url.relations.relation.object.entities.entity.disambiguated.name";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_GEO = "enriched.url.relations.relation.object.entities.entity.disambiguated.geo";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_DBPEDIA = "enriched.url.relations.relation.object.entities.entity.disambiguated.dbpedia";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_WEBSITE = "enriched.url.relations.relation.object.entities.entity.disambiguated.website";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE = "enriched.url.relations.relation.object.entities.entity.disambiguated.subType";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE_SUBTYPE_ = "enriched.url.relations.relation.object.entities.entity.disambiguated.subType.subType_";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS = "enriched.url.relations.relation.object.entities.entity.quotations";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__QUOTATION = "enriched.url.relations.relation.object.entities.entity.quotations.quotation_.quotation";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_TYPE = "enriched.url.relations.relation.object.entities.entity.quotations.quotation_.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_SCORE = "enriched.url.relations.relation.object.entities.entity.quotations.quotation_.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_MIXED = "enriched.url.relations.relation.object.entities.entity.quotations.quotation_.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_KEYWORDS = "enriched.url.relations.relation.object.keywords";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_TEXT = "enriched.url.relations.relation.object.keywords.keyword.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_RELEVANCE = "enriched.url.relations.relation.object.keywords.keyword.relevance";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_SENTIMENT_TYPE = "enriched.url.relations.relation.object.keywords.keyword.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_SENTIMENT_SCORE = "enriched.url.relations.relation.object.keywords.keyword.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_SENTIMENT_MIXED = "enriched.url.relations.relation.object.keywords.keyword.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.relations.relation.object.keywords.keyword.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_SENTIMENT_TYPE = "enriched.url.relations.relation.object.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_SENTIMENT_SCORE = "enriched.url.relations.relation.object.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_SENTIMENT_MIXED = "enriched.url.relations.relation.object.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_SENTIMENTFROMSUBJECT_TYPE = "enriched.url.relations.relation.object.sentimentFromSubject.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_SENTIMENTFROMSUBJECT_SCORE = "enriched.url.relations.relation.object.sentimentFromSubject.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_SENTIMENTFROMSUBJECT_MIXED = "enriched.url.relations.relation.object.sentimentFromSubject.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_TEXT = "enriched.url.relations.relation.location.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_SENTIMENT_TYPE = "enriched.url.relations.relation.location.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_SENTIMENT_SCORE = "enriched.url.relations.relation.location.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_SENTIMENT_MIXED = "enriched.url.relations.relation.location.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES = "enriched.url.relations.relation.location.entities";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_TEXT = "enriched.url.relations.relation.location.entities.entity.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_TYPE = "enriched.url.relations.relation.location.entities.entity.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.relations.relation.location.entities.entity.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_COUNT = "enriched.url.relations.relation.location.entities.entity.count";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_RELEVANCE = "enriched.url.relations.relation.location.entities.entity.relevance";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_SENTIMENT_TYPE = "enriched.url.relations.relation.location.entities.entity.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_SENTIMENT_SCORE = "enriched.url.relations.relation.location.entities.entity.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_SENTIMENT_MIXED = "enriched.url.relations.relation.location.entities.entity.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_NAME = "enriched.url.relations.relation.location.entities.entity.disambiguated.name";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_GEO = "enriched.url.relations.relation.location.entities.entity.disambiguated.geo";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_DBPEDIA = "enriched.url.relations.relation.location.entities.entity.disambiguated.dbpedia";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_WEBSITE = "enriched.url.relations.relation.location.entities.entity.disambiguated.website";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE = "enriched.url.relations.relation.location.entities.entity.disambiguated.subType";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE_SUBTYPE_ = "enriched.url.relations.relation.location.entities.entity.disambiguated.subType.subType_";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS = "enriched.url.relations.relation.location.entities.entity.quotations";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__QUOTATION = "enriched.url.relations.relation.location.entities.entity.quotations.quotation_.quotation";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_TYPE = "enriched.url.relations.relation.location.entities.entity.quotations.quotation_.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_SCORE = "enriched.url.relations.relation.location.entities.entity.quotations.quotation_.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_MIXED = "enriched.url.relations.relation.location.entities.entity.quotations.quotation_.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_KEYWORDS = "enriched.url.relations.relation.location.keywords";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_TEXT = "enriched.url.relations.relation.location.keywords.keyword.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_RELEVANCE = "enriched.url.relations.relation.location.keywords.keyword.relevance";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_SENTIMENT_TYPE = "enriched.url.relations.relation.location.keywords.keyword.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_SENTIMENT_SCORE = "enriched.url.relations.relation.location.keywords.keyword.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_SENTIMENT_MIXED = "enriched.url.relations.relation.location.keywords.keyword.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.relations.relation.location.keywords.keyword.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_RELATIONS_RELATION_TEMPORAL_TEXT = "enriched.url.relations.relation.temporal.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_TEMPORAL_DECODED_TYPE = "enriched.url.relations.relation.temporal.decoded.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_TEMPORAL_DECODED_VALUE = "enriched.url.relations.relation.temporal.decoded.value";
        public const string ENRICHED_URL_RELATIONS_RELATION_TEMPORAL_DECODED_START = "enriched.url.relations.relation.temporal.decoded.start";
        public const string ENRICHED_URL_RELATIONS_RELATION_TEMPORAL_DECODED_END = "enriched.url.relations.relation.temporal.decoded.end";
        public const string ENRICHED_URL_DOCSENTIMENT_TYPE = "enriched.url.docSentiment.type";
        public const string ENRICHED_URL_DOCSENTIMENT_SCORE = "enriched.url.docSentiment.score";
        public const string ENRICHED_URL_DOCSENTIMENT_MIXED = "enriched.url.docSentiment.mixed";
        public const string ENRICHED_URL_TAXONOMY = "enriched.url.taxonomy";
        public const string ENRICHED_URL_TAXONOMY_TAXONOMY__LABEL = "enriched.url.taxonomy.taxonomy_.label";
        public const string ENRICHED_URL_TAXONOMY_TAXONOMY__SCORE = "enriched.url.taxonomy.taxonomy_.score";
        public const string ENRICHED_URL_TAXONOMY_TAXONOMY__CONFIDENT = "enriched.url.taxonomy.taxonomy_.confident";
        public const string ENRICHED_URL_ENRICHEDTITLE_KEYWORDS = "enriched.url.enrichedTitle.keywords";
        public const string ENRICHED_URL_ENRICHEDTITLE_KEYWORDS_KEYWORD_TEXT = "enriched.url.enrichedTitle.keywords.keyword.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_KEYWORDS_KEYWORD_RELEVANCE = "enriched.url.enrichedTitle.keywords.keyword.relevance";
        public const string ENRICHED_URL_ENRICHEDTITLE_KEYWORDS_KEYWORD_SENTIMENT_TYPE = "enriched.url.enrichedTitle.keywords.keyword.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_KEYWORDS_KEYWORD_SENTIMENT_SCORE = "enriched.url.enrichedTitle.keywords.keyword.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_KEYWORDS_KEYWORD_SENTIMENT_MIXED = "enriched.url.enrichedTitle.keywords.keyword.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_KEYWORDS_KEYWORD_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.enrichedTitle.keywords.keyword.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES = "enriched.url.enrichedTitle.entities";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_TEXT = "enriched.url.enrichedTitle.entities.entity.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_TYPE = "enriched.url.enrichedTitle.entities.entity.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.enrichedTitle.entities.entity.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_COUNT = "enriched.url.enrichedTitle.entities.entity.count";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_RELEVANCE = "enriched.url.enrichedTitle.entities.entity.relevance";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_SENTIMENT_TYPE = "enriched.url.enrichedTitle.entities.entity.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_SENTIMENT_SCORE = "enriched.url.enrichedTitle.entities.entity.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_SENTIMENT_MIXED = "enriched.url.enrichedTitle.entities.entity.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_DISAMBIGUATED_NAME = "enriched.url.enrichedTitle.entities.entity.disambiguated.name";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_DISAMBIGUATED_GEO = "enriched.url.enrichedTitle.entities.entity.disambiguated.geo";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_DISAMBIGUATED_DBPEDIA = "enriched.url.enrichedTitle.entities.entity.disambiguated.dbpedia";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_DISAMBIGUATED_WEBSITE = "enriched.url.enrichedTitle.entities.entity.disambiguated.website";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE = "enriched.url.enrichedTitle.entities.entity.disambiguated.subType";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE_SUBTYPE_ = "enriched.url.enrichedTitle.entities.entity.disambiguated.subType.subType_";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_QUOTATIONS = "enriched.url.enrichedTitle.entities.entity.quotations";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__QUOTATION = "enriched.url.enrichedTitle.entities.entity.quotations.quotation_.quotation";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_TYPE = "enriched.url.enrichedTitle.entities.entity.quotations.quotation_.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_SCORE = "enriched.url.enrichedTitle.entities.entity.quotations.quotation_.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_MIXED = "enriched.url.enrichedTitle.entities.entity.quotations.quotation_.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_CONCEPTS = "enriched.url.enrichedTitle.concepts";
        public const string ENRICHED_URL_ENRICHEDTITLE_CONCEPTS_CONCEPT_TEXT = "enriched.url.enrichedTitle.concepts.concept.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_CONCEPTS_CONCEPT_RELEVANCE = "enriched.url.enrichedTitle.concepts.concept.relevance";
        public const string ENRICHED_URL_ENRICHEDTITLE_CONCEPTS_CONCEPT_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.enrichedTitle.concepts.concept.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS = "enriched.url.enrichedTitle.relations";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SENTENCE = "enriched.url.enrichedTitle.relations.relation.sentence";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_TEXT = "enriched.url.enrichedTitle.relations.relation.subject.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES = "enriched.url.enrichedTitle.relations.relation.subject.entities";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_TEXT = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_TYPE = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_COUNT = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.count";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_RELEVANCE = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.relevance";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_NAME = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.disambiguated.name";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_GEO = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.disambiguated.geo";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_DBPEDIA = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.disambiguated.dbpedia";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_WEBSITE = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.disambiguated.website";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.disambiguated.subType";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE_SUBTYPE_ = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.disambiguated.subType.subType_";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.quotations";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__QUOTATION = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.quotations.quotation_.quotation";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.quotations.quotation_.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.quotations.quotation_.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.quotations.quotation_.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_KEYWORDS = "enriched.url.enrichedTitle.relations.relation.subject.keywords";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_TEXT = "enriched.url.enrichedTitle.relations.relation.subject.keywords.keyword.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_RELEVANCE = "enriched.url.enrichedTitle.relations.relation.subject.keywords.keyword.relevance";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.subject.keywords.keyword.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.subject.keywords.keyword.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.subject.keywords.keyword.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.enrichedTitle.relations.relation.subject.keywords.keyword.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.subject.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.subject.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.subject.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_ACTION_TEXT = "enriched.url.enrichedTitle.relations.relation.action.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_ACTION_LEMMATIZED = "enriched.url.enrichedTitle.relations.relation.action.lemmatized";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_ACTION_VERB_TEXT = "enriched.url.enrichedTitle.relations.relation.action.verb.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_ACTION_VERB_TENSE = "enriched.url.enrichedTitle.relations.relation.action.verb.tense";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_ACTION_VERB_NEGATED = "enriched.url.enrichedTitle.relations.relation.action.verb.negated";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_TEXT = "enriched.url.enrichedTitle.relations.relation.object.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES = "enriched.url.enrichedTitle.relations.relation.object.entities";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_TEXT = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_TYPE = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_COUNT = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.count";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_RELEVANCE = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.relevance";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_NAME = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.disambiguated.name";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_GEO = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.disambiguated.geo";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_DBPEDIA = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.disambiguated.dbpedia";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_WEBSITE = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.disambiguated.website";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.disambiguated.subType";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE_SUBTYPE_ = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.disambiguated.subType.subType_";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.quotations";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__QUOTATION = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.quotations.quotation_.quotation";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.quotations.quotation_.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.quotations.quotation_.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.quotations.quotation_.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_KEYWORDS = "enriched.url.enrichedTitle.relations.relation.object.keywords";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_TEXT = "enriched.url.enrichedTitle.relations.relation.object.keywords.keyword.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_RELEVANCE = "enriched.url.enrichedTitle.relations.relation.object.keywords.keyword.relevance";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.object.keywords.keyword.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.object.keywords.keyword.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.object.keywords.keyword.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.enrichedTitle.relations.relation.object.keywords.keyword.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.object.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.object.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.object.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_SENTIMENTFROMSUBJECT_TYPE = "enriched.url.enrichedTitle.relations.relation.object.sentimentFromSubject.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_SENTIMENTFROMSUBJECT_SCORE = "enriched.url.enrichedTitle.relations.relation.object.sentimentFromSubject.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_SENTIMENTFROMSUBJECT_MIXED = "enriched.url.enrichedTitle.relations.relation.object.sentimentFromSubject.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_TEXT = "enriched.url.enrichedTitle.relations.relation.location.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.location.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.location.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.location.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES = "enriched.url.enrichedTitle.relations.relation.location.entities";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_TEXT = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_TYPE = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_COUNT = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.count";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_RELEVANCE = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.relevance";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_NAME = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.disambiguated.name";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_GEO = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.disambiguated.geo";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_DBPEDIA = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.disambiguated.dbpedia";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_WEBSITE = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.disambiguated.website";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.disambiguated.subType";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE_SUBTYPE_ = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.disambiguated.subType.subType_";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.quotations";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__QUOTATION = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.quotations.quotation_.quotation";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.quotations.quotation_.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.quotations.quotation_.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.quotations.quotation_.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_KEYWORDS = "enriched.url.enrichedTitle.relations.relation.location.keywords";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_TEXT = "enriched.url.enrichedTitle.relations.relation.location.keywords.keyword.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_RELEVANCE = "enriched.url.enrichedTitle.relations.relation.location.keywords.keyword.relevance";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.location.keywords.keyword.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.location.keywords.keyword.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.location.keywords.keyword.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.enrichedTitle.relations.relation.location.keywords.keyword.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_TEMPORAL_TEXT = "enriched.url.enrichedTitle.relations.relation.temporal.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_TEMPORAL_DECODED_TYPE = "enriched.url.enrichedTitle.relations.relation.temporal.decoded.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_TEMPORAL_DECODED_VALUE = "enriched.url.enrichedTitle.relations.relation.temporal.decoded.value";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_TEMPORAL_DECODED_START = "enriched.url.enrichedTitle.relations.relation.temporal.decoded.start";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_TEMPORAL_DECODED_END = "enriched.url.enrichedTitle.relations.relation.temporal.decoded.end";
        public const string ENRICHED_URL_ENRICHEDTITLE_DOCSENTIMENT_TYPE = "enriched.url.enrichedTitle.docSentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_DOCSENTIMENT_SCORE = "enriched.url.enrichedTitle.docSentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_DOCSENTIMENT_MIXED = "enriched.url.enrichedTitle.docSentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_TAXONOMY = "enriched.url.enrichedTitle.taxonomy";
        public const string ENRICHED_URL_ENRICHEDTITLE_TAXONOMY_TAXONOMY__LABEL = "enriched.url.enrichedTitle.taxonomy.taxonomy_.label";
        public const string ENRICHED_URL_ENRICHEDTITLE_TAXONOMY_TAXONOMY__SCORE = "enriched.url.enrichedTitle.taxonomy.taxonomy_.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_TAXONOMY_TAXONOMY__CONFIDENT = "enriched.url.enrichedTitle.taxonomy.taxonomy_.confident";
    }
    #endregion
}
