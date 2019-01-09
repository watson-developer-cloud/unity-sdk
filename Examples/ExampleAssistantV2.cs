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
#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using IBM.Watson.Assistant.V2;
using UnityEngine;

namespace IBM.Watson.Examples
{
    public class ExampleAssistantV2 : MonoBehaviour
    {
        #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
        [Space(10)]
        [Tooltip("The service URL (optional). This defaults to \"https://gateway.watsonplatform.net/assistant/api\"")]
        [SerializeField]
        private string _serviceUrl;
        [Tooltip("The assistantId to run the example.")]
        [SerializeField]
        private string _assistantId;
        [Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD.")]
        [SerializeField]
        private string _versionDate;
        [Header("IAM Authentication")]
        [Tooltip("The IAM apikey.")]
        [SerializeField]
        private string _iamApikey;
        #endregion
        private AssistantService _service;

        private bool _createSessionTested = false;
        private bool _messageTested0 = false;
        private bool _messageTested1 = false;
        private bool _messageTested2 = false;
        private bool _messageTested3 = false;
        private bool _messageTested4 = false;
        private bool _deleteSessionTested = false;
        private string _sessionId;

        private void Start()
        {
            LogSystem.InstallDefaultReactors();
            Runnable.Run(CreateService());
        }

        private IEnumerator CreateService()
        {
            if (string.IsNullOrEmpty(_iamApikey))
            {
                throw new WatsonException("Plesae provide IAM ApiKey for the service.");
            }

            //  Create credential and instantiate service
            Credentials credentials = null;

            //  Authenticate using iamApikey
            TokenOptions tokenOptions = new TokenOptions()
            {
                IamApiKey = _iamApikey
            };

            credentials = new Credentials(tokenOptions, _serviceUrl);

            //  Wait for tokendata
            while (!credentials.HasIamTokenData())
                yield return null;

            _service = new AssistantService(credentials);
            _service.VersionDate = _versionDate;

            Runnable.Run(Examples());
        }

        private IEnumerator Examples()
        {
            Log.Debug("ExampleAssistantV2.RunTest()", "Attempting to CreateSession");
            _service.CreateSession(OnCreateSession, _assistantId);

            while (!_createSessionTested)
            {
                yield return null;
            }

            Log.Debug("ExampleAssistantV2.RunTest()", "Attempting to Message");
            _service.Message(OnMessage0, _assistantId, _sessionId);

            while (!_messageTested0)
            {
                yield return null;
            }

            Log.Debug("ExampleAssistantV2.RunTest()", "Are you open on Christmas?");

            MessageRequest messageRequest1 = new MessageRequest()
            {
                Input = new MessageInput()
                {
                    Text = "Are you open on Christmas?",
                    Options = new MessageInputOptions()
                    {
                        ReturnContext = true
                    }
                }
            };
            _service.Message(OnMessage1, _assistantId, _sessionId, messageRequest1);

            while (!_messageTested1)
            {
                yield return null;
            }

            Log.Debug("ExampleAssistantV2.RunTest()", "What are your hours?");
            MessageRequest messageRequest2 = new MessageRequest()
            {
                Input = new MessageInput()
                {
                    Text = "What are your hours?",
                    Options = new MessageInputOptions()
                    {
                        ReturnContext = true
                    }
                }
            };
            _service.Message(OnMessage2, _assistantId, _sessionId, messageRequest2);

            while (!_messageTested2)
            {
                yield return null;
            }

            Log.Debug("ExampleAssistantV2.RunTest()", "I'd like to make an appointment for 12pm.");
            MessageRequest messageRequest3 = new MessageRequest()
            {
                Input = new MessageInput()
                {
                    Text = "I'd like to make an appointment for 12pm.",
                    Options = new MessageInputOptions()
                    {
                        ReturnContext = true
                    }
                }
            };
            _service.Message(OnMessage3, _assistantId, _sessionId, messageRequest3);

            while (!_messageTested3)
            {
                yield return null;
            }

            Log.Debug("ExampleAssistantV2.RunTest()", "On Friday please.");

            Dictionary<string, string> userDefinedDictionary = new Dictionary<string, string>();
            userDefinedDictionary.Add("name", "Watson");

            Dictionary<string, object> skillDictionary = new Dictionary<string, object>();
            skillDictionary.Add("user_defined", userDefinedDictionary);

            MessageContextSkills skills = new MessageContextSkills();
            skills.Add("main skill", skillDictionary);

            //SerializableDictionary<string, string> userDefinedDictionary = new SerializableDictionary<string, string>();
            //userDefinedDictionary.Add("name", "Watson");

            //SerializableDictionary<string, SerializableDictionary<string, string>> skillDictionary = new SerializableDictionary<string, SerializableDictionary<string, string>>();
            //skillDictionary.Add("user_defined", userDefinedDictionary);

            //MessageContextSkills skills = new MessageContextSkills();
            //skills.Add("main skill", skillDictionary);

            MessageRequest messageRequest4 = new MessageRequest()
            {
                Input = new MessageInput()
                {
                    Text = "On Friday please.",
                    Options = new MessageInputOptions()
                    {
                        ReturnContext = true
                    }
                },
                Context = new MessageContext()
                {
                    Skills = skills
                }
            };
            _service.Message(OnMessage4, _assistantId, _sessionId, messageRequest4);

            while (!_messageTested4)
            {
                yield return null;
            }

            Log.Debug("ExampleAssistantV2.RunTest()", "Attempting to delete session");
            _service.DeleteSession(OnDeleteSession, _assistantId, _sessionId);

            while (!_deleteSessionTested)
            {
                yield return null;
            }

            Log.Debug("ExampleAssistantV2.Examples()", "Assistant examples complete.");
        }

        private void OnDeleteSession(WatsonResponse<object> response, WatsonError error, System.Collections.Generic.Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV2.OnDeleteSession()", "Session deleted.");
            _createSessionTested = true;
        }

        private void OnMessage0(WatsonResponse<MessageResponse> response, WatsonError error, System.Collections.Generic.Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV2.OnMessage0()", "response: {0}", response.Result.Output.Generic[0].Text);
            _messageTested0 = true;
        }

        private void OnMessage1(WatsonResponse<MessageResponse> response, WatsonError error, System.Collections.Generic.Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV2.OnMessage1()", "response: {0}", response.Result.Output.Generic[0].Text);

            _messageTested1 = true;
        }
        
        private void OnMessage2(WatsonResponse<MessageResponse> response, WatsonError error, System.Collections.Generic.Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV2.OnMessage2()", "response: {0}", response.Result.Output.Generic[0].Text);
            _messageTested2 = true;
        }

        private void OnMessage3(WatsonResponse<MessageResponse> response, WatsonError error, System.Collections.Generic.Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV2.OnMessage3()", "response: {0}", response.Result.Output.Generic[0].Text);
            _messageTested3 = true;
        }
        private void OnMessage4(WatsonResponse<MessageResponse> response, WatsonError error, System.Collections.Generic.Dictionary<string, object> customData)
        {
            //Log.Debug("ExampleAssistantV2.OnMessage4()", "response: {0}", response.Result.Output.Generic[0].Text);

            object e = response.Result as object;
            Dictionary<string, object> e2 = e as Dictionary<string, object>;
            Dictionary<string, object> context = e2["context"] as Dictionary<string, object>;
            Dictionary<string, object> skills = context["skills"] as Dictionary<string, object>;
            Dictionary<string, object> main_skill = skills["main skill"] as Dictionary<string, object>;
            Dictionary<string, object> user_defined = main_skill["user_defined"] as Dictionary<string, object>;

            string name = user_defined["name"] as string;
            Log.Debug("GenericSerialization", "test: {0}", name);
            _messageTested4 = true;
        }

        private void OnCreateSession(WatsonResponse<SessionResponse> response, WatsonError error, System.Collections.Generic.Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV2.OnCreateSession()", "Session: {0}", response.Result.SessionId);
            _sessionId = response.Result.SessionId;
            _createSessionTested = true;
        }
    }
}
