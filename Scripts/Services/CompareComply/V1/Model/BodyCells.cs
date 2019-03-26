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

namespace IBM.Watson.CompareComply.V1.Model
{
    /// <summary>
    /// Cells that are not table header, column header, or row header cells.
    /// </summary>
    public class BodyCells
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
        public Location Location { get; set; }
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
        /// Gets or Sets RowHeaderIds
        /// </summary>
        [JsonProperty("row_header_ids", NullValueHandling = NullValueHandling.Ignore)]
        public List<RowHeaderIds> RowHeaderIds { get; set; }
        /// <summary>
        /// Gets or Sets RowHeaderTexts
        /// </summary>
        [JsonProperty("row_header_texts", NullValueHandling = NullValueHandling.Ignore)]
        public List<RowHeaderTexts> RowHeaderTexts { get; set; }
        /// <summary>
        /// Gets or Sets RowHeaderTextsNormalized
        /// </summary>
        [JsonProperty("row_header_texts_normalized", NullValueHandling = NullValueHandling.Ignore)]
        public List<RowHeaderTextsNormalized> RowHeaderTextsNormalized { get; set; }
        /// <summary>
        /// Gets or Sets ColumnHeaderIds
        /// </summary>
        [JsonProperty("column_header_ids", NullValueHandling = NullValueHandling.Ignore)]
        public List<ColumnHeaderIds> ColumnHeaderIds { get; set; }
        /// <summary>
        /// Gets or Sets ColumnHeaderTexts
        /// </summary>
        [JsonProperty("column_header_texts", NullValueHandling = NullValueHandling.Ignore)]
        public List<ColumnHeaderTexts> ColumnHeaderTexts { get; set; }
        /// <summary>
        /// Gets or Sets ColumnHeaderTextsNormalized
        /// </summary>
        [JsonProperty("column_header_texts_normalized", NullValueHandling = NullValueHandling.Ignore)]
        public List<ColumnHeaderTextsNormalized> ColumnHeaderTextsNormalized { get; set; }
        /// <summary>
        /// Gets or Sets Attributes
        /// </summary>
        [JsonProperty("attributes", NullValueHandling = NullValueHandling.Ignore)]
        public List<Attribute> Attributes { get; set; }
    }
}
