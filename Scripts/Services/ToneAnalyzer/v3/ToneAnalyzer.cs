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

using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using System.Text;
using MiniJSON;
using System;
using FullSerializer;

namespace IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v3
{
    /// <summary>
    /// This class wraps the Tone Analyzer service.
    /// <a href="http://www.ibm.com/watson/developercloud/tone-analyzer.html">Tone Analyzer Service</a>
    /// </summary>
    public class ToneAnalyzer : IWatsonService
    {
        #region Private Data
        private const string ServiceId = "ToneAnalyzerV3";
        private fsSerializer _serializer = new fsSerializer();
        private Credentials _credentials = null;
        private string _url = "https://gateway.watsonplatform.net/tone-analyzer/api";
        private string _versionDate;
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
        /// Gets and sets the versionDate of the service.
        /// </summary>
        public string VersionDate
        {
            get
            {
                if (string.IsNullOrEmpty(_versionDate))
                    throw new ArgumentNullException("VersionDate cannot be null. Use a VersionDate formatted as `YYYY-MM-DD`");

                return _versionDate;
            }
            set { _versionDate = value; }
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
        public ToneAnalyzer(Credentials credentials)
        {
            if (credentials.HasCredentials() || credentials.HasAuthorizationToken())
            {
                Credentials = credentials;
            }
            else
            {
                throw new WatsonException("Please provide a username and password or authorization token to use the Tone Analyzer service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
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

        #region Get Tone
        private const string ToneEndpoint = "/v3/tone";
        /// <summary>
        /// Gets the tone analyze.
        /// </summary>
        /// <returns><c>true</c>, if tone analyze was gotten, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="text">Text.</param>
        /// <param name="data">Data.</param>
        public bool GetToneAnalyze(SuccessCallback<ToneAnalyzerResponse> successCallback, FailCallback failCallback, string text, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, ToneEndpoint);
            if (connector == null)
                return false;

            GetToneAnalyzerRequest req = new GetToneAnalyzerRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.OnResponse = GetToneAnalyzerResponse;

            Dictionary<string, string> upload = new Dictionary<string, string>();
            upload["text"] = "\"" + text + "\"";
            req.Send = Encoding.UTF8.GetBytes(Json.Serialize(upload));
            req.Headers["Content-Type"] = "application/json";
            req.Parameters["version"] = VersionDate;
            req.Parameters["sentences"] = "true";
            return connector.Send(req);
        }

        private class GetToneAnalyzerRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<ToneAnalyzerResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        };

        private void GetToneAnalyzerResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ToneAnalyzerResponse result = new ToneAnalyzerResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((GetToneAnalyzerRequest)req).CustomData;

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
                    Log.Error("ToneAnalyzer.GetToneAnalyzerResponse()", "GetToneAnalyzerResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetToneAnalyzerRequest)req).SuccessCallback != null)
                    ((GetToneAnalyzerRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetToneAnalyzerRequest)req).FailCallback != null)
                    ((GetToneAnalyzerRequest)req).FailCallback(resp.Error, customData);
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

