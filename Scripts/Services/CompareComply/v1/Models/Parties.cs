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
using System.Collections.Generic;

namespace  IBM.Watson.DeveloperCloud.Services.CompareComply.v1
{
    /// <summary>
    /// A party and its corresponding role, including address and contact information if identified.
    /// </summary>
    [fsObject]
    public class Parties
    {
        /// <summary>
        /// A string that identifies the importance of the party.
        /// </summary>
        public enum ImportanceEnum
        {

            /// <summary>
            /// Enum PRIMARY for Primary
            /// </summary>
            Primary,

            /// <summary>
            /// Enum UNKNOWN for Unknown
            /// </summary>
            Unknown
        }

        /// <summary>
        /// A string that identifies the importance of the party.
        /// </summary>
        [fsProperty("importance")]
        public ImportanceEnum? Importance { get; set; }
        /// <summary>
        /// A string identifying the party.
        /// </summary>
        [fsProperty("party")]
        public string Party { get; set; }
        /// <summary>
        /// A string identifying the party's role.
        /// </summary>
        [fsProperty("role")]
        public string Role { get; set; }
        /// <summary>
        /// List of the party's address or addresses.
        /// </summary>
        [fsProperty("addresses")]
        public List<Address> Addresses { get; set; }
        /// <summary>
        /// List of the names and roles of contacts identified in the input document.
        /// </summary>
        [fsProperty("contacts")]
        public List<Contact> Contacts { get; set; }
    }

}
