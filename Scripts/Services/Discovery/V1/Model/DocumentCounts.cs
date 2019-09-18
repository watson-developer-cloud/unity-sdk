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
    /// Object containing collection document count information.
    /// </summary>
    public class DocumentCounts
    {
        /// <summary>
        /// The total number of available documents in the collection.
        /// </summary>
        [JsonProperty("available", NullValueHandling = NullValueHandling.Ignore)]
        public virtual long? Available { get; private set; }
        /// <summary>
        /// The number of documents in the collection that are currently being processed.
        /// </summary>
        [JsonProperty("processing", NullValueHandling = NullValueHandling.Ignore)]
        public virtual long? Processing { get; private set; }
        /// <summary>
        /// The number of documents in the collection that failed to be ingested.
        /// </summary>
        [JsonProperty("failed", NullValueHandling = NullValueHandling.Ignore)]
        public virtual long? Failed { get; private set; }
        /// <summary>
        /// The number of documents that have been uploaded to the collection, but have not yet started processing.
        /// </summary>
        [JsonProperty("pending", NullValueHandling = NullValueHandling.Ignore)]
        public virtual long? Pending { get; private set; }
    }
}
