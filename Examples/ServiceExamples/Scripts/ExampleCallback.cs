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

using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.Services.Discovery.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.Examples
{
    public class ExampleCallback : MonoBehaviour
    {
        #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
        [SerializeField]
        private string _conversationUsername;
        [SerializeField]
        private string _conversationPassword;
        [SerializeField]
        private string _conversationUrl;
        [SerializeField]
        private string _workspaceId;

        [SerializeField]
        private string _discoveryUsername;
        [SerializeField]
        private string _discoveryPassword;
        [SerializeField]
        private string _discoveryUrl;
        #endregion

        void Start()
        {
            LogSystem.InstallDefaultReactors();

            //  Create conversation instance
            Credentials conversationCredentials = new Credentials(_conversationUsername, _conversationPassword, _conversationUrl);
            Conversation conversation = new Conversation(conversationCredentials);
            conversation.VersionDate = "2017-05-26";

            //  Create discovery instance
            Credentials discoveryCredentials = new Credentials(_discoveryUsername, _discoveryPassword, _discoveryUrl);
            Discovery discovery = new Discovery(discoveryCredentials);
            discovery.VersionDate = "2016-12-01";

            //  Call with generic callbacks
            conversation.Message(OnSuccess, OnFail, _workspaceId, "");
            discovery.GetEnvironments(OnSuccess, OnFail);

            //  Call with sepcific callbacks
            conversation.Message(OnMessage, OnMessageFail, _workspaceId, "");
            discovery.GetEnvironments(OnGetEnvironments, OnGetEnvironmentsFail);
        }

        //  Generic success callback
        private void OnSuccess<T>(T resp, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleCallback.OnSuccess()", "Response received: {0}", customData["json"].ToString());
        }

        //  Generic fail callback
        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("ExampleCallback.OnFail()", "Error received: {0}", error.ToString());
        }

        //  OnMessage callback
        private void OnMessage(object resp, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleCallback.OnMessage()", "Response received: {0}", customData["json"].ToString());
        }

        //  OnGetEnvironments callback
        private void OnGetEnvironments(GetEnvironmentsResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleCallback.OnGetEnvironments()", "Response received: {0}", customData["json"].ToString());
        }

        //  OnMessageFail callback
        private void OnMessageFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("ExampleCallback.OnMessageFail()", "Error received: {0}", error.ToString());
        }

        //  OnGetEnvironmentsFail callback
        private void OnGetEnvironmentsFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("ExampleCallback.OnGetEnvironmentsFail()", "Error received: {0}", error.ToString());
        }
    }
}
