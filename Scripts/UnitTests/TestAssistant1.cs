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
    class TestAssistant1 : UnitTest
    {
        private string _username = null;
        private string _password = null;
        private string _createdWorkspaceId;

        private Assistant _service;
        private string _assistantVersionDate = "2017-05-26";

        private fsSerializer _serializer = new fsSerializer();

        private static string _createdWorkspaceName = "unity-sdk-example-workspace-delete";
        private static string _createdWorkspaceDescription = "A Workspace created by the Unity SDK Assistant example script. Please delete this.";
        private static string _createdWorkspaceLanguage = "en";
        private static string _createdCounterExampleText = "untiyExampleText";
        private static string _dialogNodeName = "untiyDialognode";
        private static string _dialogNodeDesc = "Unity SDK Integration test dialog node";

        private bool _listDialogNodesTested = false;
        private bool _createDialogNodeTested = false;
        private bool _getDialogNodeTested = false;
        private bool _updateDialogNodeTested = false;
        private bool _listCounterexamplesTested = false;
        private bool _createCounterexampleTested = false;
        private bool _getCounterexampleTested = false;
        private bool _updateCounterexampleTested = false;
        private bool _deleteDialogNodeTested = false;
        private bool _deleteCounterexampleTested = false;
        private bool _deleteWorkspaceTested = false;
        private bool _createWorkspaceTested = false;

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

            //  List Dialog Nodes
            _service.ListDialogNodes(OnListDialogNodes, OnFail, _createdWorkspaceId);
            while (!_listDialogNodesTested)
                yield return null;
            //  Create Dialog Nodes
            CreateDialogNode createDialogNode = new CreateDialogNode()
            {
                DialogNode = _dialogNodeName,
                Description = _dialogNodeDesc
            };
            _service.CreateDialogNode(OnCreateDialogNode, OnFail, _createdWorkspaceId, createDialogNode);
            while (!_createDialogNodeTested)
                yield return null;
            //  Get Dialog Node
            _service.GetDialogNode(OnGetDialogNode, OnFail, _createdWorkspaceId, _dialogNodeName);
            while (!_getDialogNodeTested)
                yield return null;
            //  Update Dialog Nodes
            string updatedDialogNodeName = _dialogNodeName + "_updated";
            string updatedDialogNodeDescription = _dialogNodeDesc + "_updated";
            UpdateDialogNode updateDialogNode = new UpdateDialogNode()
            {
                DialogNode = updatedDialogNodeName,
                Description = updatedDialogNodeDescription
            };
            _service.UpdateDialogNode(OnUpdateDialogNode, OnFail, _createdWorkspaceId, _dialogNodeName, updateDialogNode);
            while (!_updateDialogNodeTested)
                yield return null;
            //  List Counterexamples
            _service.ListCounterexamples(OnListCounterexamples, OnFail, _createdWorkspaceId);
            while (!_listCounterexamplesTested)
                yield return null;
            //  Create Counterexamples
            CreateCounterexample example = new CreateCounterexample()
            {
                Text = _createdCounterExampleText
            };
            _service.CreateCounterexample(OnCreateCounterexample, OnFail, _createdWorkspaceId, example);
            while (!_createCounterexampleTested)
                yield return null;
            //  Get Counterexample
            _service.GetCounterexample(OnGetCounterexample, OnFail, _createdWorkspaceId, _createdCounterExampleText);
            while (!_getCounterexampleTested)
                yield return null;
            //  Update Counterexamples
            string updatedCounterExampleText = _createdCounterExampleText + "-updated";
            UpdateCounterexample updateCounterExample = new UpdateCounterexample()
            {
                Text = updatedCounterExampleText
            };
            _service.UpdateCounterexample(OnUpdateCounterexample, OnFail, _createdWorkspaceId, _createdCounterExampleText, updateCounterExample);
            while (!_updateCounterexampleTested)
                yield return null;

            //  Delete Counterexample
            _service.DeleteCounterexample(OnDeleteCounterexample, OnFail, _createdWorkspaceId, updatedCounterExampleText);
            while (!_deleteCounterexampleTested)
                yield return null;
            //  Delete Dialog Node
            _service.DeleteDialogNode(OnDeleteDialogNode, OnFail, _createdWorkspaceId, updatedDialogNodeName);
            while (!_deleteDialogNodeTested)
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
            Log.Debug("ExampleAssistant.OnDeleteWorkspace()", "Workspace deleted");
            _deleteWorkspaceTested = true;
        }
        
        private void OnDeleteDialogNode(object response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnDeleteDialogNode()", "Dialog node deleted");
            _deleteDialogNodeTested = true;
        }

        private void OnDeleteCounterexample(object response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnDeleteCounterexample()", "Counterexample deleted");
            _deleteCounterexampleTested = true;
        }

        private void OnUpdateCounterexample(Counterexample response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnUpdateCounterexample()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _updateCounterexampleTested = true;
        }

        private void OnGetCounterexample(Counterexample response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnGetCounterexample()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _getCounterexampleTested = true;
        }

        private void OnCreateCounterexample(Counterexample response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnCreateCounterexample()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _createCounterexampleTested = true;
        }

        private void OnListCounterexamples(CounterexampleCollection response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnListCounterexamples()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _listCounterexamplesTested = true;
        }

        private void OnUpdateDialogNode(DialogNode response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnUpdateDialogNode()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _updateDialogNodeTested = true;
        }

        private void OnGetDialogNode(DialogNode response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnGetDialogNode()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _getDialogNodeTested = true;
        }

        private void OnCreateDialogNode(DialogNode response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnCreateDialogNode()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _createDialogNodeTested = true;
        }

        private void OnListDialogNodes(DialogNodeCollection response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnListDialogNodes()", "Response: {0}", customData["json"].ToString());
            Test(response != null);
            _listDialogNodesTested = true;
        }
        
        private void OnCreateWorkspace(Workspace response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnCreateWorkspace()", "Response: {0}", customData["json"].ToString());
            _createdWorkspaceId = response.WorkspaceId;
            Test(response != null);
            _createWorkspaceTested = true;
        }

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("TestAssistant.OnFail()", "Error received: {0}", error.ToString());
        }
    }
}
