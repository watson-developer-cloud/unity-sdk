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
    /// DocumentSnapshot.
    /// </summary>
    public class DocumentSnapshot
    {
        /// <summary>
        /// The step in the document conversion process that the snapshot object represents.
        /// </summary>
        public class StepValue
        {
            /// <summary>
            /// Constant HTML_INPUT for html_input
            /// </summary>
            public const string HTML_INPUT = "html_input";
            /// <summary>
            /// Constant HTML_OUTPUT for html_output
            /// </summary>
            public const string HTML_OUTPUT = "html_output";
            /// <summary>
            /// Constant JSON_OUTPUT for json_output
            /// </summary>
            public const string JSON_OUTPUT = "json_output";
            /// <summary>
            /// Constant JSON_NORMALIZATIONS_OUTPUT for json_normalizations_output
            /// </summary>
            public const string JSON_NORMALIZATIONS_OUTPUT = "json_normalizations_output";
            /// <summary>
            /// Constant ENRICHMENTS_OUTPUT for enrichments_output
            /// </summary>
            public const string ENRICHMENTS_OUTPUT = "enrichments_output";
            /// <summary>
            /// Constant NORMALIZATIONS_OUTPUT for normalizations_output
            /// </summary>
            public const string NORMALIZATIONS_OUTPUT = "normalizations_output";
            
        }

        /// <summary>
        /// The step in the document conversion process that the snapshot object represents.
        /// Constants for possible values can be found using DocumentSnapshot.StepValue
        /// </summary>
        [JsonProperty("step", NullValueHandling = NullValueHandling.Ignore)]
        public string Step { get; set; }
        /// <summary>
        /// Snapshot of the conversion.
        /// </summary>
        [JsonProperty("snapshot", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Snapshot { get; set; }
    }
}
