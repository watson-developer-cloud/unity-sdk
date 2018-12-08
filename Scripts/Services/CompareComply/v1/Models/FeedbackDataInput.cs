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
    /// Feedback data for submission.
    /// </summary>
    [fsObject(Converter = typeof(FeedbackDataInputConverter))]
    public class FeedbackDataInput
    {
        /// <summary>
        /// An optional identifier of the user submitting feedback.
        /// </summary>
        [fsProperty("user_id")]
        public string UserId { get; set; }
        /// <summary>
        /// An optional comment about the feedback.
        /// </summary>
        [fsProperty("comment")]
        public string Comment { get; set; }
        /// <summary>
        /// The type of feedback. The only permitted value is `element_classification`.
        /// </summary>
        [fsProperty("feedback_type")]
        public string FeedbackType { get; set; }
        /// <summary>
        /// Brief information about the input document.
        /// </summary>
        [fsProperty("document")]
        public ShortDoc Document { get; set; }
        /// <summary>
        /// An optional string identifying the model ID. The only permitted value is `contracts`.
        /// </summary>
        [fsProperty("model_id")]
        public string ModelId { get; set; }
        /// <summary>
        /// An optional string identifying the version of the model used.
        /// </summary>
        [fsProperty("model_version")]
        public string ModelVersion { get; set; }
        /// <summary>
        /// The numeric location of the identified element in the document, represented with two integers labeled
        /// `begin` and `end`.
        /// </summary>
        [fsProperty("location")]
        public Location Location { get; set; }
        /// <summary>
        /// The text on which to submit feedback.
        /// </summary>
        [fsProperty("text")]
        public string Text { get; set; }
        /// <summary>
        /// The original labeling from the input document, without the submitted feedback.
        /// </summary>
        [fsProperty("original_labels")]
        public OriginalLabelsIn OriginalLabels { get; set; }
        /// <summary>
        /// The updated labeling from the input document, accounting for the submitted feedback.
        /// </summary>
        [fsProperty("updated_labels")]
        public UpdatedLabelsIn UpdatedLabels { get; set; }
    }

    #region FeedbackDataInput Converter
    public class FeedbackDataInputConverter : fsConverter
    {
        private fsSerializer _serializer = new fsSerializer();

        public override bool CanProcess(Type type)
        {
            return type == typeof(FeedbackDataInput);
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
            FeedbackDataInput feedbackDataInput = (FeedbackDataInput)instance;
            serialized = null;

            Dictionary<string, fsData> serialization = new Dictionary<string, fsData>();

            fsData tempData = null;

            if (!string.IsNullOrEmpty(feedbackDataInput.UserId))
            {
                _serializer.TrySerialize(feedbackDataInput.UserId, out tempData);
                serialization.Add("user_id", tempData);
            }

            if (!string.IsNullOrEmpty(feedbackDataInput.Comment))
            {
                _serializer.TrySerialize(feedbackDataInput.Comment, out tempData);
                serialization.Add("comment", tempData);
            }

            if (!string.IsNullOrEmpty(feedbackDataInput.FeedbackType))
            {
                _serializer.TrySerialize(feedbackDataInput.FeedbackType, out tempData);
                serialization.Add("feedback_type", tempData);
            }

            if (feedbackDataInput.Document != null)
            {
                _serializer.TrySerialize(feedbackDataInput.Document, out tempData);
                serialization.Add("document", tempData);
            }

            if (!string.IsNullOrEmpty(feedbackDataInput.ModelId))
            {
                _serializer.TrySerialize(feedbackDataInput.ModelId, out tempData);
                serialization.Add("model_id", tempData);
            }

            if (!string.IsNullOrEmpty(feedbackDataInput.ModelVersion))
            {
                _serializer.TrySerialize(feedbackDataInput.ModelVersion, out tempData);
                serialization.Add("model_version", tempData);
            }

            if (feedbackDataInput.Location != null)
            {
                _serializer.TrySerialize(feedbackDataInput.Location, out tempData);
                serialization.Add("location", tempData);
            }

            if (!string.IsNullOrEmpty(feedbackDataInput.Text))
            {
                _serializer.TrySerialize(feedbackDataInput.Text, out tempData);
                serialization.Add("text", tempData);
            }

            if (feedbackDataInput.OriginalLabels != null)
            {
                _serializer.TrySerialize(feedbackDataInput.OriginalLabels, out tempData);
                serialization.Add("original_labels", tempData);
            }

            if (feedbackDataInput.UpdatedLabels != null)
            {
                _serializer.TrySerialize(feedbackDataInput.UpdatedLabels, out tempData);
                serialization.Add("updated_labels", tempData);
            }

            serialized = new fsData(serialization);

            return fsResult.Success;
        }
    }
    #endregion
}
