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

public class ExampleCustomHeaders : MonoBehaviour
{
    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [SerializeField]
    private string _assistantUsername;
    [SerializeField]
    private string _assistantPassword;
    [SerializeField]
    private string _assistantUrl;
    [SerializeField]
    private string _assistantWorkspaceId;
    [SerializeField]
    private string _assistantVersionDate;
    #endregion

    private Assistant _service;
    private string _inputString = "Turn on the winshield wipers";

    void Start()
    {
        LogSystem.InstallDefaultReactors();

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_assistantUsername, _assistantPassword, _assistantUrl);

        _service = new Assistant(credentials);
        _service.VersionDate = _assistantVersionDate;

        Dictionary<string, object> input = new Dictionary<string, object>();
        input.Add("text", _inputString);
        MessageRequest messageRequest = new MessageRequest()
        {
            Input = input,
            AlternateIntents = true
        };

        //  Create custom data object
        Dictionary<string, object> customData = new Dictionary<string, object>();
        //  Create a dictionary of custom headers
        Dictionary<string, string> customHeaders = new Dictionary<string, string>();
        //  Add to the header dictionary
        customHeaders.Add("X-Watson-Metadata", "customer_id=some-customer-id");
        //  Add the header dictionary to the custom data object
        customData.Add(Constants.String.CUSTOM_REQUEST_HEADERS, customHeaders);

        if (customData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
        {
            Log.Debug("ExampleCustomHeader.Start()", "Custom Request headers:");
            foreach (KeyValuePair<string, string> kvp in customData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
            {
                Log.Debug("ExampleCustomHeader.Start()", "\t{0}: {1}", kvp.Key, kvp.Value);
            }
        }

        //  Call service using custom data object
        _service.Message(OnMessage, OnFail, _assistantWorkspaceId, messageRequest, customData: customData);
    }

    private void OnMessage(MessageResponse response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleCustomHeader.OnMessage()", "Response: {0}", customData["json"].ToString());

        if (customData.ContainsKey(Constants.String.RESPONSE_HEADERS))
        {
            Log.Debug("ExampleCustomHeader.OnMessage()", "Response headers:");

            foreach (KeyValuePair<string, string> kvp in customData[Constants.String.RESPONSE_HEADERS] as Dictionary<string, string>)
            {
                Log.Debug("ExampleCustomHeader.OnMessage()", "\t{0}: {1}", kvp.Key, kvp.Value);
            }
        }
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleCustomHeader.OnFail()", "Response: {0}", customData["json"].ToString());
        Log.Error("ExampleCustomHeader.OnFail()", "Error received: {0}", error.ToString());
    }
}
