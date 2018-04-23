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
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// IAM Token Manager Service.
/// 
/// Retreives, stores, and refreshes IAM tokens.
/// </summary>
public class TokenManager : MonoBehaviour
{
    private string _iamUrl;
    private IamTokenData _iamTokenData;
    private string _iamApiKey;
    private string _userAcessToken;
    private fsSerializer _serializer = new fsSerializer();

    public TokenManager(TokenOptions options)
    {
        _iamUrl = !string.IsNullOrEmpty(options.IamUrl) ? options.IamUrl : "https://iam.ng.bluemix.net/identity/token";
        _iamTokenData = new IamTokenData();

        if (!string.IsNullOrEmpty(options.IamApiKey))
        {
            _iamApiKey = options.IamApiKey;
        }
        if (!string.IsNullOrEmpty(options.IamAccessToken))
        {
            this._userAcessToken = options.IamAccessToken;
        }
    }

    /// <summary>
    /// This function sends an access token back through a callback. The source of the token
    /// is determined by the following logic:
    /// 1. If user provides their own managed access token, assume it is valid and send it
    /// 2. If this class is managing tokens and does not yet have one, make a request for one
    /// 3. If this class is managing tokens and the token has expired, refresh it
    /// 4. If this class is managing tokens and has a valid token stored, send it
    /// </summary>
    /// <param name="successCallback">The success callback.</param>
    /// <param name="failCallback">The fail callback.</param>
    /// <param name="customData">Dictionary of custom data.</param>
    public void GetToken(SuccessCallback<IamTokenData> successCallback, FailCallback failCallback, Dictionary<string, object> customData = null)
    {
        if (!string.IsNullOrEmpty(_userAcessToken))
        {
            // 1. use user-managed token
            successCallback(new IamTokenData() { AccessToken = _userAcessToken }, customData);
        }
        else if (!string.IsNullOrEmpty(_iamTokenData.AccessToken) || IsRefreshTokenExpired())
        {
            // 2. request an initial token
            RequestToken(successCallback, failCallback);
        }
        else if (IsTokenExpired())
        {
            // 3. refresh a token
            RefreshToken(successCallback, failCallback);
        }
        else
        {
            //  4. use valid managed token
            successCallback(new IamTokenData() { AccessToken = _iamTokenData.AccessToken }, customData);
        }
    }

    private void HandleRequestToken(IamTokenData iamTokenData, Dictionary<string, object> customData)
    {
        SaveTokenInfo(iamTokenData);
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("TokenManager.OnFail()", "Error received: {0}", error.ToString());
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

    #region Request Token
    /// <summary>
    /// Request an IAM token using an API key.
    /// </summary>
    /// <param name="successCallback">The request callback.</param>
    /// <param name="failCallback">The fail callback.</param>
    /// <param name="customData">Dictionary of custom data.</param>
    /// <returns></returns>
    public bool RequestToken(SuccessCallback<IamTokenData> successCallback, FailCallback failCallback, Dictionary<string, object> customData = null)
    {
        if (successCallback == null)
            throw new ArgumentNullException("successCallback");
        if (failCallback == null)
            throw new ArgumentNullException("failCallback");

        RESTConnector connector = new RESTConnector();
        connector.URL = _iamUrl;
        if (connector == null)
            return false;

        RequestTokenRequest req = new RequestTokenRequest();
        req.SuccessCallback = successCallback;
        req.FailCallback = failCallback;
        req.Headers.Add("Content-type", "application/x-www-form-urlencoded");
        req.Headers.Add("Authorization", "Basic Yng6Yng=");
        req.OnResponse = OnRefreshTokenResponse;
        req.CustomData = customData;
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

    private class RequestTokenRequest : RESTConnector.Request
    {
        /// <summary>
        /// Custom data.
        /// </summary>
        public Dictionary<string, object> CustomData { get; set; }
        /// <summary>
        /// OnGetToken callback delegate
        /// </summary>
        public SuccessCallback<IamTokenData> SuccessCallback { get; set; }
        /// <summary>
        /// The fail callback.
        /// </summary>
        public FailCallback FailCallback { get; set; }
    }

    private void OnRequestTokenResponse(RESTConnector.Request req, RESTConnector.Response resp)
    {
        IamTokenData result = new IamTokenData();
        fsData data = null;
        Dictionary<string, object> customData = ((RequestTokenRequest)req).CustomData;
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
            if (((RequestTokenRequest)req).SuccessCallback != null)
                ((RequestTokenRequest)req).SuccessCallback(result, customData);
        }
        else
        {
            if (((RequestTokenRequest)req).FailCallback != null)
                ((RequestTokenRequest)req).FailCallback(resp.Error, customData);
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
    public bool RefreshToken(SuccessCallback<IamTokenData> successCallback, FailCallback failCallback, Dictionary<string, object> customData = null)
    {
        if (successCallback == null)
            throw new ArgumentNullException("successCallback");
        if (failCallback == null)
            throw new ArgumentNullException("failCallback");

        RESTConnector connector = new RESTConnector();
        connector.URL = _iamUrl;
        if (connector == null)
            return false;

        RefreshTokenRequest req = new RefreshTokenRequest();
        req.SuccessCallback = successCallback;
        req.FailCallback = failCallback;
        req.Headers.Add("Content-type", "application/x-www-form-urlencoded");
        req.Headers.Add("Authorization", "Basic Yng6Yng=");
        req.OnResponse = OnRefreshTokenResponse;
        req.CustomData = customData;
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

    private class RefreshTokenRequest : RESTConnector.Request
    {
        /// <summary>
        /// Custom data.
        /// </summary>
        public Dictionary<string, object> CustomData { get; set; }
        /// <summary>
        /// Success callback delegate
        /// </summary>
        public SuccessCallback<IamTokenData> SuccessCallback { get; set; }
        /// <summary>
        /// The fail callback.
        /// </summary>
        public FailCallback FailCallback { get; set; }
    }

    private void OnRefreshTokenResponse(RESTConnector.Request req, RESTConnector.Response resp)
    {
        IamTokenData result = new IamTokenData();
        fsData data = null;
        Dictionary<string, object> customData = ((RefreshTokenRequest)req).CustomData;
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
            if (((RefreshTokenRequest)req).SuccessCallback != null)
                ((RefreshTokenRequest)req).SuccessCallback(result, customData);
        }
        else
        {
            if (((RefreshTokenRequest)req).FailCallback != null)
                ((RefreshTokenRequest)req).FailCallback(resp.Error, customData);
        }
    }
    #endregion

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
        long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();

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
        long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
        long? newTokenTime = _iamTokenData.Expiration + sevenDays;
        return newTokenTime < currentTime;
    }

    /// <summary>
    /// Save the response from the IAM service request to the object's state.
    /// </summary>
    /// <param name="iamTokenData">Response object from IAM service request</param>
    public void SaveTokenInfo(IamTokenData iamTokenData)
    {
        _iamTokenData = iamTokenData;
    }
}

[fsObject]
public class TokenOptions
{
    [fsProperty("iamApiKey")]
    public string IamApiKey { get; set; }
    [fsProperty("iamAcessToken")]
    public string IamAccessToken { get; set; }
    [fsProperty("iamUrl")]
    public string IamUrl { get; set; }
}

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
