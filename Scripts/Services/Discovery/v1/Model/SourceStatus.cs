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

using FullSerializer;
using System;

namespace IBM.Watson.Discovery.v1.Model
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
        /// -  `complete_with_notices` indicates that some notices were generated during the crawl. Notices can be
        /// checked by using the **notices** query method.
        /// -  `stopped` indicates that the crawl has stopped but is not complete.
        /// </summary>
        public class StatusEnumValue
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
            /// Constant COMPLETE_WITH_NOTICES for complete_with_notices
            /// </summary>
            public const string COMPLETE_WITH_NOTICES = "complete_with_notices";
            /// <summary>
            /// Constant STOPPED for stopped
            /// </summary>
            public const string STOPPED = "stopped";
            /// <summary>
            /// Constant NOT_CONFIGURED for not_configured
            /// </summary>
            public const string NOT_CONFIGURED = "not_configured";
            
        }

        /// <summary>
        /// The current status of the source crawl for this collection. This field returns `not_configured` if the
        /// default configuration for this source does not have a **source** object defined.
        ///
        /// -  `running` indicates that a crawl to fetch more documents is in progress.
        /// -  `complete` indicates that the crawl has completed with no errors.
        /// -  `complete_with_notices` indicates that some notices were generated during the crawl. Notices can be
        /// checked by using the **notices** query method.
        /// -  `stopped` indicates that the crawl has stopped but is not complete.
        /// </summary>
        [fsProperty("status")]
        public string Status { get; set; }
        /// <summary>
        /// Date in UTC format indicating when the last crawl was attempted. If `null`, no crawl was completed.
        /// </summary>
        [fsProperty("last_updated")]
        public DateTime? LastUpdated { get; set; }
    }


}
