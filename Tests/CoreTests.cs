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
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Watson.Assistant.V1;
using IBM.Watson.Assistant.V1.Model;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace IBM.Watson.Tests
{
    public class CoreTests
    {
        private AssistantService service;
        private string versionDate = "2019-02-13";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            LogSystem.InstallDefaultReactors();
        }

        [Test]
        public void TestGetDefaultHeaders()
        {
            Dictionary<string, string> defaultHeaders = Common.GetSdkHeaders("TestSevice", "V1", "TestOperation");
            Assert.IsNotNull(defaultHeaders);
            Assert.IsTrue(defaultHeaders["X-IBMCloud-SDK-Analytics"] == "service_name=TestSevice;service_version=V1;operation_id=TestOperation");
            Assert.IsNotNull(defaultHeaders["User-Agent"]);
        }

        [UnityTest]
        public IEnumerator TestSetHeaders()
        {
            if (service == null)
            {
                service = new AssistantService(versionDate);
            }

            while (!service.Authenticator.CanAuthenticate())
                yield return null;

            WorkspaceCollection listWorkspacesResponse = null;
            service.WithHeader("myHeaderName", "myHeaderValue");
            service.ListWorkspaces(
                callback: (DetailedResponse<WorkspaceCollection> response, IBMError error) =>
                {
                    Log.Debug("AssistantServiceV1IntegrationTests", "ListWorkspaces result: {0}", response.Response);
                    listWorkspacesResponse = response.Result;
                }
            );
        }
    }
}
