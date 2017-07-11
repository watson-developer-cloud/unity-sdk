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
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Logging;
using System.Collections;
using IBM.Watson.DeveloperCloud.Utilities;
using FullSerializer;
using System.IO;
using System;
using System.Collections.Generic;

public class ExampleSpeechToText : MonoBehaviour
{
    private string _username;
    private string _password;
    private string _url;
    private fsSerializer _serializer = new fsSerializer();
    //private string _token = "<authentication-token>";

    private AudioClip _audioClip;
    private SpeechToText _speechToText;

    private string _modelNameToGet;
    private string _createdCustomizationID;
    private string _createdCorpusName = "unity-corpus";
    private string _customCorpusFilePath;
    private string _customWordsFilePath;

    private bool _getModelsTested = false;
    private bool _getModelTested = false;
    private bool _getCustomizationsTested = false;
    private bool _createCustomizationsTested = false;
    private bool _getCustomizationTested = false;
    private bool _deleteCustomizationsTested = false;
    private bool _trainCustomizationTested = false;
    private bool _upgradeCustomizationTested = false;
    private bool _resetCustomizationTested = false;
    private bool _getCustomCorporaTested = false;
    private bool _addCustomCorpusTested = false;
    private bool _getCustomWordsTested = false;
    private bool _addCustomWordsFromPathTested = false;
    private bool _addCustomWordsFromObjectTested = false;
    private bool _getCustomWordTested = false;
    private bool _deleteCustomWordTested = false;
    private bool _deleteCustomCorpusTested = false;

    private bool _isCustomizationReady = false;

    void Start()
    {
        LogSystem.InstallDefaultReactors();

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
        Credential credential = vcapCredentials.VCAP_SERVICES["speech_to_text"][0].Credentials;
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

        _speechToText = new SpeechToText(credentials);
        _customCorpusFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/test-stt-corpus.txt";
        _customWordsFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/test-stt-words.json";

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        //  Get models
        Log.Debug("ExampleSpeechToText", "Attempting to get models");
        _speechToText.GetModels(HandleGetModels);
        while (!_getModelsTested)
            yield return null;

        //  Get model
        Log.Debug("ExampleSpeechToText", "Attempting to get model {0}", _modelNameToGet);
        _speechToText.GetModel(HandleGetModel, _modelNameToGet);
        while (!_getModelTested)
            yield return null;

        //  Get customizations
        Log.Debug("ExampleSpeechToText", "Attempting to get customizations");
        _speechToText.GetCustomizations(HandleGetCustomizations);
        while (!_getCustomizationsTested)
            yield return null;

        //  Create customization
        Log.Debug("ExampleSpeechToText", "Attempting create customization");
        _speechToText.CreateCustomization(HandleCreateCustomization, "unity-test-customization", "en-US_BroadbandModel", "Testing customization unity");
        while (!_createCustomizationsTested)
            yield return null;

        Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
        while (!_isCustomizationReady)
            yield return null;

        //  Get customization
        Log.Debug("ExampleSpeechToText", "Attempting to get customization {0}", _createdCustomizationID);
        _speechToText.GetCustomization(HandleGetCustomization, _createdCustomizationID);
        while (!_getCustomizationTested)
            yield return null;

        //  Get custom corpora
        Log.Debug("ExampleSpeechToText", "Attempting to get custom corpora for {0}", _createdCustomizationID);
        _speechToText.GetCustomCorpora(HandleGetCustomCorpora, _createdCustomizationID);
        while (!_getCustomCorporaTested)
            yield return null;

        //  Add custom corpus
        Log.Debug("ExampleSpeechToText", "Attempting to add custom corpus {1} in customization {0}", _createdCustomizationID, _createdCorpusName);
        _speechToText.AddCustomCorpus(HandleAddCustomCorpus, _createdCustomizationID, _createdCorpusName, true, _customCorpusFilePath);
        while (!_addCustomCorpusTested)
            yield return null;

        //  Get custom words
        Log.Debug("ExampleSpeechToText", "Attempting to get custom words.");
        _speechToText.GetCustomWords(HandleGetCustomWords, _createdCustomizationID);
        while (!_getCustomWordsTested)
            yield return null;

        //  Add custom words from path
        Log.Debug("ExampleSpeechToText", "Attempting to add custom words in customization {0} using Words json path {1}", _createdCustomizationID, _customWordsFilePath);
        _speechToText.AddCustomWords(HandleAddCustomWordsFromPath, _createdCustomizationID, _customWordsFilePath);
        while (!_addCustomWordsFromPathTested)
            yield return null;

        //  Add custom words from object
        Words words = new Words();
        Word w0 = new Word();
        List<Word> wordList = new List<Word>();
        w0.word = "mikey";
        w0.sounds_like = new string[1];
        w0.sounds_like[0] = "my key";
        w0.display_as = "Mikey";
        wordList.Add(w0);
        Word w1 = new Word();
        w1.word = "charlie";
        w1.sounds_like = new string[1];
        w1.sounds_like[0] = "char lee";
        w1.display_as = "Charlie";
        wordList.Add(w1);
        Word w2 = new Word();
        w2.word = "bijou";
        w2.sounds_like = new string[1];
        w2.sounds_like[0] = "be joo";
        w2.display_as = "Bijou";
        wordList.Add(w2);
        words.words = wordList.ToArray();
        Log.Debug("ExampleSpeechToText", "Attempting to add custom words in customization {0} using Words object", _createdCustomizationID);
        _speechToText.AddCustomWords(HandleAddCustomWordsFromObject, _createdCustomizationID, words);
        while (!_addCustomWordsFromObjectTested)
            yield return null;

        //  Get custom word
        Log.Debug("ExampleSpeechToText", "Attempting to get custom word {1} in customization {0}", _createdCustomizationID, words.words[0].word);
        _speechToText.GetCustomWord(HandleGetCustomWord, _createdCustomizationID, words.words[0].word);
        while (!_getCustomWordTested)
            yield return null;

        //  Train customization
        Log.Debug("ExampleSpeechToText", "Attempting to train customization {0}", _createdCustomizationID);
        _speechToText.TrainCustomization(HandleTrainCustomization, _createdCustomizationID);
        while (!_trainCustomizationTested)
            yield return null;

        //  Upgrade customization
        Log.Debug("ExampleSpeechToText", "Attempting to upgrade customization {0}", _createdCustomizationID);
        _speechToText.UpgradeCustomization(HandleUpgradeCustomization, _createdCustomizationID);
        while (!_upgradeCustomizationTested)
            yield return null;
        
        //  Delete custom word
        Log.Debug("ExampleSpeechToText", "Attempting to delete custom word {1} in customization {0}", _createdCustomizationID, words.words[2].word);
        _speechToText.DeleteCustomWord(HandleDeleteCustomWord, _createdCustomizationID, words.words[2].word);
        while (!_deleteCustomWordTested)
            yield return null;

        //  Reset customization
        Log.Debug("ExampleSpeechToText", "Attempting to reset customization {0}", _createdCustomizationID);
        _speechToText.ResetCustomization(HandleResetCustomization, _createdCustomizationID);
        while (!_resetCustomizationTested)
            yield return null;

        //  Delete custom corpus
        Log.Debug("ExampleSpeechToText", "Attempting to delete custom corpus {1} in customization {0}", _createdCustomizationID, _createdCorpusName);
        _speechToText.DeleteCustomCorpus(HandleDeleteCustomCorpus, _createdCustomizationID, _createdCorpusName);
        while (!_deleteCustomCorpusTested)
            yield return null;

        //  Delete customization
        Log.Debug("ExampleSpeechToText", "Attempting to delete customization {0}", _createdCustomizationID);
        _speechToText.DeleteCustomization(HandleDeleteCustomization, _createdCustomizationID);
        while (!_deleteCustomizationsTested)
            yield return null;

        Log.Debug("ExampleSpeechToText", "Speech to Text examples complete.");
    }

    private void HandleGetModels(ModelSet result, string customData)
    {

        Log.Debug("ExampleSpeechToText", "Speech to Text - Get models response: {0}", customData);
        _modelNameToGet = (result.models[UnityEngine.Random.Range(0, result.models.Length - 1)] as Model).name;
        _getModelsTested = true;
    }

    private void HandleGetModel(Model model, string customData)
    {
        Log.Debug("ExampleSpeechToText", "Speech to Text - Get model response: {0}", customData);
        _getModelTested = true;
    }

    //private void HandleOnRecognize(SpeechRecognitionEvent result)
    //{
    //    if (result != null && result.results.Length > 0)
    //    {
    //        foreach (var res in result.results)
    //        {
    //            foreach (var alt in res.alternatives)
    //            {
    //                string text = alt.transcript;
    //                Log.Debug("ExampleSpeechToText", string.Format("{0} ({1}, {2:0.00})\n", text, res.final ? "Final" : "Interim", alt.confidence));
    //            }
    //        }
    //    }
    //}

    private void HandleGetCustomizations(Customizations customizations, string customData)
    {
        Log.Debug("ExampleSpeechToText", "Speech to Text - Get customizations response: {0}", customData);
        _getCustomizationsTested = true;
    }

    private void HandleCreateCustomization(CustomizationID customizationID, string customData)
    {
        Log.Debug("ExampleSpeechToText", "Speech to Text - Create customization response: {0}", customData);
        _createdCustomizationID = customizationID.customization_id;
        _createCustomizationsTested = true;
    }

    private void HandleGetCustomization(Customization customization, string customData)
    {
        Log.Debug("ExampleSpeechToText", "Speech to Text - Get customization response: {0}", customData);
        _getCustomizationTested = true;
    }

    private void HandleDeleteCustomization(bool success, string customData)
    {
        if (success)
        {
            Log.Debug("ExampleSpeechToText", "Speech to Text - Get customization response: Deleted customization {0}!", _createdCustomizationID);
            _createdCustomizationID = default(string);
        }
        else
        {
            Log.Debug("ExampleSpeechToText", "Failed to delete customization!");
        }

        _deleteCustomizationsTested = true;
    }


    private void HandleTrainCustomization(bool success, string customData)
    {
        if (success)
        {
            Log.Debug("ExampleSpeechToText", "Train customization {0}!", _createdCustomizationID);
        }
        else
        {
            Log.Debug("ExampleSpeechToText", "Failed to train customization!");
        }
    }

    private void HandleUpgradeCustomization(bool success, string customData)
    {
        if (success)
        {
            Log.Debug("ExampleSpeechToText", "Upgrade customization {0}!", _createdCustomizationID);
        }
        else
        {
            Log.Debug("ExampleSpeechToText", "Failed to upgrade customization!");
        }
    }

    private void HandleResetCustomization(bool success, string customData)
    {
        if (success)
        {
            Log.Debug("ExampleSpeechToText", "Reset customization {0}!", _createdCustomizationID);
        }
        else
        {
            Log.Debug("ExampleSpeechToText", "Failed to reset customization!");
        }
    }

    private void HandleGetCustomCorpora(Corpora corpora, string customData)
    {
        Log.Debug("ExampleSpeechToText", "Speech to Text - Get custom corpora response: {0}", customData);
        _getCustomCorporaTested = true;
    }

    private void HandleDeleteCustomCorpus(bool success, string customData)
    {
        if (success)
        {
            Log.Debug("ExampleSpeechToText", "Speech to Text - delete custom coprus response: succeeded!");
        }
        else
        {
            Log.Debug("ExampleSpeechToText", "Failed to delete custom corpus!");
        }
    }

    private void HandleAddCustomCorpus(bool success, string customData)
    {
        Log.Debug("ExampleSpeechToText", "Speech to Text - Add custom corpus response: {0}", customData);
        _addCustomCorpusTested = true;
    }

    private void HandleGetCustomWords(WordsList wordList, string customData)
    {
        Log.Debug("ExampleSpeechToText", "Speech to Text - Get custom words response: {0}", customData);
        _getCustomWordsTested = true;
    }

    private void HandleAddCustomWordsFromPath(bool success, string customData)
    {
        Log.Debug("ExampleSpeechToText", "Speech to Text - Add custom words (path) response: {0}", customData);
        _addCustomWordsFromPathTested = true;
    }

    private void HandleAddCustomWordsFromObject(bool success, string customData)
    {
        Log.Debug("ExampleSpeechToText", "Speech to Text - Add custom words (object) response: {0}", customData);
        _addCustomWordsFromObjectTested = true;
    }

    private void HandleDeleteCustomWord(bool success, string customData)
    {
        if (success)
        {
            Log.Debug("ExampleSpeechToText", "Speech to Text - Delete custom word response: succeeded!");
        }
        else
        {
            Log.Debug("ExampleSpeechToText", "Failed to delete custom word!");
        }
    }

    private void HandleGetCustomWord(WordData word, string customData)
    {
        Log.Debug("ExampleSpeechToText", "Speech to Text - Get custom word response: {0}", customData);
        _getCustomWordTested = true;
    }

    private IEnumerator CheckCustomizationStatus(string customizationID, float delay = 0.1f)
    {
        Log.Debug("TestSpeechToText", "Checking customization status in {0} seconds...", delay.ToString());
        yield return new WaitForSeconds(delay);

        //	passing customizationID in custom data
        _speechToText.GetCustomization(OnCheckCustomizationStatus, customizationID, customizationID);
    }

    private void OnCheckCustomizationStatus(Customization customization, string customData)
    {
        if (customization != null)
        {
            Log.Debug("TestSpeechToText", "Customization status: {0}", customization.status);
            if (customization.status != "ready" && customization.status != "available")
                Runnable.Run(CheckCustomizationStatus(customData, 5f));
            else
                _isCustomizationReady = true;
        }
        else
        {
            Log.Debug("TestSpeechToText", "Check customization status failed!");
        }
    }
}