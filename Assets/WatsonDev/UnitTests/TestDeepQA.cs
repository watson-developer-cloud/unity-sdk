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

//#define TEST_WOODSIDE
//#define TEST_NUMERATI
//#define CACHE_QUESTIONS
//#define EXPORT_QUESTIONS

using System.Collections;
using IBM.Watson.Services.v1;
using IBM.Watson.Logging;
using IBM.Watson.DataModels.QA;

#if EXPORT_QUESTIONS || CACHE_QUESTIONS
using UnityEngine;
using System.Xml;
using System.Text;
using System.IO;
#endif

namespace IBM.Watson.UnitTests
{
    /// <exclude />
    public class TestDeepQA : UnitTest
    {
#if TEST_WOODSIDE
#if TEST_NUMERATI
        const string SERVICE_ID = "numerati_woodside";
        //const string TEST_QUESTION = "What was the NWS Oil Production for Q1 2015?";
        const string TEST_QUESTION = "What was the total well depth of the Steel Dragon well in 2014 quarter four?";
#else
        const string SERVICE_ID = "woodside";
        const string TEST_QUESTION = "Why was a gravel packed lower completion chosen in the Sculptor field?";
#endif
#else
        const string SERVICE_ID = "thunderstone";
        const string TEST_QUESTION = "What is the capitol of Texas?";
#endif
        DeepQA m_QA = new DeepQA(SERVICE_ID);
        bool m_AskQuestionTested = false;

        /// <exclude />
        public override IEnumerator RunTest()
        {
            m_QA.DisableCache = true;
            Test( m_QA.AskQuestion( TEST_QUESTION, OnAskQuestion ) );
            while(! m_AskQuestionTested )
                yield return null;

#if EXPORT_QUESTIONS || CACHE_QUESTIONS
            byte [] question_data = File.ReadAllBytes( Application.dataPath + "/../Docs/WoodsideQuestions.xml" );
            var xml = new XmlDocument();
            xml.LoadXml( Encoding.UTF8.GetString( question_data ) );

#if CACHE_QUESTIONS
            m_QA.DisableCache = false;
#endif

#if EXPORT_QUESTIONS
            StringBuilder WoodsideCSV = new StringBuilder();
#endif

            XmlElement answerKey = xml["answerkey"] as XmlElement;
            foreach( var node in answerKey.ChildNodes )
            {
                XmlElement question = node as XmlElement;
                if (question == null )
                    continue;

                string text = question.GetAttribute("text" );
                Log.Status( "TestDeepQA", "Question: {0}", text );

#if CACHE_QUESTIONS
                Test( m_QA.AskQuestion( text, OnAskQuestion ) );
#endif

#if EXPORT_QUESTIONS
                if ( text.Contains( "," ) )
                    text = "\"" + text + "\"";
                WoodsideCSV.Append( text + ",question-woodside\r\n" );
#endif
            }

#if EXPORT_QUESTIONS
            File.WriteAllText( Application.dataPath + "/../Docs/WoodsideQuestions.csv", WoodsideCSV.ToString() );
#endif
#endif

            yield break;
        }

        private void OnAskQuestion( Question response )
        {
            Test( response != null );
            if ( response != null )
            {
                Log.Status( "TestQA", "Question: {0}", response.questionText );
                foreach( var answer in response.answers )
                {
                    Log.Status( "TestQA", "Answer: {0}", answer.text );
                }

#if TEST_NUMERATI
                Test( response.answers != null );
                Test( response.answers.Length > 0 );

                Data.XRAY.Table [] tables = response.answers[0].ExtractTables( response.answers[0].text );
                Test( tables != null );
#endif
            }
            m_AskQuestionTested = true;
        }

    }
}

