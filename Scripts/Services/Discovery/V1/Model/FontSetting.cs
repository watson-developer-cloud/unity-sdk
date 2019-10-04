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
    /// Font matching configuration.
    /// </summary>
    public class FontSetting
    {
        /// <summary>
        /// The HTML heading level that any content with the matching font is converted to.
        /// </summary>
        [JsonProperty("level", NullValueHandling = NullValueHandling.Ignore)]
        public long? Level { get; set; }
        /// <summary>
        /// The minimum size of the font to match.
        /// </summary>
        [JsonProperty("min_size", NullValueHandling = NullValueHandling.Ignore)]
        public long? MinSize { get; set; }
        /// <summary>
        /// The maximum size of the font to match.
        /// </summary>
        [JsonProperty("max_size", NullValueHandling = NullValueHandling.Ignore)]
        public long? MaxSize { get; set; }
        /// <summary>
        /// When `true`, the font is matched if it is bold.
        /// </summary>
        [JsonProperty("bold", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Bold { get; set; }
        /// <summary>
        /// When `true`, the font is matched if it is italic.
        /// </summary>
        [JsonProperty("italic", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Italic { get; set; }
        /// <summary>
        /// The name of the font.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }
}
