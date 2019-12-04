/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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

using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.PersonalityInsights.V3;
using IBM.Watson.PersonalityInsights.V3.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IBM.Watson.Examples
{
    public class ExamplePersonalityInsightsV3 : MonoBehaviour
    {
        private PersonalityInsightsService service;
        private string testString = "The IBM Watson™ Personality Insights service provides a Representational State Transfer (REST) Application Programming Interface (API) that enables applications to derive insights from social media, enterprise data, or other digital communications. The service uses linguistic analytics to infer individuals' intrinsic personality characteristics, including Big Five, Needs, and Values, from digital communications such as email, text messages, tweets, and forum posts. The service can automatically infer, from potentially noisy social media, portraits of individuals that reflect their personality characteristics. The service can report consumption preferences based on the results of its analysis, and for JSON content that is timestamped, it can report temporal behavior.";
        private string dataPath;

        private bool profileTested = false;
        private bool profileAsCsvTested = false;

        private void Start()
        {
            LogSystem.InstallDefaultReactors();
            dataPath = Application.dataPath + "Watson/Examples/TestData/PersonalityInsights/V3/personalityInsights.json";
            Runnable.Run(CreateService());
        }

        private IEnumerator CreateService()
        {

            IamAuthenticator authenticator = new IamAuthenticator(apikey: "{iamApikey}");

            //  Wait for tokendata
            while (!authenticator.CanAuthenticate())
                yield return null;

            service = new PersonalityInsightsService("2019-02-18", authenticator);
            service.SetServiceUrl("{serviceUrl}");

            Runnable.Run(Examples());
        }

        private IEnumerator Examples()
        {
            Content content = new Content()
            {
                ContentItems = new List<ContentItem>()
                {
                    new ContentItem()
                    {
                        Content = testString,
                        Contenttype = ContentItem.ContenttypeValue.TEXT_PLAIN,
                        Language = ContentItem.LanguageValue.EN
                    }
                }
            };

            Log.Debug("ExamplePersonalityInsights.Examples()", "Attempting to Profile...");
            service.Profile(OnProfile, content: content);
            while (!profileTested)
                yield return null;

            Log.Debug("ExamplePersonalityInsights.Examples()", "Attempting to ProfileAsCsv...");
            service.ProfileAsCsv(OnProfileAsCsv, content: content, consumptionPreferences: true);
            while (!profileAsCsvTested)
                yield return null;

            Log.Debug("ExamplePersonalityInsights.Examples()", "Personality insights examples complete.");
        }


        private void OnProfile(DetailedResponse<Profile> response, IBMError error)
        {
            Log.Debug("ExamplePersonaltyInsightsV3.OnProfile()", "Response: {0}", response.Response);
            profileTested = true;
        }

        private void OnProfileAsCsv(DetailedResponse<MemoryStream> response, IBMError error)
        {
            //Log.Debug("ExamplePersonaltyInsightsV3.OnProfile()", "Response: {0}", response.Response);
            profileAsCsvTested = true;
        }
    }
}
