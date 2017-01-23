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

using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Services.NaturalLanguageClassifier.v1;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

#pragma warning disable 414

namespace IBM.Watson.DeveloperCloud.Widgets
{
  /// <summary>
  /// Natural Language Classifier Widget.
  /// </summary>
  public class NaturalLanguageClassifierWidget : Widget
  {
    #region Inputs
    [SerializeField]
    private Input m_RecognizeInput = new Input("Recognize", typeof(SpeechToTextData), "OnRecognize");
    #endregion

    #region Outputs
    [SerializeField]
    private Output m_ClassifyOutput = new Output(typeof(ClassifyResultData), true);
    #endregion

    #region Private Data
    private NaturalLanguageClassifier m_NaturalLanguageClassifier = new NaturalLanguageClassifier();
    private Classifier m_Selected = null;

    [SerializeField]
    private string m_ClassifierName = string.Empty;
    [SerializeField]
    private string m_ClassifierId = string.Empty;
    [SerializeField, Tooltip("What is the minimum word confidence needed to send onto the Natural Language Classifier?")]
    private float m_MinWordConfidence = 0f;
    private float m_MinWordConfidenceDelta = 0.0f;
    [SerializeField, Tooltip("Recognized speech below this confidence is just ignored.")]
    private float m_IgnoreWordConfidence = 0f;
    private float m_IgnoreWordConfidenceDelta = 0.0f;
    [SerializeField, Tooltip("What is the minimum confidence for a classification event to be fired.")]
    private float m_MinClassEventConfidence = 0f;
    private float m_MinClassEventConfidenceDelta = 0.0f;
    [SerializeField]
    private string m_Language = "en";

    [Serializable]
    private class ClassEventMapping
    {
      public string m_Class = null;
      public string m_Event = "";
    };
    [SerializeField]
    private List<ClassEventMapping> m_ClassEventList = new List<ClassEventMapping>();
    private Dictionary<string, string> m_ClassEventMap = new Dictionary<string, string>();
    //		private Dictionary<string, Constants.Event> m_ClassEventMap = new Dictionary<string, Constants.Event>();

    [SerializeField]
    private Text m_TopClassText = null;
    #endregion

    #region Public Properties
    /// <summary>
    /// Returns the Natural Language Classifier service object.
    /// </summary>
    public NaturalLanguageClassifier NaturalLanguageClassifier { get { return m_NaturalLanguageClassifier; } }

    /// <summary>
    /// Gets or sets the value of ignore word confidence.
    /// </summary>
    /// <value>The ignore word confidence.</value>
    public float IgnoreWordConfidence
    {
      get
      {
        return Mathf.Clamp01(m_IgnoreWordConfidence + m_IgnoreWordConfidenceDelta);
      }
      set
      {
        m_IgnoreWordConfidenceDelta = value + m_IgnoreWordConfidence;
        if (IgnoreWordConfidence > MinWordConfidence)
          MinWordConfidence = IgnoreWordConfidence;
        PlayerPrefs.SetFloat("m_IgnoreWordConfidenceDelta", m_IgnoreWordConfidenceDelta);
        PlayerPrefs.Save();
      }
    }
    /// <summary>
    /// Gets or sets the value of ignore word confidence delta.
    /// </summary>
    /// <value>The ignore word confidence delta.</value>
    public float IgnoreWordConfidenceDelta
    {
      get { return m_IgnoreWordConfidenceDelta; }
      set
      {
        m_IgnoreWordConfidenceDelta = value;
        PlayerPrefs.SetFloat("m_IgnoreWordConfidenceDelta", m_IgnoreWordConfidenceDelta);
        PlayerPrefs.Save();
      }
    }

    /// <summary>
    /// Gets or sets the minimum value of word confidence.
    /// </summary>
    /// <value>The minimum word confidence.</value>
    public float MinWordConfidence
    {
      get
      {
        return Mathf.Clamp01(m_MinWordConfidence + m_MinWordConfidenceDelta);
        //                return Mathf.Clamp01(m_MinWordConfidenceDelta);
      }
      set
      {
        m_MinWordConfidenceDelta = value + m_MinWordConfidence;
        if (MinWordConfidence < IgnoreWordConfidence)
          IgnoreWordConfidence = MinWordConfidence;
        PlayerPrefs.SetFloat("m_MinWordConfidenceDelta", m_MinWordConfidenceDelta);
        PlayerPrefs.Save();
      }
    }

    /// <summary>
    /// Gets or sets the minimum value of word confidence delta.
    /// </summary>
    /// <value>The minimum word confidence delta.</value>
    public float MinWordConfidenceDelta
    {
      get { return m_MinWordConfidenceDelta; }
      set
      {
        m_MinWordConfidenceDelta = value;
        PlayerPrefs.SetFloat("m_MinWordConfidenceDelta", m_MinWordConfidenceDelta);
        PlayerPrefs.Save();
      }
    }

    /// <summary>
    /// Gets or sets the minimum value of class event confidence.
    /// </summary>
    /// <value>The minimum class event confidence.</value>
    public float MinClassEventConfidence
    {
      get
      {
        return Mathf.Clamp01(m_MinClassEventConfidence + m_MinClassEventConfidenceDelta);
      }
      set
      {
        m_MinClassEventConfidenceDelta = value + m_MinClassEventConfidence;
        PlayerPrefs.SetFloat("m_MinClassEventConfidenceDelta", m_MinClassEventConfidenceDelta);
        PlayerPrefs.Save();
      }
    }

    /// <summary>
    /// Gets or sets the minimum value of class event confidence delta.
    /// </summary>
    /// <value>The minimum class event confidence delta.</value>
    public float MinClassEventConfidenceDelta
    {
      get { return m_MinClassEventConfidenceDelta; }
      set
      {
        m_MinClassEventConfidenceDelta = value;
        PlayerPrefs.SetFloat("m_MinClassEventConfidenceDelta", m_MinClassEventConfidenceDelta);
        PlayerPrefs.Save();
      }
    }
    #endregion

    #region Widget interface
    /// <exclude />
    protected override string GetName()
    {
      return "Natural Language Classifier";
    }
    #endregion

    #region Event Handlers
    /// <exclude />
    protected override void Start()
    {
      base.Start();

      m_IgnoreWordConfidenceDelta = PlayerPrefs.GetFloat("m_IgnoreWordConfidenceDelta", 0.0f);
      m_MinWordConfidenceDelta = PlayerPrefs.GetFloat("m_MinWordConfidenceDelta", 0.0f);
      m_MinClassEventConfidenceDelta = PlayerPrefs.GetFloat("m_MinClassEventConfidenceDelta", 0.0f);

      // resolve configuration variables
      m_ClassifierName = Config.Instance.ResolveVariables(m_ClassifierName);
      m_ClassifierId = Config.Instance.ResolveVariables(m_ClassifierId);

      if (string.IsNullOrEmpty(m_ClassifierId))
      {
        Log.Status("NaturalLanguageClassifierWidget", "Auto selecting a classifier.");
        if (!m_NaturalLanguageClassifier.GetClassifiers(OnGetClassifiers))
          Log.Error("NaturalLanguageClassifierWidget", "Failed to request all classifiers.");
      }
      else
      {
        if (!m_NaturalLanguageClassifier.GetClassifier(m_ClassifierId, OnGetClassifier))
          Log.Equals("NaturalLanguageClassifierWidget", "Failed to request classifier.");
      }
    }

    private void OnEnable()
    {
      EventManager.Instance.RegisterEventReceiver("OnDebugCommand", OnDebugCommand);
    }
    private void OnDisable()
    {
      EventManager.Instance.UnregisterEventReceiver("OnDebugCommand", OnDebugCommand);
    }

    private void OnGetClassifiers(Classifiers classifiers)
    {
      if (classifiers != null)
      {
        bool bFound = false;
        foreach (var classifier in classifiers.classifiers)
        {
          if (!string.IsNullOrEmpty(m_ClassifierName) && !classifier.name.ToLower().StartsWith(m_ClassifierName.ToLower()))
            continue;
          if (classifier.language != m_Language)
            continue;

          m_NaturalLanguageClassifier.GetClassifier(classifier.classifier_id, OnGetClassifier);
          bFound = true;
        }

        if (!bFound)
          Log.Error("NaturalLanguageClassifierWidget", "No classifiers found that match {0}", m_ClassifierName);
      }
    }

    private void OnGetClassifier(Classifier classifier)
    {
      if (classifier != null && classifier.status == "Available")
      {
        if (m_Selected == null || m_Selected.created.CompareTo(classifier.created) < 0)
        {
          m_Selected = classifier;
          m_ClassifierId = m_Selected.classifier_id;

          Log.Status("NaturalLanguageClassifierWidget", "Selected classifier {0}, Created: {1}, Name: {2}",
              m_Selected.classifier_id, m_Selected.created, m_Selected.name);
        }
      }
    }

    private void OnRecognize(Data data)
    {
      SpeechRecognitionEvent result = ((SpeechToTextData)data).Results;
      if (result.HasFinalResult())
      {
        string text = result.results[0].alternatives[0].transcript;
        double textConfidence = result.results[0].alternatives[0].confidence;

        Log.Debug("NaturalLanguageClassifierWidget", "OnRecognize: {0} ({1:0.00})", text, textConfidence);
        EventManager.Instance.SendEvent("OnDebugMessage", string.Format("{0} ({1:0.00})", text, textConfidence));

        if (textConfidence > MinWordConfidence)
        {
          if (!string.IsNullOrEmpty(m_ClassifierId))
          {
            if (!m_NaturalLanguageClassifier.Classify(m_ClassifierId, text, OnClassified))
              Log.Error("NaturalLanguageClassifierWidget", "Failed to send {0} to Natural Language Classifier.", text);
          }
          else
            Log.Equals("NaturalLanguageClassifierWidget", "No valid classifier set.");
        }
        else
        {
          Log.Debug("NaturalLanguagClassifierWidget", "Text confidence {0} < {1} (Min word confidence)", textConfidence, MinWordConfidence);
          if (textConfidence > IgnoreWordConfidence)
          {
            Log.Debug("NaturalLanguageClassifierWidget", "Text confidence {0} > {1} (Ignore word confidence)", textConfidence, IgnoreWordConfidence);
            EventManager.Instance.SendEvent("OnClassifyFailure", result);
          }
        }
      }
    }

    private void OnClassified(ClassifyResult result)
    {
      EventManager.Instance.SendEvent("OnClassifyResult", result);

      if (m_ClassifyOutput.IsConnected)
        m_ClassifyOutput.SendData(new ClassifyResultData(result));

      if (result != null)
      {
        Log.Debug("NaturalLanguageClassifierWidget", "OnClassified: {0} ({1:0.00})", result.top_class, result.topConfidence);

        if (m_TopClassText != null)
          m_TopClassText.text = result.top_class;

        if (!string.IsNullOrEmpty(result.top_class))
        {
          if (result.topConfidence >= MinClassEventConfidence)
          {
            if (m_ClassEventList.Count > 0 && m_ClassEventMap.Count == 0)
            {
              // initialize the map
              foreach (var ev in m_ClassEventList)
                m_ClassEventMap[ev.m_Class] = ev.m_Event;
            }

            string sendEvent;
            //						Constants.Event sendEvent;
            if (!m_ClassEventMap.TryGetValue(result.top_class, out sendEvent))
            {
              Log.Warning("NaturalLanguageClassifierWidget", "No class mapping found for {0}", result.top_class);
              EventManager.Instance.SendEvent(result.top_class, result);
            }
            else
              EventManager.Instance.SendEvent(sendEvent, result);
          }
          else
          {
            if (result.topConfidence > IgnoreWordConfidence)
              EventManager.Instance.SendEvent("OnClassifyFailure", result);
          }
        }
      }
    }

    private void OnDebugCommand(object[] args)
    {
      string text = args != null && args.Length > 0 ? args[0] as string : string.Empty;
      if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(m_ClassifierId))
      {
        if (!m_NaturalLanguageClassifier.Classify(m_ClassifierId, text, OnClassified))
          Log.Error("NaturalLanguageClassifierWidget", "Failed to send {0} to Natural Language Classifier.", (string)args[0]);
      }
    }
    #endregion
  }
}
