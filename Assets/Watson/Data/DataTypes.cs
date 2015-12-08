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


using IBM.Watson.DataModels;
using IBM.Watson.Services.v1;
using IBM.Watson.Utilities;
using IBM.Watson.Widgets;
using UnityEngine;
using System;

/// <summary>
/// Data classes for holding data for services and widgets.
/// </summary>
namespace IBM.Watson.DataTypes
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

    public class VoiceData : Widget.Data
    {
        public VoiceData()
        { }
        public VoiceData( TextToSpeech.VoiceType voice )
        {
            Voice = voice;
        }

        public override string GetName()
        {
            return "Voice";
        }

        public TextToSpeech.VoiceType Voice { get; set; }
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
        ~AudioData()
        {
            AudioClipUtil.DestroyAudioClip( Clip );
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
        public SpeakingStateData( bool b ) 
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
        public DisableMicData( bool b ) 
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
        public LevelData( float f )
        {
            Float = f;
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
        public SpeechToTextData(SpeechResultList result)
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
        public SpeechResultList Results { get; set; }
    };

    /// <summary>
    /// This class is for NLC classify results.
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
        public ClassifyResultData( ClassifyResult result )
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
