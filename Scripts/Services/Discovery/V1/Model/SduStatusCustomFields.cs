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
    /// Information about custom smart document understanding fields that exist in this collection.
    /// </summary>
    public class SduStatusCustomFields
    {
        /// <summary>
        /// The number of custom fields defined for this collection.
        /// </summary>
        [JsonProperty("defined", NullValueHandling = NullValueHandling.Ignore)]
        public long? Defined { get; set; }
        /// <summary>
        /// The maximum number of custom fields that are allowed in this collection.
        /// </summary>
        [JsonProperty("maximum_allowed", NullValueHandling = NullValueHandling.Ignore)]
        public long? MaximumAllowed { get; set; }
    }
}
