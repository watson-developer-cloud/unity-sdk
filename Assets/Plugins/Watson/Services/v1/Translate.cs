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
        public delegate void GetLanguagesCallback(Language[] languages);
        public delegate void IdentifyCallback(string languages);
        public delegate void TranslateCallback(Translation translation);

        #endregion

        #region Private Data
        private RESTConnector m_Connector = null;
        private const string SERVICE_ID = "TranslateV1";
        #endregion

        #region Public Properties
        #endregion

        private bool CreateConnector()
        {
            if (m_Connector == null)
            {
                Config.CredentialsInfo info = Config.Instance.FindCredentials(SERVICE_ID);
                if (info == null)
                {
                    Log.Error("Translate", "Unable to find credentials for service ID: {0}", SERVICE_ID);
                    return false;
                }

                m_Connector = new RESTConnector();
                m_Connector.Authentication = info;
            }
            return true;
        }

        #region GetTranslation Functions
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
            if (!CreateConnector())
                return false;

            TranslateReq req = new TranslateReq();
            req.Callback = callback;
            req.OnResponse = TranslateResponse;
            req.Function = "/v2/translate";
            req.Send = Encoding.UTF8.GetBytes( json );

            req.Headers["accept"] = "application/json";
            req.Headers["Content-Type"] = "application/json";

            return m_Connector.Send(req);
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

            if (resp.Success)
            {
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

                if (req.Callback != null)
                    req.Callback(translation);
            }
            else
            {
                Log.Error("Translate", "GetTranslation() failed: {0}", resp.Error);
                if (req.Callback != null)
                    req.Callback(null);
            }
        }
        #endregion

        #region GetModels Functions

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
            if (!CreateConnector())
                return false;

            GetLanguagesReq req = new GetLanguagesReq();
            req.Callback = callback;
            req.Function = "/v2/identifiable_languages";
            req.OnResponse = GetLanguagesResponse;

            return m_Connector.Send(req);
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

            IdentifyReq req = new IdentifyReq();
            req.Callback = callback;
            req.Function = "/v2/identify";
            req.Send = Encoding.UTF8.GetBytes(text);
            req.Headers["Content-Type"] = "text/plain";
            req.OnResponse = OnIdentifyResponse;

            return m_Connector.Send(req);
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

