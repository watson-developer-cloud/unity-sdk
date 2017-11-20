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

namespace IBM.Watson.DeveloperCloud.Services.LanguageTranslation.v2
{
    /// <summary>
    /// This class wraps the Language Translation service.
    /// <a href="http://www.ibm.com/watson/developercloud/language-translation.html">Language Translation Service</a>
    /// </summary>
    public class LanguageTranslation : IWatsonService
    {
        #region Public Types
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

        #region Private Data
        private const string ServiceId = "LanguageTranslationV1";
        private fsSerializer _serializer = new fsSerializer();
        private Credentials _credentials = null;
        private string _url = "https://gateway.watsonplatform.net/language-translation/api";
        #endregion

        #region Constructor
        public LanguageTranslation(Credentials credentials)
        {
            if (credentials.HasCredentials() || credentials.HasAuthorizationToken())
            {
                Credentials = credentials;
            }
            else
            {
                throw new WatsonException("Please provide a username and password or authorization token to use the Language Translation service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
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

        #region GetTranslation Functions
        /// <summary>
        /// Translate the provided text using the specified model.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="text">The text to translate.</param>
        /// <param name="model_id">The ID of the model to use.</param>
        /// <returns>Returns true on success.</returns>
        public bool GetTranslation(SuccessCallback<Translations> successCallback, FailCallback failCallback, string text, string model_id, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (string.IsNullOrEmpty(model_id))
                throw new ArgumentNullException("model_id");

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["model_id"] = model_id;
            parameters["text"] = new string[] { text };

            return GetTranslation(successCallback, failCallback, Json.Serialize(parameters), customData);
        }
        /// <summary>
        /// Translate the provided text using the specified source and target.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="text">The text to translate.</param>
        /// <param name="source">The ID of the source language.</param>
        /// <param name="target">The ID of the target language.</param>
        /// <returns>Returns true on success.</returns>
        public bool GetTranslation(SuccessCallback<Translations> successCallback, FailCallback failCallback, string text, string source, string target, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(target))
                throw new ArgumentNullException("target");

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["source"] = source;
            parameters["target"] = target;
            parameters["text"] = new string[] { text };

            return GetTranslation(successCallback, failCallback, Json.Serialize(parameters), customData);
        }
        private bool GetTranslation(SuccessCallback<Translations> successCallback, FailCallback failCallback, string json, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(json))
                throw new ArgumentNullException("json");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v2/translate");
            if (connector == null)
                return false;

            TranslateReq req = new TranslateReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.OnResponse = TranslateResponse;
            req.Send = Encoding.UTF8.GetBytes(json);
            req.Headers["accept"] = "application/json";
            req.Headers["Content-Type"] = "application/json";

            return connector.Send(req);
        }

        private class TranslateReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Translations> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        };
        private void TranslateResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Translations result = new Translations();
            fsData data = null;
            Dictionary<string, object> customData = ((TranslateReq)req).CustomData;

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
                    Log.Error("LanguageTranslation.TranslateResponse()", "GetTranslation Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((TranslateReq)req).SuccessCallback != null)
                    ((TranslateReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((TranslateReq)req).FailCallback != null)
                    ((TranslateReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Models Functions
        /// <summary>
        /// This determines the types of models to return with GetModels.
        /// </summary>
        public enum TypeFilter
        {
            /// <summary>
            /// Default types
            /// </summary>
            DEFAULT,
            /// <summary>
            /// Non-Default types
            /// </summary>
            NON_DEFAULT,
            /// <summary>
            /// All types are returned.
            /// </summary>
            ALL
        }

        /// <summary>
        /// Retrieve the translation models with optional filters.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="sourceFilter">Optional source language filter.</param>
        /// <param name="targetFilter">Optional target language filter.</param>
        /// <param name="defaults">Controls if we get default, non-default, or all models.</param>
        /// <returns>Returns a true on success, false if it failed to submit the request.</returns>
        public bool GetModels(SuccessCallback<TranslationModels> successCallback, 
            FailCallback failCallback,
            string sourceFilter = null,
            string targetFilter = null,
            TypeFilter defaults = TypeFilter.ALL,
            Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v2/models");
            if (connector == null)
                return false;

            GetModelsReq req = new GetModelsReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.OnResponse = GetModelsResponse;

            if (!string.IsNullOrEmpty(sourceFilter))
                req.Parameters["source"] = sourceFilter;
            if (!string.IsNullOrEmpty(targetFilter))
                req.Parameters["target"] = targetFilter;
            if (defaults == TypeFilter.DEFAULT)
                req.Parameters["default"] = "true";
            else if (defaults == TypeFilter.NON_DEFAULT)
                req.Parameters["default"] = "false";

            return connector.Send(req);
        }

        private class GetModelsReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<TranslationModels> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void GetModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            TranslationModels result = new TranslationModels();
            fsData data = null;
            Dictionary<string, object> customData = ((GetModelsReq)req).CustomData;

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
                    Log.Error("LanguageTranslation.GetModelsResponse()", "GetModels Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetModelsReq)req).SuccessCallback != null)
                    ((GetModelsReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetModelsReq)req).FailCallback != null)
                    ((GetModelsReq)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Get a specific model by it's ID.
        /// </summary>
        /// <param name="model_id"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool GetModel(SuccessCallback<TranslationModel> successCallback, FailCallback failCallback, string model_id, Dictionary<string, object> customData = null)
        {
            if (string.IsNullOrEmpty(model_id))
                throw new ArgumentNullException("model_id");
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v2/models/");
            if (connector == null)
                return false;

            GetModelReq req = new GetModelReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.Function = WWW.EscapeURL(model_id);
            req.OnResponse = GetModelResponse;

            return connector.Send(req);
        }

        private class GetModelReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<TranslationModel> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void GetModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            TranslationModel result = new TranslationModel();
            fsData data = null;
            Dictionary<string, object> customData = ((GetModelReq)req).CustomData;

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
                    Log.Error("LanguageTranslation.GetModelResponse()", "GetModel Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetModelReq)req).SuccessCallback != null)
                    ((GetModelReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetModelReq)req).FailCallback != null)
                    ((GetModelReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region GetLanguages Functions
        /// <summary>
        /// This function returns a list to the callback of all identifiable languages.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <returns>Returns true if the request was submitted.</returns>
        public bool GetLanguages(SuccessCallback<Languages> successCallback, FailCallback failCallback, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v2/identifiable_languages");
            if (connector == null)
                return false;

            GetLanguagesReq req = new GetLanguagesReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.OnResponse = GetLanguagesResponse;

            return connector.Send(req);
        }

        private class GetLanguagesReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<Languages> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void GetLanguagesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Languages result = new Languages();
            fsData data = null;
            Dictionary<string, object> customData = ((GetLanguagesReq)req).CustomData;

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
                    Log.Error("LanguageTranslation.GetLanguagesResponse()", "GetLanguages Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetLanguagesReq)req).SuccessCallback != null)
                    ((GetLanguagesReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetLanguagesReq)req).FailCallback != null)
                    ((GetLanguagesReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Identify Functions
        /// <summary>
        /// Identifies a language from the given text.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="text">The text sample to ID.</param>
        /// <returns></returns>
        public bool Identify(SuccessCallback<IdentifyReq> successCallback, FailCallback failCallback, string text, Dictionary<string, object> customData = null)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v2/identify");
            if (connector == null)
                return false;

            IdentifyReq req = new IdentifyReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.Send = Encoding.UTF8.GetBytes(text);
            req.Headers["Content-Type"] = "text/plain";
            req.OnResponse = OnIdentifyResponse;

            return connector.Send(req);
        }

        public class IdentifyReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<IdentifyReq> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        };

        private void OnIdentifyResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            IdentifyReq result = new IdentifyReq();
            fsData data = null;
            Dictionary<string, object> customData = ((IdentifyReq)req).CustomData;

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
                    Log.Error("LanguageTranslation.OnIdentifyResponse()", "OnIdentifyResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((IdentifyReq)req).SuccessCallback != null)
                    ((IdentifyReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((IdentifyReq)req).FailCallback != null)
                    ((IdentifyReq)req).FailCallback(resp.Error, customData);
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
