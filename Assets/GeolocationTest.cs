using UnityEngine;
using System.Collections;
using IBM.Watson.Services;

public class GeolocationTest : MonoBehaviour {
	private string m_GeolocationString = "Austin, TX";
	private Geolocation m_GeoLoc = new Geolocation();

	void Start () {
		if(!m_GeoLoc.getLocation(m_GeolocationString, OnGeolocation))
			Debug.Log("Geolocation failed!");
     }

	private void OnGeolocation(Geolocation.GeolocationData data)
	{
		Debug.Log("OnGeolocation callback: " + data);
	}
}