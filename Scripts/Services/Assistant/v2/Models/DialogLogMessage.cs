/**
* Copyright 2018 IBM Corp. All Rights Reserved.
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

using FullSerializer;
using System.Runtime.Serialization;

namespace IBM.WatsonDeveloperCloud.Assistant.v2
{
    /// <summary>
    /// Dialog log message details.
    /// </summary>
    [fsObject]
    public class DialogLogMessage
    {
        /// <summary>
        /// The severity of the log message.
        /// </summary>
        public enum LevelEnum
        {
            /// <summary>
            /// Enum info for info
            /// </summary>
            [EnumMember(Value = "info")]
            info,
            /// <summary>
            /// Enum error for error
            /// </summary>
            [EnumMember(Value = "error")]
            error,
            /// <summary>
            /// Enum warn for warn
            /// </summary>
            [EnumMember(Value = "warn")]
            warn
        }

        /// <summary>
        /// The severity of the log message.
        /// </summary>
        [fsProperty("level")]
        public LevelEnum? Level { get; set; }
        /// <summary>
        /// The text of the log message.
        /// </summary>
        [fsProperty("message")]
        public string Message { get; set; }
    }

}
