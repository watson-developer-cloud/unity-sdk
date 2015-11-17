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
using IBM.Watson.Data.XRAY;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using System;
using System.Collections.Generic;

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
        public delegate void OnAskQuestion(ParseData parse, Questions questions);
        #endregion

        #region Public Properties
        /// <summary>
        /// The users current location, this is set when Login() is invoked.
        /// </summary>
        public string Location { get { return m_Location; } set { m_Location = value; } }
        #endregion

        #region Private Data
        private string m_Location = "Austin, TX";
        private ITM m_ITM = new ITM();
        private Dictionary<string,DeepQA> m_Pipelines = new Dictionary<string, DeepQA>();
        private static fsSerializer sm_Serializer = new fsSerializer();
        private const string SERVICE_ID = "XrayV1";
        private const string XRAY_SUBSYSTEM = "XRAY";
        #endregion

        private DeepQA GetPipeline( string pipeline )
        {
            DeepQA dqa = null;
            if ( m_Pipelines.TryGetValue( pipeline, out dqa ) )
                return dqa;

            if ( Config.Instance.FindCredentials( pipeline ) == null )
            {
                Log.Error( "XRAY", "Credentials not found for pipeline {0}", pipeline );
                return null;
            }

            dqa = new DeepQA( pipeline );
            m_Pipelines[ pipeline ] = dqa;
            return dqa;
        }

        #region GetQuestion
        /// <summary>
        /// This returns a single question by transaction ID.
        /// </summary>
        /// <param name="pipeline">The pipeline name.</param>
        /// <param name="questionId">The Question ID.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>Returns true if the request was submitted correctly.</returns>
        public Questions GetQuestion( string pipeline, string questionId )
        {
            if ( string.IsNullOrEmpty( pipeline ) )
                throw new ArgumentNullException("pipeline");
            if ( string.IsNullOrEmpty( questionId ) )
                throw new ArgumentNullException( "questionId" );

            DeepQA pipe = GetPipeline( pipeline );
            if ( pipe == null )
                return null;

            // just grab the question from the local data cache..
            Data.QA.Question data = pipe.FindQuestion( questionId );
            if ( data == null )
                return null;

            return new Questions( data );
        }
        #endregion

        #region GetAnswers
        /// <summary>
        /// Returns answers for the given transactionId. 
        /// </summary>
        /// <param name="pipeline">The pipeline name.</param>
        /// <param name="questionId">The Question ID.</param>
        /// <param name="callback">The callback to invoke with the results.</param>
        /// <returns>Returns false if unable to submit the result. If true is returned, the
        /// the callback will always be invoked on failure or success.</returns>
        public Answers GetAnswers(string pipeline, string questionId)
        {
            if ( string.IsNullOrEmpty( pipeline ) )
                throw new ArgumentNullException("pipeline");
            if ( string.IsNullOrEmpty( questionId ) )
                throw new ArgumentNullException( "questionId" );

            DeepQA pipe = GetPipeline( pipeline );
            if ( pipe == null )
                return null;

            Data.QA.Question data = pipe.FindQuestion( questionId );
            if ( data == null )
                return null;

            return new Answers( data );
        }
        #endregion

        #region AskQuestion
        /// <summary>
        /// Ask a question.
        /// </summary>
        /// <param name="pipeline">The pipeline name.</param>
        /// <param name="question">The text of the question to ask.</param>
        /// <param name="callback">The callback to received the Questions object.</param>
        /// <returns>Returns true if the request was submitted.</returns>
        public bool AskQuestion( string pipeline, string question, OnAskQuestion callback, int evidenceItems = 1)
        {
            if ( string.IsNullOrEmpty(pipeline) )
                throw new ArgumentNullException("pipeline");
            DeepQA pipe = GetPipeline( pipeline );
            if ( pipe == null )
                return false;

            var request = new AskQuestionReq( callback );
            if (! m_ITM.ParseQuestion( question, request.OnParseQuestion ) )
                return false;
            if (! pipe.AskQuestion( question, request.OnAskQuestion, evidenceItems ) )
                return false;

            return true;
        }

        private class AskQuestionReq
        {
            private OnAskQuestion m_Callback = null;
            private ParseData m_Parse = null;
            private Data.QA.Question m_Result = null;
            private bool m_Failure = false;

            public AskQuestionReq( OnAskQuestion callback )
            {
                m_Callback = callback;
            }

            public void OnAskQuestion( Data.QA.Question q )
            {
                if ( q != null )
                {
                    m_Result = q;
                    if ( m_Parse != null && m_Callback != null )
                        m_Callback( m_Parse, new Questions( m_Result ) );
                }
                else if (! m_Failure )
                {
                    m_Failure = true;
                    if ( m_Callback != null )
                        m_Callback( m_Parse, null );
                }
            }

            public void OnParseQuestion(ParseData p )
            {
                if ( p != null )
                { 
                    m_Parse = p;
                    if ( m_Result != null && m_Callback != null )
                        m_Callback( m_Parse, new Questions( m_Result ) );
                } 
                else if (! m_Failure )
                {
                    m_Failure = true;
                    if ( m_Callback != null )
                        m_Callback( null, null );
                }
            }
        };

        #endregion
    }

}

