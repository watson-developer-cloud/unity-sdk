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
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IBM.Watson.Services.v1
{
    /// <summary>
    /// This class wraps the ITM back-end service.
    /// </summary>
    public class ITM
    {
        #region Public Types
        public class Pipeline
        {
            public string _id { get; set; }
            public string _rev { get; set; }
            public string clientId { get; set; }
            public string pipelineName { get; set; }
            public string pipelineType { get; set; }
            public string pipelineLabel { get; set; }
            public string pipelineUrl { get; set; }
            public string pipelineCas { get; set; }
            public string pipelineAnswerKey { get; set; }
            public string pipelineModel { get; set; }
        };
        public class Pipelines
        {
            public Pipeline[] pipelines { get; set; }
        };
        public delegate void OnGetPipeline(Pipeline pipeline);
        public delegate void OnGetPipelines(Pipelines pipes);

        public class QuestionText
        {
            public string[] focus { get; set; }
            public string[] lat { get; set; }
            public string questionText { get; set; }
            public string taggedText { get; set; }
        };
        public class Question
        {
            public string _id { get; set; }
            public string _rev { get; set; }
            public double topConfidence { get; set; }
            public string createDate { get; set; }
            public string createTime { get; set; }
            public string transactionHash { get; set; }
            public long transactionId { get; set; }
            public string pipelineId { get; set; }
            public string authorizationKey { get; set; }
            public QuestionText question { get; set; }
        };
        public class Questions
        {
            public Question[] questions { get; set; }
        };
        public delegate void OnGetQuestions(Questions questions);

        public enum WordPosition
        {
            INVALID = -1,
            NOUN,
            PRONOUN,
            ADJECTIVE,
            DETERMINIER,
            VERB,
            ADVERB,
            PREPOSITION,
            CONJUNCTION,
            INTERJECTION
        };
        public class ParseWord
        {
            public string Word { get; set; }
            public WordPosition Pos { get; set; }
            public string Slot { get; set; }
            public string[] Features { get; set; }

            public string PosName
            {
                set
                {
                    WordPosition pos = WordPosition.INVALID;
                    if (!sm_WordPositions.TryGetValue(value, out pos))
                        Log.Error("ITM", "Failed to find position type for {0}, Word: {1}", value, Word);
                    Pos = pos;
                }
            }
        };
        public class ParseData
        {
            public string Id { get; set; }
            public string Rev { get; set; }
            public long TransactionId { get; set; }
            public ParseWord[] Words { get; set; }
            public string[] Heirarchy { get; set; }
            public string[] Flags { get; set; }

            public bool ParseJson(IDictionary json)
            {
                try
                {
                    Id = (string)json["_id"];
                    Rev = (string)json["_rev"];
                    TransactionId = (long)json["transactionId"];

                    IDictionary iparse = (IDictionary)json["parse"];
                    List<string> heirarchy = new List<string>();
                    IList iheirarchy = (IList)iparse["hierarchy"];
                    foreach (var h in iheirarchy)
                        heirarchy.Add((string)h);
                    Heirarchy = heirarchy.ToArray();

                    List<string> flags = new List<string>();
                    IList iflags = (IList)iparse["flags"];
                    foreach (var f in iflags)
                        flags.Add((string)f);
                    Flags = flags.ToArray();

                    List<ParseWord> words = new List<ParseWord>();

                    IList iWords = (IList)iparse["words"];
                    for (int i = 0; i < iWords.Count; ++i)
                    {
                        ParseWord word = new ParseWord();
                        word.Word = (string)iWords[i];

                        IList iPos = (IList)iparse["pos"];
                        if (iPos.Count != iWords.Count)
                            throw new WatsonException("ipos.Count != iwords.Count");
                        word.PosName = (string)((IDictionary)iPos[i])["value"];

                        IList iSlots = (IList)iparse["slot"];
                        if (iSlots.Count != iWords.Count)
                            throw new WatsonException("islots.Count != iwords.Count");
                        word.Slot = (string)((IDictionary)iSlots[i])["value"];

                        IList iFeatures = (IList)iparse["features"];
                        if (iFeatures.Count != iWords.Count)
                            throw new WatsonException("ifeatures.Count != iwords.Count");

                        List<string> features = new List<string>();
                        IList iWordFeatures = (IList)((IDictionary)iFeatures[i])["value"];
                        foreach (var k in iWordFeatures)
                            features.Add((string)k);
                        word.Features = features.ToArray();

                        words.Add(word);
                    }

                    Words = words.ToArray();
                    return true;
                }
                catch (Exception e)
                {
                    Log.Error("ITM", "Exception during parse: {0}", e.ToString());
                }

                return false;
            }
        };
        public delegate void OnGetParseData(ParseData data);

        public class Evidence
        {
            public string title { get; set; }
            public string passage { get; set; }
            public string decoratedPassage { get; set; }
            public string corpus { get; set; }
        };
        public class Variant
        {
            public string text { get; set; }
            public string relationship { get; set; }
        };
        public class Feature
        {
            public string featureId { get; set; }
            public string label { get; set; }
            public string displayLabel { get; set; }
            public double unweightedScore { get; set; }
            public double weightedScore { get; set; }
        };
        public class Answer
        {
            public string answerText { get; set; }
            public double confidence { get; set; }
            public bool correctAnswer { get; set; }
            public Evidence[] evidence { get; set; }
            public Variant[] variants { get; set; }
            public Feature[] features { get; set; }
        };
        public class Answers
        {
            public string _id { get; set; }
            public string _rev { get; set; }
            public long transactionId { get; set; }
            public double featureScoreMin { get; set; }
            public double featureScoreMax { get; set; }
            public double featureScoreRange { get; set; }
            public Answer[] answers { get; set; }
        };
        public delegate void OnGetAnswers(Answers answers);
        #endregion

        #region Public Properties
        public Pipeline SelectedPipeline { get { return m_SelectedPipeline; } set { m_SelectedPipeline = value; } }
        public long SessionKey { get; set; }
        #endregion

        #region Private Data
        private static fsSerializer sm_Serializer = new fsSerializer();
        private Pipeline m_SelectedPipeline = null;
        private static Dictionary<string, WordPosition> sm_WordPositions = new Dictionary<string, WordPosition>()
        {
            { "noun", WordPosition.NOUN },
            { "pronoun", WordPosition.PRONOUN },        // ?
            { "adj", WordPosition.ADJECTIVE },
            { "det", WordPosition.DETERMINIER },
            { "verb", WordPosition.VERB },
            { "adverb", WordPosition.ADVERB },          // ?
            { "prep", WordPosition.PREPOSITION },
            { "conj", WordPosition.CONJUNCTION },       // ?
            { "inter", WordPosition.INTERJECTION },     // ?
        };
        private const string SERVICE_ID = "ITMV1";
        private const string TEST_PARSE_DATA = "{\"_id\":\"47f26baa682b4c939cda4164f5b9059b\",\"_rev\":\"1-c353e16c83c2a903f367c30042a4eba1\",\"transactionId\":-1773927182,\"parse\":{\"pos\":[{\"text\":\"What\",\"value\":\"noun\"},{\"text\":\"is\",\"value\":\"verb\"},{\"text\":\"the\",\"value\":\"det\"},{\"text\":\"best\",\"value\":\"adj\"},{\"text\":\"treatment\",\"value\":\"noun\"},{\"text\":\"for\",\"value\":\"prep\"},{\"text\":\"an\",\"value\":\"det\"},{\"text\":\"african american\",\"value\":\"noun\"},{\"text\":\"male\",\"value\":\"noun\"},{\"text\":\"with\",\"value\":\"prep\"},{\"text\":\"heart\",\"value\":\"noun\"},{\"text\":\"failure\",\"value\":\"noun\"}],\"slot\":[{\"text\":\"What\",\"value\":\"subj\"},{\"text\":\"is\",\"value\":\"top\"},{\"text\":\"the\",\"value\":\"ndet\"},{\"text\":\"best\",\"value\":\"nadj\"},{\"text\":\"treatment\",\"value\":\"pred\"},{\"text\":\"for\",\"value\":\"ncomp\"},{\"text\":\"an\",\"value\":\"ndet\"},{\"text\":\"african american\",\"value\":\"nadj\"},{\"text\":\"male\",\"value\":\"objprep\"},{\"text\":\"with\",\"value\":\"nprep\"},{\"text\":\"heart\",\"value\":\"nnoun\"},{\"text\":\"failure\",\"value\":\"objprep\"}],\"features\":[{\"text\":\"What\",\"value\":[\"pron\",\"sg\",\"wh\",\"whnom\"]},{\"text\":\"is\",\"value\":[\"vfin\",\"vpres\",\"sg\",\"wh\",\"whnom\",\"vsubj\",\"absubj\",\"auxv\"]},{\"text\":\"the\",\"value\":[\"sg\",\"def\",\"the\",\"ingdet\"]},{\"text\":\"best\",\"value\":[\"superl\",\"adjnoun\"]},{\"text\":\"treatment\",\"value\":[\"cn\",\"sg\",\"evnt\",\"act\",\"abst\",\"cognsa\",\"activity\",\"groupact\",\"(latrwd 0.051600)\",\"(vform treat)\"]},{\"text\":\"for\",\"value\":[\"pprefv\",\"nonlocp\",\"pobjp\"]},{\"text\":\"an\",\"value\":[\"sg\",\"indef\"]},{\"text\":\"african american\",\"value\":[\"propn\",\"sg\",\"glom\",\"notfnd\",\"unkph\"]},{\"text\":\"male\",\"value\":[\"cn\",\"sg\",\"m\",\"h\",\"physobj\",\"anim\",\"anml\",\"liv\",\"(latrwd 0.051600)\"]},{\"text\":\"with\",\"value\":[\"pprefv\",\"nonlocp\"]},{\"text\":\"heart\",\"value\":[\"cn\",\"sg\",\"abst\",\"cognsa\",\"(latrwd 0.032630)\"]},{\"text\":\"failure\",\"value\":[\"cn\",\"sg\",\"abst\",\"massn\",\"illness\",\"cond\",\"state\",\"(* heart failure)\",\"(vform fail)\"]}],\"hierarchy\":[\"african american\"],\"words\":[\"What\",\"is\",\"the\",\"best\",\"treatment\",\"for\",\"an\",\"african american\",\"male\",\"with\",\"heart\",\"failure\"],\"flags\":[\"african american\"]},\"preContext\":0,\"postContext\":0,\"sessionKey\":null}";
        #endregion

        #region Login
        public delegate void OnLogin(bool success);

        public bool Login(OnLogin callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/ITM/en/user/");
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
                string jsonString = Encoding.UTF8.GetString(resp.Data);

                IDictionary json = Json.Deserialize(jsonString) as IDictionary;
                if (json != null && json.Contains("sessionKey"))
                    SessionKey = (long)json["sessionKey"];
                else
                    resp.Success = false;
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
            private ITM m_Service = null;

            public GetPipelineReq(string name, bool select, OnGetPipeline callback, ITM service)
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
                        Log.Error("ITM", "Failed to select pipeline {0}", m_Name);
                }
            }
        };

        /// <summary>
        /// Get all pipelines from the ITM service. This invokes the callback with an array of all available pipelines.
        /// </summary>
        /// <param name="callback">The callback to invoke.</param>
        /// <returns>Returns true if request was sent, if a failure occurs the callback will be invoked with null.</returns>
        public bool GetPipelines(OnGetPipelines callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/ITM/en/user/ibm");
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
                    Log.Error("ITM", "GetPipelines Exception: {0}", e.ToString());
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

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/ITM/en/stream");
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
                    Log.Error("ITM", "GetQuestions Exception: {0}", e.ToString());
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

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/ITM/en/transaction");
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

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/ITM/en/answers");
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
                    Log.Error("ITM", "GetAnswers Exception: {0}", e.ToString());
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

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/ITM/en/parse");
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
                Log.Error("ITM", "Exception during parse: {0}", e.ToString());
                resp.Success = false;
            }

            if (((GetParseDataReq)req).Callback != null)
                ((GetParseDataReq)req).Callback(resp.Success ? parse : null);
        }

        #endregion

        #region AskQuestion
        public delegate void OnAskQuestion(Questions questions);

        public bool AskQuestion(string question, OnAskQuestion callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (SelectedPipeline == null)
                throw new WatsonException("You must select a pipeline before calling AskQuestion()");


            question = WWW.EscapeURL(question);
            question = question.Replace("+", "%20");
            question = question.Replace("%0a", "");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/ITM/en/ask/" + SelectedPipeline.clientId + "/" + SelectedPipeline.pipelineName);
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
                    Log.Error("ITM", "GetAnswers Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((AskQuestionReq)req).Callback != null)
                ((AskQuestionReq)req).Callback(resp.Success ? questions : null);
        }
        #endregion
    }

}

