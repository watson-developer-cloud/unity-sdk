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

namespace IBM.Watson.Discovery.V2.Model
{
    /// <summary>
    /// Display settings for aggregations.
    /// </summary>
    public class ComponentSettingsAggregation
    {
        /// <summary>
        /// Type of visualization to use when rendering the aggregation.
        /// </summary>
        public class VisualizationTypeValue
        {
            /// <summary>
            /// Constant AUTO for auto
            /// </summary>
            public const string AUTO = "auto";
            /// <summary>
            /// Constant FACET_TABLE for facet_table
            /// </summary>
            public const string FACET_TABLE = "facet_table";
            /// <summary>
            /// Constant WORD_CLOUD for word_cloud
            /// </summary>
            public const string WORD_CLOUD = "word_cloud";
            /// <summary>
            /// Constant MAP for map
            /// </summary>
            public const string MAP = "map";
            
        }

        /// <summary>
        /// Type of visualization to use when rendering the aggregation.
        /// Constants for possible values can be found using ComponentSettingsAggregation.VisualizationTypeValue
        /// </summary>
        [JsonProperty("visualization_type", NullValueHandling = NullValueHandling.Ignore)]
        public string VisualizationType { get; set; }
        /// <summary>
        /// Identifier used to map aggregation settings to aggregation configuration.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// User-friendly alias for the aggregation.
        /// </summary>
        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }
        /// <summary>
        /// Whether users is allowed to select more than one of the aggregation terms.
        /// </summary>
        [JsonProperty("multiple_selections_allowed", NullValueHandling = NullValueHandling.Ignore)]
        public bool? MultipleSelectionsAllowed { get; set; }
    }
}
