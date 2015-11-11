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

#define EXPORT_QUESTIONS

using System.Collections;
using IBM.Watson.Services.v1;
using IBM.Watson.Logging;
using IBM.Watson.Data.QA;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Text;

namespace IBM.Watson.UnitTests
{
    /// <exclude />
    public class TestDeepQA : UnitTest
    {
        DeepQA m_QA = new DeepQA("Woodside");
        bool m_AskQuestionTested = false;

        /// <exclude />
        public override IEnumerator RunTest()
        {
            Test( m_QA.AskQuestion( "Why was a gravel packed lower completion chosen in the Sculptor field?", OnAskQuestion ) );
            while(! m_AskQuestionTested )
                yield return null;

            byte [] question_data = File.ReadAllBytes( Application.dataPath + "/../Docs/WoodsideQuestions.xml" );
            var xml = new XmlDocument();
            xml.LoadXml( Encoding.UTF8.GetString( question_data ) );

#if EXPORT_QUESTIONS
            StringBuilder WoodsideCSV = new StringBuilder();

            XmlElement answerKey = xml["answerkey"] as XmlElement;
            foreach( var node in answerKey.ChildNodes )
            {
                XmlElement question = node as XmlElement;
                if (question == null )
                    continue;

                string text = question.GetAttribute("text" );
                Log.Status( "TestDeepQA", "Question: {0}", text );

                //Test( m_QA.AskQuestion( text, OnAskQuestion ) );

                if ( text.Contains( "," ) )
                    text = "\"" + text + "\"";
                WoodsideCSV.Append( text + ",question-woodside\r\n" );
            }

            File.WriteAllText( Application.dataPath + "/../Docs/WoodsideQuestions.csv", WoodsideCSV.ToString() );
#endif

            yield break;
        }

        private void OnAskQuestion( Response response )
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

