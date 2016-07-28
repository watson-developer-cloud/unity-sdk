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
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using System;
using FullSerializer;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.Services.RetrieveAndRank.v1
{
	/// <summary>
	/// This class wraps the Retrieve and Rank service.
	/// <a href="http://www.ibm.com/watson/developercloud/retrieve-rank.html">Retrieve and Rank Service</a>
	/// </summary>
	public class RetrieveAndRank : IWatsonService
    {
        #region Private Data
        private const string SERVICE_ID = "RetrieveAndRankV1";
        private static fsSerializer sm_Serializer = new fsSerializer();
        private const float REQUEST_TIMEOUT = 10.0f * 60.0f;

        //  List clusters or create cluster.
        private const string SERVICE_CLUSTERS = "/v1/solr_clusters";
        //  Delete cluster or get cluster info.
        private const string SERVICE_CLUSTER = "/v1/solr_clusters/{0}";
        //  List Solr cluster configurations.
        private const string SERVICE_CLUSTER_CONFIGS = "/v1/solr_clusters/{0}/config";
        //  Upload, Get or Delete Solr configuration.
        private const string SERVICE_CLUSTER_CONFIG = "/v1/solr_clusters/{0}/config/{1}";
        //  Forward requests to Solr (Create, Delete, List).
        private const string SERVICE_CLUSTER_COLLECTIONS = "/v1/solr_clusters/{0}/solr/admin/collections";
        //  Index documents.
        private const string SERVICE_CLUSTER_COLLECTION_UPDATE = "/v1/solr_clusters/{0}/solr/{1}/update";
        //  Search Solr standard query parser.
        private const string SERVICE_CLUSTER_COLLECTION_SELECT = "/v1/solr_clusters/{0}/solr/{1}/select";
        //  Search Solr ranked query parser.
        private const string SERVICE_CLUSTER_COLLECTION_FCSELECT = "/v1/solr_clusters/{0}/solr/{1}/fcselect";

        //  List rankers or create ranker.
        private const string SERVICE_RANKERS = "/v1/rankers";
        //  Get ranker information or delete ranker.
        private const string SERVICE_RANKER = "/v1/rankers/{0}";
        //  Rank.
        private const string SERVICE_RANK = "/v1/rankers/{0}/rank";
        #endregion

        #region Public Types
        /// <summary>
        /// The delegate for loading a file, used by TrainClassifier().
        /// </summary>
        /// <param name="filename">The filename to load.</param>
        /// <returns>Should return a byte array of the file contents or null of failure.</returns>
        public delegate byte[] LoadFileDelegate(string filename);
        /// <summary>
        /// Set this property to overload the internal file loading of this class.
        /// </summary>
        public LoadFileDelegate LoadFile { get; set; }
        /// <summary>
        /// The delegate for saving a file, used by DownloadDialog().
        /// </summary>
        /// <param name="filename">The filename to save.</param>
        /// <param name="data">The data to save into the file.</param>
        public delegate void SaveFileDelegate(string filename, byte[] data);

        /// <summary>
        /// Set this property to overload the internal file saving for this class.
        /// </summary>
        public SaveFileDelegate SaveFile { get; set; }
        #endregion

        #region GetClusters
        /// <summary>
        /// OnGetClusters delegate.
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="data"></param>
        public delegate void OnGetClusters(SolrClusterListResponse resp, string data);

        /// <summary>
        /// Retrieves the list of Solr clusters for the service instance.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="customData"></param>
        /// <returns></returns>
        public bool GetClusters(OnGetClusters callback, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            GetClustersRequest req = new GetClustersRequest();
            req.Callback = callback;
            req.Data = customData;
            req.Timeout = REQUEST_TIMEOUT;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_CLUSTERS);
            if (connector == null)
                return false;

            req.OnResponse = OnGetClustersResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// The GetCluster request.
        /// </summary>
        public class GetClustersRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
            public OnGetClusters Callback { get; set; }
        }

        private void OnGetClustersResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            SolrClusterListResponse clustersData = new SolrClusterListResponse();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = clustersData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("RetriveAndRank", "OnGetClustersResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetClustersRequest)req).Callback != null)
                ((GetClustersRequest)req).Callback(resp.Success ? clustersData : null, ((GetClustersRequest)req).Data);
        }
        #endregion

        #region CreateCluster
        /// <summary>
        /// OnCreateCluster callback delegate.
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="data"></param>
        public delegate void OnCreateCluster(SolrClusterResponse resp, string data);

        /// <summary>
        /// Provisions a Solr cluster asynchronously. When the operation is successful, the status of the cluster is set to NOT_AVAILABLE. The status must be READY before you can use the cluster. For information about cluster sizing see http://www.ibm.com/watson/developercloud/doc/retrieve-rank/solr_ops.shtml#sizing.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="clusterName">Name to identify the cluster.</param>
        /// <param name="clusterSize">Size of the cluster to create. Ranges from 1 to 7. Send an empty value to create a small free cluster for testing. You can create only one free cluster.</param>
        /// <param name="customData"></param>
        /// <returns></returns>
        public bool CreateCluster(OnCreateCluster callback, string clusterName = default(string), string clusterSize = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            CreateClusterRequest req = new CreateClusterRequest();
            req.Callback = callback;
            req.Data = customData;
            req.Timeout = REQUEST_TIMEOUT;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_CLUSTERS);
            if (connector == null)
                return false;

            string reqJson = "";
            if(!string.IsNullOrEmpty(clusterName) && !string.IsNullOrEmpty(clusterSize))
                reqJson = "{\n\t\"cluster_name\": \"" + clusterName + "\",\n\t\"cluster_size\": \"" + clusterSize + "\"\n}";

            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Send = Encoding.UTF8.GetBytes(reqJson);
            req.OnResponse = OnCreateClusterResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// The Create Cluster request.
        /// </summary>
        public class CreateClusterRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
            public OnCreateCluster Callback { get; set; }
        }

        /// <summary>
        /// The Create Cluster response.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="resp"></param>
        private void OnCreateClusterResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            SolrClusterResponse clusterResponseData = new SolrClusterResponse();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = clusterResponseData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("RetriveAndRank", "OnCreateClusterResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((CreateClusterRequest)req).Callback != null)
                ((CreateClusterRequest)req).Callback(resp.Success ? clusterResponseData : null, ((CreateClusterRequest)req).Data);
        }
        #endregion

        #region DeleteClusters
        /// <summary>
        /// Delete cluster callback delegate.
        /// </summary>
        /// <param name="deleteSuccess"></param>
        /// <param name="data"></param>
        public delegate void OnDeleteCluster(bool success, string data);

        /// <summary>
        /// Delete a Solr cluster.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="clusterID"></param>
        /// <param name="customData"></param>
        /// <returns></returns>
        public bool DeleteCluster(OnDeleteCluster callback, string clusterID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(clusterID))
                throw new ArgumentNullException("ClusterID to be deleted is required!");
            if (clusterID == Config.Instance.GetVariableValue("RetrieveAndRank_IntegrationTestClusterID"))
                throw new WatsonException("You cannot delete the example cluster!");

            DeleteClusterRequest req = new DeleteClusterRequest();
            req.Callback = callback;
            req.Data = customData;
            req.ClusterID = clusterID;
            req.Delete = true;
            req.Timeout = REQUEST_TIMEOUT;
            req.OnResponse = OnDeleteClusterResponse;
            string service = string.Format(SERVICE_CLUSTER, clusterID);
            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if (connector == null)
                return false;
            
            return connector.Send(req);
        }

        /// <summary>
        /// The Delete Cluster request
        /// </summary>
        public class DeleteClusterRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The cluster identifier.
			/// </summary>
            public string ClusterID { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
            public OnDeleteCluster Callback { get; set; }
        }

        /// <summary>
        /// The Delete Cluster response.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="resp"></param>
        private void OnDeleteClusterResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Log.Debug("RetrieveAndRank", "OnDeleteClusterResponse success: {0}", resp.Success);
            if (((DeleteClusterRequest)req).Callback != null)
                ((DeleteClusterRequest)req).Callback(resp.Success, ((DeleteClusterRequest)req).Data);
        }
        #endregion

        #region GetCluster
        /// <summary>
        /// Get cluster info callback delegate.
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="data"></param>
        public delegate void OnGetCluster(SolrClusterResponse resp, string data);

        /// <summary>
        /// Returns status and other information about a cluster.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="clusterID">Unique identifier for this cluster.</param>
        /// <param name="customData"></param>
        /// <returns></returns>
        public bool GetCluster(OnGetCluster callback, string clusterID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(clusterID))
                throw new ArgumentNullException("ClusterID to get is required!");

            GetClusterRequest req = new GetClusterRequest();
            req.Callback = callback;
            req.Data = customData;
            req.ClusterID = clusterID;
            req.Timeout = REQUEST_TIMEOUT;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_CLUSTER, clusterID));
            if (connector == null)
                return false;

            req.OnResponse = OnGetClusterResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// The Get Cluster request
        /// </summary>
        public class GetClusterRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The cluster identifier.
			/// </summary>
            public string ClusterID { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
            public OnGetCluster Callback { get; set; }
        }

        /// <summary>
        /// The Get Cluster response.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="resp"></param>
        private void OnGetClusterResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            SolrClusterResponse clusterData = new SolrClusterResponse();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = clusterData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("RetriveAndRank", "OnGetClusterResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetClusterRequest)req).Callback != null)
                ((GetClusterRequest)req).Callback(resp.Success ? clusterData : null, ((GetClusterRequest)req).Data);
        }
        #endregion

        #region ListClusterConfigs
		/// <summary>
		/// Get Cluster Configs callback delegate.
		/// </summary>
		/// <param name="resp"></param>
		/// <param name="data"></param>
        public delegate void OnGetClusterConfigs(SolrConfigList resp, string data);

		/// <summary>
		/// Returns a configuration .zip file for a cluster.
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="clusterID"></param>
		/// <param name="customData"></param>
		/// <returns></returns>
		public bool GetClusterConfigs(OnGetClusterConfigs callback, string clusterID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(clusterID))
                throw new ArgumentNullException("ClusterID to get is required!");

            GetClusterConfigsRequest req = new GetClusterConfigsRequest();
            req.Callback = callback;
            req.ClusterID = clusterID;
            req.Data = customData;
            req.Timeout = REQUEST_TIMEOUT;
            req.OnResponse = OnGetClusterConfigsResponse;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_CLUSTER_CONFIGS, clusterID));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

		/// <summary>
		/// The GetClusterConfigs request.
		/// </summary>
        public class GetClusterConfigsRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The cluster identifier.
			/// </summary>
			public string ClusterID { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
            public OnGetClusterConfigs Callback { get; set; }
        }

		/// <summary>
		/// The OnGetClusterConfigs response.
		/// </summary>
		/// <param name="req"></param>
		/// <param name="resp"></param>
        private void OnGetClusterConfigsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            SolrConfigList configData = new SolrConfigList();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    string json = Encoding.UTF8.GetString(resp.Data);
                    fsResult r = fsJsonParser.Parse(json, out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = configData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("RetriveAndRank", "OnGetClusterConfigsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetClusterConfigsRequest)req).Callback != null)
                ((GetClusterConfigsRequest)req).Callback(resp.Success ? configData : null, ((GetClusterConfigsRequest)req).Data);
        }
        #endregion

        #region DeleteClusterConfig
        /// <summary>
        /// Delete cluster config callback delegate.
        /// </summary>
        /// <param name="deleteSuccess"></param>
        /// <param name="data"></param>
        public delegate void OnDeleteClusterConfig(bool success, string data);

        /// <summary>
        /// Deletes the configuration for a cluster. Before you delete the configuration, delete any collections that point to it.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="clusterID">The name of the configuration to delete.</param>
        /// <param name="configID">Cluster ID for the configuration.</param>
        /// <param name="customData"></param>
        /// <returns></returns>
        public bool DeleteClusterConfig(OnDeleteClusterConfig callback, string clusterID, string configID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(clusterID))
                throw new ArgumentNullException("clusterID to is required!");
            if (string.IsNullOrEmpty(configID))
                throw new ArgumentNullException("configID to be deleted is required!");
            if (configID == Config.Instance.GetVariableValue("RetrieveAndRank_IntegrationTestClusterConfigID"))
                throw new WatsonException("You cannot delete the example cluster config!");

            DeleteClusterConfigRequest req = new DeleteClusterConfigRequest();
            req.Callback = callback;
            req.Data = customData;
            req.ClusterID = clusterID;
            req.ConfigID = configID;
            req.Timeout = REQUEST_TIMEOUT;
            req.Delete = true;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_CLUSTER_CONFIG, clusterID, configID));
            if (connector == null)
                return false;

            req.OnResponse = OnDeleteClusterConfigResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// The Delete Cluster Config request
        /// </summary>
        public class DeleteClusterConfigRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The cluster identifier.
			/// </summary>
            public string ClusterID { get; set; }
			/// <summary>
			/// The config identifier.
			/// </summary>
            public string ConfigID { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
            public OnDeleteClusterConfig Callback { get; set; }
        }

        private void OnDeleteClusterConfigResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            if (((DeleteClusterConfigRequest)req).Callback != null)
                ((DeleteClusterConfigRequest)req).Callback(resp.Success, ((DeleteClusterConfigRequest)req).Data);
        }
        #endregion

        #region GetClusterConfig
        /// <summary>
        /// The GetClusterConfig delegate.
        /// </summary>
        /// <param name="getSuccess"></param>
        /// <param name="data"></param>
        public delegate void OnGetClusterConfig(byte[] resp, string data);

        /// <summary>
        /// Retrieves the configuration for a cluster by its name.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="clusterID">Cluster ID for the configuration.</param>
        /// <param name="configName">The name of the configuration to retrieve.</param>
        /// <param name="customData"></param>
        /// <returns></returns>
        public bool GetClusterConfig(OnGetClusterConfig callback, string clusterID, string configName, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(clusterID))
                throw new ArgumentNullException("A clusterID is required for GetClusterConfig!");
            if (string.IsNullOrEmpty(configName))
                throw new ArgumentNullException("A configName is required for GetClusterConfig!");

            GetClusterConfigRequest req = new GetClusterConfigRequest();
            req.Data = customData;
            req.Callback = callback;
            req.ClusterID = clusterID;
            req.ConfigName = configName;
            req.OnResponse = GetClusterConfigResponse;
            req.Timeout = REQUEST_TIMEOUT;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_CLUSTER_CONFIG, clusterID, configName));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetClusterConfigRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The cluster identifier.
			/// </summary>
            public string ClusterID { get; set; }
			/// <summary>
			/// The confguration name.
			/// </summary>
            public string ConfigName { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
            public OnGetClusterConfig Callback { get; set; }
        }

        private void GetClusterConfigResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            byte[] respData = null;

            if(resp.Success)
            {
                respData = resp.Data;
            }

            if (((GetClusterConfigRequest)req).Callback != null)
                ((GetClusterConfigRequest)req).Callback(respData, ((GetClusterConfigRequest)req).Data);
        }

        /// <summary>
        /// OnSaveClusterConfig callback delegate.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="data"></param>
        public delegate void OnSaveClusterConfig(bool success, string data);

        /// <summary>
        /// Saves the config zip to the file system.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="configData">Byte array of the config data.</param>
        /// <param name="configFileName">Where to save the zip file in the file system.</param>
        /// <param name="customData"></param>
        public void SaveConfig(OnSaveClusterConfig callback, byte[] configData, string configFileName, string customData)
        {
            bool success = true;
            if(configData != null)
            {
                try
                {
                    if (SaveFile != null)
                        SaveFile(configFileName, configData);
                    else
                    {
#if !UNITY_WEBPLAYER
                        File.WriteAllBytes(configFileName, configData);
#endif
                    }
                }
                catch (Exception e)
                {
                    success = false;
                    Log.Error("RetrieveAndRank", "Caught exception: {0}", e.ToString());
                }
            }

            if (callback != null)
                callback(success, customData);
        }
        #endregion

        #region UploadClusterConfig
        /// <summary>
        /// UploadClusterConfig callback delegate.
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="data"></param>
        public delegate void OnUploadClusterConfig(UploadResponse resp, string data);

        /// <summary>
        /// Uploads a zip file containing the configuration files for your Solr collection. The zip file must include schema.xml, solrconfig.xml, and other files you need for your configuration. Configuration files on the zip file's path are not uploaded. The request fails if a configuration with the same name exists. To update an existing config, use the Solr configuration API (https://cwiki.apache.org/confluence/display/solr/Config+API).
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="clusterID">Cluster ID for the configuration.</param>
        /// <param name="configName">The name of the configuration to create.</param>
        /// <param name="configPath">The path to the compressed configuration files.</param>
        /// <param name="customData"></param>
        /// <returns></returns>
        public bool UploadClusterConfig(OnUploadClusterConfig callback, string clusterID, string configName, string configPath, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(clusterID))
                throw new ArgumentNullException("A clusterID is required for UploadClusterConfig!");
            if (string.IsNullOrEmpty(configName))
                throw new ArgumentNullException("A configName is required for UploadClusterConfig!");
            if (string.IsNullOrEmpty(configPath))
                throw new ArgumentNullException("A configPath is required for UploadClusterConfig!");

            UploadClusterConfigRequest req = new UploadClusterConfigRequest();
            req.Callback = callback;
            req.Data = customData;
            req.ClusterID = clusterID;
            req.ConfigName = configName;
            req.OnResponse = UploadClusterConfigResponse;
            req.Timeout = REQUEST_TIMEOUT;

            byte[] configData = null;
            if (LoadFile != null)
            {
                configData = LoadFile(configPath);
            }
            else
            {
#if !UNITY_WEBPLAYER
                configData = File.ReadAllBytes(configPath);
#endif
            }

            if (configData == null)
                Log.Error("RetrieveAndRank", "Failed to upload {0}!", configPath);

            req.Headers["Content-Type"] = "application/zip";
            req.Send = configData;
            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_CLUSTER_CONFIG, clusterID, configName));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class UploadClusterConfigRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The cluster identifier.
			/// </summary>
            public string ClusterID { get; set; }
			/// <summary>
			/// The configuration name.
			/// </summary>
            public string ConfigName { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
            public OnUploadClusterConfig Callback { get; set; }
        }

        private void UploadClusterConfigResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            UploadResponse uploadResponse = new UploadResponse();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = uploadResponse;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("RetriveAndRank", "UploadClusterConfigResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((UploadClusterConfigRequest)req).Callback != null)
                ((UploadClusterConfigRequest)req).Callback(resp.Success ? uploadResponse : null, ((UploadClusterConfigRequest)req).Data);
        }
        #endregion

        #region ForwardCollectionRequest
        /// <summary>
        /// The OnGetCollections delegate.
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="data"></param>
        public delegate void OnCollections(CollectionsResponse resp, string data);

        /// <summary>
        /// An example of a method that forwards to the Solr Collections API (https://cwiki.apache.org/confluence/display/solr/Collections+API). This Retrieve and Rank resource improves error handling and resiliency of the Solr Collections API.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="clusterID">Cluster ID for the collection.</param>
        /// <param name="action">Operation to carry out. Either "CREATE", "LIST", or "DELETE"</param>
        /// <param name="collectionName">The collectionName required for "CREATE" or "DELETE".</param>
        /// <param name="configName">The cluster configuration name to use for "CREATE".</param>
        /// <param name="customData"></param>
        /// <returns></returns>
        public bool ForwardCollectionRequest(OnCollections callback, string clusterID, string action, string collectionName = default(string), string configName = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(action))
                throw new ArgumentNullException("An Action is required for ForwardCollectionRequest (CREATE, DELETE or LIST)");
            if (string.IsNullOrEmpty(clusterID))
                throw new ArgumentNullException("A clusterID is required for ForwardCollectionRequest!");

            CollectionRequest req = new CollectionRequest();
            req.Callback = callback;
            req.ClusterID = clusterID;
            req.Data = customData;
            req.Action = action;
            req.CollectionName = collectionName;
            req.ConfigName = configName;
            req.Parameters["action"] = action;
            req.Parameters["wt"] = "json";
            req.Timeout = REQUEST_TIMEOUT;

            switch (action)
            {
                case CollectionsAction.LIST:
                    break;
                case CollectionsAction.CREATE:
                    if (string.IsNullOrEmpty(collectionName))
                        throw new ArgumentNullException("A collectionName is required for ForwardCollectionRequest (CREATE)!");
                    if (string.IsNullOrEmpty(configName))
                        throw new ArgumentNullException("A configName is required for ForwardCollectionRequest (CREATE)!");
                    req.Parameters["name"] = collectionName;
                    req.Parameters["collection.configName"] = configName;
                    break;
                case CollectionsAction.DELETE:
                    if (string.IsNullOrEmpty(collectionName))
                        throw new ArgumentNullException("A collectionName is required for ForwardCollectionRequest (DELETE)!");
                    req.Parameters["name"] = collectionName;
                    break;
                default:
                    throw new WatsonException(string.Format("No case exists for action {0}!", action));
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_CLUSTER_COLLECTIONS, clusterID));
            if (connector == null)
                return false;

            req.OnResponse = OnForwardCollectionRequestResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// The ForwardCollectionRequest request
        /// </summary>
        public class CollectionRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
            public OnCollections Callback { get; set; }
            /// <summary>
            /// Cluster ID required for all actions.
            /// </summary>
            public string ClusterID { get; set; }
            /// <summary>
            /// Action for the call. Either "CREATE", "LIST", or "DELETE"
            /// </summary>
            public string Action { get; set; }
            /// <summary>
            /// The collectionName required for "CREATE" or "DELETE".
            /// </summary>
            public string CollectionName { get; set; }
            /// <summary>
            /// The cluster configuration name to use for "CREATE".
            /// </summary>
            public string ConfigName { get; set; }
        }

        private void OnForwardCollectionRequestResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            CollectionsResponse collectionsData = new CollectionsResponse();
            if(resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = collectionsData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("RetriveAndRank", "OnForwardCollectionRequestResponse exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((CollectionRequest)req).Callback != null)
                ((CollectionRequest)req).Callback(resp.Success ? collectionsData : null, ((CollectionRequest)req).Data);
        }
        #endregion

        #region IndexDocuments
        /// <summary>
		/// OnIndexDocuments callback delegate.
		/// </summary>
		/// <param name="resp"></param>
		/// <param name="data"></param>
		public delegate void OnIndexDocuments(IndexResponse resp, string data);

        /// <summary>
        /// Adds content to a Solr index so you can search it. An example of a method that forwards to Solr. For more information about indexing, see Indexing and Basic Data Operations in the Apache Solr Reference (https://cwiki.apache.org/confluence/display/solr/Indexing+and+Basic+Data+Operations). You must commit your documents to the index to search for them. For more information about when to commit, see UpdateHandlers in SolrConfig in the Solr Reference (https://cwiki.apache.org/confluence/display/solr/UpdateHandlers+in+SolrConfig).
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="indexdataPath">Path to the file that defines the content.</param>
        /// <param name="clusterID">Cluster ID.</param>
        /// <param name="collectionName">Collection.</param>
        /// <param name="customData"></param>
        /// <returns></returns>
        public bool IndexDocuments(OnIndexDocuments callback, string indexDataPath, string clusterID, string collectionName, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(indexDataPath))
                throw new ArgumentNullException("A index data json path is required to index documents!");
            if (string.IsNullOrEmpty(clusterID))
                throw new ArgumentNullException("A clusterID is required to index documents!");
            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentNullException("A collectionName is required to index documents!");

            IndexDocumentsRequest req = new IndexDocumentsRequest();
            req.Callback = callback;
            req.IndexDataPath = indexDataPath;
            req.ClusterID = clusterID;
            req.CollectionName = collectionName;
            req.Data = customData;
            req.Timeout = REQUEST_TIMEOUT;

            byte[] indexData;
            if (LoadFile != null)
            {
                indexData = File.ReadAllBytes(indexDataPath);
            }
            else
            {
#if !UNITY_WEBPLAYER
                indexData = File.ReadAllBytes(indexDataPath);
#endif
            }

            if (indexData == null)
                Log.Error("RetrieveAndRank", "Failed to upload {0}!", indexDataPath);

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_CLUSTER_COLLECTION_UPDATE, clusterID, collectionName));
            if (connector == null)
                return false;

            req.Headers["Content-Type"] = "multipart/form-data";
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["body"] = new RESTConnector.Form(indexData, "indexData.json", "application/json");
            req.OnResponse = OnIndexDocumentsResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// The Create Ranker request.
        /// </summary>
        public class IndexDocumentsRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The index data path.
			/// </summary>
            public string IndexDataPath { get; set; }
			/// <summary>
			/// The cluster identifier to use.
			/// </summary>
            public string ClusterID { get; set; }
			/// <summary>
			/// The collection name to use.
			/// </summary>
            public string CollectionName { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
            public OnIndexDocuments Callback { get; set; }
        }

        private void OnIndexDocumentsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            IndexResponse indexResponseData = new IndexResponse();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    string json = Encoding.UTF8.GetString(resp.Data);
                    Log.Debug("RetriveAndRank", "json: {0}", json);
                    fsResult r = fsJsonParser.Parse(json, out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                    object obj = indexResponseData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("RetriveAndRank", "OnIndexDocumentsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((IndexDocumentsRequest)req).Callback != null)
                ((IndexDocumentsRequest)req).Callback(resp.Success ? indexResponseData : null, ((IndexDocumentsRequest)req).Data);
        }
        #endregion

        #region Search
        /// <summary>
        /// The OnSearch callback delegate.
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="data"></param>
        public delegate void OnSearch(SearchResponse resp, string data);

        /// <summary>
        /// Return reranked results for your query. The request is similar to the Search Solr standard query parser method, but includes the ranker_id and, in the default configuration, fcselect replaces the select request handler. (https://cwiki.apache.org/confluence/display/solr/The+Standard+Query+Parser).
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="clusterID">Cluster ID.</param>
        /// <param name="collectionName">The name of the collection to use.</param>
        /// <param name="query">The query. Uses Solr standard query syntax.</param>
        /// <param name="fl">The fields to return.</param>
        /// <param name="isRankedSearch">Use ranked search instead of standard search.</param>
        /// <param name="rankerID">The trained ranker to query.</param>
        /// <param name="customData"></param>
        /// <returns></returns>
        public bool Search(OnSearch callback, string clusterID, string collectionName, string query, string[] fl, bool isRankedSearch = false, string rankerID = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(clusterID))
                throw new ArgumentNullException("A clusterID is required to search!");
            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentNullException("A collectionName is required to search!");
            if (string.IsNullOrEmpty(query))
                throw new ArgumentNullException("A query is required to search!");
            if (fl == default(string[]))
                throw new ArgumentNullException("An array of filters are required to search!");

            SearchRequest req = new SearchRequest();
            req.Callback = callback;
            req.ClusterID = clusterID;
            req.CollectionName = collectionName;
            req.Query = query;
            req.Fl = fl;
            req.Data = customData;
            req.Timeout = REQUEST_TIMEOUT;

            req.Parameters["wt"] = "json";
            req.Parameters["q"] = query;
            req.Parameters["fl"] = string.Join(",", fl);

            req.OnResponse = OnSearchResponse;

            RESTConnector connector;
            if (!isRankedSearch)
                connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_CLUSTER_COLLECTION_SELECT, clusterID, collectionName));
            else
            {
                connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_CLUSTER_COLLECTION_FCSELECT, clusterID, collectionName));
                req.Parameters["ranker_id"] = rankerID;
            }

            if (connector == null)
                return false;

            return connector.Send(req);
        }

        /// <summary>
        /// The search request.
        /// </summary>
        public class SearchRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The cluster identifier to use.
			/// </summary>
            public string ClusterID { get; set; }
			/// <summary>
			/// The collectionName to use.
			/// </summary>
            public string CollectionName { get; set; }
			/// <summary>
			/// The query.
			/// </summary>
            public string Query { get; set; }
			/// <summary>
			/// The query fields to use.
			/// </summary>
            public string[] Fl { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
            public OnSearch Callback { get; set; }
        }

        private void OnSearchResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            SearchResponse searchData = new SearchResponse();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = searchData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("RetriveAndRank", "OnSearchResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((SearchRequest)req).Callback != null)
                ((SearchRequest)req).Callback(resp.Success ? searchData : null, ((SearchRequest)req).Data);
        }
        #endregion
        
        #region GetRankers
        /// <summary>
        /// OnGetRankers delegate.
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="data"></param>
        public delegate void OnGetRankers(ListRankersPayload resp, string data);

        /// <summary>
        /// Retrieves the list of rankers for the service instance.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="customData"></param>
        /// <returns></returns>
        public bool GetRankers(OnGetRankers callback, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            GetRankersRequest req = new GetRankersRequest();
            req.Callback = callback;
            req.Data = customData;
            req.Timeout = REQUEST_TIMEOUT;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_RANKERS);
            if (connector == null)
                return false;

            req.OnResponse = OnGetRankersResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// The GetRanker request.
        /// </summary>
        public class GetRankersRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
            public OnGetRankers Callback { get; set; }
        }

        private void OnGetRankersResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ListRankersPayload rankersData = new ListRankersPayload();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = rankersData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("RetriveAndRank", "OnGetRankersResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetRankersRequest)req).Callback != null)
                ((GetRankersRequest)req).Callback(resp.Success ? rankersData : null, ((GetRankersRequest)req).Data);
        }
		#endregion

		#region CreateRanker
		/// <summary>
		/// OnCreateCluster callback delegate.
		/// </summary>
		/// <param name="resp"></param>
		/// <param name="data"></param>
		public delegate void OnCreateRanker(RankerStatusPayload resp, string data);

        /// <summary>
        /// Sends data to create and train a ranker and returns information about the new ranker. When the operation is successful, the status of the ranker is set to Training. The status must be Available before you can use the ranker.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="trainingDataPath">Training data in CSV format. The first header must be question_id and the last header must be the relevance label. The other headers are alphanumeric feature names. For details, see Using your own data (http://www.ibm.com/watson/developercloud/doc/retrieve-rank/data_format.shtml).</param>
        /// <param name="name">Metadata in JSON format. The metadata identifies an optional name to identify the ranker.</param>
        /// <param name="customData"></param>
        /// <returns></returns>
        public bool CreateRanker(OnCreateRanker callback, string trainingDataPath, string name = default(string), string customData = default(string))
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("A ranker name is required to create a ranker!");
			if (string.IsNullOrEmpty(trainingDataPath))
				throw new ArgumentNullException("Training data is required to create a ranker!");

			CreateRankerRequest req = new CreateRankerRequest();
			req.Callback = callback;
			req.Name = name;
			req.TrainingDataPath = trainingDataPath;
			req.Data = customData;
            req.Timeout = REQUEST_TIMEOUT;

            byte[] trainingData;
			if (LoadFile != null)
			{
				trainingData = File.ReadAllBytes(trainingDataPath);
			}
			else
			{
#if !UNITY_WEBPLAYER
				trainingData = File.ReadAllBytes(trainingDataPath);
#endif
			}

			if (trainingData == null)
				Log.Error("RetrieveAndRank", "Failed to upload {0}!", trainingDataPath);
		
			RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_RANKERS);
			if (connector == null)
				return false;

			req.Forms = new Dictionary<string, RESTConnector.Form>();
			req.Forms["training_data"] = new RESTConnector.Form(trainingData, "training_data.csv", "text/csv");
			if (!string.IsNullOrEmpty(name))
			{
				string reqJson = "{\n\t\"name\": \"" + name + "\"\n}";
				req.Forms["training_metadata"] = new RESTConnector.Form(reqJson);
			}
			req.OnResponse = OnCreateRankerResponse;
			return connector.Send(req);
		}

		/// <summary>
		/// The Create Ranker request.
		/// </summary>
		public class CreateRankerRequest : RESTConnector.Request
		{
			/// <summary>
			/// Custom data.
			/// </summary>
			public string Data { get; set; }
			/// <summary>
			/// The ranker name.
			/// </summary>
			public string Name { get; set; }
			/// <summary>
			/// The ranker training data path.
			/// </summary>
			public string TrainingDataPath { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
			public OnCreateRanker Callback { get; set; }
		}

		private void OnCreateRankerResponse(RESTConnector.Request req, RESTConnector.Response resp)
		{
			RankerStatusPayload rankerResponseData = new RankerStatusPayload();
			if (resp.Success)
			{
				try
				{
					fsData data = null;
					fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
					if (!r.Succeeded)
						throw new WatsonException(r.FormattedMessages);

					object obj = rankerResponseData;
					r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
					if (!r.Succeeded)
						throw new WatsonException(r.FormattedMessages);
				}
				catch (Exception e)
				{
					Log.Error("RetriveAndRank", "OnCreateRankerResponse Exception: {0}", e.ToString());
					resp.Success = false;
				}
			}

			if (((CreateRankerRequest)req).Callback != null)
				((CreateRankerRequest)req).Callback(resp.Success ? rankerResponseData : null, ((CreateRankerRequest)req).Data);
		}
		#endregion

		#region Rank
		/// <summary>
		/// OnRank callback delegate.
		/// </summary>
		/// <param name="resp"></param>
		/// <param name="data"></param>
		public delegate void OnRank(RankerOutputPayload resp, string data);

        /// <summary>
        /// Returns the top answer and a list of ranked answers with their ranked scores and confidence values. Use the Get information about a ranker method to retrieve the status (http://www.ibm.com/watson/developercloud/retrieve-and-rank/api/v1/#get_status). Use this method to return answers when you train the ranker with custom features. However, in most cases, you can use the Search and rank method (http://www.ibm.com/watson/developercloud/retrieve-and-rank/api/v1/#query_ranker).
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="rankerID">ID of the ranker to use.</param>
        /// <param name="searchResultPath">The path to the CSV file that contains the search results that you want to rank. The first column header of the CSV must be labeled answer_id. The remaining column headers must match the names of the features in the training data that was used when this ranker was created.</param>
        /// <param name="customData"></param>
        /// <returns></returns>
        public bool Rank(OnRank callback, string rankerID, string searchResultPath = default(string), string customData = default(string))
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			if (string.IsNullOrEmpty(rankerID))
				throw new ArgumentNullException("A rankerID is required rank!");
			if (string.IsNullOrEmpty(searchResultPath))
				throw new ArgumentNullException("Search results are required to rank!");

			RankRequest req = new RankRequest();
			req.Callback = callback;
			req.RankerID = rankerID;
			req.SearchResultsPath = searchResultPath;
			req.Data = customData;
            req.Timeout = REQUEST_TIMEOUT;

            byte[] searchResultData;
			if (LoadFile != null)
			{
				searchResultData = File.ReadAllBytes(searchResultPath);
			}
			else
			{
#if !UNITY_WEBPLAYER
				searchResultData = File.ReadAllBytes(searchResultPath);
#endif
			}

			if (searchResultData == null)
				Log.Error("RetrieveAndRank", "Failed to upload {0}!", searchResultData);

			RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_RANK, rankerID));
			if (connector == null)
				return false;

			//req.Headers["Content-Type"] = "multipart/form-data";
			//req.Headers["Accept"] = "*/*";

			req.Forms = new Dictionary<string, RESTConnector.Form>();
			req.Forms["answer_data"] = new RESTConnector.Form(searchResultData, "searck_data.csv", "text/csv");

			req.OnResponse = OnRankResponse;
			return connector.Send(req);
		}

		/// <summary>
		/// The Rank request.
		/// </summary>
		public class RankRequest : RESTConnector.Request
		{
			/// <summary>
			/// Custom data.
			/// </summary>
			public string Data { get; set; }
			/// <summary>
			/// The ranker identifier.
			/// </summary>
			public string RankerID { get; set; }
			/// <summary>
			/// The search results path.
			/// </summary>
			public string SearchResultsPath { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
			public OnRank Callback { get; set; }
		}

		/// <summary>
		/// Rank response.
		/// </summary>
		/// <param name="req"></param>
		/// <param name="resp"></param>
		private void OnRankResponse(RESTConnector.Request req, RESTConnector.Response resp)
		{
			RankerOutputPayload rankData = new RankerOutputPayload();
			if (resp.Success)
			{
				try
				{
					fsData data = null;
					fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
					if (!r.Succeeded)
						throw new WatsonException(r.FormattedMessages);

					object obj = rankData;
					r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
					if (!r.Succeeded)
						throw new WatsonException(r.FormattedMessages);
				}
				catch (Exception e)
				{
					Log.Error("RetriveAndRank", "OnRankResponse Exception: {0}", e.ToString());
					resp.Success = false;
				}
			}

			if (((RankRequest)req).Callback != null)
				((RankRequest)req).Callback(resp.Success ? rankData : null, ((RankRequest)req).Data);
		}
		#endregion

		#region DeleteRanker
		/// <summary>
		/// Delete Ranker callback delegate.
		/// </summary>
		/// <param name="deleteSuccess"></param>
		/// <param name="data"></param>
		public delegate void OnDeleteRanker(bool success, string data);

        /// <summary>
        /// Deletes a ranker.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="rankerID">ID of the ranker to delete.</param>
        /// <param name="customData"></param>
        /// <returns></returns>
        public bool DeleteRanker(OnDeleteRanker callback, string rankerID, string customData = default(string))
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			if (string.IsNullOrEmpty(rankerID))
				throw new ArgumentNullException("RankerID to be deleted is required!");
			if (rankerID == Config.Instance.GetVariableValue("RetrieveAndRank_IntegrationTestRankerID"))
				throw new WatsonException("You cannot delete the example ranker!");

			DeleteRankerRequest req = new DeleteRankerRequest();
			req.Callback = callback;
            req.Data = customData;
			req.RankerID = rankerID;
            req.Timeout = REQUEST_TIMEOUT;
			req.Delete = true;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_RANKER, rankerID));
			if (connector == null)
				return false;

			req.OnResponse = OnDeleteRankerResponse;
			return connector.Send(req);
		}

		/// <summary>
		/// The Delete Ranker request
		/// </summary>
		public class DeleteRankerRequest : RESTConnector.Request
		{
			/// <summary>
			/// Custom data.
			/// </summary>
			public string Data { get; set; }
			/// <summary>
			/// The ranker identifier.
			/// </summary>
			public string RankerID { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
			public OnDeleteRanker Callback { get; set; }
		}

		/// <summary>
		/// The Delete Cluster response.
		/// </summary>
		/// <param name="req"></param>
		/// <param name="resp"></param>
		private void OnDeleteRankerResponse(RESTConnector.Request req, RESTConnector.Response resp)
		{
			if (((DeleteRankerRequest)req).Callback != null)
				((DeleteRankerRequest)req).Callback(resp.Success, ((DeleteRankerRequest)req).Data);
		}
		#endregion

		#region GetRankerInfo
		/// <summary>
		/// Get ranker info callback delegate.
		/// </summary>
		/// <param name="resp"></param>
		/// <param name="data"></param>
		public delegate void OnGetRanker(RankerStatusPayload resp, string data);

        /// <summary>
        /// Returns status and other information about a ranker.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="rankerID">ID of the ranker to query.</param>
        /// <param name="customData"></param>
        /// <returns></returns>
        public bool GetRanker(OnGetRanker callback, string rankerID, string customData = default(string))
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			if (string.IsNullOrEmpty(rankerID))
				throw new ArgumentNullException("RankerID to get is required!");

			GetRankerRequest req = new GetRankerRequest();
			req.Callback = callback;
            req.Data = customData;
			req.RankerID = rankerID;
            req.Timeout = REQUEST_TIMEOUT;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_RANKER, rankerID));
			if (connector == null)
				return false;

			req.OnResponse = OnGetRankerResponse;
			return connector.Send(req);
		}

		/// <summary>
		/// The Get Ranker request
		/// </summary>
		public class GetRankerRequest : RESTConnector.Request
		{
			/// <summary>
			/// Custom data.
			/// </summary>
			public string Data { get; set; }
			/// <summary>
			/// The ranker identifier.
			/// </summary>
			public string RankerID { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
			public OnGetRanker Callback { get; set; }
		}

		/// <summary>
		/// The Get Ranker response.
		/// </summary>
		/// <param name="req"></param>
		/// <param name="resp"></param>
		private void OnGetRankerResponse(RESTConnector.Request req, RESTConnector.Response resp)
		{
			RankerStatusPayload rankerData = new RankerStatusPayload();
			if (resp.Success)
			{
				try
				{
					fsData data = null;
					fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
					if (!r.Succeeded)
						throw new WatsonException(r.FormattedMessages);

					object obj = rankerData;
					r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
					if (!r.Succeeded)
						throw new WatsonException(r.FormattedMessages);
				}
				catch (Exception e)
				{
					Log.Error("RetriveAndRank", "OnGetRankerResponse Exception: {0}", e.ToString());
					resp.Success = false;
				}
			}

			if (((GetRankerRequest)req).Callback != null)
				((GetRankerRequest)req).Callback(resp.Success ? rankerData : null, ((GetRankerRequest)req).Data);
		}
		#endregion

		#region IWatsonService Interface
		/// <exclude />
		public string GetServiceID()
        {
            return SERVICE_ID;
        }

        /// <exclude />
        public void GetServiceStatus(ServiceStatus callback)
        {
            if (Config.Instance.FindCredentials(SERVICE_ID) != null)
                new CheckServiceStatus(this, callback);
            else
                callback(SERVICE_ID, false);
        }

        private class CheckServiceStatus
        {
            private RetrieveAndRank m_Service = null;
            private ServiceStatus m_Callback = null;

            public CheckServiceStatus(RetrieveAndRank service, ServiceStatus callback)
            {
                m_Service = service;
                m_Callback = callback;

                if (!m_Service.GetClusters(OnGetClusters))
                    m_Callback(SERVICE_ID, false);
            }

            void OnGetClusters(SolrClusterListResponse clustersData, string data)
            {
                if (m_Callback != null)
                    m_Callback(SERVICE_ID, clustersData != null);
            }
        };
        #endregion
    }
}
