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
using IBM.Cloud.SDK.Authentication.Bearer;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.Discovery.V2;
using IBM.Watson.Discovery.V2.Model;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace IBM.Watson.Tests
{
    public class DiscoveryV2IntegrationTests
    {
        private DiscoveryService service;
        private string versionDate = "2020-08-12";

        private string projectId;
        private string documentId;
        private string collectionId;
        private string enrichmentId;
        private string addDocumentFile;
        private string enrichmentFile;
        private string analyzeDocumentFile;
        private string queryId;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            LogSystem.InstallDefaultReactors();
            analyzeDocumentFile = Application.dataPath + "/Watson/Tests/TestData/DiscoveryV1/exampleConfigurationData.json";

            addDocumentFile = Application.dataPath + "/Watson/Tests/TestData/DiscoveryV2/TestAddDoc.pdf";
            enrichmentFile = Application.dataPath + "/Watson/Tests/TestData/DiscoveryV2/TestEnrichments.csv";
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

        #region CreateProject
        //[UnityTest, Order(0)]
        public IEnumerator TestCreateProject()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to CreateProject...");
            ProjectDetails createProjectResponse = null;
            service.CreateProject(
                callback: (DetailedResponse<ProjectDetails> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "CreateProject result: {0}", response.Response);
                    createProjectResponse = response.Result;
                    Assert.IsNotNull(createProjectResponse);
                    projectId = createProjectResponse.ProjectId;
                    Assert.IsNotNull(projectId);
                    Assert.IsNull(error);
                },
                name: "Unity SDK test project",
                type: "other"
            );

            while (createProjectResponse == null)
                yield return null;
        }
        #endregion

        #region CreateCollection
        //[UnityTest, Order(1)]
        public IEnumerator TestCreateCollection()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to CreateCollection...");
            CollectionDetails createCollectionResponse = null;
            service.CreateCollection(
                callback: (DetailedResponse<CollectionDetails> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "CreateCollection result: {0}", response.Response);
                    createCollectionResponse = response.Result;
                    Assert.IsNotNull(createCollectionResponse);
                    collectionId = createCollectionResponse.CollectionId;
                    Assert.IsNotNull(createCollectionResponse.CollectionId);
                    Assert.IsNull(error);
                },
                projectId: projectId,
                name: "Unity SDK test collection",
                description: "test collection"
            );

            while (createCollectionResponse == null)
                yield return null;
        }
        #endregion

        #region ListCollections
        //[UnityTest, Order(2)]
        public IEnumerator TestListCollections()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to ListCollections...");
            ListCollectionsResponse listCollectionsResponse = null;
            service.ListCollections(
                callback: (DetailedResponse<ListCollectionsResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "ListCollections result: {0}", response.Response);
                    listCollectionsResponse = response.Result;
                    Assert.IsNotNull(listCollectionsResponse);
                    Assert.IsNotNull(listCollectionsResponse.Collections);
                    Assert.IsTrue(listCollectionsResponse.Collections.Count > 0);
                    Assert.IsNull(error);
                },
                projectId: projectId
            );

            while (listCollectionsResponse == null)
                yield return null;
        }
        #endregion

        #region Query
        //[UnityTest, Order(4)]
        public IEnumerator TestQuery()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to Query...");
            QueryResponse queryResponse = null;
            service.Query(
                callback: (DetailedResponse<QueryResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "Query result: {0}", response.Response);
                    queryResponse = response.Result;
                    Assert.IsNotNull(queryResponse);
                    Assert.IsNotNull(queryResponse.Aggregations);
                    Assert.IsTrue(queryResponse.Aggregations.Count > 0);
                    Assert.IsNull(error);
                },
                projectId: projectId,
                query: "hello: world"
            );

            while (queryResponse == null)
                yield return null;
        }
        #endregion


        #region GetAutocompletion
        //[UnityTest, Order(5)]
        public IEnumerator TestGetAutocompletion()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to GetAutocompletion...");
            Completions completionsResponse = null;
            service.GetAutocompletion(
                callback: (DetailedResponse<Completions> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "GetAutocompletion result: {0}", response.Response);
                    completionsResponse = response.Result;
                    Assert.IsNotNull(completionsResponse);
                    Assert.IsNull(error);
                },
                projectId: projectId,
                prefix: "Ho",
                count: 5
            );

            while (completionsResponse == null)
                yield return null;
        }
        #endregion

        #region GetComponentSettings
        //[UnityTest, Order(6)]
        public IEnumerator TestGetComponentSettings()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to GetComponentSettings...");
            ComponentSettingsResponse componentSettingsResponse = null;
            service.GetComponentSettings(
                callback: (DetailedResponse<ComponentSettingsResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "GetComponentSettings result: {0}", response.Response);
                    componentSettingsResponse = response.Result;
                    Assert.IsNotNull(componentSettingsResponse);
                    Assert.IsNotNull(componentSettingsResponse.Aggregations);
                    Assert.IsTrue(componentSettingsResponse.Aggregations.Count > 0);
                    Assert.IsNull(error);
                },
                projectId: projectId
            );

            while (componentSettingsResponse == null)
                yield return null;
        }
        #endregion

        #region AddDocument
        //[UnityTest, Order(3)]
        public IEnumerator TestAddDocument()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to AddDocument...");
            DocumentAccepted addDocumentResponse = null;
            using (FileStream fs = File.OpenRead(addDocumentFile))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    service.AddDocument(
                        callback: (DetailedResponse<DocumentAccepted> response, IBMError error) =>
                        {
                            Log.Debug("DiscoveryServiceV1IntegrationTests", "AddDocument result: {0}", response.Response);
                            addDocumentResponse = response.Result;
                            documentId = addDocumentResponse.DocumentId;
                            Assert.IsNotNull(addDocumentResponse);
                            Assert.IsNotNull(documentId);
                            Assert.IsNull(error);
                        },
                        projectId: projectId,
                        collectionId: collectionId,
                        file: ms,
                        fileContentType: Utility.GetMimeType(Path.GetExtension(addDocumentFile)),
                        filename: Path.GetFileName(addDocumentFile)
                    );

                    while (addDocumentResponse == null)
                        yield return null;
                }
            }
        }
        #endregion

        #region UpdateDocument
        //[UnityTest, Order(7)]
        public IEnumerator TestUpdateDocument()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to UpdateDocument...");
            DocumentAccepted updateDocumentResponse = null;
            service.UpdateDocument(
                callback: (DetailedResponse<DocumentAccepted> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "UpdateDocument result: {0}", response.Response);
                    updateDocumentResponse = response.Result;
                    Assert.IsNotNull(updateDocumentResponse);
                    Assert.IsNull(error);
                },
                projectId: projectId,
                collectionId: collectionId,
                documentId: documentId,
                filename: "update-file-name"
            );

            while (updateDocumentResponse == null)
                yield return null;
        }
        #endregion

        #region DeleteDocument
        //[UnityTest, Order(99)]
        public IEnumerator TestDeleteDocument()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to DeleteDocument...");
            DeleteDocumentResponse deleteDocumentResponse = null;
            service.DeleteDocument(
                callback: (DetailedResponse<DeleteDocumentResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "DeleteDocument result: {0}", response.Response);
                    deleteDocumentResponse = response.Result;
                    Assert.IsNotNull(deleteDocumentResponse);
                    Assert.IsNull(error);
                },
                projectId: projectId,
                collectionId: collectionId,
                documentId: documentId
            );

            while (deleteDocumentResponse == null)
                yield return null;
        }
        #endregion

        #region ListTrainingQueries
        //[UnityTest, Order(7)]
        public IEnumerator TestListTrainingQueries()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to ListTrainingQueries...");
            TrainingQuerySet listTrainingQueriesResponse = null;
            service.ListTrainingQueries(
                callback: (DetailedResponse<TrainingQuerySet> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "ListTrainingQueries result: {0}", response.Response);
                    listTrainingQueriesResponse = response.Result;
                    Assert.IsNotNull(listTrainingQueriesResponse);
                    Assert.IsNotNull(listTrainingQueriesResponse.Queries);
                    Assert.IsNull(error);
                },
                projectId: projectId
            );

            while (listTrainingQueriesResponse == null)
                yield return null;
        }
        #endregion

        #region CreateTrainingQuery
        //[UnityTest, Order(8)]
        public IEnumerator TestCreateTrainingQuery()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to CreateTrainingQuery...{0}???", documentId);
            TrainingExample trainingExample = new TrainingExample()
            {
              CollectionId = collectionId,
              DocumentId = documentId
            };
            TrainingQuery trainingQueryResponse = null;
            service.CreateTrainingQuery(
                callback: (DetailedResponse<TrainingQuery> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "CreateTrainingQuery result: {0}", response.Response);
                    trainingQueryResponse = response.Result;
                    queryId = trainingQueryResponse.QueryId;
                    Assert.IsNotNull(trainingQueryResponse);
                    Assert.IsNull(error);
                },
                projectId: projectId,
                examples: new List<TrainingExample>() { trainingExample },
                filter: "entities.text:IBM",
                naturalLanguageQuery: "This is an example of a query"
            );

            while (trainingQueryResponse == null)
                yield return null;
        }
        #endregion

        #region UpdateTrainingQuery
        //[UnityTest, Order(9)]
        public IEnumerator TestUpdateTrainingQuery()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to UpdateTrainingQuery...");
            TrainingExample trainingExample = new TrainingExample()
            {
              CollectionId = collectionId,
              DocumentId = documentId
            };
            TrainingQuery trainingQueryResponse = null;
            service.UpdateTrainingQuery(
                callback: (DetailedResponse<TrainingQuery> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "UpdateTrainingQuery result: {0}", response.Response);
                    trainingQueryResponse = response.Result;
                    queryId = trainingQueryResponse.QueryId;
                    Assert.IsNotNull(trainingQueryResponse);
                    Assert.IsNull(error);
                },
                projectId: projectId,
                queryId: queryId,
                examples: new List<TrainingExample>() { trainingExample },
                filter: "entities.text:IBM",
                naturalLanguageQuery: "This is a new example of a query"
            );

            while (trainingQueryResponse == null)
                yield return null;
        }
        #endregion

        #region GetTrainingQuery
        //[UnityTest, Order(10)]
        public IEnumerator TestGetTrainingQuery()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to GetTrainingQuery...");
            TrainingExample trainingExample = new TrainingExample()
            {
              CollectionId = collectionId,
              DocumentId = documentId
            };
            TrainingQuery trainingQueryResponse = null;
            service.GetTrainingQuery(
                callback: (DetailedResponse<TrainingQuery> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "GetTrainingQuery result: {0}", response.Response);
                    trainingQueryResponse = response.Result;
                    Assert.IsNotNull(trainingQueryResponse);
                    Assert.IsNull(error);
                },
                projectId: projectId,
                queryId: queryId
            );

            while (trainingQueryResponse == null)
                yield return null;
        }
        #endregion

        #region DeleteTrainingQueries
        //[UnityTest, Order(98)]
        public IEnumerator TestDeleteTrainingQueries()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to DeleteTrainingQueries...");
            var trainingQueryResponse = false;
            service.DeleteTrainingQueries(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "DeleteTrainingQueries result: {0}", response.Response);
                    trainingQueryResponse = true;
                    Assert.IsNotNull(trainingQueryResponse);
                    Assert.IsNull(error);
                },
                projectId: projectId
            );

            while (trainingQueryResponse == false)
                yield return null;
        }
        #endregion

        #region QueryNotices
        //[UnityTest, Order(11)]
        public IEnumerator TestQueryNotices()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to QueryNotices...");
            QueryNoticesResponse queryNoticesResponse = null;
            service.QueryNotices(
                callback: (DetailedResponse<QueryNoticesResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "QueryNotices result: {0}", response.Response);
                    queryNoticesResponse = response.Result;
                    Assert.IsNotNull(queryNoticesResponse);
                    Assert.IsNull(error);
                },
                projectId: projectId,
                query: "relations.action.lemmatized:acquire"
            );

            while (queryNoticesResponse == null)
                yield return null;
        }
        #endregion


        #region ListFields
        //[UnityTest, Order(12)]
        public IEnumerator TestListFields()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to ListFields...");
            ListFieldsResponse listFieldsResponse = null;
            service.ListFields(
                callback: (DetailedResponse<ListFieldsResponse> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "ListFields result: {0}", response.Response);
                    listFieldsResponse = response.Result;
                    Assert.IsNotNull(listFieldsResponse);
                    Assert.IsNull(error);
                },
                projectId: projectId,
                collectionIds: new List<string>() { collectionId }
            );

            while (listFieldsResponse == null)
                yield return null;
        }
        #endregion

        #region CreateEnrichment
        //[UnityTest, Order(13)]
        public IEnumerator TestCreateEnrichment()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to CreateEnrichment...");
            Enrichment createEnrichmentResponse = null;
            var languages = new List<string>();
            languages.Add("en");
            CreateEnrichment enrichment = new CreateEnrichment()
            {
                Name = "Dictionary Unity",
                Description = "test dictionary",
                Type = "dictionary",
                Options = new EnrichmentOptions()
                {
                    EntityType = "keyword",
                    Languages = languages
                }
            };

            using (FileStream fs = File.OpenRead(enrichmentFile))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    service.CreateEnrichment(
                        callback: (DetailedResponse<Enrichment> response, IBMError error) =>
                        {
                            Log.Debug("DiscoveryServiceV2IntegrationTests", "CreateEnrichment result: {0}", response.Response);
                            createEnrichmentResponse = response.Result;
                            enrichmentId = createEnrichmentResponse.EnrichmentId;
                            Assert.IsNotNull(createEnrichmentResponse);
                            Assert.IsNotNull(createEnrichmentResponse.EnrichmentId);
                            Assert.IsNull(error);
                        },
                        projectId: projectId,
                        file: ms,
                        enrichment: enrichment
                    );

                    while (createEnrichmentResponse == null)
                        yield return null;
                }
            }
        }
        #endregion

        #region GetEnrichment
        //[UnityTest, Order(14)]
        public IEnumerator TestGetEnrichment()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to GetEnrichment...");
            Enrichment getEnrichmentResponse = null;
            service.GetEnrichment(
                callback: (DetailedResponse<Enrichment> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "GetEnrichment result: {0}", response.Response);
                    getEnrichmentResponse = response.Result;
                    Assert.IsNotNull(getEnrichmentResponse);
                    Assert.IsNull(error);
                },
                projectId: projectId,
                enrichmentId: enrichmentId
            );

            while (getEnrichmentResponse == null)
                yield return null;
        }
        #endregion

        #region ListEnrichments
        //[UnityTest, Order(2)]
        public IEnumerator TestListEnrichments()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to ListEnrichments...");
            Enrichments listEnrichmentsResponse = null;
            service.ListEnrichments(
                callback: (DetailedResponse<Enrichments> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "ListEnrichments result: {0}", response.Response);
                    listEnrichmentsResponse = response.Result;
                    Assert.IsNotNull(listEnrichmentsResponse);
                    Assert.IsNull(error);
                },
                projectId: projectId
            );

            while (listEnrichmentsResponse == null)
                yield return null;
        }
        #endregion


        #region DeleteEnrichment
        //[UnityTest, Order(17)]
        public IEnumerator TestDeleteEnrichment()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to DeleteEnrichment...");
            bool deleteEnrichmentResponse = false;
            service.DeleteEnrichment(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "DeleteEnrichment result: {0}", response.Response);
                    deleteEnrichmentResponse = true;
                    Assert.IsNull(error);
                },
                projectId: projectId,
                enrichmentId: enrichmentId
            );

            while (!deleteEnrichmentResponse)
                yield return null;
        }
        #endregion

        #region GetCollection
        //[UnityTest, Order(18)]
        public IEnumerator TestGetCollection()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to GetCollection...");
            CollectionDetails collectionDetailsResponse = null;
            service.GetCollection(
                callback: (DetailedResponse<CollectionDetails> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "GetCollection result: {0}", response.Response);
                    collectionDetailsResponse = response.Result;
                    Assert.IsNotNull(collectionDetailsResponse);
                    Assert.IsNull(error);
                },
                projectId: projectId,
                collectionId: collectionId
            );

            while (collectionDetailsResponse == null)
                yield return null;
        }
        #endregion

        #region GetProject
        //[UnityTest, Order(19)]
        public IEnumerator TestGetProject()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to GetProject...");
            ProjectDetails projectDetailsResponse = null;
            service.GetProject(
                callback: (DetailedResponse<ProjectDetails> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "GetProject result: {0}", response.Response);
                    projectDetailsResponse = response.Result;
                    Assert.IsNotNull(projectDetailsResponse);
                    Assert.IsNull(error);
                },
                projectId: projectId
            );

            while (projectDetailsResponse == null)
                yield return null;
        }
        #endregion

        #region AnalyzeDocument
        //[UnityTest, Order(20)]
        public IEnumerator TestAnalyzeDocument()
        {
            BearerTokenAuthenticator authenticator = new BearerTokenAuthenticator(
                bearerToken: "{BEARER_TOKEN}"
            );
            DiscoveryService cpdService = new DiscoveryService(versionDate, authenticator);
            cpdService.SetServiceUrl("{SERVICE_URL}");
            cpdService.WithHeader("X-Watson-Test", "1");

            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to AnalyzeDocument...");
            AnalyzedDocument analyzeDocumentResponse = null;
            using (FileStream fs = File.OpenRead(analyzeDocumentFile))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);

                    cpdService.AnalyzeDocument(
                        callback: (DetailedResponse<AnalyzedDocument> response, IBMError error) =>
                        {
                            Log.Debug("DiscoveryServiceV2IntegrationTests", "AnalyzeDocument result: {0}", response.Response);
                            analyzeDocumentResponse = response.Result;
                            Assert.IsNull(error);
                            Assert.IsNotNull(analyzeDocumentResponse);
                        },
                        projectId: "{projectId}",
                        collectionId: "{collectionId}",
                        file: ms,
                        filename: "exampleConfigurationData.json",
                        fileContentType: "application/json"
                    );
                }
            }

            while (analyzeDocumentResponse == null)
                yield return null;
        }
        #endregion

        #region DeleteCollection
        //[UnityTest, Order(101)]
        public IEnumerator TestDeleteCollection()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to DeleteCollection...");
            bool deleteCollectionResponse = false;
            service.DeleteCollection(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "DeleteCollection result: {0}", response.Response);
                    deleteCollectionResponse = true;
                    Assert.IsNull(error);
                },
                projectId: projectId,
                collectionId: collectionId
            );

            while (!deleteCollectionResponse)
                yield return null;
        }
        #endregion

        #region DeleteProject
        //[UnityTest, Order(102)]
        public IEnumerator TestDeleteProject()
        {
            Log.Debug("DiscoveryServiceV2IntegrationTests", "Attempting to DeleteProject...");
            bool deleteProjectResponse = false;
            service.DeleteProject(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("DiscoveryServiceV2IntegrationTests", "DeleteProject result: {0}", response.Response);
                    deleteProjectResponse = true;
                    Assert.IsNull(error);
                },
                projectId: projectId
            );

            while (!deleteProjectResponse)
                yield return null;
        }
        #endregion
    }
}
