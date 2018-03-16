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
using IBM.Watson.DeveloperCloud.Services.DocumentConversion.v1;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using FullSerializer;
using System;
using System.IO;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Connection;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestDocumentConversion : UnitTest
    {
        private DocumentConversion _documentConversion;
        private string _username = null;
        private string _password = null;
        private fsSerializer _serializer = new fsSerializer();
        //private string _token = "<authentication-token>";

        private string _examplePath;
        private string _conversionTarget = ConversionTarget.NormalizedHtml;
        private bool _convertDocumentTested = false;

        public override IEnumerator RunTest()
        {
            LogSystem.InstallDefaultReactors();

            VcapCredentials vcapCredentials = new VcapCredentials();
            fsData data = null;

            string result = null;

            var vcapUrl = Environment.GetEnvironmentVariable("VCAP_URL");
            var vcapUsername = Environment.GetEnvironmentVariable("VCAP_USERNAME");
            var vcapPassword = Environment.GetEnvironmentVariable("VCAP_PASSWORD");

            using (SimpleGet simpleGet = new SimpleGet(vcapUrl, vcapUsername, vcapPassword))
            {
                while (!simpleGet.IsComplete)
                    yield return null;

                result = simpleGet.Result;
            }

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
            Credential credential = vcapCredentials.VCAP_SERVICES["document_conversion"];
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

            _documentConversion = new DocumentConversion(credentials);
            _examplePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/watson_beats_jeopardy.html";

            if (!_documentConversion.ConvertDocument(OnConvertDocument, OnFail, _examplePath, _conversionTarget))
                Log.Debug("TestDocumentConversion.RunTest()", "Document conversion failed!");

            while (!_convertDocumentTested)
                yield return null;

            Log.Debug("TestDoucmentConversion.RunTest()", "Document conversion examples complete.");

            yield break;
        }

        private void OnConvertDocument(ConvertedDocument documentConversionResponse, Dictionary<string, object> customData)
        {
            Log.Debug("TestDoucmentConversion.OnConvertDocument()", "DocumentConversion - Convert document Response: {0}", documentConversionResponse.htmlContent);
            Test(documentConversionResponse != null);
            _convertDocumentTested = true;
        }

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("TestDoucmentConversion.OnFail()", "Error received: {0}", error.ToString());
        }
    }
}
