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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Environment = IBM.Watson.DeveloperCloud.Services.Discovery.v1.Environment;

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
    private string _environmentId;
    private string _configurationJson = "{\"name\":\"IBM News {guid}\",\"description\":\"A configuration useful for ingesting IBM press releases. Safe to delete.\",\"conversions\":{\"html\":{\"exclude_tags_keep_content\":[\"span\"],\"exclude_content\":{\"xpaths\":[\"/home\"]}},\"segment\":{\"enabled\":true,\"selector_tags\":[\"h1\",\"h2\"]},\"json_normalizations\":[{\"operation\":\"move\",\"source_field\":\"extracted_metadata.title\",\"destination_field\":\"metadata.title\"},{\"operation\":\"move\",\"source_field\":\"extracted_metadata.author\",\"destination_field\":\"metadata.author\"},{\"operation\":\"remove\",\"source_field\":\"extracted_metadata\"}]},\"enrichments\":[{\"enrichment\":\"natural_language_understanding\",\"source_field\":\"title\",\"destination_field\":\"enriched_title\",\"options\":{\"features\":{\"keywords\":{\"sentiment\":true,\"emotion\":false,\"limit\":50},\"entities\":{\"sentiment\":true,\"emotion\":false,\"limit\":50,\"mentions\":true,\"mention_types\":true,\"sentence_locations\":true,\"model\":\"WKS-model-id\"},\"sentiment\":{\"document\":true,\"targets\":[\"IBM\",\"Watson\"]},\"emotion\":{\"document\":true,\"targets\":[\"IBM\",\"Watson\"]},\"categories\":{},\"concepts\":{\"limit\":8},\"semantic_roles\":{\"entities\":true,\"keywords\":true,\"limit\":50},\"relations\":{\"model\":\"WKS-model-id\"}}}}],\"normalizations\":[{\"operation\":\"move\",\"source_field\":\"metadata.title\",\"destination_field\":\"title\"},{\"operation\":\"move\",\"source_field\":\"metadata.author\",\"destination_field\":\"author\"},{\"operation\":\"move\",\"source_field\":\"alchemy_enriched_text.language\",\"destination_field\":\"language\"},{\"operation\":\"remove\",\"source_field\":\"html\"},{\"operation\":\"remove\",\"source_field\":\"alchemy_enriched_text.status\"},{\"operation\":\"remove\",\"source_field\":\"alchemy_enriched_text.text\"},{\"operation\":\"remove\",\"source_field\":\"sire_enriched_text.language\"},{\"operation\":\"remove\",\"source_field\":\"sire_enriched_text.model\"},{\"operation\":\"remove\",\"source_field\":\"sire_enriched_text.status\"},{\"operation\":\"remove_nulls\"}]}";
    private string _filePathToIngest;
    private string _metadata = "{\n\t\"Creator\": \"Unity SDK Integration Test\",\n\t\"Subject\": \"Discovery service\"\n}";
    private string _createdConfigurationId;
    private string _createdCollectionId;
    private string _createdCollectionName = "Unity SDK Created Collection";
    private string _createdCollectionDescription = "A collection created by the Unity SDK. Please delete me.";
    private string _createdDocumentID;
    private string _documentFilePath;
    private string _query = "When did Watson play Jeopardy?";
    private string _sessionToken;

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
    private float _waitTime = 10f;

    private bool _listCredentialsTested = false;
    private bool _createCredentialsTested = false;
    private bool _getCredentialTested = false;
    private bool _deleteCredentialsTested = false;
    private string _createdCredentialId = null;

    private bool _createEventTested = false;
    private bool _getMetricsEventRateTested = false;
    private bool _getMetricsQueryTested = false;
    private bool _getMetricsQueryEventTested = false;
    private bool _getMetricsQueryNoResultTested = false;
    private bool _getMetricsQueryTokenEventTested = false;
    private bool _queryLogTested = false;

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
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to get environments");
        if (!_service.GetEnvironments(OnGetEnvironments, OnFail))
            Log.Debug("ExampleDiscovery.GetEnvironments()", "Failed to get environments");
        while (!_getEnvironmentsTested)
            yield return null;

        //  Wait for environment to be ready
        Runnable.Run(CheckEnvironmentState(0f));
        while (!_isEnvironmentReady)
            yield return null;

        //  GetEnvironment
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to get environment");
        if (!_service.GetEnvironment(OnGetEnvironment, OnFail, _environmentId))
            Log.Debug("ExampleDiscovery.GetEnvironment()", "Failed to get environment");
        while (!_getEnvironmentTested)
            yield return null;

        //  Get Configurations
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to get configurations");
        if (!_service.GetConfigurations(OnGetConfigurations, OnFail, _environmentId))
            Log.Debug("ExampleDiscovery.GetConfigurations()", "Failed to get configurations");
        while (!_getConfigurationsTested)
            yield return null;

        //  Add Configuration
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to add configuration");
        if (!_service.AddConfiguration(OnAddConfiguration, OnFail, _environmentId, _configurationJson.Replace("{guid}", System.Guid.NewGuid().ToString())))
            Log.Debug("ExampleDiscovery.AddConfiguration()", "Failed to add configuration");
        while (!_addConfigurationTested)
            yield return null;

        //  Get Configuration
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to get configuration");
        if (!_service.GetConfiguration(OnGetConfiguration, OnFail, _environmentId, _createdConfigurationId))
            Log.Debug("ExampleDiscovery.GetConfiguration()", "Failed to get configuration");
        while (!_getConfigurationTested)
            yield return null;

        //  Preview Configuration
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to preview configuration");
        if (!_service.PreviewConfiguration(OnPreviewConfiguration, OnFail, _environmentId, _createdConfigurationId, null, _filePathToIngest, _metadata))
            Log.Debug("ExampleDiscovery.PreviewConfiguration()", "Failed to preview configuration");
        while (!_previewConfigurationTested)
            yield return null;

        //  Get Collections
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to get collections");
        if (!_service.GetCollections(OnGetCollections, OnFail, _environmentId))
            Log.Debug("ExampleDiscovery.GetCollections()", "Failed to get collections");
        while (!_getCollectionsTested)
            yield return null;

        //  Add Collection
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to add collection");
        if (!_service.AddCollection(OnAddCollection, OnFail, _environmentId, _createdCollectionName + System.Guid.NewGuid().ToString(), _createdCollectionDescription, _createdConfigurationId))
            Log.Debug("ExampleDiscovery.AddCollection()", "Failed to add collection");
        while (!_addCollectionTested)
            yield return null;

        //  Get Collection
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to get collection");
        if (!_service.GetCollection(OnGetCollection, OnFail, _environmentId, _createdCollectionId))
            Log.Debug("ExampleDiscovery.GetCollection()", "Failed to get collection");
        while (!_getCollectionTested)
            yield return null;

        if (!_service.GetFields(OnGetFields, OnFail, _environmentId, _createdCollectionId))
            Log.Debug("ExampleDiscovery.GetFields()", "Failed to get fields");
        while (!_getFieldsTested)
            yield return null;

        //  Add Document
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to add document");
        if (!_service.AddDocument(OnAddDocument, OnFail, _environmentId, _createdCollectionId, _documentFilePath, _createdConfigurationId, null))
            Log.Debug("ExampleDiscovery.AddDocument()", "Failed to add document");
        while (!_addDocumentTested)
            yield return null;

        //  Get Document
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to get document");
        if (!_service.GetDocument(OnGetDocument, OnFail, _environmentId, _createdCollectionId, _createdDocumentID))
            Log.Debug("ExampleDiscovery.GetDocument()", "Failed to get document");
        while (!_getDocumentTested)
            yield return null;

        //  Update Document
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to update document");
        if (!_service.UpdateDocument(OnUpdateDocument, OnFail, _environmentId, _createdCollectionId, _createdDocumentID, _documentFilePath, _createdConfigurationId, null))
            Log.Debug("ExampleDiscovery.UpdateDocument()", "Failed to update document");
        while (!_updateDocumentTested)
            yield return null;

        //  Query
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to query");
        if(!_service.Query(OnQuery, OnFail, _environmentId, _createdCollectionId, naturalLanguageQuery: _query))
        while (!_queryTested)
            yield return null;

        //  List Credentials
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to list credentials");
        _service.ListCredentials(OnListCredentials, OnFail, _environmentId);
        while (!_listCredentialsTested)
            yield return null;

        //  Create Credentials
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to create credentials");
        SourceCredentials credentialsParameter = new SourceCredentials()
        {
            SourceType = SourceCredentials.SourceTypeEnum.box,
            CredentialDetails = new CredentialDetails()
            {
                CredentialType = CredentialDetails.CredentialTypeEnum.oauth2,
                EnterpriseId = "myEnterpriseId",
                ClientId = "myClientId",
                ClientSecret = "myClientSecret",
                PublicKeyId = "myPublicIdKey",
                Passphrase = "myPassphrase",
                PrivateKey = "myPrivateKey"
            }
        };
        _service.CreateCredentials(OnCreateCredentials, OnFail, _environmentId, credentialsParameter);
        while (!_createCredentialsTested)
            yield return null;

        //  Get Credential
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to get credential");
        _service.GetCredential(OnGetCredential, OnFail, _environmentId, _createdCredentialId);
        while (!_getCredentialTested)
            yield return null;

        //  Get metrics event rate
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to Get metrics event rate");
        _service.GetMetricsEventRate(OnGetMetricsEventRate, OnFail);
        while (!_getMetricsEventRateTested)
            yield return null;

        //  Get metrics query
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to Get metrics query");
        _service.GetMetricsQuery(OnGetMetricsQuery, OnFail);
        while (!_getMetricsQueryTested)
            yield return null;

        //  Get metrics query event
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to Get metrics query event");
        _service.GetMetricsQueryEvent(OnGetMetricsQueryEvent, OnFail);
        while (!_getMetricsQueryEventTested)
            yield return null;

        //  Get metrics query no result
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to Get metrics query no result");
        _service.GetMetricsQueryNoResults(OnGetMetricsQueryNoResult, OnFail);
        while (!_getMetricsQueryNoResultTested)
            yield return null;

        //  Get metrics query token event
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to Get metrics query token event");
        _service.GetMetricsQueryTokenEvent(OnGetMetricsQueryTokenEvent, OnFail);
        while (!_getMetricsQueryTokenEventTested)
            yield return null;

        //  Query log
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to Query log");
        _service.QueryLog(OnQueryLog, OnFail);
        while (!_queryLogTested)
            yield return null;

        //  Create event
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to create event");
        CreateEventObject queryEvent = new CreateEventObject()
        {
            Type = CreateEventObject.TypeEnum.click,
            Data = new EventData()
            {
                EnvironmentId = _environmentId,
                SessionToken = _sessionToken,
                CollectionId = _createdCollectionId,
                DocumentId = _createdDocumentID
            }
        };
        _service.CreateEvent(OnCreateEvent, OnFail, queryEvent);
        while (!_createEventTested)
            yield return null;

        //  DeleteCredential
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to delete credential");
        _service.DeleteCredentials(OnDeleteCredentials, OnFail, _environmentId, _createdCredentialId);
        while (!_deleteCredentialsTested)
            yield return null;

        //  Delete Document
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to delete document {0}", _createdDocumentID);
        if (!_service.DeleteDocument(OnDeleteDocument, OnFail, _environmentId, _createdCollectionId, _createdDocumentID))
            Log.Debug("ExampleDiscovery.DeleteDocument()", "Failed to delete document");
        while (!_deleteDocumentTested)
            yield return null;

        _isEnvironmentReady = false;
        Runnable.Run(CheckEnvironmentState(_waitTime));
        while (!_isEnvironmentReady)
            yield return null;

        //  Delete Collection
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to delete collection {0}", _createdCollectionId);
        if (!_service.DeleteCollection(OnDeleteCollection, OnFail, _environmentId, _createdCollectionId))
            Log.Debug("ExampleDiscovery.DeleteCollection()", "Failed to delete collection");
        while (!_deleteCollectionTested)
            yield return null;

        _isEnvironmentReady = false;
        Runnable.Run(CheckEnvironmentState(_waitTime));
        while (!_isEnvironmentReady)
            yield return null;

        //  Delete Configuration
        Log.Debug("ExampleDiscovery.RunTest()", "Attempting to delete configuration {0}", _environmentId);
        if (!_service.DeleteConfiguration(OnDeleteConfiguration, OnFail, _environmentId, _createdConfigurationId))
            Log.Debug("ExampleDiscovery.DeleteConfiguration()", "Failed to delete configuration");
        while (!_deleteConfigurationTested)
            yield return null;

        _isEnvironmentReady = false;
        Runnable.Run(CheckEnvironmentState(_waitTime));
        while (!_isEnvironmentReady)
            yield return null;

        Log.Debug("ExampleDiscovery.RunTest()", "Discovery examples complete.");
    }

    #region Check State
    private IEnumerator CheckEnvironmentState(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Log.Debug("ExampleDiscovery.CheckEnvironmentState()", "Attempting to get environment state");
        try
        {
            _service.GetEnvironment(HandleCheckEnvironmentState, OnFail, _environmentId);
        }
        catch (System.Exception e)
        {
            Log.Debug("ExampleDiscovery.CheckEnvironmentState()", string.Format("Failed to get environment state: {0}", e.Message));
            Runnable.Run(CheckEnvironmentState(10f));
        }
    }

    private void HandleCheckEnvironmentState(Environment resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.HandleCheckEnvironmentState()", "Environment {0} is {1}", resp.environment_id, resp.status);

        if (resp.status.ToLower() == "active")
            _isEnvironmentReady = true;
        else
        {
            Runnable.Run(CheckEnvironmentState(10f));
        }
    }
    #endregion

    private void OnGetEnvironments(GetEnvironmentsResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnGetEnvironments()", "Discovery - GetEnvironments Response: {0}", customData["json"].ToString());

        foreach (var environment in resp.environments)
        {
            if (environment.read_only == false)
            {
                Log.Debug("ExampleDiscovery.OnGetEnvironments()", "setting environment to {0}", environment.environment_id);
                _environmentId = environment.environment_id;
            }
        }

        _getEnvironmentsTested = true;
    }

    private void OnGetEnvironment(Environment resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnGetEnvironment()", "Discovery - GetEnvironment Response: {0}", customData["json"].ToString());
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
        _createdConfigurationId= resp.configuration_id;
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
        _createdCollectionId = resp.collection_id;
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
        _createdCollectionId = default(string);

        _deleteCollectionTested = true;
    }

    private void OnDeleteConfiguration(DeleteConfigurationResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnDeleteConfiguration()", "Discovery - DeleteConfiguration Response: deleted:{0}", customData["json"].ToString());
        _createdConfigurationId = default(string);

        _deleteConfigurationTested = true;
    }

    private void OnQuery(QueryResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnQuery()", "Discovery - Query Response: {0}", customData["json"].ToString());
        _sessionToken = resp.SessionToken;
        _queryTested = true;
    }

    private void OnListCredentials(CredentialsList response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnListCredentials()", "Response: {0}", customData["json"].ToString());
        _listCredentialsTested = true;
    }
    private void OnCreateCredentials(SourceCredentials response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnCreateCredentials()", "Response: {0}", customData["json"].ToString());
        _createdCredentialId = response.CredentialId;
        _createCredentialsTested = true;
    }

    private void OnGetCredential(SourceCredentials response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnGetCredential()", "Response: {0}", customData["json"].ToString());
        _getCredentialTested = true;
    }

    private void OnDeleteCredentials(DeleteCredentials response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnDeleteCredentials()", "Response: {0}", customData["json"].ToString());
        _deleteCredentialsTested = true;
    }

    private void OnCreateEvent(CreateEventResponse response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnCreateEvent()", "Response: {0}", customData["json"].ToString());
        _createEventTested = true;
    }
    
    private void OnGetMetricsEventRate(MetricResponse response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnGetMetricsEventRate()", "Response: {0}", customData["json"].ToString());
        _getMetricsEventRateTested = true;
    }

    private void OnGetMetricsQuery(MetricResponse response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnGetMetricsQuery()", "Response: {0}", customData["json"].ToString());
        _getMetricsQueryTested = true;
    }

    private void OnGetMetricsQueryEvent(MetricResponse response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnGetMetricsQueryEvent()", "Response: {0}", customData["json"].ToString());
        _getMetricsQueryEventTested = true;
    }

    private void OnGetMetricsQueryNoResult(MetricResponse response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnGetMetricsQueryNoResult()", "Response: {0}", customData["json"].ToString());
        _getMetricsQueryNoResultTested = true;
    }

    private void OnGetMetricsQueryTokenEvent(MetricTokenResponse response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnGetMetricsQueryTokenEvent()", "Response: {0}", customData["json"].ToString());
        _getMetricsQueryTokenEventTested = true;
    }

    private void OnQueryLog(LogQueryResponse response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDiscovery.OnQueryLog()", "Response: {0}", customData["json"].ToString());
        _queryLogTested = true;
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("ExampleDiscovery.OnFail()", "Error received: {0}", error.ToString());
    }
}
