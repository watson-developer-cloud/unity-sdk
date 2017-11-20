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

using UnityEngine;

namespace IBM.Watson.DeveloperCloud.Services.PersonalityInsights.v3
{
    /// <summary>
    /// The Profile object.
    /// </summary>
    [SerializeField]
    public class Profile
    {
        /// <summary>
        /// The language model that was used to process the input; for example, en.
        /// </summary>
        public string processed_language { get; set; }
        /// <summary>
        /// The number of words that were found in the input.
        /// </summary>
        public int word_count { get; set; }
        /// <summary>
        /// When guidance is appropriate, a string that provides a message that indicates 
        /// the number of words found and where that value falls in the range of required 
        /// or suggested number of words.
        /// </summary>
        public string word_count_message { get; set; }
        /// <summary>
        /// Detailed results for the Big Five personality characteristics (dimensions and 
        /// facets) inferred from the input text.
        /// </summary>
        public TraitTreeNode[] personality { get; set; }
        /// <summary>
        /// Detailed results for the Needs characteristics inferred from the input text. 
        /// </summary>
        public TraitTreeNode[] values { get; set; }
        /// <summary>
        /// Detailed results for the Values characteristics inferred from the input text. 
        /// </summary>
        public TraitTreeNode[] needs { get; set; }
        /// <summary>
        /// For JSON content that is timestamped, detailed results about the social behavior 
        /// disclosed by the input in terms of temporal characteristics. The results include 
        /// information about the distribution of the content over the days of the week and 
        /// the hours of the day.
        /// </summary>
        public BehaviorNode[] behavior { get; set; }
        /// <summary>
        /// If the consumption_preferences query parameter is true, detailed results for 
        /// each category of `consumption preferences`. Each element of the array provides
        /// information inferred from the input text for the individual preferences of that 
        /// category. 
        /// </summary>
        public ConsumptionPreferencesCategoryNode[] consumption_preferences { get; set; }
        /// <summary>
        /// Warning messages associated with the input text submitted with the request. The 
        /// array is empty if the input generated no warnings.
        /// </summary>
        public Warning[] warning { get; set; }
    }

    /// <summary>
    /// The Trait Tree Node object.
    /// </summary>
    [SerializeField]
    public class TraitTreeNode
    {
        /// <summary>
        /// The unique identifier of the characteristic to which the results pertain. IDs 
        /// have the form `big5_` for Big Five personality characteristics, `need_` for Needs, 
        /// or `value_` for Values.
        /// </summary>
        public string trait_id { get; set; }
        /// <summary>
        /// The user-visible name of the characteristic.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The category of the characteristic: personality for Big Five `personality` 
        /// characteristics, `needs` for Needs, or `values` for Values. 
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// The normalized percentile score for the characteristic. The range is 0 to 1. For 
        /// example, if the percentage for Openness is 0.60, the author scored in the 60th 
        /// percentile; the author is more open than 59 percent of the population and less open 
        /// than 39 percent of the population.
        /// </summary>
        public double percentile { get; set; }
        /// <summary>
        /// The raw score for the characteristic. A positive or negative score indicates more 
        /// or less of the characteristic; zero indicates neutrality or no evidence for a 
        /// score. The raw score is computed based on the input and the service model; it is 
        /// not normalized or compared with a sample population. The raw score enables 
        /// comparison of the results against a different sampling population and with a custom 
        /// normalization approach. 
        /// </summary>
        public double raw_score { get; set; }
        /// <summary>
        /// For `personality` (Big Five) dimensions, more detailed results for the facets of 
        /// each dimension as inferred from the input text.
        /// </summary>
        public TraitTreeNode[] children { get; set; }
    }

    /// <summary>
    /// The Behavior Node object.
    /// </summary>
    [SerializeField]
    public class BehaviorNode
    {
        /// <summary>
        /// The unique identifier of the characteristic to which the results pertain. IDs have 
        /// the form `behavior_`.
        /// </summary>
        public string trait_id { get; set; }
        /// <summary>
        /// The user-visible name of the characteristic.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The category of the characteristic: behavior for temporal data.
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// For JSON content that is timestamped, the percentage of timestamped input data
        /// that occurred during that day of the week or hour of the day. The range is 0 to 1.
        /// </summary>
        public double percentage { get; set; }
    }

    /// <summary>
    /// The Consumption Preferences Category Node object.
    /// </summary>
    [SerializeField]
    public class ConsumptionPreferencesCategoryNode
    {
        /// <summary>
        /// The unique identifier of the consumption preferences category to which the results
        /// pertain. IDs have the form `consumption_preferences_`. 
        /// </summary>
        public string consumption_preference_category_id { get; set; }
        /// <summary>
        /// The user-visible name of the consumption preferences category.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Detailed results inferred from the input text for the individual preferences of
        /// the category.
        /// </summary>
        public ConsumptionPreferencesNode[] consumption_preferences { get; set; }
    }

    /// <summary>
    /// The Warning Object.
    /// </summary>
    [SerializeField]
    public class Warning
    {
        /// <summary>
        /// The identifier of the warning message, one of `WORD_COUNT_MESSAGE`, `JSON_AS_TEXT`,
        /// or `PARTIAL_TEXT_USED`. 
        /// </summary>
        public string warning_id { get; set; }
        /// <summary>
        /// The message associated with the `warning_id`. For `WORD_COUNT_MESSAGE`, "There were 
        /// &gt;number&lt; words in the input. We need a minimum of 600, preferably 1,200 or more, to 
        /// compute statistically significant estimates."; for `JSON_AS_TEXT`, "Request input
        /// was processed as text/plain as indicated, however detected a JSON input. Did you 
        /// mean application/json?"; and for `PARTIAL_TEXT_USED`, "The text provided to compute the 
        /// profile was trimmed for performance reasons. This action does not affect the accuracy
        /// of the output, as not all of the input text was required." The `PARTIAL_TEXT_USED`
        /// warning applies only when Arabic input text exceeds a threshold at which additional 
        /// words do not contribute to the accuracy of the profile.
        /// </summary>
        public string message { get; set; }
    }

    /// <summary>
    /// The Consumption Preferences Node object.
    /// </summary>
    [SerializeField]
    public class ConsumptionPreferencesNode
    {
        /// <summary>
        /// The unique identifier of the consumption preference to which the results pertain.
        /// IDs have the form `consumption_preferences_`. 
        /// </summary>
        public string consumption_preference_id { get; set; }
        /// <summary>
        /// The user-visible name of the consumption preference. 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The score for the consumption preference: `0.0` indicates unlikely, `0.5` indicates
        /// neutrality, and `1.0` indicates likely. The scores for some preferences are binary and
        /// do not allow a neutral value. The score is an indication of preference based on the
        /// results inferred from the input text, not a normalized percentile.
        /// </summary>
        public double score { get; set; }
    }

    /// <summary>
    /// The content type. Either text, html or json.
    /// </summary>
    public class ContentType
    {
        /// <summary>
        /// Mime type for plain text.
        /// </summary>
        public const string TextPlain = "text/plain";

        /// <summary>
        /// Mime type for HTML.
        /// </summary>
        public const string TextHtml = "text/html";

        /// <summary>
        /// Mime type for json.
        /// </summary>
        public const string ApplicationJson = "application/json";
    }

    /// <summary>
    /// The content language. Either English, Arabic, Spanish or Japanese.
    /// </summary>
    public class ContentLanguage
    {
        /// <summary>
        /// English.
        /// </summary>
        public const string English = "en";
        /// <summary>
        /// Arabic.
        /// </summary>
        public const string Arabic = "ar";
        /// <summary>
        /// Spanish.
        /// </summary>
        public const string Spanish = "es";
        /// <summary>
        /// Japanese
        /// </summary>
        public const string Japanese = "ja";
    }

    /// <summary>
    /// The accepted languages.
    /// </summary>
    public class AcceptLanguage
    {
        /// <summary>
        /// English.
        /// </summary>
        public const string English = "en";
        /// <summary>
        /// Arabic.
        /// </summary>
        public const string Arabic = "ar";
        /// <summary>
        /// Spanish.
        /// </summary>
        public const string Spanish = "es";
        /// <summary>
        /// Japanese.
        /// </summary>
        public const string Japanese = "ja";
        /// <summary>
        /// German.
        /// </summary>
        public const string German = "de";
        /// <summary>
        /// French.
        /// </summary>
        public const string French = "fr";
        /// <summary>
        /// Italian.
        /// </summary>
        public const string Italian = "it";
        /// <summary>
        /// Korean.
        /// </summary>
        public const string Korean = "ko";
        /// <summary>
        /// Brazilian Portuguese.
        /// </summary>
        public const string BrazilianPortuguese = "pt-br";
        /// <summary>
        /// Simplified Chinese.
        /// </summary>
        public const string SimplifiedChinese = "zh-cn";
        /// <summary>
        /// Traditional Chinese.
        /// </summary>
        public const string TraditionalChinese = "zh-tw";
    }

    /// <summary>
    /// The Personality Insights version.
    /// </summary>
    public class PersonalityInsightsVersion
    {
        /// <summary>
        /// The version.
        /// </summary>
        public const string Version = "2016-10-20";
    }
}
