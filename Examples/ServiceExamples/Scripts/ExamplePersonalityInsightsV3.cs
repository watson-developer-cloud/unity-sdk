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
using FullSerializer;
using System.IO;
using System;

public class ExamplePersonalityInsightsV3 : MonoBehaviour
{
    private string _username;
    private string _password;
    private string _url;
    private string _personalityInsightsVersionDate = "2017-05-26";
    private fsSerializer _serializer = new fsSerializer();
    private string _testString = "The IBM Watson™ Personality Insights service provides a Representational State Transfer (REST) Application Programming Interface (API) that enables applications to derive insights from social media, enterprise data, or other digital communications. The service uses linguistic analytics to infer individuals' intrinsic personality characteristics, including Big Five, Needs, and Values, from digital communications such as email, text messages, tweets, and forum posts. The service can automatically infer, from potentially noisy social media, portraits of individuals that reflect their personality characteristics. The service can report consumption preferences based on the results of its analysis, and for JSON content that is timestamped, it can report temporal behavior.";
    private string _dataPath;
    //private string _token = "<authentication-token>";

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
        Credential credential = vcapCredentials.VCAP_SERVICES["personality_insights"][0].Credentials;
        _username = credential.Username.ToString();
        _password = credential.Password.ToString();
        _url = credential.Url.ToString();

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_username, _password, _url);

        //  Or authenticate using token
        //Credentials credentials = new Credentials(_url)
        //{
        //    AuthenticationToken = _token
        //};

        PersonalityInsights personalityInsights = new PersonalityInsights(credentials);
        personalityInsights.VersionDate = _personalityInsightsVersionDate;

        _dataPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/personalityInsights.json";

        if (!personalityInsights.GetProfile(OnGetProfileJson, _dataPath, ContentType.TEXT_HTML, ContentLanguage.ENGLISH, ContentType.APPLICATION_JSON, AcceptLanguage.ENGLISH, true, true, true))
            Log.Debug("ExamplePersonalityInsights", "Failed to get profile!");

        if (!personalityInsights.GetProfile(OnGetProfileText, _testString, ContentType.TEXT_HTML, ContentLanguage.ENGLISH, ContentType.APPLICATION_JSON, AcceptLanguage.ENGLISH, true, true, true))
            Log.Debug("ExamplePersonalityInsights", "Failed to get profile!");
    }

    private void OnGetProfileText(Profile profile, string data)
    {
        Log.Debug("ExamplePersonaltyInsights", "Personality Insights - GetProfileText Response: {0}", data);
    }

    private void OnGetProfileJson(Profile profile, string data)
    {
        Log.Debug("ExamplePersonaltyInsights", "Personality Insights - GetProfileJson Response: {0}", data);
    }
}
