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
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.WatsonDeveloperCloud.Assistant.v2;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v2
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
        [Header("CF Authentication")]
        [Tooltip("The authentication username.")]
        [SerializeField]
        private string _username;
        [Tooltip("The authentication password.")]
        [SerializeField]
        private string _password;
        [Header("IAM Authentication")]
        [Tooltip("The IAM apikey.")]
        [SerializeField]
        private string _iamApikey;
        [Tooltip("The IAM url used to authenticate the apikey (optional). This defaults to \"https://iam.bluemix.net/identity/token\".")]
        [SerializeField]
        private string _iamUrl;
        #endregion

        private Assistant _service;

        private bool _createSessionTested = false;
        private bool _messageTested = false;
        private bool _deleteSessionTested = false;
        private string _sessionId;

        private void Start()
        {
            LogSystem.InstallDefaultReactors();
            Runnable.Run(CreateService());
        }

        private IEnumerator CreateService()
        {
            //  Create credential and instantiate service
            Credentials credentials = null;
            if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
            {
                //  Authenticate using username and password
                credentials = new Credentials(_username, _password, _serviceUrl);
            }
            else if (!string.IsNullOrEmpty(_iamApikey))
            {
                //  Authenticate using iamApikey
                TokenOptions tokenOptions = new TokenOptions()
                {
                    IamApiKey = _iamApikey,
                    IamUrl = _iamUrl
                };

                credentials = new Credentials(tokenOptions, _serviceUrl);

                //  Wait for tokendata
                while (!credentials.HasIamTokenData())
                    yield return null;
            }
            else
            {
                throw new WatsonException("Please provide either username and password or IAM apikey to authenticate the service.");
            }

            _service = new Assistant(credentials);
            _service.VersionDate = _versionDate;

            Runnable.Run(Examples());
        }

        private IEnumerator Examples()
        {
            Log.Debug("ExampleAssistantV2.Examples()", "Attempting to CreateSession");
            _service.CreateSession(OnCreateSession, OnFail, _assistantId);

            while (!_createSessionTested)
            {
                yield return null;
            }

            Log.Debug("ExampleAssistantV2.Examples()", "Attempting to Message");
            _service.Message(OnMessage, OnFail, _assistantId, _sessionId);

            while (!_messageTested)
            {
                yield return null;
            }

            Log.Debug("ExampleAssistantV2.Examples()", "Attempting to DeleteSession");
            _service.DeleteSession(OnDeleteSession, OnFail, _assistantId, _sessionId);

            while (!_deleteSessionTested)
            {
                yield return null;
            }

            Log.Debug("ExampleAssistantV2.Examples()", "Assistant examples complete.");
        }

        private void OnDeleteSession(object response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV2.OnDeleteSession()", "Session deleted.");
            _createSessionTested = true;
        }

        private void OnMessage(MessageResponse response, Dictionary<string, object> customData)
        {
            _messageTested = true;
        }

        private void OnCreateSession(SessionResponse response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV2.OnCreateSession()", "Session: {0}", response.SessionId);
            _sessionId = response.SessionId;
            _createSessionTested = true;
        }

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistantV2.OnFail()", "Call failed: {0}: {1}", error.ErrorCode, error.ErrorMessage);
        }
    }
}
