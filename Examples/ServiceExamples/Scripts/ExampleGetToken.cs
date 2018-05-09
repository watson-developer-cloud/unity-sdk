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
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleGetToken : MonoBehaviour
{
    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [SerializeField]
    private string _conversationUsername;
    [SerializeField]
    private string _conversationPassword;
    [SerializeField]
    private string _conversationUrl;
    [SerializeField]
    private string _conversationWorkspaceId;
    [SerializeField]
    private string _conversationVersionDate;
    #endregion

    private AuthenticationToken _authenticationToken;
    private bool _receivedAuthToken = false;

    void Start ()
    {
        LogSystem.InstallDefaultReactors();
        Runnable.Run(Example());
    }

    private IEnumerator Example()
    {
        //  Get token
        if (!Utility.GetWatsonToken(OnGetWatsonToken, _conversationUrl, _conversationUsername, _conversationPassword))
            Log.Debug("ExampleGetToken.GetToken()", "Failed to get token.");

        while (!_receivedAuthToken)
            yield return null;

        //  Use token to authenticate Conversation call
        Message();
    }

    private void OnGetWatsonToken(AuthenticationToken authenticationToken, string customData)
    {
        _authenticationToken = authenticationToken;
        Log.Debug("ExampleGetToken.OnGetToken()", "created: {0} | time to expiration: {1} minutes | token: {2}", _authenticationToken.Created, _authenticationToken.TimeUntilExpiration, _authenticationToken.Token);
        _receivedAuthToken = true;
    }

    private IEnumerator GetWatsonTokenTimeRemaining(float time)
    {
        yield return new WaitForSeconds(time);
        Log.Debug("ExampleGetToken.GetTokenTimeRemaining()", "created: {0} | time to expiration: {1} minutes | token: {2}", _authenticationToken.Created, _authenticationToken.TimeUntilExpiration, _authenticationToken.Token);
    }

    private void Message()
    {
        Credentials credentials = new Credentials()
        {
            WatsonAuthenticationToken = _authenticationToken.Token,
            Url = _conversationUrl
        };

        Conversation conversation = new Conversation(credentials);
        conversation.VersionDate = _conversationVersionDate;

        conversation.Message(OnMessage, OnFail, _conversationWorkspaceId, "hello");
    }

    private void OnMessage(object resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleGetToken.OnMessage()", "message response: {0}", customData);

        //  Check token time remaining
        Runnable.Run(GetWatsonTokenTimeRemaining(0f));
    }
    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("ExampleGetToken.OnFail()", "Error received: {0}", error.ToString());
    }
}
