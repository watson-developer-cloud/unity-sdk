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
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Services.TradeoffAnalytics.v1;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using UnityEngine;
using FullSerializer;
using System;
using System.IO;
using IBM.Watson.DeveloperCloud.Connection;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestTradeoffAnalytics : UnitTest
    {
        private string _username = null;
        private string _password = null;
        private fsSerializer _serializer = new fsSerializer();
        //private string _token = "<authentication-token>";

        TradeoffAnalytics _tradeoffAnalytics;

        private bool _GetDillemaTested = false;

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
                Credential credential = vcapCredentials.VCAP_SERVICES["tradeoff_analytics"][TestCredentialIndex].Credentials;
                _username = credential.Username.ToString();
                _password = credential.Password.ToString();
                _url = credential.Url.ToString();
            }
            catch
            {
                Log.Debug("TestTradeoffAnalytics.RunTest()", "Failed to get credentials from VCAP_SERVICES file. Please configure credentials to run this test. For more information, see: https://github.com/watson-developer-cloud/unity-sdk/#authentication");
            }

            //  Create credential and instantiate service
            Credentials credentials = new Credentials(_username, _password, _url);

            //  Or authenticate using token
            //Credentials credentials = new Credentials(_url)
            //{
            //    AuthenticationToken = _token
            //};

            _tradeoffAnalytics = new TradeoffAnalytics(credentials);

            Problem problemToSolve = new Problem();
            problemToSolve.subject = "Test Subject";

            List<Column> listColumn = new List<Column>();
            Column columnPrice = new Column();
            columnPrice.description = "Price Column to minimize";
            columnPrice.range = new ValueRange();
            ((ValueRange)columnPrice.range).high = 600;
            ((ValueRange)columnPrice.range).low = 0;
            columnPrice.type = "numeric";
            columnPrice.key = "price";
            columnPrice.full_name = "Price";
            columnPrice.goal = "min";
            columnPrice.is_objective = true;
            columnPrice.format = "$####0.00";

            Column columnWeight = new Column();
            columnWeight.description = "Weight Column to minimize";
            columnWeight.type = "numeric";
            columnWeight.key = "weight";
            columnWeight.full_name = "Weight";
            columnWeight.goal = "min";
            columnWeight.is_objective = true;
            columnWeight.format = "####0 g";

            Column columnBrandName = new Column();
            columnBrandName.description = "All Brand Names";
            columnBrandName.type = "categorical";
            columnBrandName.key = "brand";
            columnBrandName.full_name = "Brand";
            columnBrandName.goal = "max";
            columnBrandName.is_objective = true;
            columnBrandName.preference = new string[] { "Samsung", "Apple", "HTC" };
            columnBrandName.range = new CategoricalRange();
            ((CategoricalRange)columnBrandName.range).keys = new string[] { "Samsung", "Apple", "HTC" };

            listColumn.Add(columnPrice);
            listColumn.Add(columnWeight);
            //            listColumn.Add(columnBrandName);

            problemToSolve.columns = listColumn.ToArray();


            List<Option> listOption = new List<Option>();

            Option option1 = new Option();
            option1.key = "1";
            option1.name = "Samsung Galaxy S4";
            option1.values = new TestDataValue();
            (option1.values as TestDataValue).weight = 130;
            (option1.values as TestDataValue).brand = "Samsung";
            (option1.values as TestDataValue).price = 249;
            listOption.Add(option1);

            Option option2 = new Option();
            option2.key = "2";
            option2.name = "Apple iPhone 5";
            option2.values = new TestDataValue();
            (option2.values as TestDataValue).weight = 112;
            (option2.values as TestDataValue).brand = "Apple";
            (option2.values as TestDataValue).price = 599;
            listOption.Add(option2);

            Option option3 = new Option();
            option3.key = "3";
            option3.name = "HTC One";
            option3.values = new TestDataValue();
            (option3.values as TestDataValue).weight = 143;
            (option3.values as TestDataValue).brand = "HTC";
            (option3.values as TestDataValue).price = 299;
            listOption.Add(option3);

            problemToSolve.options = listOption.ToArray();

            _tradeoffAnalytics.GetDilemma(OnGetDilemma, OnFail, problemToSolve, false);
            while (!_GetDillemaTested)
                yield return null;

            Log.Debug("ExampleTradeoffAnalyitics.RunTest()", "Tradeoff analytics examples complete.");

            yield break;
        }

        private void OnGetDilemma(DilemmasResponse resp, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleTradeoffAnalyitics.OnGetDilemma()", "{0}", customData["json"].ToString());
            Test(resp != null);
            _GetDillemaTested = true;
        }

        /// <summary>
        /// Application data value.
        /// </summary>
        public class TestDataValue : IBM.Watson.DeveloperCloud.Services.TradeoffAnalytics.v1.ApplicationDataValue
        {
            public double price { get; set; }
            public double weight { get; set; }
            public string brand { get; set; }
        }

        /// <summary>
        /// Application data.
        /// </summary>
        public class TestData : IBM.Watson.DeveloperCloud.Services.TradeoffAnalytics.v1.ApplicationData
        {

        }

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("TestTradeoffAnalytics.OnFail()", "Error received: {0}", error.ToString());
        }
    }
}

