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
    /// Additional service features that are supported with the model.
    /// </summary>
    public class SupportedFeatures
    {
        /// <summary>
        /// Indicates whether the customization interface can be used to create a custom language model based on the
        /// language model.
        /// </summary>
        [JsonProperty("custom_language_model", NullValueHandling = NullValueHandling.Ignore)]
        public bool? CustomLanguageModel { get; set; }
        /// <summary>
        /// Indicates whether the `speaker_labels` parameter can be used with the language model.
        /// </summary>
        [JsonProperty("speaker_labels", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SpeakerLabels { get; set; }
    }
}
