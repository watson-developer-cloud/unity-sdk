/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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

namespace IBM.Watson.Discovery.v1.Model
{
    /// <summary>
    /// DeleteCollectionResponse.
    /// </summary>
    public class DeleteCollectionResponse
    {
        /// <summary>
        /// The status of the collection. The status of a successful deletion operation is `deleted`.
        /// </summary>
        public class StatusEnumValue
        {
            /// <summary>
            /// Constant DELETED for deleted
            /// </summary>
            public const string DELETED = "deleted";
            
        }

        /// <summary>
        /// The status of the collection. The status of a successful deletion operation is `deleted`.
        /// </summary>
        [fsProperty("status")]
        public string Status { get; set; }
        /// <summary>
        /// The unique identifier of the collection that is being deleted.
        /// </summary>
        [fsProperty("collection_id")]
        public string CollectionId { get; set; }
    }


}
