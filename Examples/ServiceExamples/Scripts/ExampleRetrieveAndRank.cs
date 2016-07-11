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
using IBM.Watson.DeveloperCloud.Utilities;

public class ExampleRetrieveAndRank : MonoBehaviour
{
    private RetrieveAndRank m_RetrieveAndRank = new RetrieveAndRank();

    void Start()
    {
        LogSystem.InstallDefaultReactors();

        string testClusterID = Config.Instance.GetVariableValue("RetrieveAndRank_IntegrationTestClusterID");
        string testClusterConfigName = "cranfield_solr_config";
        string testClusterConfigPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/RetrieveAndRank/cranfield_solr_config.zip";
		string testRankerTrainingPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/RetrieveAndRank/ranker_training_data.csv";
		string testAnswerDataPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/RetrieveAndRank/ranker_training_data.csv";
		string testRankerID = "3b140ax14-rank-10010";
		string rankerToDelete = "3b140ax14-rank-10015";

        //  Get clusters
        //Log.Debug("ExampleRetrieveAndRank", "Attempting to get clusters!");
        //if (!m_RetrieveAndRank.GetClusters(OnGetClusters))
        //    Log.Debug("ExampleRetrieveAndRank", "Failed to get clusters!");

        //  Create cluster
        //Log.Debug("ExampleRetrieveAndRank", "Attempting to create cluster!");
        //if (!m_RetrieveAndRank.CreateCluster(OnCreateCluster, "unity-test-cluster", "1"))
        //    Log.Debug("ExampleRetrieveAndRank", "Failed to create cluster!");

        //  Delete cluster
        //string clusterToDelete = "scabeadb4c_cd5a_4745_b1b9_156c6292687c";
        //Log.Debug("ExampleRetrieveAndRank", "Attempting to delete cluster {0}!", clusterToDelete);
        //if (!m_RetrieveAndRank.DeleteCluster(OnDeleteCluster, clusterToDelete))
        //    Log.Debug("ExampleRetrieveAndRank", "Failed to delete cluster!");

        //  Get cluster
        //Log.Debug("ExampleRetrieveAndRank", "Attempting to get cluster {0}!", testClusterID);
        //if (!m_RetrieveAndRank.GetCluster(OnGetCluster, testClusterID))
        //    Log.Debug("ExampleRetrieveAndRank", "Failed to get cluster!");

        //  List cluster configs
        //Log.Debug("ExampleRetrieveAndRank", "Attempting to get cluster configs for {0}!", testClusterID);
        //if (!m_RetrieveAndRank.GetClusterConfigs(OnGetClusterConfigs, testClusterID))
        //    Log.Debug("ExampleRetrieveAndRank", "Failed to get cluster configs!");

        //  Delete cluster config
        //string clusterConfigToDelete = "test-config";
        //Log.Debug("ExampleRetrieveAndRank", "Attempting to delete cluster {0} config {1}!", testClusterID, clusterConfigToDelete);
        //if (!m_RetrieveAndRank.DeleteClusterConfig(OnDeleteClusterConfig, testClusterID, clusterConfigToDelete))
        //    Log.Debug("ExampleRetriveAndRank", "Failed to delete cluster config {0}", clusterConfigToDelete);

        //  Get cluster config

        //  Upload cluster config
        //Log.Debug("ExampleRetrieveAndRank", "Attempting to upload cluster {0} config {1}!", testClusterID, testClusterConfigName);
        //if (!m_RetrieveAndRank.UploadClusterConfig(OnUploadClusterConfig, testClusterID, testClusterConfigName, testClusterConfigPath))
        //    Log.Debug("ExampleRetrieveAndRank", "Failed to upload cluster config {0}!", testClusterConfigPath);

        //  List Collection request
        //Log.Debug("ExampleRetrieveAndRank", "Attempting to list collections!");
        //if (!m_RetrieveAndRank.ForwardCollectionRequest(OnGetCollections, testClusterID, CollectionsAction.LIST))
        //    Log.Debug("ExampleRetrieveAndRank", "Failed to get collections!");

        //  Create Collection request
        //Log.Debug("ExampleRetrieveAndRank", "Attempting to create collection!");
        //if (!m_RetrieveAndRank.ForwardCollectionRequest(OnGetCollections, testClusterID, CollectionsAction.CREATE, "TestCollectionToDelete", testClusterConfigName))
        //    Log.Debug("ExampleRetrieveAndRank", "Failed to create collections!");

        //  Delete Collection request
        Log.Debug("ExampleRetrieveAndRank", "Attempting to delete collection!");
        if (!m_RetrieveAndRank.ForwardCollectionRequest(OnGetCollections, testClusterID, CollectionsAction.DELETE, "TestCollectionToDelete"))
            Log.Debug("ExampleRetrieveAndRank", "Failed to delete collections!");

        //  Index documents

        //  Search

        //  Ranked search

        //  Get rankers
        //Log.Debug("ExampleRetrieveAndRank", "Attempting to get rankers!");
        //if (!m_RetrieveAndRank.GetRankers(OnGetRankers))
        //    Log.Debug("ExampleRetrieveAndRank", "Failed to get rankers!");

        //  Create ranker
        //Log.Debug("ExampleRetrieveAndRank", "Attempting to create rankers!");
        //if (!m_RetrieveAndRank.CreateRanker(OnCreateRanker, testRankerTrainingPath, "testRanker"))
        //	Log.Debug("ExampleRetrieveAndRank", "Failed to create ranker!");

        //  Rank
        //Log.Debug("ExampleRetrieveAndRank", "Attempting to rank!");
        //if (!m_RetrieveAndRank.Rank(OnRank, testRankerID, testAnswerDataPath))
        //	Log.Debug("ExampleRetriveAndRank", "Failed to rank!");

        //  Delete rankers
        //Log.Debug("ExampleRetriveAndRank", "Attempting to delete ranker {0}!", rankerToDelete);
        //if (!m_RetrieveAndRank.DeleteRanker(OnDeleteRanker, rankerToDelete))
        //	Log.Debug("ExampleRetrieveAndRank", "Failed to delete ranker {0}!", rankerToDelete);

        //  Get ranker info
        //Log.Debug("ExampleRetrieveAndRank", "Attempting to get Ranker Info!");
        //if (!m_RetrieveAndRank.GetRanker(OnGetRanker, testRankerID))
        //	Log.Debug("ExampleRetrieveAndRank", "Failed to get ranker!");
    }

    private void OnGetClusters(SolrClusterListResponse resp, string data)
    {
        if (resp != null)
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
        if (resp != null)
        {
            Log.Debug("ExampleRetrieveAndRank", "OnCreateClusters | name: {0}, size: {1}, ID: {2}, status: {3}.", resp.cluster_name, resp.cluster_size, resp.solr_cluster_id, resp.solr_cluster_status);
        }
        else
        {
            Log.Debug("ExampleRetrieveAndRank", "OnCreateClusters | Get Cluster Response is null!");
        }
    }

    private void OnDeleteCluster(bool success, string data)
    {
        if (success)
        {
            Log.Debug("ExampleRetrieveAndRank", "OnDeleteCluster | Success!");
        }
        else
        {
            Log.Debug("ExampleRetrieveAndRank", "OnDeleteCluster | Failure!");
        }
    }

    private void OnGetCluster(SolrClusterResponse resp, string data)
    {
        if (resp != null)
        {
            Log.Debug("ExampleRetrieveAndRank", "OnGetClusters | name: {0}, size: {1}, ID: {2}, status: {3}.", resp.cluster_name, resp.cluster_size, resp.solr_cluster_id, resp.solr_cluster_status);
        }
        else
        {
            Log.Debug("ExampleRetrieveAndRank", "OnGetClusters | Get Cluster Response is null!");
        }
    }

    private void OnGetClusterConfigs(SolrConfigList resp, string data)
    {
        if (resp != null)
        {
            if (resp.solr_configs.Length == 0)
                Log.Debug("ExampleRetrieveAndRank", "OnGetClusterConfigs | no cluster configs!");

            foreach (string config in resp.solr_configs)
                Log.Debug("ExampleRetrieveAndRank", "OnGetClusterConfigs | solr_config: " + config);
        }
        else
        {
            Log.Debug("ExampleRetrieveAndRank", "OnGetClustersConfigs | Get Cluster Configs Response is null!");
        }
    }

    private void OnDeleteClusterConfig(bool success, string data)
    {
        if (success)
        {
            Log.Debug("ExampleRetrieveAndRank", "OnDeleteClusterConfig | Success!");
        }
        else
        {
            Log.Debug("ExampleRetrieveAndRank", "OnDeleteClusterConfig | Failure!");
        }
    }

    private void OnUploadClusterConfig(UploadResponse resp, string data)
    {
        if (resp != null)
        {
            Log.Debug("ExampleRetrieveAndRank", "OnUploadClusterConfig | Success! {0}, {1}", resp.message, resp.statusCode);
        }
        else
        {
            Log.Debug("ExampleRetrieveAndRank", "OnUploadClusterConfig | Failure!");
        }
    }

    private void OnGetCollections(CollectionsResponse resp, string data)
    {
        if(resp != null)
        {
            if(resp.responseHeader != null)
                Log.Debug("ExampleRetrieveAndRank", "OnGetCollections | status: {0}, QTime: {1}.", resp.responseHeader.status, resp.responseHeader.QTime);
            if(resp.collections != null)
            {
                if (resp.collections.Length == 0)
                    Log.Debug("ExampleRetrieveAndRank", "OnGetCollections | There are no collections!");
                else
                    foreach (string collection in resp.collections)
                        Log.Debug("ExampleRetrieveAndRank", "\tOnGetCollections | collection: {0}", collection);
            }
        }
        else
        {
            Log.Debug("ExampleRetrieveAndRank", "OnGetCollections | GetCollections Response is null!");
        }
    }

    private void OnGetRankers(ListRankersPayload resp, string data)
    {
        if (resp != null)
        {
            if (resp.rankers.Length == 0)
                Log.Debug("ExampleRetrieveAndRank", "OnGetRankers | no rankers!");
            foreach (RankerInfoPayload ranker in resp.rankers)
                Log.Debug("ExampleRetrieveAndRank", "OnGetRankers | ranker name: {0}, ID: {1}, created: {2}, url: {3}.", ranker.name, ranker.ranker_id, ranker.created, ranker.url);
        }
        else
        {
            Log.Debug("ExampleRetrieveAndRank", "OnGetRankers | Get Ranker Response is null!");
        }
    }

	private void OnCreateRanker(RankerStatusPayload resp, string data)
	{
		if (resp != null)
		{
			Log.Debug("ExampleRetrieveAndRank", "OnCreateRanker | ID: {0}, url: {1}, name: {2}, created: {3}, status: {4}, statusDescription: {5}.", resp.ranker_id, resp.url, resp.name, resp.created, resp.status, resp.status_description);
		}
		else
		{
			Log.Debug("ExampleRetrieveAndRank", "OnCreateRanker | Get Cluster Response is null!");
		}
	}

	private void OnRank(RankerOutputPayload resp, string data)
	{
		if (resp != null)
		{
			Log.Debug("ExampleRetrieveAndRank", "OnRank | ID: {0}, url: {1}, name: {2}, top_answer: {3}.", resp.ranker_id, resp.url, resp.name, resp.top_answer);
			if (resp.answers != null)
				if (resp.answers.Length == 0)
				{
					Log.Debug("ExampleRetrieveAndRank", "\tThere are no answers!");
				}
				else
				{
					foreach (RankedAnswer answer in resp.answers)
						Log.Debug("ExampleRetrieveAndRank", "\tOnRank | answerID: {0}, score: {1}, confidence: {2}.", answer.answer_id, answer.score, answer.confidence);
				}
		}
		else
		{
			Log.Debug("ExampleRetrieveAndRank", "OnRank | Rank response is null!");
		}
	}

	private void OnGetRanker(RankerStatusPayload resp, string data)
	{
		if(resp != null)
		{
			Log.Debug("ExampleRetrieveAndRank", "GetRanker | ranker_id: {0}, url: {1}, name: {2}, created: {3}, status: {4}, status_description: {5}.", resp.ranker_id, resp.url, resp.name, resp.created, resp.status, resp.status_description);
		}
		else
		{
			Log.Debug("ExampleRetrieveAndRank", "GetRanker | GetRanker response is null!");
		}
	}

	private void OnDeleteRanker(bool success, string data)
	{
		if (success)
		{
			Log.Debug("ExampleRetrieveAndRank", "OnDeleteRanker | Success!");
		}
		else
		{
			Log.Debug("ExampleRetrieveAndRank", "OnDeleteRanker | Failure!");
		}
	}
}
