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

namespace IBM.WatsonDeveloperCloud.Assistant.v2
{
    /// <summary>
    /// Assistant output to be rendered or processed by the client.
    /// </summary>
    [fsObject]
    public class MessageOutput
    {
        /// <summary>
        /// Output intended for any channel. It is the responsibility of the client application to implement the
        /// supported response types.
        /// </summary>
        [fsProperty("generic")]
        public List<DialogRuntimeResponseGeneric> Generic { get; set; }
        /// <summary>
        /// An array of intents recognized in the user input, sorted in descending order of confidence.
        /// </summary>
        [fsProperty("intents")]
        public List<RuntimeIntent> Intents { get; set; }
        /// <summary>
        /// An array of entities identified in the user input.
        /// </summary>
        [fsProperty("entities")]
        public List<RuntimeEntity> Entities { get; set; }
        /// <summary>
        /// An array of objects describing any actions requested by the dialog node.
        /// </summary>
        [fsProperty("actions")]
        public List<DialogNodeAction> Actions { get; set; }
        /// <summary>
        /// Additional detailed information about a message response and how it was generated.
        /// </summary>
        [fsProperty("debug")]
        public MessageOutputDebug Debug { get; set; }
    }

}
