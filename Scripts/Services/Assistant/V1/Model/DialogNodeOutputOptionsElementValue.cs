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

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// An object defining the message input to be sent to the Watson Assistant service if the user selects the
    /// corresponding option.
    /// </summary>
    public class DialogNodeOutputOptionsElementValue
    {
        /// <summary>
        /// An input object that includes the input text.
        /// </summary>
        [JsonProperty("input", NullValueHandling = NullValueHandling.Ignore)]
        public MessageInput Input { get; set; }
        /// <summary>
        /// An array of intents to be used while processing the input.
        ///
        /// **Note:** This property is supported for backward compatibility with applications that use the v1 **Get
        /// response to user input** method.
        /// </summary>
        [JsonProperty("intents", NullValueHandling = NullValueHandling.Ignore)]
        public List<RuntimeIntent> Intents { get; set; }
        /// <summary>
        /// An array of entities to be used while processing the user input.
        ///
        /// **Note:** This property is supported for backward compatibility with applications that use the v1 **Get
        /// response to user input** method.
        /// </summary>
        [JsonProperty("entities", NullValueHandling = NullValueHandling.Ignore)]
        public List<RuntimeEntity> Entities { get; set; }
    }
}
