/**
* (C) Copyright IBM Corp. 2020.
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
    /// Information about a specific enrichment.
    /// </summary>
    public class CreateEnrichment
    {
        /// <summary>
        /// The type of this enrichment.
        /// </summary>
        public class TypeValue
        {
            /// <summary>
            /// Constant DICTIONARY for dictionary
            /// </summary>
            public const string DICTIONARY = "dictionary";
            /// <summary>
            /// Constant REGULAR_EXPRESSION for regular_expression
            /// </summary>
            public const string REGULAR_EXPRESSION = "regular_expression";
            /// <summary>
            /// Constant UIMA_ANNOTATOR for uima_annotator
            /// </summary>
            public const string UIMA_ANNOTATOR = "uima_annotator";
            /// <summary>
            /// Constant RULE_BASED for rule_based
            /// </summary>
            public const string RULE_BASED = "rule_based";
            /// <summary>
            /// Constant WATSON_KNOWLEDGE_STUDIO_MODEL for watson_knowledge_studio_model
            /// </summary>
            public const string WATSON_KNOWLEDGE_STUDIO_MODEL = "watson_knowledge_studio_model";
            
        }

        /// <summary>
        /// The type of this enrichment.
        /// Constants for possible values can be found using CreateEnrichment.TypeValue
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
        /// <summary>
        /// The human readable name for this enrichment.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// The description of this enrichment.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        /// <summary>
        /// A object containing options for the current enrichment.
        /// </summary>
        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public EnrichmentOptions Options { get; set; }
    }
}
