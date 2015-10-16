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
using UnityEngine;

namespace IBM.Watson.Widgets
{
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

    public class SpeechToTextData : Widget.Data
    {
        public SpeechToTextData()
        { }
        public SpeechToTextData(SpeechToText.ResultList result)
        {
            Results = result;
        }
        public override string GetName()
        {
            return "SpeechToText";
        }

        public SpeechToText.ResultList Results { get; set; }
    };
}
