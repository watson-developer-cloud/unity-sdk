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
#pragma warning disable 0649

using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Logging;
using System.Collections;
using IBM.Watson.DeveloperCloud.Utilities;
using System.IO;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Connection;
using UnityEngine.Networking;
using Utility = IBM.Watson.DeveloperCloud.Utilities.Utility;

public class ExampleSpeechToText : MonoBehaviour
{
    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [Space(10)]
    [Tooltip("The service URL (optional). This defaults to \"https://stream.watsonplatform.net/speech-to-text/api\"")]
    [SerializeField]
    private string _serviceUrl;
    [Header("IAM Authentication")]
    [Tooltip("The IAM apikey.")]
    [SerializeField]
    private string _iamApikey;
    #endregion

    private SpeechToText _service;

    private string _modelNameToGet;
    private string _createdCustomizationID;
    private string _createdCorpusName = "the-jabberwocky-corpus";
    private string _customCorpusFilePath;
    private string _customWordsFilePath;
    private string _acousticResourceUrl = "https://ia802302.us.archive.org/10/items/Greatest_Speeches_of_the_20th_Century/TheFirstAmericaninEarthOrbit.mp3";
    private string _oggResourceUrl = "https://ia802302.us.archive.org/10/items/Greatest_Speeches_of_the_20th_Century/InauguralAddress-1981.ogg";
    private bool _isAudioLoaded = false;
    private string _createdAcousticModelId;
    private string _acousticResourceName = "unity-acoustic-resource";
    private string _createdAcousticModelName = "unity-example-acoustic-model";
    private byte[] _acousticResourceData;
    private string _acousticResourceMimeType;
    private byte[] _oggResourceData;
    private string _oggResourceMimeType;
    private bool _isOggLoaded = false;
    private string _grammarFilePath;

    private bool _recognizeTested = false;
    private bool _recognizeOggTested = false;
    private bool _getModelsTested = false;
    private bool _getModelTested = false;
    private bool _getCustomizationsTested = false;
    private bool _createCustomizationsTested = false;
    private bool _getCustomizationTested = false;
    private bool _deleteCustomizationsTested = false;
    private bool _trainCustomizationTested = false;
    //private bool _upgradeCustomizationTested = false;
    private bool _resetCustomizationTested = false;
    private bool _getCustomCorporaTested = false;
    private bool _addCustomCorpusTested = false;
    private bool _getCustomCorpusTested = false;
    private bool _getCustomWordsTested = false;
    private bool _addCustomWordsFromPathTested = false;
    private bool _addCustomWordsFromObjectTested = false;
    private bool _getCustomWordTested = false;
    private bool _deleteCustomWordTested = false;
    private bool _deleteCustomCorpusTested = false;
    private bool _getAcousticCustomizationsTested = false;
    private bool _createAcousticCustomizationsTested = false;
    private bool _deleteAcousticCustomizationsTested = false;
    private bool _getAcousticCustomizationTested = false;
    private bool _trainAcousticCustomizationsTested = false;
    private bool _resetAcousticCustomizationsTested = false;
    private bool _getAcousticResourcesTested = false;
    private bool _getAcousticResourceTested = false;
    private bool _addAcousticResourcesTested = false;
    private bool _isCustomizationReady = false;
    private bool _isAcousticCustomizationReady = false;
    private bool _deleteAcousticResource = false;
    private bool _readyToContinue = false;
    private float _delayTimeInSeconds = 10f;

    private bool _listGrammarsTested = false;
    private bool _addGrammarTested = false;
    private bool _getGrammarTested = false;
    private bool _deleteGrammarTested = false;
    private string _createdGrammarId;
    private string _grammarFileContentType = "application/srgs";
    private string _grammarName = "unity-integration-test-grammar";

    void Start()
    {
        LogSystem.InstallDefaultReactors();
        _customCorpusFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/speech-to-text/theJabberwocky-utf8.txt";
        _customWordsFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/speech-to-text/test-stt-words.json";
        _grammarFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/speech-to-text/confirm.abnf";
        _acousticResourceMimeType = Utility.GetMimeType(Path.GetExtension(_acousticResourceUrl));
        _oggResourceMimeType = Utility.GetMimeType(Path.GetExtension(_oggResourceUrl));
        Runnable.Run(CreateService());
    }

    private IEnumerator CreateService()
    {
        if (string.IsNullOrEmpty(_iamApikey))
        {
            throw new WatsonException("Plesae provide IAM ApiKey for the service.");
        }

        //  Create credential and instantiate service
        Credentials credentials = null;

        //  Authenticate using iamApikey
        TokenOptions tokenOptions = new TokenOptions()
        {
            IamApiKey = _iamApikey
        };

        credentials = new Credentials(tokenOptions, _serviceUrl);

        //  Wait for tokendata
        while (!credentials.HasIamTokenData())
            yield return null;

        _service = new SpeechToText(credentials);
        _service.StreamMultipart = true;

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        Runnable.Run(DownloadAcousticResource());
        while (!_isAudioLoaded)
            yield return null;

        Runnable.Run(DownloadOggResource());
        while (!_isOggLoaded)
            yield return null;

        //  Recognize
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to recognize");
        List<string> keywords = new List<string>();
        keywords.Add("speech");
        _service.KeywordsThreshold = 0.5f;
        _service.InactivityTimeout = 120;
        _service.StreamMultipart = false;
        _service.Keywords = keywords.ToArray();
        _service.Recognize(HandleOnRecognize, OnFail, _acousticResourceData, _acousticResourceMimeType);
        while (!_recognizeTested)
            yield return null;

        //  Recognize ogg
        Log.Debug("ExampleSpeechToText", "Attempting to recognize ogg: mimeType: {0} | _speechTText.StreamMultipart: {1}", _oggResourceMimeType, _service.StreamMultipart);
        _service.Recognize(HandleOnRecognizeOgg, OnFail, _oggResourceData, _oggResourceMimeType + ";codecs=vorbis");
        while (!_recognizeOggTested)
            yield return null;

        //  Get models
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get models");
        _service.GetModels(HandleGetModels, OnFail);
        while (!_getModelsTested)
            yield return null;

        //  Get model
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get model {0}", _modelNameToGet);
        _service.GetModel(HandleGetModel, OnFail, _modelNameToGet);
        while (!_getModelTested)
            yield return null;

        //  Get customizations
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get customizations");
        _service.GetCustomizations(HandleGetCustomizations, OnFail);
        while (!_getCustomizationsTested)
            yield return null;

        //  Create customization
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting create customization");
        _service.CreateCustomization(HandleCreateCustomization, OnFail, "unity-test-customization", "en-US_BroadbandModel", "Testing customization unity");
        while (!_createCustomizationsTested)
            yield return null;

        //  Get customization
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get customization {0}", _createdCustomizationID);
        _service.GetCustomization(HandleGetCustomization, OnFail, _createdCustomizationID);
        while (!_getCustomizationTested)
            yield return null;

        //  Get custom corpora
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get custom corpora for {0}", _createdCustomizationID);
        _service.GetCustomCorpora(HandleGetCustomCorpora, OnFail, _createdCustomizationID);
        while (!_getCustomCorporaTested)
            yield return null;

        //  Add custom corpus
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to add custom corpus {1} in customization {0}", _createdCustomizationID, _createdCorpusName);
        string corpusData = File.ReadAllText(_customCorpusFilePath);
        _service.AddCustomCorpus(HandleAddCustomCorpus, OnFail, _createdCustomizationID, _createdCorpusName, true, corpusData);
        while (!_addCustomCorpusTested)
            yield return null;

        //  Get custom corpus
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get custom corpus {1} in customization {0}", _createdCustomizationID, _createdCorpusName);
        _service.GetCustomCorpus(HandleGetCustomCorpus, OnFail, _createdCustomizationID, _createdCorpusName);
        while (!_getCustomCorpusTested)
            yield return null;

        //  Wait for customization
        Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
        while (!_isCustomizationReady)
            yield return null;

        //  Get custom words
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get custom words.");
        _service.GetCustomWords(HandleGetCustomWords, OnFail, _createdCustomizationID);
        while (!_getCustomWordsTested)
            yield return null;

        //  Add custom words from path
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to add custom words in customization {0} using Words json path {1}", _createdCustomizationID, _customWordsFilePath);
        string customWords = File.ReadAllText(_customWordsFilePath);
        _service.AddCustomWords(HandleAddCustomWordsFromPath, OnFail, _createdCustomizationID, customWords);
        while (!_addCustomWordsFromPathTested)
            yield return null;

        //  Wait for customization
        _isCustomizationReady = false;
        Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
        while (!_isCustomizationReady)
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

        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to add custom words in customization {0} using Words object", _createdCustomizationID);
        _service.AddCustomWords(HandleAddCustomWordsFromObject, OnFail, _createdCustomizationID, words);
        while (!_addCustomWordsFromObjectTested)
            yield return null;

        //  Wait for customization
        _isCustomizationReady = false;
        Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
        while (!_isCustomizationReady)
            yield return null;

        //  Get custom word
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get custom word {1} in customization {0}", _createdCustomizationID, words.words[0].word);
        _service.GetCustomWord(HandleGetCustomWord, OnFail, _createdCustomizationID, words.words[0].word);
        while (!_getCustomWordTested)
            yield return null;

        //  Train customization
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to train customization {0}", _createdCustomizationID);
        _service.TrainCustomization(HandleTrainCustomization, OnFail, _createdCustomizationID);
        while (!_trainCustomizationTested)
            yield return null;

        //  Wait for customization
        _isCustomizationReady = false;
        Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
        while (!_isCustomizationReady)
            yield return null;

        //  Delete custom word
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to delete custom word {1} in customization {0}", _createdCustomizationID, words.words[2].word);
        _service.DeleteCustomWord(HandleDeleteCustomWord, OnFail, _createdCustomizationID, words.words[2].word);
        while (!_deleteCustomWordTested)
            yield return null;

        //  Delay
        Log.Debug("ExampleSpeechToText.Examples()", string.Format("Delaying delete environment for {0} sec", _delayTimeInSeconds));
        Runnable.Run(Delay(_delayTimeInSeconds));
        while (!_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Delete custom corpus
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to delete custom corpus {1} in customization {0}", _createdCustomizationID, _createdCorpusName);
        _service.DeleteCustomCorpus(HandleDeleteCustomCorpus, OnFail, _createdCustomizationID, _createdCorpusName);
        while (!_deleteCustomCorpusTested)
            yield return null;

        //  Delay
        Log.Debug("ExampleSpeechToText.Examples()", string.Format("Delaying delete environment for {0} sec", _delayTimeInSeconds));
        Runnable.Run(Delay(_delayTimeInSeconds));
        while (!_readyToContinue)
            yield return null;

        _readyToContinue = false;
        //  Reset customization
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to reset customization {0}", _createdCustomizationID);
        _service.ResetCustomization(HandleResetCustomization, OnFail, _createdCustomizationID);
        while (!_resetCustomizationTested)
            yield return null;

        //  Delay
        Log.Debug("ExampleSpeechToText.Examples()", string.Format("Delaying delete environment for {0} sec", _delayTimeInSeconds));
        Runnable.Run(Delay(_delayTimeInSeconds));
        while (!_readyToContinue)
            yield return null;

        //  List Grammars
        Log.Debug("TestSpeechToText.Examples()", "Attempting to list grammars {0}", _createdCustomizationID);
        _service.ListGrammars(OnListGrammars, OnFail, _createdCustomizationID);
        while (!_listGrammarsTested)
            yield return null;

        //  Add Grammar
        Log.Debug("TestSpeechToText.Examples()", "Attempting to add grammar {0}", _createdCustomizationID);
        string grammarFile = File.ReadAllText(_grammarFilePath);
        _service.AddGrammar(OnAddGrammar, OnFail, _createdCustomizationID, _grammarName, grammarFile, _grammarFileContentType);
        while (!_addGrammarTested)
            yield return null;

        //  Get Grammar
        Log.Debug("TestSpeechToText.Examples()", "Attempting to get grammar {0}", _createdCustomizationID);
        _service.GetGrammar(OnGetGrammar, OnFail, _createdCustomizationID, _grammarName);
        while (!_getGrammarTested)
            yield return null;

        //  Wait for customization
        _isCustomizationReady = false;
        Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
        while (!_isCustomizationReady)
            yield return null;

        //  Delete Grammar
        Log.Debug("TestSpeechToText.Examples()", "Attempting to delete grammar {0}", _createdCustomizationID);
        _service.DeleteGrammar(OnDeleteGrammar, OnFail, _createdCustomizationID, _grammarName);
        while (!_deleteGrammarTested)
            yield return null;

        _readyToContinue = false;
        //  Delete customization
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to delete customization {0}", _createdCustomizationID);
        _service.DeleteCustomization(HandleDeleteCustomization, OnFail, _createdCustomizationID);
        while (!_deleteCustomizationsTested)
            yield return null;

        //  List acoustic customizations
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get acoustic customizations");
        _service.GetCustomAcousticModels(HandleGetCustomAcousticModels, OnFail);
        while (!_getAcousticCustomizationsTested)
            yield return null;

        //  Create acoustic customization
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to create acoustic customization");
        _service.CreateAcousticCustomization(HandleCreateAcousticCustomization, OnFail, _createdAcousticModelName);
        while (!_createAcousticCustomizationsTested)
            yield return null;

        //  Get acoustic customization
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get acoustic customization {0}", _createdAcousticModelId);
        _service.GetCustomAcousticModel(HandleGetCustomAcousticModel, OnFail, _createdAcousticModelId);
        while (!_getAcousticCustomizationTested)
            yield return null;

        while (!_isAudioLoaded)
            yield return null;

        //  Create acoustic resource
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to create audio resource {1} on {0}", _createdAcousticModelId, _acousticResourceName);
        string mimeType = Utility.GetMimeType(Path.GetExtension(_acousticResourceUrl));
        _service.AddAcousticResource(HandleAddAcousticResource, OnFail, _createdAcousticModelId, _acousticResourceName, mimeType, mimeType, true, _acousticResourceData);
        while (!_addAcousticResourcesTested)
            yield return null;

        //  Wait for customization
        _isAcousticCustomizationReady = false;
        Runnable.Run(CheckAcousticCustomizationStatus(_createdAcousticModelId));
        while (!_isAcousticCustomizationReady)
            yield return null;

        //  List acoustic resources
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get audio resources {0}", _createdAcousticModelId);
        _service.GetCustomAcousticResources(HandleGetCustomAcousticResources, OnFail, _createdAcousticModelId);
        while (!_getAcousticResourcesTested)
            yield return null;

        //  Train acoustic customization
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to train acoustic customization {0}", _createdAcousticModelId);
        _service.TrainAcousticCustomization(HandleTrainAcousticCustomization, OnFail, _createdAcousticModelId, null, true);
        while (!_trainAcousticCustomizationsTested)
            yield return null;

        //  Get acoustic resource
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get audio resource {1} from {0}", _createdAcousticModelId, _acousticResourceName);
        _service.GetCustomAcousticResource(HandleGetCustomAcousticResource, OnFail, _createdAcousticModelId, _acousticResourceName);
        while (!_getAcousticResourceTested)
            yield return null;

        //  Wait for customization
        _isAcousticCustomizationReady = false;
        Runnable.Run(CheckAcousticCustomizationStatus(_createdAcousticModelId));
        while (!_isAcousticCustomizationReady)
            yield return null;

        //  Delay
        Log.Debug("ExampleSpeechToText.Examples()", string.Format("Delaying delete acoustic resource for {0} sec", _delayTimeInSeconds));
        Runnable.Run(Delay(_delayTimeInSeconds));
        while (!_readyToContinue)
            yield return null;

        //  Delete acoustic resource
        DeleteAcousticResource();
        while (!_deleteAcousticResource)
            yield return null;

        //  Reset acoustic customization
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to reset acoustic customization {0}", _createdAcousticModelId);
        _service.ResetAcousticCustomization(HandleResetAcousticCustomization, OnFail, _createdAcousticModelId);
        while (!_resetAcousticCustomizationsTested)
            yield return null;

        //  Delay
        Log.Debug("ExampleSpeechToText.Examples()", string.Format("Delaying delete acoustic customization for {0} sec", _delayTimeInSeconds));
        Runnable.Run(Delay(_delayTimeInSeconds));
        while (!_readyToContinue)
            yield return null;

        //  Delete acoustic customization
        DeleteAcousticCustomization();
        while (!_deleteAcousticCustomizationsTested)
            yield return null;

        //  Delay
        Log.Debug("ExampleSpeechToText.Examples()", string.Format("Delaying complete for {0} sec", _delayTimeInSeconds));
        Runnable.Run(Delay(_delayTimeInSeconds));
        while (!_readyToContinue)
            yield return null;

        Log.Debug("ExampleSpeechToText.Examples()", "Speech to Text examples complete.");
    }

    private void DeleteAcousticResource()
    {
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to delete audio resource {1} from {0}", _createdAcousticModelId, _acousticResourceName);
        _service.DeleteAcousticResource(HandleDeleteAcousticResource, OnFail, _createdAcousticModelId, _acousticResourceName);
    }

    private void DeleteAcousticCustomization()
    {
        Log.Debug("ExampleSpeechToText.Examples()", "Attempting to delete acoustic customization {0}", _createdAcousticModelId);
        _service.DeleteAcousticCustomization(HandleDeleteAcousticCustomization, OnFail, _createdAcousticModelId);
    }

    private void HandleGetModels(ModelSet result, Dictionary<string, object> customData)
    {

        Log.Debug("ExampleSpeechToText.Examples()", "Speech to Text - Get models response: {0}", customData["json"].ToString());
        _modelNameToGet = (result.models[UnityEngine.Random.Range(0, result.models.Length - 1)] as Model).name;
        _getModelsTested = true;
    }

    private void HandleGetModel(Model model, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.Examples()", "Speech to Text - Get model response: {0}", customData["json"].ToString());
        _getModelTested = true;
    }

    private void HandleOnRecognize(SpeechRecognitionEvent result, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleOnRecognize()", "Speech to Text - Get model response: {0}", customData["json"].ToString());
        _recognizeTested = true;
    }

    private void HandleOnRecognizeOgg(SpeechRecognitionEvent result, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleOnRecognizeOgg()", "Speech to Text - Get model response: {0}", customData["json"].ToString());
        _recognizeOggTested = true;
    }

    private void HandleGetCustomizations(Customizations customizations, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleGetCustomizations()", "Speech to Text - Get customizations response: {0}", customData["json"].ToString());
        _getCustomizationsTested = true;
    }

    private void HandleCreateCustomization(CustomizationID customizationID, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleCreateCustomization()", "Speech to Text - Create customization response: {0}", customData["json"].ToString());
        _createdCustomizationID = customizationID.customization_id;
        _createCustomizationsTested = true;
    }

    private void HandleGetCustomization(Customization customization, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleGetCustomization()", "Speech to Text - Get customization response: {0}", customData["json"].ToString());
        _getCustomizationTested = true;
    }

    private void HandleDeleteCustomization(bool success, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleDeleteCustomization()", customData["json"].ToString());
        _createdCustomizationID = default(string);
        _deleteCustomizationsTested = true;
    }

    private void HandleTrainCustomization(bool success, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleTrainCustomization()", customData["json"].ToString());
        _trainCustomizationTested = true;
    }

    //private void HandleUpgradeCustomization(bool success, Dictionary<string, object> customData)
    //{
    //    if (success)
    //    {
    //        Log.Debug("ExampleSpeechToText.HandleUpgradeCustomization()", "Upgrade customization {0}!", customData["json"].ToString());
    //    }
    //    else
    //    {
    //        Log.Debug("ExampleSpeechToText.HandleUpgradeCustomization()", "Failed to upgrade customization!");
    //    }

    //    _upgradeCustomizationTested = true;
    //}

    private void HandleResetCustomization(bool success, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleResetCustomization()", "{0}", customData["json"].ToString());
        _resetCustomizationTested = true;
    }

    private void HandleGetCustomCorpora(Corpora corpora, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleGetCustomCorpora()", "{0}", customData["json"].ToString());
        _getCustomCorporaTested = true;
    }

    private void HandleDeleteCustomCorpus(bool success, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleDeleteCustomCorpus()", "{0}", customData["json"].ToString());
        _deleteCustomCorpusTested = true;
    }

    private void HandleAddCustomCorpus(bool success, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleAddCustomCorpus()", "{0}", customData["json"].ToString());
        _addCustomCorpusTested = true;
    }

    private void HandleGetCustomCorpus(Corpus corpus, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleGetCustomCorpus()", "{0}", customData["json"].ToString());
        _getCustomCorpusTested = true;
    }

    private void HandleGetCustomWords(WordsList wordList, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleGetCustomWords()", "{0}", customData["json"].ToString());
        _getCustomWordsTested = true;
    }

    private void HandleAddCustomWordsFromPath(bool success, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleAddCustomWordsFromPath()", "{0}", customData["json"].ToString());
        _addCustomWordsFromPathTested = true;
    }

    private void HandleAddCustomWordsFromObject(bool success, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleAddCustomWordsFromObject()", "{0}", customData["json"].ToString());
        _addCustomWordsFromObjectTested = true;
    }

    private void HandleDeleteCustomWord(bool success, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleDeleteCustomWord()", "{0}", customData["json"].ToString());
        _deleteCustomWordTested = true;
    }

    private void HandleGetCustomWord(WordData word, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleGetCustomWord()", "{0}", customData["json"].ToString());
        _getCustomWordTested = true;
    }

    private void HandleGetCustomAcousticModels(AcousticCustomizations acousticCustomizations, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleGetCustomAcousticModels()", "{0}", customData["json"].ToString());
        _getAcousticCustomizationsTested = true;
    }

    private void HandleCreateAcousticCustomization(CustomizationID customizationID, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleCreateAcousticCustomization()", "{0}", customData["json"].ToString());
        _createdAcousticModelId = customizationID.customization_id;
        _createAcousticCustomizationsTested = true;
    }

    private void HandleGetCustomAcousticModel(AcousticCustomization acousticCustomization, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleGetCustomAcousticModel()", "{0}", customData["json"].ToString());
        _getAcousticCustomizationTested = true;
    }

    private void HandleTrainAcousticCustomization(bool success, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleTrainAcousticCustomization()", "{0}", customData["json"].ToString());
        _trainAcousticCustomizationsTested = true;
    }

    private void HandleGetCustomAcousticResources(AudioResources audioResources, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleGetCustomAcousticResources()", "{0}", customData["json"].ToString());
        _getAcousticResourcesTested = true;
    }

    private void HandleAddAcousticResource(bool success, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleAddAcousticResource()", "{0}", customData["json"].ToString());
        _addAcousticResourcesTested = true;
    }

    private void HandleGetCustomAcousticResource(AudioListing audioListing, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleGetCustomAcousticResource()", "{0}", customData["json"].ToString());
        _getAcousticResourceTested = true;
    }

    private void HandleResetAcousticCustomization(bool success, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleResetAcousticCustomization()", "{0}", customData["json"].ToString());
        _resetAcousticCustomizationsTested = true;
    }

    private void HandleDeleteAcousticResource(bool success, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleDeleteAcousticResource()", "{0}", customData["json"].ToString());
        _deleteAcousticResource = true;
    }

    private void HandleDeleteAcousticCustomization(bool success, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.HandleDeleteAcousticCustomization()", "{0}", customData["json"].ToString());
        if (success)
            _deleteAcousticCustomizationsTested = true;
        else
            DeleteAcousticCustomization();
    }

    private void OnListGrammars(Grammars response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.OnListGrammars()", "{0}", customData["json"].ToString());
        _listGrammarsTested = true;
    }

    private void OnAddGrammar(object response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.OnAddGrammar()", "Success!");
        _addGrammarTested = true;
    }

    private void OnGetGrammar(Grammar response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.OnGetGrammar()", "{0}", customData["json"].ToString());
        _getGrammarTested = true;
    }

    private void OnDeleteGrammar(object response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleSpeechToText.OnDeleteGrammar()", "Success!");
        _deleteGrammarTested = true;
    }

    private IEnumerator CheckCustomizationStatus(string customizationID, float delay = 0.1f)
    {
        Log.Debug("ExampleSpeechToText.CheckCustomizationStatus()", "Checking customization status in {0} seconds...", delay.ToString());
        yield return new WaitForSeconds(delay);

        //  passing customizationID in custom data
        Dictionary<string, object> customData = new Dictionary<string, object>();
        customData["customizationID"] = customizationID;
        _service.GetCustomization(OnCheckCustomizationStatus, OnFail, customizationID, customData);
    }

    private void OnCheckCustomizationStatus(Customization customization, Dictionary<string, object> customData)
    {
        if (customization != null)
        {
            Log.Debug("ExampleSpeechToText.OnCheckCustomizationStatus()", "Customization status: {0}", customization.status);
            if (customization.status != "ready" && customization.status != "available")
                Runnable.Run(CheckCustomizationStatus(customData["customizationID"].ToString(), 5f));
            else
                _isCustomizationReady = true;
        }
        else
        {
            Log.Debug("ExampleSpeechToText.OnCheckCustomizationStatus()", "Check customization status failed!");
        }
    }

    private IEnumerator CheckAcousticCustomizationStatus(string customizationID, float delay = 0.1f)
    {
        Log.Debug("ExampleSpeechToText.CheckAcousticCustomizationStatus()", "Checking acoustic customization status in {0} seconds...", delay.ToString());
        yield return new WaitForSeconds(delay);

        //	passing customizationID in custom data
        Dictionary<string, object> customData = new Dictionary<string, object>();
        customData["customizationID"] = customizationID;
        _service.GetCustomAcousticModel(OnCheckAcousticCustomizationStatus, OnFail, customizationID, customData);
    }

    private void OnCheckAcousticCustomizationStatus(AcousticCustomization acousticCustomization, Dictionary<string, object> customData)
    {
        if (acousticCustomization != null)
        {
            Log.Debug("ExampleSpeechToText.CheckAcousticCustomizationStatus()", "Acoustic customization status: {0}", acousticCustomization.status);
            if (acousticCustomization.status != "ready" && acousticCustomization.status != "available")
                Runnable.Run(CheckAcousticCustomizationStatus(customData["customizationID"].ToString(), 5f));
            else
                _isAcousticCustomizationReady = true;
        }
        else
        {
            Log.Debug("ExampleSpeechToText.CheckAcousticCustomizationStatus()", "Check acoustic customization status failed!");
        }
    }

    private IEnumerator Delay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        _readyToContinue = true;
    }

    private IEnumerator DownloadAcousticResource()
    {
        Log.Debug("ExampleSpeechToText.DownloadAcousticResource()", "downloading acoustic resource from {0}", _acousticResourceUrl);
        using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(_acousticResourceUrl))
        {
            yield return unityWebRequest.SendWebRequest();

            Log.Debug("ExampleSpeechToText.DownloadAcousticResource()", "acoustic resource downloaded");
            _acousticResourceData = unityWebRequest.downloadHandler.data;
            _isAudioLoaded = true;
        }
    }

    private IEnumerator DownloadOggResource()
    {
        Log.Debug("ExampleSpeechToText", "downloading ogg resource from {0}", _oggResourceUrl);
        using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(_oggResourceUrl))
        {
            yield return unityWebRequest.SendWebRequest();

            Log.Debug("ExampleSpeechToText", "ogg resource downloaded");
            _oggResourceData = unityWebRequest.downloadHandler.data;
            _isOggLoaded = true;
        }
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("ExampleSpeechToText.OnFail()", "Error received: {0}", error.ToString());
    }
}
