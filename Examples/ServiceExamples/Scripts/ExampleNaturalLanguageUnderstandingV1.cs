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
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.NaturalLanguageUnderstanding.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;
using UnityEngine;

public class ExampleNaturalLanguageUnderstandingV1 : MonoBehaviour
{
    private string _username = null;
    private string _password = null;
    private string _url = null;
    private fsSerializer _serializer = new fsSerializer();

    NaturalLanguageUnderstanding _naturalLanguageUnderstanding;

    private bool _getModelsTested = false;
    private bool _analyzeTested = false;

    void Start()
    {
        LogSystem.InstallDefaultReactors();

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_username, _password, _url);

        _naturalLanguageUnderstanding = new NaturalLanguageUnderstanding(credentials);

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        Log.Debug("ExampleNaturalLanguageUnderstandingV1", "attempting to get models...");
        if (!_naturalLanguageUnderstanding.GetModels(OnGetModels))
            Log.Debug("ExampleNaturalLanguageUnderstandingV1", "Failed to get models.");
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

        Log.Debug("ExampleNaturalLanguageUnderstandingV1", "attempting to analyze...");
        if (!_naturalLanguageUnderstanding.Analyze(OnAnalyze, parameters))
            Log.Debug("ExampleNaturalLanguageUnderstandingV1", "Failed to get models.");
        while (!_analyzeTested)
            yield return null;

        Log.Debug("ExampleNaturalLanguageUnderstandingV1", "Natural language understanding examples complete.");
    }

    private void OnGetModels(ListModelsResults resp, string customData)
    {
        fsData data = null;
        _serializer.TrySerialize(resp, out data).AssertSuccess();
        Log.Debug("ExampleNaturalLanguageUnderstandingV1", "ListModelsResult: {0}", data.ToString());

        _getModelsTested = true;
    }

    private void OnAnalyze(AnalysisResults resp, string customData)
    {
        fsData data = null;
        _serializer.TrySerialize(resp, out data).AssertSuccess();
        Log.Debug("ExampleNaturalLanguageUnderstandingV1", "AnalysisResults: {0}", data.ToString());

        _analyzeTested = true;
    }
}
