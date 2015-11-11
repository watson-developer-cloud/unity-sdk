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
using MiniJSON;
using System;
using System.Collections;
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
        /// Callback for GetPipeline() method.
        /// </summary>
        /// <param name="pipeline"></param>
        public delegate void OnGetPipeline(Pipeline pipeline);
        /// <summary>
        /// Callback for GetPipelines() method.
        /// </summary>
        /// <param name="pipes"></param>
        public delegate void OnGetPipelines(Pipelines pipes);
        /// <summary>
        /// Callback for GetQuestions() method.
        /// </summary>
        /// <param name="questions"></param>
        public delegate void OnGetQuestions(Questions questions);
        /// <summary>
        /// Callback for GetParseData() method.
        /// </summary>
        /// <param name="data"></param>
        public delegate void OnGetParseData(ParseData data);
        /// <summary>
        /// Callback fro GetAnswers() method.
        /// </summary>
        /// <param name="answers"></param>
        public delegate void OnGetAnswers(Answers answers);
        #endregion

        #region Public Properties
        /// <summary>
        /// The currently selected pipeline.
        /// </summary>
        public Pipeline SelectedPipeline { get { return m_SelectedPipeline; } set { m_SelectedPipeline = value; } }
        /// <summary>
        /// Our session key, this is set when Login() is invoked.
        /// </summary>
        public long SessionKey { get; set; }
        /// <summary>
        /// The users current location, this is set when Login() is invoked.
        /// </summary>
        public string Location { get; set; }
        #endregion

        #region Private Data
        private static fsSerializer sm_Serializer = new fsSerializer();
        private Pipeline m_SelectedPipeline = null;
        private const string SERVICE_ID = "XrayV1";
        private const string XRAY_SUBSYSTEM = "XRAY";

        private const string LOGIN = "/ITM/en/user/";
        private const string GET_PIPELINES = "/ITM/en/user/ibm";
        private const string GET_QUESTIONS = "/ITM/en/stream";
        private const string GET_QUESTION = "/ITM/en/transaction";
        private const string GET_ANSWERS = "/ITM/en/answers";
        private const string GET_PARSE = "/ITM/en/parse";
        private const string ASK_QUESTION = "/ITM/en/ask/";
        #endregion

        #region Login
        /// <summary>
        /// The callback delegate for Login().
        /// </summary>
        /// <param name="success">True on success, False on failure.</param>
        public delegate void OnLogin(bool success);

        /// <summary>
        /// Login into XRAY.
        /// </summary>
        /// <param name="callback">The callback to invoke on success or failure.</param>
        /// <returns>Returns true if the request is submitted.</returns>
        public bool Login(OnLogin callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, LOGIN );
            if (connector == null)
                return false;

            LoginReq req = new LoginReq();
            req.Callback = callback;
            req.OnResponse = OLoginResponse;
            req.Function = connector.Authentication.User;

            return connector.Send(req);
        }

        private class LoginReq : RESTConnector.Request
        {
            public OnLogin Callback { get; set; }
        };

        private void OLoginResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            if (resp.Success)
            {
                try {
                    IDictionary json = Json.Deserialize(Encoding.UTF8.GetString(resp.Data)) as IDictionary;
                    if (json != null && json.Contains("sessionKey"))
                    {
                        SessionKey = (long)json["sessionKey"];
                        if ( json.Contains( "location" ) )
                            Location = (string)json["location"];
                    }
                    else
                        resp.Success = false;
                }
                catch( Exception e )
                {
                    Log.Error( XRAY_SUBSYSTEM, "Login exception: {0}", e.ToString() );
                    resp.Success = false;
                }
            }

            if (((LoginReq)req).Callback != null)
                ((LoginReq)req).Callback(resp.Success);
        }

        #endregion

        #region GetPipeline
        /// <summary>
        /// Gets/Selects a pipeline by name.
        /// </summary>
        /// <param name="name">The name of the pipeline to get.</param>
        /// <param name="select">If true, then pipeline will be set as the active pipeline.</param>
        /// <param name="callback">Optional callback to invoke.</param>
        /// <returns></returns>
        public bool GetPipeline(string name, bool select, OnGetPipeline callback = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("pipelineName");

            new GetPipelineReq(name, select, callback, this);
            return true;
        }

        private class GetPipelineReq
        {
            private string m_Name = null;
            private bool m_Select = false;
            private OnGetPipeline m_Callback = null;
            private XRAY m_Service = null;

            public GetPipelineReq(string name, bool select, OnGetPipeline callback, XRAY service)
            {
                m_Name = name;
                m_Select = select;
                m_Callback = callback;
                m_Service = service;

                m_Service.GetPipelines(OnGetPipelines);
            }

            public void OnGetPipelines(Pipelines pipes)
            {
                bool bFound = false;
                if (pipes != null)
                {
                    for (int i = 0; i < pipes.pipelines.Length; ++i)
                        if (pipes.pipelines[i].pipelineName == m_Name)
                        {
                            bFound = true;
                            if (m_Callback != null)
                                m_Callback(pipes.pipelines[i]);
                            if (m_Select)
                                m_Service.SelectedPipeline = pipes.pipelines[i];
                        }
                }

                if (!bFound)
                {
                    if (m_Callback != null)
                        m_Callback(null);
                    if (m_Select)
                        Log.Error(XRAY_SUBSYSTEM, "Failed to select pipeline {0}", m_Name);
                }
            }
        };

        /// <summary>
        /// Get all pipelines from the XRAY service. This invokes the callback with an array of all available pipelines.
        /// </summary>
        /// <param name="callback">The callback to invoke.</param>
        /// <returns>Returns true if request was sent, if a failure occurs the callback will be invoked with null.</returns>
        public bool GetPipelines(OnGetPipelines callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, GET_PIPELINES );
            if (connector == null)
                return false;

            GetPipelinesReq req = new GetPipelinesReq();
            req.Callback = callback;
            req.OnResponse = OnGetPipelinesResponse;

            return connector.Send(req);
        }

        private class GetPipelinesReq : RESTConnector.Request
        {
            public OnGetPipelines Callback { get; set; }
        };

        private void OnGetPipelinesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Pipelines pipelines = new Pipelines();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = pipelines;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error(XRAY_SUBSYSTEM, "GetPipelines Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetPipelinesReq)req).Callback != null)
                ((GetPipelinesReq)req).Callback(resp.Success ? pipelines : null);
        }
        #endregion

        #region GetQuestions
        /// <summary>
        /// This returns an array of questions from the DB that have been asked recently. 
        /// </summary>
        /// <param name="callback">The callback to invoke with the array of questions.</param>
        /// <param name="limit">Maximum number of questions to return.</param>
        /// <param name="skip">Number of questions to skip in the table.</param>
        /// <returns></returns>
        public bool GetQuestions(OnGetQuestions callback, int limit = 10, int skip = 0)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (SelectedPipeline == null)
                throw new WatsonException("You must select a pipeline before calling GetQuestions()");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, GET_QUESTIONS );
            if (connector == null)
                return false;

            GetQuestionsReq req = new GetQuestionsReq();
            req.Callback = callback;
            req.OnResponse = OnGetQuestionsResponse;
            req.Parameters["limit"] = limit.ToString();
            req.Parameters["skip"] = skip.ToString();
            req.Parameters["user"] = SelectedPipeline.clientId;

            return connector.Send(req);
        }

        private class GetQuestionsReq : RESTConnector.Request
        {
            public OnGetQuestions Callback { get; set; }
        };

        private void OnGetQuestionsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Questions questions = new Questions();

            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = questions;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error(XRAY_SUBSYSTEM, "GetQuestions Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetQuestionsReq)req).Callback != null)
                ((GetQuestionsReq)req).Callback(resp.Success ? questions : null);
        }
        #endregion

        #region GetQuestion
        /// <summary>
        /// This returns a single question by transaction ID.
        /// </summary>
        /// <param name="transactionId">The transaction ID.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>Returns true if the request was submitted correctly.</returns>
        public bool GetQuestion(long transactionId, OnGetQuestions callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, GET_QUESTION );
            if (connector == null)
                return false;

            GetQuestionsReq req = new GetQuestionsReq();
            req.Callback = callback;
            req.OnResponse = OnGetQuestionsResponse;        // we use the same callback as GetQuestions()
            req.Function = "/" + transactionId.ToString();

            return connector.Send(req);
        }
        #endregion

        #region GetAnswers
        /// <summary>
        /// Returns answers for the given transactionId. 
        /// </summary>
        /// <param name="transactionId">The transaction ID to look up the answers for.</param>
        /// <param name="callback">The callback to invoke with the results.</param>
        /// <returns>Returns false if unable to submit the result. If true is returned, the
        /// the callback will always be invoked on failure or success.</returns>
        public bool GetAnswers(long transactionId, OnGetAnswers callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, GET_ANSWERS );
            if (connector == null)
                return false;

            GetAnswersReq req = new GetAnswersReq();
            req.Function = "/" + transactionId.ToString();
            req.Callback = callback;
            req.OnResponse = OnGetAnswersResponse;

            return connector.Send(req);
        }
        private class GetAnswersReq : RESTConnector.Request
        {
            public OnGetAnswers Callback { get; set; }
        };
        private void OnGetAnswersResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Answers answers = new Answers();

            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = answers;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error(XRAY_SUBSYSTEM, "GetAnswers Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetAnswersReq)req).Callback != null)
                ((GetAnswersReq)req).Callback(resp.Success ? answers : null);
        }

        #endregion

        #region GetParseData
        /// <summary>
        /// This returns the parse data for specific transaction ID.
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool GetParseData(long transactionId, OnGetParseData callback)
        {
            if (transactionId == 0)
                throw new ArgumentNullException("transactionId");
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (SelectedPipeline == null)
                throw new WatsonException("You must select a pipeline before calling GetParseData()");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, GET_PARSE );
            if (connector == null)
                return false;

            GetParseDataReq req = new GetParseDataReq();
            req.Callback = callback;
            req.Function = "/" + transactionId.ToString() + "/" + SelectedPipeline.clientId;
            req.OnResponse = GetParseDataResponse;

            return connector.Send(req);
        }

        private class GetParseDataReq : RESTConnector.Request
        {
            public OnGetParseData Callback { get; set; }
        }

        private void GetParseDataResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ParseData parse = new ParseData();
            try
            {
                if (!parse.ParseJson((IDictionary)Json.Deserialize(Encoding.UTF8.GetString(resp.Data))))
                    resp.Success = false;
            }
            catch (Exception e)
            {
                Log.Error(XRAY_SUBSYSTEM, "Exception during parse: {0}", e.ToString());
                resp.Success = false;
            }

            if (((GetParseDataReq)req).Callback != null)
                ((GetParseDataReq)req).Callback(resp.Success ? parse : null);
        }

        #endregion

        #region AskQuestion
        /// <summary>
        /// The callback delegate for AskQuestion().
        /// </summary>
        /// <param name="questions">The </param>
        public delegate void OnAskQuestion(Questions questions);

        /// <summary>
        /// Ask a question.
        /// </summary>
        /// <param name="question">The text of the question to ask.</param>
        /// <param name="callback">The callback to received the Questions object.</param>
        /// <returns>Returns true if the request was submitted.</returns>
        public bool AskQuestion(string question, OnAskQuestion callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (SelectedPipeline == null)
                throw new WatsonException("You must select a pipeline before calling AskQuestion()");


            question = WWW.EscapeURL(question);
            question = question.Replace("+", "%20");
            question = question.Replace("%0a", "");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, ASK_QUESTION + SelectedPipeline.clientId + "/" + SelectedPipeline.pipelineName);
            if (connector == null)
                return false;

            AskQuestionReq req = new AskQuestionReq();
            req.Function = "/" + question;
            //req.Parameters["sessionKey"] = SessionKey.ToString();
            req.Callback = callback;
            req.OnResponse = AskQuestionResponse;

            return connector.Send(req);
        }

        private class AskQuestionReq : RESTConnector.Request
        {
            public OnAskQuestion Callback { get; set; }
        };

        private void AskQuestionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Questions questions = new Questions();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = questions;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error(XRAY_SUBSYSTEM, "GetAnswers Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((AskQuestionReq)req).Callback != null)
                ((AskQuestionReq)req).Callback(resp.Success ? questions : null);
        }
        #endregion
    }

}

