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

namespace IBM.Watson.NaturalLanguageUnderstanding.V1.Model
{
    /// <summary>
    /// RelationArgument.
    /// </summary>
    public class RelationArgument
    {
        /// <summary>
        /// An array of extracted entities.
        /// </summary>
        [JsonProperty("entities", NullValueHandling = NullValueHandling.Ignore)]
        public List<RelationEntity> Entities { get; set; }
        /// <summary>
        /// Character offsets indicating the beginning and end of the mention in the analyzed text.
        /// </summary>
        [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
        public List<long?> Location { get; set; }
        /// <summary>
        /// Text that corresponds to the argument.
        /// </summary>
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
    }
}
