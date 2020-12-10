/**
* (C) Copyright IBM Corp. 2020.
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
using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// An object that describes a response with response type `option`.
    /// </summary>
    public class RuntimeResponseGenericRuntimeResponseTypeOption : RuntimeResponseGeneric
    {
        /// <summary>
        /// The type of response returned by the dialog node. The specified response type must be supported by the
        /// client application or channel.
        /// </summary>
        public class ResponseTypeValue
        {
            /// <summary>
            /// Constant OPTION for option
            /// </summary>
            public const string OPTION = "option";
            
        }

        /// <summary>
        /// The preferred type of control to display.
        /// </summary>
        public class PreferenceValue
        {
            /// <summary>
            /// Constant DROPDOWN for dropdown
            /// </summary>
            public const string DROPDOWN = "dropdown";
            /// <summary>
            /// Constant BUTTON for button
            /// </summary>
            public const string BUTTON = "button";
            
        }

        /// <summary>
        /// The title or introductory text to show before the response.
        /// </summary>
        public new string Title
        {
            get { return base.Title; }
            set { base.Title = value; }
        }
        /// <summary>
        /// The description to show with the the response.
        /// </summary>
        public new string Description
        {
            get { return base.Description; }
            set { base.Description = value; }
        }
        /// <summary>
        /// An array of objects describing the options from which the user can choose.
        /// </summary>
        public new List<DialogNodeOutputOptionsElement> Options
        {
            get { return base.Options; }
            set { base.Options = value; }
        }
    }
}