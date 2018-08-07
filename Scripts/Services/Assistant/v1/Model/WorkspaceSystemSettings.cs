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
    /// WorkspaceSystemSettings
    /// </summary>
    [fsObject]
    public class WorkspaceSystemSettings
    {
        /// <summary>
        /// Workspace settings related to the Watson Assistant tool.
        /// </summary>
        [fsProperty("tooling")]
        public WorkspaceSystemSettingsTooling Tooling { get; set; }
        /// <summary>
        /// Workspace settings related to the disambiguation feature.
        ///
        /// **Note:** This feature is available only to Premium users.
        /// </summary>
        [fsProperty("disambiguation")]
        public WorkspaceSystemSettingsDisambiguation Disambiguation { get; set; }
        /// <summary>
        /// For internal use only.
        /// </summary>
        [fsProperty("human_agent_assist")]
        public object HumanAgentAssist { get; set; }
    }

}
