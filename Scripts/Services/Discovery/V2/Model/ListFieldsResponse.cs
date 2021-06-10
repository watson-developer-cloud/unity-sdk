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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IBM.Watson.Discovery.V2.Model
{
    /// <summary>
    /// The list of fetched fields.
    ///
    /// The fields are returned using a fully qualified name format, however, the format differs slightly from that used
    /// by the query operations.
    ///
    ///   * Fields which contain nested objects are assigned a type of "nested".
    ///
    ///   * Fields which belong to a nested object are prefixed with `.properties` (for example,
    /// `warnings.properties.severity` means that the `warnings` object has a property called `severity`).
    /// </summary>
    public class ListFieldsResponse
    {
        /// <summary>
        /// An array that contains information about each field in the collections.
        /// </summary>
        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        public List<Field> Fields { get; set; }
    }
}
