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
using IBM.Watson.Data.QA;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IBM.Watson.Services.v1
{
    /// <summary>
    /// The DeepQA provides an abstraction for the deepQa/ services of Watson.
    /// </summary>
    public class DeepQA
    {
        #region Public Types
        /// <exclude />
        public delegate void OnQuestion( Question response );
        #endregion

        #region Public Properties
        public bool DisableCache { get; set; }
        #endregion

        #region Private Data
        private string m_ServiceID = null;
        private DataCache m_ParseCache = null;
        private DataCache m_QuestionCache = null;

        private static fsSerializer sm_Serializer = new fsSerializer();
        private const string ASK_QUESTION = "/v1/question";
        private const string PARSE_QUESTION = "/v1/question/preprocess";
        #endregion

        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="serviceID">The ID of the service.</param>
        public DeepQA(string serviceID)
        {
            m_ServiceID = serviceID;
        }

        #region AskQuestion
        /// <summary>
        /// This function flushes all data from the answer cache.
        /// </summary>
        public void FlushAnswerCache()
        {
            if ( m_QuestionCache == null )
                m_QuestionCache  = new DataCache(m_ServiceID );

            m_QuestionCache.Flush();
        }

        /// <summary>
        /// Ask a question using the given pipeline.
        /// </summary>
        /// <param name="question">The text of the question.</param>
        /// <param name="callback">The callback to receive the response.</param>
        /// <returns>Returns true if the request was submitted.</returns>
        public bool AskQuestion( string question, OnQuestion callback, int evidenceItems = 1 )
        {
            if ( string.IsNullOrEmpty( question ) )
                throw new ArgumentNullException("question");
            if ( callback == null )
                throw new ArgumentNullException("callback");

            if ( !DisableCache )
            {
                if ( m_QuestionCache == null )
                    m_QuestionCache = new DataCache(m_ServiceID);

                byte[] cachedQuestion = m_QuestionCache.Find(question.GetHashCode().ToString());
                if (cachedQuestion != null)
                {
                    Response response = ProcessAskResp( cachedQuestion );
                    if ( response != null )
                    {
                        callback( response.question );
                        return true;
                    }
                }
            }

            RESTConnector connector = RESTConnector.GetConnector(m_ServiceID, ASK_QUESTION );
            if (connector == null)
                return false;

            Dictionary<string,object> questionJson = new Dictionary<string, object>();
            questionJson["question"] = new Dictionary<string,object>() {
                { "questionText", question },
                { "evidenceRequest", new Dictionary<string,object>() {
                    { "items", evidenceItems },
                    { "profile", "NO" }
                } }
            };
            string json = MiniJSON.Json.Serialize( questionJson );

            AskQuestionReq req = new AskQuestionReq();
            req.QuestionID = question.GetHashCode().ToString();
            req.Callback = callback;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["X-Synctimeout"] = "-1";
            req.Send = Encoding.UTF8.GetBytes( json );
            req.OnResponse = OnAskQuestionResp;

            return connector.Send(req);
        }
        private class AskQuestionReq : RESTConnector.Request
        {
            public string QuestionID { get; set; }
            public OnQuestion Callback { get; set; }
        };
        private void OnAskQuestionResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Response response = null;
            if (resp.Success)
            {
                try
                {
                    response = ProcessAskResp( resp.Data );
                    if ( m_QuestionCache != null && response != null )
                        m_QuestionCache.Save( ((AskQuestionReq)req).QuestionID, resp.Data );
                }
                catch (Exception e)
                {
                    Log.Error("NLC", "GetClassifiers Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((AskQuestionReq)req).Callback != null)
            {
                if (resp.Success && response != null)
                    ((AskQuestionReq)req).Callback(response.question);
                else
                    ((AskQuestionReq)req).Callback(null);
            }
        }

        private Response ProcessAskResp(byte[] json_data)
        {
            Response response = new Response();

            fsData data = null;
            fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(json_data), out data);
            if (!r.Succeeded)
                throw new WatsonException(r.FormattedMessages);

            object obj = response;
            r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
            if (!r.Succeeded)
                throw new WatsonException(r.FormattedMessages);

            return response;
        }

        #endregion

        #region ParseQuestion
        /// <summary>
        /// This function flushes all data from the answer cache.
        /// </summary>
        public void FlushParseCache()
        {
            if ( m_QuestionCache == null )
                m_QuestionCache  = new DataCache(m_ServiceID );

            m_QuestionCache.Flush();
        }

        /// <summary>
        /// Ask a question using the given pipeline.
        /// </summary>
        /// <param name="question">The text of the question.</param>
        /// <param name="callback">The callback to receive the response.</param>
        /// <returns>Returns true if the request was submitted.</returns>
        public bool ParseQuestion( string question, OnQuestion callback )
        {
            if ( string.IsNullOrEmpty( question ) )
                throw new ArgumentNullException("question");
            if ( callback == null )
                throw new ArgumentNullException("callback");

            string questionId = question.GetHashCode().ToString();
            if ( !DisableCache )
            {
                if ( m_ParseCache == null )
                    m_ParseCache = new DataCache(m_ServiceID + "_parse" );

                byte[] cached = m_ParseCache.Find(questionId);
                if (cached != null)
                {
                    Response response = ProcessParseResp( cached );
                    if ( response != null )
                    {
                        callback( response.question );
                        return true;
                    }
                }
            }

            RESTConnector connector = RESTConnector.GetConnector(m_ServiceID, PARSE_QUESTION );
            if (connector == null)
                return false;

            Dictionary<string,object> questionJson = new Dictionary<string, object>();
            questionJson["question"] = new Dictionary<string,object>() {
                { "questionText", question },
                { "evidenceRequest", new Dictionary<string,object>() {
                    { "items", 1 },
                    { "profile", "NO" }
                } }
            };
            string json = MiniJSON.Json.Serialize( questionJson );

            ParseQuestionReq req = new ParseQuestionReq();
            req.QuestionID = questionId;
            req.Callback = callback;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["X-Synctimeout"] = "-1";
            req.Send = Encoding.UTF8.GetBytes( json );
            req.OnResponse = OnParseQuestionResp;

            return connector.Send(req);
        }
        private class ParseQuestionReq : RESTConnector.Request
        {
            public string QuestionID { get; set; }
            public OnQuestion Callback { get; set; }
        };
        private void OnParseQuestionResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Response response = null;
            if (resp.Success)
            {
                try
                {
                    response = ProcessParseResp( resp.Data );
                    if ( m_ParseCache != null && response != null )
                        m_ParseCache.Save( ((ParseQuestionReq)req).QuestionID, resp.Data );
                }
                catch (Exception e)
                {
                    Log.Error("NLC", "GetClassifiers Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((ParseQuestionReq)req).Callback != null)
            {
                if (resp.Success && response != null)
                    ((ParseQuestionReq)req).Callback(response.question);
                else
                    ((ParseQuestionReq)req).Callback(null);
            }
        }

        private Response ProcessParseResp(byte[] json_data)
        {
            Response response = new Response();

            fsData data = null;
            fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(json_data), out data);
            if (!r.Succeeded)
                throw new WatsonException(r.FormattedMessages);

            object obj = response;
            r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
            if (!r.Succeeded)
                throw new WatsonException(r.FormattedMessages);

            return response;
        }
        #endregion
    }
}
