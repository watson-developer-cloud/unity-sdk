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

namespace  IBM.Watson.DeveloperCloud.Services.CompareComply.v1
{
    /// <summary>
    /// List of document attributes.
    /// </summary>
    [fsObject]
    public class Attribute
    {
        /// <summary>
        /// The type of attribute. Possible values are `Currency`, `DateTime`, and `Location`.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Enum currency for Currency
            /// </summary>
            [EnumMember(Value = "Currency")]
            Currency,
            /// <summary>
            /// Enum dateTime for DateTime
            /// </summary>
            [EnumMember(Value = "DateTime")]
            DateTime,
            /// <summary>
            /// Enum location for Location
            /// </summary>
            [EnumMember(Value = "Location")]
            Location,
            /// <summary>
            /// Enum ADDRESS for Address
            /// </summary>
            [EnumMember(Value = "Address")]
            ADDRESS,
        }

        /// <summary>
        /// The type of attribute. Possible values are `Currency`, `DateTime`, and `Location`.
        /// </summary>
        [fsProperty("type")]
        public Type? _Type { get; set; }
        /// <summary>
        /// The text associated with the attribute.
        /// </summary>
        [fsProperty("text")]
        public string Text { get; set; }
        /// <summary>
        /// The numeric location of the identified element in the document, represented with two integers labeled
        /// `begin` and `end`.
        /// </summary>
        [fsProperty("location")]
        public Location Location { get; set; }
    }

}
