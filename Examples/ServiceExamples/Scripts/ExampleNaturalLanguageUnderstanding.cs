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
    [SerializeField]
    private string _username;
    [SerializeField]
    private string _password;
    [SerializeField]
    private string _url;
    [SerializeField]
    private string _versionDate;
    #endregion

    NaturalLanguageUnderstanding _naturalLanguageUnderstanding;

    private bool _getModelsTested = false;
    private bool _analyzeTested = false;

    void Start()
    {
        LogSystem.InstallDefaultReactors();

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_username, _password, _url);

        _naturalLanguageUnderstanding = new NaturalLanguageUnderstanding(credentials);
        _naturalLanguageUnderstanding.VersionDate = _versionDate;

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        Log.Debug("ExampleNaturalLanguageUnderstanding.Examples()", "attempting to get models...");
        if (!_naturalLanguageUnderstanding.GetModels(OnGetModels, OnFail))
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
        if (!_naturalLanguageUnderstanding.Analyze(OnAnalyze, OnFail, parameters))
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
