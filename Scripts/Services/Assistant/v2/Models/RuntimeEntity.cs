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

namespace IBM.WatsonDeveloperCloud.Assistant.v2
{
    /// <summary>
    /// A term from the request that was identified as an entity.
    /// </summary>
    [fsObject]
    public class RuntimeEntity
    {
        /// <summary>
        /// An entity detected in the input.
        /// </summary>
        [fsProperty("entity")]
        public string Entity { get; set; }
        /// <summary>
        /// An array of zero-based character offsets that indicate where the detected entity values begin and end in the
        /// input text.
        /// </summary>
        [fsProperty("location")]
        public List<long> Location { get; set; }
        /// <summary>
        /// The term in the input text that was recognized as an entity value.
        /// </summary>
        [fsProperty("value")]
        public string Value { get; set; }
        /// <summary>
        /// A decimal percentage that represents Watson's confidence in the entity.
        /// </summary>
        [fsProperty("confidence")]
        public float? Confidence { get; set; }
        /// <summary>
        /// Any metadata for the entity.
        /// </summary>
        [fsProperty("metadata")]
        public object Metadata { get; set; }
        /// <summary>
        /// The recognized capture groups for the entity, as defined by the entity pattern.
        /// </summary>
        [fsProperty("groups")]
        public List<CaptureGroup> Groups { get; set; }
    }

}
