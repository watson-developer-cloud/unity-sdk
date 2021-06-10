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
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Watson.NaturalLanguageUnderstanding.V1;
using IBM.Watson.NaturalLanguageUnderstanding.V1.Model;
using NUnit.Framework;
using UnityEngine.TestTools;
using System.IO;
using System.Text;

namespace IBM.Watson.Tests
{
    public class NaturalLanguageUnderstandingV1IntegrationTests
    {
        private NaturalLanguageUnderstandingService service;
        private string versionDate = "2019-07-12";
        private string nluText = "Analyze various features of text content at scale. Provide text, raw HTML, or a public URL, and IBM Watson Natural Language Understanding will give you results for the features you request. The service cleans HTML content before analysis by default, so the results can ignore most advertisements and other unwanted content.";
        private string modelId;
        private MemoryStream categoriesModelTrainingData = new MemoryStream(ASCIIEncoding.Default.GetBytes("[\n"
                        + "       {\n"
                        + "           \"labels\": [\n"
                        + "               \"level1\"\n"
                        + "           ],\n"
                        + "           \"key_phrases\": [\n"
                        + "               \"key phrase\",\n"
                        + "               \"key phrase 2\"\n"
                        + "           ]\n"
                        + "       },\n"
                        + "       {\n"
                        + "           \"labels\": [\n"
                        + "               \"level1\",\n"
                        + "               \"level2\"\n"
                        + "           ],\n"
                        + "           \"key_phrases\": [\n"
                        + "               \"key phrase 3\",\n"
                        + "               \"key phrase 4\"\n"
                        + "           ]\n"
                        + "       }\n"
                        + "   ]"));

        private MemoryStream classificationModelTrainingData = new MemoryStream(ASCIIEncoding.Default.GetBytes("[\n"
                        + "    {\n"
                        + "        \"text\": \"How hot is it today?\",\n"
                        + "        \"labels\": [\"temperature\"]\n"
                        + "    },\n"
                        + "    {\n"
                        + "        \"text\": \"Is it hot outside?\",\n"
                        + "        \"labels\": [\"temperature\"]\n"
                        + "    },\n"
                        + "    {\n"
                        + "        \"text\": \"Will it be uncomfortably hot?\",\n"
                        + "        \"labels\": [\"temperature\"]\n"
                        + "    },\n"
                        + "    {\n"
                        + "        \"text\": \"Will it be sweltering?\",\n"
                        + "        \"labels\": [\"temperature\"]\n"
                        + "    },\n"
                        + "    {\n"
                        + "        \"text\": \"How cold is it today?\",\n"
                        + "        \"labels\": [\"temperature\"]\n"
                        + "    },\n"
                        + "    {\n"
                        + "        \"text\": \"Will we get snow?\",\n"
                        + "        \"labels\": [\"conditions\"]\n"
                        + "    },\n"
                        + "    {\n"
                        + "        \"text\": \"Are we expecting sunny conditions?\",\n"
                        + "        \"labels\": [\"conditions\"]\n"
                        + "    },\n"
                        + "    {\n"
                        + "        \"text\": \"Is it overcast?\",\n"
                        + "        \"labels\": [\"conditions\"]\n"
                        + "    },\n"
                        + "    {\n"
                        + "        \"text\": \"Will it be cloudy?\",\n"
                        + "        \"labels\": [\"conditions\"]\n"
                        + "    },\n"
                        + "    {\n"
                        + "        \"text\": \"How much rain will fall today?\",\n"
                        + "        \"labels\": [\"conditions\"]\n"
                        + "    }\n"
                        + "]"));

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
                service = new NaturalLanguageUnderstandingService(versionDate);
            }

            while (!service.Authenticator.CanAuthenticate())
                yield return null;
        }

        [SetUp]
        public void TestSetup()
        {
            service.WithHeader("X-Watson-Test", "1");
        }

        #region Analyze
        [UnityTest, Order(0)]
        public IEnumerator TestAnalyze()
        {
            Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "Attempting to Analyze...");
            AnalysisResults analyzeResponse = null;
            Features features = new Features()
            {
                Keywords = new KeywordsOptions()
                {
                    Limit = 8,
                    Sentiment = true,
                    Emotion = true
                },
                Categories = new CategoriesOptions()
                {
                    Limit = 10
                },
                Syntax = new SyntaxOptions()
                {
                    Sentences = true,
                    Tokens = new SyntaxOptionsTokens()
                    {
                        PartOfSpeech = true,
                        Lemma = true
                    }
                }
            };

            service.Analyze(
                callback: (DetailedResponse<AnalysisResults> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "Analyze result: {0}", response.Response);
                    analyzeResponse = response.Result;
                    Assert.IsNotNull(analyzeResponse);
                    Assert.IsNotNull(analyzeResponse.Syntax);
                    Assert.IsNull(error);
                },
                features: features,
                text: nluText
            );

            while (analyzeResponse == null)
                yield return null;
        }
        #endregion

        #region DeleteModel
        //  Skipping since we cannot create a model using the api.
        //[UnityTest, Order(1)]
        //public IEnumerator TestDeleteModel()
        //{
        //    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "Attempting to DeleteModel...");
        //    DeleteModelResults deleteModelResponse = null;
        //    service.DeleteModel(
        //        callback: (DetailedResponse<DeleteModelResults> response, IBMError error) =>
        //        {
        //            Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "DeleteModel result: {0}", response.Response);
        //            deleteModelResponse = response.Result;
        //            Assert.IsNotNull(deleteModelResponse);
        //            Assert.IsNull(error);
        //        },
        //        modelId: modelId
        //    );

        //    while (deleteModelResponse == null)
        //        yield return null;
        //}
        #endregion

        #region ListModels
        [UnityTest, Order(2)]
        public IEnumerator TestListModels()
        {
            Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "Attempting to ListModels...");
            ListModelsResults listModelsResponse = null;
            service.ListModels(
                callback: (DetailedResponse<ListModelsResults> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "ListModels result: {0}", response.Response);
                    listModelsResponse = response.Result;
                    Assert.IsNotNull(listModelsResponse);
                    Assert.IsNotNull(listModelsResponse.Models);
                    Assert.IsNull(error);
                }
            );

            while (listModelsResponse == null)
                yield return null;
        }
        #endregion

        #region SentimentModel
        [UnityTest, Order(3)]
        public IEnumerator TestCreateSentimentModel()
        {
            Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "Attempting to CreateSentimentModel...");
            SentimentModel createSentimentModelResponse = null;
            string modelId = "";
            MemoryStream trainingData = new MemoryStream(ASCIIEncoding.Default.GetBytes("This is a mock file."));
            
            service.CreateSentimentModel(
                callback: (DetailedResponse<SentimentModel> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "CreateSentimentModel result: {0}", response.Response);
                    createSentimentModelResponse = response.Result;
                    Assert.IsNotNull(createSentimentModelResponse);
                    Assert.AreEqual(createSentimentModelResponse.Name, "testString");
                    Assert.AreEqual(createSentimentModelResponse.Language, "en");
                    Assert.AreEqual(createSentimentModelResponse.Description, "testString");
                    Assert.AreEqual(createSentimentModelResponse.ModelVersion, "testString");
                    Assert.AreEqual(createSentimentModelResponse.VersionDescription, "testString");
                    Assert.IsNull(error);

                    modelId = createSentimentModelResponse.ModelId;
                },
                language: "en",
                trainingData: trainingData,
                name: "testString",
                description: "testString", 
                modelVersion: "testString",
                versionDescription: "testString"
            );

            while (createSentimentModelResponse == null)
                yield return null;

            DeleteModelResults deleteModelResults = null;
            service.DeleteSentimentModel(
              callback: (DetailedResponse<DeleteModelResults> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "DeleteSentimentModel result: {0}", response.Response);
                    deleteModelResults = response.Result;
                    Assert.IsNull(error);
                },
                modelId: modelId
            );

            while (deleteModelResults == null)
                yield return null;
        }

        [UnityTest, Order(4)]
        public IEnumerator TestListSentimentModels()
        {
            Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "Attempting to ListSentimentModels...");
            ListSentimentModelsResponse listSentimentModelsResponse = null;
            
            service.ListSentimentModels(
                callback: (DetailedResponse<ListSentimentModelsResponse> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "ListSentimentModels result: {0}", response.Response);
                    listSentimentModelsResponse = response.Result;
                    Assert.IsNotNull(listSentimentModelsResponse);
                    Assert.IsNotNull(listSentimentModelsResponse.Models);
                    Assert.IsNull(error);
                }
            );

            while (listSentimentModelsResponse == null)
                yield return null;
                
            foreach (SentimentModel sentimentModel in listSentimentModelsResponse.Models)
            {
                if (sentimentModel.Name.StartsWith("testString") || sentimentModel.Name.StartsWith("newString"))
                {
                    DeleteModelResults deleteModelResults = null;
                    service.DeleteSentimentModel(
                      callback: (DetailedResponse<DeleteModelResults> response, IBMError error) =>
                        {
                            Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "DeleteSentimentModel result: {0}", response.Response);
                            deleteModelResults = response.Result;
                            Assert.IsNull(error);
                        },
                        modelId: sentimentModel.ModelId
                    );

                    while (deleteModelResults == null)
                        yield return null;
                }
            }
        }
        
        [UnityTest, Order(5)]
        public IEnumerator TestUpdateSentimentModel()
        {
            Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "Attempting to UpdateSentimentModel...");
            SentimentModel createSentimentModelResponse = null;
            string modelId = "";
            MemoryStream trainingData = new MemoryStream(ASCIIEncoding.Default.GetBytes("This is a mock file."));
            
            service.CreateSentimentModel(
                callback: (DetailedResponse<SentimentModel> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "CreateSentimentModel result: {0}", response.Response);
                    createSentimentModelResponse = response.Result;
                    Assert.IsNotNull(createSentimentModelResponse);
                    Assert.AreEqual(createSentimentModelResponse.Name, "testString");
                    Assert.AreEqual(createSentimentModelResponse.Language, "en");
                    Assert.AreEqual(createSentimentModelResponse.Description, "testString");
                    Assert.AreEqual(createSentimentModelResponse.ModelVersion, "testString");
                    Assert.AreEqual(createSentimentModelResponse.VersionDescription, "testString");
                    Assert.IsNull(error);

                    modelId = createSentimentModelResponse.ModelId;
                },
                language: "en",
                trainingData: trainingData,
                name: "testString",
                description: "testString", 
                modelVersion: "testString",
                versionDescription: "testString"
            );

            while (createSentimentModelResponse == null)
                yield return null;

            SentimentModel updateSentimentModel = null;
            service.UpdateSentimentModel(
                callback: (DetailedResponse<SentimentModel> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "UpdateSentimentModel result: {0}", response.Response);
                    updateSentimentModel = response.Result;
                    Assert.IsNotNull(updateSentimentModel);
                    Assert.AreEqual(updateSentimentModel.Name, "newString");
                    Assert.AreEqual(updateSentimentModel.Language, "en");
                    Assert.AreEqual(updateSentimentModel.Description, "newString");
                    Assert.AreEqual(updateSentimentModel.ModelVersion, "testString");
                    Assert.AreEqual(updateSentimentModel.VersionDescription, "testString");
                    Assert.IsNull(error);

                },
                description: "newString",
                name: "newString",
                modelId: modelId,
                language: "en",
                trainingData: trainingData
            );

            while (updateSentimentModel == null)
                yield return null;

            DeleteModelResults deleteModelResults = null;
            service.DeleteSentimentModel(
              callback: (DetailedResponse<DeleteModelResults> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "DeleteSentimentModel result: {0}", response.Response);
                    deleteModelResults = response.Result;
                    Assert.IsNull(error);
                },
                modelId: modelId
            );

            while (deleteModelResults == null)
                yield return null;
        }

        [UnityTest, Order(6)]
        public IEnumerator TestGetSentimentModel()
        {
            Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "Attempting to GetSentimentModel...");
            SentimentModel createSentimentModelResponse = null;
            string modelId = "";
            MemoryStream trainingData = new MemoryStream(ASCIIEncoding.Default.GetBytes("This is a mock file."));
            
            service.CreateSentimentModel(
                callback: (DetailedResponse<SentimentModel> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "CreateSentimentModel result: {0}", response.Response);
                    createSentimentModelResponse = response.Result;
                    Assert.IsNotNull(createSentimentModelResponse);
                    Assert.AreEqual(createSentimentModelResponse.Name, "testString");
                    Assert.AreEqual(createSentimentModelResponse.Language, "en");
                    Assert.AreEqual(createSentimentModelResponse.Description, "testString");
                    Assert.AreEqual(createSentimentModelResponse.ModelVersion, "testString");
                    Assert.AreEqual(createSentimentModelResponse.VersionDescription, "testString");
                    Assert.IsNull(error);

                    modelId = createSentimentModelResponse.ModelId;
                },
                language: "en",
                trainingData: trainingData,
                name: "testString",
                description: "testString", 
                modelVersion: "testString",
                versionDescription: "testString"
            );

            while (createSentimentModelResponse == null)
                yield return null;

            SentimentModel getSentimentModel = null;
            service.GetSentimentModel(
                callback: (DetailedResponse<SentimentModel> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "GetSentimentModel result: {0}", response.Response);
                    getSentimentModel = response.Result;
                    Assert.IsNotNull(getSentimentModel);
                    Assert.AreEqual(getSentimentModel.Name, "testString");
                    Assert.AreEqual(getSentimentModel.Language, "en");
                    Assert.AreEqual(getSentimentModel.Description, "testString");
                    Assert.AreEqual(getSentimentModel.ModelVersion, "testString");
                    Assert.AreEqual(getSentimentModel.VersionDescription, "testString");
                    Assert.IsNull(error);

                },
                modelId: modelId
            );

            while (getSentimentModel == null)
                yield return null;

            DeleteModelResults deleteModelResults = null;
            service.DeleteSentimentModel(
              callback: (DetailedResponse<DeleteModelResults> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "DeleteSentimentModel result: {0}", response.Response);
                    deleteModelResults = response.Result;
                    Assert.IsNull(error);
                },
                modelId: modelId
            );

            while (deleteModelResults == null)
                yield return null;
        }

        #endregion

        #region CategoriesModel
        [UnityTest, Order(7)]
        public IEnumerator TestCreateCategoriesModel()
        {
            Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "Attempting to CreateCategoriesModel...");
            CategoriesModel createCategoriesModelResponse = null;
            string modelId = "";
            
            service.CreateCategoriesModel(
                callback: (DetailedResponse<CategoriesModel> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "CategoriesModel result: {0}", response.Response);
                    createCategoriesModelResponse = response.Result;
                    Assert.IsNotNull(createCategoriesModelResponse);
                    Assert.AreEqual(createCategoriesModelResponse.Name, "testString");
                    Assert.AreEqual(createCategoriesModelResponse.Language, "en");
                    Assert.AreEqual(createCategoriesModelResponse.Description, "testString");
                    Assert.AreEqual(createCategoriesModelResponse.ModelVersion, "testString");
                    Assert.AreEqual(createCategoriesModelResponse.VersionDescription, "testString");
                    Assert.IsNull(error);

                    modelId = createCategoriesModelResponse.ModelId;
                },
                language: "en",
                trainingData: categoriesModelTrainingData,
                trainingDataContentType: "application/json",
                name: "testString",
                description: "testString",
                modelVersion: "testString",
                versionDescription: "testString"
            );

            while (createCategoriesModelResponse == null)
                yield return null;

            DeleteModelResults deleteModelResults = null;
            service.DeleteCategoriesModel(
              callback: (DetailedResponse<DeleteModelResults> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "DeleteCategoriesModel result: {0}", response.Response);
                    deleteModelResults = response.Result;
                    Assert.IsNull(error);
                },
                modelId: modelId
            );

            while (deleteModelResults == null)
                yield return null;
        }

        [UnityTest, Order(8)]
        public IEnumerator TestListCategoriesModels()
        {
            Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "Attempting to ListSentimentModels...");
            CategoriesModelList listCategoriesModelResponse = null;
            
            service.ListCategoriesModels(
                callback: (DetailedResponse<CategoriesModelList> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "ListCategoriesModels result: {0}", response.Response);
                    listCategoriesModelResponse = response.Result;
                    Assert.IsNotNull(listCategoriesModelResponse);
                    Assert.IsNotNull(listCategoriesModelResponse.Models);
                    Assert.IsNull(error);
                }
            );

            while (listCategoriesModelResponse == null)
                yield return null;
            
            foreach (CategoriesModel categoriesModel in listCategoriesModelResponse.Models)
            {
                if (categoriesModel.Name.StartsWith("testString") || categoriesModel.Name.StartsWith("newString"))
                {
                    DeleteModelResults deleteModelResults = null;
                    service.DeleteCategoriesModel(
                      callback: (DetailedResponse<DeleteModelResults> response, IBMError error) =>
                        {
                            Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "DeleteCategoriesModel result: {0}", response.Response);
                            deleteModelResults = response.Result;
                            Assert.IsNull(error);
                        },
                        modelId: categoriesModel.ModelId
                    );

                    while (deleteModelResults == null)
                        yield return null;
                    }
            }
        }
        
        [UnityTest, Order(9)]
        public IEnumerator TestUpdateCategoriesModel()
        {
            Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "Attempting to UpdateSentimentModel...");
            CategoriesModel createCategoriesModelResponse = null;
            string modelId = "";
            
            service.CreateCategoriesModel(
                callback: (DetailedResponse<CategoriesModel> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "CategoriesModel result: {0}", response.Response);
                    createCategoriesModelResponse = response.Result;
                    Assert.IsNotNull(createCategoriesModelResponse);
                    Assert.AreEqual(createCategoriesModelResponse.Name, "testString");
                    Assert.AreEqual(createCategoriesModelResponse.Language, "en");
                    Assert.AreEqual(createCategoriesModelResponse.Description, "testString");
                    Assert.AreEqual(createCategoriesModelResponse.ModelVersion, "testString");
                    Assert.AreEqual(createCategoriesModelResponse.VersionDescription, "testString");
                    Assert.IsNull(error);

                    modelId = createCategoriesModelResponse.ModelId;
                },
                language: "en",
                trainingData: categoriesModelTrainingData,
                trainingDataContentType: "application/json",
                name: "testString",
                description: "testString",
                modelVersion: "testString",
                versionDescription: "testString"
            );

            while (createCategoriesModelResponse == null)
                yield return null;

            CategoriesModel updateCategoriesModel = null;
            service.UpdateCategoriesModel(
                callback: (DetailedResponse<CategoriesModel> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "UpdateCategoriesModel result: {0}", response.Response);
                    updateCategoriesModel = response.Result;
                    Assert.IsNotNull(updateCategoriesModel);
                    Assert.AreEqual(updateCategoriesModel.Name, "newString");
                    Assert.AreEqual(updateCategoriesModel.Language, "en");
                    Assert.AreEqual(updateCategoriesModel.Description, "newString");
                    Assert.AreEqual(updateCategoriesModel.ModelVersion, "testString");
                    Assert.AreEqual(updateCategoriesModel.VersionDescription, "testString");
                    Assert.IsNull(error);

                },
                description: "newString",
                name: "newString",
                modelId: modelId,
                language: "en",
                modelVersion: "testString",
                trainingDataContentType: "application/json",
                trainingData: categoriesModelTrainingData
            );

            while (updateCategoriesModel == null)
                yield return null;

            DeleteModelResults deleteModelResults = null;
            service.DeleteCategoriesModel(
              callback: (DetailedResponse<DeleteModelResults> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "DeleteCategoriesModel result: {0}", response.Response);
                    deleteModelResults = response.Result;
                    Assert.IsNull(error);
                },
                modelId: modelId
            );

            while (deleteModelResults == null)
                yield return null;
        }

        [UnityTest, Order(10)]
        public IEnumerator TestGetCategoriesModel()
        {
            Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "Attempting to GetSentimentModel...");
            CategoriesModel createCategoriesModelResponse = null;
            string modelId = "";
            
            service.CreateCategoriesModel(
                callback: (DetailedResponse<CategoriesModel> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "CategoriesModel result: {0}", response.Response);
                    createCategoriesModelResponse = response.Result;
                    Assert.IsNotNull(createCategoriesModelResponse);
                    Assert.AreEqual(createCategoriesModelResponse.Name, "testString");
                    Assert.AreEqual(createCategoriesModelResponse.Language, "en");
                    Assert.AreEqual(createCategoriesModelResponse.Description, "testString");
                    Assert.AreEqual(createCategoriesModelResponse.ModelVersion, "testString");
                    Assert.AreEqual(createCategoriesModelResponse.VersionDescription, "testString");
                    Assert.IsNull(error);

                    modelId = createCategoriesModelResponse.ModelId;
                },
                language: "en",
                trainingData: categoriesModelTrainingData,
                trainingDataContentType: "application/json",
                name: "testString",
                description: "testString",
                modelVersion: "testString",
                versionDescription: "testString"
            );

            while (createCategoriesModelResponse == null)
                yield return null;

            CategoriesModel getCategoriesModel = null;
            service.GetCategoriesModel(
                callback: (DetailedResponse<CategoriesModel> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "GetCategoriesModel result: {0}", response.Response);
                    getCategoriesModel = response.Result;
                    Assert.IsNotNull(getCategoriesModel);
                    Assert.AreEqual(getCategoriesModel.Name, "testString");
                    Assert.AreEqual(getCategoriesModel.Language, "en");
                    Assert.AreEqual(getCategoriesModel.Description, "testString");
                    Assert.AreEqual(getCategoriesModel.ModelVersion, "testString");
                    Assert.AreEqual(getCategoriesModel.VersionDescription, "testString");
                    Assert.IsNull(error);

                },
                modelId: modelId
            );

            while (getCategoriesModel == null)
                yield return null;

            DeleteModelResults deleteModelResults = null;
            service.DeleteCategoriesModel(
              callback: (DetailedResponse<DeleteModelResults> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "DeleteCategoriesModel result: {0}", response.Response);
                    deleteModelResults = response.Result;
                    Assert.IsNull(error);
                },
                modelId: modelId
            );

            while (deleteModelResults == null)
                yield return null;
        }

        #endregion

        #region ClassificationsModel
        [UnityTest, Order(11)]
        public IEnumerator TestCreateClassificationsModel()
        {
            Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "Attempting to CreateClassificationsModel...");
            ClassificationsModel createClassificationsModelResponse = null;
            string modelId = "";
            
            service.CreateClassificationsModel(
                callback: (DetailedResponse<ClassificationsModel> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "ClassificationsModel result: {0}", response.Response);
                    createClassificationsModelResponse = response.Result;
                    Assert.IsNotNull(createClassificationsModelResponse);
                    Assert.AreEqual(createClassificationsModelResponse.Name, "testString");
                    Assert.AreEqual(createClassificationsModelResponse.Language, "en");
                    Assert.AreEqual(createClassificationsModelResponse.Description, "testString");
                    Assert.AreEqual(createClassificationsModelResponse.ModelVersion, "testString");
                    Assert.AreEqual(createClassificationsModelResponse.VersionDescription, "testString");
                    Assert.IsNull(error);

                    modelId = createClassificationsModelResponse.ModelId;
                },
                language: "en",
                trainingData: classificationModelTrainingData,
                name: "testString",
                description: "testString",
                modelVersion: "testString",
                versionDescription: "testString",
                trainingDataContentType: "application/json"
            );

            while (createClassificationsModelResponse == null)
                yield return null;

            DeleteModelResults deleteModelResults = null;
            service.DeleteClassificationsModel(
              callback: (DetailedResponse<DeleteModelResults> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "DeleteClassificationsModel result: {0}", response.Response);
                    deleteModelResults = response.Result;
                    Assert.IsNull(error);
                },
                modelId: modelId
            );

            while (deleteModelResults == null)
                yield return null;
        }

        [UnityTest, Order(12)]
        public IEnumerator TestListClassificationsModels()
        {
            Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "Attempting to TestListClassificationsModels...");
            ListClassificationsModelsResponse listClassificationsModelsResponse = null;
            
            service.ListClassificationsModels(
                callback: (DetailedResponse<ListClassificationsModelsResponse> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "ListClassificationsModelsResponse result: {0}", response.Response);
                    listClassificationsModelsResponse = response.Result;
                    Assert.IsNotNull(listClassificationsModelsResponse);
                    Assert.IsNotNull(listClassificationsModelsResponse.Models);
                    Assert.IsNull(error);
                }
            );

            while (listClassificationsModelsResponse == null)
                yield return null;
            foreach (ClassificationsModelList classificationsModelList in listClassificationsModelsResponse.Models)
            {
                if (classificationsModelList.Models == null) {
                  continue;
                }
                foreach (ClassificationsModel classificationModel in classificationsModelList.Models)
                {                       
                    if (classificationModel.Name.Contains("testString") || classificationModel.Name.Contains("newString"))
                    {

                        DeleteModelResults deleteModelResults = null;
                        service.DeleteClassificationsModel(
                          callback: (DetailedResponse<DeleteModelResults> response, IBMError error) =>
                            {
                                Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "DeleteCategoriesModel result: {0}", response.Response);
                                deleteModelResults = response.Result;
                                Assert.IsNull(error);
                            },
                            modelId: classificationModel.ModelId
                        );

                        while (deleteModelResults == null)
                            yield return null;
                        }
                }
            }
        }
        
        [UnityTest, Order(13)]
        public IEnumerator TestUpdateClassificationsModel()
        {
            Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "Attempting to UpdateSentimentModel...");
            ClassificationsModel createClassificationsModelResponse = null;
            string modelId = "";
            
            service.CreateClassificationsModel(
                callback: (DetailedResponse<ClassificationsModel> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "ClassificationsModel result: {0}", response.Response);
                    createClassificationsModelResponse = response.Result;
                    Assert.IsNotNull(createClassificationsModelResponse);
                    Assert.AreEqual(createClassificationsModelResponse.Name, "testString");
                    Assert.AreEqual(createClassificationsModelResponse.Language, "en");
                    Assert.AreEqual(createClassificationsModelResponse.Description, "testString");
                    Assert.AreEqual(createClassificationsModelResponse.ModelVersion, "testString");
                    Assert.AreEqual(createClassificationsModelResponse.VersionDescription, "testString");
                    Assert.IsNull(error);

                    modelId = createClassificationsModelResponse.ModelId;
                },
                language: "en",
                trainingData: classificationModelTrainingData,
                name: "testString",
                description: "testString",
                modelVersion: "testString",
                versionDescription: "testString",
                trainingDataContentType: "application/json"
            );

            while (createClassificationsModelResponse == null)
                yield return null;

            ClassificationsModel updateClassificationsModel = null;
            service.UpdateClassificationsModel(
                callback: (DetailedResponse<ClassificationsModel> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "UpdateClassificationsModel result: {0}", response.Response);
                    updateClassificationsModel = response.Result;
                    Assert.IsNotNull(updateClassificationsModel);
                    Assert.AreEqual(updateClassificationsModel.Name, "newString");
                    Assert.AreEqual(updateClassificationsModel.Language, "en");
                    Assert.AreEqual(updateClassificationsModel.Description, "newString");
                    Assert.AreEqual(updateClassificationsModel.ModelVersion, "testString");
                    Assert.AreEqual(updateClassificationsModel.VersionDescription, "testString");
                    Assert.IsNull(error);

                },
                description: "newString",
                name: "newString",
                modelId: modelId,
                language: "en",
                trainingData: classificationModelTrainingData,
                trainingDataContentType: "application/json"
            );

            while (updateClassificationsModel == null)
                yield return null;

            DeleteModelResults deleteModelResults = null;
            service.DeleteClassificationsModel(
              callback: (DetailedResponse<DeleteModelResults> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "DeleteClassificationsModel result: {0}", response.Response);
                    deleteModelResults = response.Result;
                    Assert.IsNull(error);
                },
                modelId: modelId
            );

            while (deleteModelResults == null)
                yield return null;
        }

        [UnityTest, Order(14)]
        public IEnumerator TestGetClassificationsModel()
        {
            Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "Attempting to GetSentimentModel...");
            ClassificationsModel createClassificationsModelResponse = null;
            string modelId = "";
            
            service.CreateClassificationsModel(
                callback: (DetailedResponse<ClassificationsModel> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "ClassificationsModel result: {0}", response.Response);
                    createClassificationsModelResponse = response.Result;
                    Assert.IsNotNull(createClassificationsModelResponse);
                    Assert.AreEqual(createClassificationsModelResponse.Name, "testString");
                    Assert.AreEqual(createClassificationsModelResponse.Language, "en");
                    Assert.AreEqual(createClassificationsModelResponse.Description, "testString");
                    Assert.AreEqual(createClassificationsModelResponse.ModelVersion, "testString");
                    Assert.AreEqual(createClassificationsModelResponse.VersionDescription, "testString");
                    Assert.IsNull(error);

                    modelId = createClassificationsModelResponse.ModelId;
                },
                language: "en",
                trainingData: classificationModelTrainingData,
                name: "testString",
                description: "testString",
                modelVersion: "testString",
                versionDescription: "testString",
                trainingDataContentType: "application/json"
            );

            while (createClassificationsModelResponse == null)
                yield return null;

            ClassificationsModel getClassificationsModel = null;
            service.GetClassificationsModel(
                callback: (DetailedResponse<ClassificationsModel> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "GetClassificationsModel result: {0}", response.Response);
                    getClassificationsModel = response.Result;
                    Assert.IsNotNull(getClassificationsModel);
                    Assert.IsNotNull(getClassificationsModel);
                    Assert.AreEqual(getClassificationsModel.Name, "testString");
                    Assert.AreEqual(getClassificationsModel.Language, "en");
                    Assert.AreEqual(getClassificationsModel.Description, "testString");
                    Assert.AreEqual(getClassificationsModel.ModelVersion, "testString");
                    Assert.AreEqual(getClassificationsModel.VersionDescription, "testString");
                    Assert.IsNull(error);

                },
                modelId: modelId
            );

            while (getClassificationsModel == null)
                yield return null;

            DeleteModelResults deleteModelResults = null;
            service.DeleteClassificationsModel(
              callback: (DetailedResponse<DeleteModelResults> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1IntegrationTests", "DeleteClassificationsModel result: {0}", response.Response);
                    deleteModelResults = response.Result;
                    Assert.IsNull(error);
                },
                modelId: modelId
            );

            while (deleteModelResults == null)
                yield return null;
        }
        #endregion
    }
}
