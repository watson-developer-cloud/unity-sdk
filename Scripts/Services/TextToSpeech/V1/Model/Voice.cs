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
    /// Information about an available voice model.
    /// </summary>
    public class Voice
    {
        /// <summary>
        /// The URI of the voice.
        /// </summary>
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }
        /// <summary>
        /// The gender of the voice: `male` or `female`.
        /// </summary>
        [JsonProperty("gender", NullValueHandling = NullValueHandling.Ignore)]
        public string Gender { get; set; }
        /// <summary>
        /// The name of the voice. Use this as the voice identifier in all requests.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// The language and region of the voice (for example, `en-US`).
        /// </summary>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }
        /// <summary>
        /// A textual description of the voice.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        /// <summary>
        /// If `true`, the voice can be customized; if `false`, the voice cannot be customized. (Same as
        /// `custom_pronunciation`; maintained for backward compatibility.).
        /// </summary>
        [JsonProperty("customizable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Customizable { get; set; }
        /// <summary>
        /// Additional service features that are supported with the voice.
        /// </summary>
        [JsonProperty("supported_features", NullValueHandling = NullValueHandling.Ignore)]
        public SupportedFeatures SupportedFeatures { get; set; }
        /// <summary>
        /// Returns information about a specified custom voice model. This field is returned only by the **Get a voice**
        /// method and only when you specify the customization ID of a custom voice model.
        /// </summary>
        [JsonProperty("customization", NullValueHandling = NullValueHandling.Ignore)]
        public VoiceModel Customization { get; set; }
    }
}
