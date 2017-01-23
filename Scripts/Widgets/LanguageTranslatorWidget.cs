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
using UnityEngine.UI;
using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.LanguageTranslation.v1;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using System;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Utilities;

#pragma warning disable 414

namespace IBM.Watson.DeveloperCloud.Widgets
{

  /// <summary>
  /// Translation widget to handle translation service calls
  /// </summary>
  public class LanguageTranslatorWidget : Widget
  {
    #region Inputs
    [SerializeField]
    private Input m_SpeechInput = new Input("SpeechInput", typeof(SpeechToTextData), "OnSpeechInput");
    #endregion

    #region Outputs
    [SerializeField]
    private Output m_RecognizeLanguageOutput = new Output(typeof(LanguageData));
    [SerializeField]
    private Output m_SpeechOutput = new Output(typeof(TextToSpeechData));
    [SerializeField]
    private Output m_VoiceOutput = new Output(typeof(VoiceData));
    #endregion

    #region Private Data
    private LanguageTranslation m_Translate = new LanguageTranslation();

    [SerializeField, Tooltip("Source language, if empty language will be auto-detected.")]
    private string m_SourceLanguage = string.Empty;
    [SerializeField, Tooltip("Target language to translate into.")]
    private string m_TargetLanguage = "es";
    [SerializeField, Tooltip("Input field for inputting speech")]
    private InputField m_Input = null;
    [SerializeField, Tooltip("Output text for showing translated text")]
    private Text m_Output = null;
    [SerializeField]
    private Dropdown m_DropDownSourceLanguage = null;
    [SerializeField]
    private Dropdown m_DropDownTargetLanguage = null;
    [SerializeField]
    private string m_DefaultDomainToUse = "conversational";
    [SerializeField]
    private string m_DetectLanguageName = "Detect Language";
    private string m_DetectLanguageID = "";

    // Mapping from language ID to it's Name
    private Dictionary<string, string> m_LanguageIDToName = new Dictionary<string, string>();
    // Mapping from language name to ID
    private Dictionary<string, string> m_LanguageNameToID = new Dictionary<string, string>();
    // Mapping from language to a list of languages that can be translated into..
    private Dictionary<string, List<string>> m_LanguageToTranslate = new Dictionary<string, List<string>>();
    // array of availablel languages;
    private string[] m_Languages = null;
    // Last string of input text    
    private string m_TranslateText = string.Empty;
    #endregion

    #region Widget interface
    /// <exclude />
    protected override string GetName()
    {
      return "Translate";
    }
    #endregion

    #region Public Members
    /// <summary>
    /// Set or get the source language ID. If set to null or empty, then the language will be auto-detected.
    /// </summary>
    public string SourceLanguage
    {
      set
      {
        if (m_SourceLanguage != value)
        {
          m_SourceLanguage = value;

          if (m_RecognizeLanguageOutput.IsConnected && !string.IsNullOrEmpty(m_SourceLanguage))
            m_RecognizeLanguageOutput.SendData(new LanguageData(m_SourceLanguage));
          ResetSourceLanguageDropDown();
          ResetTargetLanguageDropDown();
        }
      }
      get { return m_SourceLanguage; }
    }

    /// <summary>
    /// Set or get the target language ID.
    /// </summary>
    public string TargetLanguage
    {
      set
      {
        if (TargetLanguage != value)
        {
          m_TargetLanguage = value;
          ResetVoiceForTargetLanguage();
        }
      }
      get { return m_TargetLanguage; }
    }
    #endregion

    #region Event Handlers
    private void OnEnable()
    {
      Log.Status("TranslatorWidget", "OnEnable");
      //UnityEngine.Debug.LogWarning("TranslatorWidget - OnEnable");
      m_Translate.GetLanguages(OnGetLanguages);
    }

    /// <exclude />
    protected override void Awake()
    {
      base.Awake();

      if (m_Input != null)
        m_Input.onEndEdit.AddListener(delegate { OnInputEnd(); });
      if (m_DropDownSourceLanguage != null)
        m_DropDownSourceLanguage.onValueChanged.AddListener(delegate { DropDownSourceValueChanged(); });
      if (m_DropDownTargetLanguage != null)
        m_DropDownTargetLanguage.onValueChanged.AddListener(delegate { DropDownTargetValueChanged(); });
    }

    /// <exclude />
    protected override void Start()
    {
      base.Start();

      // resolve variables
      m_SourceLanguage = Config.Instance.ResolveVariables(m_SourceLanguage);
      m_TargetLanguage = Config.Instance.ResolveVariables(m_TargetLanguage);

      if (m_RecognizeLanguageOutput.IsConnected && !string.IsNullOrEmpty(m_SourceLanguage))
        m_RecognizeLanguageOutput.SendData(new LanguageData(m_SourceLanguage));
    }

    private void OnInputEnd()
    {
      if (m_Input != null)
      {
        if (!string.IsNullOrEmpty(TargetLanguage))
          Translate(m_Input.text);
        else
          Log.Error("TranslatorWidget", "OnTranslation - Target Language should be set!");
      }
    }

    private void OnSpeechInput(Data data)
    {
      SpeechToTextData speech = data as SpeechToTextData;
      if (speech != null && speech.Results.HasFinalResult())
        Translate(speech.Results.results[0].alternatives[0].transcript);
    }

    private void OnGetLanguages(Languages languages)
    {
      if (languages != null && languages.languages.Length > 0)
      {
        Log.Status("TranslatorWidget", "OnGetLanguagesAndGetModelsAfter as {0}", languages.languages.Length);
        m_LanguageIDToName.Clear();

        foreach (var lang in languages.languages)
        {
          m_LanguageIDToName[lang.language] = lang.name;
          m_LanguageNameToID[lang.name] = lang.language;
        }

        m_LanguageIDToName[m_DetectLanguageID] = m_DetectLanguageName;
        m_LanguageNameToID[m_DetectLanguageName] = m_DetectLanguageID;
        m_Translate.GetModels(OnGetModels); //To fill dropdown with models to use in Translation
      }
      else
      {
        Log.Error("TranslatorWidget", "OnGetLanguages - There is no language to translate. Check the connections and service of Translation Service.");
      }
    }

    private void OnGetModels(TranslationModels models)
    {
      Log.Status("TranslatorWidget", "OnGetModels, Count: {0}", models.models.Length);
      if (models != null && models.models.Length > 0)
      {
        m_LanguageToTranslate.Clear();

        List<string> listLanguages = new List<string>();    //From - To language list to use in translation

        //Adding initial language as detected!
        listLanguages.Add(m_DetectLanguageID);
        m_LanguageToTranslate.Add(m_DetectLanguageID, new List<string>());

        foreach (var model in models.models)
        {
          if (string.Equals(model.domain, m_DefaultDomainToUse))
          {
            if (m_LanguageToTranslate.ContainsKey(model.source))
            {
              if (!m_LanguageToTranslate[model.source].Contains(model.target))
                m_LanguageToTranslate[model.source].Add(model.target);
            }
            else
            {
              m_LanguageToTranslate.Add(model.source, new List<string>());
              m_LanguageToTranslate[model.source].Add(model.target);
            }

            if (!listLanguages.Contains(model.source))
              listLanguages.Add(model.source);
            if (!listLanguages.Contains(model.target))
              listLanguages.Add(model.target);

            if (!m_LanguageToTranslate[m_DetectLanguageID].Contains(model.target))
              m_LanguageToTranslate[m_DetectLanguageID].Add(model.target);
          }
        }

        m_Languages = listLanguages.ToArray();
        ResetSourceLanguageDropDown();
        ResetVoiceForTargetLanguage();
      }
    }
    #endregion

    #region Private Functions
    private void Translate(string text)
    {
      if (!string.IsNullOrEmpty(text))
      {
        m_TranslateText = text;

        if (m_Input != null)
          m_Input.text = text;

        new TranslateRequest(this, text);
      }
    }

    private class TranslateRequest
    {
      private LanguageTranslatorWidget m_Widget;
      private string m_Text;

      public TranslateRequest(LanguageTranslatorWidget widget, string text)
      {
        m_Widget = widget;
        m_Text = text;

        if (string.IsNullOrEmpty(m_Widget.SourceLanguage))
          m_Widget.m_Translate.Identify(m_Text, OnIdentified);
        else
          m_Widget.m_Translate.GetTranslation(m_Text, m_Widget.SourceLanguage, m_Widget.TargetLanguage, OnGetTranslation);
      }

      private void OnIdentified(string language)
      {
        if (!string.IsNullOrEmpty(language))
        {
          m_Widget.SourceLanguage = language;
          m_Widget.m_Translate.GetTranslation(m_Text, language, m_Widget.TargetLanguage, OnGetTranslation);
        }
        else
          Log.Error("TranslateWidget", "Failed to identify language: {0}", m_Text);
      }

      private void OnGetTranslation(Translations translations)
      {
        if (translations != null && translations.translations.Length > 0)
          m_Widget.SetOutput(translations.translations[0].translation);
      }
    };

    private void SetOutput(string text)
    {
      Log.Debug("TranslateWidget", "SetOutput(): {0}", text);

      if (m_Output != null)
        m_Output.text = text;

      if (m_SpeechOutput.IsConnected)
        m_SpeechOutput.SendData(new TextToSpeechData(text));
    }

    private void ResetSourceLanguageDropDown()
    {
      if (m_DropDownSourceLanguage != null)
      {
        m_DropDownSourceLanguage.options.Clear();

        int selected = 0;
        foreach (string itemLanguage in m_Languages)
        {
          if (m_LanguageIDToName.ContainsKey(itemLanguage))
            m_DropDownSourceLanguage.options.Add(new Dropdown.OptionData(m_LanguageIDToName[itemLanguage]));

          if (String.Equals(SourceLanguage, itemLanguage))
            selected = m_DropDownSourceLanguage.options.Count - 1;
        }

        m_DropDownSourceLanguage.value = selected;
      }
    }

    private void DropDownSourceValueChanged()
    {
      if (m_DropDownSourceLanguage != null && m_DropDownSourceLanguage.options.Count > 0)
      {
        string selected = m_DropDownSourceLanguage.options[m_DropDownSourceLanguage.value].text;
        if (m_LanguageNameToID.ContainsKey(selected))
        {
          selected = m_LanguageNameToID[selected];
          if (selected != SourceLanguage)
          {
            SourceLanguage = selected;
            Translate(m_TranslateText);
          }
        }
      }
    }

    private void DropDownTargetValueChanged()
    {
      if (m_DropDownTargetLanguage != null && m_DropDownTargetLanguage.options.Count > 0)
      {
        string selected = m_DropDownTargetLanguage.options[m_DropDownTargetLanguage.value].text;
        if (m_LanguageNameToID.ContainsKey(selected))
        {
          string target = m_LanguageNameToID[selected];
          if (target != TargetLanguage)
          {
            TargetLanguage = target;
            Translate(m_TranslateText);
          }
        }
      }
    }

    private void ResetTargetLanguageDropDown()
    {
      if (m_DropDownTargetLanguage != null)
      {
        if (!string.IsNullOrEmpty(SourceLanguage) && m_LanguageToTranslate.ContainsKey(SourceLanguage))
        {
          //Add target language corresponding source language
          m_DropDownTargetLanguage.options.Clear();
          int selected = 0;

          foreach (string itemLanguage in m_LanguageToTranslate[SourceLanguage])
          {
            if (string.Equals(itemLanguage, SourceLanguage))
              continue;

            m_DropDownTargetLanguage.options.Add(new Dropdown.OptionData(m_LanguageIDToName[itemLanguage]));

            if (String.Equals(TargetLanguage, itemLanguage))
              selected = m_DropDownTargetLanguage.options.Count - 1;
          }

          m_DropDownTargetLanguage.captionText.text = m_DropDownTargetLanguage.options[selected].text;
          m_DropDownTargetLanguage.value = selected;
        }
      }
    }

    private void ResetVoiceForTargetLanguage()
    {
      if (m_VoiceOutput.IsConnected)
      {
        if (TargetLanguage == "en")
          m_VoiceOutput.SendData(new VoiceData(VoiceType.en_US_Michael));
        else if (TargetLanguage == "de")
          m_VoiceOutput.SendData(new VoiceData(VoiceType.de_DE_Dieter));
        else if (TargetLanguage == "es")
          m_VoiceOutput.SendData(new VoiceData(VoiceType.es_ES_Enrique));
        else if (TargetLanguage == "fr")
          m_VoiceOutput.SendData(new VoiceData(VoiceType.fr_FR_Renee));
        else if (TargetLanguage == "it")
          m_VoiceOutput.SendData(new VoiceData(VoiceType.it_IT_Francesca));
        else if (TargetLanguage == "ja")
          m_VoiceOutput.SendData(new VoiceData(VoiceType.ja_JP_Emi));
        else
          Log.Warning("TranslateWidget", "Unsupported voice for language {0}", TargetLanguage);
      }
    }
    #endregion
  }
}
