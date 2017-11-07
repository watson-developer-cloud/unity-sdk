﻿/**
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
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Connection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestSpeechToText : UnitTest
    {
        private string _username = null;
        private string _password = null;
        private fsSerializer _serializer = new fsSerializer();
        //private string _token = "<authentication-token>";

        private AudioClip _audioClip;
        private SpeechToText _speechToText;

        private string _modelNameToGet;
        private string _createdCustomizationID;
        private string _createdCorpusName = "the-jabberwocky-corpus";
        private string _customCorpusFilePath;
        private string _customWordsFilePath;
        private string _wavFilePath;
        private string _acousticResourceUrl = "https://ia802302.us.archive.org/10/items/Greatest_Speeches_of_the_20th_Century/TheFirstAmericaninEarthOrbit.mp3";
        private bool _isAudioLoaded = false;
        private string _createdAcousticModelId;
        private string _acousticResourceName = "unity-acoustic-resource";
        private string _createdAcousticModelName = "unity-example-acoustic-model";
        private byte[] _acousticResourceData;

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
        private float _delayTimeInSeconds = 10f;

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
                Credential credential = vcapCredentials.VCAP_SERVICES["speech_to_text"][TestCredentialIndex].Credentials;
                _username = credential.Username.ToString();
                _password = credential.Password.ToString();
                _url = credential.Url.ToString();
            }
            catch
            {
                Log.Debug("TestSpeechToText.RunTest()", "Failed to get credentials from VCAP_SERVICES file. Please configure credentials to run this test. For more information, see: https://github.com/watson-developer-cloud/unity-sdk/#authentication");
            }

            //  Create credential and instantiate service
            Credentials credentials = new Credentials(_username, _password, _url);

            //  Or authenticate using token
            //Credentials credentials = new Credentials(_url)
            //{
            //    AuthenticationToken = _token
            //};

            _speechToText = new SpeechToText(credentials);
            _customCorpusFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/theJabberwocky-utf8.txt";
            _customWordsFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/test-stt-words.json";
            _wavFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/test-audio.wav";
            _audioClip = WaveFile.ParseWAV("testClip", File.ReadAllBytes(_wavFilePath));

            Runnable.Run(DownloadAcousticResource());

            Log.Debug("TestSpeechToText.RunTest()", "Attempting to recognize");
            _speechToText.Recognize(_audioClip, HandleOnRecognize);
            while (!_recognizeTested)
                yield return null;

            //  Get models
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to get models");
            _speechToText.GetModels(HandleGetModels);
            while (!_getModelsTested)
                yield return null;

            //  Get model
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to get model {0}", _modelNameToGet);
            _speechToText.GetModel(HandleGetModel, _modelNameToGet);
            while (!_getModelTested)
                yield return null;

            //  Get customizations
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to get customizations");
            _speechToText.GetCustomizations(HandleGetCustomizations);
            while (!_getCustomizationsTested)
                yield return null;

            //  Create customization
            Log.Debug("TestSpeechToText.RunTest()", "Attempting create customization");
            _speechToText.CreateCustomization(HandleCreateCustomization, "unity-test-customization", "en-US_BroadbandModel", "Testing customization unity");
            while (!_createCustomizationsTested)
                yield return null;

            //  Get customization
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to get customization {0}", _createdCustomizationID);
            _speechToText.GetCustomization(HandleGetCustomization, _createdCustomizationID);
            while (!_getCustomizationTested)
                yield return null;

            //  Get custom corpora
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to get custom corpora for {0}", _createdCustomizationID);
            _speechToText.GetCustomCorpora(HandleGetCustomCorpora, _createdCustomizationID);
            while (!_getCustomCorporaTested)
                yield return null;

            //  Add custom corpus
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to add custom corpus {1} in customization {0}", _createdCustomizationID, _createdCorpusName);
            string corpusData = File.ReadAllText(_customCorpusFilePath);
            _speechToText.AddCustomCorpus(HandleAddCustomCorpus, _createdCustomizationID, _createdCorpusName, true, corpusData);
            while (!_addCustomCorpusTested)
                yield return null;

            //  Get custom corpus
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to get custom corpus {1} in customization {0}", _createdCustomizationID, _createdCorpusName);
            _speechToText.GetCustomCorpus(HandleGetCustomCorpus, _createdCustomizationID, _createdCorpusName);
            while (!_getCustomCorpusTested)
                yield return null;

            //  Wait for customization
            Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
            while (!_isCustomizationReady)
                yield return null;

            //  Get custom words
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to get custom words.");
            _speechToText.GetCustomWords(HandleGetCustomWords, _createdCustomizationID);
            while (!_getCustomWordsTested)
                yield return null;

            //  Add custom words from path
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to add custom words in customization {0} using Words json path {1}", _createdCustomizationID, _customWordsFilePath);
            string customWords = File.ReadAllText(_customWordsFilePath);
            _speechToText.AddCustomWords(HandleAddCustomWordsFromPath, _createdCustomizationID, customWords);
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

            Log.Debug("TestSpeechToText.RunTest()", "Attempting to add custom words in customization {0} using Words object", _createdCustomizationID);
            _speechToText.AddCustomWords(HandleAddCustomWordsFromObject, _createdCustomizationID, words);
            while (!_addCustomWordsFromObjectTested)
                yield return null;

            //  Wait for customization
            _isCustomizationReady = false;
            Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
            while (!_isCustomizationReady)
                yield return null;

            //  Get custom word
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to get custom word {1} in customization {0}", _createdCustomizationID, words.words[0].word);
            _speechToText.GetCustomWord(HandleGetCustomWord, _createdCustomizationID, words.words[0].word);
            while (!_getCustomWordTested)
                yield return null;

            //  Train customization
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to train customization {0}", _createdCustomizationID);
            _speechToText.TrainCustomization(HandleTrainCustomization, _createdCustomizationID);
            while (!_trainCustomizationTested)
                yield return null;

            //  Wait for customization
            _isCustomizationReady = false;
            Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
            while (!_isCustomizationReady)
                yield return null;

            //  Upgrade customization - not currently implemented in service
            //Log.Debug("TestSpeechToText.RunTest()", "Attempting to upgrade customization {0}", _createdCustomizationID);
            //_speechToText.UpgradeCustomization(HandleUpgradeCustomization, _createdCustomizationID);
            //while (!_upgradeCustomizationTested)
            //    yield return null;

            //  Delete custom word
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to delete custom word {1} in customization {0}", _createdCustomizationID, words.words[2].word);
            _speechToText.DeleteCustomWord(HandleDeleteCustomWord, _createdCustomizationID, words.words[2].word);
            while (!_deleteCustomWordTested)
                yield return null;

            //  Delay
            Log.Debug("TestSpeechToText.RunTest()", string.Format("Delaying delete environment for {0} sec", _delayTimeInSeconds));
            Runnable.Run(Delay(_delayTimeInSeconds));
            while (!_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Delete custom corpus
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to delete custom corpus {1} in customization {0}", _createdCustomizationID, _createdCorpusName);
            _speechToText.DeleteCustomCorpus(HandleDeleteCustomCorpus, _createdCustomizationID, _createdCorpusName);
            while (!_deleteCustomCorpusTested)
                yield return null;

            //  Delay
            Log.Debug("TestSpeechToText.RunTest()", string.Format("Delaying delete environment for {0} sec", _delayTimeInSeconds));
            Runnable.Run(Delay(_delayTimeInSeconds));
            while (!_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Reset customization
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to reset customization {0}", _createdCustomizationID);
            _speechToText.ResetCustomization(HandleResetCustomization, _createdCustomizationID);
            while (!_resetCustomizationTested)
                yield return null;
            
            //  Delay
            Log.Debug("TestSpeechToText.RunTest()", string.Format("Delaying continue for {0} sec", _delayTimeInSeconds));
            Runnable.Run(Delay(_delayTimeInSeconds));
            while (!_readyToContinue)
                yield return null;

            _readyToContinue = false;

            //  List acoustic customizations
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to get acoustic customizations");
            _speechToText.GetCustomAcousticModels(HandleGetCustomAcousticModels);
            while (!_getAcousticCustomizationsTested)
                yield return null;

            //  Create acoustic customization
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to create acoustic customization");
            _speechToText.CreateAcousticCustomization(HandleCreateAcousticCustomization, _createdAcousticModelName);
            while (!_createAcousticCustomizationsTested)
                yield return null;

            //  Get acoustic customization
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to get acoustic customization {0}", _createdAcousticModelId);
            _speechToText.GetCustomAcousticModel(HandleGetCustomAcousticModel, _createdAcousticModelId);
            while (!_getAcousticCustomizationTested)
                yield return null;

            while (!_isAudioLoaded)
                yield return null;

            //  Create acoustic resource
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to create audio resource {1} on {0}", _createdAcousticModelId, _acousticResourceName);
            string mimeType = Utility.GetMimeType(Path.GetExtension(_acousticResourceUrl));
            _speechToText.AddAcousticResource(HandleAddAcousticResource, _createdAcousticModelId, _acousticResourceName, mimeType, mimeType, true, _acousticResourceData);
            while (!_addAcousticResourcesTested)
                yield return null;

            //  Wait for customization
            _isAcousticCustomizationReady = false;
            Runnable.Run(CheckAcousticCustomizationStatus(_createdAcousticModelId));
            while (!_isAcousticCustomizationReady)
                yield return null;

            //  List acoustic resources
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to get audio resources {0}", _createdAcousticModelId);
            _speechToText.GetCustomAcousticResources(HandleGetCustomAcousticResources, _createdAcousticModelId);
            while (!_getAcousticResourcesTested)
                yield return null;

            //  Train acoustic customization
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to train acoustic customization {0}", _createdAcousticModelId);
            _speechToText.TrainAcousticCustomization(HandleTrainAcousticCustomization, _createdAcousticModelId, null, true);
            while (!_trainAcousticCustomizationsTested)
                yield return null;

            //  Get acoustic resource
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to get audio resource {1} from {0}", _createdAcousticModelId, _acousticResourceName);
            _speechToText.GetCustomAcousticResource(HandleGetCustomAcousticResource, _createdAcousticModelId, _acousticResourceName);
            while (!_getAcousticResourceTested)
                yield return null;

            //  Wait for customization
            _isAcousticCustomizationReady = false;
            Runnable.Run(CheckAcousticCustomizationStatus(_createdAcousticModelId));
            while (!_isAcousticCustomizationReady)
                yield return null;

            //  Reset acoustic customization
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to reset acoustic customization {0}", _createdAcousticModelId);
            _speechToText.ResetAcousticCustomization(HandleResetAcousticCustomization, _createdAcousticModelId);
            while (!_resetAcousticCustomizationsTested)
                yield return null;

            //  Delay
            Log.Debug("TestSpeechToText.RunTest()", string.Format("Delaying delete acoustic resource for {0} sec", _delayTimeInSeconds));
            Runnable.Run(Delay(_delayTimeInSeconds));
            while (!_readyToContinue)
                yield return null;

            //  Delete acoustic resource
            DeleteAcousticResource();

            //  Delay
            Log.Debug("TestSpeechToText.RunTest()", string.Format("Delaying delete customization for {0} sec", _delayTimeInSeconds));
            Runnable.Run(Delay(_delayTimeInSeconds));
            while (!_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Delete customization
            Log.Debug("TestSpeechToText.RunTest()", "Attempting to delete customization {0}", _createdCustomizationID);
            _speechToText.DeleteCustomization(HandleDeleteCustomization, _createdCustomizationID);

            //  Delay
            Log.Debug("TestSpeechToText.RunTest()", string.Format("Delaying delete acoustic customization for {0} sec", _delayTimeInSeconds));
            Runnable.Run(Delay(_delayTimeInSeconds));
            while (!_readyToContinue)
                yield return null;
            
            //  Delete acoustic customization
            DeleteAcousticCustomization();

            //  Delay
            Log.Debug("TestSpeechToText.RunTest()", string.Format("Delaying complete for {0} sec", _delayTimeInSeconds));
            Runnable.Run(Delay(_delayTimeInSeconds));
            while (!_readyToContinue)
                yield return null;

            Log.Debug("TestSpeechToText.RunTest()", "Speech to Text examples complete.");

            yield break;
        }

        private void HandleGetModels(RESTConnector.ParsedResponse<ModelSet> resp)
        {

            Log.Debug("TestSpeechToText.HandleGetModels()", "Speech to Text - Get models response: {0}", resp.JSON);
            _modelNameToGet = (resp.DataObject.models[UnityEngine.Random.Range(0, resp.DataObject.models.Length - 1)] as Model).name;
            Test(resp.DataObject != null);
            _getModelsTested = true;
        }

        private void HandleGetModel(RESTConnector.ParsedResponse<Model> resp)
        {
            Log.Debug("TestSpeechToText.HandleGetModel()", "Speech to Text - Get model response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getModelTested = true;
        }

        private void HandleOnRecognize(SpeechRecognitionEvent result, RESTConnector.Error error)
        {
            if (result != null && result.results.Length > 0)
            {
                foreach (var res in result.results)
                {
                    foreach (var alt in res.alternatives)
                    {
                        string text = alt.transcript;
                        Log.Debug("TestSpeechToText.HandleOnRecognize()", string.Format("{0} ({1}, {2:0.00})\n", text, res.final ? "Final" : "Interim", alt.confidence));

                        if (res.final)
                            _recognizeTested = true;
                    }
                }
            }

            Test(result != null);
        }

        private void HandleGetCustomizations(RESTConnector.ParsedResponse<Customizations> resp)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomizations()", "Speech to Text - Get customizations response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getCustomizationsTested = true;
        }

        private void HandleCreateCustomization(RESTConnector.ParsedResponse<CustomizationID> resp)
        {
            Log.Debug("TestSpeechToText.HandleCreateCustomization()", "Speech to Text - Create customization response: {0}", resp.JSON);
            _createdCustomizationID = resp.DataObject.customization_id;
            Test(resp.DataObject != null);
            _createCustomizationsTested = true;
        }

        private void HandleGetCustomization(RESTConnector.ParsedResponse<Customization> resp)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomization()", "Speech to Text - Get customization response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getCustomizationTested = true;
        }

        private void HandleDeleteCustomization(RESTConnector.ParsedResponse<object> resp)
        {
            if (resp.Success)
            {
                Log.Debug("TestSpeechToText.HandleDeleteCustomization()", "Speech to Text - Get customization response: Deleted customization {0}!", _createdCustomizationID);
                _createdCustomizationID = default(string);
            }
            else
            {
                Log.Debug("TestSpeechToText.HandleDeleteCustomization()", "Failed to delete customization!");
            }
        }


        private void HandleTrainCustomization(RESTConnector.ParsedResponse<object> resp)
        {
            if (resp.Success)
            {
                Log.Debug("TestSpeechToText.HandleTrainCustomization()", "Trained customization {0}!", _createdCustomizationID);
            }
            else
            {
                Log.Debug("TestSpeechToText.HandleTrainCustomization()", "Failed to train customization!");
            }
            Test(resp.Success);

            _trainCustomizationTested = true;
        }

        //private void HandleUpgradeCustomization(RESTConnector.ParsedResponse<object> resp)
        //{
        //    if (resp.Success)
        //    {
        //        Log.Debug("TestSpeechToText.HandleUpgradeCustomization()", "Upgrade customization {0}!", _createdCustomizationID);
        //    }
        //    else
        //    {
        //        Log.Debug("TestSpeechToText.HandleUpgradeCustomization()", "Failed to upgrade customization!");
        //    }
        //Test(resp.Success);
        //    _upgradeCustomizationTested = true;
        //}

        private void HandleResetCustomization(RESTConnector.ParsedResponse<object> resp)
        {
            if (resp.Success)
            {
                Log.Debug("TestSpeechToText.HandleResetCustomization()", "Reset customization {0}!", _createdCustomizationID);
            }
            else
            {
                Log.Debug("TestSpeechToText.HandleResetCustomization()", "Failed to reset customization!");
            }
            Test(resp.Success);

            _resetCustomizationTested = true;
        }

        private void HandleGetCustomCorpora(RESTConnector.ParsedResponse<Corpora> resp)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomCorpora()", "Speech to Text - Get custom corpora response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getCustomCorporaTested = true;
        }

        private void HandleDeleteCustomCorpus(RESTConnector.ParsedResponse<object> resp)
        {
            if (resp.Success)
            {
                Log.Debug("TestSpeechToText.HandleDeleteCustomCorpus()", "Speech to Text - delete custom coprus response: succeeded!");
            }
            else
            {
                Log.Debug("TestSpeechToText.HandleDeleteCustomCorpus()", "Failed to delete custom corpus!");
            }
            Test(resp.Success);

            _deleteCustomCorpusTested = true;
        }

        private void HandleAddCustomCorpus(RESTConnector.ParsedResponse<object> resp)
        {
            if (resp.Success)
            {
                Log.Debug("TestSpeechToText.HandleAddCustomCorpus()", "Speech to Text - Add custom corpus response: succeeded!");
            }
            else
            {
                Log.Debug("TestSpeechToText.HandleAddCustomCorpus()", "Failed to add custom corpus!");
            }
            Test(resp.Success);

            _addCustomCorpusTested = true;
        }

        private void HandleGetCustomCorpus(RESTConnector.ParsedResponse<Corpus> resp)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomCorpus()", "Speech to Text - Get custom corpus response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getCustomCorpusTested = true;
        }

        private void HandleGetCustomWords(RESTConnector.ParsedResponse<WordsList> resp)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomWords()", "Speech to Text - Get custom words response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getCustomWordsTested = true;
        }

        private void HandleAddCustomWordsFromPath(RESTConnector.ParsedResponse<object> resp)
        {
            if (resp.Success)
            {
                Log.Debug("TestSpeechToText.HandleAddCustomWordsFromPath()", "Speech to Text - Add custom words from path response: succeeded!");
            }
            else
            {
                Log.Debug("TestSpeechToText.HandleAddCustomWordsFromPath()", "Failed to delete custom word!");
            }
            Test(resp.Success);

            _addCustomWordsFromPathTested = true;
        }

        private void HandleAddCustomWordsFromObject(RESTConnector.ParsedResponse<object> resp)
        {
            if (resp.Success)
            {
                Log.Debug("TestSpeechToText.HandleAddCustomWordsFromObject()", "Speech to Text - Add custom words from object response: succeeded!");
            }
            else
            {
                Log.Debug("TestSpeechToText.HandleAddCustomWordsFromObject()", "Failed to delete custom word!");
            }
            Test(resp.Success);

            _addCustomWordsFromObjectTested = true;
        }

        private void HandleDeleteCustomWord(RESTConnector.ParsedResponse<object> resp)
        {
            if (resp.Success)
            {
                Log.Debug("TestSpeechToText.HandleDeleteCustomWord()", "Speech to Text - Delete custom word response: succeeded!");
            }
            else
            {
                Log.Debug("TestSpeechToText.HandleDeleteCustomWord()", "Failed to delete custom word!");
            }
            Test(resp.Success);

            _deleteCustomWordTested = true;
        }

        private void HandleGetCustomWord(RESTConnector.ParsedResponse<WordData> resp)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomWord()", "Speech to Text - Get custom word response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getCustomWordTested = true;
        }
        
        private void HandleGetCustomAcousticModels(RESTConnector.ParsedResponse<AcousticCustomizations> resp)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomAcousticModels()", "acousticCustomizations: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getAcousticCustomizationsTested = true;
        }

        private void HandleCreateAcousticCustomization(RESTConnector.ParsedResponse<CustomizationID> resp)
        {
            Log.Debug("TestSpeechToText.HandleCreateAcousticCustomization()", "customizationId: {0}", resp.JSON);
            _createdAcousticModelId = resp.DataObject.customization_id;
            Test(!string.IsNullOrEmpty(_createdAcousticModelId));
            _createAcousticCustomizationsTested = true;
        }

        private void HandleGetCustomAcousticModel(RESTConnector.ParsedResponse<AcousticCustomization> resp)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomAcousticModel()", "acousticCustomization: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getAcousticCustomizationTested = true;
        }

        private void HandleTrainAcousticCustomization(RESTConnector.ParsedResponse<object> resp)
        {
            Log.Debug("TestSpeechToText.HandleTrainAcousticCustomization()", "train customization success: {0}", resp.Success);
            Test(resp.Success);
            _trainAcousticCustomizationsTested = true;
        }

        private void HandleGetCustomAcousticResources(RESTConnector.ParsedResponse<AudioResources> resp)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomAcousticResources()", "audioResources: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getAcousticResourcesTested = true;
        }

        private void HandleAddAcousticResource(RESTConnector.ParsedResponse<object> resp)
        {
            Log.Debug("TestSpeechToText.HandleAddAcousticResource()", "added acoustic resource: {0}", resp.JSON);
            Test(resp.Success);
            _addAcousticResourcesTested = true;
        }

        private void HandleGetCustomAcousticResource(RESTConnector.ParsedResponse<AudioListing> resp)
        {
            Log.Debug("TestSpeechToText.HandleGetCustomAcousticResource()", "audioListing: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getAcousticResourceTested = true;
        }

        private void HandleResetAcousticCustomization(RESTConnector.ParsedResponse<object> resp)
        {
            Log.Debug("TestSpeechToText.HandleResetAcousticCustomization()", "reset customization success: {0}", resp.Success);
            Test(resp.Success);
            _resetAcousticCustomizationsTested = true;
        }

        private void HandleDeleteAcousticResource(RESTConnector.ParsedResponse<object> resp)
        {
            Log.Debug("TestSpeechToText.HandleDeleteAcousticResource()", "deleted acoustic resource: {0}", resp.Success);
        }

        private void HandleDeleteAcousticCustomization(RESTConnector.ParsedResponse<object> resp)
        {
            Log.Debug("TestSpeechToText.HandleDeleteAcousticCustomization()", "deleted acoustic customization: {0}", resp.Success);
        }

        private IEnumerator CheckCustomizationStatus(string customizationID, float delay = 0.1f)
        {
            Log.Debug("TestSpeechToText.CheckCustomizationStatus()", "Checking customization status in {0} seconds...", delay.ToString());
            yield return new WaitForSeconds(delay);

            //	passing customizationID in custom data
            _speechToText.GetCustomization(OnCheckCustomizationStatus, customizationID, customizationID);
        }

        private void OnCheckCustomizationStatus(RESTConnector.ParsedResponse<Customization> resp)
        {
            if (resp.DataObject != null)
            {
                Log.Debug("TestSpeechToText.OnCheckCustomizationStatus()", "Customization status: {0}", resp.DataObject.status);
                if (resp.DataObject.status != "ready" && resp.DataObject.status != "available")
                    Runnable.Run(CheckCustomizationStatus(resp.CustomData, 5f));
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
            _speechToText.GetCustomAcousticModel(OnCheckAcousticCustomizationStatus, customizationID, customizationID);
        }

        private void OnCheckAcousticCustomizationStatus(RESTConnector.ParsedResponse<AcousticCustomization> resp)
        {
            if (resp.DataObject != null)
            {
                Log.Debug("TestSpeechToText.OnCheckAcousticCustomizationStatus()", "Acoustic customization status: {0}", resp.DataObject.status);
                if (resp.DataObject.status != "ready" && resp.DataObject.status != "available")
                    Runnable.Run(CheckAcousticCustomizationStatus(resp.CustomData, 5f));
                else
                    _isAcousticCustomizationReady = true;
            }
            else
            {
                Log.Debug("TestSpeechToText.OnCheckAcousticCustomizationStatus()", "Check acoustic customization status failed!");
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
            WWW www = new WWW(_acousticResourceUrl);
            yield return www;

            Log.Debug("TestSpeechToText.DownloadAcousticResource()", "acoustic resource downloaded");
            _acousticResourceData = www.bytes;
            _isAudioLoaded = true;
        }

        private void DeleteAcousticResource()
        {
            Log.Debug("TestSpeechToText.DeleteAcousticResource()", "Attempting to delete audio resource {1} from {0}", _createdAcousticModelId, _acousticResourceName);
            _speechToText.DeleteAcousticResource(HandleDeleteAcousticResource, _createdAcousticModelId, _acousticResourceName);
        }

        private void DeleteAcousticCustomization()
        {
            Log.Debug("TestSpeechToText.DeleteAcousticCustomization()", "Attempting to delete acoustic customization {0}", _createdAcousticModelId);
            _speechToText.DeleteAcousticCustomization(HandleDeleteAcousticCustomization, _createdAcousticModelId);
        }
    }
}
