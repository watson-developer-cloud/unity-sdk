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
using System.Collections;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;

#pragma warning disable 0414
namespace IBM.Watson.DeveloperCloud.UnitTests
{
  public class TestTextToSpeech : UnitTest
  {
    private TextToSpeech m_TextToSpeech = new TextToSpeech();
    private bool m_GetTested = false;
    private bool m_PostTested = false;
    private bool m_GetVoicesTested = false;
    private bool m_GetVoiceTested = false;
    private bool m_GetPronunciationTested = false;
    private bool m_GetCustomizationsTested = false;
    private bool m_CreateCustomizationTested = false;
    private bool m_DeleteCustomizationTested = false;
    private bool m_GetCustomizationTested = false;
    private bool m_UpdateCustomizationTested = false;
    private bool m_GetCustomizationWordsTested = false;
    private bool m_AddCustomizationWordsTested = false;
    private bool m_GetCustomizationWordTested = false;
    private bool m_DeleteCustomizationWordTested = false;

    private string m_CustomizationIDToTest;
    private string m_CustomizationToCreateName = "unity-integration-test-created-customization";
    private string m_CustomizationToCreateLanguage = "en-US";
    private string m_CustomizationToCreateDescription = "A text to speech voice customization created within Unity.";
    private string m_UpdateWord0 = "hello";
    private string m_UpdateWord1 = "goodbye";
    private string m_UpdateTranslation0 = "hullo";
    private string m_UpdateTranslation1 = "gbye";
    private Word m_UpdateWordObject0 = new Word();
    private Word m_UpdateWordObject1 = new Word();
    private string m_CustomizationIdCreated;

    public override IEnumerator RunTest()
    {
      m_CustomizationIDToTest = Config.Instance.GetVariableValue("TextToSpeech_IntegrationTestCustomVoiceModel");
      m_UpdateWordObject0.word = m_UpdateWord0;
      m_UpdateWordObject0.translation = m_UpdateTranslation0;
      m_UpdateWordObject1.word = m_UpdateWord1;
      m_UpdateWordObject1.translation = m_UpdateTranslation1;

      if (Config.Instance.FindCredentials(m_TextToSpeech.GetServiceID()) == null)
        yield break;

      // Test GET
      m_TextToSpeech.ToSpeech("Hello World using GET", OnSpeechGET);
      while (!m_GetTested)
        yield return null;

      // Test POST
      m_TextToSpeech.ToSpeech("Hello World using POST", OnSpeechPOST, true);
      while (!m_PostTested)
        yield return null;

      //	Get Pronunciation
      string testWord = "Watson";
      Log.Debug("ExampleTextToSpeech", "Attempting to get pronunciation of {0}", testWord);
      m_TextToSpeech.GetPronunciation(OnGetPronunciation, testWord, VoiceType.en_US_Allison);
      while (!m_GetPronunciationTested)
        yield return null;

      //  Get Customizations
      Log.Debug("ExampleTextToSpeech", "Attempting to get a list of customizations");
      m_TextToSpeech.GetCustomizations(OnGetCustomizations);
      while (!m_GetCustomizationsTested)
        yield return null;

      //  Create Customization
      //Log.Debug("ExampleTextToSpeech", "Attempting to create a customization");
      //m_TextToSpeech.CreateCustomization(OnCreateCustomization, m_CustomizationToCreateName, m_CustomizationToCreateLanguage, m_CustomizationToCreateDescription);
      //while (!m_CreateCustomizationTested)
      //    yield return null;

      //  Get Customization
      Log.Debug("ExampleTextToSpeech", "Attempting to get a customization");
      if (!m_TextToSpeech.GetCustomization(OnGetCustomization, m_CustomizationIDToTest))
        Log.Debug("ExampleTextToSpeech", "Failed to get custom voice model!");
      while (!m_GetCustomizationTested)
        yield return null;

      //  Update Customization
      Log.Debug("ExampleTextToSpeech", "Attempting to update a customization");
      Word[] words = { m_UpdateWordObject0 };
      CustomVoiceUpdate customVoiceUpdate = new CustomVoiceUpdate();
      customVoiceUpdate.words = words;
      if (!m_TextToSpeech.UpdateCustomization(OnUpdateCustomization, m_CustomizationIDToTest, customVoiceUpdate))
        Log.Debug("ExampleTextToSpeech", "Failed to update customization!");
      while (!m_UpdateCustomizationTested)
        yield return null;

      //  Get Customization Words
      Log.Debug("ExampleTextToSpeech", "Attempting to get a customization's words");
      if (!m_TextToSpeech.GetCustomizationWords(OnGetCustomizationWords, m_CustomizationIDToTest))
        Log.Debug("ExampleTextToSpeech", "Failed to get {0} words!", m_CustomizationIDToTest);
      while (!m_GetCustomizationWordsTested)
        yield return null;

      //  Add Customization Words
      Log.Debug("ExampleTextToSpeech", "Attempting to add words to a customization");
      Word[] wordArray = { m_UpdateWordObject1 };
      Words wordsObject = new Words();
      wordsObject.words = wordArray;
      if (!m_TextToSpeech.AddCustomizationWords(OnAddCustomizationWords, m_CustomizationIDToTest, wordsObject))
        Log.Debug("ExampleTextToSpeech", "Failed to add words to {0}!", wordsObject);
      while (!m_AddCustomizationWordsTested)
        yield return null;

      ////  Get Customization Word
      Log.Debug("ExampleTextToSpeech", "Attempting to get the translation of a custom voice model's word.");
      if (!m_TextToSpeech.GetCustomizationWord(OnGetCustomizationWord, m_CustomizationIDToTest, m_UpdateWord1))
        Log.Debug("ExampleTextToSpeech", "Failed to get the translation of {0} from {1}!", m_UpdateWord0, m_CustomizationIDToTest);
      while (!m_GetCustomizationWordTested)
        yield return null;

      //Delete Customization Word
      //Log.Debug("ExampleTextToSpeech", "Attempting to delete customization word from custom voice model.");
      //if (!m_TextToSpeech.DeleteCustomizationWord(OnDeleteCustomizationWord, m_CustomizationIdCreated, m_UpdateWord1))
      //    Log.Debug("ExampleTextToSpeech", "Failed to delete {0} from {1}!", m_UpdateWord1, m_CustomizationIdCreated);
      //while (!m_DeleteCustomizationWordTested)
      //    yield return null;

      //  Delete Customization
      //Log.Debug("ExampleTextToSpeech", "Attempting to delete a customization");
      //if (!m_TextToSpeech.DeleteCustomization(OnDeleteCustomization, m_CustomizationIdCreated))
      //    Log.Debug("ExampleTextToSpeech", "Failed to delete custom voice model!");
      //while (!m_DeleteCustomizationTested)
      //    yield return null;

      yield break;
    }

    private void OnSpeechGET(AudioClip clip)
    {
      Log.Debug("TestTestToSpeech", "OnSpeechGET invoked.");

      Test(clip != null);
      m_GetTested = true;

      PlayClip(clip);
    }

    private void OnSpeechPOST(AudioClip clip)
    {
      Log.Debug("TestTestToSpeech", "OnSpechPOST invoked.");

      Test(clip != null);
      m_PostTested = true;

      PlayClip(clip);
    }

    private void PlayClip(AudioClip clip)
    {
      if (Application.isPlaying && clip != null)
      {
        GameObject audioObject = new GameObject("AudioObject");
        AudioSource source = audioObject.AddComponent<AudioSource>();
        source.spatialBlend = 0.0f;     // 2D sound
        source.loop = false;            // do not loop
        source.clip = clip;             // clip
        source.Play();

        // automatically destroy the object after the sound has played..
        GameObject.Destroy(audioObject, clip.length);
      }
    }

    private void OnGetVoices(Voices voices)
    {
      Log.Debug("ExampleTextToSpeech", "-----OnGetVoices-----");
      foreach (Voice voice in voices.voices)
        Log.Debug("ExampleTextToSpeech", "Voice | name: {0} | gender: {1} | language: {2} | customizable: {3} | description: {4}.", voice.name, voice.gender, voice.language, voice.customizable, voice.description);
      Log.Debug("ExampleTextToSpeech", "-----OnGetVoices-----");

      Test(voices.HasData());
      m_GetVoicesTested = true;
    }

    private void OnGetVoice(Voice voice)
    {
      Log.Debug("ExampleTextToSpeech", "-----OnGetVoice-----");
      Log.Debug("ExampleTextToSpeech", "Voice | name: {0} | gender: {1} | language: {2} | customizable: {3} | description: {4}", voice.name, voice.gender, voice.language, voice.customizable, voice.description);
      Log.Debug("ExampleTextToSpeech", "-----OnGetVoice-----");

      Test(!string.IsNullOrEmpty(voice.name));
      m_GetVoiceTested = true;
    }

    private void OnGetPronunciation(Pronunciation pronunciation)
    {
      Log.Debug("ExampleTextToSpeech", "-----OnGetPronunciation-----");
      Log.Debug("ExampleTextToSpeech", "Pronunciation: {0}.", pronunciation.pronunciation);
      Log.Debug("ExampleTextToSpeech", "-----OnGetPronunciation-----");

      Test(!string.IsNullOrEmpty(pronunciation.pronunciation));
      m_GetPronunciationTested = true;
    }

    private void OnGetCustomizations(Customizations customizations, string data)
    {
      Log.Debug("ExampleTextToSpeech", "-----OnGetCustomizations-----");
      if (data != default(string))
        Log.Debug("ExampleTextToSpeech", "data: {0}", data);
      foreach (Customization customization in customizations.customizations)
        Log.Debug("ExampleTextToSpeech", "Customization: name: {0} | customization_id: {1} | language: {2} | description: {3} | owner: {4} | created: {5} | last modified: {6}", customization.name, customization.customization_id, customization.language, customization.description, customization.owner, customization.created, customization.last_modified);
      Log.Debug("ExampleTextToSpeech", "-----OnGetCustomizations-----");

      Test(customizations.HasData());
      m_GetCustomizationsTested = true;
    }

    private void OnCreateCustomization(CustomizationID customizationID, string data)
    {
      Log.Debug("ExampleTextToSpeech", "-----OnCreateCustomization-----");
      if (data != default(string))
        Log.Debug("ExampleTextToSpeech", "data: {0}", data);
      Log.Debug("ExampleTextToSpeech", "CustomizationID: id: {0}.", customizationID.customization_id);
      Log.Debug("ExampleTextToSpeech", "-----OnCreateCustomization-----");

      m_CustomizationIdCreated = customizationID.customization_id;

      Test(!string.IsNullOrEmpty(customizationID.customization_id));
      m_CreateCustomizationTested = true;
    }

    private void OnDeleteCustomization(bool success, string data)
    {
      Log.Debug("ExampleTextToSpeech", "-----OnDeleteCustomization-----");
      if (data != default(string))
        Log.Debug("ExampleTextToSpeech", "data: {0}", data);
      Log.Debug("ExampleTextToSpeech", "Success: {0}.", success);
      Log.Debug("ExampleTextToSpeech", "-----OnDeleteCustomization-----");

      Test(success);
      m_DeleteCustomizationTested = true;
    }

    private void OnGetCustomization(Customization customization, string data)
    {
      Log.Debug("ExampleTextToSpeech", "-----OnGetCustomization-----");
      if (data != default(string))
        Log.Debug("ExampleTextToSpeech", "data: {0}", data);
      Log.Debug("ExampleTextToSpeech", "Customization: name: {0} | customization_id: {1} | language: {2} | description: {3} | owner: {4} | created: {5} | last modified: {6}", customization.name, customization.customization_id, customization.language, customization.description, customization.owner, customization.created, customization.last_modified);
      Log.Debug("ExampleTextToSpeech", "-----OnGetCustomization-----");

      Test(customization.HasData());
      m_GetCustomizationTested = true;
    }

    private void OnUpdateCustomization(bool success, string data)
    {
      Log.Debug("ExampleTextToSpeech", "-----OnUpdateCustomization-----");
      if (data != default(string))
        Log.Debug("ExampleTextToSpeech", "data: {0}", data);
      Log.Debug("ExampleTextToSpeech", "Success: {0}.", success);
      Log.Debug("ExampleTextToSpeech", "-----OnUpdateCustomization-----");

      Test(success);
      m_UpdateCustomizationTested = true;
    }

    private void OnGetCustomizationWords(Words words, string data)
    {
      Log.Debug("ExampleTextToSpeech", "-----OnGetCustomizationWords-----");
      if (data != default(string))
        Log.Debug("ExampleTextToSpeech", "data: {0}", data);
      foreach (Word word in words.words)
        Log.Debug("ExampleTextToSpeech", "Word: {0} | Translation: {1}.", word.word, word.translation);
      Log.Debug("ExampleTextToSpeech", "-----OnGetCustomizationWords-----");

      Test(words.HasData());
      m_GetCustomizationWordsTested = true;
    }

    private void OnAddCustomizationWords(bool success, string data)
    {
      Log.Debug("ExampleTextToSpeech", "-----OnAddCustomizationWords-----");
      if (data != default(string))
        Log.Debug("ExampleTextToSpeech", "data: {0}", data);
      Log.Debug("ExampleTextToSpeech", "Success: {0}.", success);
      Log.Debug("ExampleTextToSpeech", "-----OnAddCustomizationWords-----");

      Test(success);
      m_AddCustomizationWordsTested = true;
    }

    private void OnDeleteCustomizationWord(bool success, string data)
    {
      Log.Debug("ExampleTextToSpeech", "-----OnDeleteCustomizationWord-----");
      if (data != default(string))
        Log.Debug("ExampleTextToSpeech", "data: {0}", data);
      Log.Debug("ExampleTextToSpeech", "Success: {0}.", success);
      Log.Debug("ExampleTextToSpeech", "-----OnDeleteCustomizationWord-----");

      Test(success);
      m_DeleteCustomizationWordTested = true;
    }

    private void OnGetCustomizationWord(Translation translation, string data)
    {
      Log.Debug("ExampleTextToSpeech", "-----OnGetCustomizationWord-----");
      if (data != default(string))
        Log.Debug("ExampleTextToSpeech", "data: {0}", data);
      Log.Debug("ExampleTextToSpeech", "Translation: {0}.", translation.translation);
      Log.Debug("ExampleTextToSpeech", "-----OnGetCustomizationWord-----");

      Test(!string.IsNullOrEmpty(translation.translation));
      m_GetCustomizationWordTested = true;
    }
  }
}
