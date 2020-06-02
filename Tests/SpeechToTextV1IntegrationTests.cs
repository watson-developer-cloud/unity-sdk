/**
* (C) Copyright IBM Corp. 2018, 2020.
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
using System.IO;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.SpeechToText.V1;
using IBM.Watson.SpeechToText.V1.Model;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

namespace IBM.Watson.Tests
{
    public class SpeechToTextV1IntegrationTests
    {
        private SpeechToTextService service;
        private string usBroadbandModel = "en-US_BroadbandModel";
        private string grammarPath;
        private string testAudioPath;
        private string corpusPath;
        private string jobId;
        private string languageModelName = "unity-sdk-language-model";
        private string languageModelDescription = "A language model created by the Unity SDK integration tests. Safe to delete.";
        private string languageModelDialect = "en-US";
        private string customizationId;
        private string corpusName = "The Jabberwocky";
        private string wordName = "unity";
        private List<string> soundsLike = new List<string>() { "unity" };
        private string displayAs = "Unity";
        private string grammarName = "unity-sdk-grammars";
        private string grammarsContentType = "application/srgs";
        private string acousticModelName = "unity-integration-test-custom-acoustic-model";
        private string acousticModelDescription = "A custom model to test Unity SDK Speech to Text acoustic customization.";
        private string acousticResourceUrl = "https://archive.org/download/Greatest_Speeches_of_the_20th_Century/KeynoteAddressforDemocraticConvention_64kb.mp3";
        private string acousticResourceName = "firstOrbit";
        private string acousticResourceMimeType = "audio/mpeg";
        private byte[] acousticResourceData;
        private string acousticModelCustomizationId;

        private bool isLanguageModelReady = false;
        private bool isCorpusReady = false;
        private bool isGrammarsReady = false;
        private bool isAcousticModelReady = false;
        private bool isAudioReady = false;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            LogSystem.InstallDefaultReactors();
            grammarPath = Application.dataPath + "/Watson/Tests/TestData/SpeechToTextV1/confirm.abnf";
            testAudioPath = Application.dataPath + "/Watson/Tests/TestData/SpeechToTextV1/test-audio.wav";
            corpusPath = Application.dataPath + "/Watson/Tests/TestData/SpeechToTextV1/theJabberwocky-utf8.txt";
            Runnable.Run(DownloadAcousticResource());
        }

        [UnitySetUp]
        public IEnumerator UnityTestSetup()
        {
            if (service == null)
            {
                service = new SpeechToTextService();
            }

            while (!service.Authenticator.CanAuthenticate())
                yield return null;
        }

        [SetUp]
        public void TestSetup()
        {
            service.WithHeader("X-Watson-Test", "1");
        }

        #region GetModel
        [UnityTest, Order(0)]
        public IEnumerator TestGetModel()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to GetModel...");
            SpeechModel getModelResponse = null;
            service.GetModel(
                callback: (DetailedResponse<SpeechModel> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "GetModel result: {0}", response.Response);
                    getModelResponse = response.Result;
                    Assert.IsNotNull(getModelResponse);
                    Assert.IsTrue(getModelResponse.Name == usBroadbandModel);
                    Assert.IsNull(error);
                },
                modelId: usBroadbandModel
            );

            while (getModelResponse == null)
                yield return null;
        }
        #endregion

        #region ListModels
        [UnityTest, Order(1)]
        public IEnumerator TestListModels()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to ListModels...");
            SpeechModels listModelsResponse = null;
            service.ListModels(
                callback: (DetailedResponse<SpeechModels> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "ListModels result: {0}", response.Response);
                    listModelsResponse = response.Result;
                    Assert.IsNotNull(listModelsResponse);
                    Assert.IsNotNull(listModelsResponse.Models);
                    Assert.IsTrue(listModelsResponse.Models.Count > 0);
                    Assert.IsNull(error);
                }
            );

            while (listModelsResponse == null)
                yield return null;
        }
        #endregion

        #region Recognize
        [UnityTest, Order(2)]
        public IEnumerator TestRecognize()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to Recognize...");
            SpeechRecognitionResults recognizeResponse = null;
            byte[] audio = File.ReadAllBytes(testAudioPath);

            service.Recognize(
                callback: (DetailedResponse<SpeechRecognitionResults> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "Recognize result: {0}", response.Response);
                    recognizeResponse = response.Result;
                    Assert.IsNotNull(recognizeResponse);
                    Assert.IsNotNull(recognizeResponse.Results);
                    Assert.IsNull(error);
                },
                audio: audio,
                model: usBroadbandModel,
                contentType: "audio/wav",
                speechDetectorSensitivity: 1,
                backgroundAudioSuppression: 0
            );

            while (recognizeResponse == null)
                yield return null;
        }
        #endregion

        #region CreateJob
        [UnityTest, Order(3)]
        public IEnumerator TestCreateJob()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to CreateJob...");
            RecognitionJob createJobResponse = null;
            byte[] audio = File.ReadAllBytes(testAudioPath);

            service.CreateJob(
                callback: (DetailedResponse<RecognitionJob> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "CreateJob result: {0}", response.Response);
                    createJobResponse = response.Result;
                    jobId = createJobResponse.Id;
                    Assert.IsNotNull(createJobResponse);
                    Assert.IsNotNull(jobId);
                    Assert.IsNotNull(createJobResponse.Status);
                    Assert.IsNull(error);
                },
                audio: audio,
                model: usBroadbandModel,
                contentType: "auido/wav"
            );

            while (createJobResponse == null)
                yield return null;
        }
        #endregion

        #region CheckJob
        [UnityTest, Order(4)]
        public IEnumerator TestCheckJob()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to CheckJob...");
            RecognitionJob checkJobResponse = null;
            service.CheckJob(
                callback: (DetailedResponse<RecognitionJob> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "CheckJob result: {0}", response.Response);
                    checkJobResponse = response.Result;
                    Assert.IsNotNull(checkJobResponse);
                    Assert.IsTrue(checkJobResponse.Id == jobId);
                    Assert.IsNotNull(checkJobResponse.Status);
                    Assert.IsNull(error);
                },
                id: jobId
            );

            while (checkJobResponse == null)
                yield return null;
        }
        #endregion

        #region CheckJobs
        [UnityTest, Order(5)]
        public IEnumerator TestCheckJobs()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to CheckJobs...");
            RecognitionJobs checkJobsResponse = null;
            service.CheckJobs(
                callback: (DetailedResponse<RecognitionJobs> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "CheckJobs result: {0}", response.Response);
                    checkJobsResponse = response.Result;
                    Assert.IsNotNull(checkJobsResponse);
                    Assert.IsNotNull(checkJobsResponse.Recognitions);
                    Assert.IsNull(error);
                }
            );

            while (checkJobsResponse == null)
                yield return null;
        }
        #endregion

        #region RegisterCallback
        //[UnityTest, Order(6)]
        //public IEnumerator TestRegisterCallback()
        //{
        //    Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to RegisterCallback...");
        //    RegisterStatus registerCallbackResponse = null;
        //    service.RegisterCallback(
        //        callback: (DetailedResponse<RegisterStatus> response, IBMError error) =>
        //        {
        //            Log.Debug("SpeechToTextServiceV1IntegrationTests", "RegisterCallback result: {0}", response.Response);
        //            registerCallbackResponse = response.Result;
        //            Assert.IsNotNull(registerCallbackResponse);
        //            Assert.IsNull(error);
        //        },
        //        callbackUrl: callbackUrl,
        //        userSecret: userSecret
        //    );

        //    while (registerCallbackResponse == null)
        //        yield return null;
        //}
        #endregion

        #region UnregisterCallback
        //[UnityTest, Order(0)]
        //public IEnumerator TestUnregisterCallback()
        //{
        //    Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to UnregisterCallback...");
        //    object unregisterCallbackResponse = null;
        //    service.UnregisterCallback(
        //        callback: (DetailedResponse<object> response, IBMError error) =>
        //        {
        //            Log.Debug("SpeechToTextServiceV1IntegrationTests", "UnregisterCallback result: {0}", response.Response);
        //            unregisterCallbackResponse = response.Result;
        //            Assert.IsNotNull(unregisterCallbackResponse);
        //            Assert.IsNull(error);
        //        },
        //        callbackUrl: callbackUrl
        //    );

        //    while (unregisterCallbackResponse == null)
        //        yield return null;
        //}
        #endregion

        #region CreateLanguageModel
        [UnityTest, Order(8)]
        public IEnumerator TestCreateLanguageModel()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to CreateLanguageModel...");
            LanguageModel createLanguageModelResponse = null;
            service.CreateLanguageModel(
                callback: (DetailedResponse<LanguageModel> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "CreateLanguageModel result: {0}", response.Response);
                    createLanguageModelResponse = response.Result;
                    customizationId = createLanguageModelResponse.CustomizationId;
                    Assert.IsNotNull(createLanguageModelResponse);
                    Assert.IsNotNull(customizationId);
                    Assert.IsNull(error);
                },
                name: languageModelName,
                baseModelName: usBroadbandModel,
                dialect: languageModelDialect,
                description: languageModelDescription
            );

            while (createLanguageModelResponse == null)
                yield return null;
        }
        #endregion

        #region GetLanguageModel
        [UnityTest, Order(9)]
        public IEnumerator TestGetLanguageModel()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to GetLanguageModel...");
            LanguageModel getLanguageModelResponse = null;
            service.GetLanguageModel(
                callback: (DetailedResponse<LanguageModel> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "GetLanguageModel result: {0}", response.Response);
                    getLanguageModelResponse = response.Result;
                    Assert.IsNotNull(getLanguageModelResponse);
                    Assert.IsTrue(getLanguageModelResponse.CustomizationId == customizationId);
                    Assert.IsTrue(getLanguageModelResponse.Name == languageModelName);
                    Assert.IsTrue(getLanguageModelResponse.Description == languageModelDescription);
                    Assert.IsTrue(getLanguageModelResponse.Dialect == languageModelDialect);
                    Assert.IsNull(error);
                },
                customizationId: customizationId
            );

            while (getLanguageModelResponse == null)
                yield return null;
        }
        #endregion

        #region ListLanguageModels
        [UnityTest, Order(10)]
        public IEnumerator TestListLanguageModels()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to ListLanguageModels...");
            LanguageModels listLanguageModelsResponse = null;
            service.ListLanguageModels(
                callback: (DetailedResponse<LanguageModels> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "ListLanguageModels result: {0}", response.Response);
                    listLanguageModelsResponse = response.Result;
                    Assert.IsNotNull(listLanguageModelsResponse);
                    Assert.IsNotNull(listLanguageModelsResponse.Customizations);
                    Assert.IsTrue(listLanguageModelsResponse.Customizations.Count > 0);
                    Assert.IsNull(error);
                },
                language: "en-US"
            );

            while (listLanguageModelsResponse == null)
                yield return null;
        }
        #endregion

        #region AddCorpus
        [UnityTest, Order(11)]
        public IEnumerator TestAddCorpus()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to AddCorpus...");
            bool isComplete = false;
            using (FileStream fs = File.OpenRead(corpusPath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    service.AddCorpus(
                        callback: (DetailedResponse<object> response, IBMError error) =>
                        {
                            Log.Debug("SpeechToTextServiceV1IntegrationTests", "AddCorpus result: {0}", response.Response);
                            Assert.IsNull(error);
                            Assert.IsTrue(response.StatusCode == 201);
                            isComplete = true;
                        },
                        customizationId: customizationId,
                        corpusName: corpusName,
                        corpusFile: ms,
                        allowOverwrite: true
                    );

                    while (!isComplete)
                        yield return null;
                }
            }
        }
        #endregion

        #region GetCorpus
        [UnityTest, Order(12)]
        public IEnumerator TestGetCorpus()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to GetCorpus...");
            Corpus getCorpusResponse = null;
            service.GetCorpus(
                callback: (DetailedResponse<Corpus> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "GetCorpus result: {0}", response.Response);
                    getCorpusResponse = response.Result;
                    Assert.IsNotNull(getCorpusResponse);
                    Assert.IsTrue(getCorpusResponse.Name == corpusName);
                    Assert.IsNotNull(getCorpusResponse.Status);
                    Assert.IsNull(error);
                },
                customizationId: customizationId,
                corpusName: corpusName
            );

            while (getCorpusResponse == null)
                yield return null;
        }
        #endregion

        #region ListCorpora
        [UnityTest, Order(13)]
        public IEnumerator TestListCorpora()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to ListCorpora...");
            Corpora listCorporaResponse = null;
            service.ListCorpora(
                callback: (DetailedResponse<Corpora> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "ListCorpora result: {0}", response.Response);
                    listCorporaResponse = response.Result;
                    Assert.IsNotNull(listCorporaResponse);
                    Assert.IsNotNull(listCorporaResponse._Corpora);
                    Assert.IsTrue(listCorporaResponse._Corpora.Count > 0);
                    Assert.IsNull(error);
                },
                customizationId: customizationId
            );

            while (listCorporaResponse == null)
                yield return null;
        }
        #endregion

        #region WaitForCorpus
        [UnityTest, Order(14)]
        [Timeout(int.MaxValue)]
        public IEnumerator WaitForCorpus()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to WaitForCorpus...");
            isCorpusReady = false;
            Runnable.Run(CheckCorpusStatus());

            while (!isCorpusReady)
                yield return null;
        }
        #endregion

        #region TrainLanguageModel
        [UnityTest, Order(15)]
        public IEnumerator TestTrainLanguageModel()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to TrainLanguageModel...");
            object trainLanguageModelResponse = null;
            service.TrainLanguageModel(
                callback: (DetailedResponse<TrainingResponse> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "TrainLanguageModel result: {0}", response.Response);
                    trainLanguageModelResponse = response.Result;
                    Assert.IsNotNull(trainLanguageModelResponse);
                    Assert.IsNull(error);
                },
                customizationId: customizationId
            );

            while (trainLanguageModelResponse == null)
                yield return null;
        }
        #endregion

        #region WaitForLanguageModel
        [UnityTest, Order(16)]
        [Timeout(int.MaxValue)]
        public IEnumerator WaitForLanguageModel()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to WaitForLanguageModel...");
            isLanguageModelReady = false;
            Runnable.Run(CheckLanguageModelStatus());

            while (!isLanguageModelReady)
                yield return null;
        }
        #endregion

        #region AddWord
        [UnityTest, Order(17)]
        public IEnumerator TestAddWord()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to AddWord...");
            bool isComplete = false;
            service.AddWord(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "AddWord result: {0}", response.Response);
                    Assert.IsTrue(response.StatusCode == 201);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                customizationId: customizationId,
                wordName: wordName,
                soundsLike: soundsLike,
                displayAs: displayAs
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region AddWords
        [UnityTest, Order(18)]
        public IEnumerator TestAddWords()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to AddWords...");
            bool isComplete = false;
            List<CustomWord> words = new List<CustomWord>()
            {
                new CustomWord()
                {
                    DisplayAs = "Watson",
                    SoundsLike = new List<string>()
                    {
                        "wat son"
                    },
                    Word = "watson"
                },
                new CustomWord()
                {
                    DisplayAs = "C#",
                    SoundsLike = new List<string>()
                    {
                        "si sharp"
                    },
                    Word = "csharp"
                },
                new CustomWord()
                {
                    DisplayAs = "SDK",
                    SoundsLike = new List<string>()
                    {
                        "S.D.K."
                    },
                    Word = "sdk"
                }
            };
            service.AddWords(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "AddWords result: {0}", response.Response);
                    Assert.IsTrue(response.StatusCode == 201);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                customizationId: customizationId,
                words: words
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region GetWord
        [UnityTest, Order(19)]
        public IEnumerator TestGetWord()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to GetWord...");
            Word getWordResponse = null;
            service.GetWord(
                callback: (DetailedResponse<Word> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "GetWord result: {0}", response.Response);
                    getWordResponse = response.Result;
                    Assert.IsNotNull(getWordResponse);
                    Assert.IsTrue(getWordResponse._Word == wordName);
                    Assert.IsNull(error);
                },
                customizationId: customizationId,
                wordName: wordName
            );

            while (getWordResponse == null)
                yield return null;
        }
        #endregion

        #region ListWords
        [UnityTest, Order(20)]
        public IEnumerator TestListWords()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to ListWords...");
            Words listWordsResponse = null;
            service.ListWords(
                callback: (DetailedResponse<Words> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "ListWords result: {0}", response.Response);
                    listWordsResponse = response.Result;
                    Assert.IsNotNull(listWordsResponse);
                    Assert.IsNotNull(listWordsResponse._Words);
                    Assert.IsTrue(listWordsResponse._Words.Count > 0);
                    Assert.IsNull(error);
                },
                customizationId: customizationId,
                wordType: "all",
                sort: "alphabetical"
            );

            while (listWordsResponse == null)
                yield return null;
        }
        #endregion

        #region WaitForCorpus
        [UnityTest, Order(21)]
        [Timeout(int.MaxValue)]
        public IEnumerator WaitForCorpus2()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to WaitForCorpus2...");
            isCorpusReady = false;
            Runnable.Run(CheckCorpusStatus());

            while (!isCorpusReady)
                yield return null;
        }
        #endregion

        #region TrainLanguageModel
        [UnityTest, Order(22)]
        public IEnumerator TestTrainLanguageModel2()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to TrainLanguageModel2...");
            object trainLanguageModelResponse = null;
            service.TrainLanguageModel(
                callback: (DetailedResponse<TrainingResponse> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "TrainLanguageModel result: {0}", response.Response);
                    trainLanguageModelResponse = response.Result;
                    Assert.IsNotNull(trainLanguageModelResponse);
                    Assert.IsNull(error);
                },
                customizationId: customizationId
            );

            while (trainLanguageModelResponse == null)
                yield return null;
        }
        #endregion

        #region WaitForLanguageModel
        [UnityTest, Order(23)]
        [Timeout(int.MaxValue)]
        public IEnumerator WaitForLanguageModel2()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to WaitForLanguageModel2...");
            isLanguageModelReady = false;
            Runnable.Run(CheckLanguageModelStatus());

            while (!isLanguageModelReady)
                yield return null;
        }
        #endregion

        #region AddGrammar
        [UnityTest, Order(24)]
        public IEnumerator TestAddGrammar()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to AddGrammar...");
            bool isComplete = false;
            service.AddGrammar(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "AddGrammar result: {0}", response.Response);
                    Assert.IsTrue(response.StatusCode == 201);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                customizationId: customizationId,
                grammarName: grammarName,
                grammarFile: File.ReadAllText(grammarPath),
                contentType: grammarsContentType,
                allowOverwrite: true
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region GetGrammar
        [UnityTest, Order(25)]
        public IEnumerator TestGetGrammar()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to GetGrammar...");
            Grammar getGrammarResponse = null;
            service.GetGrammar(
                callback: (DetailedResponse<Grammar> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "GetGrammar result: {0}", response.Response);
                    getGrammarResponse = response.Result;
                    Assert.IsNotNull(getGrammarResponse);
                    Assert.IsTrue(getGrammarResponse.Name == grammarName);
                    Assert.IsNull(error);
                },
                customizationId: customizationId,
                grammarName: grammarName
            );

            while (getGrammarResponse == null)
                yield return null;
        }
        #endregion

        #region ListGrammars
        [UnityTest, Order(26)]
        public IEnumerator TestListGrammars()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to ListGrammars...");
            Grammars listGrammarsResponse = null;
            service.ListGrammars(
                callback: (DetailedResponse<Grammars> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "ListGrammars result: {0}", response.Response);
                    listGrammarsResponse = response.Result;
                    Assert.IsNotNull(listGrammarsResponse);
                    Assert.IsNotNull(listGrammarsResponse._Grammars);
                    Assert.IsTrue(listGrammarsResponse._Grammars.Count > 0);
                    Assert.IsNull(error);
                },
                customizationId: customizationId
            );

            while (listGrammarsResponse == null)
                yield return null;
        }
        #endregion

        #region WaitForGrammars
        [UnityTest, Order(27)]
        [Timeout(int.MaxValue)]
        public IEnumerator WaitForGrammars()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to WaitForGrammars...");
            isGrammarsReady = false;
            Runnable.Run(CheckGrammarsStatus());

            while (!isGrammarsReady)
                yield return null;
        }
        #endregion

        #region TrainLanguageModel
        [UnityTest, Order(28)]
        public IEnumerator TestTrainLanguageModel3()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to TrainLanguageModel3...");
            object trainLanguageModelResponse = null;
            service.TrainLanguageModel(
                callback: (DetailedResponse<TrainingResponse> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "TrainLanguageModel result: {0}", response.Response);
                    trainLanguageModelResponse = response.Result;
                    Assert.IsNotNull(trainLanguageModelResponse);
                    Assert.IsNull(error);
                },
                customizationId: customizationId
            );

            while (trainLanguageModelResponse == null)
                yield return null;
        }
        #endregion

        #region WaitForLanguageModel
        [UnityTest, Order(29)]
        [Timeout(int.MaxValue)]
        public IEnumerator WaitForLanguageModel3()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to WaitForLanguageModel3...");
            isLanguageModelReady = false;
            Runnable.Run(CheckLanguageModelStatus());

            while (!isLanguageModelReady)
                yield return null;
        }
        #endregion

        #region UpgradeLanguageModel
        //[UnityTest, Order(0)]
        //public IEnumerator TestUpgradeLanguageModel()
        //{
        //    Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to UpgradeLanguageModel...");
        //    object upgradeLanguageModelResponse = null;
        //    service.UpgradeLanguageModel(
        //        callback: (DetailedResponse<object> response, IBMError error) =>
        //        {
        //            Log.Debug("SpeechToTextServiceV1IntegrationTests", "UpgradeLanguageModel result: {0}", response.Response);
        //            upgradeLanguageModelResponse = response.Result;
        //            Assert.IsNotNull(upgradeLanguageModelResponse);
        //            Assert.IsNull(error);
        //        },
        //        customizationId: customizationId
        //    );

        //    while (upgradeLanguageModelResponse == null)
        //        yield return null;
        //}
        #endregion

        #region CreateAcousticModel
        [UnityTest, Order(30)]
        public IEnumerator TestCreateAcousticModel()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to CreateAcousticModel...");
            AcousticModel createAcousticModelResponse = null;
            service.CreateAcousticModel(
                callback: (DetailedResponse<AcousticModel> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "CreateAcousticModel result: {0}", response.Response);
                    createAcousticModelResponse = response.Result;
                    acousticModelCustomizationId = createAcousticModelResponse.CustomizationId;
                    Assert.IsNotNull(createAcousticModelResponse);
                    Assert.IsNotNull(acousticModelCustomizationId);
                    Assert.IsNull(error);
                },
                name: acousticModelName,
                baseModelName: usBroadbandModel,
                description: acousticModelDescription
            );

            while (createAcousticModelResponse == null)
                yield return null;
        }
        #endregion

        #region GetAcousticModel
        [UnityTest, Order(31)]
        public IEnumerator TestGetAcousticModel()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to GetAcousticModel...");
            AcousticModel getAcousticModelResponse = null;
            service.GetAcousticModel(
                callback: (DetailedResponse<AcousticModel> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "GetAcousticModel result: {0}", response.Response);
                    getAcousticModelResponse = response.Result;
                    Assert.IsNotNull(getAcousticModelResponse);
                    Assert.IsTrue(getAcousticModelResponse.CustomizationId == acousticModelCustomizationId);
                    Assert.IsTrue(getAcousticModelResponse.Name == acousticModelName);
                    Assert.IsTrue(getAcousticModelResponse.Description == acousticModelDescription);
                    Assert.IsTrue(getAcousticModelResponse.BaseModelName == usBroadbandModel);
                    Assert.IsNull(error);
                },
                customizationId: acousticModelCustomizationId
            );

            while (getAcousticModelResponse == null)
                yield return null;
        }
        #endregion

        #region ListAcousticModels
        [UnityTest, Order(32)]
        public IEnumerator TestListAcousticModels()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to ListAcousticModels...");
            AcousticModels listAcousticModelsResponse = null;
            service.ListAcousticModels(
                callback: (DetailedResponse<AcousticModels> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "ListAcousticModels result: {0}", response.Response);
                    listAcousticModelsResponse = response.Result;
                    Assert.IsNotNull(listAcousticModelsResponse);
                    Assert.IsNotNull(listAcousticModelsResponse.Customizations);
                    Assert.IsTrue(listAcousticModelsResponse.Customizations.Count > 0);
                    Assert.IsNull(error);
                },
                language: languageModelDialect
            );

            while (listAcousticModelsResponse == null)
                yield return null;
        }
        #endregion

        #region UpgradeAcousticModel
        //[UnityTest, Order(0)]
        //public IEnumerator TestUpgradeAcousticModel()
        //{
        //    Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to UpgradeAcousticModel...");
        //    object upgradeAcousticModelResponse = null;
        //    service.UpgradeAcousticModel(
        //        callback: (DetailedResponse<object> response, IBMError error) =>
        //        {
        //            Log.Debug("SpeechToTextServiceV1IntegrationTests", "UpgradeAcousticModel result: {0}", response.Response);
        //            upgradeAcousticModelResponse = response.Result;
        //            Assert.IsNotNull(upgradeAcousticModelResponse);
        //            Assert.IsNull(error);
        //        },
        //        customizationId: customizationId,
        //        customLanguageModelId: customLanguageModelId,
        //        force: force
        //    );

        //    while (upgradeAcousticModelResponse == null)
        //        yield return null;
        //}
        #endregion

        #region AddAudio
        [UnityTest, Order(33)]
        public IEnumerator TestAddAudio()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to AddAudio...");
            bool isComplete = false;
            service.AddAudio(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "AddAudio result: {0}", response.Response);
                    Assert.IsTrue(response.StatusCode == 201);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                customizationId: acousticModelCustomizationId,
                audioName: acousticResourceName,
                audioResource: acousticResourceData,
                allowOverwrite: true,
                contentType: acousticResourceMimeType,
                containedContentType: acousticResourceMimeType
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region GetAudio
        [UnityTest, Order(34)]
        public IEnumerator TestGetAudio()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to GetAudio...");
            AudioListing getAudioResponse = null;
            service.GetAudio(
                callback: (DetailedResponse<AudioListing> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "GetAudio result: {0}", response.Response);
                    getAudioResponse = response.Result;
                    Assert.IsNotNull(getAudioResponse);
                    Assert.IsTrue(getAudioResponse.Name == acousticResourceName);
                    Assert.IsTrue(getAudioResponse.Duration is long?);
                    Assert.IsNull(error);
                },
                customizationId: acousticModelCustomizationId,
                audioName: acousticResourceName
            );

            while (getAudioResponse == null)
                yield return null;
        }
        #endregion

        #region ListAudio
        [UnityTest, Order(35)]
        public IEnumerator TestListAudio()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to ListAudio...");
            AudioResources listAudioResponse = null;
            service.ListAudio(
                callback: (DetailedResponse<AudioResources> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "ListAudio result: {0}", response.Response);
                    listAudioResponse = response.Result;
                    Assert.IsNotNull(listAudioResponse);
                    Assert.IsNotNull(listAudioResponse.Audio);
                    Assert.IsTrue(listAudioResponse.Audio.Count > 0);
                    Assert.IsTrue(listAudioResponse.Audio[0].Duration is long?);
                    Assert.IsNull(error);
                },
                customizationId: acousticModelCustomizationId
            );

            while (listAudioResponse == null)
                yield return null;
        }
        #endregion

        #region WaitForAudio
        [UnityTest, Order(36)]
        [Timeout(int.MaxValue)]
        public IEnumerator WaitForAudio()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to WaitForAudio...");
            isAudioReady = false;
            Runnable.Run(CheckAudioStatus());

            while (!isAudioReady)
                yield return null;
        }
        #endregion

        #region TrainAcousticModel
        [UnityTest, Order(37)]
        public IEnumerator TestTrainAcousticModel()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to TrainAcousticModel...");
            bool isComplete = false;
            service.TrainAcousticModel(
                callback: (DetailedResponse<TrainingResponse> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "TrainAcousticModel result: {0}", response.Response);
                    Assert.IsTrue(response.StatusCode == 200);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                customizationId: acousticModelCustomizationId,
                customLanguageModelId: customizationId
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region WaitForAcousticModel
        [UnityTest, Order(38)]
        [Timeout(int.MaxValue)]
        public IEnumerator WaitForAcousticModel()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to WaitForAcousticModel...");
            isAcousticModelReady = false;
            Runnable.Run(CheckAcousticModelStatus());

            while (!isAcousticModelReady)
                yield return null;
        }
        #endregion

        #region DeleteAudio
        [UnityTest, Order(90)]
        public IEnumerator TestDeleteAudio()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to DeleteAudio...");
            bool isComplete = false;
            service.DeleteAudio(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "DeleteAudio result: {0}", response.Response);
                    Assert.IsTrue(response.StatusCode == 200);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                customizationId: acousticModelCustomizationId,
                audioName: acousticResourceName
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region ResetAcousticModel
        [UnityTest, Order(91)]
        public IEnumerator TestResetAcousticModel()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to ResetAcousticModel...");
            bool isComplete = false;
            service.ResetAcousticModel(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "ResetAcousticModel result: {0}", response.Response);
                    Assert.IsTrue(response.StatusCode == 200);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                customizationId: acousticModelCustomizationId
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region DeleteAcousticModel
        [UnityTest, Order(92)]
        public IEnumerator TestDeleteAcousticModel()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to DeleteAcousticModel...");
            bool isComplete = false;
            service.DeleteAcousticModel(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "DeleteAcousticModel result: {0}", response.Response);
                    Assert.IsTrue(response.StatusCode == 200);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                customizationId: acousticModelCustomizationId
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region DeleteGrammar
        [UnityTest, Order(93)]
        public IEnumerator TestDeleteGrammar()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to DeleteGrammar...");
            bool isComplete = false;
            service.DeleteGrammar(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "DeleteGrammar result: {0}", response.Response);
                    Assert.IsTrue(response.StatusCode == 200);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                customizationId: customizationId,
                grammarName: grammarName
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region DeleteWord
        [UnityTest, Order(94)]
        public IEnumerator TestDeleteWord()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to DeleteWord...");
            bool isComplete = false;
            service.DeleteWord(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "DeleteWord result: {0}", response.Response);
                    Assert.IsTrue(response.StatusCode == 200);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                customizationId: customizationId,
                wordName: wordName
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region DeleteCorpus
        [UnityTest, Order(95)]
        public IEnumerator TestDeleteCorpus()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to DeleteCorpus...");
            bool isComplete = false;
            service.DeleteCorpus(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "DeleteCorpus result: {0}", response.Response);
                    isComplete = true;
                    Assert.IsNull(error);
                    Assert.IsTrue(response.StatusCode == 200);
                },
                customizationId: customizationId,
                corpusName: corpusName
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region ResetLanguageModel
        [UnityTest, Order(96)]
        public IEnumerator TestResetLanguageModel()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to ResetLanguageModel...");
            bool isComplete = false;
            service.ResetLanguageModel(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "ResetLanguageModel result: {0}", response.Response);
                    Assert.IsTrue(response.StatusCode == 200);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                customizationId: customizationId
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region DeleteLanguageModel
        [UnityTest, Order(97)]
        public IEnumerator TestDeleteLanguageModel()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to DeleteLanguageModel...");
            bool isComplete = false;
            service.DeleteLanguageModel(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "DeleteLanguageModel result: {0}", response.Response);
                    Assert.IsNull(error);
                    Assert.IsTrue(response.StatusCode == 200);
                    isComplete = true;
                },
                customizationId: customizationId
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region DeleteJob
        [UnityTest, Order(98)]
        public IEnumerator TestDeleteJob()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to DeleteJob...");
            bool isComplete = false;
            service.DeleteJob(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "DeleteJob result: {0}", response.Response);
                    Assert.IsNull(error);
                    var statusCode = response.StatusCode;
                    Assert.IsTrue(response.StatusCode == 204);
                    isComplete = true;
                },
                id: jobId
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region DeleteUserData
        [UnityTest, Order(99)]
        public IEnumerator TestDeleteUserData()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to DeleteUserData...");
            object deleteUserDataResponse = null;
            service.DeleteUserData(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("SpeechToTextServiceV1IntegrationTests", "DeleteUserData result: {0}", response.Response);
                    deleteUserDataResponse = response.Result;
                    Assert.IsNotNull(deleteUserDataResponse);
                    Assert.IsNull(error);
                },
                customerId: "customerId"
            );

            while (deleteUserDataResponse == null)
                yield return null;
        }
        #endregion

        #region CheckLanguageModelStatus
        private IEnumerator CheckLanguageModelStatus()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to CheckLanguageModelStatus in 15 seconds...");
            yield return new WaitForSeconds(15f);

            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to GetLanguageModel...");
            LanguageModel getLanguageModelResponse = null;
            try
            {
                service.GetLanguageModel(
                    callback: (DetailedResponse<LanguageModel> response, IBMError error) =>
                    {
                        getLanguageModelResponse = response.Result;
                        Log.Debug("SpeechToTextServiceV1IntegrationTests", "CheckLanguageModelStatus: {0}", getLanguageModelResponse.Status);
                        if (getLanguageModelResponse.Status == LanguageModel.StatusValue.TRAINING)
                        {
                            Runnable.Run(CheckLanguageModelStatus());
                        }
                        else
                        {
                            isLanguageModelReady = true;
                        }
                    },
                    customizationId: customizationId
                );
            }
            catch
            {
                Runnable.Run(CheckLanguageModelStatus());
            }

            while (getLanguageModelResponse == null)
                yield return null;
        }
        #endregion

        #region CheckCorpusStatus
        private IEnumerator CheckCorpusStatus()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to CheckCorpusStatus in 15 seconds...");
            yield return new WaitForSeconds(15f);

            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to GetCorpus...");
            Corpus getCorpusResponse = null;
            try
            {
                service.GetCorpus(
                    callback: (DetailedResponse<Corpus> response, IBMError error) =>
                    {
                        getCorpusResponse = response.Result;
                        Log.Debug("SpeechToTextServiceV1IntegrationTests", "CheckCorpusStatus: {0}", getCorpusResponse.Status);
                        if (getCorpusResponse.Status != Corpus.StatusValue.ANALYZED)
                        {
                            Runnable.Run(CheckCorpusStatus());
                        }
                        else
                        {
                            isCorpusReady = true;
                        }
                    },
                    customizationId: customizationId,
                    corpusName: corpusName
                );
            }
            catch
            {
                Runnable.Run(CheckCorpusStatus());
            }

            while (getCorpusResponse == null)
                yield return null;
        }
        #endregion

        #region CheckGrammarsStatus
        private IEnumerator CheckGrammarsStatus()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to CheckGrammarsStatus in 15 seconds...");
            yield return new WaitForSeconds(15f);

            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to GetGrammar...");
            Grammar getGrammarResponse = null;
            try
            {
                service.GetGrammar(
                    callback: (DetailedResponse<Grammar> response, IBMError error) =>
                    {
                        getGrammarResponse = response.Result;
                        if (getGrammarResponse.Status != Grammar.StatusValue.ANALYZED)
                        {
                            Runnable.Run(CheckGrammarsStatus());
                        }
                        else
                        {
                            isGrammarsReady = true;
                        }
                    },
                    customizationId: customizationId,
                    grammarName: grammarName
                );
            }
            catch
            {
                Runnable.Run(CheckGrammarsStatus());
            }

            while (getGrammarResponse == null)
                yield return null;
        }
        #endregion

        #region CheckAcousticModelStatus
        private IEnumerator CheckAcousticModelStatus()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to CheckAcousticModelStatus in 15 seconds...");
            yield return new WaitForSeconds(15f);

            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to GetAcousticModel...");
            AcousticModel getAcousticModelResponse = null;
            try
            {
                service.GetAcousticModel(
                    callback: (DetailedResponse<AcousticModel> response, IBMError error) =>
                    {
                        getAcousticModelResponse = response.Result;
                        Log.Debug("SpeechToTextServiceV1IntegrationTests", "CheckAcousticModelStatus: {0}", getAcousticModelResponse.Status);
                        if (getAcousticModelResponse.Status == AcousticModel.StatusValue.READY || getAcousticModelResponse.Status == AcousticModel.StatusValue.AVAILABLE)
                        {
                            isAcousticModelReady = true;
                        }
                        else
                        {
                            Runnable.Run(CheckAcousticModelStatus());
                        }
                    },
                    customizationId: acousticModelCustomizationId
                );
            }
            catch
            {
                Runnable.Run(CheckAcousticModelStatus());
            }

            while (getAcousticModelResponse == null)
                yield return null;
        }
        #endregion

        #region CheckAudioStatus
        private IEnumerator CheckAudioStatus()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to CheckAcousticModelStatus in 15 seconds...");
            yield return new WaitForSeconds(15f);

            Log.Debug("SpeechToTextServiceV1IntegrationTests", "Attempting to GetAudio...");
            AudioListing getAudioResponse = null;
            try
            {
                service.GetAudio(
                    callback: (DetailedResponse<AudioListing> response, IBMError error) =>
                    {
                        getAudioResponse = response.Result;
                        Log.Debug("SpeechToTextServiceV1IntegrationTests", "CheckAudioStatus: {0}", getAudioResponse.Status);

                        if (getAudioResponse.Status != AudioListing.StatusValue.OK)
                        {
                            Runnable.Run(CheckAudioStatus());
                        }
                        else
                        {
                            isAudioReady = true;
                        }
                    },
                    customizationId: acousticModelCustomizationId,
                    audioName: acousticResourceName
                );
            }
            catch
            {
                Runnable.Run(CheckAudioStatus());
            }

            while (getAudioResponse == null)
                yield return null;
        }
        #endregion

        #region DownloadAcousticResource
        private IEnumerator DownloadAcousticResource()
        {
            Log.Debug("SpeechToTextServiceV1IntegrationTests", "downloading acoustic resource from {0}", acousticResourceUrl);
            using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(acousticResourceUrl))
            {
                yield return unityWebRequest.SendWebRequest();

                Log.Debug("SpeechToTextServiceV1IntegrationTests", "acoustic resource downloaded");
                acousticResourceData = unityWebRequest.downloadHandler.data;
            }
        }
        #endregion
    }
}
