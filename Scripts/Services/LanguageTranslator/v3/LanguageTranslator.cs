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
using System.IO;
using UnityEngine.Networking;
using Utility = IBM.Watson.DeveloperCloud.Utilities.Utility;

namespace IBM.Watson.DeveloperCloud.Services.LanguageTranslator.v3
{
    /// <summary>
    /// This class wraps the Language Translator service.
    /// <a href="http://www.ibm.com/watson/developercloud/language-translator.html">Language Translator Service</a>
    /// </summary>
    public class LanguageTranslator : IWatsonService
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

        private string _versionDate;
        /// <summary>
        /// Gets and sets the versionDate of the service.
        /// </summary>
        public string VersionDate
        {
            get { return _versionDate; }
            set { _versionDate = value; }
        }

        private bool disableSslVerification = false;
        /// <summary>
        /// Gets and sets the option to disable ssl verification
        /// </summary>
        public bool DisableSslVerification
        {
            get { return disableSslVerification; }
            set { disableSslVerification = value; }
        }
        #endregion

        #region Private Data
        private const string ServiceId = "language_translator";
        private fsSerializer _serializer = new fsSerializer();
        private Credentials _credentials = null;
        private string _url = "https://gateway.watsonplatform.net/language-translator/api";
        #endregion

        #region Constructor
        /// <summary>
        /// LanguageTranslator constructor. Use this constructor to auto load credentials via ibm-credentials.env file.
        /// </summary>
        public LanguageTranslator()
        {
            var credentialsPaths = Utility.GetCredentialsPaths();
            if (credentialsPaths.Count > 0)
            {
                foreach (string path in credentialsPaths)
                {
                    if (Utility.LoadEnvFile(path))
                    {
                        break;
                    }
                }

                string ApiKey = Environment.GetEnvironmentVariable(ServiceId.ToUpper() + "_APIKEY");
                string Username = Environment.GetEnvironmentVariable(ServiceId.ToUpper() + "_USERNAME");
                string Password = Environment.GetEnvironmentVariable(ServiceId.ToUpper() + "_PASSWORD");
                string ServiceUrl = Environment.GetEnvironmentVariable(ServiceId.ToUpper() + "_URL");

                if (string.IsNullOrEmpty(ApiKey) && (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password)))
                {
                    throw new NullReferenceException(string.Format("Either {0}_APIKEY or {0}_USERNAME and {0}_PASSWORD did not exist. Please add credentials with this key in ibm-credentials.env.", ServiceId.ToUpper()));
                }

                if (!string.IsNullOrEmpty(ApiKey))
                {
                    TokenOptions tokenOptions = new TokenOptions()
                    {
                        IamApiKey = ApiKey
                    };

                    Credentials = new Credentials(tokenOptions, ServiceUrl);

                    if (string.IsNullOrEmpty(Credentials.Url))
                    {
                        Credentials.Url = Url;
                    }
                }

                if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
                {
                    Credentials = new Credentials(Username, Password, Url);
                }
            }
        }

        public LanguageTranslator(string versionDate, Credentials credentials)
        {
            VersionDate = versionDate;
            if (credentials.HasCredentials() || credentials.HasWatsonAuthenticationToken() || credentials.HasIamTokenData())
            {
                Credentials = credentials;

                if (string.IsNullOrEmpty(credentials.Url))
                {
                    credentials.Url = Url;
                }
            }
            else
            {
                throw new WatsonException("Please provide a username and password or authorization token to use the Language Translator service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
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

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v3/translate");
            if (connector == null)
                return false;

            TranslateReq req = new TranslateReq();
            req.Parameters["version"] = VersionDate;
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.HttpMethod = UnityWebRequest.kHttpVerbPOST;
            req.DisableSslVerification = DisableSslVerification;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.OnResponse = TranslateResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=language_translator;service_version=v3;operation_id=GetTranslation";
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
                    Log.Error("LanguageTranslator.TranslateResponse()", "GetTranslation Exception: {0}", e.ToString());
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

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v3/models");
            if (connector == null)
                return false;

            GetModelsReq req = new GetModelsReq();
            req.Parameters["version"] = VersionDate;
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.HttpMethod = UnityWebRequest.kHttpVerbGET;
            req.DisableSslVerification = DisableSslVerification;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.OnResponse = GetModelsResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=language_translator;service_version=v3;operation_id=GetModels";

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
                    Log.Error("LanguageTranslator.GetModelsResponse()", "GetModels Exception: {0}", e.ToString());
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
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="model_id"></param>
        /// <returns></returns>
        public bool GetModel(SuccessCallback<TranslationModel> successCallback, FailCallback failCallback, string model_id, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(model_id))
                throw new ArgumentNullException("model_id");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v3/models/");
            if (connector == null)
                return false;

            GetModelReq req = new GetModelReq();
            req.Parameters["version"] = VersionDate;
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.HttpMethod = UnityWebRequest.kHttpVerbGET;
            req.DisableSslVerification = DisableSslVerification;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Function = UnityWebRequest.EscapeURL(model_id);
            req.OnResponse = GetModelResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=language_translator;service_version=v3;operation_id=GetModel";

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
                    Log.Error("LanguageTranslator.GetModelResponse()", "GetModel Exception: {0}", e.ToString());
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

        /// <summary>
        /// The callback used by CreateModel
        /// </summary>
        /// <param name="resp">The TranslationModel response.</param>
        /// <param name="customData">Optional custom data.</param>
        public delegate void OnCreateModel(TranslationModel resp, string customData);

        /// <summary>
        /// Uploads a TMX glossary file on top of a domain to customize a translation model.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="baseModelId">Specifies the domain model that is used as the base for the training. To see current supported domain models, use the GET /v2/models parameter..</param>
        /// <param name="customModelName">The model name. Valid characters are letters, numbers, -, and _. No spaces.</param>
        /// <param name="forcedGlossaryFilePath">A TMX file with your customizations. The customizations in the file completely overwrite the domain data translation, including high frequency or high confidence phrase translations. You can upload only one glossary with a file size less than 10 MB per call.</param>
        /// <param name="parallelCorpusFilePath">A TMX file that contains entries that are treated as a parallel corpus instead of a glossary.</param>
        /// <param name="monolingualCorpusFilePath">A UTF-8 encoded plain text file that is used to customize the target language model.</param>
        /// <param name="customData">User defined custom string data.</param>
        /// <returns>True if the call succeeded, false if the call fails.</returns>
        public bool CreateModel(SuccessCallback<TranslationModel> successCallback, FailCallback failCallback, string baseModelId, string customModelName, string forcedGlossaryFilePath = default(string), string parallelCorpusFilePath = default(string), string monolingualCorpusFilePath = default(string), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(baseModelId))
                throw new ArgumentNullException("baseModelId");
            if (string.IsNullOrEmpty(customModelName))
                throw new ArgumentNullException("customModelName");
            if (string.IsNullOrEmpty(forcedGlossaryFilePath) && string.IsNullOrEmpty(parallelCorpusFilePath) && string.IsNullOrEmpty(monolingualCorpusFilePath))
                throw new ArgumentNullException("Either a forced glossary, parallel corpus or monolingual corpus is required to create a custom model.");

            CreateModelRequest req = new CreateModelRequest();
            req.Parameters["version"] = VersionDate;
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.HttpMethod = UnityWebRequest.kHttpVerbPOST;
            req.DisableSslVerification = DisableSslVerification;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Parameters["base_model_id"] = baseModelId;
            req.Parameters["name"] = customModelName;
            req.OnResponse = OnCreateModelResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=language_translator;service_version=v3;operation_id=CreateModel";

            byte[] forcedGlossaryData = null;
            byte[] parallelCorpusData = null;
            byte[] monolingualCorpusData = null;

            if (!string.IsNullOrEmpty(forcedGlossaryFilePath))
            {
                try
                {
                    forcedGlossaryData = File.ReadAllBytes(forcedGlossaryFilePath);
                }
                catch (Exception e)
                {
                    Log.Debug("LanguageTranslator.CreateModel()", "There was an error loading the forced glossary file: {0}", e.Message);
                }
            }

            if (!string.IsNullOrEmpty(parallelCorpusFilePath))
            {
                try
                {
                    parallelCorpusData = File.ReadAllBytes(parallelCorpusFilePath);
                }
                catch (Exception e)
                {
                    Log.Debug("LanguageTranslator.CreateModel()", "There was an error loading the parallel corpus file: {0}", e.Message);
                }
            }

            if (!string.IsNullOrEmpty(monolingualCorpusFilePath))
            {
                try
                {
                    monolingualCorpusData = File.ReadAllBytes(monolingualCorpusFilePath);
                }
                catch (Exception e)
                {
                    Log.Debug("LanguageTranslator.CreateModel()", "There was an error loading the monolingual corpus file: {0}", e.Message);
                }
            }

            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (forcedGlossaryData != null)
                req.Forms["forced_glossary"] = new RESTConnector.Form(forcedGlossaryData, Path.GetFileName(forcedGlossaryFilePath), "text/xml");
            if (parallelCorpusData != null)
                req.Forms["parallel_corpus"] = new RESTConnector.Form(parallelCorpusData);
            if (monolingualCorpusData != null)
                req.Forms["monolingual_corpus"] = new RESTConnector.Form(monolingualCorpusData);

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v3/models");
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class CreateModelRequest : RESTConnector.Request
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

        private void OnCreateModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            TranslationModel result = new TranslationModel();
            fsData data = null;
            Dictionary<string, object> customData = ((CreateModelRequest)req).CustomData;
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
                    Log.Error("LanguageTranslator.OnCreateModelResponse()", "Create model Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((CreateModelRequest)req).SuccessCallback != null)
                    ((CreateModelRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((CreateModelRequest)req).FailCallback != null)
                    ((CreateModelRequest)req).FailCallback(resp.Error, customData);
            }
        }

        /// <summary>
        /// Delete a specific model by it's ID.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="model_id">The model to delete.</param>
        /// <returns></returns>
        public bool DeleteModel(SuccessCallback<DeleteModelResult> successCallback, FailCallback failCallback, string model_id, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(model_id))
                throw new ArgumentNullException("model_id");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v3/models/");
            if (connector == null)
                return false;

            DeleteModelReq req = new DeleteModelReq();
            req.Parameters["version"] = VersionDate;
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.HttpMethod = UnityWebRequest.kHttpVerbDELETE;
            req.DisableSslVerification = DisableSslVerification;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Function = UnityWebRequest.EscapeURL(model_id);
            req.OnResponse = DeleteModelResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=language_translator;service_version=v3;operation_id=DeleteModel";
            return connector.Send(req);
        }

        private class DeleteModelReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<DeleteModelResult> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void DeleteModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DeleteModelResult result = new DeleteModelResult();
            fsData data = null;
            Dictionary<string, object> customData = ((DeleteModelReq)req).CustomData;
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
                    Log.Error("LanguageTranslator.DeleteModelResponse()", "DeleteModelResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((DeleteModelReq)req).SuccessCallback != null)
                    ((DeleteModelReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DeleteModelReq)req).FailCallback != null)
                    ((DeleteModelReq)req).FailCallback(resp.Error, customData);
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

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v3/identifiable_languages");
            if (connector == null)
                return false;

            GetLanguagesReq req = new GetLanguagesReq();
            req.Parameters["version"] = VersionDate;
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.HttpMethod = UnityWebRequest.kHttpVerbGET;
            req.DisableSslVerification = DisableSslVerification;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.OnResponse = GetLanguagesResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=language_translator;service_version=v3;operation_id=GetLanguages";

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
                    Log.Error("LanguageTranslator.GetLanguagesResponse()", "GetLanguages Exception: {0}", e.ToString());
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
        public bool Identify(SuccessCallback<IdentifiedLanguages> successCallback, FailCallback failCallback, string text, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v3/identify");
            if (connector == null)
                return false;

            IdentifyReq req = new IdentifyReq();
            req.Parameters["version"] = VersionDate;
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.HttpMethod = UnityWebRequest.kHttpVerbPOST;
            req.DisableSslVerification = DisableSslVerification;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            req.Send = Encoding.UTF8.GetBytes(text);
            req.Headers["Content-Type"] = "text/plain";
            req.Headers["Accept"] = "application/json";
            req.OnResponse = OnIdentifyResponse;
            req.Headers["X-IBMCloud-SDK-Analytics"] = "service_name=language_translator;service_version=v3;operation_id=Identify";

            return connector.Send(req);
        }

        private class IdentifyReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<IdentifiedLanguages> SuccessCallback { get; set; }
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
            IdentifiedLanguages result = new IdentifiedLanguages();
            fsData data = null;
            Dictionary<string, object> customData = ((IdentifyReq)req).CustomData;
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
                    Log.Error("LanguageTranslator.OnIdentifyResponse()", "OnIdentifyResponse Exception: {0}", e.ToString());
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
