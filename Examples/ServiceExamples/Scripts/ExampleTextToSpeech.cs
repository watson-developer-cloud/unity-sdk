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
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using IBM.Watson.DeveloperCloud.Logging;

public class ExampleTextToSpeech : MonoBehaviour
{
	TextToSpeech m_TextToSpeech = new TextToSpeech();
	string m_TestString = "<speak version=\"1.0\"><say-as interpret-as=\"letters\">I'm sorry</say-as>. <prosody pitch=\"150Hz\">This is Text to Speech!</prosody></express-as><express-as type=\"GoodNews\">I'm sorry. This is Text to Speech!</express-as></speak>";


    void Start ()
	{
		LogSystem.InstallDefaultReactors();

		////	Get Voices
		//Log.Debug("ExampleTextToSpeech", "Attempting to get voices.");
		//m_TextToSpeech.GetVoices(OnGetVoices);

		////	Get Voice
		////string selectedVoice = "en-US_AllisonVoice";
		//Log.Debug("ExampleTextToSpeech", "Attempting to get voice {0}.", VoiceType.en_US_Allison);
		//m_TextToSpeech.GetVoice(OnGetVoice, VoiceType.en_US_Allison);

		////	Get Pronunciation
		//string testWord = "Watson";
		//Log.Debug("ExampleTextToSpeech", "Attempting to get pronunciation of {0}", testWord);
		//m_TextToSpeech.GetPronunciation(OnGetPronunciation, testWord, VoiceType.en_US_Allison);

		// Get Customizations
		//Log.Debug("ExampleTextToSpeech", "Attempting to get a list of customizations");
		//m_TextToSpeech.GetCustomizations(OnGetCustomizations);

		//	Create Customization
		Log.Debug("ExampleTextToSpeech", "Attempting to create a customization");
		string customizationName = "unity-example-customization";
		string customizationLanguage = "en-US";
		string customizationDescription = "A text to speech voice customization created within Unity.";
		m_TextToSpeech.CreateCustomization(OnCreateCustomization, customizationName, customizationLanguage, customizationDescription);


		//m_TextToSpeech.Voice = VoiceType.en_US_Allison;
		//m_TextToSpeech.ToSpeech(m_TestString, HandleToSpeechCallback, true);

	}

	void HandleToSpeechCallback (AudioClip clip)
	{
		PlayClip(clip);
	}

	private void PlayClip(AudioClip clip)
	{
		if (Application.isPlaying && clip != null)
		{
			GameObject audioObject = new GameObject("AudioObject");
			AudioSource source = audioObject.AddComponent<AudioSource>();
			source.spatialBlend = 0.0f;
			source.loop = false;
			source.clip = clip;
			source.Play();

			GameObject.Destroy(audioObject, clip.length);
		}
	}

	private void OnGetVoices(Voices voices)
	{
		Log.Debug("ExampleTextToSpeech", "-----OnGetVoices-----");
		foreach (Voice voice in voices.voices)
			Log.Debug("ExampleTextToSpeech", "Voice | name: {0} | gender: {1} | language: {2} | customizable: {3} | description: {4}.", voice.name, voice.gender, voice.language, voice.customizable, voice.description);
		Log.Debug("ExampleTextToSpeech", "\n");
	}

	private void OnGetVoice(Voice voice)
	{
		Log.Debug("ExampleTextToSpeech", "-----OnGetVoice-----");
		Log.Debug("ExampleTextToSpeech", "Voice | name: {0} | gender: {1} | language: {2} | customizable: {3} | description: {4}", voice.name, voice.gender, voice.language, voice.customizable, voice.description);
		Log.Debug("ExampleTextToSpeech", "\n");
	}

	private void OnGetPronunciation(Pronunciation pronunciation)
	{
		Log.Debug("ExampleTextToSpeech", "-----OnGetPronunciation-----");
		Log.Debug("ExampleTextToSpeech", "Pronunciation: {0}.", pronunciation.pronunciation);
		Log.Debug("ExampleTextToSpeech", "\n");
	}

	private void OnGetCustomizations(Customizations customizations, string data)
	{
		Log.Debug("ExampleTextToSpeech", "-----OnGetCustomizations-----");
		if(data != default(string))
			Log.Debug("ExampleTextToSpeech", "data: {0}", data);
		foreach (Customization customization in customizations.customizations)
			Log.Debug("ExampleTextToSpeech", "Customization: name: {0} | customization_id: {1} | language: {2} | description: {3} | owner: {4} | created: {5} | last modified: {6}", customization.name, customization.customization_id, customization.language, customization.description, customization.owner, customization.created, customization.last_modified);
		Log.Debug("ExampleTextToSpeech", "\n");
	}

	private void OnCreateCustomization(CustomizationID customizationID, string data)
	{
		Log.Debug("ExampleTextToSpeech", "-----OnCreateCustomization-----");
		if (data != default(string))
			Log.Debug("ExampleTextToSpeech", "data: {0}", data);
		Log.Debug("ExampleTextToSpeech", "CustomizationID: id: {0}.", customizationID.customization_id);
		Log.Debug("ExampleTextToSpeech", "\n");
	}
}
