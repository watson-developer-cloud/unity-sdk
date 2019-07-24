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

namespace IBM.Watson.CompareComply.V1.Model
{
    /// <summary>
    /// A party and its corresponding role, including address and contact information if identified.
    /// </summary>
    public class Parties
    {
        /// <summary>
        /// A string that identifies the importance of the party.
        /// </summary>
        public class ImportanceValue
        {
            /// <summary>
            /// Constant PRIMARY for Primary
            /// </summary>
            public const string PRIMARY = "Primary";
            /// <summary>
            /// Constant UNKNOWN for Unknown
            /// </summary>
            public const string UNKNOWN = "Unknown";
            
        }

        /// <summary>
        /// A string that identifies the importance of the party.
        /// Constants for possible values can be found using Parties.ImportanceValue
        /// </summary>
        [JsonProperty("importance", NullValueHandling = NullValueHandling.Ignore)]
        public string Importance { get; set; }
        /// <summary>
        /// The normalized form of the party's name.
        /// </summary>
        [JsonProperty("party", NullValueHandling = NullValueHandling.Ignore)]
        public string Party { get; set; }
        /// <summary>
        /// A string identifying the party's role.
        /// </summary>
        [JsonProperty("role", NullValueHandling = NullValueHandling.Ignore)]
        public string Role { get; set; }
        /// <summary>
        /// A list of the party's address or addresses.
        /// </summary>
        [JsonProperty("addresses", NullValueHandling = NullValueHandling.Ignore)]
        public List<Address> Addresses { get; set; }
        /// <summary>
        /// A list of the names and roles of contacts identified in the input document.
        /// </summary>
        [JsonProperty("contacts", NullValueHandling = NullValueHandling.Ignore)]
        public List<Contact> Contacts { get; set; }
        /// <summary>
        /// A list of the party's mentions in the input document.
        /// </summary>
        [JsonProperty("mentions", NullValueHandling = NullValueHandling.Ignore)]
        public List<Mention> Mentions { get; set; }
    }
}
