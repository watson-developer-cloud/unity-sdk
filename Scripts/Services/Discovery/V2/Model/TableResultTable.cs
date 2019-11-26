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
    /// Full table object retrieved from Table Understanding Enrichment.
    /// </summary>
    public class TableResultTable
    {
        /// <summary>
        /// The numeric location of the identified element in the document, represented with two integers labeled
        /// `begin` and `end`.
        /// </summary>
        [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
        public TableElementLocation Location { get; set; }
        /// <summary>
        /// The textual contents of the current table from the input document without associated markup content.
        /// </summary>
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
        /// <summary>
        /// Text and associated location within a table.
        /// </summary>
        [JsonProperty("section_title", NullValueHandling = NullValueHandling.Ignore)]
        public TableTextLocation SectionTitle { get; set; }
        /// <summary>
        /// Text and associated location within a table.
        /// </summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public TableTextLocation Title { get; set; }
        /// <summary>
        /// An array of table-level cells that apply as headers to all the other cells in the current table.
        /// </summary>
        [JsonProperty("table_headers", NullValueHandling = NullValueHandling.Ignore)]
        public List<TableHeaders> TableHeaders { get; set; }
        /// <summary>
        /// An array of row-level cells, each applicable as a header to other cells in the same row as itself, of the
        /// current table.
        /// </summary>
        [JsonProperty("row_headers", NullValueHandling = NullValueHandling.Ignore)]
        public List<TableRowHeaders> RowHeaders { get; set; }
        /// <summary>
        /// An array of column-level cells, each applicable as a header to other cells in the same column as itself, of
        /// the current table.
        /// </summary>
        [JsonProperty("column_headers", NullValueHandling = NullValueHandling.Ignore)]
        public List<TableColumnHeaders> ColumnHeaders { get; set; }
        /// <summary>
        /// An array of key-value pairs identified in the current table.
        /// </summary>
        [JsonProperty("key_value_pairs", NullValueHandling = NullValueHandling.Ignore)]
        public List<TableKeyValuePairs> KeyValuePairs { get; set; }
        /// <summary>
        /// An array of cells that are neither table header nor column header nor row header cells, of the current table
        /// with corresponding row and column header associations.
        /// </summary>
        [JsonProperty("body_cells", NullValueHandling = NullValueHandling.Ignore)]
        public List<TableBodyCells> BodyCells { get; set; }
        /// <summary>
        /// An array of lists of textual entries across the document related to the current table being parsed.
        /// </summary>
        [JsonProperty("contexts", NullValueHandling = NullValueHandling.Ignore)]
        public List<TableTextLocation> Contexts { get; set; }
    }
}
