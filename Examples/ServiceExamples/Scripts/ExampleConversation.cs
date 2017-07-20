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
using System;
using System.Collections;
using FullSerializer;
using System.IO;
using System.Collections.Generic;

public class ExampleConversation : MonoBehaviour
{
    private string _username;
    private string _password;
    private string _url;
    private string _workspaceId;
    //private string _token = "<authentication-token>";

    private Conversation _conversation;
    private string _conversationVersionDate = "2017-05-26";

    private string[] _questionArray = { "can you turn up the AC", "can you turn on the wipers", "can you turn off the wipers", "can you turn down the ac", "can you unlock the door" };
    private fsSerializer _serializer = new fsSerializer();
    private Dictionary<string, object> _context = null;
    private int _questionCount = -1;
    private bool _waitingForResponse = true;

    void Start()
    {
        LogSystem.InstallDefaultReactors();

        VcapCredentials vcapCredentials = new VcapCredentials();
        fsData data = null;

        //  Get credentials from a credential file defined in environmental variables in the VCAP_SERVICES format. 
        //  See https://www.ibm.com/watson/developercloud/doc/common/getting-started-variables.html.
        var environmentalVariable = Environment.GetEnvironmentVariable("VCAP_SERVICES");
        var fileContent = File.ReadAllText(environmentalVariable);

        //  Add in a parent object because Unity does not like to deserialize root level collection types.
        fileContent = Utility.AddTopLevelObjectToJson(fileContent, "VCAP_SERVICES");

        //  Convert json to fsResult
        fsResult r = fsJsonParser.Parse(fileContent, out data);
        if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

        //  Convert fsResult to VcapCredentials
        object obj = vcapCredentials;
        r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
        if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

        //  Set credentials from imported credntials
        Credential credential = vcapCredentials.VCAP_SERVICES["conversation"][0].Credentials;
        _username = credential.Username.ToString();
        _password = credential.Password.ToString();
        _url = credential.Url.ToString();
        _workspaceId = credential.WorkspaceId.ToString();

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_username, _password, _url);

        //  Or authenticate using token
        //Credentials credentials = new Credentials(_url)
        //{
        //    AuthenticationToken = _token
        //};

        _conversation = new Conversation(credentials);
        _conversation.VersionDate = _conversationVersionDate;

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        if (!_conversation.Message(OnMessage, _workspaceId, "hello"))
            Log.Debug("ExampleConversation", "Failed to message!");

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

        Log.Debug("ExampleConversation", "Conversation examples complete.");
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

        if (!_conversation.Message(OnMessage, _workspaceId, messageRequest))
            Log.Debug("ExampleConversation", "Failed to message!");
    }

    private void OnMessage(object resp, string data)
    {
        Log.Debug("ExampleConversation", "Conversation: Message Response: {0}", data);

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
            Log.Debug("ExampleConversation", "Failed to get context");
        _waitingForResponse = false;
    }
}
