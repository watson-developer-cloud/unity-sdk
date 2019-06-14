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
using System;

namespace IBM.Watson.CompareComply.V1.Model
{
    /// <summary>
    /// The results of a successful **Get Feedback** request for a single feedback entry.
    /// </summary>
    public class GetFeedback
    {
        /// <summary>
        /// A string uniquely identifying the feedback entry.
        /// </summary>
        [JsonProperty("feedback_id", NullValueHandling = NullValueHandling.Ignore)]
        public string FeedbackId { get; set; }
        /// <summary>
        /// A timestamp identifying the creation time of the feedback entry.
        /// </summary>
        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Created { get; set; }
        /// <summary>
        /// A string containing the user's comment about the feedback entry.
        /// </summary>
        [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
        public string Comment { get; set; }
        /// <summary>
        /// Information returned from the **Add Feedback** method.
        /// </summary>
        [JsonProperty("feedback_data", NullValueHandling = NullValueHandling.Ignore)]
        public FeedbackDataOutput FeedbackData { get; set; }
    }
}
