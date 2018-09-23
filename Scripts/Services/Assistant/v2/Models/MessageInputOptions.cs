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

namespace IBM.WatsonDeveloperCloud.Assistant.v2
{
    /// <summary>
    /// Optional properties that control how the assistant responds.
    /// </summary>
    [fsObject]
    public class MessageInputOptions
    {
        /// <summary>
        /// Whether to return additional diagnostic information. Set to `true` to return additional information under
        /// the `output.debug` key.
        /// </summary>
        [fsProperty("debug")]
        public bool? Debug { get; set; }
        /// <summary>
        /// Whether to start a new conversation with this user input. Specify `true` to clear the state information
        /// stored by the session.
        /// </summary>
        [fsProperty("restart")]
        public bool? Restart { get; set; }
        /// <summary>
        /// Whether to return more than one intent. Set to `true` to return all matching intents.
        /// </summary>
        [fsProperty("alternate_intents")]
        public bool? AlternateIntents { get; set; }
        /// <summary>
        /// Whether to return session context with the response. If you specify `true`, the response will include the
        /// `context` property.
        /// </summary>
        [fsProperty("return_context")]
        public bool? ReturnContext { get; set; }
    }

}
