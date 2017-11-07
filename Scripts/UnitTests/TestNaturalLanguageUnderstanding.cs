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
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.NaturalLanguageUnderstanding.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Connection;
using FullSerializer;
using System;
using System.IO;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestNaturalLanguageUnderstanding : UnitTest
    {
        private string _username = null;
        private string _password = null;
        private fsSerializer _serializer = new fsSerializer();
        //private string _token = "<authentication-token>";

        NaturalLanguageUnderstanding _naturalLanguageUnderstanding;

        private bool _getModelsTested = false;
        private bool _analyzeTested = false;

        public override IEnumerator RunTest()
        {
            LogSystem.InstallDefaultReactors();

            try
            {
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
                Credential credential = vcapCredentials.VCAP_SERVICES["natural_language_understanding"][TestCredentialIndex].Credentials;
                _username = credential.Username.ToString();
                _password = credential.Password.ToString();
                _url = credential.Url.ToString();
            }
            catch
            {
                Log.Debug("TestNaturalLanguageUnderstanding.RunTest()", "Failed to get credentials from VCAP_SERVICES file. Please configure credentials to run this test. For more information, see: https://github.com/watson-developer-cloud/unity-sdk/#authentication");
            }

            //  Create credential and instantiate service
            Credentials credentials = new Credentials(_username, _password, _url);

            //  Or authenticate using token
            //Credentials credentials = new Credentials(_url)
            //{
            //    AuthenticationToken = _token
            //};

            _naturalLanguageUnderstanding = new NaturalLanguageUnderstanding(credentials);

            Log.Debug("TestNaturalLanguageUnderstanding.RunTests()", "attempting to get models...");
            if (!_naturalLanguageUnderstanding.GetModels(OnGetModels))
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
            if (!_naturalLanguageUnderstanding.Analyze(OnAnalyze, parameters))
                Log.Debug("TestNaturalLanguageUnderstanding.Analyze()", "Failed to get models.");
            while (!_analyzeTested)
                yield return null;

            Log.Debug("TestNaturalLanguageUnderstanding.RunTests()", "Natural language understanding examples complete.");

            yield break;
        }

        private void OnGetModels(RESTConnector.ParsedResponse<ListModelsResults> resp)
        {
            fsData data = null;
            _serializer.TrySerialize(resp.DataObject, out data).AssertSuccess();
            Log.Debug("TestNaturalLanguageUnderstanding.OnGetModels()", "ListModelsResult: {0}", data.ToString());
            Test(resp.DataObject != null);

            _getModelsTested = true;
        }

        private void OnAnalyze(RESTConnector.ParsedResponse<AnalysisResults> resp)
        {
            fsData data = null;
            _serializer.TrySerialize(resp.DataObject, out data).AssertSuccess();
            Log.Debug("TestNaturalLanguageUnderstanding.OnAnalyze()", "AnalysisResults: {0}", data.ToString());
            Test(resp.DataObject != null);
            _analyzeTested = true;
        }
    }
}
