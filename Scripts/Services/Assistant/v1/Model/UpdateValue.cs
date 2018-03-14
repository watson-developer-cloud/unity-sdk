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
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// UpdateValue.
    /// </summary>
    [fsObject(Converter = typeof(UpdateValueConverter))]
    public class UpdateValue
    {
        /// <summary>
        /// Specifies the type of value.
        /// </summary>
        /// <value>Specifies the type of value.</value>
        public enum ValueTypeEnum
        {

            /// <summary>
            /// Enum SYNONYMS for synonyms
            /// </summary>
            [EnumMember(Value = "synonyms")]
            SYNONYMS,

            /// <summary>
            /// Enum PATTERNS for patterns
            /// </summary>
            [EnumMember(Value = "patterns")]
            PATTERNS
        }

        /// <summary>
        /// Specifies the type of value.
        /// </summary>
        /// <value>Specifies the type of value.</value>
        [fsProperty("type")]
        public ValueTypeEnum? ValueType { get; set; }
        /// <summary>
        /// The text of the entity value. This string must conform to the following restrictions:  - It cannot contain carriage return, newline, or tab characters.  - It cannot consist of only whitespace characters.  - It must be no longer than 64 characters.
        /// </summary>
        /// <value>The text of the entity value. This string must conform to the following restrictions:  - It cannot contain carriage return, newline, or tab characters.  - It cannot consist of only whitespace characters.  - It must be no longer than 64 characters.</value>
        [fsProperty("value")]
        public string Value { get; set; }
        /// <summary>
        /// Any metadata related to the entity value.
        /// </summary>
        /// <value>Any metadata related to the entity value.</value>
        [fsProperty("metadata")]
        public object Metadata { get; set; }
        /// <summary>
        /// An array of synonyms for the entity value. You can provide either synonyms or patterns (as indicated by **type**), but not both. A synonym must conform to the following resrictions:  - It cannot contain carriage return, newline, or tab characters.  - It cannot consist of only whitespace characters.  - It must be no longer than 64 characters.
        /// </summary>
        /// <value>An array of synonyms for the entity value. You can provide either synonyms or patterns (as indicated by **type**), but not both. A synonym must conform to the following resrictions:  - It cannot contain carriage return, newline, or tab characters.  - It cannot consist of only whitespace characters.  - It must be no longer than 64 characters.</value>
        [fsProperty("synonyms")]
        public List<string> Synonyms { get; set; }
        /// <summary>
        /// An array of patterns for the entity value. You can provide either synonyms or patterns (as indicated by **type**), but not both. A pattern is a regular expression no longer than 128 characters. For more information about how to specify a pattern, see the [documentation](https://console.bluemix.net/docs/services/conversation/entities.html#creating-entities).
        /// </summary>
        /// <value>An array of patterns for the entity value. You can provide either synonyms or patterns (as indicated by **type**), but not both. A pattern is a regular expression no longer than 128 characters. For more information about how to specify a pattern, see the [documentation](https://console.bluemix.net/docs/services/conversation/entities.html#creating-entities).</value>
        [fsProperty("patterns")]
        public List<string> Patterns { get; set; }
    }

    #region Updaet Value Converter
    public class UpdateValueConverter : fsConverter
    {
        private fsSerializer _serializer = new fsSerializer();

        public override bool CanProcess(Type type)
        {
            return type == typeof(CreateValue);
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            throw new NotImplementedException();
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            UpdateValue updateValue = (UpdateValue)instance;
            serialized = null;

            Dictionary<string, fsData> serialization = new Dictionary<string, fsData>();

            fsData tempData = null;

            if (updateValue.ValueType != null)
            {
                _serializer.TrySerialize(updateValue.ValueType, out tempData);
                serialization.Add("type", tempData);
            }

            if (updateValue.Value != null)
            {
                _serializer.TrySerialize(updateValue.Value, out tempData);
                serialization.Add("value", tempData);
            }

            if (updateValue.Metadata != null)
            {
                _serializer.TrySerialize(updateValue.Metadata, out tempData);
                serialization.Add("metadata", tempData);
            }

            if (updateValue.Synonyms != null)
            {
                _serializer.TrySerialize(updateValue.Synonyms, out tempData);
                serialization.Add("synonyms", tempData);
            }

            if (updateValue.Patterns != null)
            {
                _serializer.TrySerialize(updateValue.Patterns, out tempData);
                serialization.Add("patterns", tempData);
            }

            serialized = new fsData(serialization);

            return fsResult.Success;
        }
        #endregion
    }
}
