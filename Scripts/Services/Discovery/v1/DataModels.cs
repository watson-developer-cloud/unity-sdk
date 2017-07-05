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

using FullSerializer;
using System.Text;

namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    #region Environments
    /// <summary>
    /// Get Environments response.
    /// </summary>
    [fsObject]
    public class GetEnvironmentsResponse
    {
        /// <summary>
        /// An array of [environments] that are available for the service instance.
        /// </summary>
        public Environment[] environments { get; set; }
    }

    /// <summary>
    /// An environment.
    /// </summary>
    [fsObject]
    public class Environment
    {
        /// <summary>
        /// Unique identifier for this environment.
        /// </summary>
        public string environment_id { get; set; }
        /// <summary>
        /// Name that identifies the environment.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Description of the environment, if available.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// Creation date of the environment, in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        public string created { get; set; }
        /// <summary>
        /// Date of most recent environment update, in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        public string updated { get; set; }
        /// <summary>
        /// Status of the environment.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// Weather or not the environment is read only and maintained by IBM.
        /// </summary>
        public bool read_only { get; set; }
        /// <summary>
        /// Disk and memory usage.
        /// </summary>
        public IndexCapacity index_capacity { get; set; }
    }

    /// <summary>
    /// The disk and memory usage.
    /// </summary>
    [fsObject]
    public class IndexCapacity
    {
        /// <summary>
        /// Summary of disk-usage statistics for the environment.
        /// </summary>
        public DiskUsage disk_usage { get; set; }
        /// <summary>
        /// The memory usage.
        /// </summary>
        public MemoryUsage memory_usage { get; set; }
    }

    /// <summary>
    /// Summary of disk-usage statistics for the environment.
    /// </summary>
    [fsObject]
    public class DiskUsage
    {
        /// <summary>
        /// Number of bytes used on the environment's disk capacity.
        /// </summary>
        public long used_bytes { get; set; }
        /// <summary>
        /// Total number of bytes available in the environment's disk capacity.
        /// </summary>
        public long total_bytes { get; set; }
        /// <summary>
        /// Amount of disk capacity used, in KB or GB format.
        /// </summary>
        public string used { get; set; }
        /// <summary>
        /// Total amount of the environment's disk capacity, in KB or GB format.
        /// </summary>
        public string total { get; set; }
        /// <summary>
        /// Percentage of the environment's disk capacity that is being used.
        /// </summary>
        public double percent_used { get; set; }
    }

    /// <summary>
    /// Summary of memory-usage statistics for the environment.
    /// </summary>
    [fsObject]
    public class MemoryUsage
    {
        /// <summary>
        /// Number of bytes used in the environment's memory capacity.
        /// </summary>
        public long used_bytes { get; set; }
        /// <summary>
        /// Total number of bytes available in the environment's memory capacity.
        /// </summary>
        public long total_bytes { get; set; }
        /// <summary>
        /// Amount of memory capacity used, in KB or GB format.
        /// </summary>
        public string used { get; set; }
        /// <summary>
        /// Total amount of the environment's memory capacity, in KB or GB format.
        /// </summary>
        public string total { get; set; }
        /// <summary>
        /// Percentage of the environment's memory capacity that is being used.
        /// </summary>
        public double percent_used { get; set; }
    }

    /// <summary>
    /// The environment's unique identifier and its status. A status of deleted indicates that 
    /// the environment has been successfully deleted.
    /// </summary>
    [fsObject]
    public class DeleteEnvironmentResponse
    {
        /// <summary>
        /// Unique identifier for this environment.
        /// </summary>
        public string environment_id { get; set; }
        /// <summary>
        /// Status of the environment.
        /// </summary>
        public string status { get; set; }
    }
    #endregion

    #region Test Configuration
    /// <summary>
    /// Detailed information about the preview run.
    /// </summary>
    [fsObject]
    public class TestDocument
    {
        /// <summary>
        /// Unique identifier for the configuration.
        /// </summary>
        public string configuration_id { get; set; }
        /// <summary>
        /// Status of the preview operation.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// Number of 10-kB units of field data enrichments that were enriched. This can be used to estimate 
        /// the cost of ingesting the document.
        /// </summary>
        public double enriched_field_units { get; set; }
        /// <summary>
        /// Format of the test document; for example, text/html.
        /// </summary>
        public string original_media_type { get; set; }
        /// <summary>
        /// An array of notice messages about the preview operation.
        /// </summary>
        public Notice[] notices { get; set; }
    }

    /// <summary>
    /// An array of JSON strings that describe each step in the preview process.
    /// </summary>
    [fsObject]
    public class DocumentSnapshot
    {
        /// <summary>
        /// The step.
        /// </summary>
        public string step { get; set; }
        /// <summary>
        /// An array of arbitrary JSON strings that describe the step and its results.
        /// </summary>
        public object snapshot { get; set; }
    }

    /// <summary>
    /// Notice messages about the preview operation.
    /// </summary>
    [fsObject]
    public class Notice
    {
        /// <summary>
        /// Text ID of the event notice.
        /// </summary>
        public string notice_id { get; set; }
        /// <summary>
        /// Creation timestamp of the notice, in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        public string created { get; set; }
        /// <summary>
        /// ID of the document in which the event notice occurred.
        /// </summary>
        public string document_id { get; set; }
        /// <summary>
        /// Severity of the notice. Possible values warning and error.
        /// </summary>
        public string severity { get; set; }
        /// <summary>
        /// Step in the preview operation in which the event occurred.
        /// </summary>
        public string step { get; set; }
        /// <summary>
        /// Description of the notice.
        /// </summary>
        public string description { get; set; }
    }
    #endregion

    #region Configurations
    /// <summary>
    /// An array that lists each configuration's ID, name, description, creation date, and date of 
    /// last update.
    /// </summary>
    [fsObject]
    public class GetConfigurationsResponse
    {
        /// <summary>
        /// An array of [configurations] that are available for the service instance.
        /// </summary>
        public ConfigurationRef[] configurations { get; set; }
    }

    /// <summary>
    /// A list of information about the configuration.
    /// </summary>
    [fsObject]
    public class ConfigurationRef
    {
        /// <summary>
        /// Unique identifier for this configuration.
        /// </summary>
        public string configuration_id { get; set; }
        /// <summary>
        /// Creation date of the configuration, in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        public string created { get; set; }
        /// <summary>
        /// Date of most recent configuration update, in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        public string updated { get; set; }
        /// <summary>
        /// Name that identifies the configuration.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Description of the configuration, if available.
        /// </summary>
        public string description { get; set; }
    }

    /// <summary>
    /// A list of information about the configuration.
    /// </summary>
    [fsObject]
    public class Configuration
    {
        /// <summary>
        /// Unique identifier for this configuration.
        /// </summary>
        public string configuration_id { get; set; }
        /// <summary>
        /// Name that identifies the configuration.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Creation date of the configuration, in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        public string created { get; set; }
        /// <summary>
        /// Date of most recent configuration update, in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        public string updated { get; set; }
        /// <summary>
        /// Description of the configuration, if available.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// A list of the configuration's document conversion settings.
        /// </summary>
        public Conversions conversions { get; set; }
        /// <summary>
        /// An array describing the configuration's document enrichment settings.
        /// </summary>
        public Enrichment[] enrichments { get; set; }
        /// <summary>
        /// An array describing the configuration's document normalization settings.
        /// </summary>
        public NormalizationOperation[] normalizations { get; set; }
    }

    /// <summary>
    /// The configuration's document conversion settings.
    /// </summary>
    [fsObject]
    public class Conversions
    {
        /// <summary>
        /// A list of PDF conversion settings, including the conversions applied to different types of 
        /// headings as defined by font attributes.
        /// </summary>
        public PdfSettings pdf { get; set; }
        /// <summary>
        /// A list of Word conversion settings, including the conversions applied to different types of 
        /// headings as defined by font attributes and to different formatting styles of text.
        /// </summary>
        public WordSettings word { get; set; }
        /// <summary>
        /// A list of HTML conversion settings, including tags that are to be excluded completely; tags 
        /// that are to be discarded but their content kept; content that is to be excluded as defined 
        /// by xpaths; content that is to be kept as defined by xpaths; and tag attributes that are to be excluded.
        /// </summary>
        public HtmlSettings html { get; set; }
        /// <summary>
        /// An array of JSON normalization operations, including one or more of the following: 
        /// 
        /// copy — Copies the value of the source_field to the destination_field. If the destination_field already 
        /// exists, the value of the source_field overwrites the original value of the destination_field. 
        /// 
        /// move — Renames (moves) the source_field to the destination_field.If the destination_field already exists,
        /// the value of the source_field overwrites the original value of the destination_field. Rename is 
        /// identical to copy, except that the source_field is removed after the value has been copied to the 
        /// destination_field. It is the same as a copy followed by a remove. 
        /// 
        /// merge — Merges the value of the source_field with the value of the destination_field. The 
        /// destination_field is converted into an array if it is not already an array, and the value of the 
        /// source_field is appended to the array. This operation removes the source_field after the merge. If the 
        /// source_field does not exist in the current document, the destination_field is converted into an array if 
        /// it is not already an array. This is ensures the type for destination_field is consistent across all 
        /// documents. 
        /// 
        /// remove — Deletes the source_field. The destination_field is ignored for this operation. 
        /// 
        /// remove_nulls — Removes all nested null (blank) leaf values from the JSON tree. The source_field and 
        /// destination_field are ignored by this operation because remove_nulls operates on the entire JSON tree.
        /// Typically, remove_nulls is invoked as the last normalization operation; if it is invoked, it can be 
        /// time-expensive. The array also lists the source_field and destination_field for each operation. If no 
        /// JSON normalization operations are specified, the method returns an empty array.
        /// </summary>
        public NormalizationOperation[] json_normalizations { get; set; }
    }

    /// <summary>
    /// A list of PDF conversion settings, including the conversions applied to different types of headings as 
    /// defined by font attributes.
    /// </summary>
    [fsObject]
    public class PdfSettings
    {
        /// <summary>
        /// PDF heading Detection
        /// </summary>
        public PdfHeadingDetection heading { get; set; }
    }

    /// <summary>
    /// A list of Word conversion settings, including the conversions applied to different types of headings as 
    /// defined by font attributes and to different formatting styles of text.
    /// </summary>
    [fsObject]
    public class WordSettings
    {
        /// <summary>
        /// Word heading detection.
        /// </summary>
        public WordHeadingDetection heading { get; set; }
    }

    /// <summary>
    /// A list of HTML conversion settings, including tags that are to be excluded completely; tags that are to be 
    /// discarded but their content kept; content that is to be excluded as defined by xpaths; content that is to 
    /// be kept as defined by xpaths; and tag attributes that are to be excluded.
    /// </summary>
    [fsObject]
    public class HtmlSettings
    {
        /// <summary>
        /// Exclude tags completely.
        /// </summary>
        public string[] exclude_tags_completely { get; set; }
        /// <summary>
        /// Exclude tags, keep content.
        /// </summary>
        public string[] exclude_tags_keep_content { get; set; }
        /// <summary>
        /// Keep content.
        /// </summary>
        public XPathPatterns keep_content { get; set; }
        /// <summary>
        /// Exclude content.
        /// </summary>
        public XPathPatterns exclude_content { get; set; }
        /// <summary>
        /// Keep tag attributes.
        /// </summary>
        public string[] keep_tag_attributes { get; set; }
        /// <summary>
        /// Exclude tag attributes.
        /// </summary>
        public string[] exclude_tag_attributes { get; set; }
    }

    /// <summary>
    /// The configuration's document enrichment settings.
    /// </summary>
    [fsObject]
    public class Enrichment
    {
        /// <summary>
        /// Describes what the enrichment step does.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// Field where enrichments will be stored. This field must already exist or be at most 1 level deeper 
        /// than an existing field. For example, if text is a top-level field with no sub-fields, text.foo is a 
        /// valid destination but text.foo.bar is not.
        /// </summary>
        public string destination_field { get; set; }
        /// <summary>
        /// Field to be enriched.
        /// </summary>
        public string source_field { get; set; }
        /// <summary>
        /// Indicates that the enrichments will overwrite the destination_field field if it already exists.
        /// </summary>
        public bool overwrite { get; set; }
        /// <summary>
        /// Name of the enrichment service to call. Currently the only valid value is alchemy_language.
        /// </summary>
        public string enrichment { get; set; }
        /// <summary>
        /// If true, then most errors generated during the enrichment process will be treated as warnings and 
        /// will not cause the document to fail processing.
        /// </summary>
        public bool ignore_downstream_errors { get; set; }
        /// <summary>
        /// Enrichment options.
        /// </summary>
        public EnrichmentOptions options { get; set; }
    }

    /// <summary>
    ///  The configuration's document normalization settings.
    /// </summary>
    [fsObject]
    public class NormalizationOperation
    {
        /// <summary>
        /// Identifies what type of operation to perform.
        /// 
        /// copy - Copies the value of the source_field to the destination_field field. If the destination_field 
        /// already exists, then the value of the source_field overwrites the original value of the destination_field.
        /// 
        /// move - Renames(moves) the source_field to the destination_field. If the destination_field already exists, 
        /// then the value of the source_field overwrites the original value of the destination_field. Rename is 
        /// identical to copy, except that the source_field is removed after the value has been copied to the 
        /// destination_field(it is the same as a copy followed by a remove).
        /// 
        /// merge - Merges the value of the source_field with the value of the destination_field. The 
        /// destination_field is converted into an array if it is not already an array, and the value of the 
        /// source_field is appended to the array. This operation removes the source_field after the merge. If the 
        /// source_field does not exist in the current document, then the destination_field is still converted into 
        /// an array (if it is not an array already). This is ensures the type for destination_field is consistent 
        /// across all documents.
        /// 
        /// remove - Deletes the source_field field. The destination_field is ignored for this operation.
        /// 
        /// remove_nulls - Removes all nested null (blank) leif values from the JSON tree. source_field and 
        /// destination_field are ignored by this operation because remove_nulls operates on the entire JSON tree. 
        /// Typically, remove_nulls is invoked as the last normalization operation (if it is inoked at all, it can 
        /// be time-expensive). = ['copy', 'move', 'merge', 'remove', 'remove_nulls']
        /// string Enum: "copy", "move", "merge", "remove", "remove_nulls"
        /// </summary>
        public string operation { get; set; }
        /// <summary>
        /// The source field.
        /// </summary>
        public string source_field { get; set; }
        /// <summary>
        /// The destination field.
        /// </summary>
        public string destination_field { get; set; }
    }

    /// <summary>
    /// PDF conversion settings, including the conversions applied to different types of headings as defined by 
    /// font attributes.
    /// </summary>
    [fsObject]
    public class PdfHeadingDetection
    {
        /// <summary>
        /// An array of font settings.
        /// </summary>
        public FontSetting[] fonts { get; set; }
    }

    /// <summary>
    /// Word conversion settings, including the conversions applied to different types of headings as defined by 
    /// font attributes and to different formatting styles of text.
    /// </summary>
    [fsObject]
    public class WordHeadingDetection
    {
        /// <summary>
        /// An array of font settings.
        /// </summary>
        public FontSetting[] fonts { get; set; }
        /// <summary>
        /// An array of word styles.
        /// </summary>
        public WordStyle[] styles { get; set; }
    }

    /// <summary>
    /// The XPath Patterns.
    /// </summary>
    [fsObject]
    public class XPathPatterns
    {
        /// <summary>
        /// An array of xpaths.
        /// </summary>
        public string[] xpaths { get; set; }
    }

    /// <summary>
    /// Document enrichment settings for the configuration.
    /// </summary>
    [fsObject]
    public class EnrichmentOptions
    {
        /// <summary>
        ///  A comma sepeated list of analyses that should be applied when using the alchemy_language enrichment. 
        ///  See the the service documentation for details on each extract option.
        ///  Possible values include: entity, keyword, taxonomy, concept, relation, doc-sentiment, doc-emotion, 
        ///  typed-rels
        /// </summary>
        public string extract { get; set; }
        /// <summary>
        /// Show sentiment.
        /// </summary>
        public bool sentiment { get; set; }
        /// <summary>
        /// Show quotations.
        /// </summary>
        public bool quotations { get; set; }
        /// <summary>
        /// Show source text.
        /// </summary>
        public bool showSouceText { get; set; }
        /// <summary>
        /// Show hierarchical typed relations.
        /// </summary>
        public bool hierarchicalTypedRelations { get; set; }
        /// <summary>
        ///  Required when using the typed-rel extract option. Should be set to the ID of a previously published 
        ///  custom Watson Knowledge Studio model
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// If provided, then do not attempt to detect the language of the input document. Instead, assume the 
        /// language is the one specified in this field. You can set this property to work around 
        /// unsupported-text-language errors. Supported languages include English, German, French, Italian, 
        /// Portuguese, Russian, Spanish and Swedish.Supported language codes are the ISO-639-1, ISO-639-2, 
        /// ISO-639-3, and the plain english name of the language (e.g. "russian"). = ['english', 'german', 
        /// 'french', 'italian', 'portuguese', 'russian', 'spanish', 'swedish', 'en', 'eng', 'de', 'ger', 'deu', 
        /// 'fr', 'fre', 'fra', 'it', 'ita', 'pt', 'por', 'ru', 'rus', 'es', 'spa', 'sv', 'swe']
        /// </summary>
        public string language { get; set; }
    }

    /// <summary>
    /// Description of the font settings.
    /// </summary>
    [fsObject]
    public class FontSetting
    {
        /// <summary>
        /// The font level.
        /// </summary>
        public double level { get; set; }
        /// <summary>
        /// The font minimum size.
        /// </summary>
        public float min_size { get; set; }
        /// <summary>
        /// The font maximum size.
        /// </summary>
        public float max_size { get; set; }
        /// <summary>
        /// The font style bold.
        /// </summary>
        public bool bold { get; set; }
        /// <summary>
        /// The font style italic.
        /// </summary>
        public bool italic { get; set; }
        /// <summary>
        /// The font name.
        /// </summary>
        public string name { get; set; }
    }

    /// <summary>
    /// Description of the word style.
    /// </summary>
    [fsObject]
    public class WordStyle
    {
        /// <summary>
        /// The word level.
        /// </summary>
        public double level { get; set; }
        /// <summary>
        /// An array of the word names.
        /// </summary>
        public string[] names { get; set; }
    }

    /// <summary>
    /// The configuration ID and the status of the delete operation, and a warning if the configuration was 
    /// referenced by anything.
    /// </summary>
    [fsObject]
    public class DeleteConfigurationResponse
    {
        /// <summary>
        /// Unique identifier for the configuration.
        /// </summary>
        public string configuration_id { get; set; }
        /// <summary>
        /// Status of the configuration. A deleted configuration has the status deleted.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// An array of notice messages, if any.
        /// </summary>
        public Notice[] notices { get; set; }
    }
    #endregion

    #region Collections
    /// <summary>
    /// An array that lists each collection's ID, name, configuration ID, language, status, creation date, and date 
    /// of last update.
    /// </summary>
    [fsObject]
    public class GetCollectionsResponse
    {
        /// <summary>
        /// An array containing information about each collection in the environment.
        /// </summary>
        public CollectionRef[] collections { get; set; }
    }

    [fsObject]
    public class CollectionRef
    {
        /// <summary>
        /// The unique identifier of the collection.
        /// </summary>
        public string collection_id { get; set; }
        /// <summary>
        /// The name of the collection.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The description of the collection.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// The creation date of the collection, in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        public string created { get; set; }
        /// <summary>
        /// The timestamp of the most recent update to the collection, in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        public string updated { get; set; }
        /// <summary>
        /// The status of the collection.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// The unique identifier of the configuration in which the collection is located.
        /// </summary>
        public string configuration_id { get; set; }
    }

    /// <summary>
    /// The collection ID and the status of the deletion process.
    /// </summary>
    [fsObject]
    public class DeleteCollectionResponse
    {
        /// <summary>
        /// The unique identifier of the collection that is being deleted.
        /// </summary>
        public string collection_id { get; set; }
        /// <summary>
        /// The status of the collection. The status of a successful deletion operation is deleted.
        /// </summary>
        public string status { get; set; }
    }

    /// <summary>
    /// Details about the specified collection.
    /// </summary>
    [fsObject]
    public class Collection
    {
        /// <summary>
        /// The unique identifier of the collection.
        /// </summary>
        public string collection_id { get; set; }
        /// <summary>
        /// The name of the collection.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The description of the collection.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// The creation date of the collection, in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        public string created { get; set; }
        /// <summary>
        /// The timestamp of the most recent update to the collection, in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        public string updated { get; set; }
        /// <summary>
        /// The collection status.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// The configuration identifier.
        /// </summary>
        public string configuration_id { get; set; }
        /// <summary>
        /// Object providing information about the documents in the collection.
        /// </summary>
        public DocumentCounts document_counts { get; set; }
    }

    /// <summary>
    /// Information about the documents in the collection.
    /// </summary>
    [fsObject]
    public class DocumentCounts
    {
        /// <summary>
        /// The total number of ingested documents in the collection.
        /// </summary>
        public int available { get; set; }
        /// <summary>
        /// The number of documents in the collection that are currently being processed.
        /// </summary>
        public int processing { get; set; }
        /// <summary>
        /// The number of documents in the collection that failed to be ingested.
        /// </summary>
        public int failed { get; set; }
    }

    /// <summary>
    /// List of the unique fields, and each field's type, that are stored in a collection's index.
    /// </summary>
    [fsObject]
    public class GetFieldsResponse
    {
        /// <summary>
        /// An array containing information about each field in the collection.
        /// </summary>
        public Field[] fields { get; set; }
    }

    /// <summary>
    /// Information about each field in the collection.
    /// </summary>
    [fsObject]
    public class Field
    {
        /// <summary>
        /// The name of the field.
        /// </summary>
        public string field { get; set; }
        /// <summary>
        /// The type of the field.
        /// </summary>
        public string type { get; set; }
    }
    #endregion

    #region Documents
    /// <summary>
    /// Document ID and ingestion status.
    /// </summary>
    [fsObject]
    public class DocumentAccepted
    {
        /// <summary>
        /// Unique identifier of the ingested document.
        /// </summary>
        public string document_id { get; set; }
        /// <summary>
        /// Status of the document in the ingestion process.
        /// </summary>
        public string status { get; set; }
    }

    /// <summary>
    /// The delete document response.
    /// </summary>
    [fsObject]
    public class DeleteDocumentResponse
    {
        /// <summary>
        /// Unique identifier of the document.
        /// </summary>
        public string document_id { get; set; }
        /// <summary>
        /// Status of the document. A deleted document has the status deleted.
        /// </summary>
        public string status { get; set; }
    }

    /// <summary>
    /// The document ID, configuration ID, document creation date, date of last document update, submission status,
    /// and notices associated with the document's submission.
    /// </summary>
    [fsObject]
    public class DocumentStatus
    {
        /// <summary>
        /// Unique identifier of the ingested document.
        /// </summary>
        public string document_id { get; set; }
        /// <summary>
        /// Unique identifier of the configuration used to process the document.
        /// </summary>
        public string configuration_id { get; set; }
        /// <summary>
        /// The creation date of the document, in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        public string created { get; set; }
        /// <summary>
        /// The timestamp of the most recent update to the document, in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        public string updated { get; set; }
        /// <summary>
        /// Status of the document in the ingestion process.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// The description of the current document status.
        /// </summary>
        public string status_description { get; set; }
        /// <summary>
        /// Array of notices produced by the document-ingestion process.
        /// </summary>
        public Notice[] notices { get; set; }
    }
    #endregion

    #region Queries
    /// <summary>
    /// The content requested in the query.
    /// </summary>
    [fsObject]
    public class QueryResponse
    {
        /// <summary>
        /// Matching query results,.
        /// </summary>
        public double matching_results { get; set; }
        /// <summary>
        /// Array of query results.
        /// </summary>
        public QueryResult[] results { get; set; }
        /// <summary>
        /// Query aggregations.
        /// </summary>
        public QueryAggregation aggregations { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(matching_results.ToString()))
                stringBuilder.Append(string.Format("matching_results: {0}", matching_results.ToString()));

            foreach (QueryResult result in results)
            {
                if (!string.IsNullOrEmpty(result.id))
                    stringBuilder.Append(string.Format("result id: {0}", result.id) + "\n\t");

                if (!string.IsNullOrEmpty(result.score.ToString()))
                    stringBuilder.Append(string.Format("result score: {0}", result.score) + "\n\t");
            }

            stringBuilder.Append("\n");

            if (aggregations != null && aggregations.term != null && aggregations.term.results != null)
            {
                if (!string.IsNullOrEmpty(aggregations.term.results.key))
                    stringBuilder.Append(string.Format("key: {0}", aggregations.term.results.key));

                if (!string.IsNullOrEmpty(aggregations.term.results.matching_results.ToString()))
                    stringBuilder.Append(string.Format("key: {0}", aggregations.term.results.matching_results.ToString()));
            }

            return string.Format("{0}", stringBuilder.ToString());
        }
    }

    /// <summary>
    /// Query result.
    /// </summary>
    [fsObject]
    public class QueryResult
    {
        /// <summary>
        /// Query result identifier.
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Query result score.
        /// </summary>
        public double score { get; set; }
    }

    /// <summary>
    /// Query aggregation.
    /// </summary>
    [fsObject]
    public class QueryAggregation
    {
        /// <summary>
        /// The aggregation term.
        /// </summary>
        public AggregationTerm term { get; set; }
    }

    /// <summary>
    /// Query aggregation term.
    /// </summary>
    [fsObject]
    public class AggregationTerm
    {
        /// <summary>
        /// The aggregation term result.
        /// </summary>
        public AggregationResult results { get; set; }
    }

    /// <summary>
    /// The aggregation result.
    /// </summary>
    [fsObject]
    public class AggregationResult
    {
        /// <summary>
        /// The aggregation result key.
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// THe aggregation matching results.
        /// </summary>
        public double matching_results { get; set; }
    }
    #endregion
}
