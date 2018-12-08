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
namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    /// <summary>
    /// An object specifiying the concepts enrichment and related parameters.
    /// </summary>
    public class NluEnrichmentConcepts
    {
        /// <summary>
        /// The maximum number of concepts enrichments to extact from each instance of the specified field.
        /// </summary>
        [fsProperty("limit")]
        public long? Limit { get; set; }
    }

}
