using UnityEngine;
using System;
using System.Collections;

public class WSTextToSpeech : MonoBehaviour {

	public bool isTestingTextToSpeech = false;
	public string testString = "Hello world";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (isTestingTextToSpeech) {
			isTestingTextToSpeech = false;
			testTextToSpeech();
		}
	}

	private void testTextToSpeech(){
		StartSpeeking (testString, null);
	}

	private void StartSpeeking(string voiceString, Action <byte[]>  callback)
	{
		//need to move these to the config file
		string username = "a5aa6c60-c917-4519-abf8-7256169536f2";
		string password = "YVeOHnAMm48F";
		
		string ttsURL = "https://" + username + ":"+ password +"@stream.watsonplatform.net/text-to-speech-beta/api/v1/synthesize?accept=audio/wav&text=" + voiceString.Replace(" ","%20");
		WWW uri = new WWW (ttsURL);//make a Json object here as a Dictionary<string,string>
		Debug.Log (uri.url);
		StartCoroutine(SendVoiceRequest(uri, callback));
	}

	private IEnumerator SendVoiceRequest(WWW www, Action <byte[]> callback)
	{
		yield return www;
		if (www.error == null && www.isDone) 
		{
			byte[] data=www.bytes;
			System.IO.File.WriteAllBytes("tempWavTesting.wav",data);
			//callback(data);
		} 
		else 
		{
			Debug.Log ("Error: " + www.error + " - url:" + www.url);
		}
	}
}
