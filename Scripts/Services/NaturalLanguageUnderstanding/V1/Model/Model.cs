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

using Newtonsoft.Json;
using System;

namespace IBM.Watson.NaturalLanguageUnderstanding.V1.Model
{
    /// <summary>
    /// Model.
    /// </summary>
    public class Model
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
        /// Constants for possible values can be found using Model.StatusValue
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        /// <summary>
        /// Unique model ID.
        /// </summary>
        [JsonProperty("model_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ModelId { get; set; }
        /// <summary>
        /// ISO 639-1 code that indicates the language of the model.
        /// </summary>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }
        /// <summary>
        /// Model description.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        /// <summary>
        /// ID of the Watson Knowledge Studio workspace that deployed this model to Natural Language Understanding.
        /// </summary>
        [JsonProperty("workspace_id", NullValueHandling = NullValueHandling.Ignore)]
        public string WorkspaceId { get; set; }
        /// <summary>
        /// The model version, if it was manually provided in Watson Knowledge Studio.
        /// </summary>
        [JsonProperty("model_version", NullValueHandling = NullValueHandling.Ignore)]
        public string ModelVersion { get; set; }
        /// <summary>
        /// (Deprecated — use `model_version`) The model version, if it was manually provided in Watson Knowledge
        /// Studio.
        /// </summary>
        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }
        /// <summary>
        /// The description of the version, if it was manually provided in Watson Knowledge Studio.
        /// </summary>
        [JsonProperty("version_description", NullValueHandling = NullValueHandling.Ignore)]
        public string VersionDescription { get; set; }
        /// <summary>
        /// A dateTime indicating when the model was created.
        /// </summary>
        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Created { get; set; }
    }
}
