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

#define TEST_WOODSIDE
//#define CACHE_QUESTIONS
//#define EXPORT_QUESTIONS

using System.Collections;
using IBM.Watson.Services.v1;
using IBM.Watson.Logging;
using IBM.Watson.DataModels.XRAY;

#if EXPORT_QUESTIONS || CACHE_QUESTIONS
using UnityEngine;
using System.Xml;
using System.Text;
using System.IO;
#endif

namespace IBM.Watson.UnitTests
{
    public class TestXRAY : UnitTest
    {
#if TEST_WOODSIDE
        const string TEST_QUESTION = "When were Angel high rate trials conducted?";
        const string TEST_PIPELINE = "woodside";
#else
        const string TEST_QUESTION = "What is the capital of Texas";
        const string TEST_PIPELINE = "thunderstone";
#endif

        XRAY m_XRAY = new XRAY();
        bool m_AskQuestionTested = false;
        bool m_GetQuestionTested = false;
        bool m_GetAnswersTested = false;
        bool m_ParseTested = false;

        public override IEnumerator RunTest()
        {
            m_XRAY.DisableCache = true;
            m_XRAY.AskQuestion( TEST_PIPELINE, TEST_QUESTION, OnAskQuestion );
            while(! m_AskQuestionTested )
                yield return null;

            while (!m_GetQuestionTested)
                yield return null;
            while (!m_GetAnswersTested)
                yield return null;
            while (!m_ParseTested)
                yield return null;

#if EXPORT_QUESTIONS || CACHE_QUESTIONS
            byte [] question_data = File.ReadAllBytes( Application.dataPath + "/../Docs/WoodsideQuestions.xml" );
            var xml = new XmlDocument();
            xml.LoadXml( Encoding.UTF8.GetString( question_data ) );

#if CACHE_QUESTIONS
            m_XRAY.DisableCache = false;
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
                Test( m_XRAY.AskQuestion( TEST_PIPELINE, text, OnAskQuestion ) );
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

        private void OnAskQuestion( AskResponse response )
        {
            Test( response != null );
            if ( response != null  )
            {
                OnGetQuestion( response.questions );
                OnGetAnswers( response.answers );
                OnGetParseData( response.parseData );
            }
            else
            {
                // don't hang the unit test
                m_GetQuestionTested = m_GetAnswersTested = m_ParseTested = true;
            }

            m_AskQuestionTested = true;
        }

        private void OnGetQuestion( Questions questions )
        {
            Test( questions != null );
            if ( questions != null && questions.questions != null && questions.questions.Length > 0 )
                Log.Status( "TestXRAY", "OnGetQuestion: {0}",  questions.questions[0].question.questionText );
            m_GetQuestionTested = true;
        }

        private void OnGetAnswers( Answers answers )
        {
            Test( answers != null );
            if ( answers != null )
            {
                for(int i=0;i<answers.answers.Length;++i)
                    Log.Status( "TestXRAY", "OnGetAnswers: {0}, Confidence: {1}",
                        answers.answers[i].answerText, answers.answers[i].confidence.ToString() );
            }
            m_GetAnswersTested = true;
        }

        private void OnGetParseData( ParseData parse)
        {
            Test ( parse != null );
            m_ParseTested = true;
        }
    }
}

