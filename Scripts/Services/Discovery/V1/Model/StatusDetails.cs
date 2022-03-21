/**
* (C) Copyright IBM Corp. 2021, 2022.
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

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// Object that contains details about the status of the authentication process.
    /// </summary>
    public class StatusDetails
    {
        /// <summary>
        /// Indicates whether the credential is accepted by the target data source.
        /// </summary>
        [JsonProperty("authenticated", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Authenticated { get; set; }
        /// <summary>
        /// If `authenticated` is `false`, a message describes why authentication is unsuccessful.
        /// </summary>
        [JsonProperty("error_message", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage { get; set; }
    }
}
