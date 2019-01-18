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

namespace IBM.WatsonDeveloperCloud.SpeechToText.v1.Model
{
    /// <summary>
    /// CreateLanguageModel.
    /// </summary>
    public class CreateLanguageModel
    {
        /// <summary>
        /// The name of the base language model that is to be customized by the new custom language model. The new
        /// custom model can be used only with the base model that it customizes.
        ///
        /// To determine whether a base model supports language model customization, use the **Get a model** method and
        /// check that the attribute `custom_language_model` is set to `true`. You can also refer to [Language support
        /// for customization](https://cloud.ibm.com/docs/services/speech-to-text/custom.html#languageSupport).
        /// </summary>
        /// <value>
        /// The name of the base language model that is to be customized by the new custom language model. The new
        /// custom model can be used only with the base model that it customizes.
        ///
        /// To determine whether a base model supports language model customization, use the **Get a model** method and
        /// check that the attribute `custom_language_model` is set to `true`. You can also refer to [Language support
        /// for customization](https://cloud.ibm.com/docs/services/speech-to-text/custom.html#languageSupport).
        /// </value>


        /// <summary>
        /// Constant DE_DE_BROADBANDMODEL for de-DE_BroadbandModel
        /// </summary>
        private const string DE_DE_BROADBANDMODEL = "de-DE_BroadbandModel";

        /// <summary>
        /// Constant DE_DE_NARROWBANDMODEL for de-DE_NarrowbandModel
        /// </summary>
        private const string DE_DE_NARROWBANDMODEL = "de-DE_NarrowbandModel";

        /// <summary>
        /// Constant EN_GB_BROADBANDMODEL for en-GB_BroadbandModel
        /// </summary>
        private const string EN_GB_BROADBANDMODEL = "en-GB_BroadbandModel";

        /// <summary>
        /// Constant EN_GB_NARROWBANDMODEL for en-GB_NarrowbandModel
        /// </summary>
        private const string EN_GB_NARROWBANDMODEL = "en-GB_NarrowbandModel";

        /// <summary>
        /// Constant EN_US_BROADBANDMODEL for en-US_BroadbandModel
        /// </summary>
        private const string EN_US_BROADBANDMODEL = "en-US_BroadbandModel";

        /// <summary>
        /// Constant EN_US_NARROWBANDMODEL for en-US_NarrowbandModel
        /// </summary>
        private const string EN_US_NARROWBANDMODEL = "en-US_NarrowbandModel";

        /// <summary>
        /// Constant EN_US_SHORTFORM_NARROWBANDMODEL for en-US_ShortForm_NarrowbandModel
        /// </summary>
        private const string EN_US_SHORTFORM_NARROWBANDMODEL = "en-US_ShortForm_NarrowbandModel";

        /// <summary>
        /// Constant ES_ES_BROADBANDMODEL for es-ES_BroadbandModel
        /// </summary>
        private const string ES_ES_BROADBANDMODEL = "es-ES_BroadbandModel";

        /// <summary>
        /// Constant ES_ES_NARROWBANDMODEL for es-ES_NarrowbandModel
        /// </summary>
        private const string ES_ES_NARROWBANDMODEL = "es-ES_NarrowbandModel";

        /// <summary>
        /// Constant FR_FR_BROADBANDMODEL for fr-FR_BroadbandModel
        /// </summary>
        private const string FR_FR_BROADBANDMODEL = "fr-FR_BroadbandModel";

        /// <summary>
        /// Constant FR_FR_NARROWBANDMODEL for fr-FR_NarrowbandModel
        /// </summary>
        private const string FR_FR_NARROWBANDMODEL = "fr-FR_NarrowbandModel";

        /// <summary>
        /// Constant JA_JP_BROADBANDMODEL for ja-JP_BroadbandModel
        /// </summary>
        private const string JA_JP_BROADBANDMODEL = "ja-JP_BroadbandModel";

        /// <summary>
        /// Constant JA_JP_NARROWBANDMODEL for ja-JP_NarrowbandModel
        /// </summary>
        private const string JA_JP_NARROWBANDMODEL = "ja-JP_NarrowbandModel";

        /// <summary>
        /// Constant KO_KR_BROADBANDMODEL for ko-KR_BroadbandModel
        /// </summary>
        private const string KO_KR_BROADBANDMODEL = "ko-KR_BroadbandModel";

        /// <summary>
        /// Constant KO_KR_NARROWBANDMODEL for ko-KR_NarrowbandModel
        /// </summary>
        private const string KO_KR_NARROWBANDMODEL = "ko-KR_NarrowbandModel";

        /// <summary>
        /// Constant PT_BR_BROADBANDMODEL for pt-BR_BroadbandModel
        /// </summary>
        private const string PT_BR_BROADBANDMODEL = "pt-BR_BroadbandModel";

        /// <summary>
        /// Constant PT_BR_NARROWBANDMODEL for pt-BR_NarrowbandModel
        /// </summary>
        private const string PT_BR_NARROWBANDMODEL = "pt-BR_NarrowbandModel";

        /// <summary>
        /// The name of the base language model that is to be customized by the new custom language model. The new
        /// custom model can be used only with the base model that it customizes.
        ///
        /// To determine whether a base model supports language model customization, use the **Get a model** method and
        /// check that the attribute `custom_language_model` is set to `true`. You can also refer to [Language support
        /// for customization](https://cloud.ibm.com/docs/services/speech-to-text/custom.html#languageSupport).
        /// </summary>
        [fsProperty("base_model_name")]
        public string BaseModelName { get; set; }
        /// <summary>
        /// A user-defined name for the new custom language model. Use a name that is unique among all custom language
        /// models that you own. Use a localized name that matches the language of the custom model. Use a name that
        /// describes the domain of the custom model, such as `Medical custom model` or `Legal custom model`.
        /// </summary>
        [fsProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// The dialect of the specified language that is to be used with the custom language model. The parameter is
        /// meaningful only for Spanish models, for which the service creates a custom language model that is suited for
        /// speech in one of the following dialects:
        /// * `es-ES` for Castilian Spanish (the default)
        /// * `es-LA` for Latin American Spanish
        /// * `es-US` for North American (Mexican) Spanish
        ///
        /// A specified dialect must be valid for the base model. By default, the dialect matches the language of the
        /// base model; for example, `en-US` for either of the US English language models.
        /// </summary>
        [fsProperty("dialect")]
        public string Dialect { get; set; }
        /// <summary>
        /// A description of the new custom language model. Use a localized description that matches the language of the
        /// custom model.
        /// </summary>
        [fsProperty("description")]
        public string Description { get; set; }
    }

}
