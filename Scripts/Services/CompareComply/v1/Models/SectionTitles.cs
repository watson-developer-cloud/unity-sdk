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

namespace  IBM.Watson.DeveloperCloud.Services.CompareComply.v1
{
    /// <summary>
    /// An array containing one object per section or subsection detected in the input document. Sections and
    /// subsections are not nested; instead, they are flattened out and can be placed back in order by using the `begin`
    /// and `end` values of the element and the `level` value of the section.
    /// </summary>
    [fsObject]
    public class SectionTitles
    {
        /// <summary>
        /// The text of the section title, if identified.
        /// </summary>
        [fsProperty("text")]
        public string Text { get; set; }
        /// <summary>
        /// The numeric location of the identified element in the document, represented with two integers labeled
        /// `begin` and `end`.
        /// </summary>
        [fsProperty("location")]
        public Location Location { get; set; }
        /// <summary>
        /// An integer indicating the level at which the section is located in the input document. For example, `1`
        /// represents a top-level section, `2` represents a subsection within the level `1` section, and so forth.
        /// </summary>
        [fsProperty("level")]
        public long? Level { get; set; }
        /// <summary>
        /// An array of `location` objects listing the locations of detected section titles.
        /// </summary>
        [fsProperty("element_locations")]
        public List<ElementLocations> ElementLocations { get; set; }
    }

}
