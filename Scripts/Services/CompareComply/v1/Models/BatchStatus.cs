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
using System;
using System.Runtime.Serialization;

namespace  IBM.Watson.DeveloperCloud.Services.CompareComply.v1
{
    /// <summary>
    /// The batch-request status.
    /// </summary>
    [fsObject]
    public class BatchStatus
    {
        /// <summary>
        /// The method to be run against the documents. Possible values are `html_conversion`, `element_classification`,
        /// and `tables`.
        /// </summary>
        public enum Function
        {
            /// <summary>
            /// Enum elementClassification for element_classification
            /// </summary>
            [EnumMember(Value = "element_classification")]
            element_classification,
            /// <summary>
            /// Enum htmlConversion for html_conversion
            /// </summary>
            [EnumMember(Value = "html_conversion")]
            html_conversion,
            /// <summary>
            /// Enum tables for tables
            /// </summary>
            [EnumMember(Value = "tables")]
            tables
        }

        /// <summary>
        /// The method to be run against the documents. Possible values are `html_conversion`, `element_classification`,
        /// and `tables`.
        /// </summary>
        [fsProperty("function")]
        public Function? _Function { get; set; }
        /// <summary>
        /// The geographical location of the Cloud Object Storage input bucket as listed on the **Endpoint** tab of your
        /// COS instance; for example, `us-geo`, `eu-geo`, or `ap-geo`.
        /// </summary>
        [fsProperty("input_bucket_location")]
        public string InputBucketLocation { get; set; }
        /// <summary>
        /// The name of the Cloud Object Storage input bucket.
        /// </summary>
        [fsProperty("input_bucket_name")]
        public string InputBucketName { get; set; }
        /// <summary>
        /// The geographical location of the Cloud Object Storage output bucket as listed on the **Endpoint** tab of
        /// your COS instance; for example, `us-geo`, `eu-geo`, or `ap-geo`.
        /// </summary>
        [fsProperty("output_bucket_location")]
        public string OutputBucketLocation { get; set; }
        /// <summary>
        /// The name of the Cloud Object Storage output bucket.
        /// </summary>
        [fsProperty("output_bucket_name")]
        public string OutputBucketName { get; set; }
        /// <summary>
        /// The unique identifier for the batch request.
        /// </summary>
        [fsProperty("batch_id")]
        public string BatchId { get; set; }
        /// <summary>
        /// Document counts.
        /// </summary>
        [fsProperty("document_counts")]
        public DocCounts DocumentCounts { get; set; }
        /// <summary>
        /// The status of the batch request.
        /// </summary>
        [fsProperty("status")]
        public string Status { get; set; }
        /// <summary>
        /// The creation time of the batch request.
        /// </summary>
        [fsProperty("created")]
        public DateTime? Created { get; set; }
        /// <summary>
        /// The time of the most recent update to the batch request.
        /// </summary>
        [fsProperty("updated")]
        public DateTime? Updated { get; set; }
    }

}
