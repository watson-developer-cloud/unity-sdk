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
    /// DialogNodeAction
    /// </summary>
    [fsObject]
    public class DialogNodeAction
    {
        /// <summary>
        /// The type of action to invoke.
        /// </summary>
        public enum ActionTypeEnum
        {
            /// <summary>
            /// Enum client for client
            /// </summary>
            [EnumMember(Value = "client")]
            client,
            /// <summary>
            /// Enum server for server
            /// </summary>
            [EnumMember(Value = "server")]
            server,

            /// <summary>
            /// Enum WEB_ACTION for web-action
            /// </summary>
            [EnumMember(Value = "web-action")]
            web_action,

            /// <summary>
            /// Enum CLOUD_FUNCTION for cloud-function
            /// </summary>
            [EnumMember(Value = "cloud-function")]
            cloud_function
        }

        /// <summary>
        /// The type of action to invoke.
        /// </summary>
        [fsProperty("type")]
        public ActionTypeEnum? ActionType { get; set; }
        /// <summary>
        /// The name of the action.
        /// </summary>
        [fsProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// A map of key/value pairs to be provided to the action.
        /// </summary>
        [fsProperty("parameters")]
        public object Parameters { get; set; }
        /// <summary>
        /// The location in the dialog context where the result of the action is stored.
        /// </summary>
        [fsProperty("result_variable")]
        public string ResultVariable { get; set; }
        /// <summary>
        /// The name of the context variable that the client application will use to pass in credentials for the action.
        /// </summary>
        [fsProperty("credentials")]
        public string Credentials { get; set; }
    }

}
