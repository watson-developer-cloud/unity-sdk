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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// TestDocument.
    /// </summary>
    public class TestDocument
    {
        /// <summary>
        /// The unique identifier for the configuration.
        /// </summary>
        [JsonProperty("configuration_id", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string ConfigurationId { get; private set; }
        /// <summary>
        /// Status of the preview operation.
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string Status { get; private set; }
        /// <summary>
        /// The number of 10-kB chunks of field data that were enriched. This can be used to estimate the cost of
        /// running a real ingestion.
        /// </summary>
        [JsonProperty("enriched_field_units", NullValueHandling = NullValueHandling.Ignore)]
        public virtual long? EnrichedFieldUnits { get; private set; }
        /// <summary>
        /// Format of the test document.
        /// </summary>
        [JsonProperty("original_media_type", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string OriginalMediaType { get; private set; }
        /// <summary>
        /// An array of objects that describe each step in the preview process.
        /// </summary>
        [JsonProperty("snapshots", NullValueHandling = NullValueHandling.Ignore)]
        public virtual List<DocumentSnapshot> Snapshots { get; private set; }
        /// <summary>
        /// An array of notice messages about the preview operation.
        /// </summary>
        [JsonProperty("notices", NullValueHandling = NullValueHandling.Ignore)]
        public virtual List<Notice> Notices { get; private set; }
    }
}
