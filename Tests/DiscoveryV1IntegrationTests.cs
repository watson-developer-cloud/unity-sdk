/**
* (C) Copyright IBM Corp. 2018, 2020.
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.Discovery.V1;
using IBM.Watson.Discovery.V1.Model;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace IBM.Watson.Tests
{
    public class DiscoveryV1IntegrationTests
    {
        private DiscoveryService service;
        private string versionDate = "2019-02-13";
        private string watsonBeatsJeopardyTxtFilePath;
        private string watsonBeatsJeopardyHtmlFilePath;
        private string stopwordsFilePath;
        private string environmentId;
        private string createdEnvironmentName = "unity-sdk-environment";
        private string createdEnvironmentDescription = "This is an environment created in Unity SDK integration tests. Please delete.";
        private string updatedEnvironmentName = "unity-sdk-environment-updated";
        private string updatedEnvironmentDescription = "This is an environment created in Unity SDK integration tests. Please delete. (updated)";
        private string createdEnvironmentId;
        private string createdConfigurationName;
        private string createdConfigurationDescription = "This is a configuration created in Unity SDK integration tests. Please delete.";
        private string updatedConfigurationName;
        private string updatedConfigurationDescription = "This is a configuration created in Unity SDK integration tests. Please delete. (updated)";
        private string createdConfigurationId;
        private string createdCollectionName;
        private string createdCollectionDescription = "This is a collection created in Unity SDK integration tests. Please delete.";
        private string updatedCollectionName;
        private string updatedCollectionDescription = "This is a collection created in Unity SDK integration tests. Please delete. (updated)";
        private string collectionId;
        private string createdJapaneseCollectionName;
        private string createdJapaneseCollectionDescription = "This is a japanese collection created in Unity SDK integration tests. Please delete.";
        private string createdJapaneseCollectionId;
        private string addedDocumentId;
        private string queryId;
        private string sessionToken;
        private string credentialId;
        private string gatewayId;

        private bool isTokenizationDictionaryReady = false;
        private bool isStopwordsListReady = false;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            LogSystem.InstallDefaultReactors();
            environmentId = Environment.GetEnvironmentVariable("DISCOVERY_ENVIRONMENT_ID");
            createdConfigurationName = Guid.NewGuid().ToString();
            updatedConfigurationName = createdConfigurationName + "-updated";
            createdCollectionName = Guid.NewGuid().ToString();
            updatedCollectionName = createdCollectionName + "-updated";
            createdJapaneseCollectionName = Guid.NewGuid().ToString();

            watsonBeatsJeopardyTxtFilePath = Application.dataPath + "/Watson/Tests/TestData/DiscoveryV1/watson_beats_jeopardy.txt";
            watsonBeatsJeopardyHtmlFilePath = Application.dataPath + "/Watson/Tests/TestData/DiscoveryV1/watson_beats_jeopardy.html";
            stopwordsFilePath = Application.dataPath + "/Watson/Tests/TestData/DiscoveryV1/stopwords.txt";
        }

        [UnitySetUp]
        public IEnumerator UnityTestSetup()
        {
            if (service == null)
            {
                service = new DiscoveryService(versionDate);
            }

            while (!service.Authenticator.CanAuthenticate())
                yield return null;
        }

        [SetUp]
        public void TestSetup()
        {
            service.WithHeader("X-Watson-Test", "1");
        }

        #region CreateEnvironment
        //  Disabled since we only have one environment in our instance
        //[UnityTest, Order(0)]
        public IEnumerator TestCreateEnvironment()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to CreateEnvironment...");
            ModelEnvironment createEnvironmentResponse = null;
            service.CreateEnvironment(
                callback: (DetailedResponse<ModelEnvironment> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "CreateEnvironment result: {0}", response.Response);
                    createEnvironmentResponse = response.Result;
                    createdEnvironmentId = createEnvironmentResponse.EnvironmentId;
                    Assert.IsNotNull(createEnvironmentResponse);
                    Assert.IsNotNull(createdEnvironmentId);
                    Assert.IsNull(error);
                },
                name: createdEnvironmentName,
                description: createdEnvironmentDescription,
                size: "S"
            );

            while (createEnvironmentResponse == null)
                yield return null;
        }
        #endregion

        #region GetEnvironment
        [UnityTest, Order(1)]
        public IEnumerator TestGetEnvironment()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to GetEnvironment...");
            ModelEnvironment getEnvironmentResponse = null;
            service.GetEnvironment(
                callback: (DetailedResponse<ModelEnvironment> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "GetEnvironment result: {0}", response.Response);
                    getEnvironmentResponse = response.Result;
                    Assert.IsNotNull(getEnvironmentResponse);
                    Assert.IsTrue(getEnvironmentResponse.EnvironmentId == environmentId);
                    Assert.IsNull(error);
                },
                environmentId: environmentId
            );

            while (getEnvironmentResponse == null)
                yield return null;
        }
        #endregion

        #region ListEnvironments
        [UnityTest, Order(2)]
        public IEnumerator TestListEnvironments()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to ListEnvironments...");
            ListEnvironmentsResponse listEnvironmentsResponse = null;
            service.ListEnvironments(
                callback: (DetailedResponse<ListEnvironmentsResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "ListEnvironments result: {0}", response.Response);
                    listEnvironmentsResponse = response.Result;
                    Assert.IsNotNull(listEnvironmentsResponse);
                    Assert.IsNotNull(listEnvironmentsResponse.Environments);
                    Assert.IsTrue(listEnvironmentsResponse.Environments.Count > 0);
                    Assert.IsNull(error);
                }
            );

            while (listEnvironmentsResponse == null)
                yield return null;
        }
        #endregion

        #region UpdateEnvironment
        //  Disabled since we only have one environment in our instance
        //[UnityTest, Order(3)]
        public IEnumerator TestUpdateEnvironment()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to UpdateEnvironment...");
            ModelEnvironment updateEnvironmentResponse = null;
            service.UpdateEnvironment(
                callback: (DetailedResponse<ModelEnvironment> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "UpdateEnvironment result: {0}", response.Response);
                    updateEnvironmentResponse = response.Result;
                    Assert.IsNotNull(updateEnvironmentResponse);
                    Assert.IsTrue(updateEnvironmentResponse.Name == updatedEnvironmentName);
                    Assert.IsTrue(updateEnvironmentResponse.Description == updatedEnvironmentDescription);
                    Assert.IsTrue(updateEnvironmentResponse.Size == "LT");
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                name: updatedEnvironmentName,
                description: updatedEnvironmentDescription,
                size: "LT"
            );

            while (updateEnvironmentResponse == null)
                yield return null;
        }
        #endregion

        #region CreateConfiguration
        [UnityTest, Order(4)]
        public IEnumerator TestCreateConfiguration()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to CreateConfiguration...");
            Source source = new Source()
            {
                Options = new SourceOptions()
                {
                    CrawlAllBuckets = true,
                    Buckets = new List<SourceOptionsBuckets>()
                    {
                        new SourceOptionsBuckets()
                        {
                            Limit = 10,
                            Name = "bucket"
                        }
                    }
                }
            };

            Configuration createConfigurationResponse = null;
            service.CreateConfiguration(
                callback: (DetailedResponse<Configuration> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "CreateConfiguration result: {0}", response.Response);
                    createConfigurationResponse = response.Result;
                    createdConfigurationId = createConfigurationResponse.ConfigurationId;
                    Assert.IsNotNull(createConfigurationResponse);
                    Assert.IsNotNull(createConfigurationResponse.ConfigurationId);
                    Assert.IsTrue(createConfigurationResponse.Name == createdConfigurationName);
                    Assert.IsTrue(createConfigurationResponse.Description == createdConfigurationDescription);
                    Assert.IsTrue(createConfigurationResponse.Source.Options.CrawlAllBuckets);
                    Assert.IsNotNull(createConfigurationResponse.Source.Options.Buckets);
                    Assert.IsTrue(createConfigurationResponse.Source.Options.Buckets.Count > 0);
                    Assert.IsTrue(createConfigurationResponse.Source.Options.Buckets[0].Name == "bucket");
                    Assert.IsTrue(createConfigurationResponse.Source.Options.Buckets[0].Limit == 10);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                name: createdConfigurationName,
                description: createdConfigurationDescription,
                source: source
            );

            while (createConfigurationResponse == null)
                yield return null;
        }
        #endregion

        #region GetConfiguration
        [UnityTest, Order(5)]
        public IEnumerator TestGetConfiguration()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to GetConfiguration...");
            Configuration getConfigurationResponse = null;
            service.GetConfiguration(
                callback: (DetailedResponse<Configuration> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "GetConfiguration result: {0}", response.Response);
                    getConfigurationResponse = response.Result;
                    Assert.IsNotNull(getConfigurationResponse);
                    Assert.IsTrue(getConfigurationResponse.ConfigurationId == createdConfigurationId);
                    Assert.IsTrue(getConfigurationResponse.Name == createdConfigurationName);
                    Assert.IsTrue(getConfigurationResponse.Description == createdConfigurationDescription);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                configurationId: createdConfigurationId
            );

            while (getConfigurationResponse == null)
                yield return null;
        }
        #endregion

        #region ListConfigurations
        [UnityTest, Order(6)]
        public IEnumerator TestListConfigurations()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to ListConfigurations...");
            ListConfigurationsResponse listConfigurationsResponse = null;
            service.ListConfigurations(
                callback: (DetailedResponse<ListConfigurationsResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "ListConfigurations result: {0}", response.Response);
                    listConfigurationsResponse = response.Result;
                    Assert.IsNotNull(listConfigurationsResponse);
                    Assert.IsNotNull(listConfigurationsResponse.Configurations);
                    Assert.IsTrue(listConfigurationsResponse.Configurations.Count > 0);
                    Assert.IsTrue(listConfigurationsResponse.Configurations[0].Name == createdConfigurationName);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                name: createdConfigurationName
            );

            while (listConfigurationsResponse == null)
                yield return null;
        }
        #endregion

        #region UpdateConfiguration
        [UnityTest, Order(7)]
        public IEnumerator TestUpdateConfiguration()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to UpdateConfiguration...");
            Configuration updateConfigurationResponse = null;
            service.UpdateConfiguration(
                callback: (DetailedResponse<Configuration> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "UpdateConfiguration result: {0}", response.Response);
                    updateConfigurationResponse = response.Result;
                    Assert.IsNotNull(updateConfigurationResponse);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                configurationId: createdConfigurationId,
                name: updatedConfigurationName,
                description: updatedConfigurationDescription
            );

            while (updateConfigurationResponse == null)
                yield return null;
        }
        #endregion

        #region CreateCollection
        [UnityTest, Order(9)]
        public IEnumerator TestCreateCollection()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to CreateCollection...");
            Collection createCollectionResponse = null;
            service.CreateCollection(
                callback: (DetailedResponse<Collection> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "CreateCollection result: {0}", response.Response);
                    createCollectionResponse = response.Result;
                    collectionId = createCollectionResponse.CollectionId;
                    Assert.IsNotNull(createCollectionResponse);
                    Assert.IsNotNull(collectionId);
                    Assert.IsTrue(createCollectionResponse.Name == createdCollectionName);
                    Assert.IsTrue(createCollectionResponse.Description == createdCollectionDescription);
                    Assert.IsTrue(createCollectionResponse.Language == "en");
                    Assert.IsTrue(createCollectionResponse.ConfigurationId == createdConfigurationId);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                name: createdCollectionName,
                description: createdCollectionDescription,
                configurationId: createdConfigurationId,
                language: "en"
            );

            while (createCollectionResponse == null)
                yield return null;
        }
        #endregion

        #region CreateJapaneseCollection
        [UnityTest, Order(10)]
        public IEnumerator TestCreateJapaneseCollection()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to CreateJapaneseCollection...");
            Collection createCollectionResponse = null;
            service.CreateCollection(
                callback: (DetailedResponse<Collection> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "CreateCollection result: {0}", response.Response);
                    createCollectionResponse = response.Result;
                    createdJapaneseCollectionId = createCollectionResponse.CollectionId;
                    Assert.IsNotNull(createCollectionResponse);
                    Assert.IsNotNull(createdJapaneseCollectionId);
                    Assert.IsTrue(createCollectionResponse.Name == createdJapaneseCollectionName);
                    Assert.IsTrue(createCollectionResponse.Description == createdJapaneseCollectionDescription);
                    Assert.IsTrue(createCollectionResponse.Language == "ja");
                    Assert.IsTrue(createCollectionResponse.ConfigurationId == createdConfigurationId);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                name: createdJapaneseCollectionName,
                description: createdJapaneseCollectionDescription,
                configurationId: createdConfigurationId,
                language: "ja"
            );

            while (createCollectionResponse == null)
                yield return null;
        }
        #endregion

        #region GetCollection
        [UnityTest, Order(11)]
        public IEnumerator TestGetCollection()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to GetCollection...");
            Collection getCollectionResponse = null;
            service.GetCollection(
                callback: (DetailedResponse<Collection> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "GetCollection result: {0}", response.Response);
                    getCollectionResponse = response.Result;
                    Assert.IsNotNull(getCollectionResponse);
                    Assert.IsTrue(getCollectionResponse.Name == createdCollectionName);
                    Assert.IsTrue(getCollectionResponse.Description == createdCollectionDescription);
                    Assert.IsTrue(getCollectionResponse.Language == "en");
                    Assert.IsTrue(getCollectionResponse.ConfigurationId == createdConfigurationId);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: collectionId
            );

            while (getCollectionResponse == null)
                yield return null;
        }
        #endregion

        #region ListCollections
        [UnityTest, Order(12)]
        public IEnumerator TestListCollections()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to ListCollections...");
            ListCollectionsResponse listCollectionsResponse = null;
            service.ListCollections(
                callback: (DetailedResponse<ListCollectionsResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "ListCollections result: {0}", response.Response);
                    listCollectionsResponse = response.Result;
                    Assert.IsNotNull(listCollectionsResponse);
                    Assert.IsNotNull(listCollectionsResponse.Collections);
                    Assert.IsTrue(listCollectionsResponse.Collections.Count > 0);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                name: createdCollectionName
            );

            while (listCollectionsResponse == null)
                yield return null;
        }
        #endregion

        #region UpdateCollection
        [UnityTest, Order(13)]
        public IEnumerator TestUpdateCollection()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to UpdateCollection...");
            Collection updateCollectionResponse = null;
            service.UpdateCollection(
                callback: (DetailedResponse<Collection> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "UpdateCollection result: {0}", response.Response);
                    updateCollectionResponse = response.Result;
                    Assert.IsNotNull(updateCollectionResponse);
                    Assert.IsTrue(updateCollectionResponse.Name == updatedCollectionName);
                    Assert.IsTrue(updateCollectionResponse.Description == updatedCollectionDescription);
                    Assert.IsTrue(updateCollectionResponse.Language == "en");
                    Assert.IsTrue(updateCollectionResponse.ConfigurationId == createdConfigurationId);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: collectionId,
                name: updatedCollectionName,
                description: updatedCollectionDescription,
                configurationId: createdConfigurationId
            );

            while (updateCollectionResponse == null)
                yield return null;
        }
        #endregion

        #region ListFields
        [UnityTest, Order(14)]
        public IEnumerator TestListFields()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to ListFields...");
            ListCollectionFieldsResponse listFieldsResponse = null;
            service.ListFields(
                callback: (DetailedResponse<ListCollectionFieldsResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "ListFields result: {0}", response.Response);
                    listFieldsResponse = response.Result;
                    Assert.IsNotNull(listFieldsResponse);
                    Assert.IsNotNull(listFieldsResponse.Fields);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionIds: new List<string>() { collectionId }
            );

            while (listFieldsResponse == null)
                yield return null;
        }
        #endregion

        #region ListCollectionFields
        [UnityTest, Order(15)]
        public IEnumerator TestListCollectionFields()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to ListCollectionFields...");
            ListCollectionFieldsResponse listCollectionFieldsResponse = null;
            service.ListCollectionFields(
                callback: (DetailedResponse<ListCollectionFieldsResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "ListCollectionFields result: {0}", response.Response);
                    listCollectionFieldsResponse = response.Result;
                    Assert.IsNotNull(listCollectionFieldsResponse);
                    Assert.IsNotNull(listCollectionFieldsResponse.Fields);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: collectionId
            );

            while (listCollectionFieldsResponse == null)
                yield return null;
        }
        #endregion

        #region CreateExpansions
        [UnityTest, Order(16)]
        public IEnumerator TestCreateExpansions()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to CreateExpansions...");
            Expansions createExpansionsResponse = null;
            List<Expansion> expansions = new List<Expansion>()
            {
                new Expansion()
                {
                    InputTerms = new List<string>()
                    {
                        "input-term"
                    },
                    ExpandedTerms = new List<string>()
                    {
                        "expanded-term"
                    }
                }
            };
            service.CreateExpansions(
                callback: (DetailedResponse<Expansions> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "CreateExpansions result: {0}", response.Response);
                    createExpansionsResponse = response.Result;
                    Assert.IsNotNull(createExpansionsResponse);
                    Assert.IsNotNull(createExpansionsResponse._Expansions);
                    Assert.IsTrue(createExpansionsResponse._Expansions.Count > 0);
                    Assert.IsTrue(createExpansionsResponse._Expansions[0].InputTerms[0] == "input-term");
                    Assert.IsTrue(createExpansionsResponse._Expansions[0].ExpandedTerms[0] == "expanded-term");
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: collectionId,
                expansions: expansions
            );

            while (createExpansionsResponse == null)
                yield return null;
        }
        #endregion

        #region CreateStopwordList
        //  Skipping because of length
        //[UnityTest, Order(17)]
        [Timeout(int.MaxValue)]
        public IEnumerator TestCreateStopwordList()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to CreateStopwordList...");
            TokenDictStatusResponse createStopwordListResponse = null;
            using (FileStream fs = File.OpenRead(stopwordsFilePath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    service.CreateStopwordList(
                        callback: (DetailedResponse<TokenDictStatusResponse> response, IBMError error) =>
                        {
                            Log.Debug("DiscoveryServiceV1IntegrationTests", "CreateStopwordList result: {0}", response.Response);
                            createStopwordListResponse = response.Result;
                            Assert.IsNotNull(createStopwordListResponse);
                            Assert.IsNotNull(createStopwordListResponse.Status);
                            Assert.IsTrue(createStopwordListResponse.Type == "stopwords");
                            Assert.IsNull(error);
                        },
                        environmentId: environmentId,
                        collectionId: collectionId,
                        stopwordFile: ms,
                        stopwordFilename: Path.GetFileName(stopwordsFilePath)
                    );

                    while (createStopwordListResponse == null)
                        yield return null;
                }
            }

            isStopwordsListReady = false;

            Runnable.Run(CheckStopwordsListStatus());
            while (!isStopwordsListReady)
                yield return null;
        }
        #endregion

        #region CreateTokenizationDictionary
        //  Skipping because of length
        //[UnityTest, Order(18)]
        [Timeout(int.MaxValue)]
        public IEnumerator TestCreateTokenizationDictionary()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to CreateTokenizationDictionary...");
            TokenDictStatusResponse createTokenizationDictionaryResponse = null;

            #region TokenizationRules
            var tokenizationRules = new List<TokenDictRule>()
            {
                new TokenDictRule()
                {
                    Text = "すしネコ",
                    Tokens = new List<string>()
                    {
                        "すし", "ネコ"
                    },
                    Readings = new List<string>()
                    {
                        "寿司", "ネコ"
                    },
                    PartOfSpeech = "カスタム名詞"
                }
            };
            #endregion

            service.CreateTokenizationDictionary(
                callback: (DetailedResponse<TokenDictStatusResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "CreateTokenizationDictionary result: {0}", response.Response);
                    createTokenizationDictionaryResponse = response.Result;
                    Assert.IsNotNull(createTokenizationDictionaryResponse);
                    Assert.IsNotNull(createTokenizationDictionaryResponse.Status);
                    Assert.IsTrue(createTokenizationDictionaryResponse.Type == "tokenization_dictionary");
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: createdJapaneseCollectionId,
                tokenizationRules: tokenizationRules
            );

            while (createTokenizationDictionaryResponse == null)
                yield return null;

            isTokenizationDictionaryReady = false;

            Runnable.Run(CheckTokenizationDictionaryStatus(environmentId, createdJapaneseCollectionId));
            while (!isTokenizationDictionaryReady)
                yield return null;
        }
        #endregion

        #region GetStopwordListStatus
        //  Skipping because of length
        //[UnityTest, Order(19)]
        public IEnumerator TestGetStopwordListStatus()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to GetStopwordListStatus...");
            TokenDictStatusResponse getStopwordListStatusResponse = null;
            service.GetStopwordListStatus(
                callback: (DetailedResponse<TokenDictStatusResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "GetStopwordListStatus result: {0}", response.Response);
                    getStopwordListStatusResponse = response.Result;
                    Assert.IsNotNull(getStopwordListStatusResponse);
                    Assert.IsNotNull(getStopwordListStatusResponse.Status);
                    Assert.IsTrue(getStopwordListStatusResponse.Type == "stopwords");
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: collectionId
            );

            while (getStopwordListStatusResponse == null)
                yield return null;
        }
        #endregion

        #region GetTokenizationDictionaryStatus
        //  Skipping because of length
        //[UnityTest, Order(20)]
        public IEnumerator TestGetTokenizationDictionaryStatus()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to GetTokenizationDictionaryStatus...");
            TokenDictStatusResponse getTokenizationDictionaryStatusResponse = null;
            service.GetTokenizationDictionaryStatus(
                callback: (DetailedResponse<TokenDictStatusResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "GetTokenizationDictionaryStatus result: {0}", response.Response);
                    getTokenizationDictionaryStatusResponse = response.Result;
                    Assert.IsNotNull(getTokenizationDictionaryStatusResponse);
                    Assert.IsNotNull(getTokenizationDictionaryStatusResponse.Status);
                    Assert.IsTrue(getTokenizationDictionaryStatusResponse.Type == "tokenization_dictionary");
                    Assert.IsNull(error);

                },
                environmentId: environmentId,
                collectionId: createdJapaneseCollectionId
            );

            while (getTokenizationDictionaryStatusResponse == null)
                yield return null;
        }
        #endregion

        #region ListExpansions
        [UnityTest, Order(21)]
        public IEnumerator TestListExpansions()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to ListExpansions...");
            Expansions listExpansionsResponse = null;
            service.ListExpansions(
                callback: (DetailedResponse<Expansions> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "ListExpansions result: {0}", response.Response);
                    listExpansionsResponse = response.Result;
                    Assert.IsNotNull(listExpansionsResponse);
                    Assert.IsNotNull(listExpansionsResponse._Expansions);
                    Assert.IsTrue(listExpansionsResponse._Expansions.Count > 0);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: collectionId
            );

            while (listExpansionsResponse == null)
                yield return null;
        }
        #endregion

        #region AddDocument
        [UnityTest, Order(22)]
        public IEnumerator TestAddDocument()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to AddDocument...");
            DocumentAccepted addDocumentResponse = null;
            using (FileStream fs = File.OpenRead(watsonBeatsJeopardyHtmlFilePath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    service.AddDocument(
                        callback: (DetailedResponse<DocumentAccepted> response, IBMError error) =>
                        {
                            Log.Debug("DiscoveryServiceV1IntegrationTests", "AddDocument result: {0}", response.Response);
                            addDocumentResponse = response.Result;
                            addedDocumentId = addDocumentResponse.DocumentId;
                            Assert.IsNotNull(addDocumentResponse);
                            Assert.IsNotNull(addedDocumentId);
                            Assert.IsNull(error);
                        },
                        environmentId: environmentId,
                        collectionId: collectionId,
                        file: ms,
                        fileContentType: Utility.GetMimeType(Path.GetExtension(watsonBeatsJeopardyHtmlFilePath)),
                        filename: Path.GetFileName(watsonBeatsJeopardyHtmlFilePath)
                    );

                    while (addDocumentResponse == null)
                        yield return null;
                }
            }
        }
        #endregion

        #region GetDocumentStatus
        [UnityTest, Order(23)]
        public IEnumerator TestGetDocumentStatus()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to GetDocumentStatus...");
            DocumentStatus getDocumentStatusResponse = null;
            service.GetDocumentStatus(
                callback: (DetailedResponse<DocumentStatus> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "GetDocumentStatus result: {0}", response.Response);
                    getDocumentStatusResponse = response.Result;
                    Assert.IsNotNull(getDocumentStatusResponse);
                    Assert.IsTrue(getDocumentStatusResponse.DocumentId == addedDocumentId);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: collectionId,
                documentId: addedDocumentId
            );

            while (getDocumentStatusResponse == null)
                yield return null;
        }
        #endregion

        #region UpdateDocument
        [UnityTest, Order(24)]
        public IEnumerator TestUpdateDocument()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to UpdateDocument...");
            DocumentAccepted updateDocumentResponse = null;
            using (FileStream fs = File.OpenRead(watsonBeatsJeopardyHtmlFilePath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    service.UpdateDocument(
                        callback: (DetailedResponse<DocumentAccepted> response, IBMError error) =>
                        {
                            Log.Debug("DiscoveryServiceV1IntegrationTests", "UpdateDocument result: {0}", response.Response);
                            updateDocumentResponse = response.Result;
                            Assert.IsNotNull(updateDocumentResponse);
                            Assert.IsTrue(updateDocumentResponse.DocumentId == addedDocumentId);
                            Assert.IsNull(error);
                        },
                        environmentId: environmentId,
                        collectionId: collectionId,
                        documentId: addedDocumentId,
                        file: ms,
                        fileContentType: Utility.GetMimeType(Path.GetExtension(watsonBeatsJeopardyHtmlFilePath)),
                        filename: Path.GetFileName(watsonBeatsJeopardyHtmlFilePath)
                    );

                    while (updateDocumentResponse == null)
                        yield return null;
                }
            }
        }
        #endregion

        #region FederatedQuery
        [UnityTest, Order(25)]
        public IEnumerator TestFederatedQuery()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to FederatedQuery...");
            QueryResponse federatedQueryResponse = null;
            service.FederatedQuery(
                callback: (DetailedResponse<QueryResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "FederatedQuery result: {0}", response.Response);
                    federatedQueryResponse = response.Result;
                    Assert.IsNotNull(federatedQueryResponse);
                    Assert.IsNotNull(federatedQueryResponse.MatchingResults);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionIds: collectionId,
                naturalLanguageQuery: "When did Watson win Jeopardy",
                passages: true,
                count: 10,
                highlight: true
            );

            while (federatedQueryResponse == null)
                yield return null;
        }
        #endregion

        #region FederatedQueryNotices
        [UnityTest, Order(26)]
        public IEnumerator TestFederatedQueryNotices()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to FederatedQueryNotices...");
            QueryNoticesResponse federatedQueryNoticesResponse = null;
            service.FederatedQueryNotices(
                callback: (DetailedResponse<QueryNoticesResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "FederatedQueryNotices result: {0}", response.Response);
                    federatedQueryNoticesResponse = response.Result;
                    Assert.IsNotNull(federatedQueryNoticesResponse);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionIds: new List<string>() { collectionId },
                naturalLanguageQuery: "When did Watson win Jeopardy",
                count: 10,
                highlight: true
            );

            while (federatedQueryNoticesResponse == null)
                yield return null;
        }
        #endregion

        #region Query
        [UnityTest, Order(27)]
        public IEnumerator TestQuery()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to Query...");
            QueryResponse queryResponse = null;
            service.Query(
                callback: (DetailedResponse<QueryResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "Query result: {0}", response.Response);
                    queryResponse = response.Result;
                    sessionToken = queryResponse.SessionToken;
                    Assert.IsNotNull(queryResponse);
                    Assert.IsNotNull(sessionToken);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: collectionId,
                naturalLanguageQuery: "When did Watson win Jeopardy",
                passages: true,
                count: 10,
                highlight: true
            );

            while (queryResponse == null)
                yield return null;
        }
        #endregion

        #region Query Aggregation
        [UnityTest, Order(28)]
        public IEnumerator TestQueryAggregation()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to Query...");
            string naturalLanguageQuery = "Who beat Ken Jennings in Jeopardy!";
            QueryResponse queryResultTimeslice = null;
            service.Query(
                callback: (DetailedResponse<QueryResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "Query result: {0}", response.Response);
                    queryResultTimeslice = response.Result;
                    Assert.IsNotNull(queryResultTimeslice);
                    Assert.IsNull(error);
                    Assert.IsNotNull(queryResultTimeslice.Aggregations);
                    Assert.IsTrue((queryResultTimeslice.Aggregations[0] as Timeslice).Field == "product.sales");
                    Assert.IsTrue((queryResultTimeslice.Aggregations[0] as Timeslice).Interval == "2d");
                },
                environmentId: "system",
                collectionId: "news-en",
                naturalLanguageQuery: naturalLanguageQuery,
                aggregation: "timeslice(product.sales,2day)"
            );

            QueryResponse queryResultTerm = null;
            service.Query(
                callback: (DetailedResponse<QueryResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "Query result: {0}", response.Response);
                    queryResultTerm = response.Result;
                    Assert.IsNotNull(queryResultTerm);
                    Assert.IsNull(error);
                    Assert.IsNotNull(queryResultTerm.Aggregations);
                    Assert.IsTrue((queryResultTerm.Aggregations[0] as Term).Field == "enriched_text.concepts.text");
                    Assert.IsTrue((queryResultTerm.Aggregations[0] as Term).Count == 10);
                },
                environmentId: "system",
                collectionId: "news-en",
                naturalLanguageQuery: naturalLanguageQuery,
                aggregation: "term(enriched_text.concepts.text,count:10)"
            );

            QueryResponse queryResultFilter = null;
            service.Query(
                callback: (DetailedResponse<QueryResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "Query result: {0}", response.Response);
                    queryResultFilter = response.Result;
                    Assert.IsNotNull(queryResultFilter);
                    Assert.IsNull(error);
                    Assert.IsNotNull(queryResultFilter.Aggregations);
                    Assert.IsTrue((queryResultFilter.Aggregations[0] as Filter).Match == "enriched_text.concepts.text:\"cloud computing\"");
                },
                environmentId: "system",
                collectionId: "news-en",
                naturalLanguageQuery: naturalLanguageQuery,
                aggregation: "filter(enriched_text.concepts.text:\"cloud computing\")"
            );

            while (queryResultTimeslice == null || queryResultFilter == null || queryResultTerm == null)
                yield return null;

        }
        #endregion

        #region QueryNotices
        [UnityTest, Order(29)]
        public IEnumerator TestQueryNotices()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to QueryNotices...");
            QueryNoticesResponse queryNoticesResponse = null;
            service.QueryNotices(
                callback: (DetailedResponse<QueryNoticesResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "QueryNotices result: {0}", response.Response);
                    queryNoticesResponse = response.Result;
                    Assert.IsNotNull(queryNoticesResponse);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: collectionId,
                naturalLanguageQuery: "When did Watson win Jeopardy",
                passages: true,
                count: 10,
                highlight: true
            );

            while (queryNoticesResponse == null)
                yield return null;
        }
        #endregion

        #region AddTrainingData
        [UnityTest, Order(31)]
        public IEnumerator TestAddTrainingData()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to AddTrainingData...");
            TrainingQuery addTrainingDataResponse = null;
            List<TrainingExample> examples = new List<TrainingExample>()
            {
                new TrainingExample()
                {
                    DocumentId = addedDocumentId,
                    Relevance = 2
                }
            };
            service.AddTrainingData(
                callback: (DetailedResponse<TrainingQuery> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "AddTrainingData result: {0}", response.Response);
                    addTrainingDataResponse = response.Result;
                    queryId = addTrainingDataResponse.QueryId;
                    Assert.IsNotNull(addTrainingDataResponse);
                    Assert.IsNotNull(queryId);
                    Assert.IsTrue(addTrainingDataResponse.NaturalLanguageQuery == "When did Watson win Jeopardy");
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: collectionId,
                naturalLanguageQuery: "When did Watson win Jeopardy",
                examples: examples
            );

            while (addTrainingDataResponse == null)
                yield return null;
        }
        #endregion

        #region CreateTrainingExample
        //  Skipping because we only have one document
        //[UnityTest, Order(32)]
        public IEnumerator TestCreateTrainingExample()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to CreateTrainingExample...");
            TrainingExample createTrainingExampleResponse = null;
            service.CreateTrainingExample(
                callback: (DetailedResponse<TrainingExample> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "CreateTrainingExample result: {0}", response.Response);
                    createTrainingExampleResponse = response.Result;
                    Assert.IsNotNull(createTrainingExampleResponse);
                    Assert.IsTrue(createTrainingExampleResponse.DocumentId == addedDocumentId);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: collectionId,
                queryId: queryId,
                documentId: addedDocumentId,
                relevance: 2
            );

            while (createTrainingExampleResponse == null)
                yield return null;
        }
        #endregion

        #region GetTrainingData
        [UnityTest, Order(33)]
        public IEnumerator TestGetTrainingData()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to GetTrainingData...");
            TrainingQuery getTrainingDataResponse = null;
            service.GetTrainingData(
                callback: (DetailedResponse<TrainingQuery> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "GetTrainingData result: {0}", response.Response);
                    getTrainingDataResponse = response.Result;
                    Assert.IsNotNull(getTrainingDataResponse);
                    Assert.IsTrue(getTrainingDataResponse.QueryId == queryId);
                    Assert.IsTrue(getTrainingDataResponse.NaturalLanguageQuery == "When did Watson win Jeopardy");
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: collectionId,
                queryId: queryId
            );

            while (getTrainingDataResponse == null)
                yield return null;
        }
        #endregion

        #region GetTrainingExample
        [UnityTest, Order(34)]
        public IEnumerator TestGetTrainingExample()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to GetTrainingExample...");
            TrainingExample getTrainingExampleResponse = null;
            service.GetTrainingExample(
                callback: (DetailedResponse<TrainingExample> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "GetTrainingExample result: {0}", response.Response);
                    getTrainingExampleResponse = response.Result;
                    Assert.IsNotNull(getTrainingExampleResponse);
                    Assert.IsTrue(getTrainingExampleResponse.DocumentId == addedDocumentId);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: collectionId,
                queryId: queryId,
                exampleId: addedDocumentId
            );

            while (getTrainingExampleResponse == null)
                yield return null;
        }
        #endregion

        #region ListTrainingData
        [UnityTest, Order(35)]
        public IEnumerator TestListTrainingData()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to ListTrainingData...");
            TrainingDataSet listTrainingDataResponse = null;
            service.ListTrainingData(
                callback: (DetailedResponse<TrainingDataSet> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "ListTrainingData result: {0}", response.Response);
                    listTrainingDataResponse = response.Result;
                    Assert.IsNotNull(listTrainingDataResponse);
                    Assert.IsNotNull(listTrainingDataResponse.Queries);
                    Assert.IsTrue(listTrainingDataResponse.Queries.Count > 0);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: collectionId
            );

            while (listTrainingDataResponse == null)
                yield return null;
        }
        #endregion

        #region ListTrainingExamples
        [UnityTest, Order(36)]
        public IEnumerator TestListTrainingExamples()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to ListTrainingExamples...");
            TrainingExampleList listTrainingExamplesResponse = null;
            service.ListTrainingExamples(
                callback: (DetailedResponse<TrainingExampleList> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "ListTrainingExamples result: {0}", response.Response);
                    listTrainingExamplesResponse = response.Result;
                    Assert.IsNotNull(listTrainingExamplesResponse);
                    Assert.IsNotNull(listTrainingExamplesResponse.Examples);
                    Assert.IsTrue(listTrainingExamplesResponse.Examples.Count > 0);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: collectionId,
                queryId: queryId
            );

            while (listTrainingExamplesResponse == null)
                yield return null;
        }
        #endregion

        #region UpdateTrainingExample
        [UnityTest, Order(37)]
        public IEnumerator TestUpdateTrainingExample()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to UpdateTrainingExample...");
            TrainingExample updateTrainingExampleResponse = null;
            service.UpdateTrainingExample(
                callback: (DetailedResponse<TrainingExample> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "UpdateTrainingExample result: {0}", response.Response);
                    updateTrainingExampleResponse = response.Result;
                    Assert.IsNotNull(updateTrainingExampleResponse);
                    Assert.IsTrue(updateTrainingExampleResponse.DocumentId == addedDocumentId);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: collectionId,
                queryId: queryId,
                exampleId: addedDocumentId,
                relevance: 3
            );

            while (updateTrainingExampleResponse == null)
                yield return null;
        }
        #endregion

        #region CreateEvent
        [UnityTest, Order(38)]
        public IEnumerator TestCreateEvent()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to CreateEvent...");
            CreateEventResponse createEventResponse = null;
            EventData data = new EventData()
            {
                EnvironmentId = environmentId,
                SessionToken = sessionToken,
                CollectionId = collectionId,
                DocumentId = addedDocumentId
            };
            service.CreateEvent(
                callback: (DetailedResponse<CreateEventResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "CreateEvent result: {0}", response.Response);
                    createEventResponse = response.Result;
                    Assert.IsNotNull(createEventResponse);
                    Assert.IsTrue(createEventResponse.Type == "click");
                    Assert.IsNull(error);
                },
                type: "click",
                data: data
            );

            while (createEventResponse == null)
                yield return null;
        }
        #endregion

        #region GetMetricsEventRate
        [UnityTest, Order(39)]
        public IEnumerator TestGetMetricsEventRate()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to GetMetricsEventRate...");
            MetricResponse getMetricsEventRateResponse = null;
            service.GetMetricsEventRate(
                callback: (DetailedResponse<MetricResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "GetMetricsEventRate result: {0}", response.Response);
                    getMetricsEventRateResponse = response.Result;
                    Assert.IsNotNull(getMetricsEventRateResponse);
                    Assert.IsNotNull(getMetricsEventRateResponse.Aggregations);
                    Assert.IsNull(error);
                },
                resultType: "document",
                startTime: new DateTime(2019, 1, 1),
                endTime: DateTime.Today
            );

            while (getMetricsEventRateResponse == null)
                yield return null;
        }
        #endregion

        #region GetMetricsQuery
        [UnityTest, Order(40)]
        public IEnumerator TestGetMetricsQuery()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to GetMetricsQuery...");
            MetricResponse getMetricsQueryResponse = null;
            service.GetMetricsQuery(
                callback: (DetailedResponse<MetricResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "GetMetricsQuery result: {0}", response.Response);
                    getMetricsQueryResponse = response.Result;
                    Assert.IsNotNull(getMetricsQueryResponse);
                    Assert.IsNotNull(getMetricsQueryResponse.Aggregations);
                    Assert.IsNull(error);
                },
                resultType: "document"
            );

            while (getMetricsQueryResponse == null)
                yield return null;
        }
        #endregion

        #region GetMetricsQueryEvent
        [UnityTest, Order(41)]
        public IEnumerator TestGetMetricsQueryEvent()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to GetMetricsQueryEvent...");
            MetricResponse getMetricsQueryEventResponse = null;
            service.GetMetricsQueryEvent(
                callback: (DetailedResponse<MetricResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "GetMetricsQueryEvent result: {0}", response.Response);
                    getMetricsQueryEventResponse = response.Result;
                    Assert.IsNotNull(getMetricsQueryEventResponse);
                    Assert.IsNotNull(getMetricsQueryEventResponse.Aggregations);
                    Assert.IsNull(error);
                },
                resultType: "document"
            );

            while (getMetricsQueryEventResponse == null)
                yield return null;
        }
        #endregion

        #region GetMetricsQueryNoResults
        [UnityTest, Order(42)]
        public IEnumerator TestGetMetricsQueryNoResults()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to GetMetricsQueryNoResults...");
            MetricResponse getMetricsQueryNoResultsResponse = null;
            service.GetMetricsQueryNoResults(
                callback: (DetailedResponse<MetricResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "GetMetricsQueryNoResults result: {0}", response.Response);
                    getMetricsQueryNoResultsResponse = response.Result;
                    Assert.IsNotNull(getMetricsQueryNoResultsResponse);
                    Assert.IsNotNull(getMetricsQueryNoResultsResponse.Aggregations);
                    Assert.IsNull(error);
                },
                resultType: "document"
            );

            while (getMetricsQueryNoResultsResponse == null)
                yield return null;
        }
        #endregion

        #region GetMetricsQueryTokenEvent
        [UnityTest, Order(43)]
        public IEnumerator TestGetMetricsQueryTokenEvent()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to GetMetricsQueryTokenEvent...");
            MetricTokenResponse getMetricsQueryTokenEventResponse = null;
            service.GetMetricsQueryTokenEvent(
                callback: (DetailedResponse<MetricTokenResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "GetMetricsQueryTokenEvent result: {0}", response.Response);
                    getMetricsQueryTokenEventResponse = response.Result;
                    Assert.IsNotNull(getMetricsQueryTokenEventResponse);
                    Assert.IsNotNull(getMetricsQueryTokenEventResponse.Aggregations);
                    Assert.IsNull(error);
                },
                count: 10
            );

            while (getMetricsQueryTokenEventResponse == null)
                yield return null;
        }
        #endregion

        #region QueryLog
        [UnityTest, Order(44)]
        public IEnumerator TestQueryLog()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to QueryLog...");
            LogQueryResponse queryLogResponse = null;
            service.QueryLog(
                callback: (DetailedResponse<LogQueryResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "QueryLog result: {0}", response.Response);
                    queryLogResponse = response.Result;
                    Assert.IsNotNull(queryLogResponse);
                    Assert.IsNotNull(queryLogResponse.Results);
                    Assert.IsNull(error);
                },
                query: "When did Watson beat Jeopardy",
                count: 10
            );

            while (queryLogResponse == null)
                yield return null;
        }
        #endregion

        #region CreateAuthenticator
        [UnityTest, Order(45)]
        public IEnumerator TestCreateAuthenticator()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to CreateCredentials...");
            ModelCredentials createCredentialsResponse = null;
            CredentialDetails credentialDetails = new CredentialDetails()
            {
                CredentialType = CredentialDetails.CredentialTypeValue.OAUTH2,
                EnterpriseId = "myEnterpriseId",
                ClientId = "myClientId",
                ClientSecret = "myClentSecret",
                PublicKeyId = "myPublicIdKey",
                Passphrase = "myPassphrase",
                PrivateKey = "myPrivateKey"
            };
            service.CreateCredentials(
                callback: (DetailedResponse<ModelCredentials> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "CreateCredentials result: {0}", response.Response);
                    createCredentialsResponse = response.Result;
                    credentialId = createCredentialsResponse.CredentialId;
                    Assert.IsNotNull(createCredentialsResponse);
                    Assert.IsNotNull(credentialId);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                sourceType: "box",
                credentialDetails: credentialDetails
            );

            while (createCredentialsResponse == null)
                yield return null;
        }
        #endregion

        #region GetCredentials
        [UnityTest, Order(46)]
        public IEnumerator TestGetCredentials()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to GetCredentials...");
            ModelCredentials getCredentialsResponse = null;
            service.GetCredentials(
                callback: (DetailedResponse<ModelCredentials> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "GetCredentials result: {0}", response.Response);
                    getCredentialsResponse = response.Result;
                    Assert.IsNotNull(getCredentialsResponse);
                    Assert.IsTrue(getCredentialsResponse.CredentialId == credentialId);
                    Assert.IsNotNull(getCredentialsResponse.CredentialDetails);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                credentialId: credentialId
            );

            while (getCredentialsResponse == null)
                yield return null;
        }
        #endregion

        #region ListCredentials
        [UnityTest, Order(47)]
        public IEnumerator TestListCredentials()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to ListCredentials...");
            CredentialsList listCredentialsResponse = null;
            service.ListCredentials(
                callback: (DetailedResponse<CredentialsList> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "ListCredentials result: {0}", response.Response);
                    listCredentialsResponse = response.Result;
                    Assert.IsNotNull(listCredentialsResponse);
                    Assert.IsNotNull(listCredentialsResponse._Credentials);
                    Assert.IsTrue(listCredentialsResponse._Credentials.Count > 0);
                    Assert.IsNull(error);
                },
                environmentId: environmentId
            );

            while (listCredentialsResponse == null)
                yield return null;
        }
        #endregion

        #region UpdateCredentials
        [UnityTest, Order(48)]
        public IEnumerator TestUpdateCredentials()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to UpdateCredentials...");
            ModelCredentials updateCredentialsResponse = null;
            CredentialDetails credentialDetails = new CredentialDetails()
            {
                CredentialType = CredentialDetails.CredentialTypeValue.OAUTH2,
                EnterpriseId = "myEnterpriseIdUpdated",
                ClientId = "myClientIdUpdated",
                ClientSecret = "myClentSecretUpdated",
                PublicKeyId = "myPublicIdKeyUpdated",
                Passphrase = "myPassphraseUpdated",
                PrivateKey = Convert.ToBase64String(Encoding.ASCII.GetBytes("myPrivateKeyUpdated"))
            };
            service.UpdateCredentials(
                callback: (DetailedResponse<ModelCredentials> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "UpdateCredentials result: {0}", response.Response);
                    updateCredentialsResponse = response.Result;
                    Assert.IsNotNull(updateCredentialsResponse);
                    Assert.IsNotNull(updateCredentialsResponse.CredentialDetails);
                    Assert.IsTrue(updateCredentialsResponse.CredentialDetails.EnterpriseId == "myEnterpriseIdUpdated");
                    Assert.IsTrue(updateCredentialsResponse.CredentialDetails.ClientId == "myClientIdUpdated");
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                credentialId: credentialId,
                sourceType: "box",
                credentialDetails: credentialDetails
            );

            while (updateCredentialsResponse == null)
                yield return null;
        }
        #endregion

        #region CreateGateway
        [UnityTest, Order(49)]
        public IEnumerator TestCreateGateway()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to CreateGateway...");
            Gateway createGatewayResponse = null;
            service.CreateGateway(
                callback: (DetailedResponse<Gateway> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "CreateGateway result: {0}", response.Response);
                    createGatewayResponse = response.Result;
                    gatewayId = createGatewayResponse.GatewayId;
                    Assert.IsNotNull(createGatewayResponse);
                    Assert.IsNotNull(gatewayId);
                    Assert.IsNull(error);
                },
                environmentId: environmentId
            );

            while (createGatewayResponse == null)
                yield return null;
        }
        #endregion

        #region GetGateway
        [UnityTest, Order(50)]
        public IEnumerator TestGetGateway()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to GetGateway...");
            Gateway getGatewayResponse = null;
            service.GetGateway(
                callback: (DetailedResponse<Gateway> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "GetGateway result: {0}", response.Response);
                    getGatewayResponse = response.Result;
                    Assert.IsNotNull(getGatewayResponse);
                    Assert.IsTrue(getGatewayResponse.GatewayId == gatewayId);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                gatewayId: gatewayId
            );

            while (getGatewayResponse == null)
                yield return null;
        }
        #endregion

        #region ListGateways
        [UnityTest, Order(51)]
        public IEnumerator TestListGateways()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to ListGateways...");
            GatewayList listGatewaysResponse = null;
            service.ListGateways(
                callback: (DetailedResponse<GatewayList> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "ListGateways result: {0}", response.Response);
                    listGatewaysResponse = response.Result;
                    Assert.IsNotNull(listGatewaysResponse);
                    Assert.IsNotNull(listGatewaysResponse.Gateways);
                    Assert.IsTrue(listGatewaysResponse.Gateways.Count > 0);
                    Assert.IsNull(error);
                },
                environmentId: environmentId
            );

            while (listGatewaysResponse == null)
                yield return null;
        }
        #endregion

        #region DeleteGateway
        [UnityTest, Order(87)]
        public IEnumerator TestDeleteGateway()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to DeleteGateway...");
            GatewayDelete deleteGatewayResponse = null;
            service.DeleteGateway(
                callback: (DetailedResponse<GatewayDelete> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "DeleteGateway result: {0}", response.Response);
                    deleteGatewayResponse = response.Result;
                    Assert.IsNotNull(deleteGatewayResponse);
                    Assert.IsTrue(deleteGatewayResponse.GatewayId == gatewayId);
                    Assert.IsTrue(deleteGatewayResponse.Status == "deleted");
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                gatewayId: gatewayId
            );

            while (deleteGatewayResponse == null)
                yield return null;
        }
        #endregion

        #region DeleteCredentials
        [UnityTest, Order(88)]
        public IEnumerator TestDeleteCredentials()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to DeleteCredentials...");
            DeleteCredentials deleteCredentialsResponse = null;
            service.DeleteCredentials(
                callback: (DetailedResponse<DeleteCredentials> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "DeleteCredentials result: {0}", response.Response);
                    deleteCredentialsResponse = response.Result;
                    Assert.IsNotNull(deleteCredentialsResponse);
                    Assert.IsTrue(deleteCredentialsResponse.CredentialId == credentialId);
                    Assert.IsTrue(deleteCredentialsResponse.Status == "deleted");
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                credentialId: credentialId
            );

            while (deleteCredentialsResponse == null)
                yield return null;
        }
        #endregion

        #region DeleteUserData
        [UnityTest, Order(89)]
        public IEnumerator TestDeleteUserData()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to DeleteUserData...");
            bool isComplete = false;
            service.DeleteUserData(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "DeleteUserData result: {0}", response.Response);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                customerId: "customerId"
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region DeleteTrainingExample
        [UnityTest, Order(90)]
        public IEnumerator TestDeleteTrainingExample()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to DeleteTrainingExample...");
            bool isComplete = false;
            service.DeleteTrainingExample(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "DeleteTrainingExample result: {0}", response.Response);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                environmentId: environmentId,
                collectionId: collectionId,
                queryId: queryId,
                exampleId: addedDocumentId
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region DeleteTrainingData
        [UnityTest, Order(91)]
        public IEnumerator TestDeleteTrainingData()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to DeleteTrainingData...");
            bool isComplete = false;
            service.DeleteTrainingData(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "DeleteTrainingData result: {0}", response.Response);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                environmentId: environmentId,
                collectionId: collectionId,
                queryId: queryId
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region DeleteAllTrainingData
        [UnityTest, Order(92)]
        public IEnumerator TestDeleteAllTrainingData()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to DeleteAllTrainingData...");
            bool isComplete = false;
            service.DeleteAllTrainingData(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "DeleteAllTrainingData result: {0}", response.Response);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                environmentId: environmentId,
                collectionId: collectionId
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region DeleteDocument
        [UnityTest, Order(92)]
        public IEnumerator TestDeleteDocument()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to DeleteDocument...");
            DeleteDocumentResponse deleteDocumentResponse = null;
            service.DeleteDocument(
                callback: (DetailedResponse<DeleteDocumentResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "DeleteDocument result: {0}", response.Response);
                    deleteDocumentResponse = response.Result;
                    Assert.IsNotNull(deleteDocumentResponse);
                    Assert.IsNotNull(deleteDocumentResponse.DocumentId);
                    Assert.IsTrue(deleteDocumentResponse.Status == "deleted");
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: collectionId,
                documentId: addedDocumentId
            );

            while (deleteDocumentResponse == null)
                yield return null;
        }
        #endregion

        #region DeleteTokenizationDictionary
        //  Skipping because of length
        //[UnityTest, Order(93)]
        public IEnumerator TestDeleteTokenizationDictionary()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to DeleteTokenizationDictionary...");
            bool isComplete = false;
            service.DeleteTokenizationDictionary(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "DeleteTokenizationDictionary result: {0}", response.Response);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                environmentId: environmentId,
                collectionId: createdJapaneseCollectionId
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region DeleteStopwordList
        //  Skipping because of length
        //[UnityTest, Order(94)]
        public IEnumerator TestDeleteStopwordList()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to DeleteStopwordList...");
            bool isComplete = false;
            service.DeleteStopwordList(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "DeleteStopwordList result: {0}", response.Response);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                environmentId: environmentId,
                collectionId: collectionId
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region DeleteExpansions
        [UnityTest, Order(95)]
        public IEnumerator TestDeleteExpansions()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to DeleteExpansions...");
            bool isComplete = false;
            service.DeleteExpansions(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "DeleteExpansions result: {0}", response.Response);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                environmentId: environmentId,
                collectionId: collectionId
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region DeleteJapaneseCollection
        [UnityTest, Order(96)]
        public IEnumerator TestDeleteJapanseCollection()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to DeleteJapanseCollection...");
            DeleteCollectionResponse deleteCollectionResponse = null;
            service.DeleteCollection(
                callback: (DetailedResponse<DeleteCollectionResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "DeleteJapaneseCollection result: {0}", response.Response);
                    deleteCollectionResponse = response.Result;
                    Assert.IsNotNull(deleteCollectionResponse);
                    Assert.IsNotNull(deleteCollectionResponse.CollectionId);
                    Assert.IsTrue(deleteCollectionResponse.CollectionId == createdJapaneseCollectionId);
                    Assert.IsTrue(deleteCollectionResponse.Status == "deleted");
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: createdJapaneseCollectionId
            );

            while (deleteCollectionResponse == null)
                yield return null;
        }
        #endregion

        #region DeleteCollection
        [UnityTest, Order(97)]
        public IEnumerator TestDeleteCollection()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to DeleteCollection...");
            DeleteCollectionResponse deleteCollectionResponse = null;
            service.DeleteCollection(
                callback: (DetailedResponse<DeleteCollectionResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "DeleteCollection result: {0}", response.Response);
                    deleteCollectionResponse = response.Result;
                    Assert.IsNotNull(deleteCollectionResponse);
                    Assert.IsNotNull(deleteCollectionResponse.CollectionId);
                    Assert.IsTrue(deleteCollectionResponse.CollectionId == collectionId);
                    Assert.IsTrue(deleteCollectionResponse.Status == "deleted");
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                collectionId: collectionId
            );

            while (deleteCollectionResponse == null)
                yield return null;
        }
        #endregion

        #region DeleteConfiguration
        [UnityTest, Order(98)]
        public IEnumerator TestDeleteConfiguration()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to DeleteConfiguration...");
            DeleteConfigurationResponse deleteConfigurationResponse = null;
            service.DeleteConfiguration(
                callback: (DetailedResponse<DeleteConfigurationResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "DeleteConfiguration result: {0}", response.Response);
                    deleteConfigurationResponse = response.Result;
                    Assert.IsNotNull(deleteConfigurationResponse);
                    Assert.IsNull(error);
                },
                environmentId: environmentId,
                configurationId: createdConfigurationId
            );

            while (deleteConfigurationResponse == null)
                yield return null;
        }
        #endregion

        #region DeleteEnvironment
        //  Disabled since we are not creating an environment
        //[UnityTest, Order(99)]
        public IEnumerator TestDeleteEnvironment()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Attempting to DeleteEnvironment...");
            DeleteEnvironmentResponse deleteEnvironmentResponse = null;
            service.DeleteEnvironment(
                callback: (DetailedResponse<DeleteEnvironmentResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "DeleteEnvironment result: {0}", response.Response);
                    deleteEnvironmentResponse = response.Result;
                    Assert.IsNotNull(deleteEnvironmentResponse);
                    Assert.IsNull(error);
                },
                environmentId: createdEnvironmentId
            );

            while (deleteEnvironmentResponse == null)
                yield return null;
        }
        #endregion

        #region CheckTokenizationDictionaryStatus
        private IEnumerator CheckTokenizationDictionaryStatus(string environmentId, string collectionId)
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Checking tokenization dictionary status in 30 sec...");
            yield return new WaitForSeconds(30f);

            TokenDictStatusResponse getTokenizationDictionaryStatusResponse = null;
            try
            {
                service.GetTokenizationDictionaryStatus(
                callback: (DetailedResponse<TokenDictStatusResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "GetTokenizationDictionaryStatus result: {0}", response.Response);
                    if (getTokenizationDictionaryStatusResponse.Status == TokenDictStatusResponse.StatusValue.ACTIVE)
                    {
                        isTokenizationDictionaryReady = true;
                    }
                    else
                    {
                        Runnable.Run(CheckTokenizationDictionaryStatus(environmentId, collectionId));
                    }
                    getTokenizationDictionaryStatusResponse = response.Result;
                },
                environmentId: environmentId,
                collectionId: collectionId
            );
            }
            catch
            {
                Runnable.Run(CheckTokenizationDictionaryStatus(environmentId, collectionId));
            }

            while (getTokenizationDictionaryStatusResponse == null)
                yield return null;
        }
        #endregion

        #region CheckStopwordsListStatus
        private IEnumerator CheckStopwordsListStatus()
        {
            Log.Debug("DiscoveryServiceV1IntegrationTests", "Checking stopword list status in 30 sec...");
            yield return new WaitForSeconds(30f);

            try
            {
                service.GetStopwordListStatus(
                    callback: OnCheckStopwordsListStatus,
                    environmentId: environmentId,
                    collectionId: collectionId
                );
            }
            catch
            {
                Runnable.Run(CheckStopwordsListStatus());
            }
        }

        private void OnCheckStopwordsListStatus(DetailedResponse<TokenDictStatusResponse> response, IBMError error)
        {
            TokenDictStatusResponse getStopwordListStatusResponse = null;
            Log.Debug("DiscoveryServiceV1IntegrationTests", "OnCheckStopwordsListStatus result: {0}", response.Response);
            getStopwordListStatusResponse = response.Result;

            if (getStopwordListStatusResponse.Status == TokenDictStatusResponse.StatusValue.ACTIVE)
            {
                isStopwordsListReady = true;
            }
            else
            {
                Runnable.Run(CheckStopwordsListStatus());
            }
        }
        #endregion

        [UnityTest, Order(100)]
        [Timeout(int.MaxValue)]
        public IEnumerator DeleteUnityCollections()
        {
            ListCollectionsResponse listCollectionsResponse = null;
            service.ListCollections(
                callback: (DetailedResponse<ListCollectionsResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV1IntegrationTests", "ListCollections result: {0}", response.Response);
                    listCollectionsResponse = response.Result;
                    Assert.IsNotNull(listCollectionsResponse);
                    Assert.IsNotNull(listCollectionsResponse.Collections);
                    Assert.IsTrue(listCollectionsResponse.Collections.Count > 0);
                    Assert.IsNull(error);
                },
                environmentId: environmentId
            );

            while (listCollectionsResponse == null)
                yield return null;

            List<string> collectionIdsToDelete = new List<string>();
            int count = 0;
            foreach (Collection collection in listCollectionsResponse.Collections)
            {
                if (!string.IsNullOrEmpty(collection.Description) && collection.Description.Contains("Unity"))
                    collectionIdsToDelete.Add(collection.CollectionId);
            }

            foreach (string collectionId in collectionIdsToDelete)
                service.DeleteCollection(
                    callback: (DetailedResponse<DeleteCollectionResponse> response, IBMError error) =>
                    {
                        Log.Debug("DiscoveryServiceV1IntegrationTests", "DeleteCollection result: {0}", response.Response);
                        count++;
                    },
                    environmentId: environmentId,
                    collectionId: collectionId
                );

            while (count < collectionIdsToDelete.Count)
                yield return null;
        }
    }
}
