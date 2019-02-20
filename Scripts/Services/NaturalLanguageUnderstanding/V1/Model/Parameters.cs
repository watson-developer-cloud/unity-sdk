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

namespace IBM.Watson.NaturalLanguageUnderstanding.V1.Model
{
    /// <summary>
    /// An object containing request parameters.
    /// </summary>
    public class Parameters
    {
        /// <summary>
        /// The plain text to analyze. One of the `text`, `html`, or `url` parameters is required.
        /// </summary>
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
        /// <summary>
        /// The HTML file to analyze. One of the `text`, `html`, or `url` parameters is required.
        /// </summary>
        [JsonProperty("html", NullValueHandling = NullValueHandling.Ignore)]
        public string Html { get; set; }
        /// <summary>
        /// The webpage to analyze. One of the `text`, `html`, or `url` parameters is required.
        /// </summary>
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }
        /// <summary>
        /// Specific features to analyze the document for.
        /// </summary>
        [JsonProperty("features", NullValueHandling = NullValueHandling.Ignore)]
        public Features Features { get; set; }
        /// <summary>
        /// Set this to `false` to disable webpage cleaning. To learn more about webpage cleaning, see the [Analyzing
        /// webpages](https://cloud.ibm.com/docs/services/natural-language-understanding/analyzing-webpages.html)
        /// documentation.
        /// </summary>
        [JsonProperty("clean", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Clean { get; set; }
        /// <summary>
        /// An [XPath
        /// query](https://cloud.ibm.com/docs/services/natural-language-understanding/analyzing-webpages.html#xpath) to
        /// perform on `html` or `url` input. Results of the query will be appended to the cleaned webpage text before
        /// it is analyzed. To analyze only the results of the XPath query, set the `clean` parameter to `false`.
        /// </summary>
        [JsonProperty("xpath", NullValueHandling = NullValueHandling.Ignore)]
        public string Xpath { get; set; }
        /// <summary>
        /// Whether to use raw HTML content if text cleaning fails.
        /// </summary>
        [JsonProperty("fallback_to_raw", NullValueHandling = NullValueHandling.Ignore)]
        public bool? FallbackToRaw { get; set; }
        /// <summary>
        /// Whether or not to return the analyzed text.
        /// </summary>
        [JsonProperty("return_analyzed_text", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ReturnAnalyzedText { get; set; }
        /// <summary>
        /// ISO 639-1 code that specifies the language of your text. This overrides automatic language detection.
        /// Language support differs depending on the features you include in your analysis. See [Language
        /// support](https://www.bluemix.net/docs/services/natural-language-understanding/language-support.html) for
        /// more information.
        /// </summary>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }
        /// <summary>
        /// Sets the maximum number of characters that are processed by the service.
        /// </summary>
        [JsonProperty("limit_text_characters", NullValueHandling = NullValueHandling.Ignore)]
        public long? LimitTextCharacters { get; set; }
    }
}
