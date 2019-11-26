/**
* Copyright 2018, 2019 IBM Corp. All Rights Reserved.
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
        private string versionDate = "2019-11-25";

        private string projectId = "{project_id}";
        private string documentId;
        private string collectionId = "{collection_id}";
        private string addDocumentFile;
        private string queryId;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            LogSystem.InstallDefaultReactors();
            addDocumentFile = Application.dataPath + "/Watson/Tests/TestData/DiscoveryV2/TestAddDoc.pdf";
        }

        [UnitySetUp]
        public IEnumerator UnityTestSetup()
        {
            BearerTokenAuthenticator authenticator = new BearerTokenAuthenticator(bearerToken: "{bearer_token}");
            if (service == null)
            {
                service = new DiscoveryService(versionDate, authenticator);
            }
            service.SetServiceUrl("service_url");
            service.DisableSslVerification = true;

            while (!service.Authenticator.CanAuthenticate())
                yield return null;
        }

        [SetUp]
        public void TestSetup()
        {
            service.WithHeader("X-Watson-Test", "1");
        }

        #region ListCollections
        [UnityTest, Order(1)]
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
        [UnityTest, Order(2)]
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
        [UnityTest, Order(3)]
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
        [UnityTest, Order(4)]
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
        [UnityTest, Order(5)]
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
        [UnityTest, Order(6)]
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
        [UnityTest, Order(99)]
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
        [UnityTest, Order(7)]
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
        [UnityTest, Order(8)]
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
        [UnityTest, Order(9)]
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
        [UnityTest, Order(10)]
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
        [UnityTest, Order(98)]
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
        [UnityTest, Order(11)]
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
        [UnityTest, Order(12)]
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
    }
}
