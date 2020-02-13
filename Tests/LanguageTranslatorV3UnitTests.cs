/**
* (C) Copyright IBM Corp. 2019, 2020.
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NSubstitute;
using IBM.Watson.LanguageTranslator.V3;
using IBM.Watson.LanguageTranslator.V3.Model;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Connection;
using IBM.Cloud.SDK.Authentication.NoAuth;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace IBM.Watson.Tests
{
    public class LanguageTranslatorServiceUnitTests
    {
        NoAuthAuthenticator authenticator = new NoAuthAuthenticator();
        RESTConnector conn;
        LanguageTranslatorService service;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            service = new LanguageTranslatorService("versionDate", authenticator);
            LogSystem.InstallDefaultReactors();
        }

        #region Translate
        [UnityTest, Order(0)]
        public IEnumerator TestTranslate()
        {
            conn = Substitute.For<RESTConnector>();
            service.Connector = conn;

            var translationResponse = new DetailedResponse<TranslationResult>()
            {
                Result = new TranslationResult()
                {
                    WordCount = 5
                }
            };

            conn.Send(
              Arg.Do<RESTConnector.Request>(
                req => {
                    Assert.IsTrue(req.Headers["Accept"] == "application/json");
                    if (((RequestObject<TranslationResult>)req).Callback != null)
                    {
                        Assert.IsTrue(conn.URL == "https://gateway.watsonplatform.net/language-translator/api/v3/translate");
                        ((RequestObject<TranslationResult>)req).Callback(translationResponse, null);
                    }
                }
              ));

            TranslationResult translateResponse = null;
            service.Translate(
                callback: (DetailedResponse<TranslationResult> response, IBMError error) =>
                {
                    Log.Debug("LanguageTranslatorServiceV3UnitTests", "Translate result: {0}", response.Response);
                    translateResponse = response.Result;
                    Assert.IsNotNull(translateResponse);
                    Assert.IsTrue(translateResponse.WordCount > 0);
                    Assert.IsNull(error);
                },
                text: new List<string>() { "Hello World!" },
                modelId: "en-es"
            );

            while (translateResponse == null)
                yield return null;
        }
        #endregion


        #region ListIdentifiableLanguages
        [UnityTest, Order(1)]
        public IEnumerator TestListIdentifiableLanguages()
        {
            conn = Substitute.For<RESTConnector>();
            service.Connector = conn;

            var identifiableLanguagesResponse = new DetailedResponse<IdentifiableLanguages>
            {
                Result = new IdentifiableLanguages()
                {
                    Languages = new List<IdentifiableLanguage>()
                    {
                        new IdentifiableLanguage()
                        {
                            Language = "en"
                        }
                    }
                }
            };

            conn.Send(
              Arg.Do<RESTConnector.Request>(
                req => {
                    if (((RequestObject<IdentifiableLanguages>)req).Callback != null)
                    {
                        Assert.IsTrue(conn.URL == "https://gateway.watsonplatform.net/language-translator/api/v3/identifiable_languages");
                        ((RequestObject<IdentifiableLanguages>)req).Callback(identifiableLanguagesResponse, null);
                    }
                }
              ));

            IdentifiableLanguages listIdentifiableLanguagesResponse = null;
            service.ListIdentifiableLanguages(
                callback: (DetailedResponse<IdentifiableLanguages> response, IBMError error) =>
                {
                    Log.Debug("LanguageTranslatorServiceV3UnitTests", "ListIdentifiableLanguages result: {0}", response.Response);
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

        #region Identify
        [UnityTest, Order(2)]
        public IEnumerator TestIdentify()
        {
            conn = Substitute.For<RESTConnector>();
            service.Connector = conn;

            var identifyMockResponse = new DetailedResponse<IdentifiedLanguages>
            {
                Result = new IdentifiedLanguages()
                {
                    Languages = new List<IdentifiedLanguage>()
                    {
                        new IdentifiedLanguage()
                        {
                            Language = "en"
                        }
                    }
                }
            };

            conn.Send(
              Arg.Do<RESTConnector.Request>(
                req => {
                    if (((RequestObject<IdentifiedLanguages>)req).Callback != null)
                    {
                        Assert.IsTrue(conn.URL == "https://gateway.watsonplatform.net/language-translator/api/v3/identify");
                        ((RequestObject<IdentifiedLanguages>)req).Callback(identifyMockResponse, null);
                    }
                }
              ));

            IdentifiedLanguages identifyResponse = null;
            service.Identify(
                callback: (DetailedResponse<IdentifiedLanguages> response, IBMError error) =>
                {
                    Log.Debug("LanguageTranslatorServiceV3UnitTests", "ListIdentify result: {0}", response.Response);
                    identifyResponse = response.Result;
                    Assert.IsNotNull(identifyResponse);
                    Assert.IsNotNull(identifyResponse.Languages);
                    Assert.IsTrue(identifyResponse.Languages.Count > 0);
                    Assert.IsNull(error);
                },
                text: "Hello world!"
            );

            while (identifyResponse == null)
                yield return null;
        }
        #endregion

        #region ListModels
        [UnityTest, Order(6)]
        public IEnumerator TestListModels()
        {
            conn = Substitute.For<RESTConnector>();
            service.Connector = conn;

            var translationModelsResponse = new DetailedResponse<TranslationModels>()
            {
                Result = new TranslationModels()
                {
                    Models = new List<TranslationModel>()
                    {
                        new TranslationModel()
                        {
                            ModelId = "ar-en",
                            Source = "ar",
                            Target = "en",
                            BaseModelId = "",
                            Domain = "news",
                            Customizable = true,
                            DefaultModel = true,
                            Owner = "",
                            Name = ""
                        }
                    }
                }
            };

            conn.Send(
              Arg.Do<RESTConnector.Request>(
                req => {
                    if (((RequestObject<TranslationModels>)req).Callback != null)
                    {
                        Assert.IsTrue(conn.URL == "https://gateway.watsonplatform.net/language-translator/api/v3/models");
                        ((RequestObject<TranslationModels>)req).Callback(translationModelsResponse, null);
                    }
                }
              ));


            TranslationModels listModelsResponse = null;
            service.ListModels(
                callback: (DetailedResponse<TranslationModels> response, IBMError error) =>
                {
                    Log.Debug("LanguageTranslatorServiceV3UnitTests", "ListModels result: {0}", response.Result.Models[0].ModelId);
                    listModelsResponse = response.Result;
                    Assert.IsNotNull(listModelsResponse);
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


        #region Get Coverage
        [UnityTest, Order(99)]
        public IEnumerator TestCoverage()
        {
            object res = null;
            MethodInfo[] methods = typeof(LanguageTranslatorService).GetMethods();
            int coveredPoints = 0;
            int totalPoints = 0;
            for (int i = 0; i < methods.Length; i++)
            {
                CoveredMethodStats stats = Coverage.GetStatsFor(methods[i]);

                if (!methods[i].Name.Contains("get_") && !methods[i].Name.Contains("set_"))
                {
                    int coveredSequencePoints = stats.totalSequencePoints - stats.uncoveredSequencePoints;
                    coveredPoints += coveredSequencePoints;
                    totalPoints += stats.totalSequencePoints;
                }
                res = true;
            }
            Debug.Log("LanguageTranslatorServiceV3 has " + totalPoints + " sequence points of which " + coveredPoints + " were covered.");
            while (res == null)
                yield return null;
        }
        #endregion

    }
}
