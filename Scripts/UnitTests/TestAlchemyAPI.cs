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
using IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestAlchemyAPI : UnitTest
    {

        AlchemyAPI m_AlchemnyAPI = new AlchemyAPI();
        bool m_EntityExtractionTested = false;
        int m_NumberOfTestEntityExtraction = -1;

        public override IEnumerator RunTest()
        {
            Log.Status("TestAlchemyAPI", "Test waiting to start");


            if (Config.Instance.FindCredentials(m_AlchemnyAPI.GetServiceID()) == null)
                yield break;

            Log.Status("TestAlchemyAPI", "Test Started");

            m_NumberOfTestEntityExtraction = 7;
            m_AlchemnyAPI.GetCombinedCall(OnGetCombinedCall, "How are you Watson?");
            m_AlchemnyAPI.GetCombinedCall(OnGetCombinedCall, "How can Watson help patients?");
            m_AlchemnyAPI.GetCombinedCall(OnGetCombinedCall, "Where is Watson?");       //Name
            m_AlchemnyAPI.GetCombinedCall(OnGetCombinedCall, "Where is Paris?");        //Location
            m_AlchemnyAPI.GetCombinedCall(OnGetCombinedCall, "Which ships are close to Karratha");  //location with name
            m_AlchemnyAPI.GetCombinedCall(OnGetCombinedCall, "Are you artifical intelligence?");
            m_AlchemnyAPI.GetCombinedCall(OnGetCombinedCall, "What happened in NewYork in december 1913 and February 1975");

            while (!m_EntityExtractionTested)
                yield return null;

            yield break;
        }

        private void OnGetCombinedCall(CombinedCallData combinedCallData, string data)
        {
            m_NumberOfTestEntityExtraction--;
            Log.Status("TestAlchemyAPI", "Remaining: {0}, original text:{1}, OnGetCombinedCall: {2}, OnLongResult: {3}", m_NumberOfTestEntityExtraction, data, combinedCallData.EntityCombinedCommaSeperated, combinedCallData.ToLongString());

            if (m_NumberOfTestEntityExtraction <= 0)
            {
                Test(true);
                m_EntityExtractionTested = true;
            }

        }

    }
}

