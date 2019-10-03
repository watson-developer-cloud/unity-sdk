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

using Newtonsoft.Json;

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// Object containing normalization operations.
    /// </summary>
    public class NormalizationOperation
    {
        /// <summary>
        /// Identifies what type of operation to perform.
        ///
        /// **copy** - Copies the value of the **source_field** to the **destination_field** field. If the
        /// **destination_field** already exists, then the value of the **source_field** overwrites the original value
        /// of the **destination_field**.
        ///
        /// **move** - Renames (moves) the **source_field** to the **destination_field**. If the **destination_field**
        /// already exists, then the value of the **source_field** overwrites the original value of the
        /// **destination_field**. Rename is identical to copy, except that the **source_field** is removed after the
        /// value has been copied to the **destination_field** (it is the same as a _copy_ followed by a _remove_).
        ///
        /// **merge** - Merges the value of the **source_field** with the value of the **destination_field**. The
        /// **destination_field** is converted into an array if it is not already an array, and the value of the
        /// **source_field** is appended to the array. This operation removes the **source_field** after the merge. If
        /// the **source_field** does not exist in the current document, then the **destination_field** is still
        /// converted into an array (if it is not an array already). This conversion ensures the type for
        /// **destination_field** is consistent across all documents.
        ///
        /// **remove** - Deletes the **source_field** field. The **destination_field** is ignored for this operation.
        ///
        /// **remove_nulls** - Removes all nested null (blank) field values from the ingested document. **source_field**
        /// and **destination_field** are ignored by this operation because _remove_nulls_ operates on the entire
        /// ingested document. Typically, **remove_nulls** is invoked as the last normalization operation (if it is
        /// invoked at all, it can be time-expensive).
        /// </summary>
        public class OperationValue
        {
            /// <summary>
            /// Constant COPY for copy
            /// </summary>
            public const string COPY = "copy";
            /// <summary>
            /// Constant MOVE for move
            /// </summary>
            public const string MOVE = "move";
            /// <summary>
            /// Constant MERGE for merge
            /// </summary>
            public const string MERGE = "merge";
            /// <summary>
            /// Constant REMOVE for remove
            /// </summary>
            public const string REMOVE = "remove";
            /// <summary>
            /// Constant REMOVE_NULLS for remove_nulls
            /// </summary>
            public const string REMOVE_NULLS = "remove_nulls";
            
        }

        /// <summary>
        /// Identifies what type of operation to perform.
        ///
        /// **copy** - Copies the value of the **source_field** to the **destination_field** field. If the
        /// **destination_field** already exists, then the value of the **source_field** overwrites the original value
        /// of the **destination_field**.
        ///
        /// **move** - Renames (moves) the **source_field** to the **destination_field**. If the **destination_field**
        /// already exists, then the value of the **source_field** overwrites the original value of the
        /// **destination_field**. Rename is identical to copy, except that the **source_field** is removed after the
        /// value has been copied to the **destination_field** (it is the same as a _copy_ followed by a _remove_).
        ///
        /// **merge** - Merges the value of the **source_field** with the value of the **destination_field**. The
        /// **destination_field** is converted into an array if it is not already an array, and the value of the
        /// **source_field** is appended to the array. This operation removes the **source_field** after the merge. If
        /// the **source_field** does not exist in the current document, then the **destination_field** is still
        /// converted into an array (if it is not an array already). This conversion ensures the type for
        /// **destination_field** is consistent across all documents.
        ///
        /// **remove** - Deletes the **source_field** field. The **destination_field** is ignored for this operation.
        ///
        /// **remove_nulls** - Removes all nested null (blank) field values from the ingested document. **source_field**
        /// and **destination_field** are ignored by this operation because _remove_nulls_ operates on the entire
        /// ingested document. Typically, **remove_nulls** is invoked as the last normalization operation (if it is
        /// invoked at all, it can be time-expensive).
        /// Constants for possible values can be found using NormalizationOperation.OperationValue
        /// </summary>
        [JsonProperty("operation", NullValueHandling = NullValueHandling.Ignore)]
        public string Operation { get; set; }
        /// <summary>
        /// The source field for the operation.
        /// </summary>
        [JsonProperty("source_field", NullValueHandling = NullValueHandling.Ignore)]
        public string SourceField { get; set; }
        /// <summary>
        /// The destination field for the operation.
        /// </summary>
        [JsonProperty("destination_field", NullValueHandling = NullValueHandling.Ignore)]
        public string DestinationField { get; set; }
    }
}
