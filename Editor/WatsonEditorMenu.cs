using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;

public class WatsonEditorMenu: EditorWindow  {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	[MenuItem("Watson/Credentials Text to Speech", false, 1)]
	private static void OpenMenuOptionTextToSpeechCredentials(){
		// Get existing open window or if none, make a new one:
		WatsonCredentialTextToSpeech window = (WatsonCredentialTextToSpeech)EditorWindow.GetWindow (typeof (WatsonCredentialTextToSpeech));
		window.Show();
	}

	[MenuItem("Watson/Update Watson Unity SDK", false, 99)]	
	private static void UpdateWatsonUnitySDK(){
		EditorWindow window =  GetWindow<WatsonCancelableProgressBar>();
		window.Show();
		//EditorUtility.DisplayDialog("Update Watson Unity SDK",	" - It will be implemented - ", "Ok...", ""); 	
	}

}

public class WatsonCredentialTextToSpeech : EditorWindow {
	string urlToCall;
	string credentialUserName;
	string credentialPassword;
	string versionNumber;

	void OnEnable(){
		urlToCall = 			EditorPrefs.GetString("WatsonTextToSpeechCredentialURL", 		"https://stream.watsonplatform.net/text-to-speech/api");
		credentialUserName = 	EditorPrefs.GetString("WatsonTextToSpeechCredentialUserName", 	"");
		credentialPassword = 	EditorPrefs.GetString("WatsonTextToSpeechCredentialPassword", 	"");
		versionNumber = 		EditorPrefs.GetString("WatsonTextToSpeechCredentialVersion", 	"v1");
	}

	void OnGUI() {
		urlToCall = EditorGUILayout.TextField("Service URL:", urlToCall);
		versionNumber = EditorGUILayout.TextField("Version Number: ", versionNumber);
		credentialUserName = EditorGUILayout.TextField("User Name:", credentialUserName);
		credentialPassword = EditorGUILayout.TextField("Password:", credentialPassword);

		Rect r = EditorGUILayout.BeginVertical ("Button");
		if(GUI.Button(r, GUIContent.none)){

			GameObject testGameObject = new GameObject();
			testGameObject.name = "~ Watson Editor To Be Deleted";
			WSTextToSpeech wsTextToSpeech = testGameObject.AddComponent<WSTextToSpeech>();
			MonoScript ms = MonoScript.FromMonoBehaviour(wsTextToSpeech);
			string m_ScriptFilePath = AssetDatabase.GetAssetPath( ms );

			string fileText = File.ReadAllText( m_ScriptFilePath);

			fileText = ChangeValue(fileText,  "_serviceURL = \"", urlToCall);
			fileText = ChangeValue(fileText,  "_serviceCredentialUserName = \"", credentialUserName);
			fileText = ChangeValue(fileText,  "_serviceCredentialPassword = \"", credentialPassword);
			fileText = ChangeValue(fileText,  "_serviceVersion = \"", versionNumber);

			File.WriteAllText(m_ScriptFilePath, fileText);

			GameObject.DestroyImmediate(testGameObject);
			//Debug.Log("Saved - " + m_ScriptFilePath + " = datapath " + Application.dataPath + " \n " + fileText  );

			EditorPrefs.SetString("WatsonTextToSpeechCredentialURL", 		urlToCall);
			EditorPrefs.SetString("WatsonTextToSpeechCredentialUserName", 	credentialUserName);
			EditorPrefs.SetString("WatsonTextToSpeechCredentialPassword", 	credentialPassword);
			EditorPrefs.SetString("WatsonTextToSpeechCredentialVersion", 	versionNumber);
		}
		GUILayout.Label ("Save");
		EditorGUILayout.EndVertical ();

	}

	private string ChangeValue(string fileText, string stringPattern, string valueToEnter){

		int startIndex = fileText.IndexOf(stringPattern) + stringPattern.Length;
		int endIndex = fileText.IndexOf("\"", startIndex);

		fileText = fileText.Insert(startIndex, valueToEnter);
		fileText = fileText.Remove(startIndex + valueToEnter.Length , endIndex - startIndex);

		return fileText;
	}
}

// Simple Editor Script that fills a cancelable bar in the given seconds.

public class WatsonCancelableProgressBar : EditorWindow {
	int secs = 10;
	float startVal = 0f;
	float progress = 0f;
	
//	[MenuItem("Examples/Cancelable Progress Bar Usage")]
//	static void Init() {
//		EditorWindow window =  GetWindow<WatsonCancelableProgressBar>();
//		window.Show();
//	}
	
	void OnGUI() {
		secs = EditorGUILayout.IntField("Time to wait:", secs);
		if(GUILayout.Button("Display bar")) {
			if(secs < 1) {
				Debug.LogError("Seconds should be bigger than 1");
				return;
			}
			startVal = (float)EditorApplication.timeSinceStartup;
		}
		if(progress < secs) {
			if(EditorUtility.DisplayCancelableProgressBar(
				"Simple Progress Bar",
				"Shows a progress bar for the given seconds",
				progress/secs)) {
				Debug.Log("Progress bar canceled by the user");
				startVal = 0;
			}
		} else {
			EditorUtility.ClearProgressBar();
		}
		progress = (float)EditorApplication.timeSinceStartup - startVal;
	}
	
	void OnInspectorUpdate() {
		Repaint();
	}
}
