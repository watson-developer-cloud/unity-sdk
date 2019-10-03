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

namespace IBM.Watson.VisualRecognition.V4.Model
{
    /// <summary>
    /// Details about the specific area of the problem.
    /// </summary>
    public class ErrorTarget
    {
        /// <summary>
        /// The parameter or property that is the focus of the problem.
        /// </summary>
        public class TypeValue
        {
            /// <summary>
            /// Constant FIELD for field
            /// </summary>
            public const string FIELD = "field";
            /// <summary>
            /// Constant PARAMETER for parameter
            /// </summary>
            public const string PARAMETER = "parameter";
            /// <summary>
            /// Constant HEADER for header
            /// </summary>
            public const string HEADER = "header";
            
        }

        /// <summary>
        /// The parameter or property that is the focus of the problem.
        /// Constants for possible values can be found using ErrorTarget.TypeValue
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
        /// <summary>
        /// The property that is identified with the problem.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }
}
