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
* @author Richard Lyle (rolyle@us.ibm.com)
*/

using IBM.Watson.Services.v1;
using IBM.Watson.Widgets;
using UnityEngine;

/// <summary>
/// Data classes for holding data for services and widgets.
/// </summary>
namespace IBM.Watson.Data
{
    /// <summary>
    /// This data class is for text data.
    /// </summary>
    public class TextData : Widget.Data
    {
        public TextData()
        { }
        public TextData(string text)
        {
            Text = text;
        }
        public override string GetName()
        {
            return "Text";
        }

        public string Text { get; set; }
    };

    /// <summary>
    /// This class holds a AudioClip and maximum sample level.
    /// </summary>
    public class AudioData : Widget.Data
    {
        public AudioData()
        { }
        public AudioData(AudioClip clip, float maxLevel)
        {
            Clip = clip;
            MaxLevel = maxLevel;
        }
        public override string GetName()
        {
            return "Audio";
        }

        public AudioClip Clip { get; set; }
        public float MaxLevel { get; set; }
    };

    /// <summary>
    /// This class holds a boolean value.
    /// </summary>
    public class BooleanData : Widget.Data
    {
        public BooleanData()
        { }
        public BooleanData(bool b)
        {
            Boolean = b;
        }
        public override string GetName()
        {
            return "Boolean";
        }

        public bool Boolean { get; set; }
    };

    public class SpeakingStateData : Widget.Data
    {
        public SpeakingStateData()
        { }
        public SpeakingStateData( bool b ) 
        {
            Boolean = b;
        }
        public override string GetName()
        {
            return "Speaking";
        }
        public bool Boolean { get; set; }
    }

    public class DisableMicData : Widget.Data
    {
        public DisableMicData()
        { }
        public DisableMicData( bool b ) 
        {
            Boolean = b;
        }
        public override string GetName()
        {
            return "DisableMic";
        }
        public bool Boolean { get; set; }
    }

    /// <summary>
    /// This class is for float data.
    /// </summary>
    public class FloatData : Widget.Data
    {
        public FloatData()
        { }
        public FloatData( float f )
        {
            Float = f;
        }
        public override string GetName()
        {
            return "Float";
        }

        public float Float { get; set; }
    };

    /// <summary>
    /// This class is for SpeechToText results.
    /// </summary>
    public class SpeechToTextData : Widget.Data
    {
        public SpeechToTextData()
        { }
        public SpeechToTextData(SpeechResultList result)
        {
            Results = result;
        }
        public override string GetName()
        {
            return "SpeechToText";
        }

        public SpeechResultList Results { get; set; }
    };

    /// <summary>
    /// This class is for NLC classify results.
    /// </summary>
    public class ClassifyResultData : Widget.Data
    {
        public ClassifyResultData()
        { }
        public ClassifyResultData( ClassifyResult result )
        {
            Result = result;
        }

        public override string GetName()
        {
            return "ClassifyResult";
        }

        public ClassifyResult Result { get; set; }
    };
}
