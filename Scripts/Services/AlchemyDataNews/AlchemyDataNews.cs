/**
* Copyright 2015 IBM Corp. All Rights Reserved.
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

using UnityEngine;
using System.Collections;
using FullSerializer;
using IBM.Watson.DeveloperCloud.Utilities;
using System;
using IBM.Watson.DeveloperCloud.Connection;
using System.Text;
using IBM.Watson.DeveloperCloud.Logging;
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.Services.AlchemyDataNews.v1
{
    public class AlchemyDataNews : IWatsonService
    {
        #region Private Data
        private const string SERVICE_ID = "AlchemyAPIV1";
        private static string mp_ApiKey = null;

        private static fsSerializer sm_Serializer = new fsSerializer();
        #endregion

        #region SetCredentials
        private void SetCredentials()
        {
            mp_ApiKey = Config.Instance.GetAPIKey(SERVICE_ID);

            if (string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("Alchemy API Key required in config.json");
        }
        #endregion

        #region GetNews
        private const string SERVICE_GET_NEWS = "/data/GetNews";

        /// <summary>
        /// On get news delegate.
        /// </summary>
        public delegate void OnGetNews(NewsResponse newsData, string data);

        /// <summary>
        /// Gets news.
        /// </summary>
        /// <returns><c>true</c>, if news was gotten, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="returnFields">Fields returned.</param>
        /// <param name="queryFields">Values for each field.</param>
        /// <param name="startDate">Date to start the query.</param>
        /// <param name="endDate">Date to end the query.</param>
        /// <param name="maxResults">Maximum number of results.</param>
        /// <param name="timeSlice">the duration (in seconds) of each time slice. a human readable duration is also acceptable e.g. '1d', '4h', '1M', etc.
        /// If set, this parameter causes the query engine to return a time series representing the count in each slice of time. If omitted, the query engine returns the total count over the time duration.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetNews(OnGetNews callback,
            string[] returnFields = default(string[]),
            Dictionary<string, string> queryFields = null,
            string startDate = "now-1d",
            string endDate = "now",
            int maxResults = 10,
            string timeSlice = default(string),
            string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetNewsRequest req = new GetNewsRequest();
            req.Callback = callback;
            req.Data = string.IsNullOrEmpty(customData) ? returnFields.ToString() : customData;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if(returnFields.Length > 0)
                req.Forms["return"] = new RESTConnector.Form(string.Join(",", returnFields));
            if(queryFields.Count > 0)
                foreach (KeyValuePair<string, string> entry in queryFields)
                    req.Forms[entry.Key] = new RESTConnector.Form(entry.Value);
            if(!string.IsNullOrEmpty(startDate))
                req.Forms["start"] = new RESTConnector.Form(startDate);
            if (!string.IsNullOrEmpty(endDate))
                req.Forms["end"] = new RESTConnector.Form(endDate);
            req.Forms["maxResults"] = new RESTConnector.Form(maxResults);
            if (!string.IsNullOrEmpty(timeSlice))
                req.Forms["timeSlice"] = new RESTConnector.Form(timeSlice);

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_GET_NEWS);
            if (connector == null)
                return false;

            req.OnResponse = OnGetNewsResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Get News request.
        /// </summary>
        public class GetNewsRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetNews Callback { get; set; }
        }

        private void OnGetNewsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            NewsResponse newsData = new NewsResponse();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = newsData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyDataNews", "OnGetNewsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetNewsRequest)req).Callback != null)
                ((GetNewsRequest)req).Callback(resp.Success ? newsData : null, ((GetNewsRequest)req).Data);
        }
        #endregion

        #region IWatsonService interface
        /// <exclude />
        public string GetServiceID()
        {
            return SERVICE_ID;
        }

        /// <exclude />
        public void GetServiceStatus(ServiceStatus callback)
        {
            if (Config.Instance.FindCredentials(SERVICE_ID) != null)
                new CheckServiceStatus(this, callback);
            else
                callback(SERVICE_ID, false);
        }

        private class CheckServiceStatus
        {
            private AlchemyDataNews m_Service = null;
            private ServiceStatus m_Callback = null;

            public CheckServiceStatus(AlchemyDataNews service, ServiceStatus callback)
            {
                m_Service = service;
                m_Callback = callback;

                if (!m_Service.GetNews(OnGetNews))
                    m_Callback(SERVICE_ID, false);
            }

            void OnGetNews(NewsResponse newsData, string data)
            {
                if (m_Callback != null)
                    m_Callback(SERVICE_ID, newsData != null);
            }

        };
        #endregion
    }
}
