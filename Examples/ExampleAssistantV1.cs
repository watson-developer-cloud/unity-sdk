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
        private static string createdEntity = "untiyEntity";
        private static string createdEntityDescription = "Entity created by the Unity SDK Assistant example script.";
        private static string createdValue = "untiyuntiyalue";
        private static string createdIntent = "untiyIntent";
        private static string createdIntentDescription = "Intent created by the Unity SDK Assistant example script.";
        private static string createdCounterExampleText = "untiyExample text";
        private static string createdSynonym = "untiySynonym";
        private static string createdExample = "untiyExample";
        private static string dialogNodeName = "untiyDialognode";
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
            service = new AssistantService("2019-02-18");

            //  Wait for authorization token
            while (!service.Credentials.HasIamTokenData())
                yield return null;

            workspaceId = Environment.GetEnvironmentVariable("CONVERSATION_WORKSPACE_ID");
            Runnable.Run(Examples());
        }

        private IEnumerator Examples()
        {
            //  List Workspaces
            Log.Debug("ExampleAssistantV1", "Attempting to ListWorkspaces...");
            service.ListWorkspaces(callback: OnListWorkspaces, pageLimit: 1, includeCount: true, sort: "-name", includeAudit: true);
            while (!listWorkspacesTested)
                yield return null;

            //  Create Workspace
            CreateWorkspace workspace = new CreateWorkspace()
            {
                Name = createdWorkspaceName,
                Description = createdWorkspaceDescription,
                Language = createdWorkspaceLanguage,
                LearningOptOut = true
            };
            Log.Debug("ExampleAssistantV1", "Attempting to CreateWorkspace...");
            service.CreateWorkspace(OnCreateWorkspace, workspace);
            while (!createWorkspaceTested)
                yield return null;

            //  Get Workspace
            Log.Debug("ExampleAssistantV1", "Attempting to GetWorkspace...");
            service.GetWorkspace(OnGetWorkspace, createdWorkspaceId);
            while (!getWorkspaceTested)
                yield return null;

            UpdateWorkspace updateWorkspace = new UpdateWorkspace()
            {
                Name = createdWorkspaceName + "Updated",
            //  Update Workspace
                Description = createdWorkspaceDescription + "Updated",
                Language = createdWorkspaceLanguage
            };
            Log.Debug("ExampleAssistantV1", "Attempting to UpdateWorkspace...");
            service.UpdateWorkspace(OnUpdateWorkspace, createdWorkspaceId, updateWorkspace);
            while (!updateWorkspaceTested)
                yield return null;

            //  Message
            //Dictionary<string, object> input = new Dictionary<string, object>();
            //input.Add("text", inputString);
            //MessageRequest messageRequest = new MessageRequest()
            //{
            //    Input = input
            //};
            //Log.Debug("ExampleAssistantV1", "Attempting to Message...");
            //service.Message(OnMessage, workspaceId, messageRequest);
            //while (!messageTested)
            //    yield return null;
            //messageTested = false;

            //input["text"] = conversationString0;
            //MessageRequest messageRequest0 = new MessageRequest()
            //{
            //    Input = input,
            //    Context = context
            //};
            //Log.Debug("ExampleAssistantV1", "Attempting to Message...");
            //service.Message(OnMessage, workspaceId, messageRequest0);
            //while (!messageTested)
            //    yield return null;
            //messageTested = false;

            //input["text"] = conversationString1;
            //MessageRequest messageRequest1 = new MessageRequest()
            //{
            //    Input = input,
            //    Context = context
            //};
            //Log.Debug("ExampleAssistantV1", "Attempting to Message...");
            //service.Message(OnMessage, workspaceId, messageRequest1);
            //while (!messageTested)
            //    yield return null;
            //messageTested = false;

            //input["text"] = conversationString2;
            //MessageRequest messageRequest2 = new MessageRequest()
            //{
            //    Input = input,
            //    Context = context
            //};
            //Log.Debug("ExampleAssistantV1", "Attempting to Message...");
            //service.Message(OnMessage, workspaceId, messageRequest2);
            //while (!messageTested)
            //    yield return null;

            //  List Intents
            Log.Debug("ExampleAssistantV1", "Attempting to ListIntents...");
            service.ListIntents(OnListIntents, createdWorkspaceId);
            while (!listIntentsTested)
                yield return null;

            //  Create Intent
            CreateIntent createIntent = new CreateIntent()
            {
                Intent = createdIntent,
                Description = createdIntentDescription
            };
            Log.Debug("ExampleAssistantV1", "Attempting to CreateIntent...");
            service.CreateIntent(OnCreateIntent, createdWorkspaceId, createIntent);
            while (!createIntentTested)
                yield return null;
            //  Get Intent
            service.GetIntent(OnGetIntent, createdWorkspaceId, createdIntent);
            while (!getIntentTested)
                yield return null;
            //  Update Intents
            string updatedIntent = createdIntent + "Updated";
            string updatedIntentDescription = createdIntentDescription + "Updated";
            UpdateIntent updateIntent = new UpdateIntent()
            {
                Intent = updatedIntent,
                Description = updatedIntentDescription
            };
            service.UpdateIntent(OnUpdateIntent, createdWorkspaceId, createdIntent, updateIntent);
            while (!updateIntentTested)
                yield return null;

            //  List Examples
            service.ListExamples(OnListExamples, createdWorkspaceId, updatedIntent);
            while (!listExamplesTested)
                yield return null;
            //  Create Examples
            CreateExample createExample = new CreateExample()
            {
                Text = createdExample
            };
            service.CreateExample(OnCreateExample, createdWorkspaceId, updatedIntent, createExample);
            while (!createExampleTested)
                yield return null;
            //  Get Example
            service.GetExample(OnGetExample, createdWorkspaceId, updatedIntent, createdExample);
            while (!getExampleTested)
                yield return null;
            //  Update Examples
            string updatedExample = createdExample + "Updated";
            UpdateExample updateExample = new UpdateExample()
            {
                Text = updatedExample
            };
            service.UpdateExample(OnUpdateExample, createdWorkspaceId, updatedIntent, createdExample, updateExample);
            while (!updateExampleTested)
                yield return null;

            //  List Entities
            service.ListEntities(OnListEntities, createdWorkspaceId);
            while (!listEntitiesTested)
                yield return null;
            //  Create Entities
            CreateEntity entity = new CreateEntity()
            {
                Entity = createdEntity,
                Description = createdEntityDescription
            };
            service.CreateEntity(OnCreateEntity, createdWorkspaceId, entity);
            while (!createEntityTested)
                yield return null;
            //  Get Entity
            service.GetEntity(OnGetEntity, createdWorkspaceId, createdEntity);
            while (!getEntityTested)
                yield return null;
            //  Update Entities
            string updatedEntity = createdEntity + "Updated";
            string updatedEntityDescription = createdEntityDescription + "Updated";
            UpdateEntity updateEntity = new UpdateEntity()
            {
                Entity = updatedEntity,
                Description = updatedEntityDescription
            };
            service.UpdateEntity(OnUpdateEntity, createdWorkspaceId, createdEntity, updateEntity);
            while (!updateEntityTested)
                yield return null;

            // List Mentinos
            service.ListMentions(OnListMentions, createdWorkspaceId, updatedEntity);
            while (!listMentionsTested)
                yield return null;

            //  List Values
            service.ListValues(OnListValues, createdWorkspaceId, updatedEntity);
            while (!listValuesTested)
                yield return null;
            //  Create Values
            CreateValue value = new CreateValue()
            {
                Value = createdValue
            };
            service.CreateValue(OnCreateValue, createdWorkspaceId, updatedEntity, value);
            while (!createValueTested)
                yield return null;
            //  Get Value
            service.GetValue(OnGetValue, createdWorkspaceId, updatedEntity, createdValue);
            while (!getValueTested)
                yield return null;
            //  Update Values
            string updatedValue = createdValue + "Updated";
            UpdateValue updateValue = new UpdateValue()
            {
                Value = updatedValue
            };
            service.UpdateValue(OnUpdateValue, createdWorkspaceId, updatedEntity, createdValue, updateValue);
            while (!updateValueTested)
                yield return null;

            //  List Synonyms
            service.ListSynonyms(OnListSynonyms, createdWorkspaceId, updatedEntity, updatedValue);
            while (!listSynonymsTested)
                yield return null;
            //  Create Synonyms
            CreateSynonym synonym = new CreateSynonym()
            {
                Synonym = createdSynonym
            };
            service.CreateSynonym(OnCreateSynonym, createdWorkspaceId, updatedEntity, updatedValue, synonym);
            while (!createSynonymTested)
                yield return null;
            //  Get Synonym
            service.GetSynonym(OnGetSynonym, createdWorkspaceId, updatedEntity, updatedValue, createdSynonym);
            while (!getSynonymTested)
                yield return null;
            //  Update Synonyms
            string updatedSynonym = createdSynonym + "Updated";
            UpdateSynonym updateSynonym = new UpdateSynonym()
            {
                Synonym = updatedSynonym
            };
            service.UpdateSynonym(OnUpdateSynonym, createdWorkspaceId, updatedEntity, updatedValue, createdSynonym, updateSynonym);
            while (!updateSynonymTested)
                yield return null;

            //  List Dialog Nodes
            service.ListDialogNodes(OnListDialogNodes, createdWorkspaceId);
            while (!listDialogNodesTested)
                yield return null;
            //  Create Dialog Nodes
            CreateDialogNode createDialogNode = new CreateDialogNode()
            {
                DialogNode = dialogNodeName,
                Description = dialogNodeDesc
            };
            service.CreateDialogNode(OnCreateDialogNode, createdWorkspaceId, createDialogNode);
            while (!createDialogNodeTested)
                yield return null;
            //  Get Dialog Node
            service.GetDialogNode(OnGetDialogNode, createdWorkspaceId, dialogNodeName);
            while (!getDialogNodeTested)
                yield return null;
            //  Update Dialog Nodes
            string updatedDialogNodeName = dialogNodeName + "Updated";
            string updatedDialogNodeDescription = dialogNodeDesc + "Updated";
            UpdateDialogNode updateDialogNode = new UpdateDialogNode()
            {
                DialogNode = updatedDialogNodeName,
                Description = updatedDialogNodeDescription
            };
            service.UpdateDialogNode(OnUpdateDialogNode, createdWorkspaceId, dialogNodeName, updateDialogNode);
            while (!updateDialogNodeTested)
                yield return null;

            //  List Logs In Workspace
            service.ListLogs(OnListLogs, createdWorkspaceId);
            while (!listLogsInWorkspaceTested)
                yield return null;
            //  List All Logs
            var filter = "(language::en,request.context.metadata.deployment::deployment_1)";
            service.ListAllLogs(OnListAllLogs, filter);
            while (!listAllLogsTested)
                yield return null;

            //  List Counterexamples
            service.ListCounterexamples(OnListCounterexamples, createdWorkspaceId);
            while (!listCounterexamplesTested)
                yield return null;
            //  Create Counterexamples
            CreateCounterexample example = new CreateCounterexample()
            {
                Text = createdCounterExampleText
            };
            service.CreateCounterexample(OnCreateCounterexample, createdWorkspaceId, example);
            while (!createCounterexampleTested)
                yield return null;
            //  Get Counterexample
            service.GetCounterexample(OnGetCounterexample, createdWorkspaceId, createdCounterExampleText);
            while (!getCounterexampleTested)
                yield return null;
            //  Update Counterexamples
            string updatedCounterExampleText = createdCounterExampleText + "Updated";
            UpdateCounterexample updateCounterExample = new UpdateCounterexample()
            {
                Text = updatedCounterExampleText
            };
            service.UpdateCounterexample(OnUpdateCounterexample, createdWorkspaceId, createdCounterExampleText, updateCounterExample);
            while (!updateCounterexampleTested)
                yield return null;

            //  Delete Counterexample
            service.DeleteCounterexample(OnDeleteCounterexample, createdWorkspaceId, updatedCounterExampleText);
            while (!deleteCounterexampleTested)
                yield return null;
            //  Delete Dialog Node
            service.DeleteDialogNode(OnDeleteDialogNode, createdWorkspaceId, updatedDialogNodeName);
            while (!deleteDialogNodeTested)
                yield return null;
            //  Delete Synonym
            service.DeleteSynonym(OnDeleteSynonym, createdWorkspaceId, updatedEntity, updatedValue, updatedSynonym);
            while (!deleteSynonymTested)
                yield return null;
            //  Delete Value
            service.DeleteValue(OnDeleteValue, createdWorkspaceId, updatedEntity, updatedValue);
            while (!deleteValueTested)
                yield return null;
            //  Delete Entity
            service.DeleteEntity(OnDeleteEntity, createdWorkspaceId, updatedEntity);
            while (!deleteEntityTested)
                yield return null;
            //  Delete Example
            service.DeleteExample(OnDeleteExample, createdWorkspaceId, updatedIntent, updatedExample);
            while (!deleteExampleTested)
                yield return null;
            //  Delete Intent
            service.DeleteIntent(OnDeleteIntent, createdWorkspaceId, updatedIntent);
            while (!deleteIntentTested)
                yield return null;
            //  Delete Workspace
            service.DeleteWorkspace(OnDeleteWorkspace, createdWorkspaceId);
            while (!deleteWorkspaceTested)
                yield return null;

            Log.Debug("ExampleAssistantV1.RunTest()", "Assistant examples complete.");

            yield break;
        }

        private void OnListMentions(WatsonResponse<EntityMentionCollection> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnListMentions()", "Response: {0}", customData["json"].ToString());
            listMentionsTested = true;
        }

        private void OnDeleteWorkspace(WatsonResponse<object> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnDeleteWorkspace()", "Response: {0}", customData["json"].ToString());
            deleteWorkspaceTested = true;
        }

        private void OnDeleteIntent(WatsonResponse<object> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnDeleteIntent()", "Response: {0}", customData["json"].ToString());
            deleteIntentTested = true;
        }

        private void OnDeleteExample(WatsonResponse<object> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnDeleteExample()", "Response: {0}", customData["json"].ToString());
            deleteExampleTested = true;
        }

        private void OnDeleteEntity(WatsonResponse<object> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnDeleteEntity()", "Response: {0}", customData["json"].ToString());
            deleteEntityTested = true;
        }

        private void OnDeleteValue(WatsonResponse<object> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnDeleteValue()", "Response: {0}", customData["json"].ToString());
            deleteValueTested = true;
        }

        private void OnDeleteSynonym(WatsonResponse<object> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnDeleteSynonym()", "Response: {0}", customData["json"].ToString());
            deleteSynonymTested = true;
        }

        private void OnDeleteDialogNode(WatsonResponse<object> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnDeleteDialogNode()", "Response: {0}", customData["json"].ToString());
            deleteDialogNodeTested = true;
        }

        private void OnDeleteCounterexample(WatsonResponse<object> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnDeleteCounterexample()", "Response: {0}", customData["json"].ToString());
            deleteCounterexampleTested = true;
        }

        private void OnUpdateCounterexample(WatsonResponse<Counterexample> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnUpdateCounterexample()", "Response: {0}", customData["json"].ToString());
            updateCounterexampleTested = true;
        }

        private void OnGetCounterexample(WatsonResponse<Counterexample> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnGetCounterexample()", "Response: {0}", customData["json"].ToString());
            getCounterexampleTested = true;
        }

        private void OnCreateCounterexample(WatsonResponse<Counterexample> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnCreateCounterexample()", "Response: {0}", customData["json"].ToString());
            createCounterexampleTested = true;
        }

        private void OnListCounterexamples(WatsonResponse<CounterexampleCollection> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnListCounterexamples()", "Response: {0}", customData["json"].ToString());
            listCounterexamplesTested = true;
        }

        private void OnListAllLogs(WatsonResponse<LogCollection> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnListAllLogs()", "Response: {0}", customData["json"].ToString());
            listAllLogsTested = true;
        }

        private void OnListLogs(WatsonResponse<LogCollection> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnListLogs()", "Response: {0}", customData["json"].ToString());
            listLogsInWorkspaceTested = true;
        }

        private void OnUpdateDialogNode(WatsonResponse<DialogNode> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnUpdateDialogNode()", "Response: {0}", customData["json"].ToString());
            updateDialogNodeTested = true;
        }

        private void OnGetDialogNode(WatsonResponse<DialogNode> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnGetDialogNode()", "Response: {0}", customData["json"].ToString());
            getDialogNodeTested = true;
        }

        private void OnCreateDialogNode(WatsonResponse<DialogNode> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnCreateDialogNode()", "Response: {0}", customData["json"].ToString());
            createDialogNodeTested = true;
        }

        private void OnListDialogNodes(WatsonResponse<DialogNodeCollection> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnListDialogNodes()", "Response: {0}", customData["json"].ToString());
            listDialogNodesTested = true;
        }

        private void OnUpdateSynonym(WatsonResponse<Synonym> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnUpdateSynonym()", "Response: {0}", customData["json"].ToString());
            updateSynonymTested = true;
        }

        private void OnGetSynonym(WatsonResponse<Synonym> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnGetSynonym()", "Response: {0}", customData["json"].ToString());
            getSynonymTested = true;
        }

        private void OnCreateSynonym(WatsonResponse<Synonym> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnCreateSynonym()", "Response: {0}", customData["json"].ToString());
            createSynonymTested = true;
        }

        private void OnListSynonyms(WatsonResponse<SynonymCollection> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnListSynonyms()", "Response: {0}", customData["json"].ToString());
            listSynonymsTested = true;
        }

        private void OnUpdateValue(WatsonResponse<Value> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnUpdateValue()", "Response: {0}", customData["json"].ToString());
            updateValueTested = true;
        }

        private void OnGetValue(WatsonResponse<ValueExport> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnGetValue()", "Response: {0}", customData["json"].ToString());
            getValueTested = true;
        }

        private void OnCreateValue(WatsonResponse<Value> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnCreateValue()", "Response: {0}", customData["json"].ToString());
            createValueTested = true;
        }

        private void OnListValues(WatsonResponse<ValueCollection> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnListValues()", "Response: {0}", customData["json"].ToString());
            listValuesTested = true;
        }

        private void OnUpdateEntity(WatsonResponse<Entity> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnUpdateEntity()", "Response: {0}", customData["json"].ToString());
            updateEntityTested = true;
        }

        private void OnGetEntity(WatsonResponse<EntityExport> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnGetEntity()", "Response: {0}", customData["json"].ToString());
            getEntityTested = true;
        }

        private void OnCreateEntity(WatsonResponse<Entity> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnCreateEntity()", "Response: {0}", customData["json"].ToString());
            createEntityTested = true;
        }

        private void OnListEntities(WatsonResponse<EntityCollection> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnListEntities()", "Response: {0}", customData["json"].ToString());
            listEntitiesTested = true;
        }

        private void OnUpdateExample(WatsonResponse<Example> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnUpdateExample()", "Response: {0}", customData["json"].ToString());
            updateExampleTested = true;
        }

        private void OnGetExample(WatsonResponse<Example> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnGetExample()", "Response: {0}", customData["json"].ToString());
            getExampleTested = true;
        }

        private void OnCreateExample(WatsonResponse<Example> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnCreateExample()", "Response: {0}", customData["json"].ToString());
            createExampleTested = true;
        }

        private void OnListExamples(WatsonResponse<ExampleCollection> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnListExamples()", "Response: {0}", customData["json"].ToString());
            listExamplesTested = true;
        }

        private void OnUpdateIntent(WatsonResponse<Intent> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnUpdateIntent()", "Response: {0}", customData["json"].ToString());
            updateIntentTested = true;
        }

        private void OnGetIntent(WatsonResponse<IntentExport> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnGetIntent()", "Response: {0}", customData["json"].ToString());
            getIntentTested = true;
        }

        private void OnCreateIntent(WatsonResponse<Intent> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnCreateIntent()", "Response: {0}", customData["json"].ToString());
            createIntentTested = true;
        }

        private void OnListIntents(WatsonResponse<IntentCollection> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnListIntents()", "Response: {0}", customData["json"].ToString());
            listIntentsTested = true;
        }

        private void OnMessage(WatsonResponse<object> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnMessage()", "Response: {0}", customData["json"].ToString());

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

        private void OnUpdateWorkspace(WatsonResponse<Workspace> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnUpdateWorkspace()", "Response: {0}", customData["json"].ToString());
            updateWorkspaceTested = true;
        }

        private void OnGetWorkspace(WatsonResponse<WorkspaceExport> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnGetWorkspace()", "Response: {0}", customData["json"].ToString());
            getWorkspaceTested = true;
        }

        private void OnCreateWorkspace(WatsonResponse<Workspace> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnCreateWorkspace()", "Response: {0}", customData["json"].ToString());
            createdWorkspaceId = response.Result.WorkspaceId;
            createWorkspaceTested = true;
        }

        private void OnListWorkspaces(WatsonResponse<WorkspaceCollection> response, WatsonError error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV1.OnListWorkspaces()", "Response: {0}", customData["json"].ToString());

            foreach (Workspace workspace in response.Result.Workspaces)
            {
                if (workspace.Name.Contains("unity"))
                    service.DeleteWorkspace(OnDeleteWorkspace, workspace.WorkspaceId);
            }

            listWorkspacesTested = true;
        }

    }
}
