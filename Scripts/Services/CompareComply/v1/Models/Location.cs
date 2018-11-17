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

namespace  IBM.Watson.DeveloperCloud.Services.CompareComply.v1
{
    /// <summary>
    /// The numeric location of the identified element in the document, represented with two integers labeled `begin`
    /// and `end`.
    /// </summary>
    [fsObject(Converter = typeof(LocationConverter))]
    public class Location
    {
        /// <summary>
        /// The element's `begin` index.
        /// </summary>
        [fsProperty("begin")]
        public long? Begin { get; set; }
        /// <summary>
        /// The element's `end` index.
        /// </summary>
        [fsProperty("end")]
        public long? End { get; set; }
    }

    #region Location Converter
    public class LocationConverter : fsConverter
    {
        private fsSerializer _serializer = new fsSerializer();

        public override bool CanProcess(Type type)
        {
            return type == typeof(Location);
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            if (data.Type != fsDataType.Object)
            {
                return fsResult.Fail("Expected object fsData type but got " + data.Type);
            }

            var myType = (Location)instance;
            Dictionary<string, fsData> dataDict = data.AsDictionary;
            if (!dataDict["begin"].IsNull)
            {
                string beginString = dataDict["begin"].AsString;
                long beginLong;
                long.TryParse(beginString, out beginLong);
                myType.Begin = beginLong;
            }
            if (!dataDict["end"].IsNull)
            {
                string endString = dataDict["end"].AsString;
                long endLong;
                long.TryParse(endString, out endLong);
                myType.End = endLong;
            }
            return fsResult.Success;
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
            return new Location();
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            Location location = (Location)instance;
            serialized = null;

            Dictionary<string, fsData> serialization = new Dictionary<string, fsData>();

            fsData tempData = null;

            if (location.Begin != null)
            {
                _serializer.TrySerialize(location.Begin, out tempData);
                serialization.Add("begin", tempData);
            }

            if (location.End != null)
            {
                _serializer.TrySerialize(location.End, out tempData);
                serialization.Add("end", tempData);
            }

            serialized = new fsData(serialization);

            return fsResult.Success;
        }
    }
    #endregion
}
