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
    public class AlchemyAPI : IWatsonService {

        #region Private Data
        private const string SERVICE_ID = "AlchemyAPIV1";
        private static fsSerializer sm_Serializer = new fsSerializer();
        #endregion

        #region Entity Extraction
        private const string SERVICE_ENTITY_EXTRACTION = "/calls/text/TextGetRankedNamedEntities";
        private string mp_ApiKey = null;

        public delegate void OnGetEntityExtraction( EntityExtractionData entityExtractionData );

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

            return connector.Send(req);
        }

        private class GetEntityExtractionRequest : RESTConnector.Request
        {
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
                ((GetEntityExtractionRequest)req).Callback(resp.Success ? entityExtractionData : null);
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
            if ( Config.Instance.FindCredentials( SERVICE_ID ) != null )
                new CheckServiceStatus( this, callback );
            else
                callback( SERVICE_ID, false );
        }

        private class CheckServiceStatus
        {
            private AlchemyAPI m_Service = null;
            private ServiceStatus m_Callback = null;

            public CheckServiceStatus( AlchemyAPI service, ServiceStatus callback )
            {
                m_Service = service;
                m_Callback = callback;

                if (! m_Service.GetEntityExtraction( OnGetEntityExtraction, "Test" ) )
                    m_Callback( SERVICE_ID, false );
            }

            void OnGetEntityExtraction (EntityExtractionData entityExtractionData)
            {
                if ( m_Callback != null ) 
                    m_Callback( SERVICE_ID, entityExtractionData != null );
            }

        };
        #endregion
    }
}
