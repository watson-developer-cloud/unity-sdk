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
    /// List of document attributes.
    /// </summary>
    public class Attribute
    {
        /// <summary>
        /// The type of attribute.
        /// </summary>
        public class TypeValue
        {
            /// <summary>
            /// Constant CURRENCY for Currency
            /// </summary>
            public const string CURRENCY = "Currency";
            /// <summary>
            /// Constant DATETIME for DateTime
            /// </summary>
            public const string DATETIME = "DateTime";
            /// <summary>
            /// Constant DEFINEDTERM for DefinedTerm
            /// </summary>
            public const string DEFINEDTERM = "DefinedTerm";
            /// <summary>
            /// Constant DURATION for Duration
            /// </summary>
            public const string DURATION = "Duration";
            /// <summary>
            /// Constant LOCATION for Location
            /// </summary>
            public const string LOCATION = "Location";
            /// <summary>
            /// Constant NUMBER for Number
            /// </summary>
            public const string NUMBER = "Number";
            /// <summary>
            /// Constant ORGANIZATION for Organization
            /// </summary>
            public const string ORGANIZATION = "Organization";
            /// <summary>
            /// Constant PERCENTAGE for Percentage
            /// </summary>
            public const string PERCENTAGE = "Percentage";
            /// <summary>
            /// Constant PERSON for Person
            /// </summary>
            public const string PERSON = "Person";
            
        }

        /// <summary>
        /// The type of attribute.
        /// Constants for possible values can be found using Attribute.TypeValue
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
        /// <summary>
        /// The text associated with the attribute.
        /// </summary>
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
        /// <summary>
        /// The numeric location of the identified element in the document, represented with two integers labeled
        /// `begin` and `end`.
        /// </summary>
        [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
        public Location Location { get; set; }
    }
}
