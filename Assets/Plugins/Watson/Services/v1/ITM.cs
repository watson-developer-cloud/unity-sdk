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


using IBM.Watson.Connection;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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
            public string Id { get; set; }
            public string Rev { get; set; }
            public string ClientId { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Label { get; set; }
            public string URL { get; set; }
            public string CAS { get; set; }
            public string AnswerKey { get; set; }
            public string Model { get; set; }

            public bool ParseJson( IDictionary json )
            {
                try {
                    Id = (string)json["_id"];
                    Rev = (string)json["_rev"];
                    ClientId = (string)json["clientId"];
                    Name = (string)json["pipelineName"];
                    Type = (string)json["pipelineType"];
                    Label = (string)json["pipelineLabel"];
                    URL = (string)json["pipelineUrl"];
                    CAS = (string)json["pipelineCas"];
                    AnswerKey = (string)json["pipelineAnswerKey"];
                    Model = (string)json["pipelineModel"];
                    return true;
                }
                catch( Exception e )
                {
                    Log.Error( "ITM", "Exception parsing Pipeline JSON {0}", e.ToString() );
                }

                return false;
            }
        };
        public delegate void OnGetPipeline( Pipeline pipeline );
        public delegate void OnGetPipelines( Pipeline [] pipes );

        public class Question
        {
            public string Id { get; set; }
            public string Rev { get; set; }
            public double TopConfidence { get; set; }
            public string CreateDate { get; set; }
            public string CreateTime { get; set; }
            public string TransactionHash { get; set; }
            public long TransactionId { get; set; }
            public string PipelineId { get; set; }
            public string AuthorizationKey { get; set; }
            public string [] Focus { get; set; }
            public string [] Lat { get; set; }
            public string QuestionText { get; set; }
            public string TaggedText { get; set; }

            public bool ParseJson( IDictionary json )
            {
                try {
                    Id = (string)json["_id"];
                    Rev = (string)json["_rev"];
                    TopConfidence = (double)json["topConfidence"];
                    CreateDate = (string)json["createDate"];
                    CreateTime = (string)json["createTime"];
                    TransactionHash = (string)json["transactionHash"];
                    TransactionId = (long)json["transactionId"];
                    PipelineId = (string)json["pipelineId"];
                    AuthorizationKey = (string)json["authorizationKey"];

                    IDictionary iQuestion = (IDictionary)json["question"];

                    List<string> focus = new List<string>();
                    IList iFocus = (IList)iQuestion["focus"];
                    for(int i=0;i<iFocus.Count;++i)
                        focus.Add( (string)iFocus[i] );
                    Focus = focus.ToArray();

                    List<string> lat = new List<string>();
                    IList iLat = (IList)iQuestion["lat"];
                    for(int i=0;i<iLat.Count;++i)
                        lat.Add( (string)iLat[i] );
                    Lat = lat.ToArray();

                    QuestionText = (string)iQuestion["questionText"];
                    TaggedText = (string)iQuestion["taggedText"];
                    return true;
                }
                catch( Exception e )
                {
                    Log.Error( "ITM", "Exception parsing Question JSON {0}", e.ToString() );
                }

                return false;
            }
        };
        public delegate void OnGetQuestions( Question [] questions );

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
            public string [] Features { get; set; }

            public string PosName {
                set {
                    WordPosition pos = WordPosition.INVALID;
                    if (! sm_WordPositions.TryGetValue( value, out pos ) )
                        Log.Error( "ITM", "Failed to find position type for {0}, Word: {1}", value, Word );
                    Pos = pos;
                }
            }
        };
        public class ParseData
        {
            public string Id { get; set; }
            public string Rev { get; set; }
            public long TransactionId { get; set; }
            public ParseWord [] Words { get; set; }
            public string [] Heirarchy { get; set; }
            public string [] Flags { get; set; }

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
        public delegate void OnGetParseData( ParseData data );
        #endregion

        #region Public Properties
        public Pipeline SelectedPipeline { get { return m_SelectedPipeline; } set { m_SelectedPipeline = value; } }
        #endregion

        #region Private Data
        private Pipeline m_SelectedPipeline = null;
        private static Dictionary<string,WordPosition> sm_WordPositions = new Dictionary<string, WordPosition>()
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

        #region GetPipeline
        /// <summary>
        /// Gets/Selects a pipeline by name.
        /// </summary>
        /// <param name="name">The name of the pipeline to get.</param>
        /// <param name="select">If true, then pipeline will be set as the active pipeline.</param>
        /// <param name="callback">Optional callback to invoke.</param>
        /// <returns></returns>
        public bool GetPipeline( string name, bool select, OnGetPipeline callback = null )
        {
            if ( string.IsNullOrEmpty( name ) )
                throw new ArgumentNullException( "pipelineName" );

            new GetPipelineReq( name, select, callback, this );
            return true;
        }

        private class GetPipelineReq
        {
            private string m_Name = null;
            private bool m_Select = false;
            private OnGetPipeline m_Callback = null;
            private ITM m_Service = null;

            public GetPipelineReq( string name, bool select, OnGetPipeline callback, ITM service )
            {
                m_Name = name;
                m_Select = select;
                m_Callback = callback;
                m_Service = service;

                m_Service.GetPipelines( OnGetPipelines );
            }

            public void OnGetPipelines( Pipeline [] pipelines )
            {
                bool bFound = false;
                for(int i=0;i<pipelines.Length;++i)
                    if ( pipelines[i].Name == m_Name )
                    {
                        bFound = true;
                        if ( m_Callback != null )
                            m_Callback( pipelines[i] );
                        if ( m_Select )
                            m_Service.SelectedPipeline = pipelines[i];
                    }

                if (! bFound )
                {
                    if ( m_Callback != null )
                        m_Callback( null );
                    if ( m_Select )
                        Log.Error( "ITM", "Failed to select pipeline {0}", m_Name );
                }
            }
        };

        /// <summary>
        /// Get all pipelines from the ITM service. This invokes the callback with an array of all available pipelines.
        /// </summary>
        /// <param name="callback">The callback to invoke.</param>
        /// <returns>Returns true if request was sent, if a failure occurs the callback will be invoked with null.</returns>
        public bool GetPipelines( OnGetPipelines callback )
        {
            if ( callback == null )
                throw new ArgumentNullException( "callback" );

            RESTConnector connector = RESTConnector.GetConnector( SERVICE_ID, "/ITM/en/user/ibm" );
            if ( connector == null )
                return false;

            GetPipelinesReq req = new GetPipelinesReq();
            req.Callback = callback;
            req.OnResponse = OnGetPipelinesResponse;

            return connector.Send( req );
        }

        private class GetPipelinesReq : RESTConnector.Request
        {
            public OnGetPipelines Callback { get; set; }
        };

        private void OnGetPipelinesResponse( RESTConnector.Request req, RESTConnector.Response resp )
        {
            List<Pipeline> pipelines = new List<Pipeline>();
            if ( resp.Success )
            {
                try {
                    IDictionary json = (IDictionary)Json.Deserialize( Encoding.UTF8.GetString( resp.Data ) );

                    IList iPipelines = (IList)json["pipelines"];
                    for(int i=0;i<iPipelines.Count;++i)
                    {
                        Pipeline pipeline = new Pipeline();
                        if ( pipeline.ParseJson( (IDictionary)iPipelines[i] ) )
                            pipelines.Add( pipeline );
                    }

                }
                catch( Exception e )
                {
                    Log.Error( "ITM", "GetPipelines Exception: {0}", e.ToString() );
                    resp.Success = false;
                }
            }

            if ( ((GetPipelinesReq)req).Callback != null )
                ((GetPipelinesReq)req).Callback( resp.Success ? pipelines.ToArray() : null );
        }
        #endregion

        #region GetQuestions
        public bool GetQuestions( OnGetQuestions callback, int limit = 10, int skip = 0 )
        {
            if ( callback == null )
                throw new ArgumentNullException( "callback" );
            if ( SelectedPipeline == null )
                throw new WatsonException( "You must select a pipeline before calling GetQuestions()" );

            RESTConnector connector = RESTConnector.GetConnector( SERVICE_ID, "/ITM/en/stream" );
            if ( connector == null )
                return false;

            GetQuestionsReq req = new GetQuestionsReq();
            req.Callback = callback;
            req.OnResponse = OnGetQuestionsResponse;
            req.Parameters["limit"] = limit.ToString();
            req.Parameters["skip"] = skip.ToString();
            req.Parameters["user"] = SelectedPipeline.ClientId;

            return connector.Send( req );
        }

        private class GetQuestionsReq : RESTConnector.Request
        {
            public OnGetQuestions Callback { get; set; }
        };

        private void OnGetQuestionsResponse( RESTConnector.Request req, RESTConnector.Response resp )
        {
            List<Question> questions = new List<Question>();

            if ( resp.Success )
            {
                try {
                    IDictionary json = (IDictionary)Json.Deserialize( Encoding.UTF8.GetString( resp.Data ) );

                    IList iQuestions = (IList)json["questions"];
                    for(int i=0;i<iQuestions.Count;++i)
                    {
                        Question question = new Question();
                        if ( question.ParseJson( (IDictionary)iQuestions[i] ) )
                            questions.Add( question );
                    }

                }
                catch( Exception e )
                {
                    Log.Error( "ITM", "GetQuestions Exception: {0}", e.ToString() );
                    resp.Success = false;
                }
            }

            if ( ((GetQuestionsReq)req).Callback != null )
                ((GetQuestionsReq)req).Callback( resp.Success ? questions.ToArray() : null );
        }

        #endregion

        #region GetParseData
        /// <summary>
        /// This returns the parse data for specific transaction ID.
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool GetParseData( long transactionId, OnGetParseData callback )
        {
            if ( transactionId == 0 )
                throw new ArgumentNullException( "transactionId" );
            if (callback == null )
                throw new ArgumentNullException("callback");
            if ( SelectedPipeline == null )
                throw new WatsonException( "You must select a pipeline before calling GetParseData()" );

            RESTConnector connector = RESTConnector.GetConnector( SERVICE_ID, "/ITM/en/parse" );
            if ( connector == null )
                return false;

            GetParseDataReq req = new GetParseDataReq();
            req.Callback = callback;
            req.Function = "/" + transactionId.ToString() + "/" + SelectedPipeline.ClientId;
            req.OnResponse = GetParseDataResponse;

            return connector.Send( req );
        }

        private class GetParseDataReq : RESTConnector.Request
        {
            public OnGetParseData Callback { get; set; }
        }

        private void GetParseDataResponse( RESTConnector.Request req, RESTConnector.Response resp )
        {
            ParseData parse = new ParseData();
            try {
                if (! parse.ParseJson( (IDictionary)Json.Deserialize( Encoding.UTF8.GetString( resp.Data ) ) ) )
                    resp.Success = false;
            }
            catch( Exception e )
            {
                Log.Error( "ITM", "Exception during parse: {0}", e.ToString() );
                resp.Success = false;
            }

            if ( ((GetParseDataReq)req).Callback != null )
                ((GetParseDataReq)req).Callback( resp.Success ? parse : null );
        }

        private ParseData CreateParseData( string jsonResponse )
        {
            return null;
        }
        #endregion
    }

}

