using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;


/// <summary>
/// Watson service credential editor window.
/// For each services there is another credentials to use.
/// </summary>
public abstract class WatsonServiceCredentialEditorWindow : EditorWindow {
	
	protected string serviceName;
	protected string defaultUrlToCall;
	protected string urlToCall;
	protected string credentialUserName;
	protected string credentialPassword;
	protected string defaultVersionNumber;
	protected string versionNumber;
	
	public System.Type serviceComponentType;
	
	bool isSaved = false;
	float timeOutForSaveResultLabel = 2.0f;
	float timeElapsedForSaveResultLabel = 0.0f;
	float timeAtSavedButtonPressed = 0.0f;
	
	string editorPrefsServiceURL;
	string editorPrefsServiceUserName;
	string editorPrefsServicePassword;
	string editorPrefsServiceVersion;
	
	public virtual void OnEnable(){
		
		if (string.IsNullOrEmpty (serviceName)) {
			Debug.LogError("Editor Windows - Service name shouldn't be empty. Switching to use of default naming");
			serviceName = "Default";
		}
		
		editorPrefsServiceURL = string.Format("Watson{0}CredentialURL", serviceName);
		editorPrefsServiceUserName = string.Format("Watson{0}CredentialUserName", serviceName);
		editorPrefsServicePassword = string.Format("Watson{0}CredentialPassword", serviceName);
		editorPrefsServiceVersion = string.Format("Watson{0}CredentialVersion", serviceName);
		
		urlToCall = 			EditorPrefs.GetString(editorPrefsServiceURL, 		defaultUrlToCall);
		credentialUserName = 	EditorPrefs.GetString(editorPrefsServiceUserName, 	"");
		credentialPassword = 	EditorPrefs.GetString(editorPrefsServicePassword, 	"");
		versionNumber = 		EditorPrefs.GetString(editorPrefsServiceVersion, 	defaultVersionNumber);
	}
	
	void OnGUI() {
		urlToCall = EditorGUILayout.TextField("Service URL:", urlToCall);
		versionNumber = EditorGUILayout.TextField("Version Number: ", versionNumber);
		credentialUserName = EditorGUILayout.TextField("User Name:", credentialUserName);
		credentialPassword = EditorGUILayout.TextField("Password:", credentialPassword);
		
		Rect r = EditorGUILayout.BeginVertical ("Button");
		if(GUI.Button(r, GUIContent.none)){
			//Finding the related Service Script in project to change it.
			GameObject gameObjectToFindServiceElement = new GameObject();
			gameObjectToFindServiceElement.name = "~ Watson Editor To Be Deleted";
			MonoBehaviour wsTextToSpeech = (MonoBehaviour) gameObjectToFindServiceElement.AddComponent(serviceComponentType);
			MonoScript ms = MonoScript.FromMonoBehaviour(wsTextToSpeech);
			string m_ScriptFilePath = AssetDatabase.GetAssetPath( ms );
			
			//Get the Script Text to change it!
			string fileText = File.ReadAllText( m_ScriptFilePath);
			fileText = ChangeValue(fileText,  "_serviceURL = \"", urlToCall);
			fileText = ChangeValue(fileText,  "_serviceCredentialUserName = \"", credentialUserName);
			fileText = ChangeValue(fileText,  "_serviceCredentialPassword = \"", credentialPassword);
			fileText = ChangeValue(fileText,  "_serviceVersion = \"", versionNumber);
			File.WriteAllText(m_ScriptFilePath, fileText);
			
			GameObject.DestroyImmediate(gameObjectToFindServiceElement);
			
			EditorPrefs.SetString(editorPrefsServiceURL, 		urlToCall);
			EditorPrefs.SetString(editorPrefsServiceUserName, 	credentialUserName);
			EditorPrefs.SetString(editorPrefsServicePassword, 	credentialPassword);
			EditorPrefs.SetString(editorPrefsServiceVersion, 	versionNumber);
			
			isSaved = true;
			timeAtSavedButtonPressed = Time.realtimeSinceStartup;
			timeElapsedForSaveResultLabel = timeOutForSaveResultLabel;
		}
		GUILayout.Label ("Save");
		EditorGUILayout.EndVertical ();
		
		if (isSaved && timeElapsedForSaveResultLabel > 0.0f) {
			timeElapsedForSaveResultLabel -= (Time.realtimeSinceStartup - timeAtSavedButtonPressed);
			GUILayout.Label ("Saved Credentials! ");
		}
		
	}

	/// <summary>
	/// Changes the credential values from script text.
	/// </summary>
	/// <returns>The value.</returns>
	/// <param name="fileText">File text.</param>
	/// <param name="stringPattern">String pattern.</param>
	/// <param name="valueToEnter">Value to enter.</param>
	private string ChangeValue(string fileText, string stringPattern, string valueToEnter){
		
		int startIndex = fileText.IndexOf(stringPattern) + stringPattern.Length;
		int endIndex = fileText.IndexOf("\"", startIndex);
		
		fileText = fileText.Insert(startIndex, valueToEnter);
		fileText = fileText.Remove(startIndex + valueToEnter.Length , endIndex - startIndex);
		
		return fileText;
	}
	
}

public class TextToSpeechCredentialEditorWindow : WatsonServiceCredentialEditorWindow{
	public override void OnEnable(){
		serviceName = "TextToSpeech";
		defaultUrlToCall = "https://stream.watsonplatform.net/text-to-speech/api";
		defaultVersionNumber = "v1";
		serviceComponentType = typeof(WSTextToSpeech);
		base.OnEnable ();
	}
}
