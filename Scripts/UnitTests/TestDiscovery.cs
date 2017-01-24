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

using IBM.Watson.DeveloperCloud.Services.Discovery.v1;
using IBM.Watson.DeveloperCloud.Logging;
using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Utilities;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestDiscovery : UnitTest
    {
        private Discovery m_Discovery = new Discovery();

        //  Environment
        private string m_CreatedEnvironmentName = "unity-sdk-integration-test";
        private string m_CreatedEnvironmentDescription = "Integration test running for Unity SDK. Please do not delete this environment until 10 minutes after the status is 'active'. The test should delete this environment.";
        private string m_CreatedEnvironmentID;
        private bool m_IsEnvironmentActive = false;

        //  Configuration
        private string m_ConfigurationJsonPath;
        private string m_CreatedConfigurationID;

        //  Collection
        private string m_CreatedCollectionName = "Unity SDK Created Collection";
        private string m_CreatedCollectionDescription = "A collection created by the Unity SDK.";
        private string m_CreatedCollectionID;

        //  Document
        private string m_FilePathToIngest;  //  File path for PreviewConfiguration
        private string m_DocumentFilePath;
        private string m_Metadata = "{\n\t\"Creator\": \"Unity SDK Integration Test\",\n\t\"Subject\": \"Discovery service\"\n}";
        private string m_CreatedDocumentID;

        //  Query
        private string m_Query = "What is the capital of china?";

        //  Add and get Environments
        private bool m_GetEnvironmentsTested = false;
        private bool m_AddEnvironmentsTested = false;
        private bool m_GetEnvironmentTested = false;

        //  Add and get Configurations
        private bool m_GetConfigurationsTested = false;
        private bool m_AddConfigurationTested = false;
        private bool m_GetConfigurationTested = false;

        //  Preview Configuration
        private bool m_PreviewConfigurationTested = false;

        //  Add and get Collections
        private bool m_GetCollectionsTested = false;
        private bool m_AddCollectionTested = false;
        private bool m_GetCollectionTested = false;

        //  Get Fields
        private bool m_GetFieldsTested = false;

        //  Add and get Documents
        private bool m_AddDocumentTested = false;
        private bool m_GetDocumentTested = false;
        private bool m_UpdateDocumentTested = false;

        //  Query
        private bool m_QueryTested = false;

        //  Delete
        private bool m_DeleteDocumentTested = false;
        private bool m_DeleteCollectionTested = false;
        private bool m_DeleteConfigurationTested = false;
        private bool m_DeleteEnvironmentTested = false;

        public override IEnumerator RunTest()
        {
            LogSystem.InstallDefaultReactors();
            
            m_ConfigurationJsonPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/Discovery/exampleConfigurationData.json";
            m_FilePathToIngest = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/watson_beats_jeopardy.html";
            m_DocumentFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/watson_beats_jeopardy.html";

            #region Get Environments and Add Environment
            //  Get Environments
            Log.Debug("ExampleDiscoveryV1", "Attempting to get environments");
            if (!m_Discovery.GetEnvironments((GetEnvironmentsResponse resp, string data) =>
            {
                if (resp != null)
                {
                    foreach (Environment environment in resp.environments)
                        Log.Debug("ExampleDiscoveryV1", "environment_id: {0}", environment.environment_id);
                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Discovery.GetEnvironments(); resp is null");
                }

                Test(resp.environments != null);
                m_GetEnvironmentsTested = true;
            }))
                Log.Debug("ExampleDiscoveryV1", "Failed to get environments");

            while (!m_GetEnvironmentsTested)
                yield return null;

            //  Add Environment
            Log.Debug("ExampleDiscoveryV1", "Attempting to add environment");
            if (!m_Discovery.AddEnvironment((Environment resp, string data) =>
            {
                if (resp != null)
                {
                    Log.Debug("ExampleDiscoveryV1", "Added {0}", resp.environment_id);
                    m_CreatedEnvironmentID = resp.environment_id;
                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Discovery.AddEnvironment(); resp is null, {0}", data);
                }

                Test(!string.IsNullOrEmpty(resp.environment_id));
                m_AddEnvironmentsTested = true;
            }, m_CreatedEnvironmentName, m_CreatedEnvironmentDescription, 0))
                Log.Debug("ExampleDiscoveryV1", "Failed to add environment");

            while (!m_AddEnvironmentsTested)
                yield return null;

            //  Check for active environment
            Runnable.Run(CheckEnvironmentStatus());
            while (!m_IsEnvironmentActive)
                yield return null;
            #endregion

            #region Get Configurations and add Configuration
            //  Get Configurations
            Log.Debug("ExampleDiscoveryV1", "Attempting to get configurations");
            if (!m_Discovery.GetConfigurations((GetConfigurationsResponse resp, string data) =>
            {
                if (resp != null)
                {
                    if (resp.configurations != null && resp.configurations.Length > 0)
                    {
                        foreach (ConfigurationRef configuration in resp.configurations)
                        {
                            Log.Debug("ExampleDiscoveryV1", "Configuration: {0}, {1}", configuration.configuration_id, configuration.name);
                        }
                    }
                    else
                    {
                        Log.Debug("ExampleDiscoveryV1", "There are no configurations for this environment.");
                    }
                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Discovery.GetConfigurations(); resp is null, {0}", data);
                }

                Test(resp.configurations != null);
                m_GetConfigurationsTested = true;
            }, m_CreatedEnvironmentID))
                Log.Debug("ExampleDiscoveryV1", "Failed to get configurations");

            while (!m_GetConfigurationsTested)
                yield return null;

            //  Add Configuration
            Log.Debug("ExampleDiscoveryV1", "Attempting to add configuration");
            if (!m_Discovery.AddConfiguration((Configuration resp, string data) =>
            {
                if (resp != null)
                {
                    Log.Debug("ExampleDiscoveryV1", "Configuration: {0}, {1}", resp.configuration_id, resp.name);
                    m_CreatedConfigurationID = resp.configuration_id;


                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Discovery.AddConfiguration(); resp is null, {0}", data);
                }

                Test(!string.IsNullOrEmpty(resp.configuration_id));
                m_AddConfigurationTested = true;
            }, m_CreatedEnvironmentID, m_ConfigurationJsonPath))
                Log.Debug("ExampleDiscoveryV1", "Failed to add configuration");

            while (!m_AddConfigurationTested)
                yield return null;
            #endregion

            #region GetCollections and Add Collection
            //  Get Collections
            Log.Debug("ExampleDiscoveryV1", "Attempting to get collections");
            if (!m_Discovery.GetCollections((GetCollectionsResponse resp, string customData) =>
            {
                if (resp != null)
                {
                    if (resp.collections != null && resp.collections.Length > 0)
                    {
                        foreach (CollectionRef collection in resp.collections)
                            Log.Debug("ExampleDiscoveryV1", "Collection: {0}, {1}", collection.collection_id, collection.name);
                    }
                    else
                    {
                        Log.Debug("ExampleDiscoveryV1", "There are no collections");
                    }
                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Discovery.GetCollections(); resp is null");
                }

                Test(resp != null);
                m_GetCollectionsTested = true;
            }, m_CreatedEnvironmentID))
            {
                Log.Debug("ExampleDiscovery", "Failed to get collections");
            }

            while (!m_GetCollectionsTested)
                yield return null;

            //  Add Collection
            Log.Debug("ExampleDiscoveryV1", "Attempting to add collection");
            if (!m_Discovery.AddCollection((CollectionRef resp, string data) =>
            {
                if (resp != null)
                {
                    Log.Debug("ExampleDiscoveryV1", "Collection: {0}, {1}", resp.collection_id, resp.name);
                    m_CreatedCollectionID = resp.collection_id;
                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Discovery.AddCollection(); resp is null, {0}", data);
                }

                Test(!string.IsNullOrEmpty(resp.collection_id));
                m_AddCollectionTested = true;
            }, m_CreatedEnvironmentID, m_CreatedCollectionName, m_CreatedCollectionDescription, m_CreatedConfigurationID))
                Log.Debug("ExampleDiscovery", "Failed to add collection");

            while (!m_AddCollectionTested)
                yield return null;
            #endregion

            #region Get Fields
            Log.Debug("ExampleDiscoveryV1", "Attempting to get fields");
            if (!m_Discovery.GetFields((GetFieldsResponse resp, string customData) =>
             {
                 if (resp != null)
                 {
                     foreach (Field field in resp.fields)
                         Log.Debug("ExampleDiscoveryV1", "Field: {0}, type: {1}", field.field, field.type);
                 }
                 else
                 {
                     Log.Debug("ExampleDiscoveryV1", "Discovery.GetFields(); resp is null");
                 }

                 Test(resp != null);
                 m_GetFieldsTested = true;
             }, m_CreatedEnvironmentID, m_CreatedCollectionID))
                Log.Debug("ExampleDiscoveryV1", "Failed to get fields");

            while (!m_GetFieldsTested)
                yield return null;
            #endregion

            #region Add Document
            //  Add Document
            Log.Debug("ExampleDiscoveryV1", "Attempting to add document");
            if (!m_Discovery.AddDocument((DocumentAccepted resp, string data) =>
            {
                if (resp != null)
                {
                    Log.Debug("ExampleDiscoveryV1", "Added Document {0} {1}", resp.document_id, resp.status);
                    m_CreatedDocumentID = resp.document_id;
                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Discovery.AddDocument(); resp is null, {0}", data);
                }

                Test(!string.IsNullOrEmpty(resp.document_id));
                m_AddDocumentTested = true;
            }, m_CreatedEnvironmentID, m_CreatedCollectionID, m_DocumentFilePath, m_CreatedConfigurationID, null))
                Log.Debug("ExampleDiscovery", "Failed to add document");

            while (!m_AddDocumentTested)
                yield return null;
            #endregion

            #region Query
            //  Query
            Log.Debug("ExampleDiscoveryV1", "Attempting to query");
            if (!m_Discovery.Query((QueryResponse resp, string data) =>
            {
                if (resp != null)
                {
                    Log.Debug("ExampleDiscoveryV1", resp.ToString());
                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "resp is null, {0}", data);
                }

                Test(resp != null);
                m_QueryTested = true;
            }, m_CreatedEnvironmentID, m_CreatedCollectionID, null, m_Query, null, 10, null, 0))
            {
                Log.Debug("ExampleDiscovery", "Failed to query");
            }

            while (!m_QueryTested)
                yield return null;
            #endregion

            #region Document
            //  Get Document
            Log.Debug("ExampleDiscoveryV1", "Attempting to get document");
            if (!m_Discovery.GetDocument((DocumentStatus resp, string data) =>
            {
                if (resp != null)
                {
                    Log.Debug("ExampleDiscoveryV1", "Got Document {0} {1}", resp.document_id, resp.status);
                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Discovery.GetDocument(); resp is null, {0}", data);
                }

                Test(!string.IsNullOrEmpty(resp.status));
                m_GetDocumentTested = true;
            }, m_CreatedEnvironmentID, m_CreatedCollectionID, m_CreatedDocumentID))
                Log.Debug("ExampleDiscovery", "Failed to get document");

            while (!m_GetDocumentTested)
                yield return null;

            //  Update Document
            Log.Debug("ExampleDiscoveryV1", "Attempting to update document");
            if (!m_Discovery.UpdateDocument((DocumentAccepted resp, string data) =>
            {
                if (resp != null)
                {
                    Log.Debug("ExampleDiscoveryV1", "Updated Document {0} {1}", resp.document_id, resp.status);
                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Discovery.UpdateDocument(); resp is null, {0}", data);
                }

                Test(!string.IsNullOrEmpty(resp.status));
                m_UpdateDocumentTested = true;
            }, m_CreatedEnvironmentID, m_CreatedCollectionID, m_CreatedDocumentID, m_DocumentFilePath, m_CreatedConfigurationID, null))
                Log.Debug("ExampleDiscovery", "Failed to update document");

            while (!m_UpdateDocumentTested)
                yield return null;

            //  Delete Document
            Runnable.Run(DeleteDocument());
            
            while (!m_DeleteDocumentTested)
                yield return null;
            #endregion

            #region Collection
            //  Get Collection
            Log.Debug("ExampleDiscoveryV1", "Attempting to get collection");
            if (!m_Discovery.GetCollection((Collection resp, string data) =>
            {
                if (resp != null)
                {
                    Log.Debug("ExampleDiscoveryV1", "Collection: {0}, {1}", resp.collection_id, resp.name);
                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Failed to get collections");
                }

                Test(!string.IsNullOrEmpty(resp.name));
                m_GetCollectionTested = true;
            }, m_CreatedEnvironmentID, m_CreatedCollectionID))
            {
                Log.Debug("ExampleDiscovery", "Failed to get collection");
            }

            while (!m_GetCollectionTested)
                yield return null;

            //  Delete Collection
            Runnable.Run(DeleteCollection());
            while (!m_DeleteCollectionTested)
                yield return null;
            #endregion

            #region Configuration
            //  Get Configuration
            Log.Debug("ExampleDiscoveryV1", "Attempting to get configuration");
            if (!m_Discovery.GetConfiguration((Configuration resp, string data) =>
            {
                if (resp != null)
                {
                    Log.Debug("ExampleDiscoveryV1", "Configuration: {0}, {1}", resp.configuration_id, resp.name);
                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Discovery.GetConfiguration(); resp is null, {0}", data);
                }

                Test(!string.IsNullOrEmpty(resp.name));
                m_GetConfigurationTested = true;
            }, m_CreatedEnvironmentID, m_CreatedConfigurationID))
                Log.Debug("ExampleDiscoveryV1", "Failed to get configuration");

            while (!m_GetConfigurationTested)
                yield return null;

            //  Preview Configuration
            Log.Debug("ExampleDiscoveryV1", "Attempting to preview configuration");
            if (!m_Discovery.PreviewConfiguration((TestDocument resp, string data) =>
            {
                if (resp != null)
                {
                    Log.Debug("ExampleDiscoveryV1", "Preview succeeded: {0}", resp.status);
                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Discovery.PreviewConfiguration(); resp is null {0}", data);
                }

                Test(resp != null);
                m_PreviewConfigurationTested = true;
            }, m_CreatedEnvironmentID, m_CreatedConfigurationID, null, m_FilePathToIngest, m_Metadata))
                Log.Debug("ExampleDiscoveryV1", "Failed to preview configuration");

            while (!m_PreviewConfigurationTested)
                yield return null;

            //  Delete Configuration
            Runnable.Run(DeleteConfiguration());
            while (!m_DeleteConfigurationTested)
                yield return null;
            #endregion

            #region Environment
            //  Get Environment
            Log.Debug("ExampleDiscoveryV1", "Attempting to get environment");
            if (!m_Discovery.GetEnvironment((Environment resp, string data) =>
            {
                if (resp != null)
                {
                    Log.Debug("ExampleDiscoveryV1", "environment_name: {0}", resp.name);
                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Discovery.GetEnvironment(); resp is null");
                }

                Test(!string.IsNullOrEmpty(resp.name));
                m_GetEnvironmentTested = true;
            }, m_CreatedEnvironmentID))
                Log.Debug("ExampleDiscoveryV1", "Failed to get environment");

            while (!m_GetEnvironmentTested)
                yield return null;

            //  Delete Environment
            Runnable.Run(DeleteEnvironment());

            while (!m_DeleteEnvironmentTested)
                yield return null;
            #endregion

            yield break;
        }

        #region Delete Document
        private IEnumerator DeleteDocument()
        {
            Log.Debug("ExampleDiscoveryV1", "Attempting to delete document {0}", m_CreatedDocumentID);
            if (!m_Discovery.DeleteDocument(HandleDeleteDocument, m_CreatedEnvironmentID, m_CreatedCollectionID, m_CreatedDocumentID))
            {
                Log.Debug("ExampleDiscovery", "Failed to delete document.... making another attempt");
                Runnable.Run(DeleteDocument());
            }

            yield break;
        }

        private void HandleDeleteDocument(bool success, string data)
        {
            if (success)
            {
                Log.Debug("ExampleDiscoveryV1", "Delete document successful");
                m_CreatedDocumentID = default(string);
                Test(success);
            }
            else
            {
                Log.Debug("ExampleDiscovery", "Failed to delete document.... making another attempt");
                Runnable.Run(DeleteDocument());
            }

            m_DeleteDocumentTested = true;
        }
        #endregion

        #region Delete Collection
        private IEnumerator DeleteCollection()
        {
            Log.Debug("ExampleDiscoveryV1", "Attempting to delete collection {0}", m_CreatedCollectionID);
            if (!m_Discovery.DeleteCollection(HandleDeleteCollection, m_CreatedEnvironmentID, m_CreatedCollectionID))
            {
                Log.Debug("ExampleDiscovery", "Failed to add collection... making another attempt");
                Runnable.Run(DeleteCollection());
            }

            yield break;
        }

        private void HandleDeleteCollection(bool success, string data)
        {
            if (success)
            {
                Log.Debug("ExampleDiscoveryV1", "Delete collection successful");
                m_CreatedCollectionID = default(string);
                Test(success);
            }
            else
            {
                Log.Debug("ExampleDiscovery", "Failed to add collection... making another attempt");
                Runnable.Run(DeleteCollection());
            }

            m_DeleteCollectionTested = true;
        }
        #endregion

        #region Delete Configuration
        private IEnumerator DeleteConfiguration()
        {
            Log.Debug("ExampleDiscoveryV1", "Attempting to delete configuration {0}", m_CreatedConfigurationID);
            if (!m_Discovery.DeleteConfiguration(HandleDeleteConfiguration, m_CreatedEnvironmentID, m_CreatedConfigurationID))
            {
                Log.Debug("ExampleDiscoveryV1", "Failed to delete configuration... making another attempt");
                Runnable.Run(DeleteConfiguration());
            }

            yield break;
        }

        private void HandleDeleteConfiguration(bool success, string data)
        {
            if (success)
            {
                Log.Debug("ExampleDiscoveryV1", "Delete configuration successful");
                m_CreatedConfigurationID = default(string);
                Test(success);
            }
            else
            {
                Log.Debug("ExampleDiscoveryV1", "Failed to delete configuration... making another attempt");
                Runnable.Run(DeleteConfiguration());
            }

            m_DeleteConfigurationTested = true;
        }
        #endregion

        #region Delete Environment
        private IEnumerator DeleteEnvironment()
        {
            Log.Debug("ExampleDiscoveryV1", "Attempting to delete environment {0}", m_CreatedEnvironmentID);
            if (!m_Discovery.DeleteEnvironment(HandleDeleteEnvironment, m_CreatedEnvironmentID))
            {
                Log.Debug("ExampleDiscoveryV1", "Failed to delete environment... making another attempt");
                Runnable.Run(DeleteEnvironment());
            }

            yield break;
        }

        private void HandleDeleteEnvironment(bool success, string data)
        {
            if (success)
            {
                Log.Debug("ExampleDiscoveryV1", "Delete environment successful");
                m_CreatedEnvironmentID = default(string);
                Test(success);
            }
            else
            {
                Log.Debug("ExampleDiscoveryV1", "Failed to delete environment... making another attempt");
                Runnable.Run(DeleteEnvironment());
            }

            m_DeleteEnvironmentTested = true;
        }
        #endregion

        #region Environment Status
        private IEnumerator CheckEnvironmentStatus()
        {
            Log.Debug("ExampleDiscoveryV1", "Waiting 10 seconds to check environment status...");
            yield return new WaitForSeconds(10f);

            Log.Debug("ExampleDiscoveryV1", "Attempting to get environment status");
            if (!m_Discovery.GetEnvironment(OnCheckEnvironmentStatus, m_CreatedEnvironmentID))
                Log.Debug("ExampleDiscoveryV1", "Failed to get environment status");
        }

        private void OnCheckEnvironmentStatus(Environment resp, string data)
        {
            if (resp != null)
            {
                Log.Debug("ExampleDiscoveryV1", "Environment {0} is {1}", resp.environment_id, resp.status);
                if (resp.status == "active")
                    m_IsEnvironmentActive = true;
                else
                {
                    Runnable.Run(CheckEnvironmentStatus());
                }
            }
            else
            {
                Log.Debug("ExampleDiscoveryV1", "Failed to get environment status");
                Runnable.Run(CheckEnvironmentStatus());
            }
        }
        #endregion
    }
}
