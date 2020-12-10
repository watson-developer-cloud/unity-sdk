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
    /// An object that describes a response with response type `text`.
    /// </summary>
    public class DialogNodeOutputGenericDialogNodeOutputResponseTypeText : DialogNodeOutputGeneric
    {
        /// <summary>
        /// The type of response returned by the dialog node. The specified response type must be supported by the
        /// client application or channel.
        /// </summary>
        public class ResponseTypeValue
        {
            /// <summary>
            /// Constant TEXT for text
            /// </summary>
            public const string TEXT = "text";
            
        }

        /// <summary>
        /// How a response is selected from the list, if more than one response is specified.
        /// </summary>
        public class SelectionPolicyValue
        {
            /// <summary>
            /// Constant SEQUENTIAL for sequential
            /// </summary>
            public const string SEQUENTIAL = "sequential";
            /// <summary>
            /// Constant RANDOM for random
            /// </summary>
            public const string RANDOM = "random";
            /// <summary>
            /// Constant MULTILINE for multiline
            /// </summary>
            public const string MULTILINE = "multiline";
            
        }

        /// <summary>
        /// A list of one or more objects defining text responses.
        /// </summary>
        [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
        public new List<DialogNodeOutputTextValuesElement> Values
        {
            get { return base.Values; }
            set { base.Values = value; }
        }
        /// <summary>
        /// The delimiter to use as a separator between responses when `selection_policy`=`multiline`.
        /// </summary>
        [JsonProperty("delimiter", NullValueHandling = NullValueHandling.Ignore)]
        public new string Delimiter
        {
            get { return base.Delimiter; }
            set { base.Delimiter = value; }
        }
    }
}
