/**
* (C) Copyright IBM Corp. 2018, 2020.
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
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.Assistant.V2;
using IBM.Watson.Assistant.V2.Model;
using UnityEngine;

namespace IBM.Watson.Examples
{
    public class ExampleAssistantV2 : MonoBehaviour
    {
        #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
        [Space(10)]
        [Tooltip("The IAM apikey.")]
        [SerializeField]
        private string iamApikey;
        [Tooltip("The service URL (optional). This defaults to \"https://api.us-south.assistant.watson.cloud.ibm.com\"")]
        [SerializeField]
        private string serviceUrl;
        [Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD.")]
        [SerializeField]
        private string versionDate;
        [Tooltip("The assistantId to run the example.")]
        [SerializeField]
        private string assistantId;
        #endregion

        private AssistantService service;

        private bool createSessionTested = false;
        private bool messageTested0 = false;
        private bool messageTested1 = false;
        private bool messageTested2 = false;
        private bool messageTested3 = false;
        private bool messageTested4 = false;
        private bool deleteSessionTested = false;
        private string sessionId;

        private void Start()
        {
            LogSystem.InstallDefaultReactors();
            Runnable.Run(CreateService());
        }

        private IEnumerator CreateService()
        {
            if (string.IsNullOrEmpty(iamApikey))
            {
                throw new IBMException("Plesae provide IAM ApiKey for the service.");
            }

            //  Create credential and instantiate service
            IamAuthenticator authenticator = new IamAuthenticator(apikey: iamApikey);

            //  Wait for tokendata
            while (!authenticator.CanAuthenticate())
                yield return null;

            service = new AssistantService(versionDate, authenticator);
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                service.SetServiceUrl(serviceUrl);
            }

            Runnable.Run(Examples());
        }

        private IEnumerator Examples()
        {
            Log.Debug("ExampleAssistantV2.RunExample()", "Attempting to CreateSession");
            service.CreateSession(OnCreateSession, assistantId);

            while (!createSessionTested)
            {
                yield return null;
            }

            Log.Debug("ExampleAssistantV2.RunExample()", "Attempting to Message");
            service.Message(OnMessage0, assistantId, sessionId);

            while (!messageTested0)
            {
                yield return null;
            }

            Log.Debug("ExampleAssistantV2.RunExample()", "Are you open on Christmas?");

            var input1 = new MessageInput()
            {
                Text = "Are you open on Christmas?",
                Options = new MessageInputOptions()
                {
                    ReturnContext = true
                }
            };

            service.Message(OnMessage1, assistantId, sessionId, input: input1);

            while (!messageTested1)
            {
                yield return null;
            }

            Log.Debug("ExampleAssistantV2.RunExample()", "What are your hours?");

            var input2 = new MessageInput()
            {
                Text = "What are your hours?",
                Options = new MessageInputOptions()
                {
                    ReturnContext = true
                }
            };

            service.Message(OnMessage2, assistantId, sessionId, input: input2);

            while (!messageTested2)
            {
                yield return null;
            }

            Log.Debug("ExampleAssistantV2.RunExample()", "I'd like to make an appointment for 12pm.");

            var input3 = new MessageInput()
            {
                Text = "I'd like to make an appointment for 12pm.",
                Options = new MessageInputOptions()
                {
                    ReturnContext = true
                }

            };
            service.Message(OnMessage3, assistantId, sessionId, input: input3);

            while (!messageTested3)
            {
                yield return null;
            }

            Log.Debug("ExampleAssistantV2.RunExample()", "On Friday please.");

            //Dictionary<string, string> userDefinedDictionary = new Dictionary<string, string>();
            //userDefinedDictionary.Add("name", "Watson");

            //Dictionary<string, object> skillDictionary = new Dictionary<string, object>();
            //skillDictionary.Add("user_defined", userDefinedDictionary);

            //Dictionary<string, object> skills = new Dictionary<string, object>();
            //skills.Add("main skill", skillDictionary);



            SerializableDictionary<string, string> userDefinedDictionary = new SerializableDictionary<string, string>();
            userDefinedDictionary.Add("name", "Watson");

            //MessageContextSkill skillDictionary = new MessageContextSkill();
            //skillDictionary.UserDefined = userDefinedDictionary;

            //MessageContextSkills skills = new MessageContextSkills();
            //skills.MainSkill = skillDictionary;


            var input4 = new MessageInput()
            {
                Text = "On Friday please.",
                Options = new MessageInputOptions()
                {
                    ReturnContext = true
                }
            };

            var context = new MessageContext()
            {
                //Skills = skills
            };

            service.Message(OnMessage4, assistantId, sessionId, input: input4, context: context);

            while (!messageTested4)
            {
                yield return null;
            }

            Log.Debug("ExampleAssistantV2.RunExample()", "Attempting to delete session");
            service.DeleteSession(OnDeleteSession, assistantId, sessionId);

            while (!deleteSessionTested)
            {
                yield return null;
            }

            Log.Debug("ExampleAssistantV2.Examples()", "Assistant examples complete.");
        }

        private void OnDeleteSession(DetailedResponse<object> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV2.OnDeleteSession()", "Session deleted.");
            deleteSessionTested = true;
        }

        private void OnMessage0(DetailedResponse<MessageResponse> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV2.OnMessage0()", "response: {0}", response.Result.Output.Generic[0].Text);
            messageTested0 = true;
        }

        private void OnMessage1(DetailedResponse<MessageResponse> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV2.OnMessage1()", "response: {0}", response.Result.Output.Generic[0].Text);

            messageTested1 = true;
        }

        private void OnMessage2(DetailedResponse<MessageResponse> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV2.OnMessage2()", "response: {0}", response.Result.Output.Generic[0].Text);
            messageTested2 = true;
        }

        private void OnMessage3(DetailedResponse<MessageResponse> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV2.OnMessage3()", "response: {0}", response.Result.Output.Generic[0].Text);
            messageTested3 = true;
        }
        private void OnMessage4(DetailedResponse<MessageResponse> response, IBMError error)
        {
            //object tempSkill = null;
            //(response.Result.Context.Skills as Dictionary<string, object>).TryGetValue("main skill", out tempSkill);
            //object tempUserDefined = null;
            //(tempSkill as Dictionary<string, object>).TryGetValue("user_defined", out tempUserDefined);
            //string tempName = null;
            //(tempUserDefined as Dictionary<string, string>).TryGetValue("name", out tempName);

            //Log.Debug("ExampleAssistantV2.OnMessage4()", "response: {0}", response.Result.Output.Generic[0].Text);

            //object e = response.Result as object;
            //Dictionary<string, object> e2 = e as Dictionary<string, object>;
            //Dictionary<string, object> context = e2["context"] as Dictionary<string, object>;
            //Dictionary<string, object> skills = context["skills"] as Dictionary<string, object>;
            //Dictionary<string, object> main_skill = skills["main skill"] as Dictionary<string, object>;
            //Dictionary<string, object> user_defined = main_skill["user_defined"] as Dictionary<string, object>;

            //string name = user_defined["name"] as string;
            //Log.Debug("GenericSerialization", "test: {0}", name);
            messageTested4 = true;
        }

        private void OnCreateSession(DetailedResponse<SessionResponse> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV2.OnCreateSession()", "Session: {0}", response.Result.SessionId);
            sessionId = response.Result.SessionId;
            createSessionTested = true;
        }
    }
}
