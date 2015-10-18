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
        bool m_GetPipelineTested = false;
        bool m_GetPipelinesTested = false;
        bool m_GetQuestionsTested = false;
        bool m_GetAnswersTested = false;
        bool m_ParseTested = false;

        public override IEnumerator RunTest()
        {
            m_ITM.GetPipeline( "thunderstone", true, OnGetPipeline );
            while(! m_GetPipelineTested )
                yield return null;

            m_ITM.GetPipelines( OnGetPipelines );
            while(! m_GetPipelinesTested )
                yield return null;

            m_ITM.GetQuestions( OnGetQuestions );
            while(! m_GetQuestionsTested )
                yield return null;

            m_ITM.GetAnswers( -1773927182, OnGetAnswers );
            while(! m_GetAnswersTested )
                yield return null;

            m_ITM.GetParseData( -1773927182, OnGetParseData );
            while(! m_ParseTested )
                yield return null;

            yield break;
        }

        private void OnGetPipeline( ITM.Pipeline pipeline )
        {
            Test( pipeline != null );
            m_GetPipelineTested = true;
        }

        private void OnGetPipelines( ITM.Pipelines pipes )
        {
            Test( pipes != null );
            if ( pipes != null )
            {
                for(int i=0;i<pipes.pipelines.Length;++i)
                    Log.Status( "TestITM", "Pipeline: {0}", pipes.pipelines[i].pipelineName );
            }
            m_GetPipelinesTested = true;
        }

        private void OnGetQuestions( ITM.Questions questions )
        {
            Test( questions != null );
            if ( questions != null )
            {
                for(int i=0;i<questions.questions.Length;++i)
                    Log.Status( "TestITM", "Question: {0}", questions.questions[i].question.questionText );
            }
            m_GetQuestionsTested = true;
        }

        private void OnGetAnswers( ITM.Answers answers )
        {
            Test( answers != null );
            if ( answers != null )
            {
                for(int i=0;i<answers.answers.Length;++i)
                    Log.Status( "TestITM", "Answer: {0}", answers.answers[i].answerText );
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

