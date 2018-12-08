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

namespace IBM.WatsonDeveloperCloud.Assistant.v2
{
    /// <summary>
    /// The user input.
    /// </summary>
    [fsObject(Converter = typeof(MessageInputConverter))]
    public class MessageInput
    {
        /// <summary>
        /// The type of user input. Currently, only text input is supported.
        /// </summary>
        public enum MessageTypeEnum
        {
            /// <summary>
            /// Enum text for text
            /// </summary>
            [EnumMember(Value = "text")]
            text
        }

        /// <summary>
        /// The type of user input. Currently, only text input is supported.
        /// </summary>
        [fsProperty("message_type")]
        public MessageTypeEnum? MessageType { get; set; }
        /// <summary>
        /// The text of the user input. This string cannot contain carriage return, newline, or tab characters, and it
        /// must be no longer than 2048 characters.
        /// </summary>
        [fsProperty("text")]
        public string Text { get; set; }
        /// <summary>
        /// Properties that control how the assistant responds.
        /// </summary>
        [fsProperty("options")]
        public MessageInputOptions Options { get; set; }
        /// <summary>
        /// Intents to use when evaluating the user input. Include intents from the previous response to continue using
        /// those intents rather than trying to recognize intents in the new input.
        /// </summary>
        [fsProperty("intents")]
        public List<RuntimeIntent> Intents { get; set; }
        /// <summary>
        /// Entities to use when evaluating the message. Include entities from the previous response to continue using
        /// those entities rather than detecting entities in the new input.
        /// </summary>
        [fsProperty("entities")]
        public List<RuntimeEntity> Entities { get; set; }
        /// <summary>
        /// For internal use only.
        /// </summary>
        [fsProperty("suggestion_id")]
        public string SuggestionId { get; set; }
    }

    #region Message Input Converter
    public class MessageInputConverter : fsConverter
    {
        private fsSerializer _serializer = new fsSerializer();

        public override bool CanProcess(Type type)
        {
            return type == typeof(MessageInput);
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
            return new MessageInput();
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            MessageInput messageInput = (MessageInput)instance;
            serialized = null;

            Dictionary<string, fsData> serialization = new Dictionary<string, fsData>();

            fsData tempData = null;

            if (messageInput.MessageType != null)
            {
                _serializer.TrySerialize(messageInput.MessageType, out tempData);
                serialization.Add("message_type", tempData);
            }

            if (messageInput.Text != null)
            {
                _serializer.TrySerialize(messageInput.Text, out tempData);
                serialization.Add("text", tempData);
            }

            if (messageInput.Options != null)
            {
                _serializer.TrySerialize(messageInput.Options, out tempData);
                serialization.Add("options", tempData);
            }

            if (messageInput.Intents != null)
            {
                _serializer.TrySerialize(messageInput.Intents, out tempData);
                serialization.Add("intents", tempData);
            }

            if (messageInput.Entities != null)
            {
                _serializer.TrySerialize(messageInput.Entities, out tempData);
                serialization.Add("entities", tempData);
            }

            if (messageInput.SuggestionId != null)
            {
                _serializer.TrySerialize(messageInput.SuggestionId, out tempData);
                serialization.Add("suggestion_id", tempData);
            }

            serialized = new fsData(serialization);

            return fsResult.Success;
        }
        #endregion
    }
}
