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
    /// Information about an audio resource from a custom acoustic model.
    /// </summary>
    public class AudioDetails
    {
        /// <summary>
        /// The type of the audio resource:
        /// * `audio` for an individual audio file
        /// * `archive` for an archive (**.zip** or **.tar.gz**) file that contains audio files
        /// * `undetermined` for a resource that the service cannot validate (for example, if the user mistakenly passes
        /// a file that does not contain audio, such as a JPEG file).
        /// </summary>
        public class TypeValue
        {
            /// <summary>
            /// Constant AUDIO for audio
            /// </summary>
            public const string AUDIO = "audio";
            /// <summary>
            /// Constant ARCHIVE for archive
            /// </summary>
            public const string ARCHIVE = "archive";
            /// <summary>
            /// Constant UNDETERMINED for undetermined
            /// </summary>
            public const string UNDETERMINED = "undetermined";
            
        }

        /// <summary>
        /// **For an archive-type resource,** the format of the compressed archive:
        /// * `zip` for a **.zip** file
        /// * `gzip` for a **.tar.gz** file
        ///
        /// Omitted for an audio-type resource.
        /// </summary>
        public class CompressionValue
        {
            /// <summary>
            /// Constant ZIP for zip
            /// </summary>
            public const string ZIP = "zip";
            /// <summary>
            /// Constant GZIP for gzip
            /// </summary>
            public const string GZIP = "gzip";
            
        }

        /// <summary>
        /// The type of the audio resource:
        /// * `audio` for an individual audio file
        /// * `archive` for an archive (**.zip** or **.tar.gz**) file that contains audio files
        /// * `undetermined` for a resource that the service cannot validate (for example, if the user mistakenly passes
        /// a file that does not contain audio, such as a JPEG file).
        /// Constants for possible values can be found using AudioDetails.TypeValue
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
        /// <summary>
        /// **For an archive-type resource,** the format of the compressed archive:
        /// * `zip` for a **.zip** file
        /// * `gzip` for a **.tar.gz** file
        ///
        /// Omitted for an audio-type resource.
        /// Constants for possible values can be found using AudioDetails.CompressionValue
        /// </summary>
        [JsonProperty("compression", NullValueHandling = NullValueHandling.Ignore)]
        public string Compression { get; set; }
        /// <summary>
        /// **For an audio-type resource,** the codec in which the audio is encoded. Omitted for an archive-type
        /// resource.
        /// </summary>
        [JsonProperty("codec", NullValueHandling = NullValueHandling.Ignore)]
        public string Codec { get; set; }
        /// <summary>
        /// **For an audio-type resource,** the sampling rate of the audio in Hertz (samples per second). Omitted for an
        /// archive-type resource.
        /// </summary>
        [JsonProperty("frequency", NullValueHandling = NullValueHandling.Ignore)]
        public long? Frequency { get; set; }
    }
}
