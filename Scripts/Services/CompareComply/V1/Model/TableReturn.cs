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

namespace IBM.Watson.CompareComply.V1.Model
{
    /// <summary>
    /// The analysis of the document's tables.
    /// </summary>
    public class TableReturn
    {
        /// <summary>
        /// Information about the parsed input document.
        /// </summary>
        [JsonProperty("document", NullValueHandling = NullValueHandling.Ignore)]
        public DocInfo Document { get; set; }
        /// <summary>
        /// The ID of the model used to extract the table contents. The value for table extraction is `tables`.
        /// </summary>
        [JsonProperty("model_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ModelId { get; set; }
        /// <summary>
        /// The version of the `tables` model ID.
        /// </summary>
        [JsonProperty("model_version", NullValueHandling = NullValueHandling.Ignore)]
        public string ModelVersion { get; set; }
        /// <summary>
        /// Definitions of the tables identified in the input document.
        /// </summary>
        [JsonProperty("tables", NullValueHandling = NullValueHandling.Ignore)]
        public List<Tables> Tables { get; set; }
    }
}
