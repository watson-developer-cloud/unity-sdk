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
    /// Synonym.
    /// </summary>
    [fsObject]
    public class Synonym
    {
        /// <summary>
        /// The text of the synonym.
        /// </summary>
        /// <value>The text of the synonym.</value>
        [fsProperty("synonym")]
        public string SynonymText { get; set; }
        /// <summary>
        /// The timestamp for creation of the synonym.
        /// </summary>
        /// <value>The timestamp for creation of the synonym.</value>
        [fsProperty("created")]
        public virtual DateTime Created { get; private set; }
        /// <summary>
        /// The timestamp for the most recent update to the synonym.
        /// </summary>
        /// <value>The timestamp for the most recent update to the synonym.</value>
        [fsProperty("updated")]
        public virtual DateTime Updated { get; private set; }
    }

}
