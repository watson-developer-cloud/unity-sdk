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
using IBM.Watson.DeveloperCloud.Services.DocumentConversion.v1;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;
using IBM.Watson.DeveloperCloud.Connection;
using System.Collections.Generic;

public class ExampleDocumentConversion : MonoBehaviour
{
    private DocumentConversion _documentConversion;
    private string _username = null;
    private string _password = null;
    private string _url = null;
    
    private string _examplePath;
    private string _conversionTarget = ConversionTarget.NormalizedHtml;
    private bool _convertDocumentTested = false;

    void Start()
    {
        LogSystem.InstallDefaultReactors();

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_username, _password, _url);

        _documentConversion = new DocumentConversion(credentials);
        _examplePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/watson_beats_jeopardy.html";

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        if (!_documentConversion.ConvertDocument(OnConvertDocument, OnFail, _examplePath, _conversionTarget))
            Log.Debug("ExampleDocumentConversion.ConvertDocument()", "Document conversion failed!");

        while (!_convertDocumentTested)
            yield return null;

        Log.Debug("ExampleDoucmentConversion.Examples()", "Document conversion examples complete.");
    }

    private void OnConvertDocument(ConvertedDocument documentConversionResponse, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleDoucmentConversion.OnConvertDocument()", "DocumentConversion - Convert document Response: {0}", documentConversionResponse.htmlContent);
        _convertDocumentTested = true;
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("ExampleDoucmentConversion.OnFail()", "Error received: {0}", error.ToString());
    }
}
