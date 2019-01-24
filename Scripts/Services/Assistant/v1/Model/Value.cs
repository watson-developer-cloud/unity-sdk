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
using System;

namespace IBM.Watson.Assistant.v1.Model
{
    /// <summary>
    /// Value.
    /// </summary>
    public class Value
    {
        /// <summary>
        /// Specifies the type of value.
        /// </summary>
        public class ValueTypeEnumValue
        {
            /// <summary>
            /// Constant SYNONYMS for synonyms
            /// </summary>
            public const string SYNONYMS = "synonyms";
            /// <summary>
            /// Constant PATTERNS for patterns
            /// </summary>
            public const string PATTERNS = "patterns";
            
        }

        /// <summary>
        /// Specifies the type of value.
        /// </summary>
        [fsProperty("type")]
        public string ValueType { get; set; }
        /// <summary>
        /// The text of the entity value.
        /// </summary>
        [fsProperty("value")]
        public string ValueText { get; set; }
        /// <summary>
        /// Any metadata related to the entity value.
        /// </summary>
        [fsProperty("metadata")]
        public object Metadata { get; set; }
        /// <summary>
        /// The timestamp for creation of the entity value.
        /// </summary>
        [fsProperty("created")]
        public virtual DateTime? Created { get; private set; }
        /// <summary>
        /// The timestamp for the last update to the entity value.
        /// </summary>
        [fsProperty("updated")]
        public virtual DateTime? Updated { get; private set; }
        /// <summary>
        /// An array containing any synonyms for the entity value.
        /// </summary>
        [fsProperty("synonyms")]
        public List<string> Synonyms { get; set; }
        /// <summary>
        /// An array containing any patterns for the entity value.
        /// </summary>
        [fsProperty("patterns")]
        public List<string> Patterns { get; set; }
    }


}
