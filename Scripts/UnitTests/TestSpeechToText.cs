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


using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
  public class TestSpeechToText : UnitTest
  {
    private SpeechToText m_SpeechToText = new SpeechToText();

    private bool m_GetModelsTested = false;
    private bool m_GetModelTested = false;

    private bool m_GetCustomizationsTested = false;
    private bool m_CreateCustomizationTested = false;
    private bool m_GetCustomizationTested = false;
    private bool m_GetCustomCorporaTested = false;
    private bool m_AddCustomCorpusTested = false;
    private bool m_GetCustomWordsTested = false;
    private bool m_AddCustomWordsUsingFileTested = false;
    private bool m_AddCustomWordsUsingObjectTested = false;
    private bool m_GetCustomWordTested = false;
    private bool m_TrainCustomizationTested = false;
    private bool m_DeleteCustomCorpusTested = false;
    private bool m_DeleteCustomWordTested = false;
    private bool m_ResetCustomizationTested = false;
    private bool m_DeleteCustomizationTested = false;

    private string m_CreatedCustomizationID;
    private string m_CreatedCustomizationName = "unity-integration-test-customization";
    private string m_CreatedCorpusName = "unity-integration-test-corpus";
    private string m_CustomCorpusFilePath;
    private string m_SpeechToTextModelEnglish = "en-US_BroadbandModel";
    private string m_CustomWordsFilePath;
    private bool m_AllowOverwrite = true;
    private string m_WordToGet = "watson";

    private bool m_IsCustomizationBusy = false;

    public override IEnumerator RunTest()
    {
      if (Config.Instance.FindCredentials(m_SpeechToText.GetServiceID()) == null)
        yield break;

      m_CustomCorpusFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/test-stt-corpus.txt";
      m_CustomWordsFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/test-stt-words.json";

      //	GetModels
      Log.Debug("TestSpeechToText", "**********  Attempting to to GetModels");
      m_SpeechToText.GetModels(HandleGetModels);
      while (!m_GetModelsTested)
        yield return null;

      //	GetModel
      Log.Debug("TestSpeechToText", "**********  Attempting to to GetModel {0}", m_SpeechToTextModelEnglish);
      m_SpeechToText.GetModel(HandleGetModel, m_SpeechToTextModelEnglish);
      while (!m_GetModelTested)
        yield return null;

      //	GetCustomizations
      Log.Debug("TestSpeechToText", "**********  Attempting to to get customizations");
      m_SpeechToText.GetCustomizations(HandleGetCustomizations);
      while (!m_GetCustomizationsTested)
        yield return null;

      //	CreateCustomization
      Log.Debug("TestSpeechToText", "**********  Attempting to to create customization {0}", m_CreatedCustomizationName);
      m_SpeechToText.CreateCustomization(HandleCreateCustomization, m_CreatedCustomizationName);
      while (!m_CreateCustomizationTested)
        yield return null;

      //	GetCustomization
      Log.Debug("TestSpeechToText", "**********  Attempting to to get customization {0}", m_CreatedCustomizationID);
      m_SpeechToText.GetCustomization(HandleGetCustomization, m_CreatedCustomizationID);
      while (!m_GetCustomizationTested)
        yield return null;

      //	GetCustomCorpora
      Log.Debug("TestSpeechToText", "**********  Attempting to to get custom corpora");
      m_SpeechToText.GetCustomCorpora(HandleGetCustomCorpora, m_CreatedCustomizationID);
      while (!m_GetCustomCorporaTested)
        yield return null;

      //	AddCustomCorpus
      Log.Debug("TestSpeechToText", "**********  Attempting to to add custom corpus {0}", m_CreatedCorpusName);
      m_SpeechToText.AddCustomCorpus(HandleAddCustomCorpus, m_CreatedCustomizationID, m_CreatedCorpusName, m_AllowOverwrite, m_CustomCorpusFilePath);
      while (!m_AddCustomCorpusTested)
        yield return null;

      Runnable.Run(CheckCustomizationStatus(m_CreatedCustomizationID));
      while (m_IsCustomizationBusy)
        yield return null;

      //	GetCustomWords
      Log.Debug("TestSpeechToText", "**********  Attempting to to get custom words");
      m_SpeechToText.GetCustomWords(HandleGetCustomWords, m_CreatedCustomizationID);
      while (!m_GetCustomWordsTested)
        yield return null;

      //	AddCustomWordsUsingFile
      Log.Debug("TestSpeechToText", "**********  Attempting to to add custom words using file {0}", m_CustomWordsFilePath);
      m_SpeechToText.AddCustomWords(HandleAddCustomWordsUsingFile, m_CreatedCustomizationID, true, m_CustomWordsFilePath);
      while (!m_AddCustomWordsUsingFileTested)
        yield return null;

      Runnable.Run(CheckCustomizationStatus(m_CreatedCustomizationID));
      while (m_IsCustomizationBusy)
        yield return null;

      //	AddCustomWordsUsingObject
      Words words = new Words();
      Word w0 = new Word();
      List<Word> wordList = new List<Word>();
      w0.word = "mikey";
      w0.sounds_like = new string[1];
      w0.sounds_like[0] = "my key";
      w0.display_as = "Mikey";
      wordList.Add(w0);
      Word w1 = new Word();
      w1.word = "charlie";
      w1.sounds_like = new string[1];
      w1.sounds_like[0] = "char lee";
      w1.display_as = "Charlie";
      wordList.Add(w1);
      Word w2 = new Word();
      w2.word = "bijou";
      w2.sounds_like = new string[1];
      w2.sounds_like[0] = "be joo";
      w2.display_as = "Bijou";
      wordList.Add(w2);
      words.words = wordList.ToArray();

      Log.Debug("TestSpeechToText", "**********  Attempting to to add custom words using object");
      m_SpeechToText.AddCustomWords(HandleAddCustomWordsUsingObject, m_CreatedCustomizationID, words);
      while (!m_AddCustomWordsUsingObjectTested)
        yield return null;

      Runnable.Run(CheckCustomizationStatus(m_CreatedCustomizationID));
      while (m_IsCustomizationBusy)
        yield return null;

      //	GetCustomWord
      Log.Debug("TestSpeechToText", "**********  Attempting to to get custom word {0}", m_WordToGet);
      m_SpeechToText.GetCustomWord(HandleGetCustomWord, m_CreatedCustomizationID, m_WordToGet);
      while (!m_GetCustomWordTested)
        yield return null;

      //	TrainCustomization
      Log.Debug("TestSpeechToText", "**********  Attempting to to train customization {0}", m_CreatedCustomizationID);
      m_SpeechToText.TrainCustomization(HandleTrainCustomization, m_CreatedCustomizationID);
      while (!m_TrainCustomizationTested)
        yield return null;

      Runnable.Run(CheckCustomizationStatus(m_CreatedCustomizationID));
      while (m_IsCustomizationBusy)
        yield return null;

      //	DeleteCustomCorpus
      Log.Debug("TestSpeechToText", "**********  Attempting to to delete custom corpus {0}", m_CreatedCorpusName);
      m_SpeechToText.DeleteCustomCorpus(HandleDeleteCustomCorpus, m_CreatedCustomizationID, m_CreatedCorpusName);
      while (!m_DeleteCustomCorpusTested)
        yield return null;

      Runnable.Run(CheckCustomizationStatus(m_CreatedCustomizationID));
      while (m_IsCustomizationBusy)
        yield return null;

      //	DeleteCustomWord
      Log.Debug("TestSpeechToText", "**********  Attempting to to delete custom word {0}", m_WordToGet);
      m_SpeechToText.DeleteCustomWord(HandleDeleteCustomWord, m_CreatedCustomizationID, m_WordToGet);
      while (!m_DeleteCustomWordTested)
        yield return null;

      Runnable.Run(CheckCustomizationStatus(m_CreatedCustomizationID));
      while (m_IsCustomizationBusy)
        yield return null;

      //	ResetCustomization
      Log.Debug("TestSpeechToText", "**********  Attempting to to reset customization {0}", m_CreatedCustomizationID);
      m_SpeechToText.ResetCustomization(HandleResetCustomization, m_CreatedCustomizationID);
      while (!m_ResetCustomizationTested)
        yield return null;

      //	The customization is always pending after reset for some reason!
      //Runnable.Run(CheckCustomizationStatus(m_CreatedCustomizationID));
      //while (m_IsCustomizationBusy)
      //	yield return null;

      //	DeleteCustomization
      //Log.Debug("TestSpeechToText", "**********  Attempting to to delete customization {0}", m_CreatedCustomizationID);
      //m_SpeechToText.DeleteCustomization(HandleDeleteCustomization, m_CreatedCustomizationID);
      //while (!m_DeleteCustomizationTested)
      //  yield return null;

      yield break;
    }

    private void HandleGetModels(Model[] models)
    {
      if (models != null)
        Log.Status("TestSpeechToText", "GetModels() returned {0} models.", models.Length);

      Test(models != null);
      m_GetModelsTested = true;
    }

    private void HandleGetModel(Model model)
    {
      if (model != null)
      {
        Log.Debug("TestSpeechToText", "Model - name: {0} | description: {1} | language:{2} | rate: {3} | sessions: {4} | url: {5} | customLanguageModel: {6}",
          model.name, model.description, model.language, model.rate, model.sessions, model.url, model.supported_features.custom_language_model);
      }
      else
      {
        Log.Warning("TestSpeechToText", "Failed to get model!");
      }

      Test(model != null);
      m_GetModelTested = true;
    }

    private void HandleGetCustomizations(Customizations customizations, string customData)
    {
      if (!string.IsNullOrEmpty(customData))
        Log.Debug("TestSpeechToText", "custom data: {0}", customData);

      if (customizations != null)
      {
        if (customizations.customizations.Length > 0)
        {
          foreach (Customization customization in customizations.customizations)
            Log.Debug("TestSpeechToText", "Customization - name: {0} | description: {1} | status: {2}", customization.name, customization.description, customization.status);

          Log.Debug("TestSpeechToText", "GetCustomizations() succeeded!");
        }
        else
        {
          Log.Debug("TestSpeechToText", "There are no customizations!");
        }
      }
      else
      {
        Log.Debug("TestSpeechToText", "Failed to get customizations!");
      }

      Test(customizations != null);
      m_GetCustomizationsTested = true;
    }

    private void HandleCreateCustomization(CustomizationID customizationID, string customData)
    {
      if (customizationID != null)
      {
        Log.Debug("TestSpeechToText", "Customization created: {0}", customizationID.customization_id);
        Log.Debug("TestSpeechToText", "CreateCustomization() succeeded!");

        m_CreatedCustomizationID = customizationID.customization_id;
      }
      else
      {
        Log.Debug("TestSpeechToText", "Failed to create customization!");
      }

      Test(customizationID != null);
      m_CreateCustomizationTested = true;
    }

    private void HandleDeleteCustomization(bool success, string customData)
    {
      if (success)
      {
        Log.Debug("TestSpeechToText", "Deleted customization {0}!", m_CreatedCustomizationID);
        Log.Debug("TestSpeechToText", "DeletedCustomization() succeeded!");
        m_CreatedCustomizationID = default(string);
      }
      else
      {
        Log.Debug("TestSpeechToText", "Failed to delete customization!");
      }

      Test(success);
      m_DeleteCustomizationTested = true;
    }

    private void HandleGetCustomization(Customization customization, string customData)
    {
      if (!string.IsNullOrEmpty(customData))
        Log.Debug("TestSpeechToText", "custom data: {0}", customData);

      if (customization != null)
      {
        Log.Debug("TestSpeechToText", "Customization - name: {0} | description: {1} | status: {2}", customization.name, customization.description, customization.status);
        Log.Debug("TestSpeechToText", "GetCustomization() succeeded!");
      }
      else
      {
        Log.Debug("TestSpeechToText", "Failed to get customization {0}!", m_CreatedCustomizationID);
      }

      Test(customization != null);
      m_GetCustomizationTested = true;
    }

    private void HandleTrainCustomization(bool success, string customData)
    {
      if (!string.IsNullOrEmpty(customData))
        Log.Debug("TestSpeechToText", "custom data: {0}", customData);

      if (success)
      {
        Log.Debug("TestSpeechToText", "Train customization {0}!", m_CreatedCustomizationID);
        Log.Debug("TestSpeechToText", "TrainCustomization() succeeded!");
        m_IsCustomizationBusy = true;
      }
      else
      {
        Log.Debug("TestSpeechToText", "Failed to train customization!");
      }

      Test(success);
      m_TrainCustomizationTested = true;
    }

    private void HandleUpgradeCustomization(bool success, string customData)
    {
      if (!string.IsNullOrEmpty(customData))
        Log.Debug("TestSpeechToText", "custom data: {0}", customData);

      if (success)
      {
        Log.Debug("TestSpeechToText", "Upgrade customization {0}!", m_CreatedCustomizationID);
        Log.Debug("TestSpeechToText", "UpgradeCustomization() succeeded!");

      }
      else
      {
        Log.Debug("TestSpeechToText", "Failed to upgrade customization!");
      }

      Test(success);
      //m_UpgradeCustomizationTested = true;
      //	Note: This method is not yet implemented!
    }

    private void HandleResetCustomization(bool success, string customData)
    {
      if (!string.IsNullOrEmpty(customData))
        Log.Debug("TestSpeechToText", "custom data: {0}", customData);

      if (success)
      {
        Log.Debug("TestSpeechToText", "Reset customization {0}!", m_CreatedCustomizationID);
        Log.Debug("TestSpeechToText", "ResetCustomization() succeeded!");
        //m_IsCustomizationBusy = true;
      }
      else
      {
        Log.Debug("TestSpeechToText", "Failed to reset customization!");
      }

      Test(success);
      m_ResetCustomizationTested = true;
    }

    private void HandleGetCustomCorpora(Corpora corpora, string customData)
    {
      if (!string.IsNullOrEmpty(customData))
        Log.Debug("TestSpeechToText", "CustomData: {0}", customData);

      if (corpora != null)
      {
        if (corpora.corpora.Length > 0)
        {
          foreach (Corpus corpus in corpora.corpora)
            Log.Debug("TestSpeechToText", "Corpus - name: {0} | total_words: {1} | out_of_vocabulary_words: {2} | staus: {3}",
              corpus.name, corpus.total_words, corpus.out_of_vocabulary_words, corpus.status);
        }
        else
        {
          Log.Debug("TestSpeechToText", "There are no custom corpora!");
        }

        Log.Debug("TestSpeechToText", "GetCustomCorpora() succeeded!");
      }
      else
      {
        Log.Debug("TestSpeechToText", "Failed to get custom corpora!");
      }

      Test(corpora != null);
      m_GetCustomCorporaTested = true;
    }

    private void HandleDeleteCustomCorpus(bool success, string customData)
    {
      if (!string.IsNullOrEmpty(customData))
        Log.Debug("TestSpeechToText", "custom data: {0}", customData);

      if (success)
      {
        Log.Debug("TestSpeechToText", "DeleteCustomCorpus() succeeded!");
        m_IsCustomizationBusy = true;
      }
      else
      {
        Log.Debug("TestSpeechToText", "Failed to delete custom corpus!");
      }

      Test(success);
      m_DeleteCustomCorpusTested = true;
    }

    private void HandleAddCustomCorpus(bool success, string customData)
    {
      if (!string.IsNullOrEmpty(customData))
        Log.Debug("TestSpeechToText", "custom data: {0}", customData);

      if (success)
      {
        Log.Debug("TestSpeechToText", "AddCustomCorpus() succeeded!");
        m_IsCustomizationBusy = true;
      }
      else
      {
        Log.Debug("TestSpeechToText", "Failed to delete custom corpus!");
      }

      Test(success);
      m_AddCustomCorpusTested = true;
    }

    private void HandleGetCustomWords(WordsList wordList, string customData)
    {
      if (!string.IsNullOrEmpty(customData))
        Log.Debug("TestSpeechToText", "custom data: {0}", customData);

      if (wordList != null)
      {
        if (wordList.words != null && wordList.words.Length > 0)
        {
          foreach (WordData word in wordList.words)
            Log.Debug("TestSpeechToText", "WordData - word: {0} | sounds like[0]: {1} | display as: {2} | source[0]: {3}", word.word, word.sounds_like[0], word.display_as, word.source[0]);
        }
        else
        {
          Log.Debug("TestSpeechToText", "No custom words found!");
        }
      }
      else
      {
        Log.Debug("TestSpeechToText", "Failed to get custom words!");
      }

      Test(wordList != null);
      m_GetCustomWordsTested = true;
    }

    private void HandleAddCustomWordsUsingFile(bool success, string customData)
    {
      if (!string.IsNullOrEmpty(customData))
        Log.Debug("TestSpeechToText", "custom data: {0}", customData);

      if (success)
      {
        Log.Debug("TestSpeechToText", "AddCustomWords() using file succeeded!");
        m_IsCustomizationBusy = true;
      }
      else
      {
        Log.Debug("TestSpeechToText", "Failed to add custom words using file!");
      }

      Test(success);
      m_AddCustomWordsUsingFileTested = true;
    }

    private void HandleAddCustomWordsUsingObject(bool success, string customData)
    {
      if (!string.IsNullOrEmpty(customData))
        Log.Debug("TestSpeechToText", "custom data: {0}", customData);

      if (success)
      {
        Log.Debug("TestSpeechToText", "AddCustomWords() using object succeeded!");
        m_IsCustomizationBusy = true;
      }
      else
      {
        Log.Debug("TestSpeechToText", "Failed to add custom words using object!");
      }

      Test(success);
      m_AddCustomWordsUsingObjectTested = true;
    }

    private void HandleDeleteCustomWord(bool success, string customData)
    {
      if (!string.IsNullOrEmpty(customData))
        Log.Debug("TestSpeechToText", "custom data: {0}", customData);

      if (success)
      {
        Log.Debug("TestSpeechToText", "DeleteCustomWord() succeeded!");
        m_IsCustomizationBusy = true;
      }
      else
      {
        Log.Debug("TestSpeechToText", "Failed to delete custom word!");
      }

      Test(success);
      m_DeleteCustomWordTested = true;
    }

    private void HandleGetCustomWord(WordData word, string customData)
    {
      if (!string.IsNullOrEmpty(customData))
        Log.Debug("TestSpeechToText", "custom data: {0}", customData);

      if (word != null)
        Log.Debug("TestSpeechToText", "WordData - word: {0} | sounds like[0]: {1} | display as: {2} | source[0]: {3}", word.word, word.sounds_like[0], word.display_as, word.source[0]);

      Test(word != null);
      m_GetCustomWordTested = true;
    }

    private void OnCheckCustomizationStatus(Customization customization, string customData)
    {
      if (customization != null)
      {
        Log.Debug("TestSpeechToText", "Customization status: {0}", customization.status);
        if (customization.status != "ready" && customization.status != "available")
          Runnable.Run(CheckCustomizationStatus(customData, 5f));
        else
          m_IsCustomizationBusy = false;
      }
      else
      {
        Log.Debug("TestSpeechToText", "Check customization status failed!");
      }
    }

    private IEnumerator CheckCustomizationStatus(string customizationID, float delay = 0.1f)
    {
      Log.Debug("TestSpeechToText", "Checking customization status in {0} seconds...", delay.ToString());
      yield return new WaitForSeconds(delay);

      //	passing customizationID in custom data
      m_SpeechToText.GetCustomization(OnCheckCustomizationStatus, customizationID, customizationID);
    }
  }
}
