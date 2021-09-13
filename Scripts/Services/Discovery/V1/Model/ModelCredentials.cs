/**
* (C) Copyright IBM Corp. 2019, 2021.
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
    /// Object containing credential information.
    /// </summary>
    public class ModelCredentials
    {
        /// <summary>
        /// The source that this credentials object connects to.
        /// -  `box` indicates the credentials are used to connect an instance of Enterprise Box.
        /// -  `salesforce` indicates the credentials are used to connect to Salesforce.
        /// -  `sharepoint` indicates the credentials are used to connect to Microsoft SharePoint Online.
        /// -  `web_crawl` indicates the credentials are used to perform a web crawl.
        /// =  `cloud_object_storage` indicates the credentials are used to connect to an IBM Cloud Object Store.
        /// </summary>
        public class SourceTypeValue
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
        /// The source that this credentials object connects to.
        /// -  `box` indicates the credentials are used to connect an instance of Enterprise Box.
        /// -  `salesforce` indicates the credentials are used to connect to Salesforce.
        /// -  `sharepoint` indicates the credentials are used to connect to Microsoft SharePoint Online.
        /// -  `web_crawl` indicates the credentials are used to perform a web crawl.
        /// =  `cloud_object_storage` indicates the credentials are used to connect to an IBM Cloud Object Store.
        /// Constants for possible values can be found using ModelCredentials.SourceTypeValue
        /// </summary>
        [JsonProperty("source_type", NullValueHandling = NullValueHandling.Ignore)]
        public string SourceType { get; set; }
        /// <summary>
        /// Unique identifier for this set of credentials.
        /// </summary>
        [JsonProperty("credential_id", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string CredentialId { get; private set; }
        /// <summary>
        /// Object containing details of the stored credentials.
        ///
        /// Obtain credentials for your source from the administrator of the source.
        /// </summary>
        [JsonProperty("credential_details", NullValueHandling = NullValueHandling.Ignore)]
        public CredentialDetails CredentialDetails { get; set; }
        /// <summary>
        /// Object that contains details about the status of the authentication process.
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public StatusDetails Status { get; set; }
    }
}
