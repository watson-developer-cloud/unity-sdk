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

using FullSerializer;
using System;

namespace IBM.Watson.Assistant.v1.Model
{
    /// <summary>
    /// Entity.
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// The name of the entity.
        /// </summary>
        [fsProperty("entity")]
        public string EntityName { get; set; }
        /// <summary>
        /// The timestamp for creation of the entity.
        /// </summary>
        [fsProperty("created")]
        public virtual DateTime? Created { get; private set; }
        /// <summary>
        /// The timestamp for the last update to the entity.
        /// </summary>
        [fsProperty("updated")]
        public virtual DateTime? Updated { get; private set; }
        /// <summary>
        /// The description of the entity.
        /// </summary>
        [fsProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// Any metadata related to the entity.
        /// </summary>
        [fsProperty("metadata")]
        public object Metadata { get; set; }
        /// <summary>
        /// Whether fuzzy matching is used for the entity.
        /// </summary>
        [fsProperty("fuzzy_match")]
        public bool? FuzzyMatch { get; set; }
    }


}
