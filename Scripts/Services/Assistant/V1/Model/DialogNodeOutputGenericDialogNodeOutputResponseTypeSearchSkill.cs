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

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// An object that describes a response with response type `search_skill`.
    /// </summary>
    public class DialogNodeOutputGenericDialogNodeOutputResponseTypeSearchSkill : DialogNodeOutputGeneric
    {
        /// <summary>
        /// The type of response returned by the dialog node. The specified response type must be supported by the
        /// client application or channel.
        ///
        /// **Note:** The **search_skill** response type is used only by the v2 runtime API.
        /// </summary>
        public class ResponseTypeValue
        {
            /// <summary>
            /// Constant SEARCH_SKILL for search_skill
            /// </summary>
            public const string SEARCH_SKILL = "search_skill";
            
        }

        /// <summary>
        /// The type of the search query.
        /// </summary>
        public class QueryTypeValue
        {
            /// <summary>
            /// Constant NATURAL_LANGUAGE for natural_language
            /// </summary>
            public const string NATURAL_LANGUAGE = "natural_language";
            /// <summary>
            /// Constant DISCOVERY_QUERY_LANGUAGE for discovery_query_language
            /// </summary>
            public const string DISCOVERY_QUERY_LANGUAGE = "discovery_query_language";
            
        }

        /// <summary>
        /// The text of the search query. This can be either a natural-language query or a query that uses the Discovery
        /// query language syntax, depending on the value of the **query_type** property. For more information, see the
        /// [Discovery service
        /// documentation](https://cloud.ibm.com/docs/discovery?topic=discovery-query-operators#query-operators).
        /// </summary>
        [JsonProperty("query", NullValueHandling = NullValueHandling.Ignore)]
        public new string Query
        {
            get { return base.Query; }
            set { base.Query = value; }
        }
        /// <summary>
        /// An optional filter that narrows the set of documents to be searched. For more information, see the
        /// [Discovery service documentation]([Discovery service
        /// documentation](https://cloud.ibm.com/docs/discovery?topic=discovery-query-parameters#filter).
        /// </summary>
        [JsonProperty("filter", NullValueHandling = NullValueHandling.Ignore)]
        public new string Filter
        {
            get { return base.Filter; }
            set { base.Filter = value; }
        }
        /// <summary>
        /// The version of the Discovery service API to use for the query.
        /// </summary>
        [JsonProperty("discovery_version", NullValueHandling = NullValueHandling.Ignore)]
        public new string DiscoveryVersion
        {
            get { return base.DiscoveryVersion; }
            set { base.DiscoveryVersion = value; }
        }
    }
}
