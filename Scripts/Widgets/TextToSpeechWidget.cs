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
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.DataTypes;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Utilities;

#pragma warning disable 414

namespace IBM.Watson.DeveloperCloud.Widgets
{
  /// <summary>
  /// TextToSpeech widget class wraps the TextToSpeech serivce.
  /// </summary>
  [RequireComponent(typeof(AudioSource))]
  public class TextToSpeechWidget : Widget
  {
    #region Inputs
    [SerializeField]
    private Input m_TextInput = new Input("Text", typeof(TextToSpeechData), "OnTextInput");
    [SerializeField]
    private Input m_VoiceInput = new Input("Voice", typeof(VoiceData), "OnVoiceSelect");
    #endregion

    #region Outputs
    [SerializeField]
    private Output m_Speaking = new Output(typeof(SpeakingStateData));
    [SerializeField]
    private Output m_DisableMic = new Output(typeof(DisableMicData));
    [SerializeField]
    private Output m_LevelOut = new Output(typeof(LevelData));
    #endregion

    #region Private Data
    TextToSpeech m_TextToSpeech = new TextToSpeech();

    [SerializeField, Tooltip("How often to send level out data in seconds.")]
    private float m_LevelOutInterval = 0.05f;
    [SerializeField]
    private float m_LevelOutputModifier = 1.0f;
    [SerializeField]
    private Button m_TextToSpeechButton = null;
    [SerializeField]
    private InputField m_Input = null;
    [SerializeField]
    private Text m_StatusText = null;
    [SerializeField]
    private VoiceType m_Voice = VoiceType.en_US_Michael;
    [SerializeField]
    private bool m_UsePost = false;

    private AudioSource m_Source = null;
    private int m_LastPlayPos = 0;

    private class Speech
    {
      ~Speech()
      {
        if (Clip != null)
          UnityObjectUtil.DestroyUnityObject(Clip);
      }

      public bool Ready { get; set; }
      public AudioClip Clip { get; set; }

      public Speech(TextToSpeech textToSpeech, string text, bool usePost)
      {
        textToSpeech.ToSpeech(text, OnAudioClip, usePost);
      }

      private void OnAudioClip(AudioClip clip)
      {
        Clip = clip;
        Ready = true;
      }

    };

    private Queue<Speech> m_SpeechQueue = new Queue<Speech>();
    private Speech m_ActiveSpeech = null;
    #endregion

    #region Public Memebers

    /// <summary>
    /// Gets or sets the voice. Default voice is English, US - Michael
    /// </summary>
    /// <value>The voice.</value>
    public VoiceType Voice
    {
      get
      {
        return m_Voice;
      }
      set
      {
        m_Voice = value;
      }
    }

    #endregion

    #region Event Handlers
    /// <summary>
    /// Button event handler.
    /// </summary>
    public void OnTextToSpeech()
    {
      if (m_TextToSpeech.Voice != m_Voice)
        m_TextToSpeech.Voice = m_Voice;
      if (m_Input != null)
        m_SpeechQueue.Enqueue(new Speech(m_TextToSpeech, m_Input.text, m_UsePost));
      if (m_StatusText != null)
        m_StatusText.text = "THINKING";
      if (m_TextToSpeechButton != null)
        m_TextToSpeechButton.interactable = false;
    }
    #endregion

    #region Private Functions
    private void OnTextInput(Data data)
    {
      TextToSpeechData text = data as TextToSpeechData;
      if (text == null)
        throw new WatsonException("Wrong data type received.");

      if (!string.IsNullOrEmpty(text.Text))
      {
        if (m_TextToSpeech.Voice != m_Voice)
          m_TextToSpeech.Voice = m_Voice;

        m_SpeechQueue.Enqueue(new Speech(m_TextToSpeech, text.Text, m_UsePost));
      }
    }

    private void OnVoiceSelect(Data data)
    {
      VoiceData voice = data as VoiceData;
      if (voice == null)
        throw new WatsonException("Unexpected data type");

      m_Voice = voice.Voice;
    }

    private void OnEnable()
    {
      UnityObjectUtil.StartDestroyQueue();

      if (m_StatusText != null)
        m_StatusText.text = "READY";
    }

    /// <exclude />
    protected override void Start()
    {
      base.Start();
      m_Source = GetComponent<AudioSource>();
    }

    private void Update()
    {
      if (m_Source != null && !m_Source.isPlaying
          && m_SpeechQueue.Count > 0
          && m_SpeechQueue.Peek().Ready)
      {
        CancelInvoke("OnEndSpeech");

        m_ActiveSpeech = m_SpeechQueue.Dequeue();
        if (m_ActiveSpeech.Clip != null)
        {
          if (m_Speaking.IsConnected)
            m_Speaking.SendData(new SpeakingStateData(true));
          if (m_DisableMic.IsConnected)
            m_DisableMic.SendData(new DisableMicData(true));

          m_Source.spatialBlend = 0.0f;     // 2D sound
          m_Source.loop = false;            // do not loop
          m_Source.clip = m_ActiveSpeech.Clip;             // clip
          m_Source.Play();

          Invoke("OnEndSpeech", ((float)m_ActiveSpeech.Clip.samples / (float)m_ActiveSpeech.Clip.frequency) + 0.1f);
          if (m_LevelOut.IsConnected)
          {
            m_LastPlayPos = 0;
            InvokeRepeating("OnLevelOut", m_LevelOutInterval, m_LevelOutInterval);
          }
        }
        else
        {
          Log.Warning("TextToSpeechWidget", "Skipping null AudioClip");
        }
      }

      if (m_TextToSpeechButton != null)
        m_TextToSpeechButton.interactable = true;
      if (m_StatusText != null)
        m_StatusText.text = "READY";
    }

    private void OnLevelOut()
    {
      if (m_Source != null && m_Source.isPlaying)
      {
        int currentPos = m_Source.timeSamples;
        if (currentPos > m_LastPlayPos)
        {
          float[] samples = new float[currentPos - m_LastPlayPos];
          m_Source.clip.GetData(samples, m_LastPlayPos);
          m_LevelOut.SendData(new LevelData(Mathf.Max(samples) * m_LevelOutputModifier, m_LevelOutputModifier));
          m_LastPlayPos = currentPos;
        }
      }
      else
        CancelInvoke("OnLevelOut");
    }
    private void OnEndSpeech()
    {
      if (m_Speaking.IsConnected)
        m_Speaking.SendData(new SpeakingStateData(false));
      if (m_DisableMic.IsConnected)
        m_DisableMic.SendData(new DisableMicData(false));
      if (m_Source.isPlaying)
        m_Source.Stop();

      m_ActiveSpeech = null;
    }

    /// <exclude />
    protected override string GetName()
    {
      return "TextToSpeech";
    }
    #endregion
  }

}
