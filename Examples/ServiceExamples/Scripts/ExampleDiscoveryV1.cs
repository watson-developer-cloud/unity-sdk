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
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;
using UnityEngine;

public class ExampleDiscoveryV1 : MonoBehaviour
{
    private string _username = null;
    private string _password = null;
    private string _url = null;
    
    private Discovery _discovery;
    private string _discoveryVersionDate = "2016-12-01";

    private string _createdEnvironmentID;
    private string _configurationJsonPath;
    private string _createdConfigurationID;
    private string _filePathToIngest;
    private string _metadata = "{\n\t\"Creator\": \"Unity SDK Integration Test\",\n\t\"Subject\": \"Discovery service\"\n}";
    private string _createdCollectionID;
    private string _createdCollectionName = "Unity SDK Created Collection";
    private string _createdCollectionDescription = "A collection created by the Unity SDK. Please delete me.";
    private string _createdDocumentID;
    private string _documentFilePath;
    private string _query = "What is the capital of china?";

    private bool _getEnvironmentsTested = false;
    private bool _getEnvironmentTested = false;
    private bool _addEnvironmentTested = false;
    private bool _getConfigurationsTested = false;
    private bool _getConfigurationTested = false;
    private bool _addConfigurationTested = false;
    private bool _previewConfigurationTested = false;
    private bool _getCollectionsTested = false;
    private bool _addCollectionTested = false;
    private bool _getCollectionTested = false;
    private bool _getFieldsTested = false;
    private bool _addDocumentTested = false;
    private bool _getDocumentTested = false;
    private bool _updateDocumentTested = false;
    private bool _queryTested = false;
    private bool _deleteDocumentTested = false;
    private bool _deleteCollectionTested = false;
    private bool _deleteConfigurationTested = false;
    private bool _deleteEnvironmentTested = false;
    private bool _isEnvironmentReady = false;
    private bool _readyToContinue = false;

    private void Start()
    {
        LogSystem.InstallDefaultReactors();

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_username, _password, _url);

        _discovery = new Discovery(credentials);
        _discovery.VersionDate = _discoveryVersionDate;
        _configurationJsonPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/Discovery/exampleConfigurationData.json";
        _filePathToIngest = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/watson_beats_jeopardy.html";
        _documentFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/watson_beats_jeopardy.html";

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        //  Get Environments
        Log.Debug("ExampleDiscoveryV1", "Attempting to get environments");
        if (!_discovery.GetEnvironments(OnGetEnvironments))
            Log.Debug("ExampleDiscoveryV1", "Failed to get environments");
        while (!_getEnvironmentsTested)
            yield return null;

        //  AddEnvironment
        Log.Debug("ExampleDiscoveryV1", "Attempting to add environment");
        if (!_discovery.AddEnvironment(OnAddEnvironment, "unity-testing-AddEnvironment-do-not-delete-until-active", "Testing addEnvironment in Unity SDK. Please do not delete this environment until the status is 'active'", 1))
            Log.Debug("ExampleDiscoveryV1", "Failed to add environment");
        while (!_addEnvironmentTested)
            yield return null;

        //  Wait for environment to be ready
        CheckEnvironmentState();
        while (!_isEnvironmentReady)
            yield return null;

        //  GetEnvironment
        Log.Debug("ExampleDiscoveryV1", "Attempting to get environment");
        if (!_discovery.GetEnvironment(OnGetEnvironment, _createdEnvironmentID))
            Log.Debug("ExampleDiscoveryV1", "Failed to get environment");
        while (!_getEnvironmentTested)
            yield return null;

        //  Get Configurations
        Log.Debug("ExampleDiscoveryV1", "Attempting to get configurations");
        if (!_discovery.GetConfigurations(OnGetConfigurations, _createdEnvironmentID))
            Log.Debug("ExampleDiscoveryV1", "Failed to get configurations");
        while (!_getConfigurationsTested)
            yield return null;

        //  Add Configuration
        Log.Debug("ExampleDiscoveryV1", "Attempting to add configuration");
        if (!_discovery.AddConfiguration(OnAddConfiguration, _createdEnvironmentID, _configurationJsonPath))
            Log.Debug("ExampleDiscoveryV1", "Failed to add configuration");
        while (!_addConfigurationTested)
            yield return null;

        //  Get Configuration
        Log.Debug("ExampleDiscoveryV1", "Attempting to get configuration");
        if (!_discovery.GetConfiguration(OnGetConfiguration, _createdEnvironmentID, _createdConfigurationID))
            Log.Debug("ExampleDiscoveryV1", "Failed to get configuration");
        while (!_getConfigurationTested)
            yield return null;

        //  Preview Configuration
        Log.Debug("ExampleDiscoveryV1", "Attempting to preview configuration");
        if (!_discovery.PreviewConfiguration(OnPreviewConfiguration, _createdEnvironmentID, _createdConfigurationID, null, _filePathToIngest, _metadata))
            Log.Debug("ExampleDiscoveryV1", "Failed to preview configuration");
        while (!_previewConfigurationTested)
            yield return null;

        //  Get Collections
        Log.Debug("ExampleDiscoveryV1", "Attempting to get collections");
        if (!_discovery.GetCollections(OnGetCollections, _createdEnvironmentID))
            Log.Debug("ExampleDiscovery", "Failed to get collections");
        while (!_getCollectionsTested)
            yield return null;

        //  Add Collection
        Log.Debug("ExampleDiscoveryV1", "Attempting to add collection");
        if (!_discovery.AddCollection(OnAddCollection, _createdEnvironmentID, _createdCollectionName, _createdCollectionDescription, _createdConfigurationID))
            Log.Debug("ExampleDiscovery", "Failed to add collection");
        while (!_addCollectionTested)
            yield return null;

        //  Get Collection
        Log.Debug("ExampleDiscoveryV1", "Attempting to get collection");
        if (!_discovery.GetCollection(OnGetCollection, _createdEnvironmentID, _createdCollectionID))
            Log.Debug("ExampleDiscovery", "Failed to get collection");
        while (!_getCollectionTested)
            yield return null;
        
        //  Get fields
        if (!_discovery.GetFields(OnGetFields, _createdEnvironmentID, _createdCollectionID))
            Log.Debug("ExampleDiscovery", "Failed to get fields");
        while (!_getFieldsTested)
            yield return null;

        //  Add Document
        Log.Debug("ExampleDiscoveryV1", "Attempting to add document");
        if (!_discovery.AddDocument(OnAddDocument, _createdEnvironmentID, _createdCollectionID, _documentFilePath, _createdConfigurationID, null))
            Log.Debug("ExampleDiscovery", "Failed to add document");
        while (!_addDocumentTested)
            yield return null;

        //  Get Document
        Log.Debug("ExampleDiscoveryV1", "Attempting to get document");
        if (!_discovery.GetDocument(OnGetDocument, _createdEnvironmentID, _createdCollectionID, _createdDocumentID))
            Log.Debug("ExampleDiscovery", "Failed to get document");
        while (!_getDocumentTested)
            yield return null;

        //  Update Document
        Log.Debug("ExampleDiscoveryV1", "Attempting to update document");
        if (!_discovery.UpdateDocument(OnUpdateDocument, _createdEnvironmentID, _createdCollectionID, _createdDocumentID, _documentFilePath, _createdConfigurationID, null))
            Log.Debug("ExampleDiscovery", "Failed to update document");
        while (!_updateDocumentTested)
            yield return null;

        //  Query
        Log.Debug("ExampleDiscoveryV1", "Attempting to query");
        if (!_discovery.Query(OnQuery, _createdEnvironmentID, _createdCollectionID, null, _query, null, 10, null, 0))
            Log.Debug("ExampleDiscovery", "Failed to query");
        while (!_queryTested)
            yield return null;

        //  Delete Document
        Log.Debug("ExampleDiscoveryV1", "Attempting to delete document {0}", _createdDocumentID);
        if (!_discovery.DeleteDocument(OnDeleteDocument, _createdEnvironmentID, _createdCollectionID, _createdDocumentID))
            Log.Debug("ExampleDiscovery", "Failed to delete document");
        while (!_deleteDocumentTested)
            yield return null;

        //  Delay
        Log.Debug("ExampleDiscovery", "Delaying delete collection for 10 sec");
        Invoke("Delay", 10f);
        while (!_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Delete Collection
        Log.Debug("ExampleDiscoveryV1", "Attempting to delete collection {0}", _createdCollectionID);
        if (!_discovery.DeleteCollection(OnDeleteCollection, _createdEnvironmentID, _createdCollectionID))
            Log.Debug("ExampleDiscovery", "Failed to add collection");
        while (!_deleteCollectionTested)
            yield return null;

        //  Delay
        Log.Debug("ExampleDiscovery", "Delaying delete configuration for 10 sec");
        Invoke("Delay", 10f);
        while (!_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Delete Configuration
        Log.Debug("ExampleDiscoveryV1", "Attempting to delete configuration {0}", _createdConfigurationID);
        if (!_discovery.DeleteConfiguration(OnDeleteConfiguration, _createdEnvironmentID, _createdConfigurationID))
            Log.Debug("ExampleDiscoveryV1", "Failed to delete configuration");
        while (!_deleteConfigurationTested)
            yield return null;

        //  Delay
        Log.Debug("ExampleDiscovery", "Delaying delete environment for 10 sec");
        Invoke("Delay", 10f);
        while (!_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Delete Environment
        Log.Debug("ExampleDiscoveryV1", "Attempting to delete environment {0}", _createdEnvironmentID);
        if (!_discovery.DeleteEnvironment(OnDeleteEnvironment, _createdEnvironmentID))
            Log.Debug("ExampleDiscoveryV1", "Failed to delete environment");
        while (!_deleteEnvironmentTested)
            yield return null;

        Log.Debug("ExampleDiscoveryV1", "Discovery examples complete.");
    }

    #region Check State
    private void CheckEnvironmentState()
    {
        Log.Debug("ExampleDiscoveryV1", "Attempting to get environment state");
        try
        {
            _discovery.GetEnvironment(HandleCheckEnvironmentState, _createdEnvironmentID);
        }
        catch (System.Exception e)
        {
            Log.Debug("ExampleDiscoveryV1", string.Format("Failed to get environment state: {0}", e.Message));
            CheckEnvironmentState();
        }
    }

    private void HandleCheckEnvironmentState(Environment resp, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Environment {0} is {1}", resp.environment_id, resp.status);

        if (resp.status.ToLower() == "active")
            _isEnvironmentReady = true;
        else
        {
            Invoke("CheckEnvironmentState", 10f);
        }
    }

    private void Delay()
    {
        _readyToContinue = true;
    }
    #endregion

    private void OnGetEnvironments(GetEnvironmentsResponse resp, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Discovery - GetEnvironments Response: {0}", data);
        _getEnvironmentsTested = true;
    }

    private void OnGetEnvironment(Environment resp, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Discovery - GetEnvironment Response: {0}", data);
        _getEnvironmentTested = true;
    }

    private void OnAddEnvironment(Environment resp, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Discovery - AddEnvironment Response: {0}", data);
        _createdEnvironmentID = resp.environment_id;
        _addEnvironmentTested = true;
    }

    private void OnGetConfigurations(GetConfigurationsResponse resp, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Discovery - GetConfigurations Response: {0}", data);
        _getConfigurationsTested = true;
    }

    private void OnGetConfiguration(Configuration resp, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Discovery - GetConfiguration Response: {0}", data);
        _getConfigurationTested = true;

    }

    private void OnAddConfiguration(Configuration resp, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Discovery - AddConfiguration Response: {0}", data);
        _createdConfigurationID = resp.configuration_id;
        _addConfigurationTested = true;
    }

    private void OnPreviewConfiguration(TestDocument resp, string data)
    {
        _previewConfigurationTested = true;
        Log.Debug("ExampleDiscoveryV1", "Discovery - Preview configuration Response: {0}", data);
    }

    private void OnGetCollections(GetCollectionsResponse resp, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Discovery - Get collections Response: {0}", data);
        _getCollectionsTested = true;
    }

    private void OnGetCollection(Collection resp, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Discovery - Get colletion Response: {0}", data);
        _getCollectionTested = true;
    }

    private void OnAddCollection(CollectionRef resp, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Discovery - Add collection Response: {0}", data);
         _createdCollectionID = resp.collection_id;
        _addCollectionTested = true;
    }

    private void OnGetFields(GetFieldsResponse resp, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Discovery - Get fields Response: {0}", data);
        _getFieldsTested = true;
    }

    private void OnAddDocument(DocumentAccepted resp, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Discovery - Add document Response: {0}", data);
        _createdDocumentID = resp.document_id;
        _addDocumentTested = true;
    }

    private void OnGetDocument(DocumentStatus resp, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Discovery - Get document Response: {0}", data);
        _getDocumentTested = true;
    }

    private void OnUpdateDocument(DocumentAccepted resp, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Discovery - Update document Response: {0}", data);
        _updateDocumentTested = true;
    }

    private void OnDeleteDocument(bool success, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Discovery - Delete document Response: deleted:{0}", success);

        if (success)
            _createdDocumentID = default(string);

        _deleteDocumentTested = true;
    }

    private void OnDeleteCollection(bool success, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Discovery - Delete collection Response: deleted:{0}", success);

        if (success)
            _createdCollectionID = default(string);

        _deleteCollectionTested = true;
    }

    private void OnDeleteConfiguration(bool success, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Discovery - DeleteConfiguration Response: deleted:{0}", success);

        if (success)
            _createdConfigurationID = default(string);

        _deleteConfigurationTested = true;
    }

    private void OnDeleteEnvironment(bool success, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Discovery - DeleteEnvironment Response: deleted:{0}", success);

        if (success)
            _createdEnvironmentID = default(string);

        _deleteEnvironmentTested = true;
    }

    private void OnQuery(QueryResponse resp, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Discovery - Query Response: {0}", data);
        _queryTested = true;
    }
}
