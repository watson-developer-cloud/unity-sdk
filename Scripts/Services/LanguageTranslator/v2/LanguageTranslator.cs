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
using System.IO;

namespace IBM.Watson.DeveloperCloud.Services.LanguageTranslator.v2
{
    /// <summary>
    /// This class wraps the Language Translator service.
    /// <a href="http://www.ibm.com/watson/developercloud/language-translator.html">Language Translator Service</a>
    /// </summary>
    public class LanguageTranslator : IWatsonService
    {
        #region Public Types
        /// <summary>
        /// Callback for GetModels() method.
        /// </summary>
        /// <param name="models"></param>
        /// <param name="customData">User defined custom data</param>
        public delegate void GetModelsCallback(TranslationModels models, string customData);
        /// <summary>
        /// Callback for GetModel() method.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="customData">User defined custom data</param>
        public delegate void GetModelCallback(TranslationModel model, string customData);
        /// <summary>
        /// Callback for DeleteModel() method.
        /// </summary>
        /// <param name="success">Was the model deleted?</param>
        /// <param name="customData">User defined custom data</param>
        public delegate void DeleteModelCallback(bool success, string customData);
        /// <summary>
        /// Callback for GetLanguages() method.
        /// </summary>
        /// <param name="languages"></param>
        /// <param name="customData">User defined custom data</param>
        public delegate void GetLanguagesCallback(Languages languages, string customData);
        /// <summary>
        /// Callback for Identify() method.
        /// </summary>
        /// <param name="languages"></param>
        /// <param name="customData">User defined custom data</param>
        public delegate void IdentifyCallback(string languages, string customData);
        /// <summary>
        /// Callback for Translate() method.
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="customData">User defined custom data</param>
        public delegate void TranslateCallback(Translations translation, string customData);
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
        private const string ServiceId = "LanguageTranslatorV1";
        private fsSerializer _serializer = new fsSerializer();
        private Credentials _credentials = null;
        private string _url = "https://gateway.watsonplatform.net/language-translator/api";
        #endregion

        #region Constructor
        public LanguageTranslator(Credentials credentials)
        {
            if (credentials.HasCredentials() || credentials.HasAuthorizationToken())
            {
                Credentials = credentials;
            }
            else
            {
                throw new WatsonException("Please provide a username and password or authorization token to use the Language Translator service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }
        #endregion

        #region GetTranslation Functions
        /// <summary>
        /// Translate the provided text using the specified model.
        /// </summary>
        /// <param name="text">The text to translate.</param>
        /// <param name="model_id">The ID of the model to use.</param>
        /// <param name="callback">The callback to receive the translated text.</param>
        /// <returns>Returns true on success.</returns>
        public bool GetTranslation(string text, string model_id, TranslateCallback callback)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (string.IsNullOrEmpty(model_id))
                throw new ArgumentNullException("model_id");

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["model_id"] = model_id;
            parameters["text"] = new string[] { text };

            return GetTranslation(Json.Serialize(parameters), callback);
        }
        /// <summary>
        /// Translate the provided text using the specified source and target.
        /// </summary>
        /// <param name="text">The text to translate.</param>
        /// <param name="source">The ID of the source language.</param>
        /// <param name="target">The ID of the target language.</param>
        /// <param name="callback">The callback to receive the translated text.</param>
        /// <returns>Returns true on success.</returns>
        public bool GetTranslation(string text, string source, string target, TranslateCallback callback)
        {
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

            return GetTranslation(Json.Serialize(parameters), callback);
        }
        private bool GetTranslation(string json, TranslateCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v2/translate");
            if (connector == null)
                return false;

            TranslateReq req = new TranslateReq();
            req.Callback = callback;
            req.OnResponse = TranslateResponse;
            req.Send = Encoding.UTF8.GetBytes(json);
            req.Headers["accept"] = "application/json";
            req.Headers["Content-Type"] = "application/json";

            return connector.Send(req);
        }

        private class TranslateReq : RESTConnector.Request
        {
            public TranslateCallback Callback { get; set; }
            public string Data { get; set; }
        };
        private void TranslateResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Translations translations = new Translations();
            fsData data = null;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = translations;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Natural Language Classifier", "GetTranslation Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            string customData = ((TranslateReq)req).Data;
            if (((TranslateReq)req).Callback != null)
                ((TranslateReq)req).Callback(resp.Success ? translations : null, !string.IsNullOrEmpty(customData) ? customData : data.ToString());
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
        /// <param name="callback">The callback to invoke with the array of models.</param>
        /// <param name="sourceFilter">Optional source language filter.</param>
        /// <param name="targetFilter">Optional target language filter.</param>
        /// <param name="defaults">Controls if we get default, non-default, or all models.</param>
        /// <returns>Returns a true on success, false if it failed to submit the request.</returns>
        public bool GetModels(GetModelsCallback callback,
            string sourceFilter = null,
            string targetFilter = null,
            TypeFilter defaults = TypeFilter.ALL)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v2/models");
            if (connector == null)
                return false;

            GetModelsReq req = new GetModelsReq();
            req.Callback = callback;
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
            public GetModelsCallback Callback { get; set; }
            public string Data { get; set; }
        }

        private void GetModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            TranslationModels models = new TranslationModels();
            fsData data = null;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = models;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Natural Language Classifier", "GetModels Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            string customData = ((GetModelsReq)req).Data;
            if (((GetModelsReq)req).Callback != null)
                ((GetModelsReq)req).Callback(resp.Success ? models : null, (!string.IsNullOrEmpty(customData) ? customData : data.ToString()));
        }

        /// <summary>
        /// Get a specific model by it's ID.
        /// </summary>
        /// <param name="model_id"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool GetModel(GetModelCallback callback, string model_id)
        {
            if (string.IsNullOrEmpty(model_id))
                throw new ArgumentNullException("model_id");
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v2/models/");
            if (connector == null)
                return false;

            GetModelReq req = new GetModelReq();
            req.Callback = callback;
            req.Function = WWW.EscapeURL(model_id);
            req.OnResponse = GetModelResponse;

            return connector.Send(req);
        }

        private class GetModelReq : RESTConnector.Request
        {
            public GetModelCallback Callback { get; set; }
            public string Data { get; set; }
        }

        private void GetModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            TranslationModel model = new TranslationModel();
            fsData data = null;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = model;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Natural Language Classifier", "GetModel Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            string customData = ((GetModelReq)req).Data;
            if (((GetModelReq)req).Callback != null)
                ((GetModelReq)req).Callback(resp.Success ? model : null, (!string.IsNullOrEmpty(customData) ? customData : data.ToString()));
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
        /// <param name="callback">The OnCreateModel callback.</param>
        /// <param name="baseModelId">Specifies the domain model that is used as the base for the training. To see current supported domain models, use the GET /v2/models parameter..</param>
        /// <param name="customModelName">The model name. Valid characters are letters, numbers, -, and _. No spaces.</param>
        /// <param name="forcedGlossaryFilePath">A TMX file with your customizations. The customizations in the file completely overwrite the domain data translation, including high frequency or high confidence phrase translations. You can upload only one glossary with a file size less than 10 MB per call.</param>
        /// <param name="parallelCorpusFilePath">A TMX file that contains entries that are treated as a parallel corpus instead of a glossary.</param>
        /// <param name="monolingualCorpusFilePath">A UTF-8 encoded plain text file that is used to customize the target language model.</param>
        /// <param name="customData">User defined custom string data.</param>
        /// <returns>True if the call succeeded, false if the call fails.</returns>
        public bool CreateModel(OnCreateModel callback, string baseModelId, string customModelName, string forcedGlossaryFilePath = default(string), string parallelCorpusFilePath= default(string), string monolingualCorpusFilePath = default(string), string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(baseModelId))
                throw new ArgumentNullException("baseModelId");
            if (string.IsNullOrEmpty(customModelName))
                throw new ArgumentNullException("customModelName");
            if (string.IsNullOrEmpty(forcedGlossaryFilePath) && string.IsNullOrEmpty(parallelCorpusFilePath) && string.IsNullOrEmpty(monolingualCorpusFilePath))
                throw new ArgumentNullException("Either a forced glossary, parallel corpus or monolingual corpus is required to create a custom model.");
            
            CreateModelRequest req = new CreateModelRequest();
            req.Callback = callback;
            req.Data = customData;
            req.BaseModelId = baseModelId;
            req.CustomModelName = customModelName;
            req.ForcedGlossaryFilePath = forcedGlossaryFilePath;
            req.ParallelCorpusFilePath = parallelCorpusFilePath;
            req.MonolingualCorpusFilePath = monolingualCorpusFilePath;
            req.Parameters["base_model_id"] = baseModelId;
            req.Parameters["name"] = customModelName;
            req.OnResponse = OnCreateModelResponse;

            byte[] forcedGlossaryData = null;
            byte[] parallelCorpusData = null;
            byte[] monolingualCorpusData = null;

            if(!string.IsNullOrEmpty(forcedGlossaryFilePath))
            {
                try
                {
                    forcedGlossaryData = File.ReadAllBytes(forcedGlossaryFilePath);
                }
                catch(Exception e)
                {
                    Log.Debug("LanguageTranslator", "There was an error loading the forced glossary file: {0}", e.Message);
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
                    Log.Debug("LanguageTranslator", "There was an error loading the parallel corpus file: {0}", e.Message);
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
                    Log.Debug("LanguageTranslator", "There was an error loading the monolingual corpus file: {0}", e.Message);
                }
            }

            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (forcedGlossaryData != null)
                req.Forms["forced_glossary"] = new RESTConnector.Form(forcedGlossaryData, Path.GetFileName(forcedGlossaryFilePath), "text/xml");
            if (parallelCorpusData != null)
                req.Forms["parallel_corpus"] = new RESTConnector.Form(parallelCorpusData);
            if (monolingualCorpusData != null)
                req.Forms["monolingual_corpus"] = new RESTConnector.Form(monolingualCorpusData);

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v2/models");
            if (connector == null)
                return false;

            return connector.Send(req);
        }
        
        private class CreateModelRequest : RESTConnector.Request
        {
            public OnCreateModel Callback { get; set; }
            public string BaseModelId { get; set; }
            public string CustomModelName { get; set; }
            public string ForcedGlossaryFilePath { get; set; }
            public string ParallelCorpusFilePath { get; set; }
            public string MonolingualCorpusFilePath { get; set; }
            public string Data { get; set; }
        }

        private void OnCreateModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            TranslationModel result = new TranslationModel();
            fsData data = null;

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
                }
                catch (Exception e)
                {
                    Log.Error("Discovery", "Create model Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            string customData = ((CreateModelRequest)req).Data;
            if (((CreateModelRequest)req).Callback != null)
                ((CreateModelRequest)req).Callback(resp.Success ? result : null, !string.IsNullOrEmpty(customData) ? customData : data.ToString());
        }

        /// <summary>
        /// Delete a specific model by it's ID.
        /// </summary>
        /// <param name="model_id">The model to delete.</param>
        /// <param name="callback">The DeleteModel callback.</param>
        /// <returns></returns>
        public bool DeleteModel(DeleteModelCallback callback, string model_id)
        {
            if (string.IsNullOrEmpty(model_id))
                throw new ArgumentNullException("model_id");
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v2/models/");
            if (connector == null)
                return false;

            DeleteModelReq req = new DeleteModelReq();
            req.Callback = callback;
            req.Function = WWW.EscapeURL(model_id);
            req.OnResponse = DeleteModelResponse;
            req.Delete = true;
            return connector.Send(req);
        }

        private class DeleteModelReq : RESTConnector.Request
        {
            public DeleteModelCallback Callback { get; set; }
            public string Data { get; set; }
        }

        private void DeleteModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((DeleteModelReq)req).Data;
            if (((DeleteModelReq)req).Callback != null)
                ((DeleteModelReq)req).Callback(resp.Success, customData);
        }
        #endregion

        #region GetLanguages Functions
        /// <summary>
        /// This function returns a list to the callback of all identifiable languages.
        /// </summary>
        /// <param name="callback">The callback to invoke with a Language array, null on error.</param>
        /// <returns>Returns true if the request was submitted.</returns>
        public bool GetLanguages(GetLanguagesCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v2/identifiable_languages");
            if (connector == null)
                return false;

            GetLanguagesReq req = new GetLanguagesReq();
            req.Callback = callback;
            req.OnResponse = GetLanguagesResponse;

            return connector.Send(req);
        }

        private class GetLanguagesReq : RESTConnector.Request
        {
            public GetLanguagesCallback Callback { get; set; }
            public string Data { get; set; }
        }

        private void GetLanguagesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Languages langs = new Languages();
            fsData data = null;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = langs;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Natural Language Classifier", "GetLanguages Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            string customData = ((GetLanguagesReq)req).Data;
            if (((GetLanguagesReq)req).Callback != null)
                ((GetLanguagesReq)req).Callback(resp.Success ? langs : null, (!string.IsNullOrEmpty(customData) ? customData : data.ToString()));
        }
        #endregion

        #region Identify Functions
        /// <summary>
        /// Identifies a language from the given text.
        /// </summary>
        /// <param name="text">The text sample to ID.</param>
        /// <param name="callback">The callback to receive the results.</param>
        /// <returns></returns>
        public bool Identify(IdentifyCallback callback, string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v2/identify");
            if (connector == null)
                return false;

            IdentifyReq req = new IdentifyReq();
            req.Callback = callback;
            req.Send = Encoding.UTF8.GetBytes(text);
            req.Headers["Content-Type"] = "text/plain";
            req.OnResponse = OnIdentifyResponse;

            return connector.Send(req);
        }

        private class IdentifyReq : RESTConnector.Request
        {
            public IdentifyCallback Callback { get; set; }
            public string Data { get; set; }
        };

        private void OnIdentifyResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            IdentifyReq identifyRequest = req as IdentifyReq;
            if (identifyRequest == null)
                throw new WatsonException("Unexpected Request type.");

            string identifiedLanguages;

            if (resp.Success)
            {
                identifiedLanguages = Encoding.UTF8.GetString(resp.Data);
                string customData = identifyRequest.Data;
                if (((IdentifyReq)req).Callback != null)
                    ((IdentifyReq)req).Callback(resp.Success ? identifiedLanguages : null, (!string.IsNullOrEmpty(customData) ? customData : identifiedLanguages));
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
