/**
* (C) Copyright IBM Corp. 2018, 2020.
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
    /// Object containing details of the stored credentials.
    ///
    /// Obtain credentials for your source from the administrator of the source.
    /// </summary>
    public class CredentialDetails
    {
        /// <summary>
        /// The authentication method for this credentials definition. The  **credential_type** specified must be
        /// supported by the **source_type**. The following combinations are possible:
        ///
        /// -  `"source_type": "box"` - valid `credential_type`s: `oauth2`
        /// -  `"source_type": "salesforce"` - valid `credential_type`s: `username_password`
        /// -  `"source_type": "sharepoint"` - valid `credential_type`s: `saml` with **source_version** of `online`, or
        /// `ntlm_v1` with **source_version** of `2016`
        /// -  `"source_type": "web_crawl"` - valid `credential_type`s: `noauth` or `basic`
        /// -  "source_type": "cloud_object_storage"` - valid `credential_type`s: `aws4_hmac`.
        /// </summary>
        public class CredentialTypeValue
        {
            /// <summary>
            /// Constant OAUTH2 for oauth2
            /// </summary>
            public const string OAUTH2 = "oauth2";
            /// <summary>
            /// Constant SAML for saml
            /// </summary>
            public const string SAML = "saml";
            /// <summary>
            /// Constant USERNAME_PASSWORD for username_password
            /// </summary>
            public const string USERNAME_PASSWORD = "username_password";
            /// <summary>
            /// Constant NOAUTH for noauth
            /// </summary>
            public const string NOAUTH = "noauth";
            /// <summary>
            /// Constant BASIC for basic
            /// </summary>
            public const string BASIC = "basic";
            /// <summary>
            /// Constant NTLM_V1 for ntlm_v1
            /// </summary>
            public const string NTLM_V1 = "ntlm_v1";
            /// <summary>
            /// Constant AWS4_HMAC for aws4_hmac
            /// </summary>
            public const string AWS4_HMAC = "aws4_hmac";
            
        }

        /// <summary>
        /// The type of Sharepoint repository to connect to. Only valid, and required, with a **source_type** of
        /// `sharepoint`.
        /// </summary>
        public class SourceVersionValue
        {
            /// <summary>
            /// Constant ONLINE for online
            /// </summary>
            public const string ONLINE = "online";
            
        }

        /// <summary>
        /// The authentication method for this credentials definition. The  **credential_type** specified must be
        /// supported by the **source_type**. The following combinations are possible:
        ///
        /// -  `"source_type": "box"` - valid `credential_type`s: `oauth2`
        /// -  `"source_type": "salesforce"` - valid `credential_type`s: `username_password`
        /// -  `"source_type": "sharepoint"` - valid `credential_type`s: `saml` with **source_version** of `online`, or
        /// `ntlm_v1` with **source_version** of `2016`
        /// -  `"source_type": "web_crawl"` - valid `credential_type`s: `noauth` or `basic`
        /// -  "source_type": "cloud_object_storage"` - valid `credential_type`s: `aws4_hmac`.
        /// Constants for possible values can be found using CredentialDetails.CredentialTypeValue
        /// </summary>
        [JsonProperty("credential_type", NullValueHandling = NullValueHandling.Ignore)]
        public string CredentialType { get; set; }
        /// <summary>
        /// The type of Sharepoint repository to connect to. Only valid, and required, with a **source_type** of
        /// `sharepoint`.
        /// Constants for possible values can be found using CredentialDetails.SourceVersionValue
        /// </summary>
        [JsonProperty("source_version", NullValueHandling = NullValueHandling.Ignore)]
        public string SourceVersion { get; set; }
        /// <summary>
        /// The **client_id** of the source that these credentials connect to. Only valid, and required, with a
        /// **credential_type** of `oauth2`.
        /// </summary>
        [JsonProperty("client_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ClientId { get; set; }
        /// <summary>
        /// The **enterprise_id** of the Box site that these credentials connect to. Only valid, and required, with a
        /// **source_type** of `box`.
        /// </summary>
        [JsonProperty("enterprise_id", NullValueHandling = NullValueHandling.Ignore)]
        public string EnterpriseId { get; set; }
        /// <summary>
        /// The **url** of the source that these credentials connect to. Only valid, and required, with a
        /// **credential_type** of `username_password`, `noauth`, and `basic`.
        /// </summary>
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }
        /// <summary>
        /// The **username** of the source that these credentials connect to. Only valid, and required, with a
        /// **credential_type** of `saml`, `username_password`, `basic`, or `ntlm_v1`.
        /// </summary>
        [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
        public string Username { get; set; }
        /// <summary>
        /// The **organization_url** of the source that these credentials connect to. Only valid, and required, with a
        /// **credential_type** of `saml`.
        /// </summary>
        [JsonProperty("organization_url", NullValueHandling = NullValueHandling.Ignore)]
        public string OrganizationUrl { get; set; }
        /// <summary>
        /// The **site_collection.path** of the source that these credentials connect to. Only valid, and required, with
        /// a **source_type** of `sharepoint`.
        /// </summary>
        [JsonProperty("site_collection.path", NullValueHandling = NullValueHandling.Ignore)]
        public string SiteCollectionPath { get; set; }
        /// <summary>
        /// The **client_secret** of the source that these credentials connect to. Only valid, and required, with a
        /// **credential_type** of `oauth2`. This value is never returned and is only used when creating or modifying
        /// **credentials**.
        /// </summary>
        [JsonProperty("client_secret", NullValueHandling = NullValueHandling.Ignore)]
        public string ClientSecret { get; set; }
        /// <summary>
        /// The **public_key_id** of the source that these credentials connect to. Only valid, and required, with a
        /// **credential_type** of `oauth2`. This value is never returned and is only used when creating or modifying
        /// **credentials**.
        /// </summary>
        [JsonProperty("public_key_id", NullValueHandling = NullValueHandling.Ignore)]
        public string PublicKeyId { get; set; }
        /// <summary>
        /// The **private_key** of the source that these credentials connect to. Only valid, and required, with a
        /// **credential_type** of `oauth2`. This value is never returned and is only used when creating or modifying
        /// **credentials**.
        /// </summary>
        [JsonProperty("private_key", NullValueHandling = NullValueHandling.Ignore)]
        public string PrivateKey { get; set; }
        /// <summary>
        /// The **passphrase** of the source that these credentials connect to. Only valid, and required, with a
        /// **credential_type** of `oauth2`. This value is never returned and is only used when creating or modifying
        /// **credentials**.
        /// </summary>
        [JsonProperty("passphrase", NullValueHandling = NullValueHandling.Ignore)]
        public string Passphrase { get; set; }
        /// <summary>
        /// The **password** of the source that these credentials connect to. Only valid, and required, with
        /// **credential_type**s of `saml`, `username_password`, `basic`, or `ntlm_v1`.
        ///
        /// **Note:** When used with a **source_type** of `salesforce`, the password consists of the Salesforce password
        /// and a valid Salesforce security token concatenated. This value is never returned and is only used when
        /// creating or modifying **credentials**.
        /// </summary>
        [JsonProperty("password", NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; set; }
        /// <summary>
        /// The ID of the **gateway** to be connected through (when connecting to intranet sites). Only valid with a
        /// **credential_type** of `noauth`, `basic`, or `ntlm_v1`. Gateways are created using the
        /// `/v1/environments/{environment_id}/gateways` methods.
        /// </summary>
        [JsonProperty("gateway_id", NullValueHandling = NullValueHandling.Ignore)]
        public string GatewayId { get; set; }
        /// <summary>
        /// SharePoint OnPrem WebApplication URL. Only valid, and required, with a **source_version** of `2016`. If a
        /// port is not supplied, the default to port `80` for http and port `443` for https connections are used.
        /// </summary>
        [JsonProperty("web_application_url", NullValueHandling = NullValueHandling.Ignore)]
        public string WebApplicationUrl { get; set; }
        /// <summary>
        /// The domain used to log in to your OnPrem SharePoint account. Only valid, and required, with a
        /// **source_version** of `2016`.
        /// </summary>
        [JsonProperty("domain", NullValueHandling = NullValueHandling.Ignore)]
        public string Domain { get; set; }
        /// <summary>
        /// The endpoint associated with the cloud object store that your are connecting to. Only valid, and required,
        /// with a **credential_type** of `aws4_hmac`.
        /// </summary>
        [JsonProperty("endpoint", NullValueHandling = NullValueHandling.Ignore)]
        public string Endpoint { get; set; }
        /// <summary>
        /// The access key ID associated with the cloud object store. Only valid, and required, with a
        /// **credential_type** of `aws4_hmac`. This value is never returned and is only used when creating or modifying
        /// **credentials**. For more infomation, see the [cloud object store
        /// documentation](https://cloud.ibm.com/docs/cloud-object-storage?topic=cloud-object-storage-using-hmac-credentials#using-hmac-credentials).
        /// </summary>
        [JsonProperty("access_key_id", NullValueHandling = NullValueHandling.Ignore)]
        public string AccessKeyId { get; set; }
        /// <summary>
        /// The secret access key associated with the cloud object store. Only valid, and required, with a
        /// **credential_type** of `aws4_hmac`. This value is never returned and is only used when creating or modifying
        /// **credentials**. For more infomation, see the [cloud object store
        /// documentation](https://cloud.ibm.com/docs/cloud-object-storage?topic=cloud-object-storage-using-hmac-credentials#using-hmac-credentials).
        /// </summary>
        [JsonProperty("secret_access_key", NullValueHandling = NullValueHandling.Ignore)]
        public string SecretAccessKey { get; set; }
    }
}
