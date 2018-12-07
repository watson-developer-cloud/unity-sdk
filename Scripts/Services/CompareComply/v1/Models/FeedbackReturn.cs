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
    /// Information about the document and the submitted feedback.
    /// </summary>
    [fsObject]
    public class FeedbackReturn
    {
        /// <summary>
        /// The unique ID of the feedback object.
        /// </summary>
        [fsProperty("feedback_id")]
        public string FeedbackId { get; set; }
        /// <summary>
        /// An optional string identifying the person submitting feedback.
        /// </summary>
        [fsProperty("user_id")]
        public string UserId { get; set; }
        /// <summary>
        /// An optional comment from the person submitting the feedback.
        /// </summary>
        [fsProperty("comment")]
        public string Comment { get; set; }
        /// <summary>
        /// Timestamp listing the creation time of the feedback submission.
        /// </summary>
        [fsProperty("created")]
        public DateTime? Created { get; set; }
        /// <summary>
        /// Information returned from the `POST /v1/feedback` method.
        /// </summary>
        [fsProperty("feedback_data")]
        public FeedbackDataOutput FeedbackData { get; set; }
    }

}
