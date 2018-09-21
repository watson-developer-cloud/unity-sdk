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
    /// A response from the Watson Assistant service.
    /// </summary>
    [fsObject]
    public class MessageResponse
    {
        /// <summary>
        /// Assistant output to be rendered or processed by the client.
        /// </summary>
        [fsProperty("output")]
        public MessageOutput Output { get; set; }
        /// <summary>
        /// The current session context. Included in the response if the `return_context` property of the message input
        /// was set to `true`.
        /// </summary>
        [fsProperty("context")]
        public MessageContext Context { get; set; }
    }

}
