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

namespace  IBM.Watson.DeveloperCloud.Services.CompareComply.v1
{
    /// <summary>
    /// Feedback data for submission.
    /// </summary>
    [fsObject]
    public class FeedbackDataInput
    {
        /// <summary>
        /// An optional identifier of the user submitting feedback.
        /// </summary>
        [fsProperty("user_id")]
        public string UserId { get; set; }
        /// <summary>
        /// An optional comment about the feedback.
        /// </summary>
        [fsProperty("comment")]
        public string Comment { get; set; }
        /// <summary>
        /// The type of feedback. The only permitted value is `element_classification`.
        /// </summary>
        [fsProperty("feedback_type")]
        public string FeedbackType { get; set; }
        /// <summary>
        /// Brief information about the input document.
        /// </summary>
        [fsProperty("document")]
        public ShortDoc Document { get; set; }
        /// <summary>
        /// An optional string identifying the model ID. The only permitted value is `contracts`.
        /// </summary>
        [fsProperty("model_id")]
        public string ModelId { get; set; }
        /// <summary>
        /// An optional string identifying the version of the model used.
        /// </summary>
        [fsProperty("model_version")]
        public string ModelVersion { get; set; }
        /// <summary>
        /// The numeric location of the identified element in the document, represented with two integers labeled
        /// `begin` and `end`.
        /// </summary>
        [fsProperty("location")]
        public Location Location { get; set; }
        /// <summary>
        /// The text on which to submit feedback.
        /// </summary>
        [fsProperty("text")]
        public string Text { get; set; }
        /// <summary>
        /// The original labeling from the input document, without the submitted feedback.
        /// </summary>
        [fsProperty("original_labels")]
        public OriginalLabelsIn OriginalLabels { get; set; }
        /// <summary>
        /// The updated labeling from the input document, accounting for the submitted feedback.
        /// </summary>
        [fsProperty("updated_labels")]
        public UpdatedLabelsIn UpdatedLabels { get; set; }
    }

}
