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

using FullSerializer;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.LanguageTranslator.v3;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestLanguageTranslatorV3CF : UnitTest
    {
        private string _pharseToTranslate = "Hello, welcome to IBM Watson!";
        private string _username = null;
        private string _password = null;
        //private string _token = "<authentication-token>";
        private fsSerializer _serializer = new fsSerializer();

        private LanguageTranslator _languageTranslator;
        private string _baseModelName = "en-es";
        private string _customModelName = "Texan";
        private string _forcedGlossaryFilePath;
        private string _customLanguageModelId;

        private bool _getTranslationTested = false;
        private bool _getModelsTested = false;
        private bool _createModelTested = false;
        private bool _getModelTested = false;
        private bool _deleteModelTested = false;
        private bool _identifyTested = false;
        private bool _getLanguagesTested = false;
        private string _versionDate = "2018-05-01";

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
            Credential credential = vcapCredentials.GetCredentialByname("language-translator-v3-sdk-cf")[0].Credentials;
            _username = credential.Username.ToString();
            _password = credential.Password.ToString();
            _url = credential.Url.ToString();

            //  Create credential and instantiate service
            Credentials credentials = new Credentials(_username, _password, _url);

            _languageTranslator = new LanguageTranslator(_versionDate, credentials);

            _forcedGlossaryFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/glossary.tmx";

            if (!_languageTranslator.GetTranslation(OnGetTranslation, OnFail, _pharseToTranslate, "en", "es"))
                Log.Debug("TestLanguageTranslator.GetTranslation()", "Failed to translate.");
            while (!_getTranslationTested)
                yield return null;

            if (!_languageTranslator.GetModels(OnGetModels, OnFail))
                Log.Debug("TestLanguageTranslator.GetModels()", "Failed to get models.");
            while (!_getModelsTested)
                yield return null;

            if (!_languageTranslator.CreateModel(OnCreateModel, OnFail, _baseModelName, _customModelName, _forcedGlossaryFilePath))
                Log.Debug("TestLanguageTranslator.CreateModel()", "Failed to create model.");
            while (!_createModelTested)
                yield return null;

            if (!_languageTranslator.GetModel(OnGetModel, OnFail, _customLanguageModelId))
                Log.Debug("TestLanguageTranslator.GetModel()", "Failed to get model.");
            while (!_getModelTested)
                yield return null;

            if (!_languageTranslator.DeleteModel(OnDeleteModel, OnFail, _customLanguageModelId))
                Log.Debug("TestLanguageTranslator.DeleteModel()", "Failed to delete model.");
            while (!_deleteModelTested)
                yield return null;

            if (!_languageTranslator.Identify(OnIdentify, OnFail, _pharseToTranslate))
                Log.Debug("TestLanguageTranslator.Identify()", "Failed to identify language.");
            while (!_identifyTested)
                yield return null;

            if (!_languageTranslator.GetLanguages(OnGetLanguages, OnFail))
                Log.Debug("TestLanguageTranslator.GetLanguages()", "Failed to get languages.");
            while (!_getLanguagesTested)
                yield return null;

            Log.Debug("TestLanguageTranslator.RunTest()", "Language Translator examples complete.");

            yield break;
        }

        private void OnGetModels(TranslationModels models, Dictionary<string, object> customData)
        {
            Log.Debug("TestLanguageTranslator.OnGetModels()", "Language Translator - Get models response: {0}", customData["json"].ToString());
            Test(models != null);
            _getModelsTested = true;
        }

        private void OnCreateModel(TranslationModel model, Dictionary<string, object> customData)
        {
            Log.Debug("TestLanguageTranslator.OnCreateModel()", "Language Translator - Create model response: {0}", customData["json"].ToString());
            _customLanguageModelId = model.model_id;
            Test(model != null);
            _createModelTested = true;
        }

        private void OnGetModel(TranslationModel model, Dictionary<string, object> customData)
        {
            Log.Debug("TestLanguageTranslator.OnGetModel()", "Language Translator - Get model response: {0}", customData["json"].ToString());
            Test(model != null);
            _getModelTested = true;
        }

        private void OnDeleteModel(DeleteModelResult deleteModelResult, Dictionary<string, object> customData)
        {
            Log.Debug("TestLanguageTranslator.OnDeleteModel()", "Language Translator - Delete model response: success: {0}", customData["json"].ToString());
            _customLanguageModelId = null;
            Test(deleteModelResult != null);
            _deleteModelTested = true;
        }

        private void OnGetTranslation(Translations translation, Dictionary<string, object> customData)
        {
            Log.Debug("TestLanguageTranslator.OnGetTranslation()", "Langauge Translator - Translate Response: {0}", customData["json"].ToString());
            Test(translation != null);
            _getTranslationTested = true;
        }

        private void OnIdentify(IdentifiedLanguages identifiedLanguages, Dictionary<string, object> customData)
        {
            Log.Debug("TestLanguageTranslator.OnIdentify()", "Language Translator - Identify response: {0}", customData["json"].ToString());
            Test(identifiedLanguages != null);
            _identifyTested = true;
        }

        private void OnGetLanguages(Languages languages, Dictionary<string, object> customData)
        {
            Log.Debug("TestLanguageTranslator.OnGetLanguages()", "Language Translator - Get languages response: {0}", customData["json"].ToString());
            Test(languages != null);
            _getLanguagesTested = true;
        }

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("TestLanguageTranslator.OnFail()", "Error received: {0}", error.ToString());
        }
    }
}
