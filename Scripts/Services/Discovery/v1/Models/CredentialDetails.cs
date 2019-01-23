/**
* Copyright 2018 IBM Corp. All Rights Reserved.
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

using FullSerializer;

namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
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
        /// -  `\"source_type\": \"box\"` - valid `credential_type`s: `oauth2`
        /// -  `\"source_type\": \"salesforce\"` - valid `credential_type`s: `username_password`
        /// -  `\"source_type\": \"sharepoint\"` - valid `credential_type`s: `saml` with **source_version** of `online`,
        /// or `ntml_v1` with **source_version** of `2016`
        /// -  `\"source_type\": \"web_crawl\"` - valid `credential_type`s: `noauth` or `basic`.
        /// </summary>
        /// <value>
        /// The authentication method for this credentials definition. The  **credential_type** specified must be
        /// supported by the **source_type**. The following combinations are possible:
        ///
        /// -  `\"source_type\": \"box\"` - valid `credential_type`s: `oauth2`
        /// -  `\"source_type\": \"salesforce\"` - valid `credential_type`s: `username_password`
        /// -  `\"source_type\": \"sharepoint\"` - valid `credential_type`s: `saml` with **source_version** of `online`,
        /// or `ntml_v1` with **source_version** of `2016`
        /// -  `\"source_type\": \"web_crawl\"` - valid `credential_type`s: `noauth` or `basic`.
        /// </value>
        public enum CredentialTypeEnum
        {
            
            /// <summary>
            /// Enum OAUTH2 for oauth2
            /// </summary>
            oauth2,
            
            /// <summary>
            /// Enum SAML for saml
            /// </summary>
            saml,
            
            /// <summary>
            /// Enum USERNAME_PASSWORD for username_password
            /// </summary>
            username_password,
            
            /// <summary>
            /// Enum NOAUTH for noauth
            /// </summary>
            noauth,
            
            /// <summary>
            /// Enum BASIC for basic
            /// </summary>
            basic,
            
            /// <summary>
            /// Enum NTML_V1 for ntml_v1
            /// </summary>
            ntml_v1
        }

        /// <summary>
        /// The authentication method for this credentials definition. The  **credential_type** specified must be
        /// supported by the **source_type**. The following combinations are possible:
        ///
        /// -  `\"source_type\": \"box\"` - valid `credential_type`s: `oauth2`
        /// -  `\"source_type\": \"salesforce\"` - valid `credential_type`s: `username_password`
        /// -  `\"source_type\": \"sharepoint\"` - valid `credential_type`s: `saml` with **source_version** of `online`,
        /// or `ntml_v1` with **source_version** of `2016`
        /// -  `\"source_type\": \"web_crawl\"` - valid `credential_type`s: `noauth` or `basic`.
        /// </summary>
        [fsProperty("credential_type")]
        public CredentialTypeEnum? CredentialType { get; set; }
        /// <summary>
        /// The type of Sharepoint repository to connect to. Only valid, and required, with a **source_type** of
        /// `sharepoint`.
        /// </summary>
        [fsProperty("source_version")]
        public string SourceVersion { get; set; }
        /// <summary>
        /// The **client_id** of the source that these credentials connect to. Only valid, and required, with a
        /// **credential_type** of `oauth2`.
        /// </summary>
        [fsProperty("client_id")]
        public string ClientId { get; set; }
        /// <summary>
        /// The **enterprise_id** of the Box site that these credentials connect to. Only valid, and required, with a
        /// **source_type** of `box`.
        /// </summary>
        [fsProperty("enterprise_id")]
        public string EnterpriseId { get; set; }
        /// <summary>
        /// The **url** of the source that these credentials connect to. Only valid, and required, with a
        /// **credential_type** of `username_password`, `noauth`, and `basic`.
        /// </summary>
        [fsProperty("url")]
        public string Url { get; set; }
        /// <summary>
        /// The **username** of the source that these credentials connect to. Only valid, and required, with a
        /// **credential_type** of `saml`, `username_password`, `basic`, or `ntml_v1`.
        /// </summary>
        [fsProperty("username")]
        public string Username { get; set; }
        /// <summary>
        /// The **organization_url** of the source that these credentials connect to. Only valid, and required, with a
        /// **credential_type** of `saml`.
        /// </summary>
        [fsProperty("organization_url")]
        public string OrganizationUrl { get; set; }
        /// <summary>
        /// The **site_collection.path** of the source that these credentials connect to. Only valid, and required, with
        /// a **source_type** of `sharepoint`.
        /// </summary>
        [fsProperty("site_collection.path")]
        public string SiteCollectionPath { get; set; }
        /// <summary>
        /// The **client_secret** of the source that these credentials connect to. Only valid, and required, with a
        /// **credential_type** of `oauth2`. This value is never returned and is only used when creating or modifying
        /// **credentials**.
        /// </summary>
        [fsProperty("client_secret")]
        public string ClientSecret { get; set; }
        /// <summary>
        /// The **public_key_id** of the source that these credentials connect to. Only valid, and required, with a
        /// **credential_type** of `oauth2`. This value is never returned and is only used when creating or modifying
        /// **credentials**.
        /// </summary>
        [fsProperty("public_key_id")]
        public string PublicKeyId { get; set; }
        /// <summary>
        /// The **private_key** of the source that these credentials connect to. Only valid, and required, with a
        /// **credential_type** of `oauth2`. This value is never returned and is only used when creating or modifying
        /// **credentials**.
        /// </summary>
        [fsProperty("private_key")]
        public string PrivateKey { get; set; }
        /// <summary>
        /// The **passphrase** of the source that these credentials connect to. Only valid, and required, with a
        /// **credential_type** of `oauth2`. This value is never returned and is only used when creating or modifying
        /// **credentials**.
        /// </summary>
        [fsProperty("passphrase")]
        public string Passphrase { get; set; }
        /// <summary>
        /// The **password** of the source that these credentials connect to. Only valid, and required, with
        /// **credential_type**s of `saml`, `username_password`, `basic`, or `ntml_v1`.
        ///
        /// **Note:** When used with a **source_type** of `salesforce`, the password consists of the Salesforce password
        /// and a valid Salesforce security token concatenated. This value is never returned and is only used when
        /// creating or modifying **credentials**.
        /// </summary>
        [fsProperty("password")]
        public string Password { get; set; }
        /// <summary>
        /// The ID of the **gateway** to be connected through (when connecting to intranet sites). Only valid with a
        /// **credential_type** of `noauth`, `basic`, or `ntml_v1`. Gateways are created using the
        /// `/v1/environments/{environment_id}/gateways` methods.
        /// </summary>
        [fsProperty("gateway_id")]
        public string GatewayId { get; set; }
        /// <summary>
        /// SharePoint OnPrem WebApplication URL. Only valid, and required, with a **source_version** of `2016`.
        /// </summary>
        [fsProperty("web_application_url")]
        public string WebApplicationUrl { get; set; }
        /// <summary>
        /// The domain used to log in to your OnPrem SharePoint account. Only valid, and required, with a
        /// **source_version** of `2016`.
        /// </summary>
        [fsProperty("domain")]
        public string Domain { get; set; }
    }

}
