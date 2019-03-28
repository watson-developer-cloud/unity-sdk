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
    /// Object containing source parameters for the configuration.
    /// </summary>
    public class Source
    {
        /// <summary>
        /// The type of source to connect to.
        /// -  `box` indicates the configuration is to connect an instance of Enterprise Box.
        /// -  `salesforce` indicates the configuration is to connect to Salesforce.
        /// -  `sharepoint` indicates the configuration is to connect to Microsoft SharePoint Online.
        /// -  `web_crawl` indicates the configuration is to perform a web page crawl.
        /// -  `cloud_object_storage` indicates the configuration is to connect to a cloud object store.
        /// </summary>
        public class TypeValue
        {
            /// <summary>
            /// Constant BOX for box
            /// </summary>
            public const string BOX = "box";
            /// <summary>
            /// Constant SALESFORCE for salesforce
            /// </summary>
            public const string SALESFORCE = "salesforce";
            /// <summary>
            /// Constant SHAREPOINT for sharepoint
            /// </summary>
            public const string SHAREPOINT = "sharepoint";
            /// <summary>
            /// Constant WEB_CRAWL for web_crawl
            /// </summary>
            public const string WEB_CRAWL = "web_crawl";
            /// <summary>
            /// Constant CLOUD_OBJECT_STORAGE for cloud_object_storage
            /// </summary>
            public const string CLOUD_OBJECT_STORAGE = "cloud_object_storage";
            
        }

        /// <summary>
        /// The type of source to connect to.
        /// -  `box` indicates the configuration is to connect an instance of Enterprise Box.
        /// -  `salesforce` indicates the configuration is to connect to Salesforce.
        /// -  `sharepoint` indicates the configuration is to connect to Microsoft SharePoint Online.
        /// -  `web_crawl` indicates the configuration is to perform a web page crawl.
        /// -  `cloud_object_storage` indicates the configuration is to connect to a cloud object store.
        /// Constants for possible values can be found using Source.TypeValue
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
        /// <summary>
        /// The **credential_id** of the credentials to use to connect to the source. Credentials are defined using the
        /// **credentials** method. The **source_type** of the credentials used must match the **type** field specified
        /// in this object.
        /// </summary>
        [JsonProperty("credential_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CredentialId { get; set; }
        /// <summary>
        /// Object containing the schedule information for the source.
        /// </summary>
        [JsonProperty("schedule", NullValueHandling = NullValueHandling.Ignore)]
        public SourceSchedule Schedule { get; set; }
        /// <summary>
        /// The **options** object defines which items to crawl from the source system.
        /// </summary>
        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public SourceOptions Options { get; set; }
    }
}
