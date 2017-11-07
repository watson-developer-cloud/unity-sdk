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

using IBM.Watson.DeveloperCloud.Services.Discovery.v1;
using IBM.Watson.DeveloperCloud.Logging;
using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Connection;
using FullSerializer;
using System.IO;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestDiscovery : UnitTest
    {
        private string _username = null;
        private string _password = null;
        private fsSerializer _serializer = new fsSerializer();
        //private string _token = "<authentication-token>";

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
        private float _waitTime = 10f;

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

        public override IEnumerator RunTest()
        {
            LogSystem.InstallDefaultReactors();

            try
            {
                VcapCredentials vcapCredentials = new VcapCredentials();
                fsData data = null;

                //  Get credentials from a credential file defined in environmental variables in the VCAP_SERVICES format. 
                //  See https://www.ibm.com/watson/developercloud/doc/common/getting-started-variables.html.
                var environmentalVariable = System.Environment.GetEnvironmentVariable("VCAP_SERVICES");
                var fileContent = File.ReadAllText(environmentalVariable);

                //  Add in a parent object because Unity does not like to deserialize root level collection types.
                fileContent = Utility.AddTopLevelObjectToJson(fileContent, "VCAP_SERVICES");

                //  Convert json to fsResult
                fsResult r = fsJsonParser.Parse(fileContent, out data);
                if (!r.Succeeded)
                    throw new WatsonException(r.FormattedMessages);

                //  Convert fsResult to VcapCredentials
                object obj = vcapCredentials;
                r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                if (!r.Succeeded)
                    throw new WatsonException(r.FormattedMessages);

                //  Set credentials from imported credntials
                Credential credential = vcapCredentials.VCAP_SERVICES["discovery"][TestCredentialIndex].Credentials;
                _username = credential.Username.ToString();
                _password = credential.Password.ToString();
                _url = credential.Url.ToString();
            }
            catch
            {
                Log.Debug("TestDiscovery.RunTest()", "Failed to get credentials from VCAP_SERVICES file. Please configure credentials to run this test. For more information, see: https://github.com/watson-developer-cloud/unity-sdk/#authentication");
            }

            //  Create credential and instantiate service
            Credentials credentials = new Credentials(_username, _password, _url);

            //  Or authenticate using token
            //Credentials credentials = new Credentials(_url)
            //{
            //    AuthenticationToken = _token
            //};

            _discovery = new Discovery(credentials);
            _discovery.VersionDate = _discoveryVersionDate;
            _configurationJsonPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/Discovery/exampleConfigurationData.json";
            _filePathToIngest = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/watson_beats_jeopardy.html";
            _documentFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/watson_beats_jeopardy.html";

            //  Get Environments
            Log.Debug("TestDiscovery.RunTest()", "Attempting to get environments");
            if (!_discovery.GetEnvironments(OnGetEnvironments))
                Log.Debug("TestDiscovery.GetEnvironments()", "Failed to get environments");
            while (!_getEnvironmentsTested)
                yield return null;

            //  AddEnvironment
            Log.Debug("TestDiscovery.RunTest()", "Attempting to add environment");
            if (!_discovery.AddEnvironment(OnAddEnvironment, "unity-testing-AddEnvironment-do-not-delete-until-active", "Testing addEnvironment in Unity SDK. Please do not delete this environment until the status is 'active'", 1))
                Log.Debug("TestDiscovery.AddEnvironment()", "Failed to add environment");
            while (!_addEnvironmentTested)
                yield return null;

            //  Wait for environment to be ready
            Runnable.Run(CheckEnvironmentState(_waitTime));
            while (!_isEnvironmentReady)
                yield return null;

            //  GetEnvironment
            Log.Debug("TestDiscovery.RunTest()", "Attempting to get environment");
            if (!_discovery.GetEnvironment(OnGetEnvironment, _createdEnvironmentID))
                Log.Debug("TestDiscovery.GetEnvironment()", "Failed to get environment");
            while (!_getEnvironmentTested)
                yield return null;

            //  Get Configurations
            Log.Debug("TestDiscovery.RunTest()", "Attempting to get configurations");
            if (!_discovery.GetConfigurations(OnGetConfigurations, _createdEnvironmentID))
                Log.Debug("TestDiscovery.GetConfigurations()", "Failed to get configurations");
            while (!_getConfigurationsTested)
                yield return null;

            //  Add Configuration
            Log.Debug("TestDiscovery.RunTest()", "Attempting to add configuration");
            if (!_discovery.AddConfiguration(OnAddConfiguration, _createdEnvironmentID, _configurationJsonPath))
                Log.Debug("TestDiscovery.AddConfiguration()", "Failed to add configuration");
            while (!_addConfigurationTested)
                yield return null;

            //  Get Configuration
            Log.Debug("TestDiscovery.RunTest()", "Attempting to get configuration");
            if (!_discovery.GetConfiguration(OnGetConfiguration, _createdEnvironmentID, _createdConfigurationID))
                Log.Debug("TestDiscovery.GetConfiguration()", "Failed to get configuration");
            while (!_getConfigurationTested)
                yield return null;

            //  Preview Configuration
            Log.Debug("TestDiscovery.RunTest()", "Attempting to preview configuration");
            if (!_discovery.PreviewConfiguration(OnPreviewConfiguration, _createdEnvironmentID, _createdConfigurationID, null, _filePathToIngest, _metadata))
                Log.Debug("TestDiscovery.PreviewConfiguration()", "Failed to preview configuration");
            while (!_previewConfigurationTested)
                yield return null;

            //  Get Collections
            Log.Debug("TestDiscovery.RunTest()", "Attempting to get collections");
            if (!_discovery.GetCollections(OnGetCollections, _createdEnvironmentID))
                Log.Debug("TestDiscovery.GetCollections()", "Failed to get collections");
            while (!_getCollectionsTested)
                yield return null;

            //  Add Collection
            Log.Debug("TestDiscovery.RunTest()", "Attempting to add collection");
            if (!_discovery.AddCollection(OnAddCollection, _createdEnvironmentID, _createdCollectionName, _createdCollectionDescription, _createdConfigurationID))
                Log.Debug("TestDiscovery.AddCollection()", "Failed to add collection");
            while (!_addCollectionTested)
                yield return null;

            //  Get Collection
            Log.Debug("TestDiscovery.RunTest()", "Attempting to get collection");
            if (!_discovery.GetCollection(OnGetCollection, _createdEnvironmentID, _createdCollectionID))
                Log.Debug("TestDiscovery.GetCollection()", "Failed to get collection");
            while (!_getCollectionTested)
                yield return null;

            if (!_discovery.GetFields(OnGetFields, _createdEnvironmentID, _createdCollectionID))
                Log.Debug("TestDiscovery.GetFields()", "Failed to get fields");
            while (!_getFieldsTested)
                yield return null;

            //  Add Document
            Log.Debug("TestDiscovery.RunTest()", "Attempting to add document");
            if (!_discovery.AddDocument(OnAddDocument, _createdEnvironmentID, _createdCollectionID, _documentFilePath, _createdConfigurationID, null))
                Log.Debug("TestDiscovery.AddDocument()", "Failed to add document");
            while (!_addDocumentTested)
                yield return null;

            //  Get Document
            Log.Debug("TestDiscovery.RunTest()", "Attempting to get document");
            if (!_discovery.GetDocument(OnGetDocument, _createdEnvironmentID, _createdCollectionID, _createdDocumentID))
                Log.Debug("TestDiscovery.GetDocument()", "Failed to get document");
            while (!_getDocumentTested)
                yield return null;

            //  Update Document
            Log.Debug("TestDiscovery.RunTest()", "Attempting to update document");
            if (!_discovery.UpdateDocument(OnUpdateDocument, _createdEnvironmentID, _createdCollectionID, _createdDocumentID, _documentFilePath, _createdConfigurationID, null))
                Log.Debug("TestDiscovery.UpdateDocument()", "Failed to update document");
            while (!_updateDocumentTested)
                yield return null;

            //  Query
            Log.Debug("TestDiscovery.RunTest()", "Attempting to query");
            if (!_discovery.Query(OnQuery, _createdEnvironmentID, _createdCollectionID, null, _query, null, 10, null, 0))
                Log.Debug("TestDiscovery.Query()", "Failed to query");
            while (!_queryTested)
                yield return null;

            //  Delete Document
            Log.Debug("TestDiscovery.RunTest()", "Attempting to delete document {0}", _createdDocumentID);
            if (!_discovery.DeleteDocument(OnDeleteDocument, _createdEnvironmentID, _createdCollectionID, _createdDocumentID))
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
            if (!_discovery.DeleteCollection(OnDeleteCollection, _createdEnvironmentID, _createdCollectionID))
                Log.Debug("TestDiscovery.DeleteCollection()", "Failed to add collection");
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
            Log.Debug("TestDiscovery.RunTest()", "Attempting to delete configuration {0}", _createdConfigurationID);
            if (!_discovery.DeleteConfiguration(OnDeleteConfiguration, _createdEnvironmentID, _createdConfigurationID))
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

            _readyToContinue = false;
            //  Delete Environment
            Log.Debug("TestDiscovery.RunTest()", "Attempting to delete environment {0}", _createdEnvironmentID);
            if (!_discovery.DeleteEnvironment(OnDeleteEnvironment, _createdEnvironmentID))
                Log.Debug("TestDiscovery.DeleteEnvironment()", "Failed to delete environment");
            while (!_deleteEnvironmentTested)
                yield return null;

            if (!string.IsNullOrEmpty(_createdEnvironmentID))
            {
                if (!_discovery.GetEnvironment(OnGetEnvironment, _createdEnvironmentID))
                {
                    _discovery.DeleteEnvironment(OnDeleteEnvironment, _createdEnvironmentID);
                }
            }

            Log.Debug("TestDiscovery.RunTest()", "Discovery examples complete.");

            yield break;
        }

        #region Check State
        private IEnumerator CheckEnvironmentState(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            Log.Debug("TestDiscovery.CheckEnvironmentState()", "Attempting to get environment state");
            try
            {
                _discovery.GetEnvironment(HandleCheckEnvironmentState, _createdEnvironmentID);
            }
            catch (System.Exception e)
            {
                Log.Debug("TestDiscovery.CheckEnvironmentState()", string.Format("Failed to get environment state: {0}", e.Message));
                Runnable.Run(CheckEnvironmentState(10f));
            }
        }

        private void HandleCheckEnvironmentState(RESTConnector.ParsedResponse<Environment> resp)
        {
            Log.Debug("TestDiscovery.HandleCheckEnvironmentState()", "Environment {0} is {1}", resp.DataObject.environment_id, resp.DataObject.status);

            if (resp.DataObject.status.ToLower() == "active")
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

        private void OnGetEnvironments(RESTConnector.ParsedResponse<GetEnvironmentsResponse> resp)
        {
            Log.Debug("TestDiscovery.OnGetEnvironments()", "Discovery - GetEnvironments Response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getEnvironmentsTested = true;
        }

        private void OnGetEnvironment(RESTConnector.ParsedResponse<Environment> resp)
        {
            Log.Debug("TestDiscovery.OnGetEnvironment()", "Discovery - GetEnvironment Response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getEnvironmentTested = true;
        }

        private void OnAddEnvironment(RESTConnector.ParsedResponse<Environment> resp)
        {
            Log.Debug("TestDiscovery.OnAddEnvironment()", "Discovery - AddEnvironment Response: {0}", resp.JSON);
            _createdEnvironmentID = resp.DataObject.environment_id;
            Test(resp.DataObject != null);
            _addEnvironmentTested = true;
        }

        private void OnGetConfigurations(RESTConnector.ParsedResponse<GetConfigurationsResponse> resp)
        {
            Log.Debug("TestDiscovery.OnGetConfigurations()", "Discovery - GetConfigurations Response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getConfigurationsTested = true;
        }

        private void OnGetConfiguration(RESTConnector.ParsedResponse<Configuration> resp)
        {
            Log.Debug("TestDiscovery.OnGetConfiguration()", "Discovery - GetConfiguration Response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getConfigurationTested = true;

        }

        private void OnAddConfiguration(RESTConnector.ParsedResponse<Configuration> resp)
        {
            Log.Debug("TestDiscovery.OnAddConfiguration()", "Discovery - AddConfiguration Response: {0}", resp.JSON);
            _createdConfigurationID = resp.DataObject.configuration_id;
            Test(resp.DataObject != null);
            _addConfigurationTested = true;
        }

        private void OnPreviewConfiguration(RESTConnector.ParsedResponse<TestDocument> resp)
        {
            Log.Debug("TestDiscovery.OnPreviewConfiguration()", "Discovery - Preview configuration Response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _previewConfigurationTested = true;
        }

        private void OnGetCollections(RESTConnector.ParsedResponse<GetCollectionsResponse> resp)
        {
            Log.Debug("TestDiscovery.OnGetCollections()", "Discovery - Get collections Response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getCollectionsTested = true;
        }

        private void OnGetCollection(RESTConnector.ParsedResponse<Collection> resp)
        {
            Log.Debug("TestDiscovery.OnGetCollection()", "Discovery - Get colletion Response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getCollectionTested = true;
        }

        private void OnAddCollection(RESTConnector.ParsedResponse<CollectionRef> resp)
        {
            Log.Debug("TestDiscovery.OnAddCollection()", "Discovery - Add collection Response: {0}", resp.JSON);
            _createdCollectionID = resp.DataObject.collection_id;
            Test(resp.DataObject != null);
            _addCollectionTested = true;
        }

        private void OnGetFields(RESTConnector.ParsedResponse<GetFieldsResponse> resp)
        {
            Log.Debug("TestDiscovery.OnGetFields()", "Discovery - Get fields Response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getFieldsTested = true;
        }

        private void OnAddDocument(RESTConnector.ParsedResponse<DocumentAccepted> resp)
        {
            Log.Debug("TestDiscovery.OnAddDocument()", "Discovery - Add document Response: {0}", resp.JSON);
            _createdDocumentID = resp.DataObject.document_id;
            Test(resp.DataObject != null);
            _addDocumentTested = true;
        }

        private void OnGetDocument(RESTConnector.ParsedResponse<DocumentStatus> resp)
        {
            Log.Debug("TestDiscovery.OnGetDocument()", "Discovery - Get document Response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getDocumentTested = true;
        }

        private void OnUpdateDocument(RESTConnector.ParsedResponse<DocumentAccepted> resp)
        {
            Log.Debug("TestDiscovery.OnUpdateDocument()", "Discovery - Update document Response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _updateDocumentTested = true;
        }

        private void OnDeleteDocument(RESTConnector.ParsedResponse<object> resp)
        {
            Log.Debug("TestDiscovery.OnDeleteDocument()", "Discovery - Delete document Response: deleted:{0}", resp.Success);

            if (resp.Success)
                _createdDocumentID = default(string);
            Test(resp.Success);

            _deleteDocumentTested = true;
        }

        private void OnDeleteCollection(RESTConnector.ParsedResponse<object> resp)
        {
            Log.Debug("TestDiscovery.OnDeleteCollection()", "Discovery - Delete collection Response: deleted:{0}", resp.Success);

            if (resp.Success)
                _createdCollectionID = default(string);
            Test(resp.Success);

            _deleteCollectionTested = true;
        }

        private void OnDeleteConfiguration(RESTConnector.ParsedResponse<object> resp)
        {
            Log.Debug("TestDiscovery.OnDeleteConfiguration()", "Discovery - Delete configuration Response: deleted:{0}", resp.Success);

            if (resp.Success)
                _createdConfigurationID = default(string);
            Test(resp.Success);

            _deleteConfigurationTested = true;
        }

        private void OnDeleteEnvironment(RESTConnector.ParsedResponse<object> resp)
        {
            Log.Debug("TestDiscovery.OnDeleteEnvironment()", "Discovery - Delete environment Response: deleted:{0}", resp.Success);

            if (resp.Success)
                _createdEnvironmentID = default(string);
            Test(resp.Success);

            _deleteEnvironmentTested = true;
        }

        private void OnQuery(RESTConnector.ParsedResponse<QueryResponse> resp)
        {
            Log.Debug("TestDiscovery.OnQuery()", "Discovery - Query Response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _queryTested = true;
        }
    }
}
