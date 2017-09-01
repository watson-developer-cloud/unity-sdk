/**
* Copyright 2015 IBM Corp. All Rights Reserved.
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

namespace IBM.Watson.DeveloperCloud.Services.Conversation.v1
{
    /// <summary>
    /// The mesage response.
    /// </summary>
    #region MessageResponse
    [fsObject]
    public class MessageResponse
    {
        /// <summary>
        /// The input text.
        /// </summary>
        public Dictionary<string, object> input { get; set; }
        /// <summary>
        /// State information for the conversation .
        /// </summary>
        public Dictionary<string, object> context { get; set; }
        /// <summary>
        /// Terms from the request that are identified as entities.
        /// </summary>
        public RuntimeEntity[] entities { get; set; }
        /// <summary>
        /// Terms from the request that are identified as intents.
        /// </summary>
        public RuntimeIntent[] intents { get; set; }
        /// <summary>
        /// Output from the dialog, including the response to the user, the nodes that were triggered, and log messages.
        /// </summary>
        public OutputData output { get; set; }
        /// <summary>
        /// Whether to return more than one intent. true indicates that all matching intents are returned. 
        /// </summary>
        public bool alternate_intents { get; set; }
    }

    /// <summary>
    /// The entity object.
    /// </summary>
    [fsObject]
    public class RuntimeEntity
    {
        /// <summary>
        /// The entity name.
        /// </summary>
        public string entity { get; set; }
        /// <summary>
        /// The entity location.
        /// </summary>
        public int[] location { get; set; }
        /// <summary>
        /// The entity value.
        /// </summary>
        public string value { get; set; }
    }

    /// <summary>
    /// The resultant intent.
    /// </summary>
    [fsObject]
    public class RuntimeIntent
    {
        /// <summary>
        /// The intent.
        /// </summary>
        public string intent { get; set; }
        /// <summary>
        /// The confidence.
        /// </summary>
        public float confidence { get; set; }
    }

    /// <summary>
    /// The Output data.
    /// </summary>
    [fsObject]
    public class OutputData
    {
        /// <summary>
        /// Log messages.
        /// </summary>
        public RuntimeLogMessage[] log_messages { get; set; }
        /// <summary>
        /// Output text.
        /// </summary>
        public string[] text { get; set; }
        /// <summary>
        /// The nodes that were visited.
        /// </summary>
        public string[] nodes_visited { get; set; }
    }

    /// <summary>
    /// The log message object.
    /// </summary>
    [fsObject]
    public class RuntimeLogMessage
    {
        /// <summary>
        /// The log level.
        /// </summary>
        public string level { get; set; }
        /// <summary>
        /// The log message.
        /// </summary>
        public string msg { get; set; }
    }
    #endregion

    #region Message Request
    /// <summary>
    /// The user's input, with optional intents, entities, and other properties from the response.
    /// </summary>
    [fsObject]
    public class MessageRequest
    {
        /// <summary>
        /// The input text.
        /// </summary>
        public Dictionary<string, object> input { get; set; }
        /// <summary>
        /// State information for the conversation .
        /// </summary>
        public Dictionary<string, object> context { get; set; }
        /// <summary>
        /// Terms from the request that are identified as entities.
        /// </summary>
        public RuntimeEntity[] entities { get; set; }
        /// <summary>
        /// Terms from the request that are identified as intents.
        /// </summary>
        public RuntimeIntent[] intents { get; set; }
        /// <summary>
        /// Output from the dialog, including the response to the user, the nodes that were triggered, and log messages.
        /// </summary>
        public OutputData output { get; set; }
        /// <summary>
        /// Whether to return more than one intent. true indicates that all matching intents are returned. 
        /// </summary>
        public bool alternate_intents { get; set; }
    }
    #endregion
}
