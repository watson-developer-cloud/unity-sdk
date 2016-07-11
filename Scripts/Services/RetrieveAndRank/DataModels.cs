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
        /// The Solr clusters.
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
        /// The cluster id.
        /// </summary>
        public string solr_cluster_id { get; set; }
        /// <summary>
        /// The cluster name.
        /// </summary>
        public string cluster_name { get; set; }
        /// <summary>
        /// The cluster size.
        /// </summary>
        public string cluster_size { get; set; }
        /// <summary>
        /// The cluster status.
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
    }

    /// <summary>
    /// Collection actions for CollectionsRequest
    /// </summary>
    public class CollectionsAction
    {
        public const string LIST = "LIST";
        public const string CREATE = "CREATE";
        public const string DELETE = "DELETE";
    }

    /// <summary>
    /// The response for indexing documents.
    /// </summary>
    [fsObject]
    public class IndexResponse
    {
        public ResponseHeader responseHeader { get; set; }
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
        /// The Ranker.
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
        /// The Ranker ID.
        /// </summary>
        public string ranker_id { get; set; }
        /// <summary>
        /// The Ranker URL.
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// The Ranker name.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The Ranker created date (UTC).
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
        /// The Ranker ID.
        /// </summary>
        public string ranker_id { get; set; }
        /// <summary>
        /// The Ranker URL.
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// The Ranker name.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The Ranker created date (UTC).
        /// </summary>
        public string created { get; set; }
        /// <summary>
        /// The Ranker status
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// The Ranker status description
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
        /// The Ranker ID.
        /// </summary>
        public string ranker_id { get; set; }
        /// <summary>
        /// The Ranker name.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The Ranker URL.
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// The Ranker top answer.
        /// </summary>
        public string top_answer { get; set; }
        /// <summary>
        /// The Ranker answers.
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
        /// The Answer ID.
        /// </summary>
        public string answer_id { get; set; }
        /// <summary>
        /// The Answer score.
        /// </summary>
        public string score { get; set; }
        /// <summary>
        /// The Answer confidence.
        /// </summary>
        public string confidence { get; set; }
    }
    #endregion
}
