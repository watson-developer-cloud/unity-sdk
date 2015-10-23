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
* @author Richard Lyle (rolyle@us.ibm.com)
*/

using UnityEngine;
using System.Collections.Generic;
using IBM.Watson.Connection;
using IBM.Watson.Utilities;
using IBM.Watson.Logging;
using System.Text;
using MiniJSON;
using System;
using System.Collections;

namespace IBM.Watson.Services.v1
{
    public class Translate
    {
        #region Public Types
        public class Language
        {
            public string Code { get; set; }        // country code of the language 
            public string Name { get; set; }        // name of the language                                    
        }
        public class Translation
        {
            public long WordCount { get; set; }
            public long CharacterCount { get; set; }
            public string[] Translations { get; set; }
        }
        public class Model
        {
            public string ModelId { get; set; }
            public string Name { get; set; }
            public string Source { get; set; }
            public string Target { get; set; }
            public string BaseModelId { get; set; }
            public string Domain { get; set; }
            public bool Cutomizable { get; set; }
            public bool Default { get; set; }
            public string Owner { get; set; }
            public string Status { get; set; }
        }
        public delegate void GetModelsCallback(Model[] models);
        public delegate void GetModelCallback(Model model);
        public delegate void GetLanguagesCallback(Language[] languages);
        public delegate void IdentifyCallback(string languages);
        public delegate void TranslateCallback(Translation translation);

        #endregion

        #region Private Data
        private const string SERVICE_ID = "TranslateV1";
        #endregion

        #region Public Properties
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

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v2/translate");
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
        };
        private void TranslateResponse(RESTConnector.Request r, RESTConnector.Response resp)
        {
            TranslateReq req = r as TranslateReq;
            if (req == null)
                throw new WatsonException("Unexpected Request type.");

            bool bSuccess = false;
            if (resp.Success)
            {
                try {
                    Translation translation = new Translation();

                    string jsonString = Encoding.UTF8.GetString(resp.Data);
                    IDictionary json = Json.Deserialize(jsonString) as IDictionary;

                    translation.WordCount = (long)json["word_count"];
                    translation.CharacterCount = (long)json["character_count"];

                    List<string> translations = new List<string>();

                    IList itranslations = json["translations"] as IList;
                    foreach (var t in itranslations)
                    {
                        IDictionary itranslation = t as IDictionary;
                        translations.Add((string)itranslation["translation"]);
                    }

                    translation.Translations = translations.ToArray();
                    bSuccess = true;

                    if (req.Callback != null)
                        req.Callback(translation);
                }
                catch( Exception e )
                {
                    Log.Error( "Translate", "Translate response exception: {0}", e.ToString() );
                }
            }

            if (! bSuccess )
            {
                Log.Error("Translate", "GetTranslation() failed: {0}", resp.Error);
                if (req.Callback != null)
                    req.Callback(null);
            }
        }
        #endregion

        #region Models Functions
        public enum TypeFilter
        {
            DEFAULT,
            NON_DEFAULT,
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

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v2/models");
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
        }

        private void GetModelsResponse(RESTConnector.Request r, RESTConnector.Response resp)
        {
            GetModelsReq req = r as GetModelsReq;
            if (req == null)
                throw new WatsonException("Unexpected Request type.");

            if (resp.Success)
            {
                List<Model> models = new List<Model>();

                string jsonData = Encoding.UTF8.GetString(resp.Data);
                IDictionary json = Json.Deserialize(jsonData) as IDictionary;

                IList imodels = json["models"] as IList;
                foreach (var m in imodels)
                {
                    IDictionary imodel = m as IDictionary;
                    models.Add(ParseModelJson(imodel));
                }

                if (req.Callback != null)
                    req.Callback(models.ToArray());
            }
            else
            {
                Log.Error("Translate", "GetModels() failed: {0}", resp.Error);
                if (req.Callback != null)
                    req.Callback(null);
            }
        }

        private Model ParseModelJson(IDictionary imodel)
        {
            Model model = new Model();
            model.ModelId = (string)imodel["model_id"];
            model.Name = (string)imodel["name"];
            model.Source = (string)imodel["source"];
            model.Target = (string)imodel["target"];
            model.BaseModelId = (string)imodel["base_model_id"];
            model.Domain = (string)imodel["domain"];
            model.Cutomizable = (bool)imodel["customizable"];
            model.Default = (bool)imodel["default_model"];
            model.Owner = (string)imodel["owner"];
            model.Status = (string)imodel["status"];
            return model;
        }

        /// <summary>
        /// Get a specific model by it's ID.
        /// </summary>
        /// <param name="model_id"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool GetModel(string model_id, GetModelCallback callback)
        {
            if (string.IsNullOrEmpty(model_id))
                throw new ArgumentNullException("model_id");
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v2/models/");
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
        }

        private void GetModelResponse(RESTConnector.Request r, RESTConnector.Response resp)
        {
            GetModelReq req = r as GetModelReq;
            if (req == null)
                throw new WatsonException("Unexpected Request type.");

            if (resp.Success)
            {
                string jsonData = Encoding.UTF8.GetString(resp.Data);
                IDictionary json = Json.Deserialize(jsonData) as IDictionary;
                Model model = ParseModelJson(json);

                if (req.Callback != null)
                    req.Callback(model);
            }
            else
            {
                Log.Error("Translate", "GetModel() failed: {0}", resp.Error);
                if (req.Callback != null)
                    req.Callback(null);
            }
        }
        #endregion

        #region GetLanguages Functions
        /// <summary>
        /// This function returns a list to the callback of all identifiable languages.
        /// </summary>
        /// <param name="callback">The callback to invoke with a Langage array, null on error.</param>
        /// <returns>Returns true if the request was submitted.</returns>
        public bool GetLanguages(GetLanguagesCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v2/identifiable_languages");
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
        }

        private void GetLanguagesResponse(RESTConnector.Request r, RESTConnector.Response resp)
        {
            GetLanguagesReq req = r as GetLanguagesReq;
            if (req == null)
                throw new WatsonException("Unexpected Request type.");

            if (resp.Success)
            {
                List<Language> languages = new List<Language>();

                IDictionary json = Json.Deserialize(Encoding.UTF8.GetString(resp.Data)) as IDictionary;
                IList ilanguages = json["languages"] as IList;
                foreach (var l in ilanguages)
                {
                    IDictionary ilang = l as IDictionary;

                    Language lang = new Language();
                    lang.Code = (string)ilang["language"];
                    lang.Name = (string)ilang["name"];
                    languages.Add(lang);
                }

                if (req.Callback != null)
                    req.Callback(languages.ToArray());
            }
            else
            {
                Log.Error("Translate", "GetLanguages() failed: {0}", resp.Error);
                if (req.Callback != null)
                    req.Callback(null);
            }
        }
        #endregion

        #region Identify Functions
        public bool Identify(string text, IdentifyCallback callback)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v2/identify");
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
        };

        private void OnIdentifyResponse(RESTConnector.Request r, RESTConnector.Response resp)
        {
            IdentifyReq req = r as IdentifyReq;
            if (req == null)
                throw new WatsonException("Unexpected Request type.");

            if (resp.Success)
            {
                if (req.Callback != null)
                    req.Callback(Encoding.UTF8.GetString(resp.Data));
            }
            else
            {
                Log.Error("Translate", "Identify() failed: {0}", resp.Error);
                if (req.Callback != null)
                    req.Callback(null);
            }
        }
        #endregion

    }

}

