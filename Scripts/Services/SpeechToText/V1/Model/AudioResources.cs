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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IBM.Watson.SpeechToText.V1.Model
{
    /// <summary>
    /// Information about the audio resources from a custom acoustic model.
    /// </summary>
    public class AudioResources
    {
        /// <summary>
        /// The total minutes of accumulated audio summed over all of the valid audio resources for the custom acoustic
        /// model. You can use this value to determine whether the custom model has too little or too much audio to
        /// begin training.
        /// </summary>
        [JsonProperty("total_minutes_of_audio", NullValueHandling = NullValueHandling.Ignore)]
        public double? TotalMinutesOfAudio { get; set; }
        /// <summary>
        /// An array of `AudioResource` objects that provides information about the audio resources of the custom
        /// acoustic model. The array is empty if the custom model has no audio resources.
        /// </summary>
        [JsonProperty("audio", NullValueHandling = NullValueHandling.Ignore)]
        public List<AudioResource> Audio { get; set; }
    }
}
