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

namespace IBM.Watson.SpeechToText.V1.Model
{
    /// <summary>
    /// CreateAcousticModel.
    /// </summary>
    public class CreateAcousticModel
    {
        /// <summary>
        /// The name of the base language model that is to be customized by the new custom acoustic model. The new
        /// custom model can be used only with the base model that it customizes.
        ///
        /// To determine whether a base model supports acoustic model customization, refer to [Language support for
        /// customization](https://cloud.ibm.com/docs/services/speech-to-text/custom.html#languageSupport).
        /// </summary>
        public class BaseModelNameValue
        {
            /// <summary>
            /// Constant AR_AR_BROADBANDMODEL for ar-AR_BroadbandModel
            /// </summary>
            public const string AR_AR_BROADBANDMODEL = "ar-AR_BroadbandModel";
            /// <summary>
            /// Constant DE_DE_BROADBANDMODEL for de-DE_BroadbandModel
            /// </summary>
            public const string DE_DE_BROADBANDMODEL = "de-DE_BroadbandModel";
            /// <summary>
            /// Constant DE_DE_NARROWBANDMODEL for de-DE_NarrowbandModel
            /// </summary>
            public const string DE_DE_NARROWBANDMODEL = "de-DE_NarrowbandModel";
            /// <summary>
            /// Constant EN_GB_BROADBANDMODEL for en-GB_BroadbandModel
            /// </summary>
            public const string EN_GB_BROADBANDMODEL = "en-GB_BroadbandModel";
            /// <summary>
            /// Constant EN_GB_NARROWBANDMODEL for en-GB_NarrowbandModel
            /// </summary>
            public const string EN_GB_NARROWBANDMODEL = "en-GB_NarrowbandModel";
            /// <summary>
            /// Constant EN_US_BROADBANDMODEL for en-US_BroadbandModel
            /// </summary>
            public const string EN_US_BROADBANDMODEL = "en-US_BroadbandModel";
            /// <summary>
            /// Constant EN_US_NARROWBANDMODEL for en-US_NarrowbandModel
            /// </summary>
            public const string EN_US_NARROWBANDMODEL = "en-US_NarrowbandModel";
            /// <summary>
            /// Constant EN_US_SHORTFORM_NARROWBANDMODEL for en-US_ShortForm_NarrowbandModel
            /// </summary>
            public const string EN_US_SHORTFORM_NARROWBANDMODEL = "en-US_ShortForm_NarrowbandModel";
            /// <summary>
            /// Constant ES_ES_BROADBANDMODEL for es-ES_BroadbandModel
            /// </summary>
            public const string ES_ES_BROADBANDMODEL = "es-ES_BroadbandModel";
            /// <summary>
            /// Constant ES_ES_NARROWBANDMODEL for es-ES_NarrowbandModel
            /// </summary>
            public const string ES_ES_NARROWBANDMODEL = "es-ES_NarrowbandModel";
            /// <summary>
            /// Constant FR_FR_BROADBANDMODEL for fr-FR_BroadbandModel
            /// </summary>
            public const string FR_FR_BROADBANDMODEL = "fr-FR_BroadbandModel";
            /// <summary>
            /// Constant FR_FR_NARROWBANDMODEL for fr-FR_NarrowbandModel
            /// </summary>
            public const string FR_FR_NARROWBANDMODEL = "fr-FR_NarrowbandModel";
            /// <summary>
            /// Constant JA_JP_BROADBANDMODEL for ja-JP_BroadbandModel
            /// </summary>
            public const string JA_JP_BROADBANDMODEL = "ja-JP_BroadbandModel";
            /// <summary>
            /// Constant JA_JP_NARROWBANDMODEL for ja-JP_NarrowbandModel
            /// </summary>
            public const string JA_JP_NARROWBANDMODEL = "ja-JP_NarrowbandModel";
            /// <summary>
            /// Constant KO_KR_BROADBANDMODEL for ko-KR_BroadbandModel
            /// </summary>
            public const string KO_KR_BROADBANDMODEL = "ko-KR_BroadbandModel";
            /// <summary>
            /// Constant KO_KR_NARROWBANDMODEL for ko-KR_NarrowbandModel
            /// </summary>
            public const string KO_KR_NARROWBANDMODEL = "ko-KR_NarrowbandModel";
            /// <summary>
            /// Constant PT_BR_BROADBANDMODEL for pt-BR_BroadbandModel
            /// </summary>
            public const string PT_BR_BROADBANDMODEL = "pt-BR_BroadbandModel";
            /// <summary>
            /// Constant PT_BR_NARROWBANDMODEL for pt-BR_NarrowbandModel
            /// </summary>
            public const string PT_BR_NARROWBANDMODEL = "pt-BR_NarrowbandModel";
            /// <summary>
            /// Constant ZH_CN_BROADBANDMODEL for zh-CN_BroadbandModel
            /// </summary>
            public const string ZH_CN_BROADBANDMODEL = "zh-CN_BroadbandModel";
            /// <summary>
            /// Constant ZH_CN_NARROWBANDMODEL for zh-CN_NarrowbandModel
            /// </summary>
            public const string ZH_CN_NARROWBANDMODEL = "zh-CN_NarrowbandModel";
            
        }

        /// <summary>
        /// The name of the base language model that is to be customized by the new custom acoustic model. The new
        /// custom model can be used only with the base model that it customizes.
        ///
        /// To determine whether a base model supports acoustic model customization, refer to [Language support for
        /// customization](https://cloud.ibm.com/docs/services/speech-to-text/custom.html#languageSupport).
        /// Constants for possible values can be found using CreateAcousticModel.BaseModelNameValue
        /// </summary>
        [JsonProperty("base_model_name", NullValueHandling = NullValueHandling.Ignore)]
        public string BaseModelName { get; set; }
        /// <summary>
        /// A user-defined name for the new custom acoustic model. Use a name that is unique among all custom acoustic
        /// models that you own. Use a localized name that matches the language of the custom model. Use a name that
        /// describes the acoustic environment of the custom model, such as `Mobile custom model` or `Noisy car custom
        /// model`.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// A description of the new custom acoustic model. Use a localized description that matches the language of the
        /// custom model.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
    }
}
