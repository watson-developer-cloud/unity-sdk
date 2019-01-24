/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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

namespace IBM.Watson.Discovery.v1.Model
{
    /// <summary>
    /// FontSetting.
    /// </summary>
    public class FontSetting
    {
        /// <summary>
        /// Gets or Sets Level
        /// </summary>
        [fsProperty("level")]
        public long? Level { get; set; }
        /// <summary>
        /// Gets or Sets MinSize
        /// </summary>
        [fsProperty("min_size")]
        public long? MinSize { get; set; }
        /// <summary>
        /// Gets or Sets MaxSize
        /// </summary>
        [fsProperty("max_size")]
        public long? MaxSize { get; set; }
        /// <summary>
        /// Gets or Sets Bold
        /// </summary>
        [fsProperty("bold")]
        public bool? Bold { get; set; }
        /// <summary>
        /// Gets or Sets Italic
        /// </summary>
        [fsProperty("italic")]
        public bool? Italic { get; set; }
        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [fsProperty("name")]
        public string Name { get; set; }
    }


}
