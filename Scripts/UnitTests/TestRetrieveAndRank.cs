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
using System;
using IBM.Watson.DeveloperCloud.Logging;
using UnityEditor;
using System.IO;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestRetrieveAndRank : UnitTest
    {
        RetrieveAndRank m_RetrieveAndRank = new RetrieveAndRank();
        private bool m_getClustersTested = false;
        private bool m_createClusterTested = false;
        private bool m_deleteClusterTested = false;
        private bool m_getClusterTested = false;
        private bool m_listClusterConfigsTested = false;
        private bool m_deleteClusterConfigTested = false;
        private bool m_getClusterConfigTested = false;
        private bool m_uploadClusterConfigTested = false;
        private bool m_listCollectionRequestTested = false;
        private bool m_createCollectionRequestTested = false;
        private bool m_deleteCollectionRequestTested = false;
        private bool m_indexDocumentsTested = false;
        private bool m_standardSearchTested = false;
        private bool m_rankedSearchTested = false;
        private bool m_getRankersTested = false;
        private bool m_createRankerTested = false;
        private bool m_rankTested = false;
        private bool m_deleteRankersTested = false;
        private bool m_getRankerInfoTested = false;

        private string m_integrationTestClusterName = "unity-integration-test-cluster";
        private string m_integrationTestClusterID;
        private string m_integrationTestConfigName = "unity-integration-test-config";
        private string m_integrationTestCollectionName = "unity-integration-test-collection";
        private string m_integrationTestCollectionID;
        private string m_integrationTestRankerName = "unity-integration-test-ranker";
        private string m_integrationTestRankerID;

        private string m_integrationTestQuery = "What is the basic mechanisim of the transonic aileron buzz";

        private string[] fl = { "title", "id", "body", "author", "bibliography" };

        private string m_integrationTestClusterConfigPath;
        private string m_integrationTestRankerTrainingPath;
        private string m_integrationTestRankerAnswerDataPath;
        private string m_integrationTestIndexDataPath;

        private bool m_isClusterReady = false;

        public override IEnumerator RunTest()
        {
            m_integrationTestClusterConfigPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/RetrieveAndRank/cranfield_solr_config.zip";
            m_integrationTestRankerTrainingPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/RetrieveAndRank/ranker_training_data.csv";
            m_integrationTestRankerAnswerDataPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/RetrieveAndRank/ranker_answer_data.csv";
            m_integrationTestIndexDataPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/RetrieveAndRank/cranfield_data.json";

            //  Create cluster
            Log.Debug("TestRetrieveAndRank", "Attempting to create cluster!");
            m_RetrieveAndRank.CreateCluster(OnCreateCluster, m_integrationTestClusterName, "1");
            while (!m_createClusterTested)
                yield return null;

            //  Get clusters
            Log.Debug("TestRetrieveAndRank", "Attempting to get clusters!");
            m_RetrieveAndRank.GetClusters(OnGetClusters);
            while (!m_getClustersTested)
                yield return null;

            //  Get cluster
            Log.Debug("TestRetrieveAndRank", "Attempting to get cluster {0}!", m_integrationTestClusterID);
            m_RetrieveAndRank.GetCluster(OnGetCluster, m_integrationTestClusterID);
            while (!m_getClustersTested || !m_isClusterReady)
                yield return null;

            //  Upload cluster config
            Log.Debug("TestRetrieveAndRank", "Attempting to upload cluster {0} config {1}!", m_integrationTestClusterID, m_integrationTestConfigName);
            m_RetrieveAndRank.UploadClusterConfig(OnUploadClusterConfig, m_integrationTestClusterID, m_integrationTestConfigName, m_integrationTestClusterConfigPath);
            while (!m_uploadClusterConfigTested)
                yield return null;

            //  List cluster configs
            Log.Debug("TestRetrieveAndRank", "Attempting to get cluster configs for {0}!", m_integrationTestClusterID);
            m_RetrieveAndRank.GetClusterConfigs(OnGetClusterConfigs, m_integrationTestClusterID);
            while (!m_listClusterConfigsTested)
                yield return null;

            //  Get cluster config
            Log.Debug("TestRetrieveAndRank", "Attempting to get cluster {0} config {1}!", m_integrationTestClusterID, m_integrationTestConfigName);
            m_RetrieveAndRank.GetClusterConfig(OnGetClusterConfig, m_integrationTestClusterID, m_integrationTestConfigName);
            while (!m_getClusterConfigTested)
                yield return null;

            //  List Collection request
            Log.Debug("TestRetrieveAndRank", "Attempting to list collections!");
            m_RetrieveAndRank.ForwardCollectionRequest(OnListCollections, m_integrationTestClusterID, CollectionsAction.LIST);
            while (!m_listCollectionRequestTested)
                yield return null;

            //  Create Collection request
            Log.Debug("TestRetrieveAndRank", "Attempting to create collection!");
            m_RetrieveAndRank.ForwardCollectionRequest(OnCreateCollections, m_integrationTestClusterID, CollectionsAction.CREATE, m_integrationTestCollectionName, m_integrationTestConfigName);
            while (!m_createCollectionRequestTested)
                yield return null;

            //  Index documents
            Log.Debug("TestRetrieveAndRank", "Attempting to index documents!");
            m_RetrieveAndRank.IndexDocuments(OnIndexDocuments, m_integrationTestIndexDataPath, m_integrationTestClusterID, m_integrationTestCollectionName);
            while (!m_indexDocumentsTested)
                yield return null;

            //  Standard Search
            Log.Debug("TestRetrieveAndRank", "Attempting to search!");
            m_RetrieveAndRank.Search(OnStandardSearch, m_integrationTestClusterID, m_integrationTestCollectionName, m_integrationTestQuery, fl);
            while (!m_standardSearchTested)
                yield return null;

            //  Ranked Search
            Log.Debug("TestRetrieveAndRank", "Attempting to search!");
            m_RetrieveAndRank.Search(OnRankedSearch, m_integrationTestClusterID, m_integrationTestCollectionName, m_integrationTestQuery, fl, true);
            while (!m_rankedSearchTested)
                yield return null;

            //  Get rankers
            Log.Debug("TestRetrieveAndRank", "Attempting to get rankers!");
            m_RetrieveAndRank.GetRankers(OnGetRankers);
            while (!m_getRankersTested)
                yield return null;

            //  Create ranker
            Log.Debug("TestRetrieveAndRank", "Attempting to create rankers!");
            m_RetrieveAndRank.CreateRanker(OnCreateRanker, m_integrationTestRankerTrainingPath, m_integrationTestRankerName);
            while (!m_createRankerTested)
                yield return null;

            //  Rank
            Log.Debug("TestRetrieveAndRank", "Attempting to rank!");
            m_RetrieveAndRank.Rank(OnRank, m_integrationTestRankerID, m_integrationTestRankerAnswerDataPath);
            while (!m_rankTested)
                yield return null;

            //  Delete rankers
            Log.Debug("ExampleRetriveAndRank", "Attempting to delete ranker {0}!", m_integrationTestRankerName);
            m_RetrieveAndRank.DeleteRanker(OnDeleteRanker, m_integrationTestRankerName);
            while (!m_deleteRankersTested)
                yield return null;

            //  Get ranker info
            Log.Debug("TestRetrieveAndRank", "Attempting to get Ranker Info!");
            m_RetrieveAndRank.GetRanker(OnGetRanker, m_integrationTestRankerName);
            while (!m_getRankerInfoTested)
                yield return null;

            //  Delete Collection request
            Log.Debug("TestRetrieveAndRank", "Attempting to delete collection!");
            m_RetrieveAndRank.ForwardCollectionRequest(OnDeleteCollections, m_integrationTestClusterID, CollectionsAction.DELETE, m_integrationTestCollectionName);
            while (!m_deleteCollectionRequestTested)
                yield return null;

            //  Delete cluster config
            Log.Debug("TestRetrieveAndRank", "Attempting to delete cluster {0} config {1}!", m_integrationTestClusterID, m_integrationTestConfigName);
            m_RetrieveAndRank.DeleteClusterConfig(OnDeleteClusterConfig, m_integrationTestClusterID, m_integrationTestConfigName);
            while (!m_deleteClusterConfigTested)
                yield return null;

            //  Delete cluster
            Log.Debug("TestRetrieveAndRank", "Attempting to delete cluster {0}!", m_integrationTestClusterID);
            m_RetrieveAndRank.DeleteCluster(OnDeleteCluster, m_integrationTestClusterID);
            while (!m_deleteClusterTested)
                yield return null;

            yield break;
        }

        private void OnGetClusters(SolrClusterListResponse resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                foreach (SolrClusterResponse cluster in resp.clusters)
                    Log.Debug("TestRetrieveAndRank", "OnGetClusters | cluster name: {0}, size: {1}, ID: {2}, status: {3}.", cluster.cluster_name, cluster.cluster_size, cluster.solr_cluster_id, cluster.solr_cluster_status);
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "OnGetClusters | Get Cluster Response is null!");
            }

            m_getClustersTested = true;
        }

        private void OnCreateCluster(SolrClusterResponse resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                Log.Debug("TestRetrieveAndRank", "OnCreateClusters | name: {0}, size: {1}, ID: {2}, status: {3}.", resp.cluster_name, resp.cluster_size, resp.solr_cluster_id, resp.solr_cluster_status);
                m_integrationTestClusterID = resp.solr_cluster_id;
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "OnCreateClusters | Get Cluster Response is null!");
            }

            m_createClusterTested = true;
        }

        private void OnDeleteCluster(bool success, string data)
        {
            Test(success);

            if (success)
            {
                Log.Debug("TestRetrieveAndRank", "OnDeleteCluster | Success!");
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "OnDeleteCluster | Failure!");
            }

            m_deleteClusterTested = true;
        }

        private void OnGetCluster(SolrClusterResponse resp, string data)
        {
            if(!m_getClusterTested)
                Test(resp != null);

            if (resp != null)
            {
                Log.Debug("TestRetrieveAndRank", "OnGetCluster | name: {0}, size: {1}, ID: {2}, status: {3}.", resp.cluster_name, resp.cluster_size, resp.solr_cluster_id, resp.solr_cluster_status);

                if (resp.solr_cluster_status != "READY")
                    m_RetrieveAndRank.GetCluster(OnGetCluster, m_integrationTestClusterID);
                else
                    m_isClusterReady = true;
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "OnGetCluster | Get Cluster Response is null!");
            }

            m_getClusterTested = true;
        }

        private void OnGetClusterConfigs(SolrConfigList resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                if (resp.solr_configs.Length == 0)
                    Log.Debug("TestRetrieveAndRank", "OnGetClusterConfigs | no cluster configs!");

                foreach (string config in resp.solr_configs)
                    Log.Debug("TestRetrieveAndRank", "OnGetClusterConfigs | solr_config: " + config);
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "OnGetClustersConfigs | Get Cluster Configs Response is null!");
            }

            m_listClusterConfigsTested = true;
        }

        private void OnDeleteClusterConfig(bool success, string data)
        {
            Test(success);

            if (success)
            {
                Log.Debug("TestRetrieveAndRank", "OnDeleteClusterConfig | Success!");
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "OnDeleteClusterConfig | Failure!");
            }

            m_deleteClusterConfigTested = true;
        }


        private void OnGetClusterConfig(byte[] respData, string data)
        {
            Test(respData != null);

            if (respData != null)
            {
                Log.Debug("TestRetrieveAndRank", "OnGetClusterConfig | success!");
            }
            else
                Log.Debug("TestRetrieveAndRank", "OnGetClusterConfig | respData is null!");

            m_getClusterConfigTested = true;
        }

        private void OnUploadClusterConfig(UploadResponse resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                Log.Debug("TestRetrieveAndRank", "OnUploadClusterConfig | Success! {0}, {1}", resp.message, resp.statusCode);
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "OnUploadClusterConfig | Failure!");
            }

            m_uploadClusterConfigTested = true;
        }

        private void OnListCollections(CollectionsResponse resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                if (resp.responseHeader != null)
                    Log.Debug("TestRetrieveAndRank", "OnListCollections | status: {0}, QTime: {1}.", resp.responseHeader.status, resp.responseHeader.QTime);
                if (resp.collections != null)
                {
                    if (resp.collections.Length == 0)
                        Log.Debug("TestRetrieveAndRank", "OnListCollections | There are no collections!");
                    else
                        foreach (string collection in resp.collections)
                            Log.Debug("TestRetrieveAndRank", "\tOnListCollections | collection: {0}", collection);
                }
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "OnListCollections | GetCollections Response is null!");
            }
            
            m_listCollectionRequestTested = true;
        }

        private void OnCreateCollections(CollectionsResponse resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                if (resp.responseHeader != null)
                    Log.Debug("TestRetrieveAndRank", "OnCreateCollections | status: {0}, QTime: {1}.", resp.responseHeader.status, resp.responseHeader.QTime);
                if (resp.collections != null)
                {
                    if (resp.collections.Length == 0)
                        Log.Debug("TestRetrieveAndRank", "OnCreateCollections | There are no collections!");
                    else
                        foreach (string collection in resp.collections)
                            Log.Debug("TestRetrieveAndRank", "\tOnCreateCollections | collection: {0}", collection);
                }
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "OnCreateCollections | GetCollections Response is null!");
            }

            m_createCollectionRequestTested = true;
        }

        private void OnDeleteCollections(CollectionsResponse resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                if (resp.responseHeader != null)
                    Log.Debug("TestRetrieveAndRank", "OnDeleteCollections | status: {0}, QTime: {1}.", resp.responseHeader.status, resp.responseHeader.QTime);
                if (resp.collections != null)
                {
                    if (resp.collections.Length == 0)
                        Log.Debug("TestRetrieveAndRank", "OnDeleteCollections | There are no collections!");
                    else
                        foreach (string collection in resp.collections)
                            Log.Debug("TestRetrieveAndRank", "\tOnDeleteCollections | collection: {0}", collection);
                }
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "OnDeleteCollections | GetCollections Response is null!");
            }

            m_deleteCollectionRequestTested = true;
        }

        private void OnIndexDocuments(IndexResponse resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                if (resp.responseHeader != null)
                    Log.Debug("TestRetrieveAndRank", "OnIndexDocuments | status: {0}, QTime: {1}", resp.responseHeader.status, resp.responseHeader.QTime);
                else
                    Log.Debug("TestRetrieveAndRank", "OnIndexDocuments | Response header is null!");
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "OnIndexDocuments | response is null!");
            }

            m_indexDocumentsTested = true;
        }

        private void OnGetRankers(ListRankersPayload resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                if (resp.rankers.Length == 0)
                    Log.Debug("TestRetrieveAndRank", "OnGetRankers | no rankers!");
                foreach (RankerInfoPayload ranker in resp.rankers)
                    Log.Debug("TestRetrieveAndRank", "OnGetRankers | ranker name: {0}, ID: {1}, created: {2}, url: {3}.", ranker.name, ranker.ranker_id, ranker.created, ranker.url);
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "OnGetRankers | Get Ranker Response is null!");
            }

            m_getRankersTested = true;
        }

        private void OnCreateRanker(RankerStatusPayload resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                Log.Debug("TestRetrieveAndRank", "OnCreateRanker | ID: {0}, url: {1}, name: {2}, created: {3}, status: {4}, statusDescription: {5}.", resp.ranker_id, resp.url, resp.name, resp.created, resp.status, resp.status_description);
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "OnCreateRanker | Get Cluster Response is null!");
            }

            m_createRankerTested = true;
        }

        private void OnRank(RankerOutputPayload resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                Log.Debug("TestRetrieveAndRank", "OnRank | ID: {0}, url: {1}, name: {2}, top_answer: {3}.", resp.ranker_id, resp.url, resp.name, resp.top_answer);
                if (resp.answers != null)
                    if (resp.answers.Length == 0)
                    {
                        Log.Debug("TestRetrieveAndRank", "\tThere are no answers!");
                    }
                    else
                    {
                        foreach (RankedAnswer answer in resp.answers)
                            Log.Debug("TestRetrieveAndRank", "\tOnRank | answerID: {0}, score: {1}, confidence: {2}.", answer.answer_id, answer.score, answer.confidence);
                    }
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "OnRank | Rank response is null!");
            }

            m_rankTested = true;
        }

        private void OnGetRanker(RankerStatusPayload resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                Log.Debug("TestRetrieveAndRank", "GetRanker | ranker_id: {0}, url: {1}, name: {2}, created: {3}, status: {4}, status_description: {5}.", resp.ranker_id, resp.url, resp.name, resp.created, resp.status, resp.status_description);
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "GetRanker | GetRanker response is null!");
            }

            m_getRankersTested = true;
        }

        private void OnDeleteRanker(bool success, string data)
        {
            Test(success);

            if (success)
            {
                Log.Debug("TestRetrieveAndRank", "OnDeleteRanker | Success!");
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "OnDeleteRanker | Failure!");
            }

            m_deleteRankersTested = true;
        }

        private void OnStandardSearch(SearchResponse resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                if (resp.responseHeader != null)
                {
                    Log.Debug("TestRetrieveAndRank", "Search | status: {0}, QTime: {1}.", resp.responseHeader.status, resp.responseHeader.QTime);
                    if (resp.responseHeader._params != null)
                        Log.Debug("TestRetrieveAndRank", "\tSearch | params.q: {0}, params.fl: {1}, params.wt: {2}.", resp.responseHeader._params.q, resp.responseHeader._params.fl, resp.responseHeader._params.wt);
                    else
                        Log.Debug("TestRetrieveAndRank", "Search | responseHeader.params is null!");
                }
                else
                {
                    Log.Debug("TestRetrieveAndRank", "Search | response header is null!");
                }

                if (resp.response != null)
                {
                    Log.Debug("TestRetrieveAndRank", "Search | numFound: {0}, start: {1}.", resp.response.numFound, resp.response.start);
                    if (resp.response.docs != null)
                    {
                        if (resp.response.docs.Length == 0)
                            Log.Debug("TestRetrieveAndRank", "Search | There are no docs!");
                        else
                            foreach (Doc doc in resp.response.docs)
                            {
                                Log.Debug("TestRetrieveAndRank", "\tSearch | id: {0}.", doc.id);

                                if (doc.title != null)
                                {
                                    if (doc.title.Length == 0)
                                        Log.Debug("TestRetrieveAndRank", "Search | There are no title");
                                    else
                                        foreach (string s in doc.title)
                                            Log.Debug("TestRetrieveAndRank", "\tSearch | title: {0}.", s);
                                }
                                else
                                {
                                    Log.Debug("TestRetrieveAndRank", "Search | title is null");
                                }

                                if (doc.author != null)
                                {
                                    if (doc.author.Length == 0)
                                        Log.Debug("TestRetrieveAndRank", "Search | There are no authors");
                                    else
                                        foreach (string s in doc.author)
                                            Log.Debug("TestRetrieveAndRank", "\tSearch | Author: {0}.", s);
                                }
                                else
                                {
                                    Log.Debug("TestRetrieveAndRank", "Search | Authors is null");
                                }

                                if (doc.body != null)
                                {
                                    if (doc.body.Length == 0)
                                        Log.Debug("TestRetrieveAndRank", "Search | There are no body");
                                    else
                                        foreach (string s in doc.body)
                                            Log.Debug("TestRetrieveAndRank", "\tSearch | body: {0}.", s);
                                }
                                else
                                {
                                    Log.Debug("TestRetrieveAndRank", "Search | Body is null");
                                }

                                if (doc.bibliography != null)
                                {
                                    if (doc.bibliography.Length == 0)
                                        Log.Debug("TestRetrieveAndRank", "Search | There are no bibliographies");
                                    else
                                        foreach (string s in doc.bibliography)
                                            Log.Debug("TestRetrieveAndRank", "\tSearch | bibliography: {0}.", s);
                                }
                                else
                                {
                                    Log.Debug("TestRetrieveAndRank", "Search | Bibliography is null");
                                }
                            }
                    }
                    else
                    {
                        Log.Debug("TestRetrieveAndRank", "Search | docs are null!");
                    }
                }
                else
                {
                    Log.Debug("TestRetrieveAndRank", "Search | response is null!");
                }
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "Search response is null!");
            }

            m_standardSearchTested = true;
        }

        private void OnRankedSearch(SearchResponse resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                if (resp.responseHeader != null)
                {
                    Log.Debug("TestRetrieveAndRank", "Search | status: {0}, QTime: {1}.", resp.responseHeader.status, resp.responseHeader.QTime);
                    if (resp.responseHeader._params != null)
                        Log.Debug("TestRetrieveAndRank", "\tSearch | params.q: {0}, params.fl: {1}, params.wt: {2}.", resp.responseHeader._params.q, resp.responseHeader._params.fl, resp.responseHeader._params.wt);
                    else
                        Log.Debug("TestRetrieveAndRank", "Search | responseHeader.params is null!");
                }
                else
                {
                    Log.Debug("TestRetrieveAndRank", "Search | response header is null!");
                }

                if (resp.response != null)
                {
                    Log.Debug("TestRetrieveAndRank", "Search | numFound: {0}, start: {1}.", resp.response.numFound, resp.response.start);
                    if (resp.response.docs != null)
                    {
                        if (resp.response.docs.Length == 0)
                            Log.Debug("TestRetrieveAndRank", "Search | There are no docs!");
                        else
                            foreach (Doc doc in resp.response.docs)
                            {
                                Log.Debug("TestRetrieveAndRank", "\tSearch | id: {0}.", doc.id);

                                if (doc.title != null)
                                {
                                    if (doc.title.Length == 0)
                                        Log.Debug("TestRetrieveAndRank", "Search | There are no title");
                                    else
                                        foreach (string s in doc.title)
                                            Log.Debug("TestRetrieveAndRank", "\tSearch | title: {0}.", s);
                                }
                                else
                                {
                                    Log.Debug("TestRetrieveAndRank", "Search | title is null");
                                }

                                if (doc.author != null)
                                {
                                    if (doc.author.Length == 0)
                                        Log.Debug("TestRetrieveAndRank", "Search | There are no authors");
                                    else
                                        foreach (string s in doc.author)
                                            Log.Debug("TestRetrieveAndRank", "\tSearch | Author: {0}.", s);
                                }
                                else
                                {
                                    Log.Debug("TestRetrieveAndRank", "Search | Authors is null");
                                }

                                if (doc.body != null)
                                {
                                    if (doc.body.Length == 0)
                                        Log.Debug("TestRetrieveAndRank", "Search | There are no body");
                                    else
                                        foreach (string s in doc.body)
                                            Log.Debug("TestRetrieveAndRank", "\tSearch | body: {0}.", s);
                                }
                                else
                                {
                                    Log.Debug("TestRetrieveAndRank", "Search | Body is null");
                                }

                                if (doc.bibliography != null)
                                {
                                    if (doc.bibliography.Length == 0)
                                        Log.Debug("TestRetrieveAndRank", "Search | There are no bibliographies");
                                    else
                                        foreach (string s in doc.bibliography)
                                            Log.Debug("TestRetrieveAndRank", "\tSearch | bibliography: {0}.", s);
                                }
                                else
                                {
                                    Log.Debug("TestRetrieveAndRank", "Search | Bibliography is null");
                                }
                            }
                    }
                    else
                    {
                        Log.Debug("TestRetrieveAndRank", "Search | docs are null!");
                    }
                }
                else
                {
                    Log.Debug("TestRetrieveAndRank", "Search | response is null!");
                }
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "Search response is null!");
            }

            m_rankedSearchTested = true;
        }
    }
}
