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

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// Options that modify how specified output is handled.
    /// </summary>
    [fsObject]
    public class DialogNodeOutputModifiers
    {
        /// <summary>
        /// Whether values in the output will overwrite output values in an array specified by previously executed
        /// dialog nodes. If this option is set to **false**, new values will be appended to previously specified
        /// values.
        /// </summary>
        [fsProperty("overwrite")]
        public bool? Overwrite { get; set; }
    }

}
