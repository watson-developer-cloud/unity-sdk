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
        private const string ServiceId = "TradeoffAnalyticsV1";
        private fsSerializer _serializer = new fsSerializer();
        private Credentials _credentials = null;
        private string _url = "https://gateway.watsonplatform.net/tone-analyzer/api";
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets and sets the endpoint URL for the service.
        /// </summary>
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>
        /// Gets and sets the credentials of the service. Replace the default endpoint if endpoint is defined.
        /// </summary>
        public Credentials Credentials
        {
            get { return _credentials; }
            set
            {
                _credentials = value;
                if (!string.IsNullOrEmpty(_credentials.Url))
                {
                    _url = _credentials.Url;
                }
            }
        }
        #endregion

        #region Constructor
        public TradeoffAnalytics(Credentials credentials)
        {
            if (credentials.HasCredentials() || credentials.HasAuthorizationToken())
            {
                Credentials = credentials;
            }
            else
            {
                throw new WatsonException("Please provide a username and password or authorization token to use the Tradeoff Analytics service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }
        #endregion

        #region Dilemmas
        private const string DillemaEndpoint = "/v1/dilemmas";
        /// <summary>
        /// The On Dilemma callback delegate.
        /// </summary>
        /// <param name="resp"></param>
        public delegate void OnDilemma(RESTConnector.ParsedResponse<DilemmasResponse> resp);

        public bool GetDilemma(OnDilemma callback, Problem problem, Boolean generateVisualization, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, DillemaEndpoint);
            if (connector == null)
                return false;

            GetDilemmaRequest req = new GetDilemmaRequest();
            req.Callback = callback;
            req.OnResponse = GetDilemmaResponse;
            req.Data = customData;
            req.Parameters["generate_visualization"] = generateVisualization.ToString();

            fsData tempData = null;
            _serializer.TrySerialize<Problem>(problem, out tempData);

            req.Send = Encoding.UTF8.GetBytes(tempData.ToString());
            req.Headers["Content-Type"] = "application/json";

            return connector.Send(req);
        }

        private class GetDilemmaRequest : RESTConnector.Request
        {
            public OnDilemma Callback { get; set; }
            public string Data { get; set; }
        };

        private void GetDilemmaResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetDilemmaRequest)req).Data;

            RESTConnector.ParsedResponse<DilemmasResponse> parsedResp = new RESTConnector.ParsedResponse<DilemmasResponse>(resp, customData, _serializer);

            if (((GetDilemmaRequest)req).Callback != null)
                ((GetDilemmaRequest)req).Callback(parsedResp);
        }

        #endregion

        #region IWatsonService interface
        /// <exclude />
        public string GetServiceID()
        {
            return ServiceId;
        }
        #endregion
    }
}
