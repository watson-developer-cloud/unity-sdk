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
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.Discovery.v1;
using UnityEngine;

public class ExampleDiscoveryV1 : MonoBehaviour
{
    private Discovery m_Discovery = new Discovery();
    private string m_CreatedEnvironmentID;
    private string m_DefaultEnvironmentID = "6c8647b7-9dd4-42c8-9cb0-117b40b14517";
    private string m_DefaultConfigurationID = "662a2032-9e2c-472b-9eaa-1a2fa098c22e";
    private string m_DefaultCollectionID = "336f2f0e-e771-424e-a7b4-331240c8f136";
    private string m_ConfigurationJsonPath;
    private string m_CreatedConfigurationID;
    private string m_FilePathToIngest;
    private string m_Metadata = "{\n\t\"Creator\": \"Unity SDK Integration Test\",\n\t\"Subject\": \"Discovery service\"\n}";
    private string m_CreatedCollectionID;
    private string m_CreatedCollectionName = "Unity SDK Created Collection";
    private string m_CreatedCollectionDescription = "A collection created by the Unity SDK. Please delete me.";
    private string m_CreatedDocumentID;
    private string m_DocumentFilePath;
    private string m_Query = "What is the capital of china?";

    private bool m_IsConfigDeleted = false;
    private bool m_IsCollectionDeleted = false;
    private void Start()
    {
        LogSystem.InstallDefaultReactors();

        m_ConfigurationJsonPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/Discovery/exampleConfigurationData.json";
        m_FilePathToIngest = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/watson_beats_jeopardy.html";
        m_DocumentFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/watson_beats_jeopardy.html";

        ////  Get Environments
        //Log.Debug("ExampleDiscoveryV1", "Attempting to get environments");
        //if (!m_Discovery.GetEnvironments(OnGetEnvironments))
        //    Log.Debug("ExampleDiscoveryV1", "Failed to get environments");

        ////  GetEnvironment
        //Log.Debug("ExampleDiscoveryV1", "Attempting to get environment");
        //if(!m_Discovery.GetEnvironment(OnGetEnvironment, "6c8647b7-9dd4-42c8-9cb0-117b40b14517"))
        //    Log.Debug("ExampleDiscoveryV1", "Failed to get environment");

        //  AddEnvironment
        //Log.Debug("ExampleDiscoveryV1", "Attempting to add environment");
        //if (!m_Discovery.AddEnvironment(OnAddEnvironment, "unity-testing-AddEnvironment-do-not-delete-until-active", "Testing addEnvironment in Unity SDK. Please do not delete this environment until the status is 'active'", 0))
        //    Log.Debug("ExampleDiscoveryV1", "Failed to add environment");

        ////  Get Configurations
        //Log.Debug("ExampleDiscoveryV1", "Attempting to get configurations");
        //if(!m_Discovery.GetConfigurations(OnGetConfigurations, m_DefaultEnvironmentID))
        //    Log.Debug("ExampleDiscoveryV1", "Failed to get configurations");

        ////  Get Configuration
        //Log.Debug("ExampleDiscoveryV1", "Attempting to get configuration");
        //if (!m_Discovery.GetConfiguration(OnGetConfiguration, m_DefaultEnvironmentID, m_DefaultConfigurationID))
        //    Log.Debug("ExampleDiscoveryV1", "Failed to get configuration");

        ////  Get Collections
        //Log.Debug("ExampleDiscoveryV1", "Attempting to get collections");
        //if (!m_Discovery.GetCollections(OnGetCollections, m_DefaultEnvironmentID))
        //    Log.Debug("ExampleDiscovery", "Failed to get collections");

        //  Add Collection
        //Log.Debug("ExampleDiscoveryV1", "Attempting to add collection");
        //if (!m_Discovery.AddCollection(OnAddCollection, "8a73b0aa-d8f3-418f-8341-f4ddc336c363", m_CreatedCollectionName, m_CreatedCollectionDescription, "c9b51b49-e6cf-480a-be9f-5b5e06d5f2d2"))
        //    Log.Debug("ExampleDiscovery", "Failed to add collection");

        //  Get Collection
        //Log.Debug("ExampleDiscoveryV1", "Attempting to get collection");
        //if (!m_Discovery.GetCollection(OnGetCollection, "8a73b0aa-d8f3-418f-8341-f4ddc336c363", "9388fbf8-38f6-44b1-81d2-9b4bade6b9c2"))
        //    Log.Debug("ExampleDiscovery", "Failed to get collection");

        //  Delete Collection
        //Log.Debug("ExampleDiscoveryV1", "Attempting to delete collection {0}", m_CreatedCollectionID);
        //if (!m_Discovery.DeleteCollection(OnDeleteCollection, "8a73b0aa-d8f3-418f-8341-f4ddc336c363", "9388fbf8-38f6-44b1-81d2-9b4bade6b9c2"))
        //    Log.Debug("ExampleDiscovery", "Failed to add collection");
    }

    #region Check State
    private void CheckState()
    {
        Log.Debug("ExampleDiscoveryV1", "Attempting to get environment state");
        try
        {
            m_Discovery.GetEnvironment(HandleCheckEnvironmentState, m_CreatedEnvironmentID);
        }
        catch(System.Exception e)
        {
            Log.Debug("ExampleDiscoveryV1", string.Format("Failed to get environment state: {0}", e.Message));
            CheckState();
        }
    }

    private void HandleCheckEnvironmentState(Environment resp, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Environment {0} is {1}", resp.environment_id, resp.status);

        if (resp.status == "active")
            TestAddCollection();
        else
        {
            Invoke("CheckState", 10f);
        }
    }

    private void BeginDeleteCycle()
    {
        TestDeleteDocument();
        Invoke("TestDeleteCollection", 1f);
        Invoke("TestDeleteConfiguration", 2f);
    }
    #endregion

    private void TestDeleteEnvironment()
    {
        //  DeleteEnvironment
        Log.Debug("ExampleDiscoveryV1", "Attempting to delete environment {0}", m_CreatedEnvironmentID);
        if (!m_Discovery.DeleteEnvironment(OnDeleteEnvironment, m_CreatedEnvironmentID))
            Log.Debug("ExampleDiscoveryV1", "Failed to delete environment");
    }

    #region Configuration
    private void TestPreviewConfiguration()
    {
        Log.Debug("ExampleDiscoveryV1", "Attempting to preview configuration");
        if (!m_Discovery.PreviewConfiguration(OnPreviewConfiguration, m_CreatedEnvironmentID, m_CreatedConfigurationID, null, m_FilePathToIngest, m_Metadata))
            Log.Debug("ExampleDiscoveryV1", "Failed to preview configuration");
    }

    private void TestDeleteConfiguration()
    {
        //  DeleteEnvironment
        Log.Debug("ExampleDiscoveryV1", "Attempting to delete configuration {0}", m_CreatedConfigurationID);
        if (!m_Discovery.DeleteConfiguration(OnDeleteConfiguration, m_CreatedEnvironmentID, m_CreatedConfigurationID))
            Log.Debug("ExampleDiscoveryV1", "Failed to delete configuration");
    }

    private void TestAddConfiguration()
    {
        //  Add Configuration
        Log.Debug("ExampleDiscoveryV1", "Attempting to add configuration");
        if (!m_Discovery.AddConfiguration(OnAddConfiguration, m_CreatedEnvironmentID, m_ConfigurationJsonPath))
            Log.Debug("ExampleDiscoveryV1", "Failed to add configuration");
    }
    #endregion

    #region Collection
    private void TestAddCollection()
    {
        //  Add Collection
        Log.Debug("ExampleDiscoveryV1", "Attempting to add collection");
        if (!m_Discovery.AddCollection(OnAddCollection, m_CreatedEnvironmentID, m_CreatedCollectionName, m_CreatedCollectionDescription, m_CreatedConfigurationID))
            Log.Debug("ExampleDiscovery", "Failed to add collection");
    }

    private void TestGetCollection()
    {
        //  Get Collection
        Log.Debug("ExampleDiscoveryV1", "Attempting to get collection");
        if (!m_Discovery.GetCollection(OnGetCollection, m_CreatedEnvironmentID, m_CreatedCollectionID))
            Log.Debug("ExampleDiscovery", "Failed to get collection");
    }

    private void TestDeleteCollection()
    {
        //  Delete Collection
        Log.Debug("ExampleDiscoveryV1", "Attempting to delete collection {0}", m_CreatedCollectionID);
        if (!m_Discovery.DeleteCollection(OnDeleteCollection, m_CreatedEnvironmentID, m_CreatedCollectionID))
            Log.Debug("ExampleDiscovery", "Failed to add collection");
    }
    #endregion

    #region Documents
    private void TestAddDocument()
    {
        Log.Debug("ExampleDiscoveryV1", "Attempting to add document");
        if (!m_Discovery.AddDocument(OnAddDocument, m_CreatedEnvironmentID, m_CreatedCollectionID, m_DocumentFilePath, m_CreatedConfigurationID, null))
            Log.Debug("ExampleDiscovery", "Failed to add document");
    }

    private void TestGetDocument()
    {
        Log.Debug("ExampleDiscoveryV1", "Attempting to get document");
        if (!m_Discovery.GetDocument(OnGetDocument, m_CreatedEnvironmentID, m_CreatedCollectionID, m_CreatedDocumentID))
            Log.Debug("ExampleDiscovery", "Failed to get document");
    }

    private void TestUpdateDocument()
    {
        Log.Debug("ExampleDiscoveryV1", "Attempting to update document");
        if (!m_Discovery.UpdateDocument(OnUpdateDocument, m_CreatedEnvironmentID, m_CreatedCollectionID, m_CreatedDocumentID, m_DocumentFilePath, m_CreatedConfigurationID, null))
            Log.Debug("ExampleDiscovery", "Failed to update document");
    }

    private void TestDeleteDocument()
    {
        Log.Debug("ExampleDiscoveryV1", "Attempting to delete document {0}", m_CreatedDocumentID);
        if (!m_Discovery.DeleteDocument(OnDeleteDocument, m_CreatedEnvironmentID, m_CreatedCollectionID, m_CreatedDocumentID))
            Log.Debug("ExampleDiscovery", "Failed to delete document");
    }
    #endregion

    private void TestQuery()
    {
        Log.Debug("ExampleDiscoveryV1", "Attempting to query");
        if (!m_Discovery.Query(OnQuery, m_CreatedEnvironmentID, m_CreatedCollectionID, null, m_Query, null, 10, null, 0))
            Log.Debug("ExampleDiscovery", "Failed to query");
    }

    private void OnGetEnvironments(GetEnvironmentsResponse resp, string data)
    {
        if (resp != null)
        {
            foreach (Environment environment in resp.environments)
                Log.Debug("ExampleDiscoveryV1", "environment_id: {0}", environment.environment_id);
        }
        else
        {
            Log.Debug("ExampleDiscoveryV1", "resp is null");
        }
    }

    private void OnGetEnvironment(Environment resp, string data)
    {
        if (resp != null)
        {
            Log.Debug("ExampleDiscoveryV1", "environment_name: {0}", resp.name);
        }
        else
        {
            Log.Debug("ExampleDiscoveryV1", "resp is null");
        }
    }

    private void OnAddEnvironment(Environment resp, string data)
    {
        if (resp != null)
        {
            Log.Debug("ExampleDiscoveryV1", "Added {0}", resp.environment_id, data);
            m_CreatedEnvironmentID = resp.environment_id;

            TestAddConfiguration();
        }
        else
        {
            Log.Debug("ExampleDiscoveryV1", "resp is null, {0}", data);
        }
    }

    private void OnDeleteEnvironment(bool success, string data)
    {
        if (success)
        {
            Log.Debug("ExampleDiscoveryV1", "Delete environment successful");
            m_CreatedEnvironmentID = default(string);
        }
        else
            Log.Debug("ExampleDiscoveryV1", "Delete environment failed");
    }

    private void OnGetConfigurations(GetConfigurationsResponse resp, string data)
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
            Log.Debug("ExampleDiscoveryV1", "resp is null, {0}", data);
        }
    }

    private void OnGetConfiguration(Configuration resp, string data)
    {
        if (resp != null)
        {
            Log.Debug("ExampleDiscoveryV1", "Configuration: {0}, {1}", resp.configuration_id, resp.name);
        }
        else
        {
            Log.Debug("ExampleDiscoveryV1", "resp is null, {0}", data);
        }
    }

    private void OnAddConfiguration(Configuration resp, string data)
    {
        if (resp != null)
        {
            Log.Debug("ExampleDiscoveryV1", "Configuration: {0}, {1}", resp.configuration_id, resp.name);
            m_CreatedConfigurationID = resp.configuration_id;

            TestPreviewConfiguration();
        }
        else
        {
            Log.Debug("ExampleDiscoveryV1", "resp is null, {0}", data);
        }
    }

    private void OnDeleteConfiguration(bool success, string data)
    {
        if (success)
        {
            Log.Debug("ExampleDiscoveryV1", "Delete configuration successful");
            m_CreatedConfigurationID = default(string);
            m_IsConfigDeleted = true;

            if(m_IsConfigDeleted && m_IsCollectionDeleted)
                Invoke("TestDeleteEnvironment", 1f);
        }
        else
            Log.Debug("ExampleDiscoveryV1", "Delete configuration failed");
    }

    private void OnPreviewConfiguration(TestDocument resp, string data)
    {
        if (resp != null)
        {
            Log.Debug("ExampleDiscoveryV1", "Preview succeeded: {0}", resp.status);

            CheckState();
        }
        else
        {
            Log.Debug("ExampleDiscoveryV1", "Failed to preview configuration");
        }
    }

    private void OnGetCollections(GetCollectionsResponse resp, string data)
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
            Log.Debug("ExampleDiscoveryV1", "Failed to get collections");
        }
    }

    private void OnGetCollection(Collection resp, string data)
    {
        if (resp != null)
        {
            Log.Debug("ExampleDiscoveryV1", "Collection: {0}, {1}", resp.collection_id, resp.name);


            //TestAddDocument();
        }
        else
        {
            Log.Debug("ExampleDiscoveryV1", "Failed to get collections");
        }
    }

    private void OnAddCollection(CollectionRef resp, string data)
    {
        if (resp != null)
        {
            Log.Debug("ExampleDiscoveryV1", "Collection: {0}, {1}", resp.collection_id, resp.name);
            m_CreatedCollectionID = resp.collection_id;

            //TestGetCollection();
        }
        else
        {
            Log.Debug("ExampleDiscoveryV1", "resp is null, {0}", data);
        }
    }

    private void OnDeleteCollection(bool success, string data)
    {
        if (success)
        {
            Log.Debug("ExampleDiscoveryV1", "Delete collection successful");
            m_CreatedCollectionID = default(string);
            m_IsCollectionDeleted = true;

            if (m_IsConfigDeleted && m_IsCollectionDeleted)
                Invoke("TestDeleteEnvironment", 1f);
        }
        else
            Log.Debug("ExampleDiscoveryV1", "Delete collection failed");
    }

    private void OnAddDocument(DocumentAccepted resp, string data)
    {
        if(resp != null)
        {
            Log.Debug("ExampleDiscoveryV1", "Added Document {0} {1}", resp.document_id, resp.status);
            m_CreatedDocumentID = resp.document_id;

            TestGetDocument();
        }
        else
        {
            Log.Debug("ExampleDiscoveryV1", "resp is null, {0}", data);
        }
    }

    private void OnGetDocument(DocumentStatus resp, string data)
    {
        if(resp != null)
        {
            Log.Debug("ExampleDiscoveryV1", "Got Document {0} {1}", resp.document_id, resp.status);
            TestUpdateDocument();
        }
        else
        {
            Log.Debug("ExampleDiscoveryV1", "resp is null, {0}", data);
        }
    }

    private void OnUpdateDocument(DocumentAccepted resp, string data)
    {
        if (resp != null)
        {
            Log.Debug("ExampleDiscoveryV1", "Updated Document {0} {1}", resp.document_id, resp.status);
            TestQuery();
        }
        else
        {
            Log.Debug("ExampleDiscoveryV1", "resp is null, {0}", data);
        }
    }

    private void OnDeleteDocument(bool success, string data)
    {
        if (success)
        {
            Log.Debug("ExampleDiscoveryV1", "Delete document successful");
            m_CreatedDocumentID = default(string);
        }
        else
            Log.Debug("ExampleDiscoveryV1", "Delete collection failed");
    }

    private void OnQuery(QueryResponse resp, string data)
    {
        if(resp != null)
        {
            Log.Debug("ExampleDiscoveryV1", "key: {0}, matching results: {1}", resp.aggregations.term.results.key, resp.aggregations.term.results.matching_results);

            foreach(QueryResult result in resp.results)
                Log.Debug("ExampleDiscoveryV1", "Query response: id: {0}, score: {1}", result.id, result.score);
        }
        else
        {
            Log.Debug("ExampleDiscoveryV1", "resp is null, {0}", data);
        }
    }
}
