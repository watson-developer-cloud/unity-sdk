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

namespace IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v3
{
    /// <summary>
    /// UtteranceAnalyses.
    /// </summary>
    public class UtteranceAnalyses
    {
        /// <summary>
        /// An array of `UtteranceAnalysis` objects that provides the results for each utterance of the input.
        /// </summary>
        /// <value>
        /// An array of `UtteranceAnalysis` objects that provides the results for each utterance of the input.
        /// </value>
        [fsProperty("utterances_tone")]
        public List<UtteranceAnalysis> UtterancesTone { get; set; }
        /// <summary>
        /// **`2017-09-21`:** A warning message if the content contains more than 50 utterances. The service analyzes
        /// only the first 50 utterances. **`2016-05-19`:** Not returned.
        /// </summary>
        /// <value>
        /// **`2017-09-21`:** A warning message if the content contains more than 50 utterances. The service analyzes
        /// only the first 50 utterances. **`2016-05-19`:** Not returned.
        /// </value>
        [fsProperty("warning")]
        public string Warning { get; set; }
    }

}
