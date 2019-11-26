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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IBM.Watson.Discovery.V2.Model
{
    /// <summary>
    /// Cells that are not table header, column header, or row header cells.
    /// </summary>
    public class TableBodyCells
    {
        /// <summary>
        /// The unique ID of the cell in the current table.
        /// </summary>
        [JsonProperty("cell_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CellId { get; set; }
        /// <summary>
        /// The numeric location of the identified element in the document, represented with two integers labeled
        /// `begin` and `end`.
        /// </summary>
        [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
        public TableElementLocation Location { get; set; }
        /// <summary>
        /// The textual contents of this cell from the input document without associated markup content.
        /// </summary>
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
        /// <summary>
        /// The `begin` index of this cell's `row` location in the current table.
        /// </summary>
        [JsonProperty("row_index_begin", NullValueHandling = NullValueHandling.Ignore)]
        public long? RowIndexBegin { get; set; }
        /// <summary>
        /// The `end` index of this cell's `row` location in the current table.
        /// </summary>
        [JsonProperty("row_index_end", NullValueHandling = NullValueHandling.Ignore)]
        public long? RowIndexEnd { get; set; }
        /// <summary>
        /// The `begin` index of this cell's `column` location in the current table.
        /// </summary>
        [JsonProperty("column_index_begin", NullValueHandling = NullValueHandling.Ignore)]
        public long? ColumnIndexBegin { get; set; }
        /// <summary>
        /// The `end` index of this cell's `column` location in the current table.
        /// </summary>
        [JsonProperty("column_index_end", NullValueHandling = NullValueHandling.Ignore)]
        public long? ColumnIndexEnd { get; set; }
        /// <summary>
        /// A list of table row header ids.
        /// </summary>
        [JsonProperty("row_header_ids", NullValueHandling = NullValueHandling.Ignore)]
        public List<TableRowHeaderIds> RowHeaderIds { get; set; }
        /// <summary>
        /// A list of table row header texts.
        /// </summary>
        [JsonProperty("row_header_texts", NullValueHandling = NullValueHandling.Ignore)]
        public List<TableRowHeaderTexts> RowHeaderTexts { get; set; }
        /// <summary>
        /// A list of table row header texts normalized.
        /// </summary>
        [JsonProperty("row_header_texts_normalized", NullValueHandling = NullValueHandling.Ignore)]
        public List<TableRowHeaderTextsNormalized> RowHeaderTextsNormalized { get; set; }
        /// <summary>
        /// A list of table column header ids.
        /// </summary>
        [JsonProperty("column_header_ids", NullValueHandling = NullValueHandling.Ignore)]
        public List<TableColumnHeaderIds> ColumnHeaderIds { get; set; }
        /// <summary>
        /// A list of table column header texts.
        /// </summary>
        [JsonProperty("column_header_texts", NullValueHandling = NullValueHandling.Ignore)]
        public List<TableColumnHeaderTexts> ColumnHeaderTexts { get; set; }
        /// <summary>
        /// A list of table column header texts normalized.
        /// </summary>
        [JsonProperty("column_header_texts_normalized", NullValueHandling = NullValueHandling.Ignore)]
        public List<TableColumnHeaderTextsNormalized> ColumnHeaderTextsNormalized { get; set; }
        /// <summary>
        /// A list of document attributes.
        /// </summary>
        [JsonProperty("attributes", NullValueHandling = NullValueHandling.Ignore)]
        public List<DocumentAttribute> Attributes { get; set; }
    }
}
