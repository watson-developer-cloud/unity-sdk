/**
* (C) Copyright IBM Corp. 2021.
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

namespace IBM.Watson.TextToSpeech.V1.Model
{
    /// <summary>
    /// Information about the custom prompts that are defined for a custom model.
    /// </summary>
    public class Prompts
    {
        /// <summary>
        /// An array of `Prompt` objects that provides information about the prompts that are defined for the specified
        /// custom model. The array is empty if no prompts are defined for the custom model.
        /// </summary>
        [JsonProperty("prompts", NullValueHandling = NullValueHandling.Ignore)]
        public List<Prompt> _Prompts { get; set; }
    }
}
