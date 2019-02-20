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

namespace IBM.Watson.TextToSpeech.V1.Model
{
    /// <summary>
    /// CreateVoiceModel.
    /// </summary>
    public class CreateVoiceModel
    {
        /// <summary>
        /// The language of the new custom voice model. Omit the parameter to use the the default language, `en-US`.
        /// </summary>
        public class LanguageValue
        {
            /// <summary>
            /// Constant DE_DE for de-DE
            /// </summary>
            public const string DE_DE = "de-DE";
            /// <summary>
            /// Constant EN_US for en-US
            /// </summary>
            public const string EN_US = "en-US";
            /// <summary>
            /// Constant EN_GB for en-GB
            /// </summary>
            public const string EN_GB = "en-GB";
            /// <summary>
            /// Constant ES_ES for es-ES
            /// </summary>
            public const string ES_ES = "es-ES";
            /// <summary>
            /// Constant ES_LA for es-LA
            /// </summary>
            public const string ES_LA = "es-LA";
            /// <summary>
            /// Constant ES_US for es-US
            /// </summary>
            public const string ES_US = "es-US";
            /// <summary>
            /// Constant FR_FR for fr-FR
            /// </summary>
            public const string FR_FR = "fr-FR";
            /// <summary>
            /// Constant IT_IT for it-IT
            /// </summary>
            public const string IT_IT = "it-IT";
            /// <summary>
            /// Constant JA_JP for ja-JP
            /// </summary>
            public const string JA_JP = "ja-JP";
            /// <summary>
            /// Constant PT_BR for pt-BR
            /// </summary>
            public const string PT_BR = "pt-BR";
            
        }

        /// <summary>
        /// The language of the new custom voice model. Omit the parameter to use the the default language, `en-US`.
        /// Constants for possible values can be found using CreateVoiceModel.LanguageValue
        /// </summary>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }
        /// <summary>
        /// The name of the new custom voice model.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// A description of the new custom voice model. Specifying a description is recommended.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
    }
}
