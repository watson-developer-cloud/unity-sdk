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
    /// Object containing source crawl status information.
    /// </summary>
    public class SourceStatus
    {
        /// <summary>
        /// The current status of the source crawl for this collection. This field returns `not_configured` if the
        /// default configuration for this source does not have a **source** object defined.
        ///
        /// -  `running` indicates that a crawl to fetch more documents is in progress.
        /// -  `complete` indicates that the crawl has completed with no errors.
        /// -  `queued` indicates that the crawl has been paused by the system and will automatically restart when
        /// possible.
        /// -  `unknown` indicates that an unidentified error has occured in the service.
        /// </summary>
        public class StatusValue
        {
            /// <summary>
            /// Constant RUNNING for running
            /// </summary>
            public const string RUNNING = "running";
            /// <summary>
            /// Constant COMPLETE for complete
            /// </summary>
            public const string COMPLETE = "complete";
            /// <summary>
            /// Constant NOT_CONFIGURED for not_configured
            /// </summary>
            public const string NOT_CONFIGURED = "not_configured";
            /// <summary>
            /// Constant QUEUED for queued
            /// </summary>
            public const string QUEUED = "queued";
            /// <summary>
            /// Constant UNKNOWN for unknown
            /// </summary>
            public const string UNKNOWN = "unknown";
            
        }

        /// <summary>
        /// The current status of the source crawl for this collection. This field returns `not_configured` if the
        /// default configuration for this source does not have a **source** object defined.
        ///
        /// -  `running` indicates that a crawl to fetch more documents is in progress.
        /// -  `complete` indicates that the crawl has completed with no errors.
        /// -  `queued` indicates that the crawl has been paused by the system and will automatically restart when
        /// possible.
        /// -  `unknown` indicates that an unidentified error has occured in the service.
        /// Constants for possible values can be found using SourceStatus.StatusValue
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        /// <summary>
        /// Date in `RFC 3339` format indicating the time of the next crawl attempt.
        /// </summary>
        [JsonProperty("next_crawl", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? NextCrawl { get; set; }
    }
}
