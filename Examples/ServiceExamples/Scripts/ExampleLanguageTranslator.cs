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
using IBM.Watson.DeveloperCloud.Services.LanguageTranslator.v2;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using System.Collections;
using IBM.Watson.DeveloperCloud.Connection;
using System.Collections.Generic;

public class ExampleLanguageTranslator : MonoBehaviour
{
    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [SerializeField]
    private string _username;
    [SerializeField]
    private string _password;
    [SerializeField]
    private string _url;
    #endregion

    private string _pharseToTranslate = "Hello, welcome to IBM Watson!";
    private string _pharseToIdentify = "Hola, donde esta la bibliteca?";
    
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

    void Start()
    {
        LogSystem.InstallDefaultReactors();

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_username, _password, _url);

        _languageTranslator = new LanguageTranslator(credentials);
        _forcedGlossaryFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/glossary.tmx";

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        if (!_languageTranslator.GetTranslation(OnGetTranslation, OnFail, _pharseToTranslate, "en", "es"))
            Log.Debug("ExampleLanguageTranslator.GetTranslation()", "Failed to translate.");
        while (!_getTranslationTested)
            yield return null;

        if (!_languageTranslator.GetModels(OnGetModels, OnFail))
            Log.Debug("ExampleLanguageTranslator.GetModels()", "Failed to get models.");
        while (!_getModelsTested)
            yield return null;

        if (!_languageTranslator.CreateModel(OnCreateModel, OnFail, _baseModelName, _customModelName, _forcedGlossaryFilePath))
            Log.Debug("ExampleLanguageTranslator.CreateModel()", "Failed to create model.");
        while (!_createModelTested)
            yield return null;

        if (!_languageTranslator.GetModel(OnGetModel, OnFail, _customLanguageModelId))
            Log.Debug("ExampleLanguageTranslator.GetModel()", "Failed to get model.");
        while (!_getModelTested)
            yield return null;

        if (!_languageTranslator.DeleteModel(OnDeleteModel, OnFail, _customLanguageModelId))
            Log.Debug("ExampleLanguageTranslator.DeleteModel()", "Failed to delete model.");
        while (!_deleteModelTested)
            yield return null;

        if (!_languageTranslator.Identify(OnIdentify, OnFail, _pharseToIdentify))
            Log.Debug("ExampleLanguageTranslator.Identify()", "Failed to identify language.");
        while (!_identifyTested)
            yield return null;

        if (!_languageTranslator.GetLanguages(OnGetLanguages, OnFail))
            Log.Debug("ExampleLanguageTranslator.GetLanguages()", "Failed to get languages.");
        while (!_getLanguagesTested)
            yield return null;

        Log.Debug("ExampleLanguageTranslator.Examples()", "Language Translator examples complete.");
    }

    private void OnGetModels(TranslationModels models, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleLanguageTranslator.OnGetModels()", "Language Translator - Get models response: {0}", customData["json"].ToString());
        _getModelsTested = true;
    }

    private void OnCreateModel(TranslationModel model, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleLanguageTranslator.OnCreateModel()", "Language Translator - Create model response: {0}", customData["json"].ToString());
        _customLanguageModelId = model.model_id;
        _createModelTested = true;
    }

    private void OnGetModel(TranslationModel model, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleLanguageTranslator.OnGetModel()", "Language Translator - Get model response: {0}", customData["json"].ToString());
        _getModelTested = true;
    }

    private void OnDeleteModel(DeleteModelResult deleteModelResult, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleLanguageTranslator.OnDeleteModel()", "Language Translator - Delete model response: success: {0}", customData["json"].ToString());
        _customLanguageModelId = null;
        _deleteModelTested = true;
    }

    private void OnGetTranslation(Translations translation, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleLanguageTranslator.OnGetTranslation()", "Langauge Translator - Translate Response: {0}", customData["json"].ToString());
        _getTranslationTested = true;
    }

    private void OnIdentify(IdentifiedLanguages identifiedLanguages, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleLanguageTranslator.OnIdentify()", "Language Translator - Identify response: {0}", customData["json"].ToString());
        _identifyTested = true;
    }

    private void OnGetLanguages(Languages languages, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleLanguageTranslator.OnGetLanguages()", "Language Translator - Get languages response: {0}", customData["json"].ToString());
        _getLanguagesTested = true;
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("ExampleLanguageTranslator.OnFail()", "Error received: {0}", error.ToString());
    }
}
