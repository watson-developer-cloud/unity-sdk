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

using FullSerializer;

namespace IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1
{
    /// <summary>
    /// Audio format types that can be requested from the service.
    /// </summary>
    public enum AudioFormatType
    {
        /// <summary>
        /// OGG Vorbis format
        /// </summary>
        OGG = 0,
        /// <summary>
        /// Linear PCM format.
        /// </summary>
        WAV,                     //Currently used
        /// <summary>
        /// FLAC audio format.
        /// </summary>
        FLAC
    };

    /// <summary>
    /// The available voices for synthesized speech.
    /// </summary>
    public enum VoiceType
    {
        /// <summary>
        /// US male voice.
        /// </summary>
        en_US_Michael = 0,
        /// <summary>
        /// US female voice.
        /// </summary>
        en_US_Lisa,
        /// <summary>
        /// US female voice.
        /// </summary>
        en_US_Allison,
        /// <summary>
        /// Great Britan female voice.
        /// </summary>
        en_GB_Kate,
        /// <summary>
        /// Spanish male voice.
        /// </summary>
        es_ES_Enrique,
        /// <summary>
        /// Spanish female voice.
        /// </summary>
        es_ES_Laura,
        /// <summary>
        /// US female voice.
        /// </summary>
        es_US_Sofia,
        /// <summary>
        /// German male voice.
        /// </summary>
        de_DE_Dieter,
        /// <summary>
        /// German female voice.
        /// </summary>
        de_DE_Birgit,
        /// <summary>
        /// French female voice.
        /// </summary>
        fr_FR_Renee,
        /// <summary>
        /// Italian female voice.
        /// </summary>
        it_IT_Francesca,
        /// <summary>
        /// Japanese female voice.
        /// </summary>
        ja_JP_Emi,
    };

    /// <summary>
    /// A voice model object for TextToSpeech.
    /// </summary>
    [fsObject]
    public class Voice
    {
        /// <summary>
        /// The name of the voice model.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The language ID of this voice model.
        /// </summary>
        public string language { get; set; }
        /// <summary>
        /// The gender of the voice model.
        /// </summary>
        public string gender { get; set; }
        /// <summary>
        /// The URL of the voice model.
        /// </summary>
        public string url { get; set; }
    };

    /// <summary>
    /// This object contains a list of voices.
    /// </summary>
    [fsObject]
    public class Voices
    {
        /// <summary>
        /// The array of Voice objects.
        /// </summary>
        public Voice[] voices { get; set; }
    };
}
