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
    /// If audio metrics are requested, information about the signal characteristics of the input audio.
    /// </summary>
    public class AudioMetrics
    {
        /// <summary>
        /// The interval in seconds (typically 0.1 seconds) at which the service calculated the audio metrics. In other
        /// words, how often the service calculated the metrics. A single unit in each histogram (see the
        /// `AudioMetricsHistogramBin` object) is calculated based on a `sampling_interval` length of audio.
        /// </summary>
        [JsonProperty("sampling_interval", NullValueHandling = NullValueHandling.Ignore)]
        public float? SamplingInterval { get; set; }
        /// <summary>
        /// Detailed information about the signal characteristics of the input audio.
        /// </summary>
        [JsonProperty("accumulated", NullValueHandling = NullValueHandling.Ignore)]
        public AudioMetricsDetails Accumulated { get; set; }
    }
}
