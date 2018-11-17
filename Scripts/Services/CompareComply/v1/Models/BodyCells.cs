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
    /// Cells that are not table header, column header, or row header cells.
    /// </summary>
    [fsObject]
    public class BodyCells
    {
        /// <summary>
        /// A string value in the format `columnHeader-x-y`, where `x` and `y` are the begin and end offsets of this
        /// column header cell in the input document.
        /// </summary>
        [fsProperty("cell_id")]
        public string CellId { get; set; }
        /// <summary>
        /// The numeric location of the identified element in the document, represented with two integers labeled
        /// `begin` and `end`.
        /// </summary>
        [fsProperty("location")]
        public Location Location { get; set; }
        /// <summary>
        /// The textual contents of this cell from the input document without associated markup content.
        /// </summary>
        [fsProperty("text")]
        public string Text { get; set; }
        /// <summary>
        /// The `begin` index of this cell's `row` location in the current table.
        /// </summary>
        [fsProperty("row_index_begin")]
        public long? RowIndexBegin { get; set; }
        /// <summary>
        /// The `end` index of this cell's `row` location in the current table.
        /// </summary>
        [fsProperty("row_index_end")]
        public long? RowIndexEnd { get; set; }
        /// <summary>
        /// The `begin` index of this cell's `column` location in the current table.
        /// </summary>
        [fsProperty("column_index_begin")]
        public long? ColumnIndexBegin { get; set; }
        /// <summary>
        /// The `end` index of this cell's `column` location in the current table.
        /// </summary>
        [fsProperty("column_index_end")]
        public long? ColumnIndexEnd { get; set; }
        /// <summary>
        /// Gets or Sets rowHeaderIds
        /// </summary>
        [fsProperty("row_header_ids")]
        public List<RowHeaderIds> RowHeaderIds { get; set; }
        /// <summary>
        /// Gets or Sets rowHeaderTexts
        /// </summary>
        [fsProperty("row_header_texts")]
        public List<RowHeaderTexts> RowHeaderTexts { get; set; }
        /// <summary>
        /// Gets or Sets rowHeaderTextsNormalized
        /// </summary>
        [fsProperty("row_header_texts_normalized")]
        public List<RowHeaderTextsNormalized> RowHeaderTextsNormalized { get; set; }
        /// <summary>
        /// Gets or Sets columnHeaderIds
        /// </summary>
        [fsProperty("column_header_ids")]
        public List<ColumnHeaderIds> ColumnHeaderIds { get; set; }
        /// <summary>
        /// Gets or Sets columnHeaderTexts
        /// </summary>
        [fsProperty("column_header_texts")]
        public List<ColumnHeaderTexts> ColumnHeaderTexts { get; set; }
        /// <summary>
        /// Gets or Sets columnHeaderTextsNormalized
        /// </summary>
        [fsProperty("column_header_texts_normalized")]
        public List<ColumnHeaderTextsNormalized> ColumnHeaderTextsNormalized { get; set; }
    }

}
