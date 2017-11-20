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

        #region Callback delegates
        /// <summary>
        /// Success callback delegate.
        /// </summary>
        /// <typeparam name="T">Type of the returned object.</typeparam>
        /// <param name="response">The returned object.</param>
        /// <param name="customData">user defined custom data including raw json.</param>
        public delegate void SuccessCallback<T>(T response, Dictionary<string, object> customData);
        /// <summary>
        /// Fail callback delegate.
        /// </summary>
        /// <param name="error">The error object.</param>
        /// <param name="customData">User defined custom data</param>
        public delegate void FailCallback(RESTConnector.Error error, Dictionary<string, object> customData);
        #endregion

        #region Dilemmas
        private const string DillemaEndpoint = "/v1/dilemmas";
        /// <summary>
        /// Returns a dilemma that contains the problem and a resolution. The problem contains a set of options and objectives. The resolution 
        /// contains a set of optimal options, their analytical characteristics, and by default their representation on a two-dimensional space. 
        /// You can optionally request that the service also return a refined set of preferable options that are most likely to appeal to the 
        /// greatest number of users
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="problem">The decision problem.</param>
        /// <param name="generateVisualization">Indicates whether to calculate the map visualization. If true, the visualization is returned if 
        /// the is_objective field is true for at least three columns and at least three options have a status of FRONT in the problem resolution.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool GetDilemma(SuccessCallback<DilemmasResponse> successCallback, FailCallback failCallback, Problem problem, Boolean generateVisualization, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, DillemaEndpoint);
            if (connector == null)
                return false;

            GetDilemmaRequest req = new GetDilemmaRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.OnResponse = GetDilemmaResponse;
            req.Parameters["generate_visualization"] = generateVisualization.ToString();

            fsData tempData = null;
            _serializer.TrySerialize<Problem>(problem, out tempData);

            req.Send = Encoding.UTF8.GetBytes(tempData.ToString());
            req.Headers["Content-Type"] = "application/json";

            return connector.Send(req);
        }

        private class GetDilemmaRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<DilemmasResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        };

        private void GetDilemmaResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DilemmasResponse result = new DilemmasResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((GetDilemmaRequest)req).CustomData;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = result;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("TradeoffAnalytics.GetDilemmaResponse()", "GetDilemmaResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetDilemmaRequest)req).SuccessCallback != null)
                    ((GetDilemmaRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetDilemmaRequest)req).FailCallback != null)
                    ((GetDilemmaRequest)req).FailCallback(resp.Error, customData);
            }
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
