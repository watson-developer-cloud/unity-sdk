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

using FullSerializer;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.Services.PersonalityInsights.v3
{
    /// <summary>
    /// This class wraps the Personality Insights service.
    /// <a href="http://www.ibm.com/watson/developercloud/personality-insights.html">Personality Insights Service</a>
    /// </summary>
    public class PersonalityInsights : IWatsonService
    {
        #region Private Data
        private const string ServiceId = "PersonalityInsightsV3";
        private fsSerializer _serializer = new fsSerializer();
        private Credentials _credentials = null;
        private string _url = "https://gateway.watsonplatform.net/personality-insights/api";
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
        public PersonalityInsights(Credentials credentials)
        {
            if (credentials.HasCredentials() || credentials.HasAuthorizationToken())
            {
                Credentials = credentials;
            }
            else
            {
                throw new WatsonException("Please provide a username and password or authorization token to use the Personality Insights service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
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

        #region Profile
        private const string ProfileEndpoint = "/v3/profile";

        public bool GetProfile(SuccessCallback<Profile> successCallback, 
            FailCallback failCallback, 
            string source,
            string contentType = ContentType.TextPlain,
            string contentLanguage = ContentLanguage.English,
            string accept = ContentType.ApplicationJson,
            string acceptLanguage = AcceptLanguage.English,
            bool raw_scores = false,
            bool csv_headers = false,
            bool consumption_preferences = false,
            string version = PersonalityInsightsVersion.Version,
            Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("A JSON or Text source is required for GetProfile!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, ProfileEndpoint);
            if (connector == null)
                return false;

            GetProfileRequest req = new GetProfileRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.OnResponse = GetProfileResponse;

            req.Parameters["raw_scores"] = raw_scores.ToString();
            req.Parameters["csv_headers"] = csv_headers.ToString();
            req.Parameters["consumption_preferences"] = consumption_preferences.ToString();
            req.Parameters["version"] = version;

            req.Headers["Content-Type"] = contentType;
            req.Headers["Content-Language"] = contentLanguage;
            req.Headers["Accept"] = accept;
            req.Headers["Accept-Language"] = acceptLanguage;

            if (source.StartsWith(Application.dataPath))
            {
                string jsonData = default(string);
                jsonData = File.ReadAllText(source);
                req.Send = System.Text.Encoding.UTF8.GetBytes(jsonData);
            }
            else
            {
                req.Send = System.Text.Encoding.UTF8.GetBytes(source);
            }

            return connector.Send(req);
        }

        /// <summary>
        /// Get profile request.
        /// </summary>
        public class GetProfileRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Profile> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void GetProfileResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Profile result = new Profile();
            fsData data = null;
            Dictionary<string, object> customData = ((GetProfileRequest)req).CustomData;

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
                    Log.Error("PersonalityInsights.GetProfileResponse()", "GetProfileResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetProfileRequest)req).SuccessCallback != null)
                    ((GetProfileRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetProfileRequest)req).FailCallback != null)
                    ((GetProfileRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region IWatsonService implementation
        public string GetServiceID()
        {
            return ServiceId;
        }
        #endregion
    }
}