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
#pragma warning disable 0414

public class ExampleTextToSpeech : MonoBehaviour
{
  TextToSpeech m_TextToSpeech = new TextToSpeech();
  string m_TestString = "<speak version=\"1.0\"><say-as interpret-as=\"letters\">I'm sorry</say-as>. <prosody pitch=\"150Hz\">This is Text to Speech!</prosody></express-as><express-as type=\"GoodNews\">I'm sorry. This is Text to Speech!</express-as></speak>";


  void Start()
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
    //Log.Debug("ExampleTextToSpeech", "Attempting to create a customization");
    //string customizationName = "unity-example-customization";
    //string customizationLanguage = "en-US";
    //string customizationDescription = "A text to speech voice customization created within Unity.";
    //m_TextToSpeech.CreateCustomization(OnCreateCustomization, customizationName, customizationLanguage, customizationDescription);

    //	Delete Customization
    //Log.Debug("ExampleTextToSpeech", "Attempting to delete a customization");
    //string customizationIdentifierToDelete = "1476ea80-5355-4911-ac99-ba39162a2d34";
    //if (!m_TextToSpeech.DeleteCustomization(OnDeleteCustomization, customizationIdentifierToDelete))
    //	Log.Debug("ExampleTextToSpeech", "Failed to delete custom voice model!");

    //	Get Customization
    //Log.Debug("ExampleTextToSpeech", "Attempting to get a customization");
    //string customizationIdentifierToGet = "1476ea80-5355-4911-ac99-ba39162a2d34";
    //if (!m_TextToSpeech.GetCustomization(OnGetCustomization, customizationIdentifierToGet))
    //	Log.Debug("ExampleTextToSpeech", "Failed to get custom voice model!");

    //	Update Customization
    Log.Debug("ExampleTextToSpeech", "Attempting to update a customization");
    Word word0 = new Word();
    word0.word = "hello";
    word0.translation = "hullo";
    Word word1 = new Word();
    word1.word = "goodbye";
    word1.translation = "gbye";
    Word word2 = new Word();
    word2.word = "hi";
    word2.translation = "ohiooo";
    Word[] words = { word0, word1, word2 };
    CustomVoiceUpdate customVoiceUpdate = new CustomVoiceUpdate();
    customVoiceUpdate.words = words;
    customVoiceUpdate.description = "My updated description";
    customVoiceUpdate.name = "My updated name";
    string customizationIdToUpdate = "1476ea80-5355-4911-ac99-ba39162a2d34";
    if (!m_TextToSpeech.UpdateCustomization(OnUpdateCustomization, customizationIdToUpdate, customVoiceUpdate))
      Log.Debug("ExampleTextToSpeech", "Failed to update customization!");

    //	Get Customization Words
    //Log.Debug("ExampleTextToSpeech", "Attempting to get a customization's words");
    //string customIdentifierToGetWords = "1476ea80-5355-4911-ac99-ba39162a2d34";
    //if (!m_TextToSpeech.GetCustomizationWords(OnGetCustomizationWords, customIdentifierToGetWords))
    //	Log.Debug("ExampleTextToSpeech", "Failed to get {0} words!", customIdentifierToGetWords);

    //	Add Customization Words
    //Log.Debug("ExampleTextToSpeech", "Attempting to add words to a customization");
    //string customIdentifierToAddWords = "1476ea80-5355-4911-ac99-ba39162a2d34";
    //Words words = new Words();
    //Word word0 = new Word();
    //word0.word = "bananna";
    //word0.translation = "bunanna";
    //Word word1 = new Word();
    //word1.word = "orange";
    //word1.translation = "arange";
    //Word word2 = new Word();
    //word2.word = "tomato";
    //word2.translation = "tomahto";
    //Word[] wordArray = { word0, word1, word2 };
    //words.words = wordArray;
    //if (!m_TextToSpeech.AddCustomizationWords(OnAddCustomizationWords, customIdentifierToAddWords, words))
    //	Log.Debug("ExampleTextToSpeech", "Failed to add words to {0}!", customIdentifierToAddWords);

    //	Delete Customization Word
    //Log.Debug("ExampleTextToSpeech", "Attempting to delete customization word from custom voice model.");
    //string customIdentifierToDeleteWord = "1476ea80-5355-4911-ac99-ba39162a2d34";
    //string wordToDelete = "goodbye";
    //if (!m_TextToSpeech.DeleteCustomizationWord(OnDeleteCustomizationWord, customIdentifierToDeleteWord, wordToDelete))
    //	Log.Debug("ExampleTextToSpeech", "Failed to delete {0} from {1}!", wordToDelete, customIdentifierToDeleteWord);

    //	Get Customization Word
    //Log.Debug("ExampleTextToSpeech", "Attempting to get the translation of a custom voice model's word.");
    //string customIdentifierToGetWord = "1476ea80-5355-4911-ac99-ba39162a2d34";
    //string customIdentifierWord = "hello";
    //if (!m_TextToSpeech.GetCustomizationWord(OnGetCustomizationWord, customIdentifierToGetWord, customIdentifierWord))
    //	Log.Debug("ExampleTextToSpeech", "Failed to get the translation of {0} from {1}!", customIdentifierWord, customIdentifierToGetWord);

    //	Add Customization Word - This is not working. The PUT method is not supported by Unity.
    //Log.Debug("ExampleTextToSpeech", "Attempting to add a single word and translation to a custom voice model.");
    //string customIdentifierToAddWordAndTranslation = "1476ea80-5355-4911-ac99-ba39162a2d34";
    //string word = "grasshopper";
    //string translation = "guhrasshoppe";
    //if (!m_TextToSpeech.AddCustomizationWord(OnAddCustomizationWord, customIdentifierToAddWordAndTranslation, word, translation))
    //	Log.Debug("ExampleTextToSpeech", "Failed to add {0}/{1} to {2}!", word, translation, customIdentifierToAddWordAndTranslation);


    //m_TextToSpeech.Voice = VoiceType.en_US_Allison;
    //m_TextToSpeech.ToSpeech(m_TestString, HandleToSpeechCallback, true);

  }

  void HandleToSpeechCallback(AudioClip clip)
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
    Log.Debug("ExampleTextToSpeech", "-----OnGetVoices-----");
  }

  private void OnGetVoice(Voice voice)
  {
    Log.Debug("ExampleTextToSpeech", "-----OnGetVoice-----");
    Log.Debug("ExampleTextToSpeech", "Voice | name: {0} | gender: {1} | language: {2} | customizable: {3} | description: {4}", voice.name, voice.gender, voice.language, voice.customizable, voice.description);
    Log.Debug("ExampleTextToSpeech", "-----OnGetVoice-----");
  }

  private void OnGetPronunciation(Pronunciation pronunciation)
  {
    Log.Debug("ExampleTextToSpeech", "-----OnGetPronunciation-----");
    Log.Debug("ExampleTextToSpeech", "Pronunciation: {0}.", pronunciation.pronunciation);
    Log.Debug("ExampleTextToSpeech", "-----OnGetPronunciation-----");
  }

  private void OnGetCustomizations(Customizations customizations, string data)
  {
    Log.Debug("ExampleTextToSpeech", "-----OnGetCustomizations-----");
    if (data != default(string))
      Log.Debug("ExampleTextToSpeech", "data: {0}", data);
    foreach (Customization customization in customizations.customizations)
      Log.Debug("ExampleTextToSpeech", "Customization: name: {0} | customization_id: {1} | language: {2} | description: {3} | owner: {4} | created: {5} | last modified: {6}", customization.name, customization.customization_id, customization.language, customization.description, customization.owner, customization.created, customization.last_modified);
    Log.Debug("ExampleTextToSpeech", "-----OnGetCustomizations-----");
  }

  private void OnCreateCustomization(CustomizationID customizationID, string data)
  {
    Log.Debug("ExampleTextToSpeech", "-----OnCreateCustomization-----");
    if (data != default(string))
      Log.Debug("ExampleTextToSpeech", "data: {0}", data);
    Log.Debug("ExampleTextToSpeech", "CustomizationID: id: {0}.", customizationID.customization_id);
    Log.Debug("ExampleTextToSpeech", "-----OnCreateCustomization-----");
  }

  private void OnDeleteCustomization(bool success, string data)
  {
    Log.Debug("ExampleTextToSpeech", "-----OnDeleteCustomization-----");
    if (data != default(string))
      Log.Debug("ExampleTextToSpeech", "data: {0}", data);
    Log.Debug("ExampleTextToSpeech", "Success: {0}.", success);
    Log.Debug("ExampleTextToSpeech", "-----OnDeleteCustomization-----");
  }

  private void OnGetCustomization(Customization customization, string data)
  {
    Log.Debug("ExampleTextToSpeech", "-----OnGetCustomization-----");
    if (data != default(string))
      Log.Debug("ExampleTextToSpeech", "data: {0}", data);
    Log.Debug("ExampleTextToSpeech", "Customization: name: {0} | customization_id: {1} | language: {2} | description: {3} | owner: {4} | created: {5} | last modified: {6}", customization.name, customization.customization_id, customization.language, customization.description, customization.owner, customization.created, customization.last_modified);
    Log.Debug("ExampleTextToSpeech", "-----OnGetCustomization-----");
  }

  private void OnUpdateCustomization(bool success, string data)
  {
    Log.Debug("ExampleTextToSpeech", "-----OnUpdateCustomization-----");
    if (data != default(string))
      Log.Debug("ExampleTextToSpeech", "data: {0}", data);
    Log.Debug("ExampleTextToSpeech", "Success: {0}.", success);
    Log.Debug("ExampleTextToSpeech", "-----OnUpdateCustomization-----");
  }

  private void OnGetCustomizationWords(Words words, string data)
  {
    Log.Debug("ExampleTextToSpeech", "-----OnGetCustomizationWords-----");
    if (data != default(string))
      Log.Debug("ExampleTextToSpeech", "data: {0}", data);
    foreach (Word word in words.words)
      Log.Debug("ExampleTextToSpeech", "Word: {0} | Translation: {1}.", word.word, word.translation);
    Log.Debug("ExampleTextToSpeech", "-----OnGetCustomizationWords-----");
  }

  private void OnAddCustomizationWords(bool success, string data)
  {
    Log.Debug("ExampleTextToSpeech", "-----OnAddCustomizationWords-----");
    if (data != default(string))
      Log.Debug("ExampleTextToSpeech", "data: {0}", data);
    Log.Debug("ExampleTextToSpeech", "Success: {0}.", success);
    Log.Debug("ExampleTextToSpeech", "-----OnAddCustomizationWords-----");
  }

  private void OnDeleteCustomizationWord(bool success, string data)
  {
    Log.Debug("ExampleTextToSpeech", "-----OnDeleteCustomizationWord-----");
    if (data != default(string))
      Log.Debug("ExampleTextToSpeech", "data: {0}", data);
    Log.Debug("ExampleTextToSpeech", "Success: {0}.", success);
    Log.Debug("ExampleTextToSpeech", "-----OnDeleteCustomizationWord-----");
  }

  private void OnGetCustomizationWord(Translation translation, string data)
  {
    Log.Debug("ExampleTextToSpeech", "-----OnGetCustomizationWord-----");
    if (data != default(string))
      Log.Debug("ExampleTextToSpeech", "data: {0}", data);
    Log.Debug("ExampleTextToSpeech", "Translation: {0}.", translation.translation);
    Log.Debug("ExampleTextToSpeech", "-----OnGetCustomizationWord-----");
  }

  private void OnAddCustomizationWord(bool success, string data)
  {
    Log.Debug("ExampleTextToSpeech", "-----OnAddCustomizationWord-----");
    if (data != default(string))
      Log.Debug("ExampleTextToSpeech", "data: {0}", data);
    Log.Debug("ExampleTextToSpeech", "Success: {0}.", success);
    Log.Debug("ExampleTextToSpeech", "-----OnAddCustomizationWord-----");
  }
}
