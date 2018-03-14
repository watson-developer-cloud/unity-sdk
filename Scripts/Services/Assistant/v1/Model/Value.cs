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
using System.Runtime.Serialization;
using System;

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// Value.
    /// </summary>
    [fsObject]
    public class Value
    {
        /// <summary>
        /// Specifies the type of value.
        /// </summary>
        /// <value>Specifies the type of value.</value>
        public enum ValueTypeEnum
        {
            
            /// <summary>
            /// Enum SYNONYMS for synonyms
            /// </summary>
            [EnumMember(Value = "synonyms")]
            SYNONYMS,
            
            /// <summary>
            /// Enum PATTERNS for patterns
            /// </summary>
            [EnumMember(Value = "patterns")]
            PATTERNS
        }

        /// <summary>
        /// Specifies the type of value.
        /// </summary>
        /// <value>Specifies the type of value.</value>
        [fsProperty("type")]
        public ValueTypeEnum? ValueType { get; set; }
        /// <summary>
        /// The text of the entity value.
        /// </summary>
        /// <value>The text of the entity value.</value>
        [fsProperty("value")]
        public string ValueText { get; set; }
        /// <summary>
        /// Any metadata related to the entity value.
        /// </summary>
        /// <value>Any metadata related to the entity value.</value>
        [fsProperty("metadata")]
        public object Metadata { get; set; }
        /// <summary>
        /// The timestamp for creation of the entity value.
        /// </summary>
        /// <value>The timestamp for creation of the entity value.</value>
        [fsProperty("created")]
        public virtual DateTime Created { get; private set; }
        /// <summary>
        /// The timestamp for the last update to the entity value.
        /// </summary>
        /// <value>The timestamp for the last update to the entity value.</value>
        [fsProperty("updated")]
        public virtual DateTime Updated { get; private set; }
        /// <summary>
        /// An array containing any synonyms for the entity value.
        /// </summary>
        /// <value>An array containing any synonyms for the entity value.</value>
        [fsProperty("synonyms")]
        public List<string> Synonyms { get; set; }
        /// <summary>
        /// An array containing any patterns for the entity value.
        /// </summary>
        /// <value>An array containing any patterns for the entity value.</value>
        [fsProperty("patterns")]
        public List<string> Patterns { get; set; }
    }

}
