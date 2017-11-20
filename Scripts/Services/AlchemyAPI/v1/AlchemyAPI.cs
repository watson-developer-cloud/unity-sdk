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
using System;
using System.Collections.Generic;
using System.Text;
using FullSerializer;
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
        private const string ServiceId = "AlchemyAPIV1";
        private fsSerializer _serializer = new fsSerializer();
        private static string _apiKey = null;
        private Credentials _credentials = null;
        private string _url = "https://gateway-a.watsonplatform.net/calls";
        #endregion

        #region SetCredentials
        private void SetCredentials()
        {
            if (!string.IsNullOrEmpty(_credentials.ApiKey))
                _apiKey = _credentials.ApiKey;

            if (string.IsNullOrEmpty(_apiKey))
                throw new WatsonException("Alchemy API Key required.");
        }
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

        #region Constructor
        public AlchemyAPI(Credentials credentials)
        {
            if (credentials.HasApiKey())
            {
                Credentials = credentials;
            }
            else
            {
                throw new WatsonException("Please provide an apikey to use the Alchemy API. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
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

        #region GetAuthors
        private const string GetAuthorsUrl = "/url/URLGetAuthors";
        private const string GetAuthorsHtml = "/html/HTMLGetAuthors";

        /// <summary>
        /// Extracts authors from a source.
        /// </summary>
        /// <returns><c>true</c>, if authors was gotten, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="source">URL or HTML source.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetAuthors(SuccessCallback<AuthorsData> successCallback, FailCallback failCallback, string source, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("Please provide a source for GetAuthors.");
            if (string.IsNullOrEmpty(_apiKey))
                SetCredentials();

            GetAuthorsRequest req = new GetAuthorsRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            req.Parameters["apikey"] = _apiKey;
            req.Parameters["outputMode"] = "json";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = GetAuthorsUrl;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = GetAuthorsHtml;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetAuthors");
            }
            else
            {
                Log.Error("AlchemyAPI.GetAuthors()", "Either a URL or a html page source is required for GetAuthors!");
                return false;
            }

            RESTConnector connector = RESTConnector.GetConnector(Credentials, service);
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
            /// The success callback.
            /// </summary>
            public SuccessCallback<AuthorsData> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetAuthorsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            AuthorsData result = new AuthorsData();
            fsData data = null;
            Dictionary<string, object> customData = ((GetAuthorsRequest)req).CustomData;

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
                    Log.Error("AlchemyAPI.OnGetAuthorsResponse()", "OnGetAuthorsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }
            
            if (resp.Success)
            {
                if (((GetAuthorsRequest)req).SuccessCallback != null)
                    ((GetAuthorsRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetAuthorsRequest)req).FailCallback != null)
                    ((GetAuthorsRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region GetRankedConcepts
        private const string GetRankedConceptsHtml = "/html/HTMLGetRankedConcepts";
        private const string GetRankedConceptsUrl = "/url/URLGetRankedConcepts";
        private const string GetRankedConceptsText = "/text/TextGetRankedConcepts";

        /// <summary>
        /// Extracts concepts from a source.
        /// </summary>
        /// <returns><c>true</c>, if ranked concepts was gotten, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="source">HTML, URL or Text Source.</param>
        /// <param name="maxRetrieve">Maximum results retreived.</param>
        /// <param name="includeKnowledgeGraph">If set to <c>true</c> include knowledge graph.</param>
        /// <param name="includeLinkedData">If set to <c>true</c> include linked data.</param>
        /// <param name="includeSourceText">If set to <c>true</c> include source text.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetRankedConcepts(SuccessCallback<ConceptsData> successCallback, FailCallback failCallback, string source,
            int maxRetrieve = 8,
            bool includeKnowledgeGraph = false,
            bool includeLinkedData = true,
            bool includeSourceText = false,
            Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetAuthors.");
            if (string.IsNullOrEmpty(_apiKey))
                SetCredentials();

            GetRankedConceptsRequest req = new GetRankedConceptsRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            req.Parameters["apikey"] = _apiKey;
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
                service = GetRankedConceptsUrl;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (!normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://") && source.StartsWith(Application.dataPath))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = GetRankedConceptsHtml;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetRankedConcepts");
            }
            else
            {
                service = GetRankedConceptsText;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(Credentials, service);
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
            public Dictionary<string, object> CustomData { get; set; }
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<ConceptsData> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
        }

        private void OnGetRankedConceptsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ConceptsData result = new ConceptsData();
            fsData data = null;
            Dictionary<string, object> customData = ((GetRankedConceptsRequest)req).CustomData;

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
                    Log.Error("AlchemyApi.OnGetRankedConceptsResponse()", "OnGetRankedConceptsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetRankedConceptsRequest)req).SuccessCallback != null)
                    ((GetRankedConceptsRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetRankedConceptsRequest)req).FailCallback != null)
                    ((GetRankedConceptsRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region ExtractDates
        private const string GetDatesHtml = "/html/HTMLExtractDates";
        private const string GetDatesUrl = "/url/URLExtractDates";
        private const string GetDatesText = "/text/TextExtractDates";

        /// <summary>
        /// On get dates delegate.
        /// </summary>
        public delegate void OnGetDates(DateData dateData, string data);

        /// <summary>
        /// Extracts dates from a source.
        /// </summary>
        /// <returns><c>true</c>, if dates was gotten, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="source">HTML, URL or Text source.</param>
        /// <param name="anchorDate">Anchor date in the yyyy-mm-dd hh:mm:ss format. If this is not set, anchor date is set to today's date and time.</param>
        /// <param name="includeSourceText">If set to <c>true</c> include source text.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetDates(SuccessCallback<DateData> successCallback, FailCallback failCallback, string source, string anchorDate = default(string), bool includeSourceText = false, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetAuthors.");
            if (string.IsNullOrEmpty(_apiKey))
                SetCredentials();
            if (string.IsNullOrEmpty(anchorDate))
                anchorDate = GetCurrentDatetime();

            GetDatesRequest req = new GetDatesRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            req.Parameters["apikey"] = _apiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["anchorDate"] = new RESTConnector.Form(anchorDate);

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = GetDatesUrl;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = GetDatesHtml;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for ExtractDates!");
            }
            else
            {
                service = GetDatesText;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(Credentials, service);
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
            /// The success callback.
            /// </summary>
            public SuccessCallback<DateData> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetDatesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DateData result = new DateData();
            fsData data = null;
            Dictionary<string, object> customData = ((GetDatesRequest)req).CustomData;

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
                    Log.Error("AlchemyApi.OnGetDatesResponse()", "OnDatesResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetDatesRequest)req).SuccessCallback != null)
                    ((GetDatesRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetDatesRequest)req).FailCallback != null)
                    ((GetDatesRequest)req).FailCallback(resp.Error, customData);
            }
        }

        private string GetCurrentDatetime()
        {
            //  date format is yyyy-mm-dd hh:mm:ss
            string dateFormat = "{0}-{1}-{2} {3}:{4}:{5}";
            return string.Format(dateFormat, System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second);
        }
        #endregion

        #region GetEmotion
        private const string GetEmotionHtml = "/html/HTMLGetEmotion";
        private const string GetEmotionUrl = "/url/URLGetEmotion";
        private const string GetEmotionText = "/text/TextGetEmotion";

        /// <summary>
        /// On get emotions delegate.
        /// </summary>
        public delegate void OnGetEmotions(EmotionData emotionData, string data);

        /// <summary>
        /// Gets the emotions from a source.
        /// </summary>
        /// <returns><c>true</c>, if emotions was gotten, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="source">HTML, URL or Text source.</param>
        /// <param name="includeSourceText">If set to <c>true</c> include source text.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetEmotions(SuccessCallback<EmotionData> successCallback, FailCallback failCallback, string source, bool includeSourceText = false, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetEmotions.");
            if (string.IsNullOrEmpty(_apiKey))
                SetCredentials();

            GetEmotionsRequest req = new GetEmotionsRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            req.Parameters["apikey"] = _apiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = GetEmotionUrl;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = GetEmotionHtml;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetEmotions!");
            }
            else
            {
                service = GetEmotionText;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(Credentials, service);
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
            /// The success callback.
            /// </summary>
            public SuccessCallback<EmotionData> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetEmotionsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            EmotionData result = new EmotionData();
            fsData data = null;
            Dictionary<string, object> customData = ((GetEmotionsRequest)req).CustomData;

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
                    Log.Error("AlchemyApi.OnGetEmotionsResponse()", "OnGetEmotionsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetEmotionsRequest)req).SuccessCallback != null)
                    ((GetEmotionsRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetEmotionsRequest)req).FailCallback != null)
                    ((GetEmotionsRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Entity Extraction
        private const string ExtractEntityHtml = "/html/HTMLGetRankedNamedEntities";
        private const string ExtractEntityUrl = "/url/URLGetRankedNamedEntities";
        private const string ExtractEntityText = "/text/TextGetRankedNamedEntities";

        /// <summary>
        /// On get entities delegate.
        /// </summary>
        public delegate void OnGetEntities(EntityData entityData, string data);

        /// <summary>
        /// Extracts the entities from a source.
        /// </summary>
        /// <returns><c>true</c>, if entities was extracted, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
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
        public bool ExtractEntities(SuccessCallback<EntityData> successCallback, FailCallback failCallback, string source,
            int maxRetrieve = 50,
            bool resolveCoreference = true,
            bool disambiguate = true,
            bool includeKnowledgeGraph = false,
            bool includeLinkedData = true,
            bool includeQuotations = false,
            bool analyzeSentiment = false,
            bool includeSourceText = false,
            bool extractStructuredEntities = true,
            Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for ExtractEntities.");
            if (string.IsNullOrEmpty(_apiKey))
                SetCredentials();

            GetEntitiesRequest req = new GetEntitiesRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            req.Parameters["apikey"] = _apiKey;
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
                service = ExtractEntityUrl;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = ExtractEntityHtml;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for ExtractEntities!");
            }
            else
            {
                service = ExtractEntityText;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(Credentials, service);
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
            /// The success callback.
            /// </summary>
            public SuccessCallback<EntityData> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetEntitiesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            EntityData result = new EntityData();
            fsData data = null;
            Dictionary<string, object> customData = ((GetEntitiesRequest)req).CustomData;

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
                    Log.Error("AlchemyApi.OnGetEntitiesResponse()", "OnGetEntitiesResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetEntitiesRequest)req).SuccessCallback != null)
                    ((GetEntitiesRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetEntitiesRequest)req).FailCallback != null)
                    ((GetEntitiesRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region FeedDetection
        private const string DetectFeedsUrl = "/url/URLGetFeedLinks";
        private const string DectectFeedsHtml = "/html/HTMLGetFeedLinks";

        /// <summary>
        /// On detect feeds delegate.
        /// </summary>
        public delegate void OnDetectFeeds(FeedData feedData, string data);

        /// <summary>
        /// Detects RSS feeds in a source.
        /// </summary>
        /// <returns><c>true</c>, if feeds was detected, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="source">URL to detect feeds.</param>
        /// <param name="customData">Custom data.</param>
        public bool DetectFeeds(SuccessCallback<FeedData> successCallback, FailCallback failCallback, string source, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetEmotions.");
            if (string.IsNullOrEmpty(_apiKey))
                SetCredentials();

            DetectFeedsRequest req = new DetectFeedsRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            req.Parameters["apikey"] = _apiKey;
            req.Parameters["outputMode"] = "json";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = DetectFeedsUrl;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                Log.Error("AlchemyAPI.DetectFeeds()", "A URL source is required for DetectFeeds!");
                return false;
                //                service = SERVICE_DETECT_FEEDS_HTML;
                //                string htmlData = default(string);
                //                htmlData = File.ReadAllText(source);
                //                req.Forms["html"] = new RESTConnector.Form(htmlData);
            }
            else
            {
                Log.Error("AlchemyAPI.DetectFeeds()", "A URL source is required for DetectFeeds!");
                return false;
            }

            RESTConnector connector = RESTConnector.GetConnector(Credentials, service);
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
            /// The success callback.
            /// </summary>
            public SuccessCallback<FeedData> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnDetectFeedsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            FeedData result = new FeedData();
            fsData data = null;
            Dictionary<string, object> customData = ((DetectFeedsRequest)req).CustomData;

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
                    Log.Error("AlchemyApi.OnDetectFeedsResponse()", "OnDetectFeedsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((DetectFeedsRequest)req).SuccessCallback != null)
                    ((DetectFeedsRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DetectFeedsRequest)req).FailCallback != null)
                    ((DetectFeedsRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Keyword Extraction
        private const string ExtractKeywordHtml = "/html/HTMLGetRankedKeywords";
        private const string ExtractKeywordUrl = "/url/URLGetRankedKeywords";
        private const string ExtractKeywordText = "/text/TextGetRankedKeywords";

        /// <summary>
        /// On get keywords delegate.
        /// </summary>
        public delegate void OnGetKeywords(KeywordData keywordData, string data);

        /// <summary>
        /// Extracts the keywords from a source.
        /// </summary>
        /// <returns><c>true</c>, if keywords was extracted, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="source">HTML, URL or Text source.</param>
        /// <param name="maxRetrieve">Maximum results retreived.</param>
        /// <param name="includeKnowledgeGraph">If set to <c>true</c> include knowledge graph.</param>
        /// <param name="analyzeSentiment">If set to <c>true</c> analyze sentiment.</param>
        /// <param name="includeSourceText">If set to <c>true</c> include source text.</param>
        /// <param name="customData">Custom data.</param>
        public bool ExtractKeywords(SuccessCallback<KeywordData> successCallback, FailCallback failCallback, string source,
            int maxRetrieve = 50,
            bool includeKnowledgeGraph = false,
            bool analyzeSentiment = false,
            bool includeSourceText = false,
            Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for ExtractKeywords.");
            if (string.IsNullOrEmpty(_apiKey))
                SetCredentials();

            GetKeywordsRequest req = new GetKeywordsRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            req.Parameters["apikey"] = _apiKey;
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
                service = ExtractKeywordUrl;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = ExtractKeywordHtml;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for Getkeywords!");
            }
            else
            {
                service = ExtractKeywordText;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(Credentials, service);
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
            /// The success callback.
            /// </summary>
            public SuccessCallback<KeywordData> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetKeywordsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            KeywordData result = new KeywordData();
            fsData data = null;
            Dictionary<string, object> customData = ((GetKeywordsRequest)req).CustomData;

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
                    Log.Error("AlchemyApi.OnGetKeywordsResponse()", "OnGetKeywordsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetKeywordsRequest)req).SuccessCallback != null)
                    ((GetKeywordsRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetKeywordsRequest)req).FailCallback != null)
                    ((GetKeywordsRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region GetLanguage
        private const string GetLanguageHtml = "/html/HTMLGetLanguage";
        private const string GetLanguageUrl = "/url/URLGetLanguage";
        private const string GetLanguageText = "/text/TextGetLanguage";

        /// <summary>
        /// On get languages.
        /// </summary>
        public delegate void OnGetLanguages(LanguageData languageData, string data);

        /// <summary>
        /// Extracts the language a source is written.
        /// </summary>
        /// <returns><c>true</c>, if languages was gotten, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="source">HTML, URL or Text source.</param>
        /// <param name="includeSourceText">If set to <c>true</c> include source text.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetLanguages(SuccessCallback<LanguageData> successCallback, FailCallback failCallback, string source, bool includeSourceText = false, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetLanguages.");
            if (string.IsNullOrEmpty(_apiKey))
                SetCredentials();

            GetLanguagesRequest req = new GetLanguagesRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            req.Parameters["apikey"] = _apiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = GetLanguageUrl;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = GetLanguageHtml;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetLanguages!");
            }
            else
            {
                service = GetLanguageText;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(Credentials, service);
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
            /// The success callback.
            /// </summary>
            public SuccessCallback<LanguageData> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetLanguagesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            LanguageData result = new LanguageData();
            fsData data = null;
            Dictionary<string, object> customData = ((GetLanguagesRequest)req).CustomData;

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
                    Log.Error("AlchemyApi.OnGetLanguagesResponse()", "OnGetEmotionsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetLanguagesRequest)req).SuccessCallback != null)
                    ((GetLanguagesRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetLanguagesRequest)req).FailCallback != null)
                    ((GetLanguagesRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region GetMicroformat
        private const string GetMicroformatUrl = "/url/URLGetMicroformatData";
        private const string GetMicroformatHtml = "/html/HTMLGetMicroformatData";

        /// <summary>
        /// On get microformats.
        /// </summary>
        public delegate void OnGetMicroformats(MicroformatData microformatData, string data);

        /// <summary>
        /// Extracts microformats from a URL source.
        /// </summary>
        /// <returns><c>true</c>, if microformats was gotten, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="source">URL to extract microformats from.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetMicroformats(SuccessCallback<MicroformatData> successCallback, FailCallback failCallback, string source, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a url for GetMicroformats.");
            if (string.IsNullOrEmpty(_apiKey))
                SetCredentials();

            GetMicroformatsRequest req = new GetMicroformatsRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            req.Parameters["apikey"] = _apiKey;
            req.Parameters["outputMode"] = "json";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service = "";
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = GetMicroformatUrl;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                Log.Error("AlchemyAPI.GetMicroformats()", "A URL source is required for GetMicroformats!");
                return false;
                //                service = SERVICE_GET_MICROFORMAT_HTML;
                //                string htmlData = default(string);
                //                htmlData = File.ReadAllText(source);
                //                req.Forms["html"] = new RESTConnector.Form(htmlData);
            }
            else
            {
                Log.Error("AlchemyAPI.GetMicroformats()", "A URL source is required for GetMicroformats!");
                return false;
            }

            RESTConnector connector = RESTConnector.GetConnector(Credentials, service);
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
            /// The success callback.
            /// </summary>
            public SuccessCallback<MicroformatData> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetMicroformatsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            MicroformatData result = new MicroformatData();
            fsData data = null;
            Dictionary<string, object> customData = ((GetMicroformatsRequest)req).CustomData;

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
                    Log.Error("AlchemyApi.OnGetMicroformatsResponse()", "OnGetMicroformatsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetMicroformatsRequest)req).SuccessCallback != null)
                    ((GetMicroformatsRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetMicroformatsRequest)req).FailCallback != null)
                    ((GetMicroformatsRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region GetPubDate
        private const string GetPublicationDateUrl = "/url/URLGetPubDate";
        private const string GetPublicationDateHtml = "/html/HTMLGetPubDate";

        /// <summary>
        /// On get publication date delegate.
        /// </summary>
        public delegate void OnGetPublicationDate(PubDateData pubDateData, string data);

        /// <summary>
        /// Extracts the publication date from a source.
        /// </summary>
        /// <returns><c>true</c>, if publication date was gotten, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="source">URL or HTML sources.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetPublicationDate(SuccessCallback<PubDateData> successCallback, FailCallback failCallback, string source, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a url for GetPublicationDate.");
            if (string.IsNullOrEmpty(_apiKey))
                SetCredentials();

            GetPublicationDateRequest req = new GetPublicationDateRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            req.Parameters["apikey"] = _apiKey;
            req.Parameters["outputMode"] = "json";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service = "";
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = GetPublicationDateUrl;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = GetPublicationDateHtml;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetPubicationDate!");
            }
            else
            {
                Log.Error("AlchemyAPI.GetPublicationDate()", "Either a URL or a html page source is required for GetPublicationDate!");
                return false;
            }

            RESTConnector connector = RESTConnector.GetConnector(Credentials, service);
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
            /// The success callback.
            /// </summary>
            public SuccessCallback<PubDateData> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetPublicationDateResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            PubDateData result = new PubDateData();
            fsData data = null;
            Dictionary<string, object> customData = ((GetPublicationDateRequest)req).CustomData;

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
                    Log.Error("AlchemyApi.OnGetPublicationDateResponse()", "OnGetPublicationDateResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetPublicationDateRequest)req).SuccessCallback != null)
                    ((GetPublicationDateRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetPublicationDateRequest)req).FailCallback != null)
                    ((GetPublicationDateRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region GetRelations
        private const string GetRelationsHtml = "/html/HTMLGetRelations";
        private const string GetRelationsUrl = "/url/URLGetRelations";
        private const string GetRelationsText = "/text/TextGetRelations";

        /// <summary>
        /// On get relations delegate.
        /// </summary>
        public delegate void OnGetRelations(RelationsData relationData, string data);

        /// <summary>
        /// Extracts the relations from a source.
        /// </summary>
        /// <returns><c>true</c>, if relations was gotten, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
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
        public bool GetRelations(SuccessCallback<RelationsData> successCallback, FailCallback failCallback, string source,
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
            Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetRelations.");
            if (string.IsNullOrEmpty(_apiKey))
                SetCredentials();

            GetRelationsRequest req = new GetRelationsRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            req.Parameters["apikey"] = _apiKey;
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
                service = GetRelationsUrl;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = GetRelationsHtml;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetRelations!");
            }
            else
            {
                service = GetRelationsText;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(Credentials, service);
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
            /// The success callback.
            /// </summary>
            public SuccessCallback<RelationsData> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetRelationsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            RelationsData result = new RelationsData();
            fsData data = null;
            Dictionary<string, object> customData = ((GetRelationsRequest)req).CustomData;

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
                    Log.Error("AlchemyApi.OnGetRelationsResponse()", "OnGetRelationsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetRelationsRequest)req).SuccessCallback != null)
                    ((GetRelationsRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetRelationsRequest)req).FailCallback != null)
                    ((GetRelationsRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region GetTextSentiment
        private const string GetTextSentimentHtml = "/html/HTMLGetTextSentiment";
        private const string GetTextSentimentUrl = "/url/URLGetTextSentiment";
        private const string GetTextSentimentText = "/text/TextGetTextSentiment";

        /// <summary>
        /// On get text sentiment delegate.
        /// </summary>
        public delegate void OnGetTextSentiment(SentimentData sentimentData, string data);

        /// <summary>
        /// Extracts the sentiment from a source.
        /// </summary>
        /// <returns><c>true</c>, if text sentiment was gotten, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="source">HTML, URL or Text source.</param>
        /// <param name="includeSourceText">If set to <c>true</c> include source text.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetTextSentiment(SuccessCallback<SentimentData> successCallback, FailCallback failCallback, string source, bool includeSourceText = false, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetTextSentiment.");
            if (string.IsNullOrEmpty(_apiKey))
                SetCredentials();

            GetTextSentimentRequest req = new GetTextSentimentRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            req.Parameters["apikey"] = _apiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = GetTextSentimentUrl;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = GetTextSentimentHtml;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetTextSentiment!");
            }
            else
            {
                service = GetTextSentimentText;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(Credentials, service);
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
            /// The success callback.
            /// </summary>
            public SuccessCallback<SentimentData> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetTextSentimentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            SentimentData result = new SentimentData();
            fsData data = null;
            Dictionary<string, object> customData = ((GetTextSentimentRequest)req).CustomData;

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
                    Log.Error("AlchemyApi.OnGetTextSentimentResponse()", "OnGetTextSentimentResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetTextSentimentRequest)req).SuccessCallback != null)
                    ((GetTextSentimentRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetTextSentimentRequest)req).FailCallback != null)
                    ((GetTextSentimentRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region GetTargetedSentiment
        private const string GetTargetedSentimentHtml = "/html/HTMLGetTargetedSentiment";
        private const string GetTargetedSentimentUrl = "/url/URLGetTargetedSentiment";
        private const string GetTargetedSentimentText = "/text/TextGetTargetedSentiment";

        /// <summary>
        /// On get targeted sentiment delegate.
        /// </summary>
        public delegate void OnGetTargetedSentiment(TargetedSentimentData targetedSentimentData, string data);

        /// <summary>
        /// Extracts targeted sentiment from a source.
        /// </summary>
        /// <returns><c>true</c>, if targeted sentiment was gotten, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="source">HTML, URL or Text source.</param>
        /// <param name="targets">Targets.</param>
        /// <param name="includeSourceText">If set to <c>true</c> include source text.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetTargetedSentiment(SuccessCallback<TargetedSentimentData> successCallback, FailCallback failCallback, string source, string targets, bool includeSourceText = false, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetTargetedSentiment.");
            if (string.IsNullOrEmpty(targets))
                throw new WatsonException("Please provide a target for GetTargetedSentiment.");
            if (string.IsNullOrEmpty(_apiKey))
                SetCredentials();

            GetTargetedSentimentRequest req = new GetTargetedSentimentRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            req.Parameters["apikey"] = _apiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["targets"] = new RESTConnector.Form(targets);

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = GetTargetedSentimentUrl;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = GetTargetedSentimentHtml;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetTargetedSentiment!");
            }
            else
            {
                service = GetTargetedSentimentText;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(Credentials, service);
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
            /// The success callback.
            /// </summary>
            public SuccessCallback<TargetedSentimentData> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetTargetedSentimentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            TargetedSentimentData result = new TargetedSentimentData();
            fsData data = null;
            Dictionary<string, object> customData = ((GetTargetedSentimentRequest)req).CustomData;

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
                    Log.Error("AlchemyApi.OnGetTargetedSentimentResponse()", "OnGetTargetedSentimentResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetTargetedSentimentRequest)req).SuccessCallback != null)
                    ((GetTargetedSentimentRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetTargetedSentimentRequest)req).FailCallback != null)
                    ((GetTargetedSentimentRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region GetRankedTaxonomy
        private const string GetRankedTaxonomyHtml = "/html/HTMLGetRankedTaxonomy";
        private const string GetRankedTaxonomyUrl = "/url/URLGetRankedTaxonomy";
        private const string GetRankedTaxonomyText = "/text/TextGetRankedTaxonomy";

        /// <summary>
        /// On get ranked taxonomy delegate.
        /// </summary>
        public delegate void OnGetRankedTaxonomy(TaxonomyData taxonomyData, string data);

        /// <summary>
        /// Extracts the ranked taxonomy from a source.
        /// </summary>
        /// <returns><c>true</c>, if ranked taxonomy was gotten, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="source">HTML, URL or Text source.</param>
        /// <param name="includeSourceText">If set to <c>true</c> include source text.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetRankedTaxonomy(SuccessCallback<TaxonomyData> successCallback, FailCallback failCallback, string source, bool includeSourceText = false, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetRankedTaxonomy.");
            if (string.IsNullOrEmpty(_apiKey))
                SetCredentials();

            GetRankedTaxonomyRequest req = new GetRankedTaxonomyRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            req.Parameters["apikey"] = _apiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["showSourceText"] = Convert.ToInt32(includeSourceText).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service;
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = GetRankedTaxonomyUrl;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = GetRankedTaxonomyHtml;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetRankedTaxonomy!");
            }
            else
            {
                service = GetRankedTaxonomyText;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(Credentials, service);
            if (connector == null)
                return false;

            req.OnResponse = OnGetRankedTaxonomyResponse;
            return connector.Send(req);
        }

        /// <summary>
        /// Get ranked taxomomy request.
        /// </summary>
        public class GetRankedTaxonomyRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<TaxonomyData> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetRankedTaxonomyResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            TaxonomyData result = new TaxonomyData();
            fsData data = null;
            Dictionary<string, object> customData = ((GetRankedTaxonomyRequest)req).CustomData;

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
                    Log.Error("AlchemyApi.OnGetRankedTaxonomyResponse()", "OnGetRankedTaxonomyResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetRankedTaxonomyRequest)req).SuccessCallback != null)
                    ((GetRankedTaxonomyRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetRankedTaxonomyRequest)req).FailCallback != null)
                    ((GetRankedTaxonomyRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region GetText
        private const string GetTextHtml = "/html/HTMLGetText";
        private const string GetTextUrl = "/url/URLGetText";

        /// <summary>
        /// On get text delegate.
        /// </summary>
        public delegate void OnGetText(TextData textData, string data);

        /// <summary>
        /// Extracts text from a source.
        /// </summary>
        /// <returns><c>true</c>, if text was gotten, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="source">HTML or URL source.</param>
        /// <param name="extractLinks">If set to <c>true</c> extract links.</param>
        /// <param name="useMetadata">If set to <c>true</c> use metadata.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetText(SuccessCallback<TextData> successCallback, FailCallback failCallback, string source, bool extractLinks = false, bool useMetadata = true, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetText.");
            if (string.IsNullOrEmpty(_apiKey))
                SetCredentials();

            GetTextRequest req = new GetTextRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            req.Parameters["apikey"] = _apiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["extractLinks"] = Convert.ToInt32(extractLinks).ToString();
            req.Parameters["useMetadata"] = Convert.ToInt32(useMetadata).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service = "";
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = GetTextUrl;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = GetTextHtml;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetText!");
            }
            else
            {
                Log.Error("AlchemyAPI.GetText()", "Either a URL or a html page source is required for GetText!");
                return false;
            }

            RESTConnector connector = RESTConnector.GetConnector(Credentials, service);
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
            /// The success callback.
            /// </summary>
            public SuccessCallback<TextData> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetTextResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            TextData result = new TextData();
            fsData data = null;
            Dictionary<string, object> customData = ((GetTextRequest)req).CustomData;

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
                    Log.Error("AlchemyApi.OnGetTextResponse()", "OnGetTextResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetTextRequest)req).SuccessCallback != null)
                    ((GetTextRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetTextRequest)req).FailCallback != null)
                    ((GetTextRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region GetRawText
        private const string GetRawTextHtml = "/html/HTMLGetRawText";
        private const string GetRawTextUrl = "/url/URLGetRawText";

        /// <summary>
        /// Gets raw text from a source.
        /// </summary>
        /// <returns><c>true</c>, if raw text was gotten, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="source">HTML or URL source.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetRawText(SuccessCallback<TextData> successCallback, FailCallback failCallback, string source, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetRawText.");
            if (string.IsNullOrEmpty(_apiKey))
                SetCredentials();

            GetTextRequest req = new GetTextRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            req.Parameters["apikey"] = _apiKey;
            req.Parameters["outputMode"] = "json";

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service = "";
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = GetTextUrl;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = GetTextHtml;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetRawText!");
            }
            else
            {
                Log.Error("AlchemyAPI.GetRawText()", "Either a URL or a html page source is required for GetRawText!");
                return false;
            }

            RESTConnector connector = RESTConnector.GetConnector(Credentials, service);
            if (connector == null)
                return false;

            req.OnResponse = OnGetTextResponse;
            return connector.Send(req);
        }
        #endregion

        #region GetTitle
        private const string GetTitleHtml = "/html/HTMLGetTitle";
        private const string GetTitleUrl = "/url/URLGetTitle";

        /// <summary>
        /// On get title delegate.
        /// </summary>
        public delegate void OnGetTitle(Title titleData, string data);

        /// <summary>
        /// Extracts the title from a source.
        /// </summary>
        /// <returns><c>true</c>, if title was gotten, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="source">HTML or URL source.</param>
        /// <param name="useMetadata">If set to <c>true</c> use metadata.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetTitle(SuccessCallback<Title> successCallback, FailCallback failCallback, string source, bool useMetadata = true, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(source))
                throw new WatsonException("Please provide a source for GetText.");
            if (string.IsNullOrEmpty(_apiKey))
                SetCredentials();

            GetTitleRequest req = new GetTitleRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            req.Parameters["apikey"] = _apiKey;
            req.Parameters["outputMode"] = "json";
            req.Parameters["useMetadata"] = Convert.ToInt32(useMetadata).ToString();

            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            string service = "";
            string normalizedSource = source.Trim().ToLower();
            if (normalizedSource.StartsWith("http://") || normalizedSource.StartsWith("https://"))
            {
                service = GetTitleUrl;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = GetTitleHtml;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetTitle!");
            }
            else
            {
                Log.Error("AlchemyAPI.GetTitle()", "Either a URL or a html page source is required for GetTitle!");
                return false;
            }

            RESTConnector connector = RESTConnector.GetConnector(Credentials, service);
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
            /// The success callback.
            /// </summary>
            public SuccessCallback<Title> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetTitleResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Title result = new Title();
            fsData data = null;
            Dictionary<string, object> customData = ((GetTitleRequest)req).CustomData;

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
                    Log.Error("AlchemyApi.OnGetTitleResponse()", "OnGetTitleResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetTitleRequest)req).SuccessCallback != null)
                    ((GetTitleRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetTitleRequest)req).FailCallback != null)
                    ((GetTitleRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Combined Call
        private const string CombinedCallHtml = "/html/HTMLGetCombinedData";
        private const string CombinedCallUrl = "/url/URLGetCombinedData";
        private const string CombinedCallText = "/text/TextGetCombinedData";

        /// <summary>
        /// On get combined data delegate.
        /// </summary>
        public delegate void OnGetCombinedData(CombinedCallData combinedData, string data);

        /// <summary>
        /// Access multiple services in one call.
        /// </summary>
        /// <returns><c>true</c>, if combined data was gotten, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
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
        public bool GetCombinedData(SuccessCallback<CombinedCallData> successCallback, FailCallback failCallback, string source,
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
            Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
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
            if (string.IsNullOrEmpty(_apiKey))
                SetCredentials();

            CombinedCallRequest req = new CombinedCallRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            req.Parameters["apikey"] = _apiKey;
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
                service = CombinedCallUrl;
                req.Forms["url"] = new RESTConnector.Form(source);
            }
            else if (source.StartsWith(Application.dataPath) && !normalizedSource.StartsWith("http://") && !normalizedSource.StartsWith("https://"))
            {
                if (Path.GetExtension(normalizedSource).EndsWith(".html"))
                {
                    service = CombinedCallHtml;
                    string htmlData = default(string);
                    htmlData = File.ReadAllText(source);
                    req.Forms["html"] = new RESTConnector.Form(htmlData);
                }
                else
                    throw new WatsonException("An HTML source is needed for GetCombinedData!");
            }
            else
            {
                service = CombinedCallText;
                req.Forms["text"] = new RESTConnector.Form(source);
            }

            RESTConnector connector = RESTConnector.GetConnector(Credentials, service);
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
            /// The success callback.
            /// </summary>
            public SuccessCallback<CombinedCallData> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnCombinedCallResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            CombinedCallData result = new CombinedCallData();
            fsData data = null;
            Dictionary<string, object> customData = ((CombinedCallRequest)req).CustomData;

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
                    Log.Error("AlchemyApi.OnCombinedCallResponse()", "OnCombinedCallResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((CombinedCallRequest)req).SuccessCallback != null)
                    ((CombinedCallRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((CombinedCallRequest)req).FailCallback != null)
                    ((CombinedCallRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region GetNews
        private const string GetNewsEndpoint = "/data/GetNews";

        /// <summary>
        /// On get news delegate.
        /// </summary>
        public delegate void OnGetNews(NewsResponse newsData, string data);

        /// <summary>
        /// Gets news.
        /// </summary>
        /// <returns><c>true</c>, if news was gotten, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="returnFields">Fields returned.</param>
        /// <param name="queryFields">Values for each field.</param>
        /// <param name="startDate">Date to start the query.</param>
        /// <param name="endDate">Date to end the query.</param>
        /// <param name="maxResults">Maximum number of results.</param>
        /// <param name="timeSlice">the duration (in seconds) of each time slice. a human readable duration is also acceptable e.g. '1d', '4h', '1M', etc.
        /// If set, this parameter causes the query engine to return a time series representing the count in each slice of time. If omitted, the query engine returns the total count over the time duration.</param>
        /// <param name="customData">Custom data.</param>
        public bool GetNews(SuccessCallback<NewsResponse> successCallback, FailCallback failCallback,
            string[] returnFields = default(string[]),
            Dictionary<string, string> queryFields = null,
            string startDate = "now-1d",
            string endDate = "now",
            int maxResults = 10,
            string timeSlice = default(string),
            Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(_apiKey))
                SetCredentials();

            GetNewsRequest req = new GetNewsRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;

            req.Parameters["apikey"] = _apiKey;
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

            RESTConnector connector = RESTConnector.GetConnector(Credentials, GetNewsEndpoint);
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
            /// The success callback.
            /// </summary>
            public SuccessCallback<NewsResponse> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetNewsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            NewsResponse result = new NewsResponse();
            fsData data = null;
            Dictionary<string, object> customData = ((GetNewsRequest)req).CustomData;

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
                    Log.Error("AlchemyAPI.OnGetNewsResponse()", "OnGetNewsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetNewsRequest)req).SuccessCallback != null)
                    ((GetNewsRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetNewsRequest)req).FailCallback != null)
                    ((GetNewsRequest)req).FailCallback(resp.Error, customData);
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
