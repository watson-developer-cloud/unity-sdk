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
using IBM.Watson.DeveloperCloud.Services.Assistant.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExampleAssistantV1Message : MonoBehaviour
{
    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [Space(10)]
    [Header("Output")]
    [Tooltip("Text field to display the results of streaming.")]
    public Text ResultsField;
    [Header("CF Authentication")]
    [Tooltip("The service URL (optional). This defaults to \"https://gateway.watsonplatform.net/assistant/api\"")]
    [SerializeField]
    private string _serviceUrl;
    [Tooltip("The authentication username.")]
    [SerializeField]
    private string _username;
    [Tooltip("The authentication password.")]
    [SerializeField]
    private string _password;
    [Tooltip("The workspaceId to run the example.")]
    [SerializeField]
    private string _workspaceId;
    [Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD.")]
    [SerializeField]
    private string _versionDate;
    [Header("IAM Authentication")]
    [Tooltip("The IAM apikey.")]
    [SerializeField]
    private string _iamApikey;
    [Tooltip("The IAM url used to authenticate the apikey (optional). This defaults to \"https://iam.bluemix.net/identity/token\".")]
    [SerializeField]
    private string _iamUrl;
    #endregion

    private string _createdWorkspaceId;

    private Assistant _service;

    private fsSerializer _serializer = new fsSerializer();

    private Dictionary<string, object> _context = null;

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

        _service = new Assistant(credentials);
        _service.VersionDate = _versionDate;
    }

    public void ServiceSendMessage(string inputString)
    {
        Dictionary<string, object> input = new Dictionary<string, object>();
        input.Add("text", inputString);
        MessageRequest messageRequest = new MessageRequest()
        {
            Input = input,
            Context = _context
        };
        _service.Message(ServiceOnMessage, OnFail, _workspaceId, messageRequest);
    }


    private void ServiceOnMessage(object response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleAssistantV1Message.ServiceOnMessage()", "Response: {0}", customData["json"].ToString());

        //  Convert resp to fsdata
        fsData fsdata = null;
        fsResult r = _serializer.TrySerialize(response.GetType(), response, out fsdata);
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
        (response as Dictionary<string, object>).TryGetValue("context", out _tempContext);

        if (_tempContext != null)
            _context = _tempContext as Dictionary<string, object>;
        else
            Log.Debug("ExampleAssistantV1Message.ServiceOnMessage()", "Failed to get context");

        string text = "";

        // Intents
        if (messageResponse != null && messageResponse.Intents.Length > 0)
        {
        	text += "Intents: [";
        	foreach (var intent in messageResponse.Intents)
        	{
        		string intentStr = string.Format("{0} ({1:0.00}) ", intent.Intent, intent.Confidence);
        		text += intentStr;
        	}
        	text += "]";
        }

        text += " ";

        // Entities
        if (messageResponse != null && messageResponse.Entities.Length > 0)
        {
        	text += "Entities: [";
        	foreach (var entity in messageResponse.Entities)
        	{
        		string entityStr = string.Format("{0} {1} ({2:0.00}) ", entity.Entity, entity.Value, entity.Confidence);
        		text += entityStr;
        	}
        	text += "]";
        }
        Log.Debug("ExampleAssistantV1Message.ServiceOnMessage()", text);
        if (ResultsField)
        {
        	ResultsField.text = text;
        }
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleAssistantV1Message.OnFail()", "Response: {0}", customData["json"].ToString());
        Log.Error("ExampleAssistantV1Message.OnFail()", "Error received: {0}", error.ToString());
    }
}
