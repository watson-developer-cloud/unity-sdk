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

using System;
using FullSerializer;
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.Services.NaturalLanguageUnderstanding.v1
{
    [fsObject]
    public class AnalysisResults
    {
        /// <summary>
        /// The general concepts referenced or alluded to in the specified content
        /// </summary>
        public ConceptsResult[] concepts { get; set; }
        /// <summary>
        /// The important entities in the specified content
        /// </summary>
        public EntitiesResult[] entities { get; set; }
        /// <summary>
        /// The important keywords in content organized by relevance 
        /// </summary>
        public KeywordsResult[] keywords { get; set; }
        /// <summary>
        /// The hierarchical 5-level taxonomy the content is categorized into 
        /// </summary>
        public CategoriesResult[] categories { get; set; }
        /// <summary>
        /// The anger, disgust, fear, joy, or sadness conveyed by the content 
        /// </summary>
        public EmotionResult emotion { get; set; }
        /// <summary>
        /// The metadata holds author information, publication date and the title of the text/HTML content
        /// </summary>
        public MetadataResult metadata { get; set; }
        /// <summary>
        /// The relationships between entities in the content 
        /// </summary>
        public RelationsResult[] relations { get; set; }
        /// <summary>
        /// The subjects of actions and the objects the actions act upon 
        /// </summary>
        public SemanticRolesResult[] semantic_roles { get; set; }
        /// <summary>
        /// The sentiment of the content
        /// </summary>
        public SentimentResult sentiment { get; set; }
        /// <summary>
        /// Language used to analyze the text 
        /// </summary>
        public string language { get; set; }
        /// <summary>
        /// Text that was used in the analysis 
        /// </summary>
        public string analyzed_text { get; set; }
        /// <summary>
        /// URL that was used to retrieve HTML content
        /// </summary>
        public string retrieved_url { get; set; }
        /// <summary>
        /// API usage information for the request
        /// </summary>
        public Usage usage { get; set; }
    }

    [fsObject]
    public class ConceptsResult
    {
        /// <summary>
        /// Name of the concept 
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// Relevance score between 0 and 1. Higher scores indicate greater relevance 
        /// </summary>
        public float relevance { get; set; }
        /// <summary>
        /// Link to the corresponding DBpedia resource
        /// </summary>
        public string dbpedia_resource { get; set; }
    }

    [fsObject]
    public class EntitiesResult
    {
        /// <summary>
        /// Entity type
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// Relevance score from 0 to 1. Higher values indicate greater relevance 
        /// </summary>
        public float relevance { get; set; }
        /// <summary>
        /// How many times the entity was mentioned in the text
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// The name of the entity
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// Emotion analysis results for the entity, enabled with the "emotion" option
        /// </summary>
        public EmotionScores emotion { get; set; }
        /// <summary>
        /// Sentiment analysis results for the entity, enabled with the "sentiment" option
        /// </summary>
        public FeatureSentimentResults sentiment { get; set; }
        /// <summary>
        /// Disambiguation information for the entity
        /// </summary>
        public DisambiguationResult disambiguation { get; set; }
    }

    [fsObject]
    public class KeywordsResult
    {
        /// <summary>
        /// Relevance score from 0 to 1. Higher values indicate greater relevance 
        /// </summary>
        public float relevance { get; set; }
        /// <summary>
        /// The keyword text 
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// Emotion analysis results for the keyword, enabled with the "emotion" option
        /// </summary>
        public EmotionScores emotion { get; set; }
        /// <summary>
        /// Sentiment analysis results for the keyword, enabled with the "sentiment" option
        /// </summary>
        public FeatureSentimentResults sentiment { get; set; }
    }

    [fsObject]
    public class CategoriesResult
    {
        /// <summary>
        /// The path to the category through the taxonomy hierarchy
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// Confidence score for the category classification. Higher values indicate greater confidence
        /// </summary>
        public float score { get; set; }
    }

    [fsObject]
    public class EmotionResult
    {
        /// <summary>
        /// The returned emotion results across the document
        /// </summary>
        public DocumentEmotionResults document { get; set; }
        /// <summary>
        /// The returned emotion results per specified target
        /// </summary>
        public TargetedEmotionResults[] targets { get; set; }
    }

    [fsObject]
    public class MetadataResult
    {
        /// <summary>
        /// The authors of the document
        /// </summary>
        public Author[] authors { get; set; }
        /// <summary>
        /// The publication date in the format ISO 8601
        /// </summary>
        public string publication_date { get; set; }
        /// <summary>
        /// The title of the document
        /// </summary>
        public string title { get; set; }
    }

    [fsObject]
    public class RelationsResult
    {
        /// <summary>
        /// Confidence score for the relation. Higher values indicate greater confidence.
        /// </summary>
        public float score { get; set; }
        /// <summary>
        /// The sentence that contains the relation 
        /// </summary>
        public string sentence { get; set; }
        /// <summary>
        /// The type of the relation 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// The extracted relation objects from the text
        /// </summary>
        public RelationArgument[] arguments { get; set; }
    }

    [fsObject]
    public class SemanticRolesResult
    {
        /// <summary>
        /// Sentence from the source that contains the subject, action, and object 
        /// </summary>
        public string sentence { get; set; }
        /// <summary>
        /// The extracted subject from the sentence
        /// </summary>
        public SemanticRolesSubject subject { get; set; }
        /// <summary>
        /// The extracted action from the sentence
        /// </summary>
        public SemanticRolesAction action { get; set; }
        /// <summary>
        /// The extracted object from the sentence
        /// </summary>
        public SemanticRolesObject _object { get; set; }
    }

    [fsObject]
    public class SentimentResult
    {
        /// <summary>
        /// The document level sentiment 
        /// </summary>
        public DocumentSentimentResults document { get; set; }
        /// <summary>
        /// The targeted sentiment to analyze
        /// </summary>
        public TargetedSentimentResults[] targets { get; set; }
    }

    [fsObject]
    public class Usage
    {
        /// <summary>
        /// Number of features used in the API call
        /// </summary>
        public int features { get; set; }
    }

    [fsObject]
    public class EmotionScores
    {
        /// <summary>
        /// Anger score from 0 to 1. A higher score means that the text is more likely to convey anger
        /// </summary>
        public float anger { get; set; }
        /// <summary>
        /// Disgust score from 0 to 1. A higher score means that the text is more likely to convey disgust
        /// </summary>
        public float disgust { get; set; }
        /// <summary>
        /// Fear score from 0 to 1. A higher score means that the text is more likely to convey fear
        /// </summary>
        public float fear { get; set; }
        /// <summary>
        /// Joy score from 0 to 1. A higher score means that the text is more likely to convey joy
        /// </summary>
        public float joy { get; set; }
        /// <summary>
        /// Sadness score from 0 to 1. A higher score means that the text is more likely to convey sadness
        /// </summary>
        public float sadness { get; set; }
    }

    [fsObject]
    public class FeatureSentimentResults
    {
        /// <summary>
        /// Sentiment score from -1 (negative) to 1 (positive)
        /// </summary>
        public float score { get; set; }
    }

    [fsObject]
    public class DisambiguationResult
    {
        /// <summary>
        /// Common entity name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Link to the corresponding DBpedia resource
        /// </summary>
        public string dbpedia_resource { get; set; }
        /// <summary>
        /// Entity subtype information
        /// </summary>
        public string subtype { get; set; }
    }

    [fsObject]
    public class DocumentEmotionResults
    {
        /// <summary>
        /// An object containing the emotion results for the document
        /// </summary>
        public EmotionScores emotion { get; set; }
    }

    [fsObject]
    public class TargetedEmotionResults
    {
        /// <summary>
        /// Targeted text
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// An object containing the emotion results for the target
        /// </summary>
        public EmotionScores emotion { get; set; }
    }

    [fsObject]
    public class Author
    {
        /// <summary>
        /// Name of the author
        /// </summary>
        public string name { get; set; }
    }

    [fsObject]
    public class RelationArgument
    {
        public RelationEntity[] entities { get; set; }
        /// <summary>
        /// Text that corresponds to the argument
        /// </summary>
        public string text { get; set; }
    }

    [fsObject]
    public class SemanticRolesSubject
    {
        /// <summary>
        /// Text that corresponds to the subject role
        /// </summary>
        public string text { get; set; }
        public SemanticRolesEntity[] entities { get; set; }
        public SemanticRolesKeyword[] keywords { get; set; }
    }

    [fsObject]
    public class SemanticRolesAction
    {
        /// <summary>
        /// Analyzed text that corresponds to the action
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// normalized version of the action
        /// </summary>
        public string normalized { get; set; }
        public SemanticRolesVerb verb { get; set; }
    }

    [fsObject]
    public class SemanticRolesObject
    {
        /// <summary>
        /// Object text
        /// </summary>
        public string text { get; set; }
        public SemanticRolesKeyword[] keywords { get; set; }
    }

    [fsObject]
    public class DocumentSentimentResults
    {
        /// <summary>
        /// Sentiment score from -1 (negative) to 1 (positive)
        /// </summary>
        public float score { get; set; }
    }

    [fsObject]
    public class TargetedSentimentResults
    {
        /// <summary>
        /// Targeted text
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// Sentiment score from -1 (negative) to 1 (positive)
        /// </summary>
        public float score { get; set; }
    }

    [fsObject]
    public class RelationEntity
    {
        /// <summary>
        /// Text that corresponds to the entity
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// Entity type
        /// </summary>
        public string type { get; set; }
    }

    [fsObject]
    public class SemanticRolesEntity
    {
        /// <summary>
        /// Entity type
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// The entity text
        /// </summary>
        public string text { get; set; }
    }

    [fsObject]
    public class SemanticRolesKeyword
    {
        /// <summary>
        /// The keyword text
        /// </summary>
        public string text { get; set; }
    }

    [fsObject]
    public class SemanticRolesVerb
    {
        /// <summary>
        /// The keyword text
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// Verb tense
        /// </summary>
        public string tense { get; set; }
    }

    [fsObject]
    public class ListModelsResults
    {
        public CustomModel[] models { get; set; }
    }

    [fsObject]
    public class CustomModel
    {
        /// <summary>
        /// Shows as available if the model is ready for use
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// Unique model ID
        /// </summary>
        public string model_id { get; set; }
        /// <summary>
        /// ISO 639-1 code indicating the language of the model
        /// </summary>
        public string language { get; set; }
        /// <summary>
        /// Model description
        /// </summary>
        public string description { get; set; }
    }

    [fsObject]
    public class DeletedResponse
    {
        public string deleted { get; set; }
    }

    [fsObject(Converter = typeof(ParametersConverter))]
    public class Parameters
    {
        /// <summary>
        /// The plain text to analyze
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// The HTML file to analyze
        /// </summary>
        public string html { get; set; }
        /// <summary>
        /// The web page to analyze
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// Specific features to analyze the document for
        /// </summary>
        public Features features { get; set; }
        /// <summary>
        /// Remove website elements, such as links, ads, etc
        /// </summary>
        public bool? clean { get; set; }
        /// <summary>
        /// XPath query for targeting nodes in HTML
        /// </summary>
        public string xpath { get; set; }
        /// <summary>
        /// Whether to use raw HTML content if text cleaning fails
        /// </summary>
        public bool? fallback_to_raw { get; set; }
        /// <summary>
        /// Whether or not to return the analyzed text
        /// </summary>
        public bool? return_analyzed_text { get; set; }
        /// <summary>
        /// ISO 639-1 code indicating the language to use in the analysis
        /// </summary>
        public string language { get; set; }
    }

    [fsObject(Converter = typeof(FeaturesConverter))]
    public class Features
    {
        /// <summary>
        /// Whether or not to return the concepts that are mentioned in the analyzed text
        /// </summary>
        public ConceptsOptions concepts { get; set; }
        /// <summary>
        /// Whether or not to extract the emotions implied in the analyzed text
        /// </summary>
        public EmotionOptions emotion { get; set; }
        /// <summary>
        /// Whether or not to extract detected entity objects from the analyzed text
        /// </summary>
        public EntitiesOptions entities { get; set; }
        /// <summary>
        /// Whether or not to return the keywords in the analyzed text
        /// </summary>
        public KeywordsOptions keywords { get; set; }
        /// <summary>
        /// Whether or not the author, publication date, and title of the analyzed text should be returned.This parameter is only available for URL and HTML input
        /// </summary>
        public MetadataOptions metadata { get; set; }
        /// <summary>
        /// Whether or not to return the relationships between detected entities in the analyzed text
        /// </summary>
        public RelationsOptions relations { get; set; }
        /// <summary>
        /// Whether or not to return the subject-action-object relations from the analyzed text
        /// </summary>
        public SemanticRolesOptions semantic_roles { get; set; }
        /// <summary>
        /// Whether or not to return the overall sentiment of the analyzed text
        /// </summary>
        public SentimentOptions sentiment { get; set; }
        /// <summary>
        /// Whether or not to return the high level category the content is categorized as (i.e.news, art)
        /// </summary>
        public CategoriesOptions categories { get; set; }
    }

    [fsObject]
    public class ConceptsOptions
    {
        /// <summary>
        /// Maximum number of concepts to return
        /// </summary>
        public int limit { get; set; }
    }

    [fsObject(Converter = typeof(EmotionOptionsConverter))]
    public class EmotionOptions
    {
        /// <summary>
        /// Show document-level emotion results.
        /// </summary>
        public bool? document { get; set; }
        /// <summary>
        /// Emotion results will be returned for each target string that is found in the document
        /// </summary>
        public string[] targets { get; set; }
    }

    public class EmotionOptionsConverter : fsConverter
    {
        private static fsSerializer sm_Serializer = new fsSerializer();

        public override bool CanProcess(Type type)
        {
            return type == typeof(EmotionOptions);
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            throw new NotImplementedException();
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            EmotionOptions emotionOptions = (EmotionOptions)instance;
            serialized = null;

            Dictionary<string, fsData> serialization = new Dictionary<string, fsData>();

            fsData tempData = null;

            if (emotionOptions.document != null)
            {
                sm_Serializer.TrySerialize(emotionOptions.document, out tempData);
                serialization.Add("document", tempData);
            }

            if (emotionOptions.targets != null)
            {
                sm_Serializer.TrySerialize(emotionOptions.targets, out tempData);
                serialization.Add("targets", tempData);
            }

            serialized = new fsData(serialization);

            return fsResult.Success;
        }
    }

    [fsObject]
    public class EntitiesOptions
    {
        /// <summary>
        /// Maximum number of entities to return
        /// </summary>
        public int limit { get; set; }
        /// <summary>
        /// Enter a custom model ID to override the standard entity detection model
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// Set this to true to return sentiment information for detected entities
        /// </summary>
        public bool sentiment { get; set; }
        /// <summary>
        /// Set this to true to analyze emotion for detected keywords
        /// </summary>
        public bool emotion { get; set; }
    }

    [fsObject]
    public class KeywordsOptions
    {
        /// <summary>
        /// Maximum number of keywords to return
        /// </summary>
        public int limit { get; set; }
        /// <summary>
        /// Set this to true to return sentiment information for detected keywords
        /// </summary>
        public bool sentiment { get; set; }
        /// <summary>
        /// Set this to true to analyze emotion for detected keywords
        /// </summary>
        public bool emotion { get; set; }
    }

    [fsObject]
    public class MetadataOptions { }

    [fsObject]
    public class RelationsOptions
    {
        /// <summary>
        /// Enter a custom model ID to override the default model
        /// </summary>
        public string model { get; set; }
    }

    [fsObject]
    public class SemanticRolesOptions
    {
        /// <summary>
        /// Maximum number of semantic_roles results to return
        /// </summary>
        public int limit { get; set; }
        /// <summary>
        /// Set this to true to return keyword information for subjects and objects
        /// </summary>
        public bool keywords { get; set; }
        /// <summary>
        /// Set this to true to return entity information for subjects and objects
        /// </summary>
        public bool entities { get; set; }
    }

    [fsObject(Converter = typeof(SentimentOptionsConverter))]
    public class SentimentOptions
    {
        /// <summary>
        /// Show document-level sentiment result.
        /// </summary>
        public bool? document { get; set; }
        /// <summary>
        /// Sentiment results will be returned for each target string that is found in the document
        /// </summary>
        public string[] targets { get; set; }
    }

    public class SentimentOptionsConverter : fsConverter
    {
        private static fsSerializer sm_Serializer = new fsSerializer();

        public override bool CanProcess(Type type)
        {
            return type == typeof(SentimentOptions);
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            throw new NotImplementedException();
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            SentimentOptions sentimentOptions = (SentimentOptions)instance;
            serialized = null;

            Dictionary<string, fsData> serialization = new Dictionary<string, fsData>();

            fsData tempData = null;

            if (sentimentOptions.document != null)
            {
                sm_Serializer.TrySerialize(sentimentOptions.document, out tempData);
                serialization.Add("document", tempData);
            }

            if (sentimentOptions.targets != null)
            {
                sm_Serializer.TrySerialize(sentimentOptions.targets, out tempData);
                serialization.Add("targets", tempData);
            }

            serialized = new fsData(serialization);

            return fsResult.Success;
        }
    }

    [fsObject]
    public class CategoriesOptions { }

    #region Version
    /// <summary>
    /// The Discovery version.
    /// </summary>
    public class NaturalLanguageUnderstandingVersion
    {
        /// <summary>
        /// The version.
        /// </summary>
        public const string Version = "2017-02-27";
    }
    #endregion

    #region Parameters Converter
    public class ParametersConverter : fsConverter
    {
        private fsSerializer _serializer = new fsSerializer();

        public override bool CanProcess(Type type)
        {
            return type == typeof(Parameters);
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            throw new NotImplementedException();
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            Parameters parameters = (Parameters)instance;
            serialized = null;

            Dictionary<string, fsData> serialization = new Dictionary<string, fsData>();
            if (parameters.text != null)
                serialization.Add("text", new fsData(parameters.text));

            if (parameters.url != null)
                serialization.Add("url", new fsData(parameters.url));

            if (parameters.html != null)
                serialization.Add("html", new fsData(parameters.html));

            if (parameters.clean != null)
                serialization.Add("clean", new fsData((bool)parameters.clean));

            if (parameters.xpath != null)
                serialization.Add("xpath", new fsData(parameters.xpath));

            if (parameters.fallback_to_raw != null)
                serialization.Add("fallback_to_raw", new fsData((bool)parameters.fallback_to_raw));

            if (parameters.return_analyzed_text != null)
                serialization.Add("return_analyzed_text", new fsData((bool)parameters.return_analyzed_text));

            if (parameters.xpath != null)
                serialization.Add("xpath", new fsData(parameters.xpath));

            fsData tempData = null;
            _serializer.TrySerialize(parameters.features, out tempData);
            serialization.Add("features", tempData);

            serialized = new fsData(serialization);

            return fsResult.Success;
        }
    }
    #endregion

    #region Features Converter
    public class FeaturesConverter : fsConverter
    {
        private fsSerializer _serializer = new fsSerializer();

        public override bool CanProcess(Type type)
        {
            return type == typeof(Parameters);
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            throw new NotImplementedException();
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            Features features = (Features)instance;
            serialized = null;

            Dictionary<string, fsData> serialization = new Dictionary<string, fsData>();

            fsData tempData = null;

            if (features.concepts != null)
            {
                _serializer.TrySerialize(features.concepts, out tempData);
                serialization.Add("concepts", tempData);
            }

            if (features.emotion != null)
            {
                _serializer.TrySerialize(features.emotion, out tempData);
                serialization.Add("emotion", tempData);
            }

            if (features.entities != null)
            {
                _serializer.TrySerialize(features.entities, out tempData);
                serialization.Add("entities", tempData);
            }

            if (features.keywords != null)
            {
                _serializer.TrySerialize(features.keywords, out tempData);
                serialization.Add("keywords", tempData);
            }

            if (features.metadata != null)
            {
                _serializer.TrySerialize(features.metadata, out tempData);
                serialization.Add("metadata", tempData);
            }

            if (features.relations != null)
            {
                _serializer.TrySerialize(features.relations, out tempData);
                serialization.Add("relations", tempData);
            }

            if (features.semantic_roles != null)
            {
                _serializer.TrySerialize(features.semantic_roles, out tempData);
                serialization.Add("semantic_roles", tempData);
            }

            if (features.sentiment != null)
            {
                _serializer.TrySerialize(features.sentiment, out tempData);
                serialization.Add("sentiment", tempData);
            }

            if (features.categories != null)
            {
                _serializer.TrySerialize(features.categories, out tempData);
                serialization.Add("categories", tempData);
            }

            serialized = new fsData(serialization);

            return fsResult.Success;
        }
    }
    #endregion
}
