/**
* Copyright 2018, 2019 IBM Corp. All Rights Reserved.
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
using IBM.Watson.LanguageTranslator.V3;
using IBM.Watson.LanguageTranslator.V3.Model;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace IBM.Watson.Tests
{
    public class LanguageTranslatorV3IntegrationTests
    {
        private LanguageTranslatorService service;
        private string versionDate = "2019-02-13";
        private string forcedGlossaryFilepath;
        private string translateDocumentPath;
        private string documentId;
        private string englishText = "Where is the library?";
        private string spanishText = "¿Dónde está la biblioteca?";
        private string englishToSpanishModel = "en-es";
        private string englishToFrenchModel = "en-fr";
        private string customModelName = "unity-sdk-forced-glossary";
        private string customModelId;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            LogSystem.InstallDefaultReactors();
            forcedGlossaryFilepath = Application.dataPath + "/Watson/Tests/TestData/LanguageTranslatorV3/glossary.tmx";
            translateDocumentPath = Application.dataPath + "/Watson/Tests/TestData/LanguageTranslatorV3/translate-document.txt";
        }

        [UnitySetUp]
        public IEnumerator UnityTestSetup()
        {
            if (service == null)
            {
                service = new LanguageTranslatorService(versionDate);
            }

            while (!service.Authenticator.CanAuthenticate())
                yield return null;
        }

        [SetUp]
        public void TestSetup()
        {
            service.WithHeader("X-Watson-Test", "1");
        }

        #region Translate
        [UnityTest, Order(0)]
        public IEnumerator TestTranslate()
        {
            Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "Attempting to Translate...");
            TranslationResult translateResponse = null;
            service.Translate(
                callback: (DetailedResponse<TranslationResult> response, IBMError error) =>
                {
                    Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "Translate result: {0}", response.Response);
                    translateResponse = response.Result;
                    Assert.IsNotNull(translateResponse);
                    Assert.IsNotNull(translateResponse.Translations);
                    Assert.IsTrue(translateResponse.Translations.Count > 0);
                    Assert.IsTrue(translateResponse.Translations[0]._Translation == spanishText);
                    Assert.IsNull(error);
                },
                text: new List<string>() { englishText },
                modelId: englishToSpanishModel
            );

            while (translateResponse == null)
                yield return null;
        }
        #endregion

        #region Identify
        [UnityTest, Order(1)]
        public IEnumerator TestIdentify()
        {
            Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "Attempting to Identify...");
            IdentifiedLanguages identifyResponse = null;
            service.Identify(
                callback: (DetailedResponse<IdentifiedLanguages> response, IBMError error) =>
                {
                    Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "Identify result: {0}", response.Response);
                    identifyResponse = response.Result;
                    Assert.IsNotNull(identifyResponse);
                    Assert.IsNotNull(identifyResponse.Languages);
                    Assert.IsTrue(identifyResponse.Languages.Count > 0);
                    Assert.IsNull(error);
                },
                text: spanishText
            );

            while (identifyResponse == null)
                yield return null;
        }
        #endregion

        #region ListIdentifiableLanguages
        [UnityTest, Order(3)]
        public IEnumerator TestListIdentifiableLanguages()
        {
            Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "Attempting to ListIdentifiableLanguages...");
            IdentifiableLanguages listIdentifiableLanguagesResponse = null;
            service.ListIdentifiableLanguages(
                callback: (DetailedResponse<IdentifiableLanguages> response, IBMError error) =>
                {
                    Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "ListIdentifiableLanguages result: {0}", response.Response);
                    listIdentifiableLanguagesResponse = response.Result;
                    Assert.IsNotNull(listIdentifiableLanguagesResponse);
                    Assert.IsNotNull(listIdentifiableLanguagesResponse.Languages);
                    Assert.IsTrue(listIdentifiableLanguagesResponse.Languages.Count > 0);
                    Assert.IsNull(error);
                }
            );

            while (listIdentifiableLanguagesResponse == null)
                yield return null;
        }
        #endregion

        #region CreateModel
        [UnityTest, Order(4)]
        public IEnumerator TestCreateModel()
        {
            Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "Attempting to CreateModel...");
            TranslationModel createModelResponse = null;
            using (FileStream fs = File.OpenRead(forcedGlossaryFilepath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    service.CreateModel(
                        callback: (DetailedResponse<TranslationModel> response, IBMError error) =>
                        {
                            Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "CreateModel result: {0}", response.Response);
                            createModelResponse = response.Result;
                            customModelId = createModelResponse.ModelId;
                            Assert.IsNotNull(createModelResponse);
                            Assert.IsNotNull(customModelId);
                            Assert.IsTrue(createModelResponse.Source == "en");
                            Assert.IsTrue(createModelResponse.Target == "fr");
                            Assert.IsTrue(createModelResponse.Name == customModelName);
                            Assert.IsNull(error);
                        },
                        baseModelId: englishToFrenchModel,
                        forcedGlossary: ms,
                        name: customModelName
                    );

                    while (createModelResponse == null)
                        yield return null;
                }
            }
        }
        #endregion

        #region GetModel
        [UnityTest, Order(5)]
        public IEnumerator TestGetModel()
        {
            Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "Attempting to GetModel...");
            TranslationModel getModelResponse = null;
            service.GetModel(
                callback: (DetailedResponse<TranslationModel> response, IBMError error) =>
                {
                    Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "GetModel result: {0}", response.Response);
                    getModelResponse = response.Result;
                    Assert.IsNotNull(getModelResponse);
                    Assert.IsTrue(getModelResponse.ModelId == customModelId);
                    Assert.IsTrue(getModelResponse.Source == "en");
                    Assert.IsTrue(getModelResponse.Target == "fr");
                    Assert.IsTrue(getModelResponse.Name == customModelName);
                    Assert.IsNull(error);
                },
                modelId: customModelId
            );

            while (getModelResponse == null)
                yield return null;
        }
        #endregion

        #region ListModels
        [UnityTest, Order(6)]
        public IEnumerator TestListModels()
        {
            Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "Attempting to ListModels...");
            TranslationModels listModelsResponse = null;
            service.ListModels(
                callback: (DetailedResponse<TranslationModels> response, IBMError error) =>
                {
                    Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "ListModels result: {0}", response.Response);
                    listModelsResponse = response.Result;
                    Assert.IsNotNull(listModelsResponse);
                    Assert.IsNotNull(listModelsResponse.Models);
                    Assert.IsTrue(listModelsResponse.Models.Count > 0);
                    Assert.IsNull(error);
                },
                source: "en",
                target: "fr"
            );

            while (listModelsResponse == null)
                yield return null;
        }
        #endregion

        #region DeleteModel
        [UnityTest, Order(7)]
        public IEnumerator TestDeleteModel()
        {
            Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "Attempting to DeleteModel...");
            DeleteModelResult deleteModelResponse = null;
            service.DeleteModel(
                callback: (DetailedResponse<DeleteModelResult> response, IBMError error) =>
                {
                    Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "DeleteModel result: {0}", response.Response);
                    deleteModelResponse = response.Result;
                    Assert.IsNotNull(deleteModelResponse);
                    Assert.IsNotNull(deleteModelResponse.Status);
                    Assert.IsTrue(deleteModelResponse.Status == "OK");
                    Assert.IsNull(error);
                },
                modelId: customModelId
            );

            while (deleteModelResponse == null)
                yield return null;
        }
        #endregion

        #region Translate Document
        [UnityTest, Order(8)]
        public IEnumerator TestTranslateDocument()
        {
            Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "Attempting to Translate...");
            DocumentStatus documentStatus = null;

            using (FileStream fs = File.OpenRead(translateDocumentPath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    service.TranslateDocument(
                        callback: (DetailedResponse<DocumentStatus> response, IBMError error) =>
                        {
                            Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "Translate Document result: {0}", response.Response);
                            documentStatus = response.Result;
                            Assert.IsNotNull(documentStatus);
                            Assert.IsNotNull(documentStatus.DocumentId);
                            documentId = documentStatus.DocumentId;
                            Assert.IsNull(error);
                        },
                        file: ms,
                        filename: "translate-document.txt",
                        modelId: "en-fr"
                    );

                    while (documentStatus == null)
                        yield return null;
                }
            }
        }
        #endregion

        #region List Documents
        [UnityTest, Order(9)]
        public IEnumerator TestListDocuments()
        {
            Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "Getting the document list...");
            DocumentList documents = null;

            service.ListDocuments(
                callback: (DetailedResponse<DocumentList> response, IBMError error) =>
                {
                    Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "List Documents result: {0}", response.Response);
                    documents = response.Result;
                    Assert.IsNotNull(documents);
                    Assert.IsNull(error);
                }
            );

            while (documents == null)
                yield return null;
        }
        #endregion

        #region Get Document Status
        [UnityTest, Order(10)]
        public IEnumerator TestGetDocumentStatus()
        {
            Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "Getting document status...");
            DocumentStatus documentStatus = null;

            service.GetDocumentStatus(
                callback: (DetailedResponse<DocumentStatus> response, IBMError error) =>
                {
                    Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "Get Document Status: {0}", response.Response);
                    documentStatus = response.Result;
                    Assert.IsNotNull(documentStatus);
                    Assert.IsNull(error);
                },
                documentId: documentId
            );

            while (documentStatus == null)
                yield return null;
        }
        #endregion

        #region Delete Document
        [UnityTest, Order(99)]
        public IEnumerator TestDeletetDocument()
        {
            Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "Deleteing the Document...");
            object deleteResponse = null;

            service.DeleteDocument(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("LanguageTranslatorServiceV3IntegrationTests", "Delete Document result: {0}", response.Response);
                    deleteResponse = response.Result;
                    Assert.IsTrue(response.StatusCode == 204);
                    Assert.IsNull(error);
                },
                documentId: documentId
            );

            while (deleteResponse == null)
                yield return null;
        }
        #endregion
    }
}