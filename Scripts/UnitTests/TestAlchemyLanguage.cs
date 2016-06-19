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

using System.Collections;
using IBM.Watson.DeveloperCloud.Services.AlchemyLanguage.v1;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestAlchemyLanguage : UnitTest
    {

        AlchemyLanguage m_AlchemyLanguage = new AlchemyLanguage();
        bool m_EntityExtractionTested = false;
        int m_NumberOfTestEntityExtraction = -1;

        public override IEnumerator RunTest()
        {
            Log.Status("TestAlchemyLanguage", "Test waiting to start");


            if (Config.Instance.FindCredentials(m_AlchemyLanguage.GetServiceID()) == null)
                yield break;

            Log.Status("TestAlchemyLanguage", "Test Started");

            m_NumberOfTestEntityExtraction = 7;
            m_AlchemyLanguage.GetCombinedData(OnGetCombinedCall, "How are you Watson?");
            m_AlchemyLanguage.GetCombinedData(OnGetCombinedCall, "How can Watson help patients?");
            m_AlchemyLanguage.GetCombinedData(OnGetCombinedCall, "Where is Watson?");       //Name
            m_AlchemyLanguage.GetCombinedData(OnGetCombinedCall, "Where is Paris?");        //Location
            m_AlchemyLanguage.GetCombinedData(OnGetCombinedCall, "Which ships are close to Karratha");  //location with name
            m_AlchemyLanguage.GetCombinedData(OnGetCombinedCall, "Are you artifical intelligence?");
            m_AlchemyLanguage.GetCombinedData(OnGetCombinedCall, "What happened in NewYork in december 1913 and February 1975");

            while (!m_EntityExtractionTested)
                yield return null;

            yield break;
        }

        private void OnGetCombinedCall(CombinedCallData combinedCallData, string data)
        {
            m_NumberOfTestEntityExtraction--;
            Log.Status("TestAlchemyLanguage", "Remaining: {0}, original text:{1}, OnGetCombinedCall: {2}, OnLongResult: {3}", m_NumberOfTestEntityExtraction, data, combinedCallData.EntityCombinedCommaSeperated, combinedCallData.ToLongString());

            if (m_NumberOfTestEntityExtraction <= 0)
            {
                Test(true);
                m_EntityExtractionTested = true;
            }

        }

    }
}

