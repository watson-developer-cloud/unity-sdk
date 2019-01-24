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

using System.Collections.Generic;
using FullSerializer;

namespace IBM.Watson.Discovery.v1.Model
{
    /// <summary>
    /// TestDocument.
    /// </summary>
    public class TestDocument
    {
        /// <summary>
        /// The unique identifier for the configuration.
        /// </summary>
        [fsProperty("configuration_id")]
        public virtual string ConfigurationId { get; private set; }
        /// <summary>
        /// Status of the preview operation.
        /// </summary>
        [fsProperty("status")]
        public virtual string Status { get; private set; }
        /// <summary>
        /// The number of 10-kB chunks of field data that were enriched. This can be used to estimate the cost of
        /// running a real ingestion.
        /// </summary>
        [fsProperty("enriched_field_units")]
        public virtual long? EnrichedFieldUnits { get; private set; }
        /// <summary>
        /// Format of the test document.
        /// </summary>
        [fsProperty("original_media_type")]
        public virtual string OriginalMediaType { get; private set; }
        /// <summary>
        /// An array of objects that describe each step in the preview process.
        /// </summary>
        [fsProperty("snapshots")]
        public virtual List<DocumentSnapshot> Snapshots { get; private set; }
        /// <summary>
        /// An array of notice messages about the preview operation.
        /// </summary>
        [fsProperty("notices")]
        public virtual List<Notice> Notices { get; private set; }
    }


}
