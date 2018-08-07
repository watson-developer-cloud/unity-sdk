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

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// DialogRuntimeResponseTypeOption
    /// </summary>
    [fsObject]
    public class DialogRuntimeResponseTypeOption
    {
        /// <summary>
        /// The preferred type of control to display.
        /// </summary>
        public enum PreferenceEnum
        {
            /// <summary>
            /// Enum dropdown for dropdown
            /// </summary>
            [EnumMember(Value = "dropdown")]
            DROPDOWN,
            /// <summary>
            /// Enum button for button
            /// </summary>
            [EnumMember(Value = "button")]
            BUTTON
        }

        /// <summary>
        /// The preferred type of control to display.
        /// </summary>
        [fsProperty("preference")]
        public PreferenceEnum? Preference { get; set; }
        /// <summary>
        /// The title or introductory text to show before the response.
        /// </summary>
        [fsProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// The description to show with the the response.
        /// </summary>
        [fsProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// An array of objects describing the options from which the user can choose.
        /// </summary>
        [fsProperty("options")]
        public List<DialogNodeOutputOptionsElement> Options { get; set; }
    }

}
