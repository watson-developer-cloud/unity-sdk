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

namespace IBM.Watson.LanguageTranslator.V3.Model
{
    /// <summary>
    /// Response payload for models.
    /// </summary>
    public class TranslationModel
    {
        /// <summary>
        /// Availability of a model.
        /// </summary>
        public class StatusValue
        {
            /// <summary>
            /// Constant UPLOADING for uploading
            /// </summary>
            public const string UPLOADING = "uploading";
            /// <summary>
            /// Constant UPLOADED for uploaded
            /// </summary>
            public const string UPLOADED = "uploaded";
            /// <summary>
            /// Constant DISPATCHING for dispatching
            /// </summary>
            public const string DISPATCHING = "dispatching";
            /// <summary>
            /// Constant QUEUED for queued
            /// </summary>
            public const string QUEUED = "queued";
            /// <summary>
            /// Constant TRAINING for training
            /// </summary>
            public const string TRAINING = "training";
            /// <summary>
            /// Constant TRAINED for trained
            /// </summary>
            public const string TRAINED = "trained";
            /// <summary>
            /// Constant PUBLISHING for publishing
            /// </summary>
            public const string PUBLISHING = "publishing";
            /// <summary>
            /// Constant AVAILABLE for available
            /// </summary>
            public const string AVAILABLE = "available";
            /// <summary>
            /// Constant DELETED for deleted
            /// </summary>
            public const string DELETED = "deleted";
            /// <summary>
            /// Constant ERROR for error
            /// </summary>
            public const string ERROR = "error";
            
        }

        /// <summary>
        /// Availability of a model.
        /// Constants for possible values can be found using TranslationModel.StatusValue
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        /// <summary>
        /// A globally unique string that identifies the underlying model that is used for translation.
        /// </summary>
        [JsonProperty("model_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ModelId { get; set; }
        /// <summary>
        /// Optional name that can be specified when the model is created.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
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
        /// Model ID of the base model that was used to customize the model. If the model is not a custom model, this
        /// will be an empty string.
        /// </summary>
        [JsonProperty("base_model_id", NullValueHandling = NullValueHandling.Ignore)]
        public string BaseModelId { get; set; }
        /// <summary>
        /// The domain of the translation model.
        /// </summary>
        [JsonProperty("domain", NullValueHandling = NullValueHandling.Ignore)]
        public string Domain { get; set; }
        /// <summary>
        /// Whether this model can be used as a base for customization. Customized models are not further customizable,
        /// and some base models are not customizable.
        /// </summary>
        [JsonProperty("customizable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Customizable { get; set; }
        /// <summary>
        /// Whether or not the model is a default model. A default model is the model for a given language pair that
        /// will be used when that language pair is specified in the source and target parameters.
        /// </summary>
        [JsonProperty("default_model", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DefaultModel { get; set; }
        /// <summary>
        /// Either an empty string, indicating the model is not a custom model, or the ID of the service instance that
        /// created the model.
        /// </summary>
        [JsonProperty("owner", NullValueHandling = NullValueHandling.Ignore)]
        public string Owner { get; set; }
    }
}
