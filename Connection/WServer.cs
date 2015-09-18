using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WServer : MonoBehaviour {

	#region Watson Server Singleton object
	private static WServer _instance;
	/// <summary>
	/// Gets the instance of Watson Server
	/// </summary>
	/// <value>The instance of Watson Server has functions to use</value>
	public static WServer Instance{
		get{
			if(_instance == null){
				_instance = WConfiguration.gameObjectWatson.AddComponent<WServer>();
			}
			return _instance;
		}
	}
	#endregion

	#region Watson Server Variables

	private static string _serverURI = null;
	public static string ServerURI{
		get{
			if(string.IsNullOrEmpty(_serverURI)){
				_serverURI = "";
			}
			return _serverURI;
		}
	}

	private static string _Token = null;
	public static string Token{
		get{
			return _Token;
		}
		set{
			_Token = value;
		}
	}
	#endregion


	public static bool Login(string userName, string password){
		return Instance._Login(userName, password);
	}

	private bool _Login(string userName, string password){

		WWWForm form = new WWWForm();

		Dictionary<string, string> headers = form.headers.AddAuthorizationHeader(userName, password);
		WWW uri = new WWW ("https://stream.watsonplatform.net/authorization/api/v1/token?url=https://stream.watsonplatform.net/text-to-speech/api", null, headers );

		StartCoroutine(waitForResponce(uri));
	
		return true;
	}

	private IEnumerator waitForResponce(WWW www)
	{
		yield return www;
		Token = www.text;
		Debug.Log("Called : "+ www.url +" \n URL call response: \n" + www.text);
	}

}

/// <summary>
/// Helper Class to generate Basic Auth Headers for GET/POST
/// </summary>
public static class WWWHeaders
{
	public static string CreateAuthorization(string aUserName, string aPassword)
	{
		return "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(aUserName + ":" + aPassword));
	}
	
	public static Dictionary<string, string> AddAuthorizationHeader(this Dictionary<string, string> aHeaders, string aUserName, string aPassword)
	{
		aHeaders.Add("Authorization",CreateAuthorization(aUserName, aPassword));
		return aHeaders;
	}
}
