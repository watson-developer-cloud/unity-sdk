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
    /// State information for the conversation. To maintain state, include the context from the previous response.
    /// </summary>
    [fsObject]
    public class Context
    {
        /// <summary>
        /// The unique identifier of the conversation.
        /// </summary>
        /// <value>The unique identifier of the conversation.</value>
        [fsProperty("conversation_id")]
        public Dictionary<string, object> ConversationId { get; set; }
        /// <summary>
        /// For internal use only.
        /// </summary>
        /// <value>For internal use only.</value>
        [fsProperty("system")]
        public Dictionary<string, object> System { get; set; }
    }

}
