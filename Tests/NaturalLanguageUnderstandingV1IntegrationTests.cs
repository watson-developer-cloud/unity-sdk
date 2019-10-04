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

namespace IBM.Watson.Tests
{
    public class NaturalLanguageUnderstandingV1IntegrationTests
    {
        private NaturalLanguageUnderstandingService service;
        private string versionDate = "2019-02-13";
        private string nluText = "Analyze various features of text content at scale. Provide text, raw HTML, or a public URL, and IBM Watson Natural Language Understanding will give you results for the features you request. The service cleans HTML content before analysis by default, so the results can ignore most advertisements and other unwanted content.";
        private string modelId;

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
    }
}
