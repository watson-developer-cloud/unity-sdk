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

using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Services.NaturalLanguageClassifier.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Widgets;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.DataTypes
{
  /// <summary>
  /// This data class is for text data to spoken by the TextToSpeech widget.
  /// </summary>
  public class TextToSpeechData : Widget.Data
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public TextToSpeechData()
    { }
    /// <summary>
    /// String constructor.
    /// </summary>
    /// <param name="text"></param>
    public TextToSpeechData(string text)
    {
      Text = text;
    }
    /// <summary>
    /// Name of this data type.
    /// </summary>
    /// <returns>A human readable name for this data type.</returns>
    public override string GetName()
    {
      return "TextToSpeech";
    }

    /// <summary>
    /// The text to convert to speech.
    /// </summary>
    public string Text { get; set; }
  };

  /// <summary>
  /// Data type for a source language change.
  /// </summary>
  public class LanguageData : Widget.Data
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public LanguageData()
    { }

    /// <summary>
    /// Constructor which takes the language ID as a string.
    /// </summary>
    /// <param name="language">The language ID.</param>
    public LanguageData(string language)
    {
      Language = language;
    }

    /// <exclude />
    public override string GetName()
    {
      return "Language";
    }

    /// <summary>
    /// The language ID.
    /// </summary>
    public string Language { get; set; }
  };

  /// <summary>
  /// Data type sent to change the voice type.
  /// </summary>
  public class VoiceData : Widget.Data
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public VoiceData()
    { }
    /// <summary>
    /// Constructor which takes the voice type enumeration.
    /// </summary>
    /// <param name="voice">The voice to select.</param>
    public VoiceData(VoiceType voice)
    {
      Voice = voice;
    }

    /// <exclude />
    public override string GetName()
    {
      return "Voice";
    }

    /// <summary>
    /// The enumeration of the voice to select.
    /// </summary>
    public VoiceType Voice { get; set; }
  };

  /// <summary>
  /// This class holds a AudioClip and maximum sample level.
  /// </summary>
  public class AudioData : Widget.Data
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public AudioData()
    { }

    /// <exclude />
    ~AudioData()
    {
      UnityObjectUtil.DestroyUnityObject(Clip);
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="clip">The AudioClip.</param>
    /// <param name="maxLevel">The maximum sample level in the audio clip.</param>
    public AudioData(AudioClip clip, float maxLevel)
    {
      Clip = clip;
      MaxLevel = maxLevel;
    }
    /// <summary>
    /// Name of this data type.
    /// </summary>
    /// <returns>The readable name.</returns>
    public override string GetName()
    {
      return "Audio";
    }

    /// <summary>
    /// The AudioClip.
    /// </summary>
    public AudioClip Clip { get; set; }
    /// <summary>
    /// The maximum level in the audio clip.
    /// </summary>
    public float MaxLevel { get; set; }

  };

  /// <summary>
  /// This class holds a reference to WebCamTexture.
  /// </summary>
  public class WebCamTextureData : Widget.Data
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public WebCamTextureData()
    { }

    /// <exclude />
    //~WebCamTextureData()
    //{
    //	UnityObjectUtil.DestroyUnityObject(CamTexture);
    //}

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="camTexture">The WebCamTexture.</param>
    public WebCamTextureData(WebCamTexture camTexture, int requestedWidth = 640, int requestedHeight = 480, int requestedFPS = 60)
    {
      CamTexture = camTexture;
      RequestedWidth = requestedWidth;
      RequestedHeight = requestedHeight;
      RequestedFPS = requestedFPS;
    }

    /// <summary>
    /// Name of this data type.
    /// </summary>
    /// <returns>The readable name.</returns>
    public override string GetName()
    {
      return "WebCamTexture";
    }

    /// <summary>
    /// The WebCamTexture.
    /// </summary>
    public WebCamTexture CamTexture { get; set; }
    /// <summary>
    /// The Requested Width of the WebCamTexture
    /// </summary>
    public int RequestedWidth { get; set; }
    /// <summary>
    /// The Requested Height of the WebCamTexture.
    /// </summary>
    public int RequestedHeight { get; set; }
    /// <summary>
    /// The Requested FPS of the WebCamTexture.
    /// </summary>
    public int RequestedFPS { get; set; }

  }
  /// <summary>
  /// This class holds a boolean value.
  /// </summary>
  public class BooleanData : Widget.Data
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public BooleanData()
    { }
    /// <summary>
    /// Data constructor.
    /// </summary>
    /// <param name="b"></param>
    public BooleanData(bool b)
    {
      Boolean = b;
    }
    /// <summary>
    /// Name of this data type.
    /// </summary>
    /// <returns>The readable name.</returns>
    public override string GetName()
    {
      return "Boolean";
    }

    /// <summary>
    /// The bool value.
    /// </summary>
    public bool Boolean { get; set; }
  };

  /// <summary>
  /// Boolean state sent when TextToSpeech starts and ends playing audio.
  /// </summary>
  public class SpeakingStateData : Widget.Data
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public SpeakingStateData()
    { }
    /// <summary>
    /// Data constructor.
    /// </summary>
    /// <param name="b">The speaking state, true for speaking, false for not.</param>
    public SpeakingStateData(bool b)
    {
      Boolean = b;
    }
    /// <summary>
    /// Name of this data type.
    /// </summary>
    /// <returns>The readable name.</returns>
    public override string GetName()
    {
      return "Speaking";
    }

    /// <summary>
    /// Speaking state property. True if speaking, false if not.
    /// </summary>
    public bool Boolean { get; set; }
  }

  /// <summary>
  /// Boolean state for disabling the microphone input.
  /// </summary>
  public class DisableMicData : Widget.Data
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public DisableMicData()
    { }
    /// <summary>
    /// Data constructor.
    /// </summary>
    /// <param name="b">Disable microphone state.</param>
    public DisableMicData(bool b)
    {
      Boolean = b;
    }
    /// <summary>
    /// Name of this data type.
    /// </summary>
    /// <returns>The readable name.</returns>
    public override string GetName()
    {
      return "DisableMic";
    }
    /// <summary>
    /// Disable microphone state, true for disabled, false for not.
    /// </summary>
    public bool Boolean { get; set; }
  }

  /// <summary>
  /// Boolean state for disabling the WebCam input.
  /// </summary>
  public class DisableWebCamData : Widget.Data
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public DisableWebCamData()
    { }
    /// <summary>
    /// Data constructor.
    /// </summary>
    /// <param name="b">Disable WebCam state.</param>
    public DisableWebCamData(bool b)
    {
      Boolean = b;
    }
    /// <summary>
    /// Name of this data type.
    /// </summary>
    /// <returns>The readable name.</returns>
    public override string GetName()
    {
      return "DisableWebCam";
    }
    /// <summary>
    /// Disable WebCam state, true for disabled, false for not.
    /// </summary>
    public bool Boolean { get; set; }
  }

  /// <summary>
  /// Boolean state for disabling the CloseCaption output.
  /// </summary>
  public class DisableCloseCaptionData : Widget.Data
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public DisableCloseCaptionData()
    { }
    /// <summary>
    /// Data constructor.
    /// </summary>
    /// <param name="b">Disable DisableCloseCaptionData state.</param>
    public DisableCloseCaptionData(bool b)
    {
      Boolean = b;
    }
    /// <summary>
    /// Name of this data type.
    /// </summary>
    /// <returns>The readable name.</returns>
    public override string GetName()
    {
      return "DisableCloseCaptionData";
    }
    /// <summary>
    /// Disable DisableCloseCaptionData state, true for disabled, false for not.
    /// </summary>
    public bool Boolean { get; set; }
  }

  /// <summary>
  /// This class is for audio level data.
  /// </summary>
  public class LevelData : Widget.Data
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public LevelData()
    { }
    /// <summary>
    /// Data constructor.
    /// </summary>
    /// <param name="f">The level data.</param>
    public LevelData(float f, float m = 1.0f)
    {
      Float = f;
      Modifier = m;
    }
    /// <summary>
    /// Name of this data type.
    /// </summary>
    /// <returns>The readable name.</returns>
    public override string GetName()
    {
      return "Level";
    }
    /// <summary>
    /// The level data.
    /// </summary>
    public float Float { get; set; }

    /// <summary>
    /// Modifier of Level data. Default is 1.0
    /// </summary>
    /// <value>The modifier.</value>
    public float Modifier { get; set; }

    public float NormalizedFloat { get { if (Modifier == 0.0f) Modifier = 1.0f; return Float / Modifier; } }
  };

  /// <summary>
  /// This class is for SpeechToText results.
  /// </summary>
  public class SpeechToTextData : Widget.Data
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public SpeechToTextData()
    { }
    /// <summary>
    /// Data constructor.
    /// </summary>
    /// <param name="result">The SpeechToText results.</param>
    public SpeechToTextData(SpeechRecognitionEvent result)
    {
      Results = result;
    }
    /// <summary>
    /// Name of this data type.
    /// </summary>
    /// <returns>The readable name.</returns>
    public override string GetName()
    {
      return "SpeechToText";
    }
    /// <summary>
    /// The Result object.
    /// </summary>
    public SpeechRecognitionEvent Results { get; set; }

    /// <summary>
    /// Gets a value indicating whether the result text is final.
    /// </summary>
    /// <value><c>true</c> if the result text is final; otherwise, <c>false</c>.</value>
    public bool IsFinal
    {
      get
      {
        bool isFinal = false;
        if (Results != null && Results.results.Length > 0)
        {
          isFinal = Results.results[0].final;
        }

        return isFinal;
      }
    }

    public string _Text = null;
    /// <summary>
    /// Gets the highest confident text.
    /// </summary>
    /// <value>The text with highest confidence or final text</value>
    public string Text
    {
      get
      {
        if (string.IsNullOrEmpty(_Text))
        {
          if (Results != null && Results.results.Length > 0)
          {
            SpeechRecognitionResult speechResult = Results.results[0];
            if (speechResult.alternatives != null && speechResult.alternatives.Length > 0)
            {
              _Text = speechResult.alternatives[0].transcript;
            }
          }
        }
        return _Text;

      }

    }
  };

  /// <summary>
  /// This class is for Natural Language Classify results.
  /// </summary>
  public class ClassifyResultData : Widget.Data
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public ClassifyResultData()
    { }
    /// <summary>
    /// Data constructor.
    /// </summary>
    /// <param name="result">The ClassifyResult object.</param>
    public ClassifyResultData(ClassifyResult result)
    {
      Result = result;
    }

    /// <summary>
    /// Name of this data type.
    /// </summary>
    /// <returns>The readable name.</returns>
    public override string GetName()
    {
      return "ClassifyResult";
    }
    /// <summary>
    /// The ClassifyResult object.
    /// </summary>
    public ClassifyResult Result { get; set; }
  };
}
