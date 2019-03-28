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

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// An object defining the event being created.
    /// </summary>
    public class CreateEventResponse
    {
        /// <summary>
        /// The event type that was created.
        /// </summary>
        public class TypeValue
        {
            /// <summary>
            /// Constant CLICK for click
            /// </summary>
            public const string CLICK = "click";
            
        }

        /// <summary>
        /// The event type that was created.
        /// Constants for possible values can be found using CreateEventResponse.TypeValue
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
        /// <summary>
        /// Query event data object.
        /// </summary>
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public EventData Data { get; set; }
    }
}
