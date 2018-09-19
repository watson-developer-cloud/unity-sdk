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

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// An object describing a contextual entity mention.
    /// </summary>
    [fsObject]
    public class EntityMention
    {
        /// <summary>
        /// The text of the user input example.
        /// </summary>
        [fsProperty("text")]
        public string ExampleText { get; set; }
        /// <summary>
        /// The name of the intent.
        /// </summary>
        [fsProperty("intent")]
        public string IntentName { get; set; }
        /// <summary>
        /// An array of zero-based character offsets that indicate where the entity mentions begin and end in the input
        /// text.
        /// </summary>
        [fsProperty("location")]
        public List<int> Location { get; set; }
    }

}
