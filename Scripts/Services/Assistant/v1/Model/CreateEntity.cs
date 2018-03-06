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
    /// CreateEntity.
    /// </summary>
    [fsObject]
    public class CreateEntity
    {
        /// <summary>
        /// The name of the entity.
        /// </summary>
        /// <value>The name of the entity.</value>
        public string Entity { get; set; }
        /// <summary>
        /// The description of the entity.
        /// </summary>
        /// <value>The description of the entity.</value>
        public string Description { get; set; }
        /// <summary>
        /// Any metadata related to the value.
        /// </summary>
        /// <value>Any metadata related to the value.</value>
        public object Metadata { get; set; }
        /// <summary>
        /// An array of entity values.
        /// </summary>
        /// <value>An array of entity values.</value>
        public List<CreateValue> Values { get; set; }
        /// <summary>
        /// Whether to use fuzzy matching for the entity.
        /// </summary>
        /// <value>Whether to use fuzzy matching for the entity.</value>
        public bool? FuzzyMatch { get; set; }
    }

}
