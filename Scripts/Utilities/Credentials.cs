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
using UnityEngine.Networking;

namespace IBM.Watson.DeveloperCloud.Utilities
{
    /// <summary>
    /// Helper class for holding a user and password or authorization token, used by both the WSCOnnector and RESTConnector.
    /// </summary>
    public class Credentials
    {
        #region Private Data
        private fsSerializer _serializer = new fsSerializer();
        private string _iamUrl;
        private IamTokenData _iamTokenData;
        private string _iamApiKey;
        private string _userAcessToken;
        private string url;
        private string username;
        private string password;
        private const string APIKEY_AS_USERNAME = "apikey";
        private const string ICP_PREFIX = "icp-";
        #endregion

        #region Public Fields
        /// <summary>
        /// The user name.
        /// </summary>
        public string Username
        {
            get { return username; }
            set
            {
                if (!Utility.HasBadFirstOrLastCharacter(value))
                {
                    username = value;
                }
                else
                {
                    throw new WatsonException("The username shouldn't start or end with curly brackets or quotes. Be sure to remove any {} and \" characters surrounding your username.");
                }
            }
        }
        /// <summary>
        /// The password.
        /// </summary>
        public string Password
        {
            get { return password; }
            set
            {
                if (!Utility.HasBadFirstOrLastCharacter(value))
                {
                    password = value;
                }
                else
                {
                    throw new WatsonException("The password shouldn't start or end with curly brackets or quotes. Be sure to remove any {} and \" characters surrounding your password.");
                }
            }
        }
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
        public string Url
        {
            get { return url; }
            set
            {
                if (!Utility.HasBadFirstOrLastCharacter(value))
                {
                    url = value;
                }
                else
                {
                    throw new WatsonException("The service URL shouldn't start or end with curly brackets or quotes. Be sure to remove any {} and \" characters surrounding your service url.");
                }
            }
        }

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
        private bool disableSslVerification = false;
        /// <summary>
        /// Gets and sets the option to disable ssl verification for getting an IAM token.
        /// </summary>
        public bool DisableSslVerification
        {
            get { return disableSslVerification; }
            set { disableSslVerification = value; }
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

        #region Constructors
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
            SetCredentials(username, password, url);
        }

        /// <summary>
        /// Constructor that takes an authentication token created by the user or an ApiKey.
        /// If no URL is set then default to the non-IAM Visual Recognition endpoint.
        /// </summary>
        /// <param name="url">The service endpoint.</param>
        [Obsolete("Authentication using legacy apikey is deprecated. Please authenticate using TokenOptions.")]
        public Credentials(string apiKey, string url = null)
        {
            ApiKey = apiKey;
            Url = !string.IsNullOrEmpty(url) ? url : "https://gateway-a.watsonplatform.net/visual-recognition/api";
        }

        /// <summary>
        /// Constructor that takes IAM token options.
        /// </summary>
        /// <param name="iamTokenOptions"></param>
        public Credentials(TokenOptions iamTokenOptions, string serviceUrl = null)
        {
            SetCredentials(iamTokenOptions, serviceUrl);
        }
        #endregion

        #region SetCredentials
        private void SetCredentials(string username, string password, string url = null)
        {
            if (username == APIKEY_AS_USERNAME && !password.StartsWith(ICP_PREFIX))
            {
                TokenOptions tokenOptions = new TokenOptions()
                {
                    IamApiKey = password
                };

                SetCredentials(tokenOptions, url);
            }
            else
            {
                Username = username;
                Password = password;
            }

            if (!string.IsNullOrEmpty(url))
                Url = url;
        }

        private void SetCredentials(TokenOptions iamTokenOptions, string serviceUrl = null)
        {
            if (iamTokenOptions.IamApiKey.StartsWith(ICP_PREFIX))
            {
                SetCredentials(APIKEY_AS_USERNAME, iamTokenOptions.IamApiKey, serviceUrl);
            }
            else
            {
                if (!string.IsNullOrEmpty(serviceUrl))
                    Url = serviceUrl;
                _iamUrl = !string.IsNullOrEmpty(iamTokenOptions.IamUrl) ? iamTokenOptions.IamUrl : "https://iam.bluemix.net/identity/token";
                _iamTokenData = new IamTokenData();

                if (!string.IsNullOrEmpty(iamTokenOptions.IamApiKey))
                    _iamApiKey = iamTokenOptions.IamApiKey;

                if (!string.IsNullOrEmpty(iamTokenOptions.IamAccessToken))
                    this._userAcessToken = iamTokenOptions.IamAccessToken;

                GetToken();
            }
        }
        #endregion

        #region Get Token
        /// <summary>
        /// This function sends an access token back through a callback. The source of the token
        /// is determined by the following logic:
        /// 1. If user provides their own managed access token, assume it is valid and send it
        /// 2. If this class is managing tokens and does not yet have one, make a request for one
        /// 3. If this class is managing tokens and the token has expired, refresh it
        /// 4. If this class is managing tokens and has a valid token stored, send it
        /// </summary>
        public void GetToken()
        {
            if (!string.IsNullOrEmpty(_userAcessToken))
            {
                // 1. use user-managed token
                OnGetToken(new IamTokenData() { AccessToken = _userAcessToken }, new Dictionary<string, object>());
            }
            else if (!string.IsNullOrEmpty(_iamTokenData.AccessToken) || IsRefreshTokenExpired())
            {
                // 2. request an initial token
                RequestIamToken(OnGetToken, OnGetTokenFail);
            }
            else if (IsTokenExpired())
            {
                // 3. refresh a token
                RefreshIamToken(OnGetToken, OnGetTokenFail);
            }
            else
            {
                //  4. use valid managed token
                OnGetToken(new IamTokenData() { AccessToken = _iamTokenData.AccessToken }, new Dictionary<string, object>());
            }
        }

        private void OnGetToken(IamTokenData iamTokenData, Dictionary<string, object> customData)
        {
            SaveTokenInfo(iamTokenData);
        }

        private void OnGetTokenFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Debug("Credentials.OnGetTokenFail();", "Failed to get IAM Token: {0}", error.ToString());
        }
        #endregion

        #region Request Token
        /// <summary>
        /// Request an IAM token using an API key.
        /// </summary>
        /// <param name="successCallback">The request callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customData">Dictionary of custom data.</param>
        /// <returns></returns>
        public bool RequestIamToken(SuccessCallback<IamTokenData> successCallback, FailCallback failCallback, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            RESTConnector connector = new RESTConnector();
            connector.URL = _iamUrl;
            if (connector == null)
                return false;

            RequestIamTokenRequest req = new RequestIamTokenRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.HttpMethod = UnityWebRequest.kHttpVerbGET;
            req.Headers.Add("Content-type", "application/x-www-form-urlencoded");
            req.Headers.Add("Authorization", "Basic Yng6Yng=");
            req.OnResponse = OnRequestIamTokenResponse;
            req.DisableSslVerification = DisableSslVerification;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["grant_type"] = new RESTConnector.Form("urn:ibm:params:oauth:grant-type:apikey");
            req.Forms["apikey"] = new RESTConnector.Form(_iamApiKey);
            req.Forms["response_type"] = new RESTConnector.Form("cloud_iam");

            return connector.Send(req);
        }

        private class RequestIamTokenRequest : RESTConnector.Request
        {
            public Dictionary<string, object> CustomData { get; set; }
            public SuccessCallback<IamTokenData> SuccessCallback { get; set; }
            public FailCallback FailCallback { get; set; }
        }

        private void OnRequestIamTokenResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            IamTokenData result = new IamTokenData();
            fsData data = null;
            Dictionary<string, object> customData = ((RequestIamTokenRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

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
                    Log.Error("TokenManager.OnRequestTokenResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((RequestIamTokenRequest)req).SuccessCallback != null)
                    ((RequestIamTokenRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((RequestIamTokenRequest)req).FailCallback != null)
                    ((RequestIamTokenRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Refresh Token
        /// <summary>
        /// Refresh an IAM token using a refresh token.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customData">Dictionary of custom data.</param>
        /// <returns></returns>
        public bool RefreshIamToken(SuccessCallback<IamTokenData> successCallback, FailCallback failCallback, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            RESTConnector connector = new RESTConnector();
            connector.URL = _iamUrl;
            if (connector == null)
                return false;

            RefreshIamTokenRequest req = new RefreshIamTokenRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.HttpMethod = UnityWebRequest.kHttpVerbGET;
            req.Headers.Add("Content-type", "application/x-www-form-urlencoded");
            req.Headers.Add("Authorization", "Basic Yng6Yng=");
            req.OnResponse = OnRefreshIamTokenResponse;
            req.DisableSslVerification = DisableSslVerification;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["grant_type"] = new RESTConnector.Form("refresh_token");
            req.Forms["refresh_token"] = new RESTConnector.Form(_iamTokenData.RefreshToken);

            return connector.Send(req);
        }

        private class RefreshIamTokenRequest : RESTConnector.Request
        {
            public Dictionary<string, object> CustomData { get; set; }
            public SuccessCallback<IamTokenData> SuccessCallback { get; set; }
            public FailCallback FailCallback { get; set; }
        }

        private void OnRefreshIamTokenResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            IamTokenData result = new IamTokenData();
            fsData data = null;
            Dictionary<string, object> customData = ((RefreshIamTokenRequest)req).CustomData;
            customData.Add(Constants.String.RESPONSE_HEADERS, resp.Headers);

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
                    Log.Error("TokenManager.OnRefreshTokenResponse()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((RefreshIamTokenRequest)req).SuccessCallback != null)
                    ((RefreshIamTokenRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((RefreshIamTokenRequest)req).FailCallback != null)
                    ((RefreshIamTokenRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Token Operations
        /// <summary>
        /// Check if currently stored token is expired.
        /// 
        /// Using a buffer to prevent the edge case of the 
        /// token expiring before the request could be made.
        /// 
        /// The buffer will be a fraction of the total TTL. Using 80%.
        /// </summary>
        /// <returns></returns>
        public bool IsTokenExpired()
        {
            if (_iamTokenData.ExpiresIn == null || _iamTokenData.Expiration == null)
                return true;

            float fractionOfTtl = 0.8f;
            long? timeToLive = _iamTokenData.ExpiresIn;
            long? expireTime = _iamTokenData.Expiration;
            System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            long currentTime = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;

            double? refreshTime = expireTime - (timeToLive * (1.0 - fractionOfTtl));
            return refreshTime < currentTime;
        }

        /// <summary>
        /// Used as a fail-safe to prevent the condition of a refresh token expiring,
        /// which could happen after around 30 days.This function will return true
        /// if it has been at least 7 days and 1 hour since the last token was
        /// retrieved.
        /// </summary>
        /// <returns></returns>
        public bool IsRefreshTokenExpired()
        {
            if (_iamTokenData.Expiration == null)
            {
                return true;
            };

            long sevenDays = 7 * 24 * 3600;
            System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            long currentTime = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
            long? newTokenTime = _iamTokenData.Expiration + sevenDays;
            return newTokenTime < currentTime;
        }

        /// <summary>
        /// Save the response from the IAM service request to the object's state.
        /// </summary>
        /// <param name="iamTokenData">Response object from IAM service request</param>
        public void SaveTokenInfo(IamTokenData iamTokenData)
        {
            TokenData = iamTokenData;
        }

        /// <summary>
        /// Set a self-managed IAM access token.
        /// The access token should be valid and not yet expired.
        /// 
        /// By using this method, you accept responsibility for managing the
        /// access token yourself.You must set a new access token before this
        /// one expires. Failing to do so will result in authentication errors
        /// after this token expires.
        /// </summary>
        /// <param name="iamAccessToken">A valid, non-expired IAM access token.</param>
        public void SetAccessToken(string iamAccessToken)
        {
            _userAcessToken = iamAccessToken;
        }
        #endregion

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
        /// Do we have IamTokenData?
        /// </summary>
        /// <returns></returns>
        public bool HasIamTokenData()
        {
            return _tokenData != null;
        }

        /// <summary>
        /// Do we have an IAM apikey?
        /// </summary>
        /// <returns></returns>
        public bool HasIamApikey()
        {
            return !string.IsNullOrEmpty(_iamApiKey);
        }

        /// <summary>
        /// Do we have an IAM authentication token?
        /// </summary>
        /// <returns></returns>
        public bool HasIamAuthorizationToken()
        {
            return !string.IsNullOrEmpty(_userAcessToken);
        }
    }

    /// <summary>
    /// Vcap credentials object.
    /// </summary>
    [fsObject]
    public class VcapCredentials
    {
        /// <summary>
        /// List of credentials by service name.
        /// </summary>
        [fsProperty("VCAP_SERVICES")]
        public Dictionary<string, List<VcapCredential>> VCAP_SERVICES { get; set; }

        /// <summary>
        /// Gets a credential by name.
        /// </summary>
        /// <param name="name">Name of requested credential</param>
        /// <returns>A List of credentials who's names match the request name.</returns>
        public List<VcapCredential> GetCredentialByname(string name)
        {
            List<VcapCredential> credentialsList = new List<VcapCredential>();
            foreach (KeyValuePair<string, List<VcapCredential>> kvp in VCAP_SERVICES)
            {
                foreach (VcapCredential credential in kvp.Value)
                {
                    if (credential.Name == name)
                        credentialsList.Add(credential);
                }
            }

            return credentialsList;
        }
    }

    /// <summary>
    /// The Credential to a single service.
    /// </summary>
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

    /// <summary>
    /// The Credentials.
    /// </summary>
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
        [Obsolete("Authentication using legacy apikey is deprecated. Please authenticate using TokenOptions.")]
        public string ApiKey { get; set; }
        [fsProperty("apikey")]
        public string IamApikey { get; set; }
        [fsProperty("iam_url")]
        public string IamUrl { get; set; }
        [fsProperty("assistant_id")]
        public string AssistantId { get; set; }
    }

    /// <summary>
    /// IAM token options.
    /// </summary>
    [fsObject]
    public class TokenOptions
    {
        private string iamApiKey;
        [fsProperty("iamApiKey")]
        public string IamApiKey
        {
            get
            {
                return iamApiKey;
            }
            set
            {
                if (!Utility.HasBadFirstOrLastCharacter(value))
                {
                    iamApiKey = value;
                }
                else
                {
                    throw new WatsonException("The credentials shouldn't start or end with curly brackets or quotes. Be sure to remove any {} and \" characters surrounding your credentials");
                }
            }
        }
        [fsProperty("iamAcessToken")]
        public string IamAccessToken { get; set; }
        [fsProperty("iamUrl")]
        public string IamUrl { get; set; }
    }

    /// <summary>
    /// IAM Token data.
    /// </summary>
    [fsObject]
    public class IamTokenData
    {
        [fsProperty("access_token")]
        public string AccessToken { get; set; }
        [fsProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [fsProperty("token_type")]
        public string TokenType { get; set; }
        [fsProperty("expires_in")]
        public long? ExpiresIn { get; set; }
        [fsProperty("expiration")]
        public long? Expiration { get; set; }
    }
}
