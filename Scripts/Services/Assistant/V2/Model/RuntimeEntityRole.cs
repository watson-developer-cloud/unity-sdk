/**
* (C) Copyright IBM Corp. 2018, 2020.
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

namespace IBM.Watson.Assistant.V2.Model
{
    /// <summary>
    /// An object describing the role played by a system entity that is specifies the beginning or end of a range
    /// recognized in the user input. This property is included only if the new system entities are enabled for the
    /// skill.
    /// </summary>
    public class RuntimeEntityRole
    {
        /// <summary>
        /// The relationship of the entity to the range.
        /// </summary>
        public class TypeValue
        {
            /// <summary>
            /// Constant DATE_FROM for date_from
            /// </summary>
            public const string DATE_FROM = "date_from";
            /// <summary>
            /// Constant DATE_TO for date_to
            /// </summary>
            public const string DATE_TO = "date_to";
            /// <summary>
            /// Constant NUMBER_FROM for number_from
            /// </summary>
            public const string NUMBER_FROM = "number_from";
            /// <summary>
            /// Constant NUMBER_TO for number_to
            /// </summary>
            public const string NUMBER_TO = "number_to";
            /// <summary>
            /// Constant TIME_FROM for time_from
            /// </summary>
            public const string TIME_FROM = "time_from";
            /// <summary>
            /// Constant TIME_TO for time_to
            /// </summary>
            public const string TIME_TO = "time_to";
            
        }

        /// <summary>
        /// The relationship of the entity to the range.
        /// Constants for possible values can be found using RuntimeEntityRole.TypeValue
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
    }
}
