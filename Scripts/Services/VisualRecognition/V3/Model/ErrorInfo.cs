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

namespace IBM.Watson.VisualRecognition.V3.Model
{
    /// <summary>
    /// Information about what might have caused a failure, such as an image that is too large. Not returned when there
    /// is no error.
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// HTTP status code.
        /// </summary>
        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        public long? Code { get; set; }
        /// <summary>
        /// Human-readable error description. For example, `File size limit exceeded`.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        /// <summary>
        /// Codified error string. For example, `limit_exceeded`.
        /// </summary>
        [JsonProperty("error_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorId { get; set; }
    }
}
