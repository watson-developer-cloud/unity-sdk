/**
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

using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.NaturalLanguageUnderstanding.v1;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestNaturalLanguageUnderstanding : UnitTest
    {
        private NaturalLanguageUnderstanding m_NaturalLanguageUnderstanding = new NaturalLanguageUnderstanding();

        private bool m_AnalyzeTested = false;
        private bool m_ListModelsTested = false;
        public override IEnumerator RunTest()
        {
            LogSystem.InstallDefaultReactors();

            Log.Debug("TestNaturalLanguageUnderstanding", "Attempting to list environments");
            if (!m_NaturalLanguageUnderstanding.GetModels((ListModelsResults resp, string data) =>
            {
                Test(resp != null);
                m_ListModelsTested = true;
            }))
                Log.Debug("TestNaturalLanguageUnderstanding", "Failed to list models");

            while (!m_ListModelsTested)
                yield return null;

            Log.Debug("TestNaturalLanguageUnderstanding", "Attempting to analyze");
            Parameters parameters = new Parameters()
            {
                text = "Analyze various features of text content at scale. Provide text, raw HTML, or a public URL, and IBM Watson Natural Language Understanding will give you results for the features you request. The service cleans HTML content before analysis by default, so the results can ignore most advertisements and other unwanted content.",
                return_analyzed_text = true,
                language = "en",
                features = new Features()
                {
                    entities = new EntitiesOptions()
                    {
                        limit = 50,
                        sentiment = true,
                        emotion = true,
                    },
                    keywords = new KeywordsOptions()
                    {
                        limit = 50,
                        sentiment = true,
                        emotion = true
                    }
                }
            };

            if (!m_NaturalLanguageUnderstanding.Analyze((AnalysisResults resp, string data) =>
            {
                Test(resp != null);
                m_AnalyzeTested = true;
            }, parameters))
                Log.Debug("TestNaturalLanguageUnderstanding", "Failed to analyze");

            while (!m_AnalyzeTested)
                yield return null;

            yield break;
        }
    }
}
