///**
//* Copyright 2015 IBM Corp. All Rights Reserved.
//*
//* Licensed under the Apache License, Version 2.0 (the "License");
//* you may not use this file except in compliance with the License.
//* You may obtain a copy of the License at
//*
//*      http://www.apache.org/licenses/LICENSE-2.0
//*
//* Unless required by applicable law or agreed to in writing, software
//* distributed under the License is distributed on an "AS IS" BASIS,
//* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//* See the License for the specific language governing permissions and
//* limitations under the License.
//*
//*/

//using FullSerializer;
//using IBM.Watson.DeveloperCloud.Logging;
//using IBM.Watson.DeveloperCloud.Services.LanguageTranslator.v1;
//using IBM.Watson.DeveloperCloud.Utilities;
//using System;
//using System.Collections;
//using System.IO;

//namespace IBM.Watson.DeveloperCloud.UnitTests
//{
//    public class TestLanguageTranslator : UnitTest
//    {
//        private string _pharseToTranslate = "Hello, welcome to IBM Watson!";
//        private string _username;
//        private string _password;
//        private string _url;
//        //private string _token = "<authentication-token>";
//        private fsSerializer _serializer = new fsSerializer();

//        private LanguageTranslator _languageTranslator;

//        private bool _getTranslationTested = false;
//        private bool _getModelsTested = false;
//        private bool _createModelTested = false;
//        private bool _getModelTested = false;
//        private bool _deleteModelTested = false;
//        private bool _identifyTested = false;
//        private bool _getLanguagesTested = false;

//        public override IEnumerator RunTest()
//        {
//            LogSystem.InstallDefaultReactors();

//            VcapCredentials vcapCredentials = new VcapCredentials();
//            fsData data = null;

//            //  Get credentials from a credential file defined in environmental variables in the VCAP_SERVICES format. 
//            //  See https://www.ibm.com/watson/developercloud/doc/common/getting-started-variables.html.
//            var environmentalVariable = Environment.GetEnvironmentVariable("VCAP_SERVICES");
//            var fileContent = File.ReadAllText(environmentalVariable);

//            //  Add in a parent object because Unity does not like to deserialize root level collection types.
//            fileContent = Utility.AddTopLevelObjectToJson(fileContent, "VCAP_SERVICES");

//            //  Convert json to fsResult
//            fsResult r = fsJsonParser.Parse(fileContent, out data);
//            if (!r.Succeeded)
//                throw new WatsonException(r.FormattedMessages);

//            //  Convert fsResult to VcapCredentials
//            object obj = vcapCredentials;
//            r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
//            if (!r.Succeeded)
//                throw new WatsonException(r.FormattedMessages);

//            //  Set credentials from imported credntials
//            Credential credential = vcapCredentials.VCAP_SERVICES["language_translator"][0].Credentials;
//            _username = credential.Username.ToString();
//            _password = credential.Password.ToString();
//            _url = credential.Url.ToString();

//            //  Create credential and instantiate service
//            Credentials credentials = new Credentials(_username, _password, _url);

//            //  Or authenticate using token
//            //Credentials credentials = new Credentials(_url)
//            //{
//            //    AuthenticationToken = _token
//            //};

//            _languageTranslator = new LanguageTranslator(credentials);

//            if (!_languageTranslator.GetTranslation(_pharseToTranslate, "en", "es", OnGetTranslation))
//                Log.Debug("ExampleLanguageTranslator", "Failed to translate!");
//            while (!_getTranslationTested)
//                yield return null;

//            Log.Debug("ExampleLanguageTranslator", "Language Translator examples complete.");

//            yield break;
//        }

//        private void OnGetModel(TranslationModel model)
//        {
//            Test(model != null);
//            if (model != null)
//            {
//                Log.Status("TestTranslate", "ModelID: {0}, Source: {1}, Target: {2}, Domain: {3}",
//                    model.model_id, model.source, model.target, model.domain);
//            }
//            _getModelTested = true;
//        }

//        private void OnGetModels(TranslationModels models)
//        {
//            Test(models != null);
//            if (models != null)
//            {
//                foreach (var model in models.models)
//                {
//                    Log.Status("TestTranslate", "ModelID: {0}, Source: {1}, Target: {2}, Domain: {3}",
//                        model.model_id, model.source, model.target, model.domain);
//                }
//            }
//            _getModelsTested = true;
//        }

//        private void OnGetTranslation(Translations translation, string data)
//        {
//            Log.Debug("ExampleLanguageTranslator", "Langauge Translator - Translate Response: {0}", data);
//            Test(translation != null);
//            _getTranslationTested = true;
//        }

//        private void OnIdentify(string lang)
//        {
//            Test(lang != null);
//            if (lang != null)
//                Log.Status("TestTranslate", "Identified Language as {0}", lang);
//            _identifyTested = true;
//        }

//        private void OnGetLanguages(Languages languages)
//        {
//            Test(languages != null);
//            if (languages != null)
//            {
//                foreach (var lang in languages.languages)
//                    Log.Status("TestTranslate", "Language: {0}, Name: {1}", lang.language, lang.name);
//            }

//            _getLanguagesTested = true;
//        }
//    }
//}
