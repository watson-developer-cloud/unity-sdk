/**
* (C) Copyright IBM Corp. 2021.
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
    /// An object that identifies the dialog element that generated the error message.
    /// </summary>
    public class LogMessageSource
    {
        /// <summary>
        /// A string that indicates the type of dialog element that generated the error message.
        /// </summary>
        public class TypeValue
        {
            /// <summary>
            /// Constant DIALOG_NODE for dialog_node
            /// </summary>
            public const string DIALOG_NODE = "dialog_node";
            
        }

        /// <summary>
        /// A string that indicates the type of dialog element that generated the error message.
        /// Constants for possible values can be found using LogMessageSource.TypeValue
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
        /// <summary>
        /// The unique identifier of the dialog node that generated the error message.
        /// </summary>
        [JsonProperty("dialog_node", NullValueHandling = NullValueHandling.Ignore)]
        public string DialogNode { get; set; }
    }
}
