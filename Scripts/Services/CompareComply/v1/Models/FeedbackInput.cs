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

namespace IBM.Watson.DeveloperCloud.Services.CompareComply.v1
{
    /// <summary>
    /// The feedback to be added to an element in the document.
    /// </summary>
    [fsObject(Converter = typeof(FeedbackInputConverter))]
    public class FeedbackInput
    {
        /// <summary>
        /// An optional string identifying the user.
        /// </summary>
        [fsProperty("user_id")]
        public string UserId { get; set; }
        /// <summary>
        /// An optional comment on or description of the feedback.
        /// </summary>
        [fsProperty("comment")]
        public string Comment { get; set; }
        /// <summary>
        /// Feedback data for submission.
        /// </summary>
        [fsProperty("feedback_data")]
        public FeedbackDataInput FeedbackData { get; set; }
    }

    #region FeedbackInput Converter
    public class FeedbackInputConverter : fsConverter
    {
        private fsSerializer _serializer = new fsSerializer();

        public override bool CanProcess(Type type)
        {
            return type == typeof(FeedbackInput);
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
            FeedbackInput feedbackInput = (FeedbackInput)instance;
            serialized = null;

            Dictionary<string, fsData> serialization = new Dictionary<string, fsData>();

            fsData tempData = null;

            if (!string.IsNullOrEmpty(feedbackInput.UserId))
            {
                _serializer.TrySerialize(feedbackInput.UserId, out tempData);
                serialization.Add("user_id", tempData);
            }

            if (!string.IsNullOrEmpty(feedbackInput.Comment))
            {
                _serializer.TrySerialize(feedbackInput.Comment, out tempData);
                serialization.Add("comment", tempData);
            }

            if (feedbackInput.FeedbackData != null)
            {
                _serializer.TrySerialize(feedbackInput.FeedbackData, out tempData);
                serialization.Add("feedback_data", tempData);
            }

            serialized = new fsData(serialization);

            return fsResult.Success;
        }
    }
    #endregion
}
