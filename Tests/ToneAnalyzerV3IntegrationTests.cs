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
using IBM.Watson.ToneAnalyzer.V3;
using IBM.Watson.ToneAnalyzer.V3.Model;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace IBM.Watson.Tests
{
    public class ToneAnalyzerV3IntegrationTests
    {
        private ToneAnalyzerService service;
        private string versionDate = "2019-02-13";
        private string inputText = "Hello! Welcome to IBM Watson! How can I help you?";
        private string chatUser = "testChatUser";

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
                service = new ToneAnalyzerService(versionDate);
            }

            while (!service.Authenticator.CanAuthenticate())
                yield return null;
        }

        [SetUp]
        public void TestSetup()
        {
            service.WithHeader("X-Watson-Test", "1");
        }

        #region Tone
        [UnityTest, Order(0)]
        public IEnumerator TestTone()
        {
            Log.Debug("ToneAnalyzerServiceV3IntegrationTests", "Attempting to Tone...");
            ToneAnalysis toneResponse = null;
            ToneInput toneInput = new ToneInput()
            {
                Text = inputText
            };
            service.Tone(
                callback: (DetailedResponse<ToneAnalysis> response, IBMError error) =>
                {
                    Log.Debug("ToneAnalyzerServiceV3IntegrationTests", "Tone result: {0}", response.Response);
                    toneResponse = response.Result;
                    Assert.IsNotNull(toneResponse);
                    Assert.IsNotNull(toneResponse.SentencesTone);
                    Assert.IsNotNull(toneResponse.DocumentTone);
                    Assert.IsNull(error);
                },
                toneInput: toneInput,
                contentLanguage: "en",
                acceptLanguage: "en",
                contentType: "text/plain"
            );

            while (toneResponse == null)
                yield return null;
        }
        #endregion

        #region ToneChat
        [UnityTest, Order(1)]
        public IEnumerator TestToneChat()
        {
            Log.Debug("ToneAnalyzerServiceV3IntegrationTests", "Attempting to ToneChat...");
            UtteranceAnalyses toneChatResponse = null;
            List<Utterance> utterances = new List<Utterance>()
            {
                new Utterance()
                {
                    Text = inputText,
                    User = chatUser
                }
            };
            service.ToneChat(
                callback: (DetailedResponse<UtteranceAnalyses> response, IBMError error) =>
                {
                    Log.Debug("ToneAnalyzerServiceV3IntegrationTests", "ToneChat result: {0}", response.Response);
                    toneChatResponse = response.Result;
                    Assert.IsNotNull(toneChatResponse);
                    Assert.IsNotNull(toneChatResponse.UtterancesTone);
                    Assert.IsNull(error);
                },
                utterances: utterances,
                contentLanguage: "en",
                acceptLanguage: "en"
            );

            while (toneChatResponse == null)
                yield return null;
        }
        #endregion

    }
}
