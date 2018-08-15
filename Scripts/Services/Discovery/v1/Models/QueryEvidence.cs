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

namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    /// <summary>
    /// Description of evidence location supporting Knoweldge Graph query result.
    /// </summary>
    [fsObject]
    public class QueryEvidence
    {
        /// <summary>
        /// The docuemnt ID (as indexed in Discovery) of the evidence location.
        /// </summary>
        [fsProperty("document_id")]
        public string DocumentId { get; set; }
        /// <summary>
        /// The field of the document where the supporting evidence was identified.
        /// </summary>
        [fsProperty("field")]
        public string Field { get; set; }
        /// <summary>
        /// The start location of the evidence in the identified field. This value is inclusive.
        /// </summary>
        [fsProperty("start_offset")]
        public long? StartOffset { get; set; }
        /// <summary>
        /// The end location of the evidence in the identified field. This value is inclusive.
        /// </summary>
        [fsProperty("end_offset")]
        public long? EndOffset { get; set; }
        /// <summary>
        /// An array of entity objects that show evidence of the result.
        /// </summary>
        [fsProperty("entities")]
        public List<QueryEvidenceEntity> Entities { get; set; }
    }

}
