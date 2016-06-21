/**
* Copyright 2015 IBM Corp. All Rights Reserved.
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

namespace IBM.Watson.DeveloperCloud.Services.PersonalityInsights.v2
{
    using UnityEngine;
    using System.Collections;

    public class DataModels : MonoBehaviour
    {
        [fsObject]
        public class Profile
        {
            /// <summary>
            /// Detailed results for a specific characteristic of the input text. 
            /// </summary>
            /// <value>The tree.</value>
            public TraitTreeNode tree { get; set; }

            /// <summary>
            /// The unique user identifier for which these characteristics were computed. The value is derived from the userid field of the input ContentItem objects. The field is passed as-is from JSON input. Sanitize the contents of the field before displaying them to prevent cross-site scripting attacks.
            /// </summary>
            /// <value>The identifier.</value>
            public string id { get; set; }

            /// <summary>
            /// The source identifier for which these characteristics were computed. The value is derived from the sourceid field of the input ContentItem objects. The field is passed as-is from JSON input. Sanitize the contents of the field before displaying them to prevent cross-site scripting attacks. 
            /// </summary>
            /// <value>The source.</value>
            public string source { get; set; }

            /// <summary>
            /// The language model that was used to process the input; for example, en. 
            /// </summary>
            /// <value>The processed lang.</value>
            public string processed_lang { get; set; }

            /// <summary>
            /// The number of words that were found in the input.
            /// </summary>
            /// <value>The word count.</value>
            public string word_count { get; set; }

            /// <summary>
            /// When guidance is appropriate, a string that provides a message that indicates the number of words found and where that value falls in the range of required or suggested number of words.
            /// </summary>
            /// <value>The word count message.</value>
            public string word_count_message { get; set; }

            /// <summary>
            /// Warning messages associated with the input text submitted with the request. The array is empty if the input generated no warnings.
            /// </summary>
            /// <value>The warnings.</value>
            public Warning[] warnings { get; set; }
        }

        [fsObject]
        public class TraitTreeNode
        {
            /// <summary>
            /// The unique identifier of the characteristic to which the results pertain.
            /// </summary>
            /// <value>The identifier.</value>
            public string id { get; set; }

            /// <summary>
            /// The user-visible name of the characteristic.
            /// </summary>
            /// <value>The name.</value>
            public string name { get; set; }

            /// <summary>
            /// The category of the characteristic: personality, needs, values, or behavior (for temporal data).
            /// </summary>
            /// <value>The category.</value>
            public string category { get; set; }

            /// <summary>
            /// For personality, needs, and values characteristics, the normalized percentile score for the characteristic. The range is 0 to 1. For example, if the percentage for Openness is 0.25, the author scored in the 25th percentile; the author is more open than 24% of the population and less open than 74% of the population. For temporal behavior characteristics, the percentage of timestamped data that occurred during that day or hour.
            /// </summary>
            /// <value>The percentage.</value>
            public string percentage { get; set; }

            /// <summary>
            /// For personality, needs, and values characteristics, indicates the sampling error of the percentage based on the number of words in the input text. The range is 0 to 1. The number defines a 95% confidence interval around the percentage. For example, if the sampling error is 4% and the percentage is 61%, it is 95% likely that the actual percentage value is between 57% and 65% if more words are given.
            /// </summary>
            /// <value>The sampling error.</value>
            public string sampling_error { get; set; }

            /// <summary>
            /// For personality, needs, and values characteristics, the raw score for the characteristic. A positive or negative score indicates more or less of the characteristic; zero indicates neutrality or no evidence for a score. The raw score is computed based on the input and the service model; it is not normalized or compared with a sample population. The raw score enables comparison of the results against a different sampling population and with a custom normalization approach.
            /// </summary>
            /// <value>The raw score.</value>
            public string raw_score { get; set; }

            /// <summary>
            /// For personality, needs, and values characteristics, indicates the sampling error of the raw score based on the number of words in the input. The practical range is 0 to 1. The number defines a 95% confidence interval around the raw score. For example, if the raw sampling error is 5% and the raw score is 65%, it is 95% likely that the actual raw score is between 60% and 70% if more words are given.
            /// </summary>
            /// <value>The raw sampling error.</value>
            public string raw_sampling_error { get; set; }

            /// <summary>
            /// Recursive array of more detailed characteristics inferred from the input text.
            /// </summary>
            /// <value>The children.</value>
            public TraitTreeNode[] children { get; set; }
        }

        [fsObject]
        public class Warning
        {
            /// <summary>
            /// The identifier of the warning message, one of WORD_COUNT_MESSAGE or JSON_AS_TEXT.
            /// </summary>
            /// <value>The identifier.</value>
            public string id { get; set; }

            /// <summary>
            /// The message associated with the id. For WORD_COUNT_MESSAGE, "There were <number> words in the input. We need a minimum of 3,500, preferably 6,000 or more, to compute statistically significant estimates"; for JSON_AS_TEXT, "Request input was processed as text/plain as indicated, however detected a JSON input. Did you mean application/json?".
            /// </summary>
            /// <value>The message.</value>
            public string message { get; set; }
        }
    }
}
