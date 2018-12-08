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

namespace IBM.WatsonDeveloperCloud.Assistant.v2
{
    /// <summary>
    /// Optional properties that control how the assistant responds.
    /// </summary>
    [fsObject(Converter = typeof(MessageInputOptionsConverter))]
    public class MessageInputOptions
    {
        /// <summary>
        /// Whether to return additional diagnostic information. Set to `true` to return additional information under
        /// the `output.debug` key.
        /// </summary>
        [fsProperty("debug")]
        public bool? Debug { get; set; }
        /// <summary>
        /// Whether to start a new conversation with this user input. Specify `true` to clear the state information
        /// stored by the session.
        /// </summary>
        [fsProperty("restart")]
        public bool? Restart { get; set; }
        /// <summary>
        /// Whether to return more than one intent. Set to `true` to return all matching intents.
        /// </summary>
        [fsProperty("alternate_intents")]
        public bool? AlternateIntents { get; set; }
        /// <summary>
        /// Whether to return session context with the response. If you specify `true`, the response will include the
        /// `context` property.
        /// </summary>
        [fsProperty("return_context")]
        public bool? ReturnContext { get; set; }
    }

    #region Create Value Converter
    public class MessageInputOptionsConverter : fsConverter
    {
        private fsSerializer _serializer = new fsSerializer();

        public override bool CanProcess(Type type)
        {
            return type == typeof(MessageInputOptions);
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            throw new NotImplementedException();
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            MessageInputOptions messageInputOptions = (MessageInputOptions)instance;
            serialized = null;

            Dictionary<string, fsData> serialization = new Dictionary<string, fsData>();

            fsData tempData = null;

            if (messageInputOptions.Debug != null)
            {
                _serializer.TrySerialize(messageInputOptions.Debug, out tempData);
                serialization.Add("debug", tempData);
            }

            if (messageInputOptions.Restart != null)
            {
                _serializer.TrySerialize(messageInputOptions.Restart, out tempData);
                serialization.Add("restart", tempData);
            }

            if (messageInputOptions.AlternateIntents != null)
            {
                _serializer.TrySerialize(messageInputOptions.AlternateIntents, out tempData);
                serialization.Add("alternate_intents", tempData);
            }

            if (messageInputOptions.ReturnContext != null)
            {
                _serializer.TrySerialize(messageInputOptions.ReturnContext, out tempData);
                serialization.Add("return_context", tempData);
            }

            serialized = new fsData(serialization);

            return fsResult.Success;
        }
        #endregion
    }
}
