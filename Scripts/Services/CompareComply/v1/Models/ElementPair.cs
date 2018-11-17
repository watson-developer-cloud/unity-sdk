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
    /// Details of semantically aligned elements.
    /// </summary>
    [fsObject]
    public class ElementPair
    {
        /// <summary>
        /// The label of the document (that is, the value of either the `file_1_label` or `file_2_label` parameters) in
        /// which the element occurs.
        /// </summary>
        [fsProperty("document_label")]
        public string DocumentLabel { get; set; }
        /// <summary>
        /// The text of the element.
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
        /// Description of the action specified by the element  and whom it affects.
        /// </summary>
        [fsProperty("types")]
        public List<TypeLabel> Types { get; set; }
        /// <summary>
        /// List of functional categories into which the element falls; in other words, the subject matter of the
        /// element.
        /// </summary>
        [fsProperty("categories")]
        public List<Category> Categories { get; set; }
        /// <summary>
        /// List of document attributes.
        /// </summary>
        [fsProperty("attributes")]
        public List<Attribute> Attributes { get; set; }
    }

}
