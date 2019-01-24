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

namespace IBM.Watson.Assistant.v1.Model
{
    /// <summary>
    /// CreateValue.
    /// </summary>
    public class CreateValue
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
        /// The text of the entity value. This string must conform to the following restrictions:
        /// - It cannot contain carriage return, newline, or tab characters.
        /// - It cannot consist of only whitespace characters.
        /// - It must be no longer than 64 characters.
        /// </summary>
        [fsProperty("value")]
        public string Value { get; set; }
        /// <summary>
        /// Any metadata related to the entity value.
        /// </summary>
        [fsProperty("metadata")]
        public object Metadata { get; set; }
        /// <summary>
        /// An array containing any synonyms for the entity value. You can provide either synonyms or patterns (as
        /// indicated by **type**), but not both. A synonym must conform to the following restrictions:
        /// - It cannot contain carriage return, newline, or tab characters.
        /// - It cannot consist of only whitespace characters.
        /// - It must be no longer than 64 characters.
        /// </summary>
        [fsProperty("synonyms")]
        public List<string> Synonyms { get; set; }
        /// <summary>
        /// An array of patterns for the entity value. You can provide either synonyms or patterns (as indicated by
        /// **type**), but not both. A pattern is a regular expression no longer than 512 characters. For more
        /// information about how to specify a pattern, see the
        /// [documentation](https://cloud.ibm.com/docs/services/assistant/entities.html#creating-entities).
        /// </summary>
        [fsProperty("patterns")]
        public List<string> Patterns { get; set; }
    }


}
