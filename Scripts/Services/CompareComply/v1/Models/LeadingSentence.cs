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
    /// The leading sentences in a section or subsection of the input document.
    /// </summary>
    [fsObject]
    public class LeadingSentence
    {
        /// <summary>
        /// The text of the leading sentence.
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
        /// An array of `location` objects listing the locations of detected leading sentences.
        /// </summary>
        [fsProperty("element_locations")]
        public List<ElementLocations> ElementLocations { get; set; }
    }

}
