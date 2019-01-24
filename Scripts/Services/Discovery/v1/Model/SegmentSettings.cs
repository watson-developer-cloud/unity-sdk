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

using System.Collections.Generic;
using FullSerializer;

namespace IBM.Watson.Discovery.v1.Model
{
    /// <summary>
    /// A list of Document Segmentation settings.
    /// </summary>
    public class SegmentSettings
    {
        /// <summary>
        /// Enables/disables the Document Segmentation feature.
        /// </summary>
        [fsProperty("enabled")]
        public bool? Enabled { get; set; }
        /// <summary>
        /// Defines the heading level that splits into document segments. Valid values are h1, h2, h3, h4, h5, h6.
        /// </summary>
        [fsProperty("selector_tags")]
        public List<string> SelectorTags { get; set; }
    }


}
