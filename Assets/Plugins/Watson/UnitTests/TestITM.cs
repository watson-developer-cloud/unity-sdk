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

using System.Collections;
using IBM.Watson.Services.v1;
using IBM.Watson.Logging;

namespace IBM.Watson.UnitTests
{
    public class TestITM : UnitTest
    {
        ITM m_ITM = new ITM();
        bool m_LoginTested = false;
        bool m_GetPipelineTested = false;
        bool m_AskQuestionTested = false;
        bool m_GetQuestionsTested = false;
        bool m_GetQuestionTested = false;
        bool m_GetAnswersTested = false;
        bool m_ParseTested = false;

        public override IEnumerator RunTest()
        {
            m_ITM.Login( OnLogin );
            while(! m_LoginTested )
                yield return null;

            m_ITM.GetPipeline( "thunderstone", true, OnGetPipeline );
            while(! m_GetPipelineTested )
                yield return null;

            m_ITM.GetQuestions(OnGetQuestions);
            while (!m_GetQuestionsTested)
                yield return null;

            m_ITM.AskQuestion( "What is the capital of Texas", OnAskQuestion );
            while(! m_AskQuestionTested )
                yield return null;

            while (!m_GetQuestionTested)
                yield return null;
            while (!m_GetAnswersTested)
                yield return null;
            while (!m_ParseTested)
                yield return null;

            yield break;
        }

        private void OnLogin( bool success )
        {
            Test( success );
            m_LoginTested = true;
        }

        private void OnGetPipeline( ITM.Pipeline pipeline )
        {
            Test( pipeline != null );
            m_GetPipelineTested = true;
        }

        private void OnAskQuestion( ITM.Questions questions )
        {
            Test( questions != null );
            if ( questions != null  )
            {
                foreach( var question in questions.questions )
                {
                    Log.Status( "TestITM", "OnAskQuestion: {0} ({1})", question.question.questionText, question.topConfidence );
                    m_ITM.GetAnswers( question.transactionId, OnGetAnswers );
                    m_ITM.GetParseData( question.transactionId, OnGetParseData );
                    m_ITM.GetQuestion( question.transactionId, OnGetQuestion );
                }
            }
            else
            {
                // don't hang the unit test
                m_GetAnswersTested = m_ParseTested = true;
            }

            m_AskQuestionTested = true;
        }

        private void OnGetQuestion( ITM.Questions questions )
        {
            Test( questions != null );
            if ( questions != null && questions.questions != null && questions.questions.Length > 0 )
                Log.Status( "TestITM", "OnGetQuestion: {0}",  questions.questions[0].question.questionText );
            m_GetQuestionTested = true;
        }

        private void OnGetQuestions( ITM.Questions questions )
        {
            Test( questions != null && questions.questions != null );
            if ( questions != null && questions.questions != null )
            {
                for(int i=0;i<questions.questions.Length;++i)
                    Log.Status( "TestITM", "OnGetQuestions: {0}", questions.questions[i].question.questionText );
            }
            m_GetQuestionsTested = true;
        }

        private void OnGetAnswers( ITM.Answers answers )
        {
            Test( answers != null );
            if ( answers != null )
            {
                for(int i=0;i<answers.answers.Length;++i)
                    Log.Status( "TestITM", "OnGetAnswers: {0}, Confidence: {1}",
                        answers.answers[i].answerText, answers.answers[i].confidence.ToString() );
            }
            m_GetAnswersTested = true;
        }

        private void OnGetParseData( ITM.ParseData parse)
        {
            Test ( parse != null );
            m_ParseTested = true;
        }
    }
}

