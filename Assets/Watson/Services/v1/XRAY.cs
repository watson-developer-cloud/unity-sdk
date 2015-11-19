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


using FullSerializer;
using IBM.Watson.Connection;
using IBM.Watson.Data.XRAY;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IBM.Watson.Services.v1
{
    /// <summary>
    /// This class wraps the XRAY back-end service.
    /// </summary>
    /// <remarks>This is an experimental service.</remarks>
    public class XRAY
    {
        #region Public Types
        /// <summary>
        /// The callback delegate for AskQuestion().
        /// </summary>
        /// <param name="questions">The </param>
        public delegate void OnAskQuestion( AskResponse response );
        #endregion

        #region Public Properties
        /// <summary>
        /// If true, then we will not use a local cache.
        /// </summary>
        public bool DisableCache { get; set; }
        /// <summary>
        /// The users current location, this is set when Login() is invoked.
        /// </summary>
        public string Location { get { return m_Location; } set { m_Location = value; } }
        #endregion

        #region Private Data
        private string m_Location = "Austin, TX";
        private static fsSerializer sm_Serializer = new fsSerializer();
        private const string SERVICE_ID = "XrayV1";
        private const string XRAY_SUBSYSTEM = "XRAY";
        private const string ASK_QUESTION = "/ITM/en/askXRAY";
        private Dictionary<string,DataCache> m_QuestionCache = new Dictionary<string, DataCache>();
        #endregion


        #region AskQuestion
        /// <summary>
        /// Ask a question.
        /// </summary>
        /// <param name="pipeline">The pipeline name.</param>
        /// <param name="question">The text of the question to ask.</param>
        /// <param name="callback">The callback to received the Questions object.</param>
        /// <returns>Returns true if the request was submitted.</returns>
        public bool AskQuestion( string pipeline, string question, OnAskQuestion callback)
        {
            if ( string.IsNullOrEmpty(pipeline) )
                throw new ArgumentNullException("pipeline");
            if ( string.IsNullOrEmpty(question) )
                throw new ArgumentNullException("question");
            if ( callback == null )
                throw new ArgumentNullException("callback" );

            string questionId = Utility.GetMD5( question );
            if (! DisableCache )
            {
                DataCache cache = null;
                if (! m_QuestionCache.TryGetValue( pipeline, out cache ) )
                {
                    cache = new DataCache( "XRAY_" + pipeline );
                    m_QuestionCache[ pipeline ] = cache;
                }

                byte [] cached = cache.Find( questionId );
                if ( cached != null )
                {
                    AskResponse response = ProcessAskResponse( cached );
                    if ( response != null )
                    {
                        callback( response );
                        return true;
                    }
                }
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, ASK_QUESTION );
            if (connector == null)
                return false;

            AskQuestionReq req = new AskQuestionReq();
            req.Pipeline = pipeline;
            req.QuestionId = questionId;
            req.Callback = callback;
            req.OnResponse = OnAskQuestionResp;
            req.Parameters["authorizationKey"] = "fa3ca8fd7210a7bf987a823141001a6b";
            req.Parameters["pipeline"] = pipeline;
            req.Parameters["questionText"] = question;

            return connector.Send(req);
        }

        private class AskQuestionReq : RESTConnector.Request
        {
            public string Pipeline { get; set; }
            public string QuestionId { get; set; }
            public OnAskQuestion Callback { get; set; }
        };

        private void OnAskQuestionResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            AskResponse response = null;
            if (resp.Success)
            {
                response = ProcessAskResponse( resp.Data );
                if ( response != null )
                {
                    DataCache cache = null;
                    if ( m_QuestionCache.TryGetValue( ((AskQuestionReq)req).Pipeline, out cache ) )
                        cache.Save( ((AskQuestionReq)req).QuestionId, resp.Data );
                }
            }

            if (((AskQuestionReq)req).Callback != null)
                ((AskQuestionReq)req).Callback(response);
        }

        private AskResponse ProcessAskResponse( byte [] json_data )
        {
            AskResponse response = null;
            try
            {
                fsData data = null;
                fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(json_data), out data);
                if (!r.Succeeded)
                    throw new WatsonException(r.FormattedMessages);

                response = new AskResponse();

                object obj = response;
                r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                if (!r.Succeeded)
                    throw new WatsonException(r.FormattedMessages);
            }
            catch (Exception e)
            {
                Log.Error("XRAY", "GetClassifiers Exception: {0}", e.ToString());
            }

            return response;
        }

        #endregion
    }

}

