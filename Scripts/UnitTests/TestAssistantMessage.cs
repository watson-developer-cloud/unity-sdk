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

using FullSerializer;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.Assistant.v1;
using IBM.Watson.DeveloperCloud.UnitTests;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Assets.Watson.Scripts.UnitTests
{
    public class TestAssistantMessage : UnitTest
    {
        private string _username = null;
        private string _password = null;
        private string _workspaceId = null;

        private Assistant _service;
        private string _assistantVersionDate = "2017-05-26";
        private fsSerializer _serializer = new fsSerializer();

        private string _inputString = "Hello";
        private string _conversationString0 = "unlock the door";
        private string _conversationString1 = "turn on the ac";
        private string _conversationString2 = "turn down the radio";
        private static string _lastIntent = null;

        private bool _messageTested = false;
        private static Dictionary<string, object> _context = null;

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

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("TestAssistantWorkspacesWorkspaces.OnFail()", "Error received: {0}", error.ToString());
        }
    }
}
