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

        [SetUp]
        public void TestSetup()
        {
            LogSystem.InstallDefaultReactors();
        }

        [UnityTest]
        public IEnumerator TestMessage()
        {
            service = new AssistantService(versionDate);
            while (!service.Credentials.HasIamTokenData())
                yield return null;

            workspaceId = Environment.GetEnvironmentVariable("CONVERSATION_WORKSPACE_ID");
            Context context = null;

            MessageResponse messageResponse = null;
            Log.Debug("AssistantV1IntegrationTests", "Attempting to Message...");
            service.Message(
                callback: (WatsonResponse<MessageResponse> response, WatsonError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    messageResponse = response.Result;
                    context = messageResponse.Context;
                    Assert.IsNotNull(messageResponse.Context.ConversationId);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                nodesVisitedDetails: true
            );

            while (messageResponse == null)
                yield return null;

            messageResponse = null;
            var input = new InputData()
            {
                Text = "Are you open on Christmas?"
            };
            Log.Debug("AssistantV1IntegrationTests", "Attempting to Message...Are you open on Christmas?");
            service.Message(
                callback: (WatsonResponse<MessageResponse> response, WatsonError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    messageResponse = response.Result;
                    Assert.IsNotNull(messageResponse.Context.ConversationId);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                input: input, 
                context: context,
                nodesVisitedDetails: true
            );

            while (messageResponse == null)
                yield return null;

            messageResponse = null;
            input = new InputData()
            {
                Text = "What are your hours?"
            };
            Log.Debug("AssistantV1IntegrationTests", "Attempting to Message...What are your hours?");
            service.Message(
                callback: (WatsonResponse<MessageResponse> response, WatsonError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    messageResponse = response.Result;
                    Assert.IsNotNull(messageResponse.Context.ConversationId);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                input: input,
                context: context,
                nodesVisitedDetails: true
            );

            while (messageResponse == null)
                yield return null;

            messageResponse = null;
            input = new InputData()
            {
                Text = "I'd like to make an appointment for 12pm."
            };
            Log.Debug("AssistantV1IntegrationTests", "Attempting to Message...I'd like to make an appointment for 12pm.");
            service.Message(
                callback: (WatsonResponse<MessageResponse> response, WatsonError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    messageResponse = response.Result;
                    Assert.IsNotNull(messageResponse.Context.ConversationId);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                input: input,
                context: context,
                nodesVisitedDetails: true
            );

            while (messageResponse == null)
                yield return null;

            messageResponse = null;
            input = new InputData()
            {
                Text = "On Friday please."
            };
            Log.Debug("AssistantV1IntegrationTests", "Attempting to Message...On Friday please.");
            service.Message(
                callback: (WatsonResponse<MessageResponse> response, WatsonError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    messageResponse = response.Result;
                    Assert.IsNotNull(messageResponse.Context.ConversationId);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                input: input,
                context: context,
                nodesVisitedDetails: true
            );

            while (messageResponse == null)
                yield return null;

            messageResponse = null;
            input = new InputData()
            {
                Text = "Yes."
            };
            Log.Debug("AssistantV1IntegrationTests", "Attempting to Message...Yes.");
            service.Message(
                callback: (WatsonResponse<MessageResponse> response, WatsonError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    messageResponse = response.Result;
                    Assert.IsNotNull(messageResponse.Context.ConversationId);
                    Assert.IsNull(error);
                },
                workspaceId: workspaceId,
                input: input,
                context: context,
                nodesVisitedDetails: true
            );

            while (messageResponse == null)
                yield return null;
        }

        [UnityTest]
        public IEnumerator TestWorkspaces()
        {
            service = new AssistantService(versionDate);
            while (!service.Credentials.HasIamTokenData())
                yield return null;

            workspaceId = Environment.GetEnvironmentVariable("CONVERSATION_WORKSPACE_ID");
            string createdWorkspaceId = null;

            WorkspaceCollection listWorkspaceResponse = null;
            Log.Debug("AssistantV1IntegrationTests", "Attempting to ListWorkspaces...");
            service.ListWorkspaces(
                callback: (WatsonResponse<WorkspaceCollection> response, WatsonError error, Dictionary<string, object> customData) =>
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
                callback: (WatsonResponse<Workspace> response, WatsonError error, Dictionary<string, object> customData) =>
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
                callback: (WatsonResponse<Workspace> response, WatsonError error, Dictionary<string, object> customData) =>
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
                callback: (WatsonResponse<object> response, WatsonError error, Dictionary<string, object> customData) =>
                {
                    Log.Debug("AssistantV1IntegrationTests", "result: {0}", customData["json"].ToString());
                    deleteWorkspaceResponse = response.Result;
                    Assert.IsNotNull(response.Result);
                    Assert.IsNull(error);
                },
                workspaceId: createdWorkspaceId
            );
        }
    }
}
