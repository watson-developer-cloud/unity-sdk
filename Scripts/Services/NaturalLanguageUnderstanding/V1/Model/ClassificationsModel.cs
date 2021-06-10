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

using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace IBM.Watson.NaturalLanguageUnderstanding.V1.Model
{
    /// <summary>
    /// Classifications model.
    /// </summary>
    public class ClassificationsModel
    {
        /// <summary>
        /// When the status is `available`, the model is ready to use.
        /// </summary>
        public class StatusValue
        {
            /// <summary>
            /// Constant STARTING for starting
            /// </summary>
            public const string STARTING = "starting";
            /// <summary>
            /// Constant TRAINING for training
            /// </summary>
            public const string TRAINING = "training";
            /// <summary>
            /// Constant DEPLOYING for deploying
            /// </summary>
            public const string DEPLOYING = "deploying";
            /// <summary>
            /// Constant AVAILABLE for available
            /// </summary>
            public const string AVAILABLE = "available";
            /// <summary>
            /// Constant ERROR for error
            /// </summary>
            public const string ERROR = "error";
            /// <summary>
            /// Constant DELETED for deleted
            /// </summary>
            public const string DELETED = "deleted";
            
        }

        /// <summary>
        /// When the status is `available`, the model is ready to use.
        /// Constants for possible values can be found using ClassificationsModel.StatusValue
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        /// <summary>
        /// An optional name for the model.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// An optional map of metadata key-value pairs to store with this model.
        /// </summary>
        [JsonProperty("user_metadata", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> UserMetadata { get; set; }
        /// <summary>
        /// The 2-letter language code of this model.
        /// </summary>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }
        /// <summary>
        /// An optional description of the model.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        /// <summary>
        /// An optional version string.
        /// </summary>
        [JsonProperty("model_version", NullValueHandling = NullValueHandling.Ignore)]
        public string ModelVersion { get; set; }
        /// <summary>
        /// ID of the Watson Knowledge Studio workspace that deployed this model to Natural Language Understanding.
        /// </summary>
        [JsonProperty("workspace_id", NullValueHandling = NullValueHandling.Ignore)]
        public string WorkspaceId { get; set; }
        /// <summary>
        /// The description of the version.
        /// </summary>
        [JsonProperty("version_description", NullValueHandling = NullValueHandling.Ignore)]
        public string VersionDescription { get; set; }
        /// <summary>
        /// The service features that are supported by the custom model.
        /// </summary>
        [JsonProperty("features", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Features { get; set; }
        /// <summary>
        /// Unique model ID.
        /// </summary>
        [JsonProperty("model_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ModelId { get; set; }
        /// <summary>
        /// dateTime indicating when the model was created.
        /// </summary>
        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Created { get; set; }
        /// <summary>
        /// Gets or Sets Notices
        /// </summary>
        [JsonProperty("notices", NullValueHandling = NullValueHandling.Ignore)]
        public List<Notice> Notices { get; set; }
        /// <summary>
        /// dateTime of last successful model training.
        /// </summary>
        [JsonProperty("last_trained", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? LastTrained { get; set; }
        /// <summary>
        /// dateTime of last successful model deployment.
        /// </summary>
        [JsonProperty("last_deployed", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? LastDeployed { get; set; }
    }
}
