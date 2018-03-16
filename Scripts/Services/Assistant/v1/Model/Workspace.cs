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
using System;

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// Workspace.
    /// </summary>
    [fsObject]
    public class Workspace
    {
        /// <summary>
        /// The name of the workspace.
        /// </summary>
        /// <value>The name of the workspace.</value>
        [fsProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// The language of the workspace.
        /// </summary>
        /// <value>The language of the workspace.</value>
        [fsProperty("language")]
        public string Language { get; set; }
        /// <summary>
        /// The timestamp for creation of the workspace.
        /// </summary>
        /// <value>The timestamp for creation of the workspace.</value>
        [fsProperty("created")]
        public virtual DateTime Created { get; private set; }
        /// <summary>
        /// The timestamp for the last update to the workspace.
        /// </summary>
        /// <value>The timestamp for the last update to the workspace.</value>
        [fsProperty("updated")]
        public virtual DateTime Updated { get; private set; }
        /// <summary>
        /// The workspace ID.
        /// </summary>
        /// <value>The workspace ID.</value>
        [fsProperty("workspace_id")]
        public virtual string WorkspaceId { get; private set; }
        /// <summary>
        /// The description of the workspace.
        /// </summary>
        /// <value>The description of the workspace.</value>
        [fsProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// Any metadata related to the workspace.
        /// </summary>
        /// <value>Any metadata related to the workspace.</value>
        [fsProperty("metadata")]
        public object Metadata { get; set; }
        /// <summary>
        /// Whether training data from the workspace (including artifacts such as intents and entities) can be used by IBM for general service improvements. `true` indicates that workspace training data is not to be used.
        /// </summary>
        /// <value>Whether training data from the workspace (including artifacts such as intents and entities) can be used by IBM for general service improvements. `true` indicates that workspace training data is not to be used.</value>
        [fsProperty("learning_opt_out")]
        public bool? LearningOptOut { get; set; }
    }

}
