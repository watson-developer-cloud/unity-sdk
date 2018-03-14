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
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// SynonymCollection.
    /// </summary>
    [fsObject]
    public class SynonymCollection
    {
        /// <summary>
        /// An array of synonyms.
        /// </summary>
        /// <value>An array of synonyms.</value>
        [fsProperty("synonyms")]
        public List<Synonym> Synonyms { get; set; }
        /// <summary>
        /// The pagination data for the returned objects.
        /// </summary>
        /// <value>The pagination data for the returned objects.</value>
        [fsProperty("pagination")]
        public Pagination Pagination { get; set; }
    }

}
