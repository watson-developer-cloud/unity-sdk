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

using System.Collections;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.NaturalLanguageUnderstanding.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using FullSerializer;
using System.IO;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Connection;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestNaturalLanguageUnderstanding : UnitTest
    {
        private string _username = null;
        private string _password = null;
        private fsSerializer _serializer = new fsSerializer();
        private string _versionDate = "2017-02-27";
        //private string _token = "<authentication-token>";

        NaturalLanguageUnderstanding _naturalLanguageUnderstanding;

        private bool _getModelsTested = false;
        private bool _analyzeTested = false;

        public override IEnumerator RunTest()
        {
            LogSystem.InstallDefaultReactors();

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
            Credential credential = vcapCredentials.GetCredentialByname("natural-language-understanding-sdk")[0].Credentials;
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

            _naturalLanguageUnderstanding = new NaturalLanguageUnderstanding(credentials);
            _naturalLanguageUnderstanding.VersionDate = _versionDate;

            Log.Debug("TestNaturalLanguageUnderstanding.RunTests()", "attempting to get models...");
            if (!_naturalLanguageUnderstanding.GetModels(OnGetModels, OnFail))
                Log.Debug("TestNaturalLanguageUnderstanding.GetModels()", "Failed to get models.");
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

            Log.Debug("TestNaturalLanguageUnderstanding.RunTests()", "attempting to analyze...");
            if (!_naturalLanguageUnderstanding.Analyze(OnAnalyze, OnFail, parameters))
                Log.Debug("TestNaturalLanguageUnderstanding.Analyze()", "Failed to get models.");
            while (!_analyzeTested)
                yield return null;

            Log.Debug("TestNaturalLanguageUnderstanding.RunTests()", "Natural language understanding examples complete.");

            yield break;
        }

        private void OnGetModels(ListModelsResults resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestNaturalLanguageUnderstanding.OnGetModels()", "ListModelsResult: {0}", customData["json"].ToString());
            Test(resp != null);
            _getModelsTested = true;
        }

        private void OnAnalyze(AnalysisResults resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestNaturalLanguageUnderstanding.OnAnalyze()", "AnalysisResults: {0}", customData["json"].ToString());
            Test(resp != null);
            _analyzeTested = true;
        }

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("TestNaturalLanguageUnderstanding.OnFail()", "Error received: {0}", error.ToString());
        }
    }
}
