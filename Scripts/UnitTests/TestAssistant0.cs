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

using System.Collections;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.UnitTests;
using IBM.Watson.DeveloperCloud.Services.Assistant.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using FullSerializer;
using IBM.Watson.DeveloperCloud.Connection;
using System.IO;

namespace Assets.Watson.Scripts.UnitTests
{
    class TestAssistant0 : UnitTest
    {
        private string _username = null;
        private string _password = null;
        private string _workspaceId = null;
        private string _createdWorkspaceId;

        private Assistant _service;
        private string _assistantVersionDate = "2017-05-26";

        private fsSerializer _serializer = new fsSerializer();

        private string _inputString = "Hello";
        private string _conversationString0 = "unlock the door";
        private string _conversationString1 = "turn on the ac";
        private string _conversationString2 = "turn down the radio";
        private static string _lastIntent = null;

        private static string _createdWorkspaceName = "unity-sdk-example-workspace-delete";
        private static string _createdWorkspaceDescription = "A Workspace created by the Unity SDK Assistant example script. Please delete this.";
        private static string _createdWorkspaceLanguage = "en";
        private static string _createdEntity = "untiyEntity";
        private static string _createdEntityDescription = "Entity created by the Unity SDK Assistant example script.";
        private static string _createdValue = "untiyuntiyalue";
        private static string _createdIntent = "untiyIntent";
        private static string _createdIntentDescription = "Intent created by the Unity SDK Assistant example script.";
        private static string _createdSynonym = "untiySynonym";
        private static string _createdExample = "untiyExample";
        private static Dictionary<string, object> _context = null;

        private bool _listWorkspacesTested = false;
        private bool _createWorkspaceTested = false;
        private bool _getWorkspaceTested = false;
        private bool _updateWorkspaceTested = false;
        private bool _messageTested = false;
        private bool _listIntentsTested = false;
        private bool _createIntentTested = false;
        private bool _getIntentTested = false;
        private bool _updateIntentTested = false;
        private bool _listExamplesTested = false;
        private bool _createExampleTested = false;
        private bool _getExampleTested = false;
        private bool _updateExampleTested = false;
        private bool _listEntitiesTested = false;
        private bool _createEntityTested = false;
        private bool _getEntityTested = false;
        private bool _updateEntityTested = false;
        private bool _listValuesTested = false;
        private bool _createValueTested = false;
        private bool _getValueTested = false;
        private bool _updateValueTested = false;
        private bool _listSynonymsTested = false;
        private bool _createSynonymTested = false;
        private bool _getSynonymTested = false;
        private bool _updateSynonymTested = false;

        private bool _listLogsInWorkspaceTested = false;
        private bool _listAllLogsTested = false;
        
        private bool _deleteSynonymTested = false;
        private bool _deleteValueTested = false;
        private bool _deleteEntityTested = false;
        private bool _deleteExampleTested = false;
        private bool _deleteIntentTested = false;
        private bool _deleteWorkspaceTested = false;

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
            Credential credential = vcapCredentials.GetCredentialByname("assistant-sdk")[0].Credentials;
            _username = credential.Username.ToString();
            _password = credential.Password.ToString();
            _url = credential.Url.ToString();
            _workspaceId = credential.WorkspaceId.ToString();

            //  Create credential and instantiate service
            Credentials credentials = new Credentials(_username, _password, _url);

            _service = new Assistant(credentials);
            _service.VersionDate = _assistantVersionDate;

            //  List Workspaces
            _service.ListWorkspaces(OnListWorkspaces, OnFail);
            while (!_listWorkspacesTested)
                yield return null;
            //  Create Workspace
            CreateWorkspace workspace = new CreateWorkspace()
            {
                Name = _createdWorkspaceName,
                Description = _createdWorkspaceDescription,
                Language = _createdWorkspaceLanguage,
                LearningOptOut = true
            };
            _service.CreateWorkspace(OnCreateWorkspace, OnFail, workspace);
            while (!_createWorkspaceTested)
                yield return null;
            //  Get Workspace
            _service.GetWorkspace(OnGetWorkspace, OnFail, _createdWorkspaceId);
            while (!_getWorkspaceTested)
                yield return null;
            //  Update Workspace
            UpdateWorkspace updateWorkspace = new UpdateWorkspace()
            {
                Name = _createdWorkspaceName + "-updated",
                Description = _createdWorkspaceDescription + "-updated",
                Language = _createdWorkspaceLanguage
            };
            _service.UpdateWorkspace(OnUpdateWorkspace, OnFail, _createdWorkspaceId, updateWorkspace);
            while (!_updateWorkspaceTested)
                yield return null;

            //  Message
            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("text", _inputString);
            MessageRequest messageRequest = new MessageRequest()
            {
                Input = input
            };
            _service.Message(OnMessage, OnFail, _workspaceId, messageRequest);
            while (!_messageTested)
                yield return null;
            _messageTested = false;

            input["text"] = _conversationString0;
            MessageRequest messageRequest0 = new MessageRequest()
            {
                Input = input,
                Context = _context
            };
            _service.Message(OnMessage, OnFail, _workspaceId, messageRequest0);
            while (!_messageTested)
                yield return null;
            _messageTested = false;

            input["text"] = _conversationString1;
            MessageRequest messageRequest1 = new MessageRequest()
            {
                Input = input,
                Context = _context
            };
            _service.Message(OnMessage, OnFail, _workspaceId, messageRequest1);
            while (!_messageTested)
                yield return null;
            _messageTested = false;

            input["text"] = _conversationString2;
            MessageRequest messageRequest2 = new MessageRequest()
            {
                Input = input,
                Context = _context
            };
            _service.Message(OnMessage, OnFail, _workspaceId, messageRequest2);
            while (!_messageTested)
                yield return null;

            //  List Intents
            _service.ListIntents(OnListIntents, OnFail, _createdWorkspaceId);
            while (!_listIntentsTested)
                yield return null;
            //  Create Intent
            CreateIntent createIntent = new CreateIntent()
            {
                Intent = _createdIntent,
                Description = _createdIntentDescription
            };
            _service.CreateIntent(OnCreateIntent, OnFail, _createdWorkspaceId, createIntent);
            while (!_createIntentTested)
                yield return null;
            //  Get Intent
            _service.GetIntent(OnGetIntent, OnFail, _createdWorkspaceId, _createdIntent);
            while (!_getIntentTested)
                yield return null;
            //  Update Intents
            string updatedIntent = _createdIntent + "-updated";
            string updatedIntentDescription = _createdIntentDescription + "-updated";
            UpdateIntent updateIntent = new UpdateIntent()
            {
                Intent = updatedIntent,
                Description = updatedIntentDescription
            };
            _service.UpdateIntent(OnUpdateIntent, OnFail, _createdWorkspaceId, _createdIntent, updateIntent);
            while (!_updateIntentTested)
                yield return null;

            //  List Examples
            _service.ListExamples(OnListExamples, OnFail, _createdWorkspaceId, updatedIntent);
            while (!_listExamplesTested)
                yield return null;
            //  Create Examples
            CreateExample createExample = new CreateExample()
            {
                Text = _createdExample
            };
            _service.CreateExample(OnCreateExample, OnFail, _createdWorkspaceId, updatedIntent, createExample);
            while (!_createExampleTested)
                yield return null;
            //  Get Example
            _service.GetExample(OnGetExample, OnFail, _createdWorkspaceId, updatedIntent, _createdExample);
            while (!_getExampleTested)
                yield return null;
            //  Update Examples
            string updatedExample = _createdExample + "-updated";
            UpdateExample updateExample = new UpdateExample()
            {
                Text = updatedExample
            };
            _service.UpdateExample(OnUpdateExample, OnFail, _createdWorkspaceId, updatedIntent, _createdExample, updateExample);
            while (!_updateExampleTested)
                yield return null;

            //  List Entities
            _service.ListEntities(OnListEntities, OnFail, _createdWorkspaceId);
            while (!_listEntitiesTested)
                yield return null;
            //  Create Entities
            CreateEntity entity = new CreateEntity()
            {
                Entity = _createdEntity,
                Description = _createdEntityDescription
            };
            _service.CreateEntity(OnCreateEntity, OnFail, _createdWorkspaceId, entity);
            while (!_createEntityTested)
                yield return null;
            //  Get Entity
            _service.GetEntity(OnGetEntity, OnFail, _createdWorkspaceId, _createdEntity);
            while (!_getEntityTested)
                yield return null;
            //  Update Entities
            string updatedEntity = _createdEntity + "-updated";
            string updatedEntityDescription = _createdEntityDescription + "-updated";
            UpdateEntity updateEntity = new UpdateEntity()
            {
                Entity = updatedEntity,
                Description = updatedEntityDescription
            };
            _service.UpdateEntity(OnUpdateEntity, OnFail, _createdWorkspaceId, _createdEntity, updateEntity);
            while (!_updateEntityTested)
                yield return null;

            //  List Values
            _service.ListValues(OnListValues, OnFail, _createdWorkspaceId, updatedEntity);
            while (!_listValuesTested)
                yield return null;
            //  Create Values
            CreateValue value = new CreateValue()
            {
                Value = _createdValue
            };
            _service.CreateValue(OnCreateValue, OnFail, _createdWorkspaceId, updatedEntity, value);
            while (!_createValueTested)
                yield return null;
            //  Get Value
            _service.GetValue(OnGetValue, OnFail, _createdWorkspaceId, updatedEntity, _createdValue);
            while (!_getValueTested)
                yield return null;
            //  Update Values
            string updatedValue = _createdValue + "-updated";
            UpdateValue updateValue = new UpdateValue()
            {
                Value = updatedValue
            };
            _service.UpdateValue(OnUpdateValue, OnFail, _createdWorkspaceId, updatedEntity, _createdValue, updateValue);
            while (!_updateValueTested)
                yield return null;

            //  List Synonyms
            _service.ListSynonyms(OnListSynonyms, OnFail, _createdWorkspaceId, updatedEntity, updatedValue);
            while (!_listSynonymsTested)
                yield return null;
            //  Create Synonyms
            CreateSynonym synonym = new CreateSynonym()
            {
                Synonym = _createdSynonym
            };
            _service.CreateSynonym(OnCreateSynonym, OnFail, _createdWorkspaceId, updatedEntity, updatedValue, synonym);
            while (!_createSynonymTested)
                yield return null;
            //  Get Synonym
            _service.GetSynonym(OnGetSynonym, OnFail, _createdWorkspaceId, updatedEntity, updatedValue, _createdSynonym);
            while (!_getSynonymTested)
                yield return null;
            //  Update Synonyms
            string updatedSynonym = _createdSynonym + "-updated";
            UpdateSynonym updateSynonym = new UpdateSynonym()
            {
                Synonym = updatedSynonym
            };
            _service.UpdateSynonym(OnUpdateSynonym, OnFail, _createdWorkspaceId, updatedEntity, updatedValue, _createdSynonym, updateSynonym);
            while (!_updateSynonymTested)
                yield return null;
            
            //  List Logs In Workspace
            _service.ListLogs(OnListLogs, OnFail, _createdWorkspaceId);
            while (!_listLogsInWorkspaceTested)
                yield return null;
            ////  List All Logs
            //var filter = "(language::en,request.context.metadata.deployment::deployment_1)";
            //_service.ListAllLogs(OnListAllLogs, OnFail, filter);
            //while (!_listAllLogsTested)
            //    yield return null;
            
            //  Delete Synonym
            _service.DeleteSynonym(OnDeleteSynonym, OnFail, _createdWorkspaceId, updatedEntity, updatedValue, updatedSynonym);
            while (!_deleteSynonymTested)
                yield return null;
            //  Delete Value
            _service.DeleteValue(OnDeleteValue, OnFail, _createdWorkspaceId, updatedEntity, updatedValue);
            while (!_deleteValueTested)
                yield return null;
            //  Delete Entity
            _service.DeleteEntity(OnDeleteEntity, OnFail, _createdWorkspaceId, updatedEntity);
            while (!_deleteEntityTested)
                yield return null;
            //  Delete Example
            _service.DeleteExample(OnDeleteExample, OnFail, _createdWorkspaceId, updatedIntent, updatedExample);
            while (!_deleteExampleTested)
                yield return null;
            //  Delete Intent
            _service.DeleteIntent(OnDeleteIntent, OnFail, _createdWorkspaceId, updatedIntent);
            while (!_deleteIntentTested)
                yield return null;
            //  Delete Workspace
            _service.DeleteWorkspace(OnDeleteWorkspace, OnFail, _createdWorkspaceId);
            while (!_deleteWorkspaceTested)
                yield return null;

            Log.Debug("TestAssistant.RunTest()", "Assistant examples complete.");

            yield break;
        }

        private void OnDeleteWorkspace(object response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnDeleteWorkspace()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _deleteWorkspaceTested = true;
        }

        private void OnDeleteIntent(object response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnDeleteIntent()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _deleteIntentTested = true;
        }

        private void OnDeleteExample(object response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnDeleteExample()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _deleteExampleTested = true;
        }

        private void OnDeleteEntity(object response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnDeleteEntity()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _deleteEntityTested = true;
        }

        private void OnDeleteValue(object response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnDeleteValue()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _deleteValueTested = true;
        }

        private void OnDeleteSynonym(object response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnDeleteSynonym()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _deleteSynonymTested = true;
        }
        
        private void OnListAllLogs(LogCollection response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnListAllLogs()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _listAllLogsTested = true;
        }

        private void OnListLogs(LogCollection response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnListLogs()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _listLogsInWorkspaceTested = true;
        }
        
        private void OnUpdateSynonym(Synonym response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnUpdateSynonym()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _updateSynonymTested = true;
        }

        private void OnGetSynonym(Synonym response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnGetSynonym()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _getSynonymTested = true;
        }

        private void OnCreateSynonym(Synonym response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnCreateSynonym()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _createSynonymTested = true;
        }

        private void OnListSynonyms(SynonymCollection response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnListSynonyms()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _listSynonymsTested = true;
        }

        private void OnUpdateValue(Value response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnUpdateValue()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _updateValueTested = true;
        }

        private void OnGetValue(ValueExport response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnGetValue()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _getValueTested = true;
        }

        private void OnCreateValue(Value response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnCreateValue()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _createValueTested = true;
        }

        private void OnListValues(ValueCollection response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnListValues()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _listValuesTested = true;
        }

        private void OnUpdateEntity(Entity response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnUpdateEntity()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _updateEntityTested = true;
        }

        private void OnGetEntity(EntityExport response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnGetEntity()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _getEntityTested = true;
        }

        private void OnCreateEntity(Entity response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnCreateEntity()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _createEntityTested = true;
        }

        private void OnListEntities(EntityCollection response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnListEntities()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _listEntitiesTested = true;
        }

        private void OnUpdateExample(Example response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnUpdateExample()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _updateExampleTested = true;
        }

        private void OnGetExample(Example response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnGetExample()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _getExampleTested = true;
        }

        private void OnCreateExample(Example response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnCreateExample()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _createExampleTested = true;
        }

        private void OnListExamples(ExampleCollection response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnListExamples()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _listExamplesTested = true;
        }

        private void OnUpdateIntent(Intent response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnUpdateIntent()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _updateIntentTested = true;
        }

        private void OnGetIntent(IntentExport response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnGetIntent()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _getIntentTested = true;
        }

        private void OnCreateIntent(Intent response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnCreateIntent()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _createIntentTested = true;
        }

        private void OnListIntents(IntentCollection response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnListIntents()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _listIntentsTested = true;
        }

        private void OnMessage(object response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnMessage()", "Response: {0}", customData["json"].ToString());

            //  Convert resp to fsdata
            fsData fsdata = null;
            fsResult r = _serializer.TrySerialize(response.GetType(), response, out fsdata);
            if (!r.Succeeded)
                throw new WatsonException(r.FormattedMessages);

            //  Convert fsdata to MessageResponse
            MessageResponse messageResponse = new MessageResponse();
            object obj = messageResponse;
            r = _serializer.TryDeserialize(fsdata, obj.GetType(), ref obj);
            if (!r.Succeeded)
                throw new WatsonException(r.FormattedMessages);

            //  Set context for next round of messaging
            object tempContext = null;
            (response as Dictionary<string, object>).TryGetValue("context", out tempContext);

            if (tempContext != null)
                _context = tempContext as Dictionary<string, object>;
            else
                Log.Debug("ExampleConversation.OnMessage()", "Failed to get context");

            //  Get intent
            object tempIntentsObj = null;
            (response as Dictionary<string, object>).TryGetValue("intents", out tempIntentsObj);
            object tempIntentObj = (tempIntentsObj as List<object>)[0];
            object tempIntent = null;
            (tempIntentObj as Dictionary<string, object>).TryGetValue("intent", out tempIntent);
            string intent = tempIntent.ToString();

            Test(_lastIntent != intent);
            _lastIntent = intent;

            //messageResponse.Intents != _lastIntent;
            //_lastIntent = messageResponse.Intents;

            _messageTested = true;
        }

        private void OnUpdateWorkspace(Workspace response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnUpdateWorkspace()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _updateWorkspaceTested = true;
        }

        private void OnGetWorkspace(WorkspaceExport response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnGetWorkspace()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _getWorkspaceTested = true;
        }

        private void OnCreateWorkspace(Workspace response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnCreateWorkspace()", "Response: {0}", customData["json"].ToString());
            _createdWorkspaceId = response.WorkspaceId;
            Test(response != null);
            _createWorkspaceTested = true;
        }

        private void OnListWorkspaces(WorkspaceCollection response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnListWorkspaces()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _listWorkspacesTested = true;
        }

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("TestAssistant.OnFail()", "Error received: {0}", error.ToString());
        }
    }
}
