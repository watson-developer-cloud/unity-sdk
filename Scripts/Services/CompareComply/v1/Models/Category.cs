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
using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace  IBM.Watson.DeveloperCloud.Services.CompareComply.v1
{
    /// <summary>
    /// Information defining an element's subject matter.
    /// </summary>
    [fsObject(Converter = typeof(CategoryConverter))]
    public class Category
    {
        /// <summary>
        /// The category of the associated element.
        /// </summary>
        [fsProperty("label")]
        public string Label { get; set; }
        /// <summary>
        /// One or more hashed values that you can send to IBM to provide feedback or receive support.
        /// </summary>
        [fsProperty("provenance_ids")]
        public List<string> ProvenanceIds { get; set; }
        /// <summary>
        /// A string identifying the type of modification the feedback entry in the `updated_labels` array. Possible
        /// values are `added`, `unchanged`, and `removed`.
        /// </summary>
        public enum ModificationEnum
        {
            /// <summary>
            /// Enum added for added
            /// </summary>
            [EnumMember(Value = "added")]
            added,
            /// <summary>
            /// Enum notChanged for unchanged
            /// </summary>
            [EnumMember(Value = "unchanged")]
            unchanged,
            /// <summary>
            /// Enum removed for removed
            /// </summary>
            [EnumMember(Value = "removed")]
            removed
        }

        /// <summary>
        /// A string identifying the type of modification the feedback entry in the `updated_labels` array. Possible
        /// values are `added`, `unchanged`, and `removed`.
        /// </summary>
        [fsProperty("modification")]
        public ModificationEnum? Modification { get; set; }
    }

    #region CategoryConverter Converter
    public class CategoryConverter : fsConverter
    {
        private fsSerializer _serializer = new fsSerializer();

        public override bool CanProcess(Type type)
        {
            return type == typeof(Category);
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            if (data.IsString == false)
            {
                return fsResult.Fail("Type converter requires a string");
            }
            instance = fsTypeCache.GetType(data.AsString);
            if (instance == null)
            {
                return fsResult.Fail("Unable to find type " + data.AsString);
            }
            return fsResult.Success;
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
            return new Category();
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            Category category = (Category)instance;
            serialized = null;

            Dictionary<string, fsData> serialization = new Dictionary<string, fsData>();

            fsData tempData = null;

            if (category.Label != null)
            {
                _serializer.TrySerialize(category.Label, out tempData);
                serialization.Add("label", tempData);
            }

            if (category.ProvenanceIds != null && category.ProvenanceIds.Count > 0)
            {
                _serializer.TrySerialize(category.ProvenanceIds, out tempData);
                serialization.Add("provenance_ids", tempData);
            }

            serialized = new fsData(serialization);

            return fsResult.Success;
        }
    }
    #endregion
}
