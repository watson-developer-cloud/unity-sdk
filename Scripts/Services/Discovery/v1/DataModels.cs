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

namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    #region Environments
    [fsObject]
    public class GetEnvironmentsResponse
    {
        public Environment[] environments { get; set; }
    }

    [fsObject]
    public class Environment
    {
        public string environment_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string created { get; set; }
        public string updated { get; set; }
        public string status { get; set; }
        public bool read_only { get; set; }
        public double size { get; set; }
        public IndexCapacity index_capacity { get; set; }
    }

    [fsObject]
    public class IndexCapacity
    {
        public DiskUsage disk_usage { get; set; }
        public MemoryUsage memory_usage { get; set; }
    }

    [fsObject]
    public class DiskUsage
    {
        public int used_bytes { get; set; }
        public int total_bytes { get; set; }
        public string used { get; set; }
        public string total { get; set; }
        public double percent_used { get; set; }
    }

    [fsObject]
    public class MemoryUsage
    {
        public int used_bytes { get; set; }
        public int total_bytes { get; set; }
        public string used { get; set; }
        public string total { get; set; }
        public double percent_used { get; set; }
    }

    [fsObject]
    public class DeleteEnvironmentResponse
    {
        public string environment_id { get; set; }
        public string status { get; set; }
    }
    #endregion

    #region Test Configuration
    [fsObject]
    public class TestDocument
    {
        public string configuration_id { get; set; }
        public string status { get; set; }
        public double enriched_field_units { get; set; }
        public string original_media_type { get; set; }
        public Notice[] notices { get; set; }
    }

    [fsObject]
    public class DocumentSnapshot
    {
        public string step { get; set; }
        public object snapshot { get; set; }
    }

    [fsObject]
    public class Notice
    {
        public string notice_id { get; set; }
        public string created { get; set; }
        public string document_id { get; set; }
        public string severity { get; set; }
        public string step { get; set; }
        public string description { get; set; }
    }
    #endregion

    #region Configurations
    [fsObject]
    public class GetConfigurationsResponse
    {
        public ConfigurationRef[] configurations { get; set; }
    }

    [fsObject]
    public class ConfigurationRef
    {
        public string configuration_id { get; set; }
        public string created { get; set; }
        public string updated { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    [fsObject]
    public class Configuration
    {
        public string configuration_id { get; set; }
        public string name { get; set; }
        public string created { get; set; }
        public string updated { get; set; }
        public string description { get; set; }
        public Conversions conversions { get; set; }
        public Enrichment[] enrichments { get; set; }
        public NormalizationOperation[] normalizations { get; set; }
    }

    [fsObject]
    public class Conversions
    {
        public PdfSettings pdf { get; set; }
        public WordSettings word { get; set; }
        public HtmlSettings html { get; set; }
        public NormalizationOperation[] json_normalizations { get; set; }
    }

    [fsObject]
    public class PdfSettings
    {
        public PdfHeadingDetection heading { get; set; }
    }

    [fsObject]
    public class WordSettings
    {
        public WordHeadingDetection heading { get; set; }
    }

    [fsObject]
    public class HtmlSettings
    {
        public string[] exclude_tags_completely { get; set; }
        public string[] exclude_tags_keep_content { get; set; }
        public XPathPatterns keep_content { get; set; }
        public XPathPatterns exclude_content { get; set; }
        public string[] keep_tag_attributes { get; set; }
        public string[] exclude_tag_attributes { get; set; }
    }

    [fsObject]
    public class Enrichment
    {
        public string description { get; set; }
        public string destination_field { get; set; }
        public string source_field { get; set; }
        public bool overwrite { get; set; }
        public string enrichment { get; set; }
        public bool ignore_downstream_errors { get; set; }
        public EnrichmentOptions options { get; set; }
    }

    [fsObject]
    public class NormalizationOperation
    {
        public string operation { get; set; }
        public string source_field { get; set; }
        public string destination_field { get; set; }
    }

    [fsObject]
    public class PdfHeadingDetection
    {
        public FontSetting[] fonts { get; set; }
    }

    [fsObject]
    public class WordHeadingDetection
    {
        public FontSetting[] fonts { get; set; }
        public WordStyle[] styles { get; set; }
    }

    [fsObject]
    public class XPathPatterns
    {
        public string[] xpaths { get; set; }
    }

    [fsObject]
    public class EnrichmentOptions
    {
        public string extract { get; set; }
        public bool sentiment { get; set; }
        public bool quotations { get; set; }
        public bool showSouceText { get; set; }
        public bool hierarchicalTypedRelations { get; set; }
        public string model { get; set; }
        public string language { get; set; }
    }

    [fsObject]
    public class FontSetting
    {
        public double level { get; set; }
        public float min_size { get; set; }
        public float max_size { get; set; }
        public bool bold { get; set; }
        public bool italic { get; set; }
        public string name { get; set; }
    }

    [fsObject]
    public class WordStyle
    {
        public double level { get; set; }
        public string[] names { get; set; }
    }

    [fsObject]
    public class DeleteConfigurationResponse
    {
        public string configuration_id { get; set; }
        public string status { get; set; }
        public Notice[] notices { get; set; }
    }
    #endregion

    #region Collections
    [fsObject]
    public class GetCollectionsResponse
    {
        public CollectionRef[] collections { get; set; }
    }

    [fsObject]
    public class CollectionRef
    {
        public string collection_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string created { get; set; }
        public string updated { get; set; }
        public string status { get; set; }
        public string configuration_id { get; set; }
    }

    [fsObject]
    public class DeleteCollectionResponse
    {
        public string collection_id { get; set; }
        public string status { get; set; }
    }

    [fsObject]
    public class Collection
    {
        public string collection_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string created { get; set; }
        public string updated { get; set; }
        public string status { get; set; }
        public string configuration_id { get; set; }
        public DocumentCounts document_counts { get; set; }
    }

    [fsObject]
    public class DocumentCounts
    {
        public int available { get; set; }
        public int processing { get; set; }
        public int failed { get; set; }
    }

    [fsObject]
    public class GetFieldsResponse
    {
        public Field[] fields { get; set; }
    }

    [fsObject]
    public class Field
    {
        public string field { get; set; }
        public string type { get; set; }
    }
    #endregion

    #region Documents
    [fsObject]
    public class DocumentAccepted
    {
        public string document_id { get; set; }
        public string status { get; set; }
    }

    [fsObject]
    public class DeleteDocumentResponse
    {
        public string document_id { get; set; }
        public string status { get; set; }
    }

    [fsObject]
    public class DocumentStatus
    {
        public string document_id { get; set; }
        public string configuration_id { get; set; }
        public string created { get; set; }
        public string updated { get; set; }
        public string status { get; set; }
        public string status_description { get; set; }
        public Notice[] notices { get; set; }
    }
    #endregion

    #region Queries
    [fsObject]
    public class QueryResponse
    {
        public double matching_results { get; set; }
        public QueryResult[] results { get; set; }
        public QueryAggregation aggregations { get; set; }
    }

    [fsObject]
    public class QueryResult
    {
        public string id { get; set; }
        public double score { get; set; }
    }

    [fsObject]
    public class QueryAggregation
    {
        public AggregationTerm term { get; set; }
    }

    [fsObject]
    public class AggregationTerm
    {
        public AggregationResult results { get; set; }
    }

    [fsObject]
    public class AggregationResult
    {
        public string key { get; set; }
        public double matching_results { get; set; }
    }
    #endregion
}
