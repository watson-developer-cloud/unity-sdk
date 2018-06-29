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
    /// DocumentAnalysis.
    /// </summary>
    public class DocumentAnalysis
    {
        /// <summary>
        /// **`2017-09-21`:** An array of `ToneScore` objects that provides the results of the analysis for each
        /// qualifying tone of the document. The array includes results for any tone whose score is at least 0.5. The
        /// array is empty if no tone has a score that meets this threshold. **`2016-05-19`:** Not returned.
        /// </summary>
        /// <value>
        /// **`2017-09-21`:** An array of `ToneScore` objects that provides the results of the analysis for each
        /// qualifying tone of the document. The array includes results for any tone whose score is at least 0.5. The
        /// array is empty if no tone has a score that meets this threshold. **`2016-05-19`:** Not returned.
        /// </value>
        [fsProperty("tones")]
        public List<ToneScore> Tones { get; set; }
        /// <summary>
        /// **`2017-09-21`:** Not returned. **`2016-05-19`:** An array of `ToneCategory` objects that provides the
        /// results of the tone analysis for the full document of the input content. The service returns results only
        /// for the tones specified with the `tones` parameter of the request.
        /// </summary>
        /// <value>
        /// **`2017-09-21`:** Not returned. **`2016-05-19`:** An array of `ToneCategory` objects that provides the
        /// results of the tone analysis for the full document of the input content. The service returns results only
        /// for the tones specified with the `tones` parameter of the request.
        /// </value>
        [fsProperty("tone_categories")]
        public List<ToneCategory> ToneCategories { get; set; }
        /// <summary>
        /// **`2017-09-21`:** A warning message if the overall content exceeds 128 KB or contains more than 1000
        /// sentences. The service analyzes only the first 1000 sentences for document-level analysis and the first 100
        /// sentences for sentence-level analysis. **`2016-05-19`:** Not returned.
        /// </summary>
        /// <value>
        /// **`2017-09-21`:** A warning message if the overall content exceeds 128 KB or contains more than 1000
        /// sentences. The service analyzes only the first 1000 sentences for document-level analysis and the first 100
        /// sentences for sentence-level analysis. **`2016-05-19`:** Not returned.
        /// </value>
        [fsProperty("warning")]
        public string Warning { get; set; }
    }

}
