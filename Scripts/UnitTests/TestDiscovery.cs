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
using FullSerializer;
using System.IO;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Connection;
using System;
using Environment = IBM.Watson.DeveloperCloud.Services.Discovery.v1.Environment;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestDiscovery : UnitTest
    {
        private string _username = null;
        private string _password = null;
        private fsSerializer _serializer = new fsSerializer();

        private Discovery _discovery;
        private string _discoveryVersionDate = "2016-12-01";

        private string _environmentId;
        private string _createdConfigurationID;
        private string _filePathToIngest;
        private string _configurationJson = "{\"name\":\"IBM News {guid}\",\"description\":\"A configuration useful for ingesting IBM press releases. Safe to delete.\",\"conversions\":{\"html\":{\"exclude_tags_keep_content\":[\"span\"],\"exclude_content\":{\"xpaths\":[\"/home\"]}},\"segment\":{\"enabled\":true,\"selector_tags\":[\"h1\",\"h2\"]},\"json_normalizations\":[{\"operation\":\"move\",\"source_field\":\"extracted_metadata.title\",\"destination_field\":\"metadata.title\"},{\"operation\":\"move\",\"source_field\":\"extracted_metadata.author\",\"destination_field\":\"metadata.author\"},{\"operation\":\"remove\",\"source_field\":\"extracted_metadata\"}]},\"enrichments\":[{\"enrichment\":\"natural_language_understanding\",\"source_field\":\"title\",\"destination_field\":\"enriched_title\",\"options\":{\"features\":{\"keywords\":{\"sentiment\":true,\"emotion\":false,\"limit\":50},\"entities\":{\"sentiment\":true,\"emotion\":false,\"limit\":50,\"mentions\":true,\"mention_types\":true,\"sentence_locations\":true,\"model\":\"WKS-model-id\"},\"sentiment\":{\"document\":true,\"targets\":[\"IBM\",\"Watson\"]},\"emotion\":{\"document\":true,\"targets\":[\"IBM\",\"Watson\"]},\"categories\":{},\"concepts\":{\"limit\":8},\"semantic_roles\":{\"entities\":true,\"keywords\":true,\"limit\":50},\"relations\":{\"model\":\"WKS-model-id\"}}}},{\"enrichment\":\"elements\",\"source_field\":\"html\",\"destination_field\":\"enriched_html\",\"options\":{\"model\":\"contract\"}}],\"normalizations\":[{\"operation\":\"move\",\"source_field\":\"metadata.title\",\"destination_field\":\"title\"},{\"operation\":\"move\",\"source_field\":\"metadata.author\",\"destination_field\":\"author\"},{\"operation\":\"move\",\"source_field\":\"alchemy_enriched_text.language\",\"destination_field\":\"language\"},{\"operation\":\"remove\",\"source_field\":\"html\"},{\"operation\":\"remove\",\"source_field\":\"alchemy_enriched_text.status\"},{\"operation\":\"remove\",\"source_field\":\"alchemy_enriched_text.text\"},{\"operation\":\"remove\",\"source_field\":\"sire_enriched_text.language\"},{\"operation\":\"remove\",\"source_field\":\"sire_enriched_text.model\"},{\"operation\":\"remove\",\"source_field\":\"sire_enriched_text.status\"},{\"operation\":\"remove_nulls\"}]}";
        private string _metadata = "{\n\t\"Creator\": \"Unity SDK Integration Test\",\n\t\"Subject\": \"Discovery service\"\n}";
        private string _createdCollectionId;
        private string _createdCollectionName = "Unity SDK Created Collection - please delete me";
        private string _createdCollectionDescription = "A collection created by the Unity SDK. Please delete me.";
        private string _createdDocumentID;
        private string _documentFilePath;
        private string _query = "What is the capital of china?";
        private float _waitTime = 10f;
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
        private bool _deleteUserDataTested = false;

        private bool _listCredentialsTested = false;
        private bool _createCredentialsTested = false;
        private bool _getCredentialTested = false;
        private bool _deleteCredentialsTested = false;
        private string _createdCredentialId = null;

        private bool _listExpansionsTested = false;
        private bool _createExpansionTested = false;
        private bool _deleteExpansionTested = false;
        private bool _getTokenizationDictStatusTested = false;
        private bool _createTokenizationDictTested = false;
        private bool _deleteTokenizationDictTested = false;
        private string _createdJpCollection = null;
        private bool _createJpCollectionTested = false;
        private bool _deleteJpCollectionTested = false;

        private bool _createEventTested = false;
        private bool _getMetricsEventRateTested = false;
        private bool _getMetricsQueryTested = false;
        private bool _getMetricsQueryEventTested = false;
        private bool _getMetricsQueryNoResultTested = false;
        private bool _getMetricsQueryTokenEventTested = false;
        private bool _queryLogTested = false;

        public override IEnumerator RunTest()
        {
            LogSystem.InstallDefaultReactors();

            VcapCredentials vcapCredentials = new VcapCredentials();
            fsData data = null;

            string result = null;
            string credentialsFilepath = "../sdk-credentials/credentials.json";

            //  Load credentials file if it exists. If it doesn't exist, don't run the tests.
            if (File.Exists(credentialsFilepath))
                result = File.ReadAllText(credentialsFilepath);
            else
                yield break;

            //  Add in a parent object because Unity does not like to deserialize root level collection types.
            result = Utility.AddTopLevelObjectToJson(result, "VCAP_SERVICES");

            //  Convert json to fsResult
            fsResult r = fsJsonParser.Parse(result, out data);
            if (!r.Succeeded)
                throw new WatsonException(r.FormattedMessages);

            //  Convert fsResult to VcapCredentials
            object obj = vcapCredentials;
            r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
            if (!r.Succeeded)
                throw new WatsonException(r.FormattedMessages);

            //  Set credentials from imported credntials
            Credential credential = vcapCredentials.GetCredentialByname("discovery-sdk")[0].Credentials;
            _username = credential.Username.ToString();
            _password = credential.Password.ToString();
            _url = credential.Url.ToString();

            //  Create credential and instantiate service
            Credentials credentials = new Credentials(_username, _password, _url);

            _discovery = new Discovery(credentials);
            _discovery.VersionDate = _discoveryVersionDate;
            _filePathToIngest = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/Discovery/constitution.pdf";
            _documentFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/Discovery/constitution.pdf";

            //  Get Environments
            Log.Debug("TestDiscovery.RunTest()", "Attempting to get environments");
            if (!_discovery.GetEnvironments(OnGetEnvironments, OnFail))
                Log.Debug("TestDiscovery.GetEnvironments()", "Failed to get environments");
            while (!_getEnvironmentsTested)
                yield return null;

            //  Wait for environment to be ready
            Runnable.Run(CheckEnvironmentState(0f));
            while (!_isEnvironmentReady)
                yield return null;

            //  GetEnvironment
            Log.Debug("TestDiscovery.RunTest()", "Attempting to get environment");
            if (!_discovery.GetEnvironment(OnGetEnvironment, OnFail, _environmentId))
                Log.Debug("TestDiscovery.GetEnvironment()", "Failed to get environment");
            while (!_getEnvironmentTested)
                yield return null;

            //  Get Configurations
            Log.Debug("TestDiscovery.RunTest()", "Attempting to get configurations");
            if (!_discovery.GetConfigurations(OnGetConfigurations, OnFail, _environmentId))
                Log.Debug("TestDiscovery.GetConfigurations()", "Failed to get configurations");
            while (!_getConfigurationsTested)
                yield return null;

            //  Add Configuration
            Log.Debug("TestDiscovery.RunTest()", "Attempting to add configuration");
            if (!_discovery.AddConfiguration(OnAddConfiguration, OnFail, _environmentId, _configurationJson.Replace("{guid}", System.Guid.NewGuid().ToString())))
                Log.Debug("TestDiscovery.AddConfiguration()", "Failed to add configuration");
            while (!_addConfigurationTested)
                yield return null;

            //  Get Configuration
            Log.Debug("TestDiscovery.RunTest()", "Attempting to get configuration");
            if (!_discovery.GetConfiguration(OnGetConfiguration, OnFail, _environmentId, _createdConfigurationID))
                Log.Debug("TestDiscovery.GetConfiguration()", "Failed to get configuration");
            while (!_getConfigurationTested)
                yield return null;

            //  Preview Configuration
            Log.Debug("TestDiscovery.RunTest()", "Attempting to preview configuration");
            if (!_discovery.PreviewConfiguration(OnPreviewConfiguration, OnFail, _environmentId, _createdConfigurationID, null, _filePathToIngest, _metadata))
                Log.Debug("TestDiscovery.PreviewConfiguration()", "Failed to preview configuration");
            while (!_previewConfigurationTested)
                yield return null;

            //  Get Collections
            Log.Debug("TestDiscovery.RunTest()", "Attempting to get collections");
            if (!_discovery.GetCollections(OnGetCollections, OnFail, _environmentId))
                Log.Debug("TestDiscovery.GetCollections()", "Failed to get collections");
            while (!_getCollectionsTested)
                yield return null;

            //  Add Collection
            Log.Debug("TestDiscovery.RunTest()", "Attempting to add collection");
            if (!_discovery.AddCollection(OnAddCollection, OnFail, _environmentId, _createdCollectionName + System.Guid.NewGuid().ToString(), _createdCollectionDescription, _createdConfigurationID, "en"))
                Log.Debug("TestDiscovery.AddCollection()", "Failed to add collection");
            while (!_addCollectionTested)
                yield return null;

            //  Get Collection
            Log.Debug("TestDiscovery.RunTest()", "Attempting to get collection");
            if (!_discovery.GetCollection(OnGetCollection, OnFail, _environmentId, _createdCollectionId))
                Log.Debug("TestDiscovery.GetCollection()", "Failed to get collection");
            while (!_getCollectionTested)
                yield return null;

            if (!_discovery.GetFields(OnGetFields, OnFail, _environmentId, _createdCollectionId))
                Log.Debug("TestDiscovery.GetFields()", "Failed to get fields");
            while (!_getFieldsTested)
                yield return null;

            //  Add Document
            Log.Debug("TestDiscovery.RunTest()", "Attempting to add document");
            if (!_discovery.AddDocument(OnAddDocument, OnFail, _environmentId, _createdCollectionId, _documentFilePath, _createdConfigurationID, null))
                Log.Debug("TestDiscovery.AddDocument()", "Failed to add document");
            while (!_addDocumentTested)
                yield return null;

            //  Get Document
            Log.Debug("TestDiscovery.RunTest()", "Attempting to get document");
            if (!_discovery.GetDocument(OnGetDocument, OnFail, _environmentId, _createdCollectionId, _createdDocumentID))
                Log.Debug("TestDiscovery.GetDocument()", "Failed to get document");
            while (!_getDocumentTested)
                yield return null;

            //  Update Document
            Log.Debug("TestDiscovery.RunTest()", "Attempting to update document");
            if (!_discovery.UpdateDocument(OnUpdateDocument, OnFail, _environmentId, _createdCollectionId, _createdDocumentID, _documentFilePath, _createdConfigurationID, null))
                Log.Debug("TestDiscovery.UpdateDocument()", "Failed to update document");
            while (!_updateDocumentTested)
                yield return null;

            //  Query
            Log.Debug("TestDiscovery.RunTest()", "Attempting to query");
            if (!_discovery.Query(OnQuery, OnFail, _environmentId, _createdCollectionId, naturalLanguageQuery: _query))
                while (!_queryTested)
                    yield return null;

            //  List Credentials
            Log.Debug("TestDiscovery.RunTest()", "Attempting to list credentials");
            _discovery.ListCredentials(OnListCredentials, OnFail, _environmentId);
            while (!_listCredentialsTested)
                yield return null;

            //  Create Credentials
            Log.Debug("TestDiscovery.RunTest()", "Attempting to create credentials");
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
            _discovery.CreateCredentials(OnCreateCredentials, OnFail, _environmentId, credentialsParameter);
            while (!_createCredentialsTested)
                yield return null;

            //  Get Credential
            Log.Debug("TestDiscovery.RunTest()", "Attempting to get credential");
            _discovery.GetCredential(OnGetCredential, OnFail, _environmentId, _createdCredentialId);
            while (!_getCredentialTested)
                yield return null;

            //  Get metrics event rate
            Log.Debug("TestDiscovery.RunTest()", "Attempting to Get metrics event rate");
            _discovery.GetMetricsEventRate(OnGetMetricsEventRate, OnFail);
            while (!_getMetricsEventRateTested)
                yield return null;

            //  Get metrics query
            Log.Debug("TestDiscovery.RunTest()", "Attempting to Get metrics query");
            _discovery.GetMetricsQuery(OnGetMetricsQuery, OnFail);
            while (!_getMetricsQueryTested)
                yield return null;

            //  Get metrics query event
            Log.Debug("TestDiscovery.RunTest()", "Attempting to Get metrics query event");
            _discovery.GetMetricsQueryEvent(OnGetMetricsQueryEvent, OnFail);
            while (!_getMetricsQueryEventTested)
                yield return null;

            //  Get metrics query no result
            Log.Debug("TestDiscovery.RunTest()", "Attempting to Get metrics query no result");
            _discovery.GetMetricsQueryNoResults(OnGetMetricsQueryNoResult, OnFail);
            while (!_getMetricsQueryNoResultTested)
                yield return null;

            //  Get metrics query token event
            Log.Debug("TestDiscovery.RunTest()", "Attempting to Get metrics query token event");
            _discovery.GetMetricsQueryTokenEvent(OnGetMetricsQueryTokenEvent, OnFail);
            while (!_getMetricsQueryTokenEventTested)
                yield return null;

            //  Query log
            Log.Debug("TestDiscovery.RunTest()", "Attempting to Query log");
            _discovery.QueryLog(OnQueryLog, OnFail);
            while (!_queryLogTested)
                yield return null;

            //  Create event
            Log.Debug("TestDiscovery.RunTest()", "Attempting to create event");
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
            _discovery.CreateEvent(OnCreateEvent, OnFail, queryEvent);
            while (!_createEventTested)
                yield return null;

            Log.Debug("TestDiscovery.RunTests()", "Attempting to list expansions");
            _discovery.ListExpansions(OnListExpansions, OnFail, _environmentId, _createdCollectionId);
            while (!_listExpansionsTested)
                yield return null;

            Log.Debug("TestDiscovery.RunTests()", "Attempting to create expansion");
            Expansions expansions = new Expansions()
            {
                ExpansionsProperty = new List<Expansion>()
                {
                    new Expansion()
                    {
                        ExpandedTerms = new List<string>()
                        {
                            "expanded-term"
                        },
                        InputTerms = new List<string>()
                        {
                            "input-term"
                        }
                    }
                }
            };
            _discovery.CreateExpansions(OnCreateExpansion, OnFail, _environmentId, _createdCollectionId, expansions);
            while (!_createExpansionTested)
                yield return null;

            Log.Debug("TestDiscovery.RunTests()", "Attempting to delete expansion");
            _discovery.DeleteExpansions(OnDeleteExpansion, OnFail, _environmentId, _createdCollectionId);
            while (!_deleteExpansionTested)
                yield return null;
            
            Log.Debug("TestDiscovery.RunTest()", "Attempting to create jp collection");
            _discovery.AddCollection(OnCreateJpCollection, OnFail, _environmentId, _createdCollectionName + System.Guid.NewGuid().ToString(), _createdCollectionDescription, _createdConfigurationID, "ja");
            while (!_createJpCollectionTested)
                yield return null;

            TokenDict tokenizationDictionary = new TokenDict()
            {
                TokenizationRules = new List<TokenDictRule>()
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
                }
            };
            Log.Debug("TestDiscovery.RunTests()", "Attempting to create tokenization dict");
            _discovery.CreateTokenizationDictionary(OnCreateTokenizationDictionary, OnCreateTokenizationDictionaryFail, _environmentId, _createdJpCollection, tokenizationDictionary);
            while (!_createTokenizationDictTested)
                yield return null;

            if (!_getTokenizationDictStatusTested)
            {
                Log.Debug("TestDiscovery.RunTests()", "Attempting to get tokenization dict status");
                _discovery.GetTokenizationDictionaryStatus(OnGetTokenizationDictonaryStatus, OnFail, _environmentId, _createdJpCollection);
                while (!_getTokenizationDictStatusTested)
                    yield return null;
            }

            if (!_deleteTokenizationDictTested)
            {
                Log.Debug("TestDiscovery.RunTests()", "Attempting to delete tokenization dict");
                _discovery.DeleteTokenizationDictionary(OnDeleteTokenizationDictionary, OnFail, _environmentId, _createdJpCollection);
                while (!_deleteTokenizationDictTested)
                    yield return null;
            }

            Log.Debug("TestDiscovery.RunTest()", "Attempting to delete jp collection");
            _discovery.DeleteCollection(OnDeleteJpCollection, OnFail, _environmentId, _createdJpCollection);
            while (!_deleteJpCollectionTested)
                yield return null;

            //  DeleteCredential
            Log.Debug("TestDiscovery.RunTest()", "Attempting to delete credential");
            _discovery.DeleteCredentials(OnDeleteCredentials, OnFail, _environmentId, _createdCredentialId);
            while (!_deleteCredentialsTested)
                yield return null;

            //  Delete Document
            Log.Debug("TestDiscovery.RunTest()", "Attempting to delete document {0}", _createdDocumentID);
            if (!_discovery.DeleteDocument(OnDeleteDocument, OnFail, _environmentId, _createdCollectionId, _createdDocumentID))
                Log.Debug("TestDiscovery.DeleteDocument()", "Failed to delete document");
            while (!_deleteDocumentTested)
                yield return null;

            _isEnvironmentReady = false;
            Runnable.Run(CheckEnvironmentState(_waitTime));
            while (!_isEnvironmentReady)
                yield return null;

            //  Delete Collection
            Log.Debug("TestDiscovery.RunTest()", "Attempting to delete collection {0}", _createdCollectionId);
            if (!_discovery.DeleteCollection(OnDeleteCollection, OnFail, _environmentId, _createdCollectionId))
                Log.Debug("TestDiscovery.DeleteCollection()", "Failed to delete collection");
            while (!_deleteCollectionTested)
                yield return null;

            _isEnvironmentReady = false;
            Runnable.Run(CheckEnvironmentState(_waitTime));
            while (!_isEnvironmentReady)
                yield return null;

            //  Delete Configuration
            Log.Debug("TestDiscovery.RunTest()", "Attempting to delete configuration {0}", _createdConfigurationID);
            if (!_discovery.DeleteConfiguration(OnDeleteConfiguration, OnFail, _environmentId, _createdConfigurationID))
                Log.Debug("TestDiscovery.DeleteConfiguration()", "Failed to delete configuration");
            while (!_deleteConfigurationTested)
                yield return null;

            //  Delete User Data
            Log.Debug("TestDiscovery.RunTest()", "Attempting to delete user data.");
            _discovery.DeleteUserData(OnDeleteUserData, OnFail, "test-unity-user-id");
            while (!_deleteUserDataTested)
                yield return null;

            Log.Debug("TestDiscovery.RunTest()", "Discovery unit tests complete.");

            yield break;
        }

        private void OnCreateJpCollection(CollectionRef response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnAddJpCollection()", "Discovery - add jp collection Response: added:{0}", customData["json"].ToString());
            _createdJpCollection = response.collection_id;
            Test(response != null);
            _createJpCollectionTested = true;
        }

        private void OnDeleteJpCollection(DeleteCollectionResponse response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnDeleteJpCollection()", "Discovery - Delete jp collection Response: deleted:{0}", customData["json"].ToString());

            _createdJpCollection = default(string);
            Test(response != null);

            _deleteJpCollectionTested = true;
        }

        private void OnDeleteTokenizationDictionary(object response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnDeleteTokenizationDictionary()", "Discovery - delete tokenization dictionary: deleted");
            Test(response != null);

            _deleteTokenizationDictTested = true;
        }

        private void OnGetTokenizationDictonaryStatus(TokenDictStatusResponse response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnGetTokenizationDictonaryStatus()", "Discovery - get tokenization dictionary status: {0}", customData["json"].ToString());
            Test(response != null);
            _getTokenizationDictStatusTested = true;
        }
    

        private void OnCreateTokenizationDictionary(TokenDictStatusResponse response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnCreateTokenizationDictionary()", "Discovery - create tokenization dictionary status: {0}", customData["json"].ToString());
            Test(response != null);
            _createTokenizationDictTested = true;
        }

        

        private void OnDeleteExpansion(object response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnDeleteExpansion()", "Discovery - delete expansion: deleted");
            Test(response != null);

            _deleteExpansionTested = true;
        }

        private void OnCreateExpansion(Expansions response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnCreateExpansion()", "Discovery - create expansion: {0}", customData["json"].ToString());
            Test(response != null);

            _createExpansionTested = true;
        }

        private void OnListExpansions(Expansions response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnListExpansions()", "Discovery - list expansions: {0}", customData["json"].ToString());
            Test(response != null);

            _listExpansionsTested = true;
        }

        #region Check State
        private IEnumerator CheckEnvironmentState(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            Log.Debug("TestDiscovery.CheckEnvironmentState()", "Attempting to get environment state");
            try
            {
                _discovery.GetEnvironment(HandleCheckEnvironmentState, OnFail, _environmentId);
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
        #endregion

        private void OnGetEnvironments(GetEnvironmentsResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnGetEnvironments()", "Discovery - GetEnvironments Response: {0}", customData["json"].ToString());
            foreach (var environment in resp.environments)
            {
                if (environment.read_only == false)
                {
                    Log.Debug("TestDiscovery.OnGetEnvironments()", "setting environment to {0}", environment.environment_id);
                    _environmentId = environment.environment_id;
                }
            }
            Test(resp != null);
            _getEnvironmentsTested = true;
        }

        private void OnGetEnvironment(Environment resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnGetEnvironment()", "Discovery - GetEnvironment Response: {0}", customData["json"].ToString());
            Test(resp != null);
            _getEnvironmentTested = true;
        }

        private void OnGetConfigurations(GetConfigurationsResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnGetConfigurations()", "Discovery - GetConfigurations Response: {0}", customData["json"].ToString());
            Test(resp != null);
            _getConfigurationsTested = true;
        }

        private void OnGetConfiguration(Configuration resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnGetConfiguration()", "Discovery - GetConfiguration Response: {0}", customData["json"].ToString());
            Test(resp != null);
            _getConfigurationTested = true;

        }

        private void OnAddConfiguration(Configuration resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnAddConfiguration()", "Discovery - AddConfiguration Response: {0}", customData["json"].ToString());
            _createdConfigurationID = resp.configuration_id;
            Test(resp != null);
            _addConfigurationTested = true;
        }

        private void OnPreviewConfiguration(TestDocument resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnPreviewConfiguration()", "Discovery - Preview configuration Response: {0}", customData["json"].ToString());
            Test(resp != null);
            _previewConfigurationTested = true;
        }

        private void OnGetCollections(GetCollectionsResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnGetCollections()", "Discovery - Get collections Response: {0}", customData["json"].ToString());
            Test(resp != null);
            _getCollectionsTested = true;
        }

        private void OnGetCollection(Collection resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnGetCollection()", "Discovery - Get colletion Response: {0}", customData["json"].ToString());
            Test(resp != null);
            _getCollectionTested = true;
        }

        private void OnAddCollection(CollectionRef resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnAddCollection()", "Discovery - Add collection Response: {0}", customData["json"].ToString());
            _createdCollectionId = resp.collection_id;
            Test(resp != null);
            _addCollectionTested = true;
        }

        private void OnGetFields(GetFieldsResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnGetFields()", "Discovery - Get fields Response: {0}", customData["json"].ToString());
            Test(resp != null);
            _getFieldsTested = true;
        }

        private void OnAddDocument(DocumentAccepted resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnAddDocument()", "Discovery - Add document Response: {0}", customData["json"].ToString());
            _createdDocumentID = resp.document_id;
            Test(resp != null);
            _addDocumentTested = true;
        }

        private void OnGetDocument(DocumentStatus resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnGetDocument()", "Discovery - Get document Response: {0}", customData["json"].ToString());
            Test(resp != null);
            _getDocumentTested = true;
        }

        private void OnUpdateDocument(DocumentAccepted resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnUpdateDocument()", "Discovery - Update document Response: {0}", customData["json"].ToString());
            Test(resp != null);
            _updateDocumentTested = true;
        }

        private void OnDeleteDocument(DeleteDocumentResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnDeleteDocument()", "Discovery - Delete document Response: deleted:{0}", customData["json"].ToString());

            _createdDocumentID = default(string);
            Test(resp != null);

            _deleteDocumentTested = true;
        }

        private void OnDeleteCollection(DeleteCollectionResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnDeleteCollection()", "Discovery - Delete collection Response: deleted:{0}", customData["json"].ToString());

            _createdCollectionId = default(string);
            Test(resp != null);

            _deleteCollectionTested = true;
        }

        private void OnDeleteConfiguration(DeleteConfigurationResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnDeleteConfiguration()", "Discovery - Delete configuration Response: deleted:{0}", customData["json"].ToString());

            _createdConfigurationID = default(string);
            Test(resp != null);

            _deleteConfigurationTested = true;
        }

        private void OnQuery(QueryResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnQuery()", "Discovery - Query Response: {0}", customData["json"].ToString());
            _sessionToken = resp.SessionToken;
            Test(resp != null);
            _queryTested = true;
        }

        private void OnDeleteUserData(object response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnDeleteUserData()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _deleteUserDataTested = true;
        }

        private void OnListCredentials(CredentialsList response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnListCredentials()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _listCredentialsTested = true;
        }
        private void OnCreateCredentials(SourceCredentials response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnCreateCredentials()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _createdCredentialId = response.CredentialId;
            _createCredentialsTested = true;
        }

        private void OnGetCredential(SourceCredentials response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnGetCredential()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _getCredentialTested = true;
        }

        private void OnDeleteCredentials(DeleteCredentials response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnDeleteCredentials()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _deleteCredentialsTested = true;
        }
        private void OnCreateEvent(CreateEventResponse response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnCreateEvent()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _createEventTested = true;
        }

        private void OnGetMetricsEventRate(MetricResponse response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnGetMetricsEventRate()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _getMetricsEventRateTested = true;
        }

        private void OnGetMetricsQuery(MetricResponse response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnGetMetricsQuery()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _getMetricsQueryTested = true;
        }

        private void OnGetMetricsQueryEvent(MetricResponse response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnGetMetricsQueryEvent()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _getMetricsQueryEventTested = true;
        }

        private void OnGetMetricsQueryNoResult(MetricResponse response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnGetMetricsQueryNoResult()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _getMetricsQueryNoResultTested = true;
        }

        private void OnGetMetricsQueryTokenEvent(MetricTokenResponse response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnGetMetricsQueryTokenEvent()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _getMetricsQueryTokenEventTested = true;
        }

        private void OnQueryLog(LogQueryResponse response, Dictionary<string, object> customData)
        {
            Log.Debug("TestDiscovery.OnQueryLog()", "Response: {0}", customData["json"].ToString());
            _queryLogTested = true;
        }

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("TestDiscovery.OnFail()", "Error received: {0}", error.ToString());
        }

        private void OnCreateTokenizationDictionaryFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("TestDiscovery.OnCreateTokenizationDictionaryFail()", "Error received - Continuing without testing tokenization dictionaries: {0}", error.ToString());
            _createTokenizationDictTested = true;
            _getTokenizationDictStatusTested = true;
            _deleteTokenizationDictTested = true;
        }
    }
}
