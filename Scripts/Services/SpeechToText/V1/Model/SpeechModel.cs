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

namespace IBM.Watson.SpeechToText.V1.Model
{
    /// <summary>
    /// Information about an available language model.
    /// </summary>
    public class SpeechModel
    {
        /// <summary>
        /// The name of the model for use as an identifier in calls to the service (for example,
        /// `en-US_BroadbandModel`).
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// The language identifier of the model (for example, `en-US`).
        /// </summary>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }
        /// <summary>
        /// The sampling rate (minimum acceptable rate for audio) used by the model in Hertz.
        /// </summary>
        [JsonProperty("rate", NullValueHandling = NullValueHandling.Ignore)]
        public long? Rate { get; set; }
        /// <summary>
        /// The URI for the model.
        /// </summary>
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }
        /// <summary>
        /// Additional service features that are supported with the model.
        /// </summary>
        [JsonProperty("supported_features", NullValueHandling = NullValueHandling.Ignore)]
        public SupportedFeatures SupportedFeatures { get; set; }
        /// <summary>
        /// A brief description of the model.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
    }
}
