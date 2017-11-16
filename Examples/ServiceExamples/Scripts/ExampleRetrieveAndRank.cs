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
using IBM.Watson.DeveloperCloud.Connection;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExampleRetrieveAndRank : MonoBehaviour
{
    private string _username = null;
    private string _password = null;
    private string _url = null;
    
    private RetrieveAndRank _retrieveAndRank;

    private string _testClusterConfigName;
    private string _testClusterConfigPath;
    private string _testRankerTrainingPath;
    private string _testAnswerDataPath;
    private string _createdRankerName;
    private string _rankerIdToDelete;
    private string _indexDataPath;
    private string _testQuery;

    private bool _getClustersTested = false;
    private bool _createClusterTested = false;
    private bool _deleteClusterTested = false;
    private bool _getClusterTested = false;
    private bool _getClusterConfigsTested = false;
    private bool _deleteClusterConfigTested = false;
    private bool _getClusterConfigTested = false;
    //private bool _saveConfigTested = false;
    private bool _uploadClusterConfigTested = false;
    private bool _getCollectionsTested = false;
    private bool _createCollectionTested = false;
    private bool _deleteCollectionTested = false;
    private bool _getRankersTested = false;
    private bool _indexDocumentsTested = false;
    private bool _createRankerTested = false;
    private bool _rankTested = false;
    private bool _getRankerTested = false;
    private bool _deleteRankerTested = false;
    private bool _searchStandardTested = false;

    private bool _readyToContinue = false;
    private string _clusterToDelete;
    private bool _isClusterReady;
    private bool _isRankerReady;
    private string _collectionNameToDelete;
    private float _waitTime = 5f;

    void Start()
    {
        LogSystem.InstallDefaultReactors();
        
        _testClusterConfigName = "cranfield_solr_config";
        _testClusterConfigPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/RetrieveAndRank/cranfield_solr_config.zip";
        _testRankerTrainingPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/RetrieveAndRank/ranker_training_data.csv";
        _testAnswerDataPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/RetrieveAndRank/ranker_answer_data.csv";
        _indexDataPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/RetrieveAndRank/cranfield_data.json";
        _testQuery = "What is the basic mechanisim of the transonic aileron buzz";
        _collectionNameToDelete = "TestCollectionToDelete";
        _createdRankerName = "RankerToDelete";

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_username, _password, _url);

        _retrieveAndRank = new RetrieveAndRank(credentials);

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        //  Get clusters
        Log.Debug("ExampleRetrieveAndRank.Examples()", "Attempting to get clusters.");
        if (!_retrieveAndRank.GetClusters(OnGetClusters, OnFail))
            Log.Debug("ExampleRetrieveAndRank.GetClusters()", "Failed to get clusters!");
        while (!_getClustersTested || !_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Create cluster
        Log.Debug("ExampleRetrieveAndRank.Examples()", "Attempting to create cluster.");
        if (!_retrieveAndRank.CreateCluster(OnCreateCluster, OnFail, "unity-test-cluster", "1"))
            Log.Debug("ExampleRetrieveAndRank.CreateCluster()", "Failed to create cluster!");
        while (!_createClusterTested || !_readyToContinue)
            yield return null;

        //  Wait for cluster status to be `READY`.
        CheckClusterStatus();
        while (!_isClusterReady)
            yield return null;
        
        _readyToContinue = false;
        //  List cluster configs
        Log.Debug("ExampleRetrieveAndRank.Examples()", "Attempting to get cluster configs.");
        if (!_retrieveAndRank.GetClusterConfigs(OnGetClusterConfigs, OnFail, _clusterToDelete))
            Log.Debug("ExampleRetrieveAndRank.GetClusterConfigs()", "Failed to get cluster configs!");
        while (!_getClusterConfigsTested || !_readyToContinue)
            yield return null;
        
        _readyToContinue = false;
        //  Upload cluster config
        Log.Debug("ExampleRetrieveAndRank.Examples()", "Attempting to upload cluster config.");
        if (!_retrieveAndRank.UploadClusterConfig(OnUploadClusterConfig, OnFail, _clusterToDelete, _testClusterConfigName, _testClusterConfigPath))
            Log.Debug("ExampleRetrieveAndRank.UploadClusterConfig()", "Failed to upload cluster config {0}!", _testClusterConfigPath);
        while (!_uploadClusterConfigTested || !_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Get cluster
        Log.Debug("ExampleRetrieveAndRank.Examples()", "Attempting to get cluster.");
        if (!_retrieveAndRank.GetCluster(OnGetCluster, OnFail, _clusterToDelete))
            Log.Debug("ExampleRetrieveAndRank.GetCluster()", "Failed to get cluster!");
        while (!_getClusterTested || !_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Get cluster config
        Log.Debug("ExampleRetrieveAndRank.Examples()", "Attempting to get cluster config.");
        if (!_retrieveAndRank.GetClusterConfig(OnGetClusterConfig, OnFail, _clusterToDelete, _testClusterConfigName))
            Log.Debug("ExampleRetrieveAndRank.GetClusterConfig()", "Failed to get cluster config {0}!", _testClusterConfigName);
        while (!_getClusterConfigTested || !_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  List Collection request
        Log.Debug("ExampleRetrieveAndRank.Examples()", "Attempting to get collections.");
        if (!_retrieveAndRank.ForwardCollectionRequest(OnGetCollections, OnFail, _clusterToDelete, CollectionsAction.List))
            Log.Debug("ExampleRetrieveAndRank.ForwardCollectionRequest()", "Failed to get collections!");
        while (!_getCollectionsTested || !_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Create Collection request
        Log.Debug("ExampleRetrieveAndRank.Examples()", "Attempting to create collection.");
        if (!_retrieveAndRank.ForwardCollectionRequest(OnCreateCollection, OnFail, _clusterToDelete, CollectionsAction.Create, _collectionNameToDelete, _testClusterConfigName))
            Log.Debug("ExampleRetrieveAndRank.ForwardCollectionRequest()", "Failed to create collections!");
        while (!_createCollectionTested || !_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Index documents
        Log.Debug("ExampleRetrieveAndRank.Examples()", "Attempting to index documents.");
        if (!_retrieveAndRank.IndexDocuments(OnIndexDocuments, OnFail, _indexDataPath, _clusterToDelete, _collectionNameToDelete))
            Log.Debug("ExampleRetrieveAndRank.IndexDocuments()", "Failed to index documents!");
        while (!_indexDocumentsTested || !_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Get rankers
        Log.Debug("ExampleRetrieveAndRank.Examples()", "Attempting to get rankers.");
        if (!_retrieveAndRank.GetRankers(OnGetRankers, OnFail))
            Log.Debug("ExampleRetrieveAndRank.GetRankers()", "Failed to get rankers!");
        while (!_getRankersTested || !_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Create ranker
        Log.Debug("ExampleRetrieveAndRank.Examples()", "Attempting to create ranker.");
        if (!_retrieveAndRank.CreateRanker(OnCreateRanker, OnFail, _testRankerTrainingPath, _createdRankerName))
            Log.Debug("ExampleRetrieveAndRank.CreateRanker()", "Failed to create ranker!");
        while (!_createRankerTested || !_readyToContinue)
            yield return null;

        //  Wait for ranker status to be `Available`.
        Log.Debug("ExampleRetrieveAndRank.Examples()", "Checking ranker status in 10 seconds");
        CheckRankerStatus();
        while (!_isRankerReady || !_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Standard Search
        string[] fl = { "title", "id", "body", "author", "bibliography" };
        Log.Debug("ExampleRetrieveAndRank.Examples()", "Attempting to search standard.");
        if (!_retrieveAndRank.Search(OnSearchStandard, OnFail, _clusterToDelete, _collectionNameToDelete, _testQuery, fl))
            Log.Debug("ExampleRetrieveAndRank.Search()", "Failed to search!");
        while (!_searchStandardTested || !_readyToContinue)
            yield return null;
        
        _readyToContinue = false;
        //  Rank
        Log.Debug("ExampleRetrieveAndRank.Examples()", "Attempting to rank.");
        if (!_retrieveAndRank.Rank(OnRank, OnFail, _rankerIdToDelete, _testAnswerDataPath))
            Log.Debug("ExampleRetrieveAndRank.Rank()", "Failed to rank!");
        while (!_rankTested || !_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Get ranker info
        Log.Debug("ExampleRetrieveAndRank.Examples()", "Attempting to get rankers.");
        if (!_retrieveAndRank.GetRanker(OnGetRanker, OnFail, _rankerIdToDelete))
            Log.Debug("ExampleRetrieveAndRank.GetRanker()", "Failed to get ranker!");
        while (!_getRankerTested)
            yield return null;
        
        _readyToContinue = false;
        //  Delete rankers
        Log.Debug("ExampleRetrieveAndRank.Examples()", "Attempting to delete ranker {0}.", _rankerIdToDelete);
        if (!_retrieveAndRank.DeleteRanker(OnDeleteRanker, OnFail, _rankerIdToDelete))
            Log.Debug("ExampleRetrieveAndRank.DeleteRanker()", "Failed to delete ranker {0}!", _rankerIdToDelete);
        while (!_deleteRankerTested || !_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Delete Collection request
        Log.Debug("ExampleRetrieveAndRank.Examples()", "Attempting to delete collection {0}.", "TestCollectionToDelete");
        if (!_retrieveAndRank.ForwardCollectionRequest(OnDeleteCollection, OnFail, _clusterToDelete, CollectionsAction.Delete, "TestCollectionToDelete"))
            Log.Debug("ExampleRetrieveAndRank.ForwardCollectionRequest()", "Failed to delete collections!");
        while (!_deleteCollectionTested || !_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Delete cluster config
        string clusterConfigToDelete = "test-config";
        Log.Debug("ExampleRetrieveAndRank.Examples()", "Attempting to delete cluster config.");
        if (!_retrieveAndRank.DeleteClusterConfig(OnDeleteClusterConfig, OnFail, _clusterToDelete, clusterConfigToDelete))
            Log.Debug("ExampleRetrieveAndRank.DeleteClusterConfig()", "Failed to delete cluster config {0}", clusterConfigToDelete);
        while (!_deleteClusterConfigTested || !_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Delete cluster
        Log.Debug("ExampleRetrieveAndRank.Examples()", "Attempting to delete cluster {0}.", _clusterToDelete);
        if (!_retrieveAndRank.DeleteCluster(OnDeleteCluster, OnFail, _clusterToDelete))
            Log.Debug("ExampleRetrieveAndRank.DeleteCluster()", "Failed to delete cluster!");
        while (!_deleteClusterTested || !_readyToContinue)
            yield return null;

        Log.Debug("ExampleRetrieveAndRank.Examples()", "Retrieve and rank examples complete!");
    }

    private void OnGetClusters(SolrClusterListResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleRetrieveAndRank.OnGetClusters()", "Retrieve and rank - Get clusters response: {0}", customData["json"].ToString());
        _getClustersTested = true;
        Invoke("ReadyToContinue", _waitTime);
    }

    private void OnCreateCluster(SolrClusterResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleRetrieveAndRank.OnCreateCluster()", "Retrieve and rank - Create cluster response: {0}", customData["json"].ToString());
        _clusterToDelete = resp.solr_cluster_id;
        _createClusterTested = true;
        Invoke("ReadyToContinue", _waitTime);
    }

    private void OnDeleteCluster(bool success, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleRetrieveAndRank.OnDeleteCluster()", "Retrieve and rank - Delete cluster response: {0}", customData["json"].ToString());
        _deleteClusterTested = true;
        Invoke("ReadyToContinue", _waitTime);
    }

    private void OnGetCluster(SolrClusterResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleRetrieveAndRank.OnGetCluster()", "Retrieve and rank - Get cluster response: {0}", customData["json"].ToString());
        _getClusterTested = true;
        Invoke("ReadyToContinue", _waitTime);
    }

    private void OnGetClusterConfigs(SolrConfigList resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleRetrieveAndRank.OnGetClusterConfigs()", "Retrieve and rank - Get cluster config response: {0}", customData["json"].ToString());
        _getClusterConfigsTested = true;
        Invoke("ReadyToContinue", _waitTime);
    }

    private void OnDeleteClusterConfig(bool success, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleRetrieveAndRank.OnDeleteClusterConfig()", "Retrieve and rank - Deletecluster config response: {0}", customData["json"].ToString());
        _deleteClusterConfigTested = true;
        Invoke("ReadyToContinue", _waitTime);
    }


    private void OnGetClusterConfig(byte[] respData, Dictionary<string, object> customData)
    {
#if UNITY_EDITOR
        Log.Debug("ExampleRetrieveAndRank.OnGetClusterConfig()", "Retrieve and rank - Get cluster config response: {0}", customData["json"].ToString());
#else
		Log.Debug("ExampleRetrieveAndRank.OnGetClusterConfig()", "Not in editor - skipping download.");
#endif
        _getClusterConfigTested = true;
        Invoke("ReadyToContinue", _waitTime);
    }

    private void OnUploadClusterConfig(UploadResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleRetrieveAndRank.OnUploadClusterConfig()", "Retrieve and rank - Upload cluster config response: {0}", customData["json"].ToString());
        _uploadClusterConfigTested = true;
        Invoke("ReadyToContinue", _waitTime);
    }

    private void OnGetCollections(CollectionsResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleRetrieveAndRank.OnGetCollections()", "Retrieve and rank - Get collections response: {0}", customData["json"].ToString());
        _getCollectionsTested = true;
        Invoke("ReadyToContinue", _waitTime);
    }

    private void OnCreateCollection(CollectionsResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleRetrieveAndRank.OnCreateCollection()", "Retrieve and rank - Get collections response: {0}", customData["json"].ToString());
        _createCollectionTested = true;
        Invoke("ReadyToContinue", _waitTime);
    }

    private void OnDeleteCollection(CollectionsResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleRetrieveAndRank.OnDeleteCollection()", "Retrieve and rank - Get collections response: {0}", customData["json"].ToString());
        _deleteCollectionTested = true;
        Invoke("ReadyToContinue", _waitTime);
    }

    private void OnIndexDocuments(IndexResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleRetrieveAndRank.OnIndexDocuments()", "Retrieve and rank - Index documents response: {0}", customData["json"].ToString());
        _indexDocumentsTested = true;
        Invoke("ReadyToContinue", _waitTime);
    }

    private void OnGetRankers(ListRankersPayload resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleRetrieveAndRank.OnGetRankers()", "Retrieve and rank - Get rankers response: {0}", customData["json"].ToString());
        _getRankersTested = true;
        Invoke("ReadyToContinue", _waitTime);
    }

    private void OnCreateRanker(RankerStatusPayload resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleRetrieveAndRank.OnCreateRanker()", "Retrieve and rank - Create ranker response: {0}", customData["json"].ToString());
        _rankerIdToDelete = resp.ranker_id;
        _createRankerTested = true;
        Invoke("ReadyToContinue", _waitTime);
    }

    private void OnRank(RankerOutputPayload resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleRetrieveAndRank.OnRank()", "Retrieve and rank - Rank response: {0}", customData["json"].ToString());
        _rankTested = true;
        Invoke("ReadyToContinue", _waitTime);
    }

    private void OnGetRanker(RankerStatusPayload resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleRetrieveAndRank.OnGetRanker()", "Retrieve and rank - Get ranker response: {0}", customData["json"].ToString());
        _getRankerTested = true;
        Invoke("ReadyToContinue", _waitTime);
    }

    private void OnDeleteRanker(bool success, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleRetrieveAndRank.OnDeleteRanker()", "Retrieve and rank - Delete ranker response: {0}", customData["json"].ToString());
        _deleteRankerTested = true;
        Invoke("ReadyToContinue", _waitTime);
    }

    private void OnSearchStandard(SearchResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleRetrieveAndRank.OnSearchStandard()", "Retrieve and rank - Search standard response: {0}", customData["json"].ToString());
        _searchStandardTested = true;
        Invoke("ReadyToContinue", _waitTime);
    }

    private void CheckClusterStatus()
    {
        if (!_retrieveAndRank.GetCluster((SolrClusterResponse resp, Dictionary<string, object> customData) =>
         {
             Log.Debug("ExampleRetrieveAndRank.CheckClusterStatus()", "Solr cluster status is '{0}'", resp.solr_cluster_status);
             if (resp.solr_cluster_status.ToLower() != "ready")
             {
                 Log.Debug("ExampleRetrieveAndRank.CheckClusterStatus()", "Checking cluster status in 10 seconds");
                 Invoke("CheckClusterStatus", 10f);
             }
             else
             {
                 _isClusterReady = true;
             }
         }, OnFail, _clusterToDelete))
            Log.Debug("ExampleRetrieveAndRank.CheckClusterStatus()", "Failed to get cluster");
    }

    private void CheckRankerStatus()
    {
        if (!_retrieveAndRank.GetRanker((RankerStatusPayload resp, Dictionary<string, object> customData) =>
        {
            Log.Debug("ExampleRetrieveAndRank.CheckRankerStatus()", "Solr ranker status is '{0}'", resp.status);
            if (resp.status.ToLower() != "available")
            {
                Log.Debug("ExampleRetrieveAndRank.CheckRankerStatus()", "Checking ranker status in 10 seconds");
                Invoke("CheckRankerStatus", 10f);
            }
            else
            {
                _isRankerReady = true;
            }
        }, OnFail, _rankerIdToDelete))
            Log.Debug("ExampleRetrieveAndRank.CheckRankerStatus()", "Failed to get ranker");
    }

    private void ReadyToContinue()
    {
        _readyToContinue = true;
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("ExampleRetrieveAndRank.OnFail()", "Error received: {0}", error.ToString());
    }
}
