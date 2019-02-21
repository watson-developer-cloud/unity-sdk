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

using IBM.Cloud.SDK;
using IBM.Watson.Assistant.V2;
using IBM.Watson.Assistant.V2.Model;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TestTools;

namespace IBM.Watson.Tests
{
    public class AssistantV2IntegrationTests
    {
        private AssistantService service;
        private string versionDate = "2019-02-18";
        private string assistantId;
        private string sessionId;

        [SetUp]
        public void TestSetup()
        {
            LogSystem.InstallDefaultReactors();
        }

        [UnityTest]
        public IEnumerator TestCreateSession()
        {
            service = new AssistantService(versionDate);

            while (!service.Credentials.HasIamTokenData())
                yield return null;

            assistantId = Environment.GetEnvironmentVariable("CONVERSATION_ASSISTANT_ID");

            service.CreateSession((WatsonResponse<SessionResponse> response, WatsonError error, Dictionary<string, object> customData) =>
            {
                Assert.IsNotNull(response.Result);
                Assert.IsNotNull(response.Result.SessionId);
            }, assistantId);
        }

        [UnityTest]
        public IEnumerator TestMessage()
        {
            service = new AssistantService(versionDate);

            while (!service.Credentials.HasIamTokenData())
                yield return null;

            assistantId = Environment.GetEnvironmentVariable("CONVERSATION_ASSISTANT_ID");
            sessionId = null;

            service.CreateSession((WatsonResponse<SessionResponse> response, WatsonError error, Dictionary<string, object> customData) =>
            {
                sessionId = response.Result.SessionId;
            }, assistantId);

            while (string.IsNullOrEmpty(sessionId))
                yield return null;

            service.Message((WatsonResponse<MessageResponse> response, WatsonError error, Dictionary<string, object> customData) =>
            {
                Assert.IsNotNull(response.Result);
            }, assistantId, sessionId);
        }

        [UnityTest]
        public IEnumerator TestDeleteSession()
        {
            service = new AssistantService(versionDate);

            while (!service.Credentials.HasIamTokenData())
                yield return null;

            assistantId = Environment.GetEnvironmentVariable("CONVERSATION_ASSISTANT_ID");
            sessionId = null;

            service.CreateSession((WatsonResponse<SessionResponse> response, WatsonError error, Dictionary<string, object> customData) =>
            {
                sessionId = response.Result.SessionId;
            }, assistantId);

            while (string.IsNullOrEmpty(sessionId))
                yield return null;

            service.DeleteSession((WatsonResponse<object> response, WatsonError error, Dictionary<string, object> customData) =>
            {
                Assert.IsNotNull(response.Result);
                sessionId = null;
            }, assistantId, sessionId);
        }

        [TearDown]
        public void TestTearDown()
        {
            if (!string.IsNullOrEmpty(sessionId))
            {
                service.DeleteSession((WatsonResponse<object> response, WatsonError error, Dictionary<string, object> customData) =>
                {
                    Assert.IsNotNull(response.Result);
                    Log.Debug("ExampleAssistantV2.OnDeleteSession()", "Session deleted.");
                }, assistantId, sessionId);
            }
        }
    }
}
