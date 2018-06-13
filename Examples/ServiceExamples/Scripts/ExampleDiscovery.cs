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
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.Discovery.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExampleDiscovery : MonoBehaviour
{
    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [Space(10)]
    [Tooltip("The service URL (optional). This defaults to \"https://gateway.watsonplatform.net/discovery/api\"")]
    [SerializeField]
    private string _serviceUrl;
    [Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD.")]
    [SerializeField]
    private string _versionDate;
    [Header("CF Authentication")]
    [Tooltip("The authentication username.")]
    [SerializeField]
    private string _username;
    [Tooltip("The authentication password.")]
    [SerializeField]
    private string _password;
    [Header("IAM Authentication")]
    [Tooltip("The IAM apikey.")]
    [SerializeField]
    private string _iamApikey;
    [Tooltip("The IAM url used to authenticate the apikey (optional). This defaults to \"https://iam.bluemix.net/identity/token\".")]
    [SerializeField]
    private string _iamUrl;
    #endregion

    private Discovery _service;

    private string _createdEnvironmentID;
    private string _configurationJson = "{\"name\":\"IBM News {guid}\",\"description\":\"A configuration useful for ingesting IBM press releases. Safe to delete.\",\"conversions\":{\"html\":{\"exclude_tags_keep_content\":[\"span\"],\"exclude_content\":{\"xpaths\":[\"/home\"]}},\"segment\":{\"enabled\":true,\"selector_tags\":[\"h1\",\"h2\"]},\"json_normalizations\":[{\"operation\":\"move\",\"source_field\":\"extracted_metadata.title\",\"destination_field\":\"metadata.title\"},{\"operation\":\"move\",\"source_field\":\"extracted_metadata.author\",\"destination_field\":\"metadata.author\"},{\"operation\":\"remove\",\"source_field\":\"extracted_metadata\"}]},\"enrichments\":[{\"enrichment\":\"natural_language_understanding\",\"source_field\":\"title\",\"destination_field\":\"enriched_title\",\"options\":{\"features\":{\"keywords\":{\"sentiment\":true,\"emotion\":false,\"limit\":50},\"entities\":{\"sentiment\":true,\"emotion\":false,\"limit\":50,\"mentions\":true,\"mention_types\":true,\"sentence_locations\":true,\"model\":\"WKS-model-id\"},\"sentiment\":{\"document\":true,\"targets\":[\"IBM\",\"Watson\"]},\"emotion\":{\"document\":true,\"targets\":[\"IBM\",\"Watson\"]},\"categories\":{},\"concepts\":{\"limit\":8},\"semantic_roles\":{\"entities\":true,\"keywords\":true,\"limit\":50},\"relations\":{\"model\":\"WKS-model-id\"}}}},{\"enrichment\":\"elements\",\"source_field\":\"html\",\"destination_field\":\"enriched_html\",\"options\":{\"model\":\"contract\"}}],\"normalizations\":[{\"operation\":\"move\",\"source_field\":\"metadata.title\",\"destination_field\":\"title\"},{\"operation\":\"move\",\"source_field\":\"metadata.author\",\"destination_field\":\"author\"},{\"operation\":\"move\",\"source_field\":\"alchemy_enriched_text.language\",\"destination_field\":\"language\"},{\"operation\":\"remove\",\"source_field\":\"html\"},{\"operation\":\"remove\",\"source_field\":\"alchemy_enriched_text.status\"},{\"operation\":\"remove\",\"source_field\":\"alchemy_enriched_text.text\"},{\"operation\":\"remove\",\"source_field\":\"sire_enriched_text.language\"},{\"operation\":\"remove\",\"source_field\":\"sire_enriched_text.model\"},{\"operation\":\"remove\",\"source_field\":\"sire_enriched_text.status\"},{\"operation\":\"remove_nulls\"}]}";
    private string _environmentId;
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
    private bool _isEnvironmentReady = false;
    private bool _readyToContinue = false;
    private float _waitTime = 10f;

    private void Start()
    {
        LogSystem.InstallDefaultReactors();
        _filePathToIngest = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/watson_beats_jeopardy.html";
        _documentFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/watson_beats_jeopardy.html";
        Runnable.Run(CreateService());
    }

    private IEnumerator CreateService()
    {
        //  Create credential and instantiate service
        Credentials credentials = null;
        if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
        {
            //  Authenticate using username and password
            credentials = new Credentials(_username, _password, _serviceUrl);
        }
        else if (!string.IsNullOrEmpty(_iamApikey))
        {
            //  Authenticate using iamApikey
            TokenOptions tokenOptions = new TokenOptions()
            {
                IamApiKey = _iamApikey,
                IamUrl = _iamUrl
            };

            credentials = new Credentials(tokenOptions, _serviceUrl);

            //  Wait for tokendata
            while (!credentials.HasIamTokenData())
                yield return null;
        }
        else
        {
            throw new WatsonException("Please provide either username and password or IAM apikey to authenticate the service.");
        }

        _service = new Discovery(credentials);
        _service.VersionDate = _versionDate;

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        //  Get Environments
        Log.Debug("TestDiscovery.RunTest()", "Attempting to get environments");
        if (!_service.GetEnvironments(OnGetEnvironments, OnFail))
            Log.Debug("TestDiscovery.GetEnvironments()", "Failed to get environments");
        while (!_getEnvironmentsTested)
            yield return null;

        //  Wait for environment to be ready
        Runnable.Run(CheckEnvironmentState(0f));
        while (!_isEnvironmentReady)
            yield return null;

        //  GetEnvironment
        Log.Debug("TestDiscovery.RunTest()", "Attempting to get environment");
        if (!_service.GetEnvironment(OnGetEnvironment, OnFail, _environmentId))
            Log.Debug("TestDiscovery.GetEnvironment()", "Failed to get environment");
        while (!_getEnvironmentTested)
            yield return null;

        //  Get Configurations
        Log.Debug("TestDiscovery.RunTest()", "Attempting to get configurations");
        if (!_service.GetConfigurations(OnGetConfigurations, OnFail, _environmentId))
            Log.Debug("TestDiscovery.GetConfigurations()", "Failed to get configurations");
        while (!_getConfigurationsTested)
            yield return null;

        //  Add Configuration
        Log.Debug("TestDiscovery.RunTest()", "Attempting to add configuration");
        if (!_service.AddConfiguration(OnAddConfiguration, OnFail, _environmentId, _configurationJson.Replace("{guid}", GUID.Generate().ToString())))
            Log.Debug("TestDiscovery.AddConfiguration()", "Failed to add configuration");
        while (!_addConfigurationTested)
            yield return null;

        //  Get Configuration
        Log.Debug("TestDiscovery.RunTest()", "Attempting to get configuration");
        if (!_service.GetConfiguration(OnGetConfiguration, OnFail, _environmentId, _environmentId))
            Log.Debug("TestDiscovery.GetConfiguration()", "Failed to get configuration");
        while (!_getConfigurationTested)
            yield return null;

        //  Preview Configuration
        Log.Debug("TestDiscovery.RunTest()", "Attempting to preview configuration");
        if (!_service.PreviewConfiguration(OnPreviewConfiguration, OnFail, _environmentId, _environmentId, null, _filePathToIngest, _metadata))
            Log.Debug("TestDiscovery.PreviewConfiguration()", "Failed to preview configuration");
        while (!_previewConfigurationTested)
            yield return null;

        //  Get Collections
        Log.Debug("TestDiscovery.RunTest()", "Attempting to get collections");
        if (!_service.GetCollections(OnGetCollections, OnFail, _environmentId))
            Log.Debug("TestDiscovery.GetCollections()", "Failed to get collections");
        while (!_getCollectionsTested)
            yield return null;

        //  Add Collection
        Log.Debug("TestDiscovery.RunTest()", "Attempting to add collection");
        if (!_service.AddCollection(OnAddCollection, OnFail, _environmentId, _createdCollectionName + GUID.Generate().ToString(), _createdCollectionDescription, _environmentId))
            Log.Debug("TestDiscovery.AddCollection()", "Failed to add collection");
        while (!_addCollectionTested)
            yield return null;

        //  Get Collection
        Log.Debug("TestDiscovery.RunTest()", "Attempting to get collection");
        if (!_service.GetCollection(OnGetCollection, OnFail, _environmentId, _createdCollectionID))
            Log.Debug("TestDiscovery.GetCollection()", "Failed to get collection");
        while (!_getCollectionTested)
            yield return null;

        if (!_service.GetFields(OnGetFields, OnFail, _environmentId, _createdCollectionID))
            Log.Debug("TestDiscovery.GetFields()", "Failed to get fields");
        while (!_getFieldsTested)
            yield return null;

        //  Add Document
        Log.Debug("TestDiscovery.RunTest()", "Attempting to add document");
        if (!_service.AddDocument(OnAddDocument, OnFail, _environmentId, _createdCollectionID, _documentFilePath, _environmentId, null))
            Log.Debug("TestDiscovery.AddDocument()", "Failed to add document");
        while (!_addDocumentTested)
            yield return null;

        //  Get Document
        Log.Debug("TestDiscovery.RunTest()", "Attempting to get document");
        if (!_service.GetDocument(OnGetDocument, OnFail, _environmentId, _createdCollectionID, _createdDocumentID))
            Log.Debug("TestDiscovery.GetDocument()", "Failed to get document");
        while (!_getDocumentTested)
            yield return null;

        //  Update Document
        Log.Debug("TestDiscovery.RunTest()", "Attempting to update document");
        if (!_service.UpdateDocument(OnUpdateDocument, OnFail, _environmentId, _createdCollectionID, _createdDocumentID, _documentFilePath, _environmentId, null))
            Log.Debug("TestDiscovery.UpdateDocument()", "Failed to update document");
        while (!_updateDocumentTested)
            yield return null;

        //  Query
        Log.Debug("TestDiscovery.RunTest()", "Attempting to query");
        if (!_service.Query(OnQuery, OnFail, _environmentId, _createdCollectionID, null, _query, null, 10, null, 0))
            Log.Debug("TestDiscovery.Query()", "Failed to query");
        while (!_queryTested)
            yield return null;

        //  Delete Document
        Log.Debug("TestDiscovery.RunTest()", "Attempting to delete document {0}", _createdDocumentID);
        if (!_service.DeleteDocument(OnDeleteDocument, OnFail, _environmentId, _createdCollectionID, _createdDocumentID))
            Log.Debug("TestDiscovery.DeleteDocument()", "Failed to delete document");
        while (!_deleteDocumentTested)
            yield return null;

        //  Delay
        Log.Debug("TestDiscovery.RunTest()", "Delaying delete collection for 10 sec");
        Runnable.Run(Delay(_waitTime));
        while (!_readyToContinue)
            yield return null;

        _isEnvironmentReady = false;
        Runnable.Run(CheckEnvironmentState(_waitTime));
        while (!_isEnvironmentReady)
            yield return null;

        _readyToContinue = false;
        //  Delete Collection
        Log.Debug("TestDiscovery.RunTest()", "Attempting to delete collection {0}", _createdCollectionID);
        if (!_service.DeleteCollection(OnDeleteCollection, OnFail, _environmentId, _createdCollectionID))
            Log.Debug("TestDiscovery.DeleteCollection()", "Failed to delete collection");
        while (!_deleteCollectionTested)
            yield return null;

        //  Delay
        Log.Debug("TestDiscovery.RunTest()", "Delaying delete configuration for 10 sec");
        Runnable.Run(Delay(_waitTime));
        while (!_readyToContinue)
            yield return null;

        _isEnvironmentReady = false;
        Runnable.Run(CheckEnvironmentState(_waitTime));
        while (!_isEnvironmentReady)
            yield return null;

        _readyToContinue = false;
        //  Delete Configuration
        Log.Debug("TestDiscovery.RunTest()", "Attempting to delete configuration {0}", _environmentId);
        if (!_service.DeleteConfiguration(OnDeleteConfiguration, OnFail, _environmentId, _environmentId))
            Log.Debug("TestDiscovery.DeleteConfiguration()", "Failed to delete configuration");
        while (!_deleteConfigurationTested)
            yield return null;

        //  Delay
        Log.Debug("TestDiscovery.RunTest()", "Delaying delete environment for 10 sec");
        Runnable.Run(Delay(_waitTime));
        while (!_readyToContinue)
            yield return null;

        _isEnvironmentReady = false;
        Runnable.Run(CheckEnvironmentState(_waitTime));
        while (!_isEnvironmentReady)
            yield return null;

        Log.Debug("TestDiscovery.RunTest()", "Discovery examples complete.");
    }

    #region Check State
    private IEnumerator CheckEnvironmentState(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Log.Debug("TestDiscovery.CheckEnvironmentState()", "Attempting to get environment state");
        try
        {
            _service.GetEnvironment(HandleCheckEnvironmentState, OnFail, _environmentId);
        }
        catch (System.Exception e)
        {
            Log.Debug("TestDiscovery.CheckEnvironmentState()", string.Format("Failed to get environment state: {0}", e.Message));
            Runnable.Run(CheckEnvironmentState(10f));
        }
    }

    private void HandleCheckEnvironmentState(Environment resp, Dictionary<string, object> customData)
    {
        Log.Debug("TestDiscovery.HandleCheckEnvironmentState()", "Environment {0} is {1}", resp.environment_id, resp.status);

        if (resp.status.ToLower() == "active")
            _isEnvironmentReady = true;
        else
        {
            Runnable.Run(CheckEnvironmentState(10f));
        }
    }

    private IEnumerator Delay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _readyToContinue = true;
    }
    #endregion

    private void OnGetEnvironments(GetEnvironmentsResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnGetEnvironments()", "Discovery - GetEnvironments Response: {0}", customData["json"].ToString());
        _getEnvironmentsTested = true;
    }

    private void OnGetEnvironment(Environment resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnGetEnvironment()", "Discovery - GetEnvironment Response: {0}", customData["json"].ToString());
        _environmentId = resp.environment_id;
        _getEnvironmentTested = true;
    }

    private void OnGetConfigurations(GetConfigurationsResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnGetConfigurations()", "Discovery - GetConfigurations Response: {0}", customData["json"].ToString());
        _getConfigurationsTested = true;
    }

    private void OnGetConfiguration(Configuration resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnGetConfiguration()", "Discovery - GetConfiguration Response: {0}", customData["json"].ToString());
        _getConfigurationTested = true;

    }

    private void OnAddConfiguration(Configuration resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnAddConfiguration()", "Discovery - AddConfiguration Response: {0}", customData["json"].ToString());
        _environmentId = resp.configuration_id;
        _addConfigurationTested = true;
    }

    private void OnPreviewConfiguration(TestDocument resp, Dictionary<string, object> customData)
    {
        _previewConfigurationTested = true;
        Log.Debug("ExampleDiscovery.OnPreviewConfiguration()", "Discovery - Preview configuration Response: {0}", customData["json"].ToString());
    }

    private void OnGetCollections(GetCollectionsResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnGetCollections()", "Discovery - Get collections Response: {0}", customData["json"].ToString());
        _getCollectionsTested = true;
    }

    private void OnGetCollection(Collection resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnGetCollection()", "Discovery - Get colletion Response: {0}", customData["json"].ToString());
        _getCollectionTested = true;
    }

    private void OnAddCollection(CollectionRef resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnAddCollection()", "Discovery - Add collection Response: {0}", customData["json"].ToString());
        _createdCollectionID = resp.collection_id;
        _addCollectionTested = true;
    }

    private void OnGetFields(GetFieldsResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnGetFields()", "Discovery - Get fields Response: {0}", customData["json"].ToString());
        _getFieldsTested = true;
    }

    private void OnAddDocument(DocumentAccepted resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnAddDocument()", "Discovery - Add document Response: {0}", customData["json"].ToString());
        _createdDocumentID = resp.document_id;
        _addDocumentTested = true;
    }

    private void OnGetDocument(DocumentStatus resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnGetDocument()", "Discovery - Get document Response: {0}", customData["json"].ToString());
        _getDocumentTested = true;
    }

    private void OnUpdateDocument(DocumentAccepted resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnUpdateDocument()", "Discovery - Update document Response: {0}", customData["json"].ToString());
        _updateDocumentTested = true;
    }

    private void OnDeleteDocument(DeleteDocumentResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnDeleteDocument()", "Discovery - Delete document Response: deleted:{0}", customData["json"].ToString());
        _createdDocumentID = default(string);

        _deleteDocumentTested = true;
    }

    private void OnDeleteCollection(DeleteCollectionResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnDeleteCollection()", "Discovery - Delete collection Response: deleted:{0}", customData["json"].ToString());
        _createdCollectionID = default(string);

        _deleteCollectionTested = true;
    }

    private void OnDeleteConfiguration(DeleteConfigurationResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnDeleteConfiguration()", "Discovery - DeleteConfiguration Response: deleted:{0}", customData["json"].ToString());
        _environmentId = default(string);

        _deleteConfigurationTested = true;
    }

    private void OnQuery(QueryResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnQuery()", "Discovery - Query Response: {0}", customData["json"].ToString());
        _queryTested = true;
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("ExampleDiscovery.OnFail()", "Error received: {0}", error.ToString());
    }
}
