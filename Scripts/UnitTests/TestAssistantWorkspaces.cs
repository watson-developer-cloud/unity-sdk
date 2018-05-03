/**
* Copyright 2018 IBM Corp. All Rights Reserved.
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
using System.IO;
using FullSerializer;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.Assistant.v1;
using IBM.Watson.DeveloperCloud.UnitTests;
using IBM.Watson.DeveloperCloud.Utilities;

namespace Assets.Watson.Scripts.UnitTests
{
    public class TestAssistantWorkspaces : UnitTest
    {
        private string _username = null;
        private string _password = null;
        private string _createdWorkspaceId;

        private Assistant _service;
        private string _assistantVersionDate = "2017-05-26";
        private fsSerializer _serializer = new fsSerializer();

        private bool _listWorkspacesTested = false;
        private bool _createWorkspaceTested = false;
        private bool _getWorkspaceTested = false;
        private bool _updateWorkspaceTested = false;
        private bool _deleteWorkspaceTested = false;

        private static string _createdWorkspaceName = "unity-sdk-example-workspace-delete";
        private static string _createdWorkspaceDescription = "A Workspace created by the Unity SDK Assistant example script. Please delete this.";
        private static string _createdWorkspaceLanguage = "en";

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

            //  Delete Workspace
            _service.DeleteWorkspace(OnDeleteWorkspace, OnFail, _createdWorkspaceId);
            while (!_deleteWorkspaceTested)
                yield return null;
        }

        private void OnUpdateWorkspace(Workspace response, Dictionary<string, object> customData)
        {
            Log.Debug("TestAssistantWorkspaces.OnUpdateWorkspace()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _updateWorkspaceTested = true;
        }

        private void OnGetWorkspace(WorkspaceExport response, Dictionary<string, object> customData)
        {
            Log.Debug("TestAssistantWorkspaces.OnGetWorkspace()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _getWorkspaceTested = true;
        }

        private void OnCreateWorkspace(Workspace response, Dictionary<string, object> customData)
        {
            Log.Debug("TestAssistantWorkspaces.OnCreateWorkspace()", "Response: {0}", customData["json"].ToString());
            _createdWorkspaceId = response.WorkspaceId;
            Test(response != null);
            _createWorkspaceTested = true;
        }

        private void OnListWorkspaces(WorkspaceCollection response, Dictionary<string, object> customData)
        {
            Log.Debug("TestAssistantWorkspaces.OnListWorkspaces()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _listWorkspacesTested = true;
        }

        private void OnDeleteWorkspace(object response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnDeleteWorkspace()", "Workspace deleted");
            _deleteWorkspaceTested = true;
        }

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("TestAssistantWorkspacesWorkspaces.OnFail()", "Error received: {0}", error.ToString());
        }
    }
}
