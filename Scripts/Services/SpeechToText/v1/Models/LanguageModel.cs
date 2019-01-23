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
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.Services.SpeechToText.v1
{
    /// <summary>
    /// LanguageModel.
    /// </summary>
    public class LanguageModel
    {
        /// <summary>
        /// The current status of the custom language model:
        /// * `pending`: The model was created but is waiting either for training data to be added or for the service to
        /// finish analyzing added data.
        /// * `ready`: The model contains data and is ready to be trained.
        /// * `training`: The model is currently being trained.
        /// * `available`: The model is trained and ready to use.
        /// * `upgrading`: The model is currently being upgraded.
        /// * `failed`: Training of the model failed.
        /// </summary>
        /// <value>
        /// The current status of the custom language model:
        /// * `pending`: The model was created but is waiting either for training data to be added or for the service to
        /// finish analyzing added data.
        /// * `ready`: The model contains data and is ready to be trained.
        /// * `training`: The model is currently being trained.
        /// * `available`: The model is trained and ready to use.
        /// * `upgrading`: The model is currently being upgraded.
        /// * `failed`: Training of the model failed.
        /// </value>
        public enum StatusEnum
        {
            
            /// <summary>
            /// Enum PENDING for pending
            /// </summary>
            pending,
            
            /// <summary>
            /// Enum READY for ready
            /// </summary>
            ready,
            
            /// <summary>
            /// Enum TRAINING for training
            /// </summary>
            training,
            
            /// <summary>
            /// Enum AVAILABLE for available
            /// </summary>
            available,
            
            /// <summary>
            /// Enum UPGRADING for upgrading
            /// </summary>
            upgrading,
            
            /// <summary>
            /// Enum FAILED for failed
            /// </summary>
            failed
        }

        /// <summary>
        /// The current status of the custom language model:
        /// * `pending`: The model was created but is waiting either for training data to be added or for the service to
        /// finish analyzing added data.
        /// * `ready`: The model contains data and is ready to be trained.
        /// * `training`: The model is currently being trained.
        /// * `available`: The model is trained and ready to use.
        /// * `upgrading`: The model is currently being upgraded.
        /// * `failed`: Training of the model failed.
        /// </summary>
        [fsProperty("status")]
        public StatusEnum? Status { get; set; }
        /// <summary>
        /// The customization ID (GUID) of the custom language model. The **Create a custom language model** method
        /// returns only this field of the object; it does not return the other fields.
        /// </summary>
        [fsProperty("customization_id")]
        public string CustomizationId { get; set; }
        /// <summary>
        /// The date and time in Coordinated Universal Time (UTC) at which the custom language model was created. The
        /// value is provided in full ISO 8601 format (`YYYY-MM-DDThh:mm:ss.sTZD`).
        /// </summary>
        [fsProperty("created")]
        public string Created { get; set; }
        /// <summary>
        /// The language identifier of the custom language model (for example, `en-US`).
        /// </summary>
        [fsProperty("language")]
        public string Language { get; set; }
        /// <summary>
        /// The dialect of the language for the custom language model. By default, the dialect matches the language of
        /// the base model; for example, `en-US` for either of the US English language models. For Spanish models, the
        /// field indicates the dialect for which the model was created:
        /// * `es-ES` for Castilian Spanish (the default)
        /// * `es-LA` for Latin American Spanish
        /// * `es-US` for North American (Mexican) Spanish.
        /// </summary>
        [fsProperty("dialect")]
        public string Dialect { get; set; }
        /// <summary>
        /// A list of the available versions of the custom language model. Each element of the array indicates a version
        /// of the base model with which the custom model can be used. Multiple versions exist only if the custom model
        /// has been upgraded; otherwise, only a single version is shown.
        /// </summary>
        [fsProperty("versions")]
        public List<string> Versions { get; set; }
        /// <summary>
        /// The GUID of the credentials for the instance of the service that owns the custom language model.
        /// </summary>
        [fsProperty("owner")]
        public string Owner { get; set; }
        /// <summary>
        /// The name of the custom language model.
        /// </summary>
        [fsProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// The description of the custom language model.
        /// </summary>
        [fsProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// The name of the language model for which the custom language model was created.
        /// </summary>
        [fsProperty("base_model_name")]
        public string BaseModelName { get; set; }
        /// <summary>
        /// A percentage that indicates the progress of the custom language model's current training. A value of `100`
        /// means that the model is fully trained. **Note:** The `progress` field does not currently reflect the
        /// progress of the training. The field changes from `0` to `100` when training is complete.
        /// </summary>
        [fsProperty("progress")]
        public long? Progress { get; set; }
        /// <summary>
        /// If an error occurred while adding a grammar file to the custom language model, a message that describes an
        /// `Internal Server Error` and includes the string `Cannot compile grammar`. The status of the custom model is
        /// not affected by the error, but the grammar cannot be used with the model.
        /// </summary>
        [fsProperty("error")]
        public string Error { get; set; }
        /// <summary>
        /// If the request included unknown parameters, the following message: `Unexpected query parameter(s)
        /// ['parameters'] detected`, where `parameters` is a list that includes a quoted string for each unknown
        /// parameter.
        /// </summary>
        [fsProperty("warnings")]
        public string Warnings { get; set; }
    }

}
