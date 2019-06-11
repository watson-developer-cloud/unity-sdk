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

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// Workspace settings related to the Watson Assistant user interface.
    /// </summary>
    public class WorkspaceSystemSettingsTooling
    {
        /// <summary>
        /// Whether the dialog JSON editor displays text responses within the `output.generic` object.
        /// </summary>
        [JsonProperty("store_generic_responses", NullValueHandling = NullValueHandling.Ignore)]
        public bool? StoreGenericResponses { get; set; }
    }
}
