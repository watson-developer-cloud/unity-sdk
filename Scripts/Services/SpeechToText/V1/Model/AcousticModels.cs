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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IBM.Watson.SpeechToText.V1.Model
{
    /// <summary>
    /// Information about existing custom acoustic models.
    /// </summary>
    public class AcousticModels
    {
        /// <summary>
        /// An array of `AcousticModel` objects that provides information about each available custom acoustic model.
        /// The array is empty if the requesting credentials own no custom acoustic models (if no language is specified)
        /// or own no custom acoustic models for the specified language.
        /// </summary>
        [JsonProperty("customizations", NullValueHandling = NullValueHandling.Ignore)]
        public List<AcousticModel> Customizations { get; set; }
    }
}
