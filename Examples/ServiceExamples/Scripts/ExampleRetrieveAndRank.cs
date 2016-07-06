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
using IBM.Watson.DeveloperCloud.Services.RetrieveAndRank.v1;
using IBM.Watson.DeveloperCloud.Logging;

public class ExampleRetrieveAndRank : MonoBehaviour
{
    private RetrieveAndRank m_RetrieveAndRank = new RetrieveAndRank();

    void Start ()
    {
        LogSystem.InstallDefaultReactors();

        //  Get clusters
        if (!m_RetrieveAndRank.GetClusters(OnGetClusters))
            Log.Debug("ExampleRetrieveAndRank", "Failed to get clusters!");

        //  Create cluster
        if (!m_RetrieveAndRank.CreateCluster(OnCreateCluster, "TestCluster", "1"))
            Log.Debug("ExampleRetrieveAndRank", "Failed to create cluster!");

    }

    private void OnGetClusters(SolrClusterListResponse resp, string data)
    {
        if(resp != null)
        {
            foreach (SolrClusterResponse cluster in resp.clusters)
                Log.Debug("ExampleRetrieveAndRank", "OnGetClusters | cluster name: {0}, size: {1}, ID: {2}, status: {3}.", cluster.cluster_name, cluster.cluster_size, cluster.solr_cluster_id, cluster.solr_cluster_status);
        }
        else
        {
            Log.Debug("ExampleRetrieveAndRank", "OnGetClusters | Get Cluster Response is null!");
        }
    }

    private void OnCreateCluster(SolrClusterResponse resp, string data)
    {
        if(resp != null)
        {
            Log.Debug("ExampleRetrieveAndRank", "OnCreateClusters | name: {0}, size: {1}, ID: {2}, status: {3}.", resp.cluster_name, resp.cluster_size, resp.solr_cluster_id, resp.solr_cluster_status);
        }
        else
        {
            Log.Debug("ExampleRetrieveAndRank", "OnCreateClusters | Get Cluster Response is null!");
        }
    }
}
