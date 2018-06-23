

using FullSerializer;
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
namespace IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v3
{
    /// <summary>
    /// Utterance.
    /// </summary>
    public class Utterance
    {
        /// <summary>
        /// An utterance contributed by a user in the conversation that is to be analyzed. The utterance can contain
        /// multiple sentences.
        /// </summary>
        /// <value>
        /// An utterance contributed by a user in the conversation that is to be analyzed. The utterance can contain
        /// multiple sentences.
        /// </value>
        [fsProperty("text")]
        public string Text { get; set; }
        /// <summary>
        /// A string that identifies the user who contributed the utterance specified by the `text` parameter.
        /// </summary>
        /// <value>
        /// A string that identifies the user who contributed the utterance specified by the `text` parameter.
        /// </value>
        [fsProperty("user")]
        public string User { get; set; }
    }

}
