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
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Logging;
#pragma warning disable 0414

public class ExampleSpeechToText : MonoBehaviour
{
  [SerializeField]
  private AudioClip m_AudioClip = new AudioClip();
  private SpeechToText m_SpeechToText = new SpeechToText();

  private string m_CreatedCustomizationID;
  private string m_CreatedCorpusName = "unity-corpus";
  private string m_CustomCorpusFilePath;

  void Start()
  {
    //m_SpeechToText.Recognize(m_AudioClip, HandleOnRecognize);
    LogSystem.InstallDefaultReactors();

    m_CustomCorpusFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/test-stt-corpus.txt";

    //	test GetModels and GetModel
    //TestGetModels();

    //	test CreateSession
    //TestCreateSession("en-US_BroadbandModel");

    //	test GetCustomizations
    //TestGetCustomizations();

    //	test create, get and delete customizations
    //TestCreateCustomization();
  }

  private void TestGetModels()
  {
    Log.Debug("ExampleSpeechToText", "Attempting to get models");
    m_SpeechToText.GetModels(HandleGetModels);
  }

  private void TestGetModel(string modelID)
  {
    Log.Debug("ExampleSpeechToText", "Attempting to get model {0}", modelID);
    m_SpeechToText.GetModel(HandleGetModel, modelID);
  }

  private void TestGetCustomizations()
  {
    Log.Debug("ExampleSpeechToText", "Attempting to get customizations");
    m_SpeechToText.GetCustomizations(HandleGetCustomizations);
  }

  private void TestCreateCustomization()
  {
    Log.Debug("ExampleSpeechToText", "Attempting create customization");
    m_SpeechToText.CreateCustomization(HandleCreateCustomization, "unity-test-customization", "en-US_BroadbandModel", "Testing customization unity");
  }

  private void TestDeleteCustomization(string customizationID)
  {
    Log.Debug("ExampleSpeechToText", "Attempting to delete customization {0}", customizationID);
    m_SpeechToText.DeleteCustomization(HandleDeleteCustomization, customizationID);
  }

  private void TestGetCustomization(string customizationID)
  {
    Log.Debug("ExampleSpeechToText", "Attempting to get customization {0}", customizationID);
    m_SpeechToText.GetCustomization(HandleGetCustomization, customizationID);
  }

  private void TestTrainCustomization(string customizationID)
  {
    Log.Debug("ExampleSpeechToText", "Attempting to train customization {0}", customizationID);
    m_SpeechToText.TrainCustomization(HandleTrainCustomization, customizationID);
  }

  private void TestUpgradeCustomization(string customizationID)
  {
    Log.Debug("ExampleSpeechToText", "Attempting to upgrade customization {0}", customizationID);
    m_SpeechToText.UpgradeCustomization(HandleUpgradeCustomization, customizationID);
  }

  private void TestResetCustomization(string customizationID)
  {
    Log.Debug("ExampleSpeechToText", "Attempting to reset customization {0}", customizationID);
    m_SpeechToText.ResetCustomization(HandleResetCustomization, customizationID);
  }

  private void TestGetCustomCorpora(string customizationID)
  {
    Log.Debug("ExampleSpeechToText", "Attempting to get custom corpora for {0}", customizationID);
    m_SpeechToText.GetCustomCorpora(HandleGetCustomCorpora, customizationID);
  }

  private void TestDeleteCustomCorpus(string customizationID, string corpusName)
  {
    Log.Debug("ExampleSpeechToText", "Attempting to delete custom corpus {1} in customization {0}", customizationID, corpusName);
    m_SpeechToText.DeleteCustomCorpus(HandleDeleteCustomCorpus, customizationID, corpusName);
  }

  private void TestAddCustomCorpus(string customizationID, string corpusName, bool allowOverwrite, string trainingDataPath)
  {
    Log.Debug("ExampleSpeechToText", "Attempting to add custom corpus {1} in customization {0}", customizationID, corpusName);
    m_SpeechToText.AddCustomCorpus(HandleAddCustomCorpus, customizationID, corpusName, allowOverwrite, trainingDataPath);
  }

  private void TestGetCustomWords()
  {
    Log.Debug("ExampleSpeechToText", "Attempting to get custom words.");
    m_SpeechToText.GetCustomWords(HandleGetCustomWords, m_CreatedCustomizationID);
  }

  private void TestAddCustomWordsPath(string customizationID, string wordsJsonPath)
  {
    Log.Debug("ExampleSpeechToText", "Attempting to add custom words in customization {0} using Words json path {1}", customizationID, wordsJsonPath);
    m_SpeechToText.AddCustomWords(HandleAddCustomWords, customizationID, wordsJsonPath);
  }

  private void TestAddCustomWordsWordsObject(string customizationID, Words words)
  {
    Log.Debug("ExampleSpeechToText", "Attempting to add custom words in customization {0} using Words object", customizationID);
    m_SpeechToText.AddCustomWords(HandleAddCustomWords, customizationID, words);
  }

  private void TestDeleteCustomWord(string customizationID, string word)
  {
    Log.Debug("ExampleSpeechToText", "Attempting to delete custom word {1} in customization {0}", customizationID, word);
    m_SpeechToText.DeleteCustomWord(HandleDeleteCustomWord, customizationID, word);
  }

  private void TestGetCustomWord(string customizationID, string word)
  {
    Log.Debug("ExampleSpeechToText", "Attempting to get custom word {1} in customization {0}", customizationID, word);
    m_SpeechToText.GetCustomWord(HandleGetCustomWord, customizationID, word);
  }



  private void HandleGetModels(Model[] models)
  {
    if (models != null)
    {
      if (models != null)
      {
        if (models.Length == 0)
        {
          Log.Warning("ExampleSpeedchToText", "There are no custom models!");
        }
        else
        {
          foreach (Model model in models)
          {
            Log.Debug("ExampleSpeechToText", "Model: {0}", model.name);
          }

          TestGetModel((models[Random.Range(0, models.Length - 1)] as Model).name);
        }
      }
    }
    else
    {
      Log.Warning("ExampleSpeechToText", "Failed to get models!");
    }
  }

  private void HandleGetModel(Model model)
  {
    if (model != null)
    {
      Log.Debug("ExampleSpeechToText", "Model - name: {0} | description: {1} | language:{2} | rate: {3} | sessions: {4} | url: {5} | customLanguageModel: {6}",
        model.name, model.description, model.language, model.rate, model.sessions, model.url, model.supported_features.custom_language_model);
    }
    else
    {
      Log.Warning("ExampleSpeechToText", "Failed to get model!");
    }
  }

  private void HandleOnRecognize(SpeechRecognitionEvent result)
  {
    if (result != null && result.results.Length > 0)
    {
      foreach (var res in result.results)
      {
        foreach (var alt in res.alternatives)
        {
          string text = alt.transcript;
          Log.Debug("ExampleSpeechToText", string.Format("{0} ({1}, {2:0.00})\n", text, res.final ? "Final" : "Interim", alt.confidence));
        }
      }
    }
  }

  private void HandleGetCustomizations(Customizations customizations, string customData)
  {
    if (!string.IsNullOrEmpty(customData))
      Log.Debug("ExampleSpeechToText", "custom data: {0}", customData);

    if (customizations != null)
    {
      if (customizations.customizations.Length > 0)
      {
        foreach (Customization customization in customizations.customizations)
          Log.Debug("ExampleSpeechToText", "Customization - name: {0} | description: {1} | status: {2}", customization.name, customization.description, customization.status);

        Log.Debug("ExampleSpeechToText", "GetCustomizations() succeeded!");
      }
      else
      {
        Log.Debug("ExampleSpeechToText", "There are no customizations!");
      }
    }
    else
    {
      Log.Debug("ExampleSpeechToText", "Failed to get customizations!");
    }
  }

  private void HandleCreateCustomization(CustomizationID customizationID, string customData)
  {
    if (customizationID != null)
    {
      Log.Debug("ExampleSpeechToText", "Customization created: {0}", customizationID.customization_id);
      Log.Debug("ExampleSpeechToText", "CreateCustomization() succeeded!");

      m_CreatedCustomizationID = customizationID.customization_id;
      TestGetCustomization(m_CreatedCustomizationID);
    }
    else
    {
      Log.Debug("ExampleSpeechToText", "Failed to create customization!");
    }
  }

  private void HandleDeleteCustomization(bool success, string customData)
  {
    if (success)
    {
      Log.Debug("ExampleSpeechToText", "Deleted customization {0}!", m_CreatedCustomizationID);
      Log.Debug("ExampleSpeechToText", "DeletedCustomization() succeeded!");
      m_CreatedCustomizationID = default(string);
    }
    else
    {
      Log.Debug("ExampleSpeechToText", "Failed to delete customization!");
    }
  }

  private void HandleGetCustomization(Customization customization, string customData)
  {
    if (!string.IsNullOrEmpty(customData))
      Log.Debug("ExampleSpeechToText", "custom data: {0}", customData);

    if (customization != null)
    {
      Log.Debug("ExampleSpeechToText", "Customization - name: {0} | description: {1} | status: {2}", customization.name, customization.description, customization.status);
      Log.Debug("ExampleSpeechToText", "GetCustomization() succeeded!");

      //	test add custom corpora
      //TestAddCustomCorpus(m_CreatedCustomizationID, m_CreatedCorpusName, true, m_CustomCorpusFilePath);
      TestDeleteCustomization(m_CreatedCustomizationID);
    }
    else
    {
      Log.Debug("ExampleSpeechToText", "Failed to get customization {0}!", m_CreatedCustomizationID);
    }
  }

  private void HandleTrainCustomization(bool success, string customData)
  {
    if (!string.IsNullOrEmpty(customData))
      Log.Debug("ExampleSpeechToText", "custom data: {0}", customData);

    if (success)
    {
      Log.Debug("ExampleSpeechToText", "Train customization {0}!", m_CreatedCustomizationID);
      Log.Debug("ExampleSpeechToText", "TrainCustomization() succeeded!");

      TestResetCustomization(m_CreatedCustomizationID);
    }
    else
    {
      Log.Debug("ExampleSpeechToText", "Failed to train customization!");
    }
  }

  private void HandleUpgradeCustomization(bool success, string customData)
  {
    if (!string.IsNullOrEmpty(customData))
      Log.Debug("ExampleSpeechToText", "custom data: {0}", customData);

    if (success)
    {
      Log.Debug("ExampleSpeechToText", "Upgrade customization {0}!", m_CreatedCustomizationID);
      Log.Debug("ExampleSpeechToText", "UpgradeCustomization() succeeded!");

    }
    else
    {
      Log.Debug("ExampleSpeechToText", "Failed to upgrade customization!");
    }
  }

  private void HandleResetCustomization(bool success, string customData)
  {
    if (!string.IsNullOrEmpty(customData))
      Log.Debug("ExampleSpeechToText", "custom data: {0}", customData);

    if (success)
    {
      Log.Debug("ExampleSpeechToText", "Reset customization {0}!", m_CreatedCustomizationID);
      Log.Debug("ExampleSpeechToText", "ResetCustomization() succeeded!");

      //	test delete custom corpus
      TestDeleteCustomCorpus(m_CreatedCustomizationID, m_CreatedCorpusName);
    }
    else
    {
      Log.Debug("ExampleSpeechToText", "Failed to reset customization!");
    }
  }

  private void HandleGetCustomCorpora(Corpora corpora, string customData)
  {
    if (!string.IsNullOrEmpty(customData))
      Log.Debug("ExampleSpeechToText", "CustomData: {0}", customData);

    if (corpora != null)
    {
      if (corpora.corpora.Length > 0)
      {
        foreach (Corpus corpus in corpora.corpora)
          Log.Debug("ExampleSpeechToText", "Corpus - name: {0} | total_words: {1} | out_of_vocabulary_words: {2} | staus: {3}",
            corpus.name, corpus.total_words, corpus.out_of_vocabulary_words, corpus.status);

        //TestUpgradeCustomization(m_CreatedCustomizationID);
        TestTrainCustomization(m_CreatedCustomizationID);
      }
      else
      {
        Log.Debug("ExampleSpeechToText", "There are no custom corpora!");
      }

      Log.Debug("ExampleSpeechToText", "GetCustomCorpora() succeeded!");
      TestDeleteCustomization(m_CreatedCustomizationID);
    }
    else
    {
      Log.Debug("ExampleSpeechToText", "Failed to get custom corpora!");
    }
  }

  private void HandleDeleteCustomCorpus(bool success, string customData)
  {
    if (!string.IsNullOrEmpty(customData))
      Log.Debug("ExampleSpeechToText", "custom data: {0}", customData);

    if (success)
    {
      Log.Debug("ExampleSpeechToText", "DeleteCustomCorpus() succeeded!");

    }
    else
    {
      Log.Debug("ExampleSpeechToText", "Failed to delete custom corpus!");
    }
  }

  private void HandleAddCustomCorpus(bool success, string customData)
  {
    if (!string.IsNullOrEmpty(customData))
      Log.Debug("ExampleSpeechToText", "custom data: {0}", customData);

    if (success)
    {
      Log.Debug("ExampleSpeechToText", "AddCustomCorpus() succeeded!");

      //	test get custom corpora
      TestGetCustomCorpora(m_CreatedCustomizationID);
    }
    else
    {
      Log.Debug("ExampleSpeechToText", "Failed to delete custom corpus!");
    }
  }

  private void HandleGetCustomWords(WordsList wordList, string customData)
  {
    if (!string.IsNullOrEmpty(customData))
      Log.Debug("ExampleSpeechToText", "custom data: {0}", customData);

    if (wordList != null)
    {
      if (wordList.words != null && wordList.words.Length > 0)
      {
        foreach (WordData word in wordList.words)
          Log.Debug("ExampleSpeechToText", "WordData - word: {0} | sounds like: {1} | display as: {2}", word.word, word.sounds_like, word.display_as);
      }
      else
      {
        Log.Debug("ExampleSpeechToText", "No custom words found!");
      }
    }
    else
    {
      Log.Debug("ExampleSpeechToText", "Failed to get custom words!");
    }
  }

  private void HandleAddCustomWords(bool success, string customData)
  {
    if (!string.IsNullOrEmpty(customData))
      Log.Debug("ExampleSpeechToText", "custom data: {0}", customData);

    if (success)
    {
      Log.Debug("ExampleSpeechToText", "AddCustomWOrds() succeeded!");

    }
    else
    {
      Log.Debug("ExampleSpeechToText", "Failed to add custom words!");
    }
  }

  private void HandleDeleteCustomWord(bool success, string customData)
  {
    if (!string.IsNullOrEmpty(customData))
      Log.Debug("ExampleSpeechToText", "custom data: {0}", customData);

    if (success)
    {
      Log.Debug("ExampleSpeechToText", "DeleteCustomWord() succeeded!");

    }
    else
    {
      Log.Debug("ExampleSpeechToText", "Failed to delete custom word!");
    }
  }

  private void HandleGetCustomWord(WordData word, string customData)
  {
    if (!string.IsNullOrEmpty(customData))
      Log.Debug("ExampleSpeechToText", "custom data: {0}", customData);

    if (word != null)
      Log.Debug("ExampleSpeechToText", "WordData - word: {0} | sounds like: {1} | display as: {2}", word.word, word.sounds_like, word.display_as);
  }
}