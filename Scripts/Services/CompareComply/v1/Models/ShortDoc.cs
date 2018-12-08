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
    /// Brief information about the input document.
    /// </summary>
    [fsObject(Converter = typeof(ShortDocConverter))]
    public class ShortDoc
    {
        /// <summary>
        /// The title of the input document, if identified.
        /// </summary>
        [fsProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// The MD5 hash of the input document.
        /// </summary>
        [fsProperty("hash")]
        public string Hash { get; set; }
    }

    #region ShortDocConverter Converter
    public class ShortDocConverter : fsConverter
    {
        private fsSerializer _serializer = new fsSerializer();

        public override bool CanProcess(Type type)
        {
            return type == typeof(ShortDoc);
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
            return new ShortDoc();
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            ShortDoc shortDoc = (ShortDoc)instance;
            serialized = null;

            Dictionary<string, fsData> serialization = new Dictionary<string, fsData>();

            fsData tempData = null;

            if (!string.IsNullOrEmpty(shortDoc.Title))
            {
                _serializer.TrySerialize(shortDoc.Title, out tempData);
                serialization.Add("title", tempData);
            }

            if (!string.IsNullOrEmpty(shortDoc.Hash))
            {
                _serializer.TrySerialize(shortDoc.Hash, out tempData);
                serialization.Add("hash", tempData);
            }

            serialized = new fsData(serialization);

            return fsResult.Success;
        }
    }
    #endregion
}
