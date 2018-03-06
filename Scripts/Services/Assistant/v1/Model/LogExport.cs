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
        /// A request formatted for the Assistant service.
        /// </summary>
        /// <value>A request formatted for the Assistant service.</value>
        public MessageRequest Request { get; set; }
        /// <summary>
        /// A response from the Assistant service.
        /// </summary>
        /// <value>A response from the Assistant service.</value>
        public MessageResponse Response { get; set; }
        /// <summary>
        /// A unique identifier for the logged message.
        /// </summary>
        /// <value>A unique identifier for the logged message.</value>
        public string LogId { get; set; }
        /// <summary>
        /// The timestamp for receipt of the message.
        /// </summary>
        /// <value>The timestamp for receipt of the message.</value>
        public string RequestTimestamp { get; set; }
        /// <summary>
        /// The timestamp for the system response to the message.
        /// </summary>
        /// <value>The timestamp for the system response to the message.</value>
        public string ResponseTimestamp { get; set; }
        /// <summary>
        /// The workspace ID.
        /// </summary>
        /// <value>The workspace ID.</value>
        public string WorkspaceId { get; set; }
        /// <summary>
        /// The language of the workspace where the message request was made.
        /// </summary>
        /// <value>The language of the workspace where the message request was made.</value>
        public string Language { get; set; }
    }

}
