/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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

namespace IBM.Watson.Discovery.v1.Model
{
    /// <summary>
    /// CreateCollectionRequest.
    /// </summary>
    public class CreateCollectionRequest
    {
        /// <summary>
        /// The language of the documents stored in the collection, in the form of an ISO 639-1 language code.
        /// </summary>
        public class LanguageEnumValue
        {
            /// <summary>
            /// Constant EN for en
            /// </summary>
            public const string EN = "en";
            /// <summary>
            /// Constant ES for es
            /// </summary>
            public const string ES = "es";
            /// <summary>
            /// Constant DE for de
            /// </summary>
            public const string DE = "de";
            /// <summary>
            /// Constant AR for ar
            /// </summary>
            public const string AR = "ar";
            /// <summary>
            /// Constant FR for fr
            /// </summary>
            public const string FR = "fr";
            /// <summary>
            /// Constant IT for it
            /// </summary>
            public const string IT = "it";
            /// <summary>
            /// Constant JA for ja
            /// </summary>
            public const string JA = "ja";
            /// <summary>
            /// Constant KO for ko
            /// </summary>
            public const string KO = "ko";
            /// <summary>
            /// Constant PT for pt
            /// </summary>
            public const string PT = "pt";
            /// <summary>
            /// Constant NL for nl
            /// </summary>
            public const string NL = "nl";
            
        }

        /// <summary>
        /// The language of the documents stored in the collection, in the form of an ISO 639-1 language code.
        /// </summary>
        [fsProperty("language")]
        public string Language { get; set; }
        /// <summary>
        /// The name of the collection to be created.
        /// </summary>
        [fsProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// A description of the collection.
        /// </summary>
        [fsProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// The ID of the configuration in which the collection is to be created.
        /// </summary>
        [fsProperty("configuration_id")]
        public string ConfigurationId { get; set; }
    }


}
