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

namespace IBM.Watson.SpeechToText.V1.Model
{
    /// <summary>
    /// Information about a current asynchronous speech recognition job.
    /// </summary>
    public class RecognitionJob
    {
        /// <summary>
        /// The current status of the job:
        /// * `waiting`: The service is preparing the job for processing. The service returns this status when the job
        /// is initially created or when it is waiting for capacity to process the job. The job remains in this state
        /// until the service has the capacity to begin processing it.
        /// * `processing`: The service is actively processing the job.
        /// * `completed`: The service has finished processing the job. If the job specified a callback URL and the
        /// event `recognitions.completed_with_results`, the service sent the results with the callback notification.
        /// Otherwise, you must retrieve the results by checking the individual job.
        /// * `failed`: The job failed.
        /// </summary>
        public class StatusValue
        {
            /// <summary>
            /// Constant WAITING for waiting
            /// </summary>
            public const string WAITING = "waiting";
            /// <summary>
            /// Constant PROCESSING for processing
            /// </summary>
            public const string PROCESSING = "processing";
            /// <summary>
            /// Constant COMPLETED for completed
            /// </summary>
            public const string COMPLETED = "completed";
            /// <summary>
            /// Constant FAILED for failed
            /// </summary>
            public const string FAILED = "failed";
            
        }

        /// <summary>
        /// The current status of the job:
        /// * `waiting`: The service is preparing the job for processing. The service returns this status when the job
        /// is initially created or when it is waiting for capacity to process the job. The job remains in this state
        /// until the service has the capacity to begin processing it.
        /// * `processing`: The service is actively processing the job.
        /// * `completed`: The service has finished processing the job. If the job specified a callback URL and the
        /// event `recognitions.completed_with_results`, the service sent the results with the callback notification.
        /// Otherwise, you must retrieve the results by checking the individual job.
        /// * `failed`: The job failed.
        /// Constants for possible values can be found using RecognitionJob.StatusValue
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        /// <summary>
        /// The ID of the asynchronous job.
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
        /// <summary>
        /// The date and time in Coordinated Universal Time (UTC) at which the job was created. The value is provided in
        /// full ISO 8601 format (`YYYY-MM-DDThh:mm:ss.sTZD`).
        /// </summary>
        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public string Created { get; set; }
        /// <summary>
        /// The date and time in Coordinated Universal Time (UTC) at which the job was last updated by the service. The
        /// value is provided in full ISO 8601 format (`YYYY-MM-DDThh:mm:ss.sTZD`). This field is returned only by the
        /// **Check jobs** and **Check a job** methods.
        /// </summary>
        [JsonProperty("updated", NullValueHandling = NullValueHandling.Ignore)]
        public string Updated { get; set; }
        /// <summary>
        /// The URL to use to request information about the job with the **Check a job** method. This field is returned
        /// only by the **Create a job** method.
        /// </summary>
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }
        /// <summary>
        /// The user token associated with a job that was created with a callback URL and a user token. This field can
        /// be returned only by the **Check jobs** method.
        /// </summary>
        [JsonProperty("user_token", NullValueHandling = NullValueHandling.Ignore)]
        public string UserToken { get; set; }
        /// <summary>
        /// If the status is `completed`, the results of the recognition request as an array that includes a single
        /// instance of a `SpeechRecognitionResults` object. This field is returned only by the **Check a job** method.
        /// </summary>
        [JsonProperty("results", NullValueHandling = NullValueHandling.Ignore)]
        public List<SpeechRecognitionResults> Results { get; set; }
        /// <summary>
        /// An array of warning messages about invalid parameters included with the request. Each warning includes a
        /// descriptive message and a list of invalid argument strings, for example, `"unexpected query parameter
        /// 'user_token', query parameter 'callback_url' was not specified"`. The request succeeds despite the warnings.
        /// This field can be returned only by the **Create a job** method.
        /// </summary>
        [JsonProperty("warnings", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Warnings { get; set; }
    }
}
