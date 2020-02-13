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

using System.Collections.Generic;
using IBM.Cloud.SDK.Model;
using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// The output of the dialog node. For more information about how to specify dialog node output, see the
    /// [documentation](https://cloud.ibm.com/docs/assistant?topic=assistant-dialog-overview#dialog-overview-responses).
    /// </summary>
    public class DialogNodeOutput: DynamicModel<object>
    {
        /// <summary>
        /// An array of objects describing the output defined for the dialog node.
        /// </summary>
        [JsonProperty("generic", NullValueHandling = NullValueHandling.Ignore)]
        public List<DialogNodeOutputGeneric> Generic { get; set; }
        /// <summary>
        /// Options that modify how specified output is handled.
        /// </summary>
        [JsonProperty("modifiers", NullValueHandling = NullValueHandling.Ignore)]
        public DialogNodeOutputModifiers Modifiers { get; set; }
    }
}
