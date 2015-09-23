#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;

/// <summary>
/// Watson editor menu. Main class responsible of all Menu items under Watson menu.
/// </summary>
public class WatsonEditorMenu: EditorWindow  {

	[MenuItem("Watson/Service Credentials/Text to Speech", false, 1)]
	private static void OpenMenuOptionTextToSpeechCredentials(){
		TextToSpeechCredentialEditorWindow window = (TextToSpeechCredentialEditorWindow)EditorWindow.GetWindow (typeof (TextToSpeechCredentialEditorWindow));
		window.Show();
	}

	[MenuItem("Watson/Update Watson Unity SDK", false, 99)]	
	private static void UpdateWatsonUnitySDK(){
		//TODO: Implement Update SDK Code in here!
		//EditorWindow window =  GetWindow<WatsonCancelableProgressBar>();
		//window.Show();
		EditorUtility.DisplayDialog("Update Watson Unity SDK",	" - It will be implemented - ", "Ok...", ""); 	
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

#endif
