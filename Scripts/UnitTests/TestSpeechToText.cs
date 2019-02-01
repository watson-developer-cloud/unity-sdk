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
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Utility = IBM.Watson.DeveloperCloud.Utilities.Utility;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestSpeechToText : UnitTest
    {
        private fsSerializer _serializer = new fsSerializer();
        //private string _token = "<authentication-token>";

        private SpeechToText _speechToText;

        private string _modelNameToGet;
        private string _createdCustomizationID;
        private string _createdCorpusName = "the-jabberwocky-corpus";
        private string _customCorpusFilePath;
        private string _customWordsFilePath;
        private string _acousticResourceUrl = "https://ia802302.us.archive.org/10/items/Greatest_Speeches_of_the_20th_Century/TheFirstAmericaninEarthOrbit.mp3";
        private bool _isAudioLoaded = false;
        private string _createdAcousticModelId;
        private string _acousticResourceName = "unity-acoustic-resource";
        private string _createdAcousticModelName = "unity-example-acoustic-model";
        private byte[] _acousticResourceData;
        private string _acousticResourceMimeType;
        private string _grammarFilePath;

        private bool _autoGetModelsTested = false;
        private bool _recognizeTested = false;
        private bool _getModelsTested = false;
        private bool _getModelTested = false;
        private bool _getCustomizationsTested = false;
        private bool _createCustomizationsTested = false;
        private bool _getCustomizationTested = false;
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
        private bool _getAcousticCustomizationTested = false;
        private bool _trainAcousticCustomizationsTested = false;
        private bool _resetAcousticCustomizationsTested = false;
        private bool _getAcousticResourcesTested = false;
        private bool _getAcousticResourceTested = false;
        private bool _addAcousticResourcesTested = false;
        private bool _isCustomizationReady = false;
        private bool _isAcousticCustomizationReady = false;
        private bool _readyToContinue = false;
        private bool _deleteAcousticCustomizationsTested = false;
        private bool _deleteCustomizationsTested = false;
        private bool _deleteAcousticResource = false;
        private float _delayTimeInSeconds = 10f;

        private bool _listGrammarsTested = false;
        private bool _addGrammarTested = false;
        private bool _getGrammarTested = false;
        private bool _deleteGrammarTested = false;
        private string _createdGrammarId;
        private string _grammarFileContentType = "application/srgs";
        private string _grammarName = "unity-integration-test-grammar";

        public override IEnumerator RunTest()
        {
            LogSystem.InstallDefaultReactors();

            //  Test SpeechToText using loaded credentials
            SpeechToText autoSpeechToText = new SpeechToText();
            while (!autoSpeechToText.Credentials.HasIamTokenData())
                yield return null;
            autoSpeechToText.GetModels(OnAutoGetModels, OnFail);
            while (!_autoGetModelsTested)
                yield return null;

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
            Credential credential = vcapCredentials.GetCredentialByname("speech-to-text-sdk")[0].Credentials;
            //  Create credential and instantiate service
            TokenOptions tokenOptions = new TokenOptions()
            {
                IamApiKey = credential.IamApikey,
            };

            //  Create credential and instantiate service
            Credentials credentials = new Credentials(tokenOptions, credential.Url);

            //  Wait for tokendata
            while (!credentials.HasIamTokenData())
                yield return null;

            _speechToText = new SpeechToText(credentials);
            _customCorpusFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/speech-to-text/theJabberwocky-utf8.txt";
            _customWordsFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/speech-to-text/test-stt-words.json";
            _grammarFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/speech-to-text/confirm.abnf";
            _acousticResourceMimeType = Utility.GetMimeType(Path.GetExtension(_acousticResourceUrl));

            Runnable.Run(DownloadAcousticResource());
            while (!_isAudioLoaded)
                yield return null;

            //  Recognize
            Log.Debug("TestSpeechToText.Examples()", "Attempting to recognize");
            List<string> keywords = new List<string>();
            keywords.Add("speech");
            _speechToText.KeywordsThreshold = 0.5f;
            _speechToText.InactivityTimeout = 120;
            _speechToText.StreamMultipart = false;
            _speechToText.Keywords = keywords.ToArray();
            _speechToText.Recognize(HandleOnRecognize, OnFail, _acousticResourceData, _acousticResourceMimeType);
            while (!_recognizeTested)
                yield return null;

            //  Get models
            Log.Debug("TestSpeechToText.Examples()", "Attempting to get models");
            _speechToText.GetModels(HandleGetModels, OnFail);
            while (!_getModelsTested)
                yield return null;

            //  Get model
            Log.Debug("TestSpeechToText.Examples()", "Attempting to get model {0}", _modelNameToGet);
            _speechToText.GetModel(HandleGetModel, OnFail, _modelNameToGet);
            while (!_getModelTested)
                yield return null;

            //  Get customizations
            Log.Debug("TestSpeechToText.Examples()", "Attempting to get customizations");
            _speechToText.GetCustomizations(HandleGetCustomizations, OnFail);
            while (!_getCustomizationsTested)
                yield return null;

            //  Create customization
            Log.Debug("TestSpeechToText.Examples()", "Attempting create customization");
            _speechToText.CreateCustomization(HandleCreateCustomization, OnFail, "unity-test-customization", "en-US_BroadbandModel", "Testing customization unity");
            while (!_createCustomizationsTested)
                yield return null;

            //  Get customization
            Log.Debug("TestSpeechToText.Examples()", "Attempting to get customization {0}", _createdCustomizationID);
            _speechToText.GetCustomization(HandleGetCustomization, OnFail, _createdCustomizationID);
            while (!_getCustomizationTested)
                yield return null;

            //  Get custom corpora
            Log.Debug("TestSpeechToText.Examples()", "Attempting to get custom corpora for {0}", _createdCustomizationID);
            _speechToText.GetCustomCorpora(HandleGetCustomCorpora, OnFail, _createdCustomizationID);
            while (!_getCustomCorporaTested)
                yield return null;

            //  Add custom corpus
            Log.Debug("TestSpeechToText.Examples()", "Attempting to add custom corpus {1} in customization {0}", _createdCustomizationID, _createdCorpusName);
            string corpusData = File.ReadAllText(_customCorpusFilePath);
            _speechToText.AddCustomCorpus(HandleAddCustomCorpus, OnFail, _createdCustomizationID, _createdCorpusName, true, corpusData);
            while (!_addCustomCorpusTested)
                yield return null;

            //  Get custom corpus
            Log.Debug("TestSpeechToText.Examples()", "Attempting to get custom corpus {1} in customization {0}", _createdCustomizationID, _createdCorpusName);
            _speechToText.GetCustomCorpus(HandleGetCustomCorpus, OnFail, _createdCustomizationID, _createdCorpusName);
            while (!_getCustomCorpusTested)
                yield return null;

            //  Wait for customization
            Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
            while (!_isCustomizationReady)
                yield return null;

            //  Get custom words
            Log.Debug("TestSpeechToText.Examples()", "Attempting to get custom words.");
            _speechToText.GetCustomWords(HandleGetCustomWords, OnFail, _createdCustomizationID);
            while (!_getCustomWordsTested)
                yield return null;

            //  Add custom words from path
            Log.Debug("TestSpeechToText.Examples()", "Attempting to add custom words in customization {0} using Words json path {1}", _createdCustomizationID, _customWordsFilePath);
            string customWords = File.ReadAllText(_customWordsFilePath);
            _speechToText.AddCustomWords(HandleAddCustomWordsFromPath, OnFail, _createdCustomizationID, customWords);
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

            Log.Debug("TestSpeechToText.Examples()", "Attempting to add custom words in customization {0} using Words object", _createdCustomizationID);
            _speechToText.AddCustomWords(HandleAddCustomWordsFromObject, OnFail, _createdCustomizationID, words);
            while (!_addCustomWordsFromObjectTested)
                yield return null;

            //  Wait for customization
            _isCustomizationReady = false;
            Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
            while (!_isCustomizationReady)
                yield return null;

            //  Get custom word
            Log.Debug("TestSpeechToText.Examples()", "Attempting to get custom word {1} in customization {0}", _createdCustomizationID, words.words[0].word);
            _speechToText.GetCustomWord(HandleGetCustomWord, OnFail, _createdCustomizationID, words.words[0].word);
            while (!_getCustomWordTested)
                yield return null;

            //  Train customization
            Log.Debug("TestSpeechToText.Examples()", "Attempting to train customization {0}", _createdCustomizationID);
            _speechToText.TrainCustomization(HandleTrainCustomization, OnFail, _createdCustomizationID);
            while (!_trainCustomizationTested)
                yield return null;

            //  Wait for customization
            _isCustomizationReady = false;
            Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
            while (!_isCustomizationReady)
                yield return null;

            //  Delete custom word
            Log.Debug("TestSpeechToText.Examples()", "Attempting to delete custom word {1} in customization {0}", _createdCustomizationID, words.words[2].word);
            _speechToText.DeleteCustomWord(HandleDeleteCustomWord, OnFail, _createdCustomizationID, words.words[2].word);
            while (!_deleteCustomWordTested)
                yield return null;

            //  Delay
            Log.Debug("TestSpeechToText.Examples()", string.Format("Delaying delete environment for {0} sec", _delayTimeInSeconds));
            Runnable.Run(Delay(_delayTimeInSeconds));
            while (!_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Delete custom corpus
            Log.Debug("TestSpeechToText.Examples()", "Attempting to delete custom corpus {1} in customization {0}", _createdCustomizationID, _createdCorpusName);
            _speechToText.DeleteCustomCorpus(HandleDeleteCustomCorpus, OnFail, _createdCustomizationID, _createdCorpusName);
            while (!_deleteCustomCorpusTested)
                yield return null;

            //  Delay
            Log.Debug("TestSpeechToText.Examples()", string.Format("Delaying delete environment for {0} sec", _delayTimeInSeconds));
            Runnable.Run(Delay(_delayTimeInSeconds));
            while (!_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Reset customization
            Log.Debug("TestSpeechToText.Examples()", "Attempting to reset customization {0}", _createdCustomizationID);
            _speechToText.ResetCustomization(HandleResetCustomization, OnFail, _createdCustomizationID);
            while (!_resetCustomizationTested)
                yield return null;

            //  Delay
            Log.Debug("TestSpeechToText.Examples()", string.Format("Delaying delete environment for {0} sec", _delayTimeInSeconds));
            Runnable.Run(Delay(_delayTimeInSeconds));
            while (!_readyToContinue)
                yield return null;

            //  List Grammars
            Log.Debug("TestSpeechToText.Examples()", "Attempting to list grammars {0}", _createdCustomizationID);
            _speechToText.ListGrammars(OnListGrammars, OnFail, _createdCustomizationID);
            while (!_listGrammarsTested)
                yield return null;

            //  Add Grammar
            Log.Debug("TestSpeechToText.Examples()", "Attempting to add grammar {0}", _createdCustomizationID);
            string grammarFile = File.ReadAllText(_grammarFilePath);
            _speechToText.AddGrammar(OnAddGrammar, OnFail, _createdCustomizationID, _grammarName, grammarFile, _grammarFileContentType);
            while (!_addGrammarTested)
                yield return null;

            //  Get Grammar
            Log.Debug("TestSpeechToText.Examples()", "Attempting to get grammar {0}", _createdCustomizationID);
            _speechToText.GetGrammar(OnGetGrammar, OnFail, _createdCustomizationID, _grammarName);
            while (!_getGrammarTested)
                yield return null;

            //  Wait for customization
            _isCustomizationReady = false;
            Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
            while (!_isCustomizationReady)
                yield return null;

            //  Delete Grammar
            Log.Debug("TestSpeechToText.Examples()", "Attempting to delete grammar {0}", _createdCustomizationID);
            _speechToText.DeleteGrammar(OnDeleteGrammar, OnFail, _createdCustomizationID, _grammarName);
            while (!_deleteGrammarTested)
                yield return null;

            _readyToContinue = false;
            //  Delete customization
            Log.Debug("TestSpeechToText.Examples()", "Attempting to delete customization {0}", _createdCustomizationID);
            _speechToText.DeleteCustomization(HandleDeleteCustomization, OnFail, _createdCustomizationID);
            while (!_deleteCustomizationsTested)
                yield return null;

            //  List acoustic customizations
            Log.Debug("TestSpeechToText.Examples()", "Attempting to get acoustic customizations");
            _speechToText.GetCustomAcousticModels(HandleGetCustomAcousticModels, OnFail);
            while (!_getAcousticCustomizationsTested)
                yield return null;

            //  Create acoustic customization
            Log.Debug("TestSpeechToText.Examples()", "Attempting to create acoustic customization");
            _speechToText.CreateAcousticCustomization(HandleCreateAcousticCustomization, OnFail, _createdAcousticModelName);
            while (!_createAcousticCustomizationsTested)
                yield return null;

            //  Get acoustic customization
            Log.Debug("TestSpeechToText.Examples()", "Attempting to get acoustic customization {0}", _createdAcousticModelId);
            _speechToText.GetCustomAcousticModel(HandleGetCustomAcousticModel, OnFail, _createdAcousticModelId);
            while (!_getAcousticCustomizationTested)
                yield return null;

            while (!_isAudioLoaded)
                yield return null;

            //  Create acoustic resource
            Log.Debug("TestSpeechToText.Examples()", "Attempting to create audio resource {1} on {0}", _createdAcousticModelId, _acousticResourceName);
            string mimeType = Utility.GetMimeType(Path.GetExtension(_acousticResourceUrl));
            _speechToText.AddAcousticResource(HandleAddAcousticResource, OnFail, _createdAcousticModelId, _acousticResourceName, mimeType, mimeType, true, _acousticResourceData);
            while (!_addAcousticResourcesTested)
                yield return null;

            //  Wait for customization
            _isAcousticCustomizationReady = false;
            Runnable.Run(CheckAcousticCustomizationStatus(_createdAcousticModelId));
            while (!_isAcousticCustomizationReady)
                yield return null;

            //  List acoustic resources
            Log.Debug("TestSpeechToText.Examples()", "Attempting to get audio resources {0}", _createdAcousticModelId);
            _speechToText.GetCustomAcousticResources(HandleGetCustomAcousticResources, OnFail, _createdAcousticModelId);
            while (!_getAcousticResourcesTested)
                yield return null;

            //  Train acoustic customization
            Log.Debug("TestSpeechToText.Examples()", "Attempting to train acoustic customization {0}", _createdAcousticModelId);
            _speechToText.TrainAcousticCustomization(HandleTrainAcousticCustomization, OnFail, _createdAcousticModelId, null, true);
            while (!_trainAcousticCustomizationsTested)
                yield return null;

            //  Get acoustic resource
            Log.Debug("TestSpeechToText.Examples()", "Attempting to get audio resource {1} from {0}", _createdAcousticModelId, _acousticResourceName);
            _speechToText.GetCustomAcousticResource(HandleGetCustomAcousticResource, OnFail, _createdAcousticModelId, _acousticResourceName);
            while (!_getAcousticResourceTested)
                yield return null;

            //  Wait for customization
            _isAcousticCustomizationReady = false;
            Runnable.Run(CheckAcousticCustomizationStatus(_createdAcousticModelId));
            while (!_isAcousticCustomizationReady)
                yield return null;

            //  Delete acoustic resource
            DeleteAcousticResource();
            while (!_deleteAcousticResource)
                yield return null;

            //  Delay
            Log.Debug("TestSpeechToText.Examples()", string.Format("Delaying delete acoustic resource for {0} sec", _delayTimeInSeconds));
            Runnable.Run(Delay(_delayTimeInSeconds));
            while (!_readyToContinue)
                yield return null;

            //  Reset acoustic customization
            Log.Debug("TestSpeechToText.Examples()", "Attempting to reset acoustic customization {0}", _createdAcousticModelId);
            _speechToText.ResetAcousticCustomization(HandleResetAcousticCustomization, OnFail, _createdAcousticModelId);
            while (!_resetAcousticCustomizationsTested)
                yield return null;

            //  Delay
            Log.Debug("TestSpeechToText.Examples()", string.Format("Delaying delete acoustic customization for {0} sec", _delayTimeInSeconds));
            Runnable.Run(Delay(_delayTimeInSeconds));
            while (!_readyToContinue)
                yield return null;

            //  Delete acoustic customization
            DeleteAcousticCustomization();
            while (!_deleteAcousticCustomizationsTested)
                yield return null;

            //  Delay
            Log.Debug("TestSpeechToText.Examples()", string.Format("Delaying complete for {0} sec", _delayTimeInSeconds));
            Runnable.Run(Delay(_delayTimeInSeconds));
            while (!_readyToContinue)
                yield return null;

            Log.Debug("TestSpeechToText.RunTest()", "Speech to Text examples complete.");

            yield break;
        }

        private void OnAutoGetModels(ModelSet response, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.OnAutoGetModels()", "{0}", customData["json"].ToString());
            Test(response.models != null);
            _autoGetModelsTested = true;
        }

        private void DeleteAcousticResource()
        {
            Log.Debug("TestSpeechToText.DeleteAcousticResource()", "Attempting to delete audio resource {1} from {0}", _createdAcousticModelId, _acousticResourceName);
            _speechToText.DeleteAcousticResource(HandleDeleteAcousticResource, OnFail, _createdAcousticModelId, _acousticResourceName);
        }

        private void DeleteAcousticCustomization()
        {
            Log.Debug("TestSpeechToText.DeleteAcousticCustomization()", "Attempting to delete acoustic customization {0}", _createdAcousticModelId);
            _speechToText.DeleteAcousticCustomization(HandleDeleteAcousticCustomization, OnFail, _createdAcousticModelId);
        }

        private void HandleGetModels(ModelSet result, Dictionary<string, object> customData)
        {

            Log.Debug("TestSpeechToText.HandleGetModels()", "{0}", customData["json"].ToString());
            _modelNameToGet = (result.models[UnityEngine.Random.Range(0, result.models.Length - 1)] as Model).name;
            Test(result != null);
            _getModelsTested = true;
        }

        private void HandleGetModel(Model model, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleGetModel()", "{0}", customData["json"].ToString());
            Test(model != null);
            _getModelTested = true;
        }

        private void HandleOnRecognize(SpeechRecognitionEvent result, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleOnRecognize()", "{0}", customData["json"].ToString());
            Test(result != null);
            _recognizeTested = true;
        }

        private void HandleGetCustomizations(Customizations customizations, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomizations()", "{0}", customData["json"].ToString());
            Test(customizations != null);
            _getCustomizationsTested = true;
        }

        private void HandleCreateCustomization(CustomizationID customizationID, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleCreateCustomization()", "{0}", customData["json"].ToString());
            _createdCustomizationID = customizationID.customization_id;
            Test(customizationID != null);
            _createCustomizationsTested = true;
        }

        private void HandleGetCustomization(Customization customization, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomization()", "{0}", customData["json"].ToString());
            Test(customization != null);
            _getCustomizationTested = true;
        }

        private void HandleDeleteCustomization(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleDeleteCustomization()", "{0}", customData["json"].ToString());
            _createdCustomizationID = default(string);
            Test(success);
            _deleteCustomizationsTested = true;
        }

        private void HandleTrainCustomization(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleTrainCustomization()", "{0}", customData["json"].ToString());
            Test(success);
            _trainCustomizationTested = true;
        }

        //private void HandleUpgradeCustomization(bool success, Dictionary<string, object> customData)
        //{
        //     Test(success);
        //    _upgradeCustomizationTested = true;
        //}

        private void HandleResetCustomization(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleResetCustomization()", "{0}", customData["json"].ToString());
            Test(success);
            _resetCustomizationTested = true;
        }

        private void HandleGetCustomCorpora(Corpora corpora, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomCorpora()", "{0}", customData["json"].ToString());
            Test(corpora != null);
            _getCustomCorporaTested = true;
        }

        private void HandleDeleteCustomCorpus(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleDeleteCustomCorpus()", "{0}", customData["json"].ToString());
            Test(success);
            _deleteCustomCorpusTested = true;
        }

        private void HandleAddCustomCorpus(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleAddCustomCorpus()", "{0}", customData["json"].ToString());
            Test(success);
            _addCustomCorpusTested = true;
        }

        private void HandleGetCustomCorpus(Corpus corpus, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomCorpus()", "{0}", customData["json"].ToString());
            Test(corpus != null);
            _getCustomCorpusTested = true;
        }

        private void HandleGetCustomWords(WordsList wordList, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomWords()", "{0}", customData["json"].ToString());
            Test(wordList != null);
            _getCustomWordsTested = true;
        }

        private void HandleAddCustomWordsFromPath(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleAddCustomWordsFromPath()", "{0}", customData["json"].ToString());
            Test(success);
            _addCustomWordsFromPathTested = true;
        }

        private void HandleAddCustomWordsFromObject(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleAddCustomWordsFromObject()", "{0}", customData["json"].ToString());
            Test(success);
            _addCustomWordsFromObjectTested = true;
        }

        private void HandleDeleteCustomWord(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleDeleteCustomWord()", "{0}", customData["json"].ToString());
            Test(success);
            _deleteCustomWordTested = true;
        }

        private void HandleGetCustomWord(WordData word, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomWord()", "{0}", customData["json"].ToString());
            Test(word != null);
            _getCustomWordTested = true;
        }

        private void HandleGetCustomAcousticModels(AcousticCustomizations acousticCustomizations, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomAcousticModels()", "{0}", customData["json"].ToString());
            Test(acousticCustomizations != null);
            _getAcousticCustomizationsTested = true;
        }

        private void HandleCreateAcousticCustomization(CustomizationID customizationID, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleCreateAcousticCustomization()", "{0}", customData["json"].ToString());
            _createdAcousticModelId = customizationID.customization_id;
            Test(customizationID != null);
            _createAcousticCustomizationsTested = true;
        }

        private void HandleGetCustomAcousticModel(AcousticCustomization acousticCustomization, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomAcousticModel()", "{0}", customData["json"].ToString());
            Test(acousticCustomization != null);
            _getAcousticCustomizationTested = true;
        }

        private void HandleTrainAcousticCustomization(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleTrainAcousticCustomization()", "{0}", customData["json"].ToString());
            Test(success);
            _trainAcousticCustomizationsTested = true;
        }

        private void HandleGetCustomAcousticResources(AudioResources audioResources, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomAcousticResources()", "{0}", customData["json"].ToString());
            Test(audioResources != null);
            _getAcousticResourcesTested = true;
        }

        private void HandleAddAcousticResource(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleAddAcousticResource()", "{0}", customData["json"].ToString());
            Test(success);
            _addAcousticResourcesTested = true;
        }

        private void HandleGetCustomAcousticResource(AudioListing audioListing, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomAcousticResource()", "{0}", customData["json"].ToString());
            Test(audioListing != null);
            _getAcousticResourceTested = true;
        }

        private void HandleResetAcousticCustomization(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleResetAcousticCustomization()", "{0}", customData["json"].ToString());
            Test(success);
            _resetAcousticCustomizationsTested = true;
        }

        private void HandleDeleteAcousticResource(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleDeleteAcousticResource()", "{0}", customData["json"].ToString());
            Test(success);
            _deleteAcousticResource = true;
        }

        private void HandleDeleteAcousticCustomization(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.HandleDeleteAcousticCustomization()", "{0}", customData["json"].ToString());
            Test(success);
            _deleteAcousticCustomizationsTested = true;
        }

        private void OnListGrammars(Grammars response, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.OnListGrammars()", "{0}", customData["json"].ToString());
            Test(response != null);
            Test(response._Grammars != null);
            _listGrammarsTested = true;
        }

        private void OnAddGrammar(object response, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.OnAddGrammar()", "Success!");
            Test(response != null);
            _addGrammarTested = true;
        }

        private void OnGetGrammar(Grammar response, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.OnGetGrammar()", "{0}", customData["json"].ToString());
            Test(response != null);
            Test(response.Name == _grammarName);
            _getGrammarTested = true;
        }

        private void OnDeleteGrammar(object response, Dictionary<string, object> customData)
        {
            Log.Debug("TestSpeechToText.OnDeleteGrammar()", "Success!");
            Test(response != null);
            _deleteGrammarTested = true;
        }

        private IEnumerator CheckCustomizationStatus(string customizationID, float delay = 0.1f)
        {
            Log.Debug("TestSpeechToText.CheckCustomizationStatus()", "Checking customization status in {0} seconds...", delay.ToString());
            yield return new WaitForSeconds(delay);

            //  passing customizationID in custom data
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["customizationID"] = customizationID;
            _speechToText.GetCustomization(OnCheckCustomizationStatus, OnFail, customizationID, customData);
        }

        private void OnCheckCustomizationStatus(Customization customization, Dictionary<string, object> customData)
        {
            if (customization != null)
            {
                Log.Debug("TestSpeechToText.OnCheckCustomizationStatus()", "Customization status: {0}", customization.status);
                if (customization.status != "ready" && customization.status != "available")
                    Runnable.Run(CheckCustomizationStatus(customData["customizationID"].ToString(), 5f));
                else
                    _isCustomizationReady = true;
            }
            else
            {
                Log.Debug("TestSpeechToText.OnCheckCustomizationStatus()", "Check customization status failed!");
            }
        }

        private IEnumerator CheckAcousticCustomizationStatus(string customizationID, float delay = 0.1f)
        {
            Log.Debug("TestSpeechToText.CheckAcousticCustomizationStatus()", "Checking acoustic customization status in {0} seconds...", delay.ToString());
            yield return new WaitForSeconds(delay);

            //	passing customizationID in custom data
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["customizationID"] = customizationID;
            _speechToText.GetCustomAcousticModel(OnCheckAcousticCustomizationStatus, OnFail, customizationID, customData);
        }

        private void OnCheckAcousticCustomizationStatus(AcousticCustomization acousticCustomization, Dictionary<string, object> customData)
        {
            if (acousticCustomization != null)
            {
                Log.Debug("TestSpeechToText.CheckAcousticCustomizationStatus()", "Acoustic customization status: {0}", acousticCustomization.status);
                if (acousticCustomization.status != "ready" && acousticCustomization.status != "available")
                    Runnable.Run(CheckAcousticCustomizationStatus(customData["customizationID"].ToString(), 5f));
                else
                    _isAcousticCustomizationReady = true;
            }
            else
            {
                Log.Debug("TestSpeechToText.CheckAcousticCustomizationStatus()", "Check acoustic customization status failed!");
            }
        }

        private IEnumerator Delay(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            _readyToContinue = true;
        }

        private IEnumerator DownloadAcousticResource()
        {
            Log.Debug("TestSpeechToText.DownloadAcousticResource()", "downloading acoustic resource from {0}", _acousticResourceUrl);
            using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(_acousticResourceUrl))
            {
                yield return unityWebRequest.SendWebRequest();

                Log.Debug("TestSpeechToText.DownloadAcousticResource()", "acoustic resource downloaded");
                _acousticResourceData = unityWebRequest.downloadHandler.data;
                _isAudioLoaded = true;
            }
        }

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("TestSpeechToText.OnFail()", "Error received: {0}", error.ToString());
        }
    }
}
