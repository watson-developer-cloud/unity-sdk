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


using IBM.Watson.Logging;
using IBM.Watson.Connection;
using IBM.Watson.Data.XRAY;
using MiniJSON;
using System;
using System.Collections;
using UnityEngine;
using System.Text;
using IBM.Watson.Utilities;

namespace IBM.Watson.Services.v1
{
    /// <summary>
    /// In The Moment service abstraction.
    /// </summary>
    public class ITM
    {
        #region Constants
        /// <summary>
        /// This ID is used to match up a configuration record with this service.
        /// </summary>
        private const string SERVICE_ID = "ItmV1";
        private const string ITM_SUBSYSTEM = "ITM";
        #endregion

        #region Public Types
        /// <summary>
        /// This callback object is used by the Recognize() and StartListening() methods.
        /// </summary>
        /// <param name="results">The ResultList object containing the results.</param>
        public delegate void OnParseQuestion( Data.XRAY.ParseData parse );
        #endregion

        #region Public Properties
        public bool DisableCache { get; set; }
        #endregion
        #region Private Data
        private DataCache m_ParseCache = null;
        #endregion

        #region ParseQuestion
        /// <summary>
        /// Parses the given question and returns the ParseData to the callback.
        /// </summary>
        /// <param name="question"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool ParseQuestion( string question, OnParseQuestion callback )
        {
            if (string.IsNullOrEmpty(question))
                throw new ArgumentNullException("question");
            if (callback == null)
                throw new ArgumentNullException("callback");

            question = WWW.EscapeURL(question);
            question = question.Replace("+", "%20");
            question = question.Replace("%0a", "");

            string parseId = Utility.GetMD5( question );
            if (! DisableCache )
            {
                if ( m_ParseCache == null )
                    m_ParseCache = new DataCache( "ITM_parse" );

                byte [] cached = m_ParseCache.Find( parseId );
                if ( cached != null )
                {
                    ParseData parse = new ParseData();
                    if ( parse.ParseJson( (IDictionary)Json.Deserialize( Encoding.UTF8.GetString( cached ) ) ) )
                    {
                        callback( parse );
                        return true;
                    }
                }
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/ITM/en/preProc");
            if (connector == null)
                return false;

            ParseQuestionReq req = new ParseQuestionReq();
            req.ParseId = parseId;
            req.Function = "/" + question;
            req.Callback = callback;
            req.OnResponse = OnParseQuestionResp;

            return connector.Send(req);
        }

        private class ParseQuestionReq : RESTConnector.Request
        {
            public string ParseId { get; set; }
            public OnParseQuestion Callback { get; set; }
        };

        private void OnParseQuestionResp( RESTConnector.Request req, RESTConnector.Response resp )
        {
            ParseData parse = null;
            if ( resp.Success )
            {
                try
                {
                    parse = new ParseData();
                    if (!parse.ParseJson((IDictionary)Json.Deserialize(Encoding.UTF8.GetString(resp.Data))))
                        resp.Success = false;

                    if ( resp.Success && m_ParseCache != null )
                        m_ParseCache.Save( ((ParseQuestionReq)req).ParseId, resp.Data );
                }
                catch (Exception e)
                {
                    Log.Error(ITM_SUBSYSTEM, "Exception during parse: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((ParseQuestionReq)req).Callback != null)
                ((ParseQuestionReq)req).Callback(resp.Success ? parse : null);
        }

        #endregion
    }
}
