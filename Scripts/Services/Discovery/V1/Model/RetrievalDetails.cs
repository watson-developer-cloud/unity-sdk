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

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// An object contain retrieval type information.
    /// </summary>
    public class RetrievalDetails
    {
        /// <summary>
        /// Indentifies the document retrieval strategy used for this query. `relevancy_training` indicates that the
        /// results were returned using a relevancy trained model. `continuous_relevancy_training` indicates that the
        /// results were returned using the continuous relevancy training model created by result feedback analysis.
        /// `untrained` means the results were returned using the standard untrained model.
        ///
        ///  **Note**: In the event of trained collections being queried, but the trained model is not used to return
        /// results, the **document_retrieval_strategy** will be listed as `untrained`.
        /// </summary>
        public class DocumentRetrievalStrategyValue
        {
            /// <summary>
            /// Constant UNTRAINED for untrained
            /// </summary>
            public const string UNTRAINED = "untrained";
            /// <summary>
            /// Constant RELEVANCY_TRAINING for relevancy_training
            /// </summary>
            public const string RELEVANCY_TRAINING = "relevancy_training";
            /// <summary>
            /// Constant CONTINUOUS_RELEVANCY_TRAINING for continuous_relevancy_training
            /// </summary>
            public const string CONTINUOUS_RELEVANCY_TRAINING = "continuous_relevancy_training";
            
        }

        /// <summary>
        /// Indentifies the document retrieval strategy used for this query. `relevancy_training` indicates that the
        /// results were returned using a relevancy trained model. `continuous_relevancy_training` indicates that the
        /// results were returned using the continuous relevancy training model created by result feedback analysis.
        /// `untrained` means the results were returned using the standard untrained model.
        ///
        ///  **Note**: In the event of trained collections being queried, but the trained model is not used to return
        /// results, the **document_retrieval_strategy** will be listed as `untrained`.
        /// Constants for possible values can be found using RetrievalDetails.DocumentRetrievalStrategyValue
        /// </summary>
        [JsonProperty("document_retrieval_strategy", NullValueHandling = NullValueHandling.Ignore)]
        public string DocumentRetrievalStrategy { get; set; }
    }
}
