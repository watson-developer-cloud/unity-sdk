/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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

using IBM.Cloud.SDK.Connection;
using System.Collections.Generic;

namespace IBM.Cloud.SDK
{
    /// <summary>
    /// The request object for all REST requests.
    /// </summary>
    /// <typeparam name="T">The type to be returned in the callback.</typeparam>
    public class RequestObject<T> : RESTConnector.Request
    {
        /// <summary>
        /// The success callback.
        /// </summary>
        public Callback<T> Callback { get; set; }
        /// <summary>
        /// Custom data.
        /// </summary>
        public Dictionary<string, object> CustomData { get; set; }
    }
}
