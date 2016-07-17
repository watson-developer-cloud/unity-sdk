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


using FullSerializer;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IBM.Watson.DeveloperCloud.Services.DeepQA.v1
{
    /// <summary>
    /// The DeepQA provides an abstraction for the DeepQA services of Watson.
    /// </summary>
    public class DeepQA : IWatsonService
    {
        #region Public Types
        /// <exclude />
        public delegate void OnQuestion(Question response);
        #endregion

        #region Public Properties
        /// <summary>
        /// True to disable the question cache.
        /// </summary>
        public bool DisableCache { get; set; }
        #endregion

        #region Private Data
        private string m_ServiceId = null;
        private DataCache m_QuestionCache = null;

        private static fsSerializer sm_Serializer = new fsSerializer();
        private const string ASK_QUESTION = "/v1/question";
        #endregion

        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="pipelineId">The ID of the pipeline for caching purposes.</param>
        public DeepQA(string serviceId)
        {
            m_ServiceId = serviceId;
        }

        #region AskQuestion
        /// <summary>
        /// This function flushes all data from the answer cache.
        /// </summary>
        public void FlushAnswerCache()
        {
            if (m_QuestionCache == null)
                m_QuestionCache = new DataCache(m_ServiceId);

            m_QuestionCache.Flush();
        }

        /// <summary>
        /// Find a question in the question cache.
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public Question FindQuestion(string questionId)
        {
            if (m_QuestionCache == null)
                m_QuestionCache = new DataCache(m_ServiceId);

            byte[] cachedQuestion = m_QuestionCache.Find(questionId);
            if (cachedQuestion != null)
            {
                Response response = ProcessAskResp(cachedQuestion);
                if (response != null)
                {
                    response.question.questionId = questionId;
                    return response.question;
                }
            }

            return null;
        }

        /// <summary>
        /// Ask a question using the given pipeline.
        /// </summary>
        /// <param name="question">The text of the question.</param>
        /// <param name="callback">The callback to receive the response.</param>
        /// <returns>Returns true if the request was submitted.</returns>
        public bool AskQuestion(string question, OnQuestion callback, int evidenceItems = 1, bool useCache = true)
        {
            if (string.IsNullOrEmpty(question))
                throw new ArgumentNullException("question");
            if (callback == null)
                throw new ArgumentNullException("callback");

            string questionId = Utility.GetMD5(question);
            if (!DisableCache && useCache)
            {
                Question q = FindQuestion(questionId);
                if (q != null)
                {
                    callback(q);
                    return true;
                }
            }

            RESTConnector connector = RESTConnector.GetConnector(m_ServiceId, ASK_QUESTION);
            if (connector == null)
                return false;

            Dictionary<string, object> questionJson = new Dictionary<string, object>();
            questionJson["question"] = new Dictionary<string, object>() {
                { "questionText", question },
                { "formattedAnswer", "true" },
                { "evidenceRequest", new Dictionary<string,object>() {
                    { "items", evidenceItems },
                    { "profile", "NO" }
                } }
            };
            string json = MiniJSON.Json.Serialize(questionJson);

            AskQuestionReq req = new AskQuestionReq();
            req.QuestionID = questionId;
            req.Callback = callback;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["X-Synctimeout"] = "-1";
            req.Send = Encoding.UTF8.GetBytes(json);
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
                    response = ProcessAskResp(resp.Data);
                    if (m_QuestionCache != null && response != null)
                        m_QuestionCache.Save(((AskQuestionReq)req).QuestionID, resp.Data);
                }
                catch (Exception e)
                {
                    Log.Error("Natural Language Classifier", "GetClassifiers Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((AskQuestionReq)req).Callback != null)
            {
                if (resp.Success && response != null)
                {
                    response.question.questionId = ((AskQuestionReq)req).QuestionID;
                    ((AskQuestionReq)req).Callback(response.question);
                }
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

        #region IWatsonService interface
        /// <exclude />
        public string GetServiceID()
        {
            return m_ServiceId;
        }

        /// <exclude />
        public void GetServiceStatus(ServiceStatus callback)
        {
            if (Config.Instance.FindCredentials(m_ServiceId) != null)
                new CheckServiceStatus(this, callback);
            else
                callback(m_ServiceId, false);
        }

        private class CheckServiceStatus
        {
            private DeepQA m_Service = null;
            private ServiceStatus m_Callback = null;

            public CheckServiceStatus(DeepQA service, ServiceStatus callback)
            {
                m_Service = service;
                m_Callback = callback;

                if (!m_Service.AskQuestion("What is the best treatment for an African American male in heart failure?", OnQuestion, 1, false))
                    m_Callback(m_Service.GetServiceID(), false);
            }

            private void OnQuestion(Question question)
            {
                m_Callback(m_Service.GetServiceID(), question != null);
            }
        };
        #endregion
    }
}
