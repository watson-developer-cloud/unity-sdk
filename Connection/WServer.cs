using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WServer : MonoBehaviour {


	private static WServer _instance;
	/// <summary>
	/// Gets the instance of Watson Server
	/// </summary>
	/// <value>The instance of Watson Server has functions to use</value>
	public WServer instance{
		get{
			if(_instance == null){
				_instance = WConstant.gameObjectWatson.AddComponent<WServer>();
			}
			return _instance;
		}
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
