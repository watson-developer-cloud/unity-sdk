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
    /// A request formatted for the Assistant service.
    /// </summary>
    [fsObject]
    public class MessageRequest
    {
        /// <summary>
        /// An input object that includes the input text.
        /// </summary>
        /// <value>An input object that includes the input text.</value>
        [fsProperty("input")]
        public object Input { get; set; }
        /// <summary>
        /// Whether to return more than one intent. Set to `true` to return all matching intents.
        /// </summary>
        /// <value>Whether to return more than one intent. Set to `true` to return all matching intents.</value>
        [fsProperty("alternate_intents")]
        public bool? AlternateIntents { get; set; }
        /// <summary>
        /// State information for the conversation. Continue a conversation by including the context object from the previous response.
        /// </summary>
        /// <value>State information for the conversation. Continue a conversation by including the context object from the previous response.</value>
        [fsProperty("context")]
        public object Context { get; set; }
        /// <summary>
        /// Entities to use when evaluating the message. Include entities from the previous response to continue using those entities rather than detecting entities in the new input.
        /// </summary>
        /// <value>Entities to use when evaluating the message. Include entities from the previous response to continue using those entities rather than detecting entities in the new input.</value>
        [fsProperty("entities")]
        public List<object> Entities { get; set; }
        /// <summary>
        /// Intents to use when evaluating the user input. Include intents from the previous response to continue using those intents rather than trying to recognize intents in the new input.
        /// </summary>
        /// <value>Intents to use when evaluating the user input. Include intents from the previous response to continue using those intents rather than trying to recognize intents in the new input.</value>
        [fsProperty("intents")]
        public List<object> Intents { get; set; }
        /// <summary>
        /// System output. Include the output from the previous response to maintain intermediate information over multiple requests.
        /// </summary>
        /// <value>System output. Include the output from the previous response to maintain intermediate information over multiple requests.</value>
        [fsProperty("output")]
        public object Output { get; set; }
    }

}
