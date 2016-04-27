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


using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestSpeechToText : UnitTest
    {
        private SpeechToText m_SpeechToText = new SpeechToText();
        private bool m_GetModelsTested = false;

        public override IEnumerator RunTest()
        {
            if (Config.Instance.FindCredentials(m_SpeechToText.GetServiceID()) == null)
                yield break;

            m_SpeechToText.GetModels(OnGetModels);

            while (!m_GetModelsTested)
                yield return null;

            yield break;
        }

        private void OnGetModels(SpeechModel[] models)
        {
            Test(models != null);
            if (models != null)
                Log.Status("TestSpeechToText", "GetModels() returned {0} models.", models.Length);

            m_GetModelsTested = true;
        }
    }
}
