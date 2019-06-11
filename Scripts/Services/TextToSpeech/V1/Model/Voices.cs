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

namespace IBM.Watson.TextToSpeech.V1.Model
{
    /// <summary>
    /// Information about all available voice models.
    /// </summary>
    public class Voices
    {
        /// <summary>
        /// A list of available voices.
        /// </summary>
        [JsonProperty("voices", NullValueHandling = NullValueHandling.Ignore)]
        public List<Voice> _Voices { get; set; }
    }
}
