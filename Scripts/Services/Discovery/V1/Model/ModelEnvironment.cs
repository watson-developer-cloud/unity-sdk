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

using Newtonsoft.Json;
using System;

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// Details about an environment.
    /// </summary>
    public class ModelEnvironment
    {
        /// <summary>
        /// Current status of the environment. `resizing` is displayed when a request to increase the environment size
        /// has been made, but is still in the process of being completed.
        /// </summary>
        public class StatusValue
        {
            /// <summary>
            /// Constant ACTIVE for active
            /// </summary>
            public const string ACTIVE = "active";
            /// <summary>
            /// Constant PENDING for pending
            /// </summary>
            public const string PENDING = "pending";
            /// <summary>
            /// Constant MAINTENANCE for maintenance
            /// </summary>
            public const string MAINTENANCE = "maintenance";
            /// <summary>
            /// Constant RESIZING for resizing
            /// </summary>
            public const string RESIZING = "resizing";
            
        }

        /// <summary>
        /// Current size of the environment.
        /// </summary>
        public class SizeValue
        {
            /// <summary>
            /// Constant LT for LT
            /// </summary>
            public const string LT = "LT";
            /// <summary>
            /// Constant XS for XS
            /// </summary>
            public const string XS = "XS";
            /// <summary>
            /// Constant S for S
            /// </summary>
            public const string S = "S";
            /// <summary>
            /// Constant MS for MS
            /// </summary>
            public const string MS = "MS";
            /// <summary>
            /// Constant M for M
            /// </summary>
            public const string M = "M";
            /// <summary>
            /// Constant ML for ML
            /// </summary>
            public const string ML = "ML";
            /// <summary>
            /// Constant L for L
            /// </summary>
            public const string L = "L";
            /// <summary>
            /// Constant XL for XL
            /// </summary>
            public const string XL = "XL";
            /// <summary>
            /// Constant XXL for XXL
            /// </summary>
            public const string XXL = "XXL";
            /// <summary>
            /// Constant XXXL for XXXL
            /// </summary>
            public const string XXXL = "XXXL";
            
        }

        /// <summary>
        /// Current status of the environment. `resizing` is displayed when a request to increase the environment size
        /// has been made, but is still in the process of being completed.
        /// Constants for possible values can be found using ModelEnvironment.StatusValue
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        /// <summary>
        /// Current size of the environment.
        /// Constants for possible values can be found using ModelEnvironment.SizeValue
        /// </summary>
        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public string Size { get; set; }
        /// <summary>
        /// Unique identifier for the environment.
        /// </summary>
        [JsonProperty("environment_id", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string EnvironmentId { get; private set; }
        /// <summary>
        /// Name that identifies the environment.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// Description of the environment.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        /// <summary>
        /// Creation date of the environment, in the format `yyyy-MM-dd'T'HH:mm:ss.SSS'Z'`.
        /// </summary>
        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public virtual DateTime? Created { get; private set; }
        /// <summary>
        /// Date of most recent environment update, in the format `yyyy-MM-dd'T'HH:mm:ss.SSS'Z'`.
        /// </summary>
        [JsonProperty("updated", NullValueHandling = NullValueHandling.Ignore)]
        public virtual DateTime? Updated { get; private set; }
        /// <summary>
        /// If `true`, the environment contains read-only collections that are maintained by IBM.
        /// </summary>
        [JsonProperty("read_only", NullValueHandling = NullValueHandling.Ignore)]
        public virtual bool? _ReadOnly { get; private set; }
        /// <summary>
        /// The new size requested for this environment. Only returned when the environment *status* is `resizing`.
        ///
        /// *Note:* Querying and indexing can still be performed during an environment upsize.
        /// </summary>
        [JsonProperty("requested_size", NullValueHandling = NullValueHandling.Ignore)]
        public string RequestedSize { get; set; }
        /// <summary>
        /// Details about the resource usage and capacity of the environment.
        /// </summary>
        [JsonProperty("index_capacity", NullValueHandling = NullValueHandling.Ignore)]
        public IndexCapacity IndexCapacity { get; set; }
        /// <summary>
        /// Information about the Continuous Relevancy Training for this environment.
        /// </summary>
        [JsonProperty("search_status", NullValueHandling = NullValueHandling.Ignore)]
        public SearchStatus SearchStatus { get; set; }
    }
}
