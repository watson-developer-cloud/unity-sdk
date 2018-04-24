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
using System;
using System.Collections.Generic;
using System.Text;

namespace IBM.Watson.DeveloperCloud.Utilities
{
    /// <summary>
    /// Helper class for holding a user and password or authorization token, used by both the WSCOnnector and RESTConnector.
    /// </summary>
    public class Credentials
    {
        #region Private Data
        private TokenManager _tokenManager;
        TokenManager.SuccessCallback<IamTokenData> _successCallback;
        #endregion
        /// <summary>
        /// The user name.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// The password.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// The Api Key.
        /// </summary>
        public string ApiKey { get; set; }
        
        /// <summary>
        /// The Watson authentication token
        /// </summary>
        public string WatsonAuthenticationToken
        {
            get { return _watsonAuthenticationToken; }
            set { _watsonAuthenticationToken = value; }
        }
        private string _watsonAuthenticationToken;
        /// <summary>
        /// The service endpoint.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The IAM access token.
        /// </summary>
        public string IamAccessToken { get; set; }

        /// <summary>
        /// IAM token data.
        /// </summary>
        public IamTokenData TokenData
        {
            set
            {
                _tokenData = value;
                if (!string.IsNullOrEmpty(_tokenData.AccessToken))
                    IamAccessToken = _tokenData.AccessToken;
            }
        }
        private IamTokenData _tokenData = null;
        
        /// <summary>
        /// Constructor that takes the URL. Used for token authentication.
        /// </summary>
        public Credentials(string url = null)
        {
            Url = url;
        }

        /// <summary>
        /// Constructor that takes the user name and password.
        /// </summary>
        /// <param name="username">The string containing the user name.</param>
        /// <param name="password">A string containing the password.</param>
        /// <param name="url">The service endpoint.</param>
        public Credentials(string username, string password, string url = null)
        {
            Username = username;
            Password = password;
            Url = url;
        }

        /// <summary>
        /// Constructor that takes an authentication token created by the user or an ApiKey. 
        /// </summary>
        /// <param name="url">The service endpoint.</param>
        public Credentials(string apiKey, string url = null)
        {
            ApiKey = apiKey;
            Url = url;
        }

        /// <summary>
        /// Constructor that takes IAM token options.
        /// </summary>
        /// <param name="IamTokenOptions"></param>
        public Credentials(TokenOptions IamTokenOptions, string serviceUrl, TokenManager.SuccessCallback<IamTokenData> successCallback, TokenManager.FailCallback failCallback, Dictionary<string, object> customData = null)
        {
            _successCallback = successCallback;
            Url = serviceUrl;
            _tokenManager = new TokenManager(IamTokenOptions);
            _tokenManager.GetToken(OnGetToken, failCallback, customData);
        }

        private void OnGetToken(IamTokenData iamTokenData, Dictionary<string, object> customData)
        {
            TokenData = iamTokenData;
            _successCallback(iamTokenData, customData);
        }

        private void OnGetTokenFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Debug("Credentials.OnGetTokenFail();", "Failed to get IAM Token: {0}", error.ToString());
        }

        /// <summary>
        /// Create basic authentication header data for REST requests.
        /// </summary>
        /// <returns>The authentication data base64 encoded.</returns>
        public string CreateAuthorization()
        {
            return "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(Username + ":" + Password));
        }

        /// <summary>
        /// Do we have credentials?
        /// </summary>
        /// <returns>true if the class has a username and password.</returns>
        public bool HasCredentials()
        {
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
        }

        /// <summary>
        /// Do we have an authentication token?
        /// </summary>
        /// <returns>True if the class has a Authentication Token</returns>
        public bool HasWatsonAuthenticationToken()
        {
            return !string.IsNullOrEmpty(WatsonAuthenticationToken);
        }

        /// <summary>
        /// Do we have an ApiKey?
        /// </summary>
        /// <returns>True if the class has a Authentication Token</returns>
        public bool HasApiKey()
        {
            return !string.IsNullOrEmpty(ApiKey);
        }

        /// <summary>
        /// Do we have a HasIamTokenData?
        /// </summary>
        /// <returns></returns>
        public bool HasIamTokenData()
        {
            return _tokenData != null;
        }
    }

    [fsObject]
    public class VcapCredentials
    {
		public Dictionary<string, Credential> VCAP_SERVICES { get; set; }
    }

    [fsObject]
    public class VcapCredential
    {
        [fsProperty("name")]
        public string Name { get; set; }
        [fsProperty("label")]
        public string Label { get; set; }
        [fsProperty("plan")]
        public string Plan { get; set; }
        [fsProperty("credentials")]
        public Credential Credentials { get; set; }
    }

    [fsObject]
    public class Credential
    {
        [fsProperty("url")]
        public string Url { get; set; }
        [fsProperty("username")]
        public string Username { get; set; }
        [fsProperty("password")]
        public string Password { get; set; }
        [fsProperty("workspace_id")]
        public string WorkspaceId { get; set; }
        [fsProperty("api_key")]
        public string Apikey { get; set; }
        [fsProperty("note")]
        public string Note { get; set; }
    }
}
