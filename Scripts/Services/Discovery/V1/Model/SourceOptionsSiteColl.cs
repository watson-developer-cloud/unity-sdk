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
    /// Object that defines a Microsoft SharePoint site collection to crawl with this configuration.
    /// </summary>
    public class SourceOptionsSiteColl
    {
        /// <summary>
        /// The Microsoft SharePoint Online site collection path to crawl. The path must be be relative to the
        /// **organization_url** that was specified in the credentials associated with this source configuration.
        /// </summary>
        [JsonProperty("site_collection_path", NullValueHandling = NullValueHandling.Ignore)]
        public string SiteCollectionPath { get; set; }
        /// <summary>
        /// The maximum number of documents to crawl for this site collection. By default, all documents in the site
        /// collection are crawled.
        /// </summary>
        [JsonProperty("limit", NullValueHandling = NullValueHandling.Ignore)]
        public long? Limit { get; set; }
    }
}
