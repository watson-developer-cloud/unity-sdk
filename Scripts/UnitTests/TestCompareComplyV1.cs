/**
* Copyright 2018 IBM Corp. All Rights Reserved.
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
using IBM.Watson.DeveloperCloud.Services.CompareComply.v1;
using IBM.Watson.DeveloperCloud.UnitTests;
using IBM.Watson.DeveloperCloud.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestCompareComplyV1 : UnitTest
    {
        private fsSerializer _serializer = new fsSerializer();
        private string versionDate = "2018-11-15";
        private CompareComply compareComply;

        private bool convertToHtmlTested = false;
        private bool classifyElementsTested = false;
        private bool extractTablesTested = false;
        private bool compareDocumentsTested = false;
        private bool addFeedbackTested = false;
        private bool listFeedbackTested = false;
        private bool getFeedbackTested = false;
        private bool deleteFeedbackTested = false;
        private bool createBatchTested = false;
        private bool listBatchesTested = false;
        private bool getBatchTestsed = false;
        private bool upddateBatchTested = false;

        private string feedbackId;
        private string batchId;

        private string contractAFilepath;
        private string contractBFilepath;
        private string tableFilepath;

        public override IEnumerator RunTest()
        {
            LogSystem.InstallDefaultReactors();

            contractAFilepath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/compare-comply/contract_A.pdf";
            contractBFilepath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/compare-comply/contract_B.pdf";
            tableFilepath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/compare-comply/TestTable.pdf";

            #region Get Credentials
            //VcapCredentials vcapCredentials = new VcapCredentials();
            //fsData data = null;

            //string result = null;
            //string credentialsFilepath = "../sdk-credentials/credentials.json";

            ////  Load credentials file if it exists. If it doesn't exist, don't run the tests.
            //if (File.Exists(credentialsFilepath))
            //    result = File.ReadAllText(credentialsFilepath);
            //else
            //    yield break;

            ////  Add in a parent object because Unity does not like to deserialize root level collection types.
            //result = Utility.AddTopLevelObjectToJson(result, "VCAP_SERVICES");

            ////  Convert json to fsResult
            //fsResult r = fsJsonParser.Parse(result, out data);
            //if (!r.Succeeded)
            //    throw new WatsonException(r.FormattedMessages);

            ////  Convert fsResult to VcapCredentials
            //object obj = vcapCredentials;
            //r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
            //if (!r.Succeeded)
            //    throw new WatsonException(r.FormattedMessages);

            ////  Set credentials from imported credntials
            //Credential credential = vcapCredentials.GetCredentialByname("compare-comply-v1-sdk-wdc")[0].Credentials;
            //_url = credential.Url.ToString();

            ////  Create credential and instantiate service
            //TokenOptions tokenOptions = new TokenOptions()
            //{
            //    IamApiKey = credential.IamApikey,
            //    IamUrl = credential.IamUrl
            //};
            #endregion

            TokenOptions tokenOptions = new TokenOptions()
            {
                IamUrl = "https://iam.stage1.bluemix.net/identity/token"
            };

            Credentials credentials = new Credentials(tokenOptions, "https://gateway-s.watsonplatform.net/compare-comply/api");

            //  Wait for tokendata
            while (!credentials.HasIamTokenData())
                yield return null;

            compareComply = new CompareComply(credentials);
            compareComply.VersionDate = versionDate;

            byte[] contractA = File.ReadAllBytes(contractAFilepath);
            byte[] contractB = File.ReadAllBytes(contractBFilepath);
            byte[] table = File.ReadAllBytes(tableFilepath);

            compareComply.ConvertToHtml(OnConvertToHtml, OnFail, contractA, fileContentType:"application/pdf");
            while(!convertToHtmlTested)
            {
                yield return null;
            }

            compareComply.ClassifyElements(OnClassifyElements, OnFail, contractA);
            while (!classifyElementsTested)
            {
                yield return null;
            }

            compareComply.ExtractTables(OnExtractTables, OnFail, table);
            while (!extractTablesTested)
            {
                yield return null;
            }

            compareComply.CompareDocuments(OnCompareDocuments, OnFail, contractA, contractB, file1ContentType:"application/pdf", file2ContentType:"application/pdf");
            while(!compareDocumentsTested)
            {
                yield return null;
            }
        }

        

        private void OnConvertToHtml(HTMLReturn response, Dictionary<string, object> customData)
        {
            Log.Debug("TestCompareComplyV1.OnConvertToHtml()", "ConvertToHtml Response: {0}", customData["json"].ToString());
            Test(response != null);
            Test(response.Title == "Microsoft Word - contract_A.doc");
            convertToHtmlTested = true;
        }

        private void OnClassifyElements(ClassifyReturn response, Dictionary<string, object> customData)
        {
            Log.Debug("TestCompareComplyV1.OnClassifyElements()", "ClassifyElements Response: {0}", customData["json"].ToString());
            Test(response != null);
            Test(response.Document.Title == "Microsoft Word - contract_A.doc");
            classifyElementsTested = true;
        }
        private void OnExtractTables(TableReturn response, Dictionary<string, object> customData)
        {
            Log.Debug("TestCompareComplyV1.OnExtractTables()", "ExtractTables Response: {0}", customData["json"].ToString());
            Test(response != null);
            Test(response.Document.Title == "Untitled spreadsheet");
            extractTablesTested = true;
        }

        private void OnCompareDocuments(CompareReturn response, Dictionary<string, object> customData)
        {
            Log.Debug("TestCompareComplyV1.OnCompareDocuments()", "CompareDocuments Response: {0}", customData["json"].ToString());
            Test(response != null);
            Test(response.Documents[0].Html.Contains("HairPopMetal4Evah"));
            compareDocumentsTested = true;
        }

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Debug("TestCompareComplyV1.OnFail()", "Failed: {0}", error.ErrorMessage);
        }
    }
}
