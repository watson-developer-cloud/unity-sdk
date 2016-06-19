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
            else if(Path.GetExtension(normalizedSource).EndsWith(".html") && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
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
        private const string SERVICE_GET_ENTITY_EXTRACTION_HTML = "/calls/html/HTMLGetRankedNamedEntities";
        private const string SERVICE_GET_ENTITY_EXTRACTION_URL = "/calls/url/URLGetRankedNamedEntities";
        private const string SERVICE_GET_ENTITY_EXTRACTION_TEXT = "/calls/text/TextGetRankedNamedEntities";
        public delegate void OnGetEntities(EntityData entityData, string data);

        public bool ExtractEntities(OnGetEntities callback, string source, 
            int maxRetrieve = 50, 
            bool resolveCoreference = true, 
            bool disambiguate = true,
            bool includeKnowledgeGraph = false,
            bool includeLinkedData = true,
            bool includeQuotations = false,
            bool analyzeSentiment = false,
            bool showSourceText = false,
            bool extractStructuredEntities = true)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for ExtractEntities.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetEntitiesRequest req = new GetEntitiesRequest();
            req.Callback = callback;
            req.Data = source;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["maxRetrieve"] = Convert.ToInt32(maxRetrieve).ToString();
            req.Parameters["coreference"] = Convert.ToInt32(resolveCoreference).ToString();
            req.Parameters["disambiguate"] = Convert.ToInt32(disambiguate).ToString();
            req.Parameters["knowledgeGraph"] = Convert.ToInt32(includeKnowledgeGraph).ToString();
            req.Parameters["linkedData"] = Convert.ToInt32(includeLinkedData).ToString();
            req.Parameters["quotations"] = Convert.ToInt32(includeQuotations).ToString();
            req.Parameters["sentiment"] = Convert.ToInt32(analyzeSentiment).ToString();
            req.Parameters["showSourceText"] = Convert.ToInt32(showSourceText).ToString();
            req.Parameters["structuredEntities"] = Convert.ToInt32(extractStructuredEntities).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if(normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_ENTITY_EXTRACTION_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if(Path.GetExtension(normalizedSource).EndsWith(".html") && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_ENTITY_EXTRACTION_HTML;
                string htmlData = default(string);
                htmlData = File.ReadAllText(source);
                req.Forms["html"] = new RESTConnector.Form(htmlData);
            }
            else
            {
                service = SERVICE_GET_ENTITY_EXTRACTION_TEXT;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if(connector == null)
                return false;

            req.OnResponse = OnGetEntitiesResponse;
            return connector.Send(req);
        }

        public class GetEntitiesRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetEntities Callback { get; set; }
        }

        private void OnGetEntitiesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            EntityData entityData = new EntityData();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = entityData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyLanguage", "OnGetEntitiesResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetEntitiesRequest)req).Callback != null)
                ((GetEntitiesRequest)req).Callback(resp.Success ? entityData : null, ((GetEntitiesRequest)req).Data);
        }
        #endregion

        #region FeedDetection
        private const string SERVICE_DETECT_FEEDS_URL = "/calls/url/URLGetFeedLinks";
        public delegate void OnDetectFeeds(FeedData feedData, string data);

        public bool DetectFeeds(OnDetectFeeds callback, string url)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(url))
                throw new WatsonException("Please provide a url for GetEmotions.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            DetectFeedsRequest req = new DetectFeedsRequest();
            req.Callback = callback;
            req.Data = url;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["url"] = new RESTConnector.Form(url);

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_DETECT_FEEDS_URL);
            if(connector == null)
                return false;

            req.OnResponse = OnDetectFeedsResponse;
            return connector.Send(req);
        }

        public class DetectFeedsRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnDetectFeeds Callback { get; set; }
        }

        private void OnDetectFeedsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            FeedData feedData = new FeedData();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = feedData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyLanguage", "OnDetectFeedsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((DetectFeedsRequest)req).Callback != null)
                ((DetectFeedsRequest)req).Callback(resp.Success ? feedData : null, ((DetectFeedsRequest)req).Data);
        }
        #endregion

        #region Keyword Extraction
        private const string SERVICE_GET_KEYWORD_EXTRACTION_HTML = "/calls/html/HTMLGetRankedKeywords";
        private const string SERVICE_GET_KEYWORD_EXTRACTION_URL = "/calls/url/URLGetRankedKeywords";
        private const string SERVICE_GET_KEYWORD_EXTRACTION_TEXT = "/calls/text/TextGetRankedKeywords";
        public delegate void OnGetKeywords(KeywordData keywordData, string data);

        public bool ExtractKeywords(OnGetKeywords callback, string source, 
            int maxRetrieve = 50, 
            bool includeKnowledgeGraph = false,
            bool analyzeSentiment = false,
            bool showSourceText = false)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for ExtractKeywords.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetKeywordsRequest req = new GetKeywordsRequest();
            req.Callback = callback;
            req.Data = source;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["maxRetrieve"] = Convert.ToInt32(maxRetrieve).ToString();
            req.Parameters["knowledgeGraph"] = Convert.ToInt32(includeKnowledgeGraph).ToString();
            req.Parameters["sentiment"] = Convert.ToInt32(analyzeSentiment).ToString();
            req.Parameters["showSourceText"] = Convert.ToInt32(showSourceText).ToString();
            req.Parameters["keywordExtractMode"] = "strict";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if(normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_KEYWORD_EXTRACTION_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if(Path.GetExtension(normalizedSource).EndsWith(".html") && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_KEYWORD_EXTRACTION_HTML;
                string htmlData = default(string);
                htmlData = File.ReadAllText(source);
                req.Forms["html"] = new RESTConnector.Form(htmlData);
            }
            else
            {
                service = SERVICE_GET_KEYWORD_EXTRACTION_TEXT;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if(connector == null)
                return false;

            req.OnResponse = OnGetKeywordsResponse;
            return connector.Send(req);
        }

        public class GetKeywordsRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetKeywords Callback { get; set; }
        }

        private void OnGetKeywordsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            KeywordData keywordData = new KeywordData();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = keywordData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyLanguage", "OnGetKeywordsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetKeywordsRequest)req).Callback != null)
                ((GetKeywordsRequest)req).Callback(resp.Success ? keywordData : null, ((GetKeywordsRequest)req).Data);
        }
        #endregion

        #region GetLanguage
        private const string SERVICE_GET_LANGUAGE_HTML = "/calls/html/HTMLGetLanguage";
        private const string SERVICE_GET_LANGUAGE_URL = "/calls/url/URLGetLanguage";
        private const string SERVICE_GET_LANGUAGE_TEXT = "/calls/text/TextGetLanguage";
        public delegate void OnGetLanguages(LanguageData languageData, string data);

        public bool GetLanguages(OnGetLanguages callback, string source, bool includeSourceText = false)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetLanguages.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetLanguagesRequest req = new GetLanguagesRequest();
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
                service = SERVICE_GET_LANGUAGE_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if(Path.GetExtension(normalizedSource).EndsWith(".html") && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_LANGUAGE_HTML;
                string htmlData = default(string);
                htmlData = File.ReadAllText(source);
                req.Forms["html"] = new RESTConnector.Form(htmlData);
            }
            else
            {
                service = SERVICE_GET_LANGUAGE_TEXT;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if(connector == null)
                return false;

            req.OnResponse = OnGetLanguagesResponse;
            return connector.Send(req);
        }

        public class GetLanguagesRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetLanguages Callback { get; set; }
        }

        private void OnGetLanguagesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            LanguageData languageData = new LanguageData();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = languageData;
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

            if (((GetLanguagesRequest)req).Callback != null)
                ((GetLanguagesRequest)req).Callback(resp.Success ? languageData : null, ((GetLanguagesRequest)req).Data);
        } 
        #endregion

        #region GetMicroformat
        private const string SERVICE_GET_MICROFORMAT_URL = "/calls/url/URLGetMicroformatData";
        private const string SERVICE_GET_MICROFORMAT_HTML = "/calls/html/HTMLGetMicroformatData";
        public delegate void OnGetMicroformats(MicroformatData microformatData, string data);

        public bool GetMicroformats(OnGetMicroformats callback, string source)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a url for GetMicroformats.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetMicroformatsRequest req = new GetMicroformatsRequest();
            req.Callback = callback;
            req.Data = source;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service = "";
            string normalizedSource = source.Trim().ToLower();
            if(normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_MICROFORMAT_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if(Path.GetExtension(normalizedSource).EndsWith(".html") && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_MICROFORMAT_HTML;
                string htmlData = default(string);
                htmlData = File.ReadAllText(source);
                req.Forms["html"] = new RESTConnector.Form(htmlData);
            }
            else
            {
                Log.Error("Alchemy Language", "Either a URL or a html page source is required for GetMicroformats!");
                return false;
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if(connector == null)
                return false;

            req.OnResponse = OnGetMicroformatsResponse;
            return connector.Send(req);
        }

        public class GetMicroformatsRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetMicroformats Callback { get; set; }
        }

        private void OnGetMicroformatsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            MicroformatData microformatData = new MicroformatData();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = microformatData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyLanguage", "OnGetMicroformatsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetMicroformatsRequest)req).Callback != null)
                ((GetMicroformatsRequest)req).Callback(resp.Success ? microformatData : null, ((GetMicroformatsRequest)req).Data);
        }
        #endregion

        #region GetPubDate
        private const string SERVICE_GET_PUBLICATION_DATE_URL = "/calls/url/URLGetPubDate";
        //        private const string SERVICE_GET_PUBLICATION_DATE_HTML = "/calls/html/HTMLGetPubDate";
        public delegate void OnGetPublicationDate(PubDateData pubDateData, string data);

        public bool GetPublicationDate(OnGetPublicationDate callback, string source)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a url for GetPublicationDate.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetPublicationDateRequest req = new GetPublicationDateRequest();
            req.Callback = callback;
            req.Data = source;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service = "";
            string normalizedSource = source.Trim().ToLower();
            if(normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_PUBLICATION_DATE_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if(Path.GetExtension(normalizedSource).EndsWith(".html") && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                Log.Error("AlchemyLanguage", "PublicationDate by HTML is not supported!");
                return false;
                //                service = SERVICE_GET_PUBLICATION_DATE_HTML;
                //                string htmlData = default(string);
                //                htmlData = File.ReadAllText(source);
                //                req.Forms["html"] = new RESTConnector.Form(htmlData);
            }
            else
            {
                Log.Error("Alchemy Language", "Either a URL or a html page source is required for GetPublicationDate!");
                return false;
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if(connector == null)
                return false;

            req.OnResponse = OnGetPublicationDateResponse;
            return connector.Send(req);
        }

        public class GetPublicationDateRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetPublicationDate Callback { get; set; }
        }

        private void OnGetPublicationDateResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            PubDateData pubDateData = new PubDateData();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = pubDateData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyLanguage", "OnGetPublicationDateResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetPublicationDateRequest)req).Callback != null)
                ((GetPublicationDateRequest)req).Callback(resp.Success ? pubDateData : null, ((GetPublicationDateRequest)req).Data);
        }
        #endregion

        #region GetRelations
        private const string SERVICE_GET_RELATIONS_HTML = "/calls/html/HTMLGetRelations";
        private const string SERVICE_GET_RELATIONS_URL = "/calls/url/URLGetRelations";
        private const string SERVICE_GET_RELATIONS_TEXT = "/calls/text/TextGetRelations";
        public delegate void OnGetRelations(RelationsData relationData, string data);

        public bool GetRelations(OnGetRelations callback, string source, 
            int maxRetrieve = 50, 
            bool includeKeywords = false,
            bool includeEntities = false,
            bool requireEntities = false,
            bool resolveCoreferences = true,
            bool disambiguateEntities = true,
            bool includeKnowledgeGraph = false,
            bool includeLinkedData = true,
            bool analyzeSentiment = false,
            bool excludeEntitiesInSentiment = false,
            bool showSourceText = false)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetRelations.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetRelationsRequest req = new GetRelationsRequest();
            req.Callback = callback;
            req.Data = source;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["maxRetrieve"] = Convert.ToInt32(maxRetrieve).ToString();
            req.Parameters["keywords"] = Convert.ToInt32(includeKeywords).ToString();
            req.Parameters["entities"] = Convert.ToInt32(includeEntities).ToString();
            req.Parameters["requireEntities"] = Convert.ToInt32(requireEntities).ToString();
            req.Parameters["coreference"] = Convert.ToInt32(resolveCoreferences).ToString();
            req.Parameters["disambiguate"] = Convert.ToInt32(disambiguateEntities).ToString();
            req.Parameters["knowledgeGraph"] = Convert.ToInt32(includeKnowledgeGraph).ToString();
            req.Parameters["linkedData"] = Convert.ToInt32(includeLinkedData).ToString();
            req.Parameters["sentiment"] = Convert.ToInt32(analyzeSentiment).ToString();
            req.Parameters["sentimentExcludeEntities"] = Convert.ToInt32(excludeEntitiesInSentiment).ToString();
            req.Parameters["showSourceText"] = Convert.ToInt32(showSourceText).ToString();
            req.Parameters["keywordExtractMode"] = "strict";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if(normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_RELATIONS_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if(Path.GetExtension(normalizedSource).EndsWith(".html") && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_RELATIONS_HTML;
                string htmlData = default(string);
                htmlData = File.ReadAllText(source);
                req.Forms["html"] = new RESTConnector.Form(htmlData);
            }
            else
            {
                service = SERVICE_GET_RELATIONS_TEXT;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if(connector == null)
                return false;

            req.OnResponse = OnGetRelationsResponse;
            return connector.Send(req);
        }

        public class GetRelationsRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetRelations Callback { get; set; }
        }

        private void OnGetRelationsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            RelationsData relationsData = new RelationsData();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = relationsData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyLanguage", "OnGetRelationsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetRelationsRequest)req).Callback != null)
                ((GetRelationsRequest)req).Callback(resp.Success ? relationsData : null, ((GetRelationsRequest)req).Data);
        }
        #endregion

        #region GetTextSentiment
        private const string SERVICE_GET_TEXT_SENTIMENT_HTML = "/calls/html/HTMLGetTextSentiment";
        private const string SERVICE_GET_TEXT_SENTIMENT_URL = "/calls/url/URLGetTextSentiment";
        private const string SERVICE_GET_TEXT_SENTIMENT_TEXT = "/calls/text/TextGetTextSentiment";
        public delegate void OnGetTextSentiment(SentimentData sentimentData, string data);

        public bool GetTextSentiment(OnGetTextSentiment callback, string source, bool showSourceText = false)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetTextSentiment.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetTextSentimentRequest req = new GetTextSentimentRequest();
            req.Callback = callback;
            req.Data = source;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(showSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if(normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_TEXT_SENTIMENT_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if(Path.GetExtension(normalizedSource).EndsWith(".html") && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_TEXT_SENTIMENT_HTML;
                string htmlData = default(string);
                htmlData = File.ReadAllText(source);
                req.Forms["html"] = new RESTConnector.Form(htmlData);
            }
            else
            {
                service = SERVICE_GET_TEXT_SENTIMENT_TEXT;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if(connector == null)
                return false;

            req.OnResponse = OnGetTextSentimentResponse;
            return connector.Send(req);
        }

        public class GetTextSentimentRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetTextSentiment Callback { get; set; }
        }

        private void OnGetTextSentimentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            SentimentData sentimentData = new SentimentData();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = sentimentData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyLanguage", "OnGetTextSentimentResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetTextSentimentRequest)req).Callback != null)
                ((GetTextSentimentRequest)req).Callback(resp.Success ? sentimentData : null, ((GetTextSentimentRequest)req).Data);
        }
        #endregion

        #region GetTargetedSentiment
        private const string SERVICE_GET_TARGETED_SENTIMENT_HTML = "/calls/html/HTMLGetTargetedSentiment";
        private const string SERVICE_GET_TARGETED_SENTIMENT_URL = "/calls/url/URLGetTargetedSentiment";
        private const string SERVICE_GET_TARGETED_SENTIMENT_TEXT = "/calls/text/TextGetTargetedSentiment";
        public delegate void OnGetTargetedSentiment(TargetedSentimentData targetedSentimentData, string data);

        public bool GetTargetedSentiment(OnGetTargetedSentiment callback, string source, string targets, bool showSourceText = false)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetTargetedSentiment.");
            if(string.IsNullOrEmpty(targets))
                throw new WatsonException("Please provide a target for GetTargetedSentiment.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetTargetedSentimentRequest req = new GetTargetedSentimentRequest();
            req.Callback = callback;
            req.Data = source;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(showSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["targets"] = new RESTConnector.Form(targets);

            string service;
            string normalizedSource = source.Trim().ToLower();
            if(normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_TARGETED_SENTIMENT_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if(Path.GetExtension(normalizedSource).EndsWith(".html") && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_TARGETED_SENTIMENT_HTML;
                string htmlData = default(string);
                htmlData = File.ReadAllText(source);
                req.Forms["html"] = new RESTConnector.Form(htmlData);
            }
            else
            {
                service = SERVICE_GET_TARGETED_SENTIMENT_TEXT;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if(connector == null)
                return false;

            req.OnResponse = OnGetTargetedSentimentResponse;
            return connector.Send(req);
        }

        public class GetTargetedSentimentRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetTargetedSentiment Callback { get; set; }
        }

        private void OnGetTargetedSentimentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            TargetedSentimentData sentimentData = new TargetedSentimentData();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = sentimentData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyLanguage", "OnGetTargetedSentimentResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetTargetedSentimentRequest)req).Callback != null)
                ((GetTargetedSentimentRequest)req).Callback(resp.Success ? sentimentData : null, ((GetTargetedSentimentRequest)req).Data);
        }
        #endregion

        #region GetRankedTaxonomy
        private const string SERVICE_GET_RANKED_TAXONOMY_HTML = "/calls/html/HTMLGetRankedTaxonomy";
        private const string SERVICE_GET_RANKED_TAXONOMY_URL = "/calls/url/URLGetRankedTaxonomy";
        private const string SERVICE_GET_RANKED_TAXONOMY_TEXT = "/calls/text/TextGetRankedTaxonomy";
        public delegate void OnGetRankedTaxonomy(TaxonomyData taxonomyData, string data);

        public bool GetRankedTaxonomy(OnGetRankedTaxonomy callback, string source, bool showSourceText = false)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetRankedTaxonomy.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetRankedTaxomomyRequest req = new GetRankedTaxomomyRequest();
            req.Callback = callback;
            req.Data = source;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(showSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if(normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_RANKED_TAXONOMY_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if(Path.GetExtension(normalizedSource).EndsWith(".html") && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_RANKED_TAXONOMY_HTML;
                string htmlData = default(string);
                htmlData = File.ReadAllText(source);
                req.Forms["html"] = new RESTConnector.Form(htmlData);
            }
            else
            {
                service = SERVICE_GET_RANKED_TAXONOMY_TEXT;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if(connector == null)
                return false;

            req.OnResponse = OnGetRankedTaxonomyResponse;
            return connector.Send(req);
        }

        public class GetRankedTaxomomyRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetRankedTaxonomy Callback { get; set; }
        }

        private void OnGetRankedTaxonomyResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            TaxonomyData taxonomyData = new TaxonomyData();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = taxonomyData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyLanguage", "OnGetRankedTaxonomyResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetRankedTaxomomyRequest)req).Callback != null)
                ((GetRankedTaxomomyRequest)req).Callback(resp.Success ? taxonomyData : null, ((GetRankedTaxomomyRequest)req).Data);
        }
        #endregion

        #region GetText
        private const string SERVICE_GET_TEXT_HTML = "/calls/html/HTMLGetText";
        private const string SERVICE_GET_TEXT_URL = "/calls/url/URLGetText";
        public delegate void OnGetText(TextData textData, string data);

        public bool GetText(OnGetText callback, string source, bool extractLinks = false, bool useMetadata = true)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetText.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetTextRequest req = new GetTextRequest();
            req.Callback = callback;
            req.Data = source;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["extractLinks"] = Convert.ToInt32(extractLinks).ToString();
            req.Parameters["useMetadata"] = Convert.ToInt32(useMetadata).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service = "";
            string normalizedSource = source.Trim().ToLower();
            if(normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_TEXT_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if(Path.GetExtension(normalizedSource).EndsWith(".html") && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_TEXT_HTML;
                string htmlData = default(string);
                htmlData = File.ReadAllText(source);
                req.Forms["html"] = new RESTConnector.Form(htmlData);
            }
            else
            {
                Log.Error("Alchemy Language", "Either a URL or a html page source is required for GetText!");
                return false;
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if(connector == null)
                return false;

            req.OnResponse = OnGetTextResponse;
            return connector.Send(req);
        }

        public class GetTextRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetText Callback { get; set; }
        }

        private void OnGetTextResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            TextData textData = new TextData();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = textData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyLanguage", "OnGetTextResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetTextRequest)req).Callback != null)
                ((GetTextRequest)req).Callback(resp.Success ? textData : null, ((GetTextRequest)req).Data);
        }
        #endregion

        #region GetRawText
        private const string SERVICE_GET_RAW_TEXT_HTML = "/calls/html/HTMLGetRawText";
        private const string SERVICE_GET_RAW_TEXT_URL = "/calls/url/URLGetRawText";

        public bool GetRawText(OnGetText callback, string source)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetRawText.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetTextRequest req = new GetTextRequest();
            req.Callback = callback;
            req.Data = source;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service = "";
            string normalizedSource = source.Trim().ToLower();
            if(normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_TEXT_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if(Path.GetExtension(normalizedSource).EndsWith(".html") && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_TEXT_HTML;
                string htmlData = default(string);
                htmlData = File.ReadAllText(source);
                req.Forms["html"] = new RESTConnector.Form(htmlData);
            }
            else
            {
                Log.Error("Alchemy Language", "Either a URL or a html page source is required for GetText!");
                return false;
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if(connector == null)
                return false;

            req.OnResponse = OnGetTextResponse;
            return connector.Send(req);
        }
        #endregion

        #region GetTitle
        private const string SERVICE_GET_TITLE_HTML = "/calls/html/HTMLGetTitle";
        private const string SERVICE_GET_TITLE_URL = "/calls/url/URLGetTitle";
        public delegate void OnGetTitle(Title titleData, string data);

        public bool GetTitle(OnGetTitle callback, string source, bool useMetadata = true)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetText.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetTitleRequest req = new GetTitleRequest();
            req.Callback = callback;
            req.Data = source;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["useMetadata"] = Convert.ToInt32(useMetadata).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service = "";
            string normalizedSource = source.Trim().ToLower();
            if(normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_TITLE_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if(Path.GetExtension(normalizedSource).EndsWith(".html") && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_TITLE_HTML;
                string htmlData = default(string);
                htmlData = File.ReadAllText(source);
                req.Forms["html"] = new RESTConnector.Form(htmlData);
            }
            else
            {
                Log.Error("Alchemy Language", "Either a URL or a html page source is required for GetTitle!");
                return false;
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if(connector == null)
                return false;

            req.OnResponse = OnGetTitleResponse;
            return connector.Send(req);
        }

        public class GetTitleRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetTitle Callback { get; set; }
        }

        private void OnGetTitleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Title titleData = new Title();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = titleData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyLanguage", "OnGetTitleResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetTitleRequest)req).Callback != null)
                ((GetTitleRequest)req).Callback(resp.Success ? titleData : null, ((GetTitleRequest)req).Data);
        }
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

                if (!m_Service.ExtractEntities(OnGetEntityExtraction, "Test"))
                    m_Callback(SERVICE_ID, false);
            }

            void OnGetEntityExtraction(EntityData entityExtractionData, string data)
            {
                if (m_Callback != null)
                    m_Callback(SERVICE_ID, entityExtractionData != null);
            }

        };
        #endregion
    }
}
