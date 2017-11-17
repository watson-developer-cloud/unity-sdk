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
using IBM.Watson.DeveloperCloud.Services.PersonalityInsights.v3;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;
using IBM.Watson.DeveloperCloud.Connection;
using System.Collections.Generic;

public class ExamplePersonalityInsights : MonoBehaviour
{
    private string _username = null;
    private string _password = null;
    private string _url = null;
    

    private PersonalityInsights _personalityInsights;
    private string _personalityInsightsVersionDate = "2017-05-26";

    private string _testString = "The IBM Watson™ Personality Insights service provides a Representational State Transfer (REST) Application Programming Interface (API) that enables applications to derive insights from social media, enterprise data, or other digital communications. The service uses linguistic analytics to infer individuals' intrinsic personality characteristics, including Big Five, Needs, and Values, from digital communications such as email, text messages, tweets, and forum posts. The service can automatically infer, from potentially noisy social media, portraits of individuals that reflect their personality characteristics. The service can report consumption preferences based on the results of its analysis, and for JSON content that is timestamped, it can report temporal behavior.";
    private string _dataPath;

    private bool _getProfileTextTested = false;
    private bool _getProfileJsonTested = false;

    void Start()
    {
        LogSystem.InstallDefaultReactors();

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_username, _password, _url);

        _personalityInsights = new PersonalityInsights(credentials);
        _personalityInsights.VersionDate = _personalityInsightsVersionDate;

        _dataPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/personalityInsights.json";

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        if (!_personalityInsights.GetProfile(OnGetProfileJson, OnFail, _dataPath, ContentType.TextHtml, ContentLanguage.English, ContentType.ApplicationJson, AcceptLanguage.English, true, true, true))
            Log.Debug("ExamplePersonalityInsights.GetProfile()", "Failed to get profile!");
        while (!_getProfileJsonTested)
            yield return null;

        if (!_personalityInsights.GetProfile(OnGetProfileText, OnFail, _testString, ContentType.TextHtml, ContentLanguage.English, ContentType.ApplicationJson, AcceptLanguage.English, true, true, true))
            Log.Debug("ExamplePersonalityInsights.GetProfile()", "Failed to get profile!");
        while (!_getProfileTextTested)
            yield return null;

        Log.Debug("ExamplePersonalityInsights.Examples()", "Personality insights examples complete.");
    }

    private void OnGetProfileText(Profile profile, Dictionary<string, object> customData)
    {
        Log.Debug("ExamplePersonaltyInsights.OnGetProfileText()", "Personality Insights - GetProfileText Response: {0}", customData["json"].ToString());
        _getProfileTextTested = true;
    }

    private void OnGetProfileJson(Profile profile, Dictionary<string, object> customData)
    {
        Log.Debug("ExamplePersonaltyInsights.OnGetProfileJson()", "Personality Insights - GetProfileJson Response: {0}", customData["json"].ToString());
        _getProfileJsonTested = true;
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("ExamplePersonaltyInsights.OnFail()", "Error received: {0}", error.ToString());
    }
}
