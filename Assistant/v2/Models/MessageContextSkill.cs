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
using FullSerializer.Internal;
using IBM.Watson.Utilities;
using System;
using System.Collections.Generic;

namespace IBM.Watson.Assistant.V2
{
    [fsObject(Converter = typeof(MessageContextSkillConverter))]
    //[fsObject]
    public class MessageContextSkill
    {
        [fsProperty("user_defined")]
        public SerializableDictionary<string, string> UserDefined { get; set; }
    }

    #region Message Context Skill Converter
    public class MessageContextSkillConverter : fsConverter
    {
        private fsSerializer _serializer = new fsSerializer();

        public override bool CanProcess(Type type)
        {
            return type == typeof(MessageContextSkill);
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
            return new MessageContextSkill();
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            MessageContextSkill messageContextSkill = (MessageContextSkill)instance;
            serialized = null;

            Dictionary<string, fsData> serialization = new Dictionary<string, fsData>();
            //Dictionary<string, fsData> userDefinedDict = new Dictionary<string, fsData>();

            fsData tempData = null;

            foreach (KeyValuePair<string, string> kvp in messageContextSkill.UserDefined)
            {
                _serializer.TrySerialize(kvp.Value, out tempData);
                serialization.Add(kvp.Key, tempData);
            }

            //_serializer.TrySerialize(userDefinedDict, out tempData);

            //serialization.Add("user_defined", tempData);
            serialized = new fsData(serialization);

            return fsResult.Success;
        }
        #endregion
    }
}
