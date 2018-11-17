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

namespace  IBM.Watson.DeveloperCloud.Services.CompareComply.v1
{
    /// <summary>
    /// The results of a single feedback query.
    /// </summary>
    [fsObject]
    public class GetFeedback
    {
        /// <summary>
        /// A string uniquely identifying the feedback entry.
        /// </summary>
        [fsProperty("feedback_id")]
        public string FeedbackId { get; set; }
        /// <summary>
        /// An optional identifier of the user submitting feedback.
        /// </summary>
        [fsProperty("user_id")]
        public string UserId { get; set; }
        /// <summary>
        /// A timestamp identifying the creation time of the feedback entry.
        /// </summary>
        [fsProperty("created")]
        public DateTime? Created { get; set; }
        /// <summary>
        /// A string containing the user's comment about the feedback entry.
        /// </summary>
        [fsProperty("comment")]
        public string Comment { get; set; }
        /// <summary>
        /// Information returned from the `POST /v1/feedback` method.
        /// </summary>
        [fsProperty("feedback_data")]
        public FeedbackDataOutput FeedbackData { get; set; }
    }

}
