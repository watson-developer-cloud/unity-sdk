/**
* (C) Copyright IBM Corp. 2021.
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

using JsonSubTypes;
using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V2.Model
{
    /// <summary>
    /// An object that identifies the dialog element that generated the error message.
    /// Classes which extend this class:
    /// - LogMessageSourceDialogNode
    /// - LogMessageSourceAction
    /// - LogMessageSourceStep
    /// - LogMessageSourceHandler
    /// </summary>
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(LogMessageSourceDialogNode), "dialog_node")]
    [JsonSubtypes.KnownSubType(typeof(LogMessageSourceAction), "action")]
    [JsonSubtypes.KnownSubType(typeof(LogMessageSourceStep), "step")]
    [JsonSubtypes.KnownSubType(typeof(LogMessageSourceHandler), "handler")]
    public class LogMessageSource
    {
        /// This ctor is protected to prevent instantiation of this base class.
        /// Instead, users should instantiate one of the subclasses listed below:
        /// - LogMessageSourceDialogNode
        /// - LogMessageSourceAction
        /// - LogMessageSourceStep
        /// - LogMessageSourceHandler
        protected LogMessageSource()
        {
        }

        /// <summary>
        /// A string that indicates the type of dialog element that generated the error message.
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; protected set; }
        /// <summary>
        /// The unique identifier of the dialog node that generated the error message.
        /// </summary>
        [JsonProperty("dialog_node", NullValueHandling = NullValueHandling.Ignore)]
        public string DialogNode { get; protected set; }
        /// <summary>
        /// The unique identifier of the action that generated the error message.
        /// </summary>
        [JsonProperty("action", NullValueHandling = NullValueHandling.Ignore)]
        public string Action { get; protected set; }
        /// <summary>
        /// The unique identifier of the step that generated the error message.
        /// </summary>
        [JsonProperty("step", NullValueHandling = NullValueHandling.Ignore)]
        public string Step { get; protected set; }
        /// <summary>
        /// The unique identifier of the handler that generated the error message.
        /// </summary>
        [JsonProperty("handler", NullValueHandling = NullValueHandling.Ignore)]
        public string Handler { get; protected set; }
    }
}
