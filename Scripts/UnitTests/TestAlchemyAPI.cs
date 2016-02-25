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
        int m_NumberOfTestEntityExtraction = 17;

        public override IEnumerator RunTest()
        {
            if ( Config.Instance.FindCredentials( m_AlchemnyAPI.GetServiceID() ) == null )
                yield break;

            m_AlchemnyAPI.GetEntityExtraction(OnGetEntityExtraction, "Where is the Woodside Donaldson");
            m_AlchemnyAPI.GetEntityExtraction(OnGetEntityExtraction, "Where is the LNG Egis");
            m_AlchemnyAPI.GetEntityExtraction(OnGetEntityExtraction, "Where is the NWS Swift");
            m_AlchemnyAPI.GetEntityExtraction(OnGetEntityExtraction, "Where are our LNG Ships");
            m_AlchemnyAPI.GetEntityExtraction(OnGetEntityExtraction, "Which LNG ships are close to Karratha");
            m_AlchemnyAPI.GetEntityExtraction(OnGetEntityExtraction, "What is the nearest ship to Woodside Goode");
            m_AlchemnyAPI.GetEntityExtraction(OnGetEntityExtraction, "Can you tell me the location of Woodside Rogers");
            m_AlchemnyAPI.GetEntityExtraction(OnGetEntityExtraction, "Can you tell me the location of ship LNG Ebisu");
            m_AlchemnyAPI.GetEntityExtraction(OnGetEntityExtraction, "Can you tell me the location of ship Northwest Sandpiper");
            m_AlchemnyAPI.GetEntityExtraction(OnGetEntityExtraction, "Show me the location of ship Sea Eagle");
            m_AlchemnyAPI.GetEntityExtraction(OnGetEntityExtraction, "When is ship Shear Water due to reach port Karratha");
            m_AlchemnyAPI.GetEntityExtraction(OnGetEntityExtraction, "What is the eta of ship Woodside Goode");
            m_AlchemnyAPI.GetEntityExtraction(OnGetEntityExtraction, "What is the current speed of ship Woodside Donaldson");
            m_AlchemnyAPI.GetEntityExtraction(OnGetEntityExtraction, "What is the destination of ship Woodside Rogers");
            m_AlchemnyAPI.GetEntityExtraction(OnGetEntityExtraction, "What is the capacity of ship LNG Ebisu");
            m_AlchemnyAPI.GetEntityExtraction(OnGetEntityExtraction, "What is the draught of ship Northwest Sandpiper");
            m_AlchemnyAPI.GetEntityExtraction(OnGetEntityExtraction, "Is ship Snipe laden or ballast");

            while(! m_EntityExtractionTested )
                yield return null;

            yield break;
        }



        private void OnGetEntityExtraction( EntityExtractionData entityExtractionData )
        {
            m_NumberOfTestEntityExtraction--;
            Log.Status("TestAlchemyAPI", "Remaining: {0}, Has Geo Information: {1}, Geo: {2}, OnGetEntityExtraction: {3}", m_NumberOfTestEntityExtraction, entityExtractionData.HasGeographicInformation, (entityExtractionData.HasGeographicInformation?entityExtractionData.GeoLocation.ToString(): "None"), entityExtractionData);

            if(m_NumberOfTestEntityExtraction <= 0){
                Test( true );
                m_EntityExtractionTested = true;
            }

        }

    }
}

