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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// Log message details.
    /// </summary>
    [fsObject]
    public class LogMessage
    {
        /// <summary>
        /// The severity of the log message.
        /// </summary>
        /// <value>The severity of the log message.</value>
        public enum LevelEnum
        {
            
            /// <summary>
            /// Enum INFO for info
            /// </summary>
            [EnumMember(Value = "info")]
            INFO,
            
            /// <summary>
            /// Enum ERROR for error
            /// </summary>
            [EnumMember(Value = "error")]
            ERROR,
            
            /// <summary>
            /// Enum WARN for warn
            /// </summary>
            [EnumMember(Value = "warn")]
            WARN
        }

        /// <summary>
        /// The severity of the log message.
        /// </summary>
        /// <value>The severity of the log message.</value>
        [fsProperty("level")]
        public LevelEnum? Level { get; set; }
        /// <summary>
        /// The text of the log message.
        /// </summary>
        /// <value>The text of the log message.</value>
        [fsProperty("msg")]
        public Dictionary<string, object> Msg { get; set; }
    }

}
