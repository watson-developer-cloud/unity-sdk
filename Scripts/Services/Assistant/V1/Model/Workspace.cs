/**
* Copyright 2018, 2019 IBM Corp. All Rights Reserved.
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
using System;

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// Workspace.
    /// </summary>
    public class Workspace
    {
        /// <summary>
        /// The current status of the workspace.
        /// </summary>
        public class StatusValue
        {
            /// <summary>
            /// Constant NON_EXISTENT for Non Existent
            /// </summary>
            public const string NON_EXISTENT = "Non Existent";
            /// <summary>
            /// Constant TRAINING for Training
            /// </summary>
            public const string TRAINING = "Training";
            /// <summary>
            /// Constant FAILED for Failed
            /// </summary>
            public const string FAILED = "Failed";
            /// <summary>
            /// Constant AVAILABLE for Available
            /// </summary>
            public const string AVAILABLE = "Available";
            /// <summary>
            /// Constant UNAVAILABLE for Unavailable
            /// </summary>
            public const string UNAVAILABLE = "Unavailable";
            
        }

        /// <summary>
        /// The current status of the workspace.
        /// Constants for possible values can be found using Workspace.StatusValue
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        /// <summary>
        /// The name of the workspace. This string cannot contain carriage return, newline, or tab characters.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// The description of the workspace. This string cannot contain carriage return, newline, or tab characters.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        /// <summary>
        /// The language of the workspace.
        /// </summary>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }
        /// <summary>
        /// Any metadata related to the workspace.
        /// </summary>
        [JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Metadata { get; set; }
        /// <summary>
        /// Whether training data from the workspace (including artifacts such as intents and entities) can be used by
        /// IBM for general service improvements. `true` indicates that workspace training data is not to be used.
        /// </summary>
        [JsonProperty("learning_opt_out", NullValueHandling = NullValueHandling.Ignore)]
        public bool? LearningOptOut { get; set; }
        /// <summary>
        /// Global settings for the workspace.
        /// </summary>
        [JsonProperty("system_settings", NullValueHandling = NullValueHandling.Ignore)]
        public WorkspaceSystemSettings SystemSettings { get; set; }
        /// <summary>
        /// The workspace ID of the workspace.
        /// </summary>
        [JsonProperty("workspace_id", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string WorkspaceId { get; private set; }
        /// <summary>
        /// The timestamp for creation of the object.
        /// </summary>
        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public virtual DateTime? Created { get; private set; }
        /// <summary>
        /// The timestamp for the most recent update to the object.
        /// </summary>
        [JsonProperty("updated", NullValueHandling = NullValueHandling.Ignore)]
        public virtual DateTime? Updated { get; private set; }
        /// <summary>
        /// An array of intents.
        /// </summary>
        [JsonProperty("intents", NullValueHandling = NullValueHandling.Ignore)]
        public List<Intent> Intents { get; set; }
        /// <summary>
        /// An array of objects describing the entities for the workspace.
        /// </summary>
        [JsonProperty("entities", NullValueHandling = NullValueHandling.Ignore)]
        public List<Entity> Entities { get; set; }
        /// <summary>
        /// An array of objects describing the dialog nodes in the workspace.
        /// </summary>
        [JsonProperty("dialog_nodes", NullValueHandling = NullValueHandling.Ignore)]
        public List<DialogNode> DialogNodes { get; set; }
        /// <summary>
        /// An array of counterexamples.
        /// </summary>
        [JsonProperty("counterexamples", NullValueHandling = NullValueHandling.Ignore)]
        public List<Counterexample> Counterexamples { get; set; }
        /// <summary>
        /// Gets or Sets Webhooks
        /// </summary>
        [JsonProperty("webhooks", NullValueHandling = NullValueHandling.Ignore)]
        public List<Webhook> Webhooks { get; set; }
    }
}
