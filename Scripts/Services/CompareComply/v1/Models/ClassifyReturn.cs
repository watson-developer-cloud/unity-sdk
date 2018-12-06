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

namespace  IBM.Watson.DeveloperCloud.Services.CompareComply.v1
{
    /// <summary>
    /// The analysis of objects returned by the `/v1/element_classification` method.
    /// </summary>
    [fsObject]
    public class ClassifyReturn
    {
        /// <summary>
        /// Basic information about the input document.
        /// </summary>
        [fsProperty("document")]
        public Document Document { get; set; }
        /// <summary>
        /// The analysis model used to classify the input document. For the `/v1/element_classification` method, the
        /// only valid value is `contracts`.
        /// </summary>
        [fsProperty("model_id")]
        public string ModelId { get; set; }
        /// <summary>
        /// The version of the analysis model identified by the value of the `model_id` key.
        /// </summary>
        [fsProperty("model_version")]
        public string ModelVersion { get; set; }
        /// <summary>
        /// Document elements identified by the service.
        /// </summary>
        [fsProperty("elements")]
        public List<Element> Elements { get; set; }
        /// <summary>
        /// Definition of tables identified in the input document.
        /// </summary>
        [fsProperty("tables")]
        public List<Tables> Tables { get; set; }
        /// <summary>
        /// The structure of the input document.
        /// </summary>
        [fsProperty("document_structure")]
        public DocStructure DocumentStructure { get; set; }
        /// <summary>
        /// Definitions of the parties identified in the input document.
        /// </summary>
        [fsProperty("parties")]
        public List<Parties> Parties { get; set; }
        /// <summary>
        /// The effective dates of the input document.
        /// </summary>
        [fsProperty("effective_dates")]
        public List<EffectiveDates> EffectiveDates { get; set; }
        /// <summary>
        /// The monetary amounts identified in the input document.
        /// </summary>
        [fsProperty("contract_amounts")]
        public List<ContractAmts> ContractAmounts { get; set; }
        /// <summary>
        /// The input document's termination dates.
        /// </summary>
        [fsProperty("termination_dates")]
        public List<TerminationDates> TerminationDates { get; set; }
    }

}
