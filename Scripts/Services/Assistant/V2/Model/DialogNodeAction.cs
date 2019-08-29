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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V2.Model
{
    /// <summary>
    /// DialogNodeAction.
    /// </summary>
    public class DialogNodeAction
    {
        /// <summary>
        /// The type of action to invoke.
        /// </summary>
        public class TypeValue
        {
            /// <summary>
            /// Constant CLIENT for client
            /// </summary>
            public const string CLIENT = "client";
            /// <summary>
            /// Constant SERVER for server
            /// </summary>
            public const string SERVER = "server";
            /// <summary>
            /// Constant WEB_ACTION for web-action
            /// </summary>
            public const string WEB_ACTION = "web-action";
            /// <summary>
            /// Constant CLOUD_FUNCTION for cloud-function
            /// </summary>
            public const string CLOUD_FUNCTION = "cloud-function";
            
        }

        /// <summary>
        /// The type of action to invoke.
        /// Constants for possible values can be found using DialogNodeAction.TypeValue
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
        /// <summary>
        /// The name of the action.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// A map of key/value pairs to be provided to the action.
        /// </summary>
        [JsonProperty("parameters", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Parameters { get; set; }
        /// <summary>
        /// The location in the dialog context where the result of the action is stored.
        /// </summary>
        [JsonProperty("result_variable", NullValueHandling = NullValueHandling.Ignore)]
        public string ResultVariable { get; set; }
        /// <summary>
        /// The name of the context variable that the client application will use to pass in credentials for the action.
        /// </summary>
        [JsonProperty("credentials", NullValueHandling = NullValueHandling.Ignore)]
        public string _Credentials { get; set; }
    }
}
