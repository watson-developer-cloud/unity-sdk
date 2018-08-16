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
using System.Runtime.Serialization;

namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    /// <summary>
    /// An object defining the event being created.
    /// </summary>
    [fsObject]
    public class CreateEventObject
    {
        /// <summary>
        /// The event type to be created.
        /// </summary>
        public enum TypeEnum
        {
            /// <summary>
            /// Enum click for click
            /// </summary>
            click
        }

        /// <summary>
        /// The event type to be created.
        /// </summary>
        [fsProperty("type")]
        public TypeEnum? Type { get; set; }
        /// <summary>
        /// Data object used to create a query event.
        /// </summary>
        [fsProperty("data")]
        public EventData Data { get; set; }
    }

}
