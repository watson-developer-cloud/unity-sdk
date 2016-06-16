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

using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Utilities;
using MiniJSON;
using System;
using System.Collections.Generic;
using System.Text;
using FullSerializer;
using System.Collections;
using System.IO;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.Services.AlchemyLanguage.v1
{
    /// <summary>
    /// Service integration for Alchemy API
    /// </summary>
    public class AlchemyLanguage : IWatsonService
    {

        #region Private Data
        private const string SERVICE_ID = "AlchemyLanguageV1";
        private static string mp_ApiKey = null;

        private static fsSerializer sm_Serializer = new fsSerializer();
        #endregion

        #region SetCredentials
        private void SetCredentials()
        {
            mp_ApiKey = Config.Instance.GetVariableValue("ALCHEMY_API_KEY");

            if (string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("ALCHEMY_API_KEY needs to be defined in config.json");
        }
        #endregion

        #region GetAuthors
        private const string SERVICE_GET_AUTHORS_URL = "/calls/url/URLGetAuthors";
        private const string SERVICE_GET_AUTHORS_HTML = "/calls/html/HTMLGetAuthors";
        public delegate void OnGetAuthors(AuthorsData authorExtractionData, string data);

        public bool GetAuthorsURL(OnGetAuthors callback, string url)
        {
            if(callback == null)
                throw new ArgumentNullException("callback");
            if(string.IsNullOrEmpty(url))
                throw new ArgumentNullException("GetAuthorsURL needs a URL");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_GET_AUTHORS_URL);
            if(connector == null)
                return false;

            GetAuthorsRequest req = new GetAuthorsRequest();
            req.Callback = callback;
            req.Data = url;
            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["url"] = new RESTConnector.Form(url);

            req.OnResponse = OnGetAuthorsResponse;

            return connector.Send(req);
        }

        public bool GetAuthorsHTML(OnGetAuthors callback, string htmlFilePath, string url = default(string))
        {
            if(callback == null)
                throw new ArgumentNullException("callback");
            if(string.IsNullOrEmpty(htmlFilePath))
                throw new ArgumentNullException("GetAuthorsHTML needs a filepath");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            string htmlData = default(string);
            htmlData = File.ReadAllText(htmlFilePath);

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_GET_AUTHORS_HTML);
            if(connector == null)
                return false;

            GetAuthorsRequest req = new GetAuthorsRequest();
            req.Callback = callback;
            req.Data = htmlFilePath;
            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["url"] = url;
            req.Parameters["outputMode"] = "json";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["html"] = new RESTConnector.Form(htmlData);

            req.OnResponse = OnGetAuthorsResponse;

            return connector.Send(req);
        }

        public class GetAuthorsRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetAuthors Callback { get; set; }
        }

        private void OnGetAuthorsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            AuthorsData authorsData = new AuthorsData();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = authorsData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyLanguage", "OnGetAuthorsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetAuthorsRequest)req).Callback != null)
                ((GetAuthorsRequest)req).Callback(resp.Success ? authorsData : null, ((GetAuthorsRequest)req).Data);
        }
        #endregion

        #region GetRankedConcepts
        private const string SERVICE_GET_RANKED_CONCEPTS_HTML = "/calls/html/HTMLGetRankedConcepts";
        private const string SERVICE_GET_RANKED_CONCEPTS_URL = "/calls/url/URLGetRankedConcepts";
        private const string SERVICE_GET_RANKED_CONCEPTS_TEXT = "/calls/text/TextGetRankedConcepts";
        public delegate void OnGetRankedConcepts(ConceptsData conceptExtractionData, string data);

        public bool GetRankedConceptsURL(OnGetRankedConcepts callback, string url, 
            int maxRetrieve = 8, 
            bool includeKnowledgeGraph = false, 
            bool includeLinkedData = true, 
            bool includeSourceText = false)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(url))
                throw new WatsonException("Please provide a URL for GetRankedConceptsURL.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_GET_RANKED_CONCEPTS_URL);
            if(connector == null)
                return false;

            GetRankedConceptsRequest req = new GetRankedConceptsRequest();
            req.Callback = callback;
            req.Data = url;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["url"] = url;
            req.Parameters["maxRetrieve"] = maxRetrieve;
            req.Parameters["knowledgeGraph"] = Convert.ToInt32(includeKnowledgeGraph).ToString();
            req.Parameters["linkedData"] = Convert.ToInt32(includeLinkedData).ToString();
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.OnResponse = OnGetRankedConceptsResponse;
            return connector.Send(req);
        }

        public bool GetRankedConceptsText(OnGetRankedConcepts callback, string text,
            string url = default(string),
            int maxRetrieve = 8, 
            bool includeKnowledgeGraph = false, 
            bool includeLinkedData = true, 
            bool includeSourceText = false)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(text))
                throw new WatsonException("Please provide text for GetRankedConceptsText.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_GET_RANKED_CONCEPTS_TEXT);
            if(connector == null)
                return false;

            GetRankedConceptsRequest req = new GetRankedConceptsRequest();
            req.Callback = callback;
            req.Data = text;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["url"] = url;
            req.Parameters["maxRetrieve"] = maxRetrieve;
            req.Parameters["knowledgeGraph"] = Convert.ToInt32(includeKnowledgeGraph).ToString();
            req.Parameters["linkedData"] = Convert.ToInt32(includeLinkedData).ToString();
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["text"] = new RESTConnector.Form(text);

            req.OnResponse = OnGetRankedConceptsResponse;
            return connector.Send(req);
        }

        public bool GetRankedConceptsHTML(OnGetRankedConcepts callback, string htmlFilePath, 
            string url = default(string),
            int maxRetrieve = 8, 
            bool includeKnowledgeGraph = false, 
            bool includeLinkedData = true, 
            bool includeSourceText = false)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(htmlFilePath))
                throw new WatsonException("Please provide text for GetRankedConceptsHTML.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            string htmlData = default(string);
            htmlData = File.ReadAllText(htmlFilePath);

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_GET_RANKED_CONCEPTS_HTML);
            if(connector == null)
                return false;

            GetRankedConceptsRequest req = new GetRankedConceptsRequest();
            req.Callback = callback;
            req.Data = htmlFilePath;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["url"] = url;
            req.Parameters["maxRetrieve"] = maxRetrieve;
            req.Parameters["knowledgeGraph"] = Convert.ToInt32(includeKnowledgeGraph).ToString();
            req.Parameters["linkedData"] = Convert.ToInt32(includeLinkedData).ToString();
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["html"] = new RESTConnector.Form(htmlData);

            req.OnResponse = OnGetRankedConceptsResponse;
            return connector.Send(req);
        }

        public class GetRankedConceptsRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetRankedConcepts Callback { get; set; }
        }

        private void OnGetRankedConceptsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ConceptsData conceptsData = new ConceptsData();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = conceptsData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyLanguage", "OnGetRankedConceptsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetRankedConceptsRequest)req).Callback != null)
                ((GetRankedConceptsRequest)req).Callback(resp.Success ? conceptsData : null, ((GetRankedConceptsRequest)req).Data);
        }
        #endregion

        #region ExtractDates
        private const string SERVICE_GET_DATES_HTML = "/calls/html/HTMLExtractDates";
        private const string SERVICE_GET_DATES_URL = "/calls/url/URLExtractDates";
        private const string SERVICE_GET_DATES_TEXT = "/calls/text/TextExtractDates";
        public delegate void OnGetDates(DateData dateData, string data);

        public bool GetDatesURL(OnGetDates callback, string url, string anchorDate = default(string), bool includeSourceText = false)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(url))
                throw new WatsonException("Please provide a URL for GetDatesURL.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();
            if(string.IsNullOrEmpty(anchorDate))
                anchorDate = GetCurrentDatetime();

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_GET_DATES_URL);
            if(connector == null)
                return false;

            GetDatesRequest req = new GetDatesRequest();
            req.Callback = callback;
            req.Data = url;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["url"] = new RESTConnector.Form(url);
            req.Forms["anchorDate"] = new RESTConnector.Form(anchorDate);

            req.OnResponse = OnGetDatesResponse;
            return connector.Send(req);
        }

        public bool GetDatesText(OnGetDates callback, string text, string anchorDate = default(string), bool includeSourceText = false)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(text))
                throw new WatsonException("Please provide text for GetDatesText.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();
            if(string.IsNullOrEmpty(anchorDate))
                anchorDate = GetCurrentDatetime();

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_GET_DATES_TEXT);
            if(connector == null)
                return false;

            GetDatesRequest req = new GetDatesRequest();
            req.Callback = callback;
            req.Data = text;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["text"] = new RESTConnector.Form(text);
            req.Forms["anchorDate"] = new RESTConnector.Form(anchorDate);

            req.OnResponse = OnGetDatesResponse;
            return connector.Send(req);
        }

        public bool GetDatesHTML(OnGetDates callback, string htmlFilePath, string anchorDate = default(string), bool includeSourceText = false)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(htmlFilePath))
                throw new WatsonException("Please provide text for GetDatesHTML.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();
            if(string.IsNullOrEmpty(anchorDate))
                anchorDate = GetCurrentDatetime();

            string htmlData = default(string);
            htmlData = File.ReadAllText(htmlFilePath);

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_GET_DATES_HTML);
            if(connector == null)
                return false;

            GetDatesRequest req = new GetDatesRequest();
            req.Callback = callback;
            req.Data = htmlFilePath;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["html"] = new RESTConnector.Form(htmlData);
            req.Forms["anchorDate"] = new RESTConnector.Form(anchorDate);

            req.OnResponse = OnGetDatesResponse;
            return connector.Send(req);
        }

        public class GetDatesRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetDates Callback { get; set; }
        }

        private void OnGetDatesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DateData dateData = new DateData();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = dateData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyLanguage", "OnDatesResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetDatesRequest)req).Callback != null)
                ((GetDatesRequest)req).Callback(resp.Success ? dateData : null, ((GetDatesRequest)req).Data);
        }

        private string GetCurrentDatetime()
        {
            //  date format is yyyy-mm-dd hh:mm:ss
            string dateFormat = "{0}-{1}-{2} {3}:{4}:{5}";
            return string.Format(dateFormat, System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second);
        }
        #endregion

        #region GetEmotion
        private const string SERVICE_GET_EMOTION_HTML = "/calls/html/HTMLGetEmotion";
        private const string SERVICE_GET_EMOTION_URL = "/calls/url/URLGetEmotion";
        private const string SERVICE_GET_EMOTION_TEXT = "/calls/text/TextGetEmotion";
        public delegate void OnGetEmotions(EmotionData emotionData, string data);

        public bool GetEmotions(OnGetEmotions callback, string source, bool includeSourceText = false)
        {
            Log.Debug("AlchemyLanguage", "source: {0}", source);
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetEmotions.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetEmotionsRequest req = new GetEmotionsRequest();
            req.Callback = callback;
            req.Data = source;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if(normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_EMOTION_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if(Path.GetExtension(normalizedSource).EndsWith(".html"))
            {
                service = SERVICE_GET_EMOTION_HTML;
                string htmlData = default(string);
                htmlData = File.ReadAllText(source);
                req.Forms["html"] = new RESTConnector.Form(htmlData);
            }
            else
            {
                service = SERVICE_GET_EMOTION_TEXT;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if(connector == null)
                return false;

            req.OnResponse = OnGetEmotionsResponse;
            return connector.Send(req);
        }

        public class GetEmotionsRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetEmotions Callback { get; set; }
        }

        private void OnGetEmotionsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            EmotionData emotionData = new EmotionData();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = emotionData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyLanguage", "OnGetEmotionsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetEmotionsRequest)req).Callback != null)
                ((GetEmotionsRequest)req).Callback(resp.Success ? emotionData : null, ((GetEmotionsRequest)req).Data);
        }
        #endregion

        #region Entity Extraction
        private const string SERVICE_ENTITY_EXTRACTION = "/calls/text/TextGetRankedNamedEntities";

        public delegate void OnGetEntityExtraction(Entities entityExtractionData, string data);

        public bool GetEntityExtraction(OnGetEntityExtraction callback, string text)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(text))
                throw new WatsonException("GetEntityExtraction needs to have some text to work.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                mp_ApiKey = Config.Instance.GetVariableValue("ALCHEMY_API_KEY");
            if (string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("GetEntityExtraction - ALCHEMY_API_KEY needs to be defined in config.json");


            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_ENTITY_EXTRACTION);
            if (connector == null)
                return false;

            GetEntityExtractionRequest req = new GetEntityExtractionRequest();
            req.Callback = callback;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["text"] = text;
            req.Parameters["url"] = "";
            req.Parameters["outputMode"] = "json";
            //req.Parameters["jsonp"] = "";
            req.Parameters["disambiguate"] = "1";
            req.Parameters["linkedData"] = "1";
            req.Parameters["coreference"] = "1";
            req.Parameters["quotations"] = "0";
            req.Parameters["sentiment"] = "0";
            req.Parameters["showSourceText"] = "1";
            req.Parameters["maxRetrieve"] = "50";
            //req.Parameters["baseUrl"] = "";
            req.Parameters["knowledgeGraph"] = "0";
            req.Parameters["structuredEntities"] = "1";

            req.OnResponse = OnGetEntityExtractionResponse;
            req.Data = text;

            return connector.Send(req);
        }

        private class GetEntityExtractionRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetEntityExtraction Callback { get; set; }
        };

        private void OnGetEntityExtractionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Entities entityExtractionData = new Entities();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = entityExtractionData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyLanguage", "OnGetEntityExtractionResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetEntityExtractionRequest)req).Callback != null)
                ((GetEntityExtractionRequest)req).Callback(resp.Success ? entityExtractionData : null, ((GetEntityExtractionRequest)req).Data);
        }

        #endregion

        #region FeedDetection
        #endregion

        #region Keyword Extraction

        private const string SERVICE_KEYWORD_EXTRACTION = "/calls/text/TextGetRankedKeywords";

        public delegate void OnGetKeywordExtraction(KeywordExtractionData entityExtractionData, string data);

        public bool GetKeywordExtraction(OnGetKeywordExtraction callback, string text, string customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(text))
                throw new WatsonException("GetKeywordExtraction needs to have some text to work.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                mp_ApiKey = Config.Instance.GetVariableValue("ALCHEMY_API_KEY");
            if (string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("GetKeywordExtraction - ALCHEMY_API_KEY needs to be defined in config.json");


            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_KEYWORD_EXTRACTION);
            if (connector == null)
                return false;

            GetKeywordExtractionRequest req = new GetKeywordExtractionRequest();
            req.Callback = callback;

            req.Parameters["apikey"] = mp_ApiKey;
            //req.Parameters["text"] = text;
            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["text"] = new RESTConnector.Form(text);

            req.Parameters["url"] = "";
            req.Parameters["maxRetrieve"] = "1000";
            req.Parameters["keywordExtractMode"] = "strict"; //strict , normal
            req.Parameters["sentiment"] = "1";
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = "1";
            //req.Parameters["baseUrl"] = "";
            req.Parameters["knowledgeGraph"] = "0";

            req.OnResponse = OnGetKeywordExtractionResponse;
            req.Data = string.IsNullOrEmpty(customData) ? text : customData;

            return connector.Send(req);
        }

        private class GetKeywordExtractionRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetKeywordExtraction Callback { get; set; }
        };

        private void OnGetKeywordExtractionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            KeywordExtractionData keywordExtractionData = new KeywordExtractionData();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = keywordExtractionData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyLanguage", "OnGetKeywordExtractionResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetKeywordExtractionRequest)req).Callback != null)
                ((GetKeywordExtractionRequest)req).Callback(resp.Success ? keywordExtractionData : null, ((GetKeywordExtractionRequest)req).Data);
        }

        #endregion

        #region GetLanguage
        #endregion

        #region GetMicroformat
        #endregion

        #region GetPubDate
        #endregion

        #region GetRelations
        #endregion

        #region GetTextSentiment
        #endregion

        #region GetTargetedSentiment
        #endregion

        #region GetRankedTaxonomy
        #endregion

        #region GetText
        #endregion

        #region GetRawText
        #endregion

        #region GetTitle
        #endregion

        #region Combined Call
        
        private const string SERVICE_COMBINED_CALLS = "/calls/text/TextGetCombinedData";

        public delegate void OnGetCombinedCall(CombinedCallData combinedCallData, string data);

        //http://access.alchemyapi.com/calls/text/TextGetRankedNamedEntities

        public bool GetCombinedCall(OnGetCombinedCall callback, string text,
            bool includeEntities = true,
            bool includeKeywords = true,
            bool includeDates = true,
            bool includeTaxonomy = false,
            bool includeConcepts = false,
            bool includeFeeds = false,
            bool includeDocEmotion = false,
            bool includeRelations = false,
            bool includePubDate = false,
            bool includeDocSentiment = false,
            bool includePageImage = false,
            bool includeImageKW = false,
            bool includeAuthors = false,
            bool includeTitle = false,
            string language = "english",
            string customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(text))
                throw new WatsonException("GetCombinedCall needs to have some text to work.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                mp_ApiKey = Config.Instance.GetVariableValue("ALCHEMY_API_KEY");
            if (string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("GetCombinedCall - ALCHEMY_API_KEY needs to be defined in config.json");
            if (!includeEntities
                && !includeKeywords
                && !includeDates
                && !includeTaxonomy
                && !includeConcepts
                && !includeFeeds
                && !includeDocEmotion
                && !includeRelations
                && !includePubDate
                && !includeDocSentiment
                && !includePageImage
                && !includeImageKW)
                throw new WatsonException("GetCombinedCall - There should be some service included.");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_COMBINED_CALLS);
            if (connector == null)
                return false;

            GetCombinedCallRequest req = new GetCombinedCallRequest();
            req.Callback = callback;

            List<string> requestServices = new List<string>();
            if (includeEntities)
                requestServices.Add("entities");
            if (includeKeywords)
                requestServices.Add("keywords");
            if (includeKeywords)
                requestServices.Add("dates");
            if (includeTaxonomy)
                requestServices.Add("taxonomy");
            if (includeConcepts)
                requestServices.Add("concepts");
            if (includeFeeds)
                requestServices.Add("feeds");
            if (includeDocEmotion)
                requestServices.Add("doc-emotion");
            if (includeRelations)
                requestServices.Add("relations");
            if (includePubDate)
                requestServices.Add("pub-date");
            if (includeDocSentiment)
                requestServices.Add("doc-sentiment");
            if (includePageImage)
                requestServices.Add("page-image");
            if (includeImageKW)
                requestServices.Add("image-kw");

            req.Parameters["apikey"] = mp_ApiKey;
            //req.Parameters["text"] = text;
            req.Parameters["extract"] = string.Join(",", requestServices.ToArray());
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = "1";
            req.Parameters["language"] = language;

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["text"] = new RESTConnector.Form(text);

            req.OnResponse = OnGetCombinedCallResponse;
            req.Data = string.IsNullOrEmpty(customData) ? text : customData;

            return connector.Send(req);
        }

        private class GetCombinedCallRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetCombinedCall Callback { get; set; }
        };

        private void OnGetCombinedCallResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            CombinedCallData combinedCallData = new CombinedCallData();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = combinedCallData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyLanguage", "OnGetCombinedCallResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetCombinedCallRequest)req).Callback != null)
                ((GetCombinedCallRequest)req).Callback(resp.Success ? combinedCallData : null, ((GetCombinedCallRequest)req).Data);
        }

        #endregion

        #region IWatsonService interface
        /// <exclude />
        public string GetServiceID()
        {
            return SERVICE_ID;
        }

        /// <exclude />
        public void GetServiceStatus(ServiceStatus callback)
        {
            if (Config.Instance.FindCredentials(SERVICE_ID) != null)
                new CheckServiceStatus(this, callback);
            else
                callback(SERVICE_ID, false);
        }

        private class CheckServiceStatus
        {
            private AlchemyLanguage m_Service = null;
            private ServiceStatus m_Callback = null;

            public CheckServiceStatus(AlchemyLanguage service, ServiceStatus callback)
            {
                m_Service = service;
                m_Callback = callback;

                if (!m_Service.GetEntityExtraction(OnGetEntityExtraction, "Test"))
                    m_Callback(SERVICE_ID, false);
            }

            void OnGetEntityExtraction(Entities entityExtractionData, string data)
            {
                if (m_Callback != null)
                    m_Callback(SERVICE_ID, entityExtractionData != null);
            }

        };
        #endregion
    }
}
