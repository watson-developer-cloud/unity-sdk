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
* @author Taj Santiago (asantiago@us.ibm.com)
*/

using UnityEngine;
using IBM.Watson.Services;

public class GeolocationTest : MonoBehaviour {
	private string m_GeolocationString = "Austin, TX";
	private Geolocation m_GeoLoc = new Geolocation();
	private string m_token = "zzqbZSZQNw9M0g3Hy1FKd57ibcdN5aTuVxqVqEvBFhdVBm1sycAeI_tc65GO8IWXcUXZTda3lyVGvl07O2SKsXK49yEEFGy2wEj5uqAesgxAiLoSj1SE8XdzTbC6TXaWmTwzVXBRja4g9MWgM4BIrQ..";
	private Geolocation.GeolocationData geoData;


	void Start () {
		if(!m_GeoLoc.getLocation(m_GeolocationString, m_token, OnGeolocation))
			Debug.Log("Geolocation failed!");
     }

	private void OnGeolocation(Geolocation.GeolocationData data)
	{
		geoData = data;

		Debug.Log("location name: " + geoData.locations[0].name);
	}
}