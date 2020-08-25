/**
* (C) Copyright IBM Corp. 2018, 2020.
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
    /// Information about a request to register a callback for asynchronous speech recognition.
    /// </summary>
    public class RegisterStatus
    {
        /// <summary>
        /// The current status of the job:
        /// * `created`: The service successfully allowlisted the callback URL as a result of the call.
        /// * `already created`: The URL was already allowlisted.
        /// </summary>
        public class StatusValue
        {
            /// <summary>
            /// Constant CREATED for created
            /// </summary>
            public const string CREATED = "created";
            /// <summary>
            /// Constant ALREADY_CREATED for already created
            /// </summary>
            public const string ALREADY_CREATED = "already created";
            
        }

        /// <summary>
        /// The current status of the job:
        /// * `created`: The service successfully allowlisted the callback URL as a result of the call.
        /// * `already created`: The URL was already allowlisted.
        /// Constants for possible values can be found using RegisterStatus.StatusValue
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        /// <summary>
        /// The callback URL that is successfully registered.
        /// </summary>
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }
    }
}
