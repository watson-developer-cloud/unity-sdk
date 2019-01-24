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

namespace IBM.Watson.Discovery.v1.Model
{
    /// <summary>
    /// **Deprecated**: Summary of the memory usage statistics for this environment.
    /// </summary>
    public class MemoryUsage
    {
        /// <summary>
        /// **Deprecated**: Number of bytes used in the environment's memory capacity.
        /// </summary>
        [fsProperty("used_bytes")]
        public virtual long? UsedBytes { get; private set; }
        /// <summary>
        /// **Deprecated**: Total number of bytes available in the environment's memory capacity.
        /// </summary>
        [fsProperty("total_bytes")]
        public virtual long? TotalBytes { get; private set; }
        /// <summary>
        /// **Deprecated**: Amount of memory capacity used, in KB or GB format.
        /// </summary>
        [fsProperty("used")]
        public virtual string Used { get; private set; }
        /// <summary>
        /// **Deprecated**: Total amount of the environment's memory capacity, in KB or GB format.
        /// </summary>
        [fsProperty("total")]
        public virtual string Total { get; private set; }
        /// <summary>
        /// **Deprecated**: Percentage of the environment's memory capacity that is being used.
        /// </summary>
        [fsProperty("percent_used")]
        public virtual double? PercentUsed { get; private set; }
    }


}
