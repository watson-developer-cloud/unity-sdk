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
    /// Information about a custom prompt.
    /// </summary>
    public class Prompt
    {
        /// <summary>
        /// The user-specified text of the prompt.
        /// </summary>
        [JsonProperty("prompt", NullValueHandling = NullValueHandling.Ignore)]
        public string _Prompt { get; set; }
        /// <summary>
        /// The user-specified identifier (name) of the prompt.
        /// </summary>
        [JsonProperty("prompt_id", NullValueHandling = NullValueHandling.Ignore)]
        public string PromptId { get; set; }
        /// <summary>
        /// The status of the prompt:
        /// * `processing`: The service received the request to add the prompt and is analyzing the validity of the
        /// prompt.
        /// * `available`: The service successfully validated the prompt, which is now ready for use in a speech
        /// synthesis request.
        /// * `failed`: The service's validation of the prompt failed. The status of the prompt includes an `error`
        /// field that describes the reason for the failure.
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        /// <summary>
        /// If the status of the prompt is `failed`, an error message that describes the reason for the failure. The
        /// field is omitted if no error occurred.
        /// </summary>
        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public string Error { get; set; }
        /// <summary>
        /// The speaker ID (GUID) of the speaker for which the prompt was defined. The field is omitted if no speaker ID
        /// was specified.
        /// </summary>
        [JsonProperty("speaker_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SpeakerId { get; set; }
    }
}
