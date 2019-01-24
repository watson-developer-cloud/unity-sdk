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

using FullSerializer;

namespace IBM.Watson.Discovery.v1.Model
{
    /// <summary>
    /// UpdateEnvironmentRequest.
    /// </summary>
    public class UpdateEnvironmentRequest
    {
        /// <summary>
        /// Size that the environment should be increased to. Environment size cannot be modified when using a Lite
        /// plan. Environment size can only increased and not decreased.
        /// </summary>
        public class SizeEnumValue
        {
            /// <summary>
            /// Constant S for S
            /// </summary>
            public const string S = "S";
            /// <summary>
            /// Constant MS for MS
            /// </summary>
            public const string MS = "MS";
            /// <summary>
            /// Constant M for M
            /// </summary>
            public const string M = "M";
            /// <summary>
            /// Constant ML for ML
            /// </summary>
            public const string ML = "ML";
            /// <summary>
            /// Constant L for L
            /// </summary>
            public const string L = "L";
            /// <summary>
            /// Constant XL for XL
            /// </summary>
            public const string XL = "XL";
            /// <summary>
            /// Constant XXL for XXL
            /// </summary>
            public const string XXL = "XXL";
            /// <summary>
            /// Constant XXXL for XXXL
            /// </summary>
            public const string XXXL = "XXXL";
            
        }

        /// <summary>
        /// Size that the environment should be increased to. Environment size cannot be modified when using a Lite
        /// plan. Environment size can only increased and not decreased.
        /// </summary>
        [fsProperty("size")]
        public string Size { get; set; }
        /// <summary>
        /// Name that identifies the environment.
        /// </summary>
        [fsProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// Description of the environment.
        /// </summary>
        [fsProperty("description")]
        public string Description { get; set; }
    }


}
