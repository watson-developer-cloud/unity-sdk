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
using System.IO;
using System.Collections.Generic;

public class ExampleSpeechToText : MonoBehaviour
{
    private string _username = null;
    private string _password = null;
    private string _url = null;

    private AudioClip _audioClip;
    private SpeechToText _speechToText;

    private string _modelNameToGet;
    private string _createdCustomizationID;
    private string _createdCorpusName = "the-jabberwocky-corpus";
    private string _customCorpusFilePath;
    private string _customWordsFilePath;
    private string _wavFilePath;
    private string _createdAcousticModelId;
    private string _acousticResourceName = "unity acoustic resource";
    private string _createdAcousticModelName = "unity example acoustic model";

    private bool _recognizeTested = false;
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
    private bool _deleteAcousticResourcesTested = false;
    private bool _getAcousticResourceTested = false;
    private bool _addAcousticResourcesTested = false;

    private bool _isCustomizationReady = false;
    private bool _isAcousticCustomizationReady = false;
    private bool _readyToContinue = false;
    private float _delayTimeInSeconds = 10f;

    void Start()
    {
        LogSystem.InstallDefaultReactors();

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_username, _password, _url);

        _speechToText = new SpeechToText(credentials);
        _customCorpusFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/theJabberwocky-utf8.txt";
        _customWordsFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/test-stt-words.json";
        _wavFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/test-audio.wav";
        _audioClip = WaveFile.ParseWAV("testClip", File.ReadAllBytes(_wavFilePath));

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        ////  Recognize
        //Log.Debug("ExampleSpeechToText", "Attempting to recognize");
        //List<string> keywords = new List<string>();
        //keywords.Add("speech");
        //_speechToText.KeywordsThreshold = 0.5f;
        //_speechToText.Keywords = keywords.ToArray();
        //_speechToText.Recognize(_audioClip, HandleOnRecognize);
        //while (!_recognizeTested)
        //    yield return null;

        ////  Get models
        //Log.Debug("ExampleSpeechToText", "Attempting to get models");
        //_speechToText.GetModels(HandleGetModels);
        //while (!_getModelsTested)
        //    yield return null;

        ////  Get model
        //Log.Debug("ExampleSpeechToText", "Attempting to get model {0}", _modelNameToGet);
        //_speechToText.GetModel(HandleGetModel, _modelNameToGet);
        //while (!_getModelTested)
        //    yield return null;

        ////  Get customizations
        //Log.Debug("ExampleSpeechToText", "Attempting to get customizations");
        //_speechToText.GetCustomizations(HandleGetCustomizations);
        //while (!_getCustomizationsTested)
        //    yield return null;

        ////  Create customization
        //Log.Debug("ExampleSpeechToText", "Attempting create customization");
        //_speechToText.CreateCustomization(HandleCreateCustomization, "unity-test-customization", "en-US_BroadbandModel", "Testing customization unity");
        //while (!_createCustomizationsTested)
        //    yield return null;

        ////  Get customization
        //Log.Debug("ExampleSpeechToText", "Attempting to get customization {0}", _createdCustomizationID);
        //_speechToText.GetCustomization(HandleGetCustomization, _createdCustomizationID);
        //while (!_getCustomizationTested)
        //    yield return null;

        ////  Get custom corpora
        //Log.Debug("ExampleSpeechToText", "Attempting to get custom corpora for {0}", _createdCustomizationID);
        //_speechToText.GetCustomCorpora(HandleGetCustomCorpora, _createdCustomizationID);
        //while (!_getCustomCorporaTested)
        //    yield return null;

        ////  Add custom corpus
        //Log.Debug("ExampleSpeechToText", "Attempting to add custom corpus {1} in customization {0}", _createdCustomizationID, _createdCorpusName);
        //string corpusData = File.ReadAllText(_customCorpusFilePath);
        //_speechToText.AddCustomCorpus(HandleAddCustomCorpus, _createdCustomizationID, _createdCorpusName, true, corpusData);
        //while (!_addCustomCorpusTested)
        //    yield return null;

        ////  Get custom corpus
        //Log.Debug("ExampleSpeechToText", "Attempting to get custom corpus {1} in customization {0}", _createdCustomizationID, _createdCorpusName);
        //_speechToText.GetCustomCorpus(HandleGetCustomCorpus, _createdCustomizationID, _createdCorpusName);
        //while (!_getCustomCorpusTested)
        //    yield return null;

        ////  Wait for customization
        //Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
        //while (!_isCustomizationReady)
        //    yield return null;

        ////  Get custom words
        //Log.Debug("ExampleSpeechToText", "Attempting to get custom words.");
        //_speechToText.GetCustomWords(HandleGetCustomWords, _createdCustomizationID);
        //while (!_getCustomWordsTested)
        //    yield return null;

        ////  Add custom words from path
        //Log.Debug("ExampleSpeechToText", "Attempting to add custom words in customization {0} using Words json path {1}", _createdCustomizationID, _customWordsFilePath);
        //string customWords = File.ReadAllText(_customWordsFilePath);
        //_speechToText.AddCustomWords(HandleAddCustomWordsFromPath, _createdCustomizationID, customWords);
        //while (!_addCustomWordsFromPathTested)
        //    yield return null;

        ////  Wait for customization
        //_isCustomizationReady = false;
        //Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
        //while (!_isCustomizationReady)
        //    yield return null;

        ////  Add custom words from object
        //Words words = new Words();
        //Word w0 = new Word();
        //List<Word> wordList = new List<Word>();
        //w0.word = "mikey";
        //w0.sounds_like = new string[1];
        //w0.sounds_like[0] = "my key";
        //w0.display_as = "Mikey";
        //wordList.Add(w0);
        //Word w1 = new Word();
        //w1.word = "charlie";
        //w1.sounds_like = new string[1];
        //w1.sounds_like[0] = "char lee";
        //w1.display_as = "Charlie";
        //wordList.Add(w1);
        //Word w2 = new Word();
        //w2.word = "bijou";
        //w2.sounds_like = new string[1];
        //w2.sounds_like[0] = "be joo";
        //w2.display_as = "Bijou";
        //wordList.Add(w2);
        //words.words = wordList.ToArray();

        //Log.Debug("ExampleSpeechToText", "Attempting to add custom words in customization {0} using Words object", _createdCustomizationID);
        //_speechToText.AddCustomWords(HandleAddCustomWordsFromObject, _createdCustomizationID, words);
        //while (!_addCustomWordsFromObjectTested)
        //    yield return null;

        ////  Wait for customization
        //_isCustomizationReady = false;
        //Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
        //while (!_isCustomizationReady)
        //    yield return null;

        ////  Get custom word
        //Log.Debug("ExampleSpeechToText", "Attempting to get custom word {1} in customization {0}", _createdCustomizationID, words.words[0].word);
        //_speechToText.GetCustomWord(HandleGetCustomWord, _createdCustomizationID, words.words[0].word);
        //while (!_getCustomWordTested)
        //    yield return null;

        ////  Train customization
        //Log.Debug("ExampleSpeechToText", "Attempting to train customization {0}", _createdCustomizationID);
        //_speechToText.TrainCustomization(HandleTrainCustomization, _createdCustomizationID);
        //while (!_trainCustomizationTested)
        //    yield return null;

        ////  Wait for customization
        //_isCustomizationReady = false;
        //Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
        //while (!_isCustomizationReady)
        //    yield return null;

        ////  Upgrade customization - not currently implemented in service
        ////Log.Debug("ExampleSpeechToText", "Attempting to upgrade customization {0}", _createdCustomizationID);
        ////_speechToText.UpgradeCustomization(HandleUpgradeCustomization, _createdCustomizationID);
        ////while (!_upgradeCustomizationTested)
        ////    yield return null;

        ////  Delete custom word
        //Log.Debug("ExampleSpeechToText", "Attempting to delete custom word {1} in customization {0}", _createdCustomizationID, words.words[2].word);
        //_speechToText.DeleteCustomWord(HandleDeleteCustomWord, _createdCustomizationID, words.words[2].word);
        //while (!_deleteCustomWordTested)
        //    yield return null;

        ////  Delay
        //Log.Debug("ExampleDiscovery", string.Format("Delaying delete environment for {0} sec", _delayTimeInSeconds));
        //Runnable.Run(Delay(_delayTimeInSeconds));
        //while (!_readyToContinue)
        //    yield return null;

        //_readyToContinue = false;
        ////  Delete custom corpus
        //Log.Debug("ExampleSpeechToText", "Attempting to delete custom corpus {1} in customization {0}", _createdCustomizationID, _createdCorpusName);
        //_speechToText.DeleteCustomCorpus(HandleDeleteCustomCorpus, _createdCustomizationID, _createdCorpusName);
        //while (!_deleteCustomCorpusTested)
        //    yield return null;

        ////  Delay
        //Log.Debug("ExampleDiscovery", string.Format("Delaying delete environment for {0} sec", _delayTimeInSeconds));
        //Runnable.Run(Delay(_delayTimeInSeconds));
        //while (!_readyToContinue)
        //    yield return null;

        //_readyToContinue = false;
        ////  Reset customization
        //Log.Debug("ExampleSpeechToText", "Attempting to reset customization {0}", _createdCustomizationID);
        //_speechToText.ResetCustomization(HandleResetCustomization, _createdCustomizationID);
        //while (!_resetCustomizationTested)
        //    yield return null;
        
        ////  Delay
        //Log.Debug("ExampleDiscovery", string.Format("Delaying delete environment for {0} sec", _delayTimeInSeconds));
        //Runnable.Run(Delay(_delayTimeInSeconds));
        //while (!_readyToContinue)
        //    yield return null;

        //_readyToContinue = false;
        ////  Delete customization
        //Log.Debug("ExampleSpeechToText", "Attempting to delete customization {0}", _createdCustomizationID);
        //_speechToText.DeleteCustomization(HandleDeleteCustomization, _createdCustomizationID);
        //while (!_deleteCustomizationsTested)
        //    yield return null;

        //  List acoustic customizations
        Log.Debug("ExampleSpeechToText", "Attempting to get acoustic customizations");
        _speechToText.GetCustomAcousticModels(HandleGetCustomAcousticModels);
        while (!_getAcousticCustomizationsTested)
            yield return null;

        //  Create acoustic customization
        Log.Debug("ExampleSpeechToText", "Attempting to create acoustic customization");
        _speechToText.CreateAcousticCustomization(HandleCreateAcousticCustomization, _createdAcousticModelName);
        while (!_createAcousticCustomizationsTested)
            yield return null;

        //  Get acoustic customization
        Log.Debug("ExampleSpeechToText", "Attempting to get acoustic customization {0}", _createdAcousticModelId);
        _speechToText.GetCustomAcousticModel(HandleGetCustomAcousticModel, _createdAcousticModelId);
        while (!_getAcousticCustomizationTested)
            yield return null;

        //  List acoustic resources
        Log.Debug("ExampleSpeechToText", "Attempting to get audio resources {0}", _createdAcousticModelId);
        _speechToText.GetCustomAcousticResources(HandleGetCustomAcousticResources, _createdAcousticModelId);
        while (!_getAcousticResourcesTested)
            yield return null;

        //  Create acoustic resource
        Log.Debug("ExampleSpeechToText", "Attempting to create audio resource {1} on {0}", _createdCustomizationID, _acousticResourceName);
        _speechToText.AddAcousticResource(HandleAddAcousticResource, _createdAcousticModelId, _acousticResourceName, "audio/wav", "audio/wav", true, File.ReadAllBytes(_wavFilePath));
        while (!_addAcousticResourcesTested)
            yield return null;

        //  Wait for customization
        _isAcousticCustomizationReady = false;
        Runnable.Run(CheckAcousticCustomizationStatus(_createdAcousticModelId));
        while (!_isAcousticCustomizationReady)
            yield return null;

        //  Train acoustic customization
        Log.Debug("ExampleSpeechToText", "Attempting to train acoustic customization {0}", _createdAcousticModelId);
        _speechToText.TrainAcousticCustomization(HandleTrainAcousticCustomization, _createdAcousticModelId);
        while (!_trainAcousticCustomizationsTested)
            yield return null;
        
        //  Get acoustic resource
        Log.Debug("ExampleSpeechToText", "Attempting to get audio resource {1} from {0}", _createdCustomizationID, _acousticResourceName);
        _speechToText.GetCustomAcousticResource(HandleGetCustomAcousticResource, _createdAcousticModelId, _acousticResourceName);
        while (!_getAcousticResourceTested)
            yield return null;

        //  Reset acoustic customization
        Log.Debug("ExampleSpeechToText", "Attempting to reset acoustic customization {0}", _createdAcousticModelId);
        _speechToText.ResetAcousticCustomization(HandleResetAcousticCustomization, _createdAcousticModelId);
        while (!_resetAcousticCustomizationsTested)
            yield return null;

        //  Delay
        Log.Debug("ExampleDiscovery", string.Format("Delaying delete environment for {0} sec", _delayTimeInSeconds));
        Runnable.Run(Delay(_delayTimeInSeconds));
        while (!_readyToContinue)
            yield return null;

        //  Delete acoustic resource
        Log.Debug("ExampleSpeechToText", "Attempting to delete audio resource {1} from {0}", _createdAcousticModelId, _acousticResourceName);
        _speechToText.DeleteAcousticResource(HandleDeleteAcousticResource, _createdAcousticModelId, _acousticResourceName);
        while (!_deleteAcousticResourcesTested)
            yield return null;

        //  Delay
        Log.Debug("ExampleDiscovery", string.Format("Delaying delete environment for {0} sec", _delayTimeInSeconds));
        Runnable.Run(Delay(_delayTimeInSeconds));
        while (!_readyToContinue)
            yield return null;

        //  Delete acoustic customization
        Log.Debug("ExampleSpeechToText", "Attempting to delete acoustic customization {0}", _createdAcousticModelId);
        _speechToText.DeleteAcousticCustomization(HandleDeleteAcousticCustomization, _createdAcousticModelId);
        while (!_deleteAcousticCustomizationsTested)
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

    private void HandleOnRecognize(SpeechRecognitionEvent result)
    {
        if (result != null && result.results.Length > 0)
        {
            foreach (var res in result.results)
            {
                foreach (var alt in res.alternatives)
                {
                    string text = alt.transcript;
                    Log.Debug("ExampleSpeechToText", string.Format("{0} ({1}, {2:0.00})\n", text, res.final ? "Final" : "Interim", alt.confidence));

                    if (res.final)
                        _recognizeTested = true;
                }

                if (res.keywords_result != null && res.keywords_result.keyword != null)
                {
                    foreach (var keyword in res.keywords_result.keyword)
                    {
                        Log.Debug("ExampleSpeechToText", "keyword: {0}, confidence: {1}, start time: {2}, end time: {3}", keyword.normalized_text, keyword.confidence, keyword.start_time, keyword.end_time);
                    }
                }
            }
        }
    }

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
            Log.Debug("ExampleSpeechToText", "Trained customization {0}!", _createdCustomizationID);
        }
        else
        {
            Log.Debug("ExampleSpeechToText", "Failed to train customization!");
        }

        _trainCustomizationTested = true;
    }

    //private void HandleUpgradeCustomization(bool success, string customData)
    //{
    //    if (success)
    //    {
    //        Log.Debug("ExampleSpeechToText", "Upgrade customization {0}!", _createdCustomizationID);
    //    }
    //    else
    //    {
    //        Log.Debug("ExampleSpeechToText", "Failed to upgrade customization!");
    //    }

    //    _upgradeCustomizationTested = true;
    //}

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

        _resetCustomizationTested = true;
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

        _deleteCustomCorpusTested = true;
    }

    private void HandleAddCustomCorpus(bool success, string customData)
    {
        if(success)
        {
            Log.Debug("ExampleSpeechToText", "Speech to Text - Add custom corpus response: succeeded!");
        }
        else
        {
            Log.Debug("ExampleSpeechToText", "Failed to add custom corpus!");
        }

        _addCustomCorpusTested = true;
    }

    private void HandleGetCustomCorpus(Corpus corpus, string customData)
    {
        Log.Debug("ExampleSpeechToText", "Speech to Text - Get custom corpus response: {0}", customData);
        _getCustomCorpusTested = true;
    }

    private void HandleGetCustomWords(WordsList wordList, string customData)
    {
        Log.Debug("ExampleSpeechToText", "Speech to Text - Get custom words response: {0}", customData);
        _getCustomWordsTested = true;
    }

    private void HandleAddCustomWordsFromPath(bool success, string customData)
    {
        if (success)
        {
            Log.Debug("ExampleSpeechToText", "Speech to Text - Add custom words from path response: succeeded!");
        }
        else
        {
            Log.Debug("ExampleSpeechToText", "Failed to delete custom word!");
        }

        _addCustomWordsFromPathTested = true;
    }

    private void HandleAddCustomWordsFromObject(bool success, string customData)
    {
        if (success)
        {
            Log.Debug("ExampleSpeechToText", "Speech to Text - Add custom words from object response: succeeded!");
        }
        else
        {
            Log.Debug("ExampleSpeechToText", "Failed to delete custom word!");
        }

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

        _deleteCustomWordTested = true;
    }

    private void HandleGetCustomWord(WordData word, string customData)
    {
        Log.Debug("ExampleSpeechToText", "Speech to Text - Get custom word response: {0}", customData);
        _getCustomWordTested = true;
    }

    private void HandleGetCustomAcousticModels(AcousticCustomizations acousticCustomizations, string customData)
    {
        Log.Debug("ExampleSpeechToText", "acousticCustomizations: {0}", customData);
        _getAcousticCustomizationsTested = true;
    }

    private void HandleCreateAcousticCustomization(CustomizationID customizationID, string customData)
    {
        Log.Debug("ExampleSpeechToText", "customizationId: {0}", customData);
        _createdAcousticModelId = customizationID.customization_id;
        _createAcousticCustomizationsTested = true;
    }

    private void HandleGetCustomAcousticModel(AcousticCustomization acousticCustomization, string customData)
    {
        Log.Debug("ExampleSpeechToText", "acousticCustomization: {0}", customData);
        _getAcousticCustomizationTested = true;
    }

    private void HandleTrainAcousticCustomization(bool success, string customData)
    {
        Log.Debug("ExampleSpeechToText", "train customization success: {0}", success);
        _trainAcousticCustomizationsTested = true;
    }

    private void HandleGetCustomAcousticResources(AudioResources audioResources, string customData)
    {
        Log.Debug("ExampleSpeechToText", "audioResources: {0}", customData);
        _getAcousticResourcesTested = true;
    }

    private void HandleAddAcousticResource(string customData)
    {
        Log.Debug("ExampleSpeechToText", "added acoustic resource: {0}", customData);
        _addAcousticResourcesTested = true;
    }

    private void HandleGetCustomAcousticResource(AudioListing audioListing, string customData)
    {
        Log.Debug("ExampleSpeechToText", "audioListing: {0}", customData);
        _getAcousticResourceTested = true;
    }

    private void HandleResetAcousticCustomization(bool success, string customData)
    {
        Log.Debug("ExampleSpeechToText", "reset customization success: {0}", success);
        _resetAcousticCustomizationsTested = true;
    }

    private void HandleDeleteAcousticResource(bool success, string customData)
    {
        Log.Debug("ExampleSpeechToText", "deleted acoustic resource: {0}", success);
        _deleteAcousticResourcesTested = true;
    }

    private void HandleDeleteAcousticCustomization(bool success, string customData)
    {
        Log.Debug("ExampleSpeechToText", "deleted acoustic customization: {0}", success);
        _deleteAcousticCustomizationsTested = true;
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

    private IEnumerator CheckAcousticCustomizationStatus(string customizationID, float delay = 0.1f)
    {
        Log.Debug("TestSpeechToText", "Checking acoustic customization status in {0} seconds...", delay.ToString());
        yield return new WaitForSeconds(delay);

        //	passing customizationID in custom data
        _speechToText.GetCustomAcousticModel(OnCheckAcousticCustomizationStatus, customizationID, customizationID);
    }

    private void OnCheckAcousticCustomizationStatus(AcousticCustomization acousticCustomization, string customData)
    {
        if (acousticCustomization != null)
        {
            Log.Debug("TestSpeechToText", "Acoustic customization status: {0}", acousticCustomization.status);
            if (acousticCustomization.status != "ready" && acousticCustomization.status != "available")
                Runnable.Run(CheckAcousticCustomizationStatus(customData, 5f));
            else
                _isAcousticCustomizationReady = true;
        }
        else
        {
            Log.Debug("TestSpeechToText", "Check acoustic customization status failed!");
        }
    }

    private IEnumerator Delay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        _readyToContinue = true;
    }
}