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
    public class AssistantV1IntegrationTests
    {
        private AssistantService service;
        private string versionDate = "2019-02-13";
        private string workspaceId;
        private string createdWorkspaceName = "unity-sdk-example-workspace-delete";
        private string createdWorkspaceDescription = "A Workspace created by the Unity SDK Assistant example script. Please delete this.";
        private string createdWorkspaceLanguage = "en";
        private string intent = "unity-intent";
        private string intentDescription = "An intent created from the Unity SDK - Please delete this.";

        private Workspace createWorkspaceResponse = null;
        private CounterexampleCollection listCounterexamplesResponse = null;
        private Counterexample createCounterexampleResponse = null;
        private Counterexample getCounterexampleResponse = null;
        private Counterexample updateCounterexampleResponse = null;
        object deleteCounterexampleResponse = null;
        object deleteWorkspaceResponse = null;

        [UnitySetUp]
        public IEnumerator TestSetup()
        {
            LogSystem.InstallDefaultReactors();

            service = new AssistantService(versionDate);
            while (!service.Credentials.HasIamTokenData())
                yield return null;
        }

        #region Message
        [UnityTest]
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
        #endregion

        #region Workspaces
        [UnityTest]
        public IEnumerator TestWorkspaces()
        {
            string createdWorkspaceId = null;

            WorkspaceCollection listWorkspaceResponse = null;
            Log.Debug("AssistantV1IntegrationTests", "Attempting to ListWorkspaces...");
            service.ListWorkspaces(
                callback: (DetailedResponse<WorkspaceCollection> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    listWorkspaceResponse = response.Result;
                    Assert.IsNotNull(listWorkspaceResponse);
                    Assert.IsNotNull(listWorkspaceResponse.Workspaces);
                    Assert.IsNull(error);
                },
                pageLimit: 1,
                includeCount: true,
                sort: "-name",
                includeAudit: true
            );

            while (listWorkspaceResponse == null)
                yield return null;

            Workspace createWorkspaceResponse = null;
            Log.Debug("AssistantV1IntegrationTests", "Attempting to CreateWorkspace...");
            service.CreateWorkspace(
                callback: (DetailedResponse<Workspace> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    createWorkspaceResponse = response.Result;
                    createdWorkspaceId = response.Result.WorkspaceId;
                    Assert.IsNotNull(response.Result);
                    Assert.IsTrue(!string.IsNullOrEmpty(createdWorkspaceId));
                    Assert.IsNull(error);
                },
                name: createdWorkspaceName,
                description: createdWorkspaceDescription,
                language: createdWorkspaceLanguage,
                learningOptOut: true
            );

            while (createWorkspaceResponse == null)
                yield return null;

            Workspace updateWorkspaceResponse = null;
            Log.Debug("AssistantV1IntegrationTests", "Attempting to UpdateWorkspace...");
            service.UpdateWorkspace(
                callback: (DetailedResponse<Workspace> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    updateWorkspaceResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsTrue(response.Result.Name == createdWorkspaceName + "Updated");
                    Assert.IsTrue(response.Result.Description == createdWorkspaceDescription + "Updated");
                    Assert.IsTrue(response.Result.Language == createdWorkspaceLanguage);
                    Assert.IsTrue(!string.IsNullOrEmpty(updateWorkspaceResponse.WorkspaceId));
                    Assert.IsNull(error);
                },
                workspaceId: createdWorkspaceId,
                name: createdWorkspaceName + "Updated",
                description: createdWorkspaceDescription + "Updated",
                language: createdWorkspaceLanguage
            );

            while (updateWorkspaceResponse == null)
                yield return null;

            object deleteWorkspaceResponse = null;
            Log.Debug("AssistantV1IntegrationTests", "Attempting to DeleteWorkspace...");
            service.DeleteWorkspace(
                callback: (DetailedResponse<object> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    deleteWorkspaceResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: createdWorkspaceId
            );

            while (deleteWorkspaceResponse == null)
                yield return null;

            Log.Debug("AssistantV1IntegrationTests", "Workspace tests complete!");
        }
        #endregion

        #region Intents
        [UnityTest]
        public IEnumerator TestIntents()
        {
            workspaceId = Environment.GetEnvironmentVariable("CONVERSATION_WORKSPACE_ID");

            Log.Debug("AssistantV1IntegrationTests", "Attempting to ListIntents...");
            IntentCollection listIntentsResponse = null;
            service.ListIntents(
                callback: (DetailedResponse<IntentCollection> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    listIntentsResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId
            );

            while (listIntentsResponse == null)
                yield return null;

            Log.Debug("AssistantV1IntegrationTests", "Attempting to CreateIntent...");
            Intent createIntentResponse = null;
            service.CreateIntent(
                callback: (DetailedResponse<Intent> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    createIntentResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsTrue(createIntentResponse.IntentName == intent);
                    Assert.IsTrue(createIntentResponse.Description == intentDescription);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                intent: intent,
                description: intentDescription
            );

            while (createIntentResponse == null)
                yield return null;

            Log.Debug("AssistantV1IntegrationTests", "Attempting to GetIntent...");

            IntentExport getIntentResponse = null;
            service.GetIntent(
                callback: (DetailedResponse<IntentExport> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    getIntentResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsTrue(getIntentResponse.IntentName == intent);
                    Assert.IsTrue(getIntentResponse.Description == intentDescription);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                intent: intent,
                export: true,
                includeAudit: true
            );

            while (getIntentResponse == null)
                yield return null;

            Log.Debug("AssistantV1IntegrationTests", "Attempting to UpdateIntent...");
            Intent updateIntentResponse = null;
            string newIntent = intent + "-updated";
            string newIntentDescription = intentDescription + "-updated";
            service.UpdateIntent(
                callback: (DetailedResponse<Intent> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    updateIntentResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsTrue(updateIntentResponse.IntentName == newIntent);
                    Assert.IsTrue(updateIntentResponse.Description == newIntentDescription);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                intent: intent,
                newIntent: newIntent,
                newDescription: newIntentDescription
            );

            while (updateIntentResponse == null)
                yield return null;

            Log.Debug("AssistantV1IntegrationTests", "Attempting to DeleteIntent...");
            object deleteIntentResponse = null;
            service.DeleteIntent(
                callback: (DetailedResponse<object> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    deleteIntentResponse = response.Result;
                    Assert.IsNotNull(deleteIntentResponse);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                intent: newIntent
            );

            while (deleteIntentResponse == null)
                yield return null;

            Log.Debug("AssistantV1IntegrationTests", "Intents tests complete!");
        }
        #endregion

        [UnityTest, Order(0)]
        public IEnumerator TestCreateWorkspace()
        {
            Log.Debug("AssistantV1IntegrationTests", "Attempting to CreateWorkspace...");
            service.CreateWorkspace(
                callback: (DetailedResponse<Workspace> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    createWorkspaceResponse = response.Result;
                    workspaceId = response.Result.WorkspaceId;
                    Assert.IsNotNull(response.Result);
                    Assert.IsTrue(!string.IsNullOrEmpty(workspaceId));
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

        [UnityTest, Order(1)]
        public IEnumerator TestListCounterExamples()
        {
            Log.Debug("AssistantV1IntegrationTests", "Attempting to ListCounterexamples...");
            service.ListCounterexamples(
                callback: (DetailedResponse<CounterexampleCollection> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    listCounterexamplesResponse = response.Result;
                    Assert.IsNotNull(listCounterexamplesResponse);
                    Assert.IsNotNull(listCounterexamplesResponse.Counterexamples);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                pageLimit: 1,
                includeCount: true,
                sort: "-text",
                includeAudit: true
            );

            while (listCounterexamplesResponse == null)
                yield return null;
        }

        [UnityTest, Order(2)]
        public IEnumerator TestCreateCounterExample()
        {
            Log.Debug("AssistantV1IntegrationTests", "Attempting to CreateCounterexample...");
            service.CreateCounterexample(
                callback: (DetailedResponse<Counterexample> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    createCounterexampleResponse = response.Result;
                    Assert.IsNotNull(createCounterexampleResponse);
                    Assert.IsNotNull(createCounterexampleResponse.Text);
                    Assert.IsTrue(createCounterexampleResponse.Text == "unity-CreateCounterExample");
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                text: "unity-CreateCounterExample"
            );

            while (createCounterexampleResponse == null)
                yield return null;
        }

        [UnityTest, Order(3)]
        public IEnumerator TestGetCounterExample()
        {
            Log.Debug("AssistantV1IntegrationTests", "Attempting to GetCounterexample...");
            service.GetCounterexample(
                callback: (DetailedResponse<Counterexample> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    getCounterexampleResponse = response.Result;
                    Assert.IsNotNull(getCounterexampleResponse);
                    Assert.IsNotNull(getCounterexampleResponse.Text);
                    Assert.IsTrue(getCounterexampleResponse.Text == "unity-CreateCounterExample");
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                text: "unity-CreateCounterExample"
            );

            while (getCounterexampleResponse == null)
                yield return null;
        }

        [UnityTest, Order(4)]
        public IEnumerator TestUpdateCounterExample()
        {
            Log.Debug("AssistantV1IntegrationTests", "Attempting to UpdateCounterexample...");
            service.UpdateCounterexample(
                callback: (DetailedResponse<Counterexample> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    updateCounterexampleResponse = response.Result;
                    Assert.IsNotNull(updateCounterexampleResponse);
                    Assert.IsNotNull(updateCounterexampleResponse.Text);
                    Assert.IsTrue(updateCounterexampleResponse.Text == "unity-CreateCounterExample-updated");
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                text: "unity-CreateCounterExample",
                newText: "unity-CreateCounterExample-updated"
            );

            while (updateCounterexampleResponse == null)
                yield return null;
        }

        [UnityTest, Order(5)]
        public IEnumerator TestDeleteCounterExample()
        {
            Log.Debug("AssistantV1IntegrationTests", "Attempting to DeleteCounterexample...");
            service.DeleteCounterexample(
                callback: (DetailedResponse<object> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    deleteCounterexampleResponse = response.Result;
                    Assert.IsNotNull(deleteCounterexampleResponse);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                text: "unity-CreateCounterExample-updated"
            );

            while (deleteCounterexampleResponse == null)
                yield return null;
        }

        [UnityTest, Order(6)]
        public IEnumerator TestDeleteWorkspace()
        {
            Log.Debug("AssistantV1IntegrationTests", "Attempting to DeleteWorkspace...");
            service.DeleteWorkspace(
                callback: (DetailedResponse<object> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    deleteWorkspaceResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId
            );

            while (deleteWorkspaceResponse == null)
                yield return null;
        }

        [UnityTest, Order(99)]
        public IEnumerator Teardown()
        {
            WorkspaceCollection listWorkspaceResponse = null;
            service.ListWorkspaces(
                callback: (DetailedResponse<WorkspaceCollection> response, IBMError error, Dictionary<string, object> customData) =>
                {
                    listWorkspaceResponse = response.Result;
                }
            );

            while (listWorkspaceResponse == null)
                yield return null;

            foreach (Workspace workspace in listWorkspaceResponse.Workspaces)
            {
                string workspaceIdToDelete = workspace.WorkspaceId;
                string workspaceNameToDelete = workspace.Name;
                if (workspaceNameToDelete.Contains("unity"))
                {
                    service.DeleteWorkspace(
                        callback: (DetailedResponse<object> response, IBMError error, Dictionary<string, object> customData) =>
                        {
                            Log.Debug("AssistantV1IntegrationTests", "Workspace {0}, {1} deleted!", workspaceNameToDelete, workspaceIdToDelete);
                        },
                        workspaceIdToDelete
                    );
                }
            }
        }
    }
}
