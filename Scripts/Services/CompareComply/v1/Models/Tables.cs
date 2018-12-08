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
    /// The contents of the tables extracted from a document.
    /// </summary>
    [fsObject]
    public class Tables
    {
        /// <summary>
        /// The numeric location of the identified element in the document, represented with two integers labeled
        /// `begin` and `end`.
        /// </summary>
        [fsProperty("location")]
        public Location Location { get; set; }
        /// <summary>
        /// The textual contents of the current table from the input document without associated markup content.
        /// </summary>
        [fsProperty("text")]
        public string Text { get; set; }
        /// <summary>
        /// The table's section title, if identified.
        /// </summary>
        [fsProperty("section_title")]
        public SectionTitle SectionTitle { get; set; }
        /// <summary>
        /// An array of table-level cells that apply as headers to all the other cells in the current table.
        /// </summary>
        [fsProperty("table_headers")]
        public List<TableHeaders> TableHeaders { get; set; }
        /// <summary>
        /// An array of row-level cells, each applicable as a header to other cells in the same row as itself, of the
        /// current table.
        /// </summary>
        [fsProperty("row_headers")]
        public List<RowHeaders> RowHeaders { get; set; }
        /// <summary>
        /// An array of column-level cells, each applicable as a header to other cells in the same column as itself, of
        /// the current table.
        /// </summary>
        [fsProperty("column_headers")]
        public List<ColumnHeaders> ColumnHeaders { get; set; }
        /// <summary>
        /// An array of cells that are neither table header nor column header nor row header cells, of the current table
        /// with corresponding row and column header associations.
        /// </summary>
        [fsProperty("body_cells")]
        public List<BodyCells> BodyCells { get; set; }
    }

}
