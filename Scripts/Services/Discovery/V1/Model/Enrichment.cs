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

using Newtonsoft.Json;

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// Enrichment step to perform on the document. Each enrichment is performed on the specified field in the order
    /// that they are listed in the configuration.
    /// </summary>
    public class Enrichment
    {
        /// <summary>
        /// Describes what the enrichment step does.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        /// <summary>
        /// Field where enrichments will be stored. This field must already exist or be at most 1 level deeper than an
        /// existing field. For example, if `text` is a top-level field with no sub-fields, `text.foo` is a valid
        /// destination but `text.foo.bar` is not.
        /// </summary>
        [JsonProperty("destination_field", NullValueHandling = NullValueHandling.Ignore)]
        public string DestinationField { get; set; }
        /// <summary>
        /// Field to be enriched.
        ///
        /// Arrays can be specified as the **source_field** if the **enrichment** service for this enrichment is set to
        /// `natural_language_undstanding`.
        /// </summary>
        [JsonProperty("source_field", NullValueHandling = NullValueHandling.Ignore)]
        public string SourceField { get; set; }
        /// <summary>
        /// Indicates that the enrichments will overwrite the destination_field field if it already exists.
        /// </summary>
        [JsonProperty("overwrite", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Overwrite { get; set; }
        /// <summary>
        /// Name of the enrichment service to call. Current options are `natural_language_understanding` and `elements`.
        ///
        ///  When using `natual_language_understanding`, the **options** object must contain Natural Language
        /// Understanding options.
        ///
        ///  When using `elements` the **options** object must contain Element Classification options. Additionally,
        /// when using the `elements` enrichment the configuration specified and files ingested must meet all the
        /// criteria specified in [the
        /// documentation](https://cloud.ibm.com/docs/services/discovery?topic=discovery-element-classification#element-classification).
        /// </summary>
        [JsonProperty("enrichment", NullValueHandling = NullValueHandling.Ignore)]
        public string _Enrichment { get; set; }
        /// <summary>
        /// If true, then most errors generated during the enrichment process will be treated as warnings and will not
        /// cause the document to fail processing.
        /// </summary>
        [JsonProperty("ignore_downstream_errors", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IgnoreDownstreamErrors { get; set; }
        /// <summary>
        /// Options which are specific to a particular enrichment.
        /// </summary>
        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public EnrichmentOptions Options { get; set; }
    }
}
