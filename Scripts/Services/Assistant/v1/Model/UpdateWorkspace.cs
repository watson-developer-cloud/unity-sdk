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
    /// UpdateWorkspace.
    /// </summary>
    [fsObject]
    public class UpdateWorkspace
    {
        /// <summary>
        /// The name of the workspace.
        /// </summary>
        /// <value>The name of the workspace.</value>
        public string Name { get; set; }
        /// <summary>
        /// The description of the workspace.
        /// </summary>
        /// <value>The description of the workspace.</value>
        public string Description { get; set; }
        /// <summary>
        /// The language of the workspace.
        /// </summary>
        /// <value>The language of the workspace.</value>
        public string Language { get; set; }
        /// <summary>
        /// An array of objects defining the intents for the workspace.
        /// </summary>
        /// <value>An array of objects defining the intents for the workspace.</value>
        public List<CreateIntent> Intents { get; set; }
        /// <summary>
        /// An array of objects defining the entities for the workspace.
        /// </summary>
        /// <value>An array of objects defining the entities for the workspace.</value>
        public List<CreateEntity> Entities { get; set; }
        /// <summary>
        /// An array of objects defining the nodes in the workspace dialog.
        /// </summary>
        /// <value>An array of objects defining the nodes in the workspace dialog.</value>
        public List<CreateDialogNode> DialogNodes { get; set; }
        /// <summary>
        /// An array of objects defining input examples that have been marked as irrelevant input.
        /// </summary>
        /// <value>An array of objects defining input examples that have been marked as irrelevant input.</value>
        public List<CreateCounterexample> Counterexamples { get; set; }
        /// <summary>
        /// Any metadata related to the workspace.
        /// </summary>
        /// <value>Any metadata related to the workspace.</value>
        public object Metadata { get; set; }
        /// <summary>
        /// Whether training data from the workspace can be used by IBM for general service improvements. `true` indicates that workspace training data is not to be used.
        /// </summary>
        /// <value>Whether training data from the workspace can be used by IBM for general service improvements. `true` indicates that workspace training data is not to be used.</value>
        public bool? LearningOptOut { get; set; }
    }

}
