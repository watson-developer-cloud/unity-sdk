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

namespace IBM.Watson.PersonalityInsights.V3.Model
{
    /// <summary>
    /// An input content item that the service is to analyze.
    /// </summary>
    public class ContentItem
    {
        /// <summary>
        /// The MIME type of the content. The default is plain text. The tags are stripped from HTML content before it
        /// is analyzed; plain text is processed as submitted.
        /// </summary>
        public class ContenttypeValue
        {
            /// <summary>
            /// Constant TEXT_PLAIN for text/plain
            /// </summary>
            public const string TEXT_PLAIN = "text/plain";
            /// <summary>
            /// Constant TEXT_HTML for text/html
            /// </summary>
            public const string TEXT_HTML = "text/html";
            
        }

        /// <summary>
        /// The language identifier (two-letter ISO 639-1 identifier) for the language of the content item. The default
        /// is `en` (English). Regional variants are treated as their parent language; for example, `en-US` is
        /// interpreted as `en`. A language specified with the **Content-Type** parameter overrides the value of this
        /// parameter; any content items that specify a different language are ignored. Omit the **Content-Type**
        /// parameter to base the language on the most prevalent specification among the content items; again, content
        /// items that specify a different language are ignored. You can specify any combination of languages for the
        /// input and response content.
        /// </summary>
        public class LanguageValue
        {
            /// <summary>
            /// Constant AR for ar
            /// </summary>
            public const string AR = "ar";
            /// <summary>
            /// Constant EN for en
            /// </summary>
            public const string EN = "en";
            /// <summary>
            /// Constant ES for es
            /// </summary>
            public const string ES = "es";
            /// <summary>
            /// Constant JA for ja
            /// </summary>
            public const string JA = "ja";
            /// <summary>
            /// Constant KO for ko
            /// </summary>
            public const string KO = "ko";
            
        }

        /// <summary>
        /// The MIME type of the content. The default is plain text. The tags are stripped from HTML content before it
        /// is analyzed; plain text is processed as submitted.
        /// Constants for possible values can be found using ContentItem.ContenttypeValue
        /// </summary>
        [JsonProperty("contenttype", NullValueHandling = NullValueHandling.Ignore)]
        public string Contenttype { get; set; }
        /// <summary>
        /// The language identifier (two-letter ISO 639-1 identifier) for the language of the content item. The default
        /// is `en` (English). Regional variants are treated as their parent language; for example, `en-US` is
        /// interpreted as `en`. A language specified with the **Content-Type** parameter overrides the value of this
        /// parameter; any content items that specify a different language are ignored. Omit the **Content-Type**
        /// parameter to base the language on the most prevalent specification among the content items; again, content
        /// items that specify a different language are ignored. You can specify any combination of languages for the
        /// input and response content.
        /// Constants for possible values can be found using ContentItem.LanguageValue
        /// </summary>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }
        /// <summary>
        /// The content that is to be analyzed. The service supports up to 20 MB of content for all `ContentItem`
        /// objects combined.
        /// </summary>
        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; set; }
        /// <summary>
        /// A unique identifier for this content item.
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
        /// <summary>
        /// A timestamp that identifies when this content was created. Specify a value in milliseconds since the UNIX
        /// Epoch (January 1, 1970, at 0:00 UTC). Required only for results that include temporal behavior data.
        /// </summary>
        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public long? Created { get; set; }
        /// <summary>
        /// A timestamp that identifies when this content was last updated. Specify a value in milliseconds since the
        /// UNIX Epoch (January 1, 1970, at 0:00 UTC). Required only for results that include temporal behavior data.
        /// </summary>
        [JsonProperty("updated", NullValueHandling = NullValueHandling.Ignore)]
        public long? Updated { get; set; }
        /// <summary>
        /// The unique ID of the parent content item for this item. Used to identify hierarchical relationships between
        /// posts/replies, messages/replies, and so on.
        /// </summary>
        [JsonProperty("parentid", NullValueHandling = NullValueHandling.Ignore)]
        public string Parentid { get; set; }
        /// <summary>
        /// Indicates whether this content item is a reply to another content item.
        /// </summary>
        [JsonProperty("reply", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Reply { get; set; }
        /// <summary>
        /// Indicates whether this content item is a forwarded/copied version of another content item.
        /// </summary>
        [JsonProperty("forward", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Forward { get; set; }
    }
}
