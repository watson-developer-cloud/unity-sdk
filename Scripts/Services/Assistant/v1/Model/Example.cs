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
using System;

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// Example.
    /// </summary>
    [fsObject]
    public class Example
    {
        /// <summary>
        /// The text of the user input example.
        /// </summary>
        /// <value>The text of the user input example.</value>
        [fsProperty("text")]
        public string ExampleText { get; set; }
        /// <summary>
        /// The timestamp for creation of the example.
        /// </summary>
        /// <value>The timestamp for creation of the example.</value>
        [fsProperty("created")]
        public virtual DateTime Created { get; private set; }
        /// <summary>
        /// The timestamp for the last update to the example.
        /// </summary>
        /// <value>The timestamp for the last update to the example.</value>
        [fsProperty("updated")]
        public virtual DateTime Updated { get; private set; }
    }

}
