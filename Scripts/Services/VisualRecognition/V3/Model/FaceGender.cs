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

namespace IBM.Watson.VisualRecognition.V3.Model
{
    /// <summary>
    /// Information about the gender of the face.
    /// </summary>
    public class FaceGender
    {
        /// <summary>
        /// Gender identified by the face. For example, `MALE` or `FEMALE`.
        /// </summary>
        [JsonProperty("gender", NullValueHandling = NullValueHandling.Ignore)]
        public string Gender { get; set; }
        /// <summary>
        /// The word for "male" or "female" in the language defined by the **Accept-Language** request header.
        /// </summary>
        [JsonProperty("gender_label", NullValueHandling = NullValueHandling.Ignore)]
        public string GenderLabel { get; set; }
        /// <summary>
        /// Confidence score in the range of 0 to 1. A higher score indicates greater confidence in the estimated value
        /// for the property.
        /// </summary>
        [JsonProperty("score", NullValueHandling = NullValueHandling.Ignore)]
        public float? Score { get; set; }
    }
}
