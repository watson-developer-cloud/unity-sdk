/**
* (C) Copyright IBM Corp. 2018, 2021.
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

namespace IBM.Watson.Assistant.V2.Model
{
    /// <summary>
    /// Dialog log message details.
    /// </summary>
    public class DialogLogMessage
    {
        /// <summary>
        /// The severity of the log message.
        /// </summary>
        public class LevelValue
        {
            /// <summary>
            /// Constant INFO for info
            /// </summary>
            public const string INFO = "info";
            /// <summary>
            /// Constant ERROR for error
            /// </summary>
            public const string ERROR = "error";
            /// <summary>
            /// Constant WARN for warn
            /// </summary>
            public const string WARN = "warn";
            
        }

        /// <summary>
        /// The severity of the log message.
        /// Constants for possible values can be found using DialogLogMessage.LevelValue
        /// </summary>
        [JsonProperty("level", NullValueHandling = NullValueHandling.Ignore)]
        public string Level { get; set; }
        /// <summary>
        /// The text of the log message.
        /// </summary>
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
        /// <summary>
        /// A code that indicates the category to which the error message belongs.
        /// </summary>
        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }
        /// <summary>
        /// An object that identifies the dialog element that generated the error message.
        /// </summary>
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public LogMessageSource Source { get; set; }
    }
}
