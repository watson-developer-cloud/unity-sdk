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

namespace IBM.Watson.CompareComply.V1.Model
{
    /// <summary>
    /// The details of the normalized text, if applicable. This element is optional; it is returned only if normalized
    /// text exists.
    /// </summary>
    public class Interpretation
    {
        /// <summary>
        /// The value that was located in the normalized text.
        /// </summary>
        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }
        /// <summary>
        /// An integer or float expressing the numeric value of the `value` key.
        /// </summary>
        [JsonProperty("numeric_value", NullValueHandling = NullValueHandling.Ignore)]
        public float? NumericValue { get; set; }
        /// <summary>
        /// A string listing the unit of the value that was found in the normalized text.
        ///
        /// **Note:** The value of `unit` is the [ISO-4217 currency
        /// code](https://www.iso.org/iso-4217-currency-codes.html) identified for the currency amount (for example,
        /// `USD` or `EUR`). If the service cannot disambiguate a currency symbol (for example, `$` or `£`), the value
        /// of `unit` contains the ambiguous symbol as-is.
        /// </summary>
        [JsonProperty("unit", NullValueHandling = NullValueHandling.Ignore)]
        public string Unit { get; set; }
    }
}
