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
        /// An array of values, each being the `id` value of a row header that is applicable to this body cell.
        /// </summary>
        [fsProperty("row_header_ids")]
        public List<string> RowHeaderIds { get; set; }
        /// <summary>
        /// An array of values, each being the `text` value of a row header that is applicable to this body cell.
        /// </summary>
        [fsProperty("row_header_texts")]
        public List<string> RowHeaderTexts { get; set; }
        /// <summary>
        /// If you provide customization input, the normalized version of the row header texts according to the
        /// customization; otherwise, the same value as `row_header_texts`.
        /// </summary>
        [fsProperty("row_header_texts_normalized")]
        public List<string> RowHeaderTextsNormalized { get; set; }
        /// <summary>
        /// An array of values, each being the `id` value of a column header that is applicable to the current cell.
        /// </summary>
        [fsProperty("column_header_ids")]
        public List<string> ColumnHeaderIds { get; set; }
        /// <summary>
        /// An array of values, each being the `id` value of a column header that is applicable to the current cell.
        /// </summary>
        [fsProperty("column_header_texts")]
        public List<string> ColumnHeaderTexts { get; set; }
        /// <summary>
        /// If you provide customization input, the normalized version of the column header texts according to the
        /// customization; otherwise, the same value as `column_header_texts`.
        /// </summary>
        [fsProperty("column_header_texts_normalized")]
        public List<string> ColumnHeaderTextsNormalized { get; set; }
        /// <summary>
        /// Gets or Sets Attributes
        /// </summary>
        [fsProperty("attributes")]
        public List<Attribute> Attributes { get; set; }
    }

}
