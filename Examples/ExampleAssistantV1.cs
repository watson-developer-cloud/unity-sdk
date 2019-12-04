/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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
using UnityEngine;
using IBM.Watson.Assistant.V1;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.Assistant.V1.Model;
using System;

namespace IBM.Watson.Examples
{
    public class ExampleAssistantV1 : MonoBehaviour
    {
        private AssistantService service;
        private string workspaceId;
        private string createdWorkspaceId;
        private string inputString = "Hello";
        private string conversationString0 = "unlock the door";
        private string conversationString1 = "turn on the ac";
        private string conversationString2 = "turn down the radio";

        private static string createdWorkspaceName = "unity-sdk-example-workspace-delete";
        private static string createdWorkspaceDescription = "A Workspace created by the Unity SDK Assistant example script. Please delete this.";
        private static string createdWorkspaceLanguage = "en";
        private static string createdEntity = "unityEntity";
        private static string createdEntityDescription = "Entity created by the Unity SDK Assistant example script.";
        private static string createdValue = "unityunityalue";
        private static string createdIntent = "unityIntent";
        private static string createdIntentDescription = "Intent created by the Unity SDK Assistant example script.";
        private static string createdCounterExampleText = "unityExample text";
        private static string createdSynonym = "unitySynonym";
        private static string createdExample = "unityExample";
        private static string dialogNodeName = "unityDialognode";
        private static string dialogNodeDesc = "Unity SDK Integration test dialog node";
        private Dictionary<string, object> context = null;

        private bool listWorkspacesTested = false;
        private bool createWorkspaceTested = false;
        private bool getWorkspaceTested = false;
        private bool updateWorkspaceTested = false;
        private bool messageTested = false;
        private bool listIntentsTested = false;
        private bool createIntentTested = false;
        private bool getIntentTested = false;
        private bool updateIntentTested = false;
        private bool listExamplesTested = false;
        private bool createExampleTested = false;
        private bool getExampleTested = false;
        private bool updateExampleTested = false;
        private bool listEntitiesTested = false;
        private bool createEntityTested = false;
        private bool getEntityTested = false;
        private bool updateEntityTested = false;
        private bool listMentionsTested = false;
        private bool listValuesTested = false;
        private bool createValueTested = false;
        private bool getValueTested = false;
        private bool updateValueTested = false;
        private bool listSynonymsTested = false;
        private bool createSynonymTested = false;
        private bool getSynonymTested = false;
        private bool updateSynonymTested = false;
        private bool listDialogNodesTested = false;
        private bool createDialogNodeTested = false;
        private bool getDialogNodeTested = false;
        private bool updateDialogNodeTested = false;
        private bool listLogsInWorkspaceTested = false;
        private bool listAllLogsTested = false;
        private bool listCounterexamplesTested = false;
        private bool createCounterexampleTested = false;
        private bool getCounterexampleTested = false;
        private bool updateCounterexampleTested = false;
        private bool deleteCounterexampleTested = false;
        private bool deleteDialogNodeTested = false;
        private bool deleteSynonymTested = false;
        private bool deleteValueTested = false;
        private bool deleteEntityTested = false;
        private bool deleteExampleTested = false;
        private bool deleteIntentTested = false;
        private bool deleteWorkspaceTested = false;

        private void Start()
        {
            LogSystem.InstallDefaultReactors();
            Runnable.Run(CreateService());
        }

        private IEnumerator CreateService()
        {

            IamAuthenticator authenticator = new IamAuthenticator(apikey: "{iamApikey}");

            //  Wait for tokendata
            while (!authenticator.CanAuthenticate())
                yield return null;

            service = new AssistantService("2019-02-18", authenticator);
            service.SetServiceUrl("{serviceUrl}");

            workspaceId = Environment.GetEnvironmentVariable("CONVERSATION_WORKSPACE_ID");
            Runnable.Run(Examples());
        }

        private IEnumerator Examples()
        {
            //  List Workspaces
            Log.Debug("ExampleAssistantV1", "Attempting to ListWorkspaces...");
            service.ListWorkspaces(callback: OnListWorkspaces, pageLimit: 1, sort: "-name", includeAudit: true);
            while (!listWorkspacesTested)
                yield return null;

            Log.Debug("ExampleAssistantV1", "Attempting to CreateWorkspace...");
            service.CreateWorkspace(callback: OnCreateWorkspace, name: createdWorkspaceName, description: createdWorkspaceDescription, language: createdWorkspaceLanguage, learningOptOut: true);
            while (!createWorkspaceTested)
                yield return null;

            //  Get Workspace
            Log.Debug("ExampleAssistantV1", "Attempting to GetWorkspace...");
            service.GetWorkspace(callback: OnGetWorkspace, workspaceId: createdWorkspaceId);
            while (!getWorkspaceTested)
                yield return null;

            Log.Debug("ExampleAssistantV1", "Attempting to UpdateWorkspace...");
            service.UpdateWorkspace(callback: OnUpdateWorkspace, workspaceId: createdWorkspaceId, name: createdWorkspaceName + "Updated", description: createdWorkspaceDescription + "Updated", language: createdWorkspaceLanguage);
            while (!updateWorkspaceTested)
                yield return null;

            //  Message
            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("text", inputString);
            //InputData input = new InputData()
            //{
            //    Text = inputString
            //};
            //Log.Debug("ExampleAssistantV1", "Attempting to Message...");
            //service.Message(callback: OnMessage, workspaceId: workspaceId, input: input);
            //while (!messageTested)
            //    yield return null;
            //messageTested = false;

            //input = new InputData()
            //{
            //    Text = conversationString0
            //};
            //Log.Debug("ExampleAssistantV1", "Attempting to Message...");
            //service.Message(callback: OnMessage, workspaceId: workspaceId, input: input, context: context);
            //while (!messageTested)
            //    yield return null;
            //messageTested = false;

            //input = new InputData()
            //{
            //    Text = conversationString1
            //};
            //Log.Debug("ExampleAssistantV1", "Attempting to Message...");
            //service.Message(callback: OnMessage, workspaceId: workspaceId, input: input, context: context);
            //while (!messageTested)
            //    yield return null;
            //messageTested = false;

            //input = new InputData()
            //{
            //    Text = conversationString2
            //};
            //Log.Debug("ExampleAssistantV1", "Attempting to Message...");
            //service.Message(callback: OnMessage, workspaceId: workspaceId, input: input, context: context);
            //while (!messageTested)
            //    yield return null;

            //  List Intents
            Log.Debug("ExampleAssistantV1", "Attempting to ListIntents...");
            service.ListIntents(callback: OnListIntents, workspaceId: createdWorkspaceId);
            while (!listIntentsTested)
                yield return null;

            ////  Create Intent
            Log.Debug("ExampleAssistantV1", "Attempting to CreateIntent...");
            service.CreateIntent(callback: OnCreateIntent, workspaceId: createdWorkspaceId, intent: createdIntent, description: createdIntentDescription);
            while (!createIntentTested)
                yield return null;
            //  Get Intent
            service.GetIntent(callback: OnGetIntent, workspaceId: createdWorkspaceId, intent: createdIntent);
            while (!getIntentTested)
                yield return null;
            //  Update Intents
            string updatedIntent = createdIntent + "Updated";
            string updatedIntentDescription = createdIntentDescription + "Updated";
            service.UpdateIntent(callback: OnUpdateIntent, workspaceId: createdWorkspaceId, intent: createdIntent, newIntent: updatedIntent, newDescription: updatedIntentDescription);
            while (!updateIntentTested)
                yield return null;

            //  List Examples
            service.ListExamples(callback: OnListExamples, workspaceId: createdWorkspaceId, intent: updatedIntent);
            while (!listExamplesTested)
                yield return null;
            //  Create Examples
            service.CreateExample(callback: OnCreateExample, workspaceId: createdWorkspaceId, intent: updatedIntent, text: createdExample);
            while (!createExampleTested)
                yield return null;
            //  Get Example
            service.GetExample(callback: OnGetExample, workspaceId: createdWorkspaceId, intent: updatedIntent, text: createdExample);
            while (!getExampleTested)
                yield return null;
            //  Update Examples
            string updatedExample = createdExample + "Updated";
            service.UpdateExample(callback: OnUpdateExample, workspaceId: createdWorkspaceId, intent: updatedIntent, text: createdExample, newText: updatedExample);
            while (!updateExampleTested)
                yield return null;

            //  List Entities
            service.ListEntities(callback: OnListEntities, workspaceId: createdWorkspaceId);
            while (!listEntitiesTested)
                yield return null;
            //  Create Entities
            service.CreateEntity(callback: OnCreateEntity, workspaceId: createdWorkspaceId, entity: createdEntity, description: createdEntityDescription);
            while (!createEntityTested)
                yield return null;
            //  Get Entity
            service.GetEntity(callback: OnGetEntity, workspaceId: createdWorkspaceId, entity: createdEntity);
            while (!getEntityTested)
                yield return null;
            //  Update Entities
            string updatedEntity = createdEntity + "Updated";
            string updatedEntityDescription = createdEntityDescription + "Updated";
            service.UpdateEntity(callback: OnUpdateEntity, workspaceId: createdWorkspaceId, entity: createdEntity, newEntity: updatedEntity, newDescription: updatedEntityDescription);
            while (!updateEntityTested)
                yield return null;

            // List Mentinos
            service.ListMentions(callback: OnListMentions, workspaceId: createdWorkspaceId, entity: updatedEntity);
            while (!listMentionsTested)
                yield return null;

            //  List Values
            service.ListValues(callback: OnListValues, workspaceId: createdWorkspaceId, entity: updatedEntity);
            while (!listValuesTested)
                yield return null;
            //  Create Values
            service.CreateValue(callback: OnCreateValue, workspaceId: createdWorkspaceId, entity: updatedEntity, value: createdValue);
            while (!createValueTested)
                yield return null;
            //  Get Value
            service.GetValue(callback: OnGetValue, workspaceId: createdWorkspaceId, entity: updatedEntity, value: createdValue);
            while (!getValueTested)
                yield return null;
            //  Update Values
            string updatedValue = createdValue + "Updated";
            service.UpdateValue(callback: OnUpdateValue, workspaceId: createdWorkspaceId, entity: updatedEntity, value: createdValue, newValue: updatedValue);
            while (!updateValueTested)
                yield return null;

            //  List Synonyms
            service.ListSynonyms(callback: OnListSynonyms, workspaceId: createdWorkspaceId, entity: updatedEntity, value: updatedValue);
            while (!listSynonymsTested)
                yield return null;
            //  Create Synonyms
            service.CreateSynonym(callback: OnCreateSynonym, workspaceId: createdWorkspaceId, entity: updatedEntity, value: updatedValue, synonym: createdSynonym);
            while (!createSynonymTested)
                yield return null;
            //  Get Synonym
            service.GetSynonym(callback: OnGetSynonym, workspaceId: createdWorkspaceId, entity: updatedEntity, value: updatedValue, synonym: createdSynonym);
            while (!getSynonymTested)
                yield return null;
            //  Update Synonyms
            string updatedSynonym = createdSynonym + "Updated";
            service.UpdateSynonym(callback: OnUpdateSynonym, workspaceId: createdWorkspaceId, entity: updatedEntity, value: updatedValue, synonym: createdSynonym, newSynonym: updatedSynonym);
            while (!updateSynonymTested)
                yield return null;

            //  List Dialog Nodes
            service.ListDialogNodes(callback: OnListDialogNodes, workspaceId: createdWorkspaceId);
            while (!listDialogNodesTested)
                yield return null;
            //  Create Dialog Nodes
            service.CreateDialogNode(callback: OnCreateDialogNode, workspaceId: createdWorkspaceId, dialogNode: dialogNodeName, description: dialogNodeDesc);
            while (!createDialogNodeTested)
                yield return null;
            //  Get Dialog Node
            service.GetDialogNode(callback: OnGetDialogNode, workspaceId: createdWorkspaceId, dialogNode: dialogNodeName);
            while (!getDialogNodeTested)
                yield return null;
            //  Update Dialog Nodes
            string updatedDialogNodeName = dialogNodeName + "Updated";
            string updatedDialogNodeDescription = dialogNodeDesc + "Updated";
            service.UpdateDialogNode(callback: OnUpdateDialogNode, workspaceId: createdWorkspaceId, dialogNode: dialogNodeName, newDialogNode: updatedDialogNodeName, newDescription: updatedDialogNodeDescription);
            while (!updateDialogNodeTested)
                yield return null;

            //  List Logs In Workspace
            service.ListLogs(callback: OnListLogs, workspaceId: createdWorkspaceId);
            while (!listLogsInWorkspaceTested)
                yield return null;
            //  List All Logs
            var filter = "(language::en,request.context.metadata.deployment::deployment_1)";
            service.ListAllLogs(callback: OnListAllLogs, filter: filter);
            while (!listAllLogsTested)
                yield return null;

            //  List Counterexamples
            service.ListCounterexamples(callback: OnListCounterexamples, workspaceId: createdWorkspaceId);
            while (!listCounterexamplesTested)
                yield return null;
            //  Create Counterexamples
            service.CreateCounterexample(callback: OnCreateCounterexample, workspaceId: createdWorkspaceId, text: createdCounterExampleText);
            while (!createCounterexampleTested)
                yield return null;
            //  Get Counterexample
            service.GetCounterexample(callback: OnGetCounterexample, workspaceId: createdWorkspaceId, text: createdCounterExampleText);
            while (!getCounterexampleTested)
                yield return null;
            //  Update Counterexamples
            string updatedCounterExampleText = createdCounterExampleText + "Updated";
            service.UpdateCounterexample(callback: OnUpdateCounterexample, workspaceId: createdWorkspaceId, text: createdCounterExampleText, newText: updatedCounterExampleText);
            while (!updateCounterexampleTested)
                yield return null;

            //  Delete Counterexample
            service.DeleteCounterexample(callback: OnDeleteCounterexample, workspaceId: createdWorkspaceId, text: updatedCounterExampleText);
            while (!deleteCounterexampleTested)
                yield return null;
            //  Delete Dialog Node
            service.DeleteDialogNode(callback: OnDeleteDialogNode, workspaceId: createdWorkspaceId, dialogNode:  updatedDialogNodeName);
            while (!deleteDialogNodeTested)
                yield return null;
            //  Delete Synonym
            service.DeleteSynonym(callback: OnDeleteSynonym, workspaceId: createdWorkspaceId, entity: updatedEntity, value: updatedValue, synonym: updatedSynonym);
            while (!deleteSynonymTested)
                yield return null;
            //  Delete Value
            service.DeleteValue(callback: OnDeleteValue, workspaceId: createdWorkspaceId, entity: updatedEntity, value: updatedValue);
            while (!deleteValueTested)
                yield return null;
            //  Delete Entity
            service.DeleteEntity(callback: OnDeleteEntity, workspaceId: createdWorkspaceId, entity: updatedEntity);
            while (!deleteEntityTested)
                yield return null;
            //  Delete Example
            service.DeleteExample(callback: OnDeleteExample, workspaceId: createdWorkspaceId, intent: updatedIntent, text: updatedExample);
            while (!deleteExampleTested)
                yield return null;
            //  Delete Intent
            service.DeleteIntent(callback: OnDeleteIntent, workspaceId: createdWorkspaceId, intent: updatedIntent);
            while (!deleteIntentTested)
                yield return null;
            //  Delete Workspace
            service.DeleteWorkspace(callback: OnDeleteWorkspace, workspaceId: createdWorkspaceId);
            while (!deleteWorkspaceTested)
                yield return null;

            Log.Debug("ExampleAssistantV1.RunTest()", "Assistant examples complete.");

            yield break;
        }

        private void OnListMentions(DetailedResponse<EntityMentionCollection> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnListMentions()", "Response: {0}", response.Response);
            listMentionsTested = true;
        }

        private void OnDeleteWorkspace(DetailedResponse<object> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnDeleteWorkspace()", "Response: {0}", response.Response);
            deleteWorkspaceTested = true;
        }

        private void OnDeleteIntent(DetailedResponse<object> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnDeleteIntent()", "Response: {0}", response.Response);
            deleteIntentTested = true;
        }

        private void OnDeleteExample(DetailedResponse<object> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnDeleteExample()", "Response: {0}", response.Response);
            deleteExampleTested = true;
        }

        private void OnDeleteEntity(DetailedResponse<object> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnDeleteEntity()", "Response: {0}", response.Response);
            deleteEntityTested = true;
        }

        private void OnDeleteValue(DetailedResponse<object> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnDeleteValue()", "Response: {0}", response.Response);
            deleteValueTested = true;
        }

        private void OnDeleteSynonym(DetailedResponse<object> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnDeleteSynonym()", "Response: {0}", response.Response);
            deleteSynonymTested = true;
        }

        private void OnDeleteDialogNode(DetailedResponse<object> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnDeleteDialogNode()", "Response: {0}", response.Response);
            deleteDialogNodeTested = true;
        }

        private void OnDeleteCounterexample(DetailedResponse<object> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnDeleteCounterexample()", "Response: {0}", response.Response);
            deleteCounterexampleTested = true;
        }

        private void OnUpdateCounterexample(DetailedResponse<Counterexample> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnUpdateCounterexample()", "Response: {0}", response.Response);
            updateCounterexampleTested = true;
        }

        private void OnGetCounterexample(DetailedResponse<Counterexample> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnGetCounterexample()", "Response: {0}", response.Response);
            getCounterexampleTested = true;
        }

        private void OnCreateCounterexample(DetailedResponse<Counterexample> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnCreateCounterexample()", "Response: {0}", response.Response);
            createCounterexampleTested = true;
        }

        private void OnListCounterexamples(DetailedResponse<CounterexampleCollection> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnListCounterexamples()", "Response: {0}", response.Response);
            listCounterexamplesTested = true;
        }

        private void OnListAllLogs(DetailedResponse<LogCollection> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnListAllLogs()", "Response: {0}", response.Response);
            listAllLogsTested = true;
        }

        private void OnListLogs(DetailedResponse<LogCollection> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnListLogs()", "Response: {0}", response.Response);
            listLogsInWorkspaceTested = true;
        }

        private void OnUpdateDialogNode(DetailedResponse<DialogNode> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnUpdateDialogNode()", "Response: {0}", response.Response);
            updateDialogNodeTested = true;
        }

        private void OnGetDialogNode(DetailedResponse<DialogNode> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnGetDialogNode()", "Response: {0}", response.Response);
            getDialogNodeTested = true;
        }

        private void OnCreateDialogNode(DetailedResponse<DialogNode> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnCreateDialogNode()", "Response: {0}", response.Response);
            createDialogNodeTested = true;
        }

        private void OnListDialogNodes(DetailedResponse<DialogNodeCollection> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnListDialogNodes()", "Response: {0}", response.Response);
            listDialogNodesTested = true;
        }

        private void OnUpdateSynonym(DetailedResponse<Synonym> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnUpdateSynonym()", "Response: {0}", response.Response);
            updateSynonymTested = true;
        }

        private void OnGetSynonym(DetailedResponse<Synonym> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnGetSynonym()", "Response: {0}", response.Response);
            getSynonymTested = true;
        }

        private void OnCreateSynonym(DetailedResponse<Synonym> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnCreateSynonym()", "Response: {0}", response.Response);
            createSynonymTested = true;
        }

        private void OnListSynonyms(DetailedResponse<SynonymCollection> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnListSynonyms()", "Response: {0}", response.Response);
            listSynonymsTested = true;
        }

        private void OnUpdateValue(DetailedResponse<Value> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnUpdateValue()", "Response: {0}", response.Response);
            updateValueTested = true;
        }

        private void OnGetValue(DetailedResponse<Value> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnGetValue()", "Response: {0}", response.Response);
            getValueTested = true;
        }

        private void OnCreateValue(DetailedResponse<Value> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnCreateValue()", "Response: {0}", response.Response);
            createValueTested = true;
        }

        private void OnListValues(DetailedResponse<ValueCollection> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnListValues()", "Response: {0}", response.Response);
            listValuesTested = true;
        }

        private void OnUpdateEntity(DetailedResponse<Entity> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnUpdateEntity()", "Response: {0}", response.Response);
            updateEntityTested = true;
        }

        private void OnGetEntity(DetailedResponse<Entity> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnGetEntity()", "Response: {0}", response.Response);
            getEntityTested = true;
        }

        private void OnCreateEntity(DetailedResponse<Entity> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnCreateEntity()", "Response: {0}", response.Response);
            createEntityTested = true;
        }

        private void OnListEntities(DetailedResponse<EntityCollection> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnListEntities()", "Response: {0}", response.Response);
            listEntitiesTested = true;
        }

        private void OnUpdateExample(DetailedResponse<Example> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnUpdateExample()", "Response: {0}", response.Response);
            updateExampleTested = true;
        }

        private void OnGetExample(DetailedResponse<Example> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnGetExample()", "Response: {0}", response.Response);
            getExampleTested = true;
        }

        private void OnCreateExample(DetailedResponse<Example> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnCreateExample()", "Response: {0}", response.Response);
            createExampleTested = true;
        }

        private void OnListExamples(DetailedResponse<ExampleCollection> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnListExamples()", "Response: {0}", response.Response);
            listExamplesTested = true;
        }

        private void OnUpdateIntent(DetailedResponse<Intent> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnUpdateIntent()", "Response: {0}", response.Response);
            updateIntentTested = true;
        }

        private void OnGetIntent(DetailedResponse<Intent> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnGetIntent()", "Response: {0}", response.Response);
            getIntentTested = true;
        }

        private void OnCreateIntent(DetailedResponse<Intent> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnCreateIntent()", "Response: {0}", response.Response);
            createIntentTested = true;
        }

        private void OnListIntents(DetailedResponse<IntentCollection> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnListIntents()", "Response: {0}", response.Response);
            listIntentsTested = true;
        }

        private void OnMessage(DetailedResponse<Dictionary<string, object>> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnMessage()", "Response: {0}", response.Response);

            context = response.Result["context"] as Dictionary<string, object>;
            ////  Convert resp to fsdata
            //fsData fsdata = null;
            //fsResult r = _serializer.TrySerialize(response.GetType(), response, out fsdata);
            //if (!r.Succeeded)
            //    throw new WatsonException(r.FormattedMessages);

            ////  Convert fsdata to MessageResponse
            //MessageResponse messageResponse = new MessageResponse();
            //object obj = messageResponse;
            //r = _serializer.TryDeserialize(fsdata, obj.GetType(), ref obj);
            //if (!r.Succeeded)
            //    throw new WatsonException(r.FormattedMessages);

            ////  Set context for next round of messaging
            //object _tempContext = null;
            //(response as Dictionary<string, object>).TryGetValue("context", out _tempContext);

            //if (_tempContext != null)
            //    context = _tempContext as Dictionary<string, object>;
            //else
            //    Log.Debug("ExampleAssistantV1.OnMessage()", "Failed to get context");

            ////  Get intent
            //object tempIntentsObj = null;
            //(response as Dictionary<string, object>).TryGetValue("intents", out tempIntentsObj);
            //object tempIntentObj = (tempIntentsObj as List<object>)[0];
            //object tempIntent = null;
            //(tempIntentObj as Dictionary<string, object>).TryGetValue("intent", out tempIntent);
            //string intent = tempIntent.ToString();

            //Log.Debug("ExampleAssistantV1.OnMessage()", "intent: {0}", intent);

            messageTested = true;
        }

        private void OnUpdateWorkspace(DetailedResponse<Workspace> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnUpdateWorkspace()", "Response: {0}", response.Response);
            updateWorkspaceTested = true;
        }

        private void OnGetWorkspace(DetailedResponse<Workspace> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnGetWorkspace()", "Response: {0}", response.Response);
            getWorkspaceTested = true;
        }

        private void OnCreateWorkspace(DetailedResponse<Workspace> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnCreateWorkspace()", "Response: {0}", response.Response);
            createdWorkspaceId = response.Result.WorkspaceId;
            createWorkspaceTested = true;
        }

        private void OnListWorkspaces(DetailedResponse<WorkspaceCollection> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV1.OnListWorkspaces()", "Response: {0}", response.Response);

            foreach (Workspace workspace in response.Result.Workspaces)
            {
                if (workspace.Name.Contains("unity"))
                    service.DeleteWorkspace(callback: OnDeleteWorkspace, workspaceId: workspace.WorkspaceId);
            }

            listWorkspacesTested = true;
        }

    }
}
