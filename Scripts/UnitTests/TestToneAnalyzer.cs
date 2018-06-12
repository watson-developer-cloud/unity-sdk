﻿/**
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

using System.Collections;
using IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v3;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using FullSerializer;
using System.IO;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Connection;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestToneAnalyzer : UnitTest
    {
        private string _username = null;
        private string _password = null;
        private fsSerializer _serializer = new fsSerializer();
        //private string _token = "<authentication-token>";

        private ToneAnalyzer _toneAnalyzer;
        private string _toneAnalyzerVersionDate = "2017-05-26";

        private string _stringToTestTone = "This service enables people to discover and understand, and revise the impact of tone in their content. It uses linguistic analysis to detect and interpret emotional, social, and language cues found in text.";
        private bool _analyzeToneTested = false;

        public override IEnumerator RunTest()
        {
            LogSystem.InstallDefaultReactors();

            VcapCredentials vcapCredentials = new VcapCredentials();
            fsData data = null;

            string result = null;

            string credentialsFilepath = "../sdk-credentials/credentials.json";

            //  Load credentials file if it exists. If it doesn't exist, don't run the tests.
            if (File.Exists(credentialsFilepath))
            {
                result = File.ReadAllText(credentialsFilepath);
            }
            else
            {
                yield break;
            }

            //  Add in a parent object because Unity does not like to deserialize root level collection types.
            result = Utility.AddTopLevelObjectToJson(result, "VCAP_SERVICES");

            //  Convert json to fsResult
            fsResult r = fsJsonParser.Parse(result, out data);
            if (!r.Succeeded)
            {
                throw new WatsonException(r.FormattedMessages);
            }

            //  Convert fsResult to VcapCredentials
            object obj = vcapCredentials;
            r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
            if (!r.Succeeded)
            {
                throw new WatsonException(r.FormattedMessages);
            }

            //  Set credentials from imported credntials
            Credential credential = vcapCredentials.GetCredentialByname("tone-analyzer-sdk")[0].Credentials;
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

            _toneAnalyzer = new ToneAnalyzer(credentials);
            _toneAnalyzer.VersionDate = _toneAnalyzerVersionDate;

            //  Analyze tone
            if (!_toneAnalyzer.GetToneAnalyze(OnGetToneAnalyze, OnFail, _stringToTestTone))
            {
                Log.Debug("ExampleToneAnalyzer.GetToneAnalyze()", "Failed to analyze!");
            }

            while (!_analyzeToneTested)
            {
                yield return null;
            }

            Log.Debug("ExampleToneAnalyzer.RunTest()", "Tone analyzer examples complete.");

            yield break;
        }

        private void OnGetToneAnalyze(ToneAnalyzerResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleToneAnalyzer.OnGetToneAnalyze()", "Tone Analyzer - Analyze Response: {0}", customData["json"].ToString());
            Test(resp != null);
            _analyzeToneTested = true;
        }
        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("ExampleRetrieveAndRank.OnFail()", "Error received: {0}", error.ToString());
        }
    }
}

