/**
* Copyright 2018, 2019 IBM Corp. All Rights Reserved.
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

using System;
using System.Collections;
using System.Collections.Generic;
using IBM.Cloud.SDK;
using IBM.Watson.Assistant.V1;
using IBM.Watson.Assistant.V1.Model;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace IBM.Watson.Tests
{
    public class AssistantServiceV1IntegrationTests
    {
        private AssistantService service;
        private string versionDate = "2019-02-13";
        private string workspaceId;
        private string createdWorkspaceName = "unity-sdk-example-workspace-delete";
        private string createdWorkspaceDescription = "A Workspace created by the Unity SDK Assistant example script. Please delete this.";
        private string createdWorkspaceLanguage = "en";
        private string intent = "unity-intent";
        private string intentDescription = "An intent created from the Unity SDK - Please delete this.";

        [UnitySetUp]
        public IEnumerator TestSetup()
        {
            LogSystem.InstallDefaultReactors();
            service = new AssistantService(versionDate);

            while (!service.Credentials.HasIamTokenData())
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestMessage()
        {
            workspaceId = Environment.GetEnvironmentVariable("CONVERSATION_WORKSPACE_ID");
            JToken context = null;
            JObject messageResponse = null;
            JToken conversationId = null;
            Log.Debug("AssistantV1IntegrationTests", "Attempting to Message...");
            service.Message(
                callback: (DetailedResponse<JObject> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    messageResponse = response.Result;
                    messageResponse.TryGetValue("context", out context);
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", messageResponse["output"]["generic"][0]["text"]);
                    (context as JObject).TryGetValue("conversation_id", out conversationId);
                    Assert.IsNotNull(context);
                    Assert.IsNotNull(conversationId);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                nodesVisitedDetails: true
            );

            while (messageResponse == null)
                yield return null;

            messageResponse = null;
            JObject input = new JObject();
            JToken conversationId1 = null;
            input.Add("text", "Are you open on Christmas?");
            Log.Debug("AssistantV1IntegrationTests", "Attempting to Message...Are you open on Christmas?");
            service.Message(
                callback: (DetailedResponse<JObject> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    messageResponse = response.Result;
                    messageResponse.TryGetValue("context", out context);
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", messageResponse["output"]["generic"][0]["text"]);
                    (context as JObject).TryGetValue("conversation_id", out conversationId1);

                    Assert.IsNotNull(context);
                    Assert.IsNotNull(conversationId1);
                    Assert.IsTrue(conversationId1.ToString() == conversationId.ToString());
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                input: input,
                context: context as JObject,
                nodesVisitedDetails: true
            );

            while (messageResponse == null)
                yield return null;

            messageResponse = null;
            input = new JObject();
            JToken conversationId2 = null;
            input.Add("text", "What are your hours?");
            Log.Debug("AssistantV1IntegrationTests", "Attempting to Message...What are your hours?");
            service.Message(
                callback: (DetailedResponse<JObject> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    messageResponse = response.Result;
                    messageResponse.TryGetValue("context", out context);
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", messageResponse["output"]["generic"][0]["text"]);
                    (context as JObject).TryGetValue("conversation_id", out conversationId2);

                    Assert.IsNotNull(context);
                    Assert.IsNotNull(conversationId2);
                    Assert.IsTrue(conversationId2.ToString() == conversationId.ToString());
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                input: input,
                context: context as JObject,
                nodesVisitedDetails: true
            );

            while (messageResponse == null)
                yield return null;

            messageResponse = null;
            input = new JObject();
            JToken conversationId3 = null;
            input.Add("text", "I'd like to make an appointment for 12pm.");
            Log.Debug("AssistantV1IntegrationTests", "Attempting to Message...I'd like to make an appointment for 12pm.");
            service.Message(
                callback: (DetailedResponse<JObject> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    messageResponse = response.Result;
                    messageResponse.TryGetValue("context", out context);
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", messageResponse["output"]["generic"][0]["text"]);
                    (context as JObject).TryGetValue("conversation_id", out conversationId3);

                    Assert.IsNotNull(context);
                    Assert.IsNotNull(conversationId3);
                    Assert.IsTrue(conversationId3.ToString() == conversationId.ToString());
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                input: input,
                context: context as JObject,
                nodesVisitedDetails: true
            );

            while (messageResponse == null)
                yield return null;

            messageResponse = null;
            input = new JObject();
            JToken conversationId4 = null;
            input.Add("text", "On Friday please.");
            Log.Debug("AssistantV1IntegrationTests", "Attempting to Message...On Friday please.");
            service.Message(
                callback: (DetailedResponse<JObject> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    messageResponse = response.Result;
                    messageResponse.TryGetValue("context", out context);
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", messageResponse["output"]["generic"][0]["text"]);
                    (context as JObject).TryGetValue("conversation_id", out conversationId4);

                    Assert.IsNotNull(context);
                    Assert.IsNotNull(conversationId4);
                    Assert.IsTrue(conversationId4.ToString() == conversationId.ToString());
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                input: input,
                context: context as JObject,
                nodesVisitedDetails: true
            );

            while (messageResponse == null)
                yield return null;

            messageResponse = null;
            input = new JObject();
            JToken conversationId5 = null;
            input.Add("text", "Yes.");
            Log.Debug("AssistantV1IntegrationTests", "Attempting to Message...Yes.");
            service.Message(
                callback: (DetailedResponse<JObject> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    messageResponse = response.Result;
                    messageResponse.TryGetValue("context", out context);
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", messageResponse["output"]["generic"][0]["text"]);
                    (context as JObject).TryGetValue("conversation_id", out conversationId5);

                    Assert.IsNotNull(context);
                    Assert.IsNotNull(conversationId5);
                    Assert.IsTrue(conversationId5.ToString() == conversationId.ToString());
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                input: input,
                context: context as JObject,
                nodesVisitedDetails: true
            );

            while (messageResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestCreateWorkspace()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to CreateWorkspace...");
            Workspace createWorkspaceResponse = null;
            service.CreateWorkspace(
                callback: (DetailedResponse<Workspace> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "CreateWorkspace result: {0}", customData["json"].ToString());
                    createWorkspaceResponse = response.Result;
                    workspaceId = createWorkspaceResponse.WorkspaceId;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                name: createdWorkspaceName,
                description: createdWorkspaceDescription,
                language: createdWorkspaceLanguage,
                learningOptOut: true
                
            );

            while (createWorkspaceResponse == null)
                yield return null;
        }
        
        [UnityTest, Order(0)]
        public IEnumerator TestGetWorkspace()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to GetWorkspace...");
            WorkspaceExport getWorkspaceResponse = null;
            service.GetWorkspace(
                callback: (DetailedResponse<WorkspaceExport> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "GetWorkspace result: {0}", customData["json"].ToString());
                    getWorkspaceResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                export: true, 
                includeAudit: true, 
                sort: "-name"
            );

            while (getWorkspaceResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestListWorkspaces()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to ListWorkspaces...");
            WorkspaceCollection listWorkspacesResponse = null;
            service.ListWorkspaces(
                callback: (DetailedResponse<WorkspaceCollection> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "ListWorkspaces result: {0}", customData["json"].ToString());
                    listWorkspacesResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                pageLimit: 1, 
                includeCount: true, 
                sort: "-name", 
                includeAudit: true
            );

            while (listWorkspacesResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestUpdateWorkspace()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to UpdateWorkspace...");
            Workspace updateWorkspaceResponse = null;
            service.UpdateWorkspace(
                callback: (DetailedResponse<Workspace> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "UpdateWorkspace result: {0}", customData["json"].ToString());
                    updateWorkspaceResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                name: createdWorkspaceName,
                description: description,
                language: language,
                intents: intents,
                entities: entities,
                dialogNodes: dialogNodes,
                counterexamples: counterexamples,
                metadata: metadata,
                learningOptOut: learningOptOut,
                systemSettings: systemSettings,
                
                append: append
            );

            while (updateWorkspaceResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestCreateIntent()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to CreateIntent...");
            Intent createIntentResponse = null;
            service.CreateIntent(
                callback: (DetailedResponse<Intent> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "CreateIntent result: {0}", customData["json"].ToString());
                    createIntentResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                intent: intent,
                description: description,
                examples: examples,
                
            );

            while (createIntentResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestGetIntent()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to GetIntent...");
            IntentExport getIntentResponse = null;
            service.GetIntent(
                callback: (DetailedResponse<IntentExport> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "GetIntent result: {0}", customData["json"].ToString());
                    getIntentResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                intent: intent, 
                export: export, 
                includeAudit: includeAudit
            );

            while (getIntentResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestListIntents()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to ListIntents...");
            IntentCollection listIntentsResponse = null;
            service.ListIntents(
                callback: (DetailedResponse<IntentCollection> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "ListIntents result: {0}", customData["json"].ToString());
                    listIntentsResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                export: export, 
                pageLimit: pageLimit, 
                includeCount: includeCount, 
                sort: sort, 
                cursor: cursor, 
                includeAudit: includeAudit
            );

            while (listIntentsResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestUpdateIntent()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to UpdateIntent...");
            Intent updateIntentResponse = null;
            service.UpdateIntent(
                callback: (DetailedResponse<Intent> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "UpdateIntent result: {0}", customData["json"].ToString());
                    updateIntentResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                intent: intent, 
                newIntent: newIntent,
                newDescription: newDescription,
                newExamples: newExamples
            );

            while (updateIntentResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestCreateExample()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to CreateExample...");
            Example createExampleResponse = null;
            service.CreateExample(
                callback: (DetailedResponse<Example> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "CreateExample result: {0}", customData["json"].ToString());
                    createExampleResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                intent: intent, 
                text: text,
                mentions: mentions,
                
            );

            while (createExampleResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestGetExample()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to GetExample...");
            Example getExampleResponse = null;
            service.GetExample(
                callback: (DetailedResponse<Example> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "GetExample result: {0}", customData["json"].ToString());
                    getExampleResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                intent: intent, 
                text: text, 
                includeAudit: includeAudit
            );

            while (getExampleResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestListExamples()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to ListExamples...");
            ExampleCollection listExamplesResponse = null;
            service.ListExamples(
                callback: (DetailedResponse<ExampleCollection> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "ListExamples result: {0}", customData["json"].ToString());
                    listExamplesResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                intent: intent, 
                pageLimit: pageLimit, 
                includeCount: includeCount, 
                sort: sort, 
                cursor: cursor, 
                includeAudit: includeAudit
            );

            while (listExamplesResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestUpdateExample()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to UpdateExample...");
            Example updateExampleResponse = null;
            service.UpdateExample(
                callback: (DetailedResponse<Example> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "UpdateExample result: {0}", customData["json"].ToString());
                    updateExampleResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                intent: intent, 
                text: text, 
                newText: newText,
                newMentions: newMentions,
                
            );

            while (updateExampleResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestCreateCounterexample()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to CreateCounterexample...");
            Counterexample createCounterexampleResponse = null;
            service.CreateCounterexample(
                callback: (DetailedResponse<Counterexample> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "CreateCounterexample result: {0}", customData["json"].ToString());
                    createCounterexampleResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                text: text,
                
            );

            while (createCounterexampleResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestGetCounterexample()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to GetCounterexample...");
            Counterexample getCounterexampleResponse = null;
            service.GetCounterexample(
                callback: (DetailedResponse<Counterexample> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "GetCounterexample result: {0}", customData["json"].ToString());
                    getCounterexampleResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                text: text, 
                includeAudit: includeAudit
            );

            while (getCounterexampleResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestListCounterexamples()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to ListCounterexamples...");
            CounterexampleCollection listCounterexamplesResponse = null;
            service.ListCounterexamples(
                callback: (DetailedResponse<CounterexampleCollection> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "ListCounterexamples result: {0}", customData["json"].ToString());
                    listCounterexamplesResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                pageLimit: pageLimit, 
                includeCount: includeCount, 
                sort: sort, 
                cursor: cursor, 
                includeAudit: includeAudit
            );

            while (listCounterexamplesResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestUpdateCounterexample()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to UpdateCounterexample...");
            Counterexample updateCounterexampleResponse = null;
            service.UpdateCounterexample(
                callback: (DetailedResponse<Counterexample> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "UpdateCounterexample result: {0}", customData["json"].ToString());
                    updateCounterexampleResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                text: text, 
                newText: newText,
                
            );

            while (updateCounterexampleResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestCreateEntity()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to CreateEntity...");
            Entity createEntityResponse = null;
            service.CreateEntity(
                callback: (DetailedResponse<Entity> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "CreateEntity result: {0}", customData["json"].ToString());
                    createEntityResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                entity: entity,
                description: description,
                metadata: metadata,
                values: values,
                fuzzyMatch: fuzzyMatch,
                
            );

            while (createEntityResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestGetEntity()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to GetEntity...");
            EntityExport getEntityResponse = null;
            service.GetEntity(
                callback: (DetailedResponse<EntityExport> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "GetEntity result: {0}", customData["json"].ToString());
                    getEntityResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                entity: entity, 
                export: export, 
                includeAudit: includeAudit
            );

            while (getEntityResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestListEntities()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to ListEntities...");
            EntityCollection listEntitiesResponse = null;
            service.ListEntities(
                callback: (DetailedResponse<EntityCollection> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "ListEntities result: {0}", customData["json"].ToString());
                    listEntitiesResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                export: export, 
                pageLimit: pageLimit, 
                includeCount: includeCount, 
                sort: sort, 
                cursor: cursor, 
                includeAudit: includeAudit
            );

            while (listEntitiesResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestUpdateEntity()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to UpdateEntity...");
            Entity updateEntityResponse = null;
            service.UpdateEntity(
                callback: (DetailedResponse<Entity> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "UpdateEntity result: {0}", customData["json"].ToString());
                    updateEntityResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                entity: entity, 
                newEntity: newEntity,
                newDescription: newDescription,
                newMetadata: newMetadata,
                newFuzzyMatch: newFuzzyMatch,
                newValues: newValues,
                
            );

            while (updateEntityResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestListMentions()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to ListMentions...");
            EntityMentionCollection listMentionsResponse = null;
            service.ListMentions(
                callback: (DetailedResponse<EntityMentionCollection> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "ListMentions result: {0}", customData["json"].ToString());
                    listMentionsResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                entity: entity, 
                export: export, 
                includeAudit: includeAudit
            );

            while (listMentionsResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestCreateValue()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to CreateValue...");
            Value createValueResponse = null;
            service.CreateValue(
                callback: (DetailedResponse<Value> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "CreateValue result: {0}", customData["json"].ToString());
                    createValueResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                entity: entity, 
                value: value,
                metadata: metadata,
                synonyms: synonyms,
                patterns: patterns,
                type: type,
                
            );

            while (createValueResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestGetValue()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to GetValue...");
            ValueExport getValueResponse = null;
            service.GetValue(
                callback: (DetailedResponse<ValueExport> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "GetValue result: {0}", customData["json"].ToString());
                    getValueResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                entity: entity, 
                value: value, 
                export: export, 
                includeAudit: includeAudit
            );

            while (getValueResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestListValues()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to ListValues...");
            ValueCollection listValuesResponse = null;
            service.ListValues(
                callback: (DetailedResponse<ValueCollection> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "ListValues result: {0}", customData["json"].ToString());
                    listValuesResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                entity: entity, 
                export: export, 
                pageLimit: pageLimit, 
                includeCount: includeCount, 
                sort: sort, 
                cursor: cursor, 
                includeAudit: includeAudit
            );

            while (listValuesResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestUpdateValue()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to UpdateValue...");
            Value updateValueResponse = null;
            service.UpdateValue(
                callback: (DetailedResponse<Value> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "UpdateValue result: {0}", customData["json"].ToString());
                    updateValueResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                entity: entity, 
                value: value, 
                newValue: newValue,
                newMetadata: newMetadata,
                newType: newType,
                newSynonyms: newSynonyms,
                newPatterns: newPatterns,
                
            );

            while (updateValueResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestCreateSynonym()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to CreateSynonym...");
            Synonym createSynonymResponse = null;
            service.CreateSynonym(
                callback: (DetailedResponse<Synonym> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "CreateSynonym result: {0}", customData["json"].ToString());
                    createSynonymResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                entity: entity, 
                value: value, 
                synonym: synonym,
                
            );

            while (createSynonymResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestGetSynonym()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to GetSynonym...");
            Synonym getSynonymResponse = null;
            service.GetSynonym(
                callback: (DetailedResponse<Synonym> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "GetSynonym result: {0}", customData["json"].ToString());
                    getSynonymResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                entity: entity, 
                value: value, 
                synonym: synonym, 
                includeAudit: includeAudit
            );

            while (getSynonymResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestListSynonyms()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to ListSynonyms...");
            SynonymCollection listSynonymsResponse = null;
            service.ListSynonyms(
                callback: (DetailedResponse<SynonymCollection> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "ListSynonyms result: {0}", customData["json"].ToString());
                    listSynonymsResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                entity: entity, 
                value: value, 
                pageLimit: pageLimit, 
                includeCount: includeCount, 
                sort: sort, 
                cursor: cursor, 
                includeAudit: includeAudit
            );

            while (listSynonymsResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestUpdateSynonym()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to UpdateSynonym...");
            Synonym updateSynonymResponse = null;
            service.UpdateSynonym(
                callback: (DetailedResponse<Synonym> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "UpdateSynonym result: {0}", customData["json"].ToString());
                    updateSynonymResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                entity: entity, 
                value: value, 
                synonym: synonym, 
                newSynonym: newSynonym,
                
            );

            while (updateSynonymResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestCreateDialogNode()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to CreateDialogNode...");
            DialogNode createDialogNodeResponse = null;
            service.CreateDialogNode(
                callback: (DetailedResponse<DialogNode> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "CreateDialogNode result: {0}", customData["json"].ToString());
                    createDialogNodeResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                dialogNode: dialogNode,
                description: description,
                conditions: conditions,
                parent: parent,
                previousSibling: previousSibling,
                output: output,
                context: context,
                metadata: metadata,
                nextStep: nextStep,
                actions: actions,
                title: title,
                type: type,
                eventName: eventName,
                variable: variable,
                digressIn: digressIn,
                digressOut: digressOut,
                digressOutSlots: digressOutSlots,
                userLabel: userLabel,
                
            );

            while (createDialogNodeResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestGetDialogNode()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to GetDialogNode...");
            DialogNode getDialogNodeResponse = null;
            service.GetDialogNode(
                callback: (DetailedResponse<DialogNode> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "GetDialogNode result: {0}", customData["json"].ToString());
                    getDialogNodeResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                dialogNode: dialogNode, 
                includeAudit: includeAudit
            );

            while (getDialogNodeResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestListDialogNodes()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to ListDialogNodes...");
            DialogNodeCollection listDialogNodesResponse = null;
            service.ListDialogNodes(
                callback: (DetailedResponse<DialogNodeCollection> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "ListDialogNodes result: {0}", customData["json"].ToString());
                    listDialogNodesResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                pageLimit: pageLimit, 
                includeCount: includeCount, 
                sort: sort, 
                cursor: cursor, 
                includeAudit: includeAudit
            );

            while (listDialogNodesResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestUpdateDialogNode()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to UpdateDialogNode...");
            DialogNode updateDialogNodeResponse = null;
            service.UpdateDialogNode(
                callback: (DetailedResponse<DialogNode> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "UpdateDialogNode result: {0}", customData["json"].ToString());
                    updateDialogNodeResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                dialogNode: dialogNode, 
                newDialogNode: newDialogNode,
                newDescription: newDescription,
                newConditions: newConditions,
                newParent: newParent,
                newPreviousSibling: newPreviousSibling,
                newOutput: newOutput,
                newContext: newContext,
                newMetadata: newMetadata,
                newNextStep: newNextStep,
                newTitle: newTitle,
                newType: newType,
                newEventName: newEventName,
                newVariable: newVariable,
                newActions: newActions,
                newDigressIn: newDigressIn,
                newDigressOut: newDigressOut,
                newDigressOutSlots: newDigressOutSlots,
                newUserLabel: newUserLabel,
                
            );

            while (updateDialogNodeResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestListAllLogs()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to ListAllLogs...");
            LogCollection listAllLogsResponse = null;
            service.ListAllLogs(
                callback: (DetailedResponse<LogCollection> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "ListAllLogs result: {0}", customData["json"].ToString());
                    listAllLogsResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                filter: filter, 
                sort: sort, 
                pageLimit: pageLimit, 
                cursor: cursor
            );

            while (listAllLogsResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestListLogs()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to ListLogs...");
            LogCollection listLogsResponse = null;
            service.ListLogs(
                callback: (DetailedResponse<LogCollection> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "ListLogs result: {0}", customData["json"].ToString());
                    listLogsResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId, 
                sort: sort, 
                filter: filter, 
                pageLimit: pageLimit, 
                cursor: cursor
            );

            while (listLogsResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestDeleteUserData()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to DeleteUserData...");
            object deleteUserDataResponse = null;
            service.DeleteUserData(
                callback: (DetailedResponse<object> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "DeleteUserData result: {0}", customData["json"].ToString());
                    deleteUserDataResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                customerId: customerId
            );

            while (deleteUserDataResponse == null)
                yield return null;
        }


        [UnityTest, Order(0)]
        public IEnumerator TestDeleteDialogNode()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to DeleteDialogNode...");
            object deleteDialogNodeResponse = null;
            service.DeleteDialogNode(
                callback: (DetailedResponse<object> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "DeleteDialogNode result: {0}", customData["json"].ToString());
                    deleteDialogNodeResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                dialogNode: dialogNode
            );

            while (deleteDialogNodeResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestDeleteSynonym()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to DeleteSynonym...");
            object deleteSynonymResponse = null;
            service.DeleteSynonym(
                callback: (DetailedResponse<object> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "DeleteSynonym result: {0}", customData["json"].ToString());
                    deleteSynonymResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                entity: entity,
                value: value,
                synonym: synonym
            );

            while (deleteSynonymResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestDeleteValue()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to DeleteValue...");
            object deleteValueResponse = null;
            service.DeleteValue(
                callback: (DetailedResponse<object> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "DeleteValue result: {0}", customData["json"].ToString());
                    deleteValueResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                entity: entity,
                value: value
            );

            while (deleteValueResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestDeleteEntity()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to DeleteEntity...");
            object deleteEntityResponse = null;
            service.DeleteEntity(
                callback: (DetailedResponse<object> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "DeleteEntity result: {0}", customData["json"].ToString());
                    deleteEntityResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                entity: entity
            );

            while (deleteEntityResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestDeleteCounterexample()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to DeleteCounterexample...");
            object deleteCounterexampleResponse = null;
            service.DeleteCounterexample(
                callback: (DetailedResponse<object> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "DeleteCounterexample result: {0}", customData["json"].ToString());
                    deleteCounterexampleResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                text: text
            );

            while (deleteCounterexampleResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestDeleteExample()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to DeleteExample...");
            object deleteExampleResponse = null;
            service.DeleteExample(
                callback: (DetailedResponse<object> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "DeleteExample result: {0}", customData["json"].ToString());
                    deleteExampleResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                intent: intent,
                text: text
            );

            while (deleteExampleResponse == null)
                yield return null;
        }

        [UnityTest, Order(0)]
        public IEnumerator TestDeleteIntent()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to DeleteIntent...");
            object deleteIntentResponse = null;
            service.DeleteIntent(
                callback: (DetailedResponse<object> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "DeleteIntent result: {0}", customData["json"].ToString());
                    deleteIntentResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                intent: intent
            );

            while (deleteIntentResponse == null)
                yield return null;
        }


        [UnityTest, Order(0)]
        public IEnumerator TestDeleteWorkspace()
        {
            Log.Debug("AssistantServiceV1IntegrationTests", "Attempting to DeleteWorkspace...");
            object deleteWorkspaceResponse = null;
            service.DeleteWorkspace(
                callback: (DetailedResponse<object> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "DeleteWorkspace result: {0}", customData["json"].ToString());
                    deleteWorkspaceResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId
            );

            while (deleteWorkspaceResponse == null)
                yield return null;
        }
    }
}
