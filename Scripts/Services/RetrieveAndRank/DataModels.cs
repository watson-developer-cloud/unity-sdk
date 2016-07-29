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

namespace IBM.Watson.DeveloperCloud.Services.RetrieveAndRank.v1
{
    #region Solr Clusters
    /// <summary>
    /// Array of Solr Clusters.
    /// </summary>
    [fsObject]
    public class SolrClusterListResponse
    {
        /// <summary>
        /// An array of [clusters] that are available for the service instance.
        /// </summary>
        public SolrClusterResponse[] clusters { get; set; }
    }

    /// <summary>
    /// Solr cluster object.
    /// </summary>
    [fsObject]
    public class SolrClusterResponse
    {
        /// <summary>
        /// Unique identifier for this cluster.
        /// </summary>
        public string solr_cluster_id { get; set; }
        /// <summary>
        /// Name that identifies the cluster.
        /// </summary>
        public string cluster_name { get; set; }
        /// <summary>
        /// Size of the cluster to create.
        /// </summary>
        public string cluster_size { get; set; }
        /// <summary>
        /// The state of the cluster: NOT_AVAILABLE or READY.
        /// </summary>
        public string solr_cluster_status { get; set; }
    }

    /// <summary>
    /// Array of Solr Configs.
    /// </summary>
    [fsObject]
    public class SolrConfigList
    {
        /// <summary>
        /// The Solr configs.
        /// </summary>
        public string[] solr_configs { get; set; }
    }

    /// <summary>
    /// The error response for deleting Solr clusters.
    /// </summary>
    [fsObject]
    public class ErrorResponsePayload
    {
        /// <summary>
        /// The error message.
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// The error code.
        /// </summary>
        public int code { get; set; }
    }

    /// <summary>
    /// The response for uploading Solr config.
    /// </summary>
    [fsObject]
    public class UploadResponse
    {
        /// <summary>
        /// Status message.
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// Status code.
        /// </summary>
        public string statusCode { get; set; }
    }

    /// <summary>
    /// The response for deleting Solr cluster.
    /// </summary>
    [fsObject]
    public class DeleteResponse
    {
        /// <summary>
        /// Status message.
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// Status code.
        /// </summary>
        public string statusCode { get; set; }
    }

    /// <summary>
    /// The response for deleting Solr cluster config.
    /// </summary>
    [fsObject]
    public class DeleteConfigResponse
    {
        /// <summary>
        /// Status message.
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// Status code.
        /// </summary>
        public string statusCode { get; set; }
    }

    /// <summary>
    /// The error response for deleting Solr configs.
    /// </summary>
    [fsObject]
    public class MessageResponsePayload
    {
        /// <summary>
        /// The error message.
        /// </summary>
        public string message { get; set; }
    }

    /// <summary>
    /// Response for listing collections.
    /// </summary>
    [fsObject]
    public class CollectionsResponse
    {
        /// <summary>
        /// The response header.
        /// </summary>
        public ResponseHeader responseHeader { get; set; }
        /// <summary>
        /// Array of collection names.
        /// </summary>
        public string[] collections { get; set; }
        /// <summary>
        /// Array of CollectionsResponses for each core.
        /// </summary>
        public CollectionsResponse[] response { get; set; }
        /// <summary>
        /// The core name.
        /// </summary>
        public string core { get; set; }
    }

    /// <summary>
    /// Response header for collections actions.
    /// </summary>
    [fsObject]
    public class ResponseHeader
    {
        /// <summary>
        /// The response status.
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// The response QTime.
        /// </summary>
        public int QTime { get; set; }
        /// <summary>
        /// Params for Solr query
        /// </summary>
        [fsProperty("params")]
        public QueryParams _params { get; set; }
    }

    /// <summary>
    /// Collection actions for CollectionsRequest
    /// </summary>
    public class CollectionsAction
    {
        /// <summary>
        /// List collections.
        /// </summary>
        public const string LIST = "LIST";
        /// <summary>
        /// Create a collection.
        /// </summary>
        public const string CREATE = "CREATE";
        /// <summary>
        /// Delete a collection.
        /// </summary>
        public const string DELETE = "DELETE";
    }

    /// <summary>
    /// The response for indexing documents.
    /// </summary>
    [fsObject]
    public class IndexResponse
    {
        /// <summary>
        /// The response header.
        /// </summary>
        public ResponseHeader responseHeader { get; set; }
    }

	/// <summary>
	/// The query parameters.
	/// </summary>
    [fsObject]
    public class QueryParams
    {
        /// <summary>
        /// The query.
        /// </summary>
        public string q { get; set; }
        /// <summary>
        /// The filters.
        /// </summary>
        public string fl { get; set; }
        /// <summary>
        /// The writer type.
        /// </summary>
        public string wt { get; set; }
    }

    /// <summary>
    /// The search response
    /// </summary>
    [fsObject]
    public class SearchResponse
    {
        /// <summary>
        /// The response header info.
        /// </summary>
        public ResponseHeader responseHeader { get; set; }
        /// <summary>
        /// The response.
        /// </summary>
        public Response response { get; set; }
    }

    /// <summary>
    /// The response object.
    /// </summary>
    [fsObject]
    public class Response
    {
        /// <summary>
        /// Number of results found.
        /// </summary>
        public int numFound { get; set; }
        /// <summary>
        /// Start index.
        /// </summary>
        public int start { get; set; }
        /// <summary>
        /// Array of result documents.
        /// </summary>
        public Doc[] docs { get; set; }
    }

    /// <summary>
    /// The doucment object.
    /// </summary>
    [fsObject]
    public class Doc
    {
        /// <summary>
        /// The document identifier.
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// The document body.
        /// </summary>
        public string[] body { get; set; }
        /// <summary>
        /// The doucument title.
        /// </summary>
        public string[] title { get; set; }
        /// <summary>
        /// The document author.
        /// </summary>
        public string[] author { get; set; }
        /// <summary>
        /// The bibliography info.
        /// </summary>
        public string[] bibliography { get; set; }
    }

    /// <summary>
    /// Cluster object containing it's associated configs and collections.
    /// </summary>
    public class ClusterInfo
    {
        /// <summary>
        /// The Cluster's info.
        /// </summary>
        public SolrClusterResponse Cluster { get; set; }
        /// <summary>
        /// Cluster's configs.
        /// </summary>
        public string[] Configs { get; set; }
        /// <summary>
        /// Cluster's collections.
        /// </summary>
        public string[] Collections { get; set; }
    }
    #endregion

    #region Rankers
    /// <summary>
    /// Array of Rankers.
    /// </summary>
    [fsObject]
    public class ListRankersPayload
    {
        /// <summary>
        /// An array of [rankers] that available for the service instance (http://www.ibm.com/watson/developercloud/retrieve-and-rank/api/v1/#rankerinfopayload).
        /// </summary>
        public RankerInfoPayload[] rankers { get; set; }
    }

    /// <summary>
    /// The Ranker object.
    /// </summary>
    [fsObject]
    public class RankerInfoPayload
    {
        /// <summary>
        /// Unique identifier for this ranker
        /// </summary>
        public string ranker_id { get; set; }
        /// <summary>
        /// Link to the ranker
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// User-supplied name for the ranker
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Date and time in Coordinated Universal Time that the ranker was created
        /// </summary>
        public string created { get; set; }
    }

    /// <summary>
    /// The Ranker status object.
    /// </summary>
    [fsObject]
    public class RankerStatusPayload
    {
        /// <summary>
        /// Unique identifier for this ranker.
        /// </summary>
        public string ranker_id { get; set; }
        /// <summary>
        /// Link to the ranker.
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// User-supplied name for the ranker.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Date and time in Coordinated Universal Time that the ranker was created.
        /// </summary>
        public string created { get; set; }
        /// <summary>
        /// The state of the ranker: Non_Existent, Training, Failed, Available, or Unavailable.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// Additional detail about the status.
        /// </summary>
        public string status_description { get; set; }
    }

    /// <summary>
    /// Additionial Ranker training metadata.
    /// </summary>
    [fsObject]
    public class RankerTrainingMetadataPayload
    {
        /// <summary>
        /// The Ranker name.
        /// </summary>
        public string name { get; set; }
    }

    /// <summary>
    /// Ranker error response.
    /// </summary>
    [fsObject]
    public class RankerErrorResponsePayload
    {
        /// <summary>
        /// The error code.
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// The error string.
        /// </summary>
        public string error { get; set; }
        /// <summary>
        /// The error description.
        /// </summary>
        public string description { get; set; }
    }

    /// <summary>
    /// Ranker answer output
    /// </summary>
    [fsObject]
    public class RankerOutputPayload
    {
        /// <summary>
        /// Unique identifier for this ranker.
        /// </summary>
        public string ranker_id { get; set; }
        /// <summary>
        /// Link to the ranker.
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// The answer with the highest score.
        /// </summary>
        public string top_answer { get; set; }
        /// <summary>
        /// An array of of up to 10 answers that are sorted in descending order of score.
        /// </summary>
        public RankedAnswer[] answers { get; set; }
    }
    
    /// <summary>
    /// The ranked answer object.
    /// </summary>
    [fsObject]
    public class RankedAnswer
    {
        /// <summary>
        /// Pointer to the answer in the collection.
        /// </summary>
        public string answer_id { get; set; }
        /// <summary>
        /// The rank of an answer among the candidate answers. Higher values represent higher relevance. The maximum score is the total number of candidate answers in the answer_data. You can use the score to sort the answers within the response.
        /// </summary>
        public int score { get; set; }
        /// <summary>
        /// A decimal percentage from 0 - 1 that represents the preference that Watson has for this answer. Higher values represent higher confidences.
        /// </summary>
        public double confidence { get; set; }
    }
    #endregion
}
