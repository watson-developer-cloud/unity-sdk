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
* @author Taj Santiago
*/

using UnityEngine;
using UnityEngine.UI;

namespace IBM.Watson.Widgets.Question
{
    public class QuestionAndAnswer : Base
    {
        [SerializeField]
        private Text m_QuestionText;
        [SerializeField]
        private Text m_AnswerText;
        [SerializeField]
        private Text m_ConfidenceText;


        private string m_AnswerString;
        public string AnswerString
        {
			get { return m_AnswerString; }
            set
            {
				m_AnswerString = value;
                UpdateAnswer();
            }
        }

        private string m_QuestionString;
        public string QuestionString
        {
            get { return m_QuestionString; }
            set
            {
                m_QuestionString = value;
                UpdateQuestion();
            }
        }

        private double m_Confidence;
        public double Confidence
        {
            get { return m_Confidence; }
            set
            {
                m_Confidence = value;
                UpdateConfidence();
            }
        }

        override public void Init()
        {
            QuestionString = m_Question.QuestionData.QuestionDataObject.questions[0].question.questionText;
			AnswerString = m_Question.QuestionData.AnswerDataObject.answers[0].answerText;
            Confidence = m_Question.QuestionData.AnswerDataObject.answers[0].confidence;
        }

        /// <summary>
        /// Update answer view.
        /// </summary>
        private void UpdateAnswer()
        {
			m_AnswerText.text = AnswerString;
        }

        /// <summary>
        /// Update question view.
        /// </summary>
        private void UpdateQuestion()
        {
            m_QuestionText.text = QuestionString;
        }

        /// <summary>
        /// Update confidence view.
        /// </summary>
        private void UpdateConfidence()
        {
            float confidence = (float)Confidence * 100;
            m_ConfidenceText.text = confidence.ToString("f1");
        }
    }
}
