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
using IBM.Cloud.SDK.Authentication;
using IBM.Watson.PersonalityInsights.V3;
using IBM.Watson.PersonalityInsights.V3.Model;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace IBM.Watson.Tests
{
    public class PersonalityInsightsV3IntegrationTests
    {
        private PersonalityInsightsService service;
        private string versionDate = "2019-02-13";
        private string contentToProfile = "The IBM Watson™ Personality Insights service provides a Representational State Transfer (REST) Application Programming Interface (API) that enables applications to derive insights from social media, enterprise data, or other digital communications. The service uses linguistic analytics to infer individuals' intrinsic personality characteristics, including Big Five, Needs, and Values, from digital communications such as email, text messages, tweets, and forum posts. The service can automatically infer, from potentially noisy social media, portraits of individuals that reflect their personality characteristics. The service can report consumption preferences based on the results of its analysis, and for JSON content that is timestamped, it can report temporal behavior.";

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
                service = new PersonalityInsightsService(versionDate);
            }

            while (!service.Authenticator.CanAuthenticate())
                yield return null;
        }

        [SetUp]
        public void TestSetup()
        {
            service.WithHeader("X-Watson-Test", "1");
        }

        #region Profile
        [UnityTest, Order(0)]
        public IEnumerator TestProfile()
        {
            Log.Debug("PersonalityInsightsServiceV3IntegrationTests", "Attempting to Profile...");
            Profile profileResponse = null;
            Content content = new Content()
            {
                ContentItems = new List<ContentItem>()
                {
                    new ContentItem()
                    {
                        Contenttype = ContentItem.ContenttypeValue.TEXT_PLAIN,
                        Language = ContentItem.LanguageValue.EN,
                        Content = contentToProfile
                    }
                }
            };

            service.Profile(
                callback: (DetailedResponse<Profile> response, IBMError error) =>
                {
                    Log.Debug("PersonalityInsightsServiceV3IntegrationTests", "Profile result: {0}", response.Response);
                    profileResponse = response.Result;
                    Assert.IsNotNull(profileResponse);
                    Assert.IsNotNull(profileResponse.Personality);
                    Assert.IsNull(error);
                },
                content: content,
                contentLanguage: "en",
                acceptLanguage: "en",
                rawScores: true,
                csvHeaders: true,
                consumptionPreferences: true,
                contentType: "text/plain"
            );

            while (profileResponse == null)
                yield return null;
        }
        #endregion

        #region ProfileAsCsv
        [UnityTest, Order(1)]
        public IEnumerator TestProfileAsCsv()
        {
            Log.Debug("PersonalityInsightsServiceV3IntegrationTests", "Attempting to ProfileAsCsv...");
            System.IO.MemoryStream profileAsCsvResponse = null;
            Content content = new Content()
            {
                ContentItems = new List<ContentItem>()
                {
                    new ContentItem()
                    {
                        Contenttype = ContentItem.ContenttypeValue.TEXT_PLAIN,
                        Language = ContentItem.LanguageValue.EN,
                        Content = contentToProfile
                    }
                }
            };

            service.ProfileAsCsv(
                callback: (DetailedResponse<System.IO.MemoryStream> response, IBMError error) =>
                {
                    profileAsCsvResponse = response.Result;
                    Assert.IsNotNull(profileAsCsvResponse);
                    Assert.IsNull(error);

                    //  Save file
                    using (FileStream fs = File.Create(Application.dataPath + "/personality-profile.csv"))
                    {
                        profileAsCsvResponse.WriteTo(fs);
                        fs.Close();
                        profileAsCsvResponse.Close();
                    }
                },
                content: content,
                contentLanguage: "en",
                acceptLanguage: "en",
                rawScores: true,
                csvHeaders: true,
                consumptionPreferences: true,
                contentType: "text/plain"
            );

            while (profileAsCsvResponse == null)
                yield return null;
        }
        #endregion

    }
}
