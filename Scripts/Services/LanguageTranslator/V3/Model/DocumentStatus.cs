/**
* (C) Copyright IBM Corp. 2018, 2020.
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

namespace IBM.Watson.LanguageTranslator.V3.Model
{
    /// <summary>
    /// Document information, including translation status.
    /// </summary>
    public class DocumentStatus
    {
        /// <summary>
        /// The status of the translation job associated with a submitted document.
        /// </summary>
        public class StatusValue
        {
            /// <summary>
            /// Constant PROCESSING for processing
            /// </summary>
            public const string PROCESSING = "processing";
            /// <summary>
            /// Constant AVAILABLE for available
            /// </summary>
            public const string AVAILABLE = "available";
            /// <summary>
            /// Constant FAILED for failed
            /// </summary>
            public const string FAILED = "failed";
            
        }

        /// <summary>
        /// The status of the translation job associated with a submitted document.
        /// Constants for possible values can be found using DocumentStatus.StatusValue
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        /// <summary>
        /// System generated ID identifying a document being translated using one specific translation model.
        /// </summary>
        [JsonProperty("document_id", NullValueHandling = NullValueHandling.Ignore)]
        public string DocumentId { get; set; }
        /// <summary>
        /// filename from the submission (if it was missing in the multipart-form, 'noname.<ext matching content type>'
        /// is used.
        /// </summary>
        [JsonProperty("filename", NullValueHandling = NullValueHandling.Ignore)]
        public string Filename { get; set; }
        /// <summary>
        /// A globally unique string that identifies the underlying model that is used for translation.
        /// </summary>
        [JsonProperty("model_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ModelId { get; set; }
        /// <summary>
        /// Model ID of the base model that was used to customize the model. If the model is not a custom model, this
        /// will be absent or an empty string.
        /// </summary>
        [JsonProperty("base_model_id", NullValueHandling = NullValueHandling.Ignore)]
        public string BaseModelId { get; set; }
        /// <summary>
        /// Translation source language code.
        /// </summary>
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; set; }
        /// <summary>
        /// Translation target language code.
        /// </summary>
        [JsonProperty("target", NullValueHandling = NullValueHandling.Ignore)]
        public string Target { get; set; }
        /// <summary>
        /// The time when the document was submitted.
        /// </summary>
        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Created { get; set; }
        /// <summary>
        /// The time when the translation completed.
        /// </summary>
        [JsonProperty("completed", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Completed { get; set; }
        /// <summary>
        /// An estimate of the number of words in the source document. Returned only if `status` is `available`.
        /// </summary>
        [JsonProperty("word_count", NullValueHandling = NullValueHandling.Ignore)]
        public long? WordCount { get; set; }
        /// <summary>
        /// The number of characters in the source document, present only if status=available.
        /// </summary>
        [JsonProperty("character_count", NullValueHandling = NullValueHandling.Ignore)]
        public long? CharacterCount { get; set; }
    }
}
