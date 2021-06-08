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
    /// A passage query result.
    /// </summary>
    public class QueryResultPassage
    {
        /// <summary>
        /// The content of the extracted passage.
        /// </summary>
        [JsonProperty("passage_text", NullValueHandling = NullValueHandling.Ignore)]
        public string PassageText { get; set; }
        /// <summary>
        /// The position of the first character of the extracted passage in the originating field.
        /// </summary>
        [JsonProperty("start_offset", NullValueHandling = NullValueHandling.Ignore)]
        public long? StartOffset { get; set; }
        /// <summary>
        /// The position of the last character of the extracted passage in the originating field.
        /// </summary>
        [JsonProperty("end_offset", NullValueHandling = NullValueHandling.Ignore)]
        public long? EndOffset { get; set; }
        /// <summary>
        /// The label of the field from which the passage has been extracted.
        /// </summary>
        [JsonProperty("field", NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; set; }
        /// <summary>
        /// Estimate of the probability that the passage is relevant.
        /// </summary>
        [JsonProperty("confidence", NullValueHandling = NullValueHandling.Ignore)]
        public double? Confidence { get; set; }
        /// <summary>
        /// An arry of extracted answers to the specified query.
        /// </summary>
        [JsonProperty("answers", NullValueHandling = NullValueHandling.Ignore)]
        public List<ResultPassageAnswer> Answers { get; set; }
    }
}
