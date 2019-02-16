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
        private fsSerializer _serializer = new fsSerializer();

        private Assistant _service;
        private string _assistantVersionDate = "2018-09-20";

        private string _assistantId;
        private string _sessionId;

        private bool _autoCreateSessionTested = false;
        private bool _createSessionTested = false;
        private bool _messageTested0 = false;
        private bool _messageTested1 = false;
        private bool _messageTested2 = false;
        private bool _messageTested3 = false;
        private bool _messageTested4 = false;
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
            //  Create credential and instantiate service
            TokenOptions tokenOptions = new TokenOptions()
            {
                IamApiKey = credential.IamApikey,
            };
            _assistantId = credential.AssistantId.ToString();

            //  Create credential and instantiate service
            Credentials credentials = new Credentials(tokenOptions, _url);

            //  Wait for tokendata
            while (!credentials.HasIamTokenData())
                yield return null;

            _service = new Assistant(credentials);
            _service.VersionDate = _assistantVersionDate;

            //  Test Assistant using loaded credentials
            Assistant autoAssisant = new Assistant();
            while (!autoAssisant.Credentials.HasIamTokenData())
                yield return null;
            autoAssisant.VersionDate = _assistantVersionDate;
            autoAssisant.CreateSession(OnAutoCreateSession, OnFail, _assistantId);
            while (!_autoCreateSessionTested)
                yield return null;

            Log.Debug("TestAssistantV2.RunTest()", "Attempting to CreateSession");
            _service.CreateSession(OnCreateSession, OnFail, _assistantId);

            while (!_createSessionTested)
            {
                yield return null;
            }

            Log.Debug("TestAssistantV2.RunTest()", "Attempting to Message");
            _service.Message(OnMessage0, OnFail, _assistantId, _sessionId);

            while (!_messageTested0)
            {
                yield return null;
            }

            Log.Debug("TestAssistantV2.RunTest()", "Are you open on Christmas?");
            MessageRequest messageRequest1 = new MessageRequest()
            {
                Input = new MessageInput()
                {
                    Text = "Are you open on Christmas?"
                }
            };
            _service.Message(OnMessage1, OnFail, _assistantId, _sessionId, messageRequest1);

            while (!_messageTested1)
            {
                yield return null;
            }

            Log.Debug("TestAssistantV2.RunTest()", "What are your hours?");
            MessageRequest messageRequest2 = new MessageRequest()
            {
                Input = new MessageInput()
                {
                    Text = "What are your hours?"
                }
            };
            _service.Message(OnMessage2, OnFail, _assistantId, _sessionId, messageRequest2);

            while (!_messageTested2)
            {
                yield return null;
            }

            Log.Debug("TestAssistantV2.RunTest()", "I'd like to make an appointment for 12pm.");
            MessageRequest messageRequest3 = new MessageRequest()
            {
                Input = new MessageInput()
                {
                    Text = "I'd like to make an appointment for 12pm."
                }
            };
            _service.Message(OnMessage3, OnFail, _assistantId, _sessionId, messageRequest3);

            while (!_messageTested3)
            {
                yield return null;
            }

            Log.Debug("TestAssistantV2.RunTest()", "On Friday please.");
            MessageRequest messageRequest4 = new MessageRequest()
            {
                Input = new MessageInput()
                {
                    Text = "On Friday please."
                }
            };
            _service.Message(OnMessage4, OnFail, _assistantId, _sessionId, messageRequest4);

            while (!_messageTested4)
            {
                yield return null;
            }

            Log.Debug("TestAssistantV2.RunTest()", "Attempting to delete session");
            _service.DeleteSession(OnDeleteSession, OnFail, _assistantId, _sessionId);

            while (!_deleteSessionTested)
            {
                yield return null;
            }
        }

        private void OnAutoCreateSession(SessionResponse response, Dictionary<string, object> customData)
        {
            Log.Debug("TestAssistantV2.OnAutoCreateSession()", "Session: {0}", response.SessionId);
            Test(response != null);
            _autoCreateSessionTested = true;
        }

        private void OnDeleteSession(object response, Dictionary<string, object> customData)
        {
            Log.Debug("TestAssistantV2.OnDeleteSession()", "Session deleted.");
            Test(response != null);
            _deleteSessionTested = true;
        }

        private void OnMessage0(MessageResponse response, Dictionary<string, object> customData)
        {
            Test(!string.IsNullOrEmpty(response.Output.Generic[0].Text));
            Log.Debug("TestAssistantV2.OnMessage0()", "response: {0}", response.Output.Generic[0].Text);
            _messageTested0 = true;
        }

        private void OnMessage1(MessageResponse response, Dictionary<string, object> customData)
        {
            Test(!string.IsNullOrEmpty(response.Output.Generic[0].Text));
            Test(response.Output.Entities[0].Value == "christmas");
            Test(response.Output.Entities[0].Entity == "holiday");
            Log.Debug("TestAssistantV2.OnMessage1()", "response: {0}", response.Output.Generic[0].Text);

            _messageTested1 = true;
        }

        private void OnMessage2(MessageResponse response, Dictionary<string, object> customData)
        {
            Test(!string.IsNullOrEmpty(response.Output.Generic[0].Text));
            Test(response.Output.Intents[0].Intent == "Customer_Care_Store_Hours");
            Log.Debug("TestAssistantV2.OnMessage2()", "response: {0}", response.Output.Generic[0].Text);
            _messageTested2 = true;
        }

        private void OnMessage3(MessageResponse response, Dictionary<string, object> customData)
        {
            Test(!string.IsNullOrEmpty(response.Output.Generic[0].Text));
            Test(response.Output.Intents[0].Intent == "Customer_Care_Appointments");
            Log.Debug("TestAssistantV2.OnMessage3()", "response: {0}", response.Output.Generic[0].Text);
            _messageTested3 = true;
        }
        private void OnMessage4(MessageResponse response, Dictionary<string, object> customData)
        {
            Test(!string.IsNullOrEmpty(response.Output.Generic[0].Text));
            Test(response.Output.Intents[0].Intent == "Customer_Care_Appointments");
            Log.Debug("TestAssistantV2.OnMessage4()", "response: {0}", response.Output.Generic[0].Text);
            _messageTested4 = true;
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
