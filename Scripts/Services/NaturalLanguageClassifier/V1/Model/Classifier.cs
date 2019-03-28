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

namespace IBM.Watson.NaturalLanguageClassifier.V1.Model
{
    /// <summary>
    /// A classifier for natural language phrases.
    /// </summary>
    public class Classifier
    {
        /// <summary>
        /// The state of the classifier.
        /// </summary>
        public class StatusValue
        {
            /// <summary>
            /// Constant NON_EXISTENT for Non Existent
            /// </summary>
            public const string NON_EXISTENT = "Non Existent";
            /// <summary>
            /// Constant TRAINING for Training
            /// </summary>
            public const string TRAINING = "Training";
            /// <summary>
            /// Constant FAILED for Failed
            /// </summary>
            public const string FAILED = "Failed";
            /// <summary>
            /// Constant AVAILABLE for Available
            /// </summary>
            public const string AVAILABLE = "Available";
            /// <summary>
            /// Constant UNAVAILABLE for Unavailable
            /// </summary>
            public const string UNAVAILABLE = "Unavailable";
            
        }

        /// <summary>
        /// The state of the classifier.
        /// Constants for possible values can be found using Classifier.StatusValue
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        /// <summary>
        /// User-supplied name for the classifier.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// Link to the classifier.
        /// </summary>
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }
        /// <summary>
        /// Unique identifier for this classifier.
        /// </summary>
        [JsonProperty("classifier_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ClassifierId { get; set; }
        /// <summary>
        /// Date and time (UTC) the classifier was created.
        /// </summary>
        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public virtual DateTime? Created { get; private set; }
        /// <summary>
        /// Additional detail about the status.
        /// </summary>
        [JsonProperty("status_description", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string StatusDescription { get; private set; }
        /// <summary>
        /// The language used for the classifier.
        /// </summary>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }
    }
}
