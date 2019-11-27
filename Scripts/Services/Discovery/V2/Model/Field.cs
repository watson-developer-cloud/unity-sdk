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

namespace IBM.Watson.Discovery.V2.Model
{
    /// <summary>
    /// Object containing field details.
    /// </summary>
    public class Field
    {
        /// <summary>
        /// The type of the field.
        /// </summary>
        public class TypeValue
        {
            /// <summary>
            /// Constant NESTED for nested
            /// </summary>
            public const string NESTED = "nested";
            /// <summary>
            /// Constant STRING for string
            /// </summary>
            public const string STRING = "string";
            /// <summary>
            /// Constant DATE for date
            /// </summary>
            public const string DATE = "date";
            /// <summary>
            /// Constant LONG for long
            /// </summary>
            public const string LONG = "long";
            /// <summary>
            /// Constant INTEGER for integer
            /// </summary>
            public const string INTEGER = "integer";
            /// <summary>
            /// Constant SHORT for short
            /// </summary>
            public const string SHORT = "short";
            /// <summary>
            /// Constant BYTE for byte
            /// </summary>
            public const string BYTE = "byte";
            /// <summary>
            /// Constant DOUBLE for double
            /// </summary>
            public const string DOUBLE = "double";
            /// <summary>
            /// Constant FLOAT for float
            /// </summary>
            public const string FLOAT = "float";
            /// <summary>
            /// Constant BOOLEAN for boolean
            /// </summary>
            public const string BOOLEAN = "boolean";
            /// <summary>
            /// Constant BINARY for binary
            /// </summary>
            public const string BINARY = "binary";
            
        }

        /// <summary>
        /// The type of the field.
        /// Constants for possible values can be found using Field.TypeValue
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
        /// <summary>
        /// The name of the field.
        /// </summary>
        [JsonProperty("field", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string _Field { get; private set; }
        /// <summary>
        /// The collection Id of the collection where the field was found.
        /// </summary>
        [JsonProperty("collection_id", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string CollectionId { get; private set; }
    }
}
