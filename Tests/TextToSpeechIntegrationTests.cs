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
using System.Text.RegularExpressions;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.TextToSpeech.V1;
using IBM.Watson.TextToSpeech.V1.Model;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace IBM.Watson.Tests
{
    public class TextToSpeechV1IntegrationTests
    {
        private TextToSpeechService service;
        private string allisionVoice = "en-US_AllisonVoice";
        private string synthesizeText = "Hello, welcome to the Watson Unity SDK!";
        private string synthesizeMimeType = "audio/wav";
        private string customModelName = "unity-sdk-voice-model";
        private string customModelNameUpdated = "unity-sdk-voice-model-updated";
        private string customModelDescription = "Custom voice model for the Unity SDK integration tests. Safe to delete";
        private string customModelDescriptionUpdated = "Custom voice model for the Unity SDK integration tests. Safe to delete. (Updated)";
        private string customModelLanguage = "en-US";
        private string customizationId;
        private string customWord = "IBM";
        private string customWordTranslation = "eye bee m";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            LogSystem.InstallDefaultReactors();
        }

        [UnitySetUp]
        public IEnumerator UnityTestSetup()
        {
            if (service == null)
            {
                service = new TextToSpeechService();
            }

            while (!service.Authenticator.CanAuthenticate())
                yield return null;
        }

        [SetUp]
        public void TestSetup()
        {
            service.WithHeader("X-Watson-Test", "1");
        }

        #region GetVoice
        [UnityTest, Order(0)]
        public IEnumerator TestGetVoice()
        {
            Log.Debug("TextToSpeechServiceV1IntegrationTests", "Attempting to GetVoice...");
            Voice getVoiceResponse = null;
            service.GetVoice(
                callback: (DetailedResponse<Voice> response, IBMError error) =>
                {
                    Log.Debug("TextToSpeechServiceV1IntegrationTests", "GetVoice result: {0}", response.Response);
                    getVoiceResponse = response.Result;
                    Assert.IsNotNull(getVoiceResponse);
                    Assert.IsTrue(getVoiceResponse.Name == allisionVoice);
                    Assert.IsNull(error);
                },
                voice: allisionVoice
            );

            while (getVoiceResponse == null)
                yield return null;
        }
        #endregion

        #region ListVoices
        [UnityTest, Order(1)]
        public IEnumerator TestListVoices()
        {
            Log.Debug("TextToSpeechServiceV1IntegrationTests", "Attempting to ListVoices...");
            Voices listVoicesResponse = null;
            service.ListVoices(
                callback: (DetailedResponse<Voices> response, IBMError error) =>
                {
                    Log.Debug("TextToSpeechServiceV1IntegrationTests", "ListVoices result: {0}", response.Response);
                    listVoicesResponse = response.Result;
                    Assert.IsNotNull(listVoicesResponse);
                    Assert.IsNotNull(listVoicesResponse._Voices);
                    Assert.IsTrue(listVoicesResponse._Voices.Count > 0);
                    Assert.IsNull(error);
                }
            );

            while (listVoicesResponse == null)
                yield return null;
        }
        #endregion

        #region Synthesize
        [UnityTest, Order(2)]
        public IEnumerator TestSynthesize()
        {
            Log.Debug("TextToSpeechServiceV1IntegrationTests", "Attempting to Synthesize...");
            byte[] synthesizeResponse = null;
            AudioClip clip = null;
            service.Synthesize(
                callback: (DetailedResponse<byte[]> response, IBMError error) =>
                {
                    synthesizeResponse = response.Result;
                    Assert.IsNotNull(synthesizeResponse);
                    Assert.IsNull(error);
                    clip = WaveFile.ParseWAV("myClip", synthesizeResponse);
                    PlayClip(clip);

                },
                text: synthesizeText,
                voice: allisionVoice,
                accept: synthesizeMimeType
            );

            while (synthesizeResponse == null)
                yield return null;

            yield return new WaitForSeconds(clip.length);
        }
        #endregion

        #region GetPronunciation
        [UnityTest, Order(3)]
        public IEnumerator TestGetPronunciation()
        {
            Log.Debug("TextToSpeechServiceV1IntegrationTests", "Attempting to GetPronunciation...");
            Pronunciation getPronunciationResponse = null;
            service.GetPronunciation(
                callback: (DetailedResponse<Pronunciation> response, IBMError error) =>
                {
                    Log.Debug("TextToSpeechServiceV1IntegrationTests", "GetPronunciation result: {0}", response.Response);
                    getPronunciationResponse = response.Result;
                    Assert.IsNotNull(getPronunciationResponse);
                    Assert.IsNotNull(getPronunciationResponse._Pronunciation);
                    Assert.IsNull(error);
                },
                text: synthesizeText,
                voice: allisionVoice,
                format: "ipa"
            );

            while (getPronunciationResponse == null)
                yield return null;
        }
        #endregion

        #region CreateCustomModel
        [UnityTest, Order(4)]
        public IEnumerator TestCreateCustomModel()
        {
            Log.Debug("TextToSpeechServiceV1IntegrationTests", "Attempting to CreateCustomModel...");
            CustomModel createCustomModelResponse = null;
            service.CreateCustomModel(
                callback: (DetailedResponse<CustomModel> response, IBMError error) =>
                {
                    Log.Debug("TextToSpeechServiceV1IntegrationTests", "CreateCustomModel result: {0}", response.Response);
                    createCustomModelResponse = response.Result;
                    customizationId = createCustomModelResponse.CustomizationId;
                    Assert.IsNotNull(createCustomModelResponse);
                    Assert.IsNotNull(customizationId);
                    Assert.IsNull(error);
                },
                name: customModelName,
                language: customModelLanguage,
                description: customModelDescription
            );

            while (createCustomModelResponse == null)
                yield return null;
        }
        #endregion

        #region GetCustomModel
        [UnityTest, Order(5)]
        public IEnumerator TestGetCustomModel()
        {
            Log.Debug("TextToSpeechServiceV1IntegrationTests", "Attempting to GetCustomModel...");
            CustomModel getCustomModelResponse = null;
            service.GetCustomModel(
                callback: (DetailedResponse<CustomModel> response, IBMError error) =>
                {
                    Log.Debug("TextToSpeechServiceV1IntegrationTests", "GetCustomModel result: {0}", response.Response);
                    getCustomModelResponse = response.Result;
                    Assert.IsNotNull(getCustomModelResponse);
                    Assert.IsTrue(getCustomModelResponse.CustomizationId == customizationId);
                    Assert.IsTrue(getCustomModelResponse.Name == customModelName);
                    Assert.IsTrue(getCustomModelResponse.Language == customModelLanguage);
                    Assert.IsTrue(getCustomModelResponse.Description == customModelDescription);
                    Assert.IsNull(error);
                },
                customizationId: customizationId
            );

            while (getCustomModelResponse == null)
                yield return null;
        }
        #endregion

        #region ListCustomModels
        [UnityTest, Order(6)]
        public IEnumerator TestListCustomModels()
        {
            Log.Debug("TextToSpeechServiceV1IntegrationTests", "Attempting to ListCustomModels...");
            CustomModels listCustomModelsResponse = null;
            service.ListCustomModels(
                callback: (DetailedResponse<CustomModels> response, IBMError error) =>
                {
                    Log.Debug("TextToSpeechServiceV1IntegrationTests", "ListCustomModels result: {0}", response.Response);
                    listCustomModelsResponse = response.Result;
                    Assert.IsNotNull(listCustomModelsResponse);
                    Assert.IsNotNull(listCustomModelsResponse.Customizations);
                    Assert.IsTrue(listCustomModelsResponse.Customizations.Count > 0);
                    Assert.IsNull(error);
                },
                language: customModelLanguage
            );

            while (listCustomModelsResponse == null)
                yield return null;
        }
        #endregion

        #region UpdateCustomModel
        [UnityTest, Order(7)]
        public IEnumerator TestUpdateCustomModel()
        {
            Log.Debug("TextToSpeechServiceV1IntegrationTests", "Attempting to UpdateCustomModel...");
            object updateCustomModelResponse = null;
            service.UpdateCustomModel(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("TextToSpeechServiceV1IntegrationTests", "UpdateCustomModel result: {0}", response.Response);
                    updateCustomModelResponse = response.Result;
                    Assert.IsNotNull(updateCustomModelResponse);
                    Assert.IsNull(error);
                },
                customizationId: customizationId,
                name: customModelNameUpdated,
                description: customModelDescriptionUpdated
            );

            while (updateCustomModelResponse == null)
                yield return null;
        }
        #endregion

        #region AddWord
        [UnityTest, Order(8)]
        public IEnumerator TestAddWord()
        {
            Log.Debug("TextToSpeechServiceV1IntegrationTests", "Attempting to AddWord...");
            bool isComplete = false;
            service.AddWord(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("TextToSpeechServiceV1IntegrationTests", "AddWord result: {0}", response.Response);
                    Assert.IsTrue(response.StatusCode == 200);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                customizationId: customizationId,
                word: customWord,
                translation: customWordTranslation
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region AddWords
        [UnityTest, Order(9)]
        public IEnumerator TestAddWords()
        {
            Log.Debug("TextToSpeechServiceV1IntegrationTests", "Attempting to AddWords...");
            bool isComplete = false;
            List<Word> words = new List<Word>()
            {
                new Word()
                {
                    _Word = "hello",
                    Translation = "hullo"
                },
                new Word()
                {
                    _Word = "goodbye",
                    Translation = "gbye"
                },
                new Word()
                {
                    _Word = "hi",
                    Translation = "ohioooo"
                }
            };

            service.AddWords(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("TextToSpeechServiceV1IntegrationTests", "AddWords result: {0}", response.Response);
                    Assert.IsTrue(response.StatusCode == 200);
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
        [UnityTest, Order(10)]
        public IEnumerator TestGetWord()
        {
            Log.Debug("TextToSpeechServiceV1IntegrationTests", "Attempting to GetWord...");
            Translation getWordResponse = null;
            service.GetWord(
                callback: (DetailedResponse<Translation> response, IBMError error) =>
                {
                    Log.Debug("TextToSpeechServiceV1IntegrationTests", "GetWord result: {0}", response.Response);
                    getWordResponse = response.Result;
                    Assert.IsNotNull(getWordResponse);
                    Assert.IsTrue(getWordResponse._Translation == customWordTranslation);
                    Assert.IsNull(error);
                },
                customizationId: customizationId,
                word: customWord
            );

            while (getWordResponse == null)
                yield return null;
        }
        #endregion

        #region ListWords
        [UnityTest, Order(11)]
        public IEnumerator TestListWords()
        {
            Log.Debug("TextToSpeechServiceV1IntegrationTests", "Attempting to ListWords...");
            Words listWordsResponse = null;
            service.ListWords(
                callback: (DetailedResponse<Words> response, IBMError error) =>
                {
                    Log.Debug("TextToSpeechServiceV1IntegrationTests", "ListWords result: {0}", response.Response);
                    listWordsResponse = response.Result;
                    Assert.IsNotNull(listWordsResponse);
                    Assert.IsNotNull(listWordsResponse._Words);
                    Assert.IsTrue(listWordsResponse._Words.Count > 0);
                    Assert.IsNull(error);
                },
                customizationId: customizationId
            );

            while (listWordsResponse == null)
                yield return null;
        }
        #endregion

        #region DeleteWord
        [UnityTest, Order(97)]
        public IEnumerator TestDeleteWord()
        {
            Log.Debug("TextToSpeechServiceV1IntegrationTests", "Attempting to DeleteWord...");
            bool isComplete = false;
            service.DeleteWord(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("TextToSpeechServiceV1IntegrationTests", "DeleteWord result: {0}", response.Response);
                    Assert.IsTrue(response.StatusCode == 204);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                customizationId: customizationId,
                word: customWord
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region DeleteCustomModel
        [UnityTest, Order(98)]
        public IEnumerator TestDeleteCustomModel()
        {
            Log.Debug("TextToSpeechServiceV1IntegrationTests", "Attempting to DeleteCustomModel...");
            bool isComplete = false;
            service.DeleteCustomModel(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("TextToSpeechServiceV1IntegrationTests", "DeleteCustomModel result: {0}", response.Response);
                    Assert.IsTrue(response.StatusCode == 204);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                customizationId: customizationId
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region DeleteUserData
        [UnityTest, Order(99)]
        public IEnumerator TestDeleteUserData()
        {
            Log.Debug("TextToSpeechServiceV1IntegrationTests", "Attempting to DeleteUserData...");
            object deleteUserDataResponse = null;
            service.DeleteUserData(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("TextToSpeechServiceV1IntegrationTests", "DeleteUserData result: {0}", response.Response);
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

        #region PlayClip
        private void PlayClip(AudioClip clip)
        {
            if (Application.isPlaying && clip != null)
            {
                GameObject audioObject = new GameObject("AudioObject");
                AudioSource source = audioObject.AddComponent<AudioSource>();
                source.spatialBlend = 0.0f;
                source.loop = false;
                source.clip = clip;
                source.Play();

                GameObject.Destroy(audioObject, clip.length);
            }
        }
        #endregion
    }
}
