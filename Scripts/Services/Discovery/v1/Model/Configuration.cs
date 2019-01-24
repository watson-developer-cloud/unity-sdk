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

namespace IBM.Watson.Discovery.v1.Model
{
    /// <summary>
    /// A custom configuration for the environment.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// The unique identifier of the configuration.
        /// </summary>
        [fsProperty("configuration_id")]
        public virtual string ConfigurationId { get; private set; }
        /// <summary>
        /// The name of the configuration.
        /// </summary>
        [fsProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// The creation date of the configuration in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        [fsProperty("created")]
        public virtual DateTime? Created { get; private set; }
        /// <summary>
        /// The timestamp of when the configuration was last updated in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        [fsProperty("updated")]
        public virtual DateTime? Updated { get; private set; }
        /// <summary>
        /// The description of the configuration, if available.
        /// </summary>
        [fsProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// The document conversion settings for the configuration.
        /// </summary>
        [fsProperty("conversions")]
        public Conversions Conversions { get; set; }
        /// <summary>
        /// An array of document enrichment settings for the configuration.
        /// </summary>
        [fsProperty("enrichments")]
        public object Enrichments { get; set; }
        /// <summary>
        /// An array of JSON normalization operations.
        /// </summary>
        [fsProperty("normalizations")]
        public object Normalizations { get; set; }
        /// <summary>
        /// Object containing source parameters for the configuration.
        /// </summary>
        [fsProperty("source")]
        public Source Source { get; set; }
    }


}
