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
    /// Row-level cells, each applicable as a header to other cells in the same row as itself, of the current table.
    /// </summary>
    [fsObject]
    public class RowHeaders
    {
        /// <summary>
        /// A string value in the format `rowHeader-x-y`, where `x` and `y` are the begin and end offsets of this row
        /// header cell in the input document.
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
        /// If you provide customization input, the normalized version of the cell text according to the customization;
        /// otherwise, the same value as `text`.
        /// </summary>
        [fsProperty("text_normalized")]
        public string TextNormalized { get; set; }
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
    }

}
