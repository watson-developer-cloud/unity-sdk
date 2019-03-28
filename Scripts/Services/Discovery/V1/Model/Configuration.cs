/**
* Copyright 2018, 2019 IBM Corp. All Rights Reserved.
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

using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// A custom configuration for the environment.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// The unique identifier of the configuration.
        /// </summary>
        [JsonProperty("configuration_id", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string ConfigurationId { get; private set; }
        /// <summary>
        /// The name of the configuration.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// The creation date of the configuration in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public virtual DateTime? Created { get; private set; }
        /// <summary>
        /// The timestamp of when the configuration was last updated in the format yyyy-MM-dd'T'HH:mm:ss.SSS'Z'.
        /// </summary>
        [JsonProperty("updated", NullValueHandling = NullValueHandling.Ignore)]
        public virtual DateTime? Updated { get; private set; }
        /// <summary>
        /// The description of the configuration, if available.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        /// <summary>
        /// Document conversion settings.
        /// </summary>
        [JsonProperty("conversions", NullValueHandling = NullValueHandling.Ignore)]
        public Conversions Conversions { get; set; }
        /// <summary>
        /// An array of document enrichment settings for the configuration.
        /// </summary>
        [JsonProperty("enrichments", NullValueHandling = NullValueHandling.Ignore)]
        public List<Enrichment> Enrichments { get; set; }
        /// <summary>
        /// Defines operations that can be used to transform the final output JSON into a normalized form. Operations
        /// are executed in the order that they appear in the array.
        /// </summary>
        [JsonProperty("normalizations", NullValueHandling = NullValueHandling.Ignore)]
        public List<NormalizationOperation> Normalizations { get; set; }
        /// <summary>
        /// Object containing source parameters for the configuration.
        /// </summary>
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public Source Source { get; set; }
    }
}
