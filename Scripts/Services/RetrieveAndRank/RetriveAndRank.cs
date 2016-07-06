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

namespace IBM.Watson.DeveloperCloud.Services.RetriveAndRank.v1
{
    public class RetriveAndRank : IWatsonService
    {
        #region Private Data
        private const string SERVICE_ID = "RetriveAndRankV1";
        private static fsSerializer sm_Serializer = new fsSerializer();
        #endregion

        #region GetClusters
        private const string SERVICE_GET_CLUSTERS = "/V1/solr_clusters";

        /// <summary>
        /// OnGetClusters delegate.
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="data"></param>
        public delegate void OnGetClusters(SolrClusterListResponse resp, string data);

        /// <summary>
        /// Gets all available Solr clusters.
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

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_GET_CLUSTERS);
            if (connector == null)
                return false;

            req.OnResponse = OnGetClustersResponse;
            return connector.Send(req);
        }

        public class GetClustersRequest : RESTConnector.Request
        {
            public string Data { get; set; }
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

        #region CreateClusters
        #endregion

        #region DeleteClusters
        #endregion

        #region GetClusterInfo
        #endregion

        #region ListClusterConfig
        #endregion

        #region DeleteClusterConfig
        #endregion

        #region GetClusterConfig
        #endregion

        #region UploadClusterConfig
        #endregion

        #region CollectionRequest
        #endregion

        #region IndexDocuments
        #endregion

        #region Search
        #endregion

        #region RankedSearch
        #endregion

        #region GetRankers
        #endregion

        #region CreateRanker
        #endregion

        #region Rank
        #endregion

        #region DeleteRanker
        #endregion

        #region GetRankerInfo
        #endregion

        #region IWatsonService Interface
        public string GetServiceID()
        {
            throw new NotImplementedException();
        }

        public void GetServiceStatus(ServiceStatus callback)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
