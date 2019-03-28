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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IBM.Watson.NaturalLanguageUnderstanding.V1.Model
{
    /// <summary>
    /// Detects anger, disgust, fear, joy, or sadness that is conveyed in the content or by the context around target
    /// phrases specified in the targets parameter. You can analyze emotion for detected entities with
    /// `entities.emotion` and for keywords with `keywords.emotion`.
    ///
    /// Supported languages: English.
    /// </summary>
    public class EmotionOptions
    {
        /// <summary>
        /// Set this to `false` to hide document-level emotion results.
        /// </summary>
        [JsonProperty("document", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Document { get; set; }
        /// <summary>
        /// Emotion results will be returned for each target string that is found in the document.
        /// </summary>
        [JsonProperty("targets", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Targets { get; set; }
    }
}
