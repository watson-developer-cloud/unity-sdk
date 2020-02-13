/**
* (C) Copyright IBM Corp. 2018, 2020.
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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// Global settings for the workspace.
    /// </summary>
    public class WorkspaceSystemSettings
    {
        /// <summary>
        /// Workspace settings related to the Watson Assistant user interface.
        /// </summary>
        [JsonProperty("tooling", NullValueHandling = NullValueHandling.Ignore)]
        public WorkspaceSystemSettingsTooling Tooling { get; set; }
        /// <summary>
        /// Workspace settings related to the disambiguation feature.
        ///
        /// **Note:** This feature is available only to Plus and Premium users.
        /// </summary>
        [JsonProperty("disambiguation", NullValueHandling = NullValueHandling.Ignore)]
        public WorkspaceSystemSettingsDisambiguation Disambiguation { get; set; }
        /// <summary>
        /// For internal use only.
        /// </summary>
        [JsonProperty("human_agent_assist", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> HumanAgentAssist { get; set; }
        /// <summary>
        /// Workspace settings related to the behavior of system entities.
        /// </summary>
        [JsonProperty("system_entities", NullValueHandling = NullValueHandling.Ignore)]
        public WorkspaceSystemSettingsSystemEntities SystemEntities { get; set; }
        /// <summary>
        /// Workspace settings related to detection of irrelevant input.
        /// </summary>
        [JsonProperty("off_topic", NullValueHandling = NullValueHandling.Ignore)]
        public WorkspaceSystemSettingsOffTopic OffTopic { get; set; }
    }
}
