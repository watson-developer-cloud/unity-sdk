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

namespace  IBM.Watson.DeveloperCloud.Services.CompareComply.v1
{
    /// <summary>
    /// If you provide customization input, the normalized version of the row header texts according to the
    /// customization; otherwise, the same value as `row_header_texts`.
    /// </summary>
    [fsObject]
    public class RowHeaderTextsNormalized
    {
        /// <summary>
        /// The normalized version of a row header text.
        /// </summary>
        [fsProperty("text_normalized")]
        public string TextNormalized { get; set; }
    }

}
