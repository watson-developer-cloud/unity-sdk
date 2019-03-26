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
    /// The object containing the actions and the objects the actions act upon.
    /// </summary>
    public class SemanticRolesResult
    {
        /// <summary>
        /// Sentence from the source that contains the subject, action, and object.
        /// </summary>
        [JsonProperty("sentence", NullValueHandling = NullValueHandling.Ignore)]
        public string Sentence { get; set; }
        /// <summary>
        /// The extracted subject from the sentence.
        /// </summary>
        [JsonProperty("subject", NullValueHandling = NullValueHandling.Ignore)]
        public SemanticRolesResultSubject Subject { get; set; }
        /// <summary>
        /// The extracted action from the sentence.
        /// </summary>
        [JsonProperty("action", NullValueHandling = NullValueHandling.Ignore)]
        public SemanticRolesResultAction Action { get; set; }
        /// <summary>
        /// The extracted object from the sentence.
        /// </summary>
        [JsonProperty("object", NullValueHandling = NullValueHandling.Ignore)]
        public SemanticRolesResultObject _Object { get; set; }
    }
}
