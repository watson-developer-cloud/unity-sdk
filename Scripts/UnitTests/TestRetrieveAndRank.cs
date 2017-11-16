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
using System.IO;
using FullSerializer;
using System;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Connection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestRetrieveAndRank : UnitTest
    {
        private string _username = null;
        private string _password = null;
        private fsSerializer _serializer = new fsSerializer();
        //private string _token = "<authentication-token>";

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

        public override IEnumerator RunTest()
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

            try
            {
                VcapCredentials vcapCredentials = new VcapCredentials();
                fsData data = null;

                //  Get credentials from a credential file defined in environmental variables in the VCAP_SERVICES format. 
                //  See https://www.ibm.com/watson/developercloud/doc/common/getting-started-variables.html.
                var environmentalVariable = Environment.GetEnvironmentVariable("VCAP_SERVICES");
                var fileContent = File.ReadAllText(environmentalVariable);

                //  Add in a parent object because Unity does not like to deserialize root level collection types.
                fileContent = Utility.AddTopLevelObjectToJson(fileContent, "VCAP_SERVICES");

                //  Convert json to fsResult
                fsResult r = fsJsonParser.Parse(fileContent, out data);
                if (!r.Succeeded)
                    throw new WatsonException(r.FormattedMessages);

                //  Convert fsResult to VcapCredentials
                object obj = vcapCredentials;
                r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                if (!r.Succeeded)
                    throw new WatsonException(r.FormattedMessages);

                //  Set credentials from imported credntials
                Credential credential = vcapCredentials.VCAP_SERVICES["retrieve_and_rank"][TestCredentialIndex].Credentials;
                _username = credential.Username.ToString();
                _password = credential.Password.ToString();
                _url = credential.Url.ToString();
            }
            catch
            {
                Log.Debug("TestRetrieveAndRank.RunTest()", "Failed to get credentials from VCAP_SERVICES file. Please configure credentials to run this test. For more information, see: https://github.com/watson-developer-cloud/unity-sdk/#authentication");
            }

            //  Create credential and instantiate service
            Credentials credentials = new Credentials(_username, _password, _url);

            //  Or authenticate using token
            //Credentials credentials = new Credentials(_url)
            //{
            //    AuthenticationToken = _token
            //};

            _retrieveAndRank = new RetrieveAndRank(credentials);

            //  Get clusters
            Log.Debug("TestRetrieveAndRank.RunTest()", "Attempting to get clusters.");
            if (!_retrieveAndRank.GetClusters(OnGetClusters, OnFail))
                Log.Debug("TestRetrieveAndRank.GetClusters()", "Failed to get clusters!");
            while (!_getClustersTested || !_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Create cluster
            Log.Debug("TestRetrieveAndRank.RunTest()", "Attempting to create cluster.");
            if (!_retrieveAndRank.CreateCluster(OnCreateCluster, OnFail, "unity-test-cluster", "1"))
                Log.Debug("TestRetrieveAndRank.CreateCluster()", "Failed to create cluster!");
            while (!_createClusterTested || !_readyToContinue)
                yield return null;

            //  Wait for cluster status to be `READY`.
            Runnable.Run(CheckClusterStatus(10f));
            while (!_isClusterReady)
                yield return null;

            _readyToContinue = false;
            //  List cluster configs
            Log.Debug("TestRetrieveAndRank.RunTest()", "Attempting to get cluster configs.");
            if (!_retrieveAndRank.GetClusterConfigs(OnGetClusterConfigs, OnFail, _clusterToDelete))
                Log.Debug("TestRetrieveAndRank.GetClusterConfigs()", "Failed to get cluster configs!");
            while (!_getClusterConfigsTested || !_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Upload cluster config
            Log.Debug("TestRetrieveAndRank.RunTest()", "Attempting to upload cluster config.");
            if (!_retrieveAndRank.UploadClusterConfig(OnUploadClusterConfig, OnFail, _clusterToDelete, _testClusterConfigName, _testClusterConfigPath))
                Log.Debug("TestRetrieveAndRank.UploadClusterConfig()", "Failed to upload cluster config {0}!", _testClusterConfigPath);
            while (!_uploadClusterConfigTested || !_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Get cluster
            Log.Debug("TestRetrieveAndRank.RunTest()", "Attempting to get cluster.");
            if (!_retrieveAndRank.GetCluster(OnGetCluster, OnFail, _clusterToDelete))
                Log.Debug("TestRetrieveAndRank.GetCluster()", "Failed to get cluster!");
            while (!_getClusterTested || !_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Get cluster config
            Log.Debug("TestRetrieveAndRank.RunTest()", "Attempting to get cluster config.");
            if (!_retrieveAndRank.GetClusterConfig(OnGetClusterConfig, OnFail, _clusterToDelete, _testClusterConfigName))
                Log.Debug("TestRetrieveAndRank.GetClusterConfig()", "Failed to get cluster config {0}!", _testClusterConfigName);
            while (!_getClusterConfigTested || !_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  List Collection request
            Log.Debug("TestRetrieveAndRank.RunTest()", "Attempting to get collections.");
            if (!_retrieveAndRank.ForwardCollectionRequest(OnGetCollections, OnFail, _clusterToDelete, CollectionsAction.List))
                Log.Debug("TestRetrieveAndRank.ForwardCollectionRequest()", "Failed to get collections!");
            while (!_getCollectionsTested || !_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Create Collection request
            Log.Debug("TestRetrieveAndRank.RunTest()", "Attempting to create collection.");
            if (!_retrieveAndRank.ForwardCollectionRequest(OnCreateCollection, OnFail, _clusterToDelete, CollectionsAction.Create, _collectionNameToDelete, _testClusterConfigName))
                Log.Debug("TestRetrieveAndRank.ForwardCollectionRequest()", "Failed to create collections!");
            while (!_createCollectionTested || !_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Index documents
            Log.Debug("TestRetrieveAndRank.RunTest()", "Attempting to index documents.");
            if (!_retrieveAndRank.IndexDocuments(OnIndexDocuments, OnFail, _indexDataPath, _clusterToDelete, _collectionNameToDelete))
                Log.Debug("TestRetrieveAndRank.IndexDocuments()", "Failed to index documents!");
            while (!_indexDocumentsTested || !_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Get rankers
            Log.Debug("TestRetrieveAndRank.RunTest()", "Attempting to get rankers.");
            if (!_retrieveAndRank.GetRankers(OnGetRankers, OnFail))
                Log.Debug("TestRetrieveAndRank.GetRankers()", "Failed to get rankers!");
            while (!_getRankersTested || !_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Create ranker
            Log.Debug("TestRetrieveAndRank.RunTest()", "Attempting to create ranker.");
            if (!_retrieveAndRank.CreateRanker(OnCreateRanker, OnFail, _testRankerTrainingPath, _createdRankerName))
                Log.Debug("TestRetrieveAndRank.CreateRanker()", "Failed to create ranker!");
            while (!_createRankerTested || !_readyToContinue)
                yield return null;

            //  Wait for ranker status to be `Available`.
            Log.Debug("TestRetrieveAndRank.RunTest()", "Checking ranker status in 10 seconds");
            Runnable.Run(CheckRankerStatus(10f));
            while (!_isRankerReady || !_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Standard Search
            string[] fl = { "title", "id", "body", "author", "bibliography" };
            Log.Debug("TestRetrieveAndRank.RunTest()", "Attempting to search standard.");
            if (!_retrieveAndRank.Search(OnSearchStandard, OnFail, _clusterToDelete, _collectionNameToDelete, _testQuery, fl))
                Log.Debug("TestRetrieveAndRank.Search()", "Failed to search!");
            while (!_searchStandardTested || !_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Rank
            Log.Debug("TestRetrieveAndRank.RunTest()", "Attempting to rank.");
            if (!_retrieveAndRank.Rank(OnRank, OnFail, _rankerIdToDelete, _testAnswerDataPath))
                Log.Debug("TestRetrieveAndRank.Rank()", "Failed to rank!");
            while (!_rankTested || !_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Get ranker info
            Log.Debug("TestRetrieveAndRank.RunTest()", "Attempting to get rankers.");
            if (!_retrieveAndRank.GetRanker(OnGetRanker, OnFail, _rankerIdToDelete))
                Log.Debug("TestRetrieveAndRank.GetRanker()", "Failed to get ranker!");
            while (!_getRankerTested)
                yield return null;

            _readyToContinue = false;
            //  Delete rankers
            Log.Debug("TestRetrieveAndRank.RunTest()", "Attempting to delete ranker {0}.", _rankerIdToDelete);
            if (!_retrieveAndRank.DeleteRanker(OnDeleteRanker, OnFail, _rankerIdToDelete))
                Log.Debug("TestRetrieveAndRank.DeleteRanker()", "Failed to delete ranker {0}!", _rankerIdToDelete);
            while (!_deleteRankerTested || !_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Delete Collection request
            Log.Debug("TestRetrieveAndRank.RunTest()", "Attempting to delete collection {0}.", "TestCollectionToDelete");
            if (!_retrieveAndRank.ForwardCollectionRequest(OnDeleteCollection, OnFail, _clusterToDelete, CollectionsAction.Delete, "TestCollectionToDelete"))
                Log.Debug("TestRetrieveAndRank.ForwardCollectionRequest()", "Failed to delete collections!");
            while (!_deleteCollectionTested || !_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Delete cluster config
            string clusterConfigToDelete = "test-config";
            Log.Debug("TestRetrieveAndRank.RunTest()", "Attempting to delete cluster config.");
            if (!_retrieveAndRank.DeleteClusterConfig(OnDeleteClusterConfig, OnFail, _clusterToDelete, clusterConfigToDelete))
                Log.Debug("TestRetrieveAndRank.DeleteClusterConfig()", "Failed to delete cluster config {0}", clusterConfigToDelete);
            while (!_deleteClusterConfigTested || !_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Delete cluster
            Log.Debug("TestRetrieveAndRank.RunTest()", "Attempting to delete cluster {0}.", _clusterToDelete);
            if (!_retrieveAndRank.DeleteCluster(OnDeleteCluster, OnFail, _clusterToDelete))
                Log.Debug("TestRetrieveAndRank.DeleteCluster()", "Failed to delete cluster!");
            while (!_deleteClusterTested || !_readyToContinue)
                yield return null;

            Log.Debug("TestRetrieveAndRank.RunTest()", "Retrieve and rank examples complete!");
            yield break;
        }

        private void OnGetClusters(SolrClusterListResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestRetrieveAndRank.OnGetClusters()", "{0}", customData["json"].ToString());
            Test(resp != null);
            _getClustersTested = true;
            Runnable.Run(ReadyToContinue(_waitTime));
        }

        private void OnCreateCluster(SolrClusterResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestRetrieveAndRank.OnCreateCluster()", "{0}", customData["json"].ToString());
            _clusterToDelete = resp.solr_cluster_id;
            Test(resp != null);
            _createClusterTested = true;
            Runnable.Run(ReadyToContinue(_waitTime));
        }

        private void OnDeleteCluster(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("TestRetrieveAndRank.OnDeleteCluster()", "{0}", customData["json"].ToString());
            Test(success);
            _deleteClusterTested = true;
            Runnable.Run(ReadyToContinue(_waitTime));
        }

        private void OnGetCluster(SolrClusterResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestRetrieveAndRank.OnGetCluster()", "{0}", customData["json"].ToString());
            Test(resp != null);
            _getClusterTested = true;
            Runnable.Run(ReadyToContinue(_waitTime));
        }

        private void OnGetClusterConfigs(SolrConfigList resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestRetrieveAndRank.OnGetClusterConfigs()", "{0}", customData["json"].ToString());
            Test(resp != null);
            _getClusterConfigsTested = true;
            Runnable.Run(ReadyToContinue(_waitTime));
        }

        private void OnDeleteClusterConfig(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("TestRetrieveAndRank.OnDeleteClusterConfig()", "{0}", customData["json"].ToString());
            Test(success);
            _deleteClusterConfigTested = true;
            Runnable.Run(ReadyToContinue(_waitTime));
        }
        
        private void OnGetClusterConfig(byte[] respData, Dictionary<string, object> customData)
        {
#if UNITY_EDITOR
            Log.Debug("TestRetrieveAndRank.OnGetClusterConfig()", "{0}", customData["json"].ToString());
#else
		Log.Debug("TestRetrieveAndRank.RunTest()", "Not in editor - skipping download.");
#endif
            Test(respData != null);
            _getClusterConfigTested = true;
            Runnable.Run(ReadyToContinue(_waitTime));
        }

        //private void OnSaveConfig(bool success, Dictionary<string, object> customData)
        //{
        //    Log.Debug("TestRetrieveAndRank.OnSaveConfig()", "{0}", customData["json"].ToString());
        //Test(success);
        //    _saveConfigTested = true;
        //    Runnable.Run(ReadyToContinue(_waitTime));
        //}

        private void OnUploadClusterConfig(UploadResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestRetrieveAndRank.OnUploadClusterConfig()", "{0}", customData["json"].ToString());
            Test(resp != null);
            _uploadClusterConfigTested = true;
            Runnable.Run(ReadyToContinue(_waitTime));
        }

        private void OnGetCollections(CollectionsResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestRetrieveAndRank.OnGetCollections()", "{0}", customData["json"].ToString());
            Test(resp != null);
            _getCollectionsTested = true;
            Runnable.Run(ReadyToContinue(_waitTime));
        }

        private void OnCreateCollection(CollectionsResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestRetrieveAndRank.OnCreateCollection()", "{0}", customData["json"].ToString());
            Test(resp != null);
            _createCollectionTested = true;
            Runnable.Run(ReadyToContinue(_waitTime));
        }

        private void OnDeleteCollection(CollectionsResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestRetrieveAndRank.OnDeleteCollection()", "{0}", customData["json"].ToString());
            Test(resp != null);
            _deleteCollectionTested = true;
            Runnable.Run(ReadyToContinue(_waitTime));
        }

        private void OnIndexDocuments(IndexResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestRetrieveAndRank.OnIndexDocuments()", "{0}", customData["json"].ToString());
            Test(resp != null);
            _indexDocumentsTested = true;
            Runnable.Run(ReadyToContinue(_waitTime));
        }

        private void OnGetRankers(ListRankersPayload resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestRetrieveAndRank.OnGetRankers()", "{0}", customData["json"].ToString());
            Test(resp != null);
            _getRankersTested = true;
            Runnable.Run(ReadyToContinue(_waitTime));
        }

        private void OnCreateRanker(RankerStatusPayload resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestRetrieveAndRank.OnCreateRanker()", "{0}", customData["json"].ToString());
            _rankerIdToDelete = resp.ranker_id;
            Test(resp != null);
            _createRankerTested = true;
            Runnable.Run(ReadyToContinue(_waitTime));
        }

        private void OnRank(RankerOutputPayload resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestRetrieveAndRank.OnRank()", "{0}", customData["json"].ToString());
            Test(resp != null);
            _rankTested = true;
            Runnable.Run(ReadyToContinue(_waitTime));
        }

        private void OnGetRanker(RankerStatusPayload resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestRetrieveAndRank.OnGetRanker()", "{0}", customData["json"].ToString());
            Test(resp != null);
            _getRankerTested = true;
            Runnable.Run(ReadyToContinue(_waitTime));
        }

        private void OnDeleteRanker(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("TestRetrieveAndRank.OnDeleteRanker()", "{0}", customData["json"].ToString());
            Test(success);
            _deleteRankerTested = true;
            Runnable.Run(ReadyToContinue(_waitTime));
        }

        private void OnSearchStandard(SearchResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestRetrieveAndRank.OnSearchStandard()", "{0}", customData["json"].ToString());
            Test(resp != null);
            _searchStandardTested = true;
            Runnable.Run(ReadyToContinue(_waitTime));
        }

        private IEnumerator CheckClusterStatus(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            if (!_retrieveAndRank.GetCluster((SolrClusterResponse resp, Dictionary<string, object> customData) =>
            {
                Log.Debug("TestRetrieveAndRank.CheckClusterStatus()", "Solr cluster status is '{0}'", resp.solr_cluster_status);
                if (resp.solr_cluster_status.ToLower() != "ready")
                {
                    Log.Debug("TestRetrieveAndRank.CheckClusterStatus()", "Checking cluster status in 10 seconds");
                    Runnable.Run(CheckClusterStatus(waitTime));
                }
                else
                {
                    _isClusterReady = true;
                }
            }, OnFail, _clusterToDelete))
                Log.Debug("TestRetrieveAndRank.CheckClusterStatus()", "Failed to get cluster");
        }

        private IEnumerator CheckRankerStatus(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            if (!_retrieveAndRank.GetRanker((RankerStatusPayload resp, Dictionary<string, object> customData) =>
            {
                Log.Debug("TestRetrieveAndRank.CheckRankerStatus()", "Solr ranker status is '{0}'", resp.status);
                if (resp.status.ToLower() != "available")
                {
                    Log.Debug("TestRetrieveAndRank.CheckRankerStatus()", "Checking ranker status in 10 seconds");
                    Runnable.Run(CheckRankerStatus(10f));
                }
                else
                {
                    _isRankerReady = true;
                }
            }, OnFail, _rankerIdToDelete))
                Log.Debug("TestRetrieveAndRank.CheckRankerStatus()", "Failed to get ranker");
        }

        private IEnumerator ReadyToContinue(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            _readyToContinue = true;
        }

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("ExampleAlchemyLanguage.OnFail()", "Error received: {0}", error.ToString());
        }
    }
}
