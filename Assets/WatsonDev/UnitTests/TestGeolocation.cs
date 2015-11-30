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
using IBM.Watson.Services.v1;
using IBM.Watson.Logging;
using System.Collections;

namespace IBM.Watson.UnitTests
{
    public class TestGeolocation : UnitTest
    {
        private string m_GeolocationString = "Austin, TX";
        private Geolocation m_GeoLoc = new Geolocation();
        private string m_token = "zzqbZSZQNw9M0g3Hy1FKd57ibcdN5aTuVxqVqEvBFhdVBm1sycAeI_tc65GO8IWXcUXZTda3lyVGvl07O2SKsXK49yEEFGy2wEj5uqAesgxAiLoSj1SE8XdzTbC6TXaWmTwzVXBRja4g9MWgM4BIrQ..";
        private Geolocation.GeolocationData geoData;

        private bool m_GetLocationTested = false;

        private void OnGeolocation(Geolocation.GeolocationData data)
        {
            Test( data != null );

            geoData = data;
            Log.Status( "TestGeolocation", "location name: " + geoData.locations[0].name);
            m_GetLocationTested = true;
        }

        public override IEnumerator RunTest()
        {
            Test( m_GeoLoc.GetLocation(m_GeolocationString, m_token, OnGeolocation) );
            while(! m_GetLocationTested )
                yield return null;

            yield break;
        }

    }

}
