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

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// WorkspaceSystemSettingsDisambiguation
    /// </summary>
    [fsObject]
    public class WorkspaceSystemSettingsDisambiguation
    {
        /// <summary>
        /// The sensitivity of the disambiguation feature to intent detection conflicts. Set to **high** if you want the
        /// disambiguation feature to be triggered more often. This can be useful for testing or demonstration purposes.
        /// </summary>
        public enum SensitivityEnum
        {
            /// <summary>
            /// Enum auto for auto
            /// </summary>
            [EnumMember(Value = "auto")]
            AUTO,
            /// <summary>
            /// Enum high for high
            /// </summary>
            [EnumMember(Value = "high")]
            HIGH
        }

        /// <summary>
        /// The sensitivity of the disambiguation feature to intent detection conflicts. Set to **high** if you want the
        /// disambiguation feature to be triggered more often. This can be useful for testing or demonstration purposes.
        /// </summary>
        [fsProperty("sensitivity")]
        public SensitivityEnum? Sensitivity { get; set; }
        /// <summary>
        /// The text of the introductory prompt that accompanies disambiguation options presented to the user.
        /// </summary>
        [fsProperty("prompt")]
        public string Prompt { get; set; }
        /// <summary>
        /// The user-facing label for the option users can select if none of the suggested options is correct. If no
        /// value is specified for this property, this option does not appear.
        /// </summary>
        [fsProperty("none_of_the_above_prompt")]
        public string NoneOfTheAbovePrompt { get; set; }
        /// <summary>
        /// Whether the disambiguation feature is enabled for the workspace.
        /// </summary>
        [fsProperty("enabled")]
        public bool? Enabled { get; set; }
    }

}
