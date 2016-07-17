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
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using System.Text;
using MiniJSON;
using System;
using FullSerializer;

namespace IBM.Watson.DeveloperCloud.Services.TradeoffAnalytics.v1
{
	/// <summary>
	/// This class wraps the TradeOff Analytics service.
	/// <a href="http://www.ibm.com/watson/developercloud/tradeoff-analytics.html">TradeOff Analytics Service</a>
	/// </summary>
	public class TradeoffAnalytics : IWatsonService
    {

        #region Private Data
        private const string SERVICE_ID = "TradeoffAnalyticsV1";
        private static fsSerializer sm_Serializer = new fsSerializer();
        #endregion

        #region Dilemmas
        private const string FUNCTION_DILEMMA = "/v1/dilemmas";
		/// <summary>
		/// The On Dilemma callback delegate.
		/// </summary>
		/// <param name="resp"></param>
        public delegate void OnDilemma( DilemmasResponse resp );

        public bool GetDilemma(OnDilemma callback, Problem problem, Boolean generateVisualization)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, FUNCTION_DILEMMA);
            if (connector == null)
                return false;

            GetDilemmaRequest req = new GetDilemmaRequest();
            req.Callback = callback;
            req.OnResponse = GetDilemmaResponse;
            req.Parameters["generate_visualization"] = generateVisualization.ToString();

            fsData tempData = null;
            sm_Serializer.TrySerialize<Problem>(problem, out tempData);

            Log.Status("GetDilemma", "JSON: {0}", tempData.ToString());

            req.Send = Encoding.UTF8.GetBytes(tempData.ToString());
            req.Headers["Content-Type"] = "application/json";

            return connector.Send(req);
        }

        private class GetDilemmaRequest : RESTConnector.Request
        {
            public OnDilemma Callback { get; set; }
        };

        private void GetDilemmaResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DilemmasResponse response = new DilemmasResponse();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = response;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("TradeoffAnalytics", "GetDilemmaResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetDilemmaRequest)req).Callback != null)
                ((GetDilemmaRequest)req).Callback(resp.Success ? response : null);
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
            if ( Utilities.Config.Instance.FindCredentials( SERVICE_ID ) != null )
                new CheckServiceStatus( this, callback );
            else
                callback( SERVICE_ID, false );
        }

        /// <summary>
        /// Application data value.
        /// </summary>
        [fsObject]
        public class PingDataValue : IBM.Watson.DeveloperCloud.Services.TradeoffAnalytics.v1.ApplicationDataValue
        {
            public double ping { get; set; }
        }

        /// <summary>
        /// Application data.
        /// </summary>
        [fsObject]
        public class PingData : IBM.Watson.DeveloperCloud.Services.TradeoffAnalytics.v1.ApplicationData
        {

        }

        private class CheckServiceStatus
        {
            private TradeoffAnalytics m_Service = null;
            private ServiceStatus m_Callback = null;

            public CheckServiceStatus(TradeoffAnalytics service, ServiceStatus callback)
            {
                m_Service = service;
                m_Callback = callback;

                Problem problem = new Problem();
                problem.subject = "ping";

                List<Column> listColumn = new List<Column>();
                Column pingColumn = new Column();
                pingColumn.key = "ping";
                pingColumn.type = "numeric";
                pingColumn.goal = "min";
                pingColumn.is_objective = true;
                pingColumn.range = new ValueRange();
                ((ValueRange)pingColumn.range).high = 1;
                ((ValueRange)pingColumn.range).low = 0;
                listColumn.Add(pingColumn);

                problem.columns = listColumn.ToArray();

                List<Option> listOption = new List<Option>();
                Option pingOption = new Option();
                pingOption.key = "ping";
                pingOption.values = new PingDataValue();
                (pingOption.values as PingDataValue).ping = 0;
                listOption.Add(pingOption);

                problem.options = listOption.ToArray();

                if(!m_Service.GetDilemma(OnGetDilemma, problem, false))
                    OnFailure("Failed to invoke OnGetDilemma().");
            }

            private void OnGetDilemma(DilemmasResponse resp)
            {
                if (m_Callback != null && resp != null)
                {
                    m_Callback(SERVICE_ID, true);
                }
                else
                {
                    OnFailure("DillemaResponse is null");
                }
            }

            private void OnFailure(string msg)
            {
                Log.Error("TradeoffAnalytics", msg);
                m_Callback(SERVICE_ID, false);
            }
        }
        
        #endregion
    }
}
