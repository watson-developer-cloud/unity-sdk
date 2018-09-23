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

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// UpdateExample.
    /// </summary>
    [fsObject(Converter = typeof(UpdateExampleConverter))]
    public class UpdateExample
    {
        /// <summary>
        /// The text of the user input example. This string must conform to the following restrictions:  - It cannot contain carriage return, newline, or tab characters.  - It cannot consist of only whitespace characters.  - It must be no longer than 1024 characters.
        /// </summary>
        /// <value>The text of the user input example. This string must conform to the following restrictions:  - It cannot contain carriage return, newline, or tab characters.  - It cannot consist of only whitespace characters.  - It must be no longer than 1024 characters.</value>
        [fsProperty("text")]
        public string Text { get; set; }
        /// <summary>
        /// An array of contextual entity mentions.
        /// </summary>
        [fsProperty("mentions")]
        public List<Mentions> Mentions { get; set; }
    }

    public class UpdateExampleConverter : fsConverter
    {
        private static fsSerializer sm_Serializer = new fsSerializer();

        public override bool CanProcess(Type type)
        {
            return type == typeof(CreateExample);
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            throw new NotImplementedException();
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            UpdateExample updateExample = (UpdateExample)instance;
            serialized = null;

            Dictionary<string, fsData> serialization = new Dictionary<string, fsData>();

            fsData tempData = null;

            if (updateExample.Mentions != null)
            {
                sm_Serializer.TrySerialize(updateExample.Mentions, out tempData);
                serialization.Add("mentions", tempData);
            }

            if (updateExample.Text != null)
            {
                sm_Serializer.TrySerialize(updateExample.Text, out tempData);
                serialization.Add("text", tempData);
            }

            serialized = new fsData(serialization);

            return fsResult.Success;
        }
    }
}
