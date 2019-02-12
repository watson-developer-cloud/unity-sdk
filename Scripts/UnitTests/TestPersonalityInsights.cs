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
using System.Collections;
using IBM.Watson.DeveloperCloud.Services.PersonalityInsights.v3;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using FullSerializer;
using System.IO;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Connection;
using System;

namespace IBM.Watson.DeveloperCloud.UnitTests
{

    public class TestPersonalityInsights : UnitTest
    {
        private fsSerializer _serializer = new fsSerializer();

        private PersonalityInsights _personalityInsights;
        private string _personalityInsightsVersionDate = "2017-05-26";

        private string _testString = "The IBM Watson™ Personality Insights service provides a Representational State Transfer (REST) Application Programming Interface (API) that enables applications to derive insights from social media, enterprise data, or other digital communications. The service uses linguistic analytics to infer individuals' intrinsic personality characteristics, including Big Five, Needs, and Values, from digital communications such as email, text messages, tweets, and forum posts. The service can automatically infer, from potentially noisy social media, portraits of individuals that reflect their personality characteristics. The service can report consumption preferences based on the results of its analysis, and for JSON content that is timestamped, it can report temporal behavior.";
        private string _dataPath;
        //private string _token = "<authentication-token>";

        private bool _autoGetProfileTested = false;
        private bool _getProfileTextTested = false;
        private bool _getProfileJsonTested = false;

        public override IEnumerator RunTest()
        {
            LogSystem.InstallDefaultReactors();

            //  Test PersonalityInsights using loaded credentials
            PersonalityInsights autoPersonalityInsights = new PersonalityInsights();
            autoPersonalityInsights.VersionDate = _personalityInsightsVersionDate;
            while (!autoPersonalityInsights.Credentials.HasIamTokenData())
                yield return null;
            autoPersonalityInsights.GetProfile(OnAutoGetProfile, OnFail, _testString, ContentType.TextHtml, ContentLanguage.English);
            while (!_autoGetProfileTested)
                yield return null;

            VcapCredentials vcapCredentials = new VcapCredentials();
            fsData data = null;

            string result = null;
            string credentialsFilepath = "../sdk-credentials/credentials.json";

            //  Load credentials file if it exists. If it doesn't exist, don't run the tests.
            if (File.Exists(credentialsFilepath))
                result = File.ReadAllText(credentialsFilepath);
            else
                yield break;
            //  Add in a parent object because Unity does not like to deserialize root level collection types.
            result = Utility.AddTopLevelObjectToJson(result, "VCAP_SERVICES");

            //  Convert json to fsResult
            fsResult r = fsJsonParser.Parse(result, out data);
            if (!r.Succeeded)
                throw new WatsonException(r.FormattedMessages);

            //  Convert fsResult to VcapCredentials
            object obj = vcapCredentials;
            r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
            if (!r.Succeeded)
                throw new WatsonException(r.FormattedMessages);

            //  Set credentials from imported credntials
            Credential credential = vcapCredentials.GetCredentialByname("personality-insights-sdk")[0].Credentials;
            //  Create credential and instantiate service
            TokenOptions tokenOptions = new TokenOptions()
            {
                IamApiKey = credential.IamApikey,
            };

            //  Create credential and instantiate service
            Credentials credentials = new Credentials(tokenOptions, credential.Url);

            //  Wait for tokendata
            while (!credentials.HasIamTokenData())
                yield return null;

            _personalityInsights = new PersonalityInsights(credentials);
            _personalityInsights.VersionDate = _personalityInsightsVersionDate;

            _dataPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/personalityInsights.json";

            if (!_personalityInsights.GetProfile(OnGetProfileJson, OnFail, _dataPath, ContentType.TextHtml, ContentLanguage.English, ContentType.ApplicationJson, AcceptLanguage.English, true, true, true))
                Log.Debug("ExamplePersonalityInsights.GetProfile()", "Failed to get profile!");
            while (!_getProfileJsonTested)
                yield return null;

            if (!_personalityInsights.GetProfile(OnGetProfileText, OnFail, _testString, ContentType.TextHtml, ContentLanguage.English, ContentType.ApplicationJson, AcceptLanguage.English, true, true, true))
                Log.Debug("ExamplePersonalityInsights.GetProfile()", "Failed to get profile!");
            while (!_getProfileTextTested)
                yield return null;

            Log.Debug("ExamplePersonalityInsights.RunTest()", "Personality insights examples complete.");

            yield break;
        }

        private void OnAutoGetProfile(Profile response, Dictionary<string, object> customData)
        {
            Log.Debug("ExamplePersonaltyInsights.OnAutoGetProfile()", "Personality Insights - GetProfileText Response: {0}", customData["json"].ToString());
            Test(response.personality != null);
            _autoGetProfileTested = true;
        }

        private void OnGetProfileText(Profile profile, Dictionary<string, object> customData = null)
        {
            Log.Debug("ExamplePersonaltyInsights.OnGetProfileText()", "Personality Insights - GetProfileText Response: {0}", customData["json"].ToString());
            Test(profile != null);
            _getProfileTextTested = true;
        }

        private void OnGetProfileJson(Profile profile, Dictionary<string, object> customData = null)
        {
            Log.Debug("ExamplePersonaltyInsights.OnGetProfileJson()", "Personality Insights - GetProfileJson Response: {0}", customData["json"].ToString());
            Test(profile != null);
            _getProfileJsonTested = true;
        }

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("ExamplePersonaltyInsights.OnFail()", "Error received: {0}", error.ToString());
        }
    }
}
