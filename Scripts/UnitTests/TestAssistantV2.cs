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
using IBM.Watson.DeveloperCloud.Services.Assistant.v2;
using IBM.Watson.DeveloperCloud.UnitTests;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.WatsonDeveloperCloud.Assistant.v2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Assets.Watson.Scripts.UnitTests
{
    public class TestAssistantV2 : UnitTest
    {
        private string _username;
        private string _password;
        private fsSerializer _serializer = new fsSerializer();

        private Assistant _service;
        private string _assistantVersionDate = "2018-09-20";

        private string _assistantId;
        private string _sessionId;

        private bool _createSessionTested = false;
        private bool _messageTested = false;
        private bool _deleteSessionTested = false;

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
            _assistantId = credential.AssistantId.ToString();

            //  Create credential and instantiate service
            Credentials credentials = new Credentials(_username, _password, _url);

            _service = new Assistant(credentials);
            _service.VersionDate = _assistantVersionDate;

            Log.Debug("TestAssistantV2.RunTest()", "Attempting to CreateSession");
            _service.CreateSession(OnCreateSession, OnFail, _assistantId);

            while (!_createSessionTested)
            {
                yield return null;
            }

            Log.Debug("TestAssistantV2.RunTest()", "Attempting to Message");
            _service.Message(OnMessage, OnFail, _assistantId, _sessionId);

            while (!_messageTested)
            {
                yield return null;
            }

            Log.Debug("TestAssistantV2.RunTest()", "Attempting to DeleteSession");
            _service.DeleteSession(OnDeleteSession, OnFail, _assistantId, _sessionId);

            while (!_deleteSessionTested)
            {
                yield return null;
            }
        }

        private void OnDeleteSession(object response, Dictionary<string, object> customData)
        {
            Log.Debug("TestAssistantV2.OnDeleteSession()", "Session deleted.");
            Test(response != null);
            _deleteSessionTested = true;
        }

        private void OnMessage(MessageResponse response, Dictionary<string, object> customData)
        {
            Test(response != null);
            _messageTested = true;
        }

        private void OnCreateSession(SessionResponse response, Dictionary<string, object> customData)
        {
            Log.Debug("TestAssistantV2.OnCreateSession()", "Session: {0}", response.SessionId);
            _sessionId = response.SessionId;
            Test(response != null);
            _createSessionTested = true;
        }

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Debug("TestAssistantV2.OnFail()", "Call failed: {0}: {1}", error.ErrorCode, error.ErrorMessage);
        }
    }
}
