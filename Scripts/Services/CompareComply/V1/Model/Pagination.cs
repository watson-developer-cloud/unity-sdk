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

namespace IBM.Watson.CompareComply.V1.Model
{
    /// <summary>
    /// Pagination details, if required by the length of the output.
    /// </summary>
    public class Pagination
    {
        /// <summary>
        /// A token identifying the current page of results.
        /// </summary>
        [JsonProperty("refresh_cursor", NullValueHandling = NullValueHandling.Ignore)]
        public string RefreshCursor { get; set; }
        /// <summary>
        /// A token identifying the next page of results.
        /// </summary>
        [JsonProperty("next_cursor", NullValueHandling = NullValueHandling.Ignore)]
        public string NextCursor { get; set; }
        /// <summary>
        /// The URL that returns the current page of results.
        /// </summary>
        [JsonProperty("refresh_url", NullValueHandling = NullValueHandling.Ignore)]
        public string RefreshUrl { get; set; }
        /// <summary>
        /// The URL that returns the next page of results.
        /// </summary>
        [JsonProperty("next_url", NullValueHandling = NullValueHandling.Ignore)]
        public string NextUrl { get; set; }
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        [JsonProperty("total", NullValueHandling = NullValueHandling.Ignore)]
        public long? Total { get; set; }
    }
}
