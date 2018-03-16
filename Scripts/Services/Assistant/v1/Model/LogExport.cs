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

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// LogExport.
    /// </summary>
    [fsObject]
    public class LogExport
    {
        /// <summary>
        /// A request received by the workspace, including the user input and context.
        /// </summary>
        /// <value>A request received by the workspace, including the user input and context.</value>
        [fsProperty("request")]
        public MessageRequest Request { get; set; }
        /// <summary>
        /// The response sent by the workspace, including the output text, detected intents and entities, and context.
        /// </summary>
        /// <value>The response sent by the workspace, including the output text, detected intents and entities, and context.</value>
        [fsProperty("response")]
        public MessageResponse Response { get; set; }
        /// <summary>
        /// A unique identifier for the logged event.
        /// </summary>
        /// <value>A unique identifier for the logged event.</value>
        [fsProperty("log_id")]
        public string LogId { get; set; }
        /// <summary>
        /// The timestamp for receipt of the message.
        /// </summary>
        /// <value>The timestamp for receipt of the message.</value>
        [fsProperty("request_timestamp")]
        public string RequestTimestamp { get; set; }
        /// <summary>
        /// The timestamp for the system response to the message.
        /// </summary>
        /// <value>The timestamp for the system response to the message.</value>
        [fsProperty("response_timestamp")]
        public string ResponseTimestamp { get; set; }
        /// <summary>
        /// The unique identifier of the workspace where the request was made.
        /// </summary>
        /// <value>The unique identifier of the workspace where the request was made.</value>
        [fsProperty("workspace_id")]
        public string WorkspaceId { get; set; }
        /// <summary>
        /// The language of the workspace where the message request was made.
        /// </summary>
        /// <value>The language of the workspace where the message request was made.</value>
        [fsProperty("language")]
        public string Language { get; set; }
    }

}
