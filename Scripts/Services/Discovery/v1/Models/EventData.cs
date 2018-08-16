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

namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    /// <summary>
    /// Query event data object.
    /// </summary>
    [fsObject(Converter = typeof(EventDataConverter))]
    public class EventData
    {
        /// <summary>
        /// The **environment_id** associated with the query that the event is associated with.
        /// </summary>
        [fsProperty("environment_id")]
        public string EnvironmentId { get; set; }
        /// <summary>
        /// The session token that was returned as part of the query results that this event is associated with.
        /// </summary>
        [fsProperty("session_token")]
        public string SessionToken { get; set; }
        /// <summary>
        /// The optional timestamp for the event that was created. If not provided, the time that the event was created
        /// in the log was used.
        /// </summary>
        [fsProperty("client_timestamp")]
        public DateTime? ClientTimestamp { get; set; }
        /// <summary>
        /// The rank of the result item which the event is associated with.
        /// </summary>
        [fsProperty("display_rank")]
        public long? DisplayRank { get; set; }
        /// <summary>
        /// The **collection_id** of the document that this event is associated with.
        /// </summary>
        [fsProperty("collection_id")]
        public string CollectionId { get; set; }
        /// <summary>
        /// The **document_id** of the document that this event is associated with.
        /// </summary>
        [fsProperty("document_id")]
        public string DocumentId { get; set; }
        /// <summary>
        /// The query identifier stored in the log. The query and any events associated with that query are stored with
        /// the same **query_id**.
        /// </summary>
        [fsProperty("query_id")]
        public virtual string QueryId { get; private set; }
    }
    public class EventDataConverter : fsConverter
    {
        private static fsSerializer sm_Serializer = new fsSerializer();

        public override bool CanProcess(Type type)
        {
            return type == typeof(EventData);
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
            return new EventData();
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
            EventData eventData = (EventData)instance;
            serialized = null;

            Dictionary<string, fsData> serialization = new Dictionary<string, fsData>();

            fsData tempData = null;

            if (eventData.EnvironmentId != null)
            {
                sm_Serializer.TrySerialize(eventData.EnvironmentId, out tempData);
                serialization.Add("environment_id", tempData);
            }

            if (eventData.SessionToken != null)
            {
                sm_Serializer.TrySerialize(eventData.SessionToken, out tempData);
                serialization.Add("session_token", tempData);
            }

            if (eventData.ClientTimestamp != null)
            {
                sm_Serializer.TrySerialize(eventData.ClientTimestamp, out tempData);
                serialization.Add("client_timestamp", tempData);
            }

            if (eventData.DisplayRank != null)
            {
                sm_Serializer.TrySerialize(eventData.DisplayRank, out tempData);
                serialization.Add("display_rank", tempData);
            }

            if (eventData.CollectionId != null)
            {
                sm_Serializer.TrySerialize(eventData.CollectionId, out tempData);
                serialization.Add("collection_id", tempData);
            }

            if (eventData.DocumentId != null)
            {
                sm_Serializer.TrySerialize(eventData.DocumentId, out tempData);
                serialization.Add("document_id", tempData);
            }

            if (eventData.QueryId != null)
            {
                sm_Serializer.TrySerialize(eventData.QueryId, out tempData);
                serialization.Add("query_id", tempData);
            }

            serialized = new fsData(serialization);

            return fsResult.Success;
        }
    }
}
