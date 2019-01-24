/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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
using FullSerializer;

namespace IBM.Watson.Discovery.v1.Model
{
    /// <summary>
    /// DeleteConfigurationResponse.
    /// </summary>
    public class DeleteConfigurationResponse
    {
        /// <summary>
        /// Status of the configuration. A deleted configuration has the status deleted.
        /// </summary>
        public class StatusEnumValue
        {
            /// <summary>
            /// Constant DELETED for deleted
            /// </summary>
            public const string DELETED = "deleted";
            
        }

        /// <summary>
        /// Status of the configuration. A deleted configuration has the status deleted.
        /// </summary>
        [fsProperty("status")]
        public string Status { get; set; }
        /// <summary>
        /// The unique identifier for the configuration.
        /// </summary>
        [fsProperty("configuration_id")]
        public string ConfigurationId { get; set; }
        /// <summary>
        /// An array of notice messages, if any.
        /// </summary>
        [fsProperty("notices")]
        public List<Notice> Notices { get; set; }
    }


}
