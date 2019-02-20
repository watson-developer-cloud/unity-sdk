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
    /// Information about the Continuous Relevancy Training for this environment.
    /// </summary>
    public class SearchStatus
    {
        /// <summary>
        /// The current status of Continuous Relevancy Training for this environment.
        /// </summary>
        public class StatusValue
        {
            /// <summary>
            /// Constant NO_DATA for NO_DATA
            /// </summary>
            public const string NO_DATA = "NO_DATA";
            /// <summary>
            /// Constant INSUFFICENT_DATA for INSUFFICENT_DATA
            /// </summary>
            public const string INSUFFICENT_DATA = "INSUFFICENT_DATA";
            /// <summary>
            /// Constant TRAINING for TRAINING
            /// </summary>
            public const string TRAINING = "TRAINING";
            /// <summary>
            /// Constant TRAINED for TRAINED
            /// </summary>
            public const string TRAINED = "TRAINED";
            /// <summary>
            /// Constant NOT_APPLICABLE for NOT_APPLICABLE
            /// </summary>
            public const string NOT_APPLICABLE = "NOT_APPLICABLE";
            
        }

        /// <summary>
        /// The current status of Continuous Relevancy Training for this environment.
        /// Constants for possible values can be found using SearchStatus.StatusValue
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        /// <summary>
        /// Current scope of the training. Always returned as `environment`.
        /// </summary>
        [JsonProperty("scope", NullValueHandling = NullValueHandling.Ignore)]
        public string Scope { get; set; }
        /// <summary>
        /// Long description of the current Continuous Relevancy Training status.
        /// </summary>
        [JsonProperty("status_description", NullValueHandling = NullValueHandling.Ignore)]
        public string StatusDescription { get; set; }
        /// <summary>
        /// The date stamp of the most recent completed training for this environment.
        /// </summary>
        [JsonProperty("last_trained", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? LastTrained { get; set; }
    }
}
