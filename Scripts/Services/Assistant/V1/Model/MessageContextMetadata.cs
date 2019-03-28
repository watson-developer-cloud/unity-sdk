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

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// Metadata related to the message.
    /// </summary>
    public class MessageContextMetadata
    {
        /// <summary>
        /// A label identifying the deployment environment, used for filtering log data. This string cannot contain
        /// carriage return, newline, or tab characters.
        /// </summary>
        [JsonProperty("deployment", NullValueHandling = NullValueHandling.Ignore)]
        public string Deployment { get; set; }
        /// <summary>
        /// A string value that identifies the user who is interacting with the workspace. The client must provide a
        /// unique identifier for each individual end user who accesses the application. For Plus and Premium plans,
        /// this user ID is used to identify unique users for billing purposes. This string cannot contain carriage
        /// return, newline, or tab characters.
        /// </summary>
        [JsonProperty("user_id", NullValueHandling = NullValueHandling.Ignore)]
        public string UserId { get; set; }
    }
}
