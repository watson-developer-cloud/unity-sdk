

using System.Collections.Generic;
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
namespace IBM.Watson
{
    public class WatsonError
    {
        /// <summary>
        /// The url that generated the error.
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// The error code returned from the server.
        /// </summary>
        public long StatusCode { get; set; }
        /// <summary>
        /// The error message returned from the server.
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// The contents of the response from the server.
        /// </summary>
        public string Response { get; set; }
        /// <summary>
        /// Dictionary of headers returned by the request.
        /// </summary>
        public Dictionary<string, string> ResponseHeaders { get; set; }
    }
}
