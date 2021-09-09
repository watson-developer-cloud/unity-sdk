/**
* (C) Copyright IBM Corp. 2020, 2021.
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
    /// Details about a specific project.
    /// </summary>
    public class ProjectListDetails
    {
        /// <summary>
        /// The type of project.
        ///
        /// The `content_intelligence` type is a *Document Retrieval for Contracts* project and the `other` type is a
        /// *Custom* project.
        ///
        /// The `content_mining` and `content_intelligence` types are available with Premium plan managed deployments
        /// and installed deployments only.
        /// </summary>
        public class TypeValue
        {
            /// <summary>
            /// Constant DOCUMENT_RETRIEVAL for document_retrieval
            /// </summary>
            public const string DOCUMENT_RETRIEVAL = "document_retrieval";
            /// <summary>
            /// Constant CONVERSATIONAL_SEARCH for conversational_search
            /// </summary>
            public const string CONVERSATIONAL_SEARCH = "conversational_search";
            /// <summary>
            /// Constant CONTENT_MINING for content_mining
            /// </summary>
            public const string CONTENT_MINING = "content_mining";
            /// <summary>
            /// Constant CONTENT_INTELLIGENCE for content_intelligence
            /// </summary>
            public const string CONTENT_INTELLIGENCE = "content_intelligence";
            /// <summary>
            /// Constant OTHER for other
            /// </summary>
            public const string OTHER = "other";
            
        }

        /// <summary>
        /// The type of project.
        ///
        /// The `content_intelligence` type is a *Document Retrieval for Contracts* project and the `other` type is a
        /// *Custom* project.
        ///
        /// The `content_mining` and `content_intelligence` types are available with Premium plan managed deployments
        /// and installed deployments only.
        /// Constants for possible values can be found using ProjectListDetails.TypeValue
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
        /// <summary>
        /// The unique identifier of this project.
        /// </summary>
        [JsonProperty("project_id", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string ProjectId { get; private set; }
        /// <summary>
        /// The human readable name of this project.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// Relevancy training status information for this project.
        /// </summary>
        [JsonProperty("relevancy_training_status", NullValueHandling = NullValueHandling.Ignore)]
        public ProjectListDetailsRelevancyTrainingStatus RelevancyTrainingStatus { get; set; }
        /// <summary>
        /// The number of collections configured in this project.
        /// </summary>
        [JsonProperty("collection_count", NullValueHandling = NullValueHandling.Ignore)]
        public virtual long? CollectionCount { get; private set; }
    }
}
