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

namespace IBM.Watson.CompareComply.V1.Model
{
    /// <summary>
    /// The analysis of objects returned by the **Element classification** method.
    /// </summary>
    public class ClassifyReturn
    {
        /// <summary>
        /// Basic information about the input document.
        /// </summary>
        [JsonProperty("document", NullValueHandling = NullValueHandling.Ignore)]
        public Document Document { get; set; }
        /// <summary>
        /// The analysis model used to classify the input document. For the **Element classification** method, the only
        /// valid value is `contracts`.
        /// </summary>
        [JsonProperty("model_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ModelId { get; set; }
        /// <summary>
        /// The version of the analysis model identified by the value of the `model_id` key.
        /// </summary>
        [JsonProperty("model_version", NullValueHandling = NullValueHandling.Ignore)]
        public string ModelVersion { get; set; }
        /// <summary>
        /// Document elements identified by the service.
        /// </summary>
        [JsonProperty("elements", NullValueHandling = NullValueHandling.Ignore)]
        public List<Element> Elements { get; set; }
        /// <summary>
        /// Definition of tables identified in the input document.
        /// </summary>
        [JsonProperty("tables", NullValueHandling = NullValueHandling.Ignore)]
        public List<Tables> Tables { get; set; }
        /// <summary>
        /// The structure of the input document.
        /// </summary>
        [JsonProperty("document_structure", NullValueHandling = NullValueHandling.Ignore)]
        public DocStructure DocumentStructure { get; set; }
        /// <summary>
        /// Definitions of the parties identified in the input document.
        /// </summary>
        [JsonProperty("parties", NullValueHandling = NullValueHandling.Ignore)]
        public List<Parties> Parties { get; set; }
        /// <summary>
        /// The date or dates on which the document becomes effective.
        /// </summary>
        [JsonProperty("effective_dates", NullValueHandling = NullValueHandling.Ignore)]
        public List<EffectiveDates> EffectiveDates { get; set; }
        /// <summary>
        /// The monetary amounts that identify the total amount of the contract that needs to be paid from one party to
        /// another.
        /// </summary>
        [JsonProperty("contract_amounts", NullValueHandling = NullValueHandling.Ignore)]
        public List<ContractAmts> ContractAmounts { get; set; }
        /// <summary>
        /// The date or dates on which the document is to be terminated.
        /// </summary>
        [JsonProperty("termination_dates", NullValueHandling = NullValueHandling.Ignore)]
        public List<TerminationDates> TerminationDates { get; set; }
        /// <summary>
        /// The document's contract type or types as declared in the document.
        /// </summary>
        [JsonProperty("contract_type", NullValueHandling = NullValueHandling.Ignore)]
        public List<ContractType> ContractType { get; set; }
    }
}
