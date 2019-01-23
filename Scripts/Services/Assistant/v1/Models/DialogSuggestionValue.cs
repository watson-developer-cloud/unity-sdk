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

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// An object defining the message input, intents, and entities to be sent to the Watson Assistant service if the
    /// user selects the corresponding disambiguation option.
    /// </summary>
    [fsObject]
    public class DialogSuggestionValue
    {
        /// <summary>
        /// The user input.
        /// </summary>
        [fsProperty("input")]
        public InputData Input { get; set; }
        /// <summary>
        /// An array of intents to be sent along with the user input.
        /// </summary>
        [fsProperty("intents")]
        public List<object> Intents { get; set; }
        /// <summary>
        /// An array of entities to be sent along with the user input.
        /// </summary>
        [fsProperty("entities")]
        public List<object> Entities { get; set; }
    }

}
