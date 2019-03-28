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

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// Summary of the disk usage statistics for the environment.
    /// </summary>
    public class DiskUsage
    {
        /// <summary>
        /// Number of bytes within the environment's disk capacity that are currently used to store data.
        /// </summary>
        [JsonProperty("used_bytes", NullValueHandling = NullValueHandling.Ignore)]
        public virtual long? UsedBytes { get; private set; }
        /// <summary>
        /// Total number of bytes available in the environment's disk capacity.
        /// </summary>
        [JsonProperty("maximum_allowed_bytes", NullValueHandling = NullValueHandling.Ignore)]
        public virtual long? MaximumAllowedBytes { get; private set; }
    }
}
