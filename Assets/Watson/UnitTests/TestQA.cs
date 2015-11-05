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
    /// <exclude />
    public class TestQA : UnitTest
    {
        QA m_QA = new QA();
        bool m_GetServicesTested = false;
        bool m_AskQuestionTested = false;

        /// <exclude />
        public override IEnumerator RunTest()
        {
            Test( m_QA.GetServices( OnGetServices ) );
            while(! m_GetServicesTested )
                yield return null;

            Test( m_QA.AskQuestion( "travel", "What is the capital of Texas", OnAskQuestion ) );
            while(! m_AskQuestionTested )
                yield return null;

            yield break;
        }

        private void OnGetServices( Data.QA.Services services )
        {
            Test( services != null );
            if ( services != null )
            {
                foreach( var service in services.services )
                    Log.Status( "TestQA", "Service Name: {0}, Desc: {1}, ID: {2}", service.name, service.description, service.id );
            }
            m_GetServicesTested = true;         
        }

        private void OnAskQuestion( Data.QA.Response response )
        {
            Test( response != null );
            if ( response != null )
            {
                Log.Status( "TestQA", "Question: {0}", response.question.questionText );
                foreach( var answer in response.question.answers )
                {
                    Log.Status( "TestQA", "Answer: {0}", answer.text );
                }
            }
            m_AskQuestionTested = true;
        }
    }
}

