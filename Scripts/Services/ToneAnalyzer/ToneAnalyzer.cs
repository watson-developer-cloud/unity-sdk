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

namespace IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v1
{
    public class ToneAnalyzer : IWatsonService {

        #region Private Data
        private const string SERVICE_ID = "ToneAnalyzerV1";
        private static fsSerializer sm_Serializer = new fsSerializer();
        #endregion

        #region Get Tone

        private const string FUNCTION_TONE = "/v3/tone";

        public delegate void OnGetToneAnalyzed( ToneAnalyzerResponse resp, string data );

        public bool GetToneAnalyze(OnGetToneAnalyzed callback, string text, string data = null)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, FUNCTION_TONE);
            if (connector == null)
                return false;

            GetToneAnalyzerRequest req = new GetToneAnalyzerRequest();
            req.Callback = callback;
            req.OnResponse = GetToneAnalyzerResponse;

            Dictionary<string,string> upload = new Dictionary<string, string>();
            upload["text"] = "\"" + text + "\"";
            req.Send = Encoding.UTF8.GetBytes( Json.Serialize( upload ) ); 
            req.Data = data;
            req.Headers["Content-Type"] = "application/json";
            req.Parameters["version"] = "2016-02-11";
            req.Parameters["sentences"] = "true";
            return connector.Send(req);
        }

        private class GetToneAnalyzerRequest : RESTConnector.Request
        {
            public string Data {get;set;}
            public OnGetToneAnalyzed Callback { get; set; }
        };

        private void GetToneAnalyzerResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ToneAnalyzerResponse response = new ToneAnalyzerResponse();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = response;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("ToneAnalyzer", "GetToneAnalyzerResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetToneAnalyzerRequest)req).Callback != null)
                ((GetToneAnalyzerRequest)req).Callback(resp.Success ? response : null, ((GetToneAnalyzerRequest)req).Data);
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
            if ( Utilities.Config.Instance.FindCredentials( SERVICE_ID ) != null )
                new CheckServiceStatus( this, callback );
            else
                callback( SERVICE_ID, false );
        }

        private class CheckServiceStatus
        {
            private ToneAnalyzer m_Service = null;
            private ServiceStatus m_Callback = null;

            public CheckServiceStatus( ToneAnalyzer service, ServiceStatus callback )
            {
                m_Service = service;
                m_Callback = callback;

                if (! m_Service.GetToneAnalyze( OnGetToneAnalyzed, "Test" ) )
                    m_Callback( SERVICE_ID, false );
            }

            private void OnGetToneAnalyzed( ToneAnalyzerResponse resp , string data)
            {
                if ( m_Callback != null )
                    m_Callback( SERVICE_ID, resp != null );
            }
        };
        #endregion
    }
}

