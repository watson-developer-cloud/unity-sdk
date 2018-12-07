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

namespace  IBM.Watson.DeveloperCloud.Services.CompareComply.v1
{
    /// <summary>
    /// A pair of `nature` and `party` objects. The `nature` object identifies the effect of the element on the
    /// identified `party`, and the `party` object identifies the affected party.
    /// </summary>
    [fsObject(Converter = typeof(LabelConverter))]
    public class Label
    {
        /// <summary>
        /// The identified `nature` of the element.
        /// </summary>
        [fsProperty("nature")]
        public string Nature { get; set; }
        /// <summary>
        /// The identified `party` of the element.
        /// </summary>
        [fsProperty("party")]
        public string Party { get; set; }
    }

    #region Label Converter
    public class LabelConverter : fsConverter
    {
        private fsSerializer _serializer = new fsSerializer();

        public override bool CanProcess(Type type)
        {
            return type == typeof(Label);
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

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            Label label = (Label)instance;
            serialized = null;

            Dictionary<string, fsData> serialization = new Dictionary<string, fsData>();

            fsData tempData = null;

            if (!string.IsNullOrEmpty(label.Nature))
            {
                _serializer.TrySerialize(label.Nature, out tempData);
                serialization.Add("nature", tempData);
            }

            if (!string.IsNullOrEmpty(label.Party))
            {
                _serializer.TrySerialize(label.Party, out tempData);
                serialization.Add("party", tempData);
            }

            serialized = new fsData(serialization);

            return fsResult.Success;
        }
    }
    #endregion
}
