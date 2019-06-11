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
    /// Detailed information about the signal characteristics of the input audio.
    /// </summary>
    public class AudioMetricsDetails
    {
        /// <summary>
        /// If `true`, indicates the end of the audio stream, meaning that transcription is complete. Currently, the
        /// field is always `true`. The service returns metrics just once per audio stream. The results provide
        /// aggregated audio metrics that pertain to the complete audio stream.
        /// </summary>
        [JsonProperty("final", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Final { get; set; }
        /// <summary>
        /// The end time in seconds of the block of audio to which the metrics apply.
        /// </summary>
        [JsonProperty("end_time", NullValueHandling = NullValueHandling.Ignore)]
        public float? EndTime { get; set; }
        /// <summary>
        /// The signal-to-noise ratio (SNR) for the audio signal. The value indicates the ratio of speech to noise in
        /// the audio. A valid value lies in the range of 0 to 100 decibels (dB). The service omits the field if it
        /// cannot compute the SNR for the audio.
        /// </summary>
        [JsonProperty("signal_to_noise_ratio", NullValueHandling = NullValueHandling.Ignore)]
        public float? SignalToNoiseRatio { get; set; }
        /// <summary>
        /// The ratio of speech to non-speech segments in the audio signal. The value lies in the range of 0.0 to 1.0.
        /// </summary>
        [JsonProperty("speech_ratio", NullValueHandling = NullValueHandling.Ignore)]
        public float? SpeechRatio { get; set; }
        /// <summary>
        /// The probability that the audio signal is missing the upper half of its frequency content.
        /// * A value close to 1.0 typically indicates artificially up-sampled audio, which negatively impacts the
        /// accuracy of the transcription results.
        /// * A value at or near 0.0 indicates that the audio signal is good and has a full spectrum.
        /// * A value around 0.5 means that detection of the frequency content is unreliable or not available.
        /// </summary>
        [JsonProperty("high_frequency_loss", NullValueHandling = NullValueHandling.Ignore)]
        public float? HighFrequencyLoss { get; set; }
        /// <summary>
        /// An array of `AudioMetricsHistogramBin` objects that defines a histogram of the cumulative direct current
        /// (DC) component of the audio signal.
        /// </summary>
        [JsonProperty("direct_current_offset", NullValueHandling = NullValueHandling.Ignore)]
        public List<AudioMetricsHistogramBin> DirectCurrentOffset { get; set; }
        /// <summary>
        /// An array of `AudioMetricsHistogramBin` objects that defines a histogram of the clipping rate for the audio
        /// segments. The clipping rate is defined as the fraction of samples in the segment that reach the maximum or
        /// minimum value that is offered by the audio quantization range. The service auto-detects either a 16-bit
        /// Pulse-Code Modulation(PCM) audio range (-32768 to +32767) or a unit range (-1.0 to +1.0). The clipping rate
        /// is between 0.0 and 1.0, with higher values indicating possible degradation of speech recognition.
        /// </summary>
        [JsonProperty("clipping_rate", NullValueHandling = NullValueHandling.Ignore)]
        public List<AudioMetricsHistogramBin> ClippingRate { get; set; }
        /// <summary>
        /// An array of `AudioMetricsHistogramBin` objects that defines a histogram of the signal level in segments of
        /// the audio that contain speech. The signal level is computed as the Root-Mean-Square (RMS) value in a decibel
        /// (dB) scale normalized to the range 0.0 (minimum level) to 1.0 (maximum level).
        /// </summary>
        [JsonProperty("speech_level", NullValueHandling = NullValueHandling.Ignore)]
        public List<AudioMetricsHistogramBin> SpeechLevel { get; set; }
        /// <summary>
        /// An array of `AudioMetricsHistogramBin` objects that defines a histogram of the signal level in segments of
        /// the audio that do not contain speech. The signal level is computed as the Root-Mean-Square (RMS) value in a
        /// decibel (dB) scale normalized to the range 0.0 (minimum level) to 1.0 (maximum level).
        /// </summary>
        [JsonProperty("non_speech_level", NullValueHandling = NullValueHandling.Ignore)]
        public List<AudioMetricsHistogramBin> NonSpeechLevel { get; set; }
    }
}
