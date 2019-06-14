/**
* Copyright 2018, 2019 IBM Corp. All Rights Reserved.
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

using Newtonsoft.Json;

namespace IBM.Watson.TextToSpeech.V1.Model
{
    /// <summary>
    /// Additional service features that are supported with the voice.
    /// </summary>
    public class SupportedFeatures
    {
        /// <summary>
        /// If `true`, the voice can be customized; if `false`, the voice cannot be customized. (Same as
        /// `customizable`.).
        /// </summary>
        [JsonProperty("custom_pronunciation", NullValueHandling = NullValueHandling.Ignore)]
        public bool? CustomPronunciation { get; set; }
        /// <summary>
        /// If `true`, the voice can be transformed by using the SSML &lt;voice-transformation&gt; element; if `false`,
        /// the voice cannot be transformed.
        /// </summary>
        [JsonProperty("voice_transformation", NullValueHandling = NullValueHandling.Ignore)]
        public bool? VoiceTransformation { get; set; }
    }
}
