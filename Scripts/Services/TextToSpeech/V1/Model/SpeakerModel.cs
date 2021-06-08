/**
* (C) Copyright IBM Corp. 2021.
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
    /// The speaker ID of the speaker model.
    /// </summary>
    public class SpeakerModel
    {
        /// <summary>
        /// The speaker ID (GUID) of the speaker model.
        /// </summary>
        [JsonProperty("speaker_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SpeakerId { get; set; }
    }
}
