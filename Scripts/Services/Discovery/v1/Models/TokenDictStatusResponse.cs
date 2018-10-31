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
using System.Runtime.Serialization;

namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    /// <summary>
    /// Object describing the current status of the tokenization dictionary.
    /// </summary>
    [fsObject]
    public class TokenDictStatusResponse
    {
        /// <summary>
        /// Current tokenization dictionary status for the specified collection.
        /// </summary>
        [fsProperty("status")]
        public string Status { get; set; }
        /// <summary>
        /// The type for this dictionary. Always returns `tokenization_dictionary`.
        /// </summary>
        [fsProperty("type")]
        public string Type { get; set; }
    }

}
