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

using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using System.Collections;
using FullSerializer;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Connection;

public class ExampleConversation : MonoBehaviour
{
    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [Space(10)]
    [Tooltip("The service URL (optional). This defaults to \"https://gateway.watsonplatform.net/conversation/api\"")]
    [SerializeField]
    private string _serviceUrl;
    [Tooltip("The workspaceId to run the example.")]
    [SerializeField]
    private string _workspaceId;
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

    private Conversation _service;

    private string[] _questionArray = { "can you turn up the AC", "can you turn on the wipers", "can you turn off the wipers", "can you turn down the ac", "can you unlock the door" };
    private fsSerializer _serializer = new fsSerializer();
    private Dictionary<string, object> _context = null;
    private int _questionCount = -1;
    private bool _waitingForResponse = true;

    void Start()
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

        _service = new Conversation(credentials);
        _service.VersionDate = _versionDate;

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        if (!_service.Message(OnMessage, OnFail, _workspaceId, "hello"))
            Log.Debug("ExampleConversation.Message()", "Failed to message!");

        while (_waitingForResponse)
            yield return null;

        _waitingForResponse = true;
        _questionCount++;

        AskQuestion();
        while (_waitingForResponse)
            yield return null;

        _questionCount++;

        _waitingForResponse = true;

        AskQuestion();
        while (_waitingForResponse)
            yield return null;
        _questionCount++;

        _waitingForResponse = true;

        AskQuestion();
        while (_waitingForResponse)
            yield return null;
        _questionCount++;

        _waitingForResponse = true;

        AskQuestion();
        while (_waitingForResponse)
            yield return null;

        Log.Debug("ExampleConversation.Examples()", "Conversation examples complete.");
    }

    private void AskQuestion()
    {
        MessageRequest messageRequest = new MessageRequest()
        {
            input = new Dictionary<string, object>()
            {
                { "text", _questionArray[_questionCount] }
            },
            context = _context
        };

        if (!_service.Message(OnMessage, OnFail, _workspaceId, messageRequest))
            Log.Debug("ExampleConversation.AskQuestion()", "Failed to message!");
    }

    private void OnMessage(object resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleConversation.OnMessage()", "Conversation: Message Response: {0}", customData["json"].ToString());

        //  Convert resp to fsdata
        fsData fsdata = null;
        fsResult r = _serializer.TrySerialize(resp.GetType(), resp, out fsdata);
        if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

        //  Convert fsdata to MessageResponse
        MessageResponse messageResponse = new MessageResponse();
        object obj = messageResponse;
        r = _serializer.TryDeserialize(fsdata, obj.GetType(), ref obj);
        if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

        //  Set context for next round of messaging
        object _tempContext = null;
        (resp as Dictionary<string, object>).TryGetValue("context", out _tempContext);

        if (_tempContext != null)
            _context = _tempContext as Dictionary<string, object>;
        else
            Log.Debug("ExampleConversation.OnMessage()", "Failed to get context");
        _waitingForResponse = false;
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("ExampleConversation.OnFail()", "Error received: {0}", error.ToString());
    }
}
