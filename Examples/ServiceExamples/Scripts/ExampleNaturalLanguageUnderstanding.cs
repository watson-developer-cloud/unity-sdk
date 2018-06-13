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
using IBM.Watson.DeveloperCloud.Services.NaturalLanguageUnderstanding.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleNaturalLanguageUnderstanding : MonoBehaviour
{
    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [Space(10)]
    [Tooltip("The service URL (optional). This defaults to \"https://gateway.watsonplatform.net/natural-language-understanding/api\"")]
    [SerializeField]
    private string _serviceUrl;
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

    NaturalLanguageUnderstanding _service;

    private bool _getModelsTested = false;
    private bool _analyzeTested = false;

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

        _service = new NaturalLanguageUnderstanding(credentials);
        _service.VersionDate = _versionDate;

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        Log.Debug("ExampleNaturalLanguageUnderstanding.Examples()", "attempting to get models...");
        if (!_service.GetModels(OnGetModels, OnFail))
            Log.Debug("ExampleNaturalLanguageUnderstanding.GetModels()", "Failed to get models.");
        while (!_getModelsTested)
            yield return null;

        Parameters parameters = new Parameters()
        {
            text = "Analyze various features of text content at scale. Provide text, raw HTML, or a public URL, and IBM Watson Natural Language Understanding will give you results for the features you request. The service cleans HTML content before analysis by default, so the results can ignore most advertisements and other unwanted content.",
            return_analyzed_text = true,
            language = "en",
            features = new Features()
            {
                entities = new EntitiesOptions()
                {
                    limit = 50,
                    sentiment = true,
                    emotion = true,
                },
                keywords = new KeywordsOptions()
                {
                    limit = 50,
                    sentiment = true,
                    emotion = true
                }
            }
        };

        Log.Debug("ExampleNaturalLanguageUnderstanding.Examples()", "attempting to analyze...");
        if (!_service.Analyze(OnAnalyze, OnFail, parameters))
            Log.Debug("ExampleNaturalLanguageUnderstanding.Analyze()", "Failed to get models.");
        while (!_analyzeTested)
            yield return null;

        Log.Debug("ExampleNaturalLanguageUnderstanding.Examples()", "Natural language understanding examples complete.");
    }

    private void OnGetModels(ListModelsResults resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleNaturalLanguageUnderstanding.OnGetModels()", "ListModelsResult: {0}", customData["json"].ToString());
        _getModelsTested = true;
    }

    private void OnAnalyze(AnalysisResults resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleNaturalLanguageUnderstanding.OnAnalyze()", "AnalysisResults: {0}", customData["json"].ToString());
        _analyzeTested = true;
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("ExampleNaturalLanguageUnderstanding.OnFail()", "Error received: {0}", error.ToString());
    }
}
