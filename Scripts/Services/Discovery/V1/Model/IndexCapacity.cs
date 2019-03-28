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
    /// Details about the resource usage and capacity of the environment.
    /// </summary>
    public class IndexCapacity
    {
        /// <summary>
        /// Summary of the document usage statistics for the environment.
        /// </summary>
        [JsonProperty("documents", NullValueHandling = NullValueHandling.Ignore)]
        public EnvironmentDocuments Documents { get; set; }
        /// <summary>
        /// Summary of the disk usage statistics for the environment.
        /// </summary>
        [JsonProperty("disk_usage", NullValueHandling = NullValueHandling.Ignore)]
        public DiskUsage DiskUsage { get; set; }
        /// <summary>
        /// Summary of the collection usage in the environment.
        /// </summary>
        [JsonProperty("collections", NullValueHandling = NullValueHandling.Ignore)]
        public CollectionUsage Collections { get; set; }
    }
}
