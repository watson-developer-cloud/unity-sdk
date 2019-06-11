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

namespace IBM.Watson.CompareComply.V1.Model
{
    /// <summary>
    /// Information returned from the **Add Feedback** method.
    /// </summary>
    public class FeedbackDataOutput
    {
        /// <summary>
        /// A string identifying the user adding the feedback. The only permitted value is `element_classification`.
        /// </summary>
        [JsonProperty("feedback_type", NullValueHandling = NullValueHandling.Ignore)]
        public string FeedbackType { get; set; }
        /// <summary>
        /// Brief information about the input document.
        /// </summary>
        [JsonProperty("document", NullValueHandling = NullValueHandling.Ignore)]
        public ShortDoc Document { get; set; }
        /// <summary>
        /// An optional string identifying the model ID. The only permitted value is `contracts`.
        /// </summary>
        [JsonProperty("model_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ModelId { get; set; }
        /// <summary>
        /// An optional string identifying the version of the model used.
        /// </summary>
        [JsonProperty("model_version", NullValueHandling = NullValueHandling.Ignore)]
        public string ModelVersion { get; set; }
        /// <summary>
        /// The numeric location of the identified element in the document, represented with two integers labeled
        /// `begin` and `end`.
        /// </summary>
        [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
        public Location Location { get; set; }
        /// <summary>
        /// The text to which the feedback applies.
        /// </summary>
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
        /// <summary>
        /// The original labeling from the input document, without the submitted feedback.
        /// </summary>
        [JsonProperty("original_labels", NullValueHandling = NullValueHandling.Ignore)]
        public OriginalLabelsOut OriginalLabels { get; set; }
        /// <summary>
        /// The updated labeling from the input document, accounting for the submitted feedback.
        /// </summary>
        [JsonProperty("updated_labels", NullValueHandling = NullValueHandling.Ignore)]
        public UpdatedLabelsOut UpdatedLabels { get; set; }
        /// <summary>
        /// Pagination details, if required by the length of the output.
        /// </summary>
        [JsonProperty("pagination", NullValueHandling = NullValueHandling.Ignore)]
        public Pagination Pagination { get; set; }
    }
}
