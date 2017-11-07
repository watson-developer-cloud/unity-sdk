﻿/**
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
using IBM.Watson.DeveloperCloud.Connection;
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
        Log.Debug("ExampleDiscoveryV1.Examples()", "Attempting to get environments");
        if (!_discovery.GetEnvironments(OnGetEnvironments))
            Log.Debug("ExampleDiscoveryV1.GetEnvironments()", "Failed to get environments");
        while (!_getEnvironmentsTested)
            yield return null;

        //  AddEnvironment
        Log.Debug("ExampleDiscovery.Examples()", "Attempting to add environment");
        if (!_discovery.AddEnvironment(OnAddEnvironment, "unity-testing-AddEnvironment-do-not-delete-until-active", "Testing addEnvironment in Unity SDK. Please do not delete this environment until the status is 'active'", 1))
            Log.Debug("ExampleDiscovery.AddEnvironment()", "Failed to add environment");
        while (!_addEnvironmentTested)
            yield return null;

        //  Wait for environment to be ready
        CheckEnvironmentState();
        while (!_isEnvironmentReady)
            yield return null;

        //  GetEnvironment
        Log.Debug("ExampleDiscovery.Examples()", "Attempting to get environment");
        if (!_discovery.GetEnvironment(OnGetEnvironment, _createdEnvironmentID))
            Log.Debug("ExampleDiscovery.GetEnvironment()", "Failed to get environment");
        while (!_getEnvironmentTested)
            yield return null;

        //  Get Configurations
        Log.Debug("ExampleDiscovery.Examples()", "Attempting to get configurations");
        if (!_discovery.GetConfigurations(OnGetConfigurations, _createdEnvironmentID))
            Log.Debug("ExampleDiscovery.GetConfigurations()", "Failed to get configurations");
        while (!_getConfigurationsTested)
            yield return null;

        //  Add Configuration
        Log.Debug("ExampleDiscovery.Examples()", "Attempting to add configuration");
        if (!_discovery.AddConfiguration(OnAddConfiguration, _createdEnvironmentID, _configurationJsonPath))
            Log.Debug("ExampleDiscovery.AddConfiguration()", "Failed to add configuration");
        while (!_addConfigurationTested)
            yield return null;

        //  Get Configuration
        Log.Debug("ExampleDiscovery.Examples()", "Attempting to get configuration");
        if (!_discovery.GetConfiguration(OnGetConfiguration, _createdEnvironmentID, _createdConfigurationID))
            Log.Debug("ExampleDiscovery.GetConfiguration()", "Failed to get configuration");
        while (!_getConfigurationTested)
            yield return null;

        //  Preview Configuration
        Log.Debug("ExampleDiscovery.Examples()", "Attempting to preview configuration");
        if (!_discovery.PreviewConfiguration(OnPreviewConfiguration, _createdEnvironmentID, _createdConfigurationID, null, _filePathToIngest, _metadata))
            Log.Debug("ExampleDiscovery.PreviewConfiguration()", "Failed to preview configuration");
        while (!_previewConfigurationTested)
            yield return null;

        //  Get Collections
        Log.Debug("ExampleDiscovery.Examples()", "Attempting to get collections");
        if (!_discovery.GetCollections(OnGetCollections, _createdEnvironmentID))
            Log.Debug("ExampleDiscovery.GetCollections()", "Failed to get collections");
        while (!_getCollectionsTested)
            yield return null;

        //  Add Collection
        Log.Debug("ExampleDiscovery.Examples()", "Attempting to add collection");
        if (!_discovery.AddCollection(OnAddCollection, _createdEnvironmentID, _createdCollectionName, _createdCollectionDescription, _createdConfigurationID))
            Log.Debug("ExampleDiscovery.AddCollection()", "Failed to add collection");
        while (!_addCollectionTested)
            yield return null;

        //  Get Collection
        Log.Debug("ExampleDiscovery.Examples()", "Attempting to get collection");
        if (!_discovery.GetCollection(OnGetCollection, _createdEnvironmentID, _createdCollectionID))
            Log.Debug("ExampleDiscovery.GetCollection()", "Failed to get collection");
        while (!_getCollectionTested)
            yield return null;
        
        //  Get fields
        if (!_discovery.GetFields(OnGetFields, _createdEnvironmentID, _createdCollectionID))
            Log.Debug("ExampleDiscovery.GetFields()", "Failed to get fields");
        while (!_getFieldsTested)
            yield return null;

        //  Add Document
        Log.Debug("ExampleDiscovery.Examples()", "Attempting to add document");
        if (!_discovery.AddDocument(OnAddDocument, _createdEnvironmentID, _createdCollectionID, _documentFilePath, _createdConfigurationID, null))
            Log.Debug("ExampleDiscovery.AddDocument()", "Failed to add document");
        while (!_addDocumentTested)
            yield return null;

        //  Get Document
        Log.Debug("ExampleDiscovery.Examples()", "Attempting to get document");
        if (!_discovery.GetDocument(OnGetDocument, _createdEnvironmentID, _createdCollectionID, _createdDocumentID))
            Log.Debug("ExampleDiscovery.GetDocument()", "Failed to get document");
        while (!_getDocumentTested)
            yield return null;

        //  Update Document
        Log.Debug("ExampleDiscovery.Examples()", "Attempting to update document");
        if (!_discovery.UpdateDocument(OnUpdateDocument, _createdEnvironmentID, _createdCollectionID, _createdDocumentID, _documentFilePath, _createdConfigurationID, null))
            Log.Debug("ExampleDiscovery.UpdateDocument()", "Failed to update document");
        while (!_updateDocumentTested)
            yield return null;

        //  Query
        Log.Debug("ExampleDiscovery.Examples()", "Attempting to query");
        if (!_discovery.Query(OnQuery, _createdEnvironmentID, _createdCollectionID, null, _query, null, 10, null, 0))
            Log.Debug("ExampleDiscovery.Query()", "Failed to query");
        while (!_queryTested)
            yield return null;

        //  Delete Document
        Log.Debug("ExampleDiscovery.Examples()", "Attempting to delete document {0}", _createdDocumentID);
        if (!_discovery.DeleteDocument(OnDeleteDocument, _createdEnvironmentID, _createdCollectionID, _createdDocumentID))
            Log.Debug("ExampleDiscovery.DeleteDocument()", "Failed to delete document");
        while (!_deleteDocumentTested)
            yield return null;

        //  Delay
        Log.Debug("ExampleDiscovery.Examples()", "Delaying delete collection for 10 sec");
        Invoke("Delay", 10f);
        while (!_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Delete Collection
        Log.Debug("ExampleDiscovery.Examples()", "Attempting to delete collection {0}", _createdCollectionID);
        if (!_discovery.DeleteCollection(OnDeleteCollection, _createdEnvironmentID, _createdCollectionID))
            Log.Debug("ExampleDiscovery.DeleteCollection()", "Failed to add collection");
        while (!_deleteCollectionTested)
            yield return null;

        //  Delay
        Log.Debug("ExampleDiscovery.Examples()", "Delaying delete configuration for 10 sec");
        Invoke("Delay", 10f);
        while (!_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Delete Configuration
        Log.Debug("ExampleDiscovery.Examples()", "Attempting to delete configuration {0}", _createdConfigurationID);
        if (!_discovery.DeleteConfiguration(OnDeleteConfiguration, _createdEnvironmentID, _createdConfigurationID))
            Log.Debug("ExampleDiscovery.DeleteConfiguration()", "Failed to delete configuration");
        while (!_deleteConfigurationTested)
            yield return null;

        //  Delay
        Log.Debug("ExampleDiscovery.Examples()", "Delaying delete environment for 10 sec");
        Invoke("Delay", 10f);
        while (!_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Delete Environment
        Log.Debug("ExampleDiscovery.Examples()", "Attempting to delete environment {0}", _createdEnvironmentID);
        if (!_discovery.DeleteEnvironment(OnDeleteEnvironment, _createdEnvironmentID))
            Log.Debug("ExampleDiscovery.DeleteEnvironment()", "Failed to delete environment");
        while (!_deleteEnvironmentTested)
            yield return null;

        Log.Debug("ExampleDiscovery.Examples()", "Discovery examples complete.");
    }

    #region Check State
    private void CheckEnvironmentState()
    {
        Log.Debug("ExampleDiscovery.Examples()", "Attempting to get environment state");
        try
        {
            _discovery.GetEnvironment(HandleCheckEnvironmentState, _createdEnvironmentID);
        }
        catch (System.Exception e)
        {
            Log.Debug("ExampleDiscovery.Examples()", string.Format("Failed to get environment state: {0}", e.Message));
            CheckEnvironmentState();
        }
    }

    private void HandleCheckEnvironmentState(RESTConnector.ParsedResponse<Environment> resp)
    {
        Log.Debug("ExampleDiscovery.Examples()", "Environment {0} is {1}", resp.DataObject.environment_id, resp.DataObject.status);

        if (resp.DataObject.status.ToLower() == "active")
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

    private void OnGetEnvironments(RESTConnector.ParsedResponse<GetEnvironmentsResponse> resp)
    {
        Log.Debug("ExampleDiscovery.OnGetEnvironments()", "Discovery - GetEnvironments Response: {0}", resp.JSON);
        _getEnvironmentsTested = true;
    }

    private void OnGetEnvironment(RESTConnector.ParsedResponse<Environment> resp)
    {
        Log.Debug("ExampleDiscovery.OnGetEnvironment()", "Discovery - GetEnvironment Response: {0}", resp.JSON);
        _getEnvironmentTested = true;
    }

    private void OnAddEnvironment(RESTConnector.ParsedResponse<Environment> resp)
    {
        Log.Debug("ExampleDiscovery.OnAddEnvironment()", "Discovery - AddEnvironment Response: {0}", resp.JSON);
        _createdEnvironmentID = resp.DataObject.environment_id;
        _addEnvironmentTested = true;
    }

    private void OnGetConfigurations(RESTConnector.ParsedResponse<GetConfigurationsResponse> resp)
    {
        Log.Debug("ExampleDiscovery.OnGetConfigurations()", "Discovery - GetConfigurations Response: {0}", resp.JSON);
        _getConfigurationsTested = true;
    }

    private void OnGetConfiguration(RESTConnector.ParsedResponse<Configuration> resp)
    {
        Log.Debug("ExampleDiscovery.OnGetConfiguration()", "Discovery - GetConfiguration Response: {0}", resp.JSON);
        _getConfigurationTested = true;

    }

    private void OnAddConfiguration(RESTConnector.ParsedResponse<Configuration> resp)
    {
        Log.Debug("ExampleDiscovery.OnAddConfiguration()", "Discovery - AddConfiguration Response: {0}", resp.JSON);
        _createdConfigurationID = resp.DataObject.configuration_id;
        _addConfigurationTested = true;
    }

    private void OnPreviewConfiguration(RESTConnector.ParsedResponse<TestDocument> resp)
    {
        _previewConfigurationTested = true;
        Log.Debug("ExampleDiscovery.OnPreviewConfiguration()", "Discovery - Preview configuration Response: {0}", resp.JSON);
    }

    private void OnGetCollections(RESTConnector.ParsedResponse<GetCollectionsResponse> resp)
    {
        Log.Debug("ExampleDiscovery.OnGetCollections()", "Discovery - Get collections Response: {0}", resp.JSON);
        _getCollectionsTested = true;
    }

    private void OnGetCollection(RESTConnector.ParsedResponse<Collection> resp)
    {
        Log.Debug("ExampleDiscovery.OnGetCollection()", "Discovery - Get colletion Response: {0}", resp.JSON);
        _getCollectionTested = true;
    }

    private void OnAddCollection(RESTConnector.ParsedResponse<CollectionRef> resp)
    {
        Log.Debug("ExampleDiscovery.OnAddCollection()", "Discovery - Add collection Response: {0}", resp.JSON);
        _createdCollectionID = resp.DataObject.collection_id;
        _addCollectionTested = true;
    }

    private void OnGetFields(RESTConnector.ParsedResponse<GetFieldsResponse> resp)
    {
        Log.Debug("ExampleDiscovery.OnGetFields()", "Discovery - Get fields Response: {0}", resp.JSON);
        _getFieldsTested = true;
    }

    private void OnAddDocument(RESTConnector.ParsedResponse<DocumentAccepted> resp)
    {
        Log.Debug("ExampleDiscovery.OnAddDocument()", "Discovery - Add document Response: {0}", resp.JSON);
        _createdDocumentID = resp.DataObject.document_id;
        _addDocumentTested = true;
    }

    private void OnGetDocument(RESTConnector.ParsedResponse<DocumentStatus> resp)
    {
        Log.Debug("ExampleDiscovery.OnGetDocument()", "Discovery - Get document Response: {0}", resp.JSON);
        _getDocumentTested = true;
    }

    private void OnUpdateDocument(RESTConnector.ParsedResponse<DocumentAccepted> resp)
    {
        Log.Debug("ExampleDiscovery.OnUpdateDocument()", "Discovery - Update document Response: {0}", resp.JSON);
        _updateDocumentTested = true;
    }

    private void OnDeleteDocument(RESTConnector.ParsedResponse<object> resp)
    {
        Log.Debug("ExampleDiscovery.OnDeleteDocument()", "Discovery - Delete document Response: deleted:{0}", resp.Success);

        if (resp.Success)
            _createdDocumentID = default(string);

        _deleteDocumentTested = true;
    }

    private void OnDeleteCollection(RESTConnector.ParsedResponse<object> resp)
    {
        Log.Debug("ExampleDiscovery.OnDeleteCollection()", "Discovery - Delete collection Response: deleted:{0}", resp.Success);

        if (resp.Success)
            _createdCollectionID = default(string);

        _deleteCollectionTested = true;
    }

    private void OnDeleteConfiguration(RESTConnector.ParsedResponse<object> resp)
    {
        Log.Debug("ExampleDiscovery.OnDeleteConfiguration()", "Discovery - DeleteConfiguration Response: deleted:{0}", resp.Success);

        if (resp.Success)
            _createdConfigurationID = default(string);

        _deleteConfigurationTested = true;
    }

    private void OnDeleteEnvironment(RESTConnector.ParsedResponse<object> resp)
    {
        Log.Debug("ExampleDiscovery.OnDeleteEnvironment()", "Discovery - DeleteEnvironment Response: deleted:{0}", resp.Success);

        if (resp.Success)
            _createdEnvironmentID = default(string);

        _deleteEnvironmentTested = true;
    }

    private void OnQuery(RESTConnector.ParsedResponse<QueryResponse> resp)
    {
        Log.Debug("ExampleDiscovery.OnQuery()", "Discovery - Query Response: {0}", resp.JSON);
        _queryTested = true;
    }
}
