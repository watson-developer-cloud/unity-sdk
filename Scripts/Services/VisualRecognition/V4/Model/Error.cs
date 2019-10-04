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

namespace IBM.Watson.VisualRecognition.V4.Model
{
    /// <summary>
    /// Details about an error.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Identifier of the problem.
        /// </summary>
        public class CodeValue
        {
            /// <summary>
            /// Constant INVALID_FIELD for invalid_field
            /// </summary>
            public const string INVALID_FIELD = "invalid_field";
            /// <summary>
            /// Constant INVALID_HEADER for invalid_header
            /// </summary>
            public const string INVALID_HEADER = "invalid_header";
            /// <summary>
            /// Constant INVALID_METHOD for invalid_method
            /// </summary>
            public const string INVALID_METHOD = "invalid_method";
            /// <summary>
            /// Constant MISSING_FIELD for missing_field
            /// </summary>
            public const string MISSING_FIELD = "missing_field";
            /// <summary>
            /// Constant SERVER_ERROR for server_error
            /// </summary>
            public const string SERVER_ERROR = "server_error";
            
        }

        /// <summary>
        /// Identifier of the problem.
        /// Constants for possible values can be found using Error.CodeValue
        /// </summary>
        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }
        /// <summary>
        /// An explanation of the problem with possible solutions.
        /// </summary>
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
        /// <summary>
        /// A URL for more information about the solution.
        /// </summary>
        [JsonProperty("more_info", NullValueHandling = NullValueHandling.Ignore)]
        public string MoreInfo { get; set; }
        /// <summary>
        /// Details about the specific area of the problem.
        /// </summary>
        [JsonProperty("target", NullValueHandling = NullValueHandling.Ignore)]
        public ErrorTarget Target { get; set; }
    }
}
