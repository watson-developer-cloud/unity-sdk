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

namespace IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1
{
    /// <summary>
    /// Service integration for Alchemy API
    /// </summary>
    public class AlchemyAPI : IWatsonService
    {

        #region Private Data
        private const string SERVICE_ID = "AlchemyAPIV1";
        private static string mp_ApiKey = null;

        private static fsSerializer sm_Serializer = new fsSerializer();
        #endregion


        #region Entity Extraction
        private const string SERVICE_ENTITY_EXTRACTION = "/calls/text/TextGetRankedNamedEntities";

        public delegate void OnGetEntityExtraction(EntityExtractionData entityExtractionData, string data);

        //http://access.alchemyapi.com/calls/text/TextGetRankedNamedEntities

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
            EntityExtractionData entityExtractionData = new EntityExtractionData();
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
                    Log.Error("AlchemyAPI", "OnGetEntityExtractionResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetEntityExtractionRequest)req).Callback != null)
                ((GetEntityExtractionRequest)req).Callback(resp.Success ? entityExtractionData : null, ((GetEntityExtractionRequest)req).Data);
        }

        #endregion

        #region Keywoard Extraction

        private const string SERVICE_KEYWOARD_EXTRACTION = "/calls/text/TextGetRankedKeywords";

        public delegate void OnGetKeywoardExtraction(KeywoardExtractionData entityExtractionData, string data);

        public bool GetKeywoardExtraction(OnGetKeywoardExtraction callback, string text, string customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(text))
                throw new WatsonException("GetKeywoardExtraction needs to have some text to work.");
            if (string.IsNullOrEmpty(mp_ApiKey))
                mp_ApiKey = Config.Instance.GetVariableValue("ALCHEMY_API_KEY");
            if (string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("GetKeywoardExtraction - ALCHEMY_API_KEY needs to be defined in config.json");


            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_KEYWOARD_EXTRACTION);
            if (connector == null)
                return false;

            GetKeywoardExtractionRequest req = new GetKeywoardExtractionRequest();
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

            req.OnResponse = OnGetKeywoardExtractionResponse;
            req.Data = string.IsNullOrEmpty(customData) ? text : customData;

            return connector.Send(req);
        }

        private class GetKeywoardExtractionRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetKeywoardExtraction Callback { get; set; }
        };

        private void OnGetKeywoardExtractionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            KeywoardExtractionData keywoardExtractionData = new KeywoardExtractionData();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = keywoardExtractionData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("AlchemyAPI", "OnGetKeywoardExtractionResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetKeywoardExtractionRequest)req).Callback != null)
                ((GetKeywoardExtractionRequest)req).Callback(resp.Success ? keywoardExtractionData : null, ((GetKeywoardExtractionRequest)req).Data);
        }

        #endregion

        #region Combined Call

        private const string SERVICE_COMBINED_CALLS = "/calls/text/TextGetCombinedData";

        public delegate void OnGetCombinedCall(CombinedCallData combinedCallData, string data);

        //http://access.alchemyapi.com/calls/text/TextGetRankedNamedEntities

        public bool GetCombinedCall(OnGetCombinedCall callback, string text,
            bool includeEntity = true,
            bool includeKeywoard = true,
            bool includeDate = true,
            bool includeTaxonomy = false,
            bool includeConcept = false,
            bool includeFeed = false,
            bool includeDocEmotion = false,
            bool includeRelation = false,
            bool includePubDate = false,
            bool includeDocSentiment = false,
            bool includePageImage = false,
            bool includeImageKW = false,
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
            if (!includeEntity
                && !includeKeywoard
                && !includeDate
                && !includeTaxonomy
                && !includeConcept
                && !includeFeed
                && !includeDocEmotion
                && !includeRelation
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
            if (includeEntity)
                requestServices.Add("entity");
            if (includeKeywoard)
                requestServices.Add("keyword");
            if (includeKeywoard)
                requestServices.Add("dates");
            if (includeTaxonomy)
                requestServices.Add("taxonomy");
            if (includeConcept)
                requestServices.Add("concept");
            if (includeFeed)
                requestServices.Add("feed");
            if (includeDocEmotion)
                requestServices.Add("doc-emotion");
            if (includeRelation)
                requestServices.Add("relation");
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
                    Log.Error("AlchemyAPI", "OnGetCombinedCallResponse Exception: {0}", e.ToString());
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
            private AlchemyAPI m_Service = null;
            private ServiceStatus m_Callback = null;

            public CheckServiceStatus(AlchemyAPI service, ServiceStatus callback)
            {
                m_Service = service;
                m_Callback = callback;

                if (!m_Service.GetEntityExtraction(OnGetEntityExtraction, "Test"))
                    m_Callback(SERVICE_ID, false);
            }

            void OnGetEntityExtraction(EntityExtractionData entityExtractionData, string data)
            {
                if (m_Callback != null)
                    m_Callback(SERVICE_ID, entityExtractionData != null);
            }

        };
        #endregion
    }
}
