/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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
using FullSerializer;

namespace IBM.Watson.Assistant.v1.Model
{
    /// <summary>
    /// UpdateWorkspace.
    /// </summary>
    public class UpdateWorkspace
    {
        /// <summary>
        /// The name of the workspace. This string cannot contain carriage return, newline, or tab characters, and it
        /// must be no longer than 64 characters.
        /// </summary>
        [fsProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// The description of the workspace. This string cannot contain carriage return, newline, or tab characters,
        /// and it must be no longer than 128 characters.
        /// </summary>
        [fsProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// The language of the workspace.
        /// </summary>
        [fsProperty("language")]
        public string Language { get; set; }
        /// <summary>
        /// An array of objects defining the intents for the workspace.
        /// </summary>
        [fsProperty("intents")]
        public List<CreateIntent> Intents { get; set; }
        /// <summary>
        /// An array of objects defining the entities for the workspace.
        /// </summary>
        [fsProperty("entities")]
        public List<CreateEntity> Entities { get; set; }
        /// <summary>
        /// An array of objects defining the nodes in the dialog.
        /// </summary>
        [fsProperty("dialog_nodes")]
        public List<CreateDialogNode> DialogNodes { get; set; }
        /// <summary>
        /// An array of objects defining input examples that have been marked as irrelevant input.
        /// </summary>
        [fsProperty("counterexamples")]
        public List<CreateCounterexample> Counterexamples { get; set; }
        /// <summary>
        /// Any metadata related to the workspace.
        /// </summary>
        [fsProperty("metadata")]
        public object Metadata { get; set; }
        /// <summary>
        /// Whether training data from the workspace can be used by IBM for general service improvements. `true`
        /// indicates that workspace training data is not to be used.
        /// </summary>
        [fsProperty("learning_opt_out")]
        public bool? LearningOptOut { get; set; }
        /// <summary>
        /// Global settings for the workspace.
        /// </summary>
        [fsProperty("system_settings")]
        public WorkspaceSystemSettings SystemSettings { get; set; }
    }


}
