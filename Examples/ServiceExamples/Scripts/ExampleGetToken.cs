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

using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;
using UnityEngine;

public class ExampleGetToken : MonoBehaviour
{
    private string _username = null;
    private string _password = null;
    private string _url = null;
    private string _workspaceId = null;

    private AuthenticationToken _authenticationToken;
    private bool _receivedAuthToken = false;
    private string _conversationVersionDate = "2017-05-26";

    void Start ()
    {
        LogSystem.InstallDefaultReactors();
        Runnable.Run(Example());
    }

    private IEnumerator Example()
    {
        //  Get token
        if (!Utility.GetToken(OnGetToken, _url, _username, _password))
            Log.Debug("ExampleGetToken", "Failed to get token.");

        while (!_receivedAuthToken)
            yield return null;

        //  Use token to authenticate Conversation call
        Message();
    }

    private void OnGetToken(AuthenticationToken authenticationToken, string customData)
    {
        _authenticationToken = authenticationToken;
        Log.Debug("ExampleGetToken", "created: {0} | time to expiration: {1} minutes | token: {2}", _authenticationToken.Created, _authenticationToken.TimeUntilExpiration, _authenticationToken.Token);
        _receivedAuthToken = true;
    }

    private IEnumerator GetTokenTimeRemaining(float time)
    {
        yield return new WaitForSeconds(time);
        Log.Debug("ExampleGetToken", "created: {0} | time to expiration: {1} minutes | token: {2}", _authenticationToken.Created, _authenticationToken.TimeUntilExpiration, _authenticationToken.Token);
    }

    private void Message()
    {
        Credentials credentials = new Credentials()
        {
            AuthenticationToken = _authenticationToken.Token,
            Url = _url
        };

        Conversation conversation = new Conversation(credentials);
        conversation.VersionDate = _conversationVersionDate;

        conversation.Message(OnMessage, _workspaceId, "hello");
    }

    private void OnMessage(object resp, string customData)
    {
        Log.Debug("ExampleGetToken", "message response: {0}", customData);

        //  Check token time remaining
        Runnable.Run(GetTokenTimeRemaining(0f));
    }
}
