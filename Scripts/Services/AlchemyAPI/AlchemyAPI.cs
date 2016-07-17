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

namespace IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1
{
	/// <summary>
	/// This class wraps the Alchemy API Services.
	/// <a href="http://www.ibm.com/watson/developercloud/alchemy-language.html">Alchemy Language</a>
	/// <a href="http://www.ibm.com/watson/developercloud/alchemy-data-news.html">AlchemyData News</a>
	/// </summary>
	public class AlchemyAPI : IWatsonService
    {

        #region Private Data
        private const string SERVICE_ID = "AlchemyAPIV1";
        private static string mp_ApiKey = null;

        private static fsSerializer sm_Serializer = new fsSerializer();
        #endregion

        #region SetCredentials
        private void SetCredentials()
        {
            mp_ApiKey = Config.Instance.GetAPIKey(SERVICE_ID);

            if (string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("Alchemy API Key required in config.json");
        }
        #endregion

        #region GetAuthors
        private const string SERVICE_GET_AUTHORS_URL = "/url/URLGetAuthors";
        private const string SERVICE_GET_AUTHORS_HTML = "/html/HTMLGetAuthors";

        /// <summary>
        /// On get authors delegate.
        /// </summary>
        public delegate void OnGetAuthors(AuthorsData authorExtractionData, string data);

        /// <summary>
        /// Extracts authors from a source.
        /// </summary>
        /// <returns><c>true</c>, if authors was gotten, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="source">URL or HTML source.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetAuthors(OnGetAuthors callback, string source, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("Please provide a source for GetAuthors.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetAuthorsRequest req = new GetAuthorsRequest();
            req.Callback = callback;
            req.Data = string.IsNullOrEmpty(customData) ? source : customData;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_AUTHORS_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = SERVICE_GET_AUTHORS_HTML;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetAuthors");
            }
            else
            {
                Log.Error("Alchemy Language", "Either a URL or a html page source is required for GetAuthors!");
                return false;
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if (connector == null)
                return false;

            req.OnResponse = OnGetAuthorsResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Get authors request.
        /// </summary>
        public class GetAuthorsRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
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
        private const string SERVICE_GET_RANKED_CONCEPTS_HTML = "/html/HTMLGetRankedConcepts";
        private const string SERVICE_GET_RANKED_CONCEPTS_URL = "/url/URLGetRankedConcepts";
        private const string SERVICE_GET_RANKED_CONCEPTS_TEXT = "/text/TextGetRankedConcepts";

        /// <summary>
        /// On get ranked concepts delegate.
        /// </summary>
        public delegate void OnGetRankedConcepts(ConceptsData conceptExtractionData, string data);

        /// <summary>
        /// Extracts concepts from a source.
        /// </summary>
        /// <returns><c>true</c>, if ranked concepts was gotten, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="source">HTML, URL or Text Source.</param>
        /// <param name="maxRetrieve">Maximum results retreived.</param>
        /// <param name="includeKnowledgeGraph">If set to <c>true</c> include knowledge graph.</param>
        /// <param name="includeLinkedData">If set to <c>true</c> include linked data.</param>
        /// <param name="includeSourceText">If set to <c>true</c> include source text.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetRankedConcepts(OnGetRankedConcepts callback, string source,
            int maxRetrieve = 8,
            bool includeKnowledgeGraph = false,
            bool includeLinkedData = true,
            bool includeSourceText = false,
            string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetAuthors.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetRankedConceptsRequest req = new GetRankedConceptsRequest();
            req.Callback = callback;
            req.Data = string.IsNullOrEmpty(customData) ? source : customData;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["maxRetrieve"] = maxRetrieve;
            req.Parameters["knowledgeGraph"] = Convert.ToInt32(includeKnowledgeGraph).ToString();
            req.Parameters["linkedData"] = Convert.ToInt32(includeLinkedData).ToString();
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_RANKED_CONCEPTS_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (!normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://") && source.StartsWith(Application.dataPath))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = SERVICE_GET_RANKED_CONCEPTS_HTML;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetRankedConcepts");
            }
            else
            {
                service = SERVICE_GET_RANKED_CONCEPTS_TEXT;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if (connector == null)
                return false;

            req.OnResponse = OnGetRankedConceptsResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Get ranked concepts request.
        /// </summary>
        public class GetRankedConceptsRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
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
        private const string SERVICE_GET_DATES_HTML = "/html/HTMLExtractDates";
        private const string SERVICE_GET_DATES_URL = "/url/URLExtractDates";
        private const string SERVICE_GET_DATES_TEXT = "/text/TextExtractDates";

        /// <summary>
        /// On get dates delegate.
        /// </summary>
        public delegate void OnGetDates(DateData dateData, string data);

        /// <summary>
        /// Extracts dates from a source.
        /// </summary>
        /// <returns><c>true</c>, if dates was gotten, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="source">HTML, URL or Text source.</param>
        /// <param name="anchorDate">Anchor date in the yyyy-mm-dd hh:mm:ss format. If this is not set, anchor date is set to today's date and time.</param>
        /// <param name="includeSourceText">If set to <c>true</c> include source text.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetDates(OnGetDates callback, string source, string anchorDate = default(string), bool includeSourceText = false, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetAuthors.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();
            if (string.IsNullOrEmpty(anchorDate))
                anchorDate = GetCurrentDatetime();

            GetDatesRequest req = new GetDatesRequest();
            req.Callback = callback;
            req.Data = string.IsNullOrEmpty(customData) ? source : customData;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["anchorDate"] = new RESTConnector.Form(anchorDate);

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_DATES_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = SERVICE_GET_DATES_HTML;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for ExtractDates!");
            }
            else
            {
                service = SERVICE_GET_DATES_TEXT;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if (connector == null)
                return false;

            req.OnResponse = OnGetDatesResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Get dates request.
        /// </summary>
        public class GetDatesRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
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
        private const string SERVICE_GET_EMOTION_HTML = "/html/HTMLGetEmotion";
        private const string SERVICE_GET_EMOTION_URL = "/url/URLGetEmotion";
        private const string SERVICE_GET_EMOTION_TEXT = "/text/TextGetEmotion";

        /// <summary>
        /// On get emotions delegate.
        /// </summary>
        public delegate void OnGetEmotions(EmotionData emotionData, string data);

        /// <summary>
        /// Gets the emotions from a source.
        /// </summary>
        /// <returns><c>true</c>, if emotions was gotten, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="source">HTML, URL or Text source.</param>
        /// <param name="includeSourceText">If set to <c>true</c> include source text.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetEmotions(OnGetEmotions callback, string source, bool includeSourceText = false, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetEmotions.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetEmotionsRequest req = new GetEmotionsRequest();
            req.Callback = callback;
            req.Data = string.IsNullOrEmpty(customData) ? source : customData;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_EMOTION_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = SERVICE_GET_EMOTION_HTML;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetEmotions!");
            }
            else
            {
                service = SERVICE_GET_EMOTION_TEXT;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if (connector == null)
                return false;

            req.OnResponse = OnGetEmotionsResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Get emotions request.
        /// </summary>
        public class GetEmotionsRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
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
        private const string SERVICE_GET_ENTITY_EXTRACTION_HTML = "/html/HTMLGetRankedNamedEntities";
        private const string SERVICE_GET_ENTITY_EXTRACTION_URL = "/url/URLGetRankedNamedEntities";
        private const string SERVICE_GET_ENTITY_EXTRACTION_TEXT = "/text/TextGetRankedNamedEntities";

        /// <summary>
        /// On get entities delegate.
        /// </summary>
        public delegate void OnGetEntities(EntityData entityData, string data);

        /// <summary>
        /// Extracts the entities from a source.
        /// </summary>
        /// <returns><c>true</c>, if entities was extracted, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="source">HTML, URL or Text source.</param>
        /// <param name="maxRetrieve">Maximum results retreived.</param>
        /// <param name="resolveCoreference">If set to <c>true</c> resolve coreference.</param>
        /// <param name="disambiguate">If set to <c>true</c> disambiguate.</param>
        /// <param name="includeKnowledgeGraph">If set to <c>true</c> include knowledge graph.</param>
        /// <param name="includeLinkedData">If set to <c>true</c> include linked data.</param>
        /// <param name="includeQuotations">If set to <c>true</c> include quotations.</param>
        /// <param name="analyzeSentiment">If set to <c>true</c> analyze sentiment.</param>
        /// <param name="includeSourceText">If set to <c>true</c> include source text.</param>
        /// <param name="extractStructuredEntities">If set to <c>true</c> extract structured entities.</param>
        /// <param name="customData">Custom data.</param>
        public bool ExtractEntities(OnGetEntities callback, string source,
            int maxRetrieve = 50,
            bool resolveCoreference = true,
            bool disambiguate = true,
            bool includeKnowledgeGraph = false,
            bool includeLinkedData = true,
            bool includeQuotations = false,
            bool analyzeSentiment = false,
            bool includeSourceText = false,
            bool extractStructuredEntities = true,
            string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for ExtractEntities.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetEntitiesRequest req = new GetEntitiesRequest();
            req.Callback = callback;
            req.Data = string.IsNullOrEmpty(customData) ? source : customData;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["maxRetrieve"] = Convert.ToInt32(maxRetrieve).ToString();
            req.Parameters["coreference"] = Convert.ToInt32(resolveCoreference).ToString();
            req.Parameters["disambiguate"] = Convert.ToInt32(disambiguate).ToString();
            req.Parameters["knowledgeGraph"] = Convert.ToInt32(includeKnowledgeGraph).ToString();
            req.Parameters["linkedData"] = Convert.ToInt32(includeLinkedData).ToString();
            req.Parameters["quotations"] = Convert.ToInt32(includeQuotations).ToString();
            req.Parameters["sentiment"] = Convert.ToInt32(analyzeSentiment).ToString();
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();
            req.Parameters["structuredEntities"] = Convert.ToInt32(extractStructuredEntities).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_ENTITY_EXTRACTION_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = SERVICE_GET_ENTITY_EXTRACTION_HTML;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for ExtractEntities!");
            }
            else
            {
                service = SERVICE_GET_ENTITY_EXTRACTION_TEXT;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if (connector == null)
                return false;

            req.OnResponse = OnGetEntitiesResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Get entities request.
        /// </summary>
        public class GetEntitiesRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
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
        private const string SERVICE_DETECT_FEEDS_URL = "/url/URLGetFeedLinks";
        private const string SERVICE_DETECT_FEEDS_HTML = "/html/HTMLGetFeedLinks";

        /// <summary>
        /// On detect feeds delegate.
        /// </summary>
        public delegate void OnDetectFeeds(FeedData feedData, string data);

        /// <summary>
        /// Detects RSS feeds in a source.
        /// </summary>
        /// <returns><c>true</c>, if feeds was detected, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="source">URL to detect feeds.</param>
        /// <param name="customData">Custom data.</param>
        public bool DetectFeeds(OnDetectFeeds callback, string source, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetEmotions.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            DetectFeedsRequest req = new DetectFeedsRequest();
            req.Callback = callback;
            req.Data = string.IsNullOrEmpty(customData) ? source : customData;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_DETECT_FEEDS_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                Log.Error("Alchemy Language", "A URL source is required for DetectFeeds!");
                return false;
                //                service = SERVICE_DETECT_FEEDS_HTML;
                //                string htmlData = default(string);
                //                htmlData = File.ReadAllText(source);
                //                req.Forms["html"] = new RESTConnector.Form(htmlData);
            }
            else
            {
                Log.Error("Alchemy Language", "A URL source is required for DetectFeeds!");
                return false;
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if (connector == null)
                return false;

            req.OnResponse = OnDetectFeedsResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Detect feeds request.
        /// </summary>
        public class DetectFeedsRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
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
        private const string SERVICE_GET_KEYWORD_EXTRACTION_HTML = "/html/HTMLGetRankedKeywords";
        private const string SERVICE_GET_KEYWORD_EXTRACTION_URL = "/url/URLGetRankedKeywords";
        private const string SERVICE_GET_KEYWORD_EXTRACTION_TEXT = "/text/TextGetRankedKeywords";

        /// <summary>
        /// On get keywords delegate.
        /// </summary>
        public delegate void OnGetKeywords(KeywordData keywordData, string data);

        /// <summary>
        /// Extracts the keywords from a source.
        /// </summary>
        /// <returns><c>true</c>, if keywords was extracted, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="source">HTML, URL or Text source.</param>
        /// <param name="maxRetrieve">Maximum results retreived.</param>
        /// <param name="includeKnowledgeGraph">If set to <c>true</c> include knowledge graph.</param>
        /// <param name="analyzeSentiment">If set to <c>true</c> analyze sentiment.</param>
        /// <param name="includeSourceText">If set to <c>true</c> include source text.</param>
        /// <param name="customData">Custom data.</param>
        public bool ExtractKeywords(OnGetKeywords callback, string source,
            int maxRetrieve = 50,
            bool includeKnowledgeGraph = false,
            bool analyzeSentiment = false,
            bool includeSourceText = false,
            string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for ExtractKeywords.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetKeywordsRequest req = new GetKeywordsRequest();
            req.Callback = callback;
            req.Data = string.IsNullOrEmpty(customData) ? source : customData;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["maxRetrieve"] = Convert.ToInt32(maxRetrieve).ToString();
            req.Parameters["knowledgeGraph"] = Convert.ToInt32(includeKnowledgeGraph).ToString();
            req.Parameters["sentiment"] = Convert.ToInt32(analyzeSentiment).ToString();
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();
            req.Parameters["keywordExtractMode"] = "strict";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_KEYWORD_EXTRACTION_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = SERVICE_GET_KEYWORD_EXTRACTION_HTML;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for Getkeywords!");
            }
            else
            {
                service = SERVICE_GET_KEYWORD_EXTRACTION_TEXT;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if (connector == null)
                return false;

            req.OnResponse = OnGetKeywordsResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Get keywords request.
        /// </summary>
        public class GetKeywordsRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
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
        private const string SERVICE_GET_LANGUAGE_HTML = "/html/HTMLGetLanguage";
        private const string SERVICE_GET_LANGUAGE_URL = "/url/URLGetLanguage";
        private const string SERVICE_GET_LANGUAGE_TEXT = "/text/TextGetLanguage";

        /// <summary>
        /// On get languages.
        /// </summary>
        public delegate void OnGetLanguages(LanguageData languageData, string data);

        /// <summary>
        /// Extracts the language a source is written.
        /// </summary>
        /// <returns><c>true</c>, if languages was gotten, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="source">HTML, URL or Text source.</param>
        /// <param name="includeSourceText">If set to <c>true</c> include source text.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetLanguages(OnGetLanguages callback, string source, bool includeSourceText = false, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetLanguages.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetLanguagesRequest req = new GetLanguagesRequest();
            req.Callback = callback;
            req.Data = string.IsNullOrEmpty(customData) ? source : customData;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_LANGUAGE_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = SERVICE_GET_LANGUAGE_HTML;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetLanguages!");
            }
            else
            {
                service = SERVICE_GET_LANGUAGE_TEXT;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if (connector == null)
                return false;

            req.OnResponse = OnGetLanguagesResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Get languages request.
        /// </summary>
        public class GetLanguagesRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
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
        private const string SERVICE_GET_MICROFORMAT_URL = "/url/URLGetMicroformatData";
        private const string SERVICE_GET_MICROFORMAT_HTML = "/html/HTMLGetMicroformatData";

        /// <summary>
        /// On get microformats.
        /// </summary>
        public delegate void OnGetMicroformats(MicroformatData microformatData, string data);

        /// <summary>
        /// Extracts microformats from a URL source.
        /// </summary>
        /// <returns><c>true</c>, if microformats was gotten, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="source">URL to extract microformats from.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetMicroformats(OnGetMicroformats callback, string source, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a url for GetMicroformats.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetMicroformatsRequest req = new GetMicroformatsRequest();
            req.Callback = callback;
            req.Data = string.IsNullOrEmpty(customData) ? source : customData;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service = "";
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_MICROFORMAT_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                Log.Error("Alchemy Language", "A URL source is required for GetMicroformats!");
                return false;
                //                service = SERVICE_GET_MICROFORMAT_HTML;
                //                string htmlData = default(string);
                //                htmlData = File.ReadAllText(source);
                //                req.Forms["html"] = new RESTConnector.Form(htmlData);
            }
            else
            {
                Log.Error("Alchemy Language", "A URL source is required for GetMicroformats!");
                return false;
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if (connector == null)
                return false;

            req.OnResponse = OnGetMicroformatsResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Get microformats request.
        /// </summary>
        public class GetMicroformatsRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
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
        private const string SERVICE_GET_PUBLICATION_DATE_URL = "/url/URLGetPubDate";
        private const string SERVICE_GET_PUBLICATION_DATE_HTML = "/html/HTMLGetPubDate";

        /// <summary>
        /// On get publication date delegate.
        /// </summary>
        public delegate void OnGetPublicationDate(PubDateData pubDateData, string data);

        /// <summary>
        /// Extracts the publication date from a source.
        /// </summary>
        /// <returns><c>true</c>, if publication date was gotten, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="source">URL or HTML sources.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetPublicationDate(OnGetPublicationDate callback, string source, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a url for GetPublicationDate.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetPublicationDateRequest req = new GetPublicationDateRequest();
            req.Callback = callback;
            req.Data = string.IsNullOrEmpty(customData) ? source : customData;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service = "";
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_PUBLICATION_DATE_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = SERVICE_GET_PUBLICATION_DATE_HTML;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetPubicationDate!");
            }
            else
            {
                Log.Error("Alchemy Language", "Either a URL or a html page source is required for GetPublicationDate!");
                return false;
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if (connector == null)
                return false;

            req.OnResponse = OnGetPublicationDateResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Get publication date request.
        /// </summary>
        public class GetPublicationDateRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
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
        private const string SERVICE_GET_RELATIONS_HTML = "/html/HTMLGetRelations";
        private const string SERVICE_GET_RELATIONS_URL = "/url/URLGetRelations";
        private const string SERVICE_GET_RELATIONS_TEXT = "/text/TextGetRelations";

        /// <summary>
        /// On get relations delegate.
        /// </summary>
        public delegate void OnGetRelations(RelationsData relationData, string data);

        /// <summary>
        /// Extracts the relations from a source.
        /// </summary>
        /// <returns><c>true</c>, if relations was gotten, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="source">HTML, URL or Text source.</param>
        /// <param name="maxRetrieve">Max retrieve.</param>
        /// <param name="includeKeywords">If set to <c>true</c> include keywords.</param>
        /// <param name="includeEntities">If set to <c>true</c> include entities.</param>
        /// <param name="requireEntities">If set to <c>true</c> require entities.</param>
        /// <param name="resolveCoreferences">If set to <c>true</c> resolve coreferences.</param>
        /// <param name="disambiguateEntities">If set to <c>true</c> disambiguate entities.</param>
        /// <param name="includeKnowledgeGraph">If set to <c>true</c> include knowledge graph.</param>
        /// <param name="includeLinkedData">If set to <c>true</c> include linked data.</param>
        /// <param name="analyzeSentiment">If set to <c>true</c> analyze sentiment.</param>
        /// <param name="excludeEntitiesInSentiment">If set to <c>true</c> exclude entities in sentiment.</param>
        /// <param name="includeSourceText">If set to <c>true</c> include source text.</param>
        /// <param name="customData">Custom data.</param>
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
            bool includeSourceText = false,
            string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetRelations.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetRelationsRequest req = new GetRelationsRequest();
            req.Callback = callback;
            req.Data = string.IsNullOrEmpty(customData) ? source : customData;

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
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();
            req.Parameters["keywordExtractMode"] = "strict";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_RELATIONS_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = SERVICE_GET_RELATIONS_HTML;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetRelations!");
            }
            else
            {
                service = SERVICE_GET_RELATIONS_TEXT;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if (connector == null)
                return false;

            req.OnResponse = OnGetRelationsResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Get relations request.
        /// </summary>
        public class GetRelationsRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
			public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
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
        private const string SERVICE_GET_TEXT_SENTIMENT_HTML = "/html/HTMLGetTextSentiment";
        private const string SERVICE_GET_TEXT_SENTIMENT_URL = "/url/URLGetTextSentiment";
        private const string SERVICE_GET_TEXT_SENTIMENT_TEXT = "/text/TextGetTextSentiment";

        /// <summary>
        /// On get text sentiment delegate.
        /// </summary>
        public delegate void OnGetTextSentiment(SentimentData sentimentData, string data);

        /// <summary>
        /// Extracts the sentiment from a source.
        /// </summary>
        /// <returns><c>true</c>, if text sentiment was gotten, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="source">HTML, URL or Text source.</param>
        /// <param name="includeSourceText">If set to <c>true</c> include source text.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetTextSentiment(OnGetTextSentiment callback, string source, bool includeSourceText = false, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetTextSentiment.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetTextSentimentRequest req = new GetTextSentimentRequest();
            req.Callback = callback;
            req.Data = string.IsNullOrEmpty(customData) ? source : customData;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_TEXT_SENTIMENT_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = SERVICE_GET_TEXT_SENTIMENT_HTML;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetTextSentiment!");
            }
            else
            {
                service = SERVICE_GET_TEXT_SENTIMENT_TEXT;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if (connector == null)
                return false;

            req.OnResponse = OnGetTextSentimentResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Get text sentiment request.
        /// </summary>
        public class GetTextSentimentRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
			public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
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
        private const string SERVICE_GET_TARGETED_SENTIMENT_HTML = "/html/HTMLGetTargetedSentiment";
        private const string SERVICE_GET_TARGETED_SENTIMENT_URL = "/url/URLGetTargetedSentiment";
        private const string SERVICE_GET_TARGETED_SENTIMENT_TEXT = "/text/TextGetTargetedSentiment";

        /// <summary>
        /// On get targeted sentiment delegate.
        /// </summary>
        public delegate void OnGetTargetedSentiment(TargetedSentimentData targetedSentimentData, string data);

        /// <summary>
        /// Extracts targeted sentiment from a source.
        /// </summary>
        /// <returns><c>true</c>, if targeted sentiment was gotten, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="source">HTML, URL or Text source.</param>
        /// <param name="targets">Targets.</param>
        /// <param name="includeSourceText">If set to <c>true</c> include source text.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetTargetedSentiment(OnGetTargetedSentiment callback, string source, string targets, bool includeSourceText = false, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetTargetedSentiment.");
            if (string.IsNullOrEmpty(targets))
                throw new WatsonException("Please provide a target for GetTargetedSentiment.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetTargetedSentimentRequest req = new GetTargetedSentimentRequest();
            req.Callback = callback;
            req.Data = string.IsNullOrEmpty(customData) ? source : customData;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["targets"] = new RESTConnector.Form(targets);

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_TARGETED_SENTIMENT_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = SERVICE_GET_TARGETED_SENTIMENT_HTML;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetTargetedSentiment!");
            }
            else
            {
                service = SERVICE_GET_TARGETED_SENTIMENT_TEXT;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if (connector == null)
                return false;

            req.OnResponse = OnGetTargetedSentimentResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Get targeted sentiment request.
        /// </summary>
        public class GetTargetedSentimentRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
			public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
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
        private const string SERVICE_GET_RANKED_TAXONOMY_HTML = "/html/HTMLGetRankedTaxonomy";
        private const string SERVICE_GET_RANKED_TAXONOMY_URL = "/url/URLGetRankedTaxonomy";
        private const string SERVICE_GET_RANKED_TAXONOMY_TEXT = "/text/TextGetRankedTaxonomy";

        /// <summary>
        /// On get ranked taxonomy delegate.
        /// </summary>
        public delegate void OnGetRankedTaxonomy(TaxonomyData taxonomyData, string data);

        /// <summary>
        /// Extracts the ranked taxonomy from a source.
        /// </summary>
        /// <returns><c>true</c>, if ranked taxonomy was gotten, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="source">HTML, URL or Text source.</param>
        /// <param name="includeSourceText">If set to <c>true</c> include source text.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetRankedTaxonomy(OnGetRankedTaxonomy callback, string source, bool includeSourceText = false, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetRankedTaxonomy.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetRankedTaxomomyRequest req = new GetRankedTaxomomyRequest();
            req.Callback = callback;
            req.Data = string.IsNullOrEmpty(customData) ? source : customData;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_RANKED_TAXONOMY_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = SERVICE_GET_RANKED_TAXONOMY_HTML;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetRankedTaxonomy!");
            }
            else
            {
                service = SERVICE_GET_RANKED_TAXONOMY_TEXT;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if (connector == null)
                return false;

            req.OnResponse = OnGetRankedTaxonomyResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Get ranked taxomomy request.
        /// </summary>
        public class GetRankedTaxomomyRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
			public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
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
        private const string SERVICE_GET_TEXT_HTML = "/html/HTMLGetText";
        private const string SERVICE_GET_TEXT_URL = "/url/URLGetText";

        /// <summary>
        /// On get text delegate.
        /// </summary>
        public delegate void OnGetText(TextData textData, string data);

        /// <summary>
        /// Extracts text from a source.
        /// </summary>
        /// <returns><c>true</c>, if text was gotten, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="source">HTML or URL source.</param>
        /// <param name="extractLinks">If set to <c>true</c> extract links.</param>
        /// <param name="useMetadata">If set to <c>true</c> use metadata.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetText(OnGetText callback, string source, bool extractLinks = false, bool useMetadata = true, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetText.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetTextRequest req = new GetTextRequest();
            req.Callback = callback;
            req.Data = string.IsNullOrEmpty(customData) ? source : customData;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["extractLinks"] = Convert.ToInt32(extractLinks).ToString();
            req.Parameters["useMetadata"] = Convert.ToInt32(useMetadata).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service = "";
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_TEXT_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = SERVICE_GET_TEXT_HTML;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetText!");
            }
            else
            {
                Log.Error("Alchemy Language", "Either a URL or a html page source is required for GetText!");
                return false;
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if (connector == null)
                return false;

            req.OnResponse = OnGetTextResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Get text request.
        /// </summary>
        public class GetTextRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
			public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
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
        private const string SERVICE_GET_RAW_TEXT_HTML = "/html/HTMLGetRawText";
        private const string SERVICE_GET_RAW_TEXT_URL = "/url/URLGetRawText";

        /// <summary>
        /// Gets raw text from a source.
        /// </summary>
        /// <returns><c>true</c>, if raw text was gotten, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="source">HTML or URL source.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetRawText(OnGetText callback, string source, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetRawText.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetTextRequest req = new GetTextRequest();
            req.Callback = callback;
            req.Data = string.IsNullOrEmpty(customData) ? source : customData;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service = "";
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_TEXT_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = SERVICE_GET_TEXT_HTML;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetRawText!");
            }
            else
            {
                Log.Error("Alchemy Language", "Either a URL or a html page source is required for GetRawText!");
                return false;
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if (connector == null)
                return false;

            req.OnResponse = OnGetTextResponse;
            return connector.Send(req);
        }
        #endregion

        #region GetTitle
        private const string SERVICE_GET_TITLE_HTML = "/html/HTMLGetTitle";
        private const string SERVICE_GET_TITLE_URL = "/url/URLGetTitle";

        /// <summary>
        /// On get title delegate.
        /// </summary>
        public delegate void OnGetTitle(Title titleData, string data);

        /// <summary>
        /// Extracts the title from a source.
        /// </summary>
        /// <returns><c>true</c>, if title was gotten, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="source">HTML or URL source.</param>
        /// <param name="useMetadata">If set to <c>true</c> use metadata.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetTitle(OnGetTitle callback, string source, bool useMetadata = true, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetText.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetTitleRequest req = new GetTitleRequest();
            req.Callback = callback;
            req.Data = string.IsNullOrEmpty(customData) ? source : customData;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["useMetadata"] = Convert.ToInt32(useMetadata).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service = "";
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_GET_TITLE_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = SERVICE_GET_TITLE_HTML;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetTitle!");
            }
            else
            {
                Log.Error("Alchemy Language", "Either a URL or a html page source is required for GetTitle!");
                return false;
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if (connector == null)
                return false;

            req.OnResponse = OnGetTitleResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Get title request.
        /// </summary>
        public class GetTitleRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
			public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
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
        private const string SERVICE_COMBINED_CALL_HTML = "/html/HTMLGetCombinedData";
        private const string SERVICE_COMBINED_CALL_URL = "/url/URLGetCombinedData";
        private const string SERVICE_COMBINED_CALL_TEXT = "/text/TextGetCombinedData";

        /// <summary>
        /// On get combined data delegate.
        /// </summary>
        public delegate void OnGetCombinedData(CombinedCallData combinedData, string data);

        /// <summary>
        /// Access multiple services in one call.
        /// </summary>
        /// <returns><c>true</c>, if combined data was gotten, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="source">HTML, URL or Text source.</param>
        /// <param name="includeSourceText">If set to <c>true</c> include source text.</param>
        /// <param name="extractAuthors">If set to <c>true</c> extract authors.</param>
        /// <param name="extractConcepts">If set to <c>true</c> extract concepts.</param>
        /// <param name="extractDates">If set to <c>true</c> extract dates.</param>
        /// <param name="extractDocEmotion">If set to <c>true</c> extract document emotion.</param>
        /// <param name="extractEntities">If set to <c>true</c> extract entities.</param>
        /// <param name="extractFeeds">If set to <c>true</c> extract feeds.</param>
        /// <param name="extractKeywords">If set to <c>true</c> extract keywords.</param>
        /// <param name="extractPubDate">If set to <c>true</c> extract pub date.</param>
        /// <param name="extractRelations">If set to <c>true</c> extract relations.</param>
        /// <param name="extractDocSentiment">If set to <c>true</c> extract document sentiment.</param>
        /// <param name="extractTaxonomy">If set to <c>true</c> extract taxonomy.</param>
        /// <param name="extractTitle">If set to <c>true</c> extract title.</param>
        /// <param name="extractPageImage">If set to <c>true</c> extract page image.</param>
        /// <param name="extractImageKeywords">If set to <c>true</c> extract image keywords.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetCombinedData(OnGetCombinedData callback, string source,
            bool includeSourceText = false,
            bool extractAuthors = false,
            bool extractConcepts = true,
            bool extractDates = false,
            bool extractDocEmotion = false,
            bool extractEntities = true,
            bool extractFeeds = false,
            bool extractKeywords = true,
            bool extractPubDate = false,
            bool extractRelations = false,
            bool extractDocSentiment = false,
            bool extractTaxonomy = true,
            bool extractTitle = false,
            bool extractPageImage = false,
            bool extractImageKeywords = false,
            string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetCombinedData.");
            if (!extractAuthors
                && !extractConcepts
                && !extractDates
                && !extractDocEmotion
                && !extractEntities
                && !extractFeeds
                && !extractKeywords
                && !extractPubDate
                && !extractRelations
                && !extractDocSentiment
                && !extractTaxonomy
                && !extractTitle
                && !extractPageImage
                && !extractImageKeywords)
                throw new WatsonException("GetCombinedCall - Please include one or more services.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            CombinedCallRequest req = new CombinedCallRequest();
            req.Callback = callback;
            req.Data = string.IsNullOrEmpty(customData) ? source : customData;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            List<string> requestServices = new List<string>();
            if (extractAuthors)
                requestServices.Add("authors");
            if (extractConcepts)
                requestServices.Add("concepts");
            if (extractDates)
                requestServices.Add("dates");
            if (extractDocEmotion)
                requestServices.Add("doc-emotion");
            if (extractEntities)
                requestServices.Add("entities");
            if (extractFeeds)
                requestServices.Add("feeds");
            if (extractKeywords)
                requestServices.Add("keywords");
            if (extractPubDate)
                requestServices.Add("pub-date");
            if (extractRelations)
                requestServices.Add("relations");
            if (extractDocSentiment)
                requestServices.Add("doc-sentiment");
            if (extractTaxonomy)
                requestServices.Add("taxonomy");
            if (extractTitle)
                requestServices.Add("title");
            if (extractPageImage)
                requestServices.Add("page-image");
            if (extractImageKeywords)
                requestServices.Add("image-kw");
            req.Parameters["extract"] = string.Join(",", requestServices.ToArray());

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = SERVICE_COMBINED_CALL_URL;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = SERVICE_COMBINED_CALL_HTML;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetCombinedData!");
            }
            else
            {
                service = SERVICE_COMBINED_CALL_TEXT;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, service);
            if (connector == null)
                return false;

            req.OnResponse = OnCombinedCallResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Combined call request.
        /// </summary>
        public class CombinedCallRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
            public OnGetCombinedData Callback { get; set; }
        }

        private void OnCombinedCallResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            CombinedCallData combinedData = new CombinedCallData();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = combinedData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyLanguage", "OnCombinedCallResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((CombinedCallRequest)req).Callback != null)
                ((CombinedCallRequest)req).Callback(resp.Success ? combinedData : null, ((CombinedCallRequest)req).Data);
        }
        #endregion

        #region GetNews
        private const string SERVICE_GET_NEWS = "/data/GetNews";

        /// <summary>
        /// On get news delegate.
        /// </summary>
        public delegate void OnGetNews(NewsResponse newsData, string data);

        /// <summary>
        /// Gets news.
        /// </summary>
        /// <returns><c>true</c>, if news was gotten, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="returnFields">Fields returned.</param>
        /// <param name="queryFields">Values for each field.</param>
        /// <param name="startDate">Date to start the query.</param>
        /// <param name="endDate">Date to end the query.</param>
        /// <param name="maxResults">Maximum number of results.</param>
        /// <param name="timeSlice">the duration (in seconds) of each time slice. a human readable duration is also acceptable e.g. '1d', '4h', '1M', etc.
        /// If set, this parameter causes the query engine to return a time series representing the count in each slice of time. If omitted, the query engine returns the total count over the time duration.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetNews(OnGetNews callback,
            string[] returnFields = default(string[]),
            Dictionary<string, string> queryFields = null,
            string startDate = "now-1d",
            string endDate = "now",
            int maxResults = 10,
            string timeSlice = default(string),
            string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(mp_ApiKey))
                SetCredentials();

            GetNewsRequest req = new GetNewsRequest();
            req.Callback = callback;
            req.Data = customData;

            req.Parameters["apikey"] = mp_ApiKey;
            req.Parameters["outputMode"] = "json";
			req.Parameters["start"] = startDate;
			req.Parameters["end"] = endDate;
			req.Parameters["maxResults"] = maxResults;
			if (timeSlice != default(string))
				req.Parameters["timeSlice"] = timeSlice;
			if (returnFields != default(string[]))
				req.Parameters["return"] = string.Join(",", returnFields);
			if (queryFields != null)
				foreach (KeyValuePair<string, string> entry in queryFields)
					req.Parameters[entry.Key] = "q." + entry.Value;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_GET_NEWS);
            if (connector == null)
                return false;

            req.OnResponse = OnGetNewsResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Get News request.
        /// </summary>
        public class GetNewsRequest : RESTConnector.Request
        {
			/// <summary>
			/// Custom data.
			/// </summary>
            public string Data { get; set; }
			/// <summary>
			/// The callback.
			/// </summary>
            public OnGetNews Callback { get; set; }
        }

        private void OnGetNewsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            NewsResponse newsData = new NewsResponse();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = newsData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyDataNews", "OnGetNewsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetNewsRequest)req).Callback != null)
                ((GetNewsRequest)req).Callback(resp.Success ? newsData : null, ((GetNewsRequest)req).Data);
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
            private AlchemyAPI m_Service = null;
            private ServiceStatus m_Callback = null;

            public CheckServiceStatus(AlchemyAPI service, ServiceStatus callback)
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
